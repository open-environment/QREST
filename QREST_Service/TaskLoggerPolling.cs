﻿using QREST_Service;
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

                        //****************** ZENO OR SUTRON DATA LOGGER *********************************************************
                        //****************** ZENO OR SUTRON DATA LOGGER *********************************************************
                        //****************** ZENO OR SUTRON DATA LOGGER *********************************************************
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
                                General.WriteToFile("Campbell data found for:" + _config.ORG_ID + " site: " + _config.SITE_ID + "###" + _file.Substring(1,10));

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
                            null, null, null, null, null, null, null, null);
                    }
                }
            }
            else
                General.WriteToFile("No sites to poll.");


            bExecutingGenLedSvcStatus = false;
        }


    }
}
