using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// レスポンスを返す処理の実行結果。
    /// </summary>
    /// <typeparam name="TResponse">レスポンスの型</typeparam>
    public sealed class GrpcResult<TResponse> : GrpcResultBase
    {

        /// <summary>
        /// 呼び出しオブジェクトを指定してインスタンスを生成します。
        /// </summary>
        /// <param name="response">レスポンス</param>
        /// <param name="call">呼び出しオブジェクト</param>
        public GrpcResult(TResponse response, IGrpcAsyncCall call) : base(call)
        {
            m_Response = response;
        }

        /// <summary>
        /// 例外を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="exception">例外</param>
        public GrpcResult(Exception exception) : base(exception)
        {
        }

        /// <summary>
        /// レスポンスを取得します。
        /// </summary>
        public TResponse Response
        {
            get { return m_Response; }
        }
        private TResponse m_Response;

    }

}
