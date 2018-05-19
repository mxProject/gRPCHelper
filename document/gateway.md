# HTTPゲートウェイ #

## ASP\.NET Core 用ミドルウェア ##

このライブラリでは GrpcGatewayMiddleware クラスを提供しています。このクラスは、ASP\.NET Core 用のミドルウェアです。

![AspNetGateway](/document/image/AspNetGateway.png "AspNetGateway")

新規「ASP\.NET Core Web アプリケーション」プロジェクトには、Startup クラスが定義されています。
この Startup クラスの Configure メソッドでアプリケーションにミドルウェアを登録します。

```csharp

using mxProject.Helpers.Grpc.AspNetGateway;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {

        // get rpc methods of the service
        IList<GrpcServiceMethod> methods = GrpcGatewayMiddleware.GetGrpcMethods(
            "SampleService"
            , typeof(SampleServiceImpl)
            , GrpcMarshallerFactory.DefaultInstance
            );

        // create a channel for the service
        Channel channel = new Channel("localhost", 50051, ChannelCredentials.Insecure);

        // create gateway settings
        GrpcGatewaySettings settings = new GrpcGatewaySettings(channel);

        // ExceptionHandlers.ResponseException is a handler that returns the information of the exception that occurred as a response.
        settings.RpcExceptionHandler = ExceptionHandlers.ResponseException;

        // regist middleware
        app.UseGrpcGateway(methods, settings);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.Run(async (context) =>
        {
            await context.Response.WriteAsync("Hello World!");
        });
    }
}

```

「ASP\.NET Core Web アプリケーション」を実行し、`http://ホストアドレス/サービス名/メソッド名` に対してリクエストを送信すると、ゲートウェイ経由で RPC メソッドが呼び出されます。なお、ポート 56907 は ASP\.NET Core Web アプリケーションに割り当てているポート番号です。

```
http://localhost:56907/SampleService/GetSample
```

## ストリームのサポート ##

GrpcGatewayMiddleware ではストリームをサポートしていません。

### ClientStream ###

* 複数のリクエストを送信する場合、リクエストの配列を表す JSON をポストしてください。

### ServerStream ###

* GrpcGatewayMiddleware は ResponseStream から全てのレスポンスを受信した後、レスポンスの配列を JSON に変換してクライアントに返します。

### DuplexStream ###

* 複数のリクエストを送信する場合、リクエストの配列を表す JSON をポストしてください。
* GrpcGatewayMiddleware は ResponseStream から全てのレスポンスを受信した後、レスポンスの配列を JSON に変換してクライアントに返します。
