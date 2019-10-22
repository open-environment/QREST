using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QREST.Models
{
    public class vmDashboardIndex
    {
        public string Announcement { get; set; }
        public int? MySiteCount { get; set; }
        public int? MyMonitorCount { get; set; }
        public int? MyAlertCount { get; set; }
        public List<T_QREST_SITES> T_QREST_SITES { get; set; }

    }
}