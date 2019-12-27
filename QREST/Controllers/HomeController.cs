using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using QREST.App_Logic;
using QREST.Models;
using QRESTModel.DAL;
using QRESTModel.DataTableGen;

namespace QREST.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new vmHomeIndex();
            model.T_QREST_SITES = db_Air.GetT_QREST_SITES_All_Display();

            return View(model);
        }


        public ActionResult SignUp()
        {
            var model = new vmHomeSignUp();
            return View(model);
        }


        public ActionResult About()
        {
            return View();
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }



        public ActionResult ReportDaily(Guid? id, int? month, int? year, int? day, string time)
        {
            var model = new vmHomeReportDaily
            {
                selSite = id,
                selMonth = month ?? System.DateTime.Now.Month,
                selYear = year ?? System.DateTime.Now.Year,
                selDay = day ?? System.DateTime.Now.Day,
                selTime = time ?? "L"
            };

            model.Results = db_Air.SP_RPT_DAILY(id ?? Guid.Empty, model.selMonth, model.selYear, model.selDay, model.selTime);

            return View(model);
        }

        [HttpPost]
        public ActionResult ReportDaily(vmHomeReportDaily model)
        {
            return RedirectToAction("ReportDaily", new { id = model.selSite, month = model.selMonth, year = model.selYear, day = model.selDay, time = model.selTime });
        }


        public ActionResult ReportDailyExport(Guid? id, int? month, int? year, int? day, string time)
        {
            DataTable dt = DataTableGen.ReportDaily(id ?? Guid.Empty, month ?? 1, year ?? System.DateTime.Now.Year, day ?? 1, time);
            if (dt.Rows.Count > 0)
            {
                MemoryStream ms = ExcelGen.GenExcelFromDataTables(dt, null, null, null);
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
            else
            {
                TempData["Error"] = "No data found to export";
                return RedirectToAction("ReportDaily", new { id, month, year, day, time });
            }

        }


        public ActionResult ReportMonthly(Guid? id, Guid? monid, int? month, int? year, string time)
        {
            var model = new vmHomeReportMonthly { 
                selSite = id,
                selMon = monid,
                selMonth = month ?? System.DateTime.Now.Month,
                selYear = year ?? System.DateTime.Now.Year,
                selTime = time ?? "L"
            };

            //monitor dropdown
            if (model.selSite != null)
                model.ddl_Mons = ddlHelpers.get_monitors_by_site(id ?? Guid.Empty);
            else
                model.ddl_Mons = new SelectList(Enumerable.Empty<SelectListItem>());

            model.Results = db_Air.SP_RPT_MONTHLY(model.selMon ?? Guid.Empty, model.selMonth, model.selYear, model.selTime);
            model.ResultSums = db_Air.SP_RPT_MONTHLY_SUMS(model.selMon ?? Guid.Empty, model.selMonth, model.selYear, model.selTime);

            if (model.selMon != null)
            {
                SiteMonitorDisplayType xxx = db_Air.GetT_QREST_MONITORS_ByID(model.selMon ?? Guid.Empty);
                if (xxx != null && xxx.T_QREST_MONITORS != null && xxx.T_QREST_MONITORS.COLLECT_UNIT_CODE != null)
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
                MemoryStream ms = ExcelGen.GenExcelFromDataTables(dt, null, null, null);
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
            else
            {
                TempData["Error"] = "No data found to export";
                return RedirectToAction("ReportMonthly", new { id, monid, month, year, time });
            }

        }


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
                model.ddl_Mons = ddlHelpers.get_monitors_by_site(id ?? Guid.Empty);
            else
                model.ddl_Mons = new SelectList(Enumerable.Empty<SelectListItem>());

            model.Results = db_Air.SP_RPT_ANNUAL(model.selMon ?? Guid.Empty, model.selYear, model.selTime);
            model.ResultSums = db_Air.SP_RPT_ANNUAL_SUMS(model.selMon ?? Guid.Empty, model.selYear, model.selTime);

            SiteMonitorDisplayType xxx = db_Air.GetT_QREST_MONITORS_ByID(model.selMon ?? Guid.Empty);
            if (xxx != null && xxx.T_QREST_MONITORS.COLLECT_UNIT_CODE != null)
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
                MemoryStream ms = ExcelGen.GenExcelFromDataTables(dt, null, null, null);
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
            else
            {
                TempData["Error"] = "No data found to export";
                return RedirectToAction("ReportMonthly", new { id, monid, month, year, time });
            }

        }


        public ActionResult Help()
        {
            var model = new vmHomeHelp();
            model.HelpTopics = db_Ref.GetT_QREST_HELP_DOCS();
            return View(model);
        }

        
        public ActionResult Terms ()
        {
            var model = new vmHomeTerms();
            T_QREST_APP_SETTINGS_CUSTOM cust = db_Ref.GetT_QREST_APP_SETTING_CUSTOM();
            model.TermsAndConditions = cust.TERMS_AND_CONDITIONS;
            return View(model);
        }


        public ActionResult Test()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    Uri myUri = new Uri("https://aqs.epa.gov/data/api/monitors/bySite?email=test@aqs.api&key=test&bdate=20000101&edate=20251231&state=41&county=011&site=1036", UriKind.Absolute);
                    var json = httpClient.GetStringAsync(myUri).Result;
                    dynamic stuff = JsonConvert.DeserializeObject(json);
                    foreach (var item in stuff.Data)
                    {
                        TempData["Success"] = "Read stuff success.";
                    }

                    
                }

            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                    db_Ref.CreateT_QREST_SYS_LOG(null, "ERROR", "Failed to import monitor - code 5 " + errInner);
            }
            catch (Exception ex)
            {
                db_Ref.CreateT_QREST_SYS_LOG(null, "ERROR", "Failed to import monitor - code 4 " + ex.Message);
            }

            return View();
        }

        public ActionResult Test2()
        {
            //HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://aqs.epa.gov/aqsweb/codes/qa/SitesV4.txt");
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://raw.githubusercontent.com/open-environment/QREST/master/QREST/Content/Docs/DeployGuide.txt");
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                using (StreamReader csvreader = new StreamReader(resp.GetResponseStream()))
                {
                    string currentLine;
                    while ((currentLine = csvreader.ReadLine()) != null)
                    {
                    }
                    TempData["Success"] = "Worked";
                 }
            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                    db_Ref.CreateT_QREST_SYS_LOG("", "ERROR", "Failed to import monitor - code 5 " + errInner);
                TempData["Error"] = "Unable to connect to AQS, please try again later.";
            }
            catch (Exception ex)
            {
                db_Ref.CreateT_QREST_SYS_LOG("", "ERROR", "Failed to import monitor - code 4 " + ex.Message);
                TempData["Error"] = "Unable to connect to AQS, please try again later.";
            }


            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult FetchMonitorsBySite(Guid? ID)
        {
            var data = ddlHelpers.get_monitors_sampled_by_site(ID ?? Guid.Empty);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}