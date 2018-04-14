# 使用方法 #

## 準備 ##

gRPC サービスクラスとクライアントクラスを実装します。Grpc.Tools を使用して proto ファイルから C# ソースコードを生成する方法が一般的ですが、直接コーディングするなど他の方法で実装しても構いません。

proto ファイルを作成します。

```protobuf

syntax = "proto3";

message Person {
    string Code = 1;
    string Name = 2;
    int32 Age = 3;
}

message PersonSearchRequest {
	string name = 1;
}

message PersonSearchResponse {
	repeated Person Persons = 1;
}

service PersonSearch {
	rpc Search (PersonSearchRequest) returns (PersonSearchResponse){}
}

```

Grpc.Tools を使用して C# ソースファイルを生成します。上の proto ファイルからは次のクラスが生成されます。

|クラス|概要|
-|-
|PersonSearchService|サービスに関連する型が定義された静的クラス|
|PersonSearchService.PersonSearchServiceBase|サービスクラスの基底実装|
|PersonSearchService.PersonSearchServiceClient|サービスクライアント|
|Person|データクラス|
|PersonSearchRequest|RPCメソッドのリクエストクラス|
|PersonSearchResponse|PRCメソッドのレスポンスクラス|


## サーバーサイドの実装 ##

標準の Grpc では次のようにサービスを登録します。
PersonSearchServiceImpl クラスは、PersonSearchService.PersonSearchServiceBase クラスを継承したサービス実装クラスです。

```csharp

var server = new Grpc.Core.Server();
server.Services.Add(PersonSearchService.BindService(new PersonSearchServiceImpl()));

```

mxProject.Helpers.Grpc では次のようにサービスを登録します。
PersonSearchService.BindService メソッドの代わりに GrpcServiceBuilder.BuildService メソッドを使用します。

```csharp

using mxProject.Helpers.Grpc.Servers;

var builder = new GrpcServiceBuilder();
var server = new Grpc.Core.Server();
server.Services.Add(builder.BuildService(typeof(PersonSearchService), new PersonSearchServiceImpl()));

```

サービスの動作をカスタマイズするには、BuildService メソッドの引数に GrpcServiceBuilderSettings インスタンスを指定します。
次の例では、独自のシリアライザを指定しています。

```csharp

using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Servers;

var settings = new GrpcServiceBuilderSettings();
settings.MarshallerFactory = new GrpcMarshallerFactory(new CustomSerializer());

var builder = new GrpcServiceBuilder();
var server = new Grpc.Core.Server();
server.Services.Add(builder.BuildService(typeof(PersonSearchService), new PersonSearchServiceImpl(), settings));

```


## クライアントサイドの実装 ##

標準の Grpc では次のようにクライアントを生成します。

```csharp

var channel = new Grpc.Core.Channel("localhost:8081", ChannelCredentials.Insecure);
var client = new PersonSearchService.PersonSearchServiceClient(channel);

```

mxProject.Helpers.Grpc では次のようにクライアントを生成します。
GrcpCallInvoker クラスを使用します。

```csharp

using mxProject.Helpers.Grpc.Clients;

var channel = new Grpc.Core.Channel("localhost:8081", ChannelCredentials.Insecure);
var invoker = GrcpCallInvoker.Create(channel);
var client = new PersonSearchService.PersonSearchServiceClient(invoker);

```

クライアントの動作をカスタマイズするには、GrcpCallInvoker のコンストラクターに GrpcClientSettings インスタンスを渡します。
次の例では、独自のシリアライザを指定しています。

```csharp

using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Clients;

var settings = new GrpcClientSettings();
settings.MarshallerFactory = new GrpcMarshallerFactory(new CustomSerializer());

var channel = new Grpc.Core.Channel("localhost:8081", ChannelCredentials.Insecure);
var invoker = GrcpCallInvoker.Create(channel, settings);
var client = new PersonSearchService.PersonSearchServiceClient(invoker);

```
