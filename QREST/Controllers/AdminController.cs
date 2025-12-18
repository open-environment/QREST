using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QREST.App_Logic.BusinessLogicLayer;
using QREST.Models;
using QRESTModel.AQSHelper;
using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QREST.Controllers
{
    [Authorize(Roles = "GLOBAL ADMIN")]
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
            return RedirectToAction("OrgList");
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
                string userId = User.Identity.GetUserId();

                int succId = db_Ref.InsertOrUpdateT_QREST_APP_SETTINGS(model.edit_app_setting.SETTING_IDX, model.edit_app_setting.SETTING_NAME, model.edit_app_setting.SETTING_VALUE, false, null, userId);
                if (succId > 0)
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
                int succId = db_Ref.InsertUpdateT_QREST_APP_SETTING_CUSTOM(model.TermsAndConditions ?? "", null);
                if (succId > 0)
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
                int succId = db_Ref.InsertUpdateT_QREST_APP_SETTING_CUSTOM(null, model.Announcements ?? "");
                if (succId > 0)
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
                int succId = db_Ref.InsertUpdateT_QREST_APP_SETTING_CUSTOM(model.TermsAndConditions ?? "", null);
                if (succId > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("AppSettings");
        }

        public ActionResult CDXTest()
        {
            var model = new vmAdminCDXTest();
            model.CDXURL = db_Ref.GetT_QREST_APP_SETTING("CDX_URL");
            model.TestProdInd = model.CDXURL.Contains("test") ? "Test CDX" : "Production CDX";
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CDXTest(vmAdminCDXTest model)
        {
            if (ModelState.IsValid)
            {
                string succId = AQSHelper.AuthHelper(model.CDXUsername, model.CDXPassword, db_Ref.GetT_QREST_APP_SETTING("CDX_URL"));
                if (succId != null)
                    TempData["Success"] = "CDX Username/password combo correct";
                else
                    TempData["Error"] = "CDX Username/password combo failed";
            }

            return RedirectToAction("CDXTest");
        }



        //************************************* EMAIL CONFIG************************************************************
        // GET: /Admin/EmailConfig
        public ActionResult EmailConfig(int? id)
        {
            var model = new vmAdminEmailConfig
            {
                T_QREST_EMAIL_TEMPLATE = db_Ref.GetT_QREST_EMAIL_TEMPLATE()
            };

            var xxx = db_Ref.GetT_QREST_EMAIL_TEMPLATE_ByID(id ?? -1) 
                      ?? db_Ref.GetT_QREST_EMAIL_TEMPLATE_ByID(model.T_QREST_EMAIL_TEMPLATE[0].EMAIL_TEMPLATE_ID);

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

                int succId = db_Ref.InsertUpdateT_QREST_EMAIL_TEMPLATE(model.editID, model.editSUBJ, model.editMSG, UserIDX);
                if (succId > 0)
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

            model.EditHelp = db_Ref.GetT_QREST_HELP_DOCS_ByID(id ?? model.HelpTopics[0].HELP_IDX);
            model.editHelpHtml = model.EditHelp.HELP_HTML;

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult HelpConfig(vmAdminHelpConfig model)
        {
            if (ModelState.IsValid)
            {
                model.EditHelp.HELP_IDX = db_Ref.InsertUpdateT_QREST_HELP_DOCS(model.EditHelp.HELP_IDX, model.EditHelp.HELP_TITLE, model.editHelpHtml ?? "", model.EditHelp.SORT_SEQ, model.EditHelp.HELP_CAT);
                if (model.EditHelp.HELP_IDX > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("HelpConfig", new { id = model.EditHelp.HELP_IDX });
        }

        [HttpPost]
        public ContentResult ImagePosted()
        {
            var jsonString = "";
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase imageFile = Request.Files[0];
                if (imageFile != null)
                {
                    BinaryReader br = new BinaryReader(imageFile.InputStream);
                    byte[] fileBytes = br.ReadBytes(imageFile.ContentLength);
                    string base64String = Convert.ToBase64String(fileBytes);
                    string srcString = $"data:image/png;base64,{base64String}";
                    jsonString = $"{{\"location\":\"{srcString}\"}}";
                }
            }
            return Content(jsonString);
        }

        [HttpPost]
        public JsonResult HelpConfigDelete(int id)
        {
            if (id <= 0)
                return Json("No record selected to delete");
            else
            {
                int succId = db_Ref.DeleteT_HELP_DOCS(id);
                if (succId == 1)
                    return Json("Success");
                else
                    return Json("Unable to delete section.");
            }
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
                model.LOCK_ACCESS_IND = _org.LOCK_ACCESS_IND;
                model.org_users = db_Account.GetT_QREST_ORG_USERS_ByOrgID(model.ORG_ID, null, null);
                model.org_emails = db_Account.GetT_QREST_ORG_EMAIL_RULE(model.ORG_ID);
            }

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrgEdit(vmAdminOrgEdit model)
        {
            if (ModelState.IsValid)
            {
                int succId = db_Ref.InsertUpdatetT_QREST_ORGANIZATION(model.ORG_ID, model.ORG_NAME, model.STATE_CD, model.EPA_REGION,
                    null, null, model.AQS_AGENCY_CODE, model.SELF_REG_IND, true, "", null, null, null);

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


        [HttpPost]
        public JsonResult OrgDelete(string id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int succId = db_Ref.DeleteT_QREST_ORGANIZATIONS(id);
                if (succId == 1)
                    return Json("Success");
                else if (succId == -1)
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


            if (model.edit_typ == "org")
                return RedirectToAction("OrgEdit", "Admin", new { id = model.edit_org_id });
            else
                return RedirectToAction("UserEdit", "Admin", new { id = model.edit_user_idx });
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrgEditEmail(vmAdminOrgEditEmail model)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                bool succInd = db_Account.InsertUpdateT_QREST_ORG_EMAIL_RULE(model.edit_org_id, model.edit_email_rule, userId);

                if (succInd)
                    TempData["Success"] = "Record updated";
                else
                    TempData["Error"] = "Error updating record.";
            }
            else
                TempData["Error"] = "Error updating record.";
            
            return RedirectToAction("OrgEdit", "Admin", new { id = model.edit_org_id });
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id is org id</param>
        /// <param name="id2">id2 is email rule</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OrgEmailDelete(string id, string id2)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int SuccID = db_Account.DeleteT_QREST_ORG_EMAIL_RULE(id, id2);
                if (SuccID == 1)
                    return Json("Success");
                else
                    return Json("Unable to find record to delete.");
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
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //Filter based on user type
            int? usertype = Request.Form.GetValues("usertype")?.FirstOrDefault().ConvertOrDefault<int>();

            var data = db_Account.GetT_QREST_USERS(usertype, pageSize, start, orderColName, orderDir);
            var recordsTotal = db_Account.GetT_QREST_USERScount(usertype);
            
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        [HttpPost]
        public JsonResult UserListEmail(vmAdminUserList model)
        {
            if (model.usertype == null)
                return Json("No user type to email");
            else
            {
                string UserIDX = User.Identity.GetUserId();
                var mailUsers = db_Account.GetT_QREST_USERS_ByRoleType(model.usertype);
                bool mailSendStatus = db_Account.SendMailToUsers(mailUsers, model.EmailSubject, System.Uri.UnescapeDataString(model.emailBodyHtml), UserIDX);
                if (mailSendStatus)
                    return Json("True");
                else
                    return Json("Sorry, there was some error sending Email.");
            }
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
            IdentityResult succId = new IdentityResult();

            // ADDING USER TO ROLE
            if (submitButton == "Add")
            {
                if (model.Roles_Not_In_User_Selected == null)
                    TempData["Error"] = "You must select a role to add.";
                else
                    foreach (string u in model.Roles_Not_In_User_Selected)
                        succId = UserManager.AddToRole(model.user.Id, u);
            }
            // REMOVE USER FROM ROLE
            else if (submitButton == "Remove")
            {
                if (model.Roles_In_User_Selected == null)
                    TempData["Error"] = "You must select a role to remove.";
                else
                    foreach (string u in model.Roles_In_User_Selected)
                        succId = UserManager.RemoveFromRole(model.user.Id, u);
            }
            else
                TempData["Error"] = "Invalid action.";

            if (succId.Succeeded)
                TempData["Success"] = "Update successful.";

            return RedirectToAction("UserEdit", "Admin", new { id = model.user.Id });
        }




        //************************************* TASK MANAGEMENT ************************************************************
        // GET: /Admin/TaskConfig
        public ActionResult TaskConfig(int? id)
        {
            var model = new vmAdminTaskConfig();
            model.Tasks = db_Ref.GetT_VCCB_TASKS_All();
            model.EditTask = db_Ref.GetT_VCCB_TASKS_ByTaskID(id ?? -1);
            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TaskConfig(vmAdminTaskConfig model)
        {
            if (ModelState.IsValid && model.EditTask != null)
            {
                string UserIDX = User.Identity.GetUserId();

                bool succInd = db_Ref.UpdateT_QREST_TASKS(model.EditTask.TASK_IDX, model.EditTask.FREQ_TYPE, model.EditTask.FREQ_NUM,
                    null, model.EditTask.NEXT_RUN_DT, model.EditTask.STATUS, UserIDX);

                if (succInd)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("TaskConfig", new { id = model.EditTask?.TASK_IDX });
        }


        public ActionResult TaskStop(int? id)
        {
            bool succInd = db_Ref.UpdateT_QREST_TASKS_SetStopped(id ?? -1);

            if (succInd)
                TempData["Success"] = "Task Stopped.";
            else
                TempData["Error"] = "Task Not Stopped.";

            return RedirectToAction("TaskConfig", new { id });
        }

        public ActionResult TaskStart(int? id)
        {
            bool succInd = db_Ref.UpdateT_QREST_TASKS_SetCompleted(id ?? -1);

            if (succInd)
                TempData["Success"] = "Task Scheduled to Run.";
            else
                TempData["Error"] = "Task Not Scheduled to Run.";

            return RedirectToAction("TaskConfig", new { id });
        }




        //************************************* LOGGING ************************************************************

        // GET: /Admin/LogError
        public ActionResult LogError()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogErrorData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //date filters
            DateTime? minDate = Request.Form.GetValues("mini")?.FirstOrDefault().ConvertOrDefault<DateTime?>();
            DateTime? maxDate = Request.Form.GetValues("maxi")?.FirstOrDefault().ConvertOrDefault<DateTime?>();

            var data = db_Ref.GetT_QREST_SYS_LOG(minDate, maxDate, pageSize, start, orderColName, orderDir);
            var recordsTotal = db_Ref.GetT_QREST_SYS_LOG_count(minDate, maxDate);

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        public ActionResult LogActivity()
        {
            var model = new vmSharedLogActivity();
            return View(model);
        }

        public ActionResult LogEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogEmailData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //date filters
            DateTime? minDate = Request.Form.GetValues("mini")?.FirstOrDefault().ConvertOrDefault<DateTime?>();
            DateTime? maxDate = Request.Form.GetValues("maxi")?.FirstOrDefault().ConvertOrDefault<DateTime?>();

            var data = db_Ref.GetT_QREST_SYS_LOG_EMAIL(minDate, maxDate, pageSize, start, orderColName, orderDir);
            var recordsTotal = db_Ref.GetT_QREST_SYS_LOG_EMAILcount(minDate, maxDate);

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
            List<string> updateDetails = new List<string>();
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
                                else if (cols[0] == "Parameter Code" && cols[2] == "Parameter Abbreviation")
                                    importType = "PARAMETERS";
                                else if (cols[0] == "Parameter" && cols[2] == "Method Code")
                                    importType = "METHODS";
                                else if (cols[0] == "State Code")
                                    importType = "STATES COUNTIES";
                                else if (cols[0] == "Qualifier Code")
                                    importType = "QUALIFIERS";
                                else if (cols[0] == "Parameter Code" && cols[2] == "Qualifier Code")
                                    importType = "DISALLOWQUAL";
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
                                        bool succId = db_Ref.InsertUpdatetT_QREST_REF_COLLECT_FREQ(cols[0], cols[1], true, UserIDX);
                                        if (succId)
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
                                        bool succId = db_Ref.InsertUpdatetT_QREST_REF_UNITS(cols[0], cols[1], true, UserIDX);
                                        if (succId)
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
                                            bool succId = db_Ref.InsertUpdatetT_QREST_REF_PARAMETERS(cols[0], cols[1], cols[3], cols[4], _unit.UNIT_CODE, true, cols[6] == "YES", UserIDX);
                                            if (succId)
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
                                    //lookup unit code
                                    T_QREST_REF_UNITS _unit = db_Ref.GetT_QREST_REF_UNITS_ByDesc(cols[14]);
                                    if (_unit != null)
                                    {
                                        Tuple<string, string> succId = db_Ref.InsertUpdateT_QREST_REF_PAR_METHODS(null, cols[1], cols[2], cols[3], cols[4], cols[5], cols[7], cols[8], _unit.UNIT_CODE, cols[9].ConvertOrDefault<double?>(), cols[10].ConvertOrDefault<double?>(), cols[11].ConvertOrDefault<double?>(), null, null, UserIDX);
                                        if (succId.Item1 == "I")
                                            insCount++;
                                        else if (succId.Item1 == "U")
                                        {
                                            if (succId.Item2.Length == 0)
                                                existCount++;
                                            else
                                                updateDetails.Add(cols[1] + "/" + cols[2] + " has been modified: " + succId.Item2);

                                        }
                                        else
                                            errorCount++;
                                    }
                                    else
                                        errorCount++;

                                }
                                else if (importType == "STATES COUNTIES")
                                {
                                    //first check if state exists
                                    db_Ref.InsertUpdatetT_QREST_REF_STATE(cols[0], cols[1], cols[2]);

                                    //next update county
                                    T_QREST_REF_COUNTY _data = db_Ref.GetT_QREST_REF_COUNTY_ByID(cols[0], cols[3]);
                                    if (_data == null)
                                    {
                                        bool succId = db_Ref.InsertUpdatetT_QREST_REF_COUNTY(cols[0], cols[3], cols[4]);
                                        if (succId)
                                            insCount++;
                                        else
                                            errorCount++;
                                    }
                                    else
                                        existCount++;
                                }
                                else if (importType == "QUALIFIERS" && cols[4] == "YES")
                                {
                                    T_QREST_REF_QUALIFIER _data = db_Ref.GetT_QREST_REF_QUALIFIER_ByID(cols[0]);
                                    if (_data == null)
                                    {
                                        bool succId = db_Ref.InsertUpdatetT_QREST_REF_QUALIFIER(cols[0], cols[1], cols[3], UserIDX);
                                        if (succId)
                                            insCount++;
                                        else
                                            errorCount++;
                                    }
                                    else
                                        existCount++;
                                }
                                else if (importType == "DISALLOWQUAL")
                                {
                                    T_QREST_REF_QUAL_DISALLOW _data = db_Ref.GetT_QREST_REF_QUAL_DISALLOW_ByIDs(cols[2], cols[0]);
                                    if (_data == null)
                                    {
                                        bool succId = db_Ref.InsertUpdatetT_QREST_REF_QUAL_DISALLOW(cols[2], cols[0], UserIDX);
                                        if (succId)
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
                InsertCount = insCount,
                UpdateDetails = updateDetails
            };
            return View(model);
        }





        //************************************* REPORTS ************************************************************
        // GET: /Admin/Connectivity
        public ActionResult Connectivity()
        {
            var model = new vmAdminConnectivity();
            model.PollingConfig = db_Air.GetT_QREST_SITES_POLLING_CONFIG_CompleteList();
            return View(model);
        }



        //************************************* TRAINING ************************************************************
        // GET: /Admin/TrainingConfig
        public ActionResult TrainingConfig()
        {
            var model = new vmAdminTraining { 
                courses = db_Train.GetT_QREST_TRAIN_COURSE()
            };
            return View(model);
        }


        public ActionResult TrainingCourse(Guid? id)
        {
            var model = new vmAdminTrainingCourseEdit { 
                course = db_Train.GetT_QREST_TRAIN_COURSE_byID(id.GetValueOrDefault()),
                lessons = db_Train.GetT_QREST_TRAIN_LESSONS_byCourse(id.GetValueOrDefault())
            };
            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TrainingCourse(vmAdminTrainingCourseEdit model)
        {
            Guid? c = model.course.COURSE_IDX;
            if (c == Guid.Empty)
                c = null;

            Guid? succID = db_Train.InsertUpdateT_QREST_TRAIN_COURSE(c, model.course.COURSE_NAME, model.course.COURSE_DESC, model.course.COURSE_SEQ, model.course.ACT_IND);
            if (succID != null) {
                TempData["Success"] = "Data saved";
                return RedirectToAction("TrainingCourse", new { id = succID });
            }
            else
            {
                TempData["Error"] = "Error saving data";
                return View(model);
            }
        }


        [HttpPost]
        public JsonResult TrainingCourseDelete(Guid? id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int succId = db_Train.DeleteT_QREST_TRAIN_COURSE(id ?? Guid.Empty);
                if (succId == 1)
                    return Json("Success");
                else
                    return Json("Unable to delete course.");
            }
        }


        public ActionResult TrainingLesson(Guid? id, Guid? courseid)
        {
            var model = new vmAdminTrainingLessonEdit();
            if (id != null)
            {
                model.lesson = db_Train.GetT_QREST_TRAIN_LESSON_byLesson(id.GetValueOrDefault());
                model.steps = db_Train.GetT_QREST_TRAIN_LESSON_STEPS_byLessonID(model.lesson.LESSON_IDX);
            }
            else if (courseid != null)
            {
                model.lesson = new CourseLessonDisplay
                {
                    COURSE_IDX = courseid.GetValueOrDefault()
                };
            }

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TrainingLesson(vmAdminTrainingLessonEdit model)
        {
            //***************** VALIDATION ***************************
            if (model.lesson.COURSE_IDX == null)
            {
                TempData["Error"] = "Error saving data";
                return View(model);
            }
            //***************** END VALIDATION ***************************

            Guid? c = model.lesson.LESSON_IDX;
            if (c == Guid.Empty)
                c = null;

            Guid? succID = db_Train.InsertUpdateT_QREST_TRAIN_LESSON(c, model.lesson.LESSON_TITLE, model.lesson.LESSON_DESC, model.lesson.COURSE_IDX, model.lesson.LESSON_SEQ);
            if (succID != null)
            {
                TempData["Success"] = "Data saved";
                return RedirectToAction("TrainingLesson", new { id = succID });
            }
            else
            {
                TempData["Error"] = "Error saving data";
                return View(model);
            }
        }


        public ActionResult TrainingLessonStep(Guid? id, Guid? lessonid)
        {
            var model = new vmAdminTrainingStepEdit();
            model.completeType = "BTN";

            //update case
            if (id != null)
            {
                model.step = db_Train.GetT_QREST_TRAIN_LESSON_STEP_byID(id.GetValueOrDefault());
                model.stepDesc = model.step.LESSON_STEP_DESC;

                //set completion method radio button
                if (!string.IsNullOrEmpty(model.step.REQUIRED_URL)) model.completeType = "URL";
                else if (!string.IsNullOrEmpty(model.step.REQUIRED_YT_VID)) model.completeType = "VID";
            }
            else if (lessonid != null) //insert case
            {
                model.step = new T_QREST_TRAIN_LESSON_STEP
                {
                    LESSON_IDX = lessonid.GetValueOrDefault()
                };
            }
            else if (lessonid == null && id == null)
            {
                TempData["Error"] = "Unable to find lesson to edit";
                return RedirectToAction("TrainingConfig");
            }

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TrainingLessonStep(vmAdminTrainingStepEdit model)
        {
            //***************** VALIDATION ***************************
            if (model.step.LESSON_IDX == null)
            {
                TempData["Error"] = "Error saving data";
                return View(model);
            }
            //***************** END VALIDATION ***************************

            if (model.completeType == "BTN")
            {
                model.step.REQUIRED_URL = "";
                model.step.REQUIRED_YT_VID = "";
                model.step.REQ_CONFIRM = true;
            }
            if (model.completeType == "URL")
            {
                model.step.REQUIRED_YT_VID = "";
                model.step.REQ_CONFIRM = false;
            }
            else if (model.completeType == "VID")
            {
                model.step.REQUIRED_URL = "";
                model.step.REQ_CONFIRM = false;
            }

            Guid? c = model.step.LESSON_STEP_IDX;
            if (c == Guid.Empty)
                c = null;

            Guid? succID = db_Train.InsertUpdateT_QREST_TRAIN_LESSON_STEP(c, model.step.LESSON_IDX, model.step.LESSON_STEP_SEQ, model.stepDesc,
                model.step.REQUIRED_URL ?? "", model.step.REQ_CONFIRM, model.step.REQUIRED_YT_VID ?? "");
            if (succID != null)
            {
                TempData["Success"] = "Data saved";
                return RedirectToAction("TrainingLessonStep", new { id = succID });
            }
            else
            {
                TempData["Error"] = "Error saving data";
                return View(model);
            }
        }


        [HttpPost]
        public JsonResult TrainingLessonDelete(Guid? id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int succId = db_Train.DeleteT_QREST_TRAIN_LESSON(id ?? Guid.Empty);
                if (succId == 1)
                    return Json("Success");
                else
                    return Json("Unable to delete step.");
            }
        }


        [HttpPost]
        public JsonResult TrainingLessonStepDelete(Guid? id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int succId = db_Train.DeleteT_QREST_TRAIN_LESSON_STEP(id ?? Guid.Empty);
                if (succId == 1)
                    return Json("Success");
                else
                    return Json("Unable to delete step.");
            }
        }


        public ActionResult TrainingProgress()
        {
            var model = new vmAdminTrainingCourseProgress
            {
               // course_progress = db_Train.TRAINING_SNAPSHOT()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult TrainingProgressData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
            var data = db_Train.TRAINING_SNAPSHOT();
            var recordsTotal = data.Count();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TrainingCourseCopy(vmAdminTrainingCourseEdit model)
        {
            if (model.course.COURSE_IDX != Guid.Empty)
            {
                T_QREST_TRAIN_COURSE _course = db_Train.GetT_QREST_TRAIN_COURSE_byID(model.course.COURSE_IDX);
                List<T_QREST_TRAIN_LESSON> _lessons = db_Train.GetT_QREST_TRAIN_LESSONS_byCourseStraight(_course.COURSE_IDX);
                if (_course != null)
                {
                    //copy course
                    Guid? _newCourseID = db_Train.InsertUpdateT_QREST_TRAIN_COURSE(null, _course.COURSE_NAME + "_COPY", _course.COURSE_DESC, _course.COURSE_SEQ, false);
                    if (_newCourseID != null && _lessons !=null)
                    {
                        int seq = 1;
                        foreach (var _lesson in _lessons)
                        {
                            //copy lessons
                            Guid? _newLessonID = db_Train.InsertUpdateT_QREST_TRAIN_LESSON(null, _lesson.LESSON_TITLE, _lesson.LESSON_DESC, _newCourseID.Value, seq);
                            seq++;

                            if (_newLessonID != null && _newLessonID != Guid.Empty)
                            {
                                //get existing less steps
                                List<T_QREST_TRAIN_LESSON_STEP> _steps = db_Train.GetT_QREST_TRAIN_LESSON_STEPS_byLessonID(_lesson.LESSON_IDX);

                                foreach (var _step in _steps)
                                {
                                    db_Train.InsertUpdateT_QREST_TRAIN_LESSON_STEP(null, _newLessonID.Value, _step.LESSON_STEP_SEQ, _step.LESSON_STEP_DESC, _step.REQUIRED_URL, _step.REQ_CONFIRM, _step.REQUIRED_YT_VID);
                                }

                            }
                        }
                    }
                }

                TempData["SUCCESS"] = "Course copied to " + _course.COURSE_NAME + "_COPY and set to Inactive";
                return RedirectToAction("TrainingConfig");

            }
            else
            {
                TempData["ERROR"] = "Unable to copy course";
                return RedirectToAction("TrainingConfig");
            }
        }



        //************************************* TEST METHODS ************************************************************
        public ActionResult TestAIRNOW() {
            string ftpUser = db_Ref.GetT_QREST_APP_SETTING("AIRNOW_FTP_USER");
            string ftpPwd = db_Ref.GetT_QREST_APP_SETTING("AIRNOW_FTP_PWD");
            string ip = "webdmcdata.airnowtech.org";

            using (var client = new Renci.SshNet.SftpClient(ip, ftpUser, ftpPwd))
            {
                try
                {
                    client.Connect();
                    client.ChangeDirectory("AQCSV");
                    var xxx = client.ListDirectory(".");
                    client.Disconnect();
                    TempData["Success"] = "Credentials successful";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Failed: " + ex.Message;
                }
            }

            return RedirectToAction("AppSettings");

        }
      
    }
}