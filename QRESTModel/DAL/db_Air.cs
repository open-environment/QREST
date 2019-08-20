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
        public string PAR_NAME { get; set; }
        public string METHOD_CODE { get; set; }
        public string ORG_ID { get; set; }
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

        public static int InsertUpdatetT_QREST_SITES(Guid? sITE_IDX, string oRG_ID, string sITE_ID, string sITE_NAME, string aQS_SITE_ID, decimal? lATITUDE, decimal? lONGITUDE,
            string aDDRESS, string cITY, string sTATE, string zIP_CODE, DateTime? sTART_DT, DateTime? eND_DT, bool? tELEMETRY_ONLINE_IND, string tELEMETRY_SOURCE,
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
                    if (lATITUDE != null) e.LATITUDE = lATITUDE;
                    if (lONGITUDE != null) e.LONGITUDE = lONGITUDE;
                    if (aDDRESS != null) e.ADDRESS = aDDRESS;
                    if (cITY != null) e.CITY = cITY;
                    if (sTATE != null) e.STATE = sTATE;
                    if (zIP_CODE != null) e.ZIP_CODE = zIP_CODE;
                    if (sTART_DT != null) e.START_DT = sTART_DT;
                    if (eND_DT != null) e.END_DT = eND_DT;
                    if (tELEMETRY_ONLINE_IND != null) e.TELEMETRY_ONLINE_IND = tELEMETRY_ONLINE_IND;
                    if (tELEMETRY_SOURCE != null) e.TELEMETRY_SOURCE = tELEMETRY_SOURCE;
                    if (sITE_COMMENTS != null) e.SITE_COMMENTS = sITE_COMMENTS;

                    if (insInd)
                        ctx.T_QREST_SITES.Add(e);

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

        public static int InsertUpdatetT_QREST_MONITORS(Guid? mONITOR_IDX, Guid? sITE_IDX, Guid? pAR_METHOD_IDX, int? pOC, string dURATION_CODE, string cOLLECT_FREQ_CODE,
            string cOLLECT_UNIT_CODE, double? aLERT_MIN_VALUE, double? aLERT_MAX_VALUE, int? aLERT_PCT_CHANGE, int? aLERT_STUCK_REC_COUNT, string cREATE_USER)
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
                    if (aLERT_PCT_CHANGE != null) e.ALERT_PCT_CHANGE = aLERT_PCT_CHANGE;
                    if (aLERT_STUCK_REC_COUNT != null) e.ALERT_STUCK_REC_COUNT = aLERT_STUCK_REC_COUNT;

                    if (insInd)
                        ctx.T_QREST_MONITORS.Add(e);

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

    }
}
