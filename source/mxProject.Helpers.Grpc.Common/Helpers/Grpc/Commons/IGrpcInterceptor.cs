using System;
using System.Collections.Generic;
using System.Text;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// 割込処理の基本定義に必要な機能を提供します。
    /// </summary>
    public interface IGrpcInterceptor
    {

        /// <summary>
        /// 名称を取得します。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 優先順位を取得します。
        /// </summary>
        int Priority { get; }

    }

}
