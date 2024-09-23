using QREST.App_Logic;
using QREST.Models;
using QRESTModel.DAL;
using QRESTModel.DataTableGen;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QREST.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new vmHomeIndex
            {
                T_QREST_SITES = db_Air.GetT_QREST_SITES_All_Display()
            };

            return View(model);
        }


        [HttpGet]
        public ActionResult SignUp()
        {
            var model = new vmHomeSignUp();
            model.TestUrl = db_Ref.GetT_QREST_APP_SETTING("TEST_URL");
            return View(model);
        }


        public ActionResult About()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [HttpGet]
        public async Task<ActionResult> ReportDaily(Guid? id, int? month, int? year, int? day, string time)
        {
            var model = new vmHomeReportDaily
            {
                selSite = id,
                selMonth = month ?? System.DateTime.Now.Month,
                selYear = year ?? System.DateTime.Now.Year,
                selDay = day ?? System.DateTime.Now.AddHours(-7).Day,  //hack since server is in UTC
                selTime = time ?? "L",
                currServerDateTime = System.DateTime.Now
            };

            if (model.selSite != Guid.Empty && month != null && year != null && day != null)
                model.Results = await db_Air.SP_RPT_DAILY(id ?? Guid.Empty, model.selMonth, model.selYear, model.selDay, model.selTime);

            return View(model);
        }


        [HttpPost]
        public ActionResult ReportDaily(vmHomeReportDaily model)
        {
            return RedirectToAction("ReportDaily", new { id = model.selSite, month = model.selMonth, year = model.selYear, day = model.selDay, time = model.selTime });
        }


        [HttpGet]
        public async Task<ActionResult> ReportDailyExport(Guid? id, int? month, int? year, int? day, string time)
        {
            DataTable dt = await DataTableGen.ReportDaily(id ?? Guid.Empty, month ?? 1, year ?? System.DateTime.Now.Year, day ?? 1, time);
            if (dt.Rows.Count > 0)
            {
                MemoryStream ms = ExcelGen.GenExcelFromDataTables(new List<DataTable> { dt });
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=QREST_Daily.xlsx");
                ms.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();

                return null;
            }

            TempData["Error"] = "No data found to export";
            return RedirectToAction("ReportDaily", new { id, month, year, day, time });

        }


        [HttpGet]
        public ActionResult ReportMonthly(Guid? id, Guid? monid, int? month, int? year, string time)
        {
            var model = new vmHomeReportMonthly { 
                selSite = id,
                selMon = monid,
                selMonth = month ?? DateTime.Now.Month,
                selYear = year ?? DateTime.Now.Year,
                selTime = time ?? "L"
            };

            //monitor dropdown
            if (model.selSite != null)
                model.ddl_Mons = ddlHelpers.get_monitors_by_site(id ?? Guid.Empty, false, true);
            else
                model.ddl_Mons = new SelectList(Enumerable.Empty<SelectListItem>());

            model.Results = db_Air.SP_RPT_MONTHLY(model.selMon ?? Guid.Empty, model.selMonth, model.selYear, model.selTime);
            model.ResultSums = db_Air.SP_RPT_MONTHLY_SUMS(model.selMon ?? Guid.Empty, model.selMonth, model.selYear, model.selTime);

            //display units
            if (model.selMon != null)
            {
                SiteMonitorDisplayType xxx = db_Air.GetT_QREST_MONITORS_ByID(model.selMon ?? Guid.Empty);
                if (xxx?.T_QREST_MONITORS?.COLLECT_UNIT_CODE != null)
                {
                    var yyy = db_Ref.GetT_QREST_REF_UNITS_ByID(xxx.T_QREST_MONITORS.COLLECT_UNIT_CODE);
                    model.Units = yyy?.UNIT_DESC;
                }
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult ReportMonthly(vmHomeReportMonthly model)
        {
            return RedirectToAction("ReportMonthly", new { id = model.selSite, monid = model.selMon, month = model.selMonth, year = model.selYear, time = model.selTime });
        }


        public ActionResult ReportMonthlyExport(Guid? id, Guid? monid, int? month, int? year, string time)
        {
            DataTable dt = DataTableGen.ReportMonthly(monid ?? Guid.Empty, month ?? 1, year ?? 0, time);
            if (dt.Rows.Count > 0)
            {
                MemoryStream ms = ExcelGen.GenExcelFromDataTables(new List<DataTable> { dt });
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=QREST_Monthly.xlsx");
                ms.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();

                return null;
            }

            TempData["Error"] = "No data found to export";
            return RedirectToAction("ReportMonthly", new { id, monid, month, year, time });

        }


        [HttpGet]
        public ActionResult ReportAnnual(Guid? id, Guid? monid, int? year, string time)
        {
            var model = new vmHomeReportAnnual
            {
                selSite = id,
                selMon = monid,
                selYear = year ?? System.DateTime.Now.Year,
                selTime = time ?? "L"
            };

            //monitor dropdown
            if (model.selSite != null)
                model.ddl_Mons = ddlHelpers.get_monitors_by_site(id ?? Guid.Empty, false, true);
            else
                model.ddl_Mons = new SelectList(Enumerable.Empty<SelectListItem>());

            model.Results = db_Air.SP_RPT_ANNUAL(model.selMon ?? Guid.Empty, model.selYear, model.selTime);
            model.ResultSums = db_Air.SP_RPT_ANNUAL_SUMS(model.selMon ?? Guid.Empty, model.selYear, model.selTime);

            SiteMonitorDisplayType xxx = db_Air.GetT_QREST_MONITORS_ByID(model.selMon ?? Guid.Empty);
            if (xxx?.T_QREST_MONITORS.COLLECT_UNIT_CODE != null)
            {
                var yyy = db_Ref.GetT_QREST_REF_UNITS_ByID(xxx.T_QREST_MONITORS.COLLECT_UNIT_CODE);
                model.Units = yyy.UNIT_DESC;
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult ReportAnnual(vmHomeReportAnnual model)
        {
            return RedirectToAction("ReportAnnual", new { id = model.selSite, monid = model.selMon, year = model.selYear, time = model.selTime });
        }


        public ActionResult ReportAnnualExport(Guid? id, Guid? monid, int? month, int? year, string time)
        {
            DataTable dt = DataTableGen.ReportAnnual(monid ?? Guid.Empty, year ?? 0, time);
            if (dt.Rows.Count > 0)
            {
                MemoryStream ms = ExcelGen.GenExcelFromDataTables(new List<DataTable> { dt });
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=QREST_Annual.xlsx");
                ms.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();

                return null;
            }

            TempData["Error"] = "No data found to export";
            return RedirectToAction("ReportMonthly", new { id, monid, month, year, time });

        }


        [HttpGet]
        public ActionResult Help(string s)
        {
            //s with come in with -
            var model = new vmHomeHelp();
            model.HelpTopicsLeft = db_Ref.GetT_QREST_HELP_DOCS_Grouped();
            model.HelpTopics = db_Ref.GetT_QREST_HELP_DOCS_GroupedByID(s?.Replace("-"," "));
            model.selSection = s;
            model.selCat = model.HelpTopics.FirstOrDefault().HELP_CAT;
            
            return View(model);
        }


        [HttpGet]
        public ActionResult Terms()
        {
            var model = new vmHomeTerms();
            T_QREST_APP_SETTINGS_CUSTOM cust = db_Ref.GetT_QREST_APP_SETTING_CUSTOM();
            model.TermsAndConditions = cust.TERMS_AND_CONDITIONS;
            return View(model);
        }


        [HttpGet]
        public JsonResult FetchMonitorsBySite(Guid? ID)
        {
            var data = ddlHelpers.get_monitors_sampled_by_site(ID ?? Guid.Empty);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult PushDataToQREST()
        {
            return View();            
        }
    }
}