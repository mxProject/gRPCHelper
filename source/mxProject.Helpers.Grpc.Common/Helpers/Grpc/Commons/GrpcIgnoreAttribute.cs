using System;
using System.Collections.Generic;
using System.Text;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// 無視することを表す属性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class GrpcIgnoreAttribute : Attribute
    {

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public GrpcIgnoreAttribute()
        {
        }

    }

}
