using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using QRESTModel.DAL;

namespace QREST.Models
{


    public class vmDataQCList
    {
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public string selOrgID { get; set; }

        public List<T_QREST_QC_ASSESSMENT> Assessments { get; set; }
    }

    public class vmDataQCEntry
    {
        public IEnumerable<SelectListItem> ddl_Monitor { get; set; }
        public IEnumerable<SelectListItem> ddl_Assess_Type { get; set; }
        public IEnumerable<SelectListItem> ddl_AQS_Null { get; set; }
        public IEnumerable<SelectListItem> ddl_FlowRate_Unit { get; set; }

        public string ORG_ID { get; set; }

        public bool CanEdit { get; set; }

        public Guid? QC_ASSESS_IDX { get; set; }
        [Required]
        public Guid? MONITOR_IDX { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ASSESSMENT_DT { get; set; }

        public string ASSESSMENT_TM { get; set; }

        [Required]
        public string ASSESSMENT_TYPE { get; set; }
        public string UNIT_CODE { get; set; }
        public bool DisplayUnit { get; set; }
        public int? ASSESSMENT_NUM { get; set; }
        public string ASSESSED_BY { get; set; }


        public List<QC_ASSESSMENT_DTLDisplay> AssessmentDetails { get; set; }

        public int? AuditLevelDistinctCount { get; set; }

    }

    public class vmDataRaw
    {
        [Required]
        public string selOrgID { get; set; }
        public string selMon { get; set; }
        [Required]
        public string selType { get; set; }
        [Required]
        public string selTimeType { get; set; }
        [Required]
        public string selDate { get; set; }
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public IEnumerable<SelectListItem> ddl_Monitor { get; set; }
        public IEnumerable<SelectListItem> ddl_TimeType { get; set; }
        public IEnumerable<SelectListItem> ddl_DurationType { get; set; }
        public List<RawDataDisplay> RawData { get; set; }
        public int totHoursDuration { get; set; }

        public vmDataRaw()
        {
            ddl_DurationType = ddlHelpers.get_ddl_logger_duration();
            ddl_TimeType = ddlHelpers.get_ddl_time_type();
            selType = "1";
            selTimeType = "L";
            totHoursDuration = 24;
        }


    }

    public class vmDataImport
    {
        [Required]
        [Display(Name = "Organization")]
        public string selOrgID { get; set; }
        
        [Required]
        [Display(Name = "Site")]
        public Guid selSite { get; set; }

        [Required]
        [Display(Name = "Import Type")]
        public string selImportType { get; set; }
        
        [Display(Name = "Monitor")]
        public Guid? selMonitor { get; set; }

        public Guid? selPollConfig { get; set; }

        public string selTimeType { get; set; }
        public string selCalc { get; set; }
        public string selVal { get; set; }



        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public IEnumerable<SelectListItem> ddl_Sites { get; set; }
        public IEnumerable<SelectListItem> ddl_Monitors { get; set; }
        public IEnumerable<SelectListItem> ddl_ImportType { get; set; }
        public IEnumerable<SelectListItem> ddl_PollConfig { get; set; }
        public IEnumerable<SelectListItem> ddl_Time { get; set; }
        public IEnumerable<SelectListItem> ddl_Calc { get; set; }


        [Required]
        [Display(Name = "Import Data")]
        public string IMPORT_BLOCK { get; set; }  //raw text imported
    }

    public class vmDataImportList {
        public string selOrgID { get; set; }
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }

        public List<ImportListDisplay> T_QREST_DATA_IMPORTS { get; set; }

    }

    public class vmDataImportConfig
    {
        public Guid? SITE_IDX { get; set; }

        //EDITED CONFIG
        public Guid? editPOLL_CONFIG_IDX { get; set; }

        [Required]
        [Display(Name = "Configuration Name")]
        public string editCONFIG_NAME { get; set; }

        public string editCONFIG_DESC { get; set; }


        [Required]
        [Display(Name = "Delimiter")]
        public string editDELIMITER { get; set; }

        [Required]
        [Display(Name = "Timezone")]
        public string LOCAL_TIMEZONE { get; set; }

        [Required]
        [Display(Name = "Time Output Type")]
        public string editTIME_POLL_TYPE { get; set; }

        [Required]
        [Display(Name = "Date Column")]
        public int? editDATE_COL { get; set; }

        [Required]
        [Display(Name = "Date Format")]
        public string editDATE_FORMAT { get; set; }

        [Required]
        [Display(Name = "Time Column")]
        public int? editTIME_COL { get; set; }

        [Required]
        [Display(Name = "Time Format")]
        public string editTIME_FORMAT { get; set; }

        //EDIT COLUMN MAPPING
        public Guid? editPOLL_CONFIG_DTL_IDX { get; set; }
        public int editCOL { get; set; }
        public Guid? editMONITOR_IDX { get; set; }
        public string editSUM_TYPE { get; set; }
        public int? editROUNDING { get; set; }
        public double? editADJUST_FACTOR { get; set; }

        public IEnumerable<SelectListItem> ddl_LoggerDelimiter { get; set; }
        public IEnumerable<SelectListItem> ddl_TimeZone { get; set; }  //read only
        public IEnumerable<SelectListItem> ddl_LoggerTimeType { get; set; }
        public IEnumerable<SelectListItem> ddl_LoggerDate { get; set; }
        public IEnumerable<SelectListItem> ddl_LoggerTime { get; set; }


        public IEnumerable<SelectListItem> ddl_Monitors { get; set; }

        //only needed if importing n-min that requires calculation
        public IEnumerable<SelectListItem> ddl_SumType { get; set; }
        public IEnumerable<SelectListItem> ddl_Rounding { get; set; }


        public vmDataImportConfig()
        {
            ddl_LoggerDate = ddlHelpers.get_ddl_logger_date();
            ddl_LoggerTime = ddlHelpers.get_ddl_logger_time();
            ddl_LoggerDelimiter = ddlHelpers.get_ddl_logger_delimiter();
            ddl_SumType = ddlHelpers.get_ddl_sum_type();
            ddl_TimeZone = ddlHelpers.get_ddl_time_zone();
            ddl_LoggerTimeType = ddlHelpers.get_ddl_time_type();
            ddl_Rounding = ddlHelpers.get_ddl_rounding_decimals();
        }
    }


    public class vmImportStatus {
        public string selOrg { get; set; }
        public T_QREST_DATA_IMPORTS T_QREST_DATA_IMPORTS { get; set; }
        public int ImportTotalCount { get; set; }
        public int ImportValDupCount { get; set; }
        public int ImportValErrorCount { get; set; }
        public int ImportSuccCount { get; set; }
        public List<T_QREST_DATA_IMPORT_TEMP> TempDupRecords { get; set; }
        public double durationSecs { get; set; }
        public List<SP_IMPORT_DETECT_GAPS_Result> ImportGaps { get; set; }
    }


    public class vmDataReviewSummary
    {
        public Guid? selSite { get; set; }
        public int selMonth { get; set; }
        public int selYear { get; set; }
        public DateTime? selsDt { get; set; }  //start date only used for passing values to other pages
        public DateTime? seleDt { get; set; }  //end date only used for passing values to other pages
        public IEnumerable<SelectListItem> ddl_Month { get; set; }
        public IEnumerable<SelectListItem> ddl_Year { get; set; }
        public IEnumerable<SelectListItem> ddl_Sites { get; set; }
        public List<SP_AQS_REVIEW_STATUS_Result> Results { get; set; }
        public int? FiveMinGaps { get; set; }

        public vmDataReviewSummary()
        {
            ddl_Month = ddlHelpers.get_ddl_months();
            ddl_Year = ddlHelpers.get_ddl_years(2009);
        }
    }

    public class vmDataReviewFillDataSummary {
        public int selMonth { get; set; }
        public int selYear { get; set; }
        public Guid selSite { get; set; }

        public SiteMonitorDisplayType monitor  { get; set; }
        public SP_COUNT_LOST_DATA_Result lost_data { get; set; }
    }

    public class vmDataReview
    {
        public string selOrgID { get; set; }
        public Guid? selSiteIDX { get; set; }
        [Required]
        public string selMon { get; set; }
        public DateTime? selDtStart { get; set; }
        public DateTime? selDtEnd { get; set; }
        public int? selDtStartMonth { get; set; }
        public int? selDtStartYear { get; set; }

        [Required]
        public string selDuration { get; set; }
        public List<string> selMonSupp { get; set; }
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public IEnumerable<SelectListItem> ddl_Monitor { get; set; }
        public IEnumerable<SelectListItem> ddl_Duration { get; set; }
    }

    public class vmDataReview2
    {
        public SiteMonitorDisplayType selMon { get; set; }
        public DateTime selDtStart { get; set; }
        public DateTime selDtEnd { get; set; }
        public string selDuration { get; set; }
        public string selMode { get; set; } //set to a for AQS mode (used for return URL)


        public List<RawDataDisplay> RawData { get; set; }


        //supplemental 
        public SiteMonitorDisplayType selSupp1 { get; set; }
        public SiteMonitorDisplayType selSupp2 { get; set; }
        public SiteMonitorDisplayType selSupp3 { get; set; }
        public List<RawDataDisplay> SuppData1 { get; set; }
        public List<RawDataDisplay> SuppData2 { get; set; }
        public List<RawDataDisplay> SuppData3 { get; set; }


        //bulk edit
        public string editNullQual { get; set; }
        public List<string> editQual { get; set; }
        public IEnumerable<SelectListItem> ddl_NullQual { get; set; }
        public IEnumerable<SelectListItem> ddl_Qual { get; set; }
        public IEnumerable<SelectListItem> ddl_ParUnits { get; set; }

        public bool secLvl1Ind { get; set; }  //indicates if user has Lvl1 Validation rights
        public bool secLvl2Ind { get; set; }  //indicates if user has Lvl2 Validation rights
        public bool isReadOnly { get; set; }  //indicates if user is Read Only for site
        public string editLvl1 { get; set; }
        public string editLvl2 { get; set; }
        [Required]
        public string editNotes { get; set; }
        public string editUnitCode { get; set; }
        public string editValue { get; set; }
        public bool editValueBlank { get; set; }
        public string editFlag { get; set; }
        public bool editFlagBlank { get; set; }
        public bool editDeleteRecords { get; set; }

        public List<Guid> editRawDataIDX { get; set; }

        //initialize
        public vmDataReview2()
        {
            editValueBlank = false;
            editFlagBlank = false;
            editDeleteRecords = false;
        }
    }


    public class vmDataDocs
    {
        public Guid? selSite { get; set; }
        public Guid? selMon { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string PAR_CODE { get; set; }
        public string PAR_NAME { get; set; }
        public string SITE_ID { get; set; }
        public bool CanEdit { get; set; }



        public string SiteMonInd { get; set; }

        public Guid? editASSESS_DOC_IDX { get; set; }
        [Required]
        public HttpPostedFileBase fileUpload { get; set; }
        public string fileDescription { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime addStartDt { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime addEndDt { get; set; }



        public List<AssessDocDisplay> SiteDocs { get; set; }
        public List<AssessDocDisplay> MonDocs { get; set; }
    }


    public class vmDataAQSList {
        public string selOrgID { get; set; }
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }

        public List<AQSDisplay> T_QREST_AQS { get; set; }

        public Guid? editID { get; set; }
        public string editSUBMISSION_NAME { get; set; }
        public string editCOMMENT { get; set; }
    }


    public class vmDataAQSGen
    {
        public string selAQSTransType { get; set; }
        public string selOrgID { get; set; }
        public Guid? selSite { get; set; }
        public Guid? selQid { get; set; }
        public IList<Guid> selMons{ get; set; }

        public string selAQSFormat { get; set; }
        public DateTime? selDtStart { get; set; }
        public DateTime? selDtEnd { get; set; }
        public string selActionCode { get; set; }
        public bool passValidation { get; set; }


        public IEnumerable<SelectListItem> ddl_AQSTransType { get; set; }
        public IEnumerable<SelectListItem> ddl_Sites { get; set; }
        public IEnumerable<SelectListItem> ddl_Monitor { get; set; }
        public IEnumerable<SelectListItem> ddl_AQSFormat { get; set; }
        public IEnumerable<SelectListItem> ddl_ActionCode { get; set; }


        //RESULTS
        public List<SP_AQS_REVIEW_STATUS_Result> Results { get; set; }


        //initialize
        public vmDataAQSGen()
        {
            ddl_AQSTransType = ddlHelpers.get_ddl_aqs_trans_type();
            ddl_AQSFormat = ddlHelpers.get_ddl_aqs_format();
            ddl_ActionCode = ddlHelpers.get_ddl_action_code();
            selMons = new List<Guid>();
            ddl_Monitor = new List<SelectListItem>();
        }
    }


    public class vmDataAQSAcct
    {
        public string selOrgID { get; set; }
        public Guid? selSite { get; set; }

        public bool UseGlobalCDXAccount { get; set; }
        public string GlobalCDXUser { get; set; }
        public string CDXUsername { get; set; }
        public string CDXPwd { get; set; }
        public string AQSScreeningGroup { get; set; }
        public string AQSUser { get; set; }

    }

    public class vmDataFillGaps
    {
        public Guid? selSite { get; set; }
        public string selSiteID { get; set; }

        public List<SP_FIVE_MIN_DATA_GAPS_Result> dataGaps { get; set; }

    }

}