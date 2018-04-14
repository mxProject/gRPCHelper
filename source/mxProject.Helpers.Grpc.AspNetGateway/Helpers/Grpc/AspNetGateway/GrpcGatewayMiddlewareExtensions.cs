using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

using Grpc.Core;
using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.AspNetGateway
{

    /// <summary>
    /// 
    /// </summary>
    public static class GrpcGatewayMiddlewareExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="methods"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseGrpcGateway(this IApplicationBuilder app, IEnumerable<GrpcServiceMethod> methods, GrpcGatewaySettings settings)
        {

            return app.UseMiddleware<GrpcGatewayMiddleware>(methods, settings);

        }

    }

}
