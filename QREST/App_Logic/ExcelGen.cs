using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

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
                MemoryStream ms = new MemoryStream();
                wb.SaveAs(ms);
                ms.Position = 0;
                return ms;
            }
        }
    }
}