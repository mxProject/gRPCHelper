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
    /// サーバー呼び出しに対する割込処理属性の基底実装。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public abstract class GrpcServerMethodInvokedInterceptorAttribute : GrpcInterceptorAttribute, IGrpcServerMethodInvokedInterceptor
    {

        #region コンストラクタ

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        protected GrpcServerMethodInvokedInterceptorAttribute() : base()
        {
        }

        /// <summary>
        /// 優先順位・名称と割込処理を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="priority">優先順位</param>
        /// <param name="name">名称</param>
        protected GrpcServerMethodInvokedInterceptorAttribute(int priority, string name) : base(priority, name)
        {
        }

        #endregion

        #region 割込処理

        /// <summary>
        /// メソッドが呼び出されたときの処理を実行します。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <returns></returns>
        Task IGrpcServerMethodInvokedInterceptor.OnInvokedAsync(ServerCallContext context)
        {
            return OnInvokedAsync(context);
        }

        /// <summary>
        /// メソッドが呼び出されたときの処理を実行します。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <returns></returns>
        protected abstract Task OnInvokedAsync(ServerCallContext context);

        #endregion

    }

}
