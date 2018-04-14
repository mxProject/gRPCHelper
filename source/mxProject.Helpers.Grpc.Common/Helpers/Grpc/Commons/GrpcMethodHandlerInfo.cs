using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// RPC メソッド実装の定義情報。
    /// </summary>
    public class GrpcMethodHandlerInfo
    {

        /// <summary>
        /// 定義情報を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="methodType">メソッドの種類</param>
        /// <param name="requestType">リクエストの型</param>
        /// <param name="responseType">レスポンスの型</param>
        /// <param name="handler">メソッドハンドラ</param>
        public GrpcMethodHandlerInfo(MethodType methodType, Type requestType, Type responseType, MethodInfo handler)
        {
            m_MethodType = methodType;
            m_RequestType = requestType;
            m_ResponseType = responseType;
            m_Handler = handler;
        }

        /// <summary>
        /// メソッドの種類を取得します。
        /// </summary>
        public MethodType MethodType
        {
            get { return m_MethodType; }
        }
        private MethodType m_MethodType;

        /// <summary>
        /// リクエストの型を取得します。
        /// </summary>
        public Type RequestType
        {
            get { return m_RequestType; }
        }
        private Type m_RequestType;

        /// <summary>
        /// レスポンスの型を取得します。
        /// </summary>
        public Type ResponseType
        {
            get { return m_ResponseType; }
        }
        private Type m_ResponseType;

        /// <summary>
        /// メソッドハンドラを取得します。
        /// </summary>
        public MethodInfo Handler
        {
            get { return m_Handler; }
        }
        private MethodInfo m_Handler;

    }

}
