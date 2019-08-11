using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QREST.Models
{
    public class vmSiteOrgList
    {
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
        public decimal? LATITUDE { get; set; }
        public decimal? LONGITUDE { get; set; }
        public string ADDRESS { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string ZIP_CODE { get; set; }
        public DateTime? START_DT { get; set; }
        public DateTime? END_DT { get; set; }
        public bool TELEMETRY_ONLINE_IND { get; set; }
        public string SITE_COMMENTS { get; set; }
        public string TELEMETRY_SOURCE { get; set; }

        public IEnumerable<SelectListItem> ddl_Organization { get; set; }


        //monitors
        public List<SiteMonitorDisplayType> monitors { get; set; }

    }

    public class vmSiteSiteImport
    {
        public string selOrgID { get; set; }
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public List<T_QREST_SITES> ImportSites { get; set; }
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
        public Guid? PAR_METHOD_IDX { get; set; }
        public string PAR_NAME { get; set; }
        public string METHOD_CODE { get; set; }
        [Required]
        public int? POC { get; set; }
        public string DURATION_CODE { get; set; }
        [Required]
        public string COLLECT_FREQ_CODE { get; set; }
        public string COLLECT_UNIT_CODE { get; set; }
        public double? ALERT_MIN_VALUE { get; set; }
        public double? ALERT_MAX_VALUE { get; set; }
        public int? ALERT_PCT_CHANGE { get; set; }
        public int? ALERT_STUCK_REC_COUNT { get; set; }
        public DateTime? CREATE_DT { get; set; }

        public IEnumerable<SelectListItem> ddl_Ref_Duration { get; set; }
        public IEnumerable<SelectListItem> ddl_Ref_Coll_Freq { get; set; }


        public vmSiteMonitorEdit() {
            ddl_Ref_Duration = ddlHelpers.get_ddl_ref_duration();
            ddl_Ref_Coll_Freq = ddlHelpers.get_ddl_ref_coll_freq();
        }
    }

    public class vmSiteMonitorImport
    {
        public Guid siteIDX { get; set; }
        public string siteName { get; set; }
        public List<T_QREST_MONITORS> ImportMonitors { get; set; }
    }
}