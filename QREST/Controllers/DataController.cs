using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using QREST.App_Logic.BusinessLogicLayer;
using QREST.Models;
using QRESTModel.DAL;
using QRESTModel.AQSHelper;
using System.Data;
using QRESTModel.DataTableGen;
using QREST.App_Logic;
using ClosedXML.Excel;

namespace QREST.Controllers
{
    [Authorize]
    public class DataController : Controller
    {
        // GET: Data
        public ActionResult Index()
        {
            return View();
        }

        #region MANUAL IMPORT

        public ActionResult ManualImport(Guid? configid)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataImport {
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true),
                ddl_Sites = new List<SelectListItem>(),
                ddl_Monitors = new List<SelectListItem>(),
                ddl_PollConfig = new List<SelectListItem>(),
                ddl_ImportType = ddlHelpers.get_ddl_import_type(),
                ddl_Time = ddlHelpers.get_ddl_time_type(),
                ddl_TimeZone = ddlHelpers.get_ddl_time_zone(),
                ddl_Calc = ddlHelpers.get_ddl_yes_no(),
                selTimeType = "L"
            };

            //if passing in configuration ID, can prepopulate the org, site, and poll config list
            if (configid != null)
            {
                model.selPollConfig = configid;

                T_QREST_SITE_POLL_CONFIG _config = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(configid.GetValueOrDefault());
                if (_config != null)
                {
                    T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(_config.SITE_IDX);
                    if (_site != null)
                    {
                        model.selOrgID = _site.ORG_ID;
                        model.selSite = _config.SITE_IDX;
                        model.selImportType = "H";
                        model.ddl_Sites = ddlHelpers.get_ddl_my_sites(model.selOrgID, UserIDX);
                        model.ddl_PollConfig = ddlHelpers.get_ddl_import_templates(_config.SITE_IDX);
                    }
                }

            }


            return View(model);
        }


        [HttpPost]
        public ActionResult ManualImport(vmDataImport model) {

            string UserIDX = User.Identity.GetUserId();
            Guid importID = Guid.NewGuid();
            model.IMPORT_IDX = null;
            model.ImportSuccCount = 0;
            string[] allRows = model.IMPORT_BLOCK.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string[] allowedFormats = new[] { "MM/dd/yyyy HH:mm", "M/dd/yyyy HH:mm", "MM/d/yyyy HH:mm", "M/d/yyyy HH:mm", "MM/dd/yyyy H:mm", "M/dd/yyyy H:mm", "MM/d/yyyy H:mm", "M/d/yyyy H:mm" };
            T_QREST_SITE_POLL_CONFIG _pollConfig = null;
            model.error_data = new List<ImportResponse>();

            //**********************************************************************************************
            //*********************** MODEL VALIADTION PRIOR TO IMPORT**************************************
            //**********************************************************************************************
            if (model.selImportType == "H1" && model.selMonitor==null)
                ModelState.AddModelError("selMonitor", "Parameter required for this import type.");
            if (model.selImportType == "H1" && string.IsNullOrEmpty(model.selTimeZone))
                ModelState.AddModelError("selTimeZone", "Local Time Zone required for this import type.");
            if (model.selImportType == "H" && model.selPollConfig == null)
                ModelState.AddModelError("selPollConfig", "Import Template is required.");
            if (model.selImportType == "F" && model.selPollConfig == null)
                ModelState.AddModelError("selPollConfig", "Import Template is required.");
            if (model.selImportType == "H" || model.selImportType == "F")
            {
                _pollConfig = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(model.selPollConfig ?? Guid.Empty);
                if (_pollConfig != null)
                {
                    if (_pollConfig.DATE_COL == null && _pollConfig.TIME_COL == null) 
                        ModelState.AddModelError("selPollConfig", "Selected polling config does not define date and/or time column.");
                }
                else
                    ModelState.AddModelError("selPollConfig", "Polling configuration cannot be found.");

                //first need to make sure all import config columns have units
                List<SitePollingConfigDetailType> _temps = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(model.selPollConfig.GetValueOrDefault(), false);
                foreach (SitePollingConfigDetailType _temp in _temps)
                {
                    if (_temp.COLLECT_UNIT_CODE == null || _temp.COL == null)
                        ModelState.AddModelError("selPollConfig", "One or more parameters in your import configuration do not have a unit or column specified.");
                }

                //limit 2000 rows
                if (allRows.Length > 2000)
                    ModelState.AddModelError("IMPORT_BLOCK", "Please limit to a maximum of 2000 rows at a time.");

            }
            //**********************************************************************************************
            //*********************** END MODEL VALIADTION *************************************************
            //**********************************************************************************************


            if (ModelState.IsValid)            
            {
                Guid importIDX = Guid.NewGuid();

                //**************************************************************************************
                //    F                five-minute
                //**************************************************************************************
                if (model.selImportType == "F")
                {
                    char[] delimiter = _pollConfig.DELIMITER == "C" ? new char[] { ',' } : new char[] { '\t' };
                    int dateCol = (_pollConfig.DATE_COL ?? 2) - 1;
                    int timeCol = (_pollConfig.TIME_COL ?? 3) - 1;

                    //get polling config dtl
                    List<SitePollingConfigDetailType> _pollConfigDtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_pollConfig.POLL_CONFIG_IDX, true);

                    //import
                    foreach (string row in allRows)
                    {
                        //split row's columns into string array
                        string[] cols = row.Split(delimiter, StringSplitOptions.None);
                        if (cols.Length > 2) //skip blank rows
                        {
                            DateTime dt;

                            if (DateTime.TryParseExact(cols[dateCol] + " " + cols[timeCol], allowedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                            {
                                foreach (SitePollingConfigDetailType _item in _pollConfigDtl)
                                {
                                    string val = cols[(_item.COL ?? 1) - 1].ToString().Trim();

                                    ImportResponse xxx = db_Air.ImportT_QREST_DATA_FIVE_MIN(_item.MONITOR_IDX, dt, double.TryParse(val, out _) ? val : null, _item.COLLECT_UNIT_CODE, model.selCalc == "N" ? true : false, "", model.selCalc == "N" ? new DateTime(1888, 8, 8) : System.DateTime.Now, importIDX, _pollConfig.TIME_POLL_TYPE, _pollConfig.LOCAL_TIMEZONE.ConvertOrDefault<int>());
                                    if (xxx.SuccInd)
                                        model.ImportSuccCount += 1;
                                    else
                                        model.error_data.Add(xxx);
                                }
                            }
                            else
                                ModelState.AddModelError("IMPORT_BLOCK", "Date and/or time format cannot be read.");
                        }
                    }

                }


                //**************************************************************************************
                //    H                hourly
                //**************************************************************************************
                else if (model.selImportType == "H")
                {
                    int tzOffset = _pollConfig.LOCAL_TIMEZONE.ConvertOrDefault<int>();
                    char[] delimiter = _pollConfig.DELIMITER == "C" ? new char[] { ',' } : new char[] { '\t' };

                    int dateCol = (_pollConfig.DATE_COL ?? 2) - 1;
                    int timeCol = (_pollConfig.TIME_COL ?? 3) - 1;

                    //get polling config dtl
                    List<SitePollingConfigDetailType> _pollConfigDtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_pollConfig.POLL_CONFIG_IDX, true);

                    //import
                    foreach (string row in allRows)
                    {
                        //split row's columns into string array
                        string[] cols = row.Split(delimiter, StringSplitOptions.None);
                        if (cols.Length > 2) //skip blank rows
                        {
                            DateTime dt;

                            if (DateTime.TryParseExact(cols[dateCol] + " " + cols[timeCol], allowedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                            {
                                foreach (SitePollingConfigDetailType _item in _pollConfigDtl)
                                {
                                    string val = cols[(_item.COL ?? 1) - 1].ToString().Trim();
                                    ImportResponse xxx = db_Air.InsertUpdateT_QREST_DATA_HOURLY(_item.MONITOR_IDX, _pollConfig.TIME_POLL_TYPE == "L" ? dt : (DateTime?)null, _pollConfig.TIME_POLL_TYPE == "U" ? dt : (DateTime?)null, tzOffset, double.TryParse(val, out _) ? val : null, _item.COLLECT_UNIT_CODE, true, double.TryParse(val, out _) ? null : val, importID);
                                    if (xxx.SuccInd)
                                        model.ImportSuccCount += 1;
                                    else
                                        model.error_data.Add(xxx);
                                }
                            }
                            else
                                ModelState.AddModelError("IMPORT_BLOCK", "Date and/or time format cannot be read.");
                        }
                    }
                }


                //**************************************************************************************
                //    H1                hourly (1 parameter with hours arranged as columns)
                //**************************************************************************************
                else if (model.selImportType == "H1")
                {
                    //insert/upadte POLL_CONFIG
                    Guid? h1SuccID = db_Air.InsertUpdatetT_QREST_SITE_POLL_CONFIG(null, model.selSite, "H1", null, "H1", null, null, null, null, null, null, null, null, null, model.selTimeZone, false, UserIDX, null, model.selTimeType, false);
                    if (h1SuccID != null)
                    {
                        //if update succeeded, continue
                        int tzOffset = model.selTimeZone.ConvertOrDefault<int>();

                        T_QREST_MONITORS _monitor = db_Air.GetT_QREST_MONITORS_ByID_Simple(model.selMonitor ?? Guid.Empty);
                        if (_monitor != null && _monitor.COLLECT_UNIT_CODE != null)
                        {
                            foreach (string row in allRows)
                            {
                                //split row's columns into string array
                                string[] cols = row.Split(new char[] { ',' }, StringSplitOptions.None);
                                if (cols.Length > 20 && cols[0] != "Date") //skip blank rows
                                {
                                    try
                                    {
                                        DateTime dt = DateTime.ParseExact(cols[0], "MM/dd/yyyy", CultureInfo.InvariantCulture);

                                        for (int i = 0; i <= 23; i++)
                                        {
                                            ImportResponse xxx = db_Air.InsertUpdateT_QREST_DATA_HOURLY(_monitor.MONITOR_IDX, model.selTimeType == "L" ? dt : (DateTime?)null, model.selTimeType == "U" ? dt : (DateTime?)null, tzOffset, double.TryParse(cols[i + 1], out _) ? cols[i + 1] : null, _monitor.COLLECT_UNIT_CODE, true, double.TryParse(cols[i + 1], out _) ? null : cols[i + 1], importID);
                                            if (xxx.SuccInd)
                                                model.ImportSuccCount += 1;
                                            else
                                                model.error_data.Add(xxx);

                                            dt = dt.AddHours(1);
                                        }
                                    }
                                    catch
                                    {
                                        ModelState.AddModelError("IMPORT_BLOCK", "Date is not in the correct format (MM/DD/YYYY).");
                                    }
                                }
                            }                           
                        }
                        else
                            ModelState.AddModelError("selMonitor", "No collection unit defined for this monitor. No data imported.");
                    }
          
                }
                
            }

            //wrap up
            if (model.ImportSuccCount == 0)
                ModelState.AddModelError("IMPORT_BLOCK", "No data in expected format found." + (model.selImportType == "H1" ? " Data needs to be datetime followed by 24 hourly columns, comma separated." : ""));
            else
            {
                //if import was successful, then insert importIDX
                model.IMPORT_IDX = importID;
                db_Air.InsertUpdateT_QREST_DATA_IMPORTS(importID, model.selOrgID, model.selSite, "", "SUCCESS", UserIDX, System.DateTime.Now);
            }

            //reinitialize model
            model.ddl_ImportType = ddlHelpers.get_ddl_import_type();
            model.ddl_Time = ddlHelpers.get_ddl_time_type();
            model.ddl_TimeZone = ddlHelpers.get_ddl_time_zone();
            model.ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true);
            model.ddl_Sites = model.selOrgID == null ? new List<SelectListItem>() : ddlHelpers.get_ddl_my_sites(model.selOrgID, UserIDX);
            model.ddl_Monitors = model.selSite == null ? new List<SelectListItem>() : ddlHelpers.get_monitors_by_site(model.selSite, true, false);
            model.ddl_PollConfig = model.selSite == null ? new List<SelectListItem>() : ddlHelpers.get_ddl_import_templates(model.selSite);
            model.ddl_Calc = ddlHelpers.get_ddl_yes_no();
            model.IMPORT_BLOCK = "";

            return View(model);
        }



        public ActionResult ImportConfig(Guid? id, Guid? siteid)
        {
            //********* VALIDATION *******************************
            if (siteid == null && id == null)
            {
                TempData["Error"] = "No record found.";
                return RedirectToAction("ManualImport");
            }


            var model = new vmDataImportConfig { 
            };

            //*********INSERT CASE *******************************
            if (id == null && siteid != null)
            {
                T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(siteid ?? Guid.Empty);
                if (_site != null)
                {
                    model.editPOLL_CONFIG_IDX = Guid.NewGuid();
                    model.SITE_IDX = _site.SITE_IDX;
                }
                else
                {
                    //reject if user supplied SiteIDX but doesn't exist
                    TempData["Error"] = "Site not found.";
                    return RedirectToAction("SiteList", "Site");
                }

            }
            //***********EDIT CASE **********************************
            else if (id != null && siteid == null)
            {
                T_QREST_SITE_POLL_CONFIG _config = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(id.ConvertOrDefault<Guid>());
                if (_config != null)
                {
                    model.SITE_IDX = _config.SITE_IDX;
                    model.editCONFIG_NAME = _config.CONFIG_NAME;
                    model.editPOLL_CONFIG_IDX = _config.POLL_CONFIG_IDX;
                    model.editDELIMITER = _config.DELIMITER;
                    model.editDATE_COL = _config.DATE_COL;
                    model.editDATE_FORMAT = _config.DATE_FORMAT;
                    model.editTIME_COL = _config.TIME_COL;
                    model.editTIME_FORMAT = _config.TIME_FORMAT;
                    model.editLOCAL_TIMEZONE = _config.LOCAL_TIMEZONE;
                    model.editTIME_POLL_TYPE = _config.TIME_POLL_TYPE;
                }
                else
                {
                    //reject if user supplied ConfigIDX but doesn't exist
                    TempData["Error"] = "Configuration not found.";
                    return RedirectToAction("SiteList", "Site");
                }

            }



            //************* FINAL COMBINED STUFF ************************
            //reject if user doesn't have access to org
            RedirectToRouteResult r = CanAccessThisSite(User.Identity.GetUserId(), model.SITE_IDX.GetValueOrDefault(), false);
            if (r != null) return r;

            model.ddl_Monitors = ddlHelpers.get_monitors_by_site(model.SITE_IDX.GetValueOrDefault(), false, true);

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ImportConfig(vmDataImportConfig model)
        {
            string UserIDX = User.Identity.GetUserId();

            //*************** VALIDATION BEGIN *********************************

            //*************** VALIDATION END  *********************************

            if (ModelState.IsValid)
            {
                //reject if user doesn't have access to org
                RedirectToRouteResult r = CanAccessThisSite(UserIDX, model.SITE_IDX.GetValueOrDefault(), true);
                if (r != null) return r;

                Guid? SuccID = db_Air.InsertUpdatetT_QREST_SITE_POLL_CONFIG(model.editPOLL_CONFIG_IDX, model.SITE_IDX, model.editCONFIG_NAME, null, "NONE",
                    null, null, null, null, model.editDELIMITER, model.editDATE_COL, model.editDATE_FORMAT, model.editTIME_COL, model.editTIME_FORMAT, 
                    model.editLOCAL_TIMEZONE, false, UserIDX, null, model.editTIME_POLL_TYPE, false);

                if (SuccID != null)
                {
                    TempData["Success"] = "Record updated";
                    return RedirectToAction("ImportConfig", new { id = SuccID });
                }
                else
                    TempData["Error"] = "Error updating record.";
            }

            //reinitialize model
            model.ddl_Monitors = ddlHelpers.get_monitors_by_site(model.SITE_IDX.ConvertOrDefault<Guid>(), false, true);
            return View(model);
        }


        public ActionResult ImportList(string selOrgID)
        {
            string UserIDX = User.Identity.GetUserId();
            var model = new vmDataImportList
            {
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, false),
            };

            //autopopulate if only rights to 1 org that has made aqs submission
            if (model.ddl_Organization != null && model.ddl_Organization.ToList().Count == 1)
                model.selOrgID = model.ddl_Organization.First().Value;

            if (selOrgID != null)
                model.selOrgID = selOrgID;

            model.T_QREST_DATA_IMPORTS  = db_Air.GetT_QREST_DATA_IMPORTS_byORG_ID(model.selOrgID);

            return View(model);
        }


        [HttpPost]
        public JsonResult ImportDelete(Guid? id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int SuccID = db_Air.DeleteT_QREST_DATA_HOURLY_ByImportIDX(id.GetValueOrDefault());
                if (SuccID == 1)
                {
                    db_Air.DeleteT_QREST_DATA_IMPORTS(id.GetValueOrDefault());
                    if (SuccID == 1)
                    {
                        TempData["Success"] = "Deleted";
                        return Json("Success");
                    }
                }
                return Json("Unable to delete Import record.");
            }
        }


        public FileResult ImportFileDownload(Guid? id)
        {
            try
            {
                DataTable _dt = DataTableGen.GetHourlyDataByImportIDX(id.GetValueOrDefault());
                DataSet dsExport = DataTableGen.DataSetFromDataTables(new List<DataTable> { _dt });
                if (dsExport.Tables.Count > 0)
                {
                    MemoryStream ms = ExcelGen.GenExcelFromDataSet(dsExport);
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=QRESTExport.xlsx");
                    ms.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();

                    return null;
                }
                else
                {
                    TempData["Error"] = "No data found to export";
                }

            }
            catch
            { }

            return null;
        }


        public ActionResult DownloadTemplate(Guid? id)
        {
            T_QREST_SITE_POLL_CONFIG _pol = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(id ?? Guid.Empty);
            if (id != null)
            {
                List<PollConfigDtlDisplay> _poldtls = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID(id ?? Guid.Empty);
                if (_poldtls != null && _poldtls.Count > 0)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        int maxCol = new[] { 1, _pol.DATE_COL ?? 1, _pol.TIME_COL ?? 1 }.Max();

                        var ws = wb.Worksheets.Add("Inserting Data");
                        wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell(1, _pol.DATE_COL ?? 1).Value = "Date (" + _pol.DATE_FORMAT + ")";
                        ws.Cell(1, _pol.TIME_COL ?? 1).Value = "Time (" + _pol.TIME_FORMAT + ")";

                        foreach (PollConfigDtlDisplay _poldtl in _poldtls)
                        {
                            ws.Cell(1, _poldtl.COL ?? 1).Value = _poldtl.PAR_NAME;
                            maxCol = (_poldtl.COL > maxCol ? _poldtl.COL ?? 1 : maxCol);
                        }

                        //apply formatting
                        var range5 = ws.Range(1, 1, 1000, maxCol);
                        range5.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        range5.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        ws.Columns().AdjustToContents();

                        MemoryStream ms = new MemoryStream();
                        wb.SaveAs(ms);
                        ms.Position = 0;
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=QRESTImportTemplate.xlsx");
                        ms.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();

                        return null;
                    }

                }
                else
                {
                    TempData["Error"] = "No parameters defined for configuration";
                    return RedirectToAction("ManualImport");
                }
            }
            else
            {
                TempData["Error"] = "Unable to find configuration";
                return RedirectToAction("ManualImport");
            }
        }

        [HttpPost]
        public JsonResult SitePollConfigDtlDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Json("No record selected to delete");
            else
            {
                Guid idg = new Guid(id);

                //reject if user doesn't have access to org
                RedirectToRouteResult r = CanAccessThisOrg(User.Identity.GetUserId(), db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_org_ByID(idg), true);
                if (r != null)
                    return Json("Access Denied");

                int SuccID = db_Air.DeleteT_QREST_SITE_POLL_CONFIG_DTL(idg);
                if (SuccID == 1)
                    return Json("Success");
                else
                    return Json("Unable to find column mapping to delete.");
            }
        }

        #endregion



        #region QC CHECK

        public ActionResult QCList(string selOrgID)
        {
            if (string.IsNullOrEmpty(selOrgID))
                selOrgID = null;

            var model = new vmDataQCList
            {
                selOrgID = selOrgID,
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(User.Identity.GetUserId(), true)
            };

            if (model.ddl_Organization.Count() == 1)
                model.selOrgID = model.ddl_Organization.First().Value;

            return View(model);
        }


        [HttpPost]
        public ActionResult QCListData()
        {
            string UserIDX = User.Identity.GetUserId();

            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //data filters
            string selOrgID = Request.Form.GetValues("p_org")?.FirstOrDefault();
            //Guid? selSiteID = Request.Form.GetValues("p_site")?.FirstOrDefault().ConvertOrDefault<Guid?>();
            //Guid? selMonID = Request.Form.GetValues("p_mon")?.FirstOrDefault().ConvertOrDefault<Guid?>();

            var data = db_Air.GetT_QREST_QC_ASSESSMENT_Search(UserIDX, selOrgID, null, null, pageSize, start, orderCol, orderDir);
            var recordsTotal = db_Air.GetT_QREST_QC_ASSESSMENT_Search_Count(selOrgID);

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpPost]
        public JsonResult QCDelete(Guid? id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int SuccID = db_Air.DeleteT_QREST_QC_ASSESSMENT(id.GetValueOrDefault());
                if (SuccID == 1)
                    return Json("Success");
                else
                    return Json("Unable to delete QC record.");
            }
        }


        public ActionResult QCEntry(Guid? id)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataQCEntry {
                ddl_Assess_Type = ddlHelpers.get_ddl_ref_assess_type(),
                ddl_Monitor = ddlHelpers.get_ddl_my_monitors(null, UserIDX, true),
                ddl_AQS_Null = ddlHelpers.get_ddl_ref_qualifier("NULL")
            };

            //assessment
            var x = db_Air.GetT_QREST_QC_ASSESSMENT_ByID(id);
            if (x != null)
            {
                model.QC_ASSESS_IDX = x.QC_ASSESS_IDX;
                model.MONITOR_IDX = x.MONITOR_IDX;
                model.ASSESSMENT_DT = x.ASSESSMENT_DT;
                model.ASSESSMENT_TM = x.ASSESSMENT_TM;
                model.ASSESSMENT_TYPE = x.ASSESSMENT_TYPE;
                model.UNIT_CODE = x.UNIT_CODE;
                model.ASSESSMENT_NUM = x.ASSESSMENT_NUM;
                model.ASSESSED_BY = x.ASSESSED_BY;

                //populate org id
                T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByMonitorID(model.MONITOR_IDX.GetValueOrDefault());
                if (_site != null)
                    model.ORG_ID = _site.ORG_ID;

                //assessment dtl
                model.AssessmentDetails = db_Air.GetT_QREST_QC_ASSESSMENT_DTL_ByAssessID(model.QC_ASSESS_IDX);

                //if Annual PE, select distinct audit levels
                if (model.ASSESSMENT_TYPE == "Annual PE")
                {
                    var uniqueAuditLevels = model.AssessmentDetails.Where(f => f.audit_level_int != null).Where(g => g.audit_level_int > 0).Select(p => p.audit_level_int).Distinct().Count();
                    model.AuditLevelDistinctCount = uniqueAuditLevels;
                }
            }


            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult QCEntry(vmDataQCEntry model) {
            string UserIDX = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                //save generic QC assessment 
                Guid? AssessIDX = db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT(model.QC_ASSESS_IDX, model.MONITOR_IDX, model.ASSESSMENT_DT, model.ASSESSMENT_TYPE, model.UNIT_CODE,
                    model.ASSESSMENT_NUM, model.ASSESSED_BY, UserIDX, model.ASSESSMENT_TM);

                //save QC details
                if (AssessIDX != null)
                {
                    model.QC_ASSESS_IDX = AssessIDX;

                    //initialize first rows for the QC assessment based on type
                    if (model.AssessmentDetails == null)
                    {
                        if (model.ASSESSMENT_TYPE == "1-Point QC")
                        {
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, null, null, null, UserIDX);
                        }
                        else if (model.ASSESSMENT_TYPE == "Annual PE")
                        {
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, null, null, null, UserIDX);
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, null, null, null, UserIDX);
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, null, null, null, UserIDX);
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, null, null, null, UserIDX);
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, null, null, null, UserIDX);
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, null, null, null, UserIDX);
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, null, null, null, UserIDX);
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, null, null, null, UserIDX);
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, null, null, null, UserIDX);
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, null, null, null, UserIDX);
                        }
                        else if (model.ASSESSMENT_TYPE == "Zero Span")
                        {
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, 0, null, "Zero Check", UserIDX);
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, null, null, "Span Check", UserIDX);
                        }

                    }
                    else
                    {
                        //updating assessment details
                        foreach (QC_ASSESSMENT_DTLDisplay _dtl in model.AssessmentDetails)
                        {
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(_dtl.QC_ASSESS_DTL_IDX, null, _dtl.MON_CONCENTRATION, _dtl.ASSESS_KNOWN_CONCENTRATION, null, _dtl.COMMENTS, UserIDX);
                        }

                    }


                    if (AssessIDX != null)
                        TempData["Success"] = "Record updated";
                    else
                        TempData["Error"] = "Error updating record.";
                }
                return RedirectToAction("QCEntry", new { id = model.QC_ASSESS_IDX });
            }
            ////reinitialize
            ////assessment dtl
            model.AssessmentDetails = db_Air.GetT_QREST_QC_ASSESSMENT_DTL_ByAssessID(model.QC_ASSESS_IDX);
            model.ddl_Monitor = ddlHelpers.get_ddl_my_monitors(null, UserIDX, true);
            model.ddl_Assess_Type = ddlHelpers.get_ddl_ref_assess_type();
            model.ddl_AQS_Null = ddlHelpers.get_ddl_ref_qualifier("NULL");
            return View(model);

        }



        [HttpPost]
        public JsonResult QCDtlDelete(Guid? id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int SuccID = db_Air.DeleteT_QREST_QC_ASSESSMENT_DTL(id.GetValueOrDefault());
                if (SuccID == 1)
                    return Json("Success");
                else
                    return Json("Unable to delete QC record.");
            }
        }

        #endregion



        #region RAW DATA


        public ActionResult Raw(string org, string mon, string typ, string timtyp)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataRaw
            {
                selOrgID = org,
                selMon = mon,
                selType = typ,
                selTimeType = timtyp,
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true),
                ddl_Monitor = new List<SelectListItem>()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Raw(vmDataRaw model)
        {
            string UserIDX = User.Identity.GetUserId();

            string[] d = model.selDate.Replace(" - ", "z").Split('z');
            if (d.Length == 2)
            {
                DateTime? d1 = d[0].ConvertOrDefault<DateTime?>();
                DateTime? d2 = (d.Length > 1) ? d[1].ConvertOrDefault<DateTime?>() : null;

                if (model.selType == "H")
                    model.RawData = db_Air.GetT_QREST_DATA_FIVE_MIN(model.selOrgID, null, model.selMon.ConvertOrDefault<Guid?>(), d1, d2, 25000, 0, 3, "asc", model.selTimeType);
                else if (model.selType == "1")
                    model.RawData = db_Air.GetT_QREST_DATA_HOURLY(model.selOrgID, model.selMon.ConvertOrDefault<Guid?>(), d1, d2, 25000, 0, 3, "asc", model.selTimeType);
            }

            //reinitialize
            model.ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true);
            if (model.selOrgID != null)
                model.ddl_Monitor = ddlHelpers.get_monitors_sampled_by_org(model.selOrgID);


            return View(model);
        }



        [HttpPost]
        public JsonResult RawDataChart()
        {
            //filters
            string selOrg = Request.Form.GetValues("selOrg")?.FirstOrDefault();
            string selType = Request.Form.GetValues("selType")?.FirstOrDefault();
            string selDate = Request.Form.GetValues("selDate")?.FirstOrDefault().Replace(" - ", "z");
            string selTimeType = Request.Form.GetValues("selTimeType")?.FirstOrDefault();
            Guid? selMon = Request.Form.GetValues("selMon")?.FirstOrDefault().ConvertOrDefault<Guid?>();

            if (selDate != null)
            {
                string[] d = selDate.Split('z');
                DateTime? d1 = d[0].ConvertOrDefault<DateTime?>();
                DateTime? d2 = (d.Length > 1) ? d[1].ConvertOrDefault<DateTime?>() : null;

                if (selType == "F")
                {
                    var data = db_Air.GetT_QREST_DATA_FIVE_MIN(selOrg, null, selMon, d1, d2, 25000, 0, 3, "asc", selTimeType);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = db_Air.GetT_QREST_DATA_HOURLY(selOrg, selMon, d1, d2, 25000, 0, 3, "asc", selTimeType);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json("Chart data error");

        }

        #endregion



        #region DATA REVIEW

        public ActionResult DataReviewSummary(Guid? id, int? month, int? year)
        {
            string UserIDX = User.Identity.GetUserId();
            var model = new vmDataReviewSummary {
                ddl_Sites = ddlHelpers.get_ddl_my_sites(null, UserIDX),
                selSite = id,
                selMonth = month ?? System.DateTime.Today.Month,
                selYear = year ?? System.DateTime.Today.Year
            };

            if (id != null && month != null && year != null)
            {
                model.selsDt = new DateTime(model.selYear, model.selMonth, 1);
                model.seleDt = new DateTime(model.selYear, model.selMonth, 1).AddMonths(1).AddHours(-1);
                model.Results = db_Air.SP_AQS_REVIEW_STATUS(id.GetValueOrDefault(), model.selsDt.GetValueOrDefault(), model.seleDt);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult DataReviewSummary(vmHomeReportDaily model)
        {
            return RedirectToAction("DataReviewSummary", new { id = model.selSite, month = model.selMonth, year = model.selYear });
        }


        public ActionResult DataReview(Guid? monid, DateTime? sdt, DateTime? edt)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataReview
            {
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true),
                ddl_Monitor = new List<SelectListItem>(),
                ddl_Duration = ddlHelpers.get_ddl_logger_duration(),
                selMon = monid.ToString(),
                selDtStart = sdt,
                selDtEnd = edt
            };

            //get org and site from supplied mon
            if (string.IsNullOrEmpty(model.selMon) == false)
            {
                SiteMonitorDisplayType _m = db_Air.GetT_QREST_MONITORS_ByID(new Guid(model.selMon));
                if (_m != null)
                    model.selOrgID = _m.ORG_ID;
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult DataReview(vmDataReview model)
        {
            if (model.selMon != null)
            {                
                return RedirectToAction("DataReview2", new { monid = model.selMon, sdt = model.selDtStart, edt = model.selDtEnd, dur = model.selDuration, supp1 = model.selMonSupp?.ElementAtOrDefault(0), supp2 = model.selMonSupp?.ElementAtOrDefault(1), supp3 = model.selMonSupp?.ElementAtOrDefault(2) });
            }
            else
            {
                TempData["Error"] = "Please select a monitor to validate";
                return RedirectToAction("DataReview");
            }
        }

        
        [HttpGet]
        public ActionResult DataReview2(Guid? monid, DateTime? sdt, DateTime? edt, string dur, Guid? supp1)
        {
            if (monid == null || sdt == null || edt == null || dur == null)
                return RedirectToAction("DataReviewSummary");

            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataReview2
            {
                selDtStart = sdt.GetValueOrDefault(),
                selDtEnd = edt.GetValueOrDefault(),
                selDuration = dur,
                selMon = db_Air.GetT_QREST_MONITORS_ByID(monid.GetValueOrDefault())
            };

            //security check
            if (db_Account.CanAccessThisOrg(UserIDX, model.selMon.ORG_ID, true) == false)
            {
                TempData["Error"] = "Access Denied.";
                return RedirectToAction("SiteList", "Site");
            }

            //unit dropdown
            model.ddl_ParUnits = ddlHelpers.get_ddl_ref_units(model.selMon.PAR_CODE);

            //get security access rights
            model.secLvl1Ind = db_Account.IsOrgLvl1(UserIDX, model.selMon.ORG_ID);
            model.secLvl2Ind = db_Account.IsOrgLvl2(UserIDX, model.selMon.ORG_ID);

            //get raw data
            if (dur == "H")
                model.RawData = db_Air.GetT_QREST_DATA_FIVE_MIN(null, null, monid, sdt, edt, 25000, 0, 3, "asc", "L");
            else if (dur == "1")
                model.RawData = db_Air.GetT_QREST_DATA_HOURLY_ManVal(monid.GetValueOrDefault(), model.selDtStart, model.selDtEnd);

            //supp parameters
            if (supp1 != null)
            {
                if (dur == "H")
                    model.SuppData1 = db_Air.GetT_QREST_DATA_FIVE_MIN(null, null, supp1, sdt, edt, 25000, 0, 3, "asc", "L");
                else if (dur == "1")
                    model.SuppData1 = db_Air.GetT_QREST_DATA_HOURLY_ManVal(supp1.GetValueOrDefault(), model.selDtStart, model.selDtEnd);

            }
            return View(model);
        }


        [HttpPost]
        public ActionResult DataReview2(vmDataReview2 model)
        {
            if (ModelState.IsValid)
            {
                int editCount = 0;

                //lvl1
                bool? lvl1ind = null;
                if (model.editLvl1 == "Y") lvl1ind = true;
                else if (model.editLvl1 == "N") lvl1ind = false;

                //lvl2
                bool? lvl2ind = null;
                if (model.editLvl2 == "Y") lvl2ind = true;
                else if (model.editLvl2 == "N") lvl2ind = false;

                string UserIDX = User.Identity.GetUserId();

                if (model.editRawDataIDX != null)
                {
                    foreach (var item in model.editRawDataIDX)
                    {
                        editCount++;

                        Guid? SuccID = db_Air.UpdateT_QREST_DATA_HOURLY(item, model.editNullQual, lvl1ind, lvl2ind, UserIDX, model.editUnitCode, model.editNotes, (model.editValueBlank == true ? "-999" : model.editValue), (model.editFlagBlank == true ? "-999" : model.editFlag), model.editQual);
                        if (SuccID == null)
                            TempData["Error"] = "Error updating record";
                        else
                            db_Air.InsertUpdatetT_QREST_DATA_HOURLY_LOG(null, SuccID, model.editNotes, UserIDX);
                    }
                }
                else
                    TempData["Error"] = "You must select a row to edit.";

                return RedirectToAction("DataReview2", new { monid = model.selMon.T_QREST_MONITORS.MONITOR_IDX, sdt = model.selDtStart, edt = model.selDtEnd, dur = model.selDuration });

            }
            else
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                TempData["Error"] = "Model Error";
                return RedirectToAction("DataReview2", new { monid = model.selMon.T_QREST_MONITORS.MONITOR_IDX, sdt = model.selDtStart, edt = model.selDtEnd, dur = model.selDuration });
            }
        }


        [HttpPost]
        public JsonResult DataReviewData()
        {
            //filters
            string selType = Request.Form.GetValues("selType")?.FirstOrDefault();
            DateTime? selDateStart = Request.Form.GetValues("selDateStart")?.FirstOrDefault().ConvertOrDefault<DateTime?>();
            DateTime? selDateEnd = Request.Form.GetValues("selDateEnd")?.FirstOrDefault().ConvertOrDefault<DateTime?>();
            Guid? selMon = Request.Form.GetValues("selMon")?.FirstOrDefault().ConvertOrDefault<Guid?>();

            if (selDateStart != null && selDateEnd != null && selMon != null)
            {
                if (selType == "F")
                {
                    var data = db_Air.GetT_QREST_DATA_FIVE_MIN(null, null, selMon, selDateStart, selDateEnd, 25000, 0, 3, "asc", "L");
                    var data1 = JsonConvert.SerializeObject(data);
                    return Json(new { data = data });
                }
                else
                {
                    var data = db_Air.GetT_QREST_DATA_HOURLY(null, selMon, selDateStart, selDateEnd, 25000, 0, 3, "asc");
                    var data1 = JsonConvert.SerializeObject(data);
                    return Json(new { data = data });
                }
            }
            else
                return Json("Chart data error");

        }


        [HttpPost]
        public ActionResult HourlyLogData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            //int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            //int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            //int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            //string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            //string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //date filters
            Guid? id = Request.Form.GetValues("id")?.FirstOrDefault().ConvertOrDefault<Guid?>();

            var data = db_Air.GetT_QREST_DATA_HOURLY_LOG_ByHour(id.GetValueOrDefault());
            var recordsTotal = data.Count();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        public FileResult HourlyLogFileDownload(Guid? id)
        {
            try
            {
                DataTable _dt = DataTableGen.GetHourlyLogByHourlyIDX(id.GetValueOrDefault());
                DataSet dsExport = DataTableGen.DataSetFromDataTables(new List<DataTable> { _dt });
                if (dsExport.Tables.Count > 0)
                {
                    MemoryStream ms = ExcelGen.GenExcelFromDataSet(dsExport);
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=HourlyLog.xlsx");
                    ms.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                else
                    TempData["Error"] = "No data found to export";
            }
            catch
            { }

            return null;
        }

        public ActionResult DataDocs(Guid? id, Guid? monid, DateTime? sDt, DateTime? eDt)
        {
            if (id == null || sDt == null)
                return RedirectToAction("DataReviewSummary");

            var model = new vmDataDocs {
                selSite = id,
                selMon = monid,
                startDate = sDt.GetValueOrDefault(),
                endDate = eDt.GetValueOrDefault(),
                addStartDt = sDt.GetValueOrDefault(),
                addEndDt = eDt.GetValueOrDefault(),
                SiteDocs = db_Air.GetT_QREST_ASSESS_DOCS_BySite(id.GetValueOrDefault(), sDt, eDt),
                MonDocs = db_Air.GetT_QREST_ASSESS_DOCS_ByMonitor(monid.GetValueOrDefault(), sDt, eDt)
            };

            SiteMonitorDisplayType _mon = db_Air.GetT_QREST_MONITORS_ByID(monid.GetValueOrDefault());
            if (_mon != null)
            {
                model.PAR_CODE = _mon.PAR_CODE;
                model.PAR_NAME = _mon.PAR_NAME;
                model.SITE_ID = _mon.SITE_ID;
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult DataDocsAdd(vmDataDocs model)
        {
            string UserIDX = User.Identity.GetUserId();

            if (model.SiteMonInd == "E")
            {
                Guid? SuccID = db_Air.InsertUpdatetT_QREST_ASSESS_DOCS(model.editASSESS_DOC_IDX, null, null, model.addStartDt, model.addEndDt, null, null, null, null, null, model.fileDescription, null, UserIDX);

                if (SuccID != null)
                    TempData["Success"] = "Update successful";
                else
                    TempData["Error"] = "Update failed";
            }
            else
            {
                if (model.fileUpload?.ContentLength > 0)
                {
                    Stream stream = model.fileUpload.InputStream;
                    byte[] _file = UtilsText.ConvertStreamToByteArray(stream);

                    Guid? SuccID = db_Air.InsertUpdatetT_QREST_ASSESS_DOCS(null, model.selSite, (model.SiteMonInd == "S" ? null : model.selMon), model.addStartDt, model.addEndDt, _file, model.fileUpload.FileName, "",
                        model.fileUpload.ContentType, model.fileUpload.ContentLength, model.fileDescription, "", UserIDX);

                    if (SuccID != null)
                        TempData["Success"] = "File upload successful";
                    else
                        TempData["Error"] = "File upload failed";
                }
                else
                    TempData["Error"] = "You must select a file to upload";

            }

            return RedirectToAction("DataDocs", new { id= model.selSite, monid = model.selMon, sDt = model.startDate.ToString("MM/dd/yyyy"), eDt = model.endDate.ToString("MM/dd/yyyy") });    
        }


        [HttpPost]
        public JsonResult DataDocsDelete(Guid? id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int SuccID = db_Air.DeleteT_QREST_ASSESS_DOCS(id.GetValueOrDefault());
                if (SuccID == 1)
                    return Json("Success");
                else if (SuccID == -1)
                    return Json("Cannot delete Organization that still has site records. Delete sites first.");
                else
                    return Json("Unable to find organization to delete.");
            }
        }


        public FileResult DataDocsDownload(Guid? id)
        {
            try
            {
                string UserIDX = User.Identity.GetUserId();

                T_QREST_ASSESS_DOCS _doc = db_Air.GetT_QREST_ASSESS_DOCS_ByID(id.GetValueOrDefault());
                if (_doc != null)
                {
                    //reject if user doesn't have access to site
                    RedirectToRouteResult r = CanAccessThisSite(UserIDX, _doc.SITE_IDX, true);
                    if (r != null) return null;

                    return File(_doc.DOC_CONTENT, System.Net.Mime.MediaTypeNames.Application.Octet, _doc.DOC_NAME);
                }
            }
            catch
            { }

            return null;
        }


        #endregion



        #region AQS


        public ActionResult AQSList(string selOrgID)
        {
            string UserIDX = User.Identity.GetUserId();
            var model = new vmDataAQSList {
                ddl_Organization = ddlHelpers.get_ddl_my_organizations_aqs_submission(UserIDX),
            };

            //autopopulate if only rights to 1 org that has made aqs submission
            if (model.ddl_Organization != null && model.ddl_Organization.ToList().Count == 1)
                model.selOrgID = model.ddl_Organization.First().Value;

            if (selOrgID != null)
                model.selOrgID = selOrgID;

            model.T_QREST_AQS = db_Air.GetT_QREST_AQS_byORG_ID(model.selOrgID);

            return View(model);
        }

        [HttpPost]
        public ActionResult AQSListEdit(vmDataAQSList model)
        {
            Guid? succID = db_Air.InsertUpdateT_QREST_AQS(model.editID, null, null, model.editSUBMISSION_NAME, null, null, null, null, model.editCOMMENT, null, null, null, null, null);
            if (succID != null)
            {
                TempData["Success"] = "Update successful.";
                return RedirectToAction("AQSList");
            }
            else {
                TempData["Error"] = "Error updating record.";
                return RedirectToAction("AQSList");
            }
        }

        public FileResult AQSFileDownload(Guid? id)
        {
            try
            {
                T_QREST_AQS _doc = db_Air.GetT_QREST_AQS_by_ID(id.GetValueOrDefault());
                if (_doc != null)
                {
                    byte[] _file = Encoding.UTF8.GetBytes(_doc.AQS_CONTENT_XML);
                    return File(_file, System.Net.Mime.MediaTypeNames.Application.Octet, _doc.AQS_SUBMISSION_NAME);
                }
                else
                    TempData["Error"] = "No file found";
            }
            catch
            { }

            return null;
        }

        public FileResult AQSFileDownloadHeader(Guid? id)
        {
            try
            {
                T_QREST_AQS _doc = db_Air.GetT_QREST_AQS_by_ID(id.GetValueOrDefault());
                if (_doc != null)
                    return File(_doc.AQS_CONTENT, System.Net.Mime.MediaTypeNames.Application.Octet, "HeaderFile.zip");
                else
                    TempData["Error"] = "No file found";
            }
            catch
            { }

            return null;
        }

        public FileResult AQSResponseDownload(Guid? id)
        {
            try
            {
                T_QREST_AQS _doc = db_Air.GetT_QREST_AQS_by_ID(id.GetValueOrDefault());
                if (_doc != null)
                    return File(_doc.DOWNLOAD_FILE, System.Net.Mime.MediaTypeNames.Application.Octet, "Response.zip");
                else
                    TempData["Error"] = "No file found";
            }
            catch
            { }

            return null;
        }


        public ActionResult AQSGen(string typ)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataAQSGen {
                ddl_Sites = ddlHelpers.get_ddl_my_sites_sampled(null, UserIDX),
                selAQSTransType = typ ?? "RD",
                selDtStart = new DateTime(System.DateTime.Today.Year, System.DateTime.Today.Month, 1).AddMonths(-1),
                selDtEnd = new DateTime(System.DateTime.Today.Year, System.DateTime.Today.Month, 1).AddHours(-1),
                passValidation = false
            };
            return View(model);
        }


        [HttpPost]
        public ActionResult AQSGen(vmDataAQSGen model)
        {
            model.Results = db_Air.SP_AQS_REVIEW_STATUS(model.selSite ?? Guid.Empty, model.selDtStart.GetValueOrDefault(), model.selDtEnd);
            model.Results = model.Results.Where(o => model.selMons.Contains(o.MONITOR_IDX)).ToList();

            model.passValidation = true;
            bool send = false;
            foreach (SP_AQS_REVIEW_STATUS_Result _result in model.Results)
            {
                if (_result.hrs != _result.lvl2_val_ind)
                    model.passValidation = false;
            }

            //repopulate model before returning
            string UserIDX = User.Identity.GetUserId();
            model.ddl_Sites = ddlHelpers.get_ddl_my_sites_sampled(null, UserIDX);

            return View(model);
        }


        public ActionResult AQSGenQA(string typ, Guid? qid)
        {
            string UserIDX = User.Identity.GetUserId();

            T_QREST_QC_ASSESSMENT _q = db_Air.GetT_QREST_QC_ASSESSMENT_ByID(qid);
            if (_q != null)
            {
                T_QREST_SITES _s = db_Air.GetT_QREST_SITES_ByMonitorID(_q.MONITOR_IDX);
                if (_s != null)
                {
                    var model = new vmDataAQSGen
                    {
                        selAQSTransType = typ ?? "QA",
                        selSite = _s.SITE_IDX,
                        selQid = qid,
                        selOrgID = _s.ORG_ID,
                        passValidation = true
                    };
                    return View(model);
                }
            }

            TempData["Error"] = "Select QC Record to send.";
            return RedirectToAction("QCList");
        }



        public ActionResult AQSAcct(string id, string returnUrl, string returnid)
        {
            ViewBag.ReturnUrl = returnUrl ?? "AQSGen";
            ViewBag.ReturnID = returnid;

            var model = new vmDataAQSAcct
            {
                selOrgID = id,
                CDXUsername = db_Ref.GetT_QREST_APP_SETTING("CDX_GLOBAL_USER"),
                AQSUser = "",
                AQSScreeningGroup = ""
            };

            var _org = db_Ref.GetT_QREST_ORGANIZATION_ByID(id);
            if (_org != null)
            {
                model.AQSUser = _org.AQS_AQS_UID;
                model.AQSScreeningGroup = _org.AQS_AQS_SCREENING_GRP;
                if (!string.IsNullOrEmpty(_org.AQS_NAAS_UID))
                    model.CDXUsername = _org.AQS_NAAS_UID;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult AQSAcct(vmDataAQSAcct model)
        {
            string UserIDX = User.Identity.GetUserId();

            int SuccID = db_Ref.InsertUpdatetT_QREST_ORGANIZATION(model.selOrgID, null, null, null, model.CDXUsername, model.CDXPwd, null, null, true, UserIDX, model.AQSUser, model.AQSScreeningGroup);

            if (SuccID == 0)
                TempData["Error"] = "Unable to update record";
            else
                TempData["Success"] = "Record updated";

            return RedirectToAction("AQSAcct", new { model.selOrgID });
        }


        [HttpPost]
        public ActionResult AQSSubmit(vmDataAQSGen model)
        {
            string UserIDX = User.Identity.GetUserId();

            T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(model.selSite.GetValueOrDefault());
            if (_site != null)
            {
                Guid? SuccID = AQSHelper.AQSGeneration_Orchestrator(_site.ORG_ID, _site.SITE_IDX, model.selMons, model.selDtStart.GetValueOrDefault(), model.selDtEnd.GetValueOrDefault(), UserIDX, model.selActionCode, model.selAQSFormat);
                if (SuccID != null)
                {
                    TempData["Success"] = "File generated and submission initiated.";
                    return RedirectToAction("AQSList", new { selOrgID=_site.ORG_ID });
                }
            }

            TempData["Error"] = "Unable to make submission";
            return RedirectToAction("AQSGen");
        }


        [HttpPost]
        public ActionResult AQSSubmitQA(vmDataAQSGen model)
        {
            string UserIDX = User.Identity.GetUserId();

            T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(model.selSite.GetValueOrDefault());
            if (_site != null)
            {
                Guid? SuccID = AQSHelper.AQS_QA_Generation_Orchestrator(_site.ORG_ID, _site.SITE_IDX, model.selQid.GetValueOrDefault(), UserIDX, model.selActionCode, model.selAQSFormat);
                if (SuccID != null)
                {
                    TempData["Success"] = "File generated and submission initiated.";
                    return RedirectToAction("AQSList", new { selOrgID = _site.ORG_ID });
                }
            }

            TempData["Error"] = "Unable to make submission";
            return RedirectToAction("AQSGenQA", new { qid = model.selQid });
        }


        public ActionResult AQSSubmitToEPA(Guid id)
        {
            string UserIDX = User.Identity.GetUserId();


            T_QREST_AQS _aqs = db_Air.GetT_QREST_AQS_by_ID(id);
            if (_aqs != null)
            {
                //reject if user doesn't have access to org
                RedirectToRouteResult r = CanAccessThisOrg(UserIDX, _aqs.ORG_ID, true);
                if (r != null) return r;

                bool SuccID = AQSHelper.AQSSubmission_Orchestrator(_aqs.ORG_ID, _aqs.AQS_CONTENT, _aqs.AQS_IDX);
                if (SuccID)
                {
                    TempData["Success"] = "File submitted.";
                    return RedirectToAction("AQSList", new { selOrgID = _aqs.ORG_ID });
                }

                TempData["Error"] = "Unable to make submission";
                return RedirectToAction("AQSList", new { selOrgID = _aqs.ORG_ID });
            }

            TempData["Error"] = "Unable to make submission";
            return RedirectToAction("AQSList");
        }


        [HttpPost]
        public JsonResult AQSDelete(Guid? id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int SuccID = db_Air.DeleteT_QREST_AQS(id.GetValueOrDefault());
                if (SuccID == 1)
                    return Json("Success");
                else
                    return Json("Unable to delete AQS record.");
            }
        }


        public ActionResult CDXTest(string orgid)
        {
            string UserIDX = User.Identity.GetUserId();

            //reject if user doesn't have access to org
            RedirectToRouteResult r = CanAccessThisOrg(UserIDX, orgid, true);
            if (r != null) return r;

            CDXCredentials _cred = AQSHelper.GetCDXSubmitCredentials2(orgid);
            if (_cred.NodeURL != null && _cred.credential != null)
            {
                string _token = AQSHelper.AuthHelper(_cred.userID, _cred.credential, _cred.NodeURL);
                if (_token!= null && _token.Substring(0,3)=="csm" &&  _token.Length>50)
                    TempData["Success"] = "CDX Username / password combination are correct. Authentication to CDX successful.";
                else
                    TempData["Error"] = "Username/password authentication to CDX failed.";
            }
            else
                TempData["Error"] = "Required fields not supplied.";

            return RedirectToAction("AQSAcct", new { id = orgid });
        }


        public JsonResult AQSGetStatus(Guid id)
        {
            string UserIDX = User.Identity.GetUserId();

            T_QREST_AQS _aqs = db_Air.GetT_QREST_AQS_by_ID(id);
            if (_aqs != null && _aqs.CDX_TOKEN != null) {

                //reject if user doesn't have access to org
                RedirectToRouteResult r = CanAccessThisOrg(UserIDX, _aqs.ORG_ID, true);
                if (r != null) return Json("Access Denied.");

                string strStatus = AQSHelper.AQSGetStatus_Orchestrator(_aqs.ORG_ID, _aqs.AQS_IDX, _aqs.CDX_TOKEN);
                if (strStatus != null)
                    return Json(strStatus);
                else
                    return Json("ERROR");
            }
            
            return Json("Failed");
        }


        public JsonResult AQSDownload(Guid id)
        {
            string UserIDX = User.Identity.GetUserId();

            T_QREST_AQS _aqs = db_Air.GetT_QREST_AQS_by_ID(id);
            if (_aqs != null && _aqs.CDX_TOKEN != null)
            {
                //reject if user doesn't have access to org
                RedirectToRouteResult r = CanAccessThisOrg(UserIDX, _aqs.ORG_ID, true);
                if (r != null) return Json("Access Denied.");

                bool SuccID = AQSHelper.AQSDownload_Orchestrator(_aqs.ORG_ID, _aqs.AQS_IDX, _aqs.CDX_TOKEN);
                if (SuccID)
                    return Json("Success");
            }

            return Json("Failed");
        }

        #endregion



        #region SHARED



        [HttpGet]
        public JsonResult FetchSites(string ID)
        {
            string UserIDX = User.Identity.GetUserId();
            var data = ddlHelpers.get_ddl_my_sites(ID, UserIDX);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID">ORG ID</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult FetchMonitors(string ID)
        {
            string UserIDX = User.Identity.GetUserId();
            var data = ddlHelpers.get_ddl_my_monitors(ID, UserIDX, false);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID">SITE IDX</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult FetchMonitorsBySite(Guid? ID)
        {
            var data = ddlHelpers.get_monitors_by_site(ID ?? Guid.Empty, true, false);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FetchMonitorsSampledBySite(Guid? ID)
        {
            var data = ddlHelpers.get_monitors_sampled_by_site(ID ?? Guid.Empty);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FetchMonitorsWithData(string ID)
        {
            var data = ddlHelpers.get_monitors_sampled_by_org(ID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult FetchImportTemplates(Guid? ID)
        {
            var data = ddlHelpers.get_ddl_import_templates(ID ?? Guid.Empty);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Retrieves the Hourly Import Template Configuration for the Site
        /// </summary>
        /// <param name="ID">Site ID</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult FetchImportTemplateH1(Guid? ID)
        {
            var data = db_Air.GetT_QREST_SITE_POLL_CONFIG_H1_BySite(ID ?? Guid.Empty);

            if (data != null)
                return Json(data, JsonRequestBehavior.AllowGet);
            else
                return Json("null", JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public JsonResult FetchAQSAccounts(Guid? ID)
        {
            string cdx_user = db_Ref.GetT_QREST_APP_SETTING("CDX_GLOBAL_USER");
            string aqs_user = "";
            string aqs_screening_grp = "";
            string org_id = "";

            var _site = db_Air.GetT_QREST_SITES_ByID(ID ?? Guid.Empty);
            if (_site != null)
            {
                var _org = db_Ref.GetT_QREST_ORGANIZATION_ByID(_site.ORG_ID);
                if (_org != null)
                {
                    org_id = _org.ORG_ID;
                    aqs_user = _org.AQS_AQS_UID;
                    aqs_screening_grp = _org.AQS_AQS_SCREENING_GRP;
                    if (!string.IsNullOrEmpty(_org.AQS_NAAS_UID))
                        cdx_user = _org.AQS_NAAS_UID;
                }              
            }

            var data = Tuple.Create(cdx_user, aqs_user, aqs_screening_grp, org_id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult MonitorSnapshot()
        {
            //filters
            Guid? selMon = Request.Form.GetValues("selMon")?.FirstOrDefault().ConvertOrDefault<Guid?>();

            if (selMon != null)
            {
                var data = db_Air.GetMONITOR_SNAPSHOT_ByMonitor(selMon);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
                return Json("Chart data error");
        }


        public RedirectToRouteResult CanAccessThisOrg(string UserIDX, string OrgID, bool CanEditToo)
        {
            if (db_Account.CanAccessThisOrg(UserIDX, OrgID, CanEditToo) == false)
            {
                TempData["Error"] = "Access Denied.";
                return RedirectToAction("SiteList", "Site");
            }
            else
                return null;
        }


        public RedirectToRouteResult CanAccessThisSite(string UserIDX, Guid SiteIDX, bool CanEditToo)
        {

            if (db_Account.CanAccessThisSite(UserIDX, SiteIDX, CanEditToo) == false)
            {
                TempData["Error"] = "Access Denied.";
                return RedirectToAction("SiteList", "Site");
            }
            else
                return null;
        }


        #endregion

    }
}