using System;
using System.Collections.Generic;
using System.Text;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// 例外に関する汎用処理。
    /// </summary>
    public static class GrpcExceptionUtility
    {

        /// <summary>
        /// 指定された例外が RPC 処理に関する例外であるかどうかを取得します。
        /// </summary>
        /// <param name="ex">例外</param>
        /// <returns></returns>
        public static bool HasRpcException(Exception ex)
        {

            if (ex == null) { return false; }

            if (ex is RpcException) { return true; }

            if (ex is AggregateException)
            {

                AggregateException actual = ex as AggregateException;

                if (HasRpcException(actual.InnerException)) { return true; }

                if (actual.InnerExceptions!= null)
                {
                    foreach (Exception inner in actual.InnerExceptions)
                    {
                        if (HasRpcException(inner)) { return true; }
                    }
                }

            }

            if (ex is System.Reflection.TargetInvocationException)
            {
                System.Reflection.TargetInvocationException actual = ex as System.Reflection.TargetInvocationException;
                return HasRpcException(actual.InnerException);
            }

            return false;

        }

        /// <summary>
        /// 実際に発生した例外を取得します。
        /// </summary>
        /// <param name="ex">例外</param>
        /// <returns></returns>
        public static Exception GetActualException(Exception ex)
        {

            if (ex is RpcException) { return ex; }

            if (ex is AggregateException)
            {

                AggregateException actual = ex as AggregateException;

                if (HasRpcException(actual.InnerException)) { return actual.InnerException; }

                if (actual.InnerExceptions != null)
                {
                    foreach (Exception inner in actual.InnerExceptions)
                    {
                        if (HasRpcException(inner)) { return inner; }
                    }
                }

            }

            if (ex is System.Reflection.TargetInvocationException)
            {
                System.Reflection.TargetInvocationException actual = ex as System.Reflection.TargetInvocationException;
                return GetActualException(actual.InnerException);
            }

            return ex;

        }

    }

}
