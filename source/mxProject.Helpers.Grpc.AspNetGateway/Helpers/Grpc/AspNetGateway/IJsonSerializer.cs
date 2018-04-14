using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mxProject.Helpers.Grpc.AspNetGateway
{

    /// <summary>
    /// 
    /// </summary>
    public interface IJsonSerializer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string Serialize(object obj);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        object Deserialize(string json, Type objectType);

    }

}
