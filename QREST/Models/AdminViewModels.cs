﻿using QRESTModel.DAL;
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
        public string editHelpHtml { get; set; }
    }




    // ******************************** TASKS******** ****************************
    //****************************************************************************
    //****************************************************************************
    public class vmAdminTaskConfig
    {
        public List<T_QREST_APP_TASKS> Tasks { get; set; }
        public T_QREST_APP_TASKS EditTask { get; set; }
    }


    // ******************************** ORGANIZATIONS ****************************
    //****************************************************************************
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
        public bool SELF_REG_IND { get; set; }
        public bool LOCK_ACCESS_IND { get; set; }


        public List<UserOrgDisplayType> org_users { get; set; }
        public List<T_QREST_ORG_EMAIL_RULE> org_emails { get; set; }

        //dropdownlist lookups
        public IEnumerable<SelectListItem> ddl_State { get; set; }
        public IEnumerable<SelectListItem> ddl_Region { get; set; }
        public IEnumerable<SelectListItem> ddl_AqsAgency { get; set; }
        public IEnumerable<SelectListItem> ddl_User { get; set; }
        public IEnumerable<SelectListItem> ddl_user_status { get; set; }
        public IEnumerable<SelectListItem> ddl_user_role { get; set; }

        //org user editing
        public string edit_typ { get; set; }
        public string edit_org_id { get; set; }
        public string edit_user_idx { get; set; }
        public string edit_org_user_status { get; set; }
        public string edit_org_user_access_level { get; set; }

        //email rule editing
        public string edit_email_rule { get; set; }

        public vmAdminOrgEdit()
        {
            ddl_State = ddlHelpers.get_ddl_state();
            ddl_Region = ddlHelpers.get_ddl_region();
            ddl_AqsAgency = ddlHelpers.get_ddl_aqs_agency();
            ddl_user_status = ddlHelpers.get_ddl_user_status();
            ddl_user_role = ddlHelpers.get_ddl_org_user_role();
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

    public class vmAdminOrgEditEmail {
        [Required]
        public string edit_org_id { get; set; }
        [Required]
        public string edit_email_rule { get; set; }
    }

    // ******************************** USERS ************************************
    //****************************************************************************
    //****************************************************************************
    public class vmAdminUserList
    {
        public List<UserListDisplayType> T_QREST_USERS { get; set; }

        public bool IsMailSendMode { get; set; }

        [Required]
        public string EmailSubject { get; set; }

       
        [UIHint("wsywigeditor"), AllowHtml]
        [StringLength(6000)]
        public string emailBodyHtml { get; set; }
        public int? usertype { get; set; }

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
            ddl_user_role = ddlHelpers.get_ddl_org_user_role();
            ddl_Org = ddlHelpers.get_ddl_organizations(true, false);
        }

    }





    // ******************************** IMPORT************************************
    //****************************************************************************
    //****************************************************************************
    public class vmAdminImport
    {
        public List<AgencyImportType> agencies { get; set; }
        public string ImportType { get; set; }
        public int? ExistCount { get; set; }
        public int? ErrorCount { get; set; }
        public int? InsertCount { get; set; }
        public List<string> UpdateDetails { get; set; }
    }


    public class vmAdminImportHourly
    {
        public string selOrgID { get; set; }

        [Required]
        public string selMon { get; set; }
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public IEnumerable<SelectListItem> ddl_Monitor { get; set; }

        public string IMPORT_BLOCK { get; set; }
        public List<T_QREST_DATA_HOURLY> ImportData { get; set; }
        public int? ExistCount { get; set; }
        public int? ErrorCount { get; set; }
        public int? InsertCount { get; set; }
    }





    // ******************************** REPORTS******************************
    //****************************************************************************
    //****************************************************************************
    public class vmAdminConnectivity
    {
        public List<SitePollingConfigType> PollingConfig { get; set; }
    }


    public class vmAdminCDXTest
    {
        public string CDXURL { get; set; }
        public string TestProdInd { get; set; }
        public string CDXUsername { get; set; }
        public string CDXPassword { get; set; }
    }




    public class vmAdminTraining
    {
        public List<T_QREST_TRAIN_COURSE> courses { get; set; }

    }


    public class vmAdminTrainingCourseEdit
    {
        public T_QREST_TRAIN_COURSE course { get; set; }
        public List<CourseLessonDisplay> lessons { get; set; }

    }

    public class vmAdminTrainingCourseProgress
    {
        public List<TRAINING_SNAPSHOT> course_progress { get; set; }

    }


    public class vmAdminTrainingLessonEdit
    {
        public Guid? COURSE_IDX { get; set; }
        public CourseLessonDisplay lesson { get; set; }
        public List<T_QREST_TRAIN_LESSON_STEP> steps { get; set; }

    }


    public class vmAdminTrainingStepEdit
    {
        public T_QREST_TRAIN_LESSON_STEP step { get; set; }

        [DisplayName("Step Description")]
        [UIHint("wsywigeditor"), AllowHtml]
        [StringLength(12000)]
        public string stepDesc { get; set; }

        [Required]
        public string completeType { get; set; }


    }
}