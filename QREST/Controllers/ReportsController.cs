using ClosedXML.Excel;
using Microsoft.AspNet.Identity;
using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QRESTModel.DataTableGen;
using QREST.App_Logic;

namespace QREST.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Export()
        {
            return View();
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Export2(string[] exportdata)
        {
            //****validation **************
            if (exportdata == null)
            {
                TempData["Error"] = "You must select at least one option.";
                return RedirectToAction("Export", "Reports");
            }
            //***end validation ***********

            string UserIDX = User.Identity.GetUserId();

            DataTable dtSites = exportdata.Contains("Sites") ? DataTableGen.SitesByUser(UserIDX) : new DataTable("Sites");
            DataTable dtMonitors = exportdata.Contains("Monitors") ? DataTableGen.MonitorsByUser(UserIDX) : new DataTable("Monitors");
            DataSet dsExport = DataTableGen.DataSetFromDataTables(dtSites, dtMonitors, null, null);
            if (dsExport.Tables.Count > 0)
            {
                MemoryStream ms = ExcelGen.GenExcelFromDataSet(dsExport);
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=QRESTExport.xlsx");
                ms.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();

                return null;
            }
            else
            {
                TempData["Error"] = "No data found to export";
                return RedirectToAction("Export");
            }
        }


    }
}