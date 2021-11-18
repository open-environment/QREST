using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using QRESTModel.DAL;

namespace QREST.Models
{
    public class vmHomeSignUp
    {
        public string TestUrl { get; set; }
    }

    public class vmHomeHelp {
        public List<HelpDisplay> HelpTopics { get; set; }
    }


    public class vmHomeTerms {
        public string TermsAndConditions { get; set; }
    }

    public class vmHomeIndex
    {
        public List<SiteDisplay> T_QREST_SITES { get; set; }
    }


    public class vmHomeReportDaily
    {
        public Guid? selSite { get; set; }
        public int selMonth { get; set; }
        public int selDay { get; set; }
        public int selYear { get; set; }
        public string selTime { get; set; }


        public DateTime currServerDateTime { get; set; }

        public List<SP_RPT_DAILY_Result> Results { get; set; }
        public IEnumerable<SelectListItem> ddl_Day { get; set; }
        public IEnumerable<SelectListItem> ddl_Month { get; set; }
        public IEnumerable<SelectListItem> ddl_Year { get; set; }
        public IEnumerable<SelectListItem> ddl_Sites { get; set; }
        public IEnumerable<SelectListItem> ddl_Time { get; set; }

        public vmHomeReportDaily()
        {
            ddl_Day = ddlHelpers.get_ddl_days_in_month(null);
            ddl_Month = ddlHelpers.get_ddl_months();
            ddl_Year = ddlHelpers.get_ddl_years(2019);
            ddl_Sites = ddlHelpers.get_ddl_sites_sampling_public();
            ddl_Time = ddlHelpers.get_ddl_time_type();
        }
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
            ddl_Year = ddlHelpers.get_ddl_years(2019);
            ddl_Sites = ddlHelpers.get_ddl_sites_sampling_public();
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
        public List<SP_RPT_ANNUAL_Result> Results { get; set; }
        public List<SP_RPT_ANNUAL_SUMS_Result> ResultSums { get; set; }
        public IEnumerable<SelectListItem> ddl_Year { get; set; }
        public IEnumerable<SelectListItem> ddl_Sites { get; set; }
        public IEnumerable<SelectListItem> ddl_Mons { get; set; }
        public IEnumerable<SelectListItem> ddl_Time { get; set; }

        public vmHomeReportAnnual()
        {
            ddl_Year = ddlHelpers.get_ddl_years(2019);
            ddl_Sites = ddlHelpers.get_ddl_sites_sampling_public();
            ddl_Time = ddlHelpers.get_ddl_time_type();
        }
    }

    public class vmHomeSendData {
        public string API_KEY { get; set; }
        public string SiteID { get; set; }
        public string ImportTemplate { get; set; }
        public string DataBlock { get; set; }
        public string APIName { get; set; }
    }
}