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
    
    public partial class T_QREST_QC_ASSESSMENT_DTL
    {
        public System.Guid QC_ASSESS_DTL_IDX { get; set; }
        public System.Guid QC_ASSESS_IDX { get; set; }
        public Nullable<double> MON_CONCENTRATION { get; set; }
        public Nullable<double> ASSESS_KNOWN_CONCENTRATION { get; set; }
        public string AQS_NULL_CODE { get; set; }
        public string COMMENTS { get; set; }
        public string CREATE_USER_IDX { get; set; }
        public Nullable<System.DateTime> CREATE_DT { get; set; }
        public string MODIFY_USER_IDX { get; set; }
        public Nullable<System.DateTime> MODIFY_DT { get; set; }
    
        public virtual T_QREST_QC_ASSESSMENT T_QREST_QC_ASSESSMENT { get; set; }
    }
}
