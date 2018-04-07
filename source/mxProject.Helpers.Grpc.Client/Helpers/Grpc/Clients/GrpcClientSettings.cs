using System;
using System.Collections.Generic;
using System.Text;

using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// クライアントの動作設定。
    /// </summary>
    public sealed class GrpcClientSettings
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
        public IList<IGrpcClientMethodInvokingInterceptor> InvokingInterceptors
        {
            get { return m_InvokingInterceptors; }
        }
        private List<IGrpcClientMethodInvokingInterceptor> m_InvokingInterceptors = new List<IGrpcClientMethodInvokingInterceptor>();

        /// <summary>
        /// メソッド呼び出し後処理を格納しているコレクションを取得します。
        /// </summary>
        public IList<IGrpcClientMethodInvokedInterceptor> InvokedInterceptors
        {
            get { return m_InvokedInterceptors; }
        }
        private List<IGrpcClientMethodInvokedInterceptor> m_InvokedInterceptors = new List<IGrpcClientMethodInvokedInterceptor>();

        /// <summary>
        /// 例外処理を格納しているコレクションを取得します。
        /// </summary>
        public IList<IGrpcClientMethodExceptionHandler> ExceptionHandlers
        {
            get { return m_ExceptionHandlers; }
        }
        private List<IGrpcClientMethodExceptionHandler> m_ExceptionHandlers = new List<IGrpcClientMethodExceptionHandler>();

        /// <summary>
        /// 割込処理を並べ替えます。
        /// </summary>
        internal void SortInterceptors()
        {
            m_InvokingInterceptors.Sort(CompareInterceptor<IGrpcClientMethodInvokingInterceptor>);
            m_InvokedInterceptors.Sort(CompareInterceptor<IGrpcClientMethodInvokedInterceptor>);
            m_ExceptionHandlers.Sort(CompareInterceptor<IGrpcClientMethodExceptionHandler>);
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

        #region 通知

        /// <summary>
        /// パフォーマンスリスナーを取得または設定します。
        /// </summary>
        public GrpcClientPerformanceListener PerformanceListener
        {
            get { return m_PerformanceListener; }
            set { m_PerformanceListener = value; }
        }
        private GrpcClientPerformanceListener m_PerformanceListener;

        #endregion

        #region クローン

        /// <summary>
        /// ディープクローンを生成します。
        /// </summary>
        /// <returns></returns>
        internal GrpcClientSettings CreateDeepClone()
        {

            GrpcClientSettings clone = this.MemberwiseClone() as GrpcClientSettings;

            clone.m_InvokingInterceptors = new List<IGrpcClientMethodInvokingInterceptor>();
            clone.m_InvokingInterceptors.AddRange(m_InvokingInterceptors);

            clone.m_InvokedInterceptors = new List<IGrpcClientMethodInvokedInterceptor>();
            clone.m_InvokedInterceptors.AddRange(m_InvokedInterceptors);

            clone.m_ExceptionHandlers = new List<IGrpcClientMethodExceptionHandler>();
            clone.m_ExceptionHandlers.AddRange(m_ExceptionHandlers);

            return clone;

        }

        #endregion

    }

}
