//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QRESTModel.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_QREST_SITE_POLL_CONFIG_DTL
    {
        public System.Guid POLL_CONFIG_DTL_IDX { get; set; }
        public System.Guid POLL_CONFIG_IDX { get; set; }
        public System.Guid MONITOR_IDX { get; set; }
        public Nullable<int> COL { get; set; }
    
        public virtual T_QREST_SITE_POLL_CONFIG T_QREST_SITE_POLL_CONFIG { get; set; }
        public virtual T_QREST_MONITORS T_QREST_MONITORS { get; set; }
    }
}
