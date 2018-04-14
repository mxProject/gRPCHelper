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
    internal interface IDataGridBehavior
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<IDataField> GetDataFields();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetDataCount();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        object GetCellValue(int rowIndex, int columnIndex);

        /// <summary>
        /// 
        /// </summary>
        event EventHandler ItemsChanged;

    }

}
