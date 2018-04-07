using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.Clients.Core
{

    /// <summary>
    /// <see cref="GrpcCallInvoker"/> の状態を管理するコンテキスト。
    /// </summary>
    internal sealed class GrpcCallInvokerContext
    {

        /// <summary>
        /// 呼び出しオブジェクトとその状態の組み合わせ。
        /// </summary>
        private static readonly Dictionary<object, GrpcCallState> s_Calls = new Dictionary<object, GrpcCallState>();

        /// <summary>
        /// 指定されたメソッド呼び出しの状態を取得します。
        /// </summary>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="state">状態オブジェクト</param>
        /// <returns>取得できた場合、true を返します。</returns>
        internal static bool TryGetState<TResponse>(AsyncUnaryCall<TResponse> call, out GrpcCallState state)
        {
            return TryGetStateMain(call, out state);
        }

        /// <summary>
        /// 指定されたメソッド呼び出しの状態を取得します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="state">状態オブジェクト</param>
        /// <returns>取得できた場合、true を返します。</returns>
        internal static bool TryGetState<TRequest, TResponse>(AsyncClientStreamingCall<TRequest, TResponse> call, out GrpcCallState state)
        {
            return TryGetStateMain(call, out state);
        }

        /// <summary>
        /// 指定されたメソッド呼び出しの状態を取得します。
        /// </summary>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="state">状態オブジェクト</param>
        /// <returns>取得できた場合、true を返します。</returns>
        internal static bool TryGetState<TResponse>(AsyncServerStreamingCall<TResponse> call, out GrpcCallState state)
        {
            return TryGetStateMain(call, out state);
        }

        /// <summary>
        /// 指定されたメソッド呼び出しの状態を取得します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="state">状態オブジェクト</param>
        /// <returns>取得できた場合、true を返します。</returns>
        internal static bool TryGetState<TRequest, TResponse>(AsyncDuplexStreamingCall<TRequest, TResponse> call, out GrpcCallState state)
        {
            return TryGetStateMain(call, out state);
        }

        /// <summary>
        /// 指定されたメソッド呼び出しの状態を取得します。
        /// </summary>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="state">状態オブジェクト</param>
        /// <returns>取得できた場合、true を返します。</returns>
        internal static bool TryGetStateMain(object call, out GrpcCallState state)
        {
            lock (s_Calls)
            {
                return (s_Calls.TryGetValue(call, out state));
            }
        }

        /// <summary>
        /// 指定されたメソッド呼び出しの状態を追加します。
        /// </summary>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="state">状態オブジェクト</param>
        private static void AddState(object call, GrpcCallState state)
        {
            lock (s_Calls)
            {
                s_Calls[call] = state;
            }
        }

        /// <summary>
        /// 指定されたメソッド呼び出しの状態を削除します。
        /// </summary>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <returns>削除された場合、true を返します。</returns>
        private static bool RemoveState(object call)
        {
            lock (s_Calls)
            {
                bool result = s_Calls.Remove(call);
                // System.Diagnostics.Debug.WriteLine(string.Format("CallInvokerCount : {0}", s_Calls.Count));
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class Releaser : IDisposable
        {

            internal Releaser(Action onDispose)
            {
                m_OnDispose = onDispose;
            }

            private Action m_OnDispose;

            internal IDisposable Target
            {
                get { return m_Target; }
                set { m_Target = value; }
            }
            private IDisposable m_Target;

            public void Dispose()
            {

                if (m_OnDispose != null)
                {
                    m_OnDispose();
                }

                if (m_Target != null)
                {
                    GrpcCallInvokerContext.RemoveState(m_Target);
                    m_Target = null;
                }

                GC.SuppressFinalize(this);

            }

        }

        /// <summary>
        /// 指定されたメソッド呼び出しを登録します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <returns>呼び出しオブジェクト</returns>
        internal static AsyncUnaryCall<TResponse> Regist<TRequest, TResponse>(AsyncUnaryCall<TResponse> call, Method<TRequest, TResponse> method, string host, CallOptions options)
        {

            GrpcCallInvokerContext.Releaser releaser = new GrpcCallInvokerContext.Releaser(delegate ()
            {
                call.Dispose();
            });

            AsyncUnaryCall<TResponse> wrap = new AsyncUnaryCall<TResponse>(
                call.ResponseAsync
                , call.ResponseHeadersAsync
                , call.GetStatus
                , call.GetTrailers
                , releaser.Dispose
                );

            releaser.Target = wrap;

            GrpcCallInvokerContext.AddState(wrap, new GrpcCallState(method, host, options));

            return wrap;

        }

        /// <summary>
        /// 指定されたメソッド呼び出しを登録します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="performanceListener">パフォーマンスリスナー</param>
        /// <returns>呼び出しオブジェクト</returns>
        internal static AsyncServerStreamingCall<TResponse> Regist<TRequest, TResponse>(AsyncServerStreamingCall<TResponse> call, Method<TRequest, TResponse> method, string host, CallOptions options, GrpcClientPerformanceListener performanceListener)
        {

            GrpcCallInvokerContext.Releaser releaser = new GrpcCallInvokerContext.Releaser(delegate ()
            {
                call.Dispose();
            });

            GrpcCallState state = new GrpcCallState(method, host, options);

            AsyncServerStreamingCall<TResponse> wrap = new AsyncServerStreamingCall<TResponse>(
                new ResponseStreamReader<TResponse>(call.ResponseStream, method, host, options, state.OnEndResponse, performanceListener)
                , call.ResponseHeadersAsync
                , call.GetStatus
                , call.GetTrailers
                , releaser.Dispose
                );

            releaser.Target = wrap;

            GrpcCallInvokerContext.AddState(wrap, state);

            return wrap;

        }

        /// <summary>
        /// 指定されたメソッド呼び出しを登録します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="performanceListener">パフォーマンスリスナー</param>
        /// <returns>呼び出しオブジェクト</returns>
        internal static AsyncClientStreamingCall<TRequest, TResponse> Regist<TRequest, TResponse>(AsyncClientStreamingCall<TRequest, TResponse> call, Method<TRequest, TResponse> method, string host, CallOptions options, GrpcClientPerformanceListener performanceListener)
        {

            GrpcCallInvokerContext.Releaser releaser = new GrpcCallInvokerContext.Releaser(delegate ()
            {
                call.Dispose();
            });

            GrpcCallState state = new GrpcCallState(method, host, options);

            AsyncClientStreamingCall<TRequest, TResponse> wrap = new AsyncClientStreamingCall<TRequest, TResponse>(
                new RequestStreamWriter<TRequest>(call.RequestStream, method, host, options, state.OnRequestStreamCompleted, performanceListener)
                , call.ResponseAsync
                , call.ResponseHeadersAsync
                , call.GetStatus
                , call.GetTrailers
                , releaser.Dispose
                );

            releaser.Target = wrap;

            GrpcCallInvokerContext.AddState(wrap, state);

            return wrap;

        }

        /// <summary>
        /// 指定されたメソッド呼び出しを登録します。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型</typeparam>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="performanceListener">パフォーマンスリスナー</param>
        /// <returns>呼び出しオブジェクト</returns>
        internal static AsyncDuplexStreamingCall<TRequest, TResponse> Regist<TRequest, TResponse>(AsyncDuplexStreamingCall<TRequest, TResponse> call, Method<TRequest, TResponse> method, string host, CallOptions options, GrpcClientPerformanceListener performanceListener)
        {

            GrpcCallInvokerContext.Releaser releaser = new GrpcCallInvokerContext.Releaser(delegate ()
            {
                call.Dispose();
            });

            GrpcCallState state = new GrpcCallState(method, host, options);

            AsyncDuplexStreamingCall<TRequest, TResponse> wrap = new AsyncDuplexStreamingCall<TRequest, TResponse>(
                new RequestStreamWriter<TRequest>(call.RequestStream, method, host, options, state.OnRequestStreamCompleted, performanceListener)
                , new ResponseStreamReader<TResponse>(call.ResponseStream, method, host, options, state.OnEndResponse, performanceListener)
                , call.ResponseHeadersAsync
                , call.GetStatus
                , call.GetTrailers
                , releaser.Dispose
                );

            releaser.Target = wrap;

            GrpcCallInvokerContext.AddState(wrap, state);

            return wrap;

        }

    //    /// <summary>
    //    /// ステータス取得処理。
    //    /// </summary>
    //    private sealed class SafetyStatusGetter
    //    {

    //        /// <summary>
    //        /// ステータス取得処理を指定してインスタンスを生成します。
    //        /// </summary>
    //        /// <param name="getter">ステータス取得処理</param>
    //        internal SafetyStatusGetter(Func<Status> getter)
    //        {
    //            m_Getter = getter;
    //        }

    //        private object m_Call;
    //        private Func<Status> m_Getter;
    //        private Status? m_Status;

    //        /// <summary>
    //        /// ステータスを取得します。
    //        /// </summary>
    //        /// <returns></returns>
    //        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoOptimization | System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    //        internal Status GetStatus()
    //        {

    //            try
    //            {

    //                if (m_Status.HasValue) { return m_Status.Value; }

    //                m_Status = m_Getter();

    //                return m_Status.Value;

    //            }
    //            catch (Exception ex)
    //            {
    //                System.Diagnostics.Debug.WriteLine(ex.ToString());
    //                throw;
    //            }

    //        }

    //    }

    //    /// <summary>
    //    /// トレーラー取得処理。
    //    /// </summary>
    //    private sealed class SafetyTrailersGetter
    //    {

    //        /// <summary>
    //        /// トレーラー取得処理を指定してインスタンスを生成します。
    //        /// </summary>
    //        /// <param name="getter">トレーラー取得処理</param>
    //        internal SafetyTrailersGetter(Func<Metadata> getter)
    //        {
    //            m_Getter = getter;
    //        }

    //        private Metadata m_Metadata;
    //        private Func<Metadata> m_Getter;

    //        /// <summary>
    //        /// トレーラーを取得します。
    //        /// </summary>
    //        /// <returns></returns>
    //        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
    //        internal Metadata GetTrailers()
    //        {

    //            if (m_Metadata != null) { return m_Metadata; }

    //            m_Metadata = m_Getter();

    //            return m_Metadata;

    //        }

    //    }

    }

    /// <summary>
    /// メソッド呼び出しの状態オブジェクト。
    /// </summary>
    internal sealed class GrpcCallState
    {

        /// <summary>
        /// メソッド情報を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        internal GrpcCallState(IMethod method, string host, CallOptions options)
        {
            m_Method = method;
            m_Host = host;
            m_Options = options;
        }

        /// <summary>
        /// メソッドを取得します。
        /// </summary>
        internal IMethod Method
        {
            get { return m_Method; }
        }
        private IMethod m_Method;

        /// <summary>
        /// ホストを取得します。
        /// </summary>
        internal string Host
        {
            get { return m_Host; }
        }
        private string m_Host;

        /// <summary>
        /// オプションを取得します。
        /// </summary>
        internal CallOptions Options
        {
            get { return m_Options; }
        }
        private CallOptions m_Options;

        /// <summary>
        /// レスポンスデータがすべて読み取られたときの処理を行います。
        /// </summary>
        internal void OnEndResponse()
        {
            m_IsEndResponse = true;
        }

        /// <summary>
        /// レスポンスデータがすべて読み取られたかどうかを取得します。
        /// </summary>
        internal bool IsEndResponse
        {
            get { return m_IsEndResponse; }
        }
        private bool m_IsEndResponse;

        /// <summary>
        /// リクエストストリームの処理が完了したときの処理を行います。
        /// </summary>
        internal void OnRequestStreamCompleted()
        {
            m_IsRequestStreamCompleted = true;
        }

        /// <summary>
        /// リクエストストリームの処理が完了しているかどうかを取得します。
        /// </summary>
        internal bool IsRequestStreamCompleted
        {
            get { return m_IsRequestStreamCompleted; }
        }
        private bool m_IsRequestStreamCompleted;

    }

}
