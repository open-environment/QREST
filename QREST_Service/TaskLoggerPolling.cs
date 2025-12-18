using QREST_Service;
using QRESTModel.BLL;
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
                General.WriteToFile("About to poll. " + _configs.Count + " sites at " + System.DateTime.UtcNow + " as listed here");
                foreach (SitePollingConfigType _config in _configs) {
                    General.WriteToFile("  -  [" + _config.ORG_ID + "][" + _config.SITE_ID + "] : " + _config.LOGGER_TYPE + " next poll:" + _config.POLLING_NEXT_RUN_DT);
                }

                foreach (SitePollingConfigType _config in _configs)
                {
                    List<SitePollingConfigDetailType> _config_dtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_config.POLL_CONFIG_IDX, true);
                    if (_config_dtl != null && _config_dtl.Count > 0)
                    {
                        General.WriteToFile("[" + _config.ORG_ID + "][" + _config.SITE_ID + "] Start logging: " + _config.LOGGER_TYPE);

                        //****************** ZENO OR SUTRON CCSAIL DATA LOGGER *********************************************************
                        //****************** ZENO OR SUTRON CCSAIL DATA LOGGER *********************************************************
                        //****************** ZENO OR SUTRON CCSAIL DATA LOGGER *********************************************************
                        if (_config.LOGGER_TYPE == "ZENO" || _config.LOGGER_TYPE == "SUTRON")
                        {
                            //get latest date value that was polled. If exists then poll in last 10 days then send date range, otherwide query for today
                            string msg = "DL" + recCount;
                            DateTime? latestValue = db_Air.SP_LATEST_POLLED_DATE(_config.SITE_IDX, _config.RAW_DURATION_CODE, _config.TIME_POLL_TYPE);
                            if (latestValue!= null)
                                msg = "DA" + latestValue.Value.ToString("yyMMddHHmm") + "0060";

                            //fix weird bug in zeno logger where if polling on the 55th minute? 
                            try
                            {
                                if (msg.SubStringPlus(10, 2) == "55")
                                    msg = msg.SubStringPlus(0, 11) + '0' + msg.Substring(12);
                            } catch { }

                            General.WriteToFile("[" + _config.ORG_ID + "][" + _config.SITE_ID + "] ZENO/SUTRON command: " + msg);

                            CommMessageLog _log = LoggerComm.ConnectTcpClientSailer(_config.LOGGER_SOURCE, (ushort)_config.LOGGER_PORT, msg + ",", _config.SITE_ID, _config.LOGGER_RESP_DELAY_MS);
                            if (_log.CommMessageStatus && _log.CommResponse != null && _log.CommResponse.Length > 20)
                            {
                                //send the entire text response to the file parser routine
                                LoggerComm.ParseFlatFile(_log.CommResponse, _config, _config_dtl, true);

                                //log the text to file (for future auditing of parse accuracy)
                                General.WriteToPollingFile(_log.CommResponse, _config.SITE_ID);
                            }
                            else
                                General.WriteToFile("[" + _config.ORG_ID + "][" + _config.SITE_ID + "] Error in polling ###" + _log.CommMessageType + ":" + _log.CommResponse);

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
                                General.WriteToFile("[" + _config.ORG_ID + "][" + _config.SITE_ID + "] Error in polling ###" + _log.CommMessageType + ":" + _log.CommResponse);

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
                                General.WriteToFile("[" + _config.ORG_ID + "][" + _config.SITE_ID + "] Error in polling ###" + _log.CommMessageType + ":" + _log.CommResponse);

                        }
                        //****************** MET ONE BAM (1020 older models)*********************************************************
                        //****************** MET ONE BAM (1020 older models)*********************************************************
                        //****************** MET ONE BAM (1020 older models)*********************************************************
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
                                General.WriteToFile("[" + _config.ORG_ID + "][" + _config.SITE_ID + "] Error in polling ###" + _log.CommMessageType + ":" + _log.CommResponse);
                        }
                        //****************** MET ONE BAM 1022 (and newer 1020 models)*********************************************************
                        //****************** MET ONE BAM 1022 (and newer 1020 models)*********************************************************
                        //****************** MET ONE BAM 1022 (and newer 1020 models)*********************************************************
                        else if (_config.LOGGER_TYPE == "MET_BAM_1022")
                        {
                            //find out last hour of data to determine the command to send
                            string command = "4 720"; //no polling at this site yet, so retrieve 720 records (24 * 30 days)
                            DateTime? latestValue = db_Air.SP_LATEST_POLLED_DATE(_config.SITE_IDX, _config.RAW_DURATION_CODE, _config.TIME_POLL_TYPE);
                            if (latestValue != null)
                            {
                                TimeSpan difference = DateTime.Now - latestValue.Value;
                                //if latest date is in the past 
                                if (difference.TotalHours < 720)
                                    command = "4 " + latestValue.Value.AddHours(1).ToString("yyyy-MM-dd HH");
                            }
                            General.WriteToFile("[" + _config.ORG_ID + "][" + _config.SITE_ID + "] MET_BAM will send command: " + command);

                            CommMessageLog _log = LoggerComm.ConnectTcpBAM1022(_config.LOGGER_SOURCE, (ushort)_config.LOGGER_PORT, command);
                            if (_log.CommMessageStatus && _log.CommResponse != null && _log.CommResponse.Length > 20)
                            {
                                //send the entire text response to the file parser routine
                                LoggerComm.ParseFlatFile(_log.CommResponse, _config, _config_dtl, true);

                                //log the text to file (for future auditing of parse accuracy)
                                General.WriteToPollingFile(_log.CommResponse, _config.SITE_ID);
                            }
                            else
                                General.WriteToFile("[" + _config.ORG_ID + "][" + _config.SITE_ID + "] Error in polling ###" + _log.CommMessageType + ":" + _log.CommResponse);
                        }
                        //****************** WEATHER.COM WEATHER STATION *********************************************************
                        //****************** WEATHER.COM WEATHER STATION *********************************************************
                        //****************** WEATHER.COM WEATHER STATION *********************************************************
                        else if (_config.LOGGER_TYPE == "WEATHER_PWS")
                        {
                            T_QREST_SITE_POLL_CONFIG _c = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(_config.POLL_CONFIG_IDX);
                            bool xxx = LoggerComm.RetrieveWeatherCompanyPWS(_c).Result;
                            if (xxx == false)
                                General.WriteToFile("[" + _config.ORG_ID + "][" + _config.SITE_ID + "] Error in polling ###Unreadable weather station format");

                        }
                        //****************** CAMPBELL SCIENTIFIC DATA LOGGER *********************************************************
                        //****************** CAMPBELL SCIENTIFIC DATA LOGGER *********************************************************
                        //****************** CAMPBELL SCIENTIFIC DATA LOGGER *********************************************************
                        else if (_config.LOGGER_TYPE == "CAMPBELL")
                        {
                            //retrieve the file from the poll directory
                            string _file = LoggerComm.RetrieveCampbell(_config);

                            if (_file == null)
                                General.WriteToFile("[" + _config.ORG_ID + "][" + _config.SITE_ID + "] ###NO FILE");
                            else if (_file.Length > 10)
                            {
                                //send the entire text response to the file parser routine
                                General.WriteToFile("Campbell data found for:" + _config.ORG_ID + " site: " + _config.SITE_ID + "###" + _file.SubStringPlus(1, 10));

                                bool ParseSuccessInd = LoggerComm.ParseFlatFile(_file, _config, _config_dtl, true); 
                                if (ParseSuccessInd)
                                {
                                    //move file to archive folder
                                    General.ArchiveCampbellPollingFile(_config.SITE_ID);
                                }
                                else
                                    General.WriteToFile("[" + _config.ORG_ID + "][" + _config.SITE_ID + "] Campbell data parse failed: " + _file.SubStringPlus(1, 10));

                            }
                            else
                                General.WriteToFile("[" + _config.ORG_ID + "][" + _config.SITE_ID + "] Error in polling: ###NO DATA");
                        }
                        else
                            General.WriteToFile("[" + _config.ORG_ID + "][" + _config.SITE_ID + "] Logger type not found");


                        //update next run for the site
                        DateTime nextrun = System.DateTime.UtcNow.AddMinutes(15);  //default to 15 minutes next run
                        if (_config.POLLING_FREQ_TYPE == "M")
                            nextrun = System.DateTime.Now.AddMinutes(_config.POLLING_FREQ_NUM ?? 15);
                        db_Air.InsertUpdatetT_QREST_SITES(_config.SITE_IDX, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 
                            System.DateTime.UtcNow, nextrun, null, null, null, null, null, null, null, null, null, null);


                        General.WriteToFile("[" + _config.ORG_ID + "][" + _config.SITE_ID + "] End logging");
                    }
                    else
                    {
                        db_Ref.CreateT_QREST_SYS_LOG(_config.SITE_IDX.ToString(), "POLLING",
                            "No column mappings found for polling configuration [" + _config.ORG_ID + "]. Site polling taken offline");

                        db_Air.InsertUpdatetT_QREST_SITES(_config.SITE_IDX, null, null, null, null, null, null, null,
                            null, null, null, null, null, null, null, false, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
                    }
                }
            }
            else
                General.WriteToFile("No sites to poll. " + System.DateTime.UtcNow);


            bExecutingGenLedSvcStatus = false;
        }


    }
}
