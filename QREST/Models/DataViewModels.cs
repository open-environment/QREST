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
        public IEnumerable<SelectListItem> ddl_Assess_Type  { get; set; }

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
        //public List<RawDataDisplay> RawDataDisplay { get; set; }
    }
}