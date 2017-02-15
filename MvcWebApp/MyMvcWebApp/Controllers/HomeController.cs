using BLL;
using DAL.Interface;
using LoggerPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Util;

namespace MvcWebApp.Controllers
{
    public class HomeController : Controller
    {
        string userid = UserInfo.TUserId;

        /// <summary>
        /// webconfig中的連接字符串key
        /// </summary>
        string connKeyOra = "ConnectionStringOracle";

        string connKeySqlSer = "ConnectionStringSqlServer";
        //
        // GET: /Home/

        public ActionResult Index()
        {
            IDal iDal = ConfigManager.FrameWorkDataBaseType.Equals("0") ? IDal.InstanceOracle(connKeyOra) : IDal.InstanceSqlServer(connKeySqlSer);
            GetTreeMenu bll = new GetTreeMenu();
            DataTable dtUser = iDal.QuerySql(string.Format("SELECT USER_ID,DEPT_NO,USER_NAME,E_USER_NAME FROM F_USERS WHERE USER_ID ='{0}' AND ENABLE_FLAG='0'", userid)).Tables[0];
            if (dtUser == null || dtUser.Rows.Count.Equals(0))
            {
                LogHelper.WriteSqlDbLog("用户[" + userid + "]登录失败!");
                UserInfo.EmpNo = 0;
                UserInfo.DeptNo = string.Empty;
                UserInfo.UserName = string.Empty;
                UserInfo.EUserName = string.Empty;
                return RedirectToAction("Index", "Error");
            }
            else
            {
                ViewBag.UserId = dtUser.Rows[0][0];
                ViewBag.DeptNo = dtUser.Rows[0][1];
                ViewBag.UserName = dtUser.Rows[0][2];

                UserInfo.EmpNo = Convert.ToInt32(dtUser.Rows[0][0]);
                UserInfo.DeptNo = dtUser.Rows[0][1].ToString();
                UserInfo.EUserName = dtUser.Rows[0][2].ToString();
                UserInfo.UserName = dtUser.Rows[0][3].ToString();
                LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "登录成功!");
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetMenuTree()
        {
            IDal iDal = ConfigManager.FrameWorkDataBaseType.Equals("0") ? IDal.InstanceOracle(connKeyOra) : IDal.InstanceSqlServer(connKeySqlSer);
            GetTreeMenu bll = new GetTreeMenu();
            JsonTree tree = new JsonTree();

            DataTable dt = iDal.QuerySql(string.Format("SELECT * FROM F_TREE WHERE ID IN(SELECT NODE_ID FROM F_RIGHT WHERE ROLE_ID IN (SELECT ROLE_ID FROM F_USERS WHERE USER_ID ='{0}' AND ENABLE_FLAG='0'))  AND ENABLE='0'", userid)).Tables[0];
            //DataTable dt = iDal.QuerySql(string.Format("SELECT * FROM F_TREE WHERE ROLEID IN(SELECT ROLE_ID FROM F_USERS WHERE USER_ID ='{0}' AND ENABLE_FLAG='0') AND ENABLE='0'", user_id)).Tables[0];
            
            List<JsonTree> list = bll.initTree(dt);
            var json = JsonConvert.SerializeObject(list); //把对象集合转换成Json
            return Content(json);
        }

    }
}
