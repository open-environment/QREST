using ClosedXML.Excel;
using Microsoft.AspNet.Identity;
using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QREST.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Export()
        {
            return View();
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Export2(string[] exportdata)
        {
            //****validation **************
            if (exportdata == null)
            {
                TempData["Error"] = "You must select at least one option.";
                return RedirectToAction("Export", "Reports");
            }
            //***end validation ***********

            string UserIDX = User.Identity.GetUserId();

            DataTable dtSites = new DataTable("Sites");
            DataTable dtMonitors = new DataTable("Monitors");

            if (exportdata.Contains("Sites"))
            {
                List<T_QREST_SITES> _sites = db_Air.GetT_QREST_SITES_ByUser_OrgID(null, UserIDX);
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


                foreach (var _site in _sites)
                {
                    dtSites.Rows.Add(_site.ORG_ID, _site.SITE_ID, _site.SITE_NAME, _site.AQS_SITE_ID, _site.STATE_CD, _site.COUNTY_CD, _site.LATITUDE, _site.LONGITUDE, 
                        _site.ELEVATION, _site.ADDRESS, _site.CITY, _site.ZIP_CODE, _site.START_DT, _site.END_DT, _site.POLLING_ONLINE_IND, _site.POLLING_FREQ_TYPE,
                        _site.POLLING_FREQ_NUM, _site.AIRNOW_IND, _site.AQS_IND, _site.SITE_COMMENTS);
                }
            }

            if (exportdata.Contains("Monitors"))
            {
                List<SiteMonitorDisplayType> _mons = db_Air.GetT_QREST_MONITORS_ByUser_OrgID(null, UserIDX);

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

                foreach (var _mon in _mons)
                {
                    dtMonitors.Rows.Add(_mon.ORG_ID, _mon.SITE_ID, _mon.PAR_CODE, _mon.PAR_NAME, _mon.METHOD_CODE, _mon.T_QREST_MONITORS.POC, _mon.T_QREST_MONITORS.DURATION_CODE,
                        _mon.T_QREST_MONITORS.COLLECT_FREQ_CODE, _mon.T_QREST_MONITORS.COLLECT_UNIT_CODE, _mon.T_QREST_MONITORS.ALERT_MIN_VALUE, _mon.T_QREST_MONITORS.ALERT_MAX_VALUE,
                        _mon.T_QREST_MONITORS.ALERT_AMT_CHANGE, _mon.T_QREST_MONITORS.ALERT_STUCK_REC_COUNT);

                }
            }

            DataSet dsExport = new DataSet();
            if (dtSites.Rows.Count > 0)
                dsExport.Tables.Add(dtSites);

            if (dtMonitors.Rows.Count > 0)
                dsExport.Tables.Add(dtMonitors);
            if (dsExport.Tables.Count > 0)
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dsExport);
                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;

                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename= QRESTExport.xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);

                        Response.Flush();
                        Response.End();
                    }
                }

                return null;// View();
            }
            else
            {
                TempData["Error"] = "No data found to export";
                return RedirectToAction("Export");
            }
        }


    }
}