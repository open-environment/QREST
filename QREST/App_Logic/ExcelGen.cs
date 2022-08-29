using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace QREST.App_Logic
{
    public static class ExcelGen
    {
        public static MemoryStream GenExcelFromDataSet(DataSet ds)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                foreach (IXLWorksheet _ws in wb.Worksheets)
                {
                    _ws.Table(0).ShowAutoFilter = false;
                    //_ws.Table(0).Theme = XLTableTheme.None;
                    
                    foreach (var column in _ws.ColumnsUsed())
                    {
                        if (column.Cell(1).GetString() == "Value" || column.Cell(1).GetString() == "POC") {
                            try
                            {
                                string columnLetter = column.ColumnLetter();
                                string rng = $"${columnLetter}2:{columnLetter}50000";
                                _ws.Range(rng).DataType = XLDataType.Number;
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                        else if (column.Cell(1).GetString().Contains("DateTime"))
                        {
                            try
                            {
                                string columnLetter = column.ColumnLetter();
                                string rng = $"${columnLetter}2:{columnLetter}50000";
                                _ws.Range(rng).DataType = XLDataType.DateTime;
                                _ws.Range(rng).Style.NumberFormat.Format = "MM/dd/yyyy HH:mm";
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                        //else if (column.Cell(1).GetString().Contains("Time ("))
                        //{
                        //    try
                        //    {
                        //        string columnLetter = column.ColumnLetter();
                        //        string rng = $"${columnLetter}2:{columnLetter}50000";
                        //        _ws.Range(rng).DataType = XLDataType.Text;
                        //        _ws.Range(rng).Style.NumberFormat.Format = "HH:mm";
                        //    }
                        //    catch
                        //    {
                        //        // ignored
                        //    }
                        //}
                        else if (column.Cell(1).GetString().Contains("Date ("))
                        {
                            try
                            {
                                string columnLetter = column.ColumnLetter();
                                string rng = $"${columnLetter}2:{columnLetter}50000";
                                _ws.Range(rng).DataType = XLDataType.DateTime;
                                _ws.Range(rng).Style.NumberFormat.Format = "MM/dd/yyyy";
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    }
                }
                
                MemoryStream ms = new MemoryStream();
                wb.SaveAs(ms);
                ms.Position = 0;
                return ms;
            }
        }

        public static MemoryStream GenExcelFromDataTables(List<DataTable> dts)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                foreach (DataTable dt in dts)
                {
                    if (dt != null && dt.Rows.Count > 0) 
                        wb.Worksheets.Add(dt);
                }

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                MemoryStream ms = new MemoryStream();
                wb.SaveAs(ms);
                ms.Position = 0;
                return ms;
            }
        }
    }
}