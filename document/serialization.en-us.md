# Serialization #

## Implementation of Serializer ##

To replace the request / response serialization process, implement a class that implements IGrpcSerializerFactory interface.


```csharp

public interface IGrpcSerializerFactory
{
    Func<T, byte[]> GetSerializer<T>();
    Func<byte[], T> GetDeserializer<T>();
}

```

The following code is an example of a serializer using [MessagePack for C#](https://github.com/neuecc/MessagePack-CSharp).

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

mxProject.Helpers.Grpc provides GrpcMarshallerFactory class.
This class generates a Marshaller that serializes with any IGrpcSerializerFactory instance.

```csharp

var marshallerFactory = new GrpcMarshallerFactory(new MessagePackSerializerFactory());

```


## Server-Side implementation ##


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


## Client-Side implementation ##


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
