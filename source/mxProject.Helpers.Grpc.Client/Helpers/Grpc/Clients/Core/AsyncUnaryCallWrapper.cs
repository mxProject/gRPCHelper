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
    /// <typeparam name="TResponse"></typeparam>
    internal class AsyncUnaryCallWrapper<TResponse> : IGrpcAsyncCall
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="call"></param>
        internal AsyncUnaryCallWrapper(AsyncUnaryCall<TResponse> call)
        {
            m_Call = call;
        }

        private AsyncUnaryCall<TResponse> m_Call;

        /// <summary>
        /// レスポンスデータの取得が完了しているかどうかを取得します。
        /// </summary>
        public bool IsEndResponse
        {
            get
            {
                return m_Call.ResponseAsync.IsCompleted;
            }
        }

        /// <summary>
        /// リクエストストリームの処理が完了しているかどうかを取得します。
        /// </summary>
        bool IGrpcAsyncCall.IsRequestStreamCompleted
        {
            get
            {
                return true;
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
