using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// Observable 機能を提供します。
    /// </summary>
    public static class GrpcObservable
    {

        #region Observable

        /// <summary>
        /// DuplexStreaming に対する配信オブジェクトを生成します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="disposableCall">配信オブジェクトの dispose 時に呼び出しオブジェクトを dispose するかどうか</param>
        /// <returns>配信オブジェクト</returns>
        public static GrpcDuplexStreamingObservable<TRequest, TResponse> ObserveDuplexStreaming<TRequest, TResponse>(AsyncDuplexStreamingCall<TRequest, TResponse> call, bool disposableCall = true)
        {
            return GrpcDuplexStreamingObservable<TRequest, TResponse>.Observe(call, disposableCall);
        }

        /// <summary>
        /// ServerStreaming に対する配信オブジェクトを生成します。
        /// </summary>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="disposableCall">配信オブジェクトの dispose 時に呼び出しオブジェクトを dispose するかどうか</param>
        /// <returns>配信オブジェクト</returns>
        public static GrpcServerStreamingObservable<TResponse> ObserveServerStreaming<TResponse>(AsyncServerStreamingCall<TResponse> call, bool disposableCall = true)
        {
            return GrpcServerStreamingObservable<TResponse>.Observe(call, disposableCall);
        }

        /// <summary>
        /// DuplexStreaming に対する配信オブジェクトを生成します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="disposableCall">配信オブジェクトの dispose 時に呼び出しオブジェクトを dispose するかどうか</param>
        /// <returns>配信オブジェクト</returns>
        public static GrpcDuplexStreamingObservable<TRequest, TResponse> ToObservable<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, bool disposableCall = true)
        {
            return ObserveDuplexStreaming(call, disposableCall);
        }

        /// <summary>
        /// ServerStreaming に対する配信オブジェクトを生成します。
        /// </summary>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="disposableCall">配信オブジェクトの dispose 時に呼び出しオブジェクトを dispose するかどうか</param>
        /// <returns>配信オブジェクト</returns>
        public static GrpcServerStreamingObservable<TResponse> ToObservable<TResponse>(this AsyncServerStreamingCall<TResponse> call, bool disposableCall = true)
        {
            return ObserveServerStreaming(call, disposableCall);
        }

        #endregion

        #region Observer

        /// <summary>
        /// 監視オブジェクトを生成します。
        /// </summary>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="onResponse">レスポンスを受け取ったときの処理</param>
        /// <param name="onError">例外が発生したときの処理</param>
        /// <param name="onComplete">完了したときの処理</param>
        /// <returns>監視オブジェクト</returns>
        public static IObserver<TResponse> CreateObserver<TResponse>(Action<TResponse> onResponse, Action<Exception> onError = null, Action onComplete = null)
        {
            return new GrpcResponseObserver<TResponse>(onResponse, onError, onComplete);
        }

        #endregion

        /// <summary>
        /// 監視オブジェクト
        /// </summary>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        private sealed class GrpcResponseObserver<TResponse> : IObserver<TResponse>
        {

            /// <summary>
            /// インスタンスを生成します。
            /// </summary>
            /// <param name="onResponse">レスポンスを受け取ったときの処理</param>
            /// <param name="onError">例外が発生したときの処理</param>
            /// <param name="onComplete">完了したときの処理</param>
            public GrpcResponseObserver(Action<TResponse> onResponse, Action<Exception> onError, Action onComplete)
            {
                m_OnNext = onResponse;
                m_OnError = onError;
                m_OnComplete = onComplete;
            }

            private Action<TResponse> m_OnNext;
            private Action<Exception> m_OnError;
            private Action m_OnComplete;

            /// <summary>
            /// 完了したときの処理を行います。
            /// </summary>
            public void OnCompleted()
            {
                if (m_OnComplete != null) { m_OnComplete(); }
            }

            /// <summary>
            /// 例外が発生したときの処理を行います。
            /// </summary>
            /// <param name="error"></param>
            public void OnError(Exception error)
            {
                if (m_OnError != null) { m_OnError(error); }
            }

            /// <summary>
            /// レスポンスを受け取ったときの処理を行います。
            /// </summary>
            /// <param name="value"></param>
            public void OnNext(TResponse value)
            {
                if (m_OnNext != null) { m_OnNext(value); }
            }

        }

    }

}
