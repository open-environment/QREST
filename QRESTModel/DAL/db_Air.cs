using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using QRESTModel.BLL;
using EntityFramework.BulkInsert;
using EntityFramework.BulkInsert.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Data.Entity;

namespace QRESTModel.DAL
{
    public class SiteDisplay
    {
        public T_QREST_SITES T_QREST_SITES { get; set; }
        public string ORG_NAME { get; set; }
        public List<RawDataDisplay> LatestData { get; set; }
    }

    public class SitePollingConfigType
    {
        public Guid SITE_IDX { get; set; }
        public string SITE_ID { get; set; }
        public string ORG_ID { get; set; }
        public bool? POLLING_ONLINE_IND { get; set; }
        public string POLLING_FREQ_TYPE { get; set; }
        public int? POLLING_FREQ_NUM { get; set; }
        public Guid POLL_CONFIG_IDX { get; set; }
        public string LOGGER_TYPE { get; set; }
        public string RAW_DURATION_CODE { get; set; }
        public string LOGGER_SOURCE { get; set; }
        public int? LOGGER_PORT { get; set; }
        public string LOGGER_USERNAME { get; set; }
        public string LOGGER_PASSWORD { get; set; }
        public string DELIMITER { get; set; }
        public int? DATE_COL { get; set; }
        public string DATE_FORMAT { get; set; }
        public int? TIME_COL { get; set; }
        public string TIME_FORMAT { get; set; }
        public string TIME_POLL_TYPE { get; set; }
        public string LOCAL_TIMEZONE { get; set; }
        public string LOGGER_FILE_NAME { get; set; }
        public DateTime? POLLING_LAST_RUN_DT { get; set; }
        public DateTime? POLLING_NEXT_RUN_DT { get; set; }
        public List<SitePollingConfigDetailType> PollingConfigDetails { get; set; }
        public List<string> NotifyUsers { get; set; }
        public bool? AIRNOW_IND { get; set; }
        public string ConnectivityStatus { get; set; }
    }

    public class SitePollingConfigTypeExtended : SitePollingConfigType
    {
        public SitePollingConfigTypeExtended()
        {

        }
        public string CONFIG_NAME { get; set; }
        //public string LOGGER_USERNAME { get; set; }
        public bool ACT_IND { get; set; }
    }

    public class SitePollingConfigDetailType
    {
        public SitePollingConfigDetailType()
        {

        }
        public Guid POLL_CONFIG_DTL_IDX { get; set; }
        public Guid POLL_CONFIG_IDX { get; set; }
        public Guid MONITOR_IDX { get; set; }
        public string PAR_CODE { get; set; }
        public int? COL { get; set; }
        public string COLLECT_UNIT_CODE { get; set; }
        public double? ALERT_MIN_VALUE { get; set; }
        public double? ALERT_MAX_VALUE { get; set; }
        public string ALERT_MIN_TYPE { get; set; }
        public string ALERT_MAX_TYPE { get; set; }
        public double? ADJUST_FACTOR { get; set; }
    }
    public class SitePollingConfigDetailTypeExtended : SitePollingConfigDetailType
    {
        public SitePollingConfigDetailTypeExtended()
        {

        }
        public string SUM_TYPE { get; set; }
        public int? ROUNDING { get; set; }
        public string ORG_ID { get; set; }
        public string SITE_ID { get; set; }
    }

    public class SiteMonitorDisplayType
    {
        public T_QREST_MONITORS T_QREST_MONITORS { get; set; }
        public string SITE_ID { get; set; }
        public string PAR_CODE { get; set; }
        public string PAR_NAME { get; set; }
        public string METHOD_CODE { get; set; }
        public string COLLECTION_DESC { get; set; }
        public string ORG_ID { get; set; }
        public string UNIT_DESC { get; set; }
        public bool monSelInd { get; set; }
    }

    public class SiteMonitorDisplayTypeSmall
    {
        public Guid MONITOR_IDX { get; set; }
        public int POC { get; set; }
        public string SITE_ID { get; set; }
        public string PAR_CODE { get; set; }
        public string PAR_NAME { get; set; }
        public string ORG_ID { get; set; }
        public string UNIT_DESC { get; set; }
    }

    public class QcAssessmentDisplay
    {
        public Guid? QC_ASSESS_IDX { get; set; }
        public Guid? MONITOR_IDX { get; set; }
        public DateTime? ASSESSMENT_DT { get; set; }
        public string ASSESSMENT_TYPE { get; set; }
        public string ASSESSED_BY { get; set; }
        public string SITE_ID { get; set; }
        public string PAR_NAME { get; set; }
        public string ORG_ID { get; set; }

    }

    public class SiteNotifyDisplay
    {
        public T_QREST_SITE_NOTIFY T_QREST_SITE_NOTIFY { get; set; }
        public string UserName { get; set; }
    }

    public class PollConfigDtlDisplay
    {
        public Guid? POLL_CONFIG_DTL_IDX { get; set; }
        public Guid? POLL_CONFIG_IDX { get; set; }
        public Guid? MONITOR_IDX { get; set; }
        public int? COL { get; set; }
        public string SUM_TYPE { get; set; }
        public int? ROUNDING { get; set; }
        public double? ADJUST_FACTOR { get; set; }
        public string PAR_CODE { get; set; }
        public string PAR_NAME { get; set; }
        public int POC { get; set; }
        public string METHOD_CODE { get; set; }
        public string COLLECT_UNIT_CODE { get; set; }
    }

    public class RawDataDisplay
    {
        public bool EditInd { get; set; }
        public Guid DATA_RAW_IDX { get; set; }
        public Guid MONITOR_IDX { get; set; }
        public string ORG_ID { get; set; }
        public string SITE_ID { get; set; }
        public string AQS_SITE_ID { get; set; }
        public string STATE_CD { get; set; }
        public string COUNTY_CD { get; set; }
        public string PAR_CODE { get; set; }
        public string PAR_NAME { get; set; }
        public string METHOD_CODE { get; set; }
        public int? POC { get; set; }
        public string UNIT_CODE { get; set; }
        public string UNIT_DESC { get; set; }
        public string COLLECTION_DESC { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DATA_DTTM { get; set; }
        public string DATA_VALUE { get; set; }
        public bool? VAL_IND { get; set; }
        public string VAL_CD { get; set; }
        public string AQS_NULL_CODE { get; set; }
        public string AQS_QUAL_CODES { get; set; }
        public bool? LVL1_VAL_IND { get; set; }
        public string LVL1_VAL_USERIDX { get; set; }
        public string LVL1_VAL_USER { get; set; }
        public DateTime? LVL1_VAL_DT { get; set; }
        public bool? LVL2_VAL_IND { get; set; }
        public string LVL2_VAL_USERIDX { get; set; }
        public string LVL2_VAL_USER { get; set; }
        public DateTime? LVL2_VAL_DT { get; set; }
        public string NOTES { get; set; }
        public bool AQSReadyInd { get; set; }

    }

    public class AssessDocDisplay
    {
        public Guid ASSESS_DOC_IDX { get; set; }
        public string DOC_NAME { get; set; }
        public string DOC_TYPE { get; set; }
        public string DOC_FILE_TYPE { get; set; }
        public int? DOC_SIZE { get; set; }
        public string DOC_COMMENT { get; set; }
        public string DOC_AUTHOR { get; set; }
        public DateTime? START_DT { get; set; }
        public DateTime? END_DT { get; set; }

    }


    public class ImportResponse
    {
        public bool SuccInd { get; set; }
        public int? ROW_NUM { get; set; }
        public Guid? DATA_IDX { get; set; }
        public DateTime? DATA_DTTM { get; set; }
        public string DATA_VALUE { get; set; }
        public string ERROR_MSG { get; set; }
    }


    public class T_QREST_DATA_IMPORT_TEMPDisplay : T_QREST_DATA_IMPORT_TEMP
    {
        public T_QREST_DATA_IMPORT_TEMPDisplay()
        {}

        public string PAR_CODE { get; set; }
        public string PAR_NAME { get; set; }
        public string DupOrigValue{ get; set; }
    }


    public class AQSDisplay
    {
        //public T_QREST_AQS T_QREST_AQS { get; set; }

        public Guid AQS_IDX { get; set; }
        public string COMMENT { get; set; }
        public string AQS_SUBMISSION_NAME { get; set; }
        public DateTime? CREATE_DT { get; set; }
        public DateTime START_DT { get; set; }
        public DateTime END_DT { get; set; }
        public string SUBMISSION_STATUS { get; set; }
        public string SUBMISSION_SUBSTATUS { get; set; }
        public bool DOWNLOAD_FILE_IND { get; set; }

        public string SITE_ID { get; set; }
        public string SITE_NAME { get; set; }
        public string SUBMITTER { get; set; }
    }

    public class ImportListDisplay
    {
        public T_QREST_DATA_IMPORTS T_QREST_DATA_IMPORTS { get; set; }
        public string SITE_ID { get; set; }
        public string SITE_NAME { get; set; }
        public string SUBMITTER { get; set; }
        public int COUNT { get; set; }
        public int COUNT_FIVE { get; set; }
    }

    public class HourlyLogDisplay
    {
        public DateTime? DATA_DTTM { get; set; }
        public Guid DATA_HOURLY_IDX { get; set; }
        public string NOTES { get; set; }
        public DateTime? MODIFY_DT { get; set; }
        public string USER_NAME { get; set; }
    }

    public class QC_ASSESSMENT_DTLDisplay {
        public Guid QC_ASSESS_DTL_IDX { get; set; }
        public Guid QC_ASSESS_IDX { get; set; }
        public string ASSESSMENT_TYPE { get; set; }
        public double? ASSESS_KNOWN_CONCENTRATION { get; set; }
        public double? MON_CONCENTRATION { get; set; }
        public string COMMENTS { get; set; }
        public decimal? difference { get; set; }
        public double? difference_limit { get; set; }
        public bool? exceed_amt_difference_ind { get; set; }
        public double? pct_limit { get; set; }
        public decimal? pct_difference { get; set; }
        public bool? exceed_pct_difference_ind { get; set; }
        public string PAR_CODE { get; set; }
        public string UNIT_CODE { get; set; }
        public int? audit_level_int { get; set; }
        public string audit_level_disp { get; set; }
        public double? MIN_CONC { get; set; }
        public double? MAX_CONC { get; set; }

    }


    public class QcAssessmentAqsDisplay{
        public T_QREST_QC_ASSESSMENT T_QREST_QC_ASSESSMENT { get; set; }
        public string STATE_CODE { get; set; }
        public string COUNTY_CD { get; set; }
        public string AQS_SITE_ID { get; set; }
        public string PAR_CODE { get; set; }
        public int POC { get; set; }
        public string METHOD_CODE { get; set; }
        public string UNIT_CODE { get; set; }
        public string FLOW_UNIT_CODE { get; set; }
    }

    public static class db_Air
    {

        //*****************SITES**********************************

        /// <summary>
        /// Returns list of sites a user has access to, optionally filtered by OrgID
        /// </summary>
        /// <param name="OrgID"></param>
        /// <param name="userIdx"></param>
        /// <returns></returns>
        public static List<T_QREST_SITES> GetT_QREST_SITES_ByUser_OrgID(string OrgID, string userIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               join u in ctx.T_QREST_ORG_USERS.AsNoTracking() on a.ORG_ID equals u.ORG_ID
                               where u.USER_IDX == userIDX
                               && u.STATUS_IND == "A"
                               && (OrgID != null ? a.ORG_ID == OrgID : true)
                               orderby a.SITE_ID
                               select a).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns a count of the list of sites a user has access to, optionally filtered by org
        /// </summary>
        /// <param name="OrgID"></param>
        /// <param name="userIdx"></param>
        /// <returns></returns>
        public static int? GetT_QREST_SITES_ByUser_OrgID_count(string OrgID, string userIdx)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITES.AsNoTracking()
                            join u in ctx.T_QREST_ORG_USERS.AsNoTracking() on a.ORG_ID equals u.ORG_ID
                            where u.USER_IDX == userIdx
                            && u.STATUS_IND == "A"
                            && (OrgID != null ? a.ORG_ID == OrgID : true)
                            orderby a.SITE_ID
                            select a).Count();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns all sites
        /// </summary>
        /// <returns></returns>
        public static List<T_QREST_SITES> GetT_QREST_SITES_All()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               orderby a.ORG_ID, a.SITE_ID
                               select a).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns all sites that are available for public display
        /// </summary>
        /// <returns></returns>
        public static List<SiteDisplay> GetT_QREST_SITES_All_Display()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               join b in ctx.T_QREST_ORGANIZATIONS.AsNoTracking() on a.ORG_ID equals b.ORG_ID
                               where a.PUB_WEB_IND == true
                               orderby a.ORG_ID, a.SITE_ID
                               select new SiteDisplay
                               {
                                   T_QREST_SITES = a,
                                   ORG_NAME = b.ORG_NAME
                               }).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_QREST_SITES> GetT_QREST_SITES_ByOrgID(string orgId)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               where (orgId != null ? a.ORG_ID == orgId : true)
                               orderby a.SITE_ID
                               select a).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_QREST_SITES GetT_QREST_SITES_ByID(Guid siteIdx)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITES.AsNoTracking()
                            where a.SITE_IDX == siteIdx
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_QREST_SITES GetT_QREST_SITES_ByMonitorID(Guid MonitorIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITES.AsNoTracking()
                            join b in ctx.T_QREST_MONITORS.AsNoTracking() on a.SITE_IDX equals b.SITE_IDX
                            where b.MONITOR_IDX == MonitorIDX
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_QREST_SITES GetT_QREST_SITES_ByOrgandAQSID(string OrgID, string aqsSiteID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               where a.ORG_ID == OrgID
                               && a.AQS_SITE_ID == aqsSiteID
                               select a).FirstOrDefault();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int GetT_QREST_SITES_ByOrgandSiteID(string OrgID, string SiteID, Guid SiteIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               where a.ORG_ID == OrgID
                               && a.SITE_ID == SiteID
                               && a.SITE_IDX != SiteIDX
                               select a).Count();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static T_QREST_SITES GetT_QREST_SITES_ByOrgandSiteID(string OrgID, string SiteID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               where a.ORG_ID == OrgID
                               && a.SITE_ID == SiteID
                               select a).FirstOrDefault();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_QREST_SITES> GetT_QREST_SITES_ReadyToPoll()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITES.AsNoTracking()
                            where a.POLLING_ONLINE_IND == true
                            && (a.POLLING_NEXT_RUN_DT < System.DateTime.Now || a.POLLING_NEXT_RUN_DT == null)
                            orderby a.SITE_ID
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_QREST_SITES> GetT_QREST_SITES_ReadyToAirNow()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITES.AsNoTracking()
                            where a.AIRNOW_IND == true
                            && a.AIRNOW_USR != null
                            && a.AIRNOW_PWD != null
                            && a.AIRNOW_SITE != null 
                            && a.AIRNOW_ORG != null
                            orderby a.SITE_ID
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_QREST_SITES> GetT_QREST_SITES_Sampling_Public()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITES.AsNoTracking()
                            join b in ctx.T_QREST_MONITORS.AsNoTracking() on a.SITE_IDX equals b.SITE_IDX
                            join c in ctx.T_QREST_DATA_HOURLY.AsNoTracking() on b.MONITOR_IDX equals c.MONITOR_IDX
                            where a.PUB_WEB_IND == true
                            select a).Distinct().OrderBy(x => x.ORG_ID).ThenBy(x => x.SITE_NAME).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_QREST_SITES> GetT_QREST_SITES_Sampling_ByUser_OrgID(string orgId, string UserIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITES.AsNoTracking()
                            join b in ctx.T_QREST_MONITORS.AsNoTracking() on a.SITE_IDX equals b.SITE_IDX
                            join c in ctx.T_QREST_DATA_HOURLY.AsNoTracking() on b.MONITOR_IDX equals c.MONITOR_IDX
                            join u in ctx.T_QREST_ORG_USERS.AsNoTracking() on a.ORG_ID equals u.ORG_ID
                            where u.USER_IDX == UserIDX
                            && u.STATUS_IND == "A"
                            && (orgId != null ? a.ORG_ID == orgId : true)
                            select a).Distinct().ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }



        public static Guid? InsertUpdatetT_QREST_SITES(Guid? sITE_IDX, string oRG_ID, string sITE_ID, string sITE_NAME, string aQS_SITE_ID, string sTATE, string cOUNTY,
            decimal? lATITUDE, decimal? lONGITUDE, string eLEVATION, string aDDRESS, string cITY, string zIP_CODE, DateTime? sTART_DT, DateTime? eND_DT,
            bool? pOLLING_ONLINE_IND, string pOLLING_FREQ_TYPE, int? pOLLING_FREQ_NUM, DateTime? pOLLING_LAST_RUN_DT, DateTime? pOLLING_NEXT_RUN_DT, bool? aIRNOW_IND, bool? aQS_IND,
            string aIRNOW_USR, string aIRNOW_PWD, string aIRNOW_ORG, string aIRNOW_SITE, string sITE_COMMENTS, string cREATE_USER, string lOCAL_TIME_ZONE, bool? pUB_WEB_IND)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    bool insInd = false;

                    T_QREST_SITES e = (from c in ctx.T_QREST_SITES
                                       where c.SITE_IDX == sITE_IDX
                                       select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_SITES();
                        e.SITE_IDX = Guid.NewGuid();
                        e.ORG_ID = oRG_ID;
                        e.CREATE_DT = System.DateTime.Now;
                        e.CREATE_USER_IDX = cREATE_USER;
                    }
                    else
                    {
                        e.MODIFY_DT = System.DateTime.Now;
                        e.MODIFY_USER_IDX = cREATE_USER;
                    }

                    if (sITE_ID != null) e.SITE_ID = sITE_ID;
                    if (sITE_NAME != null) e.SITE_NAME = sITE_NAME;
                    if (aQS_SITE_ID != null) e.AQS_SITE_ID = aQS_SITE_ID;
                    if (sTATE != null) e.STATE_CD = sTATE;
                    if (sTATE == "") e.STATE_CD = null;
                    if (cOUNTY != null) e.COUNTY_CD = cOUNTY;
                    if (cOUNTY == "") e.COUNTY_CD = null;
                    if (lATITUDE != null) e.LATITUDE = lATITUDE;
                    if (lONGITUDE != null) e.LONGITUDE = lONGITUDE;
                    if (eLEVATION != null) e.ELEVATION = eLEVATION;
                    if (aDDRESS != null) e.ADDRESS = aDDRESS;
                    if (cITY != null) e.CITY = cITY;
                    if (zIP_CODE != null) e.ZIP_CODE = zIP_CODE;
                    if (sTART_DT != null) e.START_DT = sTART_DT;
                    if (eND_DT != null) e.END_DT = eND_DT;

                    if (pOLLING_ONLINE_IND != null) e.POLLING_ONLINE_IND = pOLLING_ONLINE_IND;
                    if (pOLLING_FREQ_TYPE != null) e.POLLING_FREQ_TYPE = pOLLING_FREQ_TYPE;
                    if (pOLLING_FREQ_NUM != null) e.POLLING_FREQ_NUM = pOLLING_FREQ_NUM;
                    if (pOLLING_LAST_RUN_DT != null) e.POLLING_LAST_RUN_DT = pOLLING_LAST_RUN_DT;
                    if (pOLLING_NEXT_RUN_DT != null) e.POLLING_NEXT_RUN_DT = pOLLING_NEXT_RUN_DT;
                    if (aIRNOW_IND != null) e.AIRNOW_IND = aIRNOW_IND;
                    if (aQS_IND != null) e.AQS_IND = aQS_IND;
                    if (aIRNOW_USR != null) e.AIRNOW_USR = aIRNOW_USR;
                    if (aIRNOW_PWD != null) e.AIRNOW_PWD = aIRNOW_PWD;
                    if (aIRNOW_ORG != null) e.AIRNOW_ORG = aIRNOW_ORG;
                    if (aIRNOW_SITE != null) e.AIRNOW_SITE = aIRNOW_SITE;
                    if (sITE_COMMENTS != null) e.SITE_COMMENTS = sITE_COMMENTS;
                    if (lOCAL_TIME_ZONE != null) e.LOCAL_TIMEZONE = lOCAL_TIME_ZONE;
                    if (pUB_WEB_IND != null) e.PUB_WEB_IND = pUB_WEB_IND;

                    if (insInd)
                        ctx.T_QREST_SITES.Add(e);

                    ctx.SaveChanges();
                    return e.SITE_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_QREST_SITES(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    //don't delete if there are sites for the org
                    List<T_QREST_MONITORS> _mon = GetT_QREST_MONITORS_bySiteIDX(id);
                    if (_mon == null || _mon.Count == 0)
                    {
                        //delete site
                        T_QREST_SITES rec = new T_QREST_SITES { SITE_IDX = id };
                        ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                        ctx.SaveChanges();

                        return 1;
                    }
                    else
                        return -1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }

            }
        }



        //*****************SITE NOTIFY**********************************
        public static T_QREST_SITE_NOTIFY GetT_QREST_SITE_NOTIFY_ByID(Guid? SiteNotifyIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_NOTIFY.AsNoTracking()
                            where a.SITE_NOTIFY_IDX == SiteNotifyIDX
                            select a).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SiteNotifyDisplay> GetT_QREST_SITE_NOTIFY_BySiteID(Guid? SiteIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_NOTIFY.AsNoTracking()
                            join u in ctx.T_QREST_USERS on a.NOTIFY_USER_IDX equals u.USER_IDX
                            where a.SITE_IDX == SiteIDX
                            select new SiteNotifyDisplay
                            {
                                T_QREST_SITE_NOTIFY = a,
                                UserName = u.FNAME + " " + u.LNAME
                            }).ToList();

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? InsertUpdatetT_QREST_SITE_NOTIFY(Guid? sITE_NOTIFY_IDX, Guid? sITE_IDX, string nOTIFY_USER_IDX, string cREATE_USER)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    bool insInd = false;

                    T_QREST_SITE_NOTIFY e = (from c in ctx.T_QREST_SITE_NOTIFY
                                             where c.SITE_NOTIFY_IDX == sITE_NOTIFY_IDX
                                             select c).FirstOrDefault();

                    //then find by site/user combo
                    if (e == null)
                        e = (from c in ctx.T_QREST_SITE_NOTIFY
                             where c.SITE_IDX == sITE_IDX
                             && c.NOTIFY_USER_IDX == nOTIFY_USER_IDX
                             select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_SITE_NOTIFY();
                        e.SITE_NOTIFY_IDX = Guid.NewGuid();
                    }

                    if (sITE_IDX != null) e.SITE_IDX = sITE_IDX.ConvertOrDefault<Guid>();
                    if (nOTIFY_USER_IDX != null) e.NOTIFY_USER_IDX = nOTIFY_USER_IDX;
                    if (cREATE_USER != null) e.MODIFY_USER_IDX = cREATE_USER;
                    e.MODIFY_DT = System.DateTime.Now;

                    if (insInd)
                        ctx.T_QREST_SITE_NOTIFY.Add(e);

                    ctx.SaveChanges();
                    return e.SITE_NOTIFY_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_QREST_SITE_NOTIFY(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_SITE_NOTIFY rec = new T_QREST_SITE_NOTIFY { SITE_NOTIFY_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }



        //*****************SITE POLL CONFIG**********************************
        public static T_QREST_SITE_POLL_CONFIG GetT_QREST_SITE_POLL_CONFIG_ActiveByID(Guid SiteIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking()
                               where a.SITE_IDX == SiteIDX
                               && a.ACT_IND == true
                               select a);

                    return (xxx.Count() == 1) ? xxx.FirstOrDefault() : null;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_QREST_SITE_POLL_CONFIG GetT_QREST_SITE_POLL_CONFIG_ByID(Guid PollConfigIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking()
                            where a.POLL_CONFIG_IDX == PollConfigIDX
                            select a).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_QREST_SITE_POLL_CONFIG GetT_QREST_SITE_POLL_CONFIG_ByNameAndSite(Guid SiteIDX, string ConfigName)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking()
                            where a.SITE_IDX == SiteIDX
                            && a.CONFIG_NAME == ConfigName
                            select a).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_QREST_SITE_POLL_CONFIG GetT_QREST_SITE_POLL_CONFIG_H1_BySite(Guid SiteIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking()
                            where a.SITE_IDX == SiteIDX
                            && a.LOGGER_TYPE == "H1"
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_QREST_SITE_POLL_CONFIG> GetT_QREST_SITE_POLL_CONFIG_BySite(Guid SiteIDX, bool OnlyActInd, bool OnlyPolling)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking()
                            where a.SITE_IDX == SiteIDX
                            && (OnlyPolling ? a.LOGGER_TYPE != "H1" : true)
                            && (OnlyPolling ? a.LOGGER_TYPE != "NONE" : true)
                            && (OnlyActInd ? a.ACT_IND == true : true)
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_QREST_SITE_POLL_CONFIG> GetT_QREST_SITE_POLL_CONFIG_BySiteByType(Guid SiteIDX, string lOGGER_TYPE)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking()
                            where a.SITE_IDX == SiteIDX
                            && a.LOGGER_TYPE == lOGGER_TYPE
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SitePollingConfigType> GetT_QREST_SITES_POLLING_CONFIG_ReadyToPoll()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               join b in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking() on a.SITE_IDX equals b.SITE_IDX
                               where a.POLLING_ONLINE_IND == true
                               && b.ACT_IND == true
                               && b.DATE_COL != null
                               && b.TIME_COL != null
                               && b.DELIMITER != ""
                               && (a.POLLING_NEXT_RUN_DT < System.DateTime.Now || a.POLLING_NEXT_RUN_DT == null)
                               orderby a.SITE_ID
                               select new SitePollingConfigType
                               {
                                   SITE_IDX = a.SITE_IDX,
                                   SITE_ID = a.SITE_ID,
                                   ORG_ID = a.ORG_ID,
                                   POLLING_FREQ_TYPE = a.POLLING_FREQ_TYPE,
                                   POLLING_FREQ_NUM = a.POLLING_FREQ_NUM,
                                   POLL_CONFIG_IDX = b.POLL_CONFIG_IDX,
                                   LOGGER_TYPE = b.LOGGER_TYPE,
                                   RAW_DURATION_CODE = b.RAW_DURATION_CODE,
                                   LOGGER_SOURCE = b.LOGGER_SOURCE,
                                   LOGGER_PORT = b.LOGGER_PORT,
                                   LOGGER_USERNAME = b.LOGGER_USERNAME,
                                   LOGGER_PASSWORD = b.LOGGER_PASSWORD,
                                   DELIMITER = b.DELIMITER,
                                   DATE_COL = b.DATE_COL,
                                   DATE_FORMAT = b.DATE_FORMAT,
                                   TIME_COL = b.TIME_COL,
                                   TIME_FORMAT = b.TIME_FORMAT,
                                   LOCAL_TIMEZONE = a.LOCAL_TIMEZONE,
                                   TIME_POLL_TYPE = b.TIME_POLL_TYPE,
                                   LOGGER_FILE_NAME = b.LOGGER_FILE_NAME
                               }).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SitePollingConfigType> GetT_QREST_SITES_POLLING_CONFIG_CompleteList()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               join b in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking() on a.SITE_IDX equals b.SITE_IDX
                               where 
                               b.ACT_IND == true
                               orderby a.ORG_ID, a.SITE_ID
                               select new SitePollingConfigType
                               {
                                   SITE_IDX = a.SITE_IDX,
                                   SITE_ID = a.SITE_ID,
                                   ORG_ID = a.ORG_ID,
                                   POLLING_ONLINE_IND = a.POLLING_ONLINE_IND,
                                   AIRNOW_IND = a.AIRNOW_IND,
                                   POLLING_FREQ_TYPE = a.POLLING_FREQ_TYPE,
                                   POLLING_FREQ_NUM = a.POLLING_FREQ_NUM,
                                   POLL_CONFIG_IDX = b.POLL_CONFIG_IDX,
                                   LOGGER_TYPE = b.LOGGER_TYPE,
                                   RAW_DURATION_CODE = b.RAW_DURATION_CODE,
                                   LOGGER_SOURCE = b.LOGGER_SOURCE,
                                   LOGGER_PORT = b.LOGGER_PORT,
                                   LOGGER_PASSWORD = b.LOGGER_PASSWORD,
                                   DELIMITER = b.DELIMITER,
                                   DATE_COL = b.DATE_COL,
                                   DATE_FORMAT = b.DATE_FORMAT,
                                   TIME_COL = b.TIME_COL,
                                   TIME_FORMAT = b.TIME_FORMAT,
                                   POLLING_LAST_RUN_DT = a.POLLING_LAST_RUN_DT,
                                   POLLING_NEXT_RUN_DT = a.POLLING_NEXT_RUN_DT,
                                   PollingConfigDetails = (from a1 in ctx.T_QREST_SITE_POLL_CONFIG_DTL
                                                           join b1 in ctx.T_QREST_MONITORS on a1.MONITOR_IDX equals b1.MONITOR_IDX
                                                           join c1 in ctx.T_QREST_REF_PAR_METHODS on b1.PAR_METHOD_IDX equals c1.PAR_METHOD_IDX
                                                           where a1.POLL_CONFIG_IDX == b.POLL_CONFIG_IDX
                                                           orderby a1.COL
                                                           select new SitePollingConfigDetailType
                                                           {
                                                               POLL_CONFIG_DTL_IDX = a1.POLL_CONFIG_DTL_IDX,
                                                               POLL_CONFIG_IDX = a1.POLL_CONFIG_IDX,
                                                               PAR_CODE = c1.PAR_CODE,
                                                               MONITOR_IDX = a1.MONITOR_IDX,
                                                               COL = a1.COL,
                                                               COLLECT_UNIT_CODE = b1.COLLECT_UNIT_CODE,
                                                               ALERT_MIN_TYPE = b1.ALERT_MIN_TYPE,
                                                               ALERT_MIN_VALUE = b1.ALERT_MIN_VALUE,
                                                               ALERT_MAX_TYPE = b1.ALERT_MAX_TYPE,
                                                               ALERT_MAX_VALUE = b1.ALERT_MAX_VALUE
                                                           }).ToList(),
                                   NotifyUsers = (from a2 in ctx.T_QREST_SITE_NOTIFY
                                                  join b2 in ctx.T_QREST_USERS on a2.NOTIFY_USER_IDX equals b2.USER_IDX
                                                  where a2.SITE_IDX == a.SITE_IDX
                                                  select b2.FNAME + " " + b2.LNAME).ToList()
                               }).ToList();

                    
                    xxx.Where(w => w.DELIMITER == "").ToList().ForEach(i => i.ConnectivityStatus = "No delimiter");
                    xxx.Where(w => w.DATE_COL == null).ToList().ForEach(i => i.ConnectivityStatus = "No date column");
                    xxx.Where(w => w.TIME_COL == null).ToList().ForEach(i => i.ConnectivityStatus = "No time column");

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static SitePollingConfigType GetT_QREST_SITES_POLLING_CONFIG_Single(Guid? ConfigIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               join b in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking() on a.SITE_IDX equals b.SITE_IDX
                               where b.POLL_CONFIG_IDX == ConfigIDX
                               select new SitePollingConfigType
                               {
                                   SITE_IDX = a.SITE_IDX,
                                   SITE_ID = a.SITE_ID,
                                   ORG_ID = a.ORG_ID,
                                   POLLING_FREQ_TYPE = a.POLLING_FREQ_TYPE,
                                   POLLING_FREQ_NUM = a.POLLING_FREQ_NUM,
                                   POLL_CONFIG_IDX = b.POLL_CONFIG_IDX,
                                   LOGGER_TYPE = b.LOGGER_TYPE,
                                   RAW_DURATION_CODE = b.RAW_DURATION_CODE,
                                   LOGGER_SOURCE = b.LOGGER_SOURCE,
                                   LOGGER_PORT = b.LOGGER_PORT,
                                   LOGGER_PASSWORD = b.LOGGER_PASSWORD,
                                   DELIMITER = b.DELIMITER,
                                   DATE_COL = b.DATE_COL,
                                   DATE_FORMAT = b.DATE_FORMAT,
                                   TIME_COL = b.TIME_COL,
                                   TIME_FORMAT = b.TIME_FORMAT,
                                   LOCAL_TIMEZONE = a.LOCAL_TIMEZONE,
                                   TIME_POLL_TYPE = b.TIME_POLL_TYPE
                               }).FirstOrDefault();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static string GetT_QREST_SITE_POLL_CONFIG_org_ByID(Guid PollConfigIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking()
                            join c in ctx.T_QREST_SITES.AsNoTracking() on a.SITE_IDX equals c.SITE_IDX
                            where a.POLL_CONFIG_IDX == PollConfigIDX
                            select c).FirstOrDefault()?.ORG_ID;

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static string GetT_QREST_SITE_POLL_CONFIG_SiteID_ByID(Guid PollConfigIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking()
                            join c in ctx.T_QREST_SITES.AsNoTracking() on a.SITE_IDX equals c.SITE_IDX
                            where a.POLL_CONFIG_IDX == PollConfigIDX
                            select c).FirstOrDefault()?.SITE_ID;

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? InsertUpdatetT_QREST_SITE_POLL_CONFIG(Guid? pOLL_CONFIG_IDX, Guid? sITE_IDX, string cONFIG_NAME, string rAW_DURATION_CODE, string lOGGER_TYPE, string lOGGER_SOURCE,
            int? lOGGER_PORT, string lOGGER_USERNAME, string lOGGER_PASSWORD, string dELIMITER, int? dATE_COL, string dATE_FORMAT, int? tIME_COL, string tIME_FORMAT, string lOCAL_TIMEZONE,
            bool aCT_IND, string cREATE_USER, string sITE_NAME, string tIME_POLL_TYPE, bool logChange = true, string logChangeCustomDesc = null, string cONFIG_DESC = null, string lOG_FILE_NAME = null)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    bool insInd = false;

                    T_QREST_SITE_POLL_CONFIG e = (from c in ctx.T_QREST_SITE_POLL_CONFIG
                                                  where c.POLL_CONFIG_IDX == pOLL_CONFIG_IDX
                                                  select c).FirstOrDefault();

                    //if H1, then grab by type and site (can only be 1)
                    if (lOGGER_TYPE == "H1" && e == null)
                    {
                        e = (from c in ctx.T_QREST_SITE_POLL_CONFIG
                             where c.SITE_IDX == sITE_IDX
                             && c.LOGGER_TYPE == "H1"
                             select c).FirstOrDefault();
                    }


                    if (e == null)
                    {
                        //************** INSERT CASE ONLY **********************
                        insInd = true;
                        e = new T_QREST_SITE_POLL_CONFIG();
                        e.POLL_CONFIG_IDX = Guid.NewGuid();
                        e.CREATE_DT = System.DateTime.Now;
                        e.CREATE_USER_IDX = cREATE_USER;
                    }
                    else
                    {
                        //************** UPDATE CASE ONLY **********************
                        e.MODIFY_USER_IDX = cREATE_USER;
                        e.MODIFY_DT = System.DateTime.Now;


                        //*************** LOG CHANGES TO POLLING CONFIG **********************************
                        if (logChange != false)
                        {
                            string logMsg = "Changed polling config [" + e.CONFIG_NAME + "] for " + sITE_NAME + ":";

                            if (e.ACT_IND == true && aCT_IND == false)
                                logMsg += Environment.NewLine + "Took polling config offline.";
                            else if (e.ACT_IND == false && aCT_IND == true)
                                logMsg += Environment.NewLine + "Put polling config online.";

                            if (logChangeCustomDesc != null)
                                logMsg += Environment.NewLine + logChangeCustomDesc;

                            if (cONFIG_NAME != null && cONFIG_NAME != e.CONFIG_NAME) logMsg += Environment.NewLine + "Config name changed from [" + e.CONFIG_NAME + "] to [" + cONFIG_NAME + "]";
                            if (rAW_DURATION_CODE != null && rAW_DURATION_CODE != e.RAW_DURATION_CODE) logMsg += Environment.NewLine + "Duration changed from [" + e.RAW_DURATION_CODE + "] to [" + rAW_DURATION_CODE + "]";
                            if (lOGGER_TYPE != null && lOGGER_TYPE != e.LOGGER_TYPE) logMsg += Environment.NewLine + "Logger Type changed from [" + e.LOGGER_TYPE + "] to [" + lOGGER_TYPE + "]";
                            if (lOGGER_SOURCE != null && lOGGER_SOURCE != e.LOGGER_SOURCE) logMsg += Environment.NewLine + "Logger Source changed from [" + e.LOGGER_SOURCE + "] to [" + lOGGER_SOURCE + "]";
                            if (lOGGER_PORT != null && lOGGER_PORT != e.LOGGER_PORT) logMsg += Environment.NewLine + "Logger Port changed from [" + e.LOGGER_PORT + "] to [" + lOGGER_PORT + "]";
                            if (lOGGER_USERNAME != null && lOGGER_USERNAME != e.LOGGER_USERNAME) logMsg += Environment.NewLine + "Logger Username changed from [" + e.LOGGER_USERNAME + "] to [" + lOGGER_USERNAME + "]";
                            if (lOGGER_PASSWORD != null && lOGGER_PASSWORD != e.LOGGER_PASSWORD) logMsg += Environment.NewLine + "Logger Password changed";
                            if (dELIMITER != null && dELIMITER != e.DELIMITER) logMsg += Environment.NewLine + "Delimiter changed from [" + e.DELIMITER + "] to [" + dELIMITER + "]";
                            if (lOG_FILE_NAME != null && lOG_FILE_NAME != e.LOGGER_FILE_NAME) logMsg += Environment.NewLine + "Log file name changed from [" + e.LOGGER_FILE_NAME + "] to [" + lOG_FILE_NAME + "]";
                            //if (lOCAL_TIMEZONE != null && lOCAL_TIMEZONE != e.LOCAL_TIMEZONE) logMsg += Environment.NewLine + "Time Zone changed from [" + e.LOCAL_TIMEZONE + "] to [" + lOCAL_TIMEZONE + "]";

                            db_Ref.CreateT_QREST_SYS_LOG_ACTIVITY("POLLING CONFIG", cREATE_USER, null, logMsg, null, e.POLL_CONFIG_IDX.ToString());
                        }
                    }

                    if (sITE_IDX != null) e.SITE_IDX = sITE_IDX.ConvertOrDefault<Guid>();
                    if (cONFIG_NAME != null) e.CONFIG_NAME = cONFIG_NAME;
                    if (cONFIG_DESC != null) e.CONFIG_DESC = cONFIG_DESC;
                    if (rAW_DURATION_CODE != null) e.RAW_DURATION_CODE = rAW_DURATION_CODE;
                    if (lOGGER_TYPE != null) e.LOGGER_TYPE = lOGGER_TYPE;
                    if (lOGGER_SOURCE != null) e.LOGGER_SOURCE = lOGGER_SOURCE;
                    if (lOGGER_PORT != null) e.LOGGER_PORT = lOGGER_PORT;
                    if (lOGGER_USERNAME != null) e.LOGGER_USERNAME = lOGGER_USERNAME;
                    if (lOGGER_PASSWORD != null) e.LOGGER_PASSWORD = lOGGER_PASSWORD;
                    if (dELIMITER != null) e.DELIMITER = dELIMITER;
                    if (dATE_COL != null) e.DATE_COL = dATE_COL;
                    if (dATE_FORMAT != null) e.DATE_FORMAT = dATE_FORMAT;
                    if (tIME_COL != null) e.TIME_COL = tIME_COL;
                    if (tIME_FORMAT != null) e.TIME_FORMAT = tIME_FORMAT;
                    //if (lOCAL_TIMEZONE != null) e.LOCAL_TIMEZONE = lOCAL_TIMEZONE;
                    if (tIME_POLL_TYPE != null) e.TIME_POLL_TYPE = tIME_POLL_TYPE;
                    if (lOG_FILE_NAME != null) e.LOGGER_FILE_NAME = lOG_FILE_NAME;

                    if (cREATE_USER != null) e.MODIFY_USER_IDX = cREATE_USER;
                    e.ACT_IND = aCT_IND;

                    if (insInd)
                        ctx.T_QREST_SITE_POLL_CONFIG.Add(e);

                    ctx.SaveChanges();
                    
                    return e.POLL_CONFIG_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? UpdatetT_QREST_SITE_POLL_CONFIG_SetInactive(Guid? pOLL_CONFIG_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_SITE_POLL_CONFIG e = (from c in ctx.T_QREST_SITE_POLL_CONFIG
                                                  where c.POLL_CONFIG_IDX == pOLL_CONFIG_IDX
                                                  select c).FirstOrDefault();

                    e.ACT_IND = false;
                    ctx.SaveChanges();
                    return e.POLL_CONFIG_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_QREST_SITE_POLL_CONFIG(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_SITE_POLL_CONFIG rec = new T_QREST_SITE_POLL_CONFIG { POLL_CONFIG_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static List<SitePollingConfigTypeExtended> GetT_QREST_SITES_POLLING_CONFIG_List(string USER_IDX, string orgID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               join b in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking() on a.SITE_IDX equals b.SITE_IDX
                               join c in ctx.T_QREST_ORG_USERS.AsNoTracking() on a.ORG_ID equals c.ORG_ID
                               where c.USER_IDX == USER_IDX
                               && (orgID != null ? a.ORG_ID == orgID : true)
                               orderby a.ORG_ID, a.SITE_ID
                               select new SitePollingConfigTypeExtended
                               {
                                   SITE_IDX = a.SITE_IDX,
                                   SITE_ID = a.SITE_ID,
                                   ORG_ID = a.ORG_ID,
                                   CONFIG_NAME = b.CONFIG_NAME,
                                   POLLING_ONLINE_IND = a.POLLING_ONLINE_IND,
                                   POLLING_FREQ_TYPE = a.POLLING_FREQ_TYPE,
                                   POLLING_FREQ_NUM = a.POLLING_FREQ_NUM,
                                   POLL_CONFIG_IDX = b.POLL_CONFIG_IDX,
                                   LOGGER_TYPE = b.LOGGER_TYPE,
                                   RAW_DURATION_CODE = b.RAW_DURATION_CODE,
                                   LOGGER_SOURCE = b.LOGGER_SOURCE,
                                   LOGGER_PORT = b.LOGGER_PORT,
                                   LOGGER_PASSWORD = b.LOGGER_PASSWORD,
                                   DELIMITER = b.DELIMITER,
                                   DATE_COL = b.DATE_COL,
                                   DATE_FORMAT = b.DATE_FORMAT,
                                   TIME_COL = b.TIME_COL,
                                   TIME_FORMAT = b.TIME_FORMAT,
                                   POLLING_LAST_RUN_DT = a.POLLING_LAST_RUN_DT,
                                   POLLING_NEXT_RUN_DT = a.POLLING_NEXT_RUN_DT
                               }).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }



        //*****************SITE POLL CONFIG_DTL**********************************
        public static T_QREST_SITE_POLL_CONFIG_DTL GetT_QREST_SITE_POLL_CONFIG_DTL_ByDTLID(Guid PollConfigDtlIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_POLL_CONFIG_DTL.AsNoTracking()
                            where a.POLL_CONFIG_DTL_IDX == PollConfigDtlIDX
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SitePollingConfigDetailType> GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(Guid PollConfigIDX, bool OnlyWithCols)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_POLL_CONFIG_DTL.AsNoTracking()
                            join b in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals b.MONITOR_IDX
                            join c in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on b.PAR_METHOD_IDX equals c.PAR_METHOD_IDX
                            where a.POLL_CONFIG_IDX == PollConfigIDX
                            && (OnlyWithCols == true ? a.COL != null : true)
                            select new SitePollingConfigDetailType
                            {
                                POLL_CONFIG_DTL_IDX = a.POLL_CONFIG_DTL_IDX,
                                POLL_CONFIG_IDX = a.POLL_CONFIG_IDX,
                                MONITOR_IDX = a.MONITOR_IDX,
                                COL = a.COL,
                                COLLECT_UNIT_CODE = b.COLLECT_UNIT_CODE,
                                ALERT_MIN_TYPE = b.ALERT_MIN_TYPE,
                                ALERT_MIN_VALUE = b.ALERT_MIN_VALUE,
                                ALERT_MAX_TYPE = b.ALERT_MAX_TYPE,
                                ALERT_MAX_VALUE = b.ALERT_MAX_VALUE,
                                ADJUST_FACTOR = a.ADJUST_FACTOR ?? 1,
                                PAR_CODE = c.PAR_CODE
                            }).ToList();

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<PollConfigDtlDisplay> GetT_QREST_SITE_POLL_CONFIG_DTL_ByID(Guid PollConfigIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_POLL_CONFIG_DTL.AsNoTracking()
                            join b in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals b.MONITOR_IDX
                            join c in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on b.PAR_METHOD_IDX equals c.PAR_METHOD_IDX
                            join d in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on c.PAR_CODE equals d.PAR_CODE
                            where a.POLL_CONFIG_IDX == PollConfigIDX
                            orderby a.COL
                            select new PollConfigDtlDisplay
                            {
                                POLL_CONFIG_DTL_IDX = a.POLL_CONFIG_DTL_IDX,
                                POLL_CONFIG_IDX = a.POLL_CONFIG_IDX,
                                COL = a.COL,
                                SUM_TYPE = a.SUM_TYPE,
                                ROUNDING = a.ROUNDING,
                                COLLECT_UNIT_CODE = b.COLLECT_UNIT_CODE,
                                ADJUST_FACTOR = a.ADJUST_FACTOR,
                                MONITOR_IDX = a.MONITOR_IDX,
                                PAR_CODE = c.PAR_CODE,
                                PAR_NAME = d.PAR_NAME,
                                POC = b.POC,
                                METHOD_CODE = c.METHOD_CODE
                            }).ToList();

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? InsertUpdatetT_QREST_SITE_POLL_CONFIG_DTL(Guid? pOLL_CONFIG_DTL_IDX, Guid? pOLL_CONFIG_IDX, Guid? mONITOR_IDX, int? cOL, string sUM_TYPE, int? rOUNDING, double? aDJUST_FACTOR, string cREATE_USER_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    bool insInd = false;

                    T_QREST_SITE_POLL_CONFIG_DTL e = (from c in ctx.T_QREST_SITE_POLL_CONFIG_DTL
                                                      where c.POLL_CONFIG_DTL_IDX == pOLL_CONFIG_DTL_IDX
                                                      select c).FirstOrDefault();

                    if (e == null)
                        e = (from c in ctx.T_QREST_SITE_POLL_CONFIG_DTL
                             where c.POLL_CONFIG_IDX == pOLL_CONFIG_IDX
                             && c.MONITOR_IDX == mONITOR_IDX
                             && c.COL == cOL
                             select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_SITE_POLL_CONFIG_DTL();
                        e.POLL_CONFIG_DTL_IDX = Guid.NewGuid();

                        SiteMonitorDisplayType _mon = db_Air.GetT_QREST_MONITORS_ByID(mONITOR_IDX ?? Guid.Empty);
                        if (_mon != null)
                            db_Ref.CreateT_QREST_SYS_LOG_ACTIVITY("POLLING CONFIG", cREATE_USER_IDX, null, "Column mapping added [" + _mon.PAR_CODE + "]", null, pOLL_CONFIG_IDX.ToString());
                    }

                    if (pOLL_CONFIG_IDX != null) e.POLL_CONFIG_IDX = pOLL_CONFIG_IDX.ConvertOrDefault<Guid>();
                    if (mONITOR_IDX != null) e.MONITOR_IDX = mONITOR_IDX.ConvertOrDefault<Guid>();
                    if (cOL != null) e.COL = cOL;
                    if (sUM_TYPE != null) e.SUM_TYPE = sUM_TYPE;
                    if (rOUNDING != null) e.ROUNDING = rOUNDING;
                    if (aDJUST_FACTOR != null) e.ADJUST_FACTOR = aDJUST_FACTOR;

                    if (insInd)
                        ctx.T_QREST_SITE_POLL_CONFIG_DTL.Add(e);

                    ctx.SaveChanges();
                    return e.POLL_CONFIG_DTL_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static string GetT_QREST_SITE_POLL_CONFIG_DTL_org_ByID(Guid PollConfigDtlIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_POLL_CONFIG_DTL.AsNoTracking()
                            join b in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals b.MONITOR_IDX
                            join c in ctx.T_QREST_SITES.AsNoTracking() on b.SITE_IDX equals c.SITE_IDX
                            where a.POLL_CONFIG_DTL_IDX == PollConfigDtlIDX
                            select c).FirstOrDefault()?.ORG_ID;

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_QREST_SITE_POLL_CONFIG_DTL(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_SITE_POLL_CONFIG_DTL _dtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByDTLID(id);
                    if (_dtl != null)
                    {
                        //log activity
                        SiteMonitorDisplayType _mon = db_Air.GetT_QREST_MONITORS_ByID(_dtl.MONITOR_IDX);
                        if (_mon != null)
                            db_Ref.CreateT_QREST_SYS_LOG_ACTIVITY("POLLING CONFIG", null, null, "Column mapping removed [" + _mon.PAR_CODE + "]", null, _dtl.POLL_CONFIG_IDX.ToString());

                        ctx.Entry(_dtl).State = System.Data.Entity.EntityState.Deleted;
                        ctx.SaveChanges();
                        return 1;
                    }

                    return 0;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static List<SitePollingConfigDetailTypeExtended> GetT_QREST_SITES_POLLING_CONFIG_DetailList(string USER_IDX, string orgID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITES.AsNoTracking()
                            join b in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking() on a.SITE_IDX equals b.SITE_IDX
                            join c in ctx.T_QREST_SITE_POLL_CONFIG_DTL.AsNoTracking() on b.POLL_CONFIG_IDX equals c.POLL_CONFIG_IDX
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on c.MONITOR_IDX equals m.MONITOR_IDX
                            join r in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals r.PAR_METHOD_IDX
                            join d in ctx.T_QREST_ORG_USERS.AsNoTracking() on a.ORG_ID equals d.ORG_ID
                            where d.USER_IDX == USER_IDX
                            && (orgID != null ? a.ORG_ID == orgID : true)
                            orderby a.ORG_ID, a.SITE_ID
                            select new SitePollingConfigDetailTypeExtended
                            {
                                POLL_CONFIG_DTL_IDX = c.POLL_CONFIG_DTL_IDX,
                                POLL_CONFIG_IDX = c.POLL_CONFIG_IDX,
                                MONITOR_IDX = c.MONITOR_IDX,
                                PAR_CODE = r.PAR_CODE,
                                COLLECT_UNIT_CODE = m.COLLECT_UNIT_CODE,
                                COL = c.COL,
                                SUM_TYPE = c.SUM_TYPE,
                                ROUNDING = c.ROUNDING,
                                ORG_ID = a.ORG_ID,
                                SITE_ID = a.SITE_ID
                            }).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }




        //*****************MONITORS**********************************
        public static T_QREST_MONITORS GetT_QREST_MONITORS_ByID_Simple(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_MONITORS.AsNoTracking()
                            where a.MONITOR_IDX == id
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static SiteMonitorDisplayType GetT_QREST_MONITORS_ByID(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_MONITORS.AsNoTracking()
                            join r in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on a.PAR_METHOD_IDX equals r.PAR_METHOD_IDX
                            join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on r.PAR_CODE equals p.PAR_CODE
                            join s in ctx.T_QREST_SITES.AsNoTracking() on a.SITE_IDX equals s.SITE_IDX
                            where a.MONITOR_IDX == id
                            select new SiteMonitorDisplayType
                            {
                                T_QREST_MONITORS = a,
                                METHOD_CODE = r.METHOD_CODE,
                                PAR_NAME = p.PAR_NAME,
                                PAR_CODE = p.PAR_CODE,
                                SITE_ID = s.SITE_ID,
                                ORG_ID = s.ORG_ID
                            }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_QREST_MONITORS> GetT_QREST_MONITORS_bySiteIDX(Guid SiteIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_MONITORS.AsNoTracking()
                               where a.SITE_IDX == SiteIDX
                               orderby a.CREATE_DT
                               select a).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_QREST_MONITORS GetT_QREST_MONITORS_bySiteIDX_ParMethod_POC(Guid SiteIDX, Guid ParMethodIDX, int POC)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_MONITORS.AsNoTracking()
                               where a.SITE_IDX == SiteIDX
                               && a.PAR_METHOD_IDX == ParMethodIDX
                               && a.POC == POC
                               select a).FirstOrDefault();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static bool GetT_QREST_MONITORS_AnyByParMethodIdx(Guid ParMethodIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_MONITORS.AsNoTracking()
                        where a.PAR_METHOD_IDX == ParMethodIDX
                        select a).Any();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return true;
                }
            }
        }


        public static List<SiteMonitorDisplayType> GetT_QREST_MONITORS_Display_bySiteIDX(Guid? SiteIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_MONITORS.AsNoTracking()
                               join r in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on a.PAR_METHOD_IDX equals r.PAR_METHOD_IDX
                               join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on r.PAR_CODE equals p.PAR_CODE
                               join d in ctx.T_QREST_REF_UNITS.AsNoTracking() on a.COLLECT_UNIT_CODE equals d.UNIT_CODE
                                   into lj
                               from d in lj.DefaultIfEmpty() //left join on monitor's unit
                               where a.SITE_IDX == SiteIDX
                               orderby p.PAR_CODE
                               select new SiteMonitorDisplayType
                               {
                                   T_QREST_MONITORS = a,
                                   METHOD_CODE = r.METHOD_CODE,
                                   COLLECTION_DESC = r.COLLECTION_DESC,
                                   PAR_CODE = p.PAR_CODE,
                                   PAR_NAME = p.PAR_NAME,
                                   UNIT_DESC = d.UNIT_DESC
                               }).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SiteMonitorDisplayType> GetT_QREST_MONITORS_Display_SampledBySiteIDX(Guid? SiteIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_MONITORS.AsNoTracking()
                               join r in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on a.PAR_METHOD_IDX equals r.PAR_METHOD_IDX
                               join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on r.PAR_CODE equals p.PAR_CODE
                               join h in ctx.T_QREST_DATA_HOURLY.AsNoTracking() on a.MONITOR_IDX equals h.MONITOR_IDX
                               where a.SITE_IDX == SiteIDX
                               select new SiteMonitorDisplayType
                               {
                                   T_QREST_MONITORS = a,
                                   METHOD_CODE = r.METHOD_CODE,
                                   COLLECTION_DESC = r.COLLECTION_DESC,
                                   PAR_CODE = p.PAR_CODE,
                                   PAR_NAME = p.PAR_NAME
                               }).Distinct().OrderBy(x => x.PAR_CODE).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SiteMonitorDisplayType> GetT_QREST_MONITORS_Display_SampledByOrgID(string orgID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_MONITORS.AsNoTracking()
                               join s in ctx.T_QREST_SITES.AsNoTracking() on a.SITE_IDX equals s.SITE_IDX
                               join r in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on a.PAR_METHOD_IDX equals r.PAR_METHOD_IDX
                               join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on r.PAR_CODE equals p.PAR_CODE
                               join h in ctx.T_QREST_DATA_HOURLY.AsNoTracking() on a.MONITOR_IDX equals h.MONITOR_IDX
                               where s.ORG_ID == orgID
                               select new SiteMonitorDisplayType
                               {
                                   T_QREST_MONITORS = a,
                                   METHOD_CODE = r.METHOD_CODE,
                                   COLLECTION_DESC = r.COLLECTION_DESC,
                                   PAR_CODE = p.PAR_CODE,
                                   PAR_NAME = p.PAR_NAME,
                                   SITE_ID = s.SITE_ID
                               }).Distinct().ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SiteMonitorDisplayTypeSmall> GetT_QREST_MONITORS_Display_SampledByOrgID_ForDdl(string orgId)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_MONITORS.AsNoTracking() 
                        join s in ctx.T_QREST_SITES.AsNoTracking() on a.SITE_IDX equals s.SITE_IDX
                        join r in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on a.PAR_METHOD_IDX equals r.PAR_METHOD_IDX
                        join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on r.PAR_CODE equals p.PAR_CODE
                        join h in ctx.T_QREST_DATA_HOURLY.AsNoTracking() on a.MONITOR_IDX equals h.MONITOR_IDX
                        where s.ORG_ID == orgId
                        select new SiteMonitorDisplayTypeSmall
                        {
                            MONITOR_IDX = a.MONITOR_IDX,
                            POC = a.POC,
                            PAR_CODE = p.PAR_CODE,
                            PAR_NAME = p.PAR_NAME,
                            SITE_ID = s.SITE_ID
                        }).Distinct().ToList();
                    xxx = xxx.OrderBy(x => x.PAR_CODE).ToList();
                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SiteMonitorDisplayTypeSmall> GetT_QREST_MONITORS_Display_Sampled_FIVE_MIN_ByOrgID_ForDdl(string orgId)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_MONITORS.AsNoTracking()
                               join s in ctx.T_QREST_SITES.AsNoTracking() on a.SITE_IDX equals s.SITE_IDX
                               join r in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on a.PAR_METHOD_IDX equals r.PAR_METHOD_IDX
                               join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on r.PAR_CODE equals p.PAR_CODE
                               join h in ctx.T_QREST_DATA_FIVE_MIN.AsNoTracking() on a.MONITOR_IDX equals h.MONITOR_IDX
                               where s.ORG_ID == orgId
                               select new SiteMonitorDisplayTypeSmall
                               {
                                   MONITOR_IDX = a.MONITOR_IDX,
                                   POC = a.POC,
                                   PAR_CODE = p.PAR_CODE,
                                   PAR_NAME = p.PAR_NAME,
                                   SITE_ID = s.SITE_ID
                               }).Distinct().ToList();
                    xxx = xxx.OrderBy(x => x.PAR_CODE).ToList();
                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SiteMonitorDisplayTypeSmall> GetT_QREST_MONITORS_Display_SampledByUser(string UserIDX, int? LastNumDays)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    int subtractDays = (LastNumDays ?? 365) * -1;
                    DateTime sinceDate = System.DateTime.Today.AddDays(subtractDays);
                    var xxx = (from a in ctx.T_QREST_MONITORS.AsNoTracking()
                               join s in ctx.T_QREST_SITES.AsNoTracking() on a.SITE_IDX equals s.SITE_IDX
                               join r in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on a.PAR_METHOD_IDX equals r.PAR_METHOD_IDX
                               join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on r.PAR_CODE equals p.PAR_CODE
                               join h in ctx.T_QREST_DATA_HOURLY.AsNoTracking() on a.MONITOR_IDX equals h.MONITOR_IDX
                               join u in ctx.T_QREST_ORG_USERS.AsNoTracking() on s.ORG_ID equals u.ORG_ID
                               where u.USER_IDX == UserIDX
                               && u.STATUS_IND == "A"
                               && h.DATA_DTTM_LOCAL > sinceDate
                               select new SiteMonitorDisplayTypeSmall
                               {
                                   MONITOR_IDX = a.MONITOR_IDX,
                                   POC = a.POC,
                                   SITE_ID = s.SITE_ID,
                                   PAR_CODE = p.PAR_CODE,
                                   PAR_NAME = p.PAR_NAME
                               }).Distinct().OrderBy(x => x.SITE_ID).ThenBy(x => x.PAR_CODE);

                    return xxx.ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SiteMonitorDisplayTypeSmall> GetT_QREST_MONITORS_Display_ByUser_QCType(string UserIDX, string QCType)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_MONITORS.AsNoTracking()
                               join s in ctx.T_QREST_SITES.AsNoTracking() on a.SITE_IDX equals s.SITE_IDX
                               join r in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on a.PAR_METHOD_IDX equals r.PAR_METHOD_IDX
                               join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on r.PAR_CODE equals p.PAR_CODE
                               join u in ctx.T_QREST_ORG_USERS.AsNoTracking() on s.ORG_ID equals u.ORG_ID
                               join q in ctx.T_QREST_REF_QC.AsNoTracking() on r.PAR_CODE equals q.PAR_CODE
                               join unit in ctx.T_QREST_REF_UNITS.AsNoTracking() on a.COLLECT_UNIT_CODE equals unit.UNIT_CODE
                               into lj from unit in lj.DefaultIfEmpty() //left join on monitor's unit
                               where u.USER_IDX == UserIDX
                               && u.STATUS_IND == "A"
                               && q.ASSESSMENT_TYPE == QCType
                               select new SiteMonitorDisplayTypeSmall
                               {
                                   MONITOR_IDX = a.MONITOR_IDX,
                                   POC = a.POC,
                                   PAR_CODE = p.PAR_CODE,
                                   PAR_NAME = p.PAR_NAME,
                                   SITE_ID = s.SITE_ID,
                                   ORG_ID = u.ORG_ID,
                                   UNIT_DESC = unit.UNIT_DESC
                               }).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }



        /// <summary>
        /// Returns list of monitors a user has access to, optionally filtered by OrgID
        /// </summary>
        /// <param name="OrgID"></param>
        /// <param name="UserIDX"></param>
        /// <returns></returns>
        public static List<SiteMonitorDisplayType> GetT_QREST_MONITORS_ByUser_OrgID(string OrgID, string UserIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from m in ctx.T_QREST_MONITORS.AsNoTracking()
                               join a in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals a.SITE_IDX
                               join u in ctx.T_QREST_ORG_USERS.AsNoTracking() on a.ORG_ID equals u.ORG_ID
                               join r in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals r.PAR_METHOD_IDX
                               join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on r.PAR_CODE equals p.PAR_CODE
                               join unit in ctx.T_QREST_REF_UNITS.AsNoTracking() on m.COLLECT_UNIT_CODE equals unit.UNIT_CODE
                               into lj from unit in lj.DefaultIfEmpty() //left join on monitor's unit
                               where u.USER_IDX == UserIDX
                               && u.STATUS_IND == "A"
                               && (OrgID != null ? a.ORG_ID == OrgID : true)
                               orderby a.SITE_ID
                               select new SiteMonitorDisplayType
                               {
                                   T_QREST_MONITORS = m,
                                   SITE_ID = a.SITE_ID,
                                   METHOD_CODE = r.METHOD_CODE,
                                   PAR_NAME = p.PAR_NAME,
                                   PAR_CODE = p.PAR_CODE,
                                   ORG_ID = a.ORG_ID,
                                   UNIT_DESC = unit.UNIT_DESC
                               }).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int? GetT_QREST_MONITORS_ByUser_OrgID_Count(string OrgID, string UserIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from m in ctx.T_QREST_MONITORS.AsNoTracking()
                            join a in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals a.SITE_IDX
                            join u in ctx.T_QREST_ORG_USERS.AsNoTracking() on a.ORG_ID equals u.ORG_ID
                            where u.USER_IDX == UserIDX
                            && u.STATUS_IND == "A"
                            && (OrgID != null ? a.ORG_ID == OrgID : true)
                            select m).Count();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? InsertUpdatetT_QREST_MONITORS(Guid? mONITOR_IDX, Guid? sITE_IDX, Guid? pAR_METHOD_IDX, int? pOC, string dURATION_CODE, string cOLLECT_FREQ_CODE,
            string cOLLECT_UNIT_CODE, double? aLERT_MIN_VALUE, double? aLERT_MAX_VALUE, double? aLERT_AMT_CHANGE, int? aLERT_STUCK_REC_COUNT,
            string aLERT_MIN_TYPE, string aLERT_MAX_TYPE, string aLERT_AMT_CHANGE_TYPE, string aLERT_STUCK_TYPE, string cREATE_USER, string _loggingPAR_NAME)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_QREST_MONITORS e = (from c in ctx.T_QREST_MONITORS
                                          where c.MONITOR_IDX == mONITOR_IDX
                                          select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_MONITORS();
                        e.MONITOR_IDX = Guid.NewGuid();
                        e.SITE_IDX = sITE_IDX.ConvertOrDefault<Guid>();
                        e.CREATE_DT = System.DateTime.Now;
                        e.CREATE_USER_IDX = cREATE_USER;
                    }
                    else
                    {
                        e.MODIFY_DT = System.DateTime.Now;
                        e.MODIFY_USER_IDX = cREATE_USER;

                        //activity logging
                        var _site = db_Air.GetT_QREST_SITES_ByID(e.SITE_IDX);
                        if (_site != null)
                        {
                            string modTxt = "Changed monitor settings for " + _loggingPAR_NAME + " at " + _site.ORG_ID;
                            if (aLERT_MIN_VALUE != null && aLERT_MIN_VALUE != -9999 && e.ALERT_MIN_VALUE != aLERT_MIN_VALUE) modTxt += " [MinAlert changed to " + aLERT_MIN_VALUE + "]";
                            if (aLERT_MAX_VALUE != null && aLERT_MAX_VALUE != -9999 && e.ALERT_MAX_VALUE != aLERT_MAX_VALUE) modTxt += " [MaxAlert changed to " + aLERT_MAX_VALUE + "]";
                            if (aLERT_AMT_CHANGE != null && aLERT_AMT_CHANGE != -9999 && e.ALERT_AMT_CHANGE != aLERT_AMT_CHANGE) modTxt += " [AmtChanged changed to " + aLERT_AMT_CHANGE + "]";
                            db_Ref.CreateT_QREST_SYS_LOG_ACTIVITY("MON EDIT", cREATE_USER, null, modTxt, null, _site.ORG_ID);
                        }
                    }

                    if (pAR_METHOD_IDX != null) e.PAR_METHOD_IDX = pAR_METHOD_IDX.ConvertOrDefault<Guid>();
                    if (pOC != null) e.POC = (int)pOC;
                    if (dURATION_CODE != null) e.DURATION_CODE = dURATION_CODE;
                    if (cOLLECT_FREQ_CODE != null) e.COLLECT_FREQ_CODE = cOLLECT_FREQ_CODE;
                    if (cOLLECT_UNIT_CODE != null) e.COLLECT_UNIT_CODE = cOLLECT_UNIT_CODE;
                    if (aLERT_MIN_VALUE != null) e.ALERT_MIN_VALUE = aLERT_MIN_VALUE;
                    if (aLERT_MIN_VALUE == -9999) e.ALERT_MIN_VALUE = null;  //explicit setting null
                    if (aLERT_MAX_VALUE != null) e.ALERT_MAX_VALUE = aLERT_MAX_VALUE;
                    if (aLERT_MAX_VALUE == -9999) e.ALERT_MAX_VALUE = null;  //explicit setting null
                    if (aLERT_AMT_CHANGE != null) e.ALERT_AMT_CHANGE = aLERT_AMT_CHANGE;
                    if (aLERT_AMT_CHANGE == -9999) e.ALERT_AMT_CHANGE = null;  //explicit setting null
                    if (aLERT_STUCK_REC_COUNT != null) e.ALERT_STUCK_REC_COUNT = aLERT_STUCK_REC_COUNT;
                    if (aLERT_STUCK_REC_COUNT == -9999) e.ALERT_STUCK_REC_COUNT = null;  //explicit setting null
                    if (aLERT_MIN_TYPE != null) e.ALERT_MIN_TYPE = aLERT_MIN_TYPE;
                    if (aLERT_MAX_TYPE != null) e.ALERT_MAX_TYPE = aLERT_MAX_TYPE;
                    if (aLERT_AMT_CHANGE_TYPE != null) e.ALERT_AMT_CHANGE_TYPE = aLERT_AMT_CHANGE_TYPE;
                    if (aLERT_STUCK_TYPE != null) e.ALERT_STUCK_TYPE = aLERT_STUCK_TYPE;

                    if (insInd)
                        ctx.T_QREST_MONITORS.Add(e);

                    ctx.SaveChanges();
                    return e.MONITOR_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_QREST_MONITORS(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_MONITORS rec = new T_QREST_MONITORS { MONITOR_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }



        //*****************QC_ASSESSMENT**********************************
        public static Guid? InsertUpdatetT_QREST_QC_ASSESSMENT(Guid? qC_ASSESS_IDX, Guid? mONITOR_IDX, DateTime? aSSESSMENT_DT, string aSSESSMENT_TYPE,
            string uNIT_CODE, int? aSSESSMENT_NUM, string aSSESSED_BY, string cREATE_USER, string aSSESSMENT_TM)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_QREST_QC_ASSESSMENT e = (from c in ctx.T_QREST_QC_ASSESSMENT
                                               where c.QC_ASSESS_IDX == qC_ASSESS_IDX
                                               select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_QC_ASSESSMENT();
                        e.QC_ASSESS_IDX = Guid.NewGuid();
                        e.CREATE_DT = System.DateTime.Now;
                        e.CREATE_USER_IDX = cREATE_USER;
                    }
                    else
                    {
                        e.MODIFY_DT = System.DateTime.Now;
                        e.MODIFY_USER_IDX = cREATE_USER;
                    }

                    if (mONITOR_IDX != null) e.MONITOR_IDX = mONITOR_IDX.ConvertOrDefault<Guid>();
                    if (aSSESSMENT_DT != null) e.ASSESSMENT_DT = aSSESSMENT_DT.GetValueOrDefault();
                    if (aSSESSMENT_TM != null) e.ASSESSMENT_TM = aSSESSMENT_TM;
                    if (aSSESSMENT_TYPE != null) e.ASSESSMENT_TYPE = aSSESSMENT_TYPE;
                    if (uNIT_CODE != null) e.UNIT_CODE = uNIT_CODE;
                    if (aSSESSMENT_NUM != null) e.ASSESSMENT_NUM = aSSESSMENT_NUM ?? 1;
                    if (aSSESSED_BY != null) e.ASSESSED_BY = aSSESSED_BY;

                    if (insInd)
                        ctx.T_QREST_QC_ASSESSMENT.Add(e);

                    ctx.SaveChanges();
                    return e.QC_ASSESS_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<QcAssessmentDisplay> GetT_QREST_QC_ASSESSMENT_Search(string UserIDX, string orgIDX, Guid? SiteIDX, Guid? MonitorIDX, int pageSize, int? skip, int orderBy, string orderDir = "asc")
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    if (string.IsNullOrEmpty(orgIDX))
                        orgIDX = null;

                    string orderCol = (orderBy == 3 ? "ASSESSMENT_DT" : "ASSESSMENT_DT");

                    var xxx = (from a in ctx.T_QREST_QC_ASSESSMENT.AsNoTracking()
                               join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                               join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                               join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                               join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                               join u in ctx.T_QREST_ORG_USERS.AsNoTracking() on s.ORG_ID equals u.ORG_ID
                               where u.USER_IDX == UserIDX
                               && u.STATUS_IND == "A"
                               && (MonitorIDX != null ? a.MONITOR_IDX == MonitorIDX : true)
                               && (SiteIDX != null ? m.SITE_IDX == SiteIDX : true)
                               && (orgIDX != null ? s.ORG_ID == orgIDX : true)
                               select new QcAssessmentDisplay
                               {
                                   QC_ASSESS_IDX = a.QC_ASSESS_IDX,
                                   MONITOR_IDX = a.MONITOR_IDX,
                                   ASSESSMENT_DT = a.ASSESSMENT_DT,
                                   ASSESSMENT_TYPE = a.ASSESSMENT_TYPE,
                                   ASSESSED_BY = a.ASSESSED_BY,
                                   SITE_ID = s.SITE_ID,
                                   PAR_NAME = p.PAR_NAME + " (" + pm.PAR_CODE + ")",
                                   ORG_ID = s.ORG_ID
                               }).OrderBy(orderCol, orderDir).Skip(skip ?? 0).Take(pageSize).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static int? GetT_QREST_QC_ASSESSMENT_Search_Count(string orgIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_QC_ASSESSMENT.AsNoTracking()
                               join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                               join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                               where (orgIDX != null ? s.ORG_ID == orgIDX : true)
                               select a).Count();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int GetT_QREST_QC_ASSESSMENT_CountByMon(Guid mON_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_QC_ASSESSMENT.AsNoTracking()
                            where a.MONITOR_IDX == mON_IDX
                            select a).Count();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }



        public static T_QREST_QC_ASSESSMENT GetT_QREST_QC_ASSESSMENT_ByID(Guid? AssessIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_QC_ASSESSMENT.AsNoTracking()
                            where a.QC_ASSESS_IDX == AssessIDX
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static QcAssessmentAqsDisplay GetT_QREST_QC_ASSESSMENT_AQSData_ByID(Guid? AssessIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_QC_ASSESSMENT.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                            join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX                            
                            where a.QC_ASSESS_IDX == AssessIDX
                            select new QcAssessmentAqsDisplay
                            {
                                T_QREST_QC_ASSESSMENT = a,
                                STATE_CODE = s.STATE_CD,
                                COUNTY_CD = s.COUNTY_CD,
                                AQS_SITE_ID = s.AQS_SITE_ID,
                                PAR_CODE = pm.PAR_CODE,
                                POC = m.POC,
                                METHOD_CODE = pm.METHOD_CODE,
                                UNIT_CODE = m.COLLECT_UNIT_CODE,
                                FLOW_UNIT_CODE = a.UNIT_CODE
                            }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_QREST_QC_ASSESSMENT(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_QC_ASSESSMENT rec = new T_QREST_QC_ASSESSMENT { QC_ASSESS_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }



        //*****************QC_ASSESSMENT_DTL**********************************
        public static List<QC_ASSESSMENT_DTLDisplay> GetT_QREST_QC_ASSESSMENT_DTL_ByAssessID(Guid? AssessIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_QC_ASSESSMENT_DTL.AsNoTracking()
                               join b in ctx.T_QREST_QC_ASSESSMENT.AsNoTracking() on a.QC_ASSESS_IDX equals b.QC_ASSESS_IDX
                               join m in ctx.T_QREST_MONITORS.AsNoTracking() on b.MONITOR_IDX equals m.MONITOR_IDX
                               join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                               join d in ctx.T_QREST_REF_QC.AsNoTracking() on new { pc = pm.PAR_CODE, typ = b.ASSESSMENT_TYPE } equals new { pc = d.PAR_CODE, typ = d.ASSESSMENT_TYPE }
                               into lj from d in lj.DefaultIfEmpty() //left join on par code to qc limits
                               where a.QC_ASSESS_IDX == AssessIDX
                            select new QC_ASSESSMENT_DTLDisplay
                            {
                                QC_ASSESS_DTL_IDX = a.QC_ASSESS_DTL_IDX,
                                QC_ASSESS_IDX = a.QC_ASSESS_IDX,
                                ASSESSMENT_TYPE = b.ASSESSMENT_TYPE,
                                ASSESS_KNOWN_CONCENTRATION = a.ASSESS_KNOWN_CONCENTRATION,
                                MON_CONCENTRATION = a.MON_CONCENTRATION,
                                MIN_CONC = d.MIN_CONC,
                                MAX_CONC = d.MAX_CONC,
                                difference_limit = d.AMT_DIFF_LIM,
                                difference = null,
                                exceed_amt_difference_ind = false,

                                pct_limit = d.PCT_DIFF_LIM,
                                pct_difference = null,
                                exceed_pct_difference_ind = false,
                                
                                COMMENTS = a.COMMENTS,
                                PAR_CODE = pm.PAR_CODE,
                                UNIT_CODE = m.COLLECT_UNIT_CODE


                            }).ToList();

                    if (xxx != null && xxx.Count() > 0)
                    {
                        string par_cd = xxx[0].PAR_CODE;
                        string unit_cd = xxx[0].UNIT_CODE;
                        string assess_typ = xxx[0].ASSESSMENT_TYPE;
                        decimal pct_exceed_lim = xxx[0].pct_limit != null ? xxx[0].pct_limit.ConvertOrDefault<decimal>() : 100;
                        decimal amt_exceed_lim = xxx[0].difference_limit != null ? xxx[0].difference_limit.ConvertOrDefault<decimal>() : 99999;

                        foreach (QC_ASSESSMENT_DTLDisplay x in xxx)
                        {
                            if (x.ASSESS_KNOWN_CONCENTRATION != null && x.MON_CONCENTRATION != null)
                            {
                                //VALIDATE RAW DIFFERENCE
                                x.difference = x.ASSESS_KNOWN_CONCENTRATION.ConvertOrDefault<decimal>() - x.MON_CONCENTRATION.ConvertOrDefault<decimal>();
                                x.difference = Math.Abs(x.difference.GetValueOrDefault());
                                x.exceed_amt_difference_ind = (x.difference > amt_exceed_lim);

                                if (x.ASSESS_KNOWN_CONCENTRATION != 0)
                                {
                                    //VALIDATE PERCENT DIFFERENCE
                                    x.pct_difference = (100M * x.difference) / x.ASSESS_KNOWN_CONCENTRATION.ConvertOrDefault<decimal>();
                                    x.pct_difference = Math.Abs(x.pct_difference.GetValueOrDefault());
                                    x.exceed_pct_difference_ind = (x.pct_difference > pct_exceed_lim);

                                    //VALIDATE AUDIT LEVEL OR VALID CONCENTRATION RANGE
                                    if (assess_typ == "Annual PE")
                                    {
                                        T_QREST_REF_QC_AUDIT_LVL _audlvl = db_Ref.GetT_QREST_REF_QC_AUDIT_LVL_ByPar(par_cd, x.ASSESS_KNOWN_CONCENTRATION.GetValueOrDefault());
                                        if (_audlvl != null)
                                        {
                                            x.audit_level_int = _audlvl.AUDIT_LVL;
                                            x.audit_level_disp = "Concentration range: " + _audlvl.MIN_AMT + " to " + _audlvl.MAX_AMT;
                                        }
                                        else
                                        {
                                            x.audit_level_int = 0;
                                            x.audit_level_disp = "Not in any valid audit level";
                                        }
                                    }
                                    else if (assess_typ == "1-Point QC" && x.MIN_CONC != null && x.MAX_CONC != null)
                                    {
                                        if (x.MON_CONCENTRATION < x.MIN_CONC || x.MON_CONCENTRATION > x.MAX_CONC)
                                        {
                                            x.audit_level_int = 0;
                                            x.audit_level_disp = "Not in any valid concentration range (" + x.MIN_CONC + "-" + x.MAX_CONC + ")";
                                        }
                                        else
                                        {
                                            x.audit_level_int = 1;
                                            x.audit_level_disp = "In valid concentration range (" + x.MIN_CONC + "-" + x.MAX_CONC + ")";
                                        }
                                    }
                                }
                            }
                        }

                    }
                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static T_QREST_QC_ASSESSMENT_DTL GetT_QREST_QC_ASSESSMENT_DTL_ByAssessID_Zero(Guid? AssessIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_QC_ASSESSMENT_DTL.AsNoTracking()
                               where a.QC_ASSESS_IDX == AssessIDX
                               && a.ASSESS_KNOWN_CONCENTRATION == 0
                               select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static T_QREST_QC_ASSESSMENT_DTL GetT_QREST_QC_ASSESSMENT_DTL_ByAssessID_NotZero(Guid? AssessIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_QC_ASSESSMENT_DTL.AsNoTracking()
                            where a.QC_ASSESS_IDX == AssessIDX
                            && a.ASSESS_KNOWN_CONCENTRATION != 0
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }





        public static Guid? InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(Guid? qC_ASSESS_DTL_IDX, Guid? qC_ASSESS_IDX, double? mON_CONC, double? aSSESS_KNOWN_CON, string aQS_NULL_CODE, 
            string cOMMENTS, string cREATE_USER)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_QREST_QC_ASSESSMENT_DTL e = (from c in ctx.T_QREST_QC_ASSESSMENT_DTL
                                               where c.QC_ASSESS_DTL_IDX == qC_ASSESS_DTL_IDX
                                               select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_QC_ASSESSMENT_DTL();
                        e.QC_ASSESS_DTL_IDX = Guid.NewGuid();
                        e.QC_ASSESS_IDX = qC_ASSESS_IDX.GetValueOrDefault();
                        e.CREATE_DT = System.DateTime.Now;
                        e.CREATE_USER_IDX = cREATE_USER;
                    }
                    else
                    {
                        e.MODIFY_DT = System.DateTime.Now;
                        e.MODIFY_USER_IDX = cREATE_USER;
                    }

                    if (mON_CONC != null) e.MON_CONCENTRATION = mON_CONC;
                    if (aSSESS_KNOWN_CON != null) e.ASSESS_KNOWN_CONCENTRATION = aSSESS_KNOWN_CON;
                    if (aQS_NULL_CODE != null) e.AQS_NULL_CODE = aQS_NULL_CODE;
                    if (cOMMENTS != null) e.COMMENTS = cOMMENTS;

                    if (insInd)
                        ctx.T_QREST_QC_ASSESSMENT_DTL.Add(e);

                    ctx.SaveChanges();
                    return e.QC_ASSESS_DTL_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static int DeleteT_QREST_QC_ASSESSMENT_DTL(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_QC_ASSESSMENT_DTL rec = new T_QREST_QC_ASSESSMENT_DTL { QC_ASSESS_DTL_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }


        //*****************FIVE MIN**********************************
        public static Guid? InsertT_QREST_DATA_FIVE_MIN(Guid mONITOR_IDX, DateTime dATA_DTTM, string dATA_VALUE, string uNIT_CODE, bool? vAL_IND, string vAL_CD, DateTime? mODIFY_DT, double? aDJUST_FACTOR, Guid? iMPORT_IDX, string timeType, int tzOffset, bool insertOnly = false)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    DateTime dtLocal = timeType == "L" ? dATA_DTTM : dATA_DTTM.AddHours(tzOffset);
                    DateTime dtUtc = timeType == "L" ? dATA_DTTM.AddHours(tzOffset * -1) : dATA_DTTM;


                    T_QREST_DATA_FIVE_MIN e = (from c in ctx.T_QREST_DATA_FIVE_MIN
                                               where c.MONITOR_IDX == mONITOR_IDX
                                               && c.DATA_DTTM == dtUtc
                                               select c).FirstOrDefault();

                    //insert case
                    if (e == null)
                    {
                        e = new T_QREST_DATA_FIVE_MIN();
                        e.DATA_FIVE_IDX = Guid.NewGuid();
                        e.MONITOR_IDX = mONITOR_IDX;
                        e.DATA_DTTM = dtUtc;
                        e.DATA_DTTM_LOCAL = dtLocal;
                        e.DATA_VALUE = dATA_VALUE;

                        if (double.TryParse(dATA_VALUE, out double n))
                            e.DATA_VALUE = (n * aDJUST_FACTOR).ToString();

                        e.UNIT_CODE = uNIT_CODE;
                        e.VAL_IND = vAL_IND ?? false;
                        e.VAL_CD = vAL_CD;
                        if (iMPORT_IDX != null) e.IMPORT_IDX = iMPORT_IDX;
                        e.MODIFY_DT = mODIFY_DT ?? System.DateTime.Now;
                        ctx.T_QREST_DATA_FIVE_MIN.Add(e);
                        ctx.SaveChanges();
                    }
                    else if (e.DATA_VALUE != dATA_VALUE && insertOnly)    // only update the record if the insertOnly flag is set and the data value has changed
                    {
                        e.DATA_VALUE = dATA_VALUE;

                        if (double.TryParse(dATA_VALUE, out double n))
                            e.DATA_VALUE = (n * aDJUST_FACTOR).ToString();

                        if (vAL_IND != null) e.VAL_IND = vAL_IND;
                        if (vAL_CD != null) e.VAL_CD = vAL_CD;
                        if (iMPORT_IDX != null) e.IMPORT_IDX = iMPORT_IDX;
                        e.MODIFY_DT = mODIFY_DT ?? System.DateTime.Now;
                        ctx.SaveChanges();
                    }

                    return e.DATA_FIVE_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? UpdateT_QREST_DATA_FIVE_MIN(Guid dATA_FIVE_IDX, string dATA_VALUE, string uNIT_CODE, bool? vAL_IND, string vAL_CD)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_DATA_FIVE_MIN e = (from c in ctx.T_QREST_DATA_FIVE_MIN
                                               where c.DATA_FIVE_IDX == dATA_FIVE_IDX
                                               select c).FirstOrDefault();

                    //update case
                    if (e != null)
                    {
                        if (dATA_VALUE != null) e.DATA_VALUE = dATA_VALUE;
                        if (uNIT_CODE != null) e.UNIT_CODE = uNIT_CODE;
                        if (vAL_CD != null) e.VAL_CD = vAL_CD;
                        e.MODIFY_DT = System.DateTime.Now;
                        ctx.SaveChanges();
                        return e.DATA_FIVE_IDX;
                    }
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;    
                }
            }
        }


        public static bool InsertT_QREST_DATA_FIVE_MIN_fromLine(string line, SitePollingConfigType config, List<SitePollingConfigDetailType> config_dtl, bool insertOnly = false)
        {
            try
            {
                //preview file to determine if tab or comma separated
                int delComma = line.Split(new char[] { ',' }, StringSplitOptions.None).Count();
                int delTab = line.Split(new char[] { '\t' }, StringSplitOptions.None).Count();
                char[] delimiter = delComma > delTab ? new char[] { ',' } : new char[] { '\t' };

                string[] cols = line.Split(delimiter);

                if (cols.Length > 1)  //skip blank row
                {
                    //date
                    string sDate = cols[config.DATE_COL.GetValueOrDefault() - 1].Replace("\"", "").Trim();
                    string sTime = cols[config.TIME_COL.GetValueOrDefault() - 1].Replace("\"", "").Trim();
                    string dateTimeString = config.DATE_COL.GetValueOrDefault() != config.TIME_COL.GetValueOrDefault()
                        ? (sDate + " " + sTime).Trim()
                        : sDate;

                    //raw date time coming from logger
                    //get allowed date/time formats
                    string[] allowedFormats = UtilsText.GetDateTimeAllowedFormats(config.DATE_FORMAT, config.TIME_FORMAT);

                    DateTime dt = DateTime.ParseExact(dateTimeString.Trim(), allowedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);

                    foreach (SitePollingConfigDetailType _map in config_dtl)
                    {
                        //apply N-minute alerts if available
                        string valCd = "";
                        Double? val = cols[_map.COL - 1 ?? 0].ToString().ConvertOrDefault<Double?>();
                        if (val != null && _map.ALERT_MAX_TYPE == "N" && val > _map.ALERT_MAX_VALUE)
                            valCd = "MAX";
                        if (val != null && _map.ALERT_MIN_TYPE == "N" && val < _map.ALERT_MIN_VALUE)
                            valCd = "MIN";

                        db_Air.InsertT_QREST_DATA_FIVE_MIN(_map.MONITOR_IDX, dt, cols[_map.COL - 1 ?? 0], _map.COLLECT_UNIT_CODE, true, valCd, null, _map.ADJUST_FACTOR, null, config.TIME_POLL_TYPE, config.LOCAL_TIMEZONE.ConvertOrDefault<int>(), insertOnly);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                db_Ref.CreateT_QREST_SYS_LOG(null, "POLLING", "Site " + config.SITE_ID + " " + ex.Message ?? ex.InnerException?.ToString());
                return false;
            }

        }


        public static List<RawDataDisplay> GetT_QREST_DATA_FIVE_MIN(string org, Guid? site, Guid? mon, DateTime? DateFrom, DateTime? DateTo, int pageSize, int? skip, int orderBy, string orderDir = "desc", string timeType = "L")
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    DateTime DateFromDt = DateFrom.GetValueOrDefault(System.DateTime.UtcNow.AddDays(-1));
                    DateTime DateToDt = DateTo.GetValueOrDefault(System.DateTime.UtcNow.AddHours(1));

                    string orderCol = "DATA_DTTM";
                    if (orderBy == 5) orderCol = "DATA_VALUE";
                    else if (orderBy == 6) orderCol = "VAL_CD";

                    var xxx = (from a in ctx.T_QREST_DATA_FIVE_MIN.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                            join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                            join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                            join u3 in ctx.T_QREST_REF_UNITS.AsNoTracking() on a.UNIT_CODE equals u3.UNIT_CODE
                            into lj3
                            from u3 in lj3.DefaultIfEmpty() //left join on minute unit
                            where (timeType == "U" ? a.DATA_DTTM >= DateFromDt : true)
                            && (timeType == "U" ? a.DATA_DTTM <= DateToDt : true)
                            && (timeType == "L" ? a.DATA_DTTM_LOCAL >= DateFromDt : true)
                            && (timeType == "L" ? a.DATA_DTTM_LOCAL <= DateToDt : true)
                            && (org != null ? s.ORG_ID == org : true)
                            && (site != null ? s.SITE_IDX == site : true)
                            && (mon != null ? a.MONITOR_IDX == mon : true)
                            select new RawDataDisplay
                            {
                                ORG_ID = s.ORG_ID,
                                SITE_ID = s.SITE_ID,
                                MONITOR_IDX = m.MONITOR_IDX,
                                DATA_RAW_IDX = a.DATA_FIVE_IDX,
                                UNIT_CODE = m.COLLECT_UNIT_CODE,
                                UNIT_DESC = u3.UNIT_DESC,
                                DATA_DTTM = (timeType == "U" ? a.DATA_DTTM : a.DATA_DTTM_LOCAL),
                                DATA_VALUE = a.DATA_VALUE,
                                PAR_CODE = p.PAR_CODE,
                                PAR_NAME = p.PAR_NAME,
                                POC = m.POC,
                                VAL_IND = a.VAL_IND,
                                VAL_CD = a.VAL_CD,
                                NOTES = null
                            }).OrderBy(orderCol, orderDir).Skip(skip ?? 0).Take(pageSize).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<RawDataDisplay> GetT_QREST_DATA_FIVE_MIN_RawDataViewLocal(string org, Guid? mon, DateTime? dateFrom, DateTime? dateTo)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    DateTime dateFromDt = dateFrom.GetValueOrDefault(System.DateTime.UtcNow.AddDays(-1));
                    DateTime dateToDt = dateTo.GetValueOrDefault(System.DateTime.UtcNow.AddHours(1));

                    return (from a in ctx.T_QREST_DATA_FIVE_MIN.AsNoTracking()
                               join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                               join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                               join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                               join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                               join u3 in ctx.T_QREST_REF_UNITS.AsNoTracking() on a.UNIT_CODE equals u3.UNIT_CODE
                               into lj3 from u3 in lj3.DefaultIfEmpty() //left join on minute unit
                               where a.DATA_DTTM_LOCAL >= dateFromDt
                               && a.DATA_DTTM_LOCAL <= dateToDt 
                               && s.ORG_ID == org
                               && (mon == null || a.MONITOR_IDX == mon)
                               orderby a.DATA_DTTM
                               select new RawDataDisplay
                               {
                                   ORG_ID = s.ORG_ID,
                                   SITE_ID = s.SITE_ID,
                                   MONITOR_IDX = m.MONITOR_IDX,
                                   DATA_RAW_IDX = a.DATA_FIVE_IDX,
                                   UNIT_CODE = m.COLLECT_UNIT_CODE,
                                   UNIT_DESC = u3.UNIT_DESC,
                                   DATA_DTTM = a.DATA_DTTM_LOCAL,
                                   DATA_VALUE = a.DATA_VALUE,
                                   PAR_CODE = p.PAR_CODE,
                                   PAR_NAME = p.PAR_NAME,
                                   POC = m.POC,
                                   VAL_IND = a.VAL_IND,
                                   VAL_CD = a.VAL_CD,
                                   NOTES = null
                               }).Take(26280).ToList();

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<RawDataDisplay> GetT_QREST_DATA_FIVE_MIN_RawDataViewUTC(string org, Guid? mon, DateTime? dateFrom, DateTime? dateTo)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    DateTime dateFromDt = dateFrom.GetValueOrDefault(System.DateTime.UtcNow.AddDays(-1));
                    DateTime dateToDt = dateTo.GetValueOrDefault(System.DateTime.UtcNow.AddHours(1));

                    return (from a in ctx.T_QREST_DATA_FIVE_MIN.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                            join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                            join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                            join u3 in ctx.T_QREST_REF_UNITS.AsNoTracking() on a.UNIT_CODE equals u3.UNIT_CODE
                            into lj3 from u3 in lj3.DefaultIfEmpty() //left join on minute unit
                            where a.DATA_DTTM >= dateFromDt
                            && a.DATA_DTTM <= dateToDt
                            && s.ORG_ID == org
                            && (mon == null || a.MONITOR_IDX == mon)
                            orderby a.DATA_DTTM
                            select new RawDataDisplay
                            {
                                ORG_ID = s.ORG_ID,
                                SITE_ID = s.SITE_ID,
                                MONITOR_IDX = m.MONITOR_IDX,
                                DATA_RAW_IDX = a.DATA_FIVE_IDX,
                                UNIT_CODE = m.COLLECT_UNIT_CODE,
                                UNIT_DESC = u3.UNIT_DESC,
                                DATA_DTTM = a.DATA_DTTM,
                                DATA_VALUE = a.DATA_VALUE,
                                PAR_CODE = p.PAR_CODE,
                                PAR_NAME = p.PAR_NAME,
                                POC = m.POC,
                                VAL_IND = a.VAL_IND,
                                VAL_CD = a.VAL_CD,
                                NOTES = null
                            }).Take(26280).ToList();

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static int GetT_QREST_DATA_FIVE_MINcount(string org, Guid? mon, DateTime? DateFrom, DateTime? DateTo, string timeType)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    DateTime DateFromDt = DateFrom.GetValueOrDefault(System.DateTime.UtcNow.AddDays(-1));
                    DateTime DateToDt = DateTo.GetValueOrDefault(System.DateTime.UtcNow.AddHours(1));

                    return (from a in ctx.T_QREST_DATA_FIVE_MIN.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                            where (timeType == "U" ? a.DATA_DTTM >= DateFromDt : true)
                            && (timeType == "U" ? a.DATA_DTTM <= DateToDt : true)
                            && (timeType == "L" ? a.DATA_DTTM_LOCAL >= DateFromDt : true)
                            && (timeType == "L" ? a.DATA_DTTM_LOCAL <= DateToDt : true)
                            && (org != null ? s.ORG_ID == org : true)
                            && (mon != null ? a.MONITOR_IDX == mon : true)
                            select a).Count();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }


        public static int GetT_QREST_DATA_FIVE_MINcountByImportIDX(Guid? imp)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_FIVE_MIN.AsNoTracking()
                            where a.IMPORT_IDX == imp
                            select a).Count();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }


        public static int DeleteT_QREST_DATA_FIVE_MIN(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_DATA_FIVE_MIN rec = new T_QREST_DATA_FIVE_MIN { DATA_FIVE_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException dbex)
                {
                    //if trying to delete record already deleted
                    foreach (System.Data.Entity.Infrastructure.DbEntityEntry entry in dbex.Entries)
                    {
                    }
                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }


        public static int DeleteT_QREST_DATA_FIVE_MIN_ByImportIDX(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    ctx.Database.ExecuteSqlCommand("DELETE FROM T_QREST_DATA_FIVE_MIN WHERE IMPORT_IDX = {0}", id);

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }


        public static List<RawDataDisplay> GetT_QREST_DATA_FIVE_MIN_ByImportIDX(Guid iMPORT_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_FIVE_MIN.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                            join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                            join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                            where a.IMPORT_IDX == iMPORT_IDX
                            orderby a.DATA_DTTM_LOCAL, p.PAR_NAME
                            select new RawDataDisplay
                            {
                                ORG_ID = s.ORG_ID,
                                SITE_ID = s.SITE_ID,
                                MONITOR_IDX = m.MONITOR_IDX,
                                POC = m.POC,
                                PAR_CODE = p.PAR_CODE,
                                PAR_NAME = p.PAR_NAME,
                                DATA_DTTM = a.DATA_DTTM_LOCAL,
                                DATA_VALUE = a.DATA_VALUE,
                                VAL_CD = a.VAL_CD,
                            }).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }




        //*****************HOURLY**********************************
        public static List<RawDataDisplay> GetT_QREST_DATA_HOURLY(string org, Guid? mon, DateTime? DateFrom, DateTime? DateTo, int pageSize, int? skip, int orderBy, string orderDir = "desc", string timeType = "L")
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    DateTime DateFromDt = DateFrom.GetValueOrDefault(System.DateTime.UtcNow.AddDays(-1));
                    DateTime DateToDt = DateTo.GetValueOrDefault(System.DateTime.UtcNow.AddHours(1));
                    if (string.IsNullOrEmpty(org))
                        org = null;

                    string orderCol = "DATA_DTTM";
                    if (orderBy == 5) orderCol = "DATA_VALUE_NUM";
                    else if (orderBy == 6) orderCol = "VAL_CD";

                    return (from a in ctx.T_QREST_DATA_HOURLY.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                            join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                            join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                            join u3 in ctx.T_QREST_REF_UNITS.AsNoTracking() on a.UNIT_CODE equals u3.UNIT_CODE
                            into lj3
                            from u3 in lj3.DefaultIfEmpty() //left join on hourly unit

                            where (timeType == "U" ? a.DATA_DTTM_UTC >= DateFromDt : true)
                            && (timeType == "U" ? a.DATA_DTTM_UTC <= DateToDt : true)
                            && (timeType == "L" ? a.DATA_DTTM_LOCAL >= DateFromDt : true)
                            && (timeType == "L" ? a.DATA_DTTM_LOCAL <= DateToDt : true)
                            && (org != null ? s.ORG_ID == org : true)
                            && (mon != null ? a.MONITOR_IDX == mon : true)
                            orderby a.DATA_DTTM_LOCAL descending
                            select new RawDataDisplay
                            {
                                ORG_ID = s.ORG_ID,
                                SITE_ID = s.SITE_ID,
                                MONITOR_IDX = m.MONITOR_IDX,
                                METHOD_CODE = pm.METHOD_CODE,
                                DATA_RAW_IDX = a.DATA_HOURLY_IDX,
                                DATA_DTTM = (timeType == "U" ? a.DATA_DTTM_UTC : a.DATA_DTTM_LOCAL),
                                DATA_VALUE = a.DATA_VALUE,
                                PAR_CODE = p.PAR_CODE,
                                PAR_NAME = p.PAR_NAME,
                                POC = m.POC,
                                VAL_IND = a.VAL_IND,
                                VAL_CD = a.VAL_CD,
                                NOTES = a.NOTES,
                                UNIT_DESC = u3.UNIT_DESC,
                                UNIT_CODE = u3.UNIT_CODE,
                                AQS_QUAL_CODES = a.AQS_QUAL_CODES,
                                AQS_NULL_CODE = a.AQS_NULL_CODE,
                                LVL1_VAL_IND = a.LVL1_VAL_IND,
                                LVL2_VAL_IND = a.LVL2_VAL_IND
                            }).OrderBy(orderCol, orderDir).Skip(skip ?? 0).Take(pageSize).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static int GetT_QREST_DATA_HOURLYcount(string org, Guid? mon, DateTime? DateFrom, DateTime? DateTo, string timeType)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    DateTime DateFromDt = DateFrom.GetValueOrDefault(System.DateTime.UtcNow.AddDays(-1));
                    DateTime DateToDt = DateTo.GetValueOrDefault(System.DateTime.UtcNow.AddHours(1));

                    return (from a in ctx.T_QREST_DATA_HOURLY.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                            where (timeType == "U" ? a.DATA_DTTM_UTC >= DateFromDt : true)
                            && (timeType == "U" ? a.DATA_DTTM_UTC <= DateToDt : true)
                            && (timeType == "L" ? a.DATA_DTTM_LOCAL >= DateFromDt : true)
                            && (timeType == "L" ? a.DATA_DTTM_LOCAL <= DateToDt : true)
                            && s.ORG_ID == org
                            && (mon != null ? a.MONITOR_IDX == mon : true)
                            select a).Count();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }


        public static List<RawDataDisplay> GetT_QREST_DATA_HOURLY_Last24Records(Guid? mon)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var TwoDaysAgo = System.DateTime.Now.AddDays(-2);
                    return (from a in ctx.T_QREST_DATA_HOURLY.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            where a.MONITOR_IDX == mon
                            && a.DATA_DTTM_UTC>TwoDaysAgo
                            orderby a.DATA_DTTM_LOCAL descending
                            select new RawDataDisplay
                            {
                                MONITOR_IDX = m.MONITOR_IDX,
                                DATA_RAW_IDX = a.DATA_HOURLY_IDX,
                                DATA_DTTM = a.DATA_DTTM_UTC,
                                DATA_VALUE = a.DATA_VALUE,
                                POC = m.POC,
                                VAL_IND = a.VAL_IND,
                                VAL_CD = a.VAL_CD,
                            }).Take(24).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static int GetT_QREST_DATA_HOURLYcountByMon(Guid? mon)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_HOURLY.AsNoTracking()
                            where a.MONITOR_IDX == mon
                            select a).Count();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }


        public static int GetT_QREST_DATA_HOURLYcountByImportIDX(Guid? imp)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_HOURLY.AsNoTracking()
                            where a.IMPORT_IDX == imp
                            select a).Count();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }


        public static List<RawDataDisplay> GetT_QREST_DATA_HOURLY_ManVal(Guid mon, DateTime dateFrom, DateTime dateTo)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_HOURLY.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                            join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                            join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                            join u1 in ctx.T_QREST_USERS.AsNoTracking() on a.LVL1_VAL_USERIDX equals u1.USER_IDX
                                into lj1
                            from u1 in lj1.DefaultIfEmpty() //left join on lvl1 user
                            join u2 in ctx.T_QREST_USERS.AsNoTracking() on a.LVL2_VAL_USERIDX equals u2.USER_IDX
                                into lj2
                            from u2 in lj2.DefaultIfEmpty() //left join on lvl2 user
                            join u3 in ctx.T_QREST_REF_UNITS.AsNoTracking() on a.UNIT_CODE equals u3.UNIT_CODE
                                into lj3
                            from u3 in lj3.DefaultIfEmpty() //left join on hourly unit
                            where a.DATA_DTTM_LOCAL >= dateFrom
                            && a.DATA_DTTM_LOCAL <= dateTo
                            && a.MONITOR_IDX == mon
                            orderby a.DATA_DTTM_LOCAL ascending
                            select new RawDataDisplay
                            {
                                ORG_ID = s.ORG_ID,
                                SITE_ID = s.SITE_ID,
                                MONITOR_IDX = m.MONITOR_IDX,
                                DATA_RAW_IDX = a.DATA_HOURLY_IDX,
                                DATA_DTTM = a.DATA_DTTM_LOCAL,
                                DATA_VALUE = a.DATA_VALUE,
                                PAR_CODE = p.PAR_CODE,
                                PAR_NAME = p.PAR_NAME,
                                UNIT_CODE = u3.UNIT_CODE,
                                UNIT_DESC = u3.UNIT_DESC,
                                POC = m.POC,
                                VAL_IND = a.VAL_IND,
                                VAL_CD = a.VAL_CD,
                                AQS_NULL_CODE = a.AQS_NULL_CODE,
                                AQS_QUAL_CODES = a.AQS_QUAL_CODES,
                                LVL1_VAL_IND = a.LVL1_VAL_IND,
                                LVL1_VAL_USERIDX = a.LVL1_VAL_USERIDX,
                                LVL1_VAL_USER = u1.FNAME + " " + u1.LNAME,
                                LVL1_VAL_DT = a.LVL1_VAL_DT,
                                LVL2_VAL_IND = a.LVL2_VAL_IND,
                                LVL2_VAL_USERIDX = a.LVL2_VAL_USERIDX,
                                LVL2_VAL_USER = u2.FNAME + " " + u2.LNAME,
                                LVL2_VAL_DT = a.LVL2_VAL_DT,
                                NOTES = a.NOTES,
                                AQSReadyInd = ((a.DATA_VALUE_NUM != null && u3.UNIT_CODE != null) || a.AQS_NULL_CODE != null)
                            }).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<string> GetT_QREST_DATA_HOURLY_NotificationUsers()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_HOURLY.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            join sn in ctx.T_QREST_SITE_NOTIFY.AsNoTracking() on m.SITE_IDX equals sn.SITE_IDX
                            where a.VAL0_NOTIFY_IND == false
                            && a.VAL_IND == true
                            && a.VAL_CD != null
                            select sn.NOTIFY_USER_IDX).Distinct().ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<RawDataDisplay> GetT_QREST_DATA_HOURLY_NotificationsListForUser(string userIdx)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_HOURLY.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                            join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                            join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                            join sn in ctx.T_QREST_SITE_NOTIFY.AsNoTracking() on m.SITE_IDX equals sn.SITE_IDX
                            where a.VAL0_NOTIFY_IND == false
                            && a.VAL_IND == true
                            && a.VAL_CD != null
                            && sn.NOTIFY_USER_IDX == userIdx
                            select new RawDataDisplay
                            {
                                ORG_ID = s.ORG_ID,
                                SITE_ID = s.SITE_ID,
                                MONITOR_IDX = m.MONITOR_IDX,
                                POC = m.POC,
                                PAR_CODE = p.PAR_CODE,
                                PAR_NAME = p.PAR_NAME,
                                VAL_CD = a.VAL_CD
                            }).Distinct().ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<T_QREST_DATA_HOURLY> GetT_QREST_DATA_HOURLY_NotNotified()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_HOURLY.AsNoTracking()
                            where a.VAL0_NOTIFY_IND == false
                            && a.VAL_IND == true
                            && a.VAL_CD != null
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<RawDataDisplay> GetT_QREST_DATA_HOURLY_AQSReady(Guid site, Guid mon, DateTime DateFrom, DateTime DateTo)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_DATA_HOURLY.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                            join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                            join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                            join u3 in ctx.T_QREST_REF_UNITS.AsNoTracking() on a.UNIT_CODE equals u3.UNIT_CODE
                                into lj3
                            from u3 in lj3.DefaultIfEmpty() //left join on hourly unit
                            where a.DATA_DTTM_LOCAL >= DateFrom
                            && a.DATA_DTTM_LOCAL <= DateTo
                            && m.SITE_IDX == site
                            && m.MONITOR_IDX == mon
                            && a.LVL2_VAL_IND == true
                            orderby a.DATA_DTTM_LOCAL ascending, p.PAR_CODE
                            select new RawDataDisplay
                            {
                                ORG_ID = s.ORG_ID,
                                SITE_ID = s.SITE_ID,
                                AQS_SITE_ID = s.AQS_SITE_ID,
                                STATE_CD = s.STATE_CD,
                                COUNTY_CD = s.COUNTY_CD,
                                MONITOR_IDX = m.MONITOR_IDX,
                                DATA_RAW_IDX = a.DATA_HOURLY_IDX,
                                DATA_DTTM = a.DATA_DTTM_LOCAL,
                                DATA_VALUE = a.DATA_VALUE,
                                PAR_CODE = p.PAR_CODE,
                                PAR_NAME = p.PAR_NAME,
                                UNIT_CODE = u3.UNIT_CODE ?? m.COLLECT_UNIT_CODE,
                                UNIT_DESC = u3.UNIT_DESC,
                                POC = m.POC,
                                METHOD_CODE = pm.METHOD_CODE,
                                VAL_IND = a.VAL_IND,
                                VAL_CD = a.VAL_CD,
                                AQS_NULL_CODE = a.AQS_NULL_CODE,
                                LVL1_VAL_IND = a.LVL1_VAL_IND,
                                LVL1_VAL_USERIDX = a.LVL1_VAL_USERIDX,
                                LVL1_VAL_DT = a.LVL1_VAL_DT,
                                LVL2_VAL_IND = a.LVL2_VAL_IND,
                                LVL2_VAL_USERIDX = a.LVL2_VAL_USERIDX,
                                LVL2_VAL_DT = a.LVL2_VAL_DT,
                                NOTES = a.NOTES
                            }).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }
        

        public static ImportResponse InsertUpdateT_QREST_DATA_HOURLY(Guid mONITOR_IDX, DateTime? dATA_DTTM_LOCAL, DateTime? dATA_DTTM_UTC, int timezoneOffset, string dATA_VALUE, string uNIT_CODE, bool? vAL_IND, string vAL_CD, Guid? iMPORT_ID = null)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    bool insInd = false;

                    if (dATA_DTTM_LOCAL != null && dATA_DTTM_UTC == null)
                        dATA_DTTM_UTC = dATA_DTTM_LOCAL.GetValueOrDefault().AddHours(timezoneOffset * (-1));
                    else if (dATA_DTTM_LOCAL == null && dATA_DTTM_UTC != null)
                        dATA_DTTM_LOCAL = dATA_DTTM_UTC.GetValueOrDefault().AddHours(timezoneOffset);

                    T_QREST_DATA_HOURLY e = (from c in ctx.T_QREST_DATA_HOURLY
                                             where c.MONITOR_IDX == mONITOR_IDX
                                             && c.DATA_DTTM_LOCAL == dATA_DTTM_LOCAL
                                             select c).FirstOrDefault();

                    //insert case
                    if (e == null)  
                    {
                        insInd = true;
                        e = new T_QREST_DATA_HOURLY();
                        e.DATA_HOURLY_IDX = Guid.NewGuid();
                        e.MONITOR_IDX = mONITOR_IDX;
                        e.DATA_DTTM_LOCAL = dATA_DTTM_LOCAL;
                        e.DATA_DTTM_UTC = dATA_DTTM_UTC;
                    }

                    if (dATA_VALUE != null)
                    {
                        //numeric
                        if (Decimal.TryParse(dATA_VALUE, out var valNum))
                        {
                            //remove leading zeroes and any plus symbols
                            e.DATA_VALUE = dATA_VALUE.Replace("+","").TrimStart(new Char[] { '0' });

                            //if negative with leading zeroes, just write the numeric as string
                            if (dATA_VALUE.Length > 1 && dATA_VALUE.Substring(0, 2) == "-0")
                                e.DATA_VALUE = valNum.ToString();

                            e.DATA_VALUE_NUM = valNum;
                        }
                        else  //non-numeric
                        {
                            e.DATA_VALUE_NUM = null;

                            //lookup AQS Null Code
                            if (db_Ref.GetT_QREST_REF_QUALIFIER_LookupNull(dATA_VALUE))
                                e.AQS_NULL_CODE = dATA_VALUE;
                            //lookup AQS Qual Codes
                            else if (db_Ref.GetT_QREST_REF_QUALIFIER_LookupNotNull(dATA_VALUE.Replace("*","")))
                            {
                                e.AQS_QUAL_CODES = dATA_VALUE.Replace("*", "");
                            }
                            else
                                e.VAL_CD = dATA_VALUE;
                        }



                    }

                    if (uNIT_CODE != null) e.UNIT_CODE = uNIT_CODE;
                    if (vAL_IND != null) e.VAL_IND = vAL_IND;
                    if (vAL_CD != null) e.VAL_CD = vAL_CD.SubStringPlus(0, 5);
                    if (iMPORT_ID != null) e.IMPORT_IDX = iMPORT_ID;
                    e.MODIFY_DT = System.DateTime.Now;


                    if (insInd)
                        ctx.T_QREST_DATA_HOURLY.Add(e);
                    
                    //ctx.Configuration.AutoDetectChangesEnabled = false;
                        
                    ctx.SaveChanges();

                    return new ImportResponse
                    {
                        SuccInd = true,
                        DATA_IDX = e.DATA_HOURLY_IDX,
                        DATA_DTTM = e.DATA_DTTM_UTC,
                        DATA_VALUE = e.DATA_VALUE
                    };
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return new ImportResponse
                    {
                        SuccInd = false,
                        DATA_IDX = null,
                        DATA_DTTM = dATA_DTTM_UTC,
                        DATA_VALUE = dATA_VALUE
                    };
                }
            }
        }



        public static bool InsertT_QREST_DATA_HOURLY_fromLine(string line, SitePollingConfigType config, List<SitePollingConfigDetailType> config_dtl, int timeZoneOffset)
        {
            try
            {
                //preview file to determine if tab or comma separated
                int delComma = line.Split(new char[] { ',' }, StringSplitOptions.None).Count();
                int delTab = line.Split(new char[] { '\t' }, StringSplitOptions.None).Count();
                char[] delimiter = delComma > delTab ? new char[] { ',' } : new char[] { '\t' };

                string[] cols = line.Split(delimiter);

                if (cols.Length > 1)  //skip blank row
                {
                    //date
                    string sDate = cols[config.DATE_COL.GetValueOrDefault() - 1].ToString().Replace("\"", "").Trim();
                    string sTime = cols[config.TIME_COL.GetValueOrDefault() - 1].ToString().Replace("\"", "").Trim();
                    //int timeZoneOffset = config.LOCAL_TIMEZONE.ConvertOrDefault<int>();
                    string timePollType = config.TIME_POLL_TYPE;
                    string dateTimeString = config.DATE_COL.GetValueOrDefault() != config.TIME_COL.GetValueOrDefault()
                        ? (sDate + " " + sTime).Trim()
                        : sDate;

                    //raw date time coming from logger
                    //get allowed date/time formats
                    string[] allowedFormats = UtilsText.GetDateTimeAllowedFormats(config.DATE_FORMAT, config.TIME_FORMAT);

                    DateTime dt = DateTime.ParseExact(dateTimeString, allowedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);

                    foreach (SitePollingConfigDetailType _map in config_dtl)
                    {
                        //apply hourly alerts if available
                        string valCd = "";
                        double? val = cols[_map.COL - 1 ?? 0].ToString().Replace("+","").ConvertOrDefault<double?>();
                        if (val != null && _map.ALERT_MAX_TYPE == "H" && val > _map.ALERT_MAX_VALUE)
                            valCd = "MAX";
                        if (val != null && _map.ALERT_MIN_TYPE == "H" && val < _map.ALERT_MIN_VALUE)
                            valCd = "MIN";

                        db_Air.InsertUpdateT_QREST_DATA_HOURLY(_map.MONITOR_IDX, timePollType == "L" ? dt.ConvertOrDefault<DateTime?>() : null, timePollType == "U" ? dt.ConvertOrDefault<DateTime?>() : null, timeZoneOffset, cols[_map.COL - 1 ?? 0], _map.COLLECT_UNIT_CODE, true, valCd);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                db_Ref.CreateT_QREST_SYS_LOG(null, "POLLING", "Site " + config.SITE_ID + " " + ex.Message ?? ex.InnerException?.ToString());
                return false;
            }

        }



        public static bool InsertT_QREST_DATA_HOURLY_fromLine_ESC(string line, SitePollingConfigType config, List<SitePollingConfigDetailType> config_dtl, int timeZoneOffset)
        {
            try
            {
                if (line.Length > 20 && line.Length < 40)  //skip blank row
                {
                    //date
                    int year = DateTime.Now.Year;  //year
                    int julday = line.SubStringPlus(9, 3).ConvertOrDefault<int>();  //day
                    int hour = line.SubStringPlus(12, 2).ConvertOrDefault<int>(); //hour
                    int minute = line.SubStringPlus(14, 2).ConvertOrDefault<int>(); //minute
                    DateTime dt = new DateTime(year, 1, 1).AddDays(julday - 1).AddHours(hour).AddMinutes(minute);
                    string timePollType = config.TIME_POLL_TYPE;

                    //read channel #
                    int channel = line.SubStringPlus(3, 2).ConvertOrDefault<int>();
                    
                    //lookup parameter that matches channel
                    SitePollingConfigDetailType _map = config_dtl.Where(c => c.COL == channel).FirstOrDefault();
                    if (_map != null)
                    {
                        //read value 
                        double? val = line.SubStringPlus(18, 10).ConvertOrDefault<Double?>();

                        //apply hourly alerts if available
                        string valCd = "";
                        if (val != null && _map.ALERT_MAX_TYPE == "H" && val > _map.ALERT_MAX_VALUE)
                            valCd = "MAX";
                        if (val != null && _map.ALERT_MIN_TYPE == "H" && val < _map.ALERT_MIN_VALUE)
                            valCd = "MIN";

                        string valStr = val.ToString();

                        //read for possible validation code
                        string flag1 = line.SubStringPlus(28, 1);
                        string flag2 = line.SubStringPlus(29, 1);
                        if (val != null && (flag1 == "<" || flag2 == "<"))
                            valStr = "FEW";
                            
                        //insert value
                        InsertUpdateT_QREST_DATA_HOURLY(_map.MONITOR_IDX, timePollType == "L" ? dt.ConvertOrDefault<DateTime?>() : null, timePollType == "U" ? dt.ConvertOrDefault<DateTime?>() : null, timeZoneOffset, valStr, _map.COLLECT_UNIT_CODE, true, valCd);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                db_Ref.CreateT_QREST_SYS_LOG(null, "POLLING", "Site " + config.SITE_ID + " " + ex.Message ?? ex.InnerException?.ToString());
                return false;
            }

        }


        public static bool InsertT_QREST_DATA_HOURLY_fromLine_BAM(string line, SitePollingConfigType config, List<SitePollingConfigDetailType> config_dtl, int timeZoneOffset)
        {
            try
            {
                if (line.Length > 20 && line.Length < 100 && line.Substring(2,1) == ":")  //skip blank row
                {
                    //date
                    int hour = line.SubStringPlus(0, 2).ConvertOrDefault<int>(); //hour
                    DateTime dt = System.DateTime.Today.AddHours(hour);
                    string timePollType = config.TIME_POLL_TYPE;

                    //read channel #
                    int channel = 1;

                    //lookup parameter that matches channel
                    SitePollingConfigDetailType _map = config_dtl.Where(c => c.COL == channel).FirstOrDefault();
                    if (_map != null)
                    {
                        //read value 
                        double? val = line.SubStringPlus(21, 5).ConvertOrDefault<Double?>();


                        //apply hourly alerts if available
                        string valCd = "";
                        if (val == 985)                         //apply special error
                            valCd = "LOST";
                        if (val != null && _map.ALERT_MAX_TYPE == "H" && val > _map.ALERT_MAX_VALUE)
                            valCd = "MAX";
                        if (val != null && _map.ALERT_MIN_TYPE == "H" && val < _map.ALERT_MIN_VALUE)
                            valCd = "MIN";

                        string valStr = val.ToString();

                        //insert value
                        InsertUpdateT_QREST_DATA_HOURLY(_map.MONITOR_IDX, timePollType == "L" ? dt.ConvertOrDefault<DateTime?>() : null, timePollType == "U" ? dt.ConvertOrDefault<DateTime?>() : null, timeZoneOffset, valStr, _map.COLLECT_UNIT_CODE, true, valCd);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                db_Ref.CreateT_QREST_SYS_LOG(null, "POLLING", "Site " + config.SITE_ID + " " + ex.Message ?? ex.InnerException?.ToString());
                return false;
            }

        }


        public static Guid? UpdateT_QREST_DATA_HOURLY_Notified(Guid dATA_HOURLY_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_DATA_HOURLY e = (from c in ctx.T_QREST_DATA_HOURLY
                                             where c.DATA_HOURLY_IDX == dATA_HOURLY_IDX
                                             select c).FirstOrDefault();

                    if (e != null)
                    {
                        e.VAL0_NOTIFY_IND = true;
                        ctx.SaveChanges();
                        return e.DATA_HOURLY_IDX;
                    }
                    return null;

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static Tuple<Guid?, string> UpdateT_QREST_DATA_HOURLY(Guid dATA_HOURLY_IDX, string aQS_NULL_CODE, bool? lVL1_VAL_IND, bool? lVL2_VAL_IND, string lVL_VAL_USERIDX, string uNIT_CODE, string nOTES, string dATA_VALUE, string vAL_CD, List<string> aQS_QUAL_CODES)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_DATA_HOURLY e = (from c in ctx.T_QREST_DATA_HOURLY
                                             where c.DATA_HOURLY_IDX == dATA_HOURLY_IDX
                                             select c).FirstOrDefault();

                    if (e != null)
                    {
                        if (aQS_NULL_CODE != null && aQS_NULL_CODE != "-1") e.AQS_NULL_CODE = aQS_NULL_CODE;  
                        else if (aQS_NULL_CODE == "-1") e.AQS_NULL_CODE = null;  //if "-1" then set to null

                        if (aQS_QUAL_CODES != null && aQS_QUAL_CODES.Count>0 && aQS_QUAL_CODES[0] != "-1") e.AQS_QUAL_CODES = String.Join("|", aQS_QUAL_CODES);  
                        else if (aQS_QUAL_CODES != null && aQS_QUAL_CODES.Count > 0 && aQS_QUAL_CODES[0] == "-1") e.AQS_QUAL_CODES = null;  //if "-1" then set to null

                        //perform Lvl1 validation 
                        if (lVL1_VAL_IND != null)
                        {
                            //prevent Lvl1 validation if date in future
                            if (e.DATA_DTTM_LOCAL > System.DateTime.Now.AddHours(24))
                                return new Tuple<Guid?,string>(null, "Cannot Lvl1 validate future data.");
                            
                            e.LVL1_VAL_IND = lVL1_VAL_IND;

                            if (lVL1_VAL_IND == true)
                            {
                                e.LVL1_VAL_USERIDX = lVL_VAL_USERIDX;
                                e.LVL1_VAL_DT = System.DateTime.Now;
                            }
                            else if (lVL1_VAL_IND == false)
                            {
                                e.LVL1_VAL_USERIDX = null;
                                e.LVL1_VAL_DT = null;
                            }
                        }

                        //perform Lvl2 validation
                        if (lVL2_VAL_IND != null)
                        {   
                            //prevent Lvl2 validation if date in future
                            if (e.DATA_DTTM_LOCAL > System.DateTime.Now.AddHours(24))
                                return new Tuple<Guid?, string>(null, "Cannot Lvl2 validate future data.");

                            e.LVL2_VAL_IND = lVL2_VAL_IND;

                            if (lVL2_VAL_IND == true)
                            {
                                e.LVL2_VAL_USERIDX = lVL_VAL_USERIDX;
                                e.LVL2_VAL_DT = System.DateTime.Now;
                            }
                            else if (lVL2_VAL_IND == false)
                            {
                                e.LVL2_VAL_USERIDX = null;
                                e.LVL2_VAL_DT = null;
                            }
                        }


                        if (uNIT_CODE != null) e.UNIT_CODE = uNIT_CODE;
                        if (nOTES != null) e.NOTES = nOTES;

                        if (dATA_VALUE != null && dATA_VALUE != "-999") 
                        { 
                            e.DATA_VALUE = dATA_VALUE;
                            e.DATA_VALUE_NUM = dATA_VALUE.ConvertOrDefault<decimal?>();
                        }
                        else if (dATA_VALUE == "-999")  //setting value to null
                        {
                            e.DATA_VALUE = null;
                            e.DATA_VALUE_NUM = null;
                        }

                        if (vAL_CD != null && vAL_CD != "-999") e.VAL_CD = vAL_CD;
                        else if (vAL_CD == "-999") e.VAL_CD = null;

                        ctx.SaveChanges();
                        return new Tuple<Guid?, string>(e.DATA_HOURLY_IDX, null);
                    }

                    return new Tuple<Guid?, string>(null, "Cannot find record to update.");

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return new Tuple<Guid?, string>(null, "Error updating hourly record.");

                }
            }
        }


        public static int DeleteT_QREST_DATA_HOURLY(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_DATA_HOURLY rec = new T_QREST_DATA_HOURLY { DATA_HOURLY_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException dbex)
                {
                    //if trying to delete record already deleted
                    foreach (System.Data.Entity.Infrastructure.DbEntityEntry entry in dbex.Entries)
                    {
                    }
                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }


        public static int DeleteT_QREST_DATA_HOURLY_ByImportIDX(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    ctx.Database.ExecuteSqlCommand("DELETE FROM T_QREST_DATA_HOURLY WHERE IMPORT_IDX = {0}", id);

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }


        public static List<RawDataDisplay> GetT_QREST_DATA_HOURLY_ByImportIDX(Guid iMPORT_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_HOURLY.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                            join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                            join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                            where a.IMPORT_IDX == iMPORT_IDX
                            orderby a.DATA_DTTM_LOCAL, p.PAR_NAME
                            select new RawDataDisplay
                            {
                                ORG_ID = s.ORG_ID,
                                SITE_ID = s.SITE_ID,
                                MONITOR_IDX = m.MONITOR_IDX,
                                POC = m.POC,
                                PAR_CODE = p.PAR_CODE,
                                PAR_NAME = p.PAR_NAME,
                                DATA_DTTM = a.DATA_DTTM_LOCAL,
                                DATA_VALUE = a.DATA_VALUE,
                                VAL_CD = a.VAL_CD,
                            }).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static Tuple<DateTime, DateTime?, double> GetT_QREST_DATA_HOURLY_MostRecentRecord()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    DateTime utcNow = DateTime.UtcNow;
                    DateTime? LastHourSampledNew = ctx.T_QREST_DATA_HOURLY.Max(p => p.DATA_DTTM_UTC);
                    TimeSpan SamplingAge = utcNow - (LastHourSampledNew ?? utcNow.AddHours(-12));

                    return Tuple.Create(utcNow, LastHourSampledNew, SamplingAge.TotalHours);
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }



        //*****************DATA_IMPORTS**********************************
        public static T_QREST_DATA_IMPORTS GetT_QREST_DATA_IMPORTS_byID(Guid iMPORT_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_IMPORTS.AsNoTracking()
                            where a.IMPORT_IDX == iMPORT_IDX
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<T_QREST_DATA_IMPORTS> GetT_QREST_DATA_IMPORTS_ByStatus(string sUBMISSION_STATUS)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_IMPORTS.AsNoTracking()
                            where a.SUBMISSION_STATUS == sUBMISSION_STATUS
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_QREST_DATA_IMPORTS GetT_QREST_DATA_IMPORTS_StartedByUser(string UserIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_IMPORTS.AsNoTracking()
                            where a.SUBMISSION_STATUS == "STARTED"
                            && a.IMPORT_USERIDX == UserIDX
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<ImportListDisplay> GetT_QREST_DATA_IMPORTS_byORG_ID(string oRG_ID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_IMPORTS.AsNoTracking()
                            join b in ctx.T_QREST_SITES.AsNoTracking() on a.SITE_IDX equals b.SITE_IDX
                            join u in ctx.T_QREST_USERS.AsNoTracking() on a.IMPORT_USERIDX equals u.USER_IDX
                                into lj1
                            from u in lj1.DefaultIfEmpty() //left join on submitter
                            where a.ORG_ID == oRG_ID
                            orderby a.IMPORT_DT descending
                            select new ImportListDisplay
                            {
                                T_QREST_DATA_IMPORTS = a,
                                SITE_ID = b.SITE_ID,
                                SITE_NAME = b.SITE_NAME,
                                SUBMITTER = u.FNAME + " " + u.LNAME,
                                COUNT = 0
//                                (a.IMPORT_TYPE == "H" || a.IMPORT_TYPE == "H1" || a.IMPORT_TYPE=="A") ? (from h in ctx.T_QREST_DATA_HOURLY where h.IMPORT_IDX == a.IMPORT_IDX select h).Count() : 
//                                a.IMPORT_TYPE == "F" ? (from h in ctx.T_QREST_DATA_FIVE_MIN where h.IMPORT_IDX == a.IMPORT_IDX select h).Count() : 0
                            }).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? InsertUpdateT_QREST_DATA_IMPORTS(Guid? iMPORT_IDX, string oRG_ID, Guid? sITE_IDX, string cOMMENT, string sUBMISSION_STATUS, string UserIDX,
            DateTime? iMPORT_DT, string sUBMISSION_FILE, string iMPORT_TYPE, Guid? mONITOR_IDX, Guid? pOLL_CONFIG_IDX, bool? rECALC_IND)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    bool insInd = false;

                    T_QREST_DATA_IMPORTS e = (from c in ctx.T_QREST_DATA_IMPORTS
                                              where c.IMPORT_IDX == iMPORT_IDX
                                              select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_DATA_IMPORTS();
                        e.IMPORT_IDX = iMPORT_IDX ?? Guid.NewGuid();
                        e.SITE_IDX = sITE_IDX.GetValueOrDefault();
                        e.ORG_ID = oRG_ID;
                        e.IMPORT_DT = iMPORT_DT;
                    }

                    if (cOMMENT != null) e.COMMENT = cOMMENT;
                    if (sUBMISSION_STATUS != null) e.SUBMISSION_STATUS = sUBMISSION_STATUS;
                    if (UserIDX != null) e.IMPORT_USERIDX = UserIDX;
                    if (sUBMISSION_FILE != null) e.SUBMISSION_FILE = sUBMISSION_FILE;
                    if (iMPORT_TYPE != null) e.IMPORT_TYPE = iMPORT_TYPE;
                    if (mONITOR_IDX != null) e.MONITOR_IDX = mONITOR_IDX;
                    if (pOLL_CONFIG_IDX != null) e.POLL_CONFIG_IDX = pOLL_CONFIG_IDX;
                    if (rECALC_IND != null) e.RECALC_IND = rECALC_IND;

                    if (insInd)
                        ctx.T_QREST_DATA_IMPORTS.Add(e);
                    ctx.SaveChanges();

                    return e.IMPORT_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_QREST_DATA_IMPORTS(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_DATA_IMPORTS rec = new T_QREST_DATA_IMPORTS { IMPORT_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }



        //*****************DATA_IMPORT_TEMP**********************************
        public static T_QREST_DATA_IMPORT_TEMP InsertT_QREST_DATA_IMPORT_TEMP(Guid mONITOR_IDX, string dateTimeString, int timeZoneOffset, string timePollType, string dATA_VALUE, 
            string uNIT_CODE, string UserIDX, Guid iMPORT_IDX, string[] allowedFormats)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    ctx.Configuration.AutoDetectChangesEnabled = false;

                    T_QREST_DATA_IMPORT_TEMP f = new T_QREST_DATA_IMPORT_TEMP
                    {
                        DATA_IMPORT_TEMP_IDX = Guid.NewGuid(),
                        IMPORT_USER_IDX = UserIDX,
                        MONITOR_IDX = mONITOR_IDX,
                        UNIT_CODE = uNIT_CODE,
                        IMPORT_DT = System.DateTime.Now,
                        IMPORT_VAL_IND = true,
                        IMPORT_DUP_IND = false,
                        IMPORT_IDX = iMPORT_IDX
                    };


                    DateTime dtRaw;
                    if (DateTime.TryParseExact(dateTimeString.Replace("\"", "").Trim(), allowedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtRaw))
                    {
                        f.DATA_DTTM_UTC = (timePollType == "U" ? dtRaw : dtRaw.AddHours(timeZoneOffset * (-1)));
                        f.DATA_DTTM_LOCAL = (timePollType == "L" ? dtRaw : dtRaw.AddHours(timeZoneOffset));
                    }
                    else
                    {
                        f.IMPORT_VAL_IND = false;
                        f.IMPORT_MSG = "Datetime cannot be read";
                    }



                    // EXISTING RECORD VALIDATION
                    T_QREST_DATA_HOURLY e = (from c in ctx.T_QREST_DATA_HOURLY.AsNoTracking()
                                             where c.MONITOR_IDX == mONITOR_IDX
                                             && c.DATA_DTTM_LOCAL == f.DATA_DTTM_LOCAL
                                             select c).FirstOrDefault();
                    if (e != null)
                    {
                        f.DATA_ORIG_TABLE_IDX = e.DATA_HOURLY_IDX;
                        f.IMPORT_DUP_IND = true;
                    }


                    //DATA VALUE VALIDATION/POPULATION
                    if (dATA_VALUE != null)
                    {
                        //if numeric, store as numeric
                        if (Decimal.TryParse(dATA_VALUE, out decimal val_num))
                        {
                            f.DATA_VALUE = dATA_VALUE;
                            f.DATA_VALUE_NUM = val_num;
                        }
                        else  //non-numeric
                        {
                            dATA_VALUE = dATA_VALUE.Replace("*", "");

                            //fail if nonnumeric too long
                            if (dATA_VALUE.Length > 5)
                            {
                                f.IMPORT_VAL_IND = false;
                                f.IMPORT_MSG = "Non-numeric and string length > 5";
                            }
                            //save as AQS Null Code if matched
                            else if (db_Ref.GetT_QREST_REF_QUALIFIER_LookupNull(dATA_VALUE))
                                f.AQS_NULL_CODE = dATA_VALUE;
                            //lookup AQS Qual Codes
                            else if (db_Ref.GetT_QREST_REF_QUALIFIER_LookupNotNull(dATA_VALUE))
                                f.AQS_QUAL_CODES = dATA_VALUE;
                            else
                                f.VAL_CD = dATA_VALUE;
                        }

                        //ctx.Configuration.ValidateOnSaveEnabled = false;

                        ctx.T_QREST_DATA_IMPORT_TEMP.Add(f);
                        ctx.SaveChanges();
                    }

                    return f;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
                finally
                {
                    ctx.Configuration.AutoDetectChangesEnabled = true;
                }
            }
        }


        public static bool BulkInsertT_QREST_DATA_IMPORT_TEMP_H(string[] bulkImportRows, T_QREST_SITE_POLL_CONFIG _pollConfig, string UserIDX, Guid iMPORT_IDX, string[] allowedFormats, int timeZoneOffset, string iMPORT_TYPE)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    //get polling config dtl
                    List<SitePollingConfigDetailType> _pollConfigDtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_pollConfig.POLL_CONFIG_IDX, false);

                    //int iCount = 0;

                    var _impList = new List<T_QREST_DATA_IMPORT_TEMP>();

                    ctx.Configuration.AutoDetectChangesEnabled = false;

                    //initial stuff from config
                    int delComma = bulkImportRows[0].Split(new char[] { ',' }, StringSplitOptions.None).Count();
                    int delTab = bulkImportRows[0].Split(new char[] { '\t' }, StringSplitOptions.None).Count();
                    char[] delimiter = delComma > delTab ? new char[] { ',' } : new char[] { '\t' };


                    int dateCol = (_pollConfig.DATE_COL ?? 2) - 1;
                    int timeCol = (_pollConfig.TIME_COL ?? 3) - 1;
                    //int timeZoneOffset = _pollConfig.LOCAL_TIMEZONE.ConvertOrDefault<int>();
                    string timePollType = _pollConfig.TIME_POLL_TYPE;


                    foreach (string row in bulkImportRows)
                    {
                        //split row's columns into string array
                        string[] cols = row.Split(delimiter, StringSplitOptions.None);
                        if (cols.Length > 1) //skip blank rows
                        {
                            string dateTimeString = dateCol != timeCol 
                                ? (cols[dateCol].Replace("\"", "").Trim() + " " + cols[timeCol].Replace("\"", "").Trim()).Trim() 
                                : cols[dateCol].Replace("\"", "").Trim();

                            foreach (SitePollingConfigDetailType _item in _pollConfigDtl)
                            {
                                //****************************** START COLUMN POLLUTANT READING******************************************************************************
                                string dATA_VALUE = cols[(_item.COL ?? 1) - 1]?.Trim();

                                if (dATA_VALUE.Length > 0)
                                {
                                    T_QREST_DATA_IMPORT_TEMP f = new T_QREST_DATA_IMPORT_TEMP
                                    {
                                        DATA_IMPORT_TEMP_IDX = Guid.NewGuid(),
                                        IMPORT_USER_IDX = UserIDX,
                                        MONITOR_IDX = _item.MONITOR_IDX,
                                        UNIT_CODE = _item.COLLECT_UNIT_CODE,
                                        IMPORT_DT = System.DateTime.Now,
                                        IMPORT_VAL_IND = true,
                                        IMPORT_DUP_IND = false,
                                        IMPORT_IDX = iMPORT_IDX
                                    };

                                    if (DateTime.TryParseExact(dateTimeString, allowedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtRaw))
                                    {
                                        if (dtRaw.Second != 0)
                                        {
                                            f.IMPORT_VAL_IND = false;
                                            f.IMPORT_MSG = "Seconds must be 0 for import";
                                        }
                                        else
                                        {
                                            f.DATA_DTTM_UTC = (timePollType == "U" ? dtRaw : dtRaw.AddHours(timeZoneOffset * (-1)));
                                            f.DATA_DTTM_LOCAL = (timePollType == "L" ? dtRaw : dtRaw.AddHours(timeZoneOffset));
                                        }
                                    }
                                    else
                                    {
                                        f.IMPORT_VAL_IND = false;
                                        f.IMPORT_MSG = "Datetime cannot be read";
                                    }

                                    //DATA VALUE VALIDATION/POPULATION
                                    //if numeric, store as numeric
                                    if (Decimal.TryParse(dATA_VALUE, out decimal val_num))
                                    {
                                        f.DATA_VALUE = dATA_VALUE;
                                        f.DATA_VALUE_NUM = val_num;
                                    }
                                    else  //non-numeric
                                    {
                                        dATA_VALUE = dATA_VALUE.Replace("*", "");

                                        //fail if non-numeric too long
                                        if (dATA_VALUE.Length > 5)
                                        {
                                            f.IMPORT_VAL_IND = false;
                                            f.IMPORT_MSG = "Non-numeric and string length > 5";
                                        }
                                        //reject if disallowed qualifier
                                        else if (db_Ref.IsDisallowedQualifier(_item.PAR_CODE, dATA_VALUE))
                                        {
                                            f.IMPORT_VAL_IND = false;
                                            f.IMPORT_MSG = "Qualifier " + dATA_VALUE + " not allowed for this par code";
                                        }
                                        //save as AQS Null Code if matched
                                        else if (db_Ref.GetT_QREST_REF_QUALIFIER_LookupNull(dATA_VALUE))
                                            f.AQS_NULL_CODE = dATA_VALUE;
                                        //lookup AQS Qual Codes
                                        else if (db_Ref.GetT_QREST_REF_QUALIFIER_LookupNotNull(dATA_VALUE))
                                            f.AQS_QUAL_CODES = dATA_VALUE;
                                        else
                                            f.VAL_CD = dATA_VALUE;
                                    }

                                    _impList.Add(f);
                                }
                                //****************************** END COLUMN POLLUTANT READING******************************************************************************
                            }
                        }
                    }

                    ctx.BulkInsert<T_QREST_DATA_IMPORT_TEMP>(_impList);
                    ctx.SaveChanges();

                    //now update for duplicates
                    if (iMPORT_TYPE == "H")
                        ctx.Database.ExecuteSqlCommand("UPDATE T set IMPORT_DUP_IND=1, DATA_ORIG_TABLE_IDX=H.DATA_HOURLY_IDX FROM T_QREST_DATA_IMPORT_TEMP T JOIN T_QREST_DATA_HOURLY H on T.MONITOR_IDX = H.MONITOR_IDX and T.DATA_DTTM_LOCAL = H.DATA_DTTM_LOCAL where T.IMPORT_IDX = '" + iMPORT_IDX + "'");
                    else if (iMPORT_TYPE == "F")
                        ctx.Database.ExecuteSqlCommand("UPDATE T set IMPORT_DUP_IND=1, DATA_ORIG_TABLE_IDX=H.DATA_FIVE_IDX FROM T_QREST_DATA_IMPORT_TEMP T JOIN T_QREST_DATA_FIVE_MIN H on T.MONITOR_IDX = H.MONITOR_IDX and T.DATA_DTTM_LOCAL = H.DATA_DTTM_LOCAL where T.IMPORT_IDX = '" + iMPORT_IDX + "'");

                    //return whether any dups or errors
                    return GetT_QREST_DATA_IMPORT_TEMP_DupCount(iMPORT_IDX) > 0 || GetT_QREST_DATA_IMPORT_TEMP_ErrorCount(iMPORT_IDX) > 0;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return true;
                }
                finally
                {
                    ctx.Configuration.AutoDetectChangesEnabled = true;
                }
            }
        }


        public static bool BulkInsertT_QREST_DATA_IMPORT_TEMP_AQS_RD(string[] bulkImportRows, string UserIDX, Guid iMPORT_IDX, Guid mONITOR_IDX, int timeZoneOffset)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var _impList = new List<T_QREST_DATA_IMPORT_TEMP>();

                    ctx.Configuration.AutoDetectChangesEnabled = false;

                    char[] delimiter = new char[] { '|' };                    

                    foreach (string row in bulkImportRows)
                    {
                        //split row's columns into string array
                        string[] cols = row.Split(delimiter, StringSplitOptions.None);
                        if (cols.Length > 1 && cols[0].SubStringPlus(0,1) != "#") //skip blank rows
                        {
                            T_QREST_DATA_IMPORT_TEMP f = new T_QREST_DATA_IMPORT_TEMP
                            {
                                DATA_IMPORT_TEMP_IDX = Guid.NewGuid(),
                                IMPORT_USER_IDX = UserIDX,
                                MONITOR_IDX = mONITOR_IDX,
                                UNIT_CODE = cols[8].Trim(),
                                IMPORT_DT = System.DateTime.Now,
                                IMPORT_VAL_IND = true,
                                IMPORT_DUP_IND = false,
                                IMPORT_IDX = iMPORT_IDX
                            };

                            string dateTimeString = cols[10].Trim() + " " + cols[11].Trim();


                            //storing/validating date and time
                            if (DateTime.TryParseExact(dateTimeString, "yyyyMMdd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtRaw))
                            {
                                f.DATA_DTTM_UTC = dtRaw.AddHours(timeZoneOffset * (-1));
                                f.DATA_DTTM_LOCAL = dtRaw;
                            }
                            else
                            {
                                f.IMPORT_VAL_IND = false;
                                f.IMPORT_MSG = "Datetime cannot be read";
                            }

                            //storing data value and numeric version
                            string dATA_VALUE = cols[12].Trim();
                            if (Decimal.TryParse(dATA_VALUE, out decimal val_num))
                            {
                                f.DATA_VALUE = dATA_VALUE;
                                f.DATA_VALUE_NUM = val_num;
                            }
                            else  //non-numeric
                            {
                                dATA_VALUE = dATA_VALUE.Replace("*", "");

                                //fail if non-numeric too long
                                if (dATA_VALUE.Length > 5)
                                {
                                    f.IMPORT_VAL_IND = false;
                                    f.IMPORT_MSG = "Non-numeric and string length > 5";
                                }
                                //save as AQS Null Code if matched
                                else if (db_Ref.GetT_QREST_REF_QUALIFIER_LookupNull(dATA_VALUE))
                                    f.AQS_NULL_CODE = dATA_VALUE;
                                //lookup AQS Qual Codes
                                else if (db_Ref.GetT_QREST_REF_QUALIFIER_LookupNotNull(dATA_VALUE))
                                    f.AQS_QUAL_CODES = dATA_VALUE;
                                else
                                    f.VAL_CD = dATA_VALUE;
                            }

                            //special AQS handling of storing AQS NULL CODE
                            if (cols[13].Trim() != "")
                                f.AQS_NULL_CODE = cols[13].Trim();

                            //special AQS handling of storing AQS QUALIFIER CODE
                            if (cols[16].Trim() != "")
                                f.AQS_QUAL_CODES = cols[16].Trim();


                            _impList.Add(f);

                        }
                    }

                    ctx.BulkInsert<T_QREST_DATA_IMPORT_TEMP>(_impList);
                    ctx.SaveChanges();

                    //now update for duplicates
                    ctx.Database.ExecuteSqlCommand("UPDATE T set IMPORT_DUP_IND=1, DATA_ORIG_TABLE_IDX=H.DATA_HOURLY_IDX FROM T_QREST_DATA_IMPORT_TEMP T JOIN T_QREST_DATA_HOURLY H on T.MONITOR_IDX = H.MONITOR_IDX and T.DATA_DTTM_LOCAL = H.DATA_DTTM_LOCAL where T.IMPORT_IDX = '" + iMPORT_IDX + "'");

                    //return whether any dups or errors
                    return GetT_QREST_DATA_IMPORT_TEMP_DupCount(iMPORT_IDX) > 0 || GetT_QREST_DATA_IMPORT_TEMP_ErrorCount(iMPORT_IDX) > 0;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return true;
                }
                finally
                {
                    ctx.Configuration.AutoDetectChangesEnabled = true;
                }
            }
        }



        public static bool BulkInsertT_QREST_DATA_IMPORT_TEMP_H1(string[] bulkImportRows, char[] delimiter, Guid mONITOR_IDX, int timeZoneOffset, string timePollType, 
            string uNIT_CODE, string UserIDX, Guid iMPORT_IDX, string[] allowedFormats)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    //int iCount = 0;
                    ctx.Configuration.AutoDetectChangesEnabled = false;

                    var _impList = new List<T_QREST_DATA_IMPORT_TEMP>();

                    foreach (string row in bulkImportRows)
                    {
                        //split row's columns into string array
                        string[] cols = row.Split(delimiter, StringSplitOptions.None);
                        if (cols.Length > 20 && cols[0] != "Date") //skip blank rows
                        {
                            for (int i = 0; i <= 23; i++)
                            {
                                //****************************** START ******************************************************************************
                                string dateTimeString = (cols[0] + " " + i + ":00").Trim();
                                string dATA_VALUE = cols[i + 1];

                                T_QREST_DATA_IMPORT_TEMP f = new T_QREST_DATA_IMPORT_TEMP
                                {
                                    DATA_IMPORT_TEMP_IDX = Guid.NewGuid(),
                                    IMPORT_USER_IDX = UserIDX,
                                    MONITOR_IDX = mONITOR_IDX,
                                    UNIT_CODE = uNIT_CODE,
                                    IMPORT_DT = System.DateTime.Now,
                                    IMPORT_VAL_IND = true,
                                    IMPORT_DUP_IND = false,
                                    IMPORT_IDX = iMPORT_IDX
                                };


                                if (DateTime.TryParseExact(dateTimeString, allowedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtRaw))
                                {
                                    f.DATA_DTTM_UTC = (timePollType == "U" ? dtRaw : dtRaw.AddHours(timeZoneOffset * (-1)));
                                    f.DATA_DTTM_LOCAL = (timePollType == "L" ? dtRaw : dtRaw.AddHours(timeZoneOffset));
                                }
                                else
                                {
                                    f.IMPORT_VAL_IND = false;
                                    f.IMPORT_MSG = "Datetime cannot be read";
                                }


                                //DATA VALUE VALIDATION/POPULATION

                                //if numeric, store as numeric
                                if (decimal.TryParse(dATA_VALUE, out decimal val_num))
                                {
                                    f.DATA_VALUE = dATA_VALUE;
                                    f.DATA_VALUE_NUM = val_num;
                                }
                                else  //non-numeric
                                {
                                    dATA_VALUE = dATA_VALUE.Replace("*", "");

                                    //fail if non-numeric too long
                                    if (dATA_VALUE.Length > 5)
                                    {
                                        f.IMPORT_VAL_IND = false;
                                        f.IMPORT_MSG = "Non-numeric and string length > 5";
                                    }
                                    //save as AQS Null Code if matched
                                    else if (db_Ref.GetT_QREST_REF_QUALIFIER_LookupNull(dATA_VALUE))
                                        f.AQS_NULL_CODE = dATA_VALUE;
                                    //lookup AQS Qual Codes
                                    else if (db_Ref.GetT_QREST_REF_QUALIFIER_LookupNotNull(dATA_VALUE))
                                        f.AQS_QUAL_CODES = dATA_VALUE;
                                    else
                                        f.VAL_CD = dATA_VALUE;
                                }

                                _impList.Add(f);

                                //ctx.T_QREST_DATA_IMPORT_TEMP.Add(f);

                                //iCount++;

                                //if (iCount == 100)
                                //{
                                //    ctx.SaveChanges();
                                //    iCount = 0;
                                //}
                                //****************************** END ******************************************************************************
                            }
                        }
                    }

                    ctx.BulkInsert<T_QREST_DATA_IMPORT_TEMP>(_impList);

                    //if (iCount>0)
                    
                    ctx.SaveChanges();


                    //now update for duplicates
                    ctx.Database.ExecuteSqlCommand("UPDATE T set IMPORT_DUP_IND=1, DATA_ORIG_TABLE_IDX=H.DATA_HOURLY_IDX FROM T_QREST_DATA_IMPORT_TEMP T JOIN T_QREST_DATA_HOURLY H on T.MONITOR_IDX = H.MONITOR_IDX and T.DATA_DTTM_LOCAL = H.DATA_DTTM_LOCAL where T.IMPORT_IDX = '" + iMPORT_IDX + "'");

                    //return whether any dups or errors
                    return GetT_QREST_DATA_IMPORT_TEMP_DupCount(iMPORT_IDX) > 0 || GetT_QREST_DATA_IMPORT_TEMP_ErrorCount(iMPORT_IDX) > 0;

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return true;
                }
                finally
                {
                    ctx.Configuration.AutoDetectChangesEnabled = true;
                }
            }
        }

        public static List<T_QREST_DATA_IMPORT_TEMPDisplay> GetT_QREST_DATA_IMPORT_TEMP_Dup(Guid iMPORT_IDX, int pageSize, int? skip, string orderBy, string orderDir = "asc")
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var _import = db_Air.GetT_QREST_DATA_IMPORTS_byID(iMPORT_IDX);

                    if (_import.IMPORT_TYPE == "F")
                    return (from a in ctx.T_QREST_DATA_IMPORT_TEMP.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS on a.MONITOR_IDX equals m.MONITOR_IDX
                            join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                            join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                            //left join on 5 min records 
                            join f in ctx.T_QREST_DATA_FIVE_MIN.AsNoTracking() on a.DATA_ORIG_TABLE_IDX equals f.DATA_FIVE_IDX into lj1
                            from u in lj1.DefaultIfEmpty() //left join on submitter

                            where a.IMPORT_IDX == iMPORT_IDX
                            && a.IMPORT_DUP_IND == true
                            select new T_QREST_DATA_IMPORT_TEMPDisplay
                            {
                                DATA_DTTM_LOCAL = a.DATA_DTTM_LOCAL,
                                DATA_DTTM_UTC = a.DATA_DTTM_UTC,
                                DATA_VALUE= a.DATA_VALUE,
                                UNIT_CODE = a.UNIT_CODE,
                                VAL_CD = a.VAL_CD,
                                AQS_NULL_CODE = a.AQS_NULL_CODE,
                                AQS_QUAL_CODES = a.AQS_QUAL_CODES,
                                PAR_CODE = p.PAR_CODE,
                                PAR_NAME = p.PAR_NAME,
                                DupOrigValue = u.DATA_VALUE
                            }).OrderBy(orderBy, orderDir).Skip(skip ?? 0).Take(pageSize).ToList();
                    else
                        return (from a in ctx.T_QREST_DATA_IMPORT_TEMP.AsNoTracking()
                                join m in ctx.T_QREST_MONITORS on a.MONITOR_IDX equals m.MONITOR_IDX
                                join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                                join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                                //left join on hourly records 
                                join f in ctx.T_QREST_DATA_HOURLY.AsNoTracking() on a.DATA_ORIG_TABLE_IDX equals f.DATA_HOURLY_IDX into lj1
                                from u in lj1.DefaultIfEmpty() //left join on submitter

                                where a.IMPORT_IDX == iMPORT_IDX
                                && a.IMPORT_DUP_IND == true
                                select new T_QREST_DATA_IMPORT_TEMPDisplay
                                {
                                    DATA_DTTM_LOCAL = a.DATA_DTTM_LOCAL,
                                    DATA_DTTM_UTC = a.DATA_DTTM_UTC,
                                    DATA_VALUE = a.DATA_VALUE,
                                    UNIT_CODE = a.UNIT_CODE,
                                    VAL_CD = a.VAL_CD,
                                    AQS_NULL_CODE = a.AQS_NULL_CODE,
                                    AQS_QUAL_CODES = a.AQS_QUAL_CODES,
                                    PAR_CODE = p.PAR_CODE,
                                    PAR_NAME =  p.PAR_NAME,
                                    DupOrigValue = u.DATA_VALUE
                                }).OrderBy(orderBy, orderDir).Skip(skip ?? 0).Take(pageSize).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<T_QREST_DATA_IMPORT_TEMPDisplay> GetT_QREST_DATA_IMPORT_TEMP_Error(Guid iMPORT_IDX, int pageSize, int? skip, string orderBy, string orderDir = "asc")
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_IMPORT_TEMP.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS on a.MONITOR_IDX equals m.MONITOR_IDX
                            join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                            join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                            where a.IMPORT_IDX == iMPORT_IDX
                            && a.IMPORT_VAL_IND == false
                            select new T_QREST_DATA_IMPORT_TEMPDisplay
                            {
                                DATA_DTTM_LOCAL = a.DATA_DTTM_LOCAL,
                                DATA_DTTM_UTC = a.DATA_DTTM_UTC,
                                DATA_VALUE = a.DATA_VALUE,
                                UNIT_CODE = a.UNIT_CODE,
                                VAL_CD = a.VAL_CD,
                                AQS_NULL_CODE = a.AQS_NULL_CODE,
                                AQS_QUAL_CODES = a.AQS_QUAL_CODES,
                                PAR_CODE = p.PAR_CODE,
                                PAR_NAME = p.PAR_NAME,
                                IMPORT_MSG = a.IMPORT_MSG
                            }).OrderBy(orderBy, orderDir).Skip(skip ?? 0).Take(pageSize).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int GetT_QREST_DATA_IMPORT_TEMP_DupCount(Guid iMPORT_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_IMPORT_TEMP.AsNoTracking()
                            where a.IMPORT_IDX == iMPORT_IDX
                            && a.IMPORT_DUP_IND == true
                            select a).Count();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int GetT_QREST_DATA_IMPORT_TEMP_ErrorCount(Guid iMPORT_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_IMPORT_TEMP.AsNoTracking()
                            where a.IMPORT_IDX == iMPORT_IDX
                            && a.IMPORT_VAL_IND == false
                            select a).Count();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int GetT_QREST_DATA_IMPORT_TEMP_Count(Guid iMPORT_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_IMPORT_TEMP.AsNoTracking()
                            where a.IMPORT_IDX == iMPORT_IDX
                            select a).Count();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static Guid? GetT_QREST_DATA_IMPORT_TEMP_ImportByUser(string UserIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_DATA_IMPORT_TEMP.AsNoTracking()
                            where a.IMPORT_USER_IDX == UserIDX
                            select a).FirstOrDefault();

                    if (xxx != null)
                        return xxx.IMPORT_IDX;
                    else
                        return null;

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }





        //*****************HOURLY LOG **********************************
        public static List<HourlyLogDisplay> GetT_QREST_DATA_HOURLY_LOG_ByHour(Guid dATA_HOURLY_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_DATA_HOURLY_LOG.AsNoTracking()
                            join b in ctx.T_QREST_DATA_HOURLY.AsNoTracking() on a.DATA_HOURLY_IDX equals b.DATA_HOURLY_IDX
                            join u in ctx.T_QREST_USERS.AsNoTracking() on a.MODIFY_USERIDX equals u.USER_IDX
                            where a.DATA_HOURLY_IDX == dATA_HOURLY_IDX
                            orderby a.MODIFY_DT descending
                            select new HourlyLogDisplay {
                                DATA_DTTM = b.DATA_DTTM_LOCAL,
                                DATA_HOURLY_IDX = a.DATA_HOURLY_IDX,
                                NOTES = a.NOTES,
                                MODIFY_DT = a.MODIFY_DT,
                                USER_NAME = u.FNAME + " " + u.LNAME
                            }).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? InsertUpdatetT_QREST_DATA_HOURLY_LOG(Guid? dATA_HOURLY_LOG_IDX, Guid? dATA_HOURLY_IDX, string nOTES, string cREATE_USER)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_QREST_DATA_HOURLY_LOG e = (from c in ctx.T_QREST_DATA_HOURLY_LOG
                                             where c.DATA_HOURLY_LOG_IDX == dATA_HOURLY_LOG_IDX
                                             select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_DATA_HOURLY_LOG();
                        e.DATA_HOURLY_LOG_IDX = Guid.NewGuid();
                    }

                    if (dATA_HOURLY_IDX != null) e.DATA_HOURLY_IDX = dATA_HOURLY_IDX.GetValueOrDefault();
                    if (nOTES != null) e.NOTES = nOTES;
                    e.MODIFY_DT = System.DateTime.Now;
                    e.MODIFY_USERIDX = cREATE_USER;

                    if (insInd)
                        ctx.T_QREST_DATA_HOURLY_LOG.Add(e);

                    ctx.SaveChanges();
                    return e.DATA_HOURLY_LOG_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }




        //*****************ASSESS DOCS**********************************
        public static T_QREST_ASSESS_DOCS GetT_QREST_ASSESS_DOCS_ByID(Guid aSSESS_DOC_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_ASSESS_DOCS.AsNoTracking()
                            where a.ASSESS_DOC_IDX == aSSESS_DOC_IDX
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<AssessDocDisplay> GetT_QREST_ASSESS_DOCS_BySite(Guid sITE_IDX, DateTime? startDate, DateTime? endDate)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    //  4/1 to 4/30 (filter range)

                    //documents
                    //---------------- 
                    //4/2 to 4/3  <-- incldue    I/I
                    //3/15 to 4/15 <-- include   N/I 
                    //3/15 to 5/15 <-- include   N/N
                    //4/15 to 5/15 <-- include   I/N
                    //3/15 to 3/30 <-- not include  N/N
                    //5/1 to 5/15 <-- not include    N/N

                    if (startDate == null) startDate = System.DateTime.Now.AddYears(-20);
                    if (endDate == null) endDate = System.DateTime.Now.AddYears(20);

                    return (from a in ctx.T_QREST_ASSESS_DOCS.AsNoTracking()
                            where a.SITE_IDX == sITE_IDX
                            && a.MONITOR_IDX == null
                            && ((a.START_DT >= startDate && a.START_DT <= endDate) || (a.END_DT >= startDate && a.END_DT <= endDate) || (a.START_DT <= startDate && a.END_DT >= endDate))
                            select new AssessDocDisplay
                            {
                                ASSESS_DOC_IDX = a.ASSESS_DOC_IDX,
                                DOC_NAME = a.DOC_NAME,
                                DOC_TYPE = a.DOC_TYPE,
                                DOC_FILE_TYPE = a.DOC_FILE_TYPE,
                                DOC_SIZE = a.DOC_SIZE,
                                DOC_COMMENT = a.DOC_COMMENT,
                                DOC_AUTHOR = a.DOC_AUTHOR,
                                START_DT = a.START_DT,
                                END_DT = a.END_DT
                            }).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<AssessDocDisplay> GetT_QREST_ASSESS_DOCS_ByMonitor(Guid mONITOR_IDX, DateTime? startDate, DateTime? endDate)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_ASSESS_DOCS.AsNoTracking()
                            where a.MONITOR_IDX == mONITOR_IDX
                            && ((a.START_DT >= startDate && a.START_DT <= endDate) || (a.END_DT >= startDate && a.END_DT <= endDate) || (a.START_DT <= startDate && a.END_DT >= endDate))
                            select new AssessDocDisplay
                            {
                                ASSESS_DOC_IDX = a.ASSESS_DOC_IDX,
                                DOC_NAME = a.DOC_NAME,
                                DOC_TYPE = a.DOC_TYPE,
                                DOC_FILE_TYPE = a.DOC_FILE_TYPE,
                                DOC_SIZE = a.DOC_SIZE,
                                DOC_COMMENT = a.DOC_COMMENT,
                                DOC_AUTHOR = a.DOC_AUTHOR,
                                START_DT = a.START_DT,
                                END_DT = a.END_DT
                            }).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int GetT_QREST_ASSESS_DOCS_CountApplyMonitor(Guid sITE_IDX, Guid mONITOR_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_ASSESS_DOCS.AsNoTracking()
                            where ((a.SITE_IDX == sITE_IDX && mONITOR_IDX == null)
                            || a.MONITOR_IDX == mONITOR_IDX)
                            select a).Count();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static Guid? InsertUpdatetT_QREST_ASSESS_DOCS(Guid? aSSESS_DOC_IDX, Guid? sITE_IDX, Guid? mONITOR_IDX, DateTime? sTART_DT, DateTime? eND_DT, byte[] dOC_CONTENT, string dOC_NAME, string dOC_TYPE,
            string dOC_FILE_TYPE, int? dOC_SIZE, string dOC_COMMENT, string dOC_AUTHOR, string cREATE_USER)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_QREST_ASSESS_DOCS e = (from c in ctx.T_QREST_ASSESS_DOCS
                                             where c.ASSESS_DOC_IDX == aSSESS_DOC_IDX
                                             select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_ASSESS_DOCS();
                        e.ASSESS_DOC_IDX = Guid.NewGuid();
                        e.SITE_IDX = sITE_IDX.GetValueOrDefault();
                        if (mONITOR_IDX != null) e.MONITOR_IDX = mONITOR_IDX.GetValueOrDefault();
                        e.CREATE_DT = System.DateTime.Now;
                        e.CREATE_USERIDX = cREATE_USER;
                    }
                    else
                    {
                        e.MODIFY_DT = System.DateTime.Now;
                        e.MODIFY_USERIDX = cREATE_USER;
                    }

                    if (sTART_DT != null) e.START_DT = sTART_DT;
                    if (eND_DT != null) e.END_DT = eND_DT;
                    if (dOC_CONTENT != null) e.DOC_CONTENT = dOC_CONTENT;
                    if (dOC_NAME != null) e.DOC_NAME = dOC_NAME;
                    if (dOC_TYPE != null) e.DOC_TYPE = dOC_TYPE;
                    if (dOC_FILE_TYPE != null) e.DOC_FILE_TYPE = dOC_FILE_TYPE;
                    if (dOC_SIZE != null) e.DOC_SIZE = dOC_SIZE;
                    if (dOC_COMMENT != null) e.DOC_COMMENT = dOC_COMMENT;
                    if (dOC_AUTHOR != null) e.DOC_AUTHOR = dOC_AUTHOR;

                    if (insInd)
                        ctx.T_QREST_ASSESS_DOCS.Add(e);

                    ctx.SaveChanges();
                    return e.ASSESS_DOC_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_QREST_ASSESS_DOCS(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_ASSESS_DOCS rec = new T_QREST_ASSESS_DOCS { ASSESS_DOC_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }




        //*****************AQS**********************************
        public static List<AQSDisplay> GetT_QREST_AQS_byORG_ID(string oRG_ID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_AQS.AsNoTracking()
                            join b in ctx.T_QREST_SITES.AsNoTracking() on a.SITE_IDX equals b.SITE_IDX
                            join u in ctx.T_QREST_USERS.AsNoTracking() on a.CREATE_USERIDX equals u.USER_IDX
                                into lj1
                            from u in lj1.DefaultIfEmpty() //left join on submitter
                            where a.ORG_ID == oRG_ID
                            orderby a.CREATE_DT descending
                            select new AQSDisplay {
                                AQS_IDX = a.AQS_IDX,
                                COMMENT = a.COMMENT, 
                                AQS_SUBMISSION_NAME = a.AQS_SUBMISSION_NAME, 
                                CREATE_DT = a.CREATE_DT, 
                                START_DT = a.START_DT, 
                                END_DT = a.END_DT,
                                SUBMISSION_STATUS = a.SUBMISSION_STATUS, 
                                SUBMISSION_SUBSTATUS = a.SUBMISSION_SUBSTATUS, 
                                DOWNLOAD_FILE_IND = (a.DOWNLOAD_FILE != null),
                                SITE_ID = b.SITE_ID,
                                SITE_NAME = b.SITE_NAME,
                                SUBMITTER = u.FNAME + " " + u.LNAME
                            }).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_QREST_AQS GetT_QREST_AQS_by_ID(Guid aQS_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_AQS.AsNoTracking()
                            where a.AQS_IDX == aQS_IDX
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? InsertUpdateT_QREST_AQS(Guid? aQS_IDX, string oRG_ID, Guid? sITE_IDX, string aQS_SUBMISSION_NAME, DateTime? sTART_DT, DateTime? eND_DT, 
            byte[] aQS_CONTENT, int? dOC_SIZE, string cOMMENT, string sUBMISSION_STATUS, string UserIDX, string aQS_CONTENT_XML, string tRANS_ID, byte[] dOWNLOAD_FILE, string sUBMISSION_SUBSTATUS = null)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    bool insInd = false;

                    T_QREST_AQS e = (from c in ctx.T_QREST_AQS
                                             where c.AQS_IDX == aQS_IDX
                                             select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_AQS();
                        e.AQS_IDX = Guid.NewGuid();
                        e.SITE_IDX = sITE_IDX.GetValueOrDefault();
                        e.ORG_ID = oRG_ID;
                        e.CREATE_USERIDX = UserIDX;
                        e.CREATE_DT = System.DateTime.Now;
                    }
                    else
                    {
                        e.MODIFY_USERIDX = UserIDX;
                        e.MODIFY_DT = System.DateTime.Now;
                    }

                    if (aQS_SUBMISSION_NAME != null) e.AQS_SUBMISSION_NAME = aQS_SUBMISSION_NAME;
                    if (sTART_DT != null) e.START_DT = sTART_DT.GetValueOrDefault();
                    if (eND_DT != null) e.END_DT = eND_DT.GetValueOrDefault();
                    if (aQS_CONTENT != null) e.AQS_CONTENT = aQS_CONTENT;
                    if (dOC_SIZE != null) e.DOC_SIZE = dOC_SIZE;
                    if (cOMMENT != null) e.COMMENT = cOMMENT;
                    if (sUBMISSION_STATUS != null) e.SUBMISSION_STATUS = sUBMISSION_STATUS;
                    if (aQS_CONTENT_XML != null) e.AQS_CONTENT_XML = aQS_CONTENT_XML;
                    if (tRANS_ID != null) e.CDX_TOKEN = tRANS_ID;
                    if (dOWNLOAD_FILE != null) e.DOWNLOAD_FILE = dOWNLOAD_FILE;
                    if (sUBMISSION_SUBSTATUS != null) e.SUBMISSION_SUBSTATUS = sUBMISSION_SUBSTATUS;

                    if (insInd)
                        ctx.T_QREST_AQS.Add(e);
                    ctx.SaveChanges();

                    return e.AQS_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static int DeleteT_QREST_AQS(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_AQS rec = new T_QREST_AQS { AQS_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }







        //*****************VIEWS**********************************
        //*****************VIEWS**********************************
        //*****************VIEWS**********************************
        public static MONITOR_SNAPSHOT GetMONITOR_SNAPSHOT_ByMonitor(Guid? mONITOR_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.MONITOR_SNAPSHOT.AsNoTracking()
                            where a.MONITOR_IDX == mONITOR_IDX
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<AIRNOW_LAST_HOUR> GetAIRNOW_LAST_HOUR()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.AIRNOW_LAST_HOUR.AsNoTracking()
                            orderby a.AIRNOW_SITE, a.PAR_CODE
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<SITE_HEALTH> GetSITE_HEALTH()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.SITE_HEALTH.AsNoTracking()
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<MONTHLY_USAGE_HOURLY> GetMONTHLY_USAGE_HOURLY(int yr)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.MONTHLY_USAGE_HOURLY.AsNoTracking()
                            where a.YR == yr
                            orderby a.MN
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<MONTHLY_USAGE_FIVEMIN> GetMONTHLY_USAGE_FIVEMIN(int yr)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.MONTHLY_USAGE_FIVEMIN.AsNoTracking()
                            where a.YR == yr
                            orderby a.MN
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        //*****************STORED PROCEDURES**********************************
        //*****************STORED PROCEDURES**********************************
        //*****************STORED PROCEDURES**********************************
        public static int SP_VALIDATE_HOURLY()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    ctx.Database.CommandTimeout = 600;
                    return ctx.SP_VALIDATE_HOURLY();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static async Task<List<SP_RPT_DAILY_Result>> SP_RPT_DAILY(Guid siteIDX, int mn, int yr, int dy, string time)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return await ctx.SP_RPT_DAILY(siteIDX, mn, yr, dy, time).ToListAsync();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SP_RPT_MONTHLY_Result> SP_RPT_MONTHLY(Guid monIDX, int mnth, int yr, string time)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_RPT_MONTHLY(monIDX, mnth, yr, time).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SP_RPT_MONTHLY_SUMS_Result> SP_RPT_MONTHLY_SUMS(Guid monIDX, int mnth, int yr, string time)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_RPT_MONTHLY_SUMS(monIDX, mnth, yr, time).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SP_RPT_ANNUAL_Result> SP_RPT_ANNUAL(Guid monIDX, int yr, string time)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_RPT_ANNUAL(monIDX, yr, time).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SP_RPT_ANNUAL_SUMS_Result> SP_RPT_ANNUAL_SUMS(Guid monIDX, int yr, string time)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_RPT_ANNUAL_SUMS(monIDX, yr, time).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SP_AQS_REVIEW_STATUS_Result> SP_AQS_REVIEW_STATUS(Guid siteIDX, DateTime adate, DateTime? edate)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    //if no edate, set to end of month
                    if (edate == null)
                        edate = adate.AddMonths(1).AddHours(-1);

                    return ctx.SP_AQS_REVIEW_STATUS(siteIDX, adate, edate.GetValueOrDefault()).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int SP_IMPORT_DATA_FROM_TEMP(Guid iMPORT_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_IMPORT_DATA_FROM_TEMP(iMPORT_IDX);
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int SP_VALIDATE_HOURLY_IMPORT(Guid iMPORT_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_VALIDATE_HOURLY_IMPORT(iMPORT_IDX);
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int SP_IMPORT_DETECT_DUPES(Guid iMPORT_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_IMPORT_DETECT_DUPES(iMPORT_IDX);
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int SP_FILL_LOST_DATA (DateTime sDate, DateTime eDate, Guid mONITOR_IDX, string tzOffet)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_FILL_LOST_DATA(sDate, eDate, mONITOR_IDX, tzOffet);
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static SP_COUNT_LOST_DATA_Result SP_COUNT_LOST_DATA(DateTime sDate, DateTime eDate, Guid mONITOR_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_COUNT_LOST_DATA(mONITOR_IDX, sDate, eDate).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SP_DAILY_AVG_Result> SP_DAILY_AVGS(DateTime sDate, DateTime eDate, Guid mONITOR_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_DAILY_AVG(sDate, eDate, mONITOR_IDX).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SP_MONTHLY_STATS_Result> SP_MONTHLY_STATS(DateTime sDate, DateTime eDate, Guid mONITOR_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_MONTHLY_STATS(sDate, eDate, mONITOR_IDX).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SP_FIVE_MIN_DATA_GAPS_Result> SP_FIVE_MIN_DATA_GAPS(Guid sITE_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_FIVE_MIN_DATA_GAPS(sITE_IDX).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static DateTime? SP_LATEST_POLLED_DATE(Guid sITE_IDX, string durCode, string timeType)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_LATEST_POLLED_DATE(sITE_IDX, durCode, timeType).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


    }
}
