using System;
using System.Collections.Generic;
using System.Text;

using Grpc.Core;
using Grpc.Core.Logging;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// 例外リスナー。
    /// </summary>
    public sealed class GrpcExceptionListener
    {

        /// <summary>
        /// 
        /// </summary>
        private GrpcExceptionListener() { }

        /// <summary>
        /// サーバー側のメソッド呼び出し処理で例外が発生したときの処理を行います。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="ex">例外</param>
        public static void NotifyCatchServerException(ServerCallContext context, Exception ex)
        {

            string message;

            if (IsCancel(ex))
            {
                message = string.Format("キャンセルされました。");
            }
            else
            {
                message = string.Format("【ServerException】{0} {1}:{2}", context.Method, ex.GetType().Name, ex.Message);
            }

            //s_Logger.Error(message);

            try
            {
                NotifyServerException h = CatchServerException;
                if (h != null) { h(context, ex); }               
            }
            catch
            {
            }

        }

        /// <summary>
        /// サーバー側のメソッド呼び出し処理で例外をキャッチしたときに発生します。
        /// </summary>
        public static event NotifyServerException CatchServerException;

        /// <summary>
        /// キャンセルされたかどうかを取得します。
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static bool IsCancel(Exception ex)
        {
            RpcException rpc = ex as RpcException;
            if (rpc == null) { return false; }
            return rpc.IsCancel();
        }

        /// <summary>
        /// クライアント側のメソッド呼び出し処理で例外が発生したときの処理を行います。
        /// </summary>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="ex">例外</param>
        public static void NotifyCatchClientException(IMethod method, string host, CallOptions options, Exception ex)
        {

            string message = string.Format("【ClientException】{0} {1}:{2}", method.FullName, ex.GetType().Name, ex.Message);
            //s_Logger.Error(message);

            try
            {
                NotifyClientException h = CatchClientException;
                if (h != null) { h(method, host, options, ex); }
            }
            catch
            {
            }

        }

        /// <summary>
        /// クライアント側のメソッド呼び出し処理で例外をキャッチしたときに発生します。
        /// </summary>
        public static event NotifyClientException CatchClientException;

        /// <summary>
        /// シリアライズ処理で例外が発生したときの処理を行います。
        /// </summary>
        /// <param name="serviceName">サービス名</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="objectType">シリアライズ対象オブジェクトの型</param>
        /// <param name="ex">例外</param>
        public static void NotifyCatchSerializerException(string serviceName, string methodName, Type objectType, Exception ex)
        {

            string message = string.Format("【SerializerException】{0} {1} {2} {3}:{4}", serviceName, methodName, objectType.FullName, ex.GetType().Name, ex.Message);
            //s_Logger.Error(message);

            try
            {
                NotifySerializerException h = CatchSerializerException;
                if (h != null) { h(serviceName, methodName, objectType, ex); }
            }
            catch
            {
            }

        }

        /// <summary>
        /// シリアライズ処理で例外をキャッチしたときに発生します。
        /// </summary>
        public static event NotifySerializerException CatchSerializerException;

    }

}
