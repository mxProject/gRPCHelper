using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

using Grpc.Core;

using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Clients.Core;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// 呼び出しオブジェクト。
    /// </summary>
    public class GrpcCallInvoker : CallInvoker
    {

        #region コンストラクタ

        /// <summary>
        /// チャネルを指定してインスタンスを生成します。
        /// </summary>
        /// <param name="channel">チャネル</param>
        /// <param name="settings">動作設定</param>
        protected GrpcCallInvoker(Channel channel, GrpcClientSettings settings) : this(new DefaultCallInvoker(channel), settings)
        {
        }

        /// <summary>
        /// 呼び出しオブジェクトを指定してインスタンスを生成します。
        /// </summary>
        /// <param name="invoker">呼び出しオブジェクト</param>
        /// <param name="settings">動作設定</param>
        protected GrpcCallInvoker(CallInvoker invoker, GrpcClientSettings settings) : base()
        {
            m_Invoker = invoker;
            m_Settings = settings ?? new GrpcClientSettings();
            m_Settings.SortInterceptors();
        }

        #endregion

        #region ファクトリメソッド

        /// <summary>
        /// チャネルを指定してインスタンスを生成します。
        /// </summary>
        /// <param name="channel">チャネル</param>
        public static GrpcCallInvoker Create(Channel channel)
        {
            return Create(channel, null);
        }

        /// <summary>
        /// チャネルと動作設定を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="channel">チャネル</param>
        /// <param name="settings">動作設定</param>
        public static GrpcCallInvoker Create(Channel channel, GrpcClientSettings settings)
        {

            GrpcClientSettings clone = null;

            if (settings != null)
            {
                clone = settings.CreateDeepClone();
            }

            return new GrpcCallInvoker(channel, clone);

        }

        /// <summary>
        /// 呼び出しオブジェクトを指定してインスタンスを生成します。
        /// </summary>
        /// <param name="invoker">呼び出しオブジェクト</param>
        public static GrpcCallInvoker Create(CallInvoker invoker)
        {

            return new GrpcCallInvoker(invoker, null);

        }

        /// <summary>
        /// 呼び出しオブジェクトと動作設定を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="invoker">呼び出しオブジェクト</param>
        /// <param name="settings">動作設定</param>
        public static GrpcCallInvoker Create(CallInvoker invoker, GrpcClientSettings settings) 
        {

            GrpcClientSettings clone = null;

            if (settings != null)
            {
                clone = settings.CreateDeepClone();
            }

            return new GrpcCallInvoker(invoker, clone);

        }

        #endregion

        /// <summary>
        /// 呼び出しオブジェクトを取得します。
        /// </summary>
        protected CallInvoker Invoker
        {
            get { return m_Invoker; }
        }
        private CallInvoker m_Invoker;

        /// <summary>
        /// 動作設定を取得します。
        /// </summary>
        protected GrpcClientSettings Settings
        {
            get { return m_Settings; }
        }
        private GrpcClientSettings m_Settings;

        #region メソッド呼び出し

        /// <summary>
        /// ClientStreaming メソッドを非同期で呼び出します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <returns>呼び出しオブジェクト</returns>
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {

            double elapsed = 0;

            try
            {

                method = GetCustomMethod<TRequest, TResponse>(method);

                OnInvokingMethod(method, host, options, default(TRequest));

                Stopwatch watch = null;
                AsyncClientStreamingCall<TRequest, TResponse> call;

                try
                {
                    watch = Stopwatch.StartNew();
                    call = m_Invoker.AsyncClientStreamingCall(method, host, options);
                }
                finally
                {
                    elapsed = GrpcPerformanceListener.GetMilliseconds(watch);
                }

                OnInvokedMethod(method, host, options, default(TRequest), elapsed);

                return GrpcCallInvokerContext.Regist<TRequest, TResponse>(call, method, host, options, m_Settings.PerformanceListener);

            }
            catch (Exception ex)
            {

                GrpcExceptionListener.NotifyCatchClientException(method, host, options, ex);

                Exception alternate;

                if (HandleException(method, host, options, ex, out alternate))
                {
                    throw alternate;
                }
                else
                {
                    throw;
                }

            }

        }

        /// <summary>
        /// DuplexStreaming メソッドを非同期で呼び出します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <returns>呼び出しオブジェクト</returns>
        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {

            double elapsed = 0;

            try
            {

                method = GetCustomMethod<TRequest, TResponse>(method);

                OnInvokingMethod(method, host, options, default(TRequest));

                Stopwatch watch = null;
                AsyncDuplexStreamingCall<TRequest, TResponse> call;

                try
                {
                    watch = Stopwatch.StartNew();
                    call = m_Invoker.AsyncDuplexStreamingCall(method, host, options);
                }
                finally
                {
                    elapsed = GrpcPerformanceListener.GetMilliseconds(watch);
                }

                OnInvokedMethod(method, host, options, default(TRequest), elapsed);

                return GrpcCallInvokerContext.Regist<TRequest, TResponse>(call, method, host, options, m_Settings.PerformanceListener);

            }
            catch (Exception ex)
            {

                GrpcExceptionListener.NotifyCatchClientException(method, host, options, ex);

                Exception alternate;

                if (HandleException(method, host, options, ex, out alternate))
                {
                    throw alternate;
                }
                else
                {
                    throw;
                }

            }

        }

        /// <summary>
        /// ServerStreaming メソッドを非同期で呼び出します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="request">リクエスト</param>
        /// <returns>呼び出しオブジェクト</returns>
        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {

            double elapsed = 0;

            try
            {

                method = GetCustomMethod<TRequest, TResponse>(method);

                OnInvokingMethod(method, host, options, request);

                Stopwatch watch = null;
                AsyncServerStreamingCall<TResponse> call;

                try
                {
                    watch = Stopwatch.StartNew();
                    call = m_Invoker.AsyncServerStreamingCall(method, host, options, request);
                }
                finally
                {
                    elapsed = GrpcPerformanceListener.GetMilliseconds(watch);
                }

                OnInvokedMethod(method, host, options, request, elapsed);

                return GrpcCallInvokerContext.Regist<TRequest, TResponse>(call, method, host, options, m_Settings.PerformanceListener);

            }
            catch (Exception ex)
            {

                GrpcExceptionListener.NotifyCatchClientException(method, host, options, ex);

                Exception alternate;

                if (HandleException(method, host, options, ex, out alternate))
                {
                    throw alternate;
                }
                else
                {
                    throw;
                }

            }

        }

        /// <summary>
        /// Unary メソッドを非同期で呼び出します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="request">リクエスト</param>
        /// <returns>呼び出しオブジェクト</returns>
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {

            double elapsed = 0;

            try
            {

                method = GetCustomMethod<TRequest, TResponse>(method);

                OnInvokingMethod(method, host, options, request);

                Stopwatch watch = null;
                AsyncUnaryCall<TResponse> call;

                try
                {
                    watch = Stopwatch.StartNew();
                    call = m_Invoker.AsyncUnaryCall(method, host, options, request);
                }
                finally
                {
                    elapsed = GrpcPerformanceListener.GetMilliseconds(watch);
                }

                OnInvokedMethod(method, host, options, request, elapsed);

                // 待機されると dispose が呼ばれず、解放されずに残ってしまう
                // ストリーム操作もないため、監視しない
                // return GrcpCallInvokerContext.Regist<TRequest, TResponse>(call, method, host, options);
                return call;

            }
            catch (Exception ex)
            {

                GrpcExceptionListener.NotifyCatchClientException(method, host, options, ex);

                Exception alternate;

                if (HandleException(method, host, options, ex, out alternate))
                {
                    throw alternate;
                }
                else
                {
                    throw;
                }

            }

        }

        /// <summary>
        /// Unary メソッドを呼び出します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="request">リクエスト</param>
        /// <returns>レスポンス</returns>
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {

            double elapsed = 0;

            try
            {

                method = GetCustomMethod<TRequest, TResponse>(method);

                OnInvokingMethod(method, host, options, request);

                Stopwatch watch = null;

                TResponse result;

                try
                {
                    watch = Stopwatch.StartNew();
                    result = m_Invoker.BlockingUnaryCall(method, host, options, request);
                }
                finally
                {
                    elapsed = GrpcPerformanceListener.GetMilliseconds(watch);
                }

                OnInvokedMethod(method, host, options, request, elapsed);

                return result;

            }
            catch (Exception ex)
            {

                GrpcExceptionListener.NotifyCatchClientException(method, host, options, ex);

                Exception alternate;

                if (HandleException(method, host, options, ex, out alternate)) {
                    throw alternate;
                }
                else
                {
                    throw;
                }

            }

        }

        #endregion

        #region メソッド実行前処理

        /// <summary>
        /// メソッドが実行されるときの処理を行います。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="request">リクエスト</param>
        /// <param name="options">オプション</param>
        protected virtual void OnInvokingMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {

            if (m_Settings.PerformanceListener != null)
            {
                m_Settings.PerformanceListener.NotifyMethodCalling(method, host, options);
            }

            foreach (IGrpcClientMethodInvokingInterceptor interceptor in m_Settings.InvokingInterceptors)
            {

                if (interceptor == null) { continue; }

                Stopwatch watch = Stopwatch.StartNew();

                try
                {
                    interceptor.OnInvoking(method, host, options);

                    if (m_Settings.PerformanceListener != null)
                    {
                        m_Settings.PerformanceListener.NotifyMethodIntercepted(method, host, options, interceptor, GrpcPerformanceListener.GetMilliseconds(watch));
                    }
                }
                catch (Exception ex)
                {
                    throw new GrpcClientMethodException(string.Format(Properties.MessageResources.ClientMethodInterceptorFailed + ex.Message, method.FullName, interceptor.Name), ex, method, host, options, interceptor);
                }

            }

        }

        #endregion

        #region メソッド実行後処理

        /// <summary>
        /// メソッドが実行されたときの処理を行います。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="request">リクエスト</param>
        /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
        protected virtual void OnInvokedMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request, double elapsedMilliseconds)
        {

            if (m_Settings.PerformanceListener != null)
            {
                m_Settings.PerformanceListener.NotifyMethodCalled(method, host, options, elapsedMilliseconds);
            }

            foreach (IGrpcClientMethodInvokedInterceptor interceptor in m_Settings.InvokingInterceptors)
            {

                if (interceptor == null) { continue; }

                Stopwatch watch = Stopwatch.StartNew();

                try
                {
                    interceptor.OnInvoked(method, host, options);

                    if (m_Settings.PerformanceListener != null)
                    {
                        m_Settings.PerformanceListener.NotifyMethodIntercepted(method, host, options, interceptor, GrpcPerformanceListener.GetMilliseconds(watch));
                    }
                }
                catch (Exception ex)
                {
                    throw new GrpcClientMethodException(string.Format(Properties.MessageResources.ClientMethodInterceptorFailed + ex.Message, method.FullName, interceptor.Name), ex, method, host, options, interceptor);
                }

            }

        }

        #endregion

        #region 例外処理

        /// <summary>
        /// 例外をキャッチしたときの処理を行います。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="original">キャッチした例外</param>
        /// <param name="alternate">代わりにスローさせる例外</param>
        /// <returns>処理された場合、true を返します。</returns>
        private bool HandleException<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, Exception original, out Exception alternate)
        {

            Exception alt = null;

            foreach (IGrpcClientMethodExceptionHandler interceptor in m_Settings.InvokingInterceptors)
            {

                if (interceptor == null) { continue; }

                Stopwatch watch = Stopwatch.StartNew();

                try
                {
                    bool wrapped = interceptor.ReplaceException(method, host, options, original, out alt);

                    if (m_Settings.PerformanceListener != null)
                    {
                        m_Settings.PerformanceListener.NotifyMethodIntercepted(method, host, options, interceptor, GrpcPerformanceListener.GetMilliseconds(watch));
                    }

                    if (wrapped) { break; }
                }
                catch (Exception ex)
                {
                    GrpcExceptionListener.NotifyCatchClientException(method, host, options, ex);
                    throw new GrpcClientMethodException(string.Format(Properties.MessageResources.ClientMethodInterceptorFailed + ex.Message, method.FullName, interceptor.Name), ex, method, host, options, interceptor);
                }

            }

            if (alt != null && !object.Equals(alt, original))
            {
                alternate = alt;
                return true;
            }
            else
            {
                alternate = null;
                return false;
            }

        }

        #endregion

        #region マーシャラー

        private readonly Dictionary<string, IMethod> m_CustomMethods = new Dictionary<string, IMethod>();

        /// <summary>
        /// メソッド定義を取得します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="original">オリジナルのメソッド定義</param>
        /// <returns></returns>
        private Method<TRequest, TResponse> GetCustomMethod<TRequest, TResponse>(Method<TRequest, TResponse> original)
        {

            IMethod custom;

            if (m_CustomMethods.TryGetValue(original.FullName, out custom))
            {
                return (Method<TRequest, TResponse>)custom;
            }

            lock (m_CustomMethods)
            {

                if (m_CustomMethods.TryGetValue(original.FullName, out custom))
                {
                    return (Method<TRequest, TResponse>)custom;
                }

                Marshaller<TRequest> request = GetMarshaller<TRequest>(original.ServiceName, original.Name, true);
                Marshaller<TResponse> response = GetMarshaller<TResponse>(original.ServiceName, original.Name, true);

                custom = new Method<TRequest, TResponse>(original.Type, original.ServiceName, original.Name, request, response);

                m_CustomMethods.Add(custom.FullName, custom);

                return (Method<TRequest, TResponse>)custom;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="outputPerformanceLog"></param>
        /// <returns></returns>
        private Marshaller<T> GetMarshaller<T>(string serviceName, string methodName, bool outputPerformanceLog)
        {

            Marshaller<T> marshaller = m_Settings.GetMarshallerFactoryOrDefault().GetMarshaller<T>();

            if (!outputPerformanceLog) { return marshaller; }

            string typeName = typeof(T).Name;

            return new Marshaller<T>(

                delegate (T arg)
                {
                    try
                    {
                        Stopwatch watch = Stopwatch.StartNew();
                        byte[] data = marshaller.Serializer(arg);

                        if (m_Settings.PerformanceListener!= null)
                        {
                            m_Settings.PerformanceListener.NotifySerialized(serviceName, methodName, typeName, GrpcPerformanceListener.GetMilliseconds(watch), data == null ? 0 : data.Length);
                        }

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

                        if (m_Settings.PerformanceListener != null)
                        {
                            m_Settings.PerformanceListener.NotifyDeserialized(serviceName, methodName, typeName, GrpcPerformanceListener.GetMilliseconds(watch), data == null ? 0 : data.Length);
                        }

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

    }
}
