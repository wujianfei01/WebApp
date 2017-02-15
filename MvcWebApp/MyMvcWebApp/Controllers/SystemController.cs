using BLL.MainDemo;
using BLL.System;
using ExcelPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Util;
using WsPlugin;

namespace MvcWebApp.Controllers
{
    public class SystemController : Controller
    {
        //
        // GET: /System/
        public ActionResult WebLog()
        {
            //指定頁面的action
            ViewBag.MyQueryAction = "/" + this.Request.RawUrl.Split('/')[1] + "/WebLogGetData";
            return View();
        }

        public ActionResult SysExpView()
        {
            Dictionary<string, string> dts = new Dictionary<string, string>();
            var urlArray = Request.Url.ToString().Split('/');
            var efUrl = urlArray[0] + @"//" + urlArray[2];
            foreach (string t in System.IO.Directory.GetFiles(Server.MapPath("~/Log/LogError")))
            {
                var array = t.Split('\\');
                dts.Add(array[array.Length - 1].Replace(".htm", ""), efUrl + "/Log/LogError/" + array[array.Length - 1]);
            }
            ViewBag.ErrorDetail = dts;
            return View();
        }

        public void Export()
        {
            WebLogQuery iQuery = new WebLogQuery();
            WebLogQueryForSQLServer iQueryForSQLServer = new WebLogQueryForSQLServer();
            new InputAndOutput().DownloadExcel((ConfigManager.FrameWorkDataBaseType.Equals("0") ? iQuery.GetAllLog() : iQueryForSQLServer.GetAllLog()), "WebLog");
        }

        public ActionResult MenuManager()
        {
            //指定頁面的action
            ViewBag.MyQueryAction = "/" + this.Request.RawUrl.Split('/')[1] + "/MenuManagerGetData";
            ViewBag.MyEditAction = "/" + this.Request.RawUrl.Split('/')[1] + "/MenuManagerEdit";
            ViewBag.MyAddAction = "/" + this.Request.RawUrl.Split('/')[1] + "/MenuManagerAdd";
            ViewBag.MyDeleteAction = "/" + this.Request.RawUrl.Split('/')[1] + "/MenuManagerDelete";
            return View();
        }
        [HttpPost]
        public ActionResult BaseDataDisable()
        {
            string key = this.Request["keys"];
            string enable = this.Request["flag"];
            BaseDataEdit iEdit = new BaseDataEdit();
            BaseDataEditForSQLServer iForSQLServer = new BaseDataEditForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iEdit.DisableBaseData(key, enable) : iForSQLServer.DisableBaseData(key, enable));
            return Content(json);
        }
        public ActionResult RoleManager()
        {
            ViewBag.MyQueryAction = "/" + this.Request.RawUrl.Split('/')[1] + "/RoleManagerGetData";
            ViewBag.MyEditAction = "/" + this.Request.RawUrl.Split('/')[1] + "/RoleManagerEdit";
            ViewBag.MyAddAction = "/" + this.Request.RawUrl.Split('/')[1] + "/RoleManagerAdd";
            ViewBag.MyDeleteAction = "/" + this.Request.RawUrl.Split('/')[1] + "/RoleManagerDelete";
            return View();
        }

        public ActionResult UserManager()
        {
            ViewBag.MyQueryAction = "/" + this.Request.RawUrl.Split('/')[1] + "/UserManagerGetData";
            ViewBag.MyEditAction = "/" + this.Request.RawUrl.Split('/')[1] + "/UserManagerEdit";
            ViewBag.MyAddAction = "/" + this.Request.RawUrl.Split('/')[1] + "/UserManagerAdd";
            ViewBag.MyDeleteAction = "/" + this.Request.RawUrl.Split('/')[1] + "/UserManagerDelete";
            return View();
        }

        public ActionResult BaseData()
        {
            ViewBag.MyQueryAction = "/" + this.Request.RawUrl.Split('/')[1] + "/BaseDataGetData";
            ViewBag.MyEditAction = "/" + this.Request.RawUrl.Split('/')[1] + "/BaseDataEdit";
            ViewBag.MyAddAction = "/" + this.Request.RawUrl.Split('/')[1] + "/BaseDataAdd";
            ViewBag.MyDeleteAction = "/" + this.Request.RawUrl.Split('/')[1] + "/BaseDataDelete";
            return View();
        }

        public ActionResult BaseDataAdd()
        {
            ViewBag.MySaveAction = "/" + this.Request.RawUrl.Split('/')[1] + "/BaseDataSave";
            return View();
        }

        public ActionResult BaseDataSave(FormCollection fc)
        {
            BaseDataEdit iMenuEdit = new BaseDataEdit();
            BaseDataEditForSQLServer iMenuEditForSQLServer = new BaseDataEditForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iMenuEdit.SaveOrUpdate(fc) : iMenuEditForSQLServer.SaveOrUpdate(fc));
            return Content(json);
        }

        /// <summary>
        /// 菜單頁面跳轉
        /// </summary>
        /// <returns></returns>
        public ActionResult MenuManagerAdd()
        {
            ViewBag.MySaveAction = "/" + this.Request.RawUrl.Split('/')[1] + "/MenuManagerSave";
            return View();
        }

        public ActionResult RoleManagerAdd()
        {
            ViewBag.MySaveAction = "/" + this.Request.RawUrl.Split('/')[1] + "/RoleManagerSave";
            ViewBag.Power = ConfigManager.FrameWorkDataBaseType.Equals("0") ? (new RoleQuery().GetPowerForInit(this.Request["EORA"].Equals("a") ? "" : this.Request["ROLE_ID"])) : (new RoleQueryForSQLServer().GetPowerForInit(this.Request["EORA"].Equals("a") ? "" : this.Request["ROLE_ID"]));
            return View();
        }

        public ActionResult UserManagerAdd()
        {
            ViewBag.MySaveAction = "/" + this.Request.RawUrl.Split('/')[1] + "/UserManagerSave";
            return View();
        }

        public ActionResult UserManagerSave(FormCollection fc)
        {
            UserEdit iUserEdit = new UserEdit();
            UserEditForSQLServer iUserEditForSQLServer = new UserEditForSQLServer();
            var json = string.Empty;

            if (CheckEmpNo(fc["USER_ID"]))
            {
                fc = GetEmpInfo(fc);
                json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iUserEdit.SaveOrUpdate(fc) : iUserEditForSQLServer.SaveOrUpdate(fc));
            }
            else
            {
                json = JsonConvert.SerializeObject(iUserEdit.CommonAlertMsg(1, "Error:Emp No is not correct."));
            }
            return Content(json);
        }

        private bool CheckEmpNo(string empNo)
        {
            DynamicWS dws = new DynamicWS(@"http://myportal.wneweb.com.tw:8808/HR_WS/User_Def/Service.asmx", "getUserName");
            dws.Params.Add("empno", empNo);
            dws.Params.Add("name", "1");
            dws.Invoke();
            if (dws.dsResult != null && dws.dsResult.Tables[0].Rows.Count > 0)
                return true;
            else if (dws.ResultXMLClassic.HasChildNodes && !string.IsNullOrEmpty(dws.ResultXMLClassic.ChildNodes[1].InnerText) && dws.ResultXMLClassic.ChildNodes[1].InnerText.ToLower() != "guest")
                return true;
            else
                return false;
        }
        private FormCollection GetEmpInfo(FormCollection fc)
        {
            DynamicWS dws = new DynamicWS(@"http://myportal.wneweb.com.tw:8808/HR_WS/WS_USER/WebService.asmx", "QueryUserOrDeptByKeyWorld");
            dws.Params.Add("keyWorlds", fc["USER_ID"]);
            dws.Invoke();
            if (dws.dsResult != null)
            {
                string uInfo = dws.dsResult.Tables[0].Rows[1]["NAME"].ToString();

                fc["USER_NAME"] = uInfo.Substring(uInfo.IndexOf(',') + 1);
                fc["DEPT_NO"] = dws.dsResult.Tables[0].Rows[1]["PID"].ToString();
                fc["E_USER_NAME"] = uInfo.Substring(uInfo.IndexOf('-') + 1, uInfo.IndexOf(',') - uInfo.IndexOf('-') - 1);
                fc["EMAIL"] = uInfo.Substring(uInfo.IndexOf('-') + 1, uInfo.IndexOf(',') - uInfo.IndexOf('-') - 1).Replace(' ', '.') + @"@wnc.com.tw";
            }
            else
            {
                fc["USER_NAME"] = "NoInfo";
                fc["DEPT_NO"] = "NoInfo";
                fc["E_USER_NAME"] = "NoInfo";
                fc["EMAIL"] = "NoInfo";
            }
            return fc;
        }

        [HttpPost]
        public ActionResult AjaxEmpInfo()
        {
            var qr = Request["q"];
            var json = string.Empty;
            if (qr.ToString().Length >= 4)
            {
                DynamicWS dws = new DynamicWS(@"http://myportal.wneweb.com.tw:8808/HR_WS/WS_USER/WebService.asmx", "QueryUserOrDeptByKeyWorld");
                dws.Params.Add("keyWorlds", qr);
                dws.Invoke();
                if (dws.dsResult != null)
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    dt.Columns.Add("Name", Type.GetType("System.String"));

                    System.Data.DataView dv = dws.dsResult.Tables[0].DefaultView;
                    dv.RowFilter = "pid not in (0)";

                    foreach (System.Data.DataRow drt in dv.ToTable().Rows)
                    {
                        string uId = drt["ID"].ToString();
                        string uName = drt["UNM"].ToString();
                        if (uName.Contains(','))
                            uName = uName.Substring(uName.IndexOf(',') + 1, uName.Length - 1 - uName.IndexOf(','));
                        System.Data.DataRow dr = dt.NewRow();
                        dr["Name"] = uId + @"|" + uName;
                        dt.Rows.Add(dr);
                    }
                    json = JsonConvert.SerializeObject(dt);
                }
                else
                    json = @"[{Name:'NoInfo|NoInfo'}]";
            }
            else
                json = @"[{Name:'NoInfo|NoInfo'}]";

            return Content(json);

        }

        public ActionResult UserManagerDelete()
        {
            string key = this.Request["keys"];
            UserEdit iUserEdit = new UserEdit();
            UserEditForSQLServer iUserEditForSQLServer = new UserEditForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iUserEdit.Delete(new string[1] { key }) : iUserEditForSQLServer.Delete(new string[1] { key }));
            return Content(json);
        }

        public ActionResult BaseDataDelete()
        {
            string key = this.Request["keys"];
            BaseDataEdit iEdit = new BaseDataEdit();
            BaseDataEditForSQLServer iForSQLServer = new BaseDataEditForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iEdit.Delete(new string[1] { key }) : iForSQLServer.Delete(new string[1] { key }));
            return Content(json);
        }

        public ActionResult MenuManagerDelete()
        {
            string key = this.Request["keys"];
            MenuEdit iMenuEdit = new MenuEdit();
            MenuEditForSQLServer iMenuEditForSQLServer = new MenuEditForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iMenuEdit.Delete(new string[1] { key }) : iMenuEditForSQLServer.Delete(new string[1] { key }));
            return Content(json);
        }

        public ActionResult MenuManagerSave(FormCollection fc)
        {
            MenuEdit iMenuEdit = new MenuEdit();
            MenuEditForSQLServer iMenuEditForSQLServer = new MenuEditForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iMenuEdit.SaveOrUpdate(fc) : iMenuEditForSQLServer.SaveOrUpdate(fc));
            return Content(json);
        }

        public ActionResult RoleManagerSave(FormCollection fc)
        {
            RoleEdit iRoleEdit = new RoleEdit();
            RoleEditForSQLServer iRoleEditForSQLServer = new RoleEditForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iRoleEdit.SaveOrUpdate(fc) : iRoleEditForSQLServer.SaveOrUpdate(fc));
            return Content(json);
        }

        public ActionResult RoleManagerDelete()
        {
            string key = this.Request["keys"];
            RoleEdit iRoleEdit = new RoleEdit();
            RoleEditForSQLServer iRoleEditForSQLServer = new RoleEditForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iRoleEdit.Delete(new string[1] { key }) : iRoleEditForSQLServer.Delete(new string[1] { key }));
            return Content(json);
        }

        [HttpPost]
        public ActionResult BaseDataGetData(FormCollection fc)
        {
            BaseDataQuery iQuery = new BaseDataQuery();
            BaseDataQueryForSQLServer iQueryForSQLServer = new BaseDataQueryForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iQuery.Query(fc, this.Request["pageindex"], this.Request["pagesize"]) : iQueryForSQLServer.Query(fc, this.Request["pageindex"], this.Request["pagesize"]));
            return Content(json);
        }

        [HttpPost]
        public ActionResult WebLogGetData(FormCollection fc)
        {
            WebLogQuery iQuery = new WebLogQuery();
            WebLogQueryForSQLServer iQueryForSQLServer = new WebLogQueryForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iQuery.Query(fc, this.Request["pageindex"], this.Request["pagesize"]) : iQueryForSQLServer.Query(fc, this.Request["pageindex"], this.Request["pagesize"]));
            return Content(json);
        }

        [HttpPost]
        public ActionResult MenuManagerGetData(FormCollection fc)
        {
            MenuQuery iQuery = new MenuQuery();
            MenuQueryForSQLServer iQueryForSQLServer = new MenuQueryForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iQuery.Query(fc, this.Request["pageindex"], this.Request["pagesize"]) : iQueryForSQLServer.Query(fc, this.Request["pageindex"], this.Request["pagesize"]));
            return Content(json);
        }

        [HttpPost]
        public ActionResult RoleManagerGetData(FormCollection fc)
        {
            RoleQuery iQuery = new RoleQuery();
            RoleQueryForSQLServer iQueryForSQLServer = new RoleQueryForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iQuery.Query(fc, this.Request["pageindex"], this.Request["pagesize"]) : iQueryForSQLServer.Query(fc, this.Request["pageindex"], this.Request["pagesize"]));
            return Content(json); ;
        }

        [HttpPost]
        public ActionResult UserManagerGetData(FormCollection fc)
        {
            UserQuery iQuery = new UserQuery();
            UserQueryForSQLServer iQueryForSQLServer = new UserQueryForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iQuery.Query(fc, this.Request["pageindex"], this.Request["pagesize"]) : iQueryForSQLServer.Query(fc, this.Request["pageindex"], this.Request["pagesize"]));
            return Content(json); ;
        }


        public ActionResult UserManagerCombRoleID()
        {
            UserEdit iUserEdit = new UserEdit();
            UserEditForSQLServer iUserEditForSQLServer = new UserEditForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iUserEdit.GetComboxDataForRoleId() : iUserEditForSQLServer.GetComboxDataForRoleId());
            return Content(json);
        }

        public ActionResult UserManagerCombEnableFlag()
        {
            UserEdit iUserEdit = new UserEdit();
            var json = JsonConvert.SerializeObject(iUserEdit.GetComboxDataForEnableFlag());
            return Content(json);
        }

        public ActionResult MenuManagerCombParentID()
        {
            MenuEdit iMenuEdit = new MenuEdit();
            MenuEditForSQLServer iMenuEditForSQLServer = new MenuEditForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iMenuEdit.GetComboxDataForParentId() : iMenuEditForSQLServer.GetComboxDataForParentId());
            return Content(json);
        }

        public ActionResult MenuManagerCombRoleID()
        {
            MenuEdit iMenuEdit = new MenuEdit();
            MenuEditForSQLServer iMenuEditForSQLServer = new MenuEditForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iMenuEdit.GetComboxDataForRoleId() : iMenuEditForSQLServer.GetComboxDataForRoleId());
            return Content(json);
        }

        //public ActionResult BaseDataCombBaseDataType()
        //{
        //    BaseDataEdit iBaseDataEdit = new BaseDataEdit();
        //    BaseDataEditForSQLServer iBaseDataEditForSQLServer = new BaseDataEditForSQLServer();
        //    var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iBaseDataEdit.GetComboxDataForBaseDataType() : iBaseDataEditForSQLServer.GetComboxDataForBaseDataType());
        //    return Content(json);
        //}
        public ActionResult GetMenu()
        {
            MenuEdit iMenuEdit = new MenuEdit();
            MenuEditForSQLServer iMenuEditForSQLServer = new MenuEditForSQLServer();
            var json = JsonConvert.SerializeObject(ConfigManager.FrameWorkDataBaseType.Equals("0") ? iMenuEdit.GetMenu(Request["key"]) : iMenuEditForSQLServer.GetMenu(Request["key"]));
            return Content(json);
        }

        /// <summary>
        /// 前端异步校验
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckMenuName()
        {
            string mName = this.Request["name"];
            if (true)
                return Content("exist");
            else
                return Content("unexist");
        }

        [HttpPost]
        public ActionResult LogTip()
        {
            var qr = Request["q"];
            var json = JsonConvert.SerializeObject(GetData(qr));
            return Content(json);
        }

        private System.Data.DataTable GetData(string q)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("Name", Type.GetType("System.String"));
            for (int i = 0; i < 20; i++)
            {
                System.Data.DataRow dr = dt.NewRow();
                dr["Name"] = q + i + @"|" + i;
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}
