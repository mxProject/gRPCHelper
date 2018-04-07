using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// <see cref="CallOptions"/> に対する拡張メソッド。
    /// </summary>
    public static class CallOptionsExtension
    {

        #region ヘッダー

        /// <summary>
        /// 指定されたキーに対応するヘッダーを取得します。
        /// </summary>
        /// <param name="options"></param>
        /// <param name="key">キー</param>
        /// <returns>値</returns>
        public static string GetHeaderValueOrNull(this CallOptions options, string key)
        {

            if (options.Headers == null) { return null; }

            return options.Headers.GetValueOrNull(key);

        }

        /// <summary>
        /// 指定されたキーに対応するヘッダーを取得します。
        /// </summary>
        /// <param name="options"></param>
        /// <param name="key">キー</param>
        /// <returns>値</returns>
        public static byte[] GetHeaderValueBytesOrNull(this CallOptions options, string key)
        {

            if (options.Headers == null) { return null; }

            return options.Headers.GetValueBytesOrNull(key);

        }

        /// <summary>
        /// 指定されたキーに対応するヘッダーを取得します。
        /// </summary>
        /// <param name="options"></param>
        /// <param name="key">キー</param>
        /// <param name="defaultValue">キーが存在しなかったときの既定値</param>
        /// <returns>値</returns>
        public static string GetHeaderValueOrDefault(this CallOptions options, string key, string defaultValue)
        {

            if (options.Headers == null) { return defaultValue; }

            return options.Headers.GetValueOrDefault(key, defaultValue);

        }

        /// <summary>
        /// 指定されたキーに対応するヘッダーを取得します。
        /// </summary>
        /// <param name="options"></param>
        /// <param name="key">キー</param>
        /// <param name="defaultValue">キーが存在しなかったときの既定値</param>
        /// <returns>値</returns>
        public static byte[] GetHeaderValueBytesOrDefault(this CallOptions options, string key, byte[] defaultValue)
        {

            if (options.Headers == null) { return defaultValue; }

            return options.Headers.GetValueBytesOrDefault(key, defaultValue);

        }

        #endregion

    }

}
