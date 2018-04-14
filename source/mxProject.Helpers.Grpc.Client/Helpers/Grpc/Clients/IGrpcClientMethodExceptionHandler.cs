using System;
using System.Collections.Generic;
using System.Text;

using Grpc.Core;
using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// クライアントからのメソッド呼び出しに対する例外処理に必要な機能を提供します。
    /// </summary>
    public interface IGrpcClientMethodExceptionHandler : IGrpcInterceptor
    {

        /// <summary>
        /// 発生した例外を置き換えます。
        /// </summary>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="original">発生した例外</param>
        /// <param name="replaced">代わりにスローさせる例外</param>
        /// <returns></returns>
        bool ReplaceException(IMethod method, string host, CallOptions options, Exception original, out Exception replaced);

    }

}
