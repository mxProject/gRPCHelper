using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Examples.GrpcClient
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SampleClientSettings
    {

        /// <summary>
        /// 
        /// </summary>
        public string ServerName
        {
            get { return m_ServerName; }
            set { m_ServerName = value; }
        }
        private string m_ServerName;

        /// <summary>
        /// 
        /// </summary>
        public int ServerPort
        {
            get { return m_ServerPort; }
            set { m_ServerPort = value; }
        }
        private int m_ServerPort;

        /// <summary>
        /// 
        /// </summary>
        public string HttpGatewayUrl
        {
            get { return m_HttpGatewayUrl; }
            set { m_HttpGatewayUrl = value; }
        }
        private string m_HttpGatewayUrl;

        #region serialize

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        internal void SaveToFile(string filePath)
        {

            XmlWriterSettings setting = new XmlWriterSettings();

            setting.Encoding = System.Text.Encoding.UTF8;
            setting.Indent = true;
            setting.IndentChars = "\t";
            setting.NewLineChars = Environment.NewLine;

            using (XmlWriter writer = XmlWriter.Create(filePath, setting))
            {
                CreateSerializer().Serialize(writer, this);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        internal static SampleClientSettings LoadFromFile(string filePath)
        {

            using (XmlReader reader = XmlReader.Create(filePath, new XmlReaderSettings()))
            {
                return (SampleClientSettings)CreateSerializer().Deserialize(reader);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static XmlSerializer CreateSerializer()
        {
            return new XmlSerializer(typeof(SampleClientSettings));
        }

        #endregion

    }

}
