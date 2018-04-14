using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
using Grpc.Core.Logging;
using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// パフォーマンスリスナー。
    /// </summary>
    public class GrpcClientPerformanceListener : GrpcPerformanceListener
    {

        /// <summary>
        /// 
        /// </summary>
        public GrpcClientPerformanceListener() : base() { }

        #region メソッド呼び出し（クライアント）

        /// <summary>
        /// クライアント側でメソッド呼び出しが行われるときの処理を行います。
        /// </summary>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        public void NotifyMethodCalling(IMethod method, string host, CallOptions options)
        {
            try
            {
                NotifyClientMethodInvoking h = MethodCalling;
                if (h != null) { h(method.ServiceName, method.Name, host); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// クライアント側でメソッド呼び出しが行われるときに発生します。
        /// </summary>
        public event NotifyClientMethodInvoking MethodCalling;

        /// <summary>
        /// クライアント側でメソッド呼び出しが行われたときの処理を行います。
        /// </summary>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
        public void NotifyMethodCalled(IMethod method, string host, CallOptions options, double elapsedMilliseconds)
        {
            try
            {
                NotifyClientMethodInvoked h = MethodCalled;
                if (h != null) { h(method.ServiceName, method.Name, host, elapsedMilliseconds); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// クライアント側でメソッド呼び出しが行われたときに発生します。
        /// </summary>
        public event NotifyClientMethodInvoked MethodCalled;

        #endregion

        #region 割込処理（クライアント）

        /// <summary>
        /// クライアント側で割込処理が行われたときの処理を行います。
        /// </summary>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="interceptor">割込処理</param>
        /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
        public void NotifyMethodIntercepted(IMethod method, string host, CallOptions options, IGrpcInterceptor interceptor, double elapsedMilliseconds)
        {
            try
            {
                NotifyClientIntercept h = MethodIntercepted;
                if (h != null) { h(method.ServiceName, method.Name, host, interceptor, elapsedMilliseconds); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// クライアント側で割込処理が行われたときに発生します。
        /// </summary>
        public event NotifyClientIntercept MethodIntercepted;

        #endregion

        #region ストリーム読み取り（クライアント）

        /// <summary>
        /// クライアント側でレスポンスストリームの読み取りが行われるときの処理を行います。
        /// </summary>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        public void NotifyResponseReading(IMethod method, string host, CallOptions options)
        {
            try
            {
                NotifyClientMethodInvoking h = ResponseReading;
                if (h != null) { h(method.ServiceName, method.Name, host); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// クライアント側でレスポンスストリームの読み取りが行われるときに発生します。
        /// </summary>
        public event NotifyClientMethodInvoking ResponseReading;

        /// <summary>
        /// クライアント側でレスポンスストリームの読み取りが行われたときの処理を行います。
        /// </summary>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
        public void NotifyResponseReaded(IMethod method, string host, CallOptions options, double elapsedMilliseconds)
        {
            try
            {
                NotifyClientMethodInvoked h = ResponseReaded;
                if (h != null) { h(method.ServiceName, method.Name, host, elapsedMilliseconds); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// クライアント側でレスポンスストリームの読み取りが行われたときに発生します。
        /// </summary>
        public event NotifyClientMethodInvoked ResponseReaded;

        #endregion

        #region ストリーム書き込み（クライアント）

        /// <summary>
        /// クライアント側でリクエストストリームの書き込みが行われるときの処理を行います。
        /// </summary>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        public void NotifyRequestWriting(IMethod method, string host, CallOptions options)
        {
            try
            {
                NotifyClientMethodInvoking h = RequestWriting;
                if (h != null) { h(method.ServiceName, method.Name, host); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// クライアント側でリクエストストリームの書き込みが行われるときに発生します。
        /// </summary>
        public event NotifyClientMethodInvoking RequestWriting;

        /// <summary>
        /// クライアント側でリクエストストリームの書き込みが行われたときの処理を行います。
        /// </summary>
        /// <param name="method">メソッド</param>
        /// <param name="host">ホスト</param>
        /// <param name="options">オプション</param>
        /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
        public void NotifyRequestWrote(IMethod method, string host, CallOptions options, double elapsedMilliseconds)
        {
            try
            {
                NotifyClientMethodInvoked h = RequestWrote;
                if (h != null) { h(method.ServiceName, method.Name, host, elapsedMilliseconds); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// クライアント側でリクエストストリームの書き込みが行われたときに発生します。
        /// </summary>
        public event NotifyClientMethodInvoked RequestWrote;

        #endregion

    }

}
