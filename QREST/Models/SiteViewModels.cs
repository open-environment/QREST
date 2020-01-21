using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QRESTModel.COMM;

namespace QREST.Models
{
    public class vmSiteOrgList
    {
    }

    public class vmSiteOrgEdit
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

        public vmSiteOrgEdit()
        {
            ddl_State = ddlHelpers.get_ddl_state();
            ddl_Region = ddlHelpers.get_ddl_region();
            ddl_AqsAgency = ddlHelpers.get_ddl_aqs_agency();
            ddl_user_status = ddlHelpers.get_ddl_user_status();
            ddl_user_role = ddlHelpers.get_ddl_org_user_role();
        }

    }

    public class vmSiteSiteList
    {
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }

        public List<T_QREST_SITES> T_QREST_SITES { get; set; }

        public string selOrgID { get; set; }
    }


    public class vmSiteSiteEdit
    {
        public Guid? SITE_IDX { get; set; }

        [Required]
        public string ORG_ID { get; set; }

        [Required]
        public string SITE_ID { get; set; }

        [Required]
        public string SITE_NAME { get; set; }

        public string AQS_SITE_ID { get; set; }
        public string STATE_CD { get; set; }
        public string COUNTY_CD { get; set; }

        [Range(19, 65)]
        public decimal? LATITUDE { get; set; }

        [Range(-162, -67)]
        public decimal? LONGITUDE { get; set; }
        public string ELEVATION { get; set; }
        public string ADDRESS { get; set; }
        public string CITY { get; set; }
        public string ZIP_CODE { get; set; }
        public DateTime? START_DT { get; set; }
        public DateTime? END_DT { get; set; }
        public bool POLLING_ONLINE_IND { get; set; }
        public bool AIRNOW_IND { get; set; }
        public bool AQS_IND { get; set; }
        public string SITE_COMMENTS { get; set; }


        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public IEnumerable<SelectListItem> ddl_State { get; set; }
        public IEnumerable<SelectListItem> ddl_County { get; set; }
        public IEnumerable<SelectListItem> ddl_User { get; set; } //list of users to possibly notify


        //monitors
        public List<SiteMonitorDisplayType> monitors { get; set; }

        //notifiee list
        public Guid? edit_notify_user_idx { get; set; }
        public List<SiteNotifyDisplay> notifiees { get; set; }

    }

    public class vmSiteEditNotifyUser
    {
        [Required]
        public Guid? edit_notify_user_idx { get; set; }
        [Required]
        public Guid? SITE_IDX { get; set; }
    }

    public class vmSiteSiteImport
    {
        public string selOrgID { get; set; }
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public List<T_QREST_SITES> ImportSites { get; set; }
    }


    public class vmSiteSitePollConfig
    {
        public Guid? SITE_IDX { get; set; }

        //SITE FIELDS
        public string POLLING_FREQ_TYPE { get; set; }
        public string POLLING_FREQ_NUM { get; set; }
        public DateTime? POLLING_LAST_RUN_DT { get; set; }
        public DateTime? POLLING_NEXT_RUN_DT { get; set; }

        //ALL CONFIGS
        public List<T_QREST_SITE_POLL_CONFIG> ConfigList { get; set; }
        
        //EDITED CONFIG
        public Guid? editPOLL_CONFIG_IDX { get; set; }

        [Required]
        [Display(Name = "Configuration Name")]
        public string editCONFIG_NAME { get; set; }
        public string editRAW_DURATION_CODE { get; set; }

        [Required]
        [Display(Name = "Logger Type")]
        public string editLOGGER_TYPE { get; set; }

        [Required]
        [Display(Name = "Logger Source")]
        public string editLOGGER_SOURCE { get; set; }

        [Required]
        [Display(Name = "Port")]
        public int? editLOGGER_PORT { get; set; }
        public string editLOGGER_USERNAME { get; set; }
        public string editLOGGER_PASSWORD { get; set; }
        public string editDELIMITER { get; set; }
        public int? editDATE_COL { get; set; }
        public string editDATE_FORMAT { get; set; }
        public int? editTIME_COL{ get; set; }
        public string editTIME_FORMAT { get; set; }
        public string editLOCAL_TIMEZONE { get; set; }
        public bool editACT_IND { get; set; }

        //EDIT COLUMN MAPPING
        public Guid? editPOLL_CONFIG_DTL_IDX { get; set; }
        public int editCOL { get; set; }
        public Guid? editMONITOR_IDX { get; set; }
        public string editSUM_TYPE { get; set; }
        public int? editROUNDING { get; set; }

        public IEnumerable<SelectListItem> ddl_LoggerDate { get; set; }
        public IEnumerable<SelectListItem> ddl_LoggerTime { get; set; }
        public IEnumerable<SelectListItem> ddl_LoggerType { get; set; }
        public IEnumerable<SelectListItem> ddl_LoggerDelimiter { get; set; }
        public IEnumerable<SelectListItem> ddl_LoggerDuration { get; set; }
        public IEnumerable<SelectListItem> ddl_Monitors { get; set; }
        public IEnumerable<SelectListItem> ddl_SumType { get; set; }
        public IEnumerable<SelectListItem> ddl_TimeZone { get; set; }
        public IEnumerable<SelectListItem> ddl_Rounding { get; set; }


        public vmSiteSitePollConfig()
        {
            ddl_LoggerDate = ddlHelpers.get_ddl_logger_date();
            ddl_LoggerTime = ddlHelpers.get_ddl_logger_time();
            ddl_LoggerType = ddlHelpers.get_ddl_logger_type();
            ddl_LoggerDelimiter = ddlHelpers.get_ddl_logger_delimiter();
            ddl_LoggerDuration = ddlHelpers.get_ddl_logger_duration();
            ddl_SumType = ddlHelpers.get_ddl_sum_type();
            ddl_TimeZone = ddlHelpers.get_ddl_time_zone();
            ddl_Rounding = ddlHelpers.get_ddl_rounding_decimals();
        }
    }


    public class vmSitePing
    {
        public Guid? POLL_CONFIG_IDX { get; set; }
        public List<Tuple<bool, string, string>> pingResults { get; set; }
        public List<CommMessageLog> pingResults2 { get; set; }
        public string loggerData { get; set; }
    }

    public class vmSiteMonitorList
    {
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }

        public string selOrgID { get; set; }
        public List<SiteMonitorDisplayType> monitors { get; set; }
    }

    public class vmSiteMonitorEdit
    {
        public Guid? MONITOR_IDX { get; set; }

        [Required]
        public Guid? SITE_IDX { get; set; }

        [Required]
        [Display(Name = "Parameter Method")]
        public Guid? PAR_METHOD_IDX { get; set; }
        public string PAR_NAME { get; set; }
        public string METHOD_CODE { get; set; }

        [Required]
        public int? POC { get; set; }

        [Required]
        public string DURATION_CODE { get; set; }

        public string COLLECT_FREQ_CODE { get; set; }

        [Required]
        [Display(Name = "Data Collection Unit")]
        public string COLLECT_UNIT_CODE { get; set; }

        public double? ALERT_MIN_VALUE { get; set; }
        public double? ALERT_MAX_VALUE { get; set; }
        public double? ALERT_AMT_CHANGE { get; set; }
        public int? ALERT_STUCK_REC_COUNT { get; set; }
        public string ALERT_MIN_TYPE { get; set; }
        public string ALERT_MAX_TYPE { get; set; }
        public string ALERT_AMT_CHANGE_TYPE { get; set; }
        public string ALERT_STUCK_TYPE { get; set; }

        public DateTime? CREATE_DT { get; set; }

        public IEnumerable<SelectListItem> ddl_Ref_Duration { get; set; }
        public IEnumerable<SelectListItem> ddl_Ref_Coll_Freq { get; set; }
        public IEnumerable<SelectListItem> ddl_Unit { get; set; }
        public IEnumerable<SelectListItem> ddl_NMIN_HOURLY { get; set; }


        public vmSiteMonitorEdit() {
            ddl_Ref_Duration = ddlHelpers.get_ddl_ref_duration();
            ddl_Ref_Coll_Freq = ddlHelpers.get_ddl_ref_coll_freq();
            ddl_NMIN_HOURLY = ddlHelpers.get_ddl_NMIN_HOURLY();
            //ddl_Unit = ddlHelpers.get_ddl_ref_units();
        }
    }

    public class vmSiteMonitorImport
    {
        public Guid siteIDX { get; set; }
        public string siteName { get; set; }
        public List<SiteMonitorDisplayType> ImportMonitors { get; set; }
    }
}