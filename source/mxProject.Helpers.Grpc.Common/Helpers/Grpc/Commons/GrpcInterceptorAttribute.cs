using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// 割込処理属性の基底実装。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public abstract class GrpcInterceptorAttribute : Attribute, IGrpcInterceptor
    {

        #region コンストラクタ

        /// <summary>
        /// 割込処理を指定してインスタンスを生成します。
        /// </summary>
        protected GrpcInterceptorAttribute() : this(int.MaxValue, null)
        {
        }

        /// <summary>
        /// 優先順位と名称を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="priority">優先順位</param>
        /// <param name="name">名称</param>
        protected GrpcInterceptorAttribute(int priority, string name)
        {
            m_Name = name;
            m_Priority = priority;
        }

        #endregion

        #region 属性値

        /// <summary>
        /// 優先順位を取得または設定します。
        /// </summary>
        public int Priority
        {
            get { return m_Priority; }
            set { m_Priority = value; }
        }
        private int m_Priority = int.MaxValue;

        /// <summary>
        /// 名称を取得または設定します。
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        private string m_Name;

        #endregion

    }

}
