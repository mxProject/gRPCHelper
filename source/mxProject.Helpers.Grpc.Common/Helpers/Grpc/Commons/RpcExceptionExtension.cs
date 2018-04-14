using System;
using System.Collections.Generic;
using System.Text;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// <see cref="RpcException"/> に対する汎用メソッド。
    /// </summary>
    public static class RpcExceptionExtension
    {

        #region 例外処理

        /// <summary>
        /// キャンセルを意味するかどうかを取得します。
        /// </summary>
        /// <returns></returns>
        public static bool IsCancel(this RpcException ex)
        {

            if (ex == null) { return false; }

            if (ex.Status.StatusCode != StatusCode.Cancelled) { return false; }

            return true;

        }

        #endregion

    }

}
