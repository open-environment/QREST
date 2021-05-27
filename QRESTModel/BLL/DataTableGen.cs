using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace QRESTModel.DataTableGen
{
    public static class DataTableGen
    {
        //*******************GENERAL SHARED ***********************************
        public static DataSet DataSetFromDataTables(List<DataTable> dts)
        {
            DataSet ds = new DataSet();
            foreach (DataTable dt in dts)
            {
                if (dt != null && dt.Rows.Count > 0)
                    ds.Tables.Add(dt);
            }
            return ds;
        }

        //*******************QUERY SPECIFIC***********************************
        public static DataTable SitesByUser(string UserIDX, string orgid)
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

            List<T_QREST_SITES> _sites = db_Air.GetT_QREST_SITES_ByUser_OrgID(orgid, UserIDX);
            foreach (var _site in _sites)
            {
                dtSites.Rows.Add(_site.ORG_ID, _site.SITE_ID, _site.SITE_NAME, _site.AQS_SITE_ID, _site.STATE_CD, _site.COUNTY_CD, _site.LATITUDE, _site.LONGITUDE,
                    _site.ELEVATION, _site.ADDRESS, _site.CITY, _site.ZIP_CODE, _site.START_DT, _site.END_DT, _site.POLLING_ONLINE_IND, _site.POLLING_FREQ_TYPE,
                    _site.POLLING_FREQ_NUM, _site.AIRNOW_IND, _site.AQS_IND, _site.SITE_COMMENTS);
            }
            return dtSites;
        }

        public static DataTable MonitorsByUser(string UserIDX, string org)
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

            List<SiteMonitorDisplayType> _mons = db_Air.GetT_QREST_MONITORS_ByUser_OrgID(org, UserIDX);
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

        public static DataTable RefPar()
        {
            DataTable dt = new DataTable("Parameters");
            dt.Columns.AddRange(new DataColumn[5] {
                                            new DataColumn("Par Code"),
                                            new DataColumn("Par Name"),
                                            new DataColumn("CAS Num"),
                                            new DataColumn("Std Unit Code"),
                                            new DataColumn("AQS Ind")
                                           });

            List<T_QREST_REF_PARAMETERS> _datas = db_Ref.GetT_QREST_REF_PARAMETERS();
            foreach (var _data in _datas)
            {
                dt.Rows.Add(_data.PAR_CODE, _data.PAR_NAME, _data.CAS_NUM, _data.STD_UNIT_CODE, _data.AQS_IND);

            }
            return dt;
        }


        public static DataTable RawData(string Freq, string orgid, Guid? SiteIDX, Guid? MonIDX, DateTime startDt, DateTime endDt, string tIME_TYPE)
        {
            DataTable dtData = new DataTable("Data");
            dtData.Columns.AddRange(new DataColumn[16] {
                                            new DataColumn("Org ID"),
                                            new DataColumn("Site ID"),
                                            new DataColumn("Par Code"),
                                            new DataColumn("Par Name"),
                                            new DataColumn("Method Code"),
                                            new DataColumn("POC"),
                                            new DataColumn("DateTime" + (tIME_TYPE == "U" ? " (UTC)" : " (local)")),
                                            new DataColumn("Date" + (tIME_TYPE == "U" ? " (UTC)" : " (local)")),
                                            new DataColumn("Time" + (tIME_TYPE == "U" ? " (UTC)" : " (local)")),
                                            new DataColumn("Value"),
                                            new DataColumn("Unit"),
                                            new DataColumn("QREST Flag"),
                                            new DataColumn("AQS Qual Code"),
                                            new DataColumn("AQS Null Code"),
                                            new DataColumn("Lvl1 Review"),
                                            new DataColumn("Lvl2 Review")
                                           });

            if (Freq == "F")
            {
                List<RawDataDisplay> _mons = db_Air.GetT_QREST_DATA_FIVE_MIN(orgid, SiteIDX, MonIDX, startDt, endDt, 50000, null, 3, "asc", tIME_TYPE);
                foreach (var _mon in _mons)
                {
                    dtData.Rows.Add(_mon.ORG_ID, _mon.SITE_ID, _mon.PAR_CODE, _mon.PAR_NAME, _mon.METHOD_CODE, _mon.POC, _mon.DATA_DTTM, _mon.DATA_DTTM.GetValueOrDefault().Date, _mon.DATA_DTTM.GetValueOrDefault().TimeOfDay, _mon.DATA_VALUE, _mon.UNIT_DESC, _mon.VAL_CD, null, null,
                        null, null);
                }
            }
            else if (Freq == "H")
            {
                List<RawDataDisplay> _mons = db_Air.GetT_QREST_DATA_HOURLY(orgid, MonIDX, startDt, endDt, 50000, null, 3, "asc", tIME_TYPE);
                foreach (var _mon in _mons)
                {
                    dtData.Rows.Add(_mon.ORG_ID, _mon.SITE_ID, _mon.PAR_CODE, _mon.PAR_NAME, _mon.METHOD_CODE, _mon.POC, _mon.DATA_DTTM, _mon.DATA_DTTM.GetValueOrDefault().Date, _mon.DATA_DTTM.GetValueOrDefault().TimeOfDay, _mon.DATA_VALUE, _mon.UNIT_DESC, _mon.VAL_CD, _mon.AQS_QUAL_CODES, _mon.AQS_NULL_CODE,
                        _mon.LVL1_VAL_IND, _mon.LVL2_VAL_IND);
                }
            }
            return dtData;
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
                dt.Rows.Add(_data.MonthDisp + " " + _data.SearchDay, _data.C0, _data.C1, _data.C2, _data.C3, _data.C4, _data.C5, _data.C6, _data.C7, _data.C8, _data.C9, _data.C10, _data.C11, _data.C12, _data.C13, _data.C14,
                    _data.C15, _data.C16, _data.C17, _data.C18, _data.C19, _data.C20, _data.C21, _data.C22, _data.C23, _data.SearchDay, _datasums[i].MAX, _datasums[i].MIN, _datasums[i].AVG,
                    _datasums[i].STDEV, _datasums[i].CAP);

                i++;
            }
            return dt;
        }

        public static async Task<DataTable> ReportDaily(Guid monIDX, int mnth, int yr, int day, string time)
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

            List<SP_RPT_DAILY_Result> _datas = await db_Air.SP_RPT_DAILY(monIDX, mnth, yr, day, time);
            foreach (var _data in _datas)
            {
                dt.Rows.Add(_data.PAR_CODE, _data.PAR_NAME, _data.C0, _data.C1, _data.C2, _data.C3, _data.C4, _data.C5, _data.C6, _data.C7, _data.C8, _data.C9, _data.C10, _data.C11, _data.C12, _data.C13, _data.C14,
                    _data.C15, _data.C16, _data.C17, _data.C18, _data.C19, _data.C20, _data.C21, _data.C22, _data.C23);

            }
            return dt;
        }

        public static DataTable GetPollingConfig(string UserIDX, string org)
        {
            DataTable pollingConfig = new DataTable("Polling_Config");
            pollingConfig.Columns.AddRange(new DataColumn[18] {
                new DataColumn("Org ID"),
                new DataColumn("Site ID"),
                new DataColumn("POLL_CONFIG_IDX"),
                new DataColumn("SITE_IDX"),
                new DataColumn("CONFIG_NAME"),
                new DataColumn("RAW_DURATION_CODE"),
                new DataColumn("LOGGER_TYPE"),
                new DataColumn("LOGGER_SOURCE"),
                new DataColumn("LOGGER_PORT"),
                new DataColumn("LOGGER_USERNAME"),
                new DataColumn("DELIMITER"),
                new DataColumn("DATE_COL"),
                new DataColumn("DATE_FORMAT"),
                new DataColumn("TIME_COL"),
                new DataColumn("TIME_FORMAT"),
                new DataColumn("LOCAL_TIMEZONE"),
                new DataColumn("TIME_POLL_TYPE"),
                new DataColumn("ACT_IND")
                });

            List<SitePollingConfigTypeExtended> _sitePollingConfigTypes = db_Air.GetT_QREST_SITES_POLLING_CONFIG_List(UserIDX, org);
            if (_sitePollingConfigTypes != null)
            {
                foreach (var _sitePollingConfigType in _sitePollingConfigTypes)
                {
                    pollingConfig.Rows.Add(
                        _sitePollingConfigType.ORG_ID,
                        _sitePollingConfigType.SITE_ID,
                        _sitePollingConfigType.POLL_CONFIG_IDX,
                        _sitePollingConfigType.SITE_IDX,
                        _sitePollingConfigType.CONFIG_NAME,
                        _sitePollingConfigType.RAW_DURATION_CODE,
                        _sitePollingConfigType.LOGGER_TYPE,
                        _sitePollingConfigType.LOGGER_SOURCE,
                        _sitePollingConfigType.LOGGER_PORT,
                        _sitePollingConfigType.LOGGER_USERNAME,
                        _sitePollingConfigType.DELIMITER,
                        _sitePollingConfigType.DATE_COL,
                        _sitePollingConfigType.DATE_FORMAT,
                        _sitePollingConfigType.TIME_COL,
                        _sitePollingConfigType.TIME_FORMAT,
                        _sitePollingConfigType.LOCAL_TIMEZONE,
                        _sitePollingConfigType.TIME_POLL_TYPE,
                        _sitePollingConfigType.ACT_IND);
                }
            }

            return pollingConfig;
        }

        public static DataTable GetPollingConfigDetail(string UserIDX, string org)
        {
            DataTable pollingConfigDetail = new DataTable("Polling_Config_Detail");
            pollingConfigDetail.Columns.AddRange(new DataColumn[10] {
                new DataColumn("Org ID"),
                new DataColumn("Site ID"),
                new DataColumn("POLL_CONFIG_DTL_IDX"),
                new DataColumn("POLL_CONFIG_IDX"),
                new DataColumn("MONITOR_IDX"),
                new DataColumn("PAR_CODE"),
                new DataColumn("COL"),
                new DataColumn("COLLECT_UNIT_CODE"),
                new DataColumn("SUM_TYPE"),
                new DataColumn("ROUNDING")
                });

            List<SitePollingConfigDetailTypeExtended> _sitePollingConfigDetailTypes = db_Air.GetT_QREST_SITES_POLLING_CONFIG_DetailList(UserIDX, org);
            if (_sitePollingConfigDetailTypes != null)
            {
                foreach (var _sitePollingConfigDetailType in _sitePollingConfigDetailTypes)
                {
                    pollingConfigDetail.Rows.Add(
                        _sitePollingConfigDetailType.ORG_ID,
                        _sitePollingConfigDetailType.SITE_ID,
                        _sitePollingConfigDetailType.POLL_CONFIG_DTL_IDX,
                        _sitePollingConfigDetailType.POLL_CONFIG_IDX,
                        _sitePollingConfigDetailType.MONITOR_IDX,
                        _sitePollingConfigDetailType.PAR_CODE,
                        _sitePollingConfigDetailType.COL,
                        _sitePollingConfigDetailType.COLLECT_UNIT_CODE,
                        _sitePollingConfigDetailType.SUM_TYPE,
                        _sitePollingConfigDetailType.ROUNDING);
                }
            }

            return pollingConfigDetail;
        }

        public static DataTable GetHourlyDataByImportIDX(Guid iMPORT_IDX)
        {
            DataTable _dt = new DataTable("Imported Data");
            _dt.Columns.AddRange(new DataColumn[8] {
                new DataColumn("ORG_ID"),
                new DataColumn("SITE_ID"),
                new DataColumn("POC"),
                new DataColumn("PAR_CODE"),
                new DataColumn("PAR_NAME"),
                new DataColumn("DATA_DTTM"),
                new DataColumn("DATA_VALUE"),
                new DataColumn("VAL_CD")
                });

            List<RawDataDisplay> _datas = db_Air.GetT_QREST_DATA_HOURLY_ByImportIDX(iMPORT_IDX);
            if (_datas != null)
            {
                foreach (var _data in _datas)
                {
                    _dt.Rows.Add(
                        _data.ORG_ID,
                        _data.SITE_ID,
                        _data.POC,
                        _data.PAR_CODE,
                        _data.PAR_NAME,
                        _data.DATA_DTTM,
                        _data.DATA_VALUE,
                        _data.VAL_CD);
                }
            }

            return _dt;
        }

        public static DataTable GetFiveMinDataByImportIDX(Guid iMPORT_IDX)
        {
            DataTable _dt = new DataTable("Imported Data");
            _dt.Columns.AddRange(new DataColumn[8] {
                new DataColumn("ORG_ID"),
                new DataColumn("SITE_ID"),
                new DataColumn("POC"),
                new DataColumn("PAR_CODE"),
                new DataColumn("PAR_NAME"),
                new DataColumn("DATA_DTTM"),
                new DataColumn("DATA_VALUE"),
                new DataColumn("VAL_CD")
                });

            List<RawDataDisplay> _datas = db_Air.GetT_QREST_DATA_FIVE_MIN_ByImportIDX(iMPORT_IDX);
            if (_datas != null)
            {
                foreach (var _data in _datas)
                {
                    _dt.Rows.Add(
                        _data.ORG_ID,
                        _data.SITE_ID,
                        _data.POC,
                        _data.PAR_CODE,
                        _data.PAR_NAME,
                        _data.DATA_DTTM,
                        _data.DATA_VALUE,
                        _data.VAL_CD);
                }
            }

            return _dt;
        }

        public static DataTable GetHourlyLogByHourlyIDX(Guid dATA_HOURLY_IDX)
        {
            DataTable _dt = new DataTable("Hourly Log");
            _dt.Columns.AddRange(new DataColumn[4] {
                //new DataColumn("SITE_ID"),
                //new DataColumn("PAR_CODE"),
                //new DataColumn("PAR_NAME"),
                new DataColumn("DATA_DTTM"),
                new DataColumn("NOTES"),
                new DataColumn("NOTE_DATE"),
                new DataColumn("USER")
                });

            List<HourlyLogDisplay> _datas = db_Air.GetT_QREST_DATA_HOURLY_LOG_ByHour(dATA_HOURLY_IDX);
            if (_datas != null)
            {
                foreach (var _data in _datas)
                {
                    _dt.Rows.Add(
                        _data.DATA_DTTM,
                        _data.NOTES,
                        _data.MODIFY_DT,
                        _data.USER_NAME);
                }
            }

            return _dt;
        }

    }
}
