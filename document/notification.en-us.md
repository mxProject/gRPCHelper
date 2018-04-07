# Notifications #

## Performance notification ##

### Server-Side implementation ###

Using notification events of GrpcServerPerformanceListener class, you can receive performance information such as RPC method execution time and byte size of request / response objects.

```csharp

using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Servers;

// create listener
var listener = new GrpcServerPerformanceListener();

listener.MethodCalled += OnMethodCalled;
listener.MethodIntercepted += OnMethodIntercepted;
listener.RequestReaded += OnRequestReaded;
listener.ResponseWrote += OnResponseWrote;
listener.Serialized += OnSerialized;
listener.Deserialized += OnDeserialized;

// set listener to service
var settings = new GrpcServiceBuilderSettings();
settings.PerformanceListener = listener;

var builder = new GrpcServiceBuilder();
var server = new Grpc.Core.Server();
server.Services.Add(builder.BuildService(typeof(PersonSearchService), new PersonSearchServiceImpl(), settings));


// event handlers

void OnMethodCalled(ServerCallContext context, double elapsedMilliseconds)
{
}
void OnMethodIntercepted(ServerCallContext context, IGrpcInterceptor interceptor, double elapsedMilliseconds)
{
}

void OnRequestReaded(ServerCallContext context, double elapsedMilliseconds)
{
}
void OnResponseWrote(ServerCallContext context, double elapsedMilliseconds)
{
}

void OnSerialized(string serviceName, string methodName, string objectTypeName, double elapsedMilliseconds, long byteSize)
{
}
void OnDeserialized(string serviceName, string methodName, string objectTypeName, double elapsedMilliseconds, long byteSize)
{
}


```

To set whether to notify performance for each service method, apply GrpcPerformanceNotifyAttribute to the service implementation class or method.
If attributes are applied to both the class and the method, the attribute applied to the method is valid.

```csharp

[GrpcPerformanceNotify(true)]
internal class PersonSearchServiceImpl : PersonSearch.PersonSearchBase
{
    [GrpcPerformanceNotify(false)]
    public override Task<PersonSearchResponse> Search(PersonSearchRequest request, ServerCallContext context)
    }
}

```
|class|method|notifible|
-|-|-
|true|true|yes|
|false|true|yes|
|true|(unset)|yes|
|(unset)|true|yes|
|true|false|no|
|false|(unset)|no|
|(unset)|false|no|


### Client-Side implementation ###

Using notification events of GrpcClientPerformanceListener class, you can receive performance information such as RPC method execution time and byte size of request / response objects.

```csharp

using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Clients;

// create listener
var listener = new GrpcClientPerformanceListener();

listener.MethodCalled += OnMethodCalled;
listener.MethodIntercepted += OnMethodIntercepted;
listener.RequestWrote += OnRequestWrote;
listener.ResponseReaded += OnResponseReaded;
listener.Serialized += OnSerialized;
listener.Deserialized += OnDeserialized;

// set listener to client
var settings = new GrpcClientSettings();
settings.PerformanceListener = listener;

var channel = new Grpc.Core.Channel("localhost:8081", ChannelCredentials.Insecure);
var invoker = new GrcpCallInvoker(channel, settings);
var client = new PersonSearchService.PersonSearchServiceClient(invoker);


// event handlers

void OnMethodCalled(string serviceName, string methodName, string host, double elapsedMilliseconds)
{
}
void OnMethodIntercepted(string serviceName, string methodName, string host, IGrpcInterceptor interceptor, double elapsedMilliseconds)
{
}

void OnRequestWrote(string serviceName, string methodName, string host, double elapsedMilliseconds)
{
}
void OnResponseReaded(string serviceName, string methodName, string host, double elapsedMilliseconds)
{
}

void OnSerialized(string serviceName, string methodName, string objectTypeName, double elapsedMilliseconds, long byteSize)
{
}
void OnDeserialized(string serviceName, string methodName, string objectTypeName, double elapsedMilliseconds, long byteSize)
{
}

```

## Exception notification ##

Using notification events of GrpcExceptionListener class, you can receive exception information.

```csharp

using Grpc.Core;
using mxProject.Helpers.Grpc.Commons;

// ClientSide
GrpcExceptionListener.CatchClientException += OnCatchClientException;

void OnCatchClientException(IMethod method, string host, CallOptions options, Exception ex)
{
}

// ServerSide
GrpcExceptionListener.CatchServerException += OnCatchServerException;

void OnCatchServerException(ServerCallContext context, Exception ex)
{
}

// Serializaion (ClientSide/ServerSide)
GrpcExceptionListener.CatchSerializerException += OnCatchSerializerException;

void OnCatchSerializerException(string serviceName, string methodName, Type objectType, Exception ex)
{
}

```
