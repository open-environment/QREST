using QRESTModel.DAL;
using System.Collections.Generic;
using System.Data;

namespace QRESTModel.DataTableGen
{
    public static class DataTableGen
    {

        public static DataTable SitesByUser(string UserIDX)
        {
            DataTable dtSites = new DataTable("Sites");
            dtSites.Columns.AddRange(new DataColumn[20] {
                                            new DataColumn("Org ID"),
                                            new DataColumn("Site ID"),
                                            new DataColumn("Site Name"),
                                            new DataColumn("AQS Site ID"),
                                            new DataColumn("State CD"),
                                            new DataColumn("County CD"),
                                            new DataColumn("Latitude"),
                                            new DataColumn("Longitude"),
                                            new DataColumn("Elevation (m)"),
                                            new DataColumn("Address"),
                                            new DataColumn("City"),
                                            new DataColumn("ZIP Code"),
                                            new DataColumn("Telemetry Start Date"),
                                            new DataColumn("Telemetry End Date"),
                                            new DataColumn("Polling Online"),
                                            new DataColumn("Polling Frequency Type"),
                                            new DataColumn("Polling Frequency Num"),
                                            new DataColumn("AirNow Ind"),
                                            new DataColumn("AQS Ind"),
                                            new DataColumn("Site Comments")
                });

            List<T_QREST_SITES> _sites = db_Air.GetT_QREST_SITES_ByUser_OrgID(null, UserIDX);
            foreach (var _site in _sites)
            {
                dtSites.Rows.Add(_site.ORG_ID, _site.SITE_ID, _site.SITE_NAME, _site.AQS_SITE_ID, _site.STATE_CD, _site.COUNTY_CD, _site.LATITUDE, _site.LONGITUDE,
                    _site.ELEVATION, _site.ADDRESS, _site.CITY, _site.ZIP_CODE, _site.START_DT, _site.END_DT, _site.POLLING_ONLINE_IND, _site.POLLING_FREQ_TYPE,
                    _site.POLLING_FREQ_NUM, _site.AIRNOW_IND, _site.AQS_IND, _site.SITE_COMMENTS);
            }
            return dtSites;
        }

        public static DataTable MonitorsByUser(string UserIDX)
        {
            DataTable dtMonitors = new DataTable("Monitors");
            dtMonitors.Columns.AddRange(new DataColumn[13] {
                                            new DataColumn("Org ID"),
                                            new DataColumn("Site ID"),
                                            new DataColumn("Par Code"),
                                            new DataColumn("Par Name"),
                                            new DataColumn("Method Code"),
                                            new DataColumn("POC"),
                                            new DataColumn("Duration Code"),
                                            new DataColumn("Collect Freq Code"),
                                            new DataColumn("Collection Unit Code"),
                                            new DataColumn("Alert Min"),
                                            new DataColumn("Alert Max"),
                                            new DataColumn("Alert Amt Change"),
                                            new DataColumn("Alert Stuck Count")
                                           });

            List<SiteMonitorDisplayType> _mons = db_Air.GetT_QREST_MONITORS_ByUser_OrgID(null, UserIDX);
            foreach (var _mon in _mons)
            {
                dtMonitors.Rows.Add(_mon.ORG_ID, _mon.SITE_ID, _mon.PAR_CODE, _mon.PAR_NAME, _mon.METHOD_CODE, _mon.T_QREST_MONITORS.POC, _mon.T_QREST_MONITORS.DURATION_CODE,
                    _mon.T_QREST_MONITORS.COLLECT_FREQ_CODE, _mon.T_QREST_MONITORS.COLLECT_UNIT_CODE, _mon.T_QREST_MONITORS.ALERT_MIN_VALUE, _mon.T_QREST_MONITORS.ALERT_MAX_VALUE,
                    _mon.T_QREST_MONITORS.ALERT_AMT_CHANGE, _mon.T_QREST_MONITORS.ALERT_STUCK_REC_COUNT);

            }
            return dtMonitors;
        }

        public static DataTable RefParMethod(string strPar, string strCollMethod)
        {
            DataTable dt = new DataTable("Ref Par Method");
            dt.Columns.AddRange(new DataColumn[7] {
                                            new DataColumn("Par Code"),
                                            new DataColumn("Par Name"),
                                            new DataColumn("Recording Mode"),
                                            new DataColumn("Method Code"),
                                            new DataColumn("Analysis Desc"),
                                            new DataColumn("Ref Method ID"),
                                            new DataColumn("Equivalent Method")
                                           });

            List<RefParMethodDisplay> _datas = db_Ref.GetT_QREST_REF_PAR_METHODS_Search(strPar, strCollMethod, 5000, 0, 1);
            foreach (var _data in _datas)
            {
                dt.Rows.Add(_data.T_QREST_REF_PAR_METHODS.PAR_CODE, _data.PAR_NAME, _data.T_QREST_REF_PAR_METHODS.RECORDING_MODE, _data.T_QREST_REF_PAR_METHODS.METHOD_CODE,
                    _data.T_QREST_REF_PAR_METHODS.ANALYSIS_DESC, _data.T_QREST_REF_PAR_METHODS.REFERENCE_METHOD_ID, _data.T_QREST_REF_PAR_METHODS.EQUIVALENT_METHOD);

            }
            return dt;
        }



        public static DataSet DataSetFromDataTables(DataTable dt1, DataTable dt2, DataTable dt3, DataTable dt4)
        {
            DataSet ds = new DataSet();

            if (dt1 != null && dt1.Rows.Count > 0)
                ds.Tables.Add(dt1);
            if (dt2 != null && dt2.Rows.Count > 0)
                ds.Tables.Add(dt2);
            if (dt3 != null && dt3.Rows.Count > 0)
                ds.Tables.Add(dt3);
            if (dt4 != null && dt4.Rows.Count > 0)
                ds.Tables.Add(dt4);

            return ds;
        }
    }
}
