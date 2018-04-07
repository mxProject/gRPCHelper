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
    /// AsyncServerStreamingCall に対する拡張メソッド。
    /// </summary>
    public static class AsyncServerStreamingCallExtension
    {

        #region レスポンスの受信

        /// <summary>
        /// 全てのレスポンスを受信します。
        /// </summary>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<IList<TResponse>>> ReadAllAsync<TResponse>(this AsyncServerStreamingCall<TResponse> call) 
            where TResponse : class
        {

            try
            {

                List<TResponse> list = new List<TResponse>();

                while (await call.ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    list.Add(call.ResponseStream.Current);
                }

                return GrpcResult.Create<IList<TResponse>>(await Task.FromResult(list).ConfigureAwait(false), call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleResponseListException(call, ex);
            }

        }

        #endregion

        #region レスポンスに対して処理を実行

        /// <summary>
        /// 全てのレスポンスに対して指定された処理を実行します。
        /// </summary>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="action">実行する処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> ForEachAsync<TResponse>(this AsyncServerStreamingCall<TResponse> call, Action<TResponse> action) 
            where TResponse : class
        {

            try
            {

                while (await call.ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    action(call.ResponseStream.Current);
                }

                return new GrpcResult(call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleException(call, ex);
            }

        }

        /// <summary>
        /// 全てのレスポンスに対して指定された処理を実行します。
        /// </summary>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="action">実行する処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> ForEachAsync<TResponse>(this AsyncServerStreamingCall<TResponse> call, AsyncAction<TResponse> action) 
            where TResponse : class
        {

            try
            {

                while (await call.ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    await action(call.ResponseStream.Current).ConfigureAwait(false);
                }

                return new GrpcResult(call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleException(call, ex);
            }

        }

        #endregion

        #region 例外処理

        /// <summary>
        /// 指定された例外を処理し、実行結果を返します。
        /// </summary>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="ex">例外</param>
        /// <returns>実行結果</returns>
        private static GrpcResult HandleException<TResponse>(AsyncServerStreamingCall<TResponse> call, Exception ex)
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
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="ex">例外</param>
        /// <returns>実行結果</returns>
        private static GrpcResult<TResponse> HandleResponseException<TResponse>(AsyncServerStreamingCall<TResponse> call, Exception ex)
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
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="ex">例外</param>
        /// <returns>実行結果</returns>
        private static GrpcResult<IList<TResponse>> HandleResponseListException<TResponse>(AsyncServerStreamingCall<TResponse> call, Exception ex)
        {

            Exception actual = GrpcExceptionUtility.GetActualException(ex);

            GrpcCallState state;

            if (GrpcCallInvokerContext.TryGetState(call, out state))
            {
                GrpcExceptionListener.NotifyCatchClientException(state.Method, state.Host, state.Options, actual);
            }

            return GrpcResult.Create<IList<TResponse>>(actual);

        }

        #endregion
        #region 汎用処理

        /// <summary>
        /// インターフェースに変換します。
        /// </summary>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <returns></returns>
        private static IGrpcAsyncCall ToInterface<TResponse>(this AsyncServerStreamingCall<TResponse> call)
        {
            return new AsyncServerStreamingCallWrapper<TResponse>(call);
        }

        #endregion

    }

}
