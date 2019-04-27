using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// 配信オブジェクトの基本実装。
    /// </summary>
    /// <typeparam name="TResponse">レスポンスの型</typeparam>
    public abstract class GrpcStreamingObservableBase<TResponse> : IObservable<TResponse>, IDisposable
    {

        #region コンストラクタ

        /// <summary>
        /// レスポンスストリームを指定してインスタンスを生成します。
        /// </summary>
        /// <param name="responseStream">レスポンスストリーム</param>
        protected GrpcStreamingObservableBase(IAsyncStreamReader<TResponse> responseStream)
        {
            m_ResponseStream = responseStream;
        }

        #endregion

        #region デストラクタ

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~GrpcStreamingObservableBase()
        {
            Dispose(false);
        }

        #endregion

        #region dispose

        /// <summary>
        /// 使用しているリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 使用しているリソースを解放します。
        /// </summary>
        /// <param name="disposing">dispose メソッドから呼び出されたかどうか</param>
        protected virtual void Dispose(bool disposing)
        {
            ReleaseObservers();
            ReleaseSubscribers();
        }

        #endregion

        #region Stream

        /// <summary>
        /// レスポンスストリーム
        /// </summary>
        private IAsyncStreamReader<TResponse> m_ResponseStream;

        #endregion

        #region 監視

        /// <summary>
        /// 監視オブジェクトを格納するコレクション
        /// </summary>
        private readonly List<IObserver<TResponse>> m_Observers = new List<IObserver<TResponse>>();

        /// <summary>
        /// 監視を開始します。
        /// </summary>
        /// <returns></returns>
        public async Task ObserveAsync()
        {

            try
            {
                while (await m_ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    OnNext(m_ResponseStream.Current);
                }
                OnCompleted();
            }
            catch (Exception ex)
            {
                OnError(ex);
                throw;
            }
            finally
            {
            }

        }

        /// <summary>
        /// 監視オブジェクトを解放します。
        /// </summary>
        private void ReleaseObservers()
        {
            lock (m_Observers)
            {
                m_Observers.Clear();
            }
        }

        #endregion

        #region IObservable<Response> インターフェースの実装

        /// <summary>
        /// 完了したときの処理を行います。
        /// </summary>
        private void OnCompleted()
        {
            if (m_Observers.Count == 0) { return; }
            lock (m_Observers)
            {
                if (m_Observers.Count > 0)
                {
                    m_Observers.ForEach(o => o.OnCompleted());
                    m_Observers.Clear();
                }
            }
        }

        /// <summary>
        /// 例外が発生したときの処理を行います。
        /// </summary>
        /// <param name="error"></param>
        private void OnError(Exception error)
        {
            if (m_Observers.Count == 0) { return; }
            lock (m_Observers)
            {
                if (m_Observers.Count > 0)
                {
                    m_Observers.ForEach(o => o.OnError(error));
                    m_Observers.Clear();
                }
            }
        }

        /// <summary>
        /// レスポンスを受け取ったときの処理を行います。
        /// </summary>
        /// <param name="response"></param>
        private void OnNext(TResponse response)
        {
            if (m_Observers.Count == 0) { return; }
            lock (m_Observers)
            {
                if (m_Observers.Count > 0)
                {
                    m_Observers.ForEach(o => o.OnNext(response));
                }
            }
        }

        #endregion

        #region 購読

        /// <summary>
        /// 購読オブジェクトを格納するコレクション
        /// </summary>
        private readonly List<IDisposable> m_Subscribers = new List<IDisposable>();

        /// <summary>
        /// 指定された監視オブジェクトを登録します。
        /// </summary>
        /// <param name="observer">監視オブジェクト</param>
        /// <returns>購読オブジェクト</returns>
        public IDisposable Subscribe(IObserver<TResponse> observer)
        {

            lock (m_Observers)
            {
                m_Observers.Add(observer);
            }

            // dispose されたら監視オブジェクトを削除する
            Action onDispose = () =>
            {
                lock (m_Observers)
                {
                    m_Observers.Remove(observer);
                }
            };

            IDisposable subscriber = new Subscriber(onDispose);

            lock (m_Subscribers)
            {
                m_Subscribers.Add(subscriber);
            }

            return subscriber;

        }

        /// <summary>
        /// 購読オブジェクトを解放します。
        /// </summary>
        private void ReleaseSubscribers()
        {
            lock (m_Subscribers)
            {
                m_Subscribers.ForEach(o => o.Dispose());
                m_Subscribers.Clear();
            }
        }

        /// <summary>
        /// 購読オブジェクト。
        /// </summary>
        private sealed class Subscriber : IDisposable
        {

            /// <summary>
            /// 解放処理を指定してインスタンスを生成します。
            /// </summary>
            /// <param name="onDispose">解放処理</param>
            internal Subscriber(Action onDispose)
            {
                m_OnDispose = onDispose;
            }

            /// <summary>
            /// デストラクタ
            /// </summary>
            ~Subscriber()
            {
                Dispose(false);
            }

            private Action m_OnDispose;

            /// <summary>
            /// 使用しているリソースを解放します。
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(true);
            }

            /// <summary>
            /// 使用しているリソースを解放します。
            /// </summary>
            /// <param name="disposing"></param>
            private void Dispose(bool disposing)
            {
                m_OnDispose();
            }

        }

        #endregion

    }

}
