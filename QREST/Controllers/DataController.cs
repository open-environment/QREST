using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using QREST.App_Logic.BusinessLogicLayer;
using QREST.Models;
using QRESTModel.DAL;

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
                ddl_Duration = ddlHelpers.get_ddl_logger_duration()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ManualImport(vmDataImport model) {

            string UserIDX = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                //5 min
                if (model.selDuration == "F")
                {

                }

                //hourly
                else if (model.selDuration == "H")
                {

                }
            }

            //reinitialize model
            model.ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true);
            model.ddl_Sites = new List<SelectListItem>();
            model.ddl_Duration = ddlHelpers.get_ddl_logger_duration();
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

            //string xxx = model.ddl_Organization.First().Value;
            //model.ddl_Monitor = ddlHelpers.get_monitors_sampled_by_org(xxx);

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
                var data = db_Air.GetT_QREST_DATA_FIVE_MIN(selOrg, selMon, d1, d2, pageSize, start, orderCol, orderDir);
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
                    var data = db_Air.GetT_QREST_DATA_FIVE_MIN(selOrg, selMon, d1, d2, 25000, 0, 3, "asc");
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

        #region MANUAL VALIDATION

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
                model.seleDt = new DateTime(model.selYear, model.selMonth, DateTime.DaysInMonth(model.selYear, model.selMonth), 23,0,0);
                model.Results = db_Air.SP_AQS_REVIEW_STATUS(id.GetValueOrDefault(), new DateTime(model.selYear, model.selMonth, 1));
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

            //get org from supplied mon
            if (string.IsNullOrEmpty(model.selMon)==false) {
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
                selMon = db_Air.GetT_QREST_MONITORS_ByID(monid.GetValueOrDefault()),
                //selDtStartSub = subsdt,
                //selDtEndSub = subedt,                
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
                model.RawData = db_Air.GetT_QREST_DATA_FIVE_MIN(null, monid, sdt, edt, 25000, 0, 3, "asc");
            else if (dur == "1")
                model.RawData = db_Air.GetT_QREST_DATA_HOURLY_ManVal(monid.GetValueOrDefault(), model.selDtStart, model.selDtEnd);

            //supp parameters
            if (supp1 != null)
            {
                if (dur == "H")
                    model.SuppData1 = db_Air.GetT_QREST_DATA_FIVE_MIN(null, supp1, sdt, edt, 25000, 0, 3, "asc");
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

                //lvl1
                bool? lvl2ind = null;
                if (model.editLvl2 == "Y") lvl2ind = true;
                else if (model.editLvl2 == "N") lvl2ind = false;

                string UserIDX = User.Identity.GetUserId();


                foreach (var item in model.RawData)
                {
                    if (item.EditInd == true)
                    {
                        editCount++;

                        Guid? SuccID = db_Air.UpdateT_QREST_DATA_HOURLY(item.DATA_RAW_IDX, model.editNullQual, lvl1ind, lvl2ind, UserIDX, model.editUnitCode, model.editNotes);
                        if (SuccID == null)
                            TempData["Error"] = "Error updating record";
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
                    var data = db_Air.GetT_QREST_DATA_FIVE_MIN(null, selMon, selDateStart, selDateEnd, 25000, 0, 3, "asc");
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


        public ActionResult DataDocs(Guid? id, Guid? monid, int? month, int? year)
        {
            var model = new vmDataDocs {
                selSite = id,
                selMon = monid,
                selMonth = month ?? System.DateTime.Today.Month,
                selYear = year ?? System.DateTime.Today.Year,
                SiteDocs = db_Air.GetT_QREST_ASSESS_DOCS_BySite(id.GetValueOrDefault(), year ?? System.DateTime.Today.Year, month ?? System.DateTime.Today.Month),
                MonDocs = db_Air.GetT_QREST_ASSESS_DOCS_ByMonitor(monid.GetValueOrDefault(), year ?? System.DateTime.Today.Year, month ?? System.DateTime.Today.Month)
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
                Guid? SuccID = db_Air.InsertUpdatetT_QREST_ASSESS_DOCS(model.editASSESS_DOC_IDX, null, null, null, null, null, null, null, null, null, model.fileDescription, null, UserIDX);

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

                    Guid? SuccID = db_Air.InsertUpdatetT_QREST_ASSESS_DOCS(null, model.selSite, (model.SiteMonInd == "S" ? null : model.selMon), model.selYear, model.selMonth, _file, model.fileUpload.FileName, "",
                        model.fileUpload.ContentType, model.fileUpload.ContentLength, model.fileDescription, "", UserIDX);

                    if (SuccID != null)
                        TempData["Success"] = "File upload successful";
                    else
                        TempData["Error"] = "File upload failed";
                }
                else
                    TempData["Error"] = "You must select a file to upload";

            }

            return RedirectToAction("DataDocs", new { id= model.selSite, monid = model.selMon, month = model.selMonth, year = model.selYear });    
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
                T_QREST_ASSESS_DOCS _doc = db_Air.GetT_QREST_ASSESS_DOCS_ByID(id.GetValueOrDefault());
                if (_doc != null)
                    return File(_doc.DOC_CONTENT, System.Net.Mime.MediaTypeNames.Application.Octet, _doc.DOC_NAME);
            }
            catch 
            {}

            return null;
        }


        public ActionResult AQS()
        {
            return View();
        }

        #endregion


        [HttpGet]
        public JsonResult FetchSites(string ID)
        {
            string UserIDX = User.Identity.GetUserId();
            var data = ddlHelpers.get_ddl_my_sites(ID, UserIDX);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult FetchMonitors(string ID)
        {
            string UserIDX = User.Identity.GetUserId();
            var data = ddlHelpers.get_ddl_my_monitors(ID, UserIDX);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FetchMonitorsWithData(string ID)
        {
            var data = ddlHelpers.get_monitors_sampled_by_org(ID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}