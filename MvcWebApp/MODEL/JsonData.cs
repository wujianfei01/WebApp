using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MODEL
{
    /// <summary>
    /// 查詢數據返回的json
    /// </summary>
    public class JsonData
    {
        private int _total;

        public int total
        {
            get { return _total; }
            set { _total = value; }
        }

        private DataTable _rows;

        public DataTable rows
        {
            get { return _rows; }
            set { _rows = value; }
        }
    }
}
