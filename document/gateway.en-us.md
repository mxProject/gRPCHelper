# Http Gateway #

## Middleware for ASP\.NET Core ##

This library provides GrpcGatewayMiddleware class. This class is middleware for ASP\.NET Core.

![AspNetGateway](/image/AspNetGateway.png "AspNetGateway")

In the new `ASP.NET Core Web Application` project, Startup class is defined.
Register the middleware in the application using Startup.Configure method.

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

When you run `ASP.NET Core web application` and send a request to `http://hostaddress/servicename/methodname`, the RPC method is called via GrpcGatewayMiddleware. Port 56907 is the port number assigned to the `ASP.NET Core web application`.

```
http://localhost:56907/SampleService/GetSample
```

## Streaming ##

GrpcGatewayMiddleware doesn't support streaming.

### ClientStream ###

* To send more than one request, send JSON representing the array of requests.

### ServerStream ###

* After receiving all responses from the ResponseStream, GrpcGatewayMiddleware converts the response array to JSON and returns it to the client.

### DuplexStream ###

* To send more than one request, send JSON representing the array of requests.
* After receiving all responses from the ResponseStream, GrpcGatewayMiddleware converts the response array to JSON and returns it to the client.
