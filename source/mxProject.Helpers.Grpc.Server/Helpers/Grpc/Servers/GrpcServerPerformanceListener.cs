using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
using Grpc.Core.Logging;
using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.Servers
{

    /// <summary>
    /// パフォーマンスリスナー。
    /// </summary>
    public class GrpcServerPerformanceListener : GrpcPerformanceListener
    {

        /// <summary>
        /// 
        /// </summary>
        public GrpcServerPerformanceListener() : base() { }

        #region メソッド呼び出し（サーバー）

        /// <summary>
        /// サーバー側でメソッド呼び出しが行われるときの処理を行います。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        public void NotifyMethodCalling(ServerCallContext context)
        {
            try
            {
                NotifyServerMethodInvoking h = MethodCalling;
                if (h != null) { h(context); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// サーバー側でメソッド呼び出しが行われるときに発生します。
        /// </summary>
        public event NotifyServerMethodInvoking MethodCalling;

        /// <summary>
        /// サーバー側でメソッド呼び出しが行われたときの処理を行います。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
        public void NotifyMethodCalled(ServerCallContext context, double elapsedMilliseconds)
        {
            try
            {
                NotifyServerMethodInvoked h = MethodCalled;
                if (h != null) { h(context, elapsedMilliseconds); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// サーバー側でメソッド呼び出しが行われたときに発生します。
        /// </summary>
        public event NotifyServerMethodInvoked MethodCalled;

        #endregion

        #region 割込処理（サーバー）

        /// <summary>
        /// サーバー側で割込処理が行われたときの処理を行います。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="interceptor">割込処理</param>
        /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
        public void NotifyMethodIntercepted(ServerCallContext context, IGrpcInterceptor interceptor, double elapsedMilliseconds)
        {
            try
            {
                NotifyServerIntercept h = MethodIntercepted;
                if (h != null) { h(context, interceptor, elapsedMilliseconds); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// サーバー側で割込処理が行われたときに発生します。
        /// </summary>
        public event NotifyServerIntercept MethodIntercepted;

        #endregion

        #region ストリーム読み取り（サーバー）

        /// <summary>
        /// サーバー側でリクエストストリームの読み取りが行われるときの処理を行います。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        public void NotifyRequestReading(ServerCallContext context)
        {
            try
            {
                NotifyServerMethodInvoking h = RequestReading;
                if (h != null) { h(context); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// サーバー側でリクエストストリームの読み取りが行われるときに発生します。
        /// </summary>
        public event NotifyServerMethodInvoking RequestReading;

        /// <summary>
        /// サーバー側でリクエストストリームの読み取りが行われたときの処理を行います。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
        public void NotifyRequestReaded(ServerCallContext context, double elapsedMilliseconds)
        {
            try
            {
                NotifyServerMethodInvoked h = RequestReaded;
                if (h != null) { h(context, elapsedMilliseconds); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// サーバー側でリクエストストリームの読み取りが行われたときに発生します。
        /// </summary>
        public event NotifyServerMethodInvoked RequestReaded;

        #endregion

        #region ストリーム書き込み（サーバー）

        /// <summary>
        /// サーバー側でレスポンスストリームの書き込みが行われるときの処理を行います。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        public void NotifyResponseWriting(ServerCallContext context)
        {
            try
            {
                NotifyServerMethodInvoking h = ResponseWriting;
                if (h != null) { h(context); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// サーバー側でレスポンスストリームの書き込みが行われるときに発生します。
        /// </summary>
        public event NotifyServerMethodInvoking ResponseWriting;

        /// <summary>
        /// サーバー側でレスポンスストリームの書き込みが行われたときの処理を行います。
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
        public void NotifyResponseWrote(ServerCallContext context, double elapsedMilliseconds)
        {
            try
            {
                NotifyServerMethodInvoked h = ResponseWrote;
                if (h != null) { h(context, elapsedMilliseconds); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// サーバー側でレスポンスストリームの書き込みが行われたときに発生します。
        /// </summary>
        public event NotifyServerMethodInvoked ResponseWrote;

        #endregion

    }

}
