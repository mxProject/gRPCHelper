using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Clients.Core;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// AsyncClientStreamingCall に対する拡張メソッド。
    /// </summary>
    public static class AsyncClientStreamingCallExtension
    {

        #region リクエストを送信

        /// <summary>
        /// 全てのリクエストを送信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエスト</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<TResponse>> WriteAndCompleteAsync<TRequest, TResponse>(this AsyncClientStreamingCall<TRequest, TResponse> call, IEnumerable<TRequest> requests)
            where TRequest : class where TResponse : class
        {

            try
            {

                foreach (TRequest request in requests)
                {
                    await call.RequestStream.WriteAsync(request).ConfigureAwait(false);
                }

                await CompleteAsyncInternal(call).ConfigureAwait(false);

                return GrpcResult.Create<TResponse>(await call.ResponseAsync.ConfigureAwait(false), call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleResponseException(call, ex);
            }

        }

        /// <summary>
        /// 全てのリクエストを送信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエストを取得する処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<TResponse>> WriteAndCompleteAsync<TRequest, TResponse>(this AsyncClientStreamingCall<TRequest, TResponse> call, AsyncFunc<IEnumerable<TRequest>> requests) 
            where TRequest : class where TResponse : class
        {

            try
            {

                foreach (TRequest request in await requests().ConfigureAwait(false))
                {
                    await call.RequestStream.WriteAsync(request).ConfigureAwait(false);
                }

                await CompleteAsyncInternal(call).ConfigureAwait(false);

                return GrpcResult.Create<TResponse>(await call.ResponseAsync.ConfigureAwait(false), call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleResponseException(call, ex);
            }

        }

        /// <summary>
        /// 全てのリクエストを送信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエストを取得する処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<TResponse>> WriteAndCompleteAsync<TRequest, TResponse>(this AsyncClientStreamingCall<TRequest, TResponse> call, IEnumerable<AsyncFunc<TRequest>> requests)
            where TRequest : class where TResponse : class
        {

            try
            {

                foreach (AsyncFunc<TRequest> request in requests)
                {
                    await call.RequestStream.WriteAsync(await request().ConfigureAwait(false)).ConfigureAwait(false);
                }

                await CompleteAsyncInternal(call).ConfigureAwait(false);

                return GrpcResult.Create<TResponse>(await call.ResponseAsync.ConfigureAwait(false), call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleResponseException(call, ex);
            }

        }

        /// <summary>
        /// 全てのリクエストを送信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <typeparam name="TResult">実行結果の型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエスト</param>
        /// <param name="converter">実行結果への変換処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<TResult>> WriteAndCompleteAsync<TRequest, TResponse, TResult>(this AsyncClientStreamingCall<TRequest, TResponse> call, IEnumerable<TRequest> requests, Converter<TResponse, TResult> converter)
            where TRequest : class where TResponse : class
        {

            try
            {

                foreach (TRequest request in requests)
                {
                    await call.RequestStream.WriteAsync(request).ConfigureAwait(false);
                }

                await CompleteAsyncInternal(call).ConfigureAwait(false);

                return GrpcResult.Create<TResult>(converter(await call.ResponseAsync.ConfigureAwait(false)), call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleResponseException<TRequest, TResponse, TResult>(call, ex);
            }

        }

        /// <summary>
        /// 全てのリクエストを送信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <typeparam name="TResult">実行結果の型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエストを取得する処理</param>
        /// <param name="converter">実行結果への変換処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<TResult>> WriteAndCompleteAsync<TRequest, TResponse, TResult>(this AsyncClientStreamingCall<TRequest, TResponse> call, AsyncFunc<IEnumerable<TRequest>> requests, Converter<TResponse, TResult> converter)
            where TRequest : class where TResponse : class
        {

            try
            {

                foreach (TRequest request in await requests().ConfigureAwait(false))
                {
                    await call.RequestStream.WriteAsync(request).ConfigureAwait(false);
                }

                await CompleteAsyncInternal(call).ConfigureAwait(false);

                return GrpcResult.Create<TResult>(converter(await call.ResponseAsync.ConfigureAwait(false)), call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleResponseException<TRequest, TResponse, TResult>(call, ex);
            }

        }

        /// <summary>
        /// 全てのリクエストを送信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <typeparam name="TResult">実行結果の型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエストを取得する処理</param>
        /// <param name="converter">実行結果への変換処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<TResult>> WriteAndCompleteAsync<TRequest, TResponse, TResult>(this AsyncClientStreamingCall<TRequest, TResponse> call, IEnumerable<AsyncFunc<TRequest>> requests, Converter<TResponse, TResult> converter)
            where TRequest : class where TResponse : class
        {

            try
            {

                foreach (AsyncFunc<TRequest> request in requests)
                {
                    await call.RequestStream.WriteAsync(await request().ConfigureAwait(false)).ConfigureAwait(false);
                }

                await CompleteAsyncInternal(call).ConfigureAwait(false);

                return GrpcResult.Create<TResult>(converter(await call.ResponseAsync.ConfigureAwait(false)), call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleResponseException<TRequest, TResponse, TResult>(call, ex);
            }

        }

        #endregion

        #region 完了

        /// <summary>
        /// リクエストの送信を完了させます。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> CompleteRequestAsync<TRequest, TResponse>(this AsyncClientStreamingCall<TRequest, TResponse> call)
        {

            try
            {
                await call.RequestStream.CompleteAsync().ConfigureAwait(false);
                return new GrpcResult(call.ToInterface());
            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleException(call, ex);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="call"></param>
        /// <returns></returns>
        private static async Task CompleteAsyncInternal<TRequest, TResponse>(AsyncClientStreamingCall<TRequest, TResponse> call)
        {
            await call.RequestStream.CompleteAsync().ConfigureAwait(false);
        }

        #endregion

        #region 例外処理

        /// <summary>
        /// 指定された例外を処理し、実行結果を返します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="ex">例外</param>
        /// <returns>実行結果</returns>
        private static GrpcResult HandleException<TRequest, TResponse>(AsyncClientStreamingCall<TRequest, TResponse> call, Exception ex)
        {

            Exception actual = GrpcExceptionUtility.GetActualException(ex);

            GrpcCallState state;

            if (GrpcCallInvokerContext.TryGetState(call, out state))
            {
                GrpcExceptionListener.NotifyCatchClientException(state.Method, state.Host, state.Options, actual);
            }

            return GrpcResult.Create(actual);

        }

        /// <summary>
        /// 指定された例外を処理し、実行結果を返します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="ex">例外</param>
        /// <returns>実行結果</returns>
        private static GrpcResult<TResponse> HandleResponseException<TRequest, TResponse>(AsyncClientStreamingCall<TRequest, TResponse> call, Exception ex)
        {

            Exception actual = GrpcExceptionUtility.GetActualException(ex);

            GrpcCallState state;

            if (GrpcCallInvokerContext.TryGetState(call, out state))
            {
                GrpcExceptionListener.NotifyCatchClientException(state.Method, state.Host, state.Options, actual);
            }

            return GrpcResult.Create<TResponse>(actual);

        }

        /// <summary>
        /// 指定された例外を処理し、実行結果を返します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <typeparam name="TResult">実行結果の型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="ex">例外</param>
        /// <returns>実行結果</returns>
        private static GrpcResult<TResult> HandleResponseException<TRequest, TResponse, TResult>(AsyncClientStreamingCall<TRequest, TResponse> call, Exception ex)
        {

            Exception actual = GrpcExceptionUtility.GetActualException(ex);

            GrpcCallState state;

            if (GrpcCallInvokerContext.TryGetState(call, out state))
            {
                GrpcExceptionListener.NotifyCatchClientException(state.Method, state.Host, state.Options, actual);
            }

            return GrpcResult.Create<TResult>(actual);

        }

        #endregion

        #region 汎用処理

        /// <summary>
        /// インターフェースに変換します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <returns></returns>
        private static IGrpcAsyncCall ToInterface<TRequest, TResponse>(this AsyncClientStreamingCall<TRequest, TResponse> call)
        {
            return new AsyncClientStreamingCallWrapper<TRequest, TResponse>(call);
        }

        #endregion

    }

}
