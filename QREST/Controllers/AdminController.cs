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
using QRESTModel.DataTableGen;
using QREST.App_Logic;
using System.Net;

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
            var model = new vmAdminEmailConfig
            {
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
                model.EditHelp.HELP_IDX = db_Ref.InsertUpdateT_QREST_HELP_DOCS(model.EditHelp.HELP_IDX, model.EditHelp.HELP_TITLE, model.editHelpHtml, model.EditHelp.SORT_SEQ);
                if (model.EditHelp.HELP_IDX > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("HelpConfig", new { id = model.EditHelp.HELP_IDX });
        }

        [HttpPost]
        public ContentResult ImagePosted(HttpPostedFileBase imageFile)
        {
            var jsonString = "";
            if (Request.Files.Count > 0)
            {
                imageFile = Request.Files[0];
                int fileSize = imageFile.ContentLength;
                string fileName = imageFile.FileName;
                string mimeType = imageFile.ContentType;
                System.IO.Stream fileContent = imageFile.InputStream;
                if (!Directory.Exists(Server.MapPath("~/TinyMCEImg/")))
                    Directory.CreateDirectory(Server.MapPath("~/TinyMCEImg/"));
                string filePath = Server.MapPath("~/TinyMCEImg/") + fileName;
                imageFile.SaveAs(filePath);
                jsonString = String.Format("{{\"location\":\"{0}\"}}", "/TinyMCEImg/" + fileName);
            }
            return Content(jsonString);

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
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //Filter based on user type
            int? usertype = Request.Form.GetValues("usertype")?.FirstOrDefault().ConvertOrDefault<int>();

            
            string mailSendMode = Request.Form.GetValues("mailsendmode").FirstOrDefault();
            bool mailSendStatus = true;

            var data = db_Account.GetT_QREST_USERS(usertype, pageSize, start, orderColName, orderDir);
            var recordsTotal = db_Account.GetT_QREST_USERScount(usertype);
            
            //Send mail to filtered users
            if (mailSendMode.Equals("1"))
            {
                string subject = System.Uri.UnescapeDataString(Request.Form.GetValues("mailsubject").FirstOrDefault());
                string body = System.Uri.UnescapeDataString(Request.Form.GetValues("mailbody").FirstOrDefault());

                //We want all the filtered records, not restricted by pageSize
                pageSize = recordsTotal;
                start = 0;

                //"mailData" is a seperated call from "data" above, since "mailData"
                //contains all the filtered records, whereas "data" is filtered by pageSize as well
                //Refactor: If we are in mail send mode, we can get "mailData" and then filter by pageSize
                //using linq to reduce duplicate db call
                var mailData = db_Account.GetT_QREST_USERS(usertype, pageSize, start, orderColName, orderDir);
                if(mailData != null && mailData.Count > 0)
                {
                    string UserIDX = User.Identity.GetUserId();
                    mailSendStatus = db_Account.SendMailToUsers(mailData, subject, body, UserIDX);
                }
            }
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data, mailSendMode = mailSendMode, mailSendStatus = mailSendStatus });
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

                bool SuccInd = db_Ref.UpdateT_QREST_TASKS(model.EditTask.TASK_IDX, model.EditTask.FREQ_TYPE, model.EditTask.FREQ_NUM,
                    null, model.EditTask.NEXT_RUN_DT, model.EditTask.STATUS, UserIDX);

                if (SuccInd)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("TaskConfig", new { id = model.EditTask?.TASK_IDX });
        }


        public ActionResult TaskStop(int? id)
        {
            bool SuccInd = db_Ref.UpdateT_QREST_TASKS_SetStopped(id ?? -1);

            if (SuccInd)
                TempData["Success"] = "Task Stopped.";
            else
                TempData["Error"] = "Task Not Stopped.";

            return RedirectToAction("TaskConfig", new { id = id });
        }

        public ActionResult TaskStart(int? id)
        {
            bool SuccInd = db_Ref.UpdateT_QREST_TASKS_SetCompleted(id ?? -1);

            if (SuccInd)
                TempData["Success"] = "Task Scheduled to Run.";
            else
                TempData["Error"] = "Task Not Scheduled to Run.";

            return RedirectToAction("TaskConfig", new { id = id });
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
            string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //date filters
            DateTime? minDate = Request.Form.GetValues("mini")?.FirstOrDefault().ConvertOrDefault<DateTime?>();
            DateTime? maxDate = Request.Form.GetValues("maxi")?.FirstOrDefault().ConvertOrDefault<DateTime?>();

            var data = db_Ref.GetT_QREST_SYS_LOG(minDate, maxDate, pageSize, start, orderColName, orderDir);
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
            string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //date filters
            DateTime? minDate = Request.Form.GetValues("mini")?.FirstOrDefault().ConvertOrDefault<DateTime?>();
            DateTime? maxDate = Request.Form.GetValues("maxi")?.FirstOrDefault().ConvertOrDefault<DateTime?>();

            var data = db_Ref.GetT_QREST_SYS_LOG_EMAIL(minDate, maxDate, pageSize, start, orderColName, orderDir);
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
            string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //date filters
            DateTime? minDate = Request.Form.GetValues("mini")?.FirstOrDefault().ConvertOrDefault<DateTime?>();
            DateTime? maxDate = Request.Form.GetValues("maxi")?.FirstOrDefault().ConvertOrDefault<DateTime?>();

            var data = db_Ref.GetT_QREST_SYS_LOG_ACTIVITY(minDate, maxDate, pageSize, start, orderColName, orderDir);
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
                                else if (cols[0] == "Parameter Code")
                                    importType = "PARAMETERS";
                                else if (cols[0] == "Parameter" && cols[2] == "Method Code")
                                    importType = "METHODS";
                                else if (cols[0] == "State Code")
                                    importType = "STATES COUNTIES";
                                else if (cols[0] == "Qualifier Code")
                                    importType = "QUALIFIERS";

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
                                            bool SuccID = db_Ref.InsertUpdatetT_QREST_REF_PARAMETERS(cols[0], cols[1], cols[3], cols[4], _unit.UNIT_CODE, true, cols[6] == "YES", UserIDX);
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
                                    //lookup unit code
                                    T_QREST_REF_UNITS _unit = db_Ref.GetT_QREST_REF_UNITS_ByDesc(cols[14]);
                                    if (_unit != null)
                                    {
                                        Tuple<string, string> SuccID = db_Ref.InsertUpdateT_QREST_REF_PAR_METHODS(null, cols[1], cols[2], cols[3], cols[4], cols[5], cols[7], cols[8], _unit.UNIT_CODE, cols[9].ConvertOrDefault<double?>(), cols[10].ConvertOrDefault<double?>(), cols[11].ConvertOrDefault<double?>(), null, null, UserIDX);
                                        if (SuccID.Item1 == "I")
                                            insCount++;
                                        else if (SuccID.Item1 == "U")
                                        {
                                            if (SuccID.Item2.Length == 0)
                                                existCount++;
                                            else
                                                updateDetails.Add(cols[1] + "/" + cols[2] + " has been modified: " + SuccID.Item2);

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
                                        bool SuccID = db_Ref.InsertUpdatetT_QREST_REF_COUNTY(cols[0], cols[3], cols[4]);
                                        if (SuccID)
                                            insCount++;
                                        else
                                            errorCount++;
                                    }
                                    else
                                        existCount++;
                                }
                                if (importType == "QUALIFIERS" && cols[4] == "YES")
                                {
                                    T_QREST_REF_QUALIFIER _data = db_Ref.GetT_QREST_REF_QUALIFIER_ByID(cols[0]);
                                    if (_data == null)
                                    {
                                        bool SuccID = db_Ref.InsertUpdatetT_QREST_REF_QUALIFIER(cols[0], cols[1], cols[3], UserIDX);
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
                InsertCount = insCount,
                UpdateDetails = updateDetails
            };
            return View(model);
        }



        //************************************* REF DATA ************************************************************
        // GET: /Admin/RefData
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


        public ActionResult RefPar()
        {
            var model = new vmAdminRefPar();
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
        public ActionResult RefParEdit(vmAdminRefPar model)
        {
            if (ModelState.IsValid)
            {
                bool SuccInd = db_Ref.InsertUpdatetT_QREST_REF_PARAMETERS(model.editPAR_CODE, model.editPAR_NAME, null, null, model.editSTD_UNIT_CODE, false, true, User.Identity.GetUserId());

                if (SuccInd)
                    TempData["Success"] = "Record updated";
                else
                    TempData["Error"] = "Error updating record.";
            }
            else
                TempData["Error"] = "Error updating record";

            return RedirectToAction("RefPar", "Admin");
        }


        [HttpPost]
        public JsonResult RefParDelete(string id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int SuccID = db_Ref.DeleteT_QREST_REF_PARAMETERS(id);
                if (SuccID == 1)
                    return Json("Success");
                else
                    return Json("This record cannot be deleted. If this parameter has parameter methods, they may need to be deleted first.");
            }
        }


        public ActionResult RefParMethod()
        {
            var model = new vmAdminRefParMethod();
            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RefParMethod(vmAdminRefParMethod model)
        {
            if (ModelState.IsValid && model.editPAR_METHOD_IDX != null)
            {
                Tuple<string, string> SuccInd = db_Ref.InsertUpdateT_QREST_REF_PAR_METHODS(model.editPAR_METHOD_IDX, null, null, null, null, null, null, null, null, null, null, null,
                    model.editCUST_MIN_VALUE ?? -9999, model.editCUST_MAX_VALUE ?? -9999, User.Identity.GetUserId());

                if (SuccInd.Item1 == "U")
                    TempData["Success"] = "Record updated";
                else
                    TempData["Error"] = "Error updating record.";
            }
            else
                TempData["Error"] = "Error updating record";

            return RedirectToAction("RefParMethod", "Admin");
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


        public ActionResult ExportExcel(string Action)
        {
            DataTable dt = null;
            if (Action == "RefParMethod")
                dt = DataTableGen.RefParMethod("", "");

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
                return RedirectToAction(Action);
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

            List<T_QREST_REF_PAR_UNITS> data = db_Ref.GetT_QREST_REF_PAR_UNITS_data(pageSize, start, orderColName, orderDir);
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



        //************************************* CONNECTIVITY ************************************************************
        // GET: /Admin/Connectivity
        public ActionResult Connectivity()
        {
            var model = new vmAdminConnectivity();
            model.PollingConfig = db_Air.GetT_QREST_SITES_POLLING_CONFIG_CompleteList();
            return View(model);
        }




        //************************************* TEST METHODS ************************************************************
        public ActionResult Testing()
        {

            List<AIRNOW_LAST_HOUR> _recs = db_Air.GetAIRNOW_LAST_HOUR();
            if (_recs != null)
            {
                foreach (AIRNOW_LAST_HOUR _rec in _recs)
                {
                    
                }
            }

            return View();
        }


        public ActionResult HourlyPollValidation()
        {
            //this is where logic for the task goes
            db_Air.SP_VALIDATE_HOURLY();

            //then send out notifications
            List<string> NotifyUsers = db_Air.GetT_QREST_DATA_HOURLY_NotificationUsers();
            foreach (string u in NotifyUsers)
            {
                string msg = "" + Environment.NewLine;
                List<RawDataDisplay> notifies = db_Air.GetT_QREST_DATA_HOURLY_NotificationsListForUser(u);
                foreach (RawDataDisplay n in notifies)
                {
                    msg += n.SITE_ID + ": " + n.PAR_NAME + ": " + n.VAL_CD + " alert." + Environment.NewLine;
                }

                var emailParams = new Dictionary<string, string> { { "notifyMsg", msg } };
                QRESTModel.BLL.UtilsNotify.NotifyUser(u, null, null, null, null, "POLLING_ALERT", emailParams, null);
            }

            //then update all records to notified
            List<T_QREST_DATA_HOURLY> xxx = db_Air.GetT_QREST_DATA_HOURLY_NotNotified();
            foreach (T_QREST_DATA_HOURLY xx in xxx)
            {
                db_Air.UpdateT_QREST_DATA_HOURLY_Notified(xx.DATA_HOURLY_IDX);
            }


            return View("Testing");
        }

        public ActionResult AirNow()
        {
            using (var client = new WebClient())
            {
                string ftpUser = db_Ref.GetT_QREST_APP_SETTING("AIRNOW_FTP_USER");
                string ftpPwd = db_Ref.GetT_QREST_APP_SETTING("AIRNOW_FTP_PWD");

                client.Credentials = new System.Net.NetworkCredential(ftpUser, ftpPwd);
                client.UploadFile("ftp://ftp.airnowdata.org/incoming/data/AQCSV/202002261403_840.TRX", WebRequestMethods.Ftp.UploadFile, @"C:\temp\202002261403_840.TRX");
            }

            return View("Testing");
        }

    }
}