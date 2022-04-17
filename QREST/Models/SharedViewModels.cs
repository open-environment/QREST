using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QRESTModel.DAL;

namespace QREST.Models
{
    public class vmSharedNotificationHeader
    {
        public int NotifyCount { get; set; }
        public List<T_QREST_USER_NOTIFICATION> Notifications { get; set; }
    }

    public class vmSharedEnvironment {
        public string environment { get; set; }

    }


    // ******************************** LOGGING***********************************
    //****************************************************************************
    //****************************************************************************
    public class vmSharedLogActivity
    {
        public IEnumerable<SelectListItem> ddl_Organization { get; set; }
        public string selOrgID { get; set; }
        public string SITE_IDX { get; set; }
        public string POLL_CONFIG_IDX { get; set; }
    }
}