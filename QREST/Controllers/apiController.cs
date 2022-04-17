using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using QRESTModel.COMM;
using QRESTModel.DAL;

namespace QREST.Controllers
{
    public class RawData
    {
        public string TransactionType { get; set; }  //RD
        public string ActionCode { get; set; }  //I for insert   D for delete
        public string QRESTMonitorID { get; set; }   //internal QREST Monitor ID
            public string SiteID { get; set; }
            public string POC { get; set; }      //e.g. 001
            public string SampleDuration { get; set; }   //5 min
            public string Unit { get; set; }           //AQS or non-AQS QREST unit code
            public string Method { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string SampleValue { get; set; }
        public string NullDataCode { get; set; }
        public List<string> QualCode { get; set; }
        public string RawDataRow { get; set; }
    }


    public class RawPackage
    {
        public string APIKey { get; set; }
        public string SiteID { get; set; }
        public string ImportTemplateName { get; set; }
        public string[] rawRow { get; set; }
        public RawData[] rawData { get; set; }
    }

    public class RawPackageResponse
    {
        public bool SuccessInd { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string[] rawRowFormat { get; set; }
    }

    public class QRESTapiController : ApiController
    {

        [Route("api/QRESTAPI/SendFiveMin")]
        [HttpPost]
        public RawPackageResponse AjaxMethod(RawPackage rawpackage)
        {
            bool OverallSuccessInd = false;
            string OverallErrorMsg = "";

            //step 0: fail if no data
            if (rawpackage.rawRow == null)
                return new RawPackageResponse { SuccessInd = false, ErrorCode = 100, ErrorMessage = "No data included in submission" };

            //step 1: find user and org based on API Key
            T_QREST_ORG_USERS _uo = db_Account.GetT_QREST_ORG_USERS_ByAPIKey(rawpackage.APIKey);
            if (rawpackage.APIKey == null || _uo == null)
                return new RawPackageResponse { SuccessInd = false, ErrorCode = 101, ErrorMessage = "Invalid API Key" };


            //step 2: verify the Site ID is correct for the API Key
            T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByOrgandSiteID(_uo.ORG_ID, rawpackage.SiteID);
            if (_site == null)
                return new RawPackageResponse { SuccessInd = false, ErrorCode = 102, ErrorMessage = "Site ID is invalid or does not match API Key" };

            //step 3: verify the Import Config
            T_QREST_SITE_POLL_CONFIG _pollConfig = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByNameAndSite(_site.SITE_IDX, rawpackage.ImportTemplateName);
            if (_pollConfig == null)
                return new RawPackageResponse { SuccessInd = false, ErrorCode = 103, ErrorMessage = "Import configuration with that name is not found" };

            //step 4: examine if selected import config is properly set up
            if (_pollConfig.DATE_COL == null && _pollConfig.TIME_COL == null)
                return new RawPackageResponse { SuccessInd = false, ErrorCode = 104, ErrorMessage = "Selected polling config does not define date and/or time column" };

            if (rawpackage.rawRow.Length > 48)
                return new RawPackageResponse { SuccessInd = false, ErrorCode = 105, ErrorMessage = "NO more than 48 records can be submitted at one time" };

            //prepare parse (get required info)
            SitePollingConfigType _config = db_Air.GetT_QREST_SITES_POLLING_CONFIG_Single(_pollConfig.POLL_CONFIG_IDX);
            List<SitePollingConfigDetailType> _config_dtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_config.POLL_CONFIG_IDX, true);
            if (_config != null && _config_dtl != null)
            {
                OverallSuccessInd = true;
                int i = 1;
                foreach (string xx in rawpackage.rawRow)
                {
                    bool rowSuccessInd  = LoggerComm.ParseFlatFile(xx, _config, _config_dtl, false, true);
                    if (rowSuccessInd == false)
                    {
                        OverallSuccessInd = false;  //set overall to fail if even 1 fails
                        OverallErrorMsg += "Row " + i + " failed; ";
                    }
                    i++;
                }
            }

            
            RawPackageResponse resp = new RawPackageResponse {
                SuccessInd = OverallSuccessInd,
            };
            if (OverallErrorMsg.Length > 1)
                resp.ErrorMessage = OverallErrorMsg;
            return resp;
        }




        [Route("api/QRESTAPI/GetDataFormat")]
        [HttpGet]
        public RawPackageResponse GetDataFormat(string APIKey, string SiteID, string ImportTemplateName)
        {
            //step 1: find user and org based on API Key
            T_QREST_ORG_USERS _uo = db_Account.GetT_QREST_ORG_USERS_ByAPIKey(APIKey);
            if (APIKey == null || _uo == null)
                return new RawPackageResponse { SuccessInd = false, ErrorCode = 101, ErrorMessage = "Invalid API Key" };

            //step 2: verify the Site ID is correct for the API Key
            T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByOrgandSiteID(_uo.ORG_ID, SiteID);
            if (_site == null)
                return new RawPackageResponse { SuccessInd = false, ErrorCode = 102, ErrorMessage = "Site ID is invalid or does not match API Key" };

            //step 3: verify the Import Config
            T_QREST_SITE_POLL_CONFIG _pollConfig = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByNameAndSite(_site.SITE_IDX, ImportTemplateName);
            if (_pollConfig == null)
                return new RawPackageResponse { SuccessInd = false, ErrorCode = 103, ErrorMessage = "Import configuration with that name is not found" };

            //step 4: examine if selected import config is properly set up
            if (_pollConfig.DATE_COL == null && _pollConfig.TIME_COL == null)
                return new RawPackageResponse { SuccessInd = false, ErrorCode = 104, ErrorMessage = "Selected polling config does not define date and/or time column" };


            //prepare parse (get required info)
            SitePollingConfigType _config = db_Air.GetT_QREST_SITES_POLLING_CONFIG_Single(_pollConfig.POLL_CONFIG_IDX);
            List<PollConfigDtlDisplay> _config_dtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID(_config.POLL_CONFIG_IDX);
            if (_config != null && _config_dtl != null)
            {
                List<string> fields = new List<string>();
                fields.Add("Field " + _config.DATE_COL + ": Date Column in " + _config.DATE_FORMAT + " format");
                fields.Add("Field " + _config.TIME_COL + ": Time Column in " + _config.TIME_FORMAT + " format");

                foreach (PollConfigDtlDisplay _dtl in _config_dtl)
                {
                    if (_dtl.COL != null)
                        fields.Add("Field " + _dtl.COL + ": " + _dtl.PAR_NAME + " (" + _dtl.PAR_CODE + ") with expected AQS unit of " + _dtl.COLLECT_UNIT_CODE);
                }

                return new RawPackageResponse
                {
                    SuccessInd = true,
                    rawRowFormat = fields.ToArray()
                };
            }
            else
                return new RawPackageResponse { SuccessInd = false, ErrorCode = 106, ErrorMessage = "Polling confiuration is not complete (parameter field order not specified)" };


        }


    }
}
