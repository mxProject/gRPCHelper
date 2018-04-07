# 通知機能 #

## パフォーマンス通知 ##

### サーバーサイドの実装 ###

GrpcServerPerformanceListener クラスの通知イベントを利用すると、RPC メソッドの実行時間やリクエスト／レスポンスオブジェクトのバイトサイズなどのパフォーマンス情報を受け取ることができます。

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

各サービスメソッドに対してパフォーマンス通知を行うかどうかを設定するには、サービス実装クラスまたはメソッドに対して GrpcPerformanceNotify 属性を適用します。クラスとメソッドの両方に属性が適用されている場合は、メソッドに適用されている属性が有効になります。

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


### クライアントサイドの実装 ###

GrpcClientPerformanceListener クラスの通知イベントを利用すると、RPC メソッドの実行時間やリクエスト／レスポンスオブジェクトのバイトサイズなどのパフォーマンス情報を受け取ることができます。

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

## 例外発生通知 ##

GrpcExceptionListener クラスの通知イベントを利用すると、例外情報を受け取ることができます。

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
