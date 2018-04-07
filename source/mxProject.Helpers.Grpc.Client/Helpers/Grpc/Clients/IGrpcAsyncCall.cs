using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// 非同期呼び出しオブジェクトに必要な機能を提供します。
    /// </summary>
    public interface IGrpcAsyncCall
    {

        /// <summary>
        /// レスポンスデータの取得が完了しているかどうかを取得します。
        /// </summary>
        bool IsEndResponse
        {
            get;
        }

        /// <summary>
        /// 処理が完了しているかどうかを取得します。
        /// </summary>
        bool IsRequestStreamCompleted
        {
            get;
        }

        /// <summary>
        /// レスポンスヘッダーを取得します。
        /// </summary>
        Task<Metadata> ResponseHeadersAsync
        {
            get;
        }

        /// <summary>
        /// ステータスを取得します。
        /// </summary>
        /// <returns></returns>
        Status GetStatus();

        /// <summary>
        /// トレーラーを取得します。
        /// </summary>
        /// <returns></returns>
        Metadata GetTrailers();

    }

}
