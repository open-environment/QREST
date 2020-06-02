using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QREST.Models
{
    public class vmReportsExport
    {
        public string selOrgID { get; set; }
        public Guid? selMon { get; set; }
        public string selType { get; set; }
        public string selDate { get; set; }
        public string selTimeType { get; set; }
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public IEnumerable<SelectListItem> ddl_Monitor { get; set; }
        public IEnumerable<SelectListItem> ddl_TimeType { get; set; }

        public vmReportsExport(){
            ddl_TimeType = ddlHelpers.get_ddl_time_type();
        }
    }
}