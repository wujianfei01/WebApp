using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MODEL
{
    /// <summary>
    /// combox json
    /// </summary>
    public class JsonCombox
    {
        private string _id;

        public string id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _text;

        public string text
        {
            get { return _text; }
            set { _text = value; }
        }

        private bool _selected;

        public bool selected
        {
            get { return _selected; }
            set { _selected = value; }
        }
    }
}
