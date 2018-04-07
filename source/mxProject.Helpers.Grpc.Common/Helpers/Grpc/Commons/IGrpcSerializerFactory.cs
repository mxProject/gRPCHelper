using System;
using System.Collections.Generic;
using System.Text;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// シリアライザの生成に必要な機能を提供します。
    /// </summary>
    public interface IGrpcSerializerFactory
    {

        /// <summary>
        /// 指定された型のシリアライズ処理を取得します。
        /// </summary>
        /// <typeparam name="T">オブジェクトの型</typeparam>
        /// <returns>シリアライズ処理</returns>
        Func<T, byte[]> GetSerializer<T>();

        /// <summary>
        /// 指定された型のデシリアライズ処理を取得します。
        /// </summary>
        /// <typeparam name="T">オブジェクトの型</typeparam>
        /// <returns>デシリアライズ処理</returns>
        Func<byte[], T> GetDeserializer<T>();

    }

}
