using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Clients.Core
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    internal class AsyncDuplexStreamingCallWrapper<TRequest, TResponse> : IGrpcAsyncCall
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="call"></param>
        internal AsyncDuplexStreamingCallWrapper(AsyncDuplexStreamingCall<TRequest, TResponse> call)
        {
            m_Call = call;
        }

        private AsyncDuplexStreamingCall<TRequest, TResponse> m_Call;

        private readonly AsyncLock m_Lock = new AsyncLock(true);

        /// <summary>
        /// レスポンスデータの取得が完了しているかどうかを取得します。
        /// </summary>
        public bool IsEndResponse
        {
            get
            {
                GrpcCallState state;

                if (!GrpcCallInvokerContext.TryGetState<TRequest, TResponse>(m_Call, out state))
                {
                    return true;
                }

                return state.IsEndResponse;
            }
        }

        /// <summary>
        /// リクエストストリームの処理が完了しているかどうかを取得します。
        /// </summary>
        public bool IsRequestStreamCompleted
        {
            get
            {
                GrpcCallState state;

                if (!GrpcCallInvokerContext.TryGetState<TRequest, TResponse>(m_Call, out state))
                {
                    return true;
                }

                return state.IsRequestStreamCompleted;
            }
        }

        /// <summary>
        /// レスポンスヘッダーを取得します。
        /// </summary>
        public Task<Metadata> ResponseHeadersAsync
        {
            get
            {
                return m_Call.ResponseHeadersAsync;
            }
        }

        #region ステータス

        /// <summary>
        /// ステータスを取得します。
        /// </summary>
        /// <returns></returns>
        public Status GetStatus()
        {
            return m_Call.GetStatus();
        }

        #endregion

        #region トレーラー

        /// <summary>
        /// トレーラーを取得します。
        /// </summary>
        /// <returns></returns>
        public Metadata GetTrailers()
        {
            return m_Call.GetTrailers();
        }

        #endregion

    }

}
