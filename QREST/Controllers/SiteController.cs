using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QREST.Models;
using Microsoft.AspNet.Identity;
using QREST.App_Logic.DataAccessLayer;
using QREST.App_Logic.BusinessLogicLayer;

namespace QREST.Controllers
{
    public class SiteController : Controller
    {
       
        // GET: Site
        public ActionResult Index()
        {
            return View();
        }


        //**********************AGENCY **************************************
        //**********************AGENCY **************************************
        //**********************AGENCY **************************************
        public ActionResult OrgList()
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmSiteOrgList();

            //List<T_QREST_ORGANIZATIONS> _orgs = db_Ref.GetT_QREST_ORGANIZATIONS_ByUser(UserIDX, false);
            //if (_orgs != null)
            //{
            //    //if only 1 organization, redirect to edit page
            //    if (_orgs.Count == 1)
            //        return RedirectToAction("OrgEdit");

            //    model.orgs = _orgs;                
            //}

            return View(model);
        }

        [HttpPost]
        public ActionResult OrgListData()
        {
            string UserIDX = User.Identity.GetUserId();
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
            var data = db_Account.GetT_QREST_ORG_USERS_byUSER_IDX(UserIDX, null);
            var recordsTotal = data.Count();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }





        //**********************SITES **************************************
        //**********************SITES **************************************
        //**********************SITES **************************************
        public ActionResult SiteList(string selOrgID)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmSiteSiteList
            {
                selOrgID = selOrgID,
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX),
                T_QREST_SITES = db_Air.GetT_QREST_SITES_ByUser_OrgID(selOrgID, UserIDX)
            };

            return View(model);
        }


        public ActionResult SiteEdit(Guid? id)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmSiteSiteEdit {
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX)
            };

            T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(id ?? Guid.Empty);
            if (_site != null)
            {
                //reject if user doesn't have access to org
                if (db_Account.CanAccessThisOrg(UserIDX, _site.ORG_ID, false) == false)
                {
                    TempData["Error"] = "Access Denied.";
                    return RedirectToAction("SiteList", "Site");
                }

                model.SITE_IDX = _site.SITE_IDX;
                model.ORG_ID = _site.ORG_ID;
                model.SITE_ID = _site.SITE_ID;
                model.SITE_NAME = _site.SITE_NAME;
                model.AQS_SITE_ID = _site.AQS_SITE_ID;
                model.LATITUDE = _site.LATITUDE;
                model.LONGITUDE = _site.LONGITUDE;
                model.ADDRESS = _site.ADDRESS;
                model.CITY = _site.CITY;
                model.ZIP_CODE = _site.ZIP_CODE;
                model.START_DT = _site.START_DT;
                model.END_DT = _site.END_DT;
                model.TELEMETRY_ONLINE_IND = _site.TELEMETRY_ONLINE_IND;
                model.SITE_COMMENTS = _site.SITE_COMMENTS;

                //monitor
                model.monitors = db_Air.GetT_QREST_MONITORS_Display_bySiteIDX(model.SITE_IDX);
            }
            else if (id != null)
            {
                //fail if user supplied SiteIDX but doesn't exist
                TempData["Error"] = "Site not found.";
                return RedirectToAction("SiteList", "Site");
            }
            

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SiteEdit(vmSiteSiteEdit model)
        {
            string UserIDX = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                // check security (whether can update)
                if (db_Account.CanAccessThisOrg(UserIDX, model.ORG_ID, true) == false)
                {
                    TempData["Error"] = "Access Denied.";
                    return RedirectToAction("SiteList", "Site");
                }


                int SuccInd = db_Air.InsertUpdatetT_QREST_SITES(model.SITE_IDX, model.ORG_ID, model.SITE_ID, model.SITE_NAME, model.AQS_SITE_ID,
                    model.LATITUDE, model.LONGITUDE, model.ADDRESS, model.CITY, model.STATE, model.ZIP_CODE,
                    model.START_DT, model.END_DT, model.TELEMETRY_ONLINE_IND, model.SITE_COMMENTS, UserIDX);

                if (SuccInd>0)
                    TempData["Success"] = "Record updated";
                else
                    TempData["Error"] = "Error updating record.";

            }

            model.ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX);
            return View(model);

        }


        [HttpPost]
        public JsonResult SiteDelete(string id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                Guid idg = new Guid(id);
                int SuccID = db_Air.DeleteT_QREST_SITES(idg);
                if (SuccID == 1)
                    return Json("Success");
                else if (SuccID == -1)
                    return Json("Cannot delete Site that still has monitor records. Delete monitors first.");
                else
                    return Json("Unable to find site to delete.");
            }
        }


        //**********************MONITORS **************************************
        //**********************MONITORS **************************************
        //**********************MONITORS **************************************
        public ActionResult MonitorList(string selOrgID)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmSiteMonitorList
            {
                selOrgID = selOrgID,
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX),
                monitors = db_Air.GetT_QREST_MONITORS_ByUser_OrgID(selOrgID, UserIDX)
            };

            return View(model);
        }


        public ActionResult MonitorEdit(Guid? id, Guid? siteIDX)
        {
            string UserIDX = User.Identity.GetUserId();
            var model = new vmSiteMonitorEdit();

            //insert case
            if (id == null && siteIDX != null)
            {
                //*************** VALIDATION BEGIN *********************************
                //reject if user doesn't have access to site
                if (db_Account.CanAccessThisSite(UserIDX, (Guid)siteIDX, false) == false)
                {
                    TempData["Error"] = "Access Denied.";
                    return RedirectToAction("SiteList", "Site");
                }
                //*************** VALIDATION END *********************************

                model.SITE_IDX = siteIDX;
            }

            //update case
            else if (id != null && siteIDX == null)
            {
                SiteMonitorDisplayType _monitor = db_Air.GetT_QREST_MONITORS_ByID(id ?? Guid.Empty);
                if (_monitor != null)
                {
                    //*************** VALIDATION BEGIN *********************************
                    //reject if user doesn't have access to site
                    if (db_Account.CanAccessThisSite(UserIDX, _monitor.T_QREST_MONITORS.SITE_IDX, false) == false)
                    {
                        TempData["Error"] = "Access Denied.";
                        return RedirectToAction("SiteList", "Site");
                    }
                    //*************** VALIDATION END *********************************

                    model.MONITOR_IDX = _monitor.T_QREST_MONITORS.MONITOR_IDX;
                    model.SITE_IDX = _monitor.T_QREST_MONITORS.SITE_IDX;
                    model.PAR_METHOD_IDX = _monitor.T_QREST_MONITORS.PAR_METHOD_IDX;
                    model.POC = _monitor.T_QREST_MONITORS.POC;
                    model.DURATION_CODE = _monitor.T_QREST_MONITORS.DURATION_CODE;
                    model.COLLECT_FREQ_CODE = _monitor.T_QREST_MONITORS.COLLECT_FREQ_CODE;
                    model.CREATE_DT = _monitor.T_QREST_MONITORS.CREATE_DT;
                    model.PAR_NAME = _monitor.PAR_NAME;
                    model.METHOD_CODE = _monitor.METHOD_CODE;
                    model.COLLECT_UNIT_CODE = _monitor.T_QREST_MONITORS.COLLECT_UNIT_CODE;
                    model.ALERT_MIN_VALUE = _monitor.T_QREST_MONITORS.ALERT_MIN_VALUE;
                    model.ALERT_MAX_VALUE = _monitor.T_QREST_MONITORS.ALERT_MAX_VALUE;
                    model.ALERT_PCT_CHANGE = _monitor.T_QREST_MONITORS.ALERT_PCT_CHANGE;
                    model.ALERT_STUCK_REC_COUNT = _monitor.T_QREST_MONITORS.ALERT_STUCK_REC_COUNT;

                }
                else
                {
                    //fail if user supplied SiteIDX but doesn't exist
                    TempData["Error"] = "Monitor not found.";
                    return RedirectToAction("SiteList", "Site");
                }
            }
            else
            {
                //fail if user supplied SiteIDX but doesn't exist
                TempData["Error"] = "Monitor or site not found.";
                return RedirectToAction("SiteList", "Site");
            }

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MonitorEdit(vmSiteMonitorEdit model)
        {
            string UserIDX = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                //*************** VALIDATION BEGIN *********************************
                //reject if user doesn't have access to site
                if (db_Account.CanAccessThisSite(UserIDX, (Guid)model.SITE_IDX, true) == false)
                {
                    TempData["Error"] = "You don't have rights to edit this monitor.";
                    return RedirectToAction("SiteList", "Site");
                }
                //*************** VALIDATION END *********************************


                int SuccInd = db_Air.InsertUpdatetT_QREST_MONITORS(model.MONITOR_IDX, model.SITE_IDX, model.PAR_METHOD_IDX, model.POC, model.DURATION_CODE, model.COLLECT_FREQ_CODE, 
                    model.COLLECT_UNIT_CODE, model.ALERT_MIN_VALUE, model.ALERT_MAX_VALUE, model.ALERT_PCT_CHANGE, model.ALERT_STUCK_REC_COUNT, UserIDX);

                if (SuccInd > 0)
                    TempData["Success"] = "Record updated";
                else
                    TempData["Error"] = "Error updating record.";

            }

            //repopulate model
            //model.ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX);

            return View(model);
        }

        [HttpPost]
        public JsonResult MonitorDelete(string id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                Guid idg = new Guid(id);
                int SuccID = db_Air.DeleteT_QREST_SITES(idg);
                if (SuccID == 1)
                    return Json("Success");
                else if (SuccID == -1)
                    return Json("Cannot delete monitor that has air quality data collected. Delete results first.");
                else
                    return Json("Unable to find monitor to delete.");
            }
        }


        [HttpPost]
        public ActionResult RefParMethodData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            string search = Request.Form.GetValues("search[value]")?.FirstOrDefault(); //search value
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            List<RefParMethodDisplay> data = db_Ref.GetT_QREST_REF_PAR_METHODS_Search(search, null, pageSize, start, orderCol, orderDir);
            var recordsTotal = db_Ref.GetT_QREST_REF_PAR_METHODS_Count(search, null);

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


    }

}