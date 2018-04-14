using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grpc;
using Grpc.Core;

namespace mxProject.Helpers.Grpc.Commons
{

    #region RPCメソッド

    /// <summary>
    /// 指定されたリクエストを実行するメソッドであることを表します。
    /// </summary>
    /// <typeparam name="TClient">サービスクライアントの型</typeparam>
    /// <typeparam name="TRequest">リクエストの型</typeparam>
    /// <typeparam name="TResponse">レスポンスの型</typeparam>
    /// <param name="client">サービスクライアント</param>
    /// <param name="request">リクエスト</param>
    /// <param name="options">オプション</param>
    /// <returns>レスポンス</returns>
    public delegate TResponse UnaryCallFunc<TClient, TRequest, TResponse>(TClient client, TRequest request, CallOptions options) where TClient : ClientBase;

    /// <summary>
    /// 指定されたリクエストを非同期で実行するメソッドであることを表します。
    /// </summary>
    /// <typeparam name="TClient">サービスクライアントの型</typeparam>
    /// <typeparam name="TRequest">リクエストの型</typeparam>
    /// <typeparam name="TResponse">レスポンスの型</typeparam>
    /// <param name="client">サービスクライアント</param>
    /// <param name="request">リクエスト</param>
    /// <param name="options">オプション</param>
    /// <returns>非同期呼び出しオブジェクト</returns>
    public delegate AsyncUnaryCall<TResponse> AsyncUnaryCallFunc<TClient, TRequest, TResponse>(TClient client, TRequest request, CallOptions options) where TClient : ClientBase;

    /// <summary>
    /// 指定されたリクエストを送信し、複数回のレスポンスを受信するメソッドであることを表します。
    /// </summary>
    /// <typeparam name="TClient">サービスクライアントの型</typeparam>
    /// <typeparam name="TRequest">リクエストの型</typeparam>
    /// <typeparam name="TResponse">レスポンスの型</typeparam>
    /// <param name="client">サービスクライアント</param>
    /// <param name="request">リクエスト</param>
    /// <param name="options">オプション</param>
    /// <returns>非同期呼び出しオブジェクト</returns>
    public delegate AsyncServerStreamingCall<TResponse> AsyncServerStreamingFunc<TClient, TRequest, TResponse>(TClient client, TRequest request, CallOptions options) where TClient : ClientBase;

    /// <summary>
    /// 指定されたリクエストを送信し、複数回のレスポンスを受信するメソッドであることを表します。
    /// </summary>
    /// <typeparam name="TClient">サービスクライアントの型</typeparam>
    /// <typeparam name="TRequest">リクエストの型</typeparam>
    /// <typeparam name="TResponse">レスポンスの型</typeparam>
    /// <param name="client">サービスクライアント</param>
    /// <param name="options">オプション</param>
    /// <returns>非同期呼び出しオブジェクト</returns>
    public delegate AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingFunc<TClient, TRequest, TResponse>(TClient client, CallOptions options) where TClient : ClientBase;

    /// <summary>
    /// 指定された複数回のリクエストを送信し、複数回のレスポンスを受信するメソッドであることを表します。
    /// </summary>
    /// <typeparam name="TClient">サービスクライアントの型</typeparam>
    /// <typeparam name="TRequest">リクエストの型</typeparam>
    /// <typeparam name="TResponse">レスポンスの型</typeparam>
    /// <param name="client">サービスクライアント</param>
    /// <param name="options">オプション</param>
    /// <returns>非同期呼び出しオブジェクト</returns>
    public delegate AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingFunc<TClient, TRequest, TResponse>(TClient client, CallOptions options) where TClient : ClientBase;

    #endregion

    #region パフォーマンス通知

    /// <summary>
    /// シリアライズ処理が行われたことを通知するメソッドであることを表します。
    /// </summary>
    /// <param name="serviceName">サービス名</param>
    /// <param name="methodName">メソッド名</param>
    /// <param name="objectTypeName">オブジェクトの型</param>
    /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
    /// <param name="byteSize">データサイズ</param>
    public delegate void NotifySerialization(string serviceName, string methodName, string objectTypeName, double elapsedMilliseconds, long byteSize);

    /// <summary>
    /// クライアント側のメソッド呼び出しが行われることを通知するメソッドであることを表します。
    /// </summary>
    /// <param name="serviceName">サービス名</param>
    /// <param name="methodName">メソッド名</param>
    /// <param name="host">ホスト</param>
    public delegate void NotifyClientMethodInvoking(string serviceName, string methodName, string host);

    /// <summary>
    /// クライアント側のメソッド呼び出しが行われたことを通知するメソッドであることを表します。
    /// </summary>
    /// <param name="serviceName">サービス名</param>
    /// <param name="methodName">メソッド名</param>
    /// <param name="host">ホスト</param>
    /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
    public delegate void NotifyClientMethodInvoked(string serviceName, string methodName, string host, double elapsedMilliseconds);

    /// <summary>
    /// サーバー側のメソッド呼び出しが行われることを通知するメソッドであることを表します。
    /// </summary>
    /// <param name="context">コンテキスト</param>
    public delegate void NotifyServerMethodInvoking(ServerCallContext context);

    /// <summary>
    /// サーバー側のメソッド呼び出しが行われたことを通知するメソッドであることを表します。
    /// </summary>
    /// <param name="context">コンテキスト</param>
    /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
    public delegate void NotifyServerMethodInvoked(ServerCallContext context, double elapsedMilliseconds);

    /// <summary>
    /// クライアント側の割込処理が行われたことを通知するメソッドであることを表します。
    /// </summary>
    /// <param name="serviceName">サービス名</param>
    /// <param name="methodName">メソッド名</param>
    /// <param name="host">ホスト</param>
    /// <param name="interceptor">割込処理</param>
    /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
    public delegate void NotifyClientIntercept(string serviceName, string methodName, string host, IGrpcInterceptor interceptor, double elapsedMilliseconds);

    /// <summary>
    /// サーバー側の割込処理が行われたことを通知するメソッドであることを表します。
    /// </summary>
    /// <param name="context">コンテキスト</param>
    /// <param name="interceptor">割込処理</param>
    /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
    public delegate void NotifyServerIntercept(ServerCallContext context, IGrpcInterceptor interceptor, double elapsedMilliseconds);

    #endregion

    #region 非同期メソッド

    /// <summary>
    /// 非同期処理を実行するメソッドであることを表します。
    /// </summary>
    /// <typeparam name="TArg">引数の型</typeparam>
    /// <param name="arg">引数</param>
    /// <returns></returns>
    public delegate Task AsyncAction<TArg>(TArg arg);

    /// <summary>
    /// 非同期処理を実行するメソッドであることを表します。
    /// </summary>
    /// <typeparam name="TResult">実行結果の型</typeparam>
    /// <returns>実行結果</returns>
    public delegate Task<TResult> AsyncFunc<TResult>();

    /// <summary>
    /// 非同期処理を実行するメソッドであることを表します。
    /// </summary>
    /// <typeparam name="TArg">引数の型</typeparam>
    /// <typeparam name="TResult">実行結果の型</typeparam>
    /// <param name="arg">引数</param>
    /// <returns>実行結果</returns>
    public delegate Task<TResult> AsyncFunc<TArg, TResult>(TArg arg);

    #endregion

    #region 例外通知

    /// <summary>
    /// サーバー側のメソッド呼び出し処理で発生した例外を通知するメソッドであることを表します。
    /// </summary>
    /// <param name="context">コンテキスト</param>
    /// <param name="ex">例外</param>
    public delegate void NotifyServerException(ServerCallContext context, Exception ex);

    /// <summary>
    /// クライアント側のメソッド呼び出し処理で発生した例外を通知するメソッドであることを表します。
    /// </summary>
    /// <param name="method">メソッド</param>
    /// <param name="host">ホスト</param>
    /// <param name="options">オプション</param>
    /// <param name="ex">例外</param>
    public delegate void NotifyClientException(IMethod method, string host, CallOptions options, Exception ex);

    /// <summary>
    /// シリアライズ処理で発生した例外を通知するメソッドであることを表します。
    /// </summary>
    /// <param name="serviceName">サービス名</param>
    /// <param name="methodName">メソッド名</param>
    /// <param name="objectType">シリアライズ対象オブジェクトの型</param>
    /// <param name="ex">例外</param>
    public delegate void NotifySerializerException(string serviceName, string methodName, Type objectType, Exception ex);

    #endregion

    #region ログ

    /// <summary>
    /// ログメッセージをフォーマットするメソッドであることを表します。
    /// </summary>
    /// <param name="logSequence">連番</param>
    /// <param name="logTime">発生時刻</param>
    /// <param name="logMessage">メッセージ</param>
    /// <returns>出力するメッセージ</returns>
    public delegate string FormatLogMessage(int logSequence, DateTime logTime, string logMessage);

    #endregion

}
