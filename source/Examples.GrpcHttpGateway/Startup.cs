using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Grpc.Core;
using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.AspNetGateway;

namespace Examples.GrpcHttpGateway
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            // get rpc methods of the service
            IList<GrpcServiceMethod> methods = GrpcGatewayMiddleware.GetGrpcMethods("PlayerSearch", typeof(Examples.GrpcModels.PlayerSearch.PlayerSearchBase), GrpcMarshallerFactory.DefaultInstance);

            // create a channel for the service
            Channel channel = new Channel("localhost", 50051, ChannelCredentials.Insecure);

            // create gateway settings
            GrpcGatewaySettings settings = new GrpcGatewaySettings(channel);
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
}
