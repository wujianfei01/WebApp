using BLL;
using BLL.MainDemo;
using DAL.Interface;
using ExcelPlugin;
using FilePreviewPlugin;
using Microsoft.Reporting.WebForms;
using MvcWebApp.Models.MainDemo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcWebApp.Controllers
{
    public class MainDemoController : Controller
    {
        public ActionResult RdlcReport()
        {
            DataTable dtHeader = GetHeaderData(true);
            DataTable dtBody = GetHeaderData(false);
            
            LocalReport localReport = new LocalReport();
            //localReport.ReportPath = Server.MapPath("~/Report/Rdlc/RdlcReportView.rdlc");
            //ReportDataSource reportDataSource = new ReportDataSource("DataSetForRdlc", dtHeader);
            //ReportDataSource reportDataSourceBody = new ReportDataSource("DataSetForRdlcBody", dtBody);
            localReport.ReportPath = Server.MapPath("~/Report/Rdlc/RdlcReportViewDemo.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dtHeader);
            ReportDataSource reportDataSourceBody = new ReportDataSource("DataSet2", dtBody);

            localReport.DataSources.Add(reportDataSource);
            localReport.DataSources.Add(reportDataSourceBody);

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx

            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + reportType + "</OutputFormat>" +
            "  <PageWidth>9.8in</PageWidth>" +
            "  <PageHeight>15in</PageHeight>" +
            "  <MarginTop>0.5in</MarginTop>" +
            "  <MarginLeft>1in</MarginLeft>" +
            "  <MarginRight>1in</MarginRight>" +
            "  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            //渲染报表
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=ReportDownload." + fileNameExtension);//通知浏览器下载
            return File(renderedBytes, (reportType.ToLower() == "image") ? "image/jpeg" : mimeType);
        }

        public ActionResult ECharts()
        {
            return View();
        }
        
        //
        // GET: /MainDemo/

        public ActionResult Index()
        {
            ViewBag.MyQueryAction = "/" + this.Request.RawUrl.Split('/')[1] + "/GetData";
            ViewBag.MyEditAction = "/" + this.Request.RawUrl.Split('/')[1] + "/Edit";
            ViewBag.MyAddAction = "/" + this.Request.RawUrl.Split('/')[1] + "/Edit";
            ViewBag.MyDeleteAction = "/" + this.Request.RawUrl.Split('/')[1] + "/Delete";
            return View();
        }

        public ActionResult Edit()
        {
            //前台傳值or后台數據庫取值
            ViewBag.key = this.Request["key"];
            ViewBag.MySaveAction = "/" + this.Request.RawUrl.Split('/')[1] + "/SaveOrUpdateData";
            return View();
        }

        public ActionResult IndexAnaEdit()
        {
            ViewBag.MyQueryAction = "/" + this.Request.RawUrl.Split('/')[1] + "/GetData";
            ViewBag.MyEditAction = "/" + this.Request.RawUrl.Split('/')[1] + "/Edit";
            ViewBag.MyAddAction = "/" + this.Request.RawUrl.Split('/')[1] + "/EditAnaEdit";
            ViewBag.MyDeleteAction = "/" + this.Request.RawUrl.Split('/')[1] + "/Delete";
            return View();
        }

        public ActionResult Delete()
        {
            string key = this.Request["keys"];
            MainQuery iQuery = new MainQuery();
            var json = JsonConvert.SerializeObject(iQuery.Delete(key));
            return Content(json);
        }

        public ActionResult EditAnaEdit()
        {
            //前台傳值or后台數據庫取值
            ViewBag.key = this.Request["key"];

            ViewBag.MySaveAction = "/" + this.Request.RawUrl.Split('/')[1] + "/SaveOrUpdateData";//为表头指定保存Action
            ViewBag.MyQueryAction4Header = "/" + this.Request.RawUrl.Split('/')[1] + "/GetData";//为表头form指定获得数据Action

            ViewBag.MySaveAction4ELine = "/" + this.Request.RawUrl.Split('/')[1] + "/SaveOrUpdateData";//为表体指定保存Action
            ViewBag.MyQueryAction4EditGridLine = "/" + this.Request.RawUrl.Split('/')[1] + "/GetData";//为表体指定获得数据Action

            MainQuery iQuery = new MainQuery();
            ViewBag.HeaderData = iQuery.GetHeader4Edit(ViewBag.key);

            return View();
        }

        [HttpPost]
        public ActionResult GetData(FormCollection fc)
        {
            MainQuery iQuery = new MainQuery();
            var json = JsonConvert.SerializeObject(iQuery.Query(fc, this.Request["pageindex"], this.Request["pagesize"]));
            return Content(json);
        }

        [HttpPost]
        public ActionResult SaveOrUpdateData(FormCollection fc)
        {
            MainEdit iEdit = new MainEdit();
            var json = JsonConvert.SerializeObject(iEdit.Edit(fc));
            return Content(json);
        }


        //
        // GET: /UploadDemo/

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return Content("請選擇文件!", "text/plain");
            }
            var fileName = Path.Combine(Request.MapPath("~/Upload"), Path.GetFileName(file.FileName));
            var prevFileDir = Request.MapPath("~/FilePreview/");
            try
            {
                if (!Directory.Exists(Request.MapPath("~/Upload")))
                    Directory.CreateDirectory(Request.MapPath("~/Upload"));
                if (!Directory.Exists(Request.MapPath("~/FilePreview")))
                    Directory.CreateDirectory(Request.MapPath("~/FilePreview"));
                file.SaveAs(fileName);
                DataSet ds = new InputAndOutput().UploadExcel(fileName);
                string htmlPrev = new FilePreview().PreviewExcel(fileName, prevFileDir, Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.LastIndexOf(@"/")).Substring(0, Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.LastIndexOf(@"/")).LastIndexOf(@"/")) + @"/FilePreview/");
                string fileDownload = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.LastIndexOf(@"/")).Substring(0, Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.LastIndexOf(@"/")).LastIndexOf(@"/")) + @"/Upload/" + Path.GetFileName(file.FileName);
                //return Content("Upload Success!" + JsonConvert.SerializeObject(ds.Tables[0]), "text/plain");//此處返回json
                ViewBag.Prev = htmlPrev;
                ViewBag.Download = fileDownload;
                return View("Upload");
            }
            catch (Exception ex)
            {
                return Content("Upload Error!" + ex.Message, "text/plain");//此處返回json
            }
        }

        /// <summary>
        /// 文件下載
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileResult GetFile(int id)
        {
            //Models.File file = dbConext.Files.Where(item => item.ID == id).FirstOrDefault();
            //List<string> typeList = new List<string>() { "png", "jpg", "jpeg" };
            //List<string> otherList = new List<string>() { "xls", "doc", "docx", "zip", "rar", "pdf" };
            //if (file == null)
            //{
            //    return null;
            //}
            //if (typeList.Contains(file.FileExtName))
            //{
            //    return new FileContentResult(file.FileContent, "image/jpg");
            //}
            //if (otherList.Contains(file.FileExtName))
            //{
            //    return File(file.FileContent, "application/octet-stream", file.FileName + "." + file.FileExtName);
            //}
            return null;
        }

        private DataTable GetHeaderData(bool flag)
        {
            if (flag)
            {
                DataTable dtHeader = new DataTable();

                dtHeader.Columns.Add("DataColumn1");
                dtHeader.Columns.Add("DataColumn2");
                dtHeader.Columns.Add("DataColumn3");
                dtHeader.Columns.Add("DataColumn4");
                dtHeader.Columns.Add("DataColumn5");
                dtHeader.Columns.Add("DataColumn6");

                DataRow dr = dtHeader.NewRow();
                for (int i = 0; i < 5; i++)
                {
                    dr["DataColumn1"] = "Pass1" + i;
                    dr["DataColumn2"] = "Pass2" + i;
                    dr["DataColumn3"] = "Pass3" + i;
                    dr["DataColumn4"] = "Pass4" + i;
                    dr["DataColumn5"] = "Pass5" + i;
                    dr["DataColumn6"] = "Pass6" + i;
                    dtHeader.Rows.Add(dr);
                    dr = dtHeader.NewRow();
                }
                return dtHeader;
            }
            else
            {
                DataTable dtBody = new DataTable();

                dtBody.Columns.Add("DataBody1");
                dtBody.Columns.Add("DataBody2");
                dtBody.Columns.Add("DataBody3");
                dtBody.Columns.Add("DataBody4");

                DataRow drBody = dtBody.NewRow();
                for (int i = 0; i < 5; i++)
                {
                    drBody["DataBody1"] = "PassBody1" + i;
                    drBody["DataBody2"] = "PassBody2" + i;
                    drBody["DataBody3"] = "PassBody3" + i;
                    drBody["DataBody4"] = "PassBody4" + i;
                    dtBody.Rows.Add(drBody);
                    drBody = dtBody.NewRow();
                }

                return dtBody;
            }
        }
    }
}
