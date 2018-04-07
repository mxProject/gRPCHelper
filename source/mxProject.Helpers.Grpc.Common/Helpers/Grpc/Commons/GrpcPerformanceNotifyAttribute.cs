using System;
using System.Collections.Generic;
using System.Text;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// パフォーマンス通知に関する属性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class GrpcPerformanceNotifyAttribute : Attribute
    {

        /// <summary>
        /// 通知を有効するかどうかを指定してインスタンスを生成します。
        /// </summary>
        /// <param name="enabled">通知を有効するかどうか</param>
        public GrpcPerformanceNotifyAttribute(bool enabled)
        {
            m_Enebled = enabled;
        }

        /// <summary>
        /// 通知を有効にするかどうかを取得または設定します。
        /// </summary>
        public bool Enabled
        {
            get { return m_Enebled; }
            set { m_Enebled = value; }
        }
        private bool m_Enebled;

    }

}
