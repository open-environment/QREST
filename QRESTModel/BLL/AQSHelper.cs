using QRESTModel.DAL;
using QRESTModel.net.epacdxnode.testngn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Services.Protocols;
using QRESTModel.BLL;
using System.Security.Policy;

namespace QRESTModel.AQSHelper
{
    public class CDXCredentials
    {
        public string userID { get; set; }
        public string credential { get; set; }
        public string NodeURL { get; set; }
    }

    public class AQSHelper
    {
        #region ************************************** FILE GENERATION ******************************************************

        public static Guid? AQSGeneration_Orchestrator(string org_id, Guid siteIDX, IList<Guid> selMons, DateTime startDt, DateTime endDt, string UserIDX, string actionCd, string format)
        {
            //Step 1: Create tracking record
            Guid? _aqsid = db_Air.InsertUpdateT_QREST_AQS(null, org_id, siteIDX, null, startDt, endDt, null, null, null, "Started", UserIDX, null, null, null);

            //Step 2: Generate file and store to the previously created record
            if (_aqsid != null)
                return AQSHelper.GenerateAQSFile(_aqsid.GetValueOrDefault(), siteIDX, selMons, startDt, endDt, actionCd, format, org_id);

            return null; 
        }

        public static Guid? AQS_QA_Generation_Orchestrator(string org_id, Guid siteIDX, Guid qC_ASSESS_IDX, string UserIDX, string actionCd, string format)
        {
            //Step 1: Create tracking record
            Guid? _aqsid = db_Air.InsertUpdateT_QREST_AQS(null, org_id, siteIDX, null, null, null, null, null, null, "Started", UserIDX, null, null, null);

            //Step 2: Generate file and store to the previously created record
            if (_aqsid != null)
                return AQSHelper.GenerateAQS_QA_File(_aqsid.GetValueOrDefault(), qC_ASSESS_IDX, actionCd, format, org_id);

            return null;
        }


        public static Guid? GenerateAQSFile(Guid aqsIDX, Guid siteIDX, IList<Guid> selMons, DateTime startDate, DateTime endDate, string actionCode, string format, string org_id)
        {
            //***************  STEP 0: GET AQS SCREENING GROUP AND AQS USER ID***************  
            T_QREST_ORGANIZATIONS _org = db_Ref.GetT_QREST_ORGANIZATION_ByID(org_id);
            if (_org != null)
            {
                string _AQS_UID = _org.AQS_AQS_UID;
                string _AQS_SCREENING_GRP = _org.AQS_AQS_SCREENING_GRP;

                //***************  STEP 1: GENERATE FILE***************  
                if (format == "F")
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        StreamWriter writer = new StreamWriter(ms);

                        //add raw data lines
                        foreach (Guid selMon in selMons)
                        {
                            List<RawDataDisplay> _rds = db_Air.GetT_QREST_DATA_HOURLY_AQSReady(siteIDX, selMon, startDate, endDate);
                            if (_rds != null)
                            {
                                foreach (RawDataDisplay _rd in _rds)
                                {
                                    string dt = _rd.DATA_DTTM.Value.ToString("yyyyMMdd");
                                    string tm = _rd.DATA_DTTM.Value.ToString("HH:mm");
                                    string val = (string.IsNullOrEmpty(_rd.AQS_NULL_CODE) && _rd.DATA_VALUE.IsNumeric()) ? _rd.DATA_VALUE : "";
                                    string nullValCd = val.Length == 0 ? "|" + _rd.AQS_NULL_CODE : "";
                                    writer.WriteLine("RD|" + actionCode + "|" + _rd.STATE_CD + "|" + _rd.COUNTY_CD + "|" + _rd.AQS_SITE_ID + "|" + _rd.PAR_CODE + "|" + _rd.POC + "|1|" + _rd.UNIT_CODE + "|" + _rd.METHOD_CODE + "|" + dt + "|" + tm + "|" + val + nullValCd);
                                }
                            }
                        }

                        writer.Flush();

                        string fileText = Encoding.UTF8.GetString(ms.ToArray());

                        //add network header
                        string fileWithHeader = AddENHeaderToFileAndSave(aqsIDX, fileText, format, _AQS_UID, _AQS_SCREENING_GRP);
                        byte[] fileWithHeaderArray = Encoding.UTF8.GetBytes(fileWithHeader);
                        MemoryStream msFormXml = new MemoryStream(fileWithHeaderArray);

                        //zip it up
                        byte[] zipbyteArray = UtilsZip.CreateZIPFileFromMemoryStream(msFormXml, "AQSSubmission.xml");

                        //save updated file with Header version and non-header version
                        string ext = format == "X" ? ".xml" : ".txt";
                        string fileName = "AQS_" + startDate.ToString("yyyyMMdd") + "_" + endDate.ToString("yyyyMMdd") + ext;

                        Guid? SuccID = db_Air.InsertUpdateT_QREST_AQS(aqsIDX, null, null, fileName, null, null, zipbyteArray, null, null, "File Created", null, fileText, null, null);
                        return SuccID;
                    }


                }
                else if (format == "X")
                {

                }
            }
            return null;
        }

        public static Guid? GenerateAQS_QA_File(Guid aqsIDX, Guid selQC_ASSESS_IDX, string actionCode, string format, string org_id)
        {
            //***************  STEP 0: GET AQS SCREENING GROUP AND AQS USER ID***************  
            T_QREST_ORGANIZATIONS _org = db_Ref.GetT_QREST_ORGANIZATION_ByID(org_id);
            if (_org != null)
            {
                string _AQS_UID = _org.AQS_AQS_UID;
                string _AQS_SCREENING_GRP = _org.AQS_AQS_SCREENING_GRP;

                //***************  STEP 1: GENERATE FILE***************  
                if (format == "F")
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        StreamWriter writer = new StreamWriter(ms);

                        //add QA data lines
                        QcAssessmentAqsDisplay _assess = db_Air.GetT_QREST_QC_ASSESSMENT_AQSData_ByID(selQC_ASSESS_IDX);
                        if (_assess != null)
                        {
                            string dt = _assess.T_QREST_QC_ASSESSMENT.ASSESSMENT_DT.ToString("yyyyMMdd");
                            string line = "QA|" + actionCode + "|" + _assess.T_QREST_QC_ASSESSMENT.ASSESSMENT_TYPE + "||" + _assess.STATE_CODE + "|" + _assess.COUNTY_CD + "|" + _assess.AQS_SITE_ID + "|" + _assess.PAR_CODE + "|" + _assess.POC + "|" + dt + "|" + _assess.T_QREST_QC_ASSESSMENT.ASSESSMENT_NUM.ToString() + _assess.METHOD_CODE + "|" + _assess.UNIT_CODE + "|";


                            List<QC_ASSESSMENT_DTLDisplay> _rds = db_Air.GetT_QREST_QC_ASSESSMENT_DTL_ByAssessID(selQC_ASSESS_IDX);
                            if (_rds != null && _rds.Count>0)
                            {
                                //*************** 1 POINT QC LOGIC **************************************
                                if (_assess.T_QREST_QC_ASSESSMENT.ASSESSMENT_TYPE == "1-Point QC")
                                {
                                    line = line + _rds[0].MON_CONCENTRATION + "|" + _rds[0].ASSESS_KNOWN_CONCENTRATION + "||" + _rds[0].COMMENTS;
                                }

                            }

                            writer.WriteLine(line);
                            writer.Flush();

                            string fileText = Encoding.UTF8.GetString(ms.ToArray());

                            //add network header
                            string fileWithHeader = AddENHeaderToFileAndSave(aqsIDX, fileText, format, _AQS_UID, _AQS_SCREENING_GRP);
                            byte[] fileWithHeaderArray = Encoding.UTF8.GetBytes(fileWithHeader);
                            MemoryStream msFormXml = new MemoryStream(fileWithHeaderArray);

                            //zip it up
                            byte[] zipbyteArray = UtilsZip.CreateZIPFileFromMemoryStream(msFormXml, "AQSSubmission.xml");

                            //save updated file with Header version and non-header version
                            string ext = format == "X" ? ".xml" : ".txt";
                            string fileName = "AQS_QA_" + dt + ext;

                            Guid? SuccID = db_Air.InsertUpdateT_QREST_AQS(aqsIDX, null, null, fileName, null, null, zipbyteArray, null, null, "File Created", null, fileText, null, null);
                            return SuccID;
                        }
                    }


                }
                else if (format == "X")
                {

                }
            }
            return null;
        }


        public static string AddENHeaderToFileAndSave(Guid AQSIdx, string fileBody, string format, string AQS_UID, string AQS_SCREENING_GRP)
        {
            //parameters
            string AQSUserID = AQS_UID;//"ROE";                                               // AQS User-ID (aka The user ID for the backend system if it is different from the NAAS user ID.)
            string AQSScreeningGroup = AQS_SCREENING_GRP;// "ITEP TRAINING";                             // AQS Screening Group (like the agency)
            string creationDate = System.DateTime.UtcNow.ToString("yyyy-MM-dd");    // "2020-02-05T15:15:36.826673Z";
            string creationTime = System.DateTime.UtcNow.ToString("HH:mm:ss");      // "2020-02-05T15:15:36.826673Z";
            string payloadType = format == "X" ? "XML" : "FLAT";                    // acceptable values are XML or FLAT

            if (format != "X")
                fileBody = "<![CDATA[" + fileBody + "]]>";

            string headerBottom = "</Payload></Document>";
            string headerTop = @"<?xml version=""1.0"" encoding=""utf-8""?><Document xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" id=""" + "ID" + AQSIdx.ToString() + @""" xmlns=""http://www.exchangenetwork.net/schema/header/2"">
  <Header>
    <AuthorName>doug.timms@open-Environment.org</AuthorName>
    <OrganizationName>ITEP</OrganizationName>
    <DocumentTitle>AQSRawDataSubmission</DocumentTitle>
    <CreationDateTime>" + creationDate + "T" + creationTime + "Z" + @"</CreationDateTime>
    <DataFlowName>AQS</DataFlowName>
    <DataServiceName>AQSRawDataSubmission</DataServiceName>
    <SenderContact>doug.timms@open-Environment.org</SenderContact>
    <ApplicationUserIdentifier>" + AQSUserID + @"</ApplicationUserIdentifier>
    <Property>
      <PropertyName>AQS.ScreeningGroup</PropertyName>
      <PropertyValue xsi:type=""xsd:string"">" + AQSScreeningGroup + @"</PropertyValue>
    </Property>
    <Property>
      <PropertyName>AQS.FinalProcessingStep</PropertyName>
      <PropertyValue xsi:type=""xsd:string"">Post</PropertyValue>
    </Property>
    <Property>
      <PropertyName>AQS.PayloadType</PropertyName>
      <PropertyValue xsi:type=""xsd:string"">" + payloadType  + @"</PropertyValue>
    </Property>
    <Property>
      <PropertyName>AQS.StopOnError</PropertyName>
      <PropertyValue xsi:type=""xsd:string"">No</PropertyValue>
    </Property>
  </Header>
  <Payload>";

            return headerTop + fileBody + headerBottom;
        }

        #endregion


        #region ********************************** FILE SUBMISSION ******************************************************

        public static bool AQSSubmission_Orchestrator(string org_id, byte[] bytes, Guid aQS_IDX)
        {
            //FIRST AUTHENTICATE
            CDXCredentials _cred = AQSHelper.GetCDXSubmitCredentials2(org_id);
            string _token = AQSHelper.AuthHelper(_cred.userID, _cred.credential, _cred.NodeURL);

            if (_token != null)
            {
                StatusResponseType subStatus = SubmitHelper(_cred.NodeURL, _token, "AQS", "default", bytes, "AQSsubmit.zip", DocumentFormatType.ZIP, "1");
                if (subStatus != null) {

                    db_Air.InsertUpdateT_QREST_AQS(aQS_IDX, null, null, null, null, null, null, null, null, "Submitted-Processing", null, null, subStatus.transactionId, null);
                    return true;
                }
            }

            return false;
        }

        public static string AQSGetStatus_Orchestrator(string org_id, Guid aQS_IDX, string transID)
        {
            //FIRST AUTHENTICATE TO GET NEW SECURITY TOKEN
            CDXCredentials _cred = AQSHelper.GetCDXSubmitCredentials2(org_id);
            string _token = AQSHelper.AuthHelper(_cred.userID, _cred.credential, _cred.NodeURL);
            if (_token != null)
            {
                StatusResponseType subStatus = GetStatusHelper(_cred.NodeURL, _token, transID);
                if (subStatus != null)
                {
                    string strStatus = subStatus.status.ToString();
                    string strStatusDtl = subStatus.statusDetail.ToString();
                    db_Air.InsertUpdateT_QREST_AQS(aQS_IDX, null, null, null, null, null, null, null, null, strStatus, null, null, null, null);
                    return strStatus;
                }
            }
            return null;
        }

        public static bool AQSDownload_Orchestrator(string org_id, Guid aQS_IDX, string transID)
        {
            //FIRST AUTHENTICATE TO GET NEW SECURITY TOKEN
            CDXCredentials _cred = AQSHelper.GetCDXSubmitCredentials2(org_id);
            string _token = AQSHelper.AuthHelper(_cred.userID, _cred.credential, _cred.NodeURL);
            if (_token != null)
            {
                int iCount = 0;
                NodeDocumentType[] dlResp = DownloadHelper(_cred.NodeURL, _token, "AQS", transID);
                foreach (NodeDocumentType ndt in dlResp)
                {
                    if (ndt.documentName.Contains("submit.xml") || ndt.documentName.Contains("Processing") || ndt.documentName.Contains("submit.zip"))
                    {
                        Byte[] resp1 = dlResp[iCount].documentContent.Value;
                        db_Air.InsertUpdateT_QREST_AQS(aQS_IDX, null, null, null, null, null, null, null, null, null, null, null, null, resp1);
                    }
                    iCount += 1;
                }

                return iCount > 0;
            }

            return false;
        }


        /// <summary>
        /// Calls Exchange Network authenticate method
        /// </summary>
        /// <returns>Security token if valid or null if failed</returns>
        public static string AuthHelper(string userID, string credential, string NodeURL)
        {
            NetworkNode2 nn = new NetworkNode2();
            nn.Url = NodeURL;
            Authenticate auth1 = new Authenticate {
                userId = userID,
                credential = credential,
                authenticationMethod = "Password",
                domain = "default"
            };

            try
            {
                AuthenticateResponse resp = nn.Authenticate(auth1);
                return resp.securityToken;
            }
            catch (SoapException sExept)
            {
                //logging an authentication failure
                db_Ref.CreateT_QREST_SYS_LOG(userID, "ERROR", "Authenticate error when submitting AQS: " + sExept.Message);
                return null;
            }
        }


        /// <summary>
        /// Calls Exchange Network submit method
        /// </summary>
        /// <param name="NodeURL"></param>
        /// <param name="secToken"></param>
        /// <param name="dataFlow"></param>
        /// <param name="flowOperation"></param>
        /// <param name="doc"></param>
        /// <param name="docName"></param>
        /// <param name="docFormat"></param>
        /// <param name="docID"></param>
        /// <returns></returns>
        internal static StatusResponseType SubmitHelper(string NodeURL, string secToken, string dataFlow, string flowOperation, byte[] doc, string docName, DocumentFormatType docFormat, string docID)
        {
            try
            {
                AttachmentType att1 = new AttachmentType();
                att1.Value = doc;
                NodeDocumentType doc1 = new NodeDocumentType();
                doc1.documentName = docName;
                doc1.documentFormat = docFormat;
                doc1.documentId = docID;
                doc1.documentContent = att1;
                NodeDocumentType[] docArray = new NodeDocumentType[1];
                docArray[0] = doc1;
                Submit sub1 = new Submit();
                sub1.securityToken = secToken;
                sub1.transactionId = "";
                sub1.dataflow = dataFlow;
                sub1.flowOperation = flowOperation;
                sub1.documents = docArray;
                NetworkNode2 nn = new NetworkNode2();
                nn.SoapVersion = SoapProtocolVersion.Soap12;
                nn.Url = NodeURL;
                return nn.Submit(sub1);
            }
            catch (SoapException sExept)
            {                
                //logging an authentication failure
                db_Ref.CreateT_QREST_SYS_LOG("", "ERROR", "Submit error when submitting AQS: " + sExept.Message);
                return null;
            }
        }


        /// <summary>
        /// Calls Exchange Network GetStatus method
        /// </summary>
        /// <param name="NodeURL"></param>
        /// <param name="secToken"></param>
        /// <param name="transID"></param>
        /// <returns></returns>
        internal static StatusResponseType GetStatusHelper(string NodeURL, string secToken, string transID)
        {
            try
            {
                NetworkNode2 nn = new NetworkNode2();
                nn.Url = NodeURL;
                GetStatus gs1 = new GetStatus();
                gs1.securityToken = secToken;
                gs1.transactionId = transID;
                return nn.GetStatus(gs1);
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Calls Exchange Network Download Method
        /// </summary>
        /// <param name="NodeURL"></param>
        /// <param name="secToken"></param>
        /// <param name="dataFlow"></param>
        /// <param name="transID"></param>
        /// <returns></returns>
        internal static NodeDocumentType[] DownloadHelper(string NodeURL, string secToken, string dataFlow, string transID)
        {
            try
            {
                NetworkNode2 nn = new NetworkNode2();
                nn.Url = NodeURL;
                Download dl1 = new Download();
                dl1.securityToken = secToken;
                dl1.dataflow = dataFlow;
                dl1.transactionId = transID;
                return nn.Download(dl1);
            }
            catch
            {
                return null;
            }

        }

        #endregion


        #region ********************************** SHARED ******************************************************

        public static CDXCredentials GetCDXSubmitCredentials2(string OrgID)
        {
            //production
            //    NodeURL = "https://cdxnodengn.epa.gov/ngn-enws20/services/NetworkNode2ServiceConditionalMTOM"; //new 2.1
            //    NodeURL = "https://cdxnodengn.epa.gov/ngn-enws20/services/NetworkNode2Service"; //new 2.0
            //test
            //    NodeURL = "https://testngn.epacdxnode.net/ngn-enws20/services/NetworkNode2ServiceConditionalMTOM"; //new 2.1
            //    NodeURL = "https://testngn.epacdxnode.net/ngn-enws20/services/NetworkNode2Service";  //new 2.0
            //    NodeURL = "https://test.epacdxnode.net/cdx-enws20/services/NetworkNode2ConditionalMtom"; //old 2.1

            var cred = new CDXCredentials();
            try
            {
                cred.NodeURL = db_Ref.GetT_QREST_APP_SETTING("CDX_URL");

                //get NAAS username and password
                T_QREST_ORGANIZATIONS _org = db_Ref.GetT_QREST_ORGANIZATION_ByID(OrgID);
                if (_org != null)
                {
                    if (!string.IsNullOrEmpty(_org.AQS_NAAS_UID) && !string.IsNullOrEmpty(_org.AQS_NAAS_PWD))
                    {
                        cred.userID = _org.AQS_NAAS_UID;
                        cred.credential = new SimpleAES().Decrypt(_org.AQS_NAAS_PWD);
                    }
                    else
                    {
                        cred.userID = db_Ref.GetT_QREST_APP_SETTING("CDX_GLOBAL_USER");
                        cred.credential = db_Ref.GetT_QREST_APP_SETTING("CDX_GLOBAL_PWD");
                    }
                }
            }
            catch { }

            return cred;
        }

        #endregion


    }
}
