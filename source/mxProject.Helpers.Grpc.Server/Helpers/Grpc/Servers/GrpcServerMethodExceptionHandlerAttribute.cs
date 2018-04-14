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
    /// サーバー呼び出しに対する例外処理属性の基底実装。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public abstract class GrpcServerMethodExceptionHandlerAttribute : GrpcInterceptorAttribute, IGrpcServerMethodExceptionHandler
    {

        #region コンストラクタ

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        protected GrpcServerMethodExceptionHandlerAttribute() : base()
        {
        }

        /// <summary>
        /// 優先順位と名称を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="priority">優先順位</param>
        /// <param name="name">名称</param>
        protected GrpcServerMethodExceptionHandlerAttribute(int priority, string name) : base(priority, name)
        {
        }

        #endregion

        #region 例外処理

        /// <summary>
        /// 発生した例外を置き換えます。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="original">発生したした例外</param>
        /// <param name="relpaced">代わりにスローさせる例外</param>
        /// <returns>置き換えた場合、true を返します。</returns>
        bool IGrpcServerMethodExceptionHandler.RelpaceException(ServerCallContext context, Exception original, out Exception relpaced)
        {
            return RelpaceException(context, original, out relpaced);
        }

        /// <summary>
        /// 発生した例外を置き換えます。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="original">発生したした例外</param>
        /// <param name="replaced">代わりにスローさせる例外</param>
        /// <returns>置き換えた場合、true を返します。</returns>
        protected abstract bool RelpaceException(ServerCallContext context, Exception original, out Exception replaced);

        #endregion

    }

}
