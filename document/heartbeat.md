# ハートビート #

簡易的なハートビートを実装するための機能を提供しています。

## サーバー側の実装 ##


```csharp

using mxProject.Helpers.Grpc.Utilities;

Server server = new Server();

// ハートビートメソッドを追加します。
// メソッドの URL は '/GrpcHeartbeat/Heartbeat' になります。
server.Services.Add(GrpcHeartbeat.BuildService());

// サービス名とメソッド名を指定してハートビートメソッドを追加します。
server.Services.Add(GrpcHeartbeat.BuildService("ServiceName", "MethodName"));

```


## クライアント側の実装 ##


```csharp

using mxProject.Helpers.Grpc.Utilities;

var channel = new Grpc.Core.Channel("localhost:8081", ChannelCredentials.Insecure);

// ハートビートの送受信をおこなうためのコンテキストを生成します。
GrpcHeartbeat.HeartbeatObject heartbeatContext = GrpcHeartbeat.CreateClientObject(channel);

// ハートビートを開始します。
// この場合、2秒後に開始され、その後5秒間隔でハートビートが送られます。
await heartbeatContext.Start(2000, 5000);

// ハートビートを停止します。
heartbeatContext.Stop();

```

## GrpcHeartbeat の概要 ##

GrpcHeartbeat は DuplexStreaming を使用して定期的にサーバーからクライアントへレスポンスを送ることによって接続を維持します。

バイト配列のストリームを送信し、バイト配列のレスポンスを受信するメソッドを動的に生成しています。Protocol Buffer IDL 風に表すと、次のようなメソッドです。

```
rpc Heartbeat (stream byte[]) returns stream byte[]
```

リクエストとレスポンスの型をバイト配列にすることによってマーシャリングを不要にしています。

GrpcHeartbeat.HeartbeatObject の Stop メソッドが呼び出されるか Dispose メソッドが呼び出されたとき、キャンセルトークンを送ってサーバー側のループを終了させています。

