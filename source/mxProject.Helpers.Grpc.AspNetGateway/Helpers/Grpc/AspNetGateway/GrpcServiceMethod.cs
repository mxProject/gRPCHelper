using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.AspNetGateway
{

    /// <summary>
    /// 
    /// </summary>
    public class GrpcServiceMethod
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestType"></param>
        /// <param name="responseType"></param>
        public GrpcServiceMethod(IMethod method, Type requestType, Type responseType)
        {
            m_Method = method;
            m_RequestType = requestType;
            m_ResponseType = responseType;
        }

        /// <summary>
        /// 
        /// </summary>
        public IMethod Method
        {
            get { return m_Method; }
        }
        private IMethod m_Method;

        /// <summary>
        /// 
        /// </summary>
        public Type RequestType
        {
            get { return m_RequestType; }
        }
        private Type m_RequestType;

        /// <summary>
        /// 
        /// </summary>
        public Type ResponseType
        {
            get { return m_ResponseType; }
        }
        private Type m_ResponseType;

        /// <summary>
        /// 
        /// </summary>
        public IJsonSerializer JsonSerializer
        {
            get { return m_JsonSerializer; }
            set { m_JsonSerializer = value; }
        }
        private IJsonSerializer m_JsonSerializer;

        /// <summary>
        /// 
        /// </summary>
        public RpcExceptionHandler RpcExceptionHandler
        {
            get { return m_RpcExceptionHandler; }
            set { m_RpcExceptionHandler = value; }
        }
        private RpcExceptionHandler m_RpcExceptionHandler;

        /// <summary>
        /// 
        /// </summary>
        public ExceptionHandler ExceptionHandler
        {
            get { return m_ExceptionHandler; }
            set { m_ExceptionHandler = value; }
        }
        private ExceptionHandler m_ExceptionHandler;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal Type GetJsonRequestType()
        {

            if (m_RequestType == null) { return null; }

            switch (Method.Type)
            {
                case MethodType.ClientStreaming:
                case MethodType.DuplexStreaming:
                    return m_RequestType.MakeArrayType();

                default:
                    return m_RequestType;
            }

        }

    }

}
