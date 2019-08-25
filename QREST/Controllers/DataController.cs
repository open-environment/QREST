using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
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

        public ActionResult QCList(Guid? id, string selOrgID, Guid? selSiteID)
        {
            //id is monitorIDX

            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataQCList
            {
                selOrgID = selOrgID,
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX)
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

            //date filters
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
                ddl_Monitor = ddlHelpers.get_ddl_my_monitors(UserIDX)                
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
                DateTime? combined = model.ASSESSMENT_TM==null ? model.ASSESSMENT_DT : model.ASSESSMENT_DT.GetValueOrDefault().Date.Add(TimeSpan.Parse(model.ASSESSMENT_TM));

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

            model.ddl_Monitor = ddlHelpers.get_ddl_my_monitors(UserIDX);
            model.ddl_Assess_Type = ddlHelpers.get_ddl_ref_assess_type();
            return View(model);

        }
    }
}