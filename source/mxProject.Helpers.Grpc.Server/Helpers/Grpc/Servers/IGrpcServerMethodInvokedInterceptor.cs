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
    /// サーバー側のメソッド呼び出しの割り込みに必要な機能を提供します。
    /// </summary>
    public interface IGrpcServerMethodInvokedInterceptor: IGrpcInterceptor
    {

        /// <summary>
        /// メソッドが実行されたときの処理を行います。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <returns></returns>
        Task OnInvokedAsync(ServerCallContext context);

    }

}
