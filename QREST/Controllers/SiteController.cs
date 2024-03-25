using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using QREST.App_Logic.BusinessLogicLayer;
using QREST.Models;
using QRESTModel.COMM;
using QRESTModel.DAL;
using QRESTModel.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QREST.Controllers
{
    [Authorize]
    public class SiteController : Controller
    {
       
        // GET: Site
        public ActionResult Index()
        {
            return RedirectToAction("SiteList");
        }


        //**********************AGENCY **************************************
        //**********************AGENCY **************************************
        //**********************AGENCY **************************************
        public ActionResult OrgList()
        {
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
                    model.LOCK_ACCESS_IND = _org.LOCK_ACCESS_IND;
                    model.edit_org_id = _org.ORG_ID;
                    model.edit_typ = "org";

                    model.org_users = db_Account.GetT_QREST_ORG_USERS_ByOrgID(model.ORG_ID, null, null);
                }

                return View(model);
            }
            else
            {
                TempData["Error"] = "You are not an Admin for this organization.";
                return RedirectToAction("Index", "Dashboard");
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
                    int succId = db_Ref.InsertUpdatetT_QREST_ORGANIZATION(model.ORG_ID, model.ORG_NAME, model.STATE_CD, model.EPA_REGION,
                        model.AQS_NAAS_UID, model.AQS_NAAS_PWD, model.AQS_AGENCY_CODE, model.SELF_REG_IND, true, "", null, null, model.LOCK_ACCESS_IND);

                    if (succId == 1)
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
                return RedirectToAction("Index", "Dashboard");
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrgEditUser(vmAdminOrgEditUser model)
        {
            if (ModelState.IsValid)
            {
                Guid? succId = db_Account.InsertUpdateT_QREST_ORG_USERS(model.edit_user_idx, model.edit_org_id, model.edit_org_user_access_level, model.edit_org_user_status, "");

                if (succId != null)
                {
                    //also update user activity log
                    string UserIDX = User.Identity.GetUserId();  //lookup user
                    var user = db_Account.GetT_QREST_USERS_ByID(UserIDX);
                    db_Ref.CreateT_QREST_SYS_LOG_ACTIVITY("ORG EDIT", UserIDX, null, "Edit org access for " + (user != null ? user.Email : "unknown") + " at " + model.edit_org_id, GetIP.GetLocalIPAddress(System.Web.HttpContext.Current), model.edit_org_id);

                    TempData["Success"] = "Record updated";
                }
                else
                    TempData["Error"] = "Error updating record.";
            }
            else
                TempData["Error"] = "Error updating record.";
            
            return RedirectToAction("OrgEdit", "Site", new { id = model.edit_org_id });
        }


        [HttpPost]
        public JsonResult UserOrgDelete(string id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                Guid _guid = new Guid(id);
                int SuccID = db_Account.DeleteT_QREST_ORG_USER(_guid);
                if (SuccID == 1)
                    return Json("Success");
                else
                    return Json("Unable to find user to delete.");
            }
        }



        //**********************SITES **************************************
        //**********************SITES **************************************
        //**********************SITES **************************************
        [HttpGet]
        public ActionResult SiteList(string selOrgID)
        {
            string UserIDX = User.Identity.GetUserId();

            if (string.IsNullOrEmpty(selOrgID))
                selOrgID = null;

            var model = new vmSiteSiteList
            {
                selOrgID = selOrgID,
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, false),
                T_QREST_SITES = db_Air.GetT_QREST_SITES_ByUser_OrgID(selOrgID, UserIDX)
            };

            return View(model);
        }


        [HttpGet]
        public ActionResult SiteEdit(Guid? id)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmSiteSiteEdit(); 

            T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(id ?? Guid.Empty);
            if (_site != null)
            {
                //reject if user doesn't have access to org
                RedirectToRouteResult r = CanAccessThisOrg(UserIDX, _site.ORG_ID, false);
                if (r != null) return r;

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
                model.PUB_WEB_IND = _site.PUB_WEB_IND ?? false;
                model.AIRNOW_USR = _site.AIRNOW_USR;
                model.AIRNOW_PWD = _site.AIRNOW_PWD;
                model.AIRNOW_ORG = _site.AIRNOW_ORG;
                model.AIRNOW_SITE = _site.AIRNOW_SITE;
                model.SITE_COMMENTS = _site.SITE_COMMENTS;
                model.LOCAL_TIMEZONE = _site.LOCAL_TIMEZONE;
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
            model.ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, false);
            model.monitors = db_Air.GetT_QREST_MONITORS_Display_bySiteIDX(model.SITE_IDX);
            model.notifiees = db_Air.GetT_QREST_SITE_NOTIFY_BySiteID(model.SITE_IDX);
            model.ddl_User = ddlHelpers.get_ddl_users(true);
            model.ddl_TimeZone = ddlHelpers.get_ddl_time_zone();

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

            //*************** VALIDATION BEGIN *********************************
            int SiteIDcheckCnt = db_Air.GetT_QREST_SITES_ByOrgandSiteID(model.ORG_ID, model.SITE_ID, model.SITE_IDX ?? Guid.Empty);
            if (SiteIDcheckCnt > 0) 
                ModelState.AddModelError("SITE_ID", "Site ID must be unique.");

            //reject if user doesn't have access to org
            RedirectToRouteResult r = CanAccessThisOrg(UserIDX, model.ORG_ID, true);
            if (r != null) return r;

            if (model.AIRNOW_IND == true && model.AIRNOW_SITE == null)
                ModelState.AddModelError("AIRNOW_SITE", "If enabling AirNow streaming, 9-digit AirNow site ID is needed.");

            //*************** VALIDATION END *********************************


            if (ModelState.IsValid)
            {
                Guid? succId = db_Air.InsertUpdatetT_QREST_SITES(model.SITE_IDX, model.ORG_ID, model.SITE_ID, model.SITE_NAME, model.AQS_SITE_ID ?? "",
                    model.STATE_CD ?? "", model.COUNTY_CD ?? "", model.LATITUDE, model.LONGITUDE, model.ELEVATION, model.ADDRESS ?? "", model.CITY ?? "", model.ZIP_CODE ?? "",
                    model.START_DT, model.END_DT, model.POLLING_ONLINE_IND, null, null, null, null, model.AIRNOW_IND, model.AQS_IND, model.AIRNOW_USR ?? "", model.AIRNOW_PWD, 
                    model.AIRNOW_ORG, model.AIRNOW_SITE ?? "", model.SITE_COMMENTS ?? "", UserIDX, model.LOCAL_TIMEZONE, model.PUB_WEB_IND);

                if (succId != null)
                {
                    TempData["Success"] = "Record updated";
                    return RedirectToAction("SiteEdit", "Site", new { id = succId });
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
                //reject if user doesn't have access to org
                RedirectToRouteResult r = CanAccessThisSite(User.Identity.GetUserId(), model.SITE_IDX.GetValueOrDefault(), true);
                if (r != null) return Json("Access Denied.");


                Guid? succId = db_Air.InsertUpdatetT_QREST_SITE_NOTIFY(null, model.SITE_IDX, model.edit_notify_user_idx.ToString(), User.Identity.GetUserId());

                if (succId != null)
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
                Guid idg = new Guid(id);

                //reject if user doesn't have access to org
                T_QREST_SITE_NOTIFY _sitenotify = db_Air.GetT_QREST_SITE_NOTIFY_ByID(idg);
                if (_sitenotify != null)
                {
                    RedirectToRouteResult r = CanAccessThisSite(User.Identity.GetUserId(), _sitenotify.SITE_IDX, true);
                    if (r != null) return Json("Access Denied.");
                }
                else
                    return Json("No record selected to delete.");



                int succId = db_Air.DeleteT_QREST_SITE_NOTIFY(idg);
                if (succId == 1)
                    return Json("Success");
                else
                    return Json("Unable to find record to delete.");
            }
        }


        [HttpGet]
        public ActionResult SiteImport(string selOrgID)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmSiteSiteImport
            {
                selOrgID = selOrgID,
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, false),
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
                RedirectToRouteResult r = CanAccessThisOrg(UserIDX, model.selOrgID, true);
                if (r != null) return r;

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
                catch (AggregateException err)
                {
                    foreach (var errInner in err.InnerExceptions)
                        db_Ref.CreateT_QREST_SYS_LOG(UserIDX, "ERROR", "Failed to import monitor - code 5 " + errInner);
                    TempData["Error"] = "Unable to connect to AQS, please try again later.";
                    return View(model);
                }
                catch (Exception ex)
                {
                    db_Ref.CreateT_QREST_SYS_LOG(UserIDX, "ERROR", "Failed to import monitor - code 4 " + ex.Message);
                    TempData["Error"] = "Unable to connect to AQS, please try again later.";
                    return View(model);
                }
            }

            return View(model); ;
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SiteImport(vmSiteSiteImport model)
        {
            //if user hasn't selected an org, return view now
            if (model.selOrgID == null)
            {
                TempData["Error"] = "Organization not found";
                return View(model);
            }

            //reject if user doesn't have access to org
            string UserIDX = User.Identity.GetUserId();
            RedirectToRouteResult r = CanAccessThisOrg(UserIDX, model.selOrgID, true);
            if (r != null) return r;

            //lookup to get the AQS Tribal Code for 
            T_QREST_ORGANIZATIONS _org = db_Ref.GetT_QREST_ORGANIZATION_ByID(model.selOrgID);
            if (_org == null)
            {
                TempData["Error"] = "Organization not found";
                return View(model);
            }

            try
            {
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
                        if (cols.Length > 0 && cols[9] != "None" && cols[9] == _org.AQS_AGENCY_CODE) //skip blank rows
                        {
                            //check if QREST already has the site.
                            T_QREST_SITES _existSite = db_Air.GetT_QREST_SITES_ByOrgandAQSID(model.selOrgID, cols[5]);
                            if (_existSite == null)
                                db_Air.InsertUpdatetT_QREST_SITES(null, model.selOrgID, cols[5], cols[7], cols[5], cols[1], cols[3], null, null, null, null, null,
                                    null, null, null, false, null, null, null, null, false, false, null, null, null, null, null, UserIDX, null, false);
                        }

                    }
                }
            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                    db_Ref.CreateT_QREST_SYS_LOG(UserIDX, "ERROR", "Failed to import monitor - code 5 " + errInner);
                TempData["Error"] = "Unable to connect to AQS, please try again later.";
            }
            catch (Exception ex)
            {
                db_Ref.CreateT_QREST_SYS_LOG(UserIDX, "ERROR", "Failed to import monitor - code 4 " + ex.Message);
                TempData["Error"] = "Unable to connect to AQS, please try again later.";
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
                {
                    RedirectToRouteResult r = CanAccessThisOrg(User.Identity.GetUserId(), _site.ORG_ID, true);
                    if (r != null) return Json("Access Denied.");
                }

                int succId = db_Air.DeleteT_QREST_SITES(idg);
                if (succId == 1)
                    return Json("Success");
                else if (succId == -1)
                    return Json("Cannot delete Site that still has monitor records. Delete monitors first.");
                else
                    return Json("Unable to find site to delete.");
            }
        }

        [HttpGet]
        public JsonResult TestAIRNOW(Guid? id)
        {
            T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(id ?? Guid.Empty);
            if (_site != null)
            {
                string ftpUser = _site.AIRNOW_USR;
                string ftpPwd = _site.AIRNOW_PWD;
                string ip = "webdmcdata.airnowtech.org";
                using (var client = new Renci.SshNet.SftpClient(ip, ftpUser, ftpPwd))
                {
                    try
                    {
                        client.Connect();
                        client.ChangeDirectory("AQCSV");
                        var xxx = client.ListDirectory(".");
                        client.Disconnect();
                        return Json("Success", JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        return Json("Failed: " + ex.Message + " Note: this might mean you have entered valid AirNOW credentials, but these credentials do not have sufficient FTP rights to AirNOW", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json("Failed");
        }


        //**********************SITE POLLING **************************************
        //**********************SITE POLLING **************************************
        //**********************SITE POLLING **************************************
        [HttpGet]
        public ActionResult SitePollConfig(Guid? id, Guid? configid, int? n)
        {
            //*********if config id supplied and site id not supplied, then grab site id
            if (configid != null && id == null)
            {
                var temp = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(configid.ConvertOrDefault<Guid>());
                if (temp != null)
                    id = temp.SITE_IDX;
            }



            T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(id ?? Guid.Empty);
            if (_site != null)
            {
                //reject if user doesn't have access to org
                RedirectToRouteResult r = CanAccessThisOrg(User.Identity.GetUserId(), _site.ORG_ID, false);
                if (r != null) return r;

                //get listing of configs
                var model = new vmSiteSitePollConfig {
                    SITE_IDX = id,
                    ConfigList = db_Air.GetT_QREST_SITE_POLL_CONFIG_BySite(id.ConvertOrDefault<Guid>(), false, true),
                    ddl_Monitors = ddlHelpers.get_monitors_by_site(_site.SITE_IDX, false, true),
                    POLLING_LAST_RUN_DT = _site.POLLING_LAST_RUN_DT,
                    POLLING_NEXT_RUN_DT = _site.POLLING_NEXT_RUN_DT,
                };

                //get config to edit
                if (n == 1) //new case
                {
                    model.editPOLL_CONFIG_IDX = Guid.NewGuid();
                    model.editACT_IND = false;
                }
                else  //update case
                {
                    //get config if supplied
                    T_QREST_SITE_POLL_CONFIG e = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(configid.ConvertOrDefault<Guid>());

                    //if none, then display active
                    if (e == null)
                        e = db_Air.GetT_QREST_SITE_POLL_CONFIG_ActiveByID(_site.SITE_IDX);

                    //if still none, then display first in config list
                    if (e == null && model.ConfigList.Count > 0)
                        e = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(model.ConfigList[0].POLL_CONFIG_IDX);

                    if (e != null)
                    {
                        model.editCONFIG_NAME = e.CONFIG_NAME;
                        model.editPOLL_CONFIG_IDX = e.POLL_CONFIG_IDX;
                        model.editRAW_DURATION_CODE = e.RAW_DURATION_CODE;
                        model.editLOGGER_TYPE = e.LOGGER_TYPE;
                        model.editLOGGER_SOURCE = e.LOGGER_SOURCE;
                        model.editLOGGER_PORT = e.LOGGER_PORT;
                        model.editLOGGER_USERNAME = e.LOGGER_USERNAME;
                        model.editLOGGER_PASSWORD = e.LOGGER_PASSWORD;
                        model.editDELIMITER = e.DELIMITER;
                        model.editDATE_COL = e.DATE_COL;
                        model.editDATE_FORMAT = e.DATE_FORMAT;
                        model.editTIME_COL = e.TIME_COL;
                        model.editTIME_FORMAT = e.TIME_FORMAT;
                        model.editLOCAL_TIMEZONE = _site.LOCAL_TIMEZONE;
                        model.editTIME_POLL_TYPE = e.TIME_POLL_TYPE;
                        model.editLOGGER_FILE_NAME = e.LOGGER_FILE_NAME;
                        model.editACT_IND = e.ACT_IND;

                        //display polling warning message
                        if ((e.LOGGER_TYPE== "ZENO" || e.LOGGER_TYPE == "SUTRON") && (e.RAW_DURATION_CODE == null || e.DELIMITER == null || _site.LOCAL_TIMEZONE == null || e.DATE_COL == null || e.TIME_COL == null))
                            ViewBag.PollError = true;
                        else if (e.LOGGER_TYPE == "WEATHER_PWS" && (e.RAW_DURATION_CODE == null || _site.LOCAL_TIMEZONE == null))
                            ViewBag.PollError = true;
                    }

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

            //*************** VALIDATION BEGIN *********************************
            if (model.editLOGGER_TYPE == "WEATHER_PWS")
            {
                if (model.editLOGGER_SOURCE == null)
                    ModelState.AddModelError("editLOGGER_SOURCE", "Station ID required");
                if (model.editLOGGER_PORT == null)
                    model.editLOGGER_PORT = 0;
                model.editTIME_COL = 2;
                model.editDATE_COL = 1;
                model.editDELIMITER = "C";
            }

            //if (model.editLOGGER_TYPE == "SUTRON_LEADS")
            //{
            //    if (string.IsNullOrEmpty(model.editLOGGER_FILE_NAME))
            //        ModelState.AddModelError("editLOGGER_FILE_NAME", "Log file name required for Sutron w/LEADS");
            //}
            //*************** VALIDATION END  *********************************

            model.ConfigList = db_Air.GetT_QREST_SITE_POLL_CONFIG_BySite(model.SITE_IDX.ConvertOrDefault<Guid>(), false, true);

            if (ModelState.IsValid)
            {
                T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(model.SITE_IDX ?? Guid.Empty);
                if (_site != null)
                {
                    //reject if user doesn't have access to org
                    RedirectToRouteResult r = CanAccessThisOrg(UserIDX, _site.ORG_ID, true);
                    if (r != null) return r;

                    //set all others to inactive if this one is active
                    if (model.editACT_IND == true)
                    {
                        List<T_QREST_SITE_POLL_CONFIG> _ps = db_Air.GetT_QREST_SITE_POLL_CONFIG_BySite(model.SITE_IDX.ConvertOrDefault<Guid>(), true, true);
                        foreach (T_QREST_SITE_POLL_CONFIG _p in _ps)
                            db_Air.UpdatetT_QREST_SITE_POLL_CONFIG_SetInactive(_p.POLL_CONFIG_IDX);
                    }

                    //prepopulate some default info for WEATHER STATION
                    if (model.editLOGGER_TYPE == "WEATHER_PWS")
                    {
                        model.editRAW_DURATION_CODE = "1";
                        model.editTIME_POLL_TYPE = "L";
                    }

                    if (model.editLOGGER_TYPE == "ESC" && model.editDELIMITER == null) model.editDELIMITER = "C";
                    if (model.editLOGGER_TYPE == "ESC" && model.editDATE_COL == null) model.editDATE_COL = 1;
                    if (model.editLOGGER_TYPE == "ESC" && model.editTIME_COL == null) model.editTIME_COL = 1;


                    Guid? succId = db_Air.InsertUpdatetT_QREST_SITE_POLL_CONFIG(model.editPOLL_CONFIG_IDX, model.SITE_IDX, model.editCONFIG_NAME, model.editRAW_DURATION_CODE, model.editLOGGER_TYPE,
                        model.editLOGGER_SOURCE, model.editLOGGER_PORT, model.editLOGGER_USERNAME, model.editLOGGER_PASSWORD, model.editDELIMITER, model.editDATE_COL,
                        model.editDATE_FORMAT, model.editTIME_COL, model.editTIME_FORMAT, null, model.editACT_IND, UserIDX, _site.SITE_NAME, model.editTIME_POLL_TYPE, true, model.editPOLL_LOG_DESC, null, model.editLOGGER_FILE_NAME);

                    if (succId != null)
                    {
                        TempData["Success"] = "Record updated";
                        return RedirectToAction("SitePollConfig", new { id = model.SITE_IDX, configid = succId });
                    }
                    else
                        TempData["Error"] = "Error updating record.";
                }
            }

            //reinitialize model
            model.ddl_Monitors = ddlHelpers.get_monitors_by_site(model.SITE_IDX.ConvertOrDefault<Guid>(), false, true);
            return View(model);
        }


        [HttpPost]
        public JsonResult SitePollConfigDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Json("No record selected to delete");
            else
            {
                Guid idg = new Guid(id);

                //reject if user doesn't have access to org
                RedirectToRouteResult r = CanAccessThisOrg(User.Identity.GetUserId(), db_Air.GetT_QREST_SITE_POLL_CONFIG_org_ByID(idg), true);
                if (r != null) return Json("Access Denied");

                int succId = db_Air.DeleteT_QREST_SITE_POLL_CONFIG(idg);
                if (succId == 1)
                    return Json("Success");
                else
                    return Json("Unable to find polling configuration to delete.");
            }
        }


        [HttpPost]
        public JsonResult SitePollConfigDtlData()
        {
            var id = Request.Form.GetValues("pollid")?.FirstOrDefault();
            if (id != null)
            {
                Guid idg = new Guid(id);
                var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
                var data = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID(idg);
                return Json(new { draw, recordsFiltered = 100, recordsTotal = 100, data = data });
            }
            else
                return null;
        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult SitePollConfigDtlEdit(Guid? configid, Guid? configdtlid, Guid? monid, int? col, string sumtype, int? rounding, double? adjustfactor)
        {
            if (configid != null && monid != null && col != null)
            {
                string UserIDX = User.Identity.GetUserId();
                SiteMonitorDisplayType _mon = db_Air.GetT_QREST_MONITORS_ByID(monid ?? Guid.Empty);
                if (_mon != null)
                {
                    T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(_mon.T_QREST_MONITORS.SITE_IDX);
                    if (_site != null)
                    {
                        //reject if user doesn't have access to org
                        RedirectToRouteResult r = CanAccessThisOrg(UserIDX, _site.ORG_ID, true);
                        if (r != null) return Json(new { msg = "Access Denied" });

                        Guid? succId = db_Air.InsertUpdatetT_QREST_SITE_POLL_CONFIG_DTL(configdtlid, configid, monid, col, sumtype, rounding, adjustfactor, UserIDX);
                        if (succId != null)
                            return Json(new { msg = "Success" });

                    }
                }
            }
            //return ERROR
            return Json(new { msg = "Unable to save." });
        }


        [HttpPost]
        public JsonResult SitePollConfigDtlDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Json("No record selected to delete");
            else
            {
                Guid idg = new Guid(id);

                //reject if user doesn't have access to org
                RedirectToRouteResult r = CanAccessThisOrg(User.Identity.GetUserId(), db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_org_ByID(idg), true);
                if (r != null) 
                    return Json("Access Denied");

                int succId = db_Air.DeleteT_QREST_SITE_POLL_CONFIG_DTL(idg);
                if (succId == 1)
                    return Json("Success");
                else
                    return Json("Unable to find column mapping to delete.");
            }
        }



        public ActionResult SitePollPing(Guid? id)
        {
            T_QREST_SITE_POLL_CONFIG _config = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(id ?? Guid.Empty);
            if (_config == null)
            {
                TempData["Error"] = "Monitor polling configuration not found";
                return RedirectToAction("SiteList", "Site");
            }
            //FAIL IF LOGGER CONFIGURATION IS INCOMPLETE
            else if (_config.LOGGER_SOURCE == null || _config.LOGGER_PORT == null)
            {
                TempData["Error"] = "Polling configuration is incomplete: logger source and port must be supplied";
                return RedirectToAction("SiteList", "Site");
            }

            //reject if user doesn't have access to org
            string OrgID = db_Air.GetT_QREST_SITE_POLL_CONFIG_org_ByID(id ?? Guid.Empty);
            RedirectToRouteResult r = CanAccessThisOrg(User.Identity.GetUserId(), OrgID, true);
            if (r != null) return r;


            //initialize model
            var model = new vmSitePing
            {
                POLL_CONFIG_IDX = id ?? Guid.Empty,
                recCount = _config.LOGGER_TYPE == "SUTRON_LEADS" ? 0 : 5,
                LOGGER_TYPE = _config.LOGGER_TYPE
            };


            return View(model);
        }



        public ActionResult SitePollOffline(Guid id)
        {
            string UserIDX = User.Identity.GetUserId();

            //reject if user doesn't have access to org
            RedirectToRouteResult r = CanAccessThisOrg(UserIDX, db_Air.GetT_QREST_SITE_POLL_CONFIG_org_ByID(id), true);
            if (r != null) return Json("Access Denied");


            var model = new vmSitePollOffline
            {
                POLL_CONFIG_IDX = id
            };

            var _config = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(model.POLL_CONFIG_IDX);
            if (_config != null)
            {
                model.SITE_IDX = _config.SITE_IDX;
            }


            return View(model);
        }


        [HttpPost]
        public ActionResult SitePollOffline(vmSitePollOffline model)
        {
            string UserIDX = User.Identity.GetUserId();

            //reject if user doesn't have access to org
            RedirectToRouteResult r = CanAccessThisOrg(UserIDX, db_Air.GetT_QREST_SITE_POLL_CONFIG_org_ByID(model.POLL_CONFIG_IDX), true);
            if (r != null) return Json("Access Denied");

            var _config = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(model.POLL_CONFIG_IDX);
            if (_config != null)
            {
                T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(_config.SITE_IDX);
                if (_site != null)
                {
                    //take offline
                    Guid? succId = db_Air.InsertUpdatetT_QREST_SITE_POLL_CONFIG(model.POLL_CONFIG_IDX, null, null, null, null, null, null, null, null,
                    null, null, null, null, null, null, false, UserIDX, _site.SITE_NAME, null);

                    //log reason why
                    db_Ref.CreateT_QREST_SYS_LOG_ACTIVITY("POLLING CONFIG", UserIDX, null, "Taking offline; reason: " + model.OfflineReason, null, model.POLL_CONFIG_IDX.ToString());


                    if (succId != null)
                    {
                        TempData["Success"] = "Configuration Taken Offline";
                        return RedirectToAction("SitePollConfig", new { configid = model.POLL_CONFIG_IDX });
                    }
                }
            }

            TempData["Error"] = "Error updating record.";
            return RedirectToAction("SitePollConfig", new { configid = model.POLL_CONFIG_IDX });
        }


        [HttpPost]
        public ActionResult SitePollPing(vmSitePing model)
        {
            //reject if user doesn't have access to org
            string OrgID = db_Air.GetT_QREST_SITE_POLL_CONFIG_org_ByID(model.POLL_CONFIG_IDX);
            RedirectToRouteResult r = CanAccessThisOrg(User.Identity.GetUserId(), OrgID, true);
            if (r != null) return r;

            T_QREST_SITE_POLL_CONFIG _config = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(model.POLL_CONFIG_IDX);


            //FAIL IF NO CONFIGURATION FOUND
            if (_config == null || _config.LOGGER_SOURCE == null || _config.LOGGER_PORT == null)
                ModelState.AddModelError("pingType", "Polling configuration is incomplete: logger source and port must be supplied.");

            //SITE ID MUST BE 4 DIGITS (ZENO only)
            string siteID = db_Air.GetT_QREST_SITE_POLL_CONFIG_SiteID_ByID(_config.POLL_CONFIG_IDX);
            if ((siteID == null || siteID.Length != 4) && _config?.LOGGER_TYPE == "ZENO")
                ModelState.AddModelError("pingType", "Site ID must be a 4 digit number that corresponds to ID configured on logger.");

            //SITE ID MUST BE 2 DIGITS (ESC only)
            if ((siteID == null || siteID.Length != 2) && _config?.LOGGER_TYPE == "ESC")
                ModelState.AddModelError("pingType", "Site ID must be a 2 character code that corresponds to ID configured on logger.");


            //THIS PAGE ONLY WORKS WITH SPECIFIED LOGGERS
            if (_config?.LOGGER_TYPE != "ZENO" && _config?.LOGGER_TYPE != "SUTRON" && _config?.LOGGER_TYPE != "ESC" && _config?.LOGGER_TYPE != "SUTRON_LEADS" && _config?.LOGGER_TYPE != "MET_ONE_BAM" && _config?.LOGGER_TYPE != "MET_BAM_1022")
                ModelState.AddModelError("pingType", "Ping currently only available for Zeno, Sutron, or ESC dataloggers, or MetOne BAM.");


            //ONLY ZENO LOGGERS ALLOW THE PING-ONLY OPTION
            if (_config?.LOGGER_TYPE != "ZENO" && model.pingType == "Ping Only")
                ModelState.AddModelError("pingType", "Ping Only option is only available for Zeno logger.");


            if (ModelState.IsValid)
            {
                // ***************** PING ONLY: THIS ATTEMPTS TO ACCESS THE USER INTERFACE **********************
                if (model.pingType == "Ping Only")
                    model.pingResults2 = LoggerComm.ConnectTcpClientPing(_config.LOGGER_SOURCE, _config.LOGGER_PORT.ConvertOrDefault<ushort>(), _config.LOGGER_PASSWORD);
                // ***************** RETRIEVE DATA: THIS POLLS DATA FROM THE LOGGER USING SAIL **********************
                else if (model.pingType == "Retrieve Data")
                {
                    CommMessageLog _log = new CommMessageLog();
                    if (_config.LOGGER_TYPE == "ESC")
                    {
                        string todayJulian = System.DateTime.Now.AddHours(-7).DayOfYear.ToString().PadLeft(3, '0');
                        //_log = LoggerComm.ConnectTcpESC(_config.LOGGER_SOURCE, _config.LOGGER_PORT.ConvertOrDefault<ushort>(), todayJulian + "060000" + "|Y|" + todayJulian + "230000", siteID);
                        _log = LoggerComm.ConnectTcpESC(_config.LOGGER_SOURCE, _config.LOGGER_PORT.ConvertOrDefault<ushort>(), todayJulian + "010000" + "|Y|" + todayJulian + "230000", siteID);
                        if (_log.CommMessageStatus)
                            model.loggerData = _log.CommResponse;
                        else
                            model.pingResults2 = new List<CommMessageLog> { _log };
                    }
                    else if (_config.LOGGER_TYPE == "SUTRON_LEADS")
                    {
                        //10 hours ago
                        //var tenHrAgo = System.DateTime.Now.AddHours(-10).ToString("MM-dd-yyyy HH:00:00");
                        //_log = LoggerComm.ConnectTcpSutron(_config.LOGGER_SOURCE, _config.LOGGER_PORT.ConvertOrDefault<ushort>(), _config.LOGGER_USERNAME, _config.LOGGER_PASSWORD, "GET /S 06-06-2022 03:00:00");
                        string msg1 = string.IsNullOrEmpty(_config.LOGGER_FILE_NAME) ? "GET /TODAY /C" : "GET /TODAY /F " + _config.LOGGER_FILE_NAME + " /C";
                        _log = LoggerComm.ConnectTcpSutron(_config.LOGGER_SOURCE, _config.LOGGER_PORT.ConvertOrDefault<ushort>(), _config.LOGGER_USERNAME, _config.LOGGER_PASSWORD, msg1);
                        if (_log.CommMessageStatus)
                            model.loggerData = _log.CommResponse;
                        else
                            model.pingResults2 = new List<CommMessageLog> { _log };
                    }
                    else if (_config.LOGGER_TYPE == "MET_ONE_BAM")
                    {
                        _log = LoggerComm.ConnectTcpBAM(_config.LOGGER_SOURCE, _config.LOGGER_PORT.ConvertOrDefault<ushort>());
                        if (_log.CommMessageStatus)
                            model.loggerData = _log.CommResponse;
                        else
                            model.pingResults2 = new List<CommMessageLog> { _log };
                    }
                    else if (_config.LOGGER_TYPE == "MET_BAM_1022")
                    {
                        _log = LoggerComm.ConnectTcpBAM1022(_config.LOGGER_SOURCE, _config.LOGGER_PORT.ConvertOrDefault<ushort>(), model.recCount);
                        if (_log.CommMessageStatus)
                            model.loggerData = _log.CommResponse;
                        else
                            model.pingResults2 = new List<CommMessageLog> { _log };
                    }
                    else
                    {
                        _log = LoggerComm.ConnectTcpClientSailer(_config.LOGGER_SOURCE, _config.LOGGER_PORT.ConvertOrDefault<ushort>(), "DL" + model.recCount + ",", siteID);
                        if (_log.CommMessageStatus)
                            model.loggerData = LoggerComm.stripMessage(_log.CommResponse, "#0001" + siteID);
                        else
                            model.pingResults2 = new List<CommMessageLog> { _log };
                    }
                }
            }

            return View(model);
        }


        public ActionResult ViewChangeLog(Guid? id)
        {
            T_QREST_SITE_POLL_CONFIG _config = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(id ?? Guid.Empty);
            if (_config == null)
            {
                TempData["Error"] = "Monitor polling configuration not found";
                return RedirectToAction("SiteList", "Site");
            }

            //reject if user doesn't have access to org
            string OrgID = db_Air.GetT_QREST_SITE_POLL_CONFIG_org_ByID(id ?? Guid.Empty);
            RedirectToRouteResult r = CanAccessThisOrg(User.Identity.GetUserId(), OrgID, true);
            if (r != null) return r;

            //initialize model
            var model = new vmSharedLogActivity
            { 
                SITE_IDX = _config.SITE_IDX.ToString(),
                POLL_CONFIG_IDX = _config.POLL_CONFIG_IDX.ToString()
            };

            return View(model);
        }


        public async Task<ActionResult> LoggerPollManual(Guid? id)
        {
            T_QREST_SITE_POLL_CONFIG _config = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(id ?? Guid.Empty);
            if (_config == null)
            {
                TempData["Error"] = "Monitor polling configuration not found";
                return RedirectToAction("SiteList", "Site");
            }

            //reject if user doesn't have access to site
            string UserIDX = User.Identity.GetUserId();
            RedirectToRouteResult r = CanAccessThisSite(UserIDX, _config.SITE_IDX, false);
            if (r != null) return r;


            if (_config.LOGGER_TYPE == "WEATHER_PWS")
            {
                bool xxx = await LoggerComm.RetrieveWeatherCompanyPWS(_config);
                if (xxx == false)
                    TempData["Error"] = "Weather station not returning valid data";
                else
                    TempData["Success"] = "Polling successful";
            }
            else if (_config.LOGGER_TYPE == "SUTRON_LEADS")
            {
                SitePollingConfigType _config2 = db_Air.GetT_QREST_SITES_POLLING_CONFIG_Single(_config.POLL_CONFIG_IDX);
                List<SitePollingConfigDetailType> _config_dtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_config.POLL_CONFIG_IDX, true);

                string msg1 = string.IsNullOrEmpty(_config.LOGGER_FILE_NAME) ? "GET /TODAY /C" : "GET /TODAY /F " + _config.LOGGER_FILE_NAME + " /C";
                CommMessageLog _log = LoggerComm.ConnectTcpSutron(_config.LOGGER_SOURCE, (ushort)_config.LOGGER_PORT, _config.LOGGER_USERNAME, _config.LOGGER_PASSWORD, msg1);
                if (_log.CommMessageStatus && _log.CommResponse != null && _log.CommResponse.Length > 20)
                {
                    //send the entire text response to the file parser routine
                    LoggerComm.ParseFlatFile(_log.CommResponse, _config2, _config_dtl, true);
                }
                else
                    TempData["Error"] = "Error in polling: " + _log.CommMessageType;
            }
            else if (_config.LOGGER_TYPE == "ESC")
            {
                SitePollingConfigType _config2 = db_Air.GetT_QREST_SITES_POLLING_CONFIG_Single(_config.POLL_CONFIG_IDX);
                List<SitePollingConfigDetailType> _config_dtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_config.POLL_CONFIG_IDX, true);

                DateTime stDate = System.DateTime.Now.AddHours(-20);
                DateTime endDate = System.DateTime.Now.AddHours(5);
                string startJulian = stDate.DayOfYear.ToString().PadLeft(3, '0');
                string endJulian = endDate.DayOfYear.ToString().PadLeft(3, '0');
                string startHour = stDate.Hour.ToString().PadLeft(2, '0');
                string endtHour = endDate.Hour.ToString().PadLeft(2, '0');
                CommMessageLog _log = LoggerComm.ConnectTcpESC(_config2.LOGGER_SOURCE, (ushort)_config2.LOGGER_PORT, startJulian + startHour + "0000" + "|Y|" + endJulian + endtHour + "0000", _config2.SITE_ID);
                if (_log.CommMessageStatus && _log.CommResponse != null && _log.CommResponse.Length > 20)
                {
                    //send the entire text response to the file parser routine
                    LoggerComm.ParseFlatFileESC(_log.CommResponse, _config2, _config_dtl, true);

                }
                else
                    TempData["Error"] = "Error in polling: " + _log.CommMessageType;
            }
            else if (_config.LOGGER_TYPE == "MET_ONE_BAM")
            {
                SitePollingConfigType _config2 = db_Air.GetT_QREST_SITES_POLLING_CONFIG_Single(_config.POLL_CONFIG_IDX);
                List<SitePollingConfigDetailType> _config_dtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_config.POLL_CONFIG_IDX, true);

                CommMessageLog _log = LoggerComm.ConnectTcpBAM(_config.LOGGER_SOURCE, (ushort)_config.LOGGER_PORT);
                if (_log.CommMessageStatus && _log.CommResponse != null && _log.CommResponse.Length > 20)
                {
                    //send the entire text response to the file parser routine
                    LoggerComm.ParseFlatFileMetOneBAM(_log.CommResponse, _config2, _config_dtl, true);
                }
                else
                    TempData["Error"] = "Error in polling: " + _log.CommMessageType;
            }
            else if (_config.LOGGER_TYPE == "MET_BAM_1022")
            {
                SitePollingConfigType _config2 = db_Air.GetT_QREST_SITES_POLLING_CONFIG_Single(_config.POLL_CONFIG_IDX);
                List<SitePollingConfigDetailType> _config_dtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_config.POLL_CONFIG_IDX, true);

                //find how many records to retrieve
                int hrs = -1;
                DateTime? latestValue = db_Air.SP_LATEST_POLLED_DATE(_config.SITE_IDX, _config.RAW_DURATION_CODE, _config.TIME_POLL_TYPE);
                if (latestValue != null)
                {
                    TimeSpan difference = DateTime.Now - latestValue.Value;
                    hrs = difference.TotalHours.ConvertOrDefault<int>();
                    if (hrs > 2000) hrs = -1;
                }
                CommMessageLog _log = LoggerComm.ConnectTcpBAM1022(_config.LOGGER_SOURCE, (ushort)_config.LOGGER_PORT, hrs);
                if (_log.CommMessageStatus && _log.CommResponse != null && _log.CommResponse.Length > 20)
                {
                    //send the entire text response to the file parser routine
                    LoggerComm.ParseFlatFile(_log.CommResponse, _config2, _config_dtl, true);
                }
                else
                    TempData["Error"] = "Error in polling: " + _log.CommMessageType;
            }
            return RedirectToAction("SitePollConfig", new { configid = id });
        }


        //**********************MONITORS **************************************
        //**********************MONITORS **************************************
        //**********************MONITORS **************************************
        public ActionResult MonitorList(string selOrgID)
        {
            string UserIDX = User.Identity.GetUserId();

            if (string.IsNullOrEmpty(selOrgID))
                selOrgID = null;

            var model = new vmSiteMonitorList
            {
                selOrgID = selOrgID,
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, false),
                monitors = db_Air.GetT_QREST_MONITORS_ByUser_OrgID(selOrgID, UserIDX)
            };

            return View(model);
        }


        public ActionResult MonitorEdit(Guid? id, Guid? siteIDX, string refr)
        {
            string UserIDX = User.Identity.GetUserId();
            var model = new vmSiteMonitorEdit();
            model.refr = refr ?? "s";
            //insert case (Site ID provided but no Monitor ID)
            if (id == null && siteIDX != null)
            {
                //reject if user doesn't have access to site
                RedirectToRouteResult r = CanAccessThisSite(UserIDX, (Guid)siteIDX, false);
                if (r != null) return r;

                model.SITE_IDX = siteIDX;
                model.ddl_Unit = ddlHelpers.get_ddl_ref_units_with_code_too(null);
            }

            //update case
            else if (id != null && siteIDX == null)
            {
                SiteMonitorDisplayType _monitor = db_Air.GetT_QREST_MONITORS_ByID(id ?? Guid.Empty);
                if (_monitor != null)
                {
                    //reject if user doesn't have access to site
                    RedirectToRouteResult r = CanAccessThisSite(UserIDX, _monitor.T_QREST_MONITORS.SITE_IDX, false);
                    if (r != null) return r;

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
                    model.ALERT_MIN_TYPE = _monitor.T_QREST_MONITORS.ALERT_MIN_TYPE;
                    model.ALERT_MAX_TYPE = _monitor.T_QREST_MONITORS.ALERT_MAX_TYPE;
                    model.ALERT_AMT_CHANGE_TYPE = _monitor.T_QREST_MONITORS.ALERT_AMT_CHANGE_TYPE;
                    model.ALERT_STUCK_TYPE = _monitor.T_QREST_MONITORS.ALERT_STUCK_TYPE;
                    model.ddl_Unit = ddlHelpers.get_ddl_ref_units_with_code_too(_monitor.PAR_CODE);

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

            //populate site name
            if (model.SITE_IDX != null)
            {
                T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(model.SITE_IDX.GetValueOrDefault());
                model.SITE_NAME = _site.SITE_NAME;
            }

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MonitorEdit(vmSiteMonitorEdit model)
        {
            string UserIDX = User.Identity.GetUserId();

            //*************** VALIDATION BEGIN *********************************
            if (model.ALERT_MIN_VALUE != null && model.ALERT_MAX_VALUE != null && model.ALERT_MAX_VALUE <= model.ALERT_MIN_VALUE)
                ModelState.AddModelError("ALERT_MIN_VALUE", "Alert minimum must be lower than alert maximum.");
            
            if (model.MONITOR_IDX == null && model.POC != null && model.PAR_METHOD_IDX != null)
            {
                T_QREST_MONITORS _exist = db_Air.GetT_QREST_MONITORS_bySiteIDX_ParMethod_POC(model.SITE_IDX.ConvertOrDefault<Guid>(), model.PAR_METHOD_IDX.ConvertOrDefault<Guid>(), model.POC.GetValueOrDefault());
                if (_exist != null)
                    ModelState.AddModelError("POC", "This parameter / method / POC already exists for this site.");
            }

            //reject if user doesn't have access to site
            RedirectToRouteResult r = CanAccessThisSite(UserIDX, (Guid)model.SITE_IDX, true);
            if (r != null) return r;
            //*************** VALIDATION END *********************************

            if (ModelState.IsValid)
            {
                //add default values on insert
                if (model.MONITOR_IDX == null)
                {
                    model.ALERT_STUCK_REC_COUNT = 3;
                    model.ALERT_MIN_TYPE = "H";
                    model.ALERT_MAX_TYPE = "H";
                    model.ALERT_AMT_CHANGE_TYPE = "H";
                    model.ALERT_STUCK_TYPE = "N";

                    T_QREST_REF_PAR_METHODS _parMeth = db_Ref.GetT_QREST_REF_PAR_METHODS_ByID(model.PAR_METHOD_IDX);
                    if (_parMeth != null)
                    {
                        if (_parMeth.CUST_MIN_VALUE != null || _parMeth.MIN_VALUE != null)
                            model.ALERT_MIN_VALUE = UnitConvert.ConvertUnit(_parMeth.CUST_MIN_VALUE ?? _parMeth.MIN_VALUE ?? 0, _parMeth.STD_UNIT_CODE, model.COLLECT_UNIT_CODE);
                        if (_parMeth.CUST_MAX_VALUE != null || _parMeth.MAX_VALUE != null)
                            model.ALERT_MAX_VALUE = UnitConvert.ConvertUnit(_parMeth.CUST_MAX_VALUE ?? _parMeth.MAX_VALUE ?? 0, _parMeth.STD_UNIT_CODE, model.COLLECT_UNIT_CODE);
                        if ((_parMeth.CUST_MIN_VALUE != null || model.ALERT_MIN_VALUE != null) && model.ALERT_MIN_VALUE != 0)
                            model.ALERT_AMT_CHANGE = Math.Abs((UnitConvert.ConvertUnit(_parMeth.CUST_MIN_VALUE ?? _parMeth.MIN_VALUE ?? 0, _parMeth.STD_UNIT_CODE, model.COLLECT_UNIT_CODE) ?? 0) * (double)3);
                    }
                }

                Guid? SuccInd = db_Air.InsertUpdatetT_QREST_MONITORS(model.MONITOR_IDX, model.SITE_IDX, model.PAR_METHOD_IDX, model.POC, model.DURATION_CODE, model.COLLECT_FREQ_CODE, 
                    model.COLLECT_UNIT_CODE, model.ALERT_MIN_VALUE ?? -9999, model.ALERT_MAX_VALUE ?? -9999, model.ALERT_AMT_CHANGE ?? -9999, model.ALERT_STUCK_REC_COUNT ?? -9999, 
                    model.ALERT_MIN_TYPE, model.ALERT_MAX_TYPE, model.ALERT_AMT_CHANGE_TYPE, model.ALERT_STUCK_TYPE, UserIDX, model.PAR_NAME);

                if (SuccInd != null)
                {
                    TempData["Success"] = "Record updated";
                    return RedirectToAction("MonitorEdit", new { id = SuccInd });
                }
                else
                    TempData["Error"] = "Error updating record.";

            }

            //repopulate model
            model.ddl_Unit = ddlHelpers.get_ddl_ref_units(null);
            return View(model);
        }


        public ActionResult MonitorEditMethod(Guid? id)
        {
            string UserIDX = User.Identity.GetUserId();
            var model = new vmSiteMonitorEditMethod();

            SiteMonitorDisplayType _monitor = db_Air.GetT_QREST_MONITORS_ByID(id ?? Guid.Empty);
            if (_monitor != null)
            {
                //reject if user doesn't have access to site
                RedirectToRouteResult r = CanAccessThisSite(UserIDX, _monitor.T_QREST_MONITORS.SITE_IDX, false);
                if (r != null) return r;

                model.MONITOR_IDX = _monitor.T_QREST_MONITORS.MONITOR_IDX;
                model.SITE_IDX = _monitor.T_QREST_MONITORS.SITE_IDX;
                model.PAR_METHOD_IDX = _monitor.T_QREST_MONITORS.PAR_METHOD_IDX;
                model.POC = _monitor.T_QREST_MONITORS.POC;
                model.PAR_NAME = _monitor.PAR_NAME;
                model.PAR_CODE = _monitor.PAR_CODE;
                model.METHOD_CODE = _monitor.METHOD_CODE;

                model.PerMethods = db_Ref.GetT_QREST_REF_PAR_METHODS_Search(model.PAR_CODE, null, 1000, null);

                //populate site name
                if (model.SITE_IDX != null)
                {
                    T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(model.SITE_IDX.GetValueOrDefault());
                    model.SITE_NAME = _site.SITE_NAME;
                }

            }
            else
            {
                //fail if monitor doesn't exist
                TempData["Error"] = "Monitor not found.";
                return RedirectToAction("SiteList", "Site");
            }

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id is selected par method idx</param>
        /// <param name="id2">id2 is monitor idx</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MonitorEditMethod(Guid? id, Guid? id2)
        {
            if (id == null || id2 == null)
                return Json("No record selected");
            else
            {
                string UserIDX = User.Identity.GetUserId();

                Guid? succId = db_Air.InsertUpdatetT_QREST_MONITORS(id2.GetValueOrDefault(), null, id.GetValueOrDefault(), null, null, null, null, null, null, null, null, null, null, null, null, UserIDX, null);
                if (succId != null)
                    return Json("Success");
                else
                    return Json("Unspecified error attempting to delete monitor.");
            }
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
                    try
                    {
                        //https://aqs.epa.gov/data/api/monitors/bySite?email=info@open-environment.org&key=dunhawk56&bdate=20000101&edate=20251231&state=04&county=013&site=5100
                        Uri myUri = new Uri("https://aqs.epa.gov/data/api/monitors/bySite?email=info@open-environment.org&key=dunhawk56&bdate=20000101&edate=20251231&state=" + _site.STATE_CD + "&county=" + _site.COUNTY_CD + "&site=" + _site.AQS_SITE_ID, UriKind.Absolute);
                        var json = httpClient.GetStringAsync(myUri).Result;
                        dynamic stuff = JsonConvert.DeserializeObject(json);

                        foreach (var item in stuff.Data)
                        {
                            string _parcode = item.parameter_code;
                            string _method_code = item.last_method_code;
                            if (_method_code != null)
                            {
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
                    }
                    catch (AggregateException err)
                    {
                        foreach (var errInner in err.InnerExceptions)
                            db_Ref.CreateT_QREST_SYS_LOG(UserIDX, "ERROR", "Failed to import monitor - code 5 " + errInner);
                        TempData["Error"] = "Unable to connect to AQS, please try again later.";
                    }
                    catch (Exception ex)
                    {
                        db_Ref.CreateT_QREST_SYS_LOG(UserIDX, "ERROR", "Failed to import monitor - code 4 " + ex.Message);
                        TempData["Error"] = "Unable to connect to AQS, please try again later.";
                    }
                }

                return View(model);
            }

        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MonitorImport(vmSiteMonitorImport model)
        {
            string UserIDX = User.Identity.GetUserId();

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
                if (item.monSelInd)
                {
                    i++;
                    db_Air.InsertUpdatetT_QREST_MONITORS(null, model.siteIDX, item.T_QREST_MONITORS.PAR_METHOD_IDX, item.T_QREST_MONITORS.POC,
                        "1", "1", null, null, null, null, null, null, null, null, null, UserIDX, null);
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

                //get counts
                int hrCnt = db_Air.GetT_QREST_DATA_HOURLYcountByMon(idg);
                int qcCnt = db_Air.GetT_QREST_QC_ASSESSMENT_CountByMon(idg);

                if (hrCnt == 0 && qcCnt == 0)
                {
                    int succId = db_Air.DeleteT_QREST_MONITORS(idg);
                    if (succId == 1)
                        return Json("Success");
                    else
                        return Json("Unspecified error attempting to delete monitor.");
                }
                else
                    return Json("Cannot delete monitor that has air quality data collected. Delete results first. (" + hrCnt + " hourly records and " + qcCnt + " QC assessments)");
            }
        }


        [HttpPost]
        public ActionResult RefParMethodData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            string search = Request.Form.GetValues("search[value]")?.FirstOrDefault(); //search value
            if (search.Length > 2)
            {
                int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
                int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #

                List<RefParMethodDisplay> data = db_Ref.GetT_QREST_REF_PAR_METHODS_Search(search, null, pageSize, start);
                var recordsTotal = db_Ref.GetT_QREST_REF_PAR_METHODS_Count(search, null);
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            else
            {
                List<RefParMethodDisplay> data = new List<RefParMethodDisplay>();
                return Json(new { draw = draw, recordsFiltered = 0, recordsTotal = 0, data = data });
            }
        }


        //**********************SHARED **************************************
        //**********************SHARED **************************************
        //**********************SHARED **************************************

        [HttpGet]
        public JsonResult FetchUnits(string ID)
        {
            var data = ddlHelpers.get_ddl_ref_units(ID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FetchRefParMethod(string ID)
        {
            var data = db_Ref.GetT_QREST_REF_PAR_METHODS_ByID(ID.ConvertOrDefault<Guid?>());
            return Json(data, JsonRequestBehavior.AllowGet);
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