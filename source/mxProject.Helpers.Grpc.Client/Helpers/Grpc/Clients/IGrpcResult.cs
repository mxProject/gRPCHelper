using System;
using System.Collections.Generic;
using System.Text;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// 実行結果に必要な機能を提供します。
    /// </summary>
    public interface IGrpcResult
    {

        /// <summary>
        /// ステータスを取得可能かどうかを取得します。
        /// </summary>
        bool CanGetStatus
        {
            get;
        }

        /// <summary>
        /// ステータスが OK かどうかを取得します。
        /// </summary>
        bool IsOK
        {
            get;
        }

        /// <summary>
        /// キャンセルされたかどうかを取得します。
        /// </summary>
        bool IsCanceled
        {
            get;
        }

        /// <summary>
        /// エラーが発生したかどうかを取得します。
        /// </summary>
        bool IsError
        {
            get;
        }

        /// <summary>
        /// キャンセルまたはエラーが発生したかどうかを取得します。
        /// </summary>
        bool IsCanceledOrError
        {
            get;
        }

        /// <summary>
        /// ステータスコードを取得します。
        /// </summary>
        StatusCode? GetStatusCode();

        /// <summary>
        /// ステータスの詳細を取得します。
        /// </summary>
        string GetStatusDetail();

        /// <summary>
        /// トレーラーを取得可能かどうかを取得します。
        /// </summary>
        bool CanGetTrailers
        {
            get;
        }

        /// <summary>
        /// トレーラーを取得します。
        /// </summary>
        Metadata Trailers
        {
            get;
        }

        /// <summary>
        /// 発生した例外を取得します。
        /// </summary>
        Exception Exception
        {
            get;
        }

    }

}
