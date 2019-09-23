using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QREST.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using QREST.App_Logic.BusinessLogicLayer;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using QRESTModel.DAL;

namespace QREST.Controllers
{
    [Authorize(Roles ="ADMIN")]
    public class AdminController : Controller
    {
        #region CONSTRUCTOR
        private ApplicationUserManager _userManager;

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion


        public ActionResult Index()
        {
            return View();
        }


        
        //************************************* APP SETTINGS ************************************************************
        
            // GET: /Admin/AppSettings
        public ActionResult AppSettings()
        {
            T_QREST_APP_SETTINGS_CUSTOM custSettings = db_Ref.GetT_QREST_APP_SETTING_CUSTOM();

            if (custSettings != null)
            {
                var model = new vmAdminAppSettings
                {
                    T_VCCB_APP_SETTINGS = db_Ref.GetT_QREST_APP_SETTING_list(),
                    TermsAndConditions = custSettings.TERMS_AND_CONDITIONS,
                    Announcements = custSettings.ANNOUNCEMENTS
                };
                return View(model);
            }
            else
            {
                TempData["Error"] = "Unable to load custom settings";
                return View();
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppSettings(vmAdminAppSettings model)
        {
            if (ModelState.IsValid)
            {
                string UserID = User.Identity.GetUserId();

                int SuccID = db_Ref.InsertOrUpdateT_QREST_APP_SETTINGS(model.edit_app_setting.SETTING_IDX, model.edit_app_setting.SETTING_NAME, model.edit_app_setting.SETTING_VALUE, false, null, UserID);
                if (SuccID > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("AppSettings");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CustomSettings(vmAdminAppSettings model)
        {
            if (ModelState.IsValid)
            {
                int SuccID = db_Ref.InsertUpdateT_QREST_APP_SETTING_CUSTOM(model.TermsAndConditions ?? "", null);
                if (SuccID > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("AppSettings");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CustomSettingsAnnounce(vmAdminAppSettings model)
        {
            if (ModelState.IsValid)
            {
                int SuccID = db_Ref.InsertUpdateT_QREST_APP_SETTING_CUSTOM(null, model.Announcements ?? "");
                if (SuccID > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("AppSettings");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CustomSettingsTerms(vmAdminAppSettings model)
        {
            if (ModelState.IsValid)
            {
                int SuccID = db_Ref.InsertUpdateT_QREST_APP_SETTING_CUSTOM(model.TermsAndConditions ?? "", null);
                if (SuccID > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("AppSettings");
        }



        //************************************* EMAIL CONFIG************************************************************
        // GET: /Admin/EmailConfig
        public ActionResult EmailConfig(int? id)
        {
            var model = new vmAdminEmailConfig {
                T_QREST_EMAIL_TEMPLATE = db_Ref.GetT_QREST_EMAIL_TEMPLATE()
            };

            var xxx = db_Ref.GetT_QREST_EMAIL_TEMPLATE_ByID(id ?? -1);
            if (xxx == null)
                xxx = db_Ref.GetT_QREST_EMAIL_TEMPLATE_ByID(model.T_QREST_EMAIL_TEMPLATE[0].EMAIL_TEMPLATE_ID);

            if (xxx != null)
            {
                model.editID = xxx.EMAIL_TEMPLATE_ID;
                model.editDESC = xxx.EMAIL_TEMPLATE_DESC;
                model.editSUBJ = xxx.SUBJ;
                model.editMSG = xxx.MSG;
            }

            return View(model);
        }
         
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EmailConfig(vmAdminEmailConfig model)
        {
            if (ModelState.IsValid)
            {
                string UserIDX = User.Identity.GetUserId();

                int SuccID = db_Ref.InsertUpdateT_QREST_EMAIL_TEMPLATE(model.editID, model.editSUBJ, model.editMSG, UserIDX);
                if (SuccID > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("EmailConfig", new { id = model.editID });
        }





        //************************************* HELP CONFIG************************************************************
        // GET: /Admin/HelpConfig
        public ActionResult HelpConfig(int? id)
        {
            var model = new vmAdminHelpConfig
            {
                HelpTopics = db_Ref.GetT_QREST_HELP_DOCS()
            };

            model.EditHelp = db_Ref.GetT_QREST_HELP_DOCS_ByID(id ?? -1);
            if (model.EditHelp == null)
                model.EditHelp = db_Ref.GetT_QREST_HELP_DOCS_ByID(model.HelpTopics[0].HELP_IDX);

            model.editHelpHtml = model.EditHelp.HELP_HTML;

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult HelpConfig(vmAdminHelpConfig model)
        {
            if (ModelState.IsValid)
            {
                string UserIDX = User.Identity.GetUserId();

                model.EditHelp.HELP_IDX = db_Ref.InsertUpdateT_QREST_HELP_DOCS(model.EditHelp.HELP_IDX, model.EditHelp.HELP_TITLE, model.editHelpHtml, model.EditHelp.SORT_SEQ);
                if (model.EditHelp.HELP_IDX > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("HelpConfig", new { id = model.EditHelp.HELP_IDX });
        }


        //************************************* ORGANIZATIONS ************************************************************

        // GET: /Admin/OrgList
        public ActionResult OrgList()
        {
            var model = new vmAdminOrgList();
            return View(model);
        }

        [HttpPost]
        public ActionResult OrgListData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
            var data = db_Ref.GetT_QREST_ORGANIZATIONS(true, false);
            var recordsTotal = data.Count();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        public ActionResult OrgEdit(string id)
        {
            var model = new vmAdminOrgEdit
            {
                ddl_User = ddlHelpers.get_ddl_users(false)
            };

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


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrgEdit(vmAdminOrgEdit model)
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

        
        [HttpPost]
        public JsonResult OrgDelete(string id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int SuccID = db_Ref.DeleteT_QREST_ORGANIZATIONS(id);
                if (SuccID == 1)
                    return Json("Success");
                else if (SuccID == -1)
                    return Json("Cannot delete Organization that still has site records. Delete sites first.");
                else
                    return Json("Unable to find organization to delete.");
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
                return RedirectToAction("OrgEdit", "Admin", new { id = model.edit_org_id });
            else
                return RedirectToAction("UserEdit", "Admin", new { id = model.edit_user_idx });
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
                else if (SuccID == -1)
                    return Json("Cannot delete Organization that still has site records. Delete sites first.");
                else
                    return Json("Unable to find user to delete.");
            }
        }


        //************************************* USERS ************************************************************

        // GET: /Admin/UserList
        public ActionResult UserList()
        {
            var model = new vmAdminUserList();
            return View(model);
        }


        [HttpPost]
        public ActionResult UserListData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
            var data = db_Account.GetT_QREST_USERS_byOrgID(null);
            var recordsTotal = data.Count();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        [HttpPost]
        public async Task<JsonResult> UserDelete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult x = await UserManager.DeleteAsync(user);
                if (x.Succeeded)
                    return Json("Success");
                else
                    return Json("Unable to delete user.");
            }
            else
                return Json("Unable to find user to delete.");
        }


        public ActionResult UserEdit(string id)
        {
            var model = new vmAdminUserEdit();
            model.user = UserManager.FindById(id);
            if (model.user != null)
            {
                model.edit_user_idx = model.user.Id;
                model.edit_typ = "user";

                model.Roles_In_User = UserManager.GetRoles(model.user.Id).Select(x => new SelectListItem
                {
                    Value = x,
                    Text = x
                });
                model.Roles_Not_In_User = db_Account.GetT_QREST_ROLESNotInUserIDX(model.user.Id).Select(x => new SelectListItem
                {
                    Value = x.Name,
                    Text = x.Name
                });
                model.user_orgs = db_Account.GetT_QREST_ORG_USERS_byUSER_IDX(model.user.Id, null);
                return View(model);
            }
            else
                return RedirectToAction("UserList", "Admin");
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UserEdit(vmAdminUserEdit model)
        {
            if (ModelState.IsValid)
            {
                var _user = UserManager.FindById(model.user.Id);
                if (_user != null)
                {
                    _user.FNAME = model.user.FNAME;
                    _user.LNAME = model.user.LNAME;
                    _user.Email = model.user.Email;

                    IdentityResult succ = UserManager.Update(_user);
                    if (succ.Succeeded)
                        TempData["Success"] = "Record updated";
                    else
                        TempData["Error"] = "Error updating record.";
                }
                else
                    return RedirectToAction("UserList", "Admin");

            }


            return RedirectToAction("UserEdit", "Admin", new { id = model.user.Id });
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UserRoleEdit(vmAdminUserEdit model, string submitButton)
        {
            IdentityResult succID = new IdentityResult();

            // ADDING USER TO ROLE
            if (submitButton == "Add")
            {
                if (model.Roles_Not_In_User_Selected == null)
                    TempData["Error"] = "You must select a role to add.";
                else
                    foreach (string u in model.Roles_Not_In_User_Selected)
                        succID = UserManager.AddToRole(model.user.Id, u); 
            }
            // REMOVE USER FROM ROLE
            else if (submitButton == "Remove")
            {
                if (model.Roles_In_User_Selected == null)
                    TempData["Error"] = "You must select a role to remove.";
                else
                    foreach (string u in model.Roles_In_User_Selected)
                        succID = UserManager.RemoveFromRole(model.user.Id, u);
            }
            else
                return View(model);

            if (succID.Succeeded)
                TempData["Success"] = "Update successful.";

            return RedirectToAction("UserEdit", "Admin", new { id = model.user.Id });
        }





        public ActionResult DocConfig()
        {
            var model = new vmAdminDocConfig();
            return View(model);
        }


        public ActionResult TaskConfig()
        {
            var model = new vmAdminTaskConfig();
            return View(model);
        }


        //************************************* LOGGING ************************************************************

        // GET: /Admin/LogError
        public ActionResult LogError()
        {
            var model = new vmAdminLogError();
            return View(model);
        }

        [HttpPost]
        public ActionResult LogErrorData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //date filters
            DateTime? minDate = Request.Form.GetValues("mini")?.FirstOrDefault().ConvertOrDefault<DateTime?>();
            DateTime? maxDate = Request.Form.GetValues("maxi")?.FirstOrDefault().ConvertOrDefault<DateTime?>();

            var data = db_Ref.GetT_QREST_SYS_LOG(minDate, maxDate, pageSize, start, orderCol, orderDir);
            var recordsTotal = db_Ref.GetT_QREST_SYS_LOG_count(minDate, maxDate);

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        public ActionResult LogEmail()
        {
            var model = new vmAdminLogEmail();
            return View(model);
        }

        [HttpPost]
        public ActionResult LogEmailData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //date filters
            DateTime? minDate = Request.Form.GetValues("mini")?.FirstOrDefault().ConvertOrDefault<DateTime?>();
            DateTime? maxDate = Request.Form.GetValues("maxi")?.FirstOrDefault().ConvertOrDefault<DateTime?>();

            var data = db_Ref.GetT_QREST_SYS_LOG_EMAIL(minDate, maxDate, pageSize, start, orderCol, orderDir);
            var recordsTotal = db_Ref.GetT_QREST_SYS_LOG_EMAILcount(minDate, maxDate);

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        public ActionResult LogActivity()
        {
            var model = new vmAdminLogActivity();
            return View(model);
        }

        [HttpPost]
        public ActionResult LogActivityData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //date filters
            DateTime? minDate = Request.Form.GetValues("mini")?.FirstOrDefault().ConvertOrDefault<DateTime?>();
            DateTime? maxDate = Request.Form.GetValues("maxi")?.FirstOrDefault().ConvertOrDefault<DateTime?>();

            var data = db_Ref.GetT_QREST_SYS_LOG_ACTIVITY(minDate, maxDate, pageSize, start, orderCol, orderDir);
            var recordsTotal = db_Ref.GetT_QREST_SYS_LOG_ACTIVITYcount(minDate, maxDate);

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }



        //************************************* IMPORT DATA ************************************************************

        // GET: /Admin/ImportData
        public ActionResult ImportData()
        {
            var model = new vmAdminImport();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportData(HttpPostedFileBase postedFile)
        {
            //initialize variables
            bool headInd = true;
            string importType = "";
            int insCount = 0;
            int existCount = 0;
            int errorCount = 0;
            string UserIDX = User.Identity.GetUserId();

            if (postedFile != null && postedFile.InputStream != null)
            {
                //loop through each row
                using (StreamReader csvreader = new StreamReader(postedFile.InputStream))
                {
                    string currentLine;
                    while ((currentLine = csvreader.ReadLine()) != null)
                    {
                        //split row's columns into string array
                        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        string[] cols = CSVParser.Split(currentLine);
                        cols = cols.Select(x => x.Replace("\"", "")).ToArray();   //remove double quotes

                        if (cols.Length > 0) //skip blank rows
                        {
                            if (headInd)
                            {
                                //**********************************************************
                                //HEADER ROW - LOGIC TO DETERMINE WHAT IS IN EACH COLUMN
                                //**********************************************************
                                if (cols[0] == "Agency Code")
                                    importType = "AGENCY";
                                else if (cols[0] == "Duration Code")
                                    importType = "DURATION";
                                else if (cols[0] == "Frequency Code")
                                    importType = "COLLECTION FREQUENCY";
                                else if (cols[0] == "Unit Code")
                                    importType = "UNITS";
                                else if (cols[0] == "Parameter Code")
                                    importType = "PARAMETERS";
                                else if (cols[0] == "Parameter" && cols[2] == "Method Code")
                                    importType = "METHODS";
                                else if (cols[0] == "State Code")
                                    importType = "STATES COUNTIES";

                                headInd = false;
                            }
                            else
                            {
                                //**********************************************************
                                //NOT HEADER ROW - READING IN VALUES
                                //**********************************************************
                                if (importType == "AGENCY")
                                {
                                    T_QREST_REF_AQS_AGENCY _data = db_Ref.GetT_QREST_REF_AQS_AGENCY_ByID(cols[0]);
                                    if (_data == null)
                                    {
                                        bool SuccID = db_Ref.InsertUpdatetT_QREST_REF_AQS_AGENCY(cols[0], cols[1], cols[2], true, UserIDX);
                                        if (SuccID)
                                            insCount++;
                                        else
                                            errorCount++;
                                    }
                                    else
                                        existCount++;
                                }
                                else if (importType == "DURATION")
                                {
                                    T_QREST_REF_DURATION _data = db_Ref.GetT_QREST_REF_DURATION_ByID(cols[0]);
                                    if (_data == null)
                                    {
                                        bool SuccID = db_Ref.InsertUpdatetT_QREST_REF_DURATION(cols[0], cols[1], true, UserIDX);
                                        if (SuccID)
                                            insCount++;
                                        else
                                            errorCount++;
                                    }
                                    else
                                        existCount++;
                                }
                                else if (importType == "COLLECTION FREQUENCY")
                                {
                                    T_QREST_REF_COLLECT_FREQ _data = db_Ref.GetT_QREST_REF_COLLECT_FREQ_ByID(cols[0]);
                                    if (_data == null)
                                    {
                                        bool SuccID = db_Ref.InsertUpdatetT_QREST_REF_COLLECT_FREQ(cols[0], cols[1], true, UserIDX);
                                        if (SuccID)
                                            insCount++;
                                        else
                                            errorCount++;
                                    }
                                    else
                                        existCount++;
                                }
                                else if (importType == "UNITS")
                                {
                                    T_QREST_REF_UNITS _data = db_Ref.GetT_QREST_REF_UNITS_ByID(cols[0]);
                                    if (_data == null)
                                    {
                                        bool SuccID = db_Ref.InsertUpdatetT_QREST_REF_UNITS(cols[0], cols[1], true, UserIDX);
                                        if (SuccID)
                                            insCount++;
                                        else
                                            errorCount++;
                                    }
                                    else
                                        existCount++;
                                }
                                else if (importType == "PARAMETERS")
                                {
                                    T_QREST_REF_PARAMETERS _data = db_Ref.GetT_QREST_REF_PARAMETERS_ByID(cols[0]);
                                    if (_data == null)
                                    {
                                        //lookup unit code
                                        T_QREST_REF_UNITS _unit = db_Ref.GetT_QREST_REF_UNITS_ByDesc(cols[5]);
                                        if (_unit != null)
                                        {
                                            bool SuccID = db_Ref.InsertUpdatetT_QREST_REF_PARAMETERS(cols[0], cols[1], cols[3], cols[4], _unit.UNIT_CODE, cols[6]=="YES", UserIDX);
                                            if (SuccID)
                                                insCount++;
                                            else
                                                errorCount++;
                                        }
                                        else
                                            errorCount++;
                                    }
                                    else
                                        existCount++;
                                }
                                else if (importType == "METHODS")
                                {
                                    T_QREST_REF_PAR_METHODS _data = db_Ref.GetT_QREST_REF_PAR_METHODS_ByParCdMethodCd(cols[1],cols[2]);
                                    if (_data == null)
                                    {
                                        //lookup unit code
                                        T_QREST_REF_UNITS _unit = db_Ref.GetT_QREST_REF_UNITS_ByDesc(cols[14]);
                                        if (_unit != null)
                                        {
                                            bool SuccID = db_Ref.InsertT_QREST_REF_PAR_METHODS(null, cols[1], cols[2], cols[3], cols[4], cols[5], cols[7], cols[8], _unit.UNIT_CODE, cols[9].ConvertOrDefault<double?>(), cols[10].ConvertOrDefault<double?>(), cols[11].ConvertOrDefault<double?>(), UserIDX);
                                            if (SuccID)
                                                insCount++;
                                            else
                                                errorCount++;
                                        }
                                        else
                                            errorCount++;
                                    }
                                    else
                                        existCount++;
                                }
                                else if (importType == "STATES COUNTIES")
                                {
                                    //first check if state exists
                                    db_Ref.InsertUpdatetT_QREST_REF_STATE(cols[0], cols[1], cols[2]);

                                    //next update county
                                    T_QREST_REF_COUNTY _data = db_Ref.GetT_QREST_REF_COUNTY_ByID(cols[0],cols[3]);
                                    if (_data == null)
                                    {
                                        bool SuccID = db_Ref.InsertUpdatetT_QREST_REF_COUNTY(cols[0], cols[3], cols[4]);
                                        if (SuccID)
                                            insCount++;
                                        else
                                            errorCount++;
                                    }
                                    else
                                        existCount++;
                                }
                            }
                        }
                    }
                }
            }
            else
                TempData["Error"] = "You must select a file to import.";



            var model = new vmAdminImport
            {
                ImportType = importType,
                ExistCount = existCount,
                ErrorCount = errorCount,
                InsertCount = insCount
            };
            return View(model);
        }


        //************************************* POLLING ************************************************************

        // GET: /Admin/ImportData
        public ActionResult PollData()
        {

            //attempt #1 ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^6
            //using (Client client = new Client("68.185.0.142", 14109, new System.Threading.CancellationToken()))
            //{
            //    var xxx = client.IsConnected;
            //    client.WriteLine("u" + Environment.NewLine);

            //    string response = client.ReadAsync().Result;
            //    //string response = client.TerminatedReadAsync(":", TimeSpan.FromMilliseconds(10000)).Result;
            //}




            ////attempt #2 ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^6

            ////Create a TCPClient object at the IP and port
            //TcpClient client = new TcpClient("1.1.1.1", 14109);

            //// Get a client stream for reading and writing.
            //NetworkStream stream = client.GetStream();


            //// Send "U" to the connected TcpServer. 
            //Byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes("u\r\n");
            //stream.Write(bytesToSend, 0, bytesToSend.Length);


            ////Read back the text---
            //byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            //int bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            //string responseData = System.Text.Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);


            //// Send password to the connected TcpServer. 
            //bytesToSend = System.Text.Encoding.ASCII.GetBytes("password");
            //stream.Write(bytesToSend, 0, bytesToSend.Length);

            ////Read back the text---
            //bytesToRead = new byte[client.ReceiveBufferSize];
            //bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            //responseData = System.Text.Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);


            //// Send "D" to the connected TcpServer. 
            //bytesToSend = System.Text.Encoding.ASCII.GetBytes("D"  );
            //stream.Write(bytesToSend, 0, bytesToSend.Length);

            ////Read back the text---
            //bytesToRead = new byte[client.ReceiveBufferSize];
            //bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            //responseData = System.Text.Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);


            //// Close everything.
            //stream.Close();
            //client.Close();

            //try { 

            //    var fileToOpen = @"C:\Users\dougt\Downloads\hyperterminal1\hyperterminal\ttt.ht";
            //    var myProcess = new Process();
            //    myProcess.StartInfo = new ProcessStartInfo()
            //    {
            //        UseShellExecute = true,
            //        FileName = fileToOpen
            //    };

            //    myProcess.Start();

            //    if (myProcess.Responding)
            //    {
            //        string username = "u";
            //        System.Windows.Forms.SendKeys.SendWait(username + "{ENTER}");
            //        Application.DoEvents();
            //        Thread.Sleep(5000);
            //        string password = "";
            //        System.Windows.Forms.SendKeys.SendWait(password + "{ENTER}");
            //        Application.DoEvents();
            //        Thread.Sleep(5000);
            //        string Command = "";
            //        System.Windows.Forms.SendKeys.SendWait(Command + "{ENTER}");
            //        Application.DoEvents();
            //        Thread.Sleep(5000);

            //        using (var writer = new StreamWriter(@"C:\Users\dougt\Downloads\hyperterminal1\hyperterminal\data.txt"))
            //        {
            //            writer.Write(myProcess.StandardOutput.ReadToEnd());
            //        }

            //        myProcess.Close();

            //    }

            //}           
            //catch
            //{
            //}


            return View();
        }



    }
}