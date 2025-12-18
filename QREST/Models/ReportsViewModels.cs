using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QREST.Models
{
    public class vmReportsExport
    {
        public string selOrgID { get; set; }
        public string selOrgIDAdmin { get; set; }
        public Guid? selMon { get; set; }
        public string selType { get; set; }
        public string selDate { get; set; }
        public string selTimeType { get; set; }
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public IEnumerable<SelectListItem> ddl_Monitor { get; set; }
        public IEnumerable<SelectListItem> ddl_TimeType { get; set; }
        public bool chkDaily { get; set; }
        public bool chkMonthly { get; set; }
        public vmReportsExport(){
            ddl_TimeType = ddlHelpers.get_ddl_time_type();
            chkDaily = true;
            chkMonthly = true;
        }
    }

    public class vmReportsDaily 
    {
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public IEnumerable<SelectListItem> ddl_Monitor { get; set; }
        public IEnumerable<SelectListItem> ddl_TimeType { get; set; }

        public string selOrgID { get; set; }

        [Required]
        public Guid selMon { get; set; }

        [Required]
        public string selDate { get; set; }
        public string selTimeType { get; set; }

        public List<SP_DAILY_AVG_Result> stats { get; set; }

        public vmReportsDaily()
        {
            ddl_TimeType = ddlHelpers.get_ddl_time_type();
        }
    }

    public class vmReportsMonthlyStats
    {
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public IEnumerable<SelectListItem> ddl_Monitor { get; set; }

        public string selOrgID { get; set; }

        [Required]
        public Guid selMon { get; set; }

        [Required]
        [DataType(DataType.DateTime, ErrorMessage = "Date must be in MM/YYYY")]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime selDate { get; set; }

        [Required]
        [DataType(DataType.DateTime, ErrorMessage = "Date must be in MM/YYYY")]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime endDate { get; set; }

        public List<SP_MONTHLY_STATS_Result> stats { get; set; }
    }



    public class vmReportUsageStats
    {
        public IEnumerable<SelectListItem> ddl_Years { get; set; }

        public int? selYear { get; set; }
        public int? HourlyCnt { get; set; }
        public int? FiveMinCnt { get; set; }
        public int? OrgCnt { get; set; }
        public List<GlobalHourlyUsage> HourlyCnts { get; set; }
        public List<GlobalHourlyUsage> FiveMinCnts { get; set; }
        public int? RD_cnt { get; set; }
        public int? RD_sum { get; set; }
        public int? QA_cnt { get; set; }
        public int? QA_sum { get; set; }

    }



    // ******************************** REF DATA************************************
    //****************************************************************************
    //****************************************************************************
    public class vmReportsRefPar
    {
        public string editPAR_CODE { get; set; }
        [Required]
        public string editPAR_NAME { get; set; }
        [Required]
        public string editSTD_UNIT_CODE { get; set; }

        public IEnumerable<SelectListItem> ddl_units { get; set; }

        public vmReportsRefPar()
        {
            ddl_units = ddlHelpers.get_ddl_ref_units(null);
        }
    }


    public class vmReportsRefParMethod
    {
        public Guid? editPAR_METHOD_IDX { get; set; }
        public double? editCUST_MIN_VALUE { get; set; }
        public double? editCUST_MAX_VALUE { get; set; }
        public string editCOLLECTION_DESC { get; set; }

    }


    public class vmReportsRefParUnit
    {
        [Required]
        public string editPAR_CODE { get; set; }

        [Required]
        public string editUNIT_CODE { get; set; }

        public IEnumerable<SelectListItem> ddl_units { get; set; }
        public IEnumerable<SelectListItem> ddl_par { get; set; }

        public vmReportsRefParUnit()
        {
            ddl_units = ddlHelpers.get_ddl_ref_units(null);
            ddl_par = ddlHelpers.get_ddl_ref_par();
        }
    }

}