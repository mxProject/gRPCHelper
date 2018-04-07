using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.Servers
{

    /// <summary>
    /// サーバー側のメソッド呼び出しに対する例外処理に必要な機能を提供します。
    /// </summary>
    public interface IGrpcServerMethodExceptionHandler: IGrpcInterceptor
    {

        /// <summary>
        /// 発生した例外を置き換えます。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="original">発生したした例外</param>
        /// <param name="replaced">代わりにスローさせる例外</param>
        /// <returns>置き換えた場合、true を返します。</returns>
        bool RelpaceException(ServerCallContext context, Exception original, out Exception replaced);

    }

}
