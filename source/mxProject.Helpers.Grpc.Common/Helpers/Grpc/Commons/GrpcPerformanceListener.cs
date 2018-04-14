using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
using Grpc.Core.Logging;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// パフォーマンスリスナー。
    /// </summary>
    public class GrpcPerformanceListener
    {

        /// <summary>
        /// 
        /// </summary>
        public GrpcPerformanceListener() { }

        #region シリアライズ

        /// <summary>
        /// シリアライズ処理が行われたときの処理を行います。
        /// </summary>
        /// <param name="serviceName">サービス名</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="objectTypeName">オブジェクトの型</param>
        /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
        /// <param name="byteSize">データサイズ</param>
        public void NotifySerialized(string serviceName, string methodName, string objectTypeName, double elapsedMilliseconds, long byteSize)
        {
            try
            {
                NotifySerialization h = Serialized;
                if (h != null) { h(serviceName, methodName, objectTypeName, elapsedMilliseconds, byteSize); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// シリアライズ処理が行われたときに発生します。
        /// </summary>
        public event NotifySerialization Serialized;

        /// <summary>
        /// デシリアライズ処理が行われたときの処理を行います。
        /// </summary>
        /// <param name="serviceName">サービス名</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="objectTypeName">オブジェクトの型</param>
        /// <param name="elapsedMilliseconds">処理時間（ミリ秒）</param>
        /// <param name="byteSize">データサイズ</param>
        public void NotifyDeserialized(string serviceName, string methodName, string objectTypeName, double elapsedMilliseconds, long byteSize)
        {
            try
            {
                NotifySerialization h = Deserialized;
                if (h != null) { h(serviceName, methodName, objectTypeName, elapsedMilliseconds, byteSize); }
            }
            catch
            {
            }
        }

        /// <summary>
        /// デシリアライズ処理が行われたときに発生します。
        /// </summary>
        public event NotifySerialization Deserialized;

        #endregion

        #region 時間計測

        /// <summary>
        /// 指定されたストップウォッチの詳細な経過時間（秒）を取得します。
        /// </summary>
        /// <param name="watch">ストップウォッチ</param>
        /// <returns>経過時間（秒）</returns>
        public static double GetSeconds(System.Diagnostics.Stopwatch watch)
        {
            return (double)watch.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency;
        }

        /// <summary>
        /// 指定されたストップウォッチの詳細な経過時間（ミリ秒）を取得します。
        /// </summary>
        /// <param name="watch">ストップウォッチ</param>
        /// <returns>経過時間（ミリ秒）</returns>
        public static double GetMilliseconds(System.Diagnostics.Stopwatch watch)
        {
            return GetSeconds(watch) * 1000;
        }

        #endregion

    }

}
