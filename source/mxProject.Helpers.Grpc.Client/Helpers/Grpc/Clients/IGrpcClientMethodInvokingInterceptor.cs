using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// クライアントからのメソッド呼び出しの割り込みに必要な機能を提供します。
    /// </summary>
    public interface IGrpcClientMethodInvokingInterceptor : IGrpcInterceptor
    {

        /// <summary>
        /// メソッドが実行されるときの処理を行います。
        /// </summary>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        void OnInvoking(IMethod method, string host, CallOptions options);

    }

}
