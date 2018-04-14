using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Grpc.Core;
using System.Net;

namespace mxProject.Helpers.Grpc.AspNetGateway
{

    /// <summary>
    /// Represents the method that will handle an exception.
    /// </summary>
    /// <param name="ex">the exception that occurred</param>
    /// <param name="responseStatus">status to return as response</param>
    /// <param name="responseObject">object to return as response</param>
    /// <returns>true if the exception is handled. otherwise, false.</returns>
    public delegate bool RpcExceptionHandler(RpcException ex, out HttpStatusCode? responseStatus, out object responseObject);

    /// <summary>
    /// Represents the method that will handle an exception.
    /// </summary>
    /// <param name="ex">the exception that occurred</param>
    /// <param name="responseStatus">status to return as response</param>
    /// <param name="responseObject">object to return as response</param>
    /// <returns>true if the exception is handled. otherwise, false.</returns>
    public delegate bool ExceptionHandler(Exception ex, out HttpStatusCode? responseStatus, out object responseObject);

    /// <summary>
    /// Represents the method that will get whether the key is the key of Grpc request header.
    /// </summary>
    /// <param name="httpHeaderKey">the key of http request header</param>
    /// <param name="grpcHeaderKey">the key of Grpc request header</param>
    /// <returns>true if the key is the key of Grpc request header. otherwise, false.</returns>
    public delegate bool GrpcRequestHeaderFilter(string httpHeaderKey, out string grpcHeaderKey);

}
