using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Grpc.Core;
using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.Servers.Core
{

    /// <summary>
    /// <see cref="IAsyncStreamReader{TRequest}"/> のラッパーオブジェクト。
    /// </summary>
    /// <typeparam name="TRequest">リクエストの型</typeparam>
    internal class RequestStreamReader<TRequest> : IAsyncStreamReader<TRequest>
    {

        /// <summary>
        /// ラップ対象オブジェクトとメソッド情報を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="target">ラップ対象オブジェクト</param>
        /// <param name="context">コンテキスト</param>
        /// <param name="performanceListener">パフォーマンスリスナー</param>
        internal RequestStreamReader(IAsyncStreamReader<TRequest> target, ServerCallContext context, GrpcServerPerformanceListener performanceListener)
        {
            m_Target = target;
            m_Context = context;
            m_PerformanceListener = performanceListener;
        }

        private IAsyncStreamReader<TRequest> m_Target;
        private ServerCallContext m_Context;
        private GrpcServerPerformanceListener m_PerformanceListener;

        /// <summary>
        /// 
        /// </summary>
        TRequest IAsyncEnumerator<TRequest>.Current
        {
            get { return m_Target.Current; }
        }

        /// <summary>
        /// 
        /// </summary>
        void IDisposable.Dispose()
        {
            m_Target.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> IAsyncEnumerator<TRequest>.MoveNext(CancellationToken cancellationToken)
        {

            if (m_PerformanceListener!= null)
            {
                m_PerformanceListener.NotifyRequestReading(m_Context);
            }

            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                return m_Target.MoveNext(cancellationToken);
            }
            finally
            {
                if (m_PerformanceListener != null)
                {
                    m_PerformanceListener.NotifyRequestReaded(m_Context, GrpcPerformanceListener.GetMilliseconds(watch));
                }
            }

        }

    }

}
