using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL
{
    /// <summary>
    /// 動態menu tree bll
    /// </summary>
    public class GetTreeMenu
    {
        public List<JsonTree> initTree(DataTable dt)
        {
            DataRow[] drList = dt.Select("parentid='0'");
            List<JsonTree> rootNode = new List<JsonTree>();
            foreach (DataRow dr in drList)
            {
                JsonTree jt = new JsonTree();
                jt.id = dr["id"].ToString();
                jt.text = dr["menuname"].ToString();
                jt.state = dr["state"].ToString();
                //jt.url = dr["url"].ToString();;
                jt.attributes = CreateUrl(dt, jt);
                jt.children = CreateChildTree(dt, jt);
                rootNode.Add(jt);
            }
            return rootNode;
        }

        public List<MyJsonTree> initMyTree(DataTable dt)
        {
            DataRow[] drList = dt.Select("parentid='0'");
            List<MyJsonTree> rootNode = new List<MyJsonTree>();
            foreach (DataRow dr in drList)
            {
                MyJsonTree jt = new MyJsonTree();
                jt.id = dr["id"].ToString();
                jt.text = dr["menuname"].ToString();
                jt.state = dr["state"].ToString();
                jt.ischecked = dr["checked"].ToString();
                jt.attributes = CreateUrl(dt, jt);
                jt.children = CreateMyChildTree(dt, jt);
                rootNode.Add(jt);
            }
            return rootNode;
        }

        private List<JsonTree> CreateChildTree(DataTable dt, JsonTree jt)
        {
            string keyid = jt.id;                                        //根节点ID
            List<JsonTree> nodeList = new List<JsonTree>();
            DataRow[] children = dt.Select("Parentid='" + keyid + "'");
            foreach (DataRow dr in children)
            {
                JsonTree node = new JsonTree();
                node.id = dr["id"].ToString();
                node.text = dr["menuname"].ToString();
                node.state = dr["state"].ToString();
                //node.url = dr["url"].ToString();
                node.attributes = CreateUrl(dt, node);
                node.children = CreateChildTree(dt, node);
                nodeList.Add(node);
            }
            return nodeList;
        }

        private List<MyJsonTree> CreateMyChildTree(DataTable dt, JsonTree jt)
        {
            string keyid = jt.id;                                        //根节点ID
            List<MyJsonTree> nodeList = new List<MyJsonTree>();
            DataRow[] children = dt.Select("Parentid='" + keyid + "'");
            foreach (DataRow dr in children)
            {
                MyJsonTree node = new MyJsonTree();
                node.id = dr["id"].ToString();
                node.text = dr["menuname"].ToString();
                node.state = dr["state"].ToString();
                node.ischecked = dr["checked"].ToString();
                node.attributes = CreateUrl(dt, node);
                node.children = CreateMyChildTree(dt, node);
                nodeList.Add(node);
            }
            return nodeList;
        }


        private Dictionary<string, string> CreateUrl(DataTable dt, JsonTree jt)    //把Url属性添加到attribute中，如果需要别的属性，也可以在这里添加
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string keyid = jt.id;
            DataRow[] urlList = dt.Select("id='" + keyid + "'");
            string url = urlList[0]["Url"].ToString();
            dic.Add("url", url);
            return dic;
        }
    }

    /// <summary>
    /// tree menu的entity
    /// </summary>
    public class JsonTree
    {
        private string _id;
        private string _text;
        private string _state = "open";
        //private string _url;
        private Dictionary<string, string> _attributes = new Dictionary<string, string>();
        private object _children;

        public string id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string text
        {
            get { return _text; }
            set { _text = value; }
        }
        public string state
        {
            get { return _state; }
            set { _state = value; }
        }

        //public string url
        //{
        //    get { return _url; }
        //    set { _url = value; }
        //}

        public Dictionary<string, string> attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }
        public object children
        {
            get { return _children; }
            set { _children = value; }
        }
    }

    public class MyJsonTree : JsonTree
    {
        private string _checked = "false";

        public string ischecked
        {
            get { return _checked; }
            set { _checked = value; }
        }
    }
}
