using System;
using System.Collections.Generic;
using System.Text;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// シリアライズ処理で発生した例外。
    /// </summary>
    [Serializable]
    public sealed class GrpcSerializerException : Exception
    {

        /// <summary>
        /// メソッド呼び出し情報を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="innerException">内部例外</param>
        /// <param name="serviceName">サービス名</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="objectTypeName">シリアライズ対象オブジェクトの型名</param>
        public GrpcSerializerException(string message, Exception innerException, string serviceName, string methodName, string objectTypeName) : base(message, innerException)
        {
            m_ServiceName = serviceName;
            m_MethodName = methodName;
            m_ObjectTypeName = objectTypeName;
        }

        /// <summary>
        /// サービス名を取得します。
        /// </summary>
        public string ServiceName
        {
            get { return m_ServiceName; }
        }
        private string m_ServiceName;

        /// <summary>
        /// メソッド名を取得します。
        /// </summary>
        public string MethodName
        {
            get { return m_MethodName; }
        }
        private string m_MethodName;

        /// <summary>
        /// シリアライズ対象オブジェクトの型名を取得します。
        /// </summary>
        public string ObjectTypeName
        {
            get { return m_ObjectTypeName; }
        }
        private string m_ObjectTypeName;

    }

}
