# Usage #

## Preparation ##

Implement gRPC service class and client class. It is common to use Grpc.Tools to generate C # source code from a proto file, but you can implement it in other ways such as direct coding.

Create a proto file.

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

Use Grpc.Tools to generate a C # source file. The following class is generated from the above proto file.

|class|summary|
-|-
|PersonSearchService|Static class.|
|PersonSearchService.PersonSearchServiceBase|Basic implementation of service class.|
|PersonSearchService.PersonSearchServiceClient|Service client class.|
|Person|Data Class.|
|PersonSearchRequest|Request class of RPC method.|
|PersonSearchResponse|Response class of RPC method.|


## Server-Side implementation ##

In the pure Grpc, you register the service as follows.
PersonSearchServiceImpl class is a service implementation class that inherits PersonSearchService.PersonSearchServiceBase class.

```csharp

var server = new Grpc.Core.Server();
server.Services.Add(PersonSearchService.BindService(new PersonSearchServiceImpl()));

```

In mxProject.Helpers.Grpc, you register the service as follows.
Instead of PersonSearchService.BindService method, use GrpcServiceBuilder.BuildService method.

```csharp

using mxProject.Helpers.Grpc.Servers;

var builder = new GrpcServiceBuilder();
var server = new Grpc.Core.Server();
server.Services.Add(builder.BuildService(typeof(PersonSearchService), new PersonSearchServiceImpl()));

```

To customize the behavior of the service, use GrpcServiceBuilderSettings class.
The following code is an example configuration for using a custom serializer.

```csharp

using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Servers;

var settings = new GrpcServiceBuilderSettings();
settings.MarshallerFactory = new GrpcMarshallerFactory(new CustomSerializer());

var builder = new GrpcServiceBuilder();
var server = new Grpc.Core.Server();
server.Services.Add(builder.BuildService(typeof(PersonSearchService), new PersonSearchServiceImpl(), settings));

```


## Client-Side implementation ##

In the pure Grpc, you generate a client instance as follows.

```csharp

var channel = new Grpc.Core.Channel("localhost:8081", ChannelCredentials.Insecure);
var client = new PersonSearchService.PersonSearchServiceClient(channel);

```

In mxProject.Helpers.Grpc, generate the client as follows.
Use GrcpCallInvoker class.

```csharp

using mxProject.Helpers.Grpc.Clients;

var channel = new Grpc.Core.Channel("localhost:8081", ChannelCredentials.Insecure);
var invoker = GrcpCallInvoker.Create(channel);
var client = new PersonSearchService.PersonSearchServiceClient(invoker);

```

To customize the behavior of the client, use GrpcClientSettings class.
The following code is an example configuration for using a custom serializer.

```csharp

using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Clients;

var settings = new GrpcClientSettings();
settings.MarshallerFactory = new GrpcMarshallerFactory(new CustomSerializer());

var channel = new Grpc.Core.Channel("localhost:8081", ChannelCredentials.Insecure);
var invoker = GrcpCallInvoker.Create(channel, settings);
var client = new PersonSearchService.PersonSearchServiceClient(invoker);

```