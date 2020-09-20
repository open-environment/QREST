using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            return RedirectToAction("Raw");
        }

        #region MANUAL IMPORT

        public ActionResult ManualImport(Guid? configid)
        {
            string UserIDX = User.Identity.GetUserId();

            //if there is a pending import for this user, redirect to status page
            Guid? prevImportIDX = db_Air.GetT_QREST_DATA_IMPORT_TEMP_ImportByUser(UserIDX);
            if (prevImportIDX != null)
                return RedirectToAction("ImportStatus", new { id = prevImportIDX });

            //if the user has an import with status of STARTED, then also redirect
            T_QREST_DATA_IMPORTS prevStartedIDX = db_Air.GetT_QREST_DATA_IMPORTS_StartedByUser(UserIDX);
            if (prevStartedIDX != null)
                return RedirectToAction("ImportStatus", new { id = prevStartedIDX.IMPORT_IDX });


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


            //**********************************************************************************************
            //*********************** MODEL VALIDATION PRIOR TO IMPORT**************************************
            //**********************************************************************************************

            //*********model validation for H1 *************************
            if (model.selImportType == "H1" && model.selMonitor==null)
                ModelState.AddModelError("selMonitor", "Parameter required for this import type.");
            if (model.selImportType == "H1" && string.IsNullOrEmpty(model.selTimeZone))
                ModelState.AddModelError("selTimeZone", "Local Time Zone required for this import type.");

            //*********model validation for H and F *************************
            T_QREST_SITE_POLL_CONFIG _pollConfig = null;
            List<SitePollingConfigDetailType> _pollConfigDtl = null;

            if (model.selImportType == "H" || model.selImportType == "F")
            {
                if (model.selPollConfig == null)
                    ModelState.AddModelError("selPollConfig", "Import Template is required.");

                _pollConfig = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(model.selPollConfig ?? Guid.Empty);

                if (_pollConfig != null)
                {
                    if (_pollConfig.DATE_COL == null && _pollConfig.TIME_COL == null) 
                        ModelState.AddModelError("selPollConfig", "Selected polling config does not define date and/or time column.");
                }
                else
                    ModelState.AddModelError("selPollConfig", "Polling configuration cannot be found.");

                //Verify all import config columns have units
                _pollConfigDtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_pollConfig.POLL_CONFIG_IDX, false);
                foreach (SitePollingConfigDetailType _temp in _pollConfigDtl)
                {
                    if (_temp.COLLECT_UNIT_CODE == null || _temp.COL == null)
                        ModelState.AddModelError("selPollConfig", "One or more parameters in your import configuration do not have a unit or column specified.");
                }
            }
            //**********************************************************************************************
            //*********************** END MODEL VALIDATION *************************************************
            //**********************************************************************************************


            string UserIDX = User.Identity.GetUserId();


            if (ModelState.IsValid)
            {
                //insert/update POLL_CONFIG if H1 with default timezone and time type
                if (model.selImportType == "H1")
                    model.selPollConfig = db_Air.InsertUpdatetT_QREST_SITE_POLL_CONFIG(null, model.selSite, "H1", null, "H1", null, null, null, null, null, null, null, null, null, model.selTimeZone, false, UserIDX, null, model.selTimeType, false);

                //create a new import tracking record, storing the import data block and setting status to STARTED
                Guid? importIDX = db_Air.InsertUpdateT_QREST_DATA_IMPORTS(Guid.NewGuid(), model.selOrgID, model.selSite, "", "STARTED", UserIDX, System.DateTime.Now, model.IMPORT_BLOCK,
                    model.selImportType, model.selMonitor, model.selPollConfig, (model.selImportType == "F" ? (model.selCalc == "Y") : (model.selVal == "Y")) );

                if (importIDX != null)
                {
                    //if not too many records, import immediately
                    if (model.IMPORT_BLOCK.Length < 10000000)
                        QRESTModel.BLL.ImportHelper.ImportValidateAndSaveToTemp(importIDX.GetValueOrDefault());
                    else
                    {
                        //set the import task to run now
                        db_Ref.UpdateT_QREST_TASKS(9999, null, null, null, System.DateTime.Now, null, UserIDX, "Import");

                        TempData["Success"] = "Import file is large and scheduled to run in a few minutes";
                    }

                    return RedirectToAction("ImportStatus", new { id = importIDX });
                }
                else
                    TempData["Error"] = "Import failure";
            }


            //if got this far, import errors occurred; reinitialize model
            model.ddl_ImportType = ddlHelpers.get_ddl_import_type();
            model.ddl_Time = ddlHelpers.get_ddl_time_type();
            model.ddl_TimeZone = ddlHelpers.get_ddl_time_zone();
            model.ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, true);
            model.ddl_Sites = model.selOrgID == null ? new List<SelectListItem>() : ddlHelpers.get_ddl_my_sites(model.selOrgID, UserIDX);
            model.ddl_Monitors = model.selSite == null ? new List<SelectListItem>() : ddlHelpers.get_monitors_by_site(model.selSite, true, false);
            model.ddl_PollConfig = model.selSite == null ? new List<SelectListItem>() : ddlHelpers.get_ddl_import_templates(model.selSite);
            model.ddl_Calc = ddlHelpers.get_ddl_yes_no();
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
                T_QREST_SITE_POLL_CONFIG _config = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(id.GetValueOrDefault());
                if (_config != null)
                {
                    model.SITE_IDX = _config.SITE_IDX;
                    model.editCONFIG_NAME = _config.CONFIG_NAME;
                    model.editCONFIG_DESC = _config.CONFIG_DESC;
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
                    model.editLOCAL_TIMEZONE, false, UserIDX, null, model.editTIME_POLL_TYPE, false, null, model.editCONFIG_DESC);

                if (SuccID != null)
                {
                    TempData["Success"] = "Record updated";
                    return RedirectToAction("ImportConfig", new { id = SuccID });
                }
                else
                    TempData["Error"] = "Error updating record.";
            }

            //reinitialize model
            model.ddl_Monitors = ddlHelpers.get_monitors_by_site(model.SITE_IDX.GetValueOrDefault(), false, true);
            return View(model);
        }


        [HttpPost]
        public JsonResult ImportConfigDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Json("No record selected to delete");
            else
            {
                Guid idg = new Guid(id);

                //reject if user doesn't have access to org
                RedirectToRouteResult r = CanAccessThisOrg(User.Identity.GetUserId(), db_Air.GetT_QREST_SITE_POLL_CONFIG_org_ByID(idg), true);
                if (r != null) return Json("Access Denied");

                int SuccID = db_Air.DeleteT_QREST_SITE_POLL_CONFIG(idg);
                if (SuccID == 1)
                    return Json("Success");
                else
                    return Json("Unable to find polling configuration to delete.");
            }
        }


        [HttpPost]
        public ActionResult ImportManualOverride(vmImportStatus model)
        {
            if (model.T_QREST_DATA_IMPORTS.IMPORT_IDX != null)
            {
                QRESTModel.BLL.ImportHelper.ImportFinal(model.T_QREST_DATA_IMPORTS.IMPORT_IDX);

                return RedirectToAction("ImportStatus", new { id = model.T_QREST_DATA_IMPORTS.IMPORT_IDX });
            }
            else
            {
                TempData["Error"] = "Unable to find record to import";
                return RedirectToAction("ManualImport");
            }
        }


        public ActionResult ImportStatus(Guid? id) {
            var model = new vmImportStatus
            {
                T_QREST_DATA_IMPORTS = db_Air.GetT_QREST_DATA_IMPORTS_byID(id.GetValueOrDefault()),
                ImportTotalCount = 0,
                ImportValDupCount = db_Air.GetT_QREST_DATA_IMPORT_TEMP_DupCount(id.GetValueOrDefault()),
                ImportValErrorCount = db_Air.GetT_QREST_DATA_IMPORT_TEMP_ErrorCount(id.GetValueOrDefault())
            };

            if (model.T_QREST_DATA_IMPORTS != null)
            {
                model.selOrg = model.T_QREST_DATA_IMPORTS.ORG_ID;
                model.durationSecs = System.DateTime.Now.Subtract(model.T_QREST_DATA_IMPORTS.IMPORT_DT.GetValueOrDefault()).TotalSeconds;

                if (model.T_QREST_DATA_IMPORTS.SUBMISSION_STATUS == "IMPORTED")
                {
                    if (model.T_QREST_DATA_IMPORTS.IMPORT_TYPE=="F")
                        model.ImportTotalCount = db_Air.GetT_QREST_DATA_FIVE_MINcountByImportIDX(id.GetValueOrDefault());
                    else
                        model.ImportTotalCount = db_Air.GetT_QREST_DATA_HOURLYcountByImportIDX(id.GetValueOrDefault());
                }
                else
                    model.ImportTotalCount = db_Air.GetT_QREST_DATA_IMPORT_TEMP_Count(id.GetValueOrDefault());
            }
            return View(model);
        }


        [HttpPost]
        public JsonResult ImportStatusDelete(Guid? id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                int SuccID = db_Air.DeleteT_QREST_DATA_IMPORTS(id.GetValueOrDefault());
                if (SuccID == 1)
                    return Json("Success");
                else
                    return Json("Unable to find polling configuration to delete.");
            }
        }


        public JsonResult ImportStatusDupData(Guid? id)
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //data filters
            Guid? selImportID = Request.Form.GetValues("p_imp")?.FirstOrDefault().ConvertOrDefault<Guid?>();

            var data = db_Air.GetT_QREST_DATA_IMPORT_TEMP_Dup(selImportID.GetValueOrDefault(), pageSize, start, "DATA_DTTM_UTC", orderDir);
            var recordsTotal = db_Air.GetT_QREST_DATA_IMPORT_TEMP_DupCount(selImportID.GetValueOrDefault());

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        public JsonResult ImportStatusErrorData(Guid? id)
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //data filters
            Guid? selImportID = Request.Form.GetValues("p_imp")?.FirstOrDefault().ConvertOrDefault<Guid?>();

            var data = db_Air.GetT_QREST_DATA_IMPORT_TEMP_Error(selImportID.GetValueOrDefault(), pageSize, start, "DATA_DTTM_UTC", orderDir);
            var recordsTotal = db_Air.GetT_QREST_DATA_IMPORT_TEMP_ErrorCount(selImportID.GetValueOrDefault());

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        public ActionResult ImportList(string selOrgID)
        {
            string UserIDX = User.Identity.GetUserId();
            var model = new vmDataImportList
            {
                ddl_Organization = ddlHelpers.get_ddl_my_organizations(UserIDX, false),
            };

            //auto-populate if only rights to 1 org that has made aqs submission
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
                T_QREST_DATA_IMPORTS _imp = db_Air.GetT_QREST_DATA_IMPORTS_byID(id.GetValueOrDefault());
                if (_imp != null)
                {
                    int SuccID = 0;
                    if (_imp.IMPORT_TYPE == "H" || _imp.IMPORT_TYPE == "H1" || _imp.IMPORT_TYPE == null)
                        SuccID = db_Air.DeleteT_QREST_DATA_HOURLY_ByImportIDX(id.GetValueOrDefault());
                    else if (_imp.IMPORT_TYPE == "F")
                        SuccID = db_Air.DeleteT_QREST_DATA_FIVE_MIN_ByImportIDX(id.GetValueOrDefault());

                    if (SuccID == 1)
                    {
                        db_Air.DeleteT_QREST_DATA_IMPORTS(id.GetValueOrDefault());
                        if (SuccID == 1)
                        {
                            TempData["Success"] = "Deleted";
                            return Json("Success");
                        }
                    }
                }

                return Json("Unable to delete Import record.");
            }
        }


        public FileResult ImportFileDownload(Guid? id)
        {
            try
            {
                T_QREST_DATA_IMPORTS _imp = db_Air.GetT_QREST_DATA_IMPORTS_byID(id.GetValueOrDefault());
                if (_imp != null)
                {
                    DataTable _dt = DataTableGen.GetHourlyDataByImportIDX(id.GetValueOrDefault());

                    //five min
                    if (_imp.IMPORT_TYPE == "F")
                        _dt = DataTableGen.GetFiveMinDataByImportIDX(id.GetValueOrDefault());

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
                        TempData["Error"] = "No data found to export";
                }
                else
                    TempData["Error"] = "No data found to export";

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
                ddl_AQS_Null = ddlHelpers.get_ddl_ref_qualifier("NULL"),
                ddl_FlowRate_Unit = ddlHelpers.get_ddl_ref_units("68101"),
                DisplayUnit = false
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

                //unit display
                if (model.ASSESSMENT_TYPE == "Flow Rate Verification" || model.ASSESSMENT_TYPE == "Semi-Annual Flow Rate Audit")
                    model.DisplayUnit = true;

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
                        else if (model.ASSESSMENT_TYPE == "Flow Rate Verification")
                        {
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, null, null, null, UserIDX);
                        }
                        else if (model.ASSESSMENT_TYPE == "Semi-Annual Flow Rate Audit")
                        {
                            db_Air.InsertUpdatetT_QREST_QC_ASSESSMENT_DTL(null, AssessIDX, null, null, null, null, UserIDX);
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
            model.ddl_FlowRate_Unit = ddlHelpers.get_ddl_ref_units("68101");
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
            string userIdx = User.Identity.GetUserId();

            string[] d = model.selDate.Replace(" - ", "z").Split('z');
            if (d.Length == 2)
            {
                DateTime? d1 = d[0].ConvertOrDefault<DateTime?>();
                DateTime? d2 = (d.Length > 1) ? d[1].ConvertOrDefault<DateTime?>() : null;

                if (model.selType == "H")
                {
                    if (model.selTimeType == "L")
                        model.RawData = db_Air.GetT_QREST_DATA_FIVE_MIN_RawDataViewLocal(model.selOrgID, model.selMon.ConvertOrDefault<Guid?>(), d1, d2);
                    else
                        model.RawData = db_Air.GetT_QREST_DATA_FIVE_MIN_RawDataViewUTC(model.selOrgID, model.selMon.ConvertOrDefault<Guid?>(), d1, d2);
                }
                else if (model.selType == "1")
                    model.RawData = db_Air.GetT_QREST_DATA_HOURLY(model.selOrgID, model.selMon.ConvertOrDefault<Guid?>(), d1, d2, 25000, 0, 3, "asc", model.selTimeType);
            }

            //reinitialize
            model.ddl_Organization = ddlHelpers.get_ddl_my_organizations(userIdx, true);
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
            string userIdx = User.Identity.GetUserId();
            var model = new vmDataReviewSummary {
                ddl_Sites = ddlHelpers.get_ddl_my_sites(null, userIdx),
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
        public ActionResult DataReview2(Guid? monid, DateTime? sdt, DateTime? edt, string dur, Guid? supp1, string md)
        {
            if (monid == null || sdt == null || edt == null || dur == null)
                return RedirectToAction("DataReviewSummary");

            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataReview2
            {
                selDtStart = sdt.GetValueOrDefault(),
                selDtEnd = edt.GetValueOrDefault(),
                selDuration = dur,
                selMon = db_Air.GetT_QREST_MONITORS_ByID(monid.GetValueOrDefault()),
                selMode = md
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

                if (model.editDeleteRecords == true)
                {
                    foreach (var item in model.editRawDataIDX)
                    {
                        int succId = db_Air.DeleteT_QREST_DATA_HOURLY(item);
                        if (succId == 0)
                            TempData["Error"] = "Error deleting record";
                    }
                }
                else if (model.editRawDataIDX != null)
                {
                    foreach (var item in model.editRawDataIDX)
                    {
                        editCount++;

                        Guid? succId = db_Air.UpdateT_QREST_DATA_HOURLY(item, model.editNullQual, lvl1ind, lvl2ind, UserIDX, model.editUnitCode, model.editNotes, (model.editValueBlank == true ? "-999" : model.editValue), (model.editFlagBlank == true ? "-999" : model.editFlag), model.editQual);
                        if (succId == null)
                            TempData["Error"] = "Error updating record";
                        else
                            db_Air.InsertUpdatetT_QREST_DATA_HOURLY_LOG(null, succId, model.editNotes, UserIDX);
                    }
                }
                else
                    TempData["Error"] = "You must select a row to edit.";

                return RedirectToAction("DataReview2", new { monid = model.selMon.T_QREST_MONITORS.MONITOR_IDX, sdt = model.selDtStart, edt = model.selDtEnd, dur = model.selDuration, md = model.selMode });

            }
            else
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                TempData["Error"] = "Model Error";
                return RedirectToAction("DataReview2", new { monid = model.selMon.T_QREST_MONITORS.MONITOR_IDX, sdt = model.selDtStart, edt = model.selDtEnd, dur = model.selDuration, md = model.selMode });
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
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault(); 

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
                int succId = db_Air.DeleteT_QREST_ASSESS_DOCS(id.GetValueOrDefault());
                if (succId == 1)
                    return Json("Success");
                else if (succId == -1)
                    return Json("Cannot delete Organization that still has site records. Delete sites first.");
                else
                    return Json("Unable to find organization to delete.");
            }
        }


        public FileResult DataDocsDownload(Guid? id)
        {
            try
            {
                T_QREST_ASSESS_DOCS _doc = db_Air.GetT_QREST_ASSESS_DOCS_ByID(id.GetValueOrDefault());
                if (_doc != null)
                {
                    //reject if user doesn't have access to site
                    RedirectToRouteResult r = CanAccessThisSite(User.Identity.GetUserId(), _doc.SITE_IDX, true);
                    if (r != null) return null;

                    return File(_doc.DOC_CONTENT, System.Net.Mime.MediaTypeNames.Application.Octet, _doc.DOC_NAME);
                }
            }
            catch
            {
                // ignored
            }

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


        public ActionResult AQSGen(Guid? id, DateTime? sDt, DateTime? eDt, string typ)
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmDataAQSGen {
                ddl_Sites = ddlHelpers.get_ddl_my_sites_sampled(null, UserIDX),
                selAQSTransType = typ ?? "RD",
                selDtStart = sDt ?? new DateTime(System.DateTime.Today.Year, System.DateTime.Today.Month, 1).AddMonths(-1),
                selDtEnd = eDt ?? new DateTime(System.DateTime.Today.Year, System.DateTime.Today.Month, 1).AddHours(-1),
                passValidation = false,
                selSite = id
            };

            if (model.selActionCode == null)
                model.selActionCode = "I";
            if (model.selAQSFormat == null)
                model.selAQSFormat = "F";

            //if site and dates are possed in, then go for the review
            if (model.selSite != null && model.selDtStart != null && model.selDtEnd != null)
            {
                List<SiteMonitorDisplayType> _ms = db_Air.GetT_QREST_MONITORS_Display_SampledBySiteIDX(model.selSite);
                foreach (SiteMonitorDisplayType _m in _ms) {
                    model.selMons.Add(_m.T_QREST_MONITORS.MONITOR_IDX);
                }

                AQSGenDataReview(model);
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult AQSGen(vmDataAQSGen model)
        {
            AQSGenDataReview(model);

            //repopulate model before returning
            string UserIDX = User.Identity.GetUserId();
            model.ddl_Sites = ddlHelpers.get_ddl_my_sites_sampled(null, UserIDX);

            return View(model);
        }


        private static void AQSGenDataReview(vmDataAQSGen model)
        {
            model.Results = db_Air.SP_AQS_REVIEW_STATUS(model.selSite ?? Guid.Empty, model.selDtStart.GetValueOrDefault(), model.selDtEnd);
            model.Results = model.Results.Where(o => model.selMons.Contains(o.MONITOR_IDX)).ToList();

            model.passValidation = true;

            foreach (SP_AQS_REVIEW_STATUS_Result _result in model.Results)
            {
                if (_result.hrs != _result.lvl2_val_ind)
                    model.passValidation = false;
            }
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
                    TempData["Success"] = "AQS File generated - click to proceed with submission";
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
        public JsonResult FetchMonitorsQC(string qctyp)
        {
            string UserIDX = User.Identity.GetUserId();
            var data = ddlHelpers.get_monitors_sampled_by_user_qc_type(UserIDX, qctyp);

            if (data.Count() == 0)
                data = ddlHelpers.get_monitors_sampled_by_user(UserIDX);
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


        public RedirectToRouteResult CanAccessThisOrg(string UserIDX, string orgId, bool canEditToo)
        {
            if (db_Account.CanAccessThisOrg(UserIDX, orgId, canEditToo) == false)
            {
                TempData["Error"] = "Access Denied.";
                return RedirectToAction("SiteList", "Site");
            }
            else
                return null;
        }


        public RedirectToRouteResult CanAccessThisSite(string UserIDX, Guid SiteIDX, bool canEditToo)
        {

            if (db_Account.CanAccessThisSite(UserIDX, SiteIDX, canEditToo) == false)
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