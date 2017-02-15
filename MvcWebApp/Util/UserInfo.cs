using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Util
{
    /// <summary>
    /// 當前用戶信息
    /// </summary>
    public class UserInfo
    {
        private static string _tUserId;
        public static string TUserId
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigManager.ReadValueByKey(ConfigurationFile.WebConfig, "TestEmpNo")))
                {
                    WindowsPrincipal myPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                    UserInfo._tUserId = System.Web.HttpContext.Current.Request.LogonUserIdentity.Name;
                    //UserInfo._tUserId = System.Web.HttpContext.Current.User.Identity.Name;
                    if (!string.IsNullOrEmpty(UserInfo._tUserId) && UserInfo._tUserId.IndexOf(@"\") != -1)
                    {
                        UserInfo._tUserId = UserInfo._tUserId.Substring(UserInfo._tUserId.IndexOf(@"\") + 1);
                    }
                    else if (myPrincipal != null)
                    {
                        UserInfo._tUserId = myPrincipal.Identity.Name;
                        UserInfo._tUserId = UserInfo._tUserId.Substring(UserInfo._tUserId.IndexOf(@"\") + 1);
                    }
                }
                else
                    UserInfo._tUserId = ConfigManager.ReadValueByKey(ConfigurationFile.WebConfig, "TestEmpNo");

                return UserInfo._tUserId;
            }
        }

        private static string _eUserName;
        /// <summary>
        /// 員工英文名稱
        /// </summary>
        public static string EUserName
        {
            get { return UserInfo._eUserName; }
            set { UserInfo._eUserName = value; }
        }

        private static string _userName;
        /// <summary>
        /// 用户名
        /// </summary>
        public static string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private static int _empNo;

        /// <summary>
        /// 工號
        /// </summary>
        public static int EmpNo
        {
            get { return _empNo; }
            set { _empNo = value; }
        }

        private static string _deptNo;

        /// <summary>
        /// 部門號
        /// </summary>
        public static string DeptNo
        {
            get { return _deptNo; }
            set { _deptNo = value; }
        }


    }
}
