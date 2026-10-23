using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;


namespace QREST.Controllers
{

    //public class RawPackage
    //{
    //    public string APIKey { get; set; }
    //    public string SiteID { get; set; }
    //    public string ImportTemplateName { get; set; }
    //    public string[] rawRow { get; set; }
    //    //public RawData[] rawData { get; set; }
    //}

    public class RawPackageResponse
    {
        public bool SuccessInd { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<HourlyDataDisp> HourlyData { get; set; }
    }

    public class QRESTapiController : ApiController
    {
        // -------------------------------------------------------
        // Helper: centralizes Bearer token extraction and validation.
        // -------------------------------------------------------
        private T_QREST_ORG_USERS AuthenticateRequest(out RawPackageResponse errorResponse)
        {
            errorResponse = null;

            string authHeader = null;
            IEnumerable<string> headerValues;
            if (Request.Headers.TryGetValues("Authorization", out headerValues))
                authHeader = headerValues.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                errorResponse = new RawPackageResponse
                {
                    SuccessInd = false,
                    ErrorCode = 101,
                    ErrorMessage = "Missing or malformed Authorization header. Expected: Bearer <api_key>"
                };
                return null;
            }

            string apiKey = authHeader.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                errorResponse = new RawPackageResponse { SuccessInd = false, ErrorCode = 101, ErrorMessage = "Invalid API Key" };
                return null;
            }

            T_QREST_ORG_USERS uo = db_Account.GetT_QREST_ORG_USERS_ByAPIKey(apiKey);
            if (uo == null)
            {
                errorResponse = new RawPackageResponse { SuccessInd = false, ErrorCode = 101, ErrorMessage = "Invalid API Key" };
                return null;
            }

            return uo;
        }



        //[Route("api/QRESTAPI/SendFiveMin")]
        //[HttpPost]
        //public RawPackageResponse AjaxMethod(RawPackage rawpackage)
        //{
        //    bool OverallSuccessInd = false;
        //    string OverallErrorMsg = "";

        //    //step 0: fail if no data
        //    if (rawpackage.rawRow == null)
        //        return new RawPackageResponse { SuccessInd = false, ErrorCode = 100, ErrorMessage = "No data included in submission" };

        //    //step 1: find user and org based on API Key
        //    T_QREST_ORG_USERS _uo = db_Account.GetT_QREST_ORG_USERS_ByAPIKey(rawpackage.APIKey);
        //    if (rawpackage.APIKey == null || _uo == null)
        //        return new RawPackageResponse { SuccessInd = false, ErrorCode = 101, ErrorMessage = "Invalid API Key" };


        //    //step 2: verify the Site ID is correct for the API Key
        //    T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByOrgandSiteID(_uo.ORG_ID, rawpackage.SiteID);
        //    if (_site == null)
        //        return new RawPackageResponse { SuccessInd = false, ErrorCode = 102, ErrorMessage = "Site ID is invalid or does not match API Key" };

        //    //step 3: verify the Import Config
        //    T_QREST_SITE_POLL_CONFIG _pollConfig = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByNameAndSite(_site.SITE_IDX, rawpackage.ImportTemplateName);
        //    if (_pollConfig == null)
        //        return new RawPackageResponse { SuccessInd = false, ErrorCode = 103, ErrorMessage = "Import configuration with that name is not found" };

        //    //step 4: examine if selected import config is properly set up
        //    if (_pollConfig.DATE_COL == null && _pollConfig.TIME_COL == null)
        //        return new RawPackageResponse { SuccessInd = false, ErrorCode = 104, ErrorMessage = "Selected polling config does not define date and/or time column" };

        //    if (rawpackage.rawRow.Length > 48)
        //        return new RawPackageResponse { SuccessInd = false, ErrorCode = 105, ErrorMessage = "NO more than 48 records can be submitted at one time" };

        //    //prepare parse (get required info)
        //    SitePollingConfigType _config = db_Air.GetT_QREST_SITES_POLLING_CONFIG_Single(_pollConfig.POLL_CONFIG_IDX);
        //    List<SitePollingConfigDetailType> _config_dtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_config.POLL_CONFIG_IDX, true);
        //    if (_config != null && _config_dtl != null)
        //    {
        //        OverallSuccessInd = true;
        //        int i = 1;
        //        foreach (string xx in rawpackage.rawRow)
        //        {
        //            bool rowSuccessInd  = LoggerComm.ParseFlatFile(xx, _config, _config_dtl, false, true);
        //            if (rowSuccessInd == false)
        //            {
        //                OverallSuccessInd = false;  //set overall to fail if even 1 fails
        //                OverallErrorMsg += "Row " + i + " failed; ";
        //            }
        //            i++;
        //        }
        //    }

            
        //    RawPackageResponse resp = new RawPackageResponse {
        //        SuccessInd = OverallSuccessInd,
        //    };
        //    if (OverallErrorMsg.Length > 1)
        //        resp.ErrorMessage = OverallErrorMsg;
        //    return resp;
        //}



        // -------------------------------------------------------
        // GET /api/QRESTAPI/GetDataFormat?SiteID=yyy&ImportTemplateName=zzz Authorization: Bearer <api_key>
        // -------------------------------------------------------
        //[Route("api/QRESTAPI/GetDataFormat")]
        //[HttpGet]
        //public RawPackageResponse GetDataFormat(string SiteID, string ImportTemplateName)
        //{
        //    // Step 0: Log all requests
        //    db_Ref.CreateT_QREST_SYS_LOG(SiteID, "API", "GetDataFormat API called for SiteID=" + SiteID);

        //    // Step 1: authenticate via header
        //    T_QREST_ORG_USERS uo = AuthenticateRequest(out RawPackageResponse authError);
        //    if (uo == null) return authError;

        //    //step 2: verify the Site ID is correct for the API Key
        //    T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByOrgandSiteID(uo.ORG_ID, SiteID);
        //    if (_site == null)
        //        return new RawPackageResponse { SuccessInd = false, ErrorCode = 102, ErrorMessage = "Site ID is invalid or does not match API Key" };

        //    //step 3: verify the import config exists for the site
        //    T_QREST_SITE_POLL_CONFIG _pollConfig = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByNameAndSite(_site.SITE_IDX, ImportTemplateName);
        //    if (_pollConfig == null)
        //        return new RawPackageResponse { SuccessInd = false, ErrorCode = 103, ErrorMessage = "Import configuration with that name is not found" };

        //    //step 4: examine if selected import config is properly set up
        //    if (_pollConfig.DATE_COL == null && _pollConfig.TIME_COL == null)
        //        return new RawPackageResponse { SuccessInd = false, ErrorCode = 104, ErrorMessage = "Selected polling config does not define date and/or time column" };


        //    //Step 5: build field format list
        //    SitePollingConfigType _config = db_Air.GetT_QREST_SITES_POLLING_CONFIG_Single(_pollConfig.POLL_CONFIG_IDX);
        //    List<PollConfigDtlDisplay> _config_dtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID(_config.POLL_CONFIG_IDX);

        //    if (_config == null || _config_dtl == null)
        //        return new RawPackageResponse { SuccessInd = false, ErrorCode = 106, ErrorMessage = "Polling configuration is not complete (parameter field order not specified)" };

        //    var fields = new List<string>
        //    {
        //        $"Field {_config.DATE_COL}: Date Column in {_config.DATE_FORMAT} format",
        //        $"Field {_config.TIME_COL}: Time Column in {_config.TIME_FORMAT} format"
        //    };

        //    foreach (PollConfigDtlDisplay _dtl in _config_dtl)
        //    {
        //        if (_dtl.COL != null)
        //            fields.Add("Field " + _dtl.COL + ": " + _dtl.PAR_NAME + " (" + _dtl.PAR_CODE + ") with expected AQS unit of " + _dtl.COLLECT_UNIT_CODE);
        //    }

        //    return new RawPackageResponse { SuccessInd = true, rawRowFormat = fields.ToArray() };
        //}


        // -------------------------------------------------------
        // Returns the latest hourly readings for a site.
        //
        // Call: GET /api/QRESTAPI/GetLatestHourly?SiteID=1000 Authorization: Bearer <api_key>
        // -------------------------------------------------------
        [Route("api/QRESTAPI/GetLatestHourly")]
        [HttpGet]
        public RawPackageResponse GetLatestHourly(string SiteID)
        {
            // Step 0: Log all requests
            db_Ref.CreateT_QREST_SYS_LOG(SiteID, "API", "GetLatestHourly API called for SiteID=" + SiteID);

            // Step 1: authenticate via header
            T_QREST_ORG_USERS uo = AuthenticateRequest(out RawPackageResponse authError);
            if (uo == null) return authError;

            // Step 2: confirm the site exists and belongs to this org.
            T_QREST_SITES site = db_Air.GetT_QREST_SITES_ByOrgandSiteID(uo.ORG_ID, SiteID);
            if (site == null)
                return new RawPackageResponse { SuccessInd = false, ErrorCode = 102, ErrorMessage = "Site ID is invalid or does not match API Key" };

            // Step 3: fetch latest hourly data
            List<HourlyDataDisp> hourlyData = db_Air.GetT_QREST_DATA_HOURLY_MostRecentHourlyDataForSite(site.SITE_IDX);

            if (hourlyData == null || hourlyData.Count == 0)
                return new RawPackageResponse { SuccessInd = false, ErrorCode = 201, ErrorMessage = "No hourly data found for this site" };

            return new RawPackageResponse
            {
                SuccessInd = true,
                HourlyData = hourlyData
            };
        }
    }
}
