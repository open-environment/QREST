using System;
using System.Collections.Generic;
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
    public class DataController : Controller
    {
        // GET: Data
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManualImport()
        {
            return View();
        }


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


        public ActionResult Raw()
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataRaw
            {
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true)
            };

            string xxx = model.ddl_Organization.First().Value;
            model.ddl_Monitor = ddlHelpers.get_ddl_my_monitors(xxx, UserIDX);

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
        public ActionResult RawDataChart()
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
                    var data1 = JsonConvert.SerializeObject(data);
                    return Json(new { data = data1 });
                }
                else
                {
                    var data = db_Air.GetT_QREST_DATA_HOURLY(selOrg, selMon, d1, d2, 25000, 0, 3, "asc");
                    var data1 = JsonConvert.SerializeObject(data);
                    return Json(new { data = data1 });
                }
            }
            else
                return Json("Chart data error");

        }


        public ActionResult DataReview()
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataRaw
            {
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true)
            };

            string xxx = model.ddl_Organization.First().Value;
            model.ddl_Monitor = ddlHelpers.get_ddl_my_monitors(xxx, UserIDX);

            return View(model);
        }

        [HttpGet]
        public JsonResult FetchMonitors(string ID)
        {
            string UserIDX = User.Identity.GetUserId();
            var data = ddlHelpers.get_ddl_my_monitors(ID, UserIDX);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}