# Heartbeat #

This library provides features to implement simple heartbeat.

## Server-Side implementation ##


```csharp

using mxProject.Helpers.Grpc.Utilities;

Server server = new Server();

// add a heartbeat method
// method url is '/GrpcHeartbeat/Heartbeat'
server.Services.Add(GrpcHeartbeat.BuildService());

// add a heartbeat method by specifying the service name and method name
server.Services.Add(GrpcHeartbeat.BuildService("ServiceName", "MethodName"));

```


## Client-Side implementation ##


```csharp

using mxProject.Helpers.Grpc.Utilities;

var channel = new Grpc.Core.Channel("localhost:8081", ChannelCredentials.Insecure);

// create context
GrpcHeartbeat.HeartbeatObject heartbeatContext = GrpcHeartbeat.CreateClientObject(channel);

// start heartbeat
// delay 2sec, interval 5sec
await heartbeatContext.Start(2000, 5000);

// stop heartbeat
heartbeatContext.Stop();

```

## Overview of GrpcHeartbeat ##

GrpcHeartbeat uses DuplexStreaming to keep the connection periodically by sending a response from the server to the client.

GrpcHeartbeat dynamically generates RPC methods for heartbeat. It's a method of sending a stream of byte array and receiving response of byte array. In Protocol Buffer style, it's the following method.

```
rpc Heartbeat (stream byte[]) returns stream byte[]
```

There is no need to marshal by making the request and response types a byte array.

When the Stop method of GrpcHeartbeat.HeartbeatObject is called or the Dispose method is called, a cancellation token is sent and the server side loop is terminated.

