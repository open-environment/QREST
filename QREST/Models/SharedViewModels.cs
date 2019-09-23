using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QRESTModel.DAL;

namespace QREST.Models
{
    public class vmSharedNotificationHeader
    {
        public int NotifyCount { get; set; }
        public List<T_QREST_USER_NOTIFICATION> Notifications { get; set; }
    }
}