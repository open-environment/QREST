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
                MemoryStream ms = new MemoryStream();
                wb.SaveAs(ms);
                ms.Position = 0;
                return ms;
            }
        }

        public static MemoryStream GenExcelFromDataTables(DataTable dt1, DataTable dt2, DataTable dt3, DataTable dt4)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                if (dt1 != null && dt1.Rows.Count > 0)
                    wb.Worksheets.Add(dt1);
                if (dt2 != null && dt2.Rows.Count > 0)
                    wb.Worksheets.Add(dt2);
                if (dt3 != null && dt3.Rows.Count > 0)
                    wb.Worksheets.Add(dt3);
                if (dt4 != null && dt4.Rows.Count > 0)
                    wb.Worksheets.Add(dt4);

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