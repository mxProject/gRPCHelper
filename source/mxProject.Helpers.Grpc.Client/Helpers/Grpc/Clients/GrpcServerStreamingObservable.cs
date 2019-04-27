using System;
using System.Collections.Generic;
using System.Text;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// ServerStreaming に対する配信オブジェクト。
    /// </summary>
    /// <typeparam name="TResponse">レスポンスの型</typeparam>
    public class GrpcServerStreamingObservable<TResponse> : GrpcStreamingObservableBase<TResponse>
    {

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="disposableCall">配信オブジェクトの dispose 時に呼び出しオブジェクトを dispose するかどうか</param>
        private GrpcServerStreamingObservable(AsyncServerStreamingCall<TResponse> call, bool disposableCall) : base(call.ResponseStream)
        {
            m_Call = call;
            m_DisposableCall = disposableCall;
        }

        /// <summary>
        /// ServerStreaming に対する配信オブジェクトを生成します。
        /// </summary>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="disposableCall">配信オブジェクトの dispose 時に呼び出しオブジェクトを dispose するかどうか</param>
        public static GrpcServerStreamingObservable<TResponse> Observe(AsyncServerStreamingCall<TResponse> call, bool disposableCall)
        {

            GrpcServerStreamingObservable<TResponse> observable = new GrpcServerStreamingObservable<TResponse>(call, disposableCall);

            return observable;

        }

        private AsyncServerStreamingCall<TResponse> m_Call;
        private bool m_DisposableCall;

        /// <summary>
        /// 使用しているリソースを解放します。
        /// </summary>
        /// <param name="disposing">dispose メソッドから呼び出されたかどうか</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (m_DisposableCall) { m_Call.Dispose(); }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

    }

}
