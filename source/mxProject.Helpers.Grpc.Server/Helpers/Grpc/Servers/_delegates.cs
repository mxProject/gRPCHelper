using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Servers
{

    /// <summary>
    /// サービスメソッド呼び出しに対する割込処理を行うメソッドであることを表します。
    /// </summary>
    /// <param name="context">コンテキスト</param>
    /// <returns></returns>
    public delegate Task ServiceMethodInterceptor(ServerCallContext context);

    /// <summary>
    /// サービスメソッド呼び出しで発生した例外を置き換えるメソッドであることを表します。
    /// </summary>
    /// <param name="context">コンテキスト</param>
    /// <param name="original">発生したした例外</param>
    /// <param name="replaced">代わりにスローさせる例外</param>
    /// <returns>置き換えた場合、true を返します。</returns>
    public delegate bool ServiceMethodExceptionHandler(ServerCallContext context, Exception original, out Exception replaced);

}
