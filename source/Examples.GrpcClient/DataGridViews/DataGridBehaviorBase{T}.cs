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
    internal abstract class DataGridBehaviorBase<T> : IDataGridBehavior
    {

        /// <summary>
        /// 
        /// </summary>
        protected DataGridBehaviorBase()
        {
            m_Fields = CreateDataFields();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<IDataField> GetDataFields()
        {
            return m_Fields.ToList<IDataField>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetDataCount()
        {
            return m_Items.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public object GetCellValue(int rowIndex, int columnIndex)
        {

            if (columnIndex < 0 || m_Fields.Count <= columnIndex) { return null; }
            if (rowIndex < 0 || m_Items.Count <= rowIndex) { return null; }

            return m_Fields[columnIndex].GetValue(m_Items[rowIndex]);

        }

        private IList<DataField<T>> m_Fields;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract IList<DataField<T>> CreateDataFields();

        /// <summary>
        /// 
        /// </summary>
        public IList<T> Items
        {
            get { return m_Items.AsReadOnly(); }
        }
        private List<T> m_Items = new List<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void AddItems(IEnumerable<T> items)
        {
            if (m_Items == null) { return; }
            m_Items.AddRange(items);
            OnItemsChanged(EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearItems()
        {
            m_Items.Clear();
            OnItemsChanged(EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler ItemsChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnItemsChanged(EventArgs e)
        {
            EventHandler h = ItemsChanged;
            if (h != null) { h(this, e); }
        }

    }

}
