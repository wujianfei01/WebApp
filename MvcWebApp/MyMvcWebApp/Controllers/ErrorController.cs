using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcWebApp.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SysError()
        {
            return View();
        }
    }
}
