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
    /// <see cref="IClientStreamWriter{TRequest}"/> のラッパーオブジェクト。
    /// </summary>
    /// <typeparam name="TRequest">リクエストの型</typeparam>
    internal class RequestStreamWriter<TRequest> : IClientStreamWriter<TRequest>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="method"></param>
        /// <param name="host"></param>
        /// <param name="options"></param>
        /// <param name="onCompleted"></param>
        /// <param name="performanceListener"></param>
        internal RequestStreamWriter(IClientStreamWriter<TRequest> target, IMethod method, string host, CallOptions options, Action onCompleted, GrpcClientPerformanceListener performanceListener)
        {
            m_Target = target;
            m_Method = method;
            m_Host = host;
            m_Options = options;
            m_OnCompleted = onCompleted;
            m_PerformanceListener = performanceListener;
        }

        private IClientStreamWriter<TRequest> m_Target;
        private IMethod m_Method;
        private string m_Host;
        private CallOptions m_Options;
        private Action m_OnCompleted;
        private GrpcClientPerformanceListener m_PerformanceListener;

        /// <summary>
        /// 
        /// </summary>
        WriteOptions IAsyncStreamWriter<TRequest>.WriteOptions
        {
            get { return m_Target.WriteOptions; }
            set { m_Target.WriteOptions = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        async Task IClientStreamWriter<TRequest>.CompleteAsync()
        {

            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                await m_Target.CompleteAsync().ConfigureAwait(false);
            }
            finally
            {
                GrpcPerformanceListener.GetMilliseconds(watch);
            }

            if (m_OnCompleted != null) { m_OnCompleted(); }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        async Task IAsyncStreamWriter<TRequest>.WriteAsync(TRequest message)
        {

            if (m_PerformanceListener!= null)
            {
                m_PerformanceListener.NotifyRequestWriting(m_Method, m_Host, m_Options);
            }

            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                await m_Target.WriteAsync(message).ConfigureAwait(false);
            }
            finally
            {
                if (m_PerformanceListener != null)
                {
                    m_PerformanceListener.NotifyRequestWrote(m_Method, m_Host, m_Options, GrpcPerformanceListener.GetMilliseconds(watch));
                }
            }

        }

    }

}
