using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using QRESTModel.BLL;

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
        public string LOGGER_PASSWORD { get; set; }
        public string DELIMITER { get; set; }
        public int? DATE_COL { get; set; }
        public string DATE_FORMAT { get; set; }
        public int? TIME_COL { get; set; }
        public string TIME_FORMAT { get; set; }
        public string TIME_POLL_TYPE { get; set; }
        public string LOCAL_TIMEZONE { get; set; }
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
        public string LOGGER_USERNAME { get; set; }
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
        public String ORG_ID { get; set; }
        public String SITE_ID { get; set; }
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
    }

    public class RawDataDisplay
    {
        public bool EditInd { get; set; }
        public Guid DATA_RAW_IDX { get; set; }
        public Guid MONITOR_IDX { get; set; }
        public string ORG_ID { get; set; }
        public string SITE_ID { get; set; }
        public string PAR_CODE { get; set; }
        public string PAR_NAME { get; set; }
        public string METHOD_CODE { get; set; }
        public int? POC { get; set; }
        public string UNIT_CODE { get; set; }
        public string UNIT_DESC { get; set; }
        public string COLLECTION_DESC { get; set; }
        public DateTime? DATA_DTTM { get; set; }
        public string DATA_VALUE { get; set; }
        public bool? VAL_IND { get; set; }
        public string VAL_CD { get; set; }
        public string AQS_NULL_CODE { get; set; }
        public bool? LVL1_VAL_IND { get; set; }
        public string LVL1_VAL_USERIDX { get; set; }
        public string LVL1_VAL_USER { get; set; }
        public DateTime? LVL1_VAL_DT { get; set; }
        public bool? LVL2_VAL_IND { get; set; }
        public string LVL2_VAL_USERIDX { get; set; }
        public string LVL2_VAL_USER { get; set; }
        public DateTime? LVL2_VAL_DT { get; set; }
        public string NOTES { get; set; }

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

    }


    public class ImportResponse
    {
        public bool SuccInd { get; set; }
        public Guid? DATA_IDX { get; set; }
        public DateTime? DATA_DTTM { get; set; }
        public string DATA_VALUE { get; set; }
    }

    public class db_Air
    {
        //*****************SITES**********************************

        /// <summary>
        /// Returns list of sites a user has access to, optionally filtered by OrgID
        /// </summary>
        /// <param name="OrgID"></param>
        /// <param name="UserIDX"></param>
        /// <returns></returns>
        public static List<T_QREST_SITES> GetT_QREST_SITES_ByUser_OrgID(string OrgID, string UserIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               join u in ctx.T_QREST_ORG_USERS.AsNoTracking() on a.ORG_ID equals u.ORG_ID
                               where u.USER_IDX == UserIDX
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
        /// <param name="UserIDX"></param>
        /// <returns></returns>
        public static int? GetT_QREST_SITES_ByUser_OrgID_count(string OrgID, string UserIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITES.AsNoTracking()
                            join u in ctx.T_QREST_ORG_USERS.AsNoTracking() on a.ORG_ID equals u.ORG_ID
                            where u.USER_IDX == UserIDX
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
                               where a.AIRNOW_IND == true
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

        public static List<T_QREST_SITES> GetT_QREST_SITES_ByOrgID(string OrgID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               where (OrgID != null ? a.ORG_ID == OrgID : true)
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

        public static T_QREST_SITES GetT_QREST_SITES_ByID(Guid SiteIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITES.AsNoTracking()
                            where a.SITE_IDX == SiteIDX
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

        public static List<T_QREST_SITES> GetT_QREST_SITES_Sampling()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITES.AsNoTracking()
                            join b in ctx.T_QREST_MONITORS.AsNoTracking() on a.SITE_IDX equals b.SITE_IDX
                            join c in ctx.T_QREST_DATA_HOURLY.AsNoTracking() on b.MONITOR_IDX equals c.MONITOR_IDX
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
            string aIRNOW_USR, string aIRNOW_PWD, string aIRNOW_ORG, string aIRNOW_SITE, string sITE_COMMENTS, string cREATE_USER)
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

        public static List<T_QREST_SITE_POLL_CONFIG> GetT_QREST_SITE_POLL_CONFIG_BySite(Guid SiteIDX, bool OnlyActInd)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking()
                            where a.SITE_IDX == SiteIDX
                            && (OnlyActInd == true ? a.ACT_IND == true : true)
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
                                   LOGGER_PASSWORD = b.LOGGER_PASSWORD,
                                   DELIMITER = b.DELIMITER,
                                   DATE_COL = b.DATE_COL,
                                   DATE_FORMAT = b.DATE_FORMAT,
                                   TIME_COL = b.TIME_COL,
                                   TIME_FORMAT = b.TIME_FORMAT,
                                   LOCAL_TIMEZONE = b.LOCAL_TIMEZONE,
                                   TIME_POLL_TYPE = b.TIME_POLL_TYPE
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
            bool aCT_IND, string cREATE_USER, string sITE_NAME, string tIME_POLL_TYPE)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    bool insInd = false;

                    T_QREST_SITE_POLL_CONFIG e = (from c in ctx.T_QREST_SITE_POLL_CONFIG
                                                  where c.POLL_CONFIG_IDX == pOLL_CONFIG_IDX
                                                  select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_SITE_POLL_CONFIG();
                        e.POLL_CONFIG_IDX = Guid.NewGuid();
                        e.CREATE_DT = System.DateTime.Now;
                        e.CREATE_USER_IDX = cREATE_USER;
                    }
                    else
                    {
                        e.MODIFY_USER_IDX = cREATE_USER;
                        e.MODIFY_DT = System.DateTime.Now;

                        db_Ref.CreateT_QREST_SYS_LOG_ACTIVITY("POLLING CONFIG", cREATE_USER, null, "Changed polling config for " + sITE_NAME, null, e.POLL_CONFIG_IDX.ToString());
                    }

                    if (sITE_IDX != null) e.SITE_IDX = sITE_IDX.ConvertOrDefault<Guid>();
                    if (cONFIG_NAME != null) e.CONFIG_NAME = cONFIG_NAME;
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
                    if (lOCAL_TIMEZONE != null) e.LOCAL_TIMEZONE = lOCAL_TIMEZONE;
                    if (tIME_POLL_TYPE != null) e.TIME_POLL_TYPE = tIME_POLL_TYPE;


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


        public static List<SitePollingConfigTypeExtended> GetT_QREST_SITES_POLLING_CONFIG_List(string USER_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               join b in ctx.T_QREST_SITE_POLL_CONFIG.AsNoTracking() on a.SITE_IDX equals b.SITE_IDX
                               join c in ctx.T_QREST_ORG_USERS.AsNoTracking() on a.ORG_ID equals c.ORG_ID
                               where c.USER_IDX == USER_IDX
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
        public static List<SitePollingConfigDetailType> GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(Guid PollConfigIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_SITE_POLL_CONFIG_DTL.AsNoTracking()
                            join b in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals b.MONITOR_IDX
                            where a.POLL_CONFIG_IDX == PollConfigIDX
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
                                ADJUST_FACTOR = a.ADJUST_FACTOR ?? 1
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

        public static Guid? InsertUpdatetT_QREST_SITE_POLL_CONFIG_DTL(Guid? pOLL_CONFIG_DTL_IDX, Guid? pOLL_CONFIG_IDX, Guid? mONITOR_IDX, int? cOL, string sUM_TYPE, int? rOUNDING, double? aDJUST_FACTOR)
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
                    T_QREST_SITE_POLL_CONFIG_DTL rec = new T_QREST_SITE_POLL_CONFIG_DTL { POLL_CONFIG_DTL_IDX = id };
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

        public static List<SitePollingConfigDetailTypeExtended> GetT_QREST_SITES_POLLING_CONFIG_DetailList(String USER_IDX)
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

        public static List<SiteMonitorDisplayType> GetT_QREST_MONITORS_Display_SampledByUser(string UserIDX)
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
                               join u in ctx.T_QREST_ORG_USERS.AsNoTracking() on s.ORG_ID equals u.ORG_ID
                               where u.USER_IDX == UserIDX
                               && u.STATUS_IND == "A"
                               select new SiteMonitorDisplayType
                               {
                                   T_QREST_MONITORS = a,
                                   METHOD_CODE = r.METHOD_CODE,
                                   COLLECTION_DESC = r.COLLECTION_DESC,
                                   PAR_CODE = p.PAR_CODE,
                                   PAR_NAME = p.PAR_NAME,
                                   SITE_ID = s.SITE_ID,
                                   ORG_ID = u.ORG_ID
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
                                   ORG_ID = a.ORG_ID
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
            string aLERT_MIN_TYPE, string aLERT_MAX_TYPE, string aLERT_AMT_CHANGE_TYPE, string aLERT_STUCK_TYPE, string cREATE_USER)
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
                    //don't delete if there is data for the monitor
                    //List<T_QREST_MONITORS> _mon = GetT_QREST_MONITORS_bySiteIDX(id);
                    //if (_mon == null || _mon.Count == 0)
                    //{
                    T_QREST_MONITORS rec = new T_QREST_MONITORS { MONITOR_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                    //}
                    //else
                    //    return -1;
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
            string uNIT_CODE, int? aSSESSMENT_NUM, string aSSESSED_BY, string cREATE_USER)
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

        public static List<QcAssessmentDisplay> GetT_QREST_QC_ASSESSMENT_Search(string orgIDX, Guid? SiteIDX, Guid? MonitorIDX, int pageSize, int? skip, int orderBy, string orderDir = "asc")
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    string orderCol = (orderBy == 3 ? "ASSESSMENT_DT" : "ASSESSMENT_DT");

                    var xxx = (from a in ctx.T_QREST_QC_ASSESSMENT.AsNoTracking()
                               join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                               join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                               join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                               where (MonitorIDX != null ? a.MONITOR_IDX == MonitorIDX : true)
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
                                   PAR_NAME = "(" + pm.PAR_CODE + ")",
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



        //*****************FIVE MIN**********************************
        public static Guid? InsertT_QREST_DATA_FIVE_MIN(Guid mONITOR_IDX, DateTime dATA_DTTM, string dATA_VALUE, string uNIT_CODE, bool? vAL_IND, string vAL_CD, DateTime? mODIFY_DT, double? aDJUST_FACTOR, Guid? iMPORT_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_DATA_FIVE_MIN e = (from c in ctx.T_QREST_DATA_FIVE_MIN
                                               where c.MONITOR_IDX == mONITOR_IDX
                                               && c.DATA_DTTM == dATA_DTTM
                                               select c).FirstOrDefault();

                    //insert case
                    if (e == null)
                    {
                        e = new T_QREST_DATA_FIVE_MIN();
                        e.DATA_FIVE_IDX = Guid.NewGuid();
                        e.MONITOR_IDX = mONITOR_IDX;
                        e.DATA_DTTM = dATA_DTTM;
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
                    else if (e.DATA_VALUE != dATA_VALUE)
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

        public static ImportResponse ImportT_QREST_DATA_FIVE_MIN(Guid mONITOR_IDX, DateTime dATA_DTTM, string dATA_VALUE, string uNIT_CODE, bool? vAL_IND, string vAL_CD, DateTime? mODIFY_DT, Guid? iMPORT_IDX)
        {
            Guid? SuccID = db_Air.InsertT_QREST_DATA_FIVE_MIN(mONITOR_IDX, dATA_DTTM, dATA_VALUE, uNIT_CODE, vAL_IND, vAL_CD, mODIFY_DT, 1, iMPORT_IDX);

            return new ImportResponse
            {
                SuccInd = SuccID != null,
                DATA_IDX = SuccID,
                DATA_DTTM = dATA_DTTM,
                DATA_VALUE = dATA_VALUE
            };
        }

        public static bool InsertT_QREST_DATA_FIVE_MIN_fromLine(string line, SitePollingConfigType config, List<SitePollingConfigDetailType> config_dtl)
        {
            try
            {
                string delimiter = config.DELIMITER == "C" ? "," : @"\t";
                string[] cols = line.Split(delimiter.ToCharArray());

                if (cols.Length > 1)  //skip blank row
                {
                    //date
                    string sDate = cols[config.DATE_COL.GetValueOrDefault() - 1].ToString();
                    string sTime = cols[config.TIME_COL.GetValueOrDefault() - 1].ToString();


                    //raw date time coming from logger, assumed UTC time by default
                    DateTime dt = DateTime.ParseExact(sDate + " " + sTime, config.DATE_FORMAT + " " + config.TIME_FORMAT, CultureInfo.InvariantCulture);

                    //if logger outputs in local time, need to convert to UTC time
                    if (config.TIME_POLL_TYPE == "L")
                        dt = dt.AddHours(config.LOCAL_TIMEZONE.ConvertOrDefault<int>() * -1);

                    foreach (SitePollingConfigDetailType _map in config_dtl)
                    {
                        if (_map.COL != null)
                        {
                            //apply N-minute alerts if available
                            string valCd = "";
                            Double? val = cols[_map.COL - 1 ?? 0].ToString().ConvertOrDefault<Double?>();
                            if (val != null && _map.ALERT_MAX_TYPE == "N" && val > _map.ALERT_MAX_VALUE)
                                valCd = "MAX";
                            if (val != null && _map.ALERT_MIN_TYPE == "N" && val > _map.ALERT_MIN_VALUE)
                                valCd = "MIN";

                            db_Air.InsertT_QREST_DATA_FIVE_MIN(_map.MONITOR_IDX, dt, cols[_map.COL - 1 ?? 0], _map.COLLECT_UNIT_CODE, false, valCd, null, _map.ADJUST_FACTOR, null);
                        }
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

        public static List<RawDataDisplay> GetT_QREST_DATA_FIVE_MIN(string org, Guid? site, Guid? mon, DateTime? DateFrom, DateTime? DateTo, int pageSize, int? skip, int orderBy, string orderDir = "desc")
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

                    return (from a in ctx.T_QREST_DATA_FIVE_MIN.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                            join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                            join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                            join u3 in ctx.T_QREST_REF_UNITS.AsNoTracking() on a.UNIT_CODE equals u3.UNIT_CODE
                            into lj3
                            from u3 in lj3.DefaultIfEmpty() //left join on minute unit
                            where a.DATA_DTTM >= DateFromDt
                            && a.DATA_DTTM <= DateToDt
                            && (org != null ? s.ORG_ID == org : true)
                            && (site != null ? s.SITE_IDX == site : true)
                            && (mon != null ? a.MONITOR_IDX == mon : true)
                            orderby a.DATA_DTTM descending
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
                            }).OrderBy(orderCol, orderDir).Skip(skip ?? 0).Take(pageSize).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int GetT_QREST_DATA_FIVE_MINcount(string org, Guid? mon, DateTime? DateFrom, DateTime? DateTo)
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
                            where a.DATA_DTTM >= DateFromDt
                            && a.DATA_DTTM <= DateToDt
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



        //*****************HOURLY**********************************
        public static List<RawDataDisplay> GetT_QREST_DATA_HOURLY(string org, Guid? mon, DateTime? DateFrom, DateTime? DateTo, int pageSize, int? skip, int orderBy, string orderDir = "desc")
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
                    if (orderBy == 5) orderCol = "DATA_VALUE";
                    else if (orderBy == 6) orderCol = "VAL_CD";

                    return (from a in ctx.T_QREST_DATA_HOURLY.AsNoTracking()
                            join m in ctx.T_QREST_MONITORS.AsNoTracking() on a.MONITOR_IDX equals m.MONITOR_IDX
                            join s in ctx.T_QREST_SITES.AsNoTracking() on m.SITE_IDX equals s.SITE_IDX
                            join pm in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on m.PAR_METHOD_IDX equals pm.PAR_METHOD_IDX
                            join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on pm.PAR_CODE equals p.PAR_CODE
                            where a.DATA_DTTM_UTC >= DateFromDt
                            && a.DATA_DTTM_UTC <= DateToDt
                            && (org != null ? s.ORG_ID == org : true)
                            && (mon != null ? a.MONITOR_IDX == mon : true)
                            orderby a.DATA_DTTM_UTC descending
                            select new RawDataDisplay
                            {
                                ORG_ID = s.ORG_ID,
                                SITE_ID = s.SITE_ID,
                                MONITOR_IDX = m.MONITOR_IDX,
                                DATA_RAW_IDX = a.DATA_HOURLY_IDX,
                                DATA_DTTM = a.DATA_DTTM_UTC,
                                DATA_VALUE = a.DATA_VALUE,
                                PAR_CODE = p.PAR_CODE,
                                PAR_NAME = p.PAR_NAME,
                                POC = m.POC,
                                VAL_IND = a.VAL_IND,
                                VAL_CD = a.VAL_CD,
                                NOTES = a.NOTES
                            }).OrderBy(orderCol, orderDir).Skip(skip ?? 0).Take(pageSize).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int GetT_QREST_DATA_HOURLYcount(string org, Guid? mon, DateTime? DateFrom, DateTime? DateTo)
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
                            where a.DATA_DTTM_UTC >= DateFromDt
                            && a.DATA_DTTM_UTC <= DateToDt
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

        public static List<RawDataDisplay> GetT_QREST_DATA_HOURLY_ManVal(Guid mon, DateTime DateFrom, DateTime DateTo)
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
                            where a.DATA_DTTM_UTC >= DateFrom
                            && a.DATA_DTTM_UTC <= DateTo
                            && a.MONITOR_IDX == mon
                            orderby a.DATA_DTTM_UTC ascending
                            select new RawDataDisplay
                            {
                                ORG_ID = s.ORG_ID,
                                SITE_ID = s.SITE_ID,
                                MONITOR_IDX = m.MONITOR_IDX,
                                DATA_RAW_IDX = a.DATA_HOURLY_IDX,
                                DATA_DTTM = a.DATA_DTTM_UTC,
                                DATA_VALUE = a.DATA_VALUE,
                                PAR_CODE = p.PAR_CODE,
                                PAR_NAME = p.PAR_NAME,
                                UNIT_CODE = u3.UNIT_CODE,
                                UNIT_DESC = u3.UNIT_DESC,
                                POC = m.POC,
                                VAL_IND = a.VAL_IND,
                                VAL_CD = a.VAL_CD,
                                AQS_NULL_CODE = a.AQS_NULL_CODE,
                                LVL1_VAL_IND = a.LVL1_VAL_IND,
                                LVL1_VAL_USERIDX = a.LVL1_VAL_USERIDX,
                                LVL1_VAL_USER = u1.FNAME + " " + u1.LNAME,
                                LVL1_VAL_DT = a.LVL1_VAL_DT,
                                LVL2_VAL_IND = a.LVL2_VAL_IND,
                                LVL2_VAL_USERIDX = a.LVL2_VAL_USERIDX,
                                LVL2_VAL_USER = u2.FNAME + " " + u2.LNAME,
                                LVL2_VAL_DT = a.LVL2_VAL_DT,
                                NOTES = a.NOTES
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

        public static List<RawDataDisplay> GetT_QREST_DATA_HOURLY_NotificationsListForUser(string UserIDX)
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
                            && sn.NOTIFY_USER_IDX == UserIDX
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

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_DATA_HOURLY();
                        e.DATA_HOURLY_IDX = Guid.NewGuid();
                        e.MONITOR_IDX = mONITOR_IDX;
                        e.DATA_DTTM_LOCAL = dATA_DTTM_LOCAL;
                        e.DATA_DTTM_UTC = dATA_DTTM_UTC;
                    }

                    if (dATA_VALUE != null) e.DATA_VALUE = dATA_VALUE;
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

        public static Guid? UpdateT_QREST_DATA_HOURLY(Guid dATA_HOURLY_IDX, string aQS_NULL_CODE, bool? lVL1_VAL_IND, bool? lVL2_VAL_IND, string lVL_VAL_USERIDX, string uNIT_CODE, string nOTES)
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
                        if (aQS_NULL_CODE == "-1") e.AQS_NULL_CODE = null;

                        if (lVL1_VAL_IND != null) e.LVL1_VAL_IND = lVL1_VAL_IND;
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
                        if (lVL2_VAL_IND != null) e.LVL2_VAL_IND = lVL2_VAL_IND;
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

                        if (uNIT_CODE != null) e.UNIT_CODE = uNIT_CODE;
                        if (nOTES != null) e.NOTES = nOTES;

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

        public static List<AssessDocDisplay> GetT_QREST_ASSESS_DOCS_BySite(Guid sITE_IDX, int year, int month)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_ASSESS_DOCS.AsNoTracking()
                            where a.SITE_IDX == sITE_IDX
                            && a.MONITOR_IDX == null
                            && a.YR == year
                            && a.MON == month
                            select new AssessDocDisplay
                            {
                                ASSESS_DOC_IDX = a.ASSESS_DOC_IDX,
                                DOC_NAME = a.DOC_NAME,
                                DOC_TYPE = a.DOC_TYPE,
                                DOC_FILE_TYPE = a.DOC_FILE_TYPE,
                                DOC_SIZE = a.DOC_SIZE,
                                DOC_COMMENT = a.DOC_COMMENT,
                                DOC_AUTHOR = a.DOC_AUTHOR
                            }).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<AssessDocDisplay> GetT_QREST_ASSESS_DOCS_ByMonitor(Guid mONITOR_IDX, int year, int month)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_ASSESS_DOCS.AsNoTracking()
                            where a.MONITOR_IDX == mONITOR_IDX
                            && a.YR == year
                            && a.MON == month
                            select new AssessDocDisplay
                            {
                                ASSESS_DOC_IDX = a.ASSESS_DOC_IDX,
                                DOC_NAME = a.DOC_NAME,
                                DOC_TYPE = a.DOC_TYPE,
                                DOC_FILE_TYPE = a.DOC_FILE_TYPE,
                                DOC_SIZE = a.DOC_SIZE,
                                DOC_COMMENT = a.DOC_COMMENT,
                                DOC_AUTHOR = a.DOC_AUTHOR
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

        public static Guid? InsertUpdatetT_QREST_ASSESS_DOCS(Guid? aSSESS_DOC_IDX, Guid? sITE_IDX, Guid? mONITOR_IDX, int? yEAR, int? mONTH, byte[] dOC_CONTENT, string dOC_NAME, string dOC_TYPE,
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
                        e.YR = yEAR ?? 0;
                        e.MON = mONTH ?? 1;
                        e.CREATE_DT = System.DateTime.Now;
                        e.CREATE_USERIDX = cREATE_USER;
                    }
                    else
                    {
                        e.MODIFY_DT = System.DateTime.Now;
                        e.MODIFY_USERIDX = cREATE_USER;
                    }

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
        public static List<T_QREST_AQS> GetT_QREST_AQS_byORG_ID(string oRG_ID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_AQS.AsNoTracking()
                            where a.ORG_ID == oRG_ID
                            select a).ToList();
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
                    return ctx.SP_VALIDATE_HOURLY();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static List<SP_RPT_DAILY_Result> SP_RPT_DAILY(Guid siteIDX, int mn, int yr, int dy, string time)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_RPT_DAILY(siteIDX, mn, yr, dy, time).ToList();
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

        public static List<SP_AQS_REVIEW_STATUS_Result> SP_AQS_REVIEW_STATUS(Guid siteIDX, DateTime adate)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_AQS_REVIEW_STATUS(siteIDX, adate).ToList();
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
