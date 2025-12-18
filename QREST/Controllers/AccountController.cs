using QREST.App_Logic.BusinessLogicLayer;
using QREST.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System;
using QRESTModel.DAL;
using QRESTModel.BLL;

namespace QREST.Controllers
{

    public class AccountController : Controller
    {
        #region CONSTRUCTOR
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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



        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            //attempt to sign in
            var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = await UserManager.FindByEmailAsync(model.Username);

                    //update the last login datetime
                    user.LAST_LOGIN_DT = System.DateTime.Now;
                    UserManager.Update(user);

                    // Require the user to have a confirmed email before they can log on.
                    if (!user.EmailConfirmed)
                    {
                        //automatically resend the confirmation email link
                        bool succInd = await SendEmailConfirmationTokenAsync(user.Id, user.Email, "EMAIL_CONFIRM");

                        //force sign out of user
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                        ModelState.AddModelError("", "You must have a confirmed email to log on. The confirmation token has been resent to your email.");
                        return View(model);
                    }
                    else
                    {
                        db_Ref.CreateT_QREST_SYS_LOG_ACTIVITY("LOGIN", user.Id, System.DateTime.Now, "User logged in", GetIP.GetLocalIPAddress(System.Web.HttpContext.Current), null);
                        return RedirectToLocal(returnUrl);
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }


        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }


        public ActionResult QRESTRegister()
        {
            return View(new RegisterViewModel()
            {
                termsConditions = db_Ref.GetT_QREST_APP_SETTING_CUSTOM()?.TERMS_AND_CONDITIONS,
                ddl_Agencies = ddlHelpers.get_ddl_organizations(true, true)
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QRESTRegister(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FNAME = model.FirstName,
                    LNAME = model.LastName,
                    NOTIFY_APP_IND = true,
                    NOTIFY_EMAIL_IND = true,
                    NOTIFY_SMS_IND = false,
                    CREATE_USER_IDX = "SYSTEM",
                    CREATE_DT = System.DateTime.Now
                };

                //********************* SPAM CHECK *******************************
                var tld = model.Email.Substring(model.Email.LastIndexOf('.') + 1).ToLower();
                if (tld == "ru" || tld == "xyz" || tld == "de" 
                    || tld == "tst" || tld == "fr" || tld == "top"
                    || model.Email.Contains("andex"))
                {
                    TempData["Error"] = "QREST is intended for tribal users only. Please contact ITEP to obtain an account.";
                    return RedirectToAction("QRESTRegister", "Account");
                }

                // ******************** AGENCY VALIDATION ******************************
                if (!string.IsNullOrEmpty(model.selOrgID))
                {
                    //see if the agency filters registration by email rule 
                    List<T_QREST_ORG_EMAIL_RULE> emails = db_Account.GetT_QREST_ORG_EMAIL_RULE(model.selOrgID);
                    if (emails != null && emails.Count > 0)
                    {
                        if (db_Account.IsValidT_QREST_ORG_EMAIL(model.Email, model.selOrgID) == false)
                        {
                            await Task.Delay(5000); // Wait for 5 seconds
                            TempData["Error"] = "Registration for this organization is restricted by email. Please contact ITEP or the tribal representative to obtain an account.";
                            db_Ref.CreateT_QREST_SYS_LOG(null, "REGISTER", model.Email + "Failed to register - email not valid for tribe " + model.selOrgID + " " + GetIP.GetLocalIPAddress(System.Web.HttpContext.Current));
                            return RedirectToAction("QRESTRegister", "Account");
                        }
                    }
                    else
                    {
                        await Task.Delay(5000); // Wait for 5 seconds
                        TempData["Error"] = "Registration for this organization is not allowed at this time. (Ref: email settings).";
                        db_Ref.CreateT_QREST_SYS_LOG(null, "REGISTER", model.Email + " Failed to register for org [" + model.selOrgID + "] no email rule " + GetIP.GetLocalIPAddress(System.Web.HttpContext.Current));
                        return RedirectToAction("QRESTRegister", "Account");
                    }


                }
                else
                {
                    await Task.Delay(5000); // Wait for 5 seconds
                    TempData["Error"] = "QREST is intended for tribal users only. Please select the tribe you wish to register with.";
                    db_Ref.CreateT_QREST_SYS_LOG(null, "REGISTER", model.Email + "Failed to register by not selecting any tribe " + GetIP.GetLocalIPAddress(System.Web.HttpContext.Current));
                    return RedirectToAction("QRESTRegister", "Account");
                }
                // ****************** END AGENCY VALIDATION ******************************


                //create user record
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //if first user, initialize with GLOBAL ADMIN role
                    if (UserManager.Users.Count() == 1)
                        UserManager.AddToRole(user.Id, "GLOBAL ADMIN");

                    // Generate secure verification link for email
                    bool succInd = await SendEmailConfirmationTokenAsync(user.Id, model.Email, "EMAIL_CONFIRM");
                    if (succInd)
                    {
                        //************************************************************
                        //**************** USER / ORG ASSOCIATION ********************
                        //************************************************************
                        if (model.selOrgID != null)
                        {
                            string regStatus = "P";  //default org registration status is pending

                            //if user has matching email domain for the org, then automatically authorize (no need for pending)
                            T_QREST_ORGANIZATIONS oe = db_Ref.GetT_QREST_ORGANIZATION_ByOrg_Email(model.selOrgID, model.Email);
                            if (oe != null)
                                regStatus = "A";

                            //associate the user with the organization
                            Guid? OrgUserID = db_Account.InsertUpdateT_QREST_ORG_USERS(user.Id, model.selOrgID, "U", regStatus, null);

                            //notify organization admins (via email)
                            if (OrgUserID != Guid.Empty && OrgUserID != null)
                            {
                                //find if any org admins exist
                                List<UserOrgDisplayType> OrgAdmins = db_Account.GetT_QREST_ORG_USERS_ByOrgID(model.selOrgID, "A", "A");
                                if (OrgAdmins != null && OrgAdmins.Count > 0)
                                {
                                    string fullUrl = this.Url.Action("OrgEdit", "Site", new { id = model.selOrgID }, this.Request.Url.Scheme);
                                    fullUrl = "<a href='" + fullUrl + "'>" + fullUrl + "</a>";
                                    var emailParams = new Dictionary<string, string> { { "UserName", model.Email }, { "orgName", model.selOrgID }, { "siteUrl", fullUrl } };

                                    foreach (UserOrgDisplayType OrgAdmin in OrgAdmins)
                                        UtilsNotify.NotifyUser(OrgAdmin.USER_IDX, null, null, null, null, "ACCESS_REQUEST", emailParams, null);
                                }
                                else  //there are no org admins, so send email to global admins
                                {
                                    string fullUrl = this.Url.Action("OrgEdit", "Admin", new { id = model.selOrgID }, this.Request.Url.Scheme);
                                    fullUrl = "<a href='" + fullUrl + "'>" + fullUrl + "</a>";
                                    var emailParams = new Dictionary<string, string> { { "UserName", model.Email }, { "orgName", model.selOrgID }, { "siteUrl", fullUrl } };

                                    List<T_QREST_USERS> _admins = db_Account.GetT_QREST_USERSInRole("GLOBAL ADMIN");
                                    if (_admins != null)
                                    {
                                        foreach (T_QREST_USERS _admin in _admins)
                                            UtilsNotify.NotifyUser(_admin.USER_IDX, null, null, null, null, "ACCESS_REQUEST", emailParams, null);
                                    }
                                }
                            }
                        }

                        return RedirectToAction("RegisterSuccess", "Account");
                    }
                    else
                    {
                        //roll back created user
                        var rollback = await UserManager.DeleteAsync(user);
                        TempData["Error"] = "Email service is currently down, so account registration is unavailable.";
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            model.ddl_Agencies = ddlHelpers.get_ddl_organizations(true, true);
            model.termsConditions = db_Ref.GetT_QREST_APP_SETTING_CUSTOM()?.TERMS_AND_CONDITIONS;
            return View(model);
        }


        public ActionResult RegisterSuccess()
        {
            return View();
        }


        public ActionResult Lockout()
        {
            return View();
        }


        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            //don't allow user to just navigate to page
            if (userId == null || code == null)
                return RedirectToAction("Index", "Home");

            var result = await UserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
                return View();
            else
            {
                //check if user based on email is not confirmed, if not confirmed, redirect to ConfirmEmail view, otherwise
                var user = UserManager.FindById(userId);
                if (user != null && user.EmailConfirmed == false)
                {
                    bool succInd = await SendEmailConfirmationTokenAsync(user.Id, user.Email, "EMAIL_CONFIRM");
                    TempData["Error"] = "Your confirmation link has expired. A new confirmation has just been emailed to you. Please click the link within that email within the next 24 hours.";
                }
                else
                    TempData["Error"] = "Invalid Provider/PIN combination.";

                return RedirectToAction("Index", "Home");
            }
        }


        public ActionResult ResendConfirmEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendConfirmEmail(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);

                // Don't reveal that the user does not exist or is not confirmed
                if (user == null)
                    return View();

                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    //automatically resend the confirmation email link
                    bool succInd = await SendEmailConfirmationTokenAsync(user.Id, user.Email, "EMAIL_CONFIRM");

                    ModelState.AddModelError("", "Confirmation email has been resent.  **IF YOU DO NOT SEE IT, DOUBLECHECK YOUR SPAM FOLDER.**");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", "Email has already been confirmed.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);

        }


        public ActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {   
                    //automatically resend the confirmation email link
                    bool succInd = await SendEmailConfirmationTokenAsync(user.Id, user.Email, "EMAIL_CONFIRM");

                    ModelState.AddModelError("", "You must have a confirmed email before you can reset your password. A confirmation token has been resent to your email.");
                    return View(model);
                }

                // Generate secure verification link for email
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                code = HttpUtility.UrlEncode(code);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                // Send Email
                var emailParams = new Dictionary<string, string> { { "callbackUrl", callbackUrl } };
                UtilsEmail.SendEmail(null, model.Email, null, null, null, null, "RESET_PASSWORD", emailParams);
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        public ActionResult ResetPassword(string code)
        {
            //if no code supplied, then redirect to home page
            if (code == null)
                return RedirectToAction("Index", "Home");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            model.Code = HttpUtility.UrlDecode(model.Code);
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }


        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }



        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }


        [Authorize]
        public ActionResult MyProfile()
        {
            string UserIDX = User.Identity.GetUserId();
            var user = UserManager.FindById(UserIDX);

            if (user != null)
            {
                var model = new vmAccountMyProfile {
                    FNAME = user.FNAME,
                    LNAME = user.LNAME,
                    EMAIL = user.Email,
                    NOTIFY_APP_IND = user.NOTIFY_APP_IND ?? false,
                    NOTIFY_EMAIL_IND = user.NOTIFY_EMAIL_IND ?? false,
                    NOTIFY_SMS_IND = user.NOTIFY_SMS_IND ?? false,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed
                };

                model.ROLES = UserManager.GetRoles(user.Id);
                model.user_orgs = db_Account.GetT_QREST_ORG_USERS_byUSER_IDX(user.Id, null);

                return View(model);
            }
            else
                return RedirectToAction("Index", "Dashboard");

        }


        public ActionResult GetAPIKey(string org)
        {
            string UserIDX = User.Identity.GetUserId();
            db_Account.SetT_QREST_ORG_USERS_APIKEY(UserIDX, org);

            return RedirectToAction("MyProfile");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyProfile(vmAccountMyProfile model)
        {
            if (model.NOTIFY_SMS_IND == true && model.PhoneNumber == null)
            {
                ModelState.AddModelError("PhoneNumber", "You must supply a phone number.");
            }


            if (ModelState.IsValid)
            {
                string UserIDX = User.Identity.GetUserId();
                var user = UserManager.FindById(UserIDX);

                user.FNAME = model.FNAME;
                user.LNAME = model.LNAME;
                user.NOTIFY_APP_IND = model.NOTIFY_APP_IND;
                user.NOTIFY_EMAIL_IND = model.NOTIFY_EMAIL_IND;
                user.NOTIFY_SMS_IND = false;// model.NOTIFY_SMS_IND;
                user.PhoneNumber = model.PhoneNumber ?? "";
                user.PhoneNumberConfirmed = model.PhoneNumberConfirmed;

                UserManager.Update(user);

                TempData["Success"] = "Profile updated.";
                model.ROLES = UserManager.GetRoles(user.Id);
                model.user_orgs = db_Account.GetT_QREST_ORG_USERS_byUSER_IDX(user.Id, null);
            }

            return View(model);
        }

        ///// <summary>
        ///// Twilio removed from QREST in 2025
        ///// </summary>
        ///// <returns></returns>
        //[Authorize]
        //public ActionResult TestSMS()
        //{
        //    string UserIDX = User.Identity.GetUserId();
        //    bool SuccInd = UtilsSMS.sendSMS(UserIDX, "This is a test message from QREST. You can now receive QREST alerts directly to your phone.");
        //    if (SuccInd)
        //        TempData["Success"] = "Text message sent.";
        //    else
        //        TempData["Error"] = "Unable to send text message.";

        //    return RedirectToAction("MyProfile");
        //}


        [Authorize]
        public ActionResult Notifications()
        {
            string UserIDX = User.Identity.GetUserId();
            var model = new vmAccountNotifications();
            model.notifications = db_Account.GetT_QREST_USER_NOTIFICATION_ByUserID(UserIDX);
            return View(model);
        }


        public ActionResult NotificationDelete2(Guid? id)
        {
            string UserIDX = User.Identity.GetUserId();

            //CHECK PERMISSIONS
            T_QREST_USER_NOTIFICATION n = db_Account.GetT_QREST_USER_NOTIFICATION_ByID(id.GetValueOrDefault());
            if (n != null && UserIDX == n.USER_IDX)
            {
                int SuccID = db_Account.DeleteT_QREST_USER_NOTIFICATION(id.GetValueOrDefault());
                if (SuccID > 0)
                    TempData["Success"] = "Deleted";
                else
                    TempData["Error"] = "Unable to delete notification";
            }

            return RedirectToAction("Notifications", "Account");
        }


        [HttpPost]
        public JsonResult NotificationDeleteAll(string id)
        {
            string UserIDX = User.Identity.GetUserId();

            int succId = db_Account.DeleteT_QREST_USER_NOTIFICATION_All_ByUserID(UserIDX);
            if (succId == 1)
                return Json("Success");
            else
                return Json("Unable to delete");
        }


        [HttpPost]
        public JsonResult NotificationRead(Guid? id)
        {
            string UserIDX = User.Identity.GetUserId();

            //CHECK PERMISSIONS
            T_QREST_USER_NOTIFICATION n = db_Account.GetT_QREST_USER_NOTIFICATION_ByID(id.GetValueOrDefault());
            if (n != null && UserIDX == n.USER_IDX)
            {
                Guid? SuccID = db_Account.InsertUpdateT_QREST_USER_NOTIFICATION(n.NOTIFICATION_IDX, null, null, null, null, null, true, UserIDX);
                if (SuccID != null)
                    return Json(new { msg = "Success" });
            }

            //return ERROR
            return Json(new { msg = "Unable to mark read." });
        }



        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }



        #region Helpers
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Dashboard");
        }


        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

        }


        private async Task<bool> SendEmailConfirmationTokenAsync(string userID, string email, string emailTemplate)
        {
            // Generate secure verification link for email
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userID, code }, protocol: Request.Url.Scheme);

            // Send Email
            var emailParams = new Dictionary<string, string> { { "callbackUrl", callbackUrl } };
            bool succInd = UtilsEmail.SendEmail(null, email, null, null, null, null, "EMAIL_CONFIRM", emailParams);
            return succInd;
        }

        #endregion
    }
}