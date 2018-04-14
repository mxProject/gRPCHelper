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
    /// <see cref="IServerStreamWriter{TResponse}"/> のラッパーオブジェクト。
    /// </summary>
    /// <typeparam name="TResponse">レスポンスの型</typeparam>
    internal class ResponseStreamWriter<TResponse> : IServerStreamWriter<TResponse>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="context"></param>
        /// <param name="performanceListener"></param>
        internal ResponseStreamWriter(IServerStreamWriter<TResponse> target, ServerCallContext context, GrpcServerPerformanceListener performanceListener)
        {
            m_Target = target;
            m_Context = context;
            m_PerformanceListener = performanceListener;
        }

        private IServerStreamWriter<TResponse> m_Target;
        private ServerCallContext m_Context;
        private GrpcServerPerformanceListener m_PerformanceListener;

        /// <summary>
        /// 
        /// </summary>
        WriteOptions IAsyncStreamWriter<TResponse>.WriteOptions
        {
            get { return m_Target.WriteOptions; }
            set { m_Target.WriteOptions = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task IAsyncStreamWriter<TResponse>.WriteAsync(TResponse message)
        {

            if (m_PerformanceListener != null)
            {
                m_PerformanceListener.NotifyResponseWriting(m_Context);
            }

            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                return m_Target.WriteAsync(message);
            }
            finally
            {
                if (m_PerformanceListener != null)
                {
                    m_PerformanceListener.NotifyResponseWrote(m_Context, GrpcPerformanceListener.GetMilliseconds(watch));
                }
            }

        }


    }

}
