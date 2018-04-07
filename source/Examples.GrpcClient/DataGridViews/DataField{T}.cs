using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.GrpcClient.DataGridViews
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class DataField<T> : IDataField
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="headerText"></param>
        /// <param name="valueGetter"></param>
        internal DataField(string name, string headerText, Func<T, object> valueGetter)
        {
            this.Name = name;
            this.HeaderText = headerText;
            m_ValueGetter = valueGetter;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public string HeaderText
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object GetValue(T obj)
        {
            if (m_ValueGetter == null) { return null; }
            return m_ValueGetter(obj);
        }
        private Func<T, object> m_ValueGetter;

    }

}
