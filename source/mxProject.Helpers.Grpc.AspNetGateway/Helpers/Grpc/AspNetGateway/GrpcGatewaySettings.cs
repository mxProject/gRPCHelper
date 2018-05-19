using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

using Grpc.Core;

namespace mxProject.Helpers.Grpc.AspNetGateway
{

    /// <summary>
    /// 
    /// </summary>
    public class GrpcGatewaySettings
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        public GrpcGatewaySettings(Channel channel)
        {
            m_Channel = channel;
        }

        /// <summary>
        /// 
        /// </summary>
        public Channel Channel
        {
            get { return m_Channel; }
        }
        private Channel m_Channel;

        /// <summary>
        /// 
        /// </summary>
        public GrpcRequestHeaderFilter RequestHeaderFilter
        {
            get { return m_RequestHeaderFilter; }
            set { m_RequestHeaderFilter = value; }
        }
        private GrpcRequestHeaderFilter m_RequestHeaderFilter = IsGrpcRequestHeader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpHeaderKey"></param>
        /// <param name="grpcHeaderKey"></param>
        /// <returns></returns>
        private static bool IsGrpcRequestHeader(string httpHeaderKey, out string grpcHeaderKey)
        {

            string prefix = "grpc.";

            if (httpHeaderKey.Length > prefix.Length && httpHeaderKey.StartsWith(prefix))
            {
                grpcHeaderKey = httpHeaderKey.Substring(prefix.Length);
                return true;
            }

            grpcHeaderKey = null;
            return false;

        }

        /// <summary>
        /// 
        /// </summary>
        public Encoding RequestEncoding
        {
            get { return m_RequestEncoding; }
            set { m_RequestEncoding = value; }
        }
        private Encoding m_RequestEncoding = DefaultRequestEncoding;

        /// <summary>
        /// 
        /// </summary>
        internal readonly static Encoding DefaultRequestEncoding = Encoding.UTF8;

        #region json serialization

        /// <summary>
        /// 
        /// </summary>
        public IJsonSerializer JsonSerializer
        {
            get { return m_JsonSerializer; }
            set { m_JsonSerializer = value; }
        }
        private IJsonSerializer m_JsonSerializer = DefaultJsonSerializer;

        /// <summary>
        /// 
        /// </summary>
        internal readonly static IJsonSerializer DefaultJsonSerializer = JsonNetSerializer.DefaultInstance;

        #endregion

        #region exception handling

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

        #endregion

    }

}
