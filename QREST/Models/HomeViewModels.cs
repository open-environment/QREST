using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using QRESTModel.DAL;

namespace QREST.Models
{
    public class vmHomeIndex
    {
    }

    public class vmHomeHelp {
        public List<T_QREST_HELP_DOCS> HelpTopics { get; set; }
    }


    public class vmHomeTerms {
        public string TermsAndConditions { get; set; }
    }

    public class vmHomeMap
    {
        public List<T_QREST_SITES> T_QREST_SITES { get; set; }
    }

    public class vmHomeReportMonthly
    {
        public Guid? selSite { get; set; }
        public Guid? selMon { get; set; }
        public int selMonth { get; set; }
        public int selYear { get; set; }
        public string selTime { get; set; }
        public string Units { get; set; }
        public List<SP_RPT_MONTHLY_Result> Results { get; set; }
        public List<SP_RPT_MONTHLY_SUMS_Result> ResultSums { get; set; }
        public IEnumerable<SelectListItem> ddl_Month { get; set; }
        public IEnumerable<SelectListItem> ddl_Year { get; set; }
        public IEnumerable<SelectListItem> ddl_Sites { get; set; }
        public IEnumerable<SelectListItem> ddl_Mons { get; set; }
        public IEnumerable<SelectListItem> ddl_Time { get; set; }

        public vmHomeReportMonthly()
        {
            ddl_Month = ddlHelpers.get_ddl_months();
            ddl_Year = ddlHelpers.get_ddl_years();
            ddl_Sites = ddlHelpers.get_ddl_sites_all();
            ddl_Time = ddlHelpers.get_ddl_time_type();
        }
    }

    public class vmHomeReportAnnual
    {
        public Guid? selSite { get; set; }
        public Guid? selMon { get; set; }
        public int selYear { get; set; }
        public string selTime { get; set; }
        public string Units { get; set; }
        public List<SP_RPT_MONTHLY_Result> Results { get; set; }
        public List<SP_RPT_MONTHLY_SUMS_Result> ResultSums { get; set; }
        public IEnumerable<SelectListItem> ddl_Year { get; set; }
        public IEnumerable<SelectListItem> ddl_Sites { get; set; }
        public IEnumerable<SelectListItem> ddl_Mons { get; set; }
        public IEnumerable<SelectListItem> ddl_Time { get; set; }

        public vmHomeReportAnnual()
        {
            ddl_Year = ddlHelpers.get_ddl_years();
            ddl_Sites = ddlHelpers.get_ddl_sites_all();
            ddl_Time = ddlHelpers.get_ddl_time_type();
        }
    }
}