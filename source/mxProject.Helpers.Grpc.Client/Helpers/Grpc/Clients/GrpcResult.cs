using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// レスポンスを返さない処理の実行結果。
    /// </summary>
    public sealed class GrpcResult : GrpcResultBase
    {

        /// <summary>
        /// 呼び出しオブジェクトを指定してインスタンスを生成します。
        /// </summary>
        /// <param name="call">呼び出しオブジェクト</param>
        public GrpcResult(IGrpcAsyncCall call) : base(call)
        {
        }

        /// <summary>
        /// 例外を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="exception">例外</param>
        public GrpcResult(Exception exception) : base(exception)
        {
        }

        #region ファクトリメソッド

        /// <summary>
        /// 指定されたレスポンスを格納したインスタンスを生成します。
        /// </summary>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="response">レスポンス</param>
        /// <param name="call">呼び出しオブジェクト</param>
        /// <returns></returns>
        public static GrpcResult<TResponse> Create<TResponse>(TResponse response, IGrpcAsyncCall call)
        {
            return new GrpcResult<TResponse>(response, call);
        }

        /// <summary>
        /// 指定された例外を格納したインスタンスを生成します。
        /// </summary>
        /// <typeparam name="TResponse">レスポンスの型</typeparam>
        /// <param name="exception">例外</param>
        /// <returns></returns>
        public static GrpcResult<TResponse> Create<TResponse>(Exception exception)
        {
            return new GrpcResult<TResponse>(exception);
        }

        /// <summary>
        /// 指定された例外を格納したインスタンスを生成します。
        /// </summary>
        /// <param name="exception">例外</param>
        /// <returns></returns>
        public static GrpcResult Create(Exception exception)
        {
            return new GrpcResult(exception);
        }

        #endregion

    }

}
