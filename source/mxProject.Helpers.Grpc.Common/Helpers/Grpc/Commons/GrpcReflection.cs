using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// gRPC に関するリフレクション処理。
    /// </summary>
    public static class GrpcReflection
    {

        #region サービスメソッドの取得

        /// <summary>
        /// 指定された型に実装されているサービスメソッドを列挙します。
        /// </summary>
        /// <param name="serviceImplType">サービス実装の型</param>
        /// <returns></returns>
        public static IEnumerable<GrpcMethodHandlerInfo> EnumerateServiceMethods(Type serviceImplType)
        {

            foreach (MethodInfo method in serviceImplType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {

                if (IsIgnore(method)) { continue; }

                MethodType methodType;
                Type requestType;
                Type responseType;

                if (!TryGetServiceMethodInfo(method, out methodType, out requestType, out responseType)) { continue; }

                yield return new GrpcMethodHandlerInfo(methodType, requestType, responseType, method);

            }

        }

        /// <summary>
        /// 指定されたメソッドを無視するかどうかを取得します。
        /// </summary>
        /// <param name="methodImpl">メソッド実装</param>
        /// <returns>無視する場合、true を返します。</returns>
        private static bool IsIgnore(MethodInfo methodImpl)
        {

            GrpcIgnoreAttribute attr = methodImpl.GetCustomAttribute<GrpcIgnoreAttribute>(false);

            return (attr != null);

        }

        /// <summary>
        /// 指定されたメソッドがサービスメソッドである場合、サービスメソッド情報を取得します。
        /// </summary>
        /// <param name="methodImpl">メソッド実装</param>
        /// <param name="methodType">サービスメソッドの種類</param>
        /// <param name="requestType">リクエストの型</param>
        /// <param name="responseType">レスポンスの型</param>
        /// <returns>サービスメソッドである場合、true を返します。</returns>
        private static bool TryGetServiceMethodInfo(MethodInfo methodImpl, out MethodType methodType, out Type requestType, out Type responseType)
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

            //Task<TResponse> Method(TRequest request, ServerCallContext context)

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

            //Task Method(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context)

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

            //Task<TResponse> Method(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context)

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

            //Task Method(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context)

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

        #region メソッド定義の生成

        /// <summary>
        /// メソッド定義を生成します。
        /// </summary>
        /// <param name="serviceName">サービス名</param>
        /// <param name="handler">メソッドハンドラ情報</param>
        /// <param name="marshallerFactory">マーシャラーの生成処理</param>
        /// <returns>メソッド定義</returns>
        public static IMethod CreateMethod(string serviceName, GrpcMethodHandlerInfo handler, IGrpcMarshallerFactory marshallerFactory)
        {

            MethodInfo m = typeof(GrpcReflection).GetMethod("CreateMethodCore", BindingFlags.Static | BindingFlags.NonPublic);

            return (IMethod)m.MakeGenericMethod(new Type[] { handler.RequestType, handler.ResponseType }).Invoke(null, new object[] { serviceName, handler, marshallerFactory });

        }

        /// <summary>
        /// メソッド定義を生成します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="serviceName">サービス名</param>
        /// <param name="handler">メソッドハンドラ情報</param>
        /// <param name="marshallerFactory">マーシャラーの生成処理</param>
        /// <returns>メソッド定義</returns>
        private static Method<TRequest, TResponse> CreateMethodCore<TRequest, TResponse>(string serviceName, GrpcMethodHandlerInfo handler, IGrpcMarshallerFactory marshallerFactory)
        {

            return new Method<TRequest, TResponse>(handler.MethodType, serviceName, handler.Handler.Name, marshallerFactory.GetMarshaller<TRequest>(), marshallerFactory.GetMarshaller<TResponse>());

        }

        #endregion

    }

}
