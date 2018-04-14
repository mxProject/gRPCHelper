# シリアライズ #

## シリアライザの実装 ##

リクエスト／レスポンスのシリアライズ処理を置き換えるには、IGrpcSerializerFactory インターフェースを実装したクラスを実装します。


```csharp

public interface IGrpcSerializerFactory
{
    Func<T, byte[]> GetSerializer<T>();
    Func<byte[], T> GetDeserializer<T>();
}

```

次のコードは [MessagePack for C#](https://github.com/neuecc/MessagePack-CSharp) を使用したシリアライザの例です。

```csharp

using mxProject.Helpers.Grpc.Commons;

public class MessagePackSerializerFactory : IGrpcSerializerFactory
{

    public Func<byte[], T> GetDeserializer<T>()
    {
        return delegate (byte[] data)
        {
            if (data == null) { return default(T); }
            return MessagePack.MessagePackSerializer.Deserialize<T>(data);
        };
    }

    public Func<T, byte[]> GetSerializer<T>()
    {
        return delegate (T arg)
        {
            if (arg == null) { return null; }
            return MessagePack.MessagePackSerializer.Serialize<T>(arg);
        };
    }

}

```

mxProject.Helpers.Grpc では GrpcMarshallerFactory クラスを提供しています。
このクラスは任意の IGrpcSerializerFactory インスタンスによってシリアライズを行う Marshaller を生成します。

```csharp

var marshallerFactory = new GrpcMarshallerFactory(new MessagePackSerializerFactory());

```


## サーバー側の実装 ##


```csharp

using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Servers;

var marshallerFactory = new GrpcMarshallerFactory(new MessagePackSerializerFactory());
var settings = new GrpcServiceBuilderSettings();
settings.MarshallerFactory = marshallerFactory;

var builder = new GrpcServiceBuilder();
var server = new Grpc.Core.Server();
server.Services.Add(builder.BuildService(typeof(PersonSearchService), new PersonSearchServiceImpl(), settings));

```


## クライアント側の実装 ##


```csharp

using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Clients;

var marshallerFactory = new GrpcMarshallerFactory(new MessagePackSerializerFactory());
var settings = new GrpcClientSettings();
settings.MarshallerFactory = marshallerFactory;

var channel = new Grpc.Core.Channel("localhost:8081", ChannelCredentials.Insecure);
var invoker = new GrcpCallInvoker(channel, settings);
var client = new PersonSearchService.PersonSearchServiceClient(invoker);

```
