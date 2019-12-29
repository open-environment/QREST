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

        public Guid? QC_ASSESS_IDX { get; set; }
        public Guid? MONITOR_IDX { get; set; }
        public DateTime? ASSESSMENT_DT { get; set; }

        [DisplayFormat(DataFormatString = "{0: hh:mm tt}", ApplyFormatInEditMode = true)]
        public string ASSESSMENT_TM { get; set; }
        public string ASSESSMENT_TYPE { get; set; }
        public string UNIT_CODE { get; set; }
        public int? ASSESSMENT_NUM { get; set; }
        public string ASSESSED_BY { get; set; }

    }

    public class vmDataRaw
    {
        public string selOrgID { get; set; }
        public string selMon { get; set; }
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public IEnumerable<SelectListItem> ddl_Monitor { get; set; }
    }

    public class vmDataImport
    {
        public string selOrgID { get; set; }
        [Required]
        public string selMon { get; set; }
        [Required]
        public string selDuration { get; set; }
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public IEnumerable<SelectListItem> ddl_Monitor { get; set; }
        public IEnumerable<SelectListItem> ddl_Duration { get; set; }
        public string IMPORT_BLOCK { get; set; }  //raw text imported
        public List<RawDataDisplay> raw_data { get; set; }   //in-memory storage of array of projects to import

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

        public vmDataReviewSummary()
        {
            ddl_Month = ddlHelpers.get_ddl_months();
            ddl_Year = ddlHelpers.get_ddl_years();
        }
    }


    public class vmDataReview
    {
        public string selOrgID { get; set; }

        [Required]
        public string selMon { get; set; }
        public DateTime? selDtStart { get; set; }
        public DateTime? selDtEnd { get; set; }

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
        public DateTime? selDtStartSub { get; set; }
        public DateTime? selDtEndSub { get; set; }


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
        public IEnumerable<SelectListItem> ddl_NullQual { get; set; }
        public IEnumerable<SelectListItem> ddl_ParUnits { get; set; }

        public bool secLvl1Ind { get; set; }  //indicates if user has Lvl1 Validation rights
        public bool secLvl2Ind { get; set; }  //indicates if user has Lvl2 Validation rights
        public string editLvl1 { get; set; }
        public string editLvl2 { get; set; }
        [Required]
        public string editNotes { get; set; }
        public string editUnitCode { get; set; }

        //initialize
        public vmDataReview2()
        {
            ddl_NullQual = ddlHelpers.get_ddl_ref_qualifier("NULL");
        }
    }


    public class vmDataDocs
    {
        public Guid? selSite { get; set; }
        public Guid? selMon { get; set; }
        public int selMonth { get; set; }
        public int selYear { get; set; }
        public string PAR_CODE { get; set; }
        public string PAR_NAME { get; set; }
        public string SITE_ID { get; set; }


        public string SiteMonInd { get; set; }

        public Guid? editASSESS_DOC_IDX { get; set; }
        [Required]
        public HttpPostedFileBase fileUpload { get; set; }
        public string fileDescription { get; set; }



        public List<AssessDocDisplay> SiteDocs { get; set; }
        public List<AssessDocDisplay> MonDocs { get; set; }
    }

}