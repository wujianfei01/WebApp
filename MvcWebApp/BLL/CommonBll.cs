using MODEL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL
{
    public class CommonBll
    {
        /// <summary>
        // 組裝json對象
        /// </summary>
        /// <param name="dt">待構造的datatable對象</param>
        /// <param name="totalRows">可自定義:數據總條數</param> 
        /// <returns></returns>
        public JsonData initJsonData(DataTable dt,int totalRows)
        {
            JsonData jd = new JsonData();
            if (dt != null && dt.Rows.Count > 0)
            {
                jd.total = totalRows;
                jd.rows = dt;
            }
            else
            {
                jd.total = totalRows;
                jd.rows = new DataTable();//構造非空json
            }
            return jd;
        }
    }
}
