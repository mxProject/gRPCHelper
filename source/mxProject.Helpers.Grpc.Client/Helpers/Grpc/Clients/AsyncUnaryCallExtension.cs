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
    /// AsyncUnaryCall に対する拡張メソッド。
    /// </summary>
    public static class AsyncUnaryCallExtension
    {

        #region 実行結果の取得

        /// <summary>
        /// 実行結果を取得します。
        /// </summary>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<TResponse>> GetResultAsync<TResponse>(this AsyncUnaryCall<TResponse> call)
            where TResponse : class
        {

            try
            {

                return GrpcResult.Create<TResponse>(await call.ResponseAsync.ConfigureAwait(false), call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleResponseException(call, ex);
            }
            finally
            {
                call.Dispose();
            }

        }

        /// <summary>
        /// 実行結果を取得します。
        /// </summary>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <typeparam name="TResult">実行結果の型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="converter">実行結果への変換処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<TResult>> GetResultAsync<TResponse, TResult>(this AsyncUnaryCall<TResponse> call, Converter<TResponse, TResult> converter)
            where TResponse : class
        {
            
            try
            {

                return GrpcResult.Create<TResult>(converter(await call.ResponseAsync.ConfigureAwait(false)), call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleResponseException<TResponse, TResult>(call, ex);
            }
            finally
            {
                call.Dispose();
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
        private static GrpcResult HandleException<TResponse>(AsyncUnaryCall<TResponse> call, Exception ex)
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
        private static GrpcResult<TResponse> HandleResponseException<TResponse>(AsyncUnaryCall<TResponse> call, Exception ex)
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
        /// <typeparam name="TResult">実行結果の型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="ex">例外</param>
        /// <returns>実行結果</returns>
        private static GrpcResult<TResult> HandleResponseException<TResponse, TResult>(AsyncUnaryCall<TResponse> call, Exception ex)
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
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <returns></returns>
        private static IGrpcAsyncCall ToInterface<TResponse>(this AsyncUnaryCall<TResponse> call)
        {
            return new AsyncUnaryCallWrapper<TResponse>(call);
        }

        #endregion

    }

}
