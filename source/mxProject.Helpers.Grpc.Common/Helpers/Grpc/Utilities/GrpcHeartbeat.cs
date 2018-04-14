using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Utilities
{

    /// <summary>
    /// ハートビートの送信を行います。
    /// </summary>
    public class GrpcHeartbeat
    {

        #region サーバー

        /// <summary>
        /// 既定のサービスメソッド名でサービス定義を生成します。
        /// </summary>
        /// <returns>サービス定義</returns>
        public static ServerServiceDefinition BuildService()
        {
            return BuildService(DefaultServiceName, DefaultMethodName);
        }

        /// <summary>
        /// サービスメソッド名を指定してサービス定義を生成します。
        /// </summary>
        /// <param name="serviceName">サービス名</param>
        /// <param name="methodName">メソッド名</param>
        /// <returns>サービス定義</returns>
        public static ServerServiceDefinition BuildService(string serviceName, string methodName)
        {

            ServerServiceDefinition.Builder builder = ServerServiceDefinition.CreateBuilder();

            builder.AddMethod<byte[], byte[]>(CreateMethod(serviceName, methodName), CreateHandler());
            return builder.Build();

        }

        /// <summary>
        /// ハートビート送信メソッドのメソッド実装を生成します。
        /// </summary>
        /// <returns></returns>
        private static DuplexStreamingServerMethod<byte[], byte[]> CreateHandler()
        {

            return async delegate (IAsyncStreamReader<byte[]> requestStream, IServerStreamWriter<byte[]> responseStream, ServerCallContext context)
            {

                bool canceled = false;

                context.CancellationToken.Register(delegate ()
                {
                    canceled = true;
                }
                );

                string peer = context.Peer;

                try
                {

                    while (!canceled && await requestStream.MoveNext().ConfigureAwait(false))
                    {

                        if (context.CancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        await responseStream.WriteAsync(new byte[] { 1 }).ConfigureAwait(false);

                        // Console.WriteLine(string.Format("[Heartbeat] Request from {0}.", peer));

                    }

                }
                finally
                {
                    // Console.WriteLine(string.Format("[Heartbeat] End of {0}.", peer));
                }

            };

        }

        #endregion

        #region クライアント

        /// <summary>
        /// ハートビート送信を行うためのクライアントオブジェクトを生成します。
        /// </summary>
        /// <param name="channel">チャネル</param>
        /// <returns>ハートビート送信オブジェクト</returns>
        public static HeartbeatObject CreateClientObject(Channel channel)
        {

            return new HeartbeatObject(channel);

        }

        /// <summary>
        /// ハートビート送信を行うためのクライアントオブジェクトを生成します。
        /// </summary>
        /// <param name="channel">チャネル</param>
        /// <param name="serviceName">サービス名</param>
        /// <param name="methodName">メソッド名</param>
        /// <returns>ハートビート送信オブジェクト</returns>
        public static HeartbeatObject CreateClientObject(Channel channel, string serviceName, string methodName)
        {

            return new HeartbeatObject(channel, serviceName, methodName);

        }

        /// <summary>
        /// ハートビート送信オブジェクト。
        /// </summary>
        public sealed class HeartbeatObject : IDisposable
        {

            /// <summary>
            /// チャネルを指定してインスタンスを生成します。
            /// </summary>
            /// <param name="channel">チャネル</param>
            internal HeartbeatObject(Channel channel) : this(channel, DefaultServiceName, DefaultMethodName)
            {
            }

            /// <summary>
            /// チャネルとサービスメソッド名を指定してインスタンスを生成します。
            /// </summary>
            /// <param name="channel">チャネル</param>
            /// <param name="serviceName">サービス名</param>
            /// <param name="methodName">メソッド名</param>
            internal HeartbeatObject(Channel channel, string serviceName, string methodName)
            {

                m_Channel = channel;
                m_Invoker = new DefaultCallInvoker(channel);
                m_Method = CreateMethod(serviceName, methodName);

            }

            /// <summary>
            /// 使用しているリソースを解放します。
            /// </summary>
            public void Dispose()
            {
                Stop();
            }

            /// <summary>
            /// チャネルを取得します。
            /// </summary>
            public Channel Channel
            {
                get { return m_Channel; }
            }
            private readonly Channel m_Channel;

            /// <summary>
            /// 呼び出しオブジェクト
            /// </summary>
            private readonly CallInvoker m_Invoker;

            /// <summary>
            /// メソッド定義
            /// </summary>
            private readonly Method<byte[], byte[]> m_Method;

            /// <summary>
            /// ハートビートの送信を行っているかどうかを取得します。
            /// </summary>
            public bool IsActive
            {
                get { return m_IsActive; }
            }
            private bool m_IsActive;

            private CancellationTokenSource m_Cancellation;

            /// <summary>
            /// ハートビートの送信を開始します。
            /// </summary>
            /// <param name="delayMilliseconds">初回送信までの遅延時間（ミリ秒）</param>
            /// <param name="intervalMilliseconds">送信間隔（ミリ秒）</param>
            /// <returns></returns>
            public async Task Start(int delayMilliseconds, int intervalMilliseconds)
            {

                if (m_IsActive) { return; }

                await Task.Delay(delayMilliseconds);

                CancellationTokenSource cancellation = new CancellationTokenSource();

                m_Cancellation = cancellation;

                m_IsActive = true;

                CallOptions callOptions = CreateCallOption(cancellation);

                try
                {

                    using (AsyncDuplexStreamingCall<byte[], byte[]> call = m_Invoker.AsyncDuplexStreamingCall(m_Method, null, callOptions))
                    {

                        while (true)
                        {

                            await call.RequestStream.WriteAsync(new byte[] { 0 });

                            while (await call.ResponseStream.MoveNext())
                            {
                                await Task.Delay(intervalMilliseconds);
                                await call.RequestStream.WriteAsync(new byte[] { 0 });
                            }

                        }

                    }

                }
                catch (TaskCanceledException)
                {
                }
                finally
                {
                    m_IsActive = false;
                }

            }

            /// <summary>
            /// ハートビートの送信を停止します。
            /// </summary>
            public void Stop()
            {

                if (!m_IsActive) { return; }

                m_Cancellation.Cancel();

                m_IsActive = false;

            }

            /// <summary>
            /// 呼び出しオプションを生成します。
            /// </summary>
            /// <returns></returns>
            private CallOptions CreateCallOption(CancellationTokenSource cancellation)
            {

                return new CallOptions(null, null, cancellation.Token);

            }

        }

        #endregion

        #region メソッド定義

        /// <summary>
        /// 既定のサービス名。
        /// </summary>
        public static readonly string DefaultServiceName = "GrpcHeartbeat";

        /// <summary>
        /// 既定のメソッド名。
        /// </summary>
        public static readonly string DefaultMethodName = "Heartbeat";

        /// <summary>
        /// 既定のサービスメソッド名でメソッド定義を生成します。
        /// </summary>
        /// <returns>メソッド定義</returns>
        private static Method<byte[], byte[]> CreateMethod()
        {
            return CreateMethod(DefaultServiceName, DefaultMethodName);
        }

        /// <summary>
        /// メソッド定義を生成します。
        /// </summary>
        /// <param name="serviceName">サービス名</param>
        /// <param name="methodName">メソッド名</param>
        /// <returns>メソッド定義</returns>
        private static Method<byte[], byte[]> CreateMethod(string serviceName, string methodName)
        {

            Marshaller<byte[]> marshaller = CreateMarshaller();

            return new Method<byte[], byte[]>(
                MethodType.DuplexStreaming
                , serviceName
                , methodName
                , marshaller
                , marshaller
                );

        }

        /// <summary>
        /// マーシャラーを生成します。
        /// </summary>
        /// <returns>マーシャラー</returns>
        private static Marshaller<byte[]> CreateMarshaller()
        {
            return new Marshaller<byte[]>(
                delegate (byte[] obj) { return obj; }
                , delegate (byte[] obj) { return obj; }
                );
        }

        #endregion

    }

}
