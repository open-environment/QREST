using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace QREST.Models
{


    public class vmDataQCList
    {
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public string selOrgID { get; set; }
    }

    public class vmDataQCEntry
    {
    }
}