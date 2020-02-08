using QREST_Service;
using QRESTModel.COMM;
using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;

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
                    List<SitePollingConfigDetailType> _config_dtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_config.POLL_CONFIG_IDX);
                    if (_config_dtl != null && _config_dtl.Count > 0)
                    {
                        General.WriteToFile("Start poll for org:" + _config.ORG_ID + " site: " + _config.SITE_ID);

                        //****************** ZENO OR SUTRON DATA LOGGER *********************************************************
                        //****************** ZENO OR SUTRON DATA LOGGER *********************************************************
                        //****************** ZENO OR SUTRON DATA LOGGER *********************************************************
                        if (_config.LOGGER_TYPE == "ZENO" || _config.LOGGER_TYPE=="SUTRON")
                        {
                            CommMessageLog _log = LoggerComm.ConnectTcpClientSailer(_config.LOGGER_SOURCE, (ushort)_config.LOGGER_PORT, _config.LOGGER_PASSWORD, "#" + _config.SITE_ID + "0001DL" + recCount + ",");
                            if (_log.CommMessageStatus && _log.CommResponse != null && _log.CommResponse.Length > 20)
                            {
                                //remove extraneous stuff from beginning of response file from logger
                                _log.CommResponse = LoggerComm.stripMessage(_log.CommResponse, "#0001" + _config.SITE_ID);

                                //send the entire text response to the file parser routine
                                ParseFile(_log.CommResponse, _config, _config_dtl);

                                //log the text to file (for future auditing of parse accuracy)
                                General.WriteToPollingFile(_log.CommResponse, _config.SITE_ID);
                            }
                            else
                                General.WriteToFile("Error in polling:" + _config.ORG_ID + " site: " + _config.SITE_ID + " ###" + _log.CommMessageType + ":" +  _log.CommResponse);

                        }

                        General.WriteToFile("End poll for org:" + _config.ORG_ID + " site: " + _config.SITE_ID);
                    }
                    else
                        db_Ref.CreateT_QREST_SYS_LOG(_config.SITE_IDX.ToString(),"POLLING", "No column mappings found for polling configuration");
                }
            }
                        
            bExecutingGenLedSvcStatus = false;
        }


        public static bool ParseFile(string loggerData, SitePollingConfigType config, List<SitePollingConfigDetailType> config_dtl)
        {
            try
            {
                string line;
                using (System.IO.StringReader sr = new System.IO.StringReader(loggerData))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length > 0)
                        {
                            //FIVE MINUTE RAW DATA
                            if (config.RAW_DURATION_CODE == "H")
                                db_Air.InsertT_QREST_DATA_FIVE_MIN_fromLine(line, config, config_dtl);
                            //ONE MINUTE RAW DATA
                            //if (config.RAW_DURATION_CODE == "G")
                            //    db_Air.InsertT_QREST_DATA_ONE_MIN_fromLine(line, config, config_dtl);
                        }
                    }
                }

                //update next run for the site
                DateTime nextrun = System.DateTime.Now.AddMinutes(15);  //default to 15 minutes next run
                if (config.POLLING_FREQ_TYPE == "M")
                    nextrun = System.DateTime.Now.AddMinutes(config.POLLING_FREQ_NUM ?? 15);

                db_Air.InsertUpdatetT_QREST_SITES(config.SITE_IDX, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                    System.DateTime.Now, nextrun, null, null, null, null, null, null);

                return true;
            }
            catch (Exception ex)
            {
                db_Ref.CreateT_QREST_SYS_LOG(null, "POLLING", ex.InnerException?.ToString());
                return false;
            }
        }
    }
}
