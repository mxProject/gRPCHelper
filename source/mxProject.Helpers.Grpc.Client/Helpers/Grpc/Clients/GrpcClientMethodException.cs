using System;
using System.Collections.Generic;
using System.Text;

using Grpc.Core;

using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// クライアント側のメソッド呼び出しで発生した例外。
    /// </summary>
    public sealed class GrpcClientMethodException : Exception
    {

        /// <summary>
        /// メソッド呼び出し情報を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="innerException">内部例外</param>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        public GrpcClientMethodException(string message, Exception innerException, IMethod method, string host, CallOptions options) : this(message, innerException, method, host, options, null)
        {
        }

        /// <summary>
        /// メソッド呼び出し情報を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="innerException">内部例外</param>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="interceptor">失敗した割込処理</param>
        public GrpcClientMethodException(string message, Exception innerException, IMethod method, string host, CallOptions options, IGrpcInterceptor interceptor) : base(message, innerException)
        {
            m_Method = method;
            m_Host = host;
            m_Options = options;
            m_Interceptor = interceptor;
        }

        /// <summary>
        /// メソッドを取得します。
        /// </summary>
        public IMethod Method
        {
            get { return m_Method; }
        }
        private IMethod m_Method;

        /// <summary>
        /// ホストを取得します。
        /// </summary>
        public string Host
        {
            get { return m_Host; }
        }
        private string m_Host;

        /// <summary>
        /// オプションを取得します。
        /// </summary>
        public CallOptions Options
        {
            get { return m_Options; }
        }
        private CallOptions m_Options;

        /// <summary>
        /// 失敗した割込処理を取得します。
        /// </summary>
        public IGrpcInterceptor FailedInterceptor
        {
            get { return m_Interceptor; }
        }
        private IGrpcInterceptor m_Interceptor;

    }

}
