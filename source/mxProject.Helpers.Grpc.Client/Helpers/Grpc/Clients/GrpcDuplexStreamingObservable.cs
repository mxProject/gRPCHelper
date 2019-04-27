using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// DuplexStreaming に対する配信オブジェクト。
    /// </summary>
    /// <typeparam name="TRequest">リクエストの型</typeparam>
    /// <typeparam name="TResponse">レスポンスの型</typeparam>
    public class GrpcDuplexStreamingObservable<TRequest, TResponse> : GrpcStreamingObservableBase<TResponse>
    {

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="disposableCall">配信オブジェクトの dispose 時に呼び出しオブジェクトを dispose するかどうか</param>
        private GrpcDuplexStreamingObservable(AsyncDuplexStreamingCall<TRequest, TResponse> call, bool disposableCall) : base(call.ResponseStream)
        {
            m_Call = call;
            m_DisposableCall = disposableCall;
        }

        /// <summary>
        /// DuplexStreaming に対する配信オブジェクトを生成します。
        /// </summary>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="disposableCall">配信オブジェクトの dispose 時に呼び出しオブジェクトを dispose するかどうか</param>
        public static GrpcDuplexStreamingObservable<TRequest, TResponse> Observe(AsyncDuplexStreamingCall<TRequest, TResponse> call, bool disposableCall)
        {

            GrpcDuplexStreamingObservable<TRequest, TResponse> observable = new GrpcDuplexStreamingObservable<TRequest, TResponse>(call, disposableCall);

            return observable;

        }

        private AsyncDuplexStreamingCall<TRequest, TResponse> m_Call;
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

        /// <summary>
        /// 指定されたリクエストを書き込みます。
        /// </summary>
        /// <param name="request">リクエスト</param>
        /// <returns></returns>
        public async Task WriteRequestAsync(TRequest request)
        {
            await m_Call.RequestStream.WriteAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// リクエストの完了を通知します。
        /// </summary>
        /// <returns></returns>
        public async Task CompleteRequestAsync()
        {
            await m_Call.RequestStream.CompleteAsync().ConfigureAwait(false);
        }

    }

}
