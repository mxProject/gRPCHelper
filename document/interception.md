# メソッド呼び出しに対する割込み #

## サーバーサイドの実装 ##

このライブラリでは3種類のインターフェースを提供しています。RPC メソッド呼び出しの前後に任意の処理を実行するには、これらのインターフェースを実装したクラスを実装します。

1. IGrpcServerMethodInvokingInterceptor
1. IGrpcServerMethodInvokedInterceptor
1. IGrpcServerMethodExceptionHandler

```csharp

using Grpc.Core;
using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Servers;

public interface IGrpcServerMethodInvokingInterceptor : IGrpcInterceptor
{
    Task OnInvokingAsync(ServerCallContext context);
}

public interface IGrpcServerMethodInvokedInterceptor : IGrpcInterceptor
{
    Task OnInvokedAsync(ServerCallContext context);
}

public interface IGrpcServerMethodExceptionHandler : IGrpcInterceptor
{
    bool ReplaceException(ServerCallContext context, Exception original, out Exception replaced);
}

public interface IGrpcInterceptor
{
    string Name { get; }
    int Priority { get; }
}

```

RPC メソッド呼び出しに対して割り込みを行うには、インターセプターを GrpcServiceBuilderSettings クラスのインターセプターコレクションに追加します。

```csharp

var settings = new GrpcServiceBuilderSettings();

// add interceptor.
var interceptor = new SampleServerInterceptor();

settings.InvokingInterceptors.Add(interceptor);
settings.InvokedInterceptors.Add(interceptor);
settings.ExceptionHandlers.Add(interceptor);

var server = new SampleService();
var builder = new GrpcServiceBuilder();

server.Ports.Add("localhost", 50000, ServerCredentials.Insecure);
server.Services.Add(builder.BuildService(typeof(SampleService), new SampleServiceImpl(), settings));


// SampleServerInterceptor implementation

internal class SampleServerInterceptor :
    IGrpcServerMethodInvokingInterceptor
    , IGrpcServerMethodInvokedInterceptor
    , IGrpcServerMethodExceptionHandler
{

    public string Name
    {
        get { return "SampleServerInterceptor"; }
    }

    public int Priority
    {
        get { return 1; }
    }

    public async Task OnInvokingAsync(ServerCallContext context)
    {
        // Implement the process before the RPC method is invoked.
    }

    public async Task OnInvokedAsync(ServerCallContext context)
    {
       // Implement the process after the RPC method is invoked.
    }

    public bool RelpaceException(ServerCallContext context, Exception original, out Exception replaced)
    {
        // To replace the exception that occurred, return the exception you want to throw.
        replaced = null;
        return false;
    }

}

```

インターフェースの代わりに属性を使用する方法も提供しています。これらは抽象クラスです。

1. GrpcServerMethodInvokingInterceptorAttribute
1. GrpcServerMethodInvokedInterceptorAttribute
1. GrpcServerMethodExceptionHandlerAttribute

サービスの利用時間を限定するインターセプターの例です。

```csharp

using Grpc.Core;
using mxProject.Helpers.Grpc.Servers;

internal class ServiceTimeFilterAttribute : GrpcServerMethodInvokingInterceptorAttribute
{

    internal ServiceTimeFilterAttribute(string openTime, string closeTime) : this(TimeSpan.Parse(openTime), TimeSpan.Parse(closeTime))
    {
    }

    internal ServiceTimeFilterAttribute(TimeSpan openTime, TimeSpan closeTime) : base(1, "ServiceTimeFilterAttribute")
    {
        m_OpenTime = openTime;
        m_CloseTime = closeTime;
    }

    public TimeSpan OpenTime
    {
        get { return m_OpenTime; }
    }
    private TimeSpan m_OpenTime;

    public TimeSpan CloseTime
    {
        get { return m_CloseTime; }
    }
    private TimeSpan m_CloseTime;

    protected async override Task OnInvokingAsync(ServerCallContext context)
    {

        await Task.Yield();

        TimeSpan now = DateTime.Now.TimeOfDay;

        if (now < m_OpenTime || m_CloseTime < now)
        {
            context.Status = new Status(StatusCode.Internal, "outside hours.");
        }

    }

```

この属性をサービスクラスやメソッドに適用します。

```csharp

[ServiceTimeFilter("10:00:00", "18:00:00")]
internal class SampleServiceImpl : SampleService.SampleServiceBase
{
}

```


## クライアントサイドの実装 ##

このライブラリでは3種類のインターフェースを提供しています。RPC メソッド呼び出しの前後に任意の処理を実行するには、これらのインターフェースを実装したクラスを実装します。

1. IGrpcClientMethodInvokingInterceptor
1. IGrpcClientMethodInvokedInterceptor
1. IGrpcClientMethodExceptionHandler

```csharp

using Grpc.Core;
using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Clients;

public interface IGrpcClientMethodInvokingInterceptor : IGrpcInterceptor
{
    void OnInvoking(IMethod method, string host, CallOptions options);
}

public interface IGrpcClientMethodInvokedInterceptor : IGrpcInterceptor
{
    void OnInvoked(IMethod method, string host, CallOptions options);
}

public interface IGrpcClientMethodExceptionHandler : IGrpcInterceptor
{
    bool ReplaceException(IMethod method, string host, CallOptions options, Exception original, out Exception replaced);
}

public interface IGrpcInterceptor
{
    string Name { get; }
    int Priority { get; }
}

```

RPC メソッド呼び出しに対して割り込みを行うには、インターセプターを GrpcClientSettings クラスのインターセプターコレクションに追加します。

```csharp

var settings = new GrpcClientSettings();

// add interceptor.
var interceptor = new SampleClientInterceptor();

settings.InvokingInterceptors.Add(interceptor);
settings.InvokedInterceptors.Add(interceptor);
settings.ExceptionHandlers.Add(interceptor);

var channel = new Channel("localhost", 50000, ChannelCredentials.Insecure);
var invoker = GrpcCallInvoker.Create(channel, settings);
var client = new SampleServiceClient(invoker);


// SampleClientInterceptor implementation

internal class SampleClientInterceptor :
    IGrpcClientMethodInvokingInterceptor
    , IGrpcClientMethodInvokedInterceptor
    , IGrpcClientMethodExceptionHandler
{

    public string Name
    {
        get { return "SampleClientInterceptor"; }
    }

    public int Priority
    {
        get { return 1; }
    }

    public void OnInvoking(IMethod method, string host, CallOptions options)
    {
        // Implement the process before the RPC method is invoked.
    }

    public void OnInvoked(IMethod method, string host, CallOptions options)
    {
       // Implement the process after the RPC method is invoked.
    }

    public bool ReplaceException(IMethod method, string host, CallOptions options, Exception original, out Exception replaced)
    {
        // To replace the exception that occurred, return the exception you want to throw.
        replaced = null;
        return false;
    }

}

```



