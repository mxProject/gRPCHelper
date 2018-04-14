using System;
using System.Collections.Generic;
using System.Text;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.AspNetGateway
{

    /// <summary>
    /// 
    /// </summary>
    public static class ExceptionHandlers
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="responseStatus"></param>
        /// <param name="responseObject"></param>
        /// <returns></returns>
        public static bool ResponseException(RpcException ex, out System.Net.HttpStatusCode? responseStatus, out object responseObject)
        {

            responseStatus = null;
            responseObject = ex;
            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="responseStatus"></param>
        /// <param name="responseObject"></param>
        /// <returns></returns>
        public static bool ResponseException(Exception ex, out System.Net.HttpStatusCode? responseStatus, out object responseObject)
        {

            responseStatus = null;
            responseObject = ex;
            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="responseStatus"></param>
        /// <param name="responseObject"></param>
        /// <returns></returns>
        public static bool NotHandle(RpcException ex, out System.Net.HttpStatusCode? responseStatus, out object responseObject)
        {

            responseStatus = null;
            responseObject = null;
            return false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="responseStatus"></param>
        /// <param name="responseObject"></param>
        /// <returns></returns>
        public static bool NotHandle(Exception ex, out System.Net.HttpStatusCode? responseStatus, out object responseObject)
        {

            responseStatus = null;
            responseObject = null;
            return false;

        }

    }

}
