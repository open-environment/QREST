using QREST.App_Logic.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QREST.Models
{
    public class vmAdminAppSettings
    {
        public List<T_QREST_APP_SETTINGS> T_VCCB_APP_SETTINGS { get; set; }
        public T_QREST_APP_SETTINGS edit_app_setting { get; set; }

        [DisplayName("Terms & Conditions")]
        [UIHint("wsywigeditor"), AllowHtml]
        [StringLength(6000)]
        public string TermsAndConditions { get; set; }

        [DisplayName("Announcements")]
        [UIHint("wsywigeditor"), AllowHtml]
        [StringLength(6000)]
        public string Announcements { get; set; }

    }



    public class vmAdminEmailConfig
    {
        public List<T_QREST_EMAIL_TEMPLATE> T_QREST_EMAIL_TEMPLATE { get; set; }

        public int? editID { get; set; }
        public string editDESC { get; set; }
        public string editSUBJ { get; set; }

        [UIHint("wsywigeditor"), AllowHtml]
        [StringLength(6000)]
        public string editMSG { get; set; }
    }


    public class vmAdminHelpConfig
    {
        public List<T_QREST_HELP_DOCS> HelpTopics { get; set; }
        public T_QREST_HELP_DOCS EditHelp { get; set; }

        [UIHint("wsywigeditor"), AllowHtml]
        [StringLength(6000)]
        public string editHelpHtml { get; set; }

    }


    public class vmAdminDocConfig
    {
    }


    public class vmAdminTaskConfig
    {
    }

    //****************************************************************************
    // ******************************** ORGANIZATIONS ****************************
    //****************************************************************************
    public class vmAdminOrgList
    {
        public List<T_QREST_ORGANIZATIONS> T_QREST_ORGANIZATIONS { get; set; }
    }

    public class vmAdminOrgEdit
    {
        [Required]
        [MaxLength(30, ErrorMessage = "Name cannot be longer than 30 characters.")]
        public string ORG_ID { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string ORG_NAME { get; set; }

        public string AQS_AGENCY_CODE { get; set; }
        public string STATE_CD { get; set; }
        public int? EPA_REGION { get; set; }
        public string AQS_NAAS_UID { get; set; }
        public string AQS_NAAS_PWD { get; set; }

        public List<UserOrgDisplayType> org_users { get; set; }

        public IEnumerable<SelectListItem> ddl_State { get; set; }
        public IEnumerable<SelectListItem> ddl_Region { get; set; }
        public IEnumerable<SelectListItem> ddl_AqsAgency { get; set; }
        public IEnumerable<SelectListItem> ddl_User { get; set; }
        public IEnumerable<SelectListItem> ddl_user_status { get; set; }
        public IEnumerable<SelectListItem> ddl_user_role { get; set; }

        //org user
        public string edit_typ { get; set; }
        public string edit_org_id { get; set; }
        public string edit_user_idx { get; set; }
        public string edit_org_user_status { get; set; }
        public string edit_org_user_access_level { get; set; }

        public vmAdminOrgEdit()
        {
            ddl_State = ddlHelpers.get_ddl_state();
            ddl_Region = ddlHelpers.get_ddl_region();
            ddl_AqsAgency = ddlHelpers.get_ddl_aqs_agency();
            ddl_user_status = ddlHelpers.get_ddl_user_status();
            ddl_user_role = ddlHelpers.get_ddl_user_role();
        }

    }


    public class vmAdminOrgEditUser {
        [Required]
        public string edit_typ { get; set; }
        [Required]
        public string edit_org_id { get; set; }
        [Required]
        public string edit_user_idx { get; set; }
        [Required]
        public string edit_org_user_status { get; set; }
        [Required]
        public string edit_org_user_access_level { get; set; }
    }

    //****************************************************************************
    // ******************************** USERS ************************************
    //****************************************************************************
    public class vmAdminUserList
    {
        public List<T_QREST_USERS> T_QREST_USERS { get; set; }
    }


    public class vmAdminUserEdit {
        public ApplicationUser user { get; set; }


        //role editing
        public IEnumerable<SelectListItem> Roles_In_User { get; set; }
        public IEnumerable<string> Roles_In_User_Selected { get; set; }
        public IEnumerable<SelectListItem> Roles_Not_In_User { get; set; }
        public IEnumerable<string> Roles_Not_In_User_Selected { get; set; }

        //org user
        public List<UserOrgDisplayType> user_orgs { get; set; }
        public IEnumerable<SelectListItem> ddl_user_status { get; set; }
        public IEnumerable<SelectListItem> ddl_user_role { get; set; }
        public IEnumerable<SelectListItem> ddl_Org { get; set; }

        public string edit_typ { get; set; }
        public string edit_org_id { get; set; }
        public string edit_user_idx { get; set; }
        public string edit_org_user_status { get; set; }
        public string edit_org_user_access_level { get; set; }


        public vmAdminUserEdit()
        {
            ddl_user_status = ddlHelpers.get_ddl_user_status();
            ddl_user_role = ddlHelpers.get_ddl_user_role();
            ddl_Org = ddlHelpers.get_ddl_organizations(true);
        }

    }


    //****************************************************************************
    // ******************************** LOGGING***********************************
    //****************************************************************************
    public class vmAdminLogError
    {}

    public class vmAdminLogEmail
    {}

    public class vmAdminLogActivity
    {}


    //****************************************************************************
    // ******************************** IMPORT************************************
    //****************************************************************************
    public class vmAdminImport
    {
        public List<T_QREST_SYS_LOG> T_QREST_SYS_LOG { get; set; }
        public List<AgencyImportType> agencies { get; set; }
        public string ImportType { get; set; }
        public int? ExistCount { get; set; }
        public int? ErrorCount { get; set; }
        public int? InsertCount { get; set; }
    }
}