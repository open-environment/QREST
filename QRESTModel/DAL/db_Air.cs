using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRESTModel.BLL;

namespace QRESTModel.DAL
{
    public class SiteMonitorDisplayType
    {
        public T_QREST_MONITORS T_QREST_MONITORS { get; set; }
        public string SITE_ID { get; set; }
        public string PAR_CODE { get; set; }
        public string PAR_NAME { get; set; }
        public string METHOD_CODE { get; set; }
        public string COLLECTION_DESC { get; set; }
        public string ORG_ID { get; set; }
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
        /// Returns all sites belonging to an organization
        /// </summary>
        /// <param name="OrgID"></param>
        /// <returns></returns>
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
                    var xxx = (from a in ctx.T_QREST_SITES.AsNoTracking()
                               where a.POLLING_ONLINE_IND == true 
                               && a.POLLING_NEXT_RUN_DT < System.DateTime.Now
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

        public static Guid? InsertUpdatetT_QREST_SITES(Guid? sITE_IDX, string oRG_ID, string sITE_ID, string sITE_NAME, string aQS_SITE_ID, string sTATE, string cOUNTY, 
            decimal? lATITUDE, decimal? lONGITUDE, string eLEVATION, string aDDRESS, string cITY, string zIP_CODE, DateTime? sTART_DT, DateTime? eND_DT, 
            bool? pOLLING_ONLINE_IND, string pOLLING_FREQ_TYPE, int? pOLLING_FREQ_NUM, DateTime? pOLLING_LAST_RUN_DT, DateTime? pOLLING_NEXT_RUN_DT, bool? aIRNOW_IND, bool? aQS_IND,
            string sITE_COMMENTS, string cREATE_USER)
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
                            select new SiteNotifyDisplay {
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



        //*****************MONITORS**********************************
        public static SiteMonitorDisplayType GetT_QREST_MONITORS_ByID(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_MONITORS.AsNoTracking()
                            join r in ctx.T_QREST_REF_PAR_METHODS.AsNoTracking() on a.PAR_METHOD_IDX equals r.PAR_METHOD_IDX
                            join p in ctx.T_QREST_REF_PARAMETERS.AsNoTracking() on r.PAR_CODE equals p.PAR_CODE
                            where a.MONITOR_IDX == id
                            select new SiteMonitorDisplayType
                            {
                                T_QREST_MONITORS = a,
                                METHOD_CODE = r.METHOD_CODE,
                                PAR_NAME = p.PAR_NAME
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
                               join d in ctx.T_QREST_REF_DURATION.AsNoTracking() on a.DURATION_CODE equals d.DURATION_CODE
                                   into lj
                               from d in lj.DefaultIfEmpty() //left join on duration
                               join f in ctx.T_QREST_REF_COLLECT_FREQ.AsNoTracking() on a.COLLECT_FREQ_CODE equals f.COLLECT_FREQ_CODE
                                   into lj2
                               from f in lj.DefaultIfEmpty() //left join on coll freq
                               where a.SITE_IDX == SiteIDX
                               orderby a.CREATE_DT
                               select new SiteMonitorDisplayType
                               {
                                   T_QREST_MONITORS = a,
                                   METHOD_CODE = r.METHOD_CODE,
                                   COLLECTION_DESC = r.COLLECTION_DESC,
                                   PAR_CODE = p.PAR_CODE,
                                   PAR_NAME = p.PAR_NAME
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
                               join d in ctx.T_QREST_REF_DURATION.AsNoTracking() on m.DURATION_CODE equals d.DURATION_CODE
                                   into lj
                               from d in lj.DefaultIfEmpty() //left join on duration
                               join f in ctx.T_QREST_REF_COLLECT_FREQ.AsNoTracking() on m.COLLECT_FREQ_CODE equals f.COLLECT_FREQ_CODE
                                   into lj2
                               from f in lj.DefaultIfEmpty() //left join on coll freq
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
            string cOLLECT_UNIT_CODE, double? aLERT_MIN_VALUE, double? aLERT_MAX_VALUE, double? aLERT_AMT_CHANGE, int? aLERT_STUCK_REC_COUNT, string cREATE_USER)
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
                    if (aLERT_MAX_VALUE != null) e.ALERT_MAX_VALUE = aLERT_MAX_VALUE;
                    if (aLERT_AMT_CHANGE != null) e.ALERT_AMT_CHANGE = aLERT_AMT_CHANGE;
                    if (aLERT_STUCK_REC_COUNT != null) e.ALERT_STUCK_REC_COUNT = aLERT_STUCK_REC_COUNT;

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
                               select new QcAssessmentDisplay {
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



    }
}
