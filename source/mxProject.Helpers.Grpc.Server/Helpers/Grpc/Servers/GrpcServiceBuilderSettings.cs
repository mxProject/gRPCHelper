using System;
using System.Collections.Generic;
using System.Text;

using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.Servers
{

    /// <summary>
    /// サービスビルダーの動作設定。
    /// </summary>
    public sealed class GrpcServiceBuilderSettings
    {

        #region マーシャラー

        /// <summary>
        /// マーシャラーの生成処理を取得または設定します。
        /// </summary>
        public IGrpcMarshallerFactory MarshallerFactory
        {
            get { return m_MarshallerFactory; }
            set { m_MarshallerFactory = value; }
        }
        private IGrpcMarshallerFactory m_MarshallerFactory;

        /// <summary>
        /// マーシャラーの生成処理を取得します。
        /// </summary>
        /// <returns></returns>
        internal IGrpcMarshallerFactory GetMarshallerFactoryOrDefault()
        {
            if ( m_MarshallerFactory != null) { return m_MarshallerFactory; }
            return GrpcMarshallerFactory.DefaultInstance;
        }

        #endregion

        #region 割り込み

        /// <summary>
        /// メソッド呼び出し前処理を格納しているコレクションを取得します。
        /// </summary>
        public IList<IGrpcServerMethodInvokingInterceptor> InvokingInterceptors
        {
            get { return m_InvokingInterceptors; }
        }
        private List<IGrpcServerMethodInvokingInterceptor> m_InvokingInterceptors = new List<IGrpcServerMethodInvokingInterceptor>();

        /// <summary>
        /// メソッド呼び出し後処理を格納しているコレクションを取得します。
        /// </summary>
        public IList<IGrpcServerMethodInvokedInterceptor> InvokedInterceptors
        {
            get { return m_InvokedInterceptors; }
        }
        private List<IGrpcServerMethodInvokedInterceptor> m_InvokedInterceptors = new List<IGrpcServerMethodInvokedInterceptor>();

        /// <summary>
        /// 例外処理を格納しているコレクションを取得します。
        /// </summary>
        public IList<IGrpcServerMethodExceptionHandler> ExceptionHandlers
        {
            get { return m_ExceptionHandlers; }
        }
        private List<IGrpcServerMethodExceptionHandler> m_ExceptionHandlers = new List<IGrpcServerMethodExceptionHandler>();

        /// <summary>
        /// 割込処理を並べ替えます。
        /// </summary>
        internal void SortInterceptors()
        {
            m_InvokingInterceptors.Sort(CompareInterceptor<IGrpcServerMethodInvokingInterceptor>);
            m_InvokedInterceptors.Sort(CompareInterceptor<IGrpcServerMethodInvokedInterceptor>);
            m_ExceptionHandlers.Sort(CompareInterceptor<IGrpcServerMethodExceptionHandler>);
        }

        /// <summary>
        /// 指定された割込処理の大小比較を行います。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int CompareInterceptor<T>(T a, T b) where T : IGrpcInterceptor
        {
            if (a != null && b != null)
            {
                return a.Priority.CompareTo(b.Priority);
            }
            else if (a != null)
            {
                return 1;
            }
            else if (b != null)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region クローン

        /// <summary>
        /// ディープクローンを生成します。
        /// </summary>
        /// <returns></returns>
        internal GrpcServiceBuilderSettings CreateDeepClone()
        {

            GrpcServiceBuilderSettings clone = this.MemberwiseClone() as GrpcServiceBuilderSettings;

            clone.m_InvokingInterceptors = new List<IGrpcServerMethodInvokingInterceptor>();
            clone.m_InvokingInterceptors.AddRange(m_InvokingInterceptors);

            clone.m_InvokedInterceptors = new List<IGrpcServerMethodInvokedInterceptor>();
            clone.m_InvokedInterceptors.AddRange(m_InvokedInterceptors);

            clone.m_ExceptionHandlers = new List<IGrpcServerMethodExceptionHandler>();
            clone.m_ExceptionHandlers.AddRange(m_ExceptionHandlers);

            return clone;

        }

        #endregion

        #region 通知

        /// <summary>
        /// パフォーマンスリスナーを取得または設定します。
        /// </summary>
        public GrpcServerPerformanceListener PerformanceListener
        {
            get { return m_PerformanceListener; }
            set { m_PerformanceListener = value; }
        }
        private GrpcServerPerformanceListener m_PerformanceListener;

        #endregion

    }

}
