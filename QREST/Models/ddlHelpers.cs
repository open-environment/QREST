using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QRESTModel.DAL;
using QREST.App_Logic.BusinessLogicLayer;

namespace QREST.Models
{
    public static class ddlHelpers
    {
        public static IEnumerable<SelectListItem> get_ddl_action_code()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "I", Text = "Insert" });
            _list.Add(new SelectListItem() { Value = "U", Text = "Update" });
            _list.Add(new SelectListItem() { Value = "D", Text = "Delete" });
            return _list;
        }

        public static IEnumerable<SelectListItem> get_ddl_aqs_format()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "F", Text = "Flat File" });
            _list.Add(new SelectListItem() { Value = "X", Text = "XML" });
            return _list;
        }

        public static IEnumerable<SelectListItem> get_ddl_aqs_trans_type()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "RD", Text = "RD - Raw Data" });
            _list.Add(new SelectListItem() { Value = "QA", Text = "QA - Quality Assurance" });
            return _list;
        }

        /// <summary>
        /// Returns all organizations, optionally filtered for active only
        /// </summary>
        /// <param name="activeInd"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> get_ddl_organizations(bool activeInd, bool selfRegOnly)
        {
            return db_Ref.GetT_QREST_ORGANIZATIONS(activeInd, selfRegOnly).Select(x => new SelectListItem
            {
                Value = x.ORG_ID,
                Text = x.ORG_NAME
            });
        }
        
        public static IEnumerable<SelectListItem> get_ddl_state()
        {
            return db_Ref.GetT_QREST_REF_STATE().Select(x => new SelectListItem
            {
                Value = x.STATE_CD,
                Text = x.STATE_CD + " - " + x.STATE_NAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_county(string StateCd)
        {
            return db_Ref.GetT_QREST_REF_COUNTY_ByState(StateCd).Select(x => new SelectListItem
            {
                Value = x.COUNTY_CD,
                Text = x.COUNTY_CD + " - " + x.COUNTY_NAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_import_type()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "H1", Text = "Hourly: 1-Param, Hours are 24 Columns" });
            _list.Add(new SelectListItem() { Value = "H", Text = "Hourly" });
            _list.Add(new SelectListItem() { Value = "F", Text = "5-Minute" });
            return _list;
        }

        public static IEnumerable<SelectListItem> get_ddl_logger_date()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "yy/MM/dd", Text = "yy/MM/dd" });
            _list.Add(new SelectListItem() { Value = "MM/dd/yyyy", Text = "MM/dd/yyyy" });
            _list.Add(new SelectListItem() { Value = "MMddyyyy", Text = "MMddyyyy" });
            return _list;
        }
        
        public static IEnumerable<SelectListItem> get_ddl_logger_delimiter()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "C", Text = "Comma" });
            _list.Add(new SelectListItem() { Value = "T", Text = "Tab" });
            return _list;
        }

        public static IEnumerable<SelectListItem> get_ddl_logger_duration()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "1", Text = "1 HOUR" });
            //_list.Add(new SelectListItem() { Value = "G", Text = "1 MINUTE" });
            _list.Add(new SelectListItem() { Value = "H", Text = "5 MINUTE" });
            //_list.Add(new SelectListItem() { Value = "I", Text = "10 MINUTE" });
            //_list.Add(new SelectListItem() { Value = "J", Text = "15 MINUTE" });
            return _list;
        }

        public static IEnumerable<SelectListItem> get_ddl_logger_time()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "HH:mm:ss", Text = "HH:mm:ss (eg 22:12:14)" });
            _list.Add(new SelectListItem() { Value = "HH:MM", Text = "HH:MM (eg 22:12)" });
            _list.Add(new SelectListItem() { Value = "hh:MM:ss tt", Text = "HH:MM:ss tt (eg 10:12:14 PM)" });
            return _list;
        }
        
        public static IEnumerable<SelectListItem> get_ddl_logger_type()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "ZENO", Text = "Zeno 3200 - TCP Connection" });
            _list.Add(new SelectListItem() { Value = "SUTRON", Text = "Sutron - TCP Connection" });
            _list.Add(new SelectListItem() { Value = "WEATHER_PWS", Text = "Weather.com Personal Weather Station" });
            return _list;
        }
                
        public static IEnumerable<SelectListItem> get_ddl_region()
        {
            return db_Ref.GetT_QREST_REF_REGION().Select(x => new SelectListItem
            {
                Value = x.EPA_REGION.ToString(),
                Text = x.EPA_REGION_NAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_aqs_agency()
        {
            return db_Ref.GetT_QREST_REF_AQS_AGENCY().Select(x => new SelectListItem
            {
                Value = x.AQS_AGENCY_CODE,
                Text = "(" + x.AQS_AGENCY_CODE + ") " + x.AQS_AGENCY_NAME
            });
        }
        
        public static IEnumerable<SelectListItem> get_ddl_my_organizations(string UserIDX, bool showID)
        {
            return db_Account.GetT_QREST_ORG_USERS_byUSER_IDX(UserIDX, "A").Select(x => new SelectListItem
            {
                Value = x.ORG_ID,
                Text = showID ? x.ORG_ID : x.ORG_NAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_my_organizations_aqs_submission(string UserIDX)
        {
            return db_Account.GetT_QREST_ORG_USERS_AQS_byUSER_IDX(UserIDX).Select(x => new SelectListItem
            {
                Value = x.ORG_ID,
                Text = x.ORG_NAME
            });
        }


        public static IEnumerable<SelectListItem> get_ddl_my_sites(string OrgID, string UserIDX)
        {
            return db_Air.GetT_QREST_SITES_ByUser_OrgID(OrgID, UserIDX).Select(x => new SelectListItem
            {
                Value = x.SITE_IDX.ToString(),
                Text = x.SITE_ID + " " + x.SITE_NAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_my_sites_sampled(string OrgID, string UserIDX)
        {
            return db_Air.GetT_QREST_SITES_Sampling_ByUser_OrgID(OrgID, UserIDX).Select(x => new SelectListItem
            {
                Value = x.SITE_IDX.ToString(),
                Text = x.SITE_ID + " " + x.SITE_NAME
            });
        }


        /// <summary>
        /// Leave OrgID blank to list all monitors the user has access to
        /// </summary>
        /// <param name="OrgID"></param>
        /// <param name="UserIDX"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> get_ddl_my_monitors(string OrgID, string UserIDX, bool dispUnit)
        {
            return db_Air.GetT_QREST_MONITORS_ByUser_OrgID(OrgID, UserIDX).Select(x => new SelectListItem
            {
                Value = x.T_QREST_MONITORS.MONITOR_IDX.ToString(),
                Text = "Site: " + x.SITE_ID + " | Par: " + x.PAR_NAME + " | POC: " + x.T_QREST_MONITORS.POC + (dispUnit == true ? " | " + x.UNIT_DESC : "")
            });
        }

        public static IEnumerable<SelectListItem> get_monitors_by_site(Guid SiteIDX, bool dispUnit, bool dispPOC)
        {
            return db_Air.GetT_QREST_MONITORS_Display_bySiteIDX(SiteIDX).Select(x => new SelectListItem
            {
                Value = x.T_QREST_MONITORS.MONITOR_IDX.ToString(),
                Text = "Par: (" + x.PAR_CODE + ") " + x.PAR_NAME + " | Method: " + x.METHOD_CODE + (dispPOC==true ? " | " + x.T_QREST_MONITORS.POC : "") + (dispUnit == true ? " | " + x.UNIT_DESC : "")
            });
        }

        public static IEnumerable<SelectListItem> get_monitors_sampled_by_site(Guid SiteIDX)
        {
            var yyy = db_Air.GetT_QREST_MONITORS_Display_SampledBySiteIDX(SiteIDX);
            var zzz = yyy.Select(x => new SelectListItem
            {
                Value = x.T_QREST_MONITORS.MONITOR_IDX.ToString(),
                Text = "Par: (" + x.PAR_CODE + ") " + x.PAR_NAME + " | Method: " + x.METHOD_CODE + " | POC: " + x.T_QREST_MONITORS.POC
            });

            return zzz;
        }

        public static IEnumerable<SelectListItem> get_monitors_sampled_by_org(string orgID)
        {
            return db_Air.GetT_QREST_MONITORS_Display_SampledByOrgID(orgID).Select(x => new SelectListItem
            {
                Value = x.T_QREST_MONITORS.MONITOR_IDX.ToString(),
                Text = "Site: " + x.SITE_ID + " | Par: (" + x.PAR_CODE + ") " + x.PAR_NAME + " | POC: " + x.T_QREST_MONITORS.POC
            });
        }

        public static IEnumerable<SelectListItem> get_monitors_sampled_by_user(string userIDX)
        {
            return db_Air.GetT_QREST_MONITORS_Display_SampledByUser(userIDX).Select(x => new SelectListItem
            {
                Value = x.T_QREST_MONITORS.MONITOR_IDX.ToString(),
                Text = "Site: " + x.SITE_ID + " | Par: (" + x.PAR_CODE + ") " + x.PAR_NAME + " | POC: " + x.T_QREST_MONITORS.POC
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_days_in_month(int? month)
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            for (int i=1; i<32; i++)
            {
                _list.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString() });
            }

            return _list;
        }

        public static IEnumerable<SelectListItem> get_ddl_months()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "1", Text = "January" });
            _list.Add(new SelectListItem() { Value = "2", Text = "February" });
            _list.Add(new SelectListItem() { Value = "3", Text = "March" });
            _list.Add(new SelectListItem() { Value = "4", Text = "April" });
            _list.Add(new SelectListItem() { Value = "5", Text = "May" });
            _list.Add(new SelectListItem() { Value = "6", Text = "June" });
            _list.Add(new SelectListItem() { Value = "7", Text = "July" });
            _list.Add(new SelectListItem() { Value = "8", Text = "August" });
            _list.Add(new SelectListItem() { Value = "9", Text = "September" });
            _list.Add(new SelectListItem() { Value = "10", Text = "October" });
            _list.Add(new SelectListItem() { Value = "11", Text = "November" });
            _list.Add(new SelectListItem() { Value = "12", Text = "December" });
            return _list;
        }

        public static IEnumerable<SelectListItem> get_ddl_import_templates(Guid siteIDX)
        {
            return db_Air.GetT_QREST_SITE_POLL_CONFIG_BySiteByType(siteIDX, "NONE").Select(x => new SelectListItem
            {
                Value = x.POLL_CONFIG_IDX.ToString(),
                Text = x.CONFIG_NAME
            });
        }


        public static IEnumerable<SelectListItem> get_ddl_ref_assess_type()
        {
            return db_Ref.GetT_QREST_REF_ASSESS_TYPE().Select(x => new SelectListItem
            {
                Value = x.ASSESSMENT_TYPE,
                Text = x.ASSESSMENT_TYPE
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_ref_coll_freq()
        {
            return db_Ref.GetT_QREST_REF_COLLECT_FREQ().Select(x => new SelectListItem
            {
                Value = x.COLLECT_FREQ_CODE,
                Text = x.COLLECT_FEQ_DESC
            });
        }
        
        public static IEnumerable<SelectListItem> get_ddl_ref_duration()
        {
            return db_Ref.GetT_QREST_REF_DURATION().Select(x => new SelectListItem
            {
                Value = x.DURATION_CODE,
                Text = x.DURATION_DESC
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_ref_qualifier(string type)
        {
            return db_Ref.GetT_QREST_REF_QUALIFIER_ByType(type).Select(x => new SelectListItem
            {
                Value = x.QUAL_CODE,
                Text = x.QUAL_CODE + " - " + x.QUAL_DESC.Truncate(50)
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_ref_units(string parCode)
        {
            return db_Ref.GetT_QREST_REF_UNITS(parCode).Select(x => new SelectListItem
            {
                Value = x.UNIT_CODE,
                Text = x.UNIT_DESC
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_rounding_decimals()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "-2", Text = "-2" });
            _list.Add(new SelectListItem() { Value = "-1", Text = "-1" });
            _list.Add(new SelectListItem() { Value = "0", Text = "0" });
            _list.Add(new SelectListItem() { Value = "1", Text = "1" });
            _list.Add(new SelectListItem() { Value = "2", Text = "2" });
            _list.Add(new SelectListItem() { Value = "3", Text = "3" });
            _list.Add(new SelectListItem() { Value = "4", Text = "4" });
            _list.Add(new SelectListItem() { Value = "5", Text = "5" });
            return _list;
        }

        public static IEnumerable<SelectListItem> get_ddl_sites_all()
        {
            return db_Air.GetT_QREST_SITES_All().Select(x => new SelectListItem
            {
                Value = x.SITE_IDX.ToString(),
                Text = x.ORG_ID + ": " + x.SITE_NAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_sites_sampling()
        {
            return db_Air.GetT_QREST_SITES_Sampling().Select(x => new SelectListItem
            {
                Value = x.SITE_IDX.ToString(),
                Text = x.ORG_ID + ": " + x.SITE_NAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_time_zone()
        {
            return db_Ref.GetT_QREST_REF_TIMEZONE().Select(x => new SelectListItem
            {
                Value = x.TZ_CODE,
                Text = x.TZ_NAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_time_type()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "L", Text = "Local Time" });
            _list.Add(new SelectListItem() { Value = "U", Text = "UTC Time" });
            return _list;
        }

        public static IEnumerable<SelectListItem> get_ddl_sum_type()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "AVG", Text = "Average" });
            _list.Add(new SelectListItem() { Value = "MAX", Text = "Maximum" });
            _list.Add(new SelectListItem() { Value = "MIN", Text = "Minimum" });
            _list.Add(new SelectListItem() { Value = "TOT", Text = "Total (Sum)" });
            _list.Add(new SelectListItem() { Value = "AAVG", Text = "Angular Average" });
            _list.Add(new SelectListItem() { Value = "DEV", Text = "Standard Deviation" });
            _list.Add(new SelectListItem() { Value = "ADEV", Text = "Angular StdDev (Yamartino)" });
            return _list;
        }

        public static IEnumerable<SelectListItem> get_ddl_user_status()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "A", Text = "Active" });
            _list.Add(new SelectListItem() { Value = "P", Text = "Pending" });
            _list.Add(new SelectListItem() { Value = "R", Text = "Rejected" });
            return _list;
        }

        public static IEnumerable<SelectListItem> get_ddl_org_user_role()
        {
            return db_Ref.GetT_QREST_REF_ACCESS_LEVEL().Select(x => new SelectListItem
            {
                Value = x.ACCESS_LEVEL,
                Text = x.ACCESS_LEVEL_DESC
            });
        }
        
        public static IEnumerable<SelectListItem> get_ddl_users(bool ConfirmedOnly)
        {
            return db_Account.GetT_QREST_USERS(ConfirmedOnly).Select(x => new SelectListItem
            {
                Value = x.USER_IDX,
                Text = x.FNAME + " " + x.LNAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_years(int startYr)
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            for (int i = startYr; i <= System.DateTime.Now.Year; i++)
                _list.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString() });
                
            return _list;
        }

        public static IEnumerable<SelectListItem> get_ddl_yes_no()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "Y", Text = "Yes" });
            _list.Add(new SelectListItem() { Value = "N", Text = "No" });
            return _list;
        }

        public static IEnumerable<SelectListItem> get_ddl_NMIN_HOURLY()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "N", Text = "N" });
            _list.Add(new SelectListItem() { Value = "H", Text = "H" });
            return _list;
        }

    }
}