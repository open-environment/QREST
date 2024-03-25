using QREST_Service;
using QRESTModel.COMM;
using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Mapping;

namespace QRESTServiceCatalog
{
    class LoggerPolling
    {
        private static LoggerPolling objInstance = null;
        private static bool bExecutingGenLedSvcStatus = false;

        public static LoggerPolling Instance
        {
            get
            {
                if (objInstance == null)
                    objInstance = new LoggerPolling();
                return objInstance;
            }
        }


        public void RunService()
        {
            try
            {
                if (!bExecutingGenLedSvcStatus)
                    ExecuteLoggerPolling();
            }
            catch (Exception ex)
            {
                General.WriteToFile("Error in ExecuteLoggerPolling: Error Message : " + ex.ToString());
                bExecutingGenLedSvcStatus = false;
            }
        }


        private void ExecuteLoggerPolling()
        {
            bExecutingGenLedSvcStatus = true;

            string recCount = db_Ref.GetT_QREST_APP_SETTING("NUM_POLL_RECS") ?? "12";

            //this is where logic for the task goes
            List<SitePollingConfigType> _configs = db_Air.GetT_QREST_SITES_POLLING_CONFIG_ReadyToPoll();
            if (_configs != null && _configs.Count > 0)
            {
                foreach (SitePollingConfigType _config in _configs)
                {
                    List<SitePollingConfigDetailType> _config_dtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_config.POLL_CONFIG_IDX, true);
                    if (_config_dtl != null && _config_dtl.Count > 0)
                    {
                        General.WriteToFile("Start poll for org:" + _config.ORG_ID + " site: " + _config.SITE_ID);

                        //****************** ZENO OR SUTRON CCSAIL DATA LOGGER *********************************************************
                        //****************** ZENO OR SUTRON CCSAIL DATA LOGGER *********************************************************
                        //****************** ZENO OR SUTRON CCSAIL DATA LOGGER *********************************************************
                        if (_config.LOGGER_TYPE == "ZENO" || _config.LOGGER_TYPE == "SUTRON")
                        {
                            CommMessageLog _log = LoggerComm.ConnectTcpClientSailer(_config.LOGGER_SOURCE, (ushort)_config.LOGGER_PORT, "DL" + recCount + ",", _config.SITE_ID);
                            if (_log.CommMessageStatus && _log.CommResponse != null && _log.CommResponse.Length > 20)
                            {
                                //send the entire text response to the file parser routine
                                LoggerComm.ParseFlatFile(_log.CommResponse, _config, _config_dtl, true);

                                //log the text to file (for future auditing of parse accuracy)
                                General.WriteToPollingFile(_log.CommResponse, _config.SITE_ID);
                            }
                            else
                                General.WriteToFile("Error in polling:" + _config.ORG_ID + " site: " + _config.SITE_ID + " ###" + _log.CommMessageType + ":" + _log.CommResponse);

                        }
                        //****************** SUTRON WITH LEADS DATA LOGGER *********************************************************
                        //****************** SUTRON WITH LEADS DATA LOGGER *********************************************************
                        //****************** SUTRON WITH LEADS DATA LOGGER *********************************************************
                        else if (_config.LOGGER_TYPE == "SUTRON_LEADS")
                        {
                            //get latest date value that was polled. If in last 10 days then send date range, otherwide query for today
                            string msg = "GET /TODAY";
                            DateTime? latestValue = db_Air.SP_LATEST_POLLED_DATE(_config.SITE_IDX, _config.RAW_DURATION_CODE, _config.TIME_POLL_TYPE);
                            if (latestValue != null && latestValue > System.DateTime.Today.AddDays(-10))    
                                msg = "GET /S " + latestValue.GetValueOrDefault().ToString("MM-dd-yyyy HH:mm:ss");

                            //if log file is explicitly specified, then specify it
                            if (string.IsNullOrEmpty(_config.LOGGER_FILE_NAME) == false)
                                msg = msg + " /F " + _config.LOGGER_FILE_NAME;

                            CommMessageLog _log = LoggerComm.ConnectTcpSutron(_config.LOGGER_SOURCE, (ushort)_config.LOGGER_PORT, _config.LOGGER_USERNAME, _config.LOGGER_PASSWORD, msg + " /C");
                            if (_log.CommMessageStatus && _log.CommResponse != null && _log.CommResponse.Length > 20)
                            {
                                //send the entire text response to the file parser routine
                                LoggerComm.ParseFlatFile(_log.CommResponse, _config, _config_dtl, true);

                                //log the text to file (for future auditing of parse accuracy)
                                General.WriteToPollingFile(_log.CommResponse, _config.SITE_ID);
                            }
                            else
                                General.WriteToFile("Error in polling:" + _config.ORG_ID + " site: " + _config.SITE_ID + " ###" + _log.CommMessageType + ":" + _log.CommResponse);

                        }
                        //****************** ESC/AGILAIRE LOGGER *********************************************************
                        //****************** ESC/AGILAIRE LOGGER  *********************************************************
                        //****************** ESC/AGILAIRE LOGGER  *********************************************************
                        else if (_config.LOGGER_TYPE == "ESC")
                        {
                            DateTime stDate = System.DateTime.Now.AddHours(-5);
                            DateTime endDate = System.DateTime.Now.AddHours(5);
                            string startJulian = stDate.DayOfYear.ToString().PadLeft(3, '0');
                            string endJulian = endDate.DayOfYear.ToString().PadLeft(3, '0');
                            string startHour = stDate.Hour.ToString().PadLeft(2, '0');
                            string endtHour = endDate.Hour.ToString().PadLeft(2, '0');

                            CommMessageLog _log = LoggerComm.ConnectTcpESC(_config.LOGGER_SOURCE, (ushort)_config.LOGGER_PORT, startJulian + startHour + "0000" + "|Y|" + endJulian + endtHour + "0000", _config.SITE_ID);
                            if (_log.CommMessageStatus && _log.CommResponse != null && _log.CommResponse.Length > 20)
                            {
                                //send the entire text response to the file parser routine
                                LoggerComm.ParseFlatFileESC(_log.CommResponse, _config, _config_dtl, true);

                                //log the text to file (for future auditing of parse accuracy)
                                General.WriteToPollingFile(_log.CommResponse, _config.SITE_ID);
                            }
                            else
                                General.WriteToFile("Error in polling:" + _config.ORG_ID + " site: " + _config.SITE_ID + " ###" + _log.CommMessageType + ":" + _log.CommResponse);

                        }
                        //****************** MET ONE BAM*********************************************************
                        //****************** MET ONE BAM*********************************************************
                        //****************** MET ONE BAM*********************************************************
                        else if (_config.LOGGER_TYPE == "MET_ONE_BAM")
                        {
                            CommMessageLog _log = LoggerComm.ConnectTcpBAM(_config.LOGGER_SOURCE, (ushort)_config.LOGGER_PORT);
                            if (_log.CommMessageStatus && _log.CommResponse != null && _log.CommResponse.Length > 20)
                            {
                                //send the entire text response to the file parser routine
                                LoggerComm.ParseFlatFileMetOneBAM(_log.CommResponse, _config, _config_dtl, true);

                                //log the text to file (for future auditing of parse accuracy)
                                General.WriteToPollingFile(_log.CommResponse, _config.SITE_ID);
                            }
                            else
                                General.WriteToFile("Error in polling:" + _config.ORG_ID + " site: " + _config.SITE_ID + " ###" + _log.CommMessageType + ":" + _log.CommResponse);
                        }
                        //****************** MET ONE BAM 1022*********************************************************
                        //****************** MET ONE BAM 1022*********************************************************
                        //****************** MET ONE BAM 1022*********************************************************
                        else if (_config.LOGGER_TYPE == "MET_BAM_1022")
                        {
                            General.WriteToFile("MET_BAM_1022 logging happening for:" + _config.ORG_ID + " site: " + _config.SITE_ID + "");

                            //find how many records to retrieve
                            int hrs = -1;
                            DateTime? latestValue = db_Air.SP_LATEST_POLLED_DATE(_config.SITE_IDX, _config.RAW_DURATION_CODE, _config.TIME_POLL_TYPE);
                            if (latestValue != null)
                            {
                                TimeSpan difference = DateTime.Now - latestValue.Value;
                                hrs = ((int)difference.TotalHours);
                                if (hrs > 2000) hrs = -1;
                            }

                            General.WriteToFile("MET_BAM_1022 will retrieve for hours count (-1 means all):" + hrs);

                            CommMessageLog _log = LoggerComm.ConnectTcpBAM1022(_config.LOGGER_SOURCE, (ushort)_config.LOGGER_PORT, hrs);
                            if (_log.CommMessageStatus && _log.CommResponse != null && _log.CommResponse.Length > 20)
                            {
                                //send the entire text response to the file parser routine
                                LoggerComm.ParseFlatFile(_log.CommResponse, _config, _config_dtl, true);

                                //log the text to file (for future auditing of parse accuracy)
                                General.WriteToPollingFile(_log.CommResponse, _config.SITE_ID);
                            }
                            else
                                General.WriteToFile("Error in polling:" + _config.ORG_ID + " site: " + _config.SITE_ID + " ###" + _log.CommMessageType + ":" + _log.CommResponse);
                        }
                        //****************** WEATHER.COM WEATHER STATION *********************************************************
                        //****************** WEATHER.COM WEATHER STATION *********************************************************
                        //****************** WEATHER.COM WEATHER STATION *********************************************************
                        else if (_config.LOGGER_TYPE == "WEATHER_PWS")
                        {
                            T_QREST_SITE_POLL_CONFIG _c = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(_config.POLL_CONFIG_IDX);
                            bool xxx = LoggerComm.RetrieveWeatherCompanyPWS(_c).Result;
                            if (xxx == false)
                                General.WriteToFile("Error in polling:" + _config.ORG_ID + " site: " + _config.SITE_ID + " ###Unreadable weather station format");

                        }
                        //****************** CAMPBELL SCIENTIFIC DATA LOGGER *********************************************************
                        //****************** CAMPBELL SCIENTIFIC DATA LOGGER *********************************************************
                        //****************** CAMPBELL SCIENTIFIC DATA LOGGER *********************************************************
                        else if (_config.LOGGER_TYPE == "CAMPBELL")
                        {
                            General.WriteToFile("Campbell logging happening for:" + _config.ORG_ID + " site: " + _config.SITE_ID + "");

                            //retrieve the file from the poll directory
                            string _file = LoggerComm.RetrieveCampbell(_config);

                            if (_file == null)
                                General.WriteToFile("Error in polling:" + _config.ORG_ID + " site: " + _config.SITE_ID + " ###NO FILE");
                            else if (_file.Length > 10)
                            {
                                //send the entire text response to the file parser routine
                                General.WriteToFile("Campbell data found for:" + _config.ORG_ID + " site: " + _config.SITE_ID + "###" + _file.Substring(1, 10));

                                bool ParseSuccessInd = LoggerComm.ParseFlatFile(_file, _config, _config_dtl, true); 
                                if (ParseSuccessInd)
                                {
                                    //move file to archive folder
                                    General.ArchiveCampbellPollingFile(_config.SITE_ID);
                                }
                                else
                                    General.WriteToFile("Campbell data parse failed:" + _config.ORG_ID + " site: " + _config.SITE_ID + "###" + _file.Substring(1, 10));

                            }
                            else
                                General.WriteToFile("Error in polling:" + _config.ORG_ID + " site: " + _config.SITE_ID + " ###NO DATA");
                        }

                        General.WriteToFile("End poll for org:" + _config.ORG_ID + " site: " + _config.SITE_ID);
                    }
                    else
                    {
                        db_Ref.CreateT_QREST_SYS_LOG(_config.SITE_IDX.ToString(), "POLLING",
                            "No column mappings found for polling configuration [" + _config.ORG_ID + "]. Site polling taken offline");

                        db_Air.InsertUpdatetT_QREST_SITES(_config.SITE_IDX, null, null, null, null, null, null, null,
                            null, null, null, null, null, null, null, false, null, null, 
                            null, null, null,
                            null, null, null, null, null, null, null, null, null);
                    }
                }
            }
            else
                General.WriteToFile("No sites to poll.");


            bExecutingGenLedSvcStatus = false;
        }


    }
}
