using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

using Grpc.Core;
using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Servers.Core;

namespace mxProject.Helpers.Grpc.Servers
{

    /// <summary>
    /// サービスビルダー。
    /// </summary>
    public class GrpcServiceBuilder
    {

        /// <summary>
        /// 指定された型に対するサービス定義を生成します。
        /// </summary>
        /// <param name="serviceType">サービスの型</param>
        /// <param name="serviceInstance">サービスインスタンス</param>
        /// <param name="settings">動作設定</param>
        /// <returns></returns>
        public ServerServiceDefinition BuildService(Type serviceType, object serviceInstance, GrpcServiceBuilderSettings settings)
        {

            settings = settings ?? new GrpcServiceBuilderSettings();

            ServerServiceDefinition.Builder builder = ServerServiceDefinition.CreateBuilder();

            Type implType = serviceInstance.GetType();

            IList<IGrpcServerMethodInvokingInterceptor> classInvokingInterceptors = GetInvokingInterceptors(implType);
            IList<IGrpcServerMethodInvokedInterceptor> classInvokedInterceptors = GetInvokedInterceptors(implType);

            IDictionary<string, IList<MethodInfo>> methodImpls = GetMethodHandlers(implType);

            foreach (string name in methodImpls.Keys)
            {
                foreach (MethodInfo methodImpl in methodImpls[name])
                {

                    MethodType methodType;
                    Type requestType;
                    Type responseType;

                    if (!TryGetServiceMathodInfo(methodImpl, out methodType, out requestType, out responseType)) { continue; }

                    IList<IGrpcServerMethodInvokingInterceptor> methodInvokingInterceptors = GetInvokingInterceptors(methodImpl);
                    IList<IGrpcServerMethodInvokedInterceptor> methodInvokedInterceptors = GetInvokedInterceptors(methodImpl);

                    MethodBuildContext context = new MethodBuildContext(serviceType, serviceInstance, methodType, requestType, responseType, methodImpl, settings
                        , Sort<IGrpcServerMethodInvokingInterceptor>(CompareInterceptor, new IEnumerable<IGrpcServerMethodInvokingInterceptor>[] { settings.InvokingInterceptors, classInvokingInterceptors, methodInvokingInterceptors })
                        , Sort<IGrpcServerMethodInvokedInterceptor>(CompareInterceptor, new IEnumerable<IGrpcServerMethodInvokedInterceptor>[] { settings.InvokedInterceptors, classInvokedInterceptors, methodInvokedInterceptors })
                        , Sort<IGrpcServerMethodExceptionHandler>(CompareInterceptor, new IEnumerable<IGrpcServerMethodExceptionHandler>[] { settings.ExceptionHandlers })
                        );

                    AddServiceMethod(builder, context);

                    break;

                }

            }

            return builder.Build();

        }

        /// <summary>
        /// 
        /// </summary>
        private struct MethodBuildContext
        {

            /// <summary>
            /// 
            /// </summary>
            /// <param name="serviceType">サービスの型</param>
            /// <param name="serviceInstance">サービスインスタンス</param>
            /// <param name="methodType">サービスメソッドの種類</param>
            /// <param name="requestType">リクエストの型</param>
            /// <param name="responseType">レスポンスの型</param>
            /// <param name="methodImpl">メソッド実装</param>
            /// <param name="settings">動作設定</param>
            /// <param name="invokingInterceptors">メソッド呼び出し前の割込処理</param>
            /// <param name="invokedInterceptors">メソッド呼び出し後の割込処理</param>
            /// <param name="exceptionHandlers">例外ハンドラ</param>
            internal MethodBuildContext(Type serviceType, object serviceInstance, MethodType methodType, Type requestType, Type responseType, MethodInfo methodImpl, GrpcServiceBuilderSettings settings
                , IEnumerable<IGrpcServerMethodInvokingInterceptor> invokingInterceptors, IEnumerable<IGrpcServerMethodInvokedInterceptor> invokedInterceptors, IEnumerable<IGrpcServerMethodExceptionHandler> exceptionHandlers)
            {
                m_ServiceType = serviceType;
                m_ServiceInstance = serviceInstance;
                m_MethodImpl = methodImpl;
                m_MethodType = methodType;
                m_RequestType = requestType;
                m_ResponseType = responseType;
                m_Settings = settings;
                m_InvokingInterceptors = invokingInterceptors;
                m_InvokedInterceptors = invokedInterceptors;
                m_ExceptionHandlers = exceptionHandlers;

                m_NeedNotifyPerformanceLog = GetNeedNotifyPerformanceLog(methodImpl);

            }

            /// <summary>
            /// サービスの型を取得します。
            /// </summary>
            internal Type ServiceType
            {
                get { return m_ServiceType; }
            }
            private Type m_ServiceType;

            /// <summary>
            /// サービスインスタンスを取得します。
            /// </summary>
            internal object ServiceInstance
            {
                get { return m_ServiceInstance; }
            }
            private object m_ServiceInstance;

            /// <summary>
            /// メソッド実装を取得します。
            /// </summary>
            internal MethodInfo MethodImpl
            {
                get { return m_MethodImpl; }
            }
            private MethodInfo m_MethodImpl;

            /// <summary>
            /// サービスメソッドの種類を取得します。
            /// </summary>
            internal MethodType MethodType
            {
                get { return m_MethodType; }
            }
            private MethodType m_MethodType;

            /// <summary>
            /// リクエストの型を取得します。
            /// </summary>
            internal Type RequestType
            {
                get { return m_RequestType; }
            }
            private Type m_RequestType;

            /// <summary>
            /// レスポンスの型を取得します。
            /// </summary>
            internal Type ResponseType
            {
                get { return m_ResponseType; }
            }
            private Type m_ResponseType;
            
            /// <summary>
            /// 動作設定を取得します。
            /// </summary>
            internal GrpcServiceBuilderSettings Settings
            {
                get { return m_Settings; }
            }
            private GrpcServiceBuilderSettings m_Settings;

            /// <summary>
            /// メソッド呼び出し前の割込処理を取得します。
            /// </summary>
            internal IEnumerable<IGrpcServerMethodInvokingInterceptor> InvokingInterceptors
            {
                get { return m_InvokingInterceptors; }
            }
            private IEnumerable<IGrpcServerMethodInvokingInterceptor> m_InvokingInterceptors;

            /// <summary>
            /// メソッド呼び出し後の割込処理を取得します。
            /// </summary>
            internal IEnumerable<IGrpcServerMethodInvokedInterceptor> InvokedInterceptors
            {
                get { return m_InvokedInterceptors; }
            }
            private IEnumerable<IGrpcServerMethodInvokedInterceptor> m_InvokedInterceptors;
            
            /// <summary>
            /// 例外ハンドラを取得します。
            /// </summary>
            internal IEnumerable<IGrpcServerMethodExceptionHandler> ExceptionHandlers
            {
                get { return m_ExceptionHandlers; }
            }
            private IEnumerable<IGrpcServerMethodExceptionHandler> m_ExceptionHandlers;

            /// <summary>
            /// サービス名を取得します。
            /// </summary>
            /// <returns>サービス名</returns>
            internal string GetServiceName()
            {
                return m_ServiceType.Name;
            }

            /// <summary>
            /// サービスメソッド名を取得します。
            /// </summary>
            /// <returns>サービスメソッド名</returns>
            internal string GetServiceMethodName()
            {
                return m_MethodImpl.Name;
            }

            /// <summary>
            /// パフォーマンスログを通知する必要があるかどうかを取得します。
            /// </summary>
            /// <returns></returns>
            internal bool NeedNotifyPerformanceLog
            {
                get { return m_NeedNotifyPerformanceLog; }
            }
            private bool m_NeedNotifyPerformanceLog;

            /// <summary>
            /// パフォーマンスログを通知する必要があるかどうかを取得します。
            /// </summary>
            /// <returns></returns>
            private static bool GetNeedNotifyPerformanceLog(MethodInfo methodImpl)
            {

                GrpcPerformanceNotifyAttribute attr = methodImpl.GetCustomAttribute<GrpcPerformanceNotifyAttribute>(false);

                if (attr != null) { return attr.Enabled; }

                attr = methodImpl.ReflectedType.GetCustomAttribute<GrpcPerformanceNotifyAttribute>(false);

                if (attr != null) { return attr.Enabled; }

                return false;

            }

        }

        #region サービスメソッド定義の生成

        /// <summary>
        /// サービスメソッド定義を生成します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="context">コンテキスト</param>
        /// <returns>サービスメソッド定義></returns>
        private Method<TRequest, TResponse> CreateServiceMethodFieldGeneric<TRequest, TResponse>(MethodBuildContext context)
        {

            string serviceName = context.GetServiceName();
            string methodName = context.GetServiceMethodName();

            GrpcServerPerformanceListener performanceListener = context.NeedNotifyPerformanceLog ? context.Settings.PerformanceListener : null;

            Marshaller<TRequest> request = CreateMethodMarshaller<TRequest>(serviceName, methodName, performanceListener, context.Settings);
            Marshaller<TResponse> response = CreateMethodMarshaller<TResponse>(serviceName, methodName, performanceListener, context.Settings);

            return new Method<TRequest, TResponse>(context.MethodType, serviceName, methodName, request, response);

        }

        #endregion

        #region メソッド実装の取得

        /// <summary>
        /// 指定されたメソッドがサービスメソッドである場合、サービスメソッド情報を取得します。
        /// </summary>
        /// <param name="methodImpl">メソッド実装</param>
        /// <param name="methodType">サービスメソッドの種類</param>
        /// <param name="requestType">リクエストの型</param>
        /// <param name="responseType">レスポンスの型</param>
        /// <returns>サービスメソッドである場合、true を返します。</returns>
        private static bool TryGetServiceMathodInfo(MethodInfo methodImpl, out MethodType methodType, out Type requestType, out Type responseType)
        {

            if (methodImpl.IsPublic && !methodImpl.IsStatic)
            {

                if (IsUnaryMathod(methodImpl, out requestType, out responseType))
                {
                    methodType = MethodType.Unary;
                    return true;
                }
                else if (IsClientStreamingMathod(methodImpl, out requestType, out responseType))
                {
                    methodType = MethodType.ClientStreaming;
                    return true;
                }
                else if (IsServerStreamingMathod(methodImpl, out requestType, out responseType))
                {
                    methodType = MethodType.ServerStreaming;
                    return true;
                }
                else if (IsDuplexStreamingMathod(methodImpl, out requestType, out responseType))
                {
                    methodType = MethodType.DuplexStreaming;
                    return true;
                }

            }

            methodType = default(MethodType);
            responseType = null;
            requestType = null;

            return false;

        }

        /// <summary>
        /// 指定されたメソッドが <see cref="MethodType.Unary"/> と一致するかどうかを取得します。
        /// </summary>
        /// <param name="methodImpl">メソッド定義</param>
        /// <param name="requestType">リクエストの型</param>
        /// <param name="responseType">レスポンスの型</param>
        /// <returns>一致する場合、true を返します。</returns>
        private static bool IsUnaryMathod(MethodInfo methodImpl, out Type requestType, out Type responseType)
        {

            //Task<PersonSearchResponse> Search(PersonSearchRequest request, ServerCallContext context)

            requestType = null;
            responseType = null;

            if (!methodImpl.ReturnType.IsGenericType) { return false; }
            if (methodImpl.ReturnType.GetGenericTypeDefinition() != typeof(Task<>)) { return false; }

            ParameterInfo[] parameters = methodImpl.GetParameters();

            if (parameters.Length != 2) { return false; }

            if (IsRequestStreamType(parameters[0].ParameterType)) { return false; }
            if (parameters[1].ParameterType != typeof(ServerCallContext)) { return false; }

            requestType = parameters[0].ParameterType;
            responseType = methodImpl.ReturnType.GetGenericArguments()[0];

            return true;

        }

        /// <summary>
        /// 指定されたメソッドが <see cref="MethodType.ServerStreaming"/> と一致するかどうかを取得します。
        /// </summary>
        /// <param name="methodImpl">メソッド定義</param>
        /// <param name="requestType">リクエストの型</param>
        /// <param name="responseType">レスポンスの型</param>
        /// <returns>一致する場合、true を返します。</returns>
        private static bool IsServerStreamingMathod(MethodInfo methodImpl, out Type requestType, out Type responseType)
        {

            //Task BigSearch(PersonSearchRequest request, IServerStreamWriter<PersonSearchResponse> responseStream, ServerCallContext context)

            requestType = null;
            responseType = null;

            if (methodImpl.ReturnType != typeof(Task)) { return false; }

            ParameterInfo[] parameters = methodImpl.GetParameters();

            if (parameters.Length != 3) { return false; }

            if (IsRequestStreamType(parameters[0].ParameterType)) { return false; }
            if (!IsResponseStreamType(parameters[1].ParameterType)) { return false; }
            if (parameters[2].ParameterType != typeof(ServerCallContext)) { return false; }

            requestType = parameters[0].ParameterType;
            responseType = parameters[1].ParameterType.GetGenericArguments()[0];

            return true;

        }

        /// <summary>
        /// 指定されたメソッドが <see cref="MethodType.ClientStreaming"/> と一致するかどうかを取得します。
        /// </summary>
        /// <param name="methodImpl">メソッド定義</param>
        /// <param name="requestType">リクエストの型</param>
        /// <param name="responseType">レスポンスの型</param>
        /// <returns>一致する場合、true を返します。</returns>
        private static bool IsClientStreamingMathod(MethodInfo methodImpl, out Type requestType, out Type responseType)
        {

            //Task<PersonSearchResponse> RepeatSearch(IAsyncStreamReader<PersonSearchRequest> requestStream, ServerCallContext context)

            requestType = null;
            responseType = null;

            if (!methodImpl.ReturnType.IsGenericType) { return false; }
            if (methodImpl.ReturnType.GetGenericTypeDefinition() != typeof(Task<>)) { return false; }

            ParameterInfo[] parameters = methodImpl.GetParameters();

            if (parameters.Length != 2) { return false; }

            if (!IsRequestStreamType(parameters[0].ParameterType)) { return false; }
            if (parameters[1].ParameterType != typeof(ServerCallContext)) { return false; }

            requestType = parameters[0].ParameterType.GetGenericArguments()[0];
            responseType = methodImpl.ReturnType.GetGenericArguments()[0];

            return true;

        }

        /// <summary>
        /// 指定されたメソッドが <see cref="MethodType.DuplexStreaming"/> と一致するかどうかを取得します。
        /// </summary>
        /// <param name="methodImpl">メソッド定義</param>
        /// <param name="requestType">リクエストの型</param>
        /// <param name="responseType">レスポンスの型</param>
        /// <returns>一致する場合、true を返します。</returns>
        private static bool IsDuplexStreamingMathod(MethodInfo methodImpl, out Type requestType, out Type responseType)
        {

            //Task RepeatBigSearch(IAsyncStreamReader<PersonSearchRequest> requestStream, IServerStreamWriter<PersonSearchResponse> responseStream, ServerCallContext context)

            requestType = null;
            responseType = null;

            if (methodImpl.ReturnType != typeof(Task)) { return false; }

            ParameterInfo[] parameters = methodImpl.GetParameters();

            if (parameters.Length != 3) { return false; }

            if (!IsRequestStreamType(parameters[0].ParameterType)) { return false; }
            if (!IsResponseStreamType(parameters[1].ParameterType)) { return false; }
            if (parameters[2].ParameterType != typeof(ServerCallContext)) { return false; }

            requestType = parameters[0].ParameterType.GetGenericArguments()[0];
            responseType = parameters[1].ParameterType.GetGenericArguments()[0];

            return true;

        }

        /// <summary>
        /// 指定された型がリクエストストリームかどうかを取得します。
        /// </summary>
        /// <param name="t">型</param>
        /// <returns></returns>
        private static bool IsRequestStreamType(Type t)
        {

            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IAsyncStreamReader<>));

        }

        /// <summary>
        /// 指定された型がレスポンスストリームかどうかを取得します。
        /// </summary>
        /// <param name="t">型</param>
        /// <returns></returns>
        private static bool IsResponseStreamType(Type t)
        {

            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IServerStreamWriter<>));

        }

        #endregion

        #region サービスメソッドの追加

        /// <summary>
        /// サービスメソッドを追加します。
        /// </summary>
        /// <param name="builder">サービスビルダー</param>
        /// <param name="context">コンテキスト</param>
        private void AddServiceMethod(ServerServiceDefinition.Builder builder, MethodBuildContext context)
        {
            typeof(GrpcServiceBuilder).GetMethod("AddServiceMethodGeneric", BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(new Type[] { context.RequestType, context.ResponseType })
                .Invoke(this, new object[] { builder, context });
        }

        /// <summary>
        /// サービスメソッドを追加します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="builder">サービスビルダー</param>
        /// <param name="context">コンテキスト</param>
        private void AddServiceMethodGeneric<TRequest, TResponse>(ServerServiceDefinition.Builder builder, MethodBuildContext context)
            where TRequest : class where TResponse : class
        {

            Method<TRequest, TResponse> method = CreateServiceMethodFieldGeneric<TRequest, TResponse>(context);

            switch (method.Type)
            {

                case MethodType.Unary:

                    builder.AddMethod<TRequest, TResponse>(method, CreateUnaryServerMethod<TRequest, TResponse>(context));
                    break;

                case MethodType.ClientStreaming:

                    builder.AddMethod<TRequest, TResponse>(method, CreateClientStreamingServerMethod<TRequest, TResponse>(context));
                    break;

                case MethodType.ServerStreaming:

                    builder.AddMethod<TRequest, TResponse>(method, CreateServerStreamingServerMethod<TRequest, TResponse>(context));
                    break;

                case MethodType.DuplexStreaming:

                    builder.AddMethod<TRequest, TResponse>(method, CreateDuplexStreamingServerMethod<TRequest, TResponse>(context));
                    break;

            }

        }

        #endregion

        #region シリアライズ

        /// <summary>
        /// 指定されたメソッドに対するマーシャラーを生成します。
        /// </summary>
        /// <typeparam name="T">オブジェクトの型</typeparam>
        /// <param name="serviceName">サービス名</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="performanceListener">パフォーマンスリスナー</param>
        /// <param name="settings">動作設定</param>
        /// <returns>マーシャラ</returns>
        private Marshaller<T> CreateMethodMarshaller<T>(string serviceName, string methodName, GrpcServerPerformanceListener performanceListener, GrpcServiceBuilderSettings settings)
        {

            Marshaller<T> marshaller = settings.GetMarshallerFactoryOrDefault().GetMarshaller<T>();

            if (performanceListener == null) { return marshaller; }

            string typeName = typeof(T).Name;

            return new Marshaller<T>(

                delegate (T arg)
                {
                    try
                    {
                        Stopwatch watch = Stopwatch.StartNew();
                        byte[] data = marshaller.Serializer(arg);
                        performanceListener.NotifySerialized(serviceName, methodName, typeName, GrpcPerformanceListener.GetMilliseconds(watch), data == null ? 0 : data.Length);
                        return data;
                    }
                    catch (Exception ex)
                    {
                        GrpcExceptionListener.NotifyCatchSerializerException(serviceName, methodName, typeof(T), ex);
                        throw new GrpcSerializerException(ex.Message, ex, serviceName, methodName, typeName);
                    }
                }

                , delegate (byte[] data)
                {
                    try
                    {
                        Stopwatch watch = Stopwatch.StartNew();
                        T arg = marshaller.Deserializer(data);
                        performanceListener.NotifyDeserialized(serviceName, methodName, typeName, GrpcPerformanceListener.GetMilliseconds(watch), data == null ? 0 : data.Length);
                        return arg;
                    }
                    catch (Exception ex)
                    {
                        GrpcExceptionListener.NotifyCatchSerializerException(serviceName, methodName, typeof(T), ex);
                        throw new GrpcSerializerException(ex.Message, ex, serviceName, methodName, typeName);
                    }
                }

                );

        }

        #endregion

        #region メソッドの生成

        /// <summary>
        /// Unary メソッドを生成します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="builderContext">コンテキスト</param>
        /// <returns>メソッド</returns>
        private UnaryServerMethod<TRequest, TResponse> CreateUnaryServerMethod<TRequest, TResponse>(MethodBuildContext builderContext)
            where TRequest : class where TResponse : class
        {

            UnaryServerMethod<TRequest, TResponse> method = builderContext.MethodImpl.CreateDelegate(typeof(UnaryServerMethod<TRequest, TResponse>), builderContext.ServiceInstance) as UnaryServerMethod<TRequest, TResponse>;

            GrpcServerPerformanceListener performanceListener = builderContext.NeedNotifyPerformanceLog ? builderContext.Settings.PerformanceListener : null;

            return async delegate (TRequest request, ServerCallContext context)
            {

                try
                {

                    await OnExecutingServiceMethodAsync(context, builderContext.InvokingInterceptors, performanceListener).ConfigureAwait(false);

                    if (performanceListener != null) { performanceListener.NotifyMethodCalling(context); }

                    TResponse result;

                    Stopwatch watch = Stopwatch.StartNew();
                    double elapsd;

                    try
                    {
                        result = await method(request, context).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        throw new GrpcServerMethodException(string.Format(Properties.MessageResources.ServerMethodFailed, context.Method) + ex.Message, ex, context);
                    }
                    finally
                    {
                        elapsd = GrpcPerformanceListener.GetMilliseconds(watch);
                        if (performanceListener != null) { performanceListener.NotifyMethodCalled(context, elapsd); }
                    }

                    await OnExecutedServiceMethodAsync(context, builderContext.InvokedInterceptors, performanceListener).ConfigureAwait(false);

                    return result;

                }
                catch (Exception ex)
                {
                    Exception wrapped;
                    if (HandleException(context, builderContext.ExceptionHandlers, performanceListener, ex, out wrapped))
                    {
                        GrpcExceptionListener.NotifyCatchServerException(context, wrapped);
                        throw wrapped;
                    }
                    else
                    {
                        GrpcExceptionListener.NotifyCatchServerException(context, ex);
                        throw;
                    }
                }

            };

        }

        /// <summary>
        /// ClientStreaming メソッドを生成します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="builderContext">コンテキスト</param>
        /// <returns>メソッド</returns>
        private ClientStreamingServerMethod<TRequest, TResponse> CreateClientStreamingServerMethod<TRequest, TResponse>(MethodBuildContext builderContext)
            where TRequest : class where TResponse : class
        {

            ClientStreamingServerMethod<TRequest, TResponse> method = builderContext.MethodImpl.CreateDelegate(typeof(ClientStreamingServerMethod<TRequest, TResponse>), builderContext.ServiceInstance) as ClientStreamingServerMethod<TRequest, TResponse>;

            GrpcServerPerformanceListener performanceListener = builderContext.NeedNotifyPerformanceLog ? builderContext.Settings.PerformanceListener : null;

            return async delegate (IAsyncStreamReader<TRequest> requestStream, ServerCallContext context)
            {

                try
                {

                    await OnExecutingServiceMethodAsync(context, builderContext.InvokingInterceptors, performanceListener).ConfigureAwait(false);

                    if (performanceListener != null) { performanceListener.NotifyMethodCalling(context); }

                    TResponse result;

                    Stopwatch watch = Stopwatch.StartNew();
                    double elapsd;

                    try
                    {
                        result = await method(
                            new RequestStreamReader<TRequest>(requestStream, context, performanceListener)
                            , context
                            ).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        throw new GrpcServerMethodException(string.Format(Properties.MessageResources.ServerMethodFailed, context.Method) + ex.Message, ex, context);
                    }
                    finally
                    {
                        elapsd = GrpcPerformanceListener.GetMilliseconds(watch);
                        if (performanceListener != null) { performanceListener.NotifyMethodCalled(context, elapsd); }
                    }

                    await OnExecutedServiceMethodAsync(context, builderContext.InvokedInterceptors, performanceListener).ConfigureAwait(false);

                    return result;

                }
                catch (Exception ex)
                {
                    Exception wrapped;
                    if (HandleException(context, builderContext.ExceptionHandlers, performanceListener, ex, out wrapped))
                    {
                        GrpcExceptionListener.NotifyCatchServerException(context, wrapped);
                        throw wrapped;
                    }
                    else
                    {
                        GrpcExceptionListener.NotifyCatchServerException(context, ex);
                        throw;
                    }
                }

            };

        }

        /// <summary>
        ///  ServerStreaming メソッドを生成します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="builderContext">コンテキスト</param>
        /// <returns>メソッド</returns>
        private ServerStreamingServerMethod<TRequest, TResponse> CreateServerStreamingServerMethod<TRequest, TResponse>(MethodBuildContext builderContext)
            where TRequest : class where TResponse : class
        {

            ServerStreamingServerMethod<TRequest, TResponse> method = builderContext.MethodImpl.CreateDelegate(typeof(ServerStreamingServerMethod<TRequest, TResponse>), builderContext.ServiceInstance) as ServerStreamingServerMethod<TRequest, TResponse>;

            GrpcServerPerformanceListener performanceListener = builderContext.NeedNotifyPerformanceLog ? builderContext.Settings.PerformanceListener : null;

            return async delegate (TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context)
            {

                try
                {

                    await OnExecutingServiceMethodAsync(context, builderContext.InvokingInterceptors, performanceListener).ConfigureAwait(false);

                    if (performanceListener != null) { performanceListener.NotifyMethodCalling(context); }

                    Stopwatch watch = Stopwatch.StartNew();
                    double elapsd;

                    try
                    {
                        await method(request, new ResponseStreamWriter<TResponse>(responseStream, context, performanceListener), context).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        throw new GrpcServerMethodException(string.Format(Properties.MessageResources.ServerMethodFailed, context.Method) + ex.Message, ex, context);
                    }
                    finally
                    {
                        elapsd = GrpcPerformanceListener.GetMilliseconds(watch);
                        if (performanceListener != null) { performanceListener.NotifyMethodCalled(context, elapsd); }
                    }

                    await OnExecutedServiceMethodAsync(context, builderContext.InvokedInterceptors, performanceListener).ConfigureAwait(false);

                }
                catch (Exception ex)
                {
                    Exception wrapped;
                    if (HandleException(context, builderContext.ExceptionHandlers, performanceListener, ex, out wrapped))
                    {
                        GrpcExceptionListener.NotifyCatchServerException(context, wrapped);
                        throw wrapped;
                    }
                    else
                    {
                        GrpcExceptionListener.NotifyCatchServerException(context, ex);
                        throw;
                    }
                }

            };

        }

        /// <summary>
        ///  DuplexStreaming メソッドを生成します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="builderContext">コンテキスト</param>
        /// <returns>メソッド</returns>
        private DuplexStreamingServerMethod<TRequest, TResponse> CreateDuplexStreamingServerMethod<TRequest, TResponse>(MethodBuildContext builderContext)
            where TRequest : class where TResponse : class
        {

            DuplexStreamingServerMethod<TRequest, TResponse> method = builderContext.MethodImpl.CreateDelegate(typeof(DuplexStreamingServerMethod<TRequest, TResponse>), builderContext.ServiceInstance) as DuplexStreamingServerMethod<TRequest, TResponse>;

            GrpcServerPerformanceListener performanceListener = builderContext.NeedNotifyPerformanceLog ? builderContext.Settings.PerformanceListener : null;

            return async delegate (IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context)
            {

                try
                {

                    await OnExecutingServiceMethodAsync(context, builderContext.InvokingInterceptors, performanceListener).ConfigureAwait(false);

                    if (performanceListener != null) { performanceListener.NotifyMethodCalling(context); }

                    Stopwatch watch = Stopwatch.StartNew();
                    double elapsd;

                    try
                    {
                        await method(
                            new RequestStreamReader<TRequest>(requestStream, context, performanceListener)
                            , new ResponseStreamWriter<TResponse>(responseStream, context, performanceListener)
                            , context
                            ).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        throw new GrpcServerMethodException(string.Format(Properties.MessageResources.ServerMethodFailed, context.Method) + ex.Message, ex, context);
                    }
                    finally
                    {
                        elapsd = GrpcPerformanceListener.GetMilliseconds(watch);
                        if (performanceListener != null) { performanceListener.NotifyMethodCalled(context, elapsd); }
                    }

                    await OnExecutedServiceMethodAsync(context, builderContext.InvokedInterceptors, performanceListener).ConfigureAwait(false);

                }
                catch (Exception ex)
                {
                    Exception wrapped;
                    if (HandleException(context, builderContext.ExceptionHandlers, performanceListener, ex, out wrapped))
                    {
                        GrpcExceptionListener.NotifyCatchServerException(context, wrapped);
                        throw wrapped;
                    }
                    else
                    {
                        GrpcExceptionListener.NotifyCatchServerException(context, ex);
                        throw;
                    }
                }

            };

        }

        #endregion

        #region メソッド呼び出し前処理

        /// <summary>
        /// サービスメソッドが呼び出されるときの処理を行います。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="interceptors">割込処理</param>
        /// <param name="performanceListener">パフォーマンスリスナー</param>
        /// <returns></returns>
        private async Task OnExecutingServiceMethodAsync(ServerCallContext context, IEnumerable<IGrpcServerMethodInvokingInterceptor> interceptors, GrpcServerPerformanceListener performanceListener)
        {

            if (interceptors != null)
            {
                foreach (IGrpcServerMethodInvokingInterceptor interceptor in interceptors)
                {

                    if (interceptor == null) { continue; }

                    Stopwatch watch = Stopwatch.StartNew();

                    try
                    {
                        await interceptor.OnInvokingAsync(context).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        throw new GrpcServerMethodException(string.Format(Properties.MessageResources.ServerMethodInterceptorFailed + ex.Message, context.Method, interceptor.Name), ex, context, interceptor);
                    }
                    finally
                    {
                        if (performanceListener != null) { performanceListener.NotifyMethodIntercepted(context, interceptor, GrpcPerformanceListener.GetMilliseconds(watch)); }
                    }

                }
            }

        }

        #endregion

        #region メソッド呼び出し後処理

        /// <summary>
        /// サービスメソッドが呼び出されたときの処理を行います。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="interceptors">割込処理</param>
        /// <param name="performanceListener">パフォーマンスリスナー</param>
        /// <returns></returns>
        private async Task OnExecutedServiceMethodAsync(ServerCallContext context, IEnumerable<IGrpcServerMethodInvokedInterceptor> interceptors, GrpcServerPerformanceListener performanceListener)
        {

            if (interceptors != null)
            {
                foreach (IGrpcServerMethodInvokedInterceptor interceptor in interceptors)
                {

                    if (interceptor == null) { continue; }

                    Stopwatch watch = Stopwatch.StartNew();

                    try
                    {
                        await interceptor.OnInvokedAsync(context).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        throw new GrpcServerMethodException(string.Format(Properties.MessageResources.ServerMethodInterceptorFailed + ex.Message, context.Method, interceptor.Name), ex, context, interceptor);
                    }
                    finally
                    {
                        if (performanceListener != null) { performanceListener.NotifyMethodIntercepted(context, interceptor, GrpcPerformanceListener.GetMilliseconds(watch)); }
                    }

                }
            }

        }

        #endregion

        #region 例外処理

        /// <summary>
        /// 例外を処理します。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="exceptionHandlers">例外ハンドラ</param>
        /// <param name="performanceListener">パフォーマンスリスナー</param>
        /// <param name="original">発生した例外</param>
        /// <param name="alternate">代わりにスローする例外</param>
        /// <returns>例外がラップされた場合、true を返します。</returns>
        private bool HandleException(ServerCallContext context, IEnumerable<IGrpcServerMethodExceptionHandler> exceptionHandlers, GrpcServerPerformanceListener performanceListener, Exception original, out Exception alternate)
        {

            Exception alt = null;

            if (exceptionHandlers != null)
            {
                foreach (IGrpcServerMethodExceptionHandler handler in exceptionHandlers)
                {

                    if (handler == null) { continue; }

                    Stopwatch watch = Stopwatch.StartNew();

                    try
                    {
                        if (handler.RelpaceException(context, original, out alt))
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new GrpcServerMethodException(string.Format(Properties.MessageResources.ServerMethodInterceptorFailed + ex.Message, context.Method, handler.Name), ex, context, handler);
                    }
                    finally
                    {
                        if (performanceListener != null) { performanceListener.NotifyMethodIntercepted(context, handler, GrpcPerformanceListener.GetMilliseconds(watch)); }
                    }

                }
            }

            if (alt == null) { alt = original; }

            RpcException rpc = alt as RpcException;

            if (rpc == null)
            {
                alternate = CreateRpcException(context, alt);
            }
            else
            {
                alternate = rpc;
            }

            return (!object.Equals(original, alternate));

        }

        /// <summary>
        /// 指定された例外から RpcException を生成します。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="ex">例外</param>
        /// <returns>RpcException</returns>
        private RpcException CreateRpcException(ServerCallContext context, Exception ex)
        {

            Metadata trailers = new Metadata();

            return new RpcException(new Status(StatusCode.Internal, ex.Message), trailers);

        }

        #endregion

        #region 呼び出し前割込処理

        /// <summary>
        /// 指定された型に定義されている呼び出し前割込処理を取得します。
        /// </summary>
        /// <param name="type">型</param>
        /// <returns>割込処理</returns>
        private IList<IGrpcServerMethodInvokingInterceptor> GetInvokingInterceptors(Type type)
        {

            IEnumerable<GrpcServerMethodInvokingInterceptorAttribute> attrs = type.GetCustomAttributes<GrpcServerMethodInvokingInterceptorAttribute>(true);

            if (attrs == null) { return new IGrpcServerMethodInvokingInterceptor[] { }; }

            List<IGrpcServerMethodInvokingInterceptor> interceptors = new List<IGrpcServerMethodInvokingInterceptor>();

            foreach (GrpcServerMethodInvokingInterceptorAttribute attr in attrs)
            {
                interceptors.Add(attr);
            }

            return interceptors;

        }

        /// <summary>
        /// 指定されたメソッドに定義されている呼び出し前割込処理を取得します。
        /// </summary>
        /// <param name="method">メソッド</param>
        /// <returns>割込処理</returns>
        private IList<IGrpcServerMethodInvokingInterceptor> GetInvokingInterceptors(MethodInfo method)
        {

            IEnumerable<GrpcServerMethodInvokingInterceptorAttribute> attrs = method.GetCustomAttributes<GrpcServerMethodInvokingInterceptorAttribute>(true);

            if (attrs == null) { return new IGrpcServerMethodInvokingInterceptor[] { }; }

            List<IGrpcServerMethodInvokingInterceptor> interceptors = new List<IGrpcServerMethodInvokingInterceptor>();

            foreach (GrpcServerMethodInvokingInterceptorAttribute attr in attrs)
            {
                interceptors.Add(attr);
            }

            return interceptors;

        }

        #endregion

        #region 呼び出し後割込処理

        /// <summary>
        /// 指定された型に定義されている呼び出し後割込処理を取得します。
        /// </summary>
        /// <param name="type">型</param>
        /// <returns>割込処理</returns>
        private IList<IGrpcServerMethodInvokedInterceptor> GetInvokedInterceptors(Type type)
        {

            IEnumerable<GrpcServerMethodInvokedInterceptorAttribute> attrs = type.GetCustomAttributes<GrpcServerMethodInvokedInterceptorAttribute>(true);

            if (attrs == null) { return new IGrpcServerMethodInvokedInterceptor[] { }; }

            List<IGrpcServerMethodInvokedInterceptor> interceptors = new List<IGrpcServerMethodInvokedInterceptor>();

            foreach (GrpcServerMethodInvokedInterceptorAttribute attr in attrs)
            {
                interceptors.Add(attr);
            }

            return interceptors;

        }

        /// <summary>
        /// 指定されたメソッドに定義されている呼び出し後割込処理を取得します。
        /// </summary>
        /// <param name="method">メソッド</param>
        /// <returns>割込処理</returns>
        private IList<IGrpcServerMethodInvokedInterceptor> GetInvokedInterceptors(MethodInfo method)
        {

            IEnumerable<GrpcServerMethodInvokedInterceptorAttribute> attrs = method.GetCustomAttributes<GrpcServerMethodInvokedInterceptorAttribute>(true);

            if (attrs == null) { return new IGrpcServerMethodInvokedInterceptor[] { }; }

            List<IGrpcServerMethodInvokedInterceptor> interceptors = new List<IGrpcServerMethodInvokedInterceptor>();

            foreach (GrpcServerMethodInvokedInterceptorAttribute attr in attrs)
            {
                interceptors.Add(attr);
            }

            return interceptors;

        }

        #endregion

        #region 汎用処理

        /// <summary>
        /// 指定された型に実装されているメソッドを取得します。
        /// </summary>
        /// <param name="implType"></param>
        /// <returns></returns>
        private IDictionary<string, IList<MethodInfo>> GetMethodHandlers(Type implType)
        {

            Dictionary<string, IList<MethodInfo>> dic = new Dictionary<string, IList<MethodInfo>>();

            foreach (MethodInfo method in implType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {

                IList<MethodInfo> list;

                if (!dic.TryGetValue(method.Name, out list))
                {
                    list = new List<MethodInfo>();
                    dic.Add(method.Name, list);
                }

                list.Add(method);

            }

            return dic;

        }

        /// <summary>
        /// 指定されたコレクションの要素をソートします。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="comparer">比較メソッド</param>
        /// <param name="collections">コレクション</param>
        /// <returns></returns>
        private static IEnumerable<T> Sort<T>(Comparison<T> comparer, IEnumerable<IEnumerable<T>> collections)
        {

            List<T> list = new List<T>();

            if (collections != null)
            {
                foreach (IEnumerable<T> collection in collections)
                {
                    if (collection == null) { continue; }
                    list.AddRange(collection);
                }
            }

            if (list.Count > 1) { list.Sort(comparer); }

            return list;

        }

        /// <summary>
        /// 指定された割込処理の大小比較を行います。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int CompareInterceptor(IGrpcInterceptor a, IGrpcInterceptor b)
        {
            if (a != null && b != null)
            {
                return a.Priority.CompareTo(b.Priority);
            }
            else if (a != null)
            {
                return 1;
            }
            else if (b != null)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        #endregion

    }

}
