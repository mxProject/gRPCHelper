using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// <see cref="Metadata"/> に対する拡張メソッド。
    /// </summary>
    public static class MetadataExtension
    {

        /// <summary>
        /// 指定されたキーに対応する値を取得します。
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        /// <returns>取得できた場合、true を返します。</returns>
        public static bool TryGetValue(this Metadata metadata, string key, out string value)
        {

            string k = NormalizeHeaderKey(key);

            for (int i = 0; i < metadata.Count; ++i)
            {
                if (metadata[i].Key == k)
                {
                    value = metadata[i].Value;
                    return true;
                }
            }

            value = null;
            return false;

        }

        /// <summary>
        /// 指定されたキーに対応する値を取得します。
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        /// <returns>取得できた場合、true を返します。</returns>
        public static bool TryGetValueBytes(this Metadata metadata, string key, out byte[] value)
        {

            string k = NormalizeHeaderKey(key);

            for (int i = 0; i < metadata.Count; ++i)
            {
                if (metadata[i].Key == k)
                {
                    value = metadata[i].ValueBytes;
                    return true;
                }
            }

            value = null;
            return false;

        }

        /// <summary>
        /// 指定されたキーに対応する値を取得します。
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="key">キー</param>
        /// <returns>値</returns>
        public static string GetValueOrNull(this Metadata metadata, string key)
        {

            string value;

            if (!TryGetValue(metadata, key, out value))
            {
                return null;
            }
            else
            {
                return value;
            }

        }

        /// <summary>
        /// 指定されたキーに対応する値を取得します。
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="key">キー</param>
        /// <returns>値</returns>
        public static byte[] GetValueBytesOrNull(this Metadata metadata, string key)
        {

            byte[] value;

            if (!TryGetValueBytes(metadata, key, out value))
            {
                return null;
            }
            else
            {
                return value;
            }

        }

        /// <summary>
        /// 指定されたキーに対応する値を取得します。
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="key">キー</param>
        /// <param name="defaultValue">キーが存在しなかったときの既定値</param>
        /// <returns>値</returns>
        public static string GetValueOrDefault(this Metadata metadata, string key, string defaultValue)
        {

            string value;

            if (!TryGetValue(metadata, key, out value))
            {
                return defaultValue;
            }
            else
            {
                return value;
            }

        }

        /// <summary>
        /// 指定されたキーに対応する値を取得します。
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="key">キー</param>
        /// <param name="defaultValue">キーが存在しなかったときの既定値</param>
        /// <returns>値</returns>
        public static byte[] GetValueBytesOrDefault(this Metadata metadata, string key, byte[] defaultValue)
        {

            byte[] value;

            if (!TryGetValueBytes(metadata, key, out value))
            {
                return defaultValue;
            }
            else
            {
                return value;
            }

        }

        /// <summary>
        /// 指定されたキーを平準化します。
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>平準化されたキー</returns>
        private static string NormalizeHeaderKey(string key)
        {
            if (string.IsNullOrEmpty(key)) { return key; }
            return key.ToLower();
        }

    }

}
