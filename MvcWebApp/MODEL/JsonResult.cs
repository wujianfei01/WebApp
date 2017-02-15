using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MODEL
{
    /// <summary>
    /// 操作數據返回的json
    /// </summary>
    public class MyJsonResult
    {
        /// <summary>
        /// 操作結果:0-success 1-fail
        /// </summary>
        private int _code;
        public int code
        {
            get { return _code; }
            set { _code = value; }
        }

        /// <summary>
        /// 報錯信息
        /// </summary>
        private string _msg;

        public string msg
        {
            get { return _msg; }
            set { _msg = value; }
        }
    }
}
