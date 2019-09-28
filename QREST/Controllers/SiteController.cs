using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using QREST.App_Logic.BusinessLogicLayer;
using QREST.Models;
using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace QREST.Controllers
{
    [Authorize]
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


        public ActionResult OrgEdit(string id)
        {
            string UserIDX = User.Identity.GetUserId();

            if (db_Account.IsAnOrgAdmin(UserIDX, id))
            {
                var model = new vmSiteOrgEdit();
                model.ddl_User = ddlHelpers.get_ddl_users(false);

                var _org = db_Ref.GetT_QREST_ORGANIZATION_ByID(id);
                if (_org != null)
                {
                    model.ORG_ID = _org.ORG_ID;
                    model.ORG_NAME = _org.ORG_NAME;
                    model.STATE_CD = _org.STATE_CD;
                    model.EPA_REGION = _org.EPA_REGION;
                    model.AQS_AGENCY_CODE = _org.AQS_AGENCY_CODE;
                    model.SELF_REG_IND = _org.SELF_REG_IND ?? true;
                    model.edit_org_id = _org.ORG_ID;
                    model.edit_typ = "org";

                    model.org_users = db_Account.GetT_QREST_ORG_USERS_ByOrgID(model.ORG_ID, null, null);
                }

                return View(model);
            }
            else
            {
                TempData["Error"] = "You are not an Admin for this organization.";
                return RedirectToAction("Dashboard", "Index");
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrgEdit(vmSiteOrgEdit model)
        {
            string UserIDX = User.Identity.GetUserId();
            if (db_Account.IsAnOrgAdmin(UserIDX, model.ORG_ID))
            {
                if (ModelState.IsValid)
                {
                    int SuccID = db_Ref.InsertUpdatetT_QREST_ORGANIZATION(model.ORG_ID, model.ORG_NAME, model.STATE_CD, model.EPA_REGION,
                        model.AQS_NAAS_UID, model.AQS_NAAS_PWD, model.AQS_AGENCY_CODE, model.SELF_REG_IND, true, "");

                    if (SuccID == 1)
                        TempData["Success"] = "Record updated";
                    else
                        TempData["Error"] = "Error updating record.";

                }

                //repopulate model
                model.org_users = db_Account.GetT_QREST_ORG_USERS_ByOrgID(model.ORG_ID, null, null);
                model.ddl_User = ddlHelpers.get_ddl_users(false);
                return View(model);
            }
            else
            {
                TempData["Error"] = "You are not an Admin for this organization.";
                return RedirectToAction("Dashboard", "Index");
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrgEditUser(vmAdminOrgEditUser model)
        {
            if (ModelState.IsValid)
            {
                Guid? SuccID = db_Account.InsertUpdateT_QREST_ORG_USERS(model.edit_user_idx, model.edit_org_id, model.edit_org_user_access_level, model.edit_org_user_status, "");

                if (SuccID != null)
                    TempData["Success"] = "Record updated";
                else
                    TempData["Error"] = "Error updating record.";
            }
            else
                TempData["Error"] = "Error updating record.";


            if (model.edit_typ == "org")
                return RedirectToAction("OrgEdit", "Site", new { id = model.edit_org_id });
            else
                return RedirectToAction("UserEdit", "Site", new { id = model.edit_user_idx });
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

            var model = new vmSiteSiteEdit(); 

            T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(id ?? Guid.Empty);
            if (_site != null)
            {
                //reject if user doesn't have access to org
                CanAccessThisOrg(UserIDX, _site.ORG_ID, false);

                //EDIT CASE
                model.SITE_IDX = _site.SITE_IDX;
                model.ORG_ID = _site.ORG_ID;
                model.SITE_ID = _site.SITE_ID;
                model.SITE_NAME = _site.SITE_NAME;
                model.AQS_SITE_ID = _site.AQS_SITE_ID;
                model.STATE_CD = _site.STATE_CD;
                model.COUNTY_CD = _site.COUNTY_CD;
                model.LATITUDE = _site.LATITUDE;
                model.LONGITUDE = _site.LONGITUDE;
                model.ELEVATION = _site.ELEVATION;
                model.ADDRESS = _site.ADDRESS;
                model.CITY = _site.CITY;
                model.ZIP_CODE = _site.ZIP_CODE;
                model.START_DT = _site.START_DT;
                model.END_DT = _site.END_DT;
                model.POLLING_ONLINE_IND = _site.POLLING_ONLINE_IND ?? false;
                model.AIRNOW_IND = _site.AIRNOW_IND ?? false;
                model.AQS_IND = _site.AQS_IND ?? false;
                model.SITE_COMMENTS = _site.SITE_COMMENTS;
            }
            else if (id != null)
            {
                //reject if user supplied SiteIDX but doesn't exist
                TempData["Error"] = "Site not found.";
                return RedirectToAction("SiteList", "Site");
            }

            InitializeSiteEditModel(model, UserIDX);
            return View(model);
        }


        private static void InitializeSiteEditModel(vmSiteSiteEdit model, string UserIDX)
        {
            model.ddl_State = ddlHelpers.get_ddl_state();
            model.ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX);
            model.monitors = db_Air.GetT_QREST_MONITORS_Display_bySiteIDX(model.SITE_IDX);
            model.notifiees = db_Air.GetT_QREST_SITE_NOTIFY_BySiteID(model.SITE_IDX);
            model.ddl_User = ddlHelpers.get_ddl_users(true);

            //county
            if (model.STATE_CD != null)
                model.ddl_County = ddlHelpers.get_ddl_county(model.STATE_CD);
            else
                model.ddl_County = new SelectList(Enumerable.Empty<SelectListItem>());
        }

        [HttpGet]
        public JsonResult FetchCounties(string ID)
        {
            var data = ddlHelpers.get_ddl_county(ID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SiteEdit(vmSiteSiteEdit model)
        {
            string UserIDX = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                //reject if user doesn't have access to org
                CanAccessThisOrg(UserIDX, model.ORG_ID, true);

                Guid? SuccId = db_Air.InsertUpdatetT_QREST_SITES(model.SITE_IDX, model.ORG_ID, model.SITE_ID, model.SITE_NAME, model.AQS_SITE_ID ?? "",
                    model.STATE_CD ?? "", model.COUNTY_CD ?? "", model.LATITUDE, model.LONGITUDE, model.ELEVATION, model.ADDRESS ?? "", model.CITY ?? "", model.ZIP_CODE ?? "",
                    model.START_DT, model.END_DT, model.POLLING_ONLINE_IND, null, null, null, null, model.AIRNOW_IND, model.AQS_IND, model.SITE_COMMENTS ?? "", UserIDX);

                if (SuccId != null)
                {
                    TempData["Success"] = "Record updated";
                    return RedirectToAction("SiteEdit", "Site", new { id = SuccId });
                }
                else
                    TempData["Error"] = "Error updating record.";

            }

            InitializeSiteEditModel(model, UserIDX);
            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SiteEditNotifyUser(vmSiteEditNotifyUser model)
        {
            if (ModelState.IsValid)
            {
                Guid? SuccID = db_Air.InsertUpdatetT_QREST_SITE_NOTIFY(null, model.SITE_IDX, model.edit_notify_user_idx.ToString(), User.Identity.GetUserId());

                if (SuccID != null)
                    TempData["Success"] = "Record updated";
                else
                    TempData["Error"] = "Error updating record.";
            }
            else
                TempData["Error"] = "Error updating record";

            return RedirectToAction("SiteEdit", "Site", new { id = model.SITE_IDX });
        }

        [HttpPost]
        public JsonResult SiteNotifyDelete(string id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int SuccID = db_Air.DeleteT_QREST_SITE_NOTIFY(new Guid(id));
                if (SuccID == 1)
                    return Json("Success");
                else
                    return Json("Unable to find record to delete.");
            }
        }


        public ActionResult SiteImport(string selOrgID)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmSiteSiteImport
            {
                selOrgID = selOrgID,
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX),
                ImportSites = new List<T_QREST_SITES>()
            };

            //if no org specified but user has access to one only, then set it
            if (model.ddl_Organization.Count() == 1 && model.selOrgID == null)
                model.selOrgID = model.ddl_Organization.FirstOrDefault().Value;

            if (model.selOrgID == null)
                //if user hasn't selected an org, return view now
                return View(model);
            else
            {
                //reject if user doesn't have access to org
                CanAccessThisOrg(UserIDX, model.selOrgID, true);

                //lookup to get the AQS Tribal Code for 
                T_QREST_ORGANIZATIONS _org = db_Ref.GetT_QREST_ORGANIZATION_ByID(model.selOrgID);
                if (_org == null)
                {
                    TempData["Error"] = "Organization not found";
                    return View(model);
                }
                else if (_org.AQS_AGENCY_CODE?.Length < 3)
                {
                    TempData["Error"] = "You must specify an AQS Code for the agency before importing sites from AQS.";
                    return View(model);
                }

                //grab remote CSV file from EPA AQS
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://aqs.epa.gov/aqsweb/codes/qa/SitesV4.txt");
                try
                {
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                    using (StreamReader csvreader = new StreamReader(resp.GetResponseStream()))
                    {
                        string currentLine;
                        while ((currentLine = csvreader.ReadLine()) != null)
                        {
                            //split row's columns into string array
                            string[] cols = currentLine.Split('"');
                            if (cols.Length > 0) //skip blank rows
                            {
                                if (cols[9] != "None")
                                {
                                    string tribCode = cols[9];

                                    if (_org.AQS_AGENCY_CODE == tribCode)
                                    {
                                        string siteID = cols[5];
                                        string siteName = cols[7];
                                        string state = cols[1];
                                        string county = cols[3];
                                        T_QREST_SITES s = new T_QREST_SITES
                                        {
                                            SITE_IDX = Guid.NewGuid(),
                                            ORG_ID = model.selOrgID,
                                            SITE_ID = siteID,
                                            SITE_NAME = siteName,
                                            AQS_SITE_ID = siteID,
                                            STATE_CD = state,
                                            COUNTY_CD = county,
                                            CREATE_DT = System.DateTime.Now,
                                            CREATE_USER_IDX = UserIDX
                                        };

                                        //check if QREST already has the site.
                                        T_QREST_SITES _existSite = db_Air.GetT_QREST_SITES_ByOrgandAQSID(model.selOrgID, siteID);
                                        if (_existSite != null)
                                            s.SITE_COMMENTS = "U";
                                        else
                                            s.SITE_COMMENTS = "I";

                                        model.ImportSites.Add(s);
                                    }
                                }
                            }
                        }
                    }
                }
                catch {
                    TempData["Error"] = "Unable to connect to AQS, please try again later.";
                    return View(model);
                }
            }

            return View(model); ;
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SiteImport(vmSiteSiteImport model)
        {
            string UserIDX = User.Identity.GetUserId();

            //if user hasn't selected an org, return view now
            if (model.selOrgID == null)
            {
                TempData["Error"] = "Organization not found";
                return View(model);
            }

            //reject if user doesn't have access to org
            CanAccessThisOrg(UserIDX, model.selOrgID, true);

            //lookup to get the AQS Tribal Code for 
            T_QREST_ORGANIZATIONS _org = db_Ref.GetT_QREST_ORGANIZATION_ByID(model.selOrgID);
            if (_org == null)
            {
                TempData["Error"] = "Organization not found";
                return View(model);
            }

            //grab remote CSV file from EPA AQS
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://aqs.epa.gov/aqsweb/codes/qa/SitesV4.txt");
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            using (StreamReader csvreader = new StreamReader(resp.GetResponseStream()))
            {
                string currentLine;
                while ((currentLine = csvreader.ReadLine()) != null)
                {
                    //split row's columns into string array
                    string[] cols = currentLine.Split('"');
                    if (cols.Length > 0) //skip blank rows
                    {
                        if (cols[9] != "None")
                        {
                            if (_org.AQS_AGENCY_CODE == cols[9])
                            {
                                //check if QREST already has the site.
                                T_QREST_SITES _existSite = db_Air.GetT_QREST_SITES_ByOrgandAQSID(model.selOrgID, cols[5]);
                                if (_existSite == null)
                                    db_Air.InsertUpdatetT_QREST_SITES(null, model.selOrgID, cols[5], cols[7], cols[5], cols[1], cols[3], null, null, null, null, null, 
                                        null, null, null, false, null, null, null, null, false, false, null, UserIDX);
                            }
                        }
                    }

                }
            }

            return RedirectToAction("SiteList", new { selOrgID = model.selOrgID });
        }


        [HttpPost]
        public JsonResult SiteDelete(string id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                Guid idg = new Guid(id);

                //reject if user doesn't have access to org
                T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(idg);
                if (_site != null)
                    CanAccessThisOrg(User.Identity.GetUserId(), _site.ORG_ID, false);

                int SuccID = db_Air.DeleteT_QREST_SITES(idg);
                if (SuccID == 1)
                    return Json("Success");
                else if (SuccID == -1)
                    return Json("Cannot delete Site that still has monitor records. Delete monitors first.");
                else
                    return Json("Unable to find site to delete.");
            }
        }


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

        public ActionResult SitePollConfig(Guid? id, Guid? configid, int? n)
        {
            T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(id ?? Guid.Empty);
            if (_site != null)
            {
                //reject if user doesn't have access to org
                CanAccessThisOrg(User.Identity.GetUserId(), _site.ORG_ID, false);

                //get listing of configs
                var model = new vmSiteSitePollConfig {
                    SITE_IDX = id,
                    ConfigList = db_Air.GetT_QREST_SITE_POLL_CONFIG_BySite(id.ConvertOrDefault<Guid>())
                };

                //get config to display
                if (n == 1) //new case
                {
                    model.CurrentConfig = new T_QREST_SITE_POLL_CONFIG
                    {
                        POLL_CONFIG_IDX = Guid.NewGuid(),
                        ACT_IND = true
                    };
                }
                else
                {
                    //get config by supplied
                    model.CurrentConfig = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(configid.ConvertOrDefault<Guid>());

                    //if none, then display active
                    if (model.CurrentConfig == null)
                        model.CurrentConfig = db_Air.GetT_QREST_SITE_POLL_CONFIG_ActiveByID(_site.SITE_IDX);
                }

                return View(model);
            }
            else
            {
                //reject if user supplied SiteIDX but doesn't exist
                TempData["Error"] = "Site not found.";
                return RedirectToAction("SiteList", "Site");
            }

        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SitePollConfig(vmSiteSitePollConfig model)
        {
            string UserIDX = User.Identity.GetUserId();
            var e = model.CurrentConfig;

            //set all others to inactive if this one is active
            if (e.ACT_IND == true)
            {
                List<T_QREST_SITE_POLL_CONFIG> _ps = db_Air.GetT_QREST_SITE_POLL_CONFIG_BySite(model.SITE_IDX.ConvertOrDefault<Guid>());
                foreach (T_QREST_SITE_POLL_CONFIG _p in _ps)
                    db_Air.InsertUpdatetT_QREST_SITE_POLL_CONFIG(_p.POLL_CONFIG_IDX, null, null, null, null, null, null, null, null, null, null, null, null, false, null);
            }

            Guid? SuccID = db_Air.InsertUpdatetT_QREST_SITE_POLL_CONFIG(e.POLL_CONFIG_IDX, model.SITE_IDX, e.RAW_DURATION_CODE, e.LOGGER_TYPE, e.LOGGER_SOURCE, e.LOGGER_PORT,
                e.LOGGER_USERNAME, e.LOGGER_PASSWORD, e.DELIMITER, e.DATE_COL, e.DATE_FORMAT, e.TIME_COL, e.TIME_FORMAT, e.ACT_IND, UserIDX);

            if (SuccID != null)
            {
                TempData["Success"] = "Record updated";
                return RedirectToAction("SitePollConfig", new { id = model.SITE_IDX, configid = SuccID });
            }
            else
                TempData["Error"] = "Error updating record.";

            return View(model);
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
                    model.ALERT_AMT_CHANGE = _monitor.T_QREST_MONITORS.ALERT_AMT_CHANGE;
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

            //additioal model validations
            if (model.ALERT_MIN_VALUE != null && model.ALERT_MAX_VALUE != null && model.ALERT_MAX_VALUE <= model.ALERT_MIN_VALUE)
                ModelState.AddModelError("ALERT_MIN_VALUE", "Alert minimum must be lower than alert maximum.");

            //insert put in default values
            if (model.MONITOR_IDX == null && model.POC != null && model.PAR_METHOD_IDX != null)
            {
                T_QREST_MONITORS _exist = db_Air.GetT_QREST_MONITORS_bySiteIDX_ParMethod_POC(model.SITE_IDX.ConvertOrDefault<Guid>(), model.PAR_METHOD_IDX.ConvertOrDefault<Guid>(), model.POC.GetValueOrDefault());
                if (_exist != null && _exist.PAR_METHOD_IDX != null)
                    ModelState.AddModelError("POC", "This parameter / method / POC already exists for this site.");

                model.ALERT_STUCK_REC_COUNT = 3;  //default value on insert
            }

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


                Guid? SuccInd = db_Air.InsertUpdatetT_QREST_MONITORS(model.MONITOR_IDX, model.SITE_IDX, model.PAR_METHOD_IDX, model.POC, model.DURATION_CODE, model.COLLECT_FREQ_CODE, 
                    model.COLLECT_UNIT_CODE, model.ALERT_MIN_VALUE, model.ALERT_MAX_VALUE, model.ALERT_AMT_CHANGE, model.ALERT_STUCK_REC_COUNT, UserIDX);

                if (SuccInd != null)
                {
                    TempData["Success"] = "Record updated";
                    return RedirectToAction("MonitorEdit", new { id = SuccInd });
                }
                else
                    TempData["Error"] = "Error updating record.";

            }

            //repopulate model
            //model.ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX);

            return View(model);
        }


        public ActionResult MonitorImport(Guid? id)
        {
            string UserIDX = User.Identity.GetUserId();


            //lookup site
            T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(id ?? Guid.Empty);
            if (_site == null)
            //site not found - return to site list
            {
                TempData["Error"] = "Site must be specified.";
                return RedirectToAction("SiteList", "Site");
            }
            else
            {
                //dont attempt import if missing information
                if (_site.COUNTY_CD == null || _site.STATE_CD == null || string.IsNullOrEmpty(_site.AQS_SITE_ID))
                {
                    TempData["Error"] = "You must set the county, state code, and AQS Site ID for the site before importing monitors.";
                    return RedirectToAction("SiteEdit", "Site", new { id = _site.SITE_IDX });
                }

                // check security (whether can update)
                if (db_Account.CanAccessThisOrg(UserIDX, _site.ORG_ID, true) == false)
                {
                    TempData["Error"] = "You don't have rights to edit sites for this agency.";
                    return RedirectToAction("SiteList", "Site");
                }

                //lookup to get the AQS Tribal Code for 
                T_QREST_ORGANIZATIONS _org = db_Ref.GetT_QREST_ORGANIZATION_ByID(_site.ORG_ID);

                if (_org.AQS_AGENCY_CODE?.Length < 3)
                {
                    TempData["Error"] = "You must specify an AQS Code for the agency before importing sites from AQS.";
                    return RedirectToAction("OrgEdit","Site");
                }

                var model = new vmSiteMonitorImport
                {
                    siteIDX = _site.SITE_IDX,
                    siteName = _site.SITE_NAME,
                    ImportMonitors = new List<SiteMonitorDisplayType>()
                };

                //grab remote JSON file from EPA AQS API
                using (var httpClient = new HttpClient())
                {
                    var json = httpClient.GetStringAsync("https://aqs.epa.gov/data/api/monitors/bySite?email=test@aqs.api&key=test&bdate=20000101&edate=20251231&state=" + _site.STATE_CD  + "&county=" + _site.COUNTY_CD + "&site=" + _site.AQS_SITE_ID).Result;
                    dynamic stuff = JsonConvert.DeserializeObject(json);
                    var data1 = stuff.Data;

                    foreach (var item in stuff.Data)
                    {
                        string _parcode = item.parameter_code;
                        string _method_code = item.last_method_code;
                        T_QREST_REF_PAR_METHODS _refMeth = db_Ref.GetT_QREST_REF_PAR_METHODS_ByParCdMethodCd(_parcode, _method_code);

                        T_QREST_MONITORS m = new T_QREST_MONITORS
                        {
                            MONITOR_IDX = Guid.NewGuid(),
                            SITE_IDX = _site.SITE_IDX,
                            PAR_METHOD_IDX = _refMeth.PAR_METHOD_IDX,
                            POC = item.poc,
                            CREATE_DT = System.DateTime.Now,
                            CREATE_USER_IDX = UserIDX
                        };

                        SiteMonitorDisplayType md = new SiteMonitorDisplayType
                        {
                            T_QREST_MONITORS = m,
                            SITE_ID = _site.STATE_CD + "-" + _site.COUNTY_CD + "-" + _site.AQS_SITE_ID,
                            PAR_NAME = _parcode + " - " + item.parameter_name,
                            METHOD_CODE = _method_code + " - " + item.last_method_description,
                            ORG_ID = _site.ORG_ID
                        };

                        //check if QREST already has the monitor
                        T_QREST_MONITORS _existMonitor = db_Air.GetT_QREST_MONITORS_bySiteIDX_ParMethod_POC(_site.SITE_IDX, _refMeth.PAR_METHOD_IDX, (int)item.poc);
                        m.MODIFY_USER_IDX = _existMonitor != null ? "U" : "I";
                        model.ImportMonitors.Add(md);
                    }
                }

                return View(model); ;
            }

        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MonitorImport(vmSiteMonitorImport model)
        {
            string UserIDX = User.Identity.GetUserId();

            //if user hasn't selected an org, return view now
            if (model.siteIDX == null)
            {
                TempData["Error"] = "Site not found";
                return View(model);
            }

            //// check security (whether can update)
            //if (db_Account.CanAccessThisOrg(UserIDX, model.selOrgID, true) == false)
            //{
            //    TempData["Error"] = "You don't have rights to edit this agency.";
            //    return RedirectToAction("SiteList", "Site");
            //}

            ////lookup to get the AQS Tribal Code for 
            //T_QREST_ORGANIZATIONS _org = db_Ref.GetT_QREST_ORGANIZATION_ByID(model.selOrgID);
            //if (_org == null)
            //{
            //    TempData["Error"] = "Organization not found";
            //    return View(model);
            //}

            int i = 0;
            foreach (var item in model.ImportMonitors)
            {
                if (item.T_QREST_MONITORS.MONITOR_IDX != null && item.monSelInd == true)
                {
                    i++;
                    db_Air.InsertUpdatetT_QREST_MONITORS(null, model.siteIDX, item.T_QREST_MONITORS.PAR_METHOD_IDX, item.T_QREST_MONITORS.POC,
                        "1", "1", null, null, null, null, null, UserIDX);
                }
            }

            if (i==0)
                TempData["Error"] = "You must select records to import.";

            return RedirectToAction("SiteEdit", new { id = model.siteIDX });
        }


        [HttpPost]
        public JsonResult MonitorDelete(string id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                Guid idg = new Guid(id);
                int SuccID = db_Air.DeleteT_QREST_MONITORS(idg);
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