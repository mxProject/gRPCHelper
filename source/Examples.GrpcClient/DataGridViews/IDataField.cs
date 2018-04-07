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
    internal interface IDataField
    {

        /// <summary>
        /// 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        string HeaderText { get; }

    }

}
