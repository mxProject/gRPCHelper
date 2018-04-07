using System;
using System.Collections.Generic;
using System.Text;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// マーシャラーの生成に必要な機能を提供します。
    /// </summary>
    public interface IGrpcMarshallerFactory
    {

        /// <summary>
        /// 指定された型に対するマーシャラーを取得します。
        /// </summary>
        /// <typeparam name="T">オブジェクトの型</typeparam>
        /// <returns>マーシャラー</returns>
        Marshaller<T> GetMarshaller<T>();

    }

}
