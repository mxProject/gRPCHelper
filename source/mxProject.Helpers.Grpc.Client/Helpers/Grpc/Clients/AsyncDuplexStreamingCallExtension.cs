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
    /// AsyncDuplexStreamingCall に対する拡張メソッド。
    /// </summary>
    public static class AsyncDuplexStreamingCallExtension
    {

        #region リクエストを送信

        /// <summary>
        /// リクエストを送信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="request">リクエスト</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> WriteAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, TRequest request)
            where TRequest : class where TResponse : class
        {

            try
            {

                await call.RequestStream.WriteAsync(request).ConfigureAwait(false);
                
                return new GrpcResult(call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleException(call, ex);
            }

        }

        /// <summary>
        /// リクエストを送信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="request">リクエスト</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> WriteAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, Func<TRequest> request)
            where TRequest : class where TResponse : class
        {

            try
            {

                await call.RequestStream.WriteAsync(request()).ConfigureAwait(false);

                return new GrpcResult(call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleException(call, ex);
            }

        }

        /// <summary>
        /// リクエストを送信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="request">リクエスト</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> WriteAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, AsyncFunc<TRequest> request)
            where TRequest : class where TResponse : class
        {

            try
            {

                await call.RequestStream.WriteAsync(await request().ConfigureAwait(false)).ConfigureAwait(false);

                return new GrpcResult(call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleException(call, ex);
            }

        }

        #endregion

        #region 複数のリクエストを送信

        /// <summary>
        /// 全てのリクエストを送信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="completeStream">ストリームを完了させるかどうか</param>
        /// <param name="requests">リクエスト</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> WriteAllAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, IEnumerable<TRequest> requests, bool completeStream)
            where TRequest : class where TResponse : class
        {

            try
            {

                foreach (TRequest request in requests)
                {
                    await call.RequestStream.WriteAsync(request).ConfigureAwait(false);
                }

                return new GrpcResult(call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleException(call, ex);
            }
            finally
            {
                if (completeStream)
                {
                    await CompleteAsyncInternal(call);
                }
            }

        }

        /// <summary>
        /// 全てのリクエストを送信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエストを取得する処理</param>
        /// <param name="completeStream">ストリームを完了させるかどうか</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> WriteAllAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, AsyncFunc<IEnumerable<TRequest>> requests, bool completeStream)
            where TRequest : class where TResponse : class
        {

            try
            {

                foreach (TRequest request in await requests().ConfigureAwait(false))
                {
                    await call.RequestStream.WriteAsync(request).ConfigureAwait(false);
                }

                return new GrpcResult(call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleException(call, ex);
            }
            finally
            {
                if (completeStream)
                {
                    await CompleteAsyncInternal(call);
                }
            }

        }

        /// <summary>
        /// 全てのリクエストを送信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエストを取得する処理</param>
        /// <param name="completeStream">ストリームを完了させるかどうか</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> WriteAllAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, IEnumerable<AsyncFunc<TRequest>> requests, bool completeStream)
            where TRequest : class where TResponse : class
        {

            try
            {

                foreach (AsyncFunc<TRequest> request in requests)
                {
                    await call.RequestStream.WriteAsync(await request().ConfigureAwait(false)).ConfigureAwait(false);
                }

                return new GrpcResult(call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleException(call, ex);
            }
            finally
            {
                if (completeStream)
                {
                    await CompleteAsyncInternal(call);
                }
            }

        }

        #endregion

        #region レスポンスを受信

        /// <summary>
        /// 全てのレスポンスを受信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<IList<TResponse>>> ReadAllAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call)
            where TRequest : class where TResponse : class
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

        /// <summary>
        /// 全てのレスポンスを受信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <typeparam name="TResult">実行結果の型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="converter">実行結果への変換処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<IList<TResult>>> ReadAllAsync<TRequest, TResponse, TResult>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, Converter<TResponse, TResult> converter)
            where TRequest : class where TResponse : class
        {

            try
            {

                List<TResult> list = new List<TResult>();

                while (await call.ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    list.Add(converter(call.ResponseStream.Current));
                }

                return GrpcResult.Create<IList<TResult>>(list, call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleResponseListException<TRequest, TResponse, TResult>(call, ex);
            }

        }

        #endregion

        #region 送受信

        /// <summary>
        /// 全てのリクエストを送信し、全てのレスポンスを受信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエスト</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<IList<TResponse>>> WriteReadAllAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, IEnumerable<TRequest> requests)
            where TRequest : class where TResponse : class
        {

            Task<GrpcResult> send = WriteAllAsync(call, requests, true);
            Task<GrpcResult<IList<TResponse>>> receive = ReadAllAsync(call);

            await Task.WhenAll(send, receive).ConfigureAwait(false);

            return await receive;

        }

        /// <summary>
        /// 全てのリクエストを送信し、全てのレスポンスを受信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエストを取得する処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<IList<TResponse>>> WriteReadAllAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, AsyncFunc<IEnumerable<TRequest>> requests)
            where TRequest : class where TResponse : class
        {

            Task<GrpcResult> send = WriteAllAsync(call, requests, true);
            Task<GrpcResult<IList<TResponse>>> receive = ReadAllAsync(call);

            await Task.WhenAll(send, receive).ConfigureAwait(false);

            return await receive;

        }

        /// <summary>
        /// 全てのリクエストを送信し、全てのレスポンスを受信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエストを取得する処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<IList<TResponse>>> WriteReadAllAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, IEnumerable<AsyncFunc<TRequest>> requests)
            where TRequest : class where TResponse : class
        {

            Task<GrpcResult> send = WriteAllAsync(call, requests, true);
            Task<GrpcResult<IList<TResponse>>> receive = ReadAllAsync(call);

            await Task.WhenAll(send, receive).ConfigureAwait(false);

            return await receive;

        }

        /// <summary>
        /// 全てのリクエストを送信し、全てのレスポンスを受信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <typeparam name="TResult">実行結果の型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエスト</param>
        /// <param name="converter">実行結果への変換処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<IList<TResult>>> WriteReadAllAsync<TRequest, TResponse, TResult>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, IEnumerable<TRequest> requests, Converter<TResponse, TResult> converter)
            where TRequest : class where TResponse : class
        {

            Task<GrpcResult> send = WriteAllAsync(call, requests, true);
            Task<GrpcResult<IList<TResult>>> receive = ReadAllAsync<TRequest, TResponse, TResult>(call, converter);

            await Task.WhenAll(send, receive).ConfigureAwait(false);

            return await receive;

        }

        /// <summary>
        /// 全てのリクエストを送信し、全てのレスポンスを受信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <typeparam name="TResult">実行結果の型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエストを取得する処理</param>
        /// <param name="converter">実行結果への変換処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<IList<TResult>>> WriteReadAllAsync<TRequest, TResponse, TResult>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, AsyncFunc<IEnumerable<TRequest>> requests, Converter<TResponse, TResult> converter)
            where TRequest : class where TResponse : class
        {

            Task<GrpcResult> send = WriteAllAsync(call, requests, true);
            Task<GrpcResult<IList<TResult>>> receive = ReadAllAsync<TRequest, TResponse, TResult>(call, converter);

            await Task.WhenAll(send, receive).ConfigureAwait(false);

            return await receive;

        }

        /// <summary>
        /// 全てのリクエストを送信し、全てのレスポンスを受信します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <typeparam name="TResult">実行結果の型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエストを取得する処理</param>
        /// <param name="converter">実行結果への変換処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult<IList<TResult>>> WriteReadAllAsync<TRequest, TResponse, TResult>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, IEnumerable<AsyncFunc<TRequest>> requests, Converter<TResponse, TResult> converter)
            where TRequest : class where TResponse : class
        {

            Task<GrpcResult> send = WriteAllAsync(call, requests, true);
            Task<GrpcResult<IList<TResult>>> receive = ReadAllAsync<TRequest, TResponse, TResult>(call, converter);

            await Task.WhenAll(send, receive).ConfigureAwait(false);

            return await receive;

        }

        #endregion

        #region レスポンスに対して処理を実行

        /// <summary>
        /// 全てのレスポンスに対して指定された処理を実行します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="onResponse">実行する処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> ForEachAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, Action<TResponse> onResponse) 
            where TRequest : class where TResponse : class
        {

            try
            {

                while (await call.ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    onResponse(call.ResponseStream.Current);
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
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="onResponse">実行する処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> ForEachAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, AsyncAction<TResponse> onResponse) 
            where TRequest : class where TResponse : class
        {

            try
            {

                while (await call.ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    await onResponse(call.ResponseStream.Current).ConfigureAwait(false);
                }

                return new GrpcResult(call.ToInterface());

            }
            catch (Exception ex) when (GrpcExceptionUtility.HasRpcException(ex))
            {
                return HandleException(call, ex);
            }

        }

        #endregion

        #region リクエストを送信し、レスポンスに対して処理を実行

        /// <summary>
        /// 全てのリクエストを送信し、全てのレスポンスに対して指定された処理を実行します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエスト</param>
        /// <param name="action">実行する処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> WriteAndForEachAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, IEnumerable<TRequest> requests, Action<TResponse> action) 
            where TRequest : class where TResponse : class
        {

            Task<GrpcResult> send = WriteAllAsync(call, requests, true);
            Task<GrpcResult> receive = ForEachAsync(call, action);

            await Task.WhenAll(send, receive).ConfigureAwait(false);

            return await receive;

        }

        /// <summary>
        /// 全てのリクエストを送信し、全てのレスポンスに対して指定された処理を実行します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエストを取得する処理</param>
        /// <param name="action">実行する処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> WriteAndForEachAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, AsyncFunc<IEnumerable<TRequest>> requests, Action<TResponse> action)
            where TRequest : class where TResponse : class
        {

            Task<GrpcResult> send = WriteAllAsync(call, requests, true);
            Task<GrpcResult> receive = ForEachAsync(call, action);

            await Task.WhenAll(send, receive).ConfigureAwait(false);

            return await receive;

        }

        /// <summary>
        /// 全てのリクエストを送信し、全てのレスポンスに対して指定された処理を実行します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエストを取得する処理</param>
        /// <param name="action">実行する処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> WriteAndForEachAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, IEnumerable<AsyncFunc<TRequest>> requests, Action<TResponse> action)
            where TRequest : class where TResponse : class
        {

            Task<GrpcResult> send = WriteAllAsync(call, requests, true);
            Task<GrpcResult> receive = ForEachAsync(call, action);

            await Task.WhenAll(send, receive).ConfigureAwait(false);

            return await receive;

        }

        /// <summary>
        /// 全てのリクエストを送信し、全てのレスポンスに対して指定された処理を実行します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエスト</param>
        /// <param name="action">実行する処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> WriteAndForEachAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, IEnumerable<TRequest> requests, AsyncAction<TResponse> action)
            where TRequest : class where TResponse : class
        {

            Task<GrpcResult> send = WriteAllAsync(call, requests, true);
            Task<GrpcResult> receive = ForEachAsync(call, action);

            await Task.WhenAll(send, receive).ConfigureAwait(false);

            return await receive;

        }

        /// <summary>
        /// 全てのリクエストを送信し、全てのレスポンスに対して指定された処理を実行します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエストを取得する処理</param>
        /// <param name="action">実行する処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> WriteAndForEachAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, AsyncFunc<IEnumerable<TRequest>> requests, AsyncAction<TResponse> action) 
            where TRequest : class where TResponse : class
        {

            Task<GrpcResult> send = WriteAllAsync(call, requests, true);
            Task<GrpcResult> receive = ForEachAsync(call, action);

            await Task.WhenAll(send, receive).ConfigureAwait(false);

            return await receive;

        }

        /// <summary>
        /// 全てのリクエストを送信し、全てのレスポンスに対して指定された処理を実行します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="requests">リクエストを取得する処理</param>
        /// <param name="action">実行する処理</param>
        /// <returns>実行結果</returns>
        public static async Task<GrpcResult> WriteAndForEachAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, IEnumerable<AsyncFunc<TRequest>> requests, AsyncAction<TResponse> action)
            where TRequest : class where TResponse : class
        {

            Task<GrpcResult> send = WriteAllAsync(call, requests, true);
            Task<GrpcResult> receive = ForEachAsync(call, action);

            await Task.WhenAll(send, receive).ConfigureAwait(false);

            return await receive;

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
        public static async Task<GrpcResult> CompleteRequestAsync<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call)
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
        private static async Task CompleteAsyncInternal<TRequest, TResponse>(AsyncDuplexStreamingCall<TRequest, TResponse> call)
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
        private static GrpcResult HandleException<TRequest, TResponse>(AsyncDuplexStreamingCall<TRequest, TResponse> call, Exception ex)
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
        private static GrpcResult<TResponse> HandleResponseException<TRequest, TResponse>(AsyncDuplexStreamingCall<TRequest, TResponse> call, Exception ex)
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
        private static GrpcResult<TResult> HandleResponseException<TRequest, TResponse, TResult>(AsyncDuplexStreamingCall<TRequest, TResponse> call, Exception ex)
        {

            Exception actual = GrpcExceptionUtility.GetActualException(ex);

            GrpcCallState state;

            if (GrpcCallInvokerContext.TryGetState(call, out state))
            {
                GrpcExceptionListener.NotifyCatchClientException(state.Method, state.Host, state.Options, actual);
            }

            return GrpcResult.Create<TResult>(actual);

        }

        /// <summary>
        /// 指定された例外を処理し、実行結果を返します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="ex">例外</param>
        /// <returns>実行結果</returns>
        private static GrpcResult<IList<TResponse>> HandleResponseListException<TRequest, TResponse>(AsyncDuplexStreamingCall<TRequest, TResponse> call, Exception ex)
        {

            Exception actual = GrpcExceptionUtility.GetActualException(ex);

            GrpcCallState state;

            if (GrpcCallInvokerContext.TryGetState(call, out state))
            {
                GrpcExceptionListener.NotifyCatchClientException(state.Method, state.Host, state.Options, actual);
            }

            return GrpcResult.Create<IList<TResponse>>(actual);

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
        private static GrpcResult<IList<TResult>> HandleResponseListException<TRequest, TResponse, TResult>(AsyncDuplexStreamingCall<TRequest, TResponse> call, Exception ex)
        {

            Exception actual = GrpcExceptionUtility.GetActualException(ex);

            GrpcCallState state;

            if (GrpcCallInvokerContext.TryGetState(call, out state))
            {
                GrpcExceptionListener.NotifyCatchClientException(state.Method, state.Host, state.Options, actual);
            }

            return GrpcResult.Create<IList<TResult>>(actual);

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
        private static IGrpcAsyncCall ToInterface<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call)
        {
            return new AsyncDuplexStreamingCallWrapper<TRequest, TResponse>(call);
        }

        #endregion

    }

}
