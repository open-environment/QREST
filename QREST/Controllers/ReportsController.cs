using Microsoft.AspNet.Identity;
using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

            var _dts = new List<DataTable>();

            string UserIDX = User.Identity.GetUserId();

            if (exportdata.Contains("Sites"))
                _dts.Add(DataTableGen.SitesByUser(UserIDX, model.selOrgID));

            if (exportdata.Contains("Monitors"))
                _dts.Add(DataTableGen.MonitorsByUser(UserIDX, model.selOrgID));

            //Polling Config
            if (exportdata.Contains("PollingConfig"))
            {
                _dts.Add(DataTableGen.GetPollingConfig(UserIDX, model.selOrgID));
                _dts.Add(DataTableGen.GetPollingConfigDetail(UserIDX, model.selOrgID));
            }

            //raw data
            DataTable dtData = new DataTable("Data");
            if (exportdata.Contains("Data"))
            {
                string[] d = model.selDate.Replace(" - ", "z").Split('z');
                if (d.Length == 2)
                {
                    DateTime? d1 = d[0].ConvertOrDefault<DateTime?>();
                    DateTime? d2 = (d.Length > 1) ? d[1].ConvertOrDefault<DateTime?>() : null;

                    _dts.Add(DataTableGen.RawData(model.selType, model.selOrgID, null, model.selMon, d1.GetValueOrDefault(), d2.GetValueOrDefault(), model.selTimeType));

                    //daily averages
                    if (model.chkDaily)
                        _dts.Add(DataTableGen.DailyAverages(model.selMon, d1.GetValueOrDefault(), d2.GetValueOrDefault(), model.selTimeType));

                    //monthly stats
                    if (model.chkMonthly)
                        _dts.Add(DataTableGen.MonthlyStatistics(model.selMon, d1.GetValueOrDefault(), d2.GetValueOrDefault(), model.selTimeType));
                }
            }



            DataSet dsExport = DataTableGen.DataSetFromDataTables(_dts);
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


        public ActionResult Daily(string org, Guid? mon, string typ, string timtyp, string sDate)
        {
            string UserIDX = User.Identity.GetUserId();
            var model = new vmReportsDaily
            {
                selOrgID = org,
                selMon = mon.GetValueOrDefault(),
                selTimeType = timtyp,
                selDate = sDate,
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true),
                ddl_Monitor = new List<SelectListItem>()
            };

            if (mon != null && sDate != null)
            {
                string[] d = sDate.Replace(" - ", "z").Split('z');
                if (d.Length == 2)
                {
                    DateTime d1 = d[0].ConvertOrDefault<DateTime>();
                    DateTime d2 = d[1].ConvertOrDefault<DateTime>().AddDays(1).AddHours(-1);
                    model.stats = db_Air.SP_DAILY_AVGS(d1, d2, model.selMon);
                }

            }

            //supply mon ddl
            if (model.selOrgID != null)
                model.ddl_Monitor = ddlHelpers.get_monitors_sampled_by_org(model.selOrgID);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Daily(vmReportsDaily model)
        {
            return RedirectToAction("Daily", "Reports", new { org = model.selOrgID, mon = model.selMon, sDate = model.selDate, timtyp = model.selTimeType });
        }

        public ActionResult MonthlyStats(string org, Guid? mon, DateTime? sDate, DateTime? eDate)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmReportsMonthlyStats {
                selOrgID = org,
                selMon = mon.GetValueOrDefault(),
                selDate = sDate ?? new DateTime(System.DateTime.Now.Year,1,1),
                endDate = eDate != null ? (eDate.GetValueOrDefault().Day == 1 ? eDate.GetValueOrDefault().AddMonths(1).AddHours(-1) : eDate.GetValueOrDefault()) : new DateTime(System.DateTime.Now.Year, 1, 1).AddYears(1).AddHours(-1),
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true),
                ddl_Monitor = new List<SelectListItem>()
            };

            //get org and site from supplied mon
            if (model.selMon != Guid.Empty)
            {
                SiteMonitorDisplayType _m = db_Air.GetT_QREST_MONITORS_ByID(model.selMon);
                if (_m != null)
                {
                    model.selOrgID = _m.ORG_ID;
                    model.ddl_Monitor = ddlHelpers.get_monitors_sampled_by_org(model.selOrgID);
                }
            }

            //supply dates
            if (mon != null && sDate != null && eDate != null)
                model.stats = db_Air.SP_MONTHLY_STATS(model.selDate, model.endDate, model.selMon);

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MonthlyStats(vmReportsMonthlyStats model)
        {            
            return RedirectToAction("MonthlyStats", "Reports", new { org = model.selOrgID, mon = model.selMon, sDate = model.selDate, eDate = model.endDate });
        }


        //************************************* LOGGING************************************************************
        public ActionResult LogActivity()
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmSharedLogActivity {
                ddl_Organization = ddlHelpers.get_ddl_my_organizations_admin(UserIDX)
            };
            return View(model);
        }





        //************************************* USAGE STATS ************************************************************
        public ActionResult UsageStats(int? yr)
        {
            var model = new vmReportUsageStats {
                ddl_Years = ddlHelpers.get_ddl_years(2015),
                selYear = yr ?? System.DateTime.Now.Year
            };

            if (model.selYear != null)
            {
                model.HourlyCnts = db_Air.GetMONTHLY_USAGE_HOURLY(model.selYear.GetValueOrDefault());
                model.FiveMinCnts = db_Air.GetMONTHLY_USAGE_FIVEMIN(model.selYear.GetValueOrDefault());
            }
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UsageStats(vmReportUsageStats model)
        {
            return RedirectToAction("UsageStats", "Reports", new { yr = model.selYear });
        }




        //************************************* REF DATA ************************************************************
        public ActionResult RefCollFreq()
        {
            return View();
        }


        [HttpPost]
        public ActionResult RefCollFreqData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            List<T_QREST_REF_COLLECT_FREQ> data = db_Ref.GetT_QREST_REF_COLLECT_FREQ_data(pageSize, start, orderColName, orderDir);
            var recordsTotal = db_Ref.GetT_QREST_REF_COLLECT_FREQ().Count();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        public ActionResult RefDuration()
        {
            return View();
        }


        [HttpPost]
        public ActionResult RefDurationData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            List<T_QREST_REF_DURATION> data = db_Ref.GetT_QREST_REF_DURATION_data(pageSize, start, orderCol, orderDir);
            var recordsTotal = db_Ref.GetT_QREST_REF_DURATION().Count();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        [HttpGet]
        public ActionResult RefPar()
        {
            var model = new vmReportsRefPar();
            model.editPAR_CODE = db_Ref.GetT_QREST_REF_PARAMETERS_NextNonAQS();
            return View(model);
        }


        [HttpPost]
        public ActionResult RefParData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            List<T_QREST_REF_PARAMETERS> data = db_Ref.GetT_QREST_REF_PARAMETERS_data(pageSize, start, orderColName, orderDir);
            var recordsTotal = db_Ref.GetT_QREST_REF_PARAMETERS().Count();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RefParEdit(vmReportsRefPar model)
        {
            if (ModelState.IsValid)
            {
                bool succInd = db_Ref.InsertUpdatetT_QREST_REF_PARAMETERS(model.editPAR_CODE, model.editPAR_NAME, null, null, model.editSTD_UNIT_CODE, false, true, User.Identity.GetUserId());

                if (succInd)
                    TempData["Success"] = "Record updated";
                else
                    TempData["Error"] = "Error updating record.";
            }
            else
                TempData["Error"] = "Error updating record";

            return RedirectToAction("RefPar", "Reports");
        }


        [HttpPost]
        public JsonResult RefParDelete(string id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                if (db_Ref.GetT_QREST_REF_PAR_METHODS_CountByPar(id))
                    return Json("This record cannot be deleted because a parameter method exists for it, which must be deleted first.");

                int succId = db_Ref.DeleteT_QREST_REF_PARAMETERS(id);
                if (succId == 1)
                    return Json("Success");
                else
                    return Json("This record cannot be deleted. There may be data or a monitor configure to use it.");
            }
        }


        [HttpGet]
        public ActionResult RefParMethod()
        {
            var model = new vmReportsRefParMethod();
            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RefParMethod(vmReportsRefParMethod model)
        {
            if (ModelState.IsValid && model.editPAR_METHOD_IDX != null)
            {
                Tuple<string, string> SuccInd = db_Ref.InsertUpdateT_QREST_REF_PAR_METHODS(model.editPAR_METHOD_IDX, null, null, null, model.editCOLLECTION_DESC, null, null, null, null, null, null, null,
                    model.editCUST_MIN_VALUE ?? -9999, model.editCUST_MAX_VALUE ?? -9999, User.Identity.GetUserId());

                if (SuccInd.Item1 == "U")
                    TempData["Success"] = "Record updated";
                else
                    TempData["Error"] = "Error updating record.";
            }
            else
                TempData["Error"] = "Error updating record";

            return RedirectToAction("RefParMethod", "Reports");
        }


        [HttpPost]
        public ActionResult RefParMethodData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #

            //data filters
            string selSearch = Request.Form.GetValues("selSearch")?.FirstOrDefault();

            List<RefParMethodDisplay> data = db_Ref.GetT_QREST_REF_PAR_METHODS_Search(selSearch, null, pageSize, start);
            var recordsTotal = db_Ref.GetT_QREST_REF_PAR_METHODS_Count(selSearch, null);
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        [HttpPost]
        public JsonResult RefParMethDelete(string id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                Guid idg = Guid.Parse(id);
                if (db_Air.GetT_QREST_MONITORS_AnyByParMethodIdx(idg))
                    return Json("This record cannot be deleted because a monitor is currently using it. Delete the site's monitor first.");

                if (db_Ref.DeleteT_QREST_REF_PAR_METHODS(idg))
                    return Json("Success");
                else
                    return Json("This record cannot be deleted. There may be data or a monitor configure to use it.");
            }
        }


        public ActionResult ExportExcel(string act)
        {
            DataTable dt = null;
            if (act == "RefParMethod")
                dt = DataTableGen.RefParMethod("", "");
            else if (act == "RefPar")
                dt = DataTableGen.RefPar();

            DataSet dsExport = DataTableGen.DataSetFromDataTables(new List<DataTable> { dt });
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
                return RedirectToAction(act);
            }
        }


        public ActionResult RefParUnit()
        {
            return View();
        }


        [HttpPost]
        public ActionResult RefParUnitData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            List<RefParUnitDisplay> data = db_Ref.GetT_QREST_REF_PAR_UNITS_data(pageSize, start, orderColName, orderDir);
            var recordsTotal = db_Ref.GetT_QREST_REF_PAR_UNITS_count();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        public ActionResult RefUnit()
        {
            return View();
        }


        [HttpPost]
        public ActionResult RefUnitData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            List<T_QREST_REF_UNITS> data = db_Ref.GetT_QREST_REF_UNITS_data(pageSize, start, orderColName, orderDir);
            var recordsTotal = db_Ref.GetT_QREST_REF_UNITS(null).Count();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        public ActionResult RefQualifier()
        {
            return View();
        }


        [HttpPost]
        public ActionResult RefQualifierData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            List<T_QREST_REF_QUALIFIER> data = db_Ref.GetT_QREST_REF_QUALIFIER_data(pageSize, start, orderColName, orderDir);
            var recordsTotal = db_Ref.GetT_QREST_REF_QUALIFIERCount();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }



        public ActionResult RefDisallowQual()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RefDisallowQualData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            List<T_QREST_REF_QUAL_DISALLOW> data = db_Ref.GetT_QREST_REF_QUAL_DISALLOW_data(pageSize, start, orderColName, orderDir);
            var recordsTotal = db_Ref.GetT_QREST_REF_QUAL_DISALLOWCount();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


    }
}