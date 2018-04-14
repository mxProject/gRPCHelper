using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mxProject.Helpers.Grpc.AspNetGateway
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class JsonNetSerializer : IJsonSerializer
    {

        /// <summary>
        /// 
        /// </summary>
        private JsonNetSerializer()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly JsonNetSerializer DefaultInstance = new JsonNetSerializer();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize(object obj)
        {

            if (obj == null) { return null; }

            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, new[] { new Newtonsoft.Json.Converters.StringEnumConverter() });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public object Deserialize(string json, Type objectType)
        {

            if (json == null) { return null; }

            return Newtonsoft.Json.JsonConvert.DeserializeObject(json, objectType);

        }

    }

}
