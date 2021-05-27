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
        public Guid? selMon { get; set; }
        public string selType { get; set; }
        public string selDate { get; set; }
        public string selTimeType { get; set; }
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public IEnumerable<SelectListItem> ddl_Monitor { get; set; }
        public IEnumerable<SelectListItem> ddl_TimeType { get; set; }

        public vmReportsExport(){
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


}