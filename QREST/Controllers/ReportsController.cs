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
using QREST.Models;
using QREST.App_Logic.BusinessLogicLayer;

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
            string UserIDX = User.Identity.GetUserId();

            var model = new vmReportsExport
            {
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true),
                ddl_Monitor = new List<SelectListItem>()
            };

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Export2(string[] exportdata, vmReportsExport model)
        {
            //****validation **************
            if (exportdata == null)
            {
                TempData["Error"] = "You must select at least one option.";
                return RedirectToAction("Export", "Reports");
            }

            if (exportdata.Contains("Data") && string.IsNullOrEmpty(model.selOrgID))
            {
                TempData["Error"] = "Select an organization.";
                return RedirectToAction("Export", "Reports");
            }
            //***end validation ***********

            string UserIDX = User.Identity.GetUserId();

            DataTable dtSites = exportdata.Contains("Sites") ? DataTableGen.SitesByUser(UserIDX) : new DataTable("Sites");
            DataTable dtMonitors = exportdata.Contains("Monitors") ? DataTableGen.MonitorsByUser(UserIDX) : new DataTable("Monitors");

            //raw data
            DataTable dtData = new DataTable("Data");
            if (exportdata.Contains("Data"))
            {
                string[] d = model.selDate.Replace(" - ", "z").Split('z');
                if (d.Length == 2)
                {
                    DateTime? d1 = d[0].ConvertOrDefault<DateTime?>();
                    DateTime? d2 = (d.Length > 1) ? d[1].ConvertOrDefault<DateTime?>() : null;
                    dtData = DataTableGen.RawData(model.selType, model.selOrgID, null, model.selMon, d1.GetValueOrDefault(), d2.GetValueOrDefault());
                }
            }

            //Polling Config
            DataTable dtPollingConfig = new DataTable("Polling_Config");
            DataTable dtPollingConfigDetail = new DataTable("Polling_Config_Detail");
            if (exportdata.Contains("ProfileConfig"))
            {
                dtPollingConfig = DataTableGen.GetPollingConfig(UserIDX);
                dtPollingConfigDetail = DataTableGen.GetPollingConfigDetail(UserIDX);
            }

            DataSet dsExport = DataTableGen.DataSetFromDataTables(new List<DataTable> { dtSites, dtMonitors, dtData, dtPollingConfig, dtPollingConfigDetail });
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