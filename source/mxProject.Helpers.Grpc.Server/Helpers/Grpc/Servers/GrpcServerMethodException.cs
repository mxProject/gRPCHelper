using System;
using System.Collections.Generic;
using System.Text;

using Grpc.Core;

using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.Servers
{

    /// <summary>
    /// サーバー側のメソッド呼び出しで発生した例外。
    /// </summary>
    public sealed class GrpcServerMethodException : Exception
    {

        /// <summary>
        /// メソッド呼び出し情報を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="innerException">内部例外</param>
        /// <param name="context">コンテキスト</param>
        public GrpcServerMethodException(string message, Exception innerException, ServerCallContext context) : this(message, innerException, context, null)
        {
        }

        /// <summary>
        /// メソッド呼び出し情報を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="innerException">内部例外</param>
        /// <param name="context">コンテキスト</param>
        /// <param name="interceptor">失敗した割込処理</param>
        public GrpcServerMethodException(string message, Exception innerException, ServerCallContext context, IGrpcInterceptor interceptor) : base(message, innerException)
        {
            m_Context = context;
            m_Interceptor = interceptor;
        }

        /// <summary>
        /// コンテキストを取得します。
        /// </summary>
        public ServerCallContext Context
        {
            get { return m_Context; }
        }
        private ServerCallContext m_Context;

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
