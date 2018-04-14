using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Grpc.Core;
using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.AspNetGateway
{

    /// <summary>
    /// 
    /// </summary>
    public class GrpcGatewayMiddleware
    {

        #region ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="methods"></param>
        /// <param name="settings"></param>
        public GrpcGatewayMiddleware(RequestDelegate next, IEnumerable<GrpcServiceMethod> methods, GrpcGatewaySettings settings)
        {

            m_Next = next;
            m_Settings = settings ?? new GrpcGatewaySettings(null);

            m_ServiceMethods = methods == null ? null : methods.ToDictionary(x => x.Method.FullName);

        }

        #endregion

        #region fields

        /// <summary>
        /// 
        /// </summary>
        private readonly RequestDelegate m_Next;

        /// <summary>
        /// 
        /// </summary>
        private readonly GrpcGatewaySettings m_Settings;

        /// <summary>
        /// 
        /// </summary>
        private readonly IDictionary<string, GrpcServiceMethod> m_ServiceMethods;

        #endregion

        #region http handling

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {

            GrpcServiceMethod method = null;
            RpcException rpc;
            System.Net.HttpStatusCode? errorStatus = null;
            object errorResponseObject = null;

            try
            {

                string requestPath = httpContext.Request.Path.Value;

                if (m_ServiceMethods == null || !m_ServiceMethods.TryGetValue(requestPath, out method))
                {
                    if (m_Next != null) { await m_Next(httpContext); }
                    return;
                }

                IDictionary<string, string> headers = GetRequestHeaders(httpContext, IsGrpcRequestHeader);

                string requestJson = GetRequestJson(httpContext);

                object requestObject = DeselializeFromJson(method, requestJson, method.GetJsonRequestType());

                object responseObject = await CallGrpcAsync(method, headers, requestObject);

                string responseJson = SerializeToJson(method, responseObject);

                httpContext.Response.ContentType = "application/json";

                await httpContext.Response.WriteAsync(responseJson);

            }
            catch (Exception ex) when (TryGetRpcException(ex, out rpc) && HandleRpcException(method, rpc, out errorStatus, out errorResponseObject))
            {
                if (errorStatus.HasValue) { httpContext.Response.StatusCode = (int)errorStatus; }
                if (errorResponseObject != null) { await httpContext.Response.WriteAsync(SerializeToJson(method, errorResponseObject)); }
            }
            catch (Exception ex) when (HandleException(method, ex, out errorStatus, out errorResponseObject))
            {
                if (errorStatus.HasValue) { httpContext.Response.StatusCode = (int)errorStatus; }
                if (errorResponseObject != null) { await httpContext.Response.WriteAsync(SerializeToJson(method, errorResponseObject)); }
            }
            catch
            {
                httpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="keyNameFilter"></param>
        /// <returns></returns>
        private IDictionary<string, string> GetRequestHeaders(HttpContext context, GrpcRequestHeaderFilter keyNameFilter)
        {

            Dictionary<string, string> headers = new Dictionary<string, string>();

            foreach ( string key in context.Request.Headers.Keys)
            {

                string grpcKey = null;

                if (keyNameFilter!= null && !keyNameFilter(key, out grpcKey)) { continue; }

                Microsoft.Extensions.Primitives.StringValues value = context.Request.Headers[key];

                headers.Add(grpcKey, value.FirstOrDefault());

            }

            return headers;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpHeaderKey"></param>
        /// <param name="grpcHeaderKey"></param>
        /// <returns></returns>
        private bool IsGrpcRequestHeader(string httpHeaderKey, out string grpcHeaderKey)
        {

            if (m_Settings.RequestHeaderFilter != null)
            {
                return m_Settings.RequestHeaderFilter(httpHeaderKey, out grpcHeaderKey);
            }

            grpcHeaderKey = null;
            return false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetRequestJson(HttpContext context)
        {

            using (var sr = new System.IO.StreamReader(context.Request.Body, m_Settings.RequestEncoding ?? GrpcGatewaySettings.DefaultRequestEncoding))
            {
                return sr.ReadToEnd();
            }

        }

        #endregion

        #region json serialization

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="json"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        private object DeselializeFromJson(GrpcServiceMethod method, string json, Type objectType)
        {

            if (method.JsonSerializer!= null)
            {
                return method.JsonSerializer.Deserialize(json, objectType);
            }

            if (m_Settings.JsonSerializer != null)
            {
                return m_Settings.JsonSerializer.Deserialize(json, objectType);
            }

            return GrpcGatewaySettings.DefaultJsonSerializer.Deserialize(json, objectType);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string SerializeToJson(GrpcServiceMethod method, object obj)
        {

            if (method.JsonSerializer != null)
            {
                return method.JsonSerializer.Serialize(obj);
            }

            if (m_Settings.JsonSerializer != null)
            {
                return m_Settings.JsonSerializer.Serialize(obj);
            }

            return GrpcGatewaySettings.DefaultJsonSerializer.Serialize(obj);

        }

        #endregion

        #region exception handling

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="rpc"></param>
        /// <returns></returns>
        private bool TryGetRpcException(Exception ex, out RpcException rpc)
        {

            if (ex is RpcException)
            {
                rpc = ex as RpcException;
                return true;
            }

            if (TryGetRpcException(ex.InnerException, out rpc))
            {
                return true;
            }

            if (ex is AggregateException)
            {
                foreach (Exception inner in ((AggregateException)ex).InnerExceptions)
                {
                    if (TryGetRpcException(inner, out rpc))
                    {
                        return true;
                    }
                }
            }

            rpc = null;
            return false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="ex"></param>
        /// <param name="responseStatus"></param>
        /// <param name="responseObject"></param>
        /// <returns></returns>
        private bool HandleRpcException(GrpcServiceMethod method, RpcException ex, out System.Net.HttpStatusCode? responseStatus, out object responseObject)
        {

            if (method.RpcExceptionHandler != null)
            {
                if (method.RpcExceptionHandler(ex, out responseStatus, out responseObject)) { return true; }
            }

            if (m_Settings.RpcExceptionHandler != null)
            {
                if (m_Settings.RpcExceptionHandler(ex, out responseStatus, out responseObject)) { return true; }
            }

            return ExceptionHandlers.NotHandle(ex, out responseStatus, out responseObject);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="ex"></param>
        /// <param name="responseStatus"></param>
        /// <param name="responseObject"></param>
        /// <returns></returns>
        private bool HandleException(GrpcServiceMethod method, Exception ex, out System.Net.HttpStatusCode? responseStatus, out object responseObject)
        {

            if (method.ExceptionHandler != null)
            {
                if (method.ExceptionHandler(ex, out responseStatus, out responseObject)) { return true; }
            }

            if (m_Settings.ExceptionHandler != null)
            {
                if (m_Settings.ExceptionHandler(ex, out responseStatus, out responseObject)) { return true; }
            }

            return ExceptionHandlers.NotHandle(ex, out responseStatus, out responseObject);

        }

        #endregion

        #region gRPC

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        private Task<object> CallGrpcAsync(GrpcServiceMethod method, IDictionary<string, string> headers, object requestObject)
        {

            object requests;

            if (requestObject != null && typeof(IEnumerable<>).MakeGenericType(method.RequestType).IsAssignableFrom(requestObject.GetType()))
            {
                requests = requestObject;
            }
            else
            {
                Array ary = Array.CreateInstance(method.RequestType, 1);
                ary.SetValue(requestObject, 0);
                requests = ary;
            }

            System.Reflection.MethodInfo m = typeof(GrpcGatewayMiddleware).GetMethod("CallGrpcAsyncCore", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            Task<object> task = (Task<object>)m.MakeGenericMethod(new Type[] { method.RequestType, method.ResponseType }).Invoke(this, new object[] { method, headers, requests });

            return task;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="requests"></param>
        /// <returns></returns>
        private Task<object> CallGrpcAsyncCore<TRequest, TResponse>(GrpcServiceMethod method, IDictionary<string, string> headers, IEnumerable<TRequest> requests) where TRequest : class where TResponse : class
        {

            CallInvoker invoker = new DefaultCallInvoker(m_Settings.Channel);

            CallOptions option = CreateCallOptions(headers);

            Method<TRequest, TResponse> rpc = (Method<TRequest, TResponse>)method.Method;

            switch (rpc.Type)
            {

                case MethodType.Unary:

                    Task<TResponse> taskUnary = AsyncUnaryCall(invoker, rpc, option, requests.FirstOrDefault());

                    return Task.FromResult<object>(taskUnary.Result);

                case MethodType.ClientStreaming:

                    Task<TResponse> taskClientStreaming = AsyncClientStreamingCall(invoker, rpc, option, requests);

                    return Task.FromResult<object>(taskClientStreaming.Result);

                case MethodType.ServerStreaming:

                    Task<IList<TResponse>> taskServerStreaming = AsyncServerStreamingCall(invoker, rpc, option, requests.FirstOrDefault());

                    return Task.FromResult<object>(taskServerStreaming.Result);

                case MethodType.DuplexStreaming:

                    Task<IList<TResponse>> taskDuplexStreaming = AsyncDuplexStreamingCall(invoker, rpc, option, requests);

                    return Task.FromResult<object>(taskDuplexStreaming.Result);

                default:
                    throw new NotSupportedException(string.Format("MethodType '{0}' is not supported.", rpc.Type));

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        private CallOptions CreateCallOptions(IDictionary<string, string> headers)
        {

            Metadata meta = new Metadata();

            foreach ( KeyValuePair<string, string> entry in headers)
            {
                meta.Add(entry.Key, entry.Value);
            }

            CallOptions option = new CallOptions(meta);

            return option;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="invoker"></param>
        /// <param name="method"></param>
        /// <param name="option"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private Task<TResponse> AsyncUnaryCall<TRequest, TResponse>(CallInvoker invoker, Method<TRequest, TResponse> method, CallOptions option, TRequest request) where TRequest : class where TResponse : class
        {

            return invoker.AsyncUnaryCall(method, null, option, request).ResponseAsync;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="invoker"></param>
        /// <param name="method"></param>
        /// <param name="option"></param>
        /// <param name="requests"></param>
        /// <returns></returns>
        private async Task<TResponse> AsyncClientStreamingCall<TRequest, TResponse>(CallInvoker invoker, Method<TRequest, TResponse> method, CallOptions option, IEnumerable<TRequest> requests) where TRequest : class where TResponse : class
        {

            using (AsyncClientStreamingCall<TRequest, TResponse> call = invoker.AsyncClientStreamingCall(method, null, option))
            {

                if (requests != null)
                {
                    foreach (TRequest request in requests)
                    {
                        await call.RequestStream.WriteAsync(request).ConfigureAwait(false);
                    }
                }

                await call.RequestStream.CompleteAsync().ConfigureAwait(false);

                return call.ResponseAsync.Result;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="invoker"></param>
        /// <param name="method"></param>
        /// <param name="option"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<IList<TResponse>> AsyncServerStreamingCall<TRequest, TResponse>(CallInvoker invoker, Method<TRequest, TResponse> method, CallOptions option, TRequest request) where TRequest : class where TResponse : class
        {

            using (AsyncServerStreamingCall<TResponse> call = invoker.AsyncServerStreamingCall(method, null, option, request))
            {

                List<TResponse> responses = new List<TResponse>();

                while (await call.ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    responses.Add(call.ResponseStream.Current);
                }

                return responses;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="invoker"></param>
        /// <param name="method"></param>
        /// <param name="option"></param>
        /// <param name="requests"></param>
        /// <returns></returns>
        private async Task<IList<TResponse>> AsyncDuplexStreamingCall<TRequest, TResponse>(CallInvoker invoker, Method<TRequest, TResponse> method, CallOptions option, IEnumerable<TRequest> requests) where TRequest : class where TResponse : class
        {

            using (AsyncDuplexStreamingCall<TRequest, TResponse> call = invoker.AsyncDuplexStreamingCall(method, null, option))
            {

                if (requests != null)
                {
                    foreach (TRequest request in requests)
                    {
                        await call.RequestStream.WriteAsync(request).ConfigureAwait(false);
                    }
                }

                await call.RequestStream.CompleteAsync().ConfigureAwait(false);

                List<TResponse> responses = new List<TResponse>();

                while (await call.ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    responses.Add(call.ResponseStream.Current);
                }

                return responses;

            }

        }

        #endregion

        #region utility

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static IList<GrpcServiceMethod> GetGrpcMethods(string serviceName, Type serviceType)
        {
            return GetGrpcMethods(serviceName, serviceType, GrpcMarshallerFactory.DefaultInstance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="serviceType"></param>
        /// <param name="marshallerFactory"></param>
        /// <returns></returns>
        public static IList<GrpcServiceMethod> GetGrpcMethods(string serviceName, Type serviceType, IGrpcMarshallerFactory marshallerFactory)
        {

            List<GrpcServiceMethod> methods = new List<GrpcServiceMethod>();

            foreach (GrpcMethodHandlerInfo handler in GrpcReflection.EnumerateServiceMethods(serviceType))
            {

                IMethod method = GrpcReflection.CreateMethod(serviceName, handler, marshallerFactory);

                methods.Add(new GrpcServiceMethod(method, handler.RequestType, handler.ResponseType));

            }

            return methods;

        }

        #endregion

    }

}
