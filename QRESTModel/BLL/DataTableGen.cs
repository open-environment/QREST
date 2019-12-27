using QRESTModel.DAL;
using System;
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

            List<RefParMethodDisplay> _datas = db_Ref.GetT_QREST_REF_PAR_METHODS_Search(strPar, strCollMethod, 5000, 0);
            foreach (var _data in _datas)
            {
                dt.Rows.Add(_data.T_QREST_REF_PAR_METHODS.PAR_CODE, _data.PAR_NAME, _data.T_QREST_REF_PAR_METHODS.RECORDING_MODE, _data.T_QREST_REF_PAR_METHODS.METHOD_CODE,
                    _data.T_QREST_REF_PAR_METHODS.ANALYSIS_DESC, _data.T_QREST_REF_PAR_METHODS.REFERENCE_METHOD_ID, _data.T_QREST_REF_PAR_METHODS.EQUIVALENT_METHOD);

            }
            return dt;
        }

        public static DataTable ReportMonthly(Guid monIDX, int mnth, int yr, string time)
        {
            DataTable dt = new DataTable("Monthly Data");
            dt.Columns.AddRange(new DataColumn[31] {
                                            new DataColumn("Day"),
                                            new DataColumn("Mid"),
                                            new DataColumn("1:00"),
                                            new DataColumn("2:00"),
                                            new DataColumn("3:00"),
                                            new DataColumn("4:00"),
                                            new DataColumn("5:00"),
                                            new DataColumn("6:00"),
                                            new DataColumn("7:00"),
                                            new DataColumn("8:00"),
                                            new DataColumn("9:00"),
                                            new DataColumn("10:00"),
                                            new DataColumn("11:00"),
                                            new DataColumn("Noon"),
                                            new DataColumn("13:00"),
                                            new DataColumn("14:00"),
                                            new DataColumn("15:00"),
                                            new DataColumn("16:00"),
                                            new DataColumn("17:00"),
                                            new DataColumn("18:00"),
                                            new DataColumn("19:00"),
                                            new DataColumn("20:00"),
                                            new DataColumn("21:00"),
                                            new DataColumn("22:00"),
                                            new DataColumn("23:00"),
                                            new DataColumn("Day-"),
                                            new DataColumn("Max"),
                                            new DataColumn("Min"),
                                            new DataColumn("Avg"),
                                            new DataColumn("STD"),
                                            new DataColumn("Cap")
                                           });

            List<SP_RPT_MONTHLY_Result> _datas = db_Air.SP_RPT_MONTHLY(monIDX, mnth, yr, time);
            List<SP_RPT_MONTHLY_SUMS_Result> _datasums = db_Air.SP_RPT_MONTHLY_SUMS(monIDX, mnth, yr, time);
            int i = 0;
            foreach (var _data in _datas)
            {
                dt.Rows.Add(_data.SearchDay, _data.C0, _data.C1, _data.C2, _data.C3, _data.C4, _data.C5, _data.C6, _data.C7, _data.C8, _data.C9, _data.C10, _data.C11, _data.C12, _data.C13, _data.C14, 
                    _data.C15, _data.C16, _data.C17, _data.C18, _data.C19, _data.C20, _data.C21, _data.C22, _data.C23, _data.SearchDay, _datasums[i].MAX, _datasums[i].MIN, _datasums[i].AVG, 
                    _datasums[i].STDEV, _datasums[i].CAP);

                i++;
            }
            return dt;
        }

        public static DataTable ReportAnnual(Guid monIDX, int yr, string time)
        {
            DataTable dt = new DataTable("Annual Data");
            dt.Columns.AddRange(new DataColumn[31] {
                                            new DataColumn("Day"),
                                            new DataColumn("Mid"),
                                            new DataColumn("1:00"),
                                            new DataColumn("2:00"),
                                            new DataColumn("3:00"),
                                            new DataColumn("4:00"),
                                            new DataColumn("5:00"),
                                            new DataColumn("6:00"),
                                            new DataColumn("7:00"),
                                            new DataColumn("8:00"),
                                            new DataColumn("9:00"),
                                            new DataColumn("10:00"),
                                            new DataColumn("11:00"),
                                            new DataColumn("Noon"),
                                            new DataColumn("13:00"),
                                            new DataColumn("14:00"),
                                            new DataColumn("15:00"),
                                            new DataColumn("16:00"),
                                            new DataColumn("17:00"),
                                            new DataColumn("18:00"),
                                            new DataColumn("19:00"),
                                            new DataColumn("20:00"),
                                            new DataColumn("21:00"),
                                            new DataColumn("22:00"),
                                            new DataColumn("23:00"),
                                            new DataColumn("Day-"),
                                            new DataColumn("Max"),
                                            new DataColumn("Min"),
                                            new DataColumn("Avg"),
                                            new DataColumn("STD"),
                                            new DataColumn("Cap")
                                           });

            List<SP_RPT_ANNUAL_Result> _datas = db_Air.SP_RPT_ANNUAL(monIDX, yr, time);
            List<SP_RPT_ANNUAL_SUMS_Result> _datasums = db_Air.SP_RPT_ANNUAL_SUMS(monIDX, yr, time);
            int i = 0;
            foreach (var _data in _datas)
            {
                dt.Rows.Add(_data.MonthDisp + " "  + _data.SearchDay, _data.C0, _data.C1, _data.C2, _data.C3, _data.C4, _data.C5, _data.C6, _data.C7, _data.C8, _data.C9, _data.C10, _data.C11, _data.C12, _data.C13, _data.C14,
                    _data.C15, _data.C16, _data.C17, _data.C18, _data.C19, _data.C20, _data.C21, _data.C22, _data.C23, _data.SearchDay, _datasums[i].MAX, _datasums[i].MIN, _datasums[i].AVG,
                    _datasums[i].STDEV, _datasums[i].CAP);

                i++;
            }
            return dt;
        }

        public static DataTable ReportDaily(Guid monIDX, int mnth, int yr, int day, string time)
        {
            DataTable dt = new DataTable("Daily Data");
            dt.Columns.AddRange(new DataColumn[26] {
                                            new DataColumn("Par Code"),
                                            new DataColumn("Par Name"),
                                            new DataColumn("Mid"),
                                            new DataColumn("1:00"),
                                            new DataColumn("2:00"),
                                            new DataColumn("3:00"),
                                            new DataColumn("4:00"),
                                            new DataColumn("5:00"),
                                            new DataColumn("6:00"),
                                            new DataColumn("7:00"),
                                            new DataColumn("8:00"),
                                            new DataColumn("9:00"),
                                            new DataColumn("10:00"),
                                            new DataColumn("11:00"),
                                            new DataColumn("Noon"),
                                            new DataColumn("13:00"),
                                            new DataColumn("14:00"),
                                            new DataColumn("15:00"),
                                            new DataColumn("16:00"),
                                            new DataColumn("17:00"),
                                            new DataColumn("18:00"),
                                            new DataColumn("19:00"),
                                            new DataColumn("20:00"),
                                            new DataColumn("21:00"),
                                            new DataColumn("22:00"),
                                            new DataColumn("23:00")
                                           });

            List<SP_RPT_DAILY_Result> _datas = db_Air.SP_RPT_DAILY(monIDX, mnth, yr, day, time);
            foreach (var _data in _datas)
            {
                dt.Rows.Add(_data.PAR_CODE, _data.PAR_NAME, _data.C0, _data.C1, _data.C2, _data.C3, _data.C4, _data.C5, _data.C6, _data.C7, _data.C8, _data.C9, _data.C10, _data.C11, _data.C12, _data.C13, _data.C14,
                    _data.C15, _data.C16, _data.C17, _data.C18, _data.C19, _data.C20, _data.C21, _data.C22, _data.C23);

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
