using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using QREST.App_Logic.BusinessLogicLayer;
using QREST.Models;
using QRESTModel.DAL;
using QRESTModel.AQSHelper;

namespace QREST.Controllers
{
    [Authorize]
    public class DataController : Controller
    {
        // GET: Data
        public ActionResult Index()
        {
            return View();
        }

        #region MANUAL IMPORT

        public ActionResult ManualImport()
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataImport {
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true),
                ddl_Sites = new List<SelectListItem>(),
                ddl_Monitors = new List<SelectListItem>(),
                ddl_PollConfig = new List<SelectListItem>(),
                ddl_ImportType = ddlHelpers.get_ddl_import_type(),
                ddl_Time = ddlHelpers.get_ddl_time_type(),
                ddl_TimeZone = ddlHelpers.get_ddl_time_zone(),
                ddl_Calc = ddlHelpers.get_ddl_yes_no(),
                selTimeType = "L"
        };

            return View(model);
        }


        [HttpPost]
        public ActionResult ManualImport(vmDataImport model) {

            string UserIDX = User.Identity.GetUserId();
            Guid importID = Guid.NewGuid();
            model.IMPORT_IDX = null;
            model.ImportSuccCount = 0;

            model.error_data = new List<ImportResponse>();

            //*********************** ADDITIONAL CUSTOM MODEL ERRORS **************************************8
            if (model.selImportType == "F" && model.selTimeType == "L")
                ModelState.AddModelError("selTimeType", "Local time import not supported for 5 minute data.");
            if (model.selImportType == "H1" && string.IsNullOrEmpty(model.selTimeZone))
                ModelState.AddModelError("selTimeZone", "Local Time Zone required for this import type.");

            if (ModelState.IsValid)            
            {
                Guid importIDX = Guid.NewGuid();

                //**************************************************************************************
                //    F                five-minute
                //**************************************************************************************
                if (model.selImportType == "F")
                {
                    T_QREST_SITE_POLL_CONFIG _pollConfig = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(model.selPollConfig ?? Guid.Empty);
                    if (_pollConfig != null)
                    {
                        char[] delimiter = _pollConfig.DELIMITER == "C" ? new char[] { ',' } : new char[] { '\t' };

                        if (_pollConfig.DATE_COL != null && _pollConfig.TIME_COL != null)
                        {
                            int dateCol = (_pollConfig.DATE_COL ?? 2) - 1;
                            int timeCol = (_pollConfig.TIME_COL ?? 3) - 1;
                            string[] allowedFormats = new[] { "MM/dd/yyyy HH:mm", "M/dd/yyyy HH:mm", "MM/d/yyyy HH:mm", "M/d/yyyy HH:mm", "MM/dd/yyyy H:mm", "M/dd/yyyy H:mm", "MM/d/yyyy H:mm", "M/d/yyyy H:mm" };

                            //get polling config dtl
                            List<SitePollingConfigDetailType> _pollConfigDtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_pollConfig.POLL_CONFIG_IDX);

                            //import
                            foreach (string row in model.IMPORT_BLOCK.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                //split row's columns into string array
                                string[] cols = row.Split(delimiter, StringSplitOptions.None);
                                if (cols.Length > 2) //skip blank rows
                                {
                                    DateTime dt;

                                    if (DateTime.TryParseExact(cols[dateCol] + " " + cols[timeCol], allowedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                    {
                                        foreach (SitePollingConfigDetailType _item in _pollConfigDtl)
                                        {
                                            if (_item.COLLECT_UNIT_CODE != null && _item.COL != null && string.IsNullOrEmpty(cols[(_item.COL ?? 1) - 1].ToString()) == false)
                                            {
                                                string val = cols[(_item.COL ?? 1) - 1].ToString();

                                                ImportResponse xxx = db_Air.ImportT_QREST_DATA_FIVE_MIN(_item.MONITOR_IDX, dt, double.TryParse(val, out _) ? val : null, _item.COLLECT_UNIT_CODE, model.selCalc == "N" ? true : false, "", model.selCalc == "N" ? new DateTime(1888, 8, 8) : System.DateTime.Now, importIDX);
                                                if (xxx.SuccInd)
                                                    model.ImportSuccCount += 1;
                                                else
                                                    model.error_data.Add(xxx);
                                            }
                                            else
                                                ModelState.AddModelError("selMonitor", "No collection unit defined for " + _item.PAR_CODE + ". Record not imported.");
                                        }
                                    }
                                }

                                if (model.ImportSuccCount == 0)
                                    ModelState.AddModelError("IMPORT_BLOCK", "No data in expected format found.");
                                else
                                    model.IMPORT_IDX = importID;
                            }
                        }
                        else
                            ModelState.AddModelError("selPollConfig", "Selected polling config does not define date and/or time column.");
                    }
                    else
                        ModelState.AddModelError("selPollConfig", "No polling configuration found.");
                }


                //**************************************************************************************
                //    H                hourly
                //**************************************************************************************
                else if (model.selImportType == "H")
                {
                    T_QREST_SITE_POLL_CONFIG _pollConfig = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(model.selPollConfig ?? Guid.Empty);
                    if (_pollConfig != null)
                    {
                        int tzOffset = _pollConfig.LOCAL_TIMEZONE.ConvertOrDefault<int>();
                        char[] delimiter = _pollConfig.DELIMITER == "C" ? new char[] { ',' } : new char[] { '\t' };

                        if (_pollConfig.DATE_COL != null && _pollConfig.TIME_COL != null)
                        {
                            int dateCol = (_pollConfig.DATE_COL ?? 2) - 1;
                            int timeCol = (_pollConfig.TIME_COL ?? 3) - 1;
                            string dateFormat = _pollConfig.DATE_FORMAT + " " + _pollConfig.TIME_FORMAT.Replace("MM","mm");
                            string[] allowedFormats = new[] { "MM/dd/yyyy HH:mm", "M/dd/yyyy HH:mm", "MM/d/yyyy HH:mm", "M/d/yyyy HH:mm", "MM/dd/yyyy H:mm", "M/dd/yyyy H:mm", "MM/d/yyyy H:mm", "M/d/yyyy H:mm" };

                            //get polling config dtl
                            List<SitePollingConfigDetailType> _pollConfigDtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_pollConfig.POLL_CONFIG_IDX);

                            //import
                            foreach (string row in model.IMPORT_BLOCK.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                //split row's columns into string array
                                string[] cols = row.Split(delimiter, StringSplitOptions.None);
                                if (cols.Length > 2) //skip blank rows
                                {
                                    DateTime dt;

                                    if (DateTime.TryParseExact(cols[dateCol] + " " + cols[timeCol], allowedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                    {
                                        foreach (SitePollingConfigDetailType _item in _pollConfigDtl)
                                        {
                                            if (_item.COLLECT_UNIT_CODE != null && _item.COL != null && string.IsNullOrEmpty(cols[(_item.COL ?? 1) - 1].ToString()) == false)
                                            {
                                                string val = cols[(_item.COL ?? 1) - 1].ToString();
                                                ImportResponse xxx = db_Air.InsertUpdateT_QREST_DATA_HOURLY(_item.MONITOR_IDX, model.selTimeType == "L" ? dt : (DateTime?)null, model.selTimeType == "U" ? dt : (DateTime?)null, tzOffset, double.TryParse(val, out _) ? val : null, _item.COLLECT_UNIT_CODE, true, double.TryParse(val, out _) ? null : val, importID);
                                                if (xxx.SuccInd)
                                                    model.ImportSuccCount += 1;
                                                else
                                                    model.error_data.Add(xxx);
                                            }
                                            else
                                                ModelState.AddModelError("selMonitor", "No collection unit defined for " + _item.PAR_CODE + ". Record not imported.");
                                        }
                                    }
                                    else
                                        ModelState.AddModelError("IMPORT_BLOCK", "Date and/or time format cannot be read.");
                                }

                                if (model.ImportSuccCount == 0)
                                    ModelState.AddModelError("IMPORT_BLOCK", "No data in expected format found.");
                                else
                                    model.IMPORT_IDX = importID;
                            }
                        }
                        else
                            ModelState.AddModelError("selPollConfig", "Selected polling config does not define date and/or time column.");
                    }
                    else
                        ModelState.AddModelError("selPollConfig", "No polling configuration found.");

                }


                //**************************************************************************************
                //    H1                hourly (1 parameter with hours arranged as columns)
                //**************************************************************************************
                else if (model.selImportType == "H1")
                {
                    //insert/upadte POLL_CONFIG
                    Guid? h1SuccID = db_Air.InsertUpdatetT_QREST_SITE_POLL_CONFIG(null, model.selSite, "H1", null, "H1", null, null, null, null, null, null, null, null, null, model.selTimeZone, false, UserIDX, null, model.selTimeType, false);
                    if (h1SuccID != null)
                    {
                        //if update succeeded, continue
                        int tzOffset = model.selTimeZone.ConvertOrDefault<int>();

                        T_QREST_MONITORS _monitor = db_Air.GetT_QREST_MONITORS_ByID_Simple(model.selMonitor ?? Guid.Empty);
                        if (_monitor != null && _monitor.COLLECT_UNIT_CODE != null)
                        {
                            bool impAny = false;
                            foreach (string row in model.IMPORT_BLOCK.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                //split row's columns into string array
                                string[] cols = row.Split(new char[] { ',' }, StringSplitOptions.None);
                                if (cols.Length > 20 && cols[0] != "Date") //skip blank rows
                                {
                                    impAny = true;
                                    try
                                    {
                                        DateTime dt = DateTime.ParseExact(cols[0], "MM/dd/yyyy", CultureInfo.InvariantCulture);

                                        for (int i = 0; i <= 23; i++)
                                        {
                                            ImportResponse xxx = db_Air.InsertUpdateT_QREST_DATA_HOURLY(_monitor.MONITOR_IDX, model.selTimeType == "L" ? dt : (DateTime?)null, model.selTimeType == "U" ? dt : (DateTime?)null, tzOffset, double.TryParse(cols[i + 1], out _) ? cols[i + 1] : null, _monitor.COLLECT_UNIT_CODE, true, double.TryParse(cols[i + 1], out _) ? null : cols[i + 1], importID);
                                            if (xxx.SuccInd)
                                                model.ImportSuccCount += 1;
                                            else
                                                model.error_data.Add(xxx);

                                            dt = dt.AddHours(1);
                                        }
                                    }
                                    catch
                                    {
                                        ModelState.AddModelError("IMPORT_BLOCK", "Date is not in the correct format (MM/DD/YYYY).");
                                    }
                                }
                            }

                            if (!impAny)
                                ModelState.AddModelError("IMPORT_BLOCK", "No data in expected format found. Data needs to be datetime followed by 24 hourly columns, comma separated.");

                        }
                        else
                            ModelState.AddModelError("selMonitor", "No collection unit defined for this monitor. No data imported.");
                    }



           

                }



            
            }

            //reinitialize model
            model.ddl_ImportType = ddlHelpers.get_ddl_import_type();
            model.ddl_Time = ddlHelpers.get_ddl_time_type();
            model.ddl_TimeZone = ddlHelpers.get_ddl_time_zone();
            model.ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true);
            model.ddl_Sites = model.selOrgID == null ? new List<SelectListItem>() : ddlHelpers.get_ddl_my_sites(model.selOrgID, UserIDX);
            model.ddl_Monitors = model.selSite == null ? new List<SelectListItem>() : ddlHelpers.get_monitors_by_site(model.selSite, true, false);
            model.ddl_PollConfig = model.selSite == null ? new List<SelectListItem>() : ddlHelpers.get_ddl_polling_config(model.selSite);
            model.ddl_Calc = ddlHelpers.get_ddl_yes_no();
            model.IMPORT_BLOCK = "";

            return View(model);
        }

        #endregion


        #region QC CHECK

        public ActionResult QCList(Guid? id, string selOrgID, Guid? selSiteID)
        {
            //id is monitorIDX

            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataQCList
            {
                selOrgID = selOrgID,
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true)
            };

            if (model.ddl_Organization.Count() == 1)
                model.selOrgID = model.ddl_Organization.First().Value;

            return View(model);
        }

        [HttpPost]
        public ActionResult QCListData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //data filters
            string selOrgID = Request.Form.GetValues("p_org")?.FirstOrDefault();
            Guid? selSiteID = Request.Form.GetValues("p_site")?.FirstOrDefault().ConvertOrDefault<Guid?>();
            Guid? selMonID = Request.Form.GetValues("p_mon")?.FirstOrDefault().ConvertOrDefault<Guid?>();

            var data = db_Air.GetT_QREST_QC_ASSESSMENT_Search(selOrgID ?? "X", selSiteID, selMonID, pageSize, start, orderCol, orderDir);
            var recordsTotal = 5;// db_Ref.GetT_QREST_SYS_LOG_count(minDate, maxDate);

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        public ActionResult QCEntry(Guid? id)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataQCEntry {
                ddl_Assess_Type = ddlHelpers.get_ddl_ref_assess_type(),
                ddl_Monitor = ddlHelpers.get_ddl_my_monitors(null, UserIDX)
            };

            var x = db_Air.GetT_QREST_QC_ASSESSMENT_ByID(id);
            if (x != null)
            {
                model.QC_ASSESS_IDX = x.QC_ASSESS_IDX;
                model.MONITOR_IDX = x.MONITOR_IDX;
                model.ASSESSMENT_DT = x.ASSESSMENT_DT;
                model.ASSESSMENT_TM = x.ASSESSMENT_DT.ToString("hh:mm tt");
                model.ASSESSMENT_TYPE = x.ASSESSMENT_TYPE;
                model.UNIT_CODE = x.UNIT_CODE;
                model.ASSESSMENT_NUM = x.ASSESSMENT_NUM;
                model.ASSESSED_BY = x.ASSESSED_BY;
            }

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult QCEntry(vmDataQCEntry model) {
            string UserIDX = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                DateTime? combined = model.ASSESSMENT_TM == null ? model.ASSESSMENT_DT : model.ASSESSMENT_DT.GetValueOrDefault().Date.Add(TimeSpan.Parse(model.ASSESSMENT_TM));

                //save generic QC assessment 
                Guid? AssessIDX = db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT(model.QC_ASSESS_IDX, model.MONITOR_IDX, combined, model.ASSESSMENT_TYPE, model.UNIT_CODE,
                    model.ASSESSMENT_NUM, model.ASSESSED_BY, UserIDX);

                //save QC details
                if (AssessIDX != null)
                {

                    if (AssessIDX != null)
                        TempData["Success"] = "Record updated";
                    else
                        TempData["Error"] = "Error updating record.";
                }
            }

            model.ddl_Monitor = ddlHelpers.get_ddl_my_monitors(null, UserIDX);
            model.ddl_Assess_Type = ddlHelpers.get_ddl_ref_assess_type();
            return View(model);

        }


        #endregion



        public ActionResult Raw()
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataRaw
            {
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true),
                ddl_Monitor = new List<SelectListItem>()
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult RawData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //filters
            string selOrg = Request.Form.GetValues("selOrg")?.FirstOrDefault();
            string selType = Request.Form.GetValues("selType")?.FirstOrDefault();
            string selDate = Request.Form.GetValues("selDate")?.FirstOrDefault().Replace(" - ", "z");
            Guid? selMon = Request.Form.GetValues("selMon")?.FirstOrDefault().ConvertOrDefault<Guid?>();

            string[] d = selDate.Split('z');
            DateTime? d1 = d[0].ConvertOrDefault<DateTime?>();
            DateTime? d2 = (d.Length > 1) ? d[1].ConvertOrDefault<DateTime?>() : null;

            if (selType == "F")
            {
                var data = db_Air.GetT_QREST_DATA_FIVE_MIN(selOrg, null, selMon, d1, d2, pageSize, start, orderCol, orderDir);
                var recordsTotal = db_Air.GetT_QREST_DATA_FIVE_MINcount(selOrg, selMon, d1, d2);

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            else
            {
                var data = db_Air.GetT_QREST_DATA_HOURLY(selOrg, selMon, d1, d2, pageSize, start, orderCol, orderDir);
                var recordsTotal = db_Air.GetT_QREST_DATA_HOURLYcount(selOrg, selMon, d1, d2);

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }

        }


        [HttpPost]
        public JsonResult RawDataChart()
        {
            //filters
            string selOrg = Request.Form.GetValues("selOrg")?.FirstOrDefault();
            string selType = Request.Form.GetValues("selType")?.FirstOrDefault();
            string selDate = Request.Form.GetValues("selDate")?.FirstOrDefault().Replace(" - ", "z");
            Guid? selMon = Request.Form.GetValues("selMon")?.FirstOrDefault().ConvertOrDefault<Guid?>();

            if (selDate != null)
            {
                string[] d = selDate.Split('z');
                DateTime? d1 = d[0].ConvertOrDefault<DateTime?>();
                DateTime? d2 = (d.Length > 1) ? d[1].ConvertOrDefault<DateTime?>() : null;

                if (selType == "F")
                {
                    var data = db_Air.GetT_QREST_DATA_FIVE_MIN(selOrg, null, selMon, d1, d2, 25000, 0, 3, "asc");
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = db_Air.GetT_QREST_DATA_HOURLY(selOrg, selMon, d1, d2, 25000, 0, 3, "asc");
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json("Chart data error");

        }


        public JsonResult MonitorSnapshot()
        {
            //filters
            Guid? selMon = Request.Form.GetValues("selMon")?.FirstOrDefault().ConvertOrDefault<Guid?>();

            if (selMon != null)
            {
                var data = db_Air.GetMONITOR_SNAPSHOT_ByMonitor(selMon);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
                return Json("Chart data error");
        }


        #region DATA REVIEW

        public ActionResult DataReviewSummary(Guid? id, int? month, int? year)
        {
            string UserIDX = User.Identity.GetUserId();
            var model = new vmDataReviewSummary {
                ddl_Sites = ddlHelpers.get_ddl_my_sites(null, UserIDX),
                selSite = id,
                selMonth = month ?? System.DateTime.Today.Month,
                selYear = year ?? System.DateTime.Today.Year
            };

            if (id != null && month != null && year != null)
            {
                model.selsDt = new DateTime(model.selYear, model.selMonth, 1);
                model.seleDt = new DateTime(model.selYear, model.selMonth, 1).AddMonths(1).AddHours(-1);
                model.Results = db_Air.SP_AQS_REVIEW_STATUS(id.GetValueOrDefault(), model.selsDt.GetValueOrDefault(), model.seleDt);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult DataReviewSummary(vmHomeReportDaily model)
        {
            return RedirectToAction("DataReviewSummary", new { id = model.selSite, month = model.selMonth, year = model.selYear });
        }


        public ActionResult DataReview(Guid? monid, DateTime? sdt, DateTime? edt)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataReview
            {
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true),
                ddl_Monitor = new List<SelectListItem>(),
                ddl_Duration = ddlHelpers.get_ddl_logger_duration(),
                selMon = monid.ToString(),
                selDtStart = sdt,
                selDtEnd = edt
            };

            //get org and site from supplied mon
            if (string.IsNullOrEmpty(model.selMon) == false)
            {
                SiteMonitorDisplayType _m = db_Air.GetT_QREST_MONITORS_ByID(new Guid(model.selMon));
                if (_m != null)
                    model.selOrgID = _m.ORG_ID;
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult DataReview(vmDataReview model)
        {
            if (model.selMon != null)
            {                
                return RedirectToAction("DataReview2", new { monid = model.selMon, sdt = model.selDtStart, edt = model.selDtEnd, dur = model.selDuration, supp1 = model.selMonSupp?.ElementAtOrDefault(0), supp2 = model.selMonSupp?.ElementAtOrDefault(1), supp3 = model.selMonSupp?.ElementAtOrDefault(2) });
            }
            else
            {
                TempData["Error"] = "Please select a monitor to validate";
                return RedirectToAction("DataReview");
            }
        }

        
        [HttpGet]
        public ActionResult DataReview2(Guid? monid, DateTime? sdt, DateTime? edt, string dur, Guid? supp1, Guid? supp2, Guid? supp3, DateTime? subsdt, DateTime? subedt)
        {
            if (monid == null || sdt == null || edt == null || dur == null)
                return RedirectToAction("DataReview");

            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataReview2
            {
                selDtStart = sdt.GetValueOrDefault(),
                selDtEnd = edt.GetValueOrDefault(),
                selDuration = dur,
                selMon = db_Air.GetT_QREST_MONITORS_ByID(monid.GetValueOrDefault())
            };

            string orgid = model.selMon.ORG_ID;

            //security check
            if (db_Account.CanAccessThisOrg(UserIDX, orgid, true) == false)
            {
                TempData["Error"] = "Access Denied.";
                return RedirectToAction("SiteList", "Site");
            }

            //unit dropdown
            model.ddl_ParUnits = ddlHelpers.get_ddl_ref_units(model.selMon.PAR_CODE);

            //get security access rights
            model.secLvl1Ind = db_Account.IsOrgLvl1(UserIDX, orgid);
            model.secLvl2Ind = db_Account.IsOrgLvl2(UserIDX, orgid);

            //get raw data
            if (dur == "H")
                model.RawData = db_Air.GetT_QREST_DATA_FIVE_MIN(null, null, monid, sdt, edt, 25000, 0, 3, "asc");
            else if (dur == "1")
                model.RawData = db_Air.GetT_QREST_DATA_HOURLY_ManVal(monid.GetValueOrDefault(), model.selDtStart, model.selDtEnd);

            //supp parameters
            if (supp1 != null)
            {
                if (dur == "H")
                    model.SuppData1 = db_Air.GetT_QREST_DATA_FIVE_MIN(null, null, supp1, sdt, edt, 25000, 0, 3, "asc");
                else if (dur == "1")
                    model.SuppData1 = db_Air.GetT_QREST_DATA_HOURLY_ManVal(supp1.GetValueOrDefault(), model.selDtStart, model.selDtEnd);

            }
            return View(model);
        }


        [HttpPost]
        public ActionResult DataReview2(vmDataReview2 model)
        {
            if (ModelState.IsValid)
            {
                int editCount = 0;

                //lvl1
                bool? lvl1ind = null;
                if (model.editLvl1 == "Y") lvl1ind = true;
                else if (model.editLvl1 == "N") lvl1ind = false;

                //lvl2
                bool? lvl2ind = null;
                if (model.editLvl2 == "Y") lvl2ind = true;
                else if (model.editLvl2 == "N") lvl2ind = false;

                string UserIDX = User.Identity.GetUserId();


                foreach (var item in model.RawData)
                {
                    if (item.EditInd == true)
                    {
                        editCount++;

                        Guid? SuccID = db_Air.UpdateT_QREST_DATA_HOURLY(item.DATA_RAW_IDX, model.editNullQual, lvl1ind, lvl2ind, UserIDX, model.editUnitCode, model.editNotes, (model.editValueBlank==true ? "-999" : model.editValue), (model.editFlagBlank == true ? "-999" : model.editFlag));
                        if (SuccID == null)
                            TempData["Error"] = "Error updating record";
                        else
                            db_Air.InsertUpdatetT_QREST_DATA_HOURLY_LOG(null, SuccID, model.editNotes, UserIDX);
                    }
                }
                return RedirectToAction("DataReview2", new { monid = model.selMon.T_QREST_MONITORS.MONITOR_IDX, sdt = model.selDtStart, edt = model.selDtEnd, dur = model.selDuration });

            }
            else
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                TempData["Error"] = "Model Error";
                return RedirectToAction("DataReview2", new { monid = model.selMon.T_QREST_MONITORS.MONITOR_IDX, sdt = model.selDtStart, edt = model.selDtEnd, dur = model.selDuration });
            }
        }


        [HttpPost]
        public JsonResult DataReviewData()
        {
            //filters
            string selType = Request.Form.GetValues("selType")?.FirstOrDefault();
            DateTime? selDateStart = Request.Form.GetValues("selDateStart")?.FirstOrDefault().ConvertOrDefault<DateTime?>();
            DateTime? selDateEnd = Request.Form.GetValues("selDateEnd")?.FirstOrDefault().ConvertOrDefault<DateTime?>();
            Guid? selMon = Request.Form.GetValues("selMon")?.FirstOrDefault().ConvertOrDefault<Guid?>();

            if (selDateStart != null && selDateEnd != null && selMon != null)
            {
                if (selType == "F")
                {
                    var data = db_Air.GetT_QREST_DATA_FIVE_MIN(null, null, selMon, selDateStart, selDateEnd, 25000, 0, 3, "asc");
                    var data1 = JsonConvert.SerializeObject(data);
                    return Json(new { data = data });
                }
                else
                {
                    var data = db_Air.GetT_QREST_DATA_HOURLY(null, selMon, selDateStart, selDateEnd, 25000, 0, 3, "asc");
                    var data1 = JsonConvert.SerializeObject(data);
                    return Json(new { data = data });
                }
            }
            else
                return Json("Chart data error");

        }


        [HttpPost]
        public ActionResult HourlyLogData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            //int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            //int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            //int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            //string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            //string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //date filters
            Guid? id = Request.Form.GetValues("id")?.FirstOrDefault().ConvertOrDefault<Guid?>();

            var data = db_Air.GetT_QREST_DATA_HOURLY_LOG_ByHour(id.GetValueOrDefault());
            var recordsTotal = data.Count();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        public ActionResult DataDocs(Guid? id, Guid? monid, DateTime? sDt, DateTime? eDt)
        {
            if (id == null || sDt == null)
                return RedirectToAction("DataReviewSummary");

            var model = new vmDataDocs {
                selSite = id,
                selMon = monid,
                startDate = sDt.GetValueOrDefault(),
                endDate = eDt.GetValueOrDefault(),
                addStartDt = sDt.GetValueOrDefault(),
                addEndDt = eDt.GetValueOrDefault(),
                SiteDocs = db_Air.GetT_QREST_ASSESS_DOCS_BySite(id.GetValueOrDefault(), sDt, eDt),
                MonDocs = db_Air.GetT_QREST_ASSESS_DOCS_ByMonitor(monid.GetValueOrDefault(), sDt, eDt)
            };

            SiteMonitorDisplayType _mon = db_Air.GetT_QREST_MONITORS_ByID(monid.GetValueOrDefault());
            if (_mon != null)
            {
                model.PAR_CODE = _mon.PAR_CODE;
                model.PAR_NAME = _mon.PAR_NAME;
                model.SITE_ID = _mon.SITE_ID;
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult DataDocsAdd(vmDataDocs model)
        {
            string UserIDX = User.Identity.GetUserId();

            if (model.SiteMonInd == "E")
            {
                Guid? SuccID = db_Air.InsertUpdatetT_QREST_ASSESS_DOCS(model.editASSESS_DOC_IDX, null, null, model.addStartDt, model.addEndDt, null, null, null, null, null, model.fileDescription, null, UserIDX);

                if (SuccID != null)
                    TempData["Success"] = "Update successful";
                else
                    TempData["Error"] = "Update failed";
            }
            else
            {
                if (model.fileUpload?.ContentLength > 0)
                {
                    Stream stream = model.fileUpload.InputStream;
                    byte[] _file = UtilsText.ConvertStreamToByteArray(stream);

                    Guid? SuccID = db_Air.InsertUpdatetT_QREST_ASSESS_DOCS(null, model.selSite, (model.SiteMonInd == "S" ? null : model.selMon), model.addStartDt, model.addEndDt, _file, model.fileUpload.FileName, "",
                        model.fileUpload.ContentType, model.fileUpload.ContentLength, model.fileDescription, "", UserIDX);

                    if (SuccID != null)
                        TempData["Success"] = "File upload successful";
                    else
                        TempData["Error"] = "File upload failed";
                }
                else
                    TempData["Error"] = "You must select a file to upload";

            }

            return RedirectToAction("DataDocs", new { id= model.selSite, monid = model.selMon, sDt = model.startDate.ToString("MM/dd/yyyy"), eDt = model.endDate.ToString("MM/dd/yyyy") });    
        }


        [HttpPost]
        public JsonResult DataDocsDelete(Guid? id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int SuccID = db_Air.DeleteT_QREST_ASSESS_DOCS(id.GetValueOrDefault());
                if (SuccID == 1)
                    return Json("Success");
                else if (SuccID == -1)
                    return Json("Cannot delete Organization that still has site records. Delete sites first.");
                else
                    return Json("Unable to find organization to delete.");
            }
        }


        public FileResult DataDocsDownload(Guid? id)
        {
            try
            {
                string UserIDX = User.Identity.GetUserId();

                T_QREST_ASSESS_DOCS _doc = db_Air.GetT_QREST_ASSESS_DOCS_ByID(id.GetValueOrDefault());
                if (_doc != null)
                {
                    //reject if user doesn't have access to site
                    RedirectToRouteResult r = CanAccessThisSite(UserIDX, _doc.SITE_IDX, true);
                    if (r != null) return null;


                    return File(_doc.DOC_CONTENT, System.Net.Mime.MediaTypeNames.Application.Octet, _doc.DOC_NAME);
                }
            }
            catch
            { }

            return null;
        }


        #endregion



        #region AQS


        public ActionResult AQSList(string selOrgID)
        {
            string UserIDX = User.Identity.GetUserId();
            var model = new vmDataAQSList {
                ddl_Organization = ddlHelpers.get_ddl_my_organizations_aqs_submission(UserIDX),
            };

            //autopopulate if only rights to 1 org that has made aqs submission
            if (model.ddl_Organization != null && model.ddl_Organization.ToList().Count == 1)
                model.selOrgID = model.ddl_Organization.First().Value;

            if (selOrgID != null)
                model.selOrgID = selOrgID;

            model.T_QREST_AQS = db_Air.GetT_QREST_AQS_byORG_ID(model.selOrgID);

            return View(model);
        }

        [HttpPost]
        public ActionResult AQSListEdit(vmDataAQSList model)
        {
            Guid? succID = db_Air.InsertUpdateT_QREST_AQS(model.editID, null, null, model.editSUBMISSION_NAME, null, null, null, null, model.editCOMMENT, null, null, null, null, null);
            if (succID != null)
            {
                TempData["Success"] = "Update successful.";
                return RedirectToAction("AQSList");
            }
            else {
                TempData["Error"] = "Error updating record.";
                return RedirectToAction("AQSList");
            }
        }

        public FileResult AQSFileDownload(Guid? id)
        {
            try
            {
                T_QREST_AQS _doc = db_Air.GetT_QREST_AQS_by_ID(id.GetValueOrDefault());
                if (_doc != null)
                {
                    byte[] _file = Encoding.UTF8.GetBytes(_doc.AQS_CONTENT_XML);
                    return File(_file, System.Net.Mime.MediaTypeNames.Application.Octet, _doc.AQS_SUBMISSION_NAME);
                }
                else
                    TempData["Error"] = "No file found";
            }
            catch
            { }

            return null;
        }

        public FileResult AQSFileDownloadHeader(Guid? id)
        {
            try
            {
                T_QREST_AQS _doc = db_Air.GetT_QREST_AQS_by_ID(id.GetValueOrDefault());
                if (_doc != null)
                    return File(_doc.AQS_CONTENT, System.Net.Mime.MediaTypeNames.Application.Octet, "HeaderFile.zip");
                else
                    TempData["Error"] = "No file found";
            }
            catch
            { }

            return null;
        }

        public FileResult AQSResponseDownload(Guid? id)
        {
            try
            {
                T_QREST_AQS _doc = db_Air.GetT_QREST_AQS_by_ID(id.GetValueOrDefault());
                if (_doc != null)
                    return File(_doc.DOWNLOAD_FILE, System.Net.Mime.MediaTypeNames.Application.Octet, "Response.zip");
                else
                    TempData["Error"] = "No file found";
            }
            catch
            { }

            return null;
        }


        public ActionResult AQSGen()
        {
            string UserIDX = User.Identity.GetUserId();
            var model = new vmDataAQSGen {
                ddl_Sites = ddlHelpers.get_ddl_my_sites_sampled(null, UserIDX),
                selDtStart = new DateTime(System.DateTime.Today.Year, System.DateTime.Today.Month, 1).AddMonths(-1),
                selDtEnd = new DateTime(System.DateTime.Today.Year, System.DateTime.Today.Month, 1).AddHours(-1),
                passValidation = false
            };
            return View(model);
        }


        [HttpPost]
        public ActionResult AQSGen(vmDataAQSGen model)
        {
            model.Results = db_Air.SP_AQS_REVIEW_STATUS(model.selSite ?? Guid.Empty, model.selDtStart.GetValueOrDefault(), model.selDtEnd);
            model.Results = model.Results.Where(o => model.selMons.Contains(o.MONITOR_IDX)).ToList();

            model.passValidation = true;
            bool send = false;
            foreach (SP_AQS_REVIEW_STATUS_Result _result in model.Results)
            {
                if (_result.hrs != _result.lvl2_val_ind)
                    model.passValidation = false;
            }

            //repopulate model before returning
            string UserIDX = User.Identity.GetUserId();
            model.ddl_Sites = ddlHelpers.get_ddl_my_sites_sampled(null, UserIDX);

            return View(model);
        }

        public ActionResult AQSAcct(string id)
        {
            var model = new vmDataAQSAcct
            {
                selOrgID = id,
                CDXUsername = db_Ref.GetT_QREST_APP_SETTING("CDX_GLOBAL_USER"),
                AQSUser = "",
                AQSScreeningGroup = ""
            };

            var _org = db_Ref.GetT_QREST_ORGANIZATION_ByID(id);
            if (_org != null)
            {
                model.AQSUser = _org.AQS_AQS_UID;
                model.AQSScreeningGroup = _org.AQS_AQS_SCREENING_GRP;
                if (!string.IsNullOrEmpty(_org.AQS_NAAS_UID))
                    model.CDXUsername = _org.AQS_NAAS_UID;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult AQSAcct(vmDataAQSAcct model)
        {
            string UserIDX = User.Identity.GetUserId();

            int SuccID = db_Ref.InsertUpdatetT_QREST_ORGANIZATION(model.selOrgID, null, null, null, model.CDXUsername, model.CDXPwd, null, null, true, UserIDX, model.AQSUser, model.AQSScreeningGroup);

            if (SuccID == 0)
                TempData["Error"] = "Unable to update record";
            else
                TempData["Success"] = "Record updated";

            return RedirectToAction("AQSAcct", new { model.selOrgID });
        }


        [HttpPost]
        public ActionResult AQSSubmit(vmDataAQSGen model)
        {
            string UserIDX = User.Identity.GetUserId();

            T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(model.selSite.GetValueOrDefault());
            if (_site != null)
            {
                Guid? SuccID = AQSHelper.AQSGeneration_Orchestrator(_site.ORG_ID, _site.SITE_IDX, model.selMons, model.selDtStart.GetValueOrDefault(), model.selDtEnd.GetValueOrDefault(), UserIDX, model.selActionCode, model.selAQSFormat);
                if (SuccID != null)
                {
                    TempData["Success"] = "File generated and submission initiated.";
                    return RedirectToAction("AQSList", new { selOrgID=_site.ORG_ID });
                }
            }

            TempData["Error"] = "Unable to make submission";
            return RedirectToAction("AQSGen");
        }


        public ActionResult AQSSubmitToEPA(Guid id)
        {
            string UserIDX = User.Identity.GetUserId();


            T_QREST_AQS _aqs = db_Air.GetT_QREST_AQS_by_ID(id);
            if (_aqs != null)
            {
                //reject if user doesn't have access to org
                RedirectToRouteResult r = CanAccessThisOrg(UserIDX, _aqs.ORG_ID, true);
                if (r != null) return r;

                bool SuccID = AQSHelper.AQSSubmission_Orchestrator(_aqs.ORG_ID, _aqs.AQS_CONTENT, _aqs.AQS_IDX);
                if (SuccID)
                {
                    TempData["Success"] = "File submitted.";
                    return RedirectToAction("AQSList", new { selOrgID = _aqs.ORG_ID });
                }

                TempData["Error"] = "Unable to make submission";
                return RedirectToAction("AQSList", new { selOrgID = _aqs.ORG_ID });
            }

            TempData["Error"] = "Unable to make submission";
            return RedirectToAction("AQSList");
        }


        [HttpPost]
        public JsonResult AQSDelete(Guid? id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int SuccID = db_Air.DeleteT_QREST_AQS(id.GetValueOrDefault());
                if (SuccID == 1)
                    return Json("Success");
                else
                    return Json("Unable to delete AQS record.");
            }
        }


        public ActionResult CDXTest(string orgid)
        {
            string UserIDX = User.Identity.GetUserId();

            //reject if user doesn't have access to org
            RedirectToRouteResult r = CanAccessThisOrg(UserIDX, orgid, true);
            if (r != null) return r;

            CDXCredentials _cred = AQSHelper.GetCDXSubmitCredentials2(orgid);
            if (_cred.NodeURL != null && _cred.credential != null)
            {
                string _token = AQSHelper.AuthHelper(_cred.userID, _cred.credential, _cred.NodeURL);
                if (_token!= null && _token.Substring(0,3)=="csm" &&  _token.Length>50)
                    TempData["Success"] = "CDX Username / password combination are correct. Authentication to CDX successful.";
                else
                    TempData["Error"] = "Username/password authentication to CDX failed.";
            }
            else
                TempData["Error"] = "Required fields not supplied.";

            return RedirectToAction("AQSAcct", new { id = orgid });
        }


        public JsonResult AQSGetStatus(Guid id)
        {
            string UserIDX = User.Identity.GetUserId();

            T_QREST_AQS _aqs = db_Air.GetT_QREST_AQS_by_ID(id);
            if (_aqs != null && _aqs.CDX_TOKEN != null) {

                //reject if user doesn't have access to org
                RedirectToRouteResult r = CanAccessThisOrg(UserIDX, _aqs.ORG_ID, true);
                if (r != null) return Json("Access Denied.");

                string strStatus = AQSHelper.AQSGetStatus_Orchestrator(_aqs.ORG_ID, _aqs.AQS_IDX, _aqs.CDX_TOKEN);
                if (strStatus != null)
                    return Json(strStatus);
                else
                    return Json("ERROR");
            }
            
            return Json("Failed");
        }


        public JsonResult AQSDownload(Guid id)
        {
            string UserIDX = User.Identity.GetUserId();

            T_QREST_AQS _aqs = db_Air.GetT_QREST_AQS_by_ID(id);
            if (_aqs != null && _aqs.CDX_TOKEN != null)
            {
                //reject if user doesn't have access to org
                RedirectToRouteResult r = CanAccessThisOrg(UserIDX, _aqs.ORG_ID, true);
                if (r != null) return Json("Access Denied.");

                bool SuccID = AQSHelper.AQSDownload_Orchestrator(_aqs.ORG_ID, _aqs.AQS_IDX, _aqs.CDX_TOKEN);
                if (SuccID)
                    return Json("Success");
            }

            return Json("Failed");
        }

        #endregion


        #region SHARED



        [HttpGet]
        public JsonResult FetchSites(string ID)
        {
            string UserIDX = User.Identity.GetUserId();
            var data = ddlHelpers.get_ddl_my_sites(ID, UserIDX);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID">ORG ID</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult FetchMonitors(string ID)
        {
            string UserIDX = User.Identity.GetUserId();
            var data = ddlHelpers.get_ddl_my_monitors(ID, UserIDX);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID">SITE IDX</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult FetchMonitorsBySite(Guid? ID)
        {
            var data = ddlHelpers.get_monitors_by_site(ID ?? Guid.Empty, true, false);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FetchMonitorsSampledBySite(Guid? ID)
        {
            var data = ddlHelpers.get_monitors_sampled_by_site(ID ?? Guid.Empty);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FetchMonitorsWithData(string ID)
        {
            var data = ddlHelpers.get_monitors_sampled_by_org(ID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult FetchImportTemplates(Guid? ID)
        {
            var data = ddlHelpers.get_ddl_polling_config(ID ?? Guid.Empty);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Retrieves the Hourly Import Template Configuration for the Site
        /// </summary>
        /// <param name="ID">Site ID</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult FetchImportTemplateH1(Guid? ID)
        {
            var data = db_Air.GetT_QREST_SITE_POLL_CONFIG_H1_BySite(ID ?? Guid.Empty);

            if (data != null)
                return Json(data, JsonRequestBehavior.AllowGet);
            else
                return Json("null", JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public JsonResult FetchAQSAccounts(Guid? ID)
        {
            string cdx_user = db_Ref.GetT_QREST_APP_SETTING("CDX_GLOBAL_USER");
            string aqs_user = "";
            string aqs_screening_grp = "";
            string org_id = "";

            var _site = db_Air.GetT_QREST_SITES_ByID(ID ?? Guid.Empty);
            if (_site != null)
            {
                var _org = db_Ref.GetT_QREST_ORGANIZATION_ByID(_site.ORG_ID);
                if (_org != null)
                {
                    org_id = _org.ORG_ID;
                    aqs_user = _org.AQS_AQS_UID;
                    aqs_screening_grp = _org.AQS_AQS_SCREENING_GRP;
                    if (!string.IsNullOrEmpty(_org.AQS_NAAS_UID))
                        cdx_user = _org.AQS_NAAS_UID;
                }              
            }

            var data = Tuple.Create(cdx_user, aqs_user, aqs_screening_grp, org_id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        #endregion



        //**********************SHARED **************************************
        //**********************SHARED **************************************
        //**********************SHARED **************************************
        public RedirectToRouteResult CanAccessThisOrg(string UserIDX, string OrgID, bool CanEditToo)
        {
            if (db_Account.CanAccessThisOrg(UserIDX, OrgID, CanEditToo) == false)
            {
                TempData["Error"] = "Access Denied.";
                return RedirectToAction("SiteList", "Site");
            }
            else
                return null;
        }

        public RedirectToRouteResult CanAccessThisSite(string UserIDX, Guid SiteIDX, bool CanEditToo)
        {

            if (db_Account.CanAccessThisSite(UserIDX, SiteIDX, CanEditToo) == false)
            {
                TempData["Error"] = "Access Denied.";
                return RedirectToAction("SiteList", "Site");
            }
            else
                return null;
        }

    }
}