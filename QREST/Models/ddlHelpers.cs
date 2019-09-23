using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QRESTModel.DAL;

namespace QREST.Models
{
    public static class ddlHelpers
    {
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
        
        public static IEnumerable<SelectListItem> get_ddl_my_organizations(string UserIDX)
        {
            return db_Account.GetT_QREST_ORG_USERS_byUSER_IDX(UserIDX, "A").Select(x => new SelectListItem
            {
                Value = x.ORG_ID,
                Text = x.ORG_NAME
            });
        }
        
        public static IEnumerable<SelectListItem> get_ddl_my_monitors(string UserIDX)
        {
            return db_Air.GetT_QREST_MONITORS_ByUser_OrgID(null, UserIDX).Select(x => new SelectListItem
            {
                Value = x.T_QREST_MONITORS.MONITOR_IDX.ToString(),
                Text = x.ORG_ID + " | Site: " + x.SITE_ID + " | Par: " + x.PAR_NAME
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

        public static IEnumerable<SelectListItem> get_ddl_ref_units()
        {
            return db_Ref.GetT_QREST_REF_UNITS().Select(x => new SelectListItem
            {
                Value = x.UNIT_CODE,
                Text = x.UNIT_DESC
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_user_status()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "A", Text = "Active" });
            _list.Add(new SelectListItem() { Value = "P", Text = "Pending" });
            _list.Add(new SelectListItem() { Value = "R", Text = "Rejected" });
            return _list;
        }

        public static IEnumerable<SelectListItem> get_ddl_user_role()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "U", Text = "User" });
            _list.Add(new SelectListItem() { Value = "A", Text = "Admin" });
            _list.Add(new SelectListItem() { Value = "R", Text = "Read Only" });
            return _list;
        }
        
        public static IEnumerable<SelectListItem> get_ddl_users(bool ConfirmedOnly)
        {
            return db_Account.GetT_QREST_USERS(ConfirmedOnly).Select(x => new SelectListItem
            {
                Value = x.USER_IDX,
                Text = x.FNAME + " " + x.LNAME
            });
        }


    }
}