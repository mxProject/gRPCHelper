using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Grpc.Core;
using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.Clients.Core
{

    /// <summary>
    /// <see cref="IAsyncStreamReader{TResponse}"/> のラッパーオブジェクト。
    /// </summary>
    /// <typeparam name="TResponse">レスポンスの型</typeparam>
    internal class ResponseStreamReader<TResponse> : IAsyncStreamReader<TResponse>
    {

        /// <summary>
        /// ラップ対象オブジェクトとメソッド情報を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="target">ラップ対象オブジェクト</param>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="onEndResponse"></param>
        /// <param name="performanceListener">パフォーマンスリスナー</param>
        internal ResponseStreamReader(IAsyncStreamReader<TResponse> target, IMethod method, string host, CallOptions options, Action onEndResponse, GrpcClientPerformanceListener performanceListener)
        {
            m_Target = target;
            m_Method = method;
            m_Host = host;
            m_Options = options;
            m_OnEndResponse = onEndResponse;
            m_PerformanceListener = performanceListener;
        }

        private IAsyncStreamReader<TResponse> m_Target;
        private IMethod m_Method;
        private string m_Host;
        private CallOptions m_Options;
        private Action m_OnEndResponse;
        private GrpcClientPerformanceListener m_PerformanceListener;

        /// <summary>
        /// 
        /// </summary>
        TResponse IAsyncEnumerator<TResponse>.Current
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
        async Task<bool> IAsyncEnumerator<TResponse>.MoveNext(CancellationToken cancellationToken)
        {

            if (m_PerformanceListener != null)
            {
                m_PerformanceListener.NotifyResponseReading(m_Method, m_Host, m_Options);
            }

            Stopwatch watch = Stopwatch.StartNew();

            bool result;

            try
            {
                result = await m_Target.MoveNext(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (m_PerformanceListener != null)
                {
                    m_PerformanceListener.NotifyResponseReaded(m_Method, m_Host, m_Options, GrpcPerformanceListener.GetMilliseconds(watch));
                }
            }

            if (!result && m_OnEndResponse != null) { m_OnEndResponse(); }

            return result;

        }

    }

}
