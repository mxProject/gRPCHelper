using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// マーシャラーの生成処理。
    /// </summary>
    public class GrpcMarshallerFactory : IGrpcMarshallerFactory
    {

        #region コンストラクタ

        /// <summary>
        /// シリアライザの生成処理を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="serializerFactory">シリアライザの生成処理</param>
        public GrpcMarshallerFactory(IGrpcSerializerFactory serializerFactory)
        {
            m_SerializerFactory = serializerFactory;
        }

        #endregion

        #region 既定のインスタンス

        /// <summary>
        /// 既定のインスタンス。
        /// </summary>
        public static readonly GrpcMarshallerFactory DefaultInstance = new GrpcMarshallerFactory(null);

        #endregion

        /// <summary>
        /// シリアライザの生成処理を取得します。
        /// </summary>
        public IGrpcSerializerFactory SerializerFactory
        {
            get { return m_SerializerFactory; }
        }
        private IGrpcSerializerFactory m_SerializerFactory;

        #region マーシャラーの生成

        /// <summary>
        /// 指定された型に対するマーシャラーを取得します。
        /// </summary>
        /// <typeparam name="T">オブジェクトの型</typeparam>
        /// <returns>マーシャラー</returns>
        public Marshaller<T> GetMarshaller<T>()
        {

            object obj;

            // 生成済であればそれを返す
            if (m_TypeMarshallers.TryGetValue(typeof(T), out obj))
            {
                return (Marshaller<T>)obj;
            }

            // カスタムマーシャラーを生成
            Marshaller<T> marshaller;

            if (TryCreateCustomMarshaller<T>(out marshaller))
            {
                m_TypeMarshallers.Add(typeof(T), marshaller);
                return marshaller;
            }

            // 既定のマーシャラーを生成
            Type t = typeof(Google.Protobuf.IMessage<>).MakeGenericType(new Type[] { typeof(T) });

            if (t.IsAssignableFrom(typeof(T)) && HasDefaultConstructor(typeof(T)))
            {

                obj = typeof(GrpcMarshallerFactory).GetMethod("CreateDefaultMarshaller", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(new Type[] { typeof(T) })
                    .Invoke(null, new object[] { });

                m_TypeMarshallers.Add(typeof(T), obj);

                return (Marshaller<T>)obj;

            }

            return null;

        }

        /// <summary>
        /// 指定された型に対するカスタムマーシャラーを生成します。
        /// </summary>
        /// <typeparam name="T">オブジェクトの型</typeparam>
        /// <param name="marshaller">マーシャラー</param>
        /// <returns>生成された場合、true を返します。</returns>
        private bool TryCreateCustomMarshaller<T>(out Marshaller<T> marshaller)
        {

            if (m_SerializerFactory == null)
            {
                marshaller = null;
                return false;
            }

            Func<T, byte[]> s = m_SerializerFactory.GetSerializer<T>();
            Func<byte[], T> d = m_SerializerFactory.GetDeserializer<T>();

            if (s == null || d == null)
            {
                marshaller = null;
                return false;
            }

            marshaller = new Marshaller<T>(s, d);
            return true;

        }

        /// <summary>
        /// 型に対する生成済のマーシャラー。
        /// </summary>
        private Dictionary<Type, object> m_TypeMarshallers = new Dictionary<Type, object>();

        /// <summary>
        /// 指定された型にデフォルトコンストラクタが定義されているかどうかを取得します。
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool HasDefaultConstructor(Type t)
        {

            ConstructorInfo ctor = t.GetConstructor(new Type[] { });

            return (t != null);

        }

        /// <summary>
        /// 指定された型に対する既定のマーシャラーを生成します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static Marshaller<T> CreateDefaultMarshaller<T>() where T : Google.Protobuf.IMessage<T>, new()
        {

            Func<T, byte[]> s = delegate (T arg)
            {
                if (arg == null) { return null; }
                return Google.Protobuf.MessageExtensions.ToByteArray((Google.Protobuf.IMessage)arg);
            };

            Func<byte[], T> d = delegate (byte[] data)
            {
                if (data == null) { return default(T); }
                return new global::Google.Protobuf.MessageParser<T>(delegate () { return new T(); }).ParseFrom(data);
            };

            return Marshallers.Create<T>(s, d);

        }

        #endregion

    }

}
