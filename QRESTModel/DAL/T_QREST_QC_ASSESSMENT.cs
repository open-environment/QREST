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
    
    public partial class T_QREST_QC_ASSESSMENT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_QREST_QC_ASSESSMENT()
        {
            this.T_QREST_QC_ASSESSMENT_DTL = new HashSet<T_QREST_QC_ASSESSMENT_DTL>();
        }
    
        public System.Guid QC_ASSESS_IDX { get; set; }
        public System.Guid MONITOR_IDX { get; set; }
        public System.DateTime ASSESSMENT_DT { get; set; }
        public string ASSESSMENT_TYPE { get; set; }
        public int ASSESSMENT_NUM { get; set; }
        public string UNIT_CODE { get; set; }
        public string ASSESSED_BY { get; set; }
        public string CREATE_USER_IDX { get; set; }
        public Nullable<System.DateTime> CREATE_DT { get; set; }
        public string MODIFY_USER_IDX { get; set; }
        public Nullable<System.DateTime> MODIFY_DT { get; set; }
        public string ASSESSMENT_TM { get; set; }
        public Nullable<System.Guid> AQS_IDX { get; set; }
    
        public virtual T_QREST_REF_UNITS T_QREST_REF_UNITS { get; set; }
        public virtual T_QREST_MONITORS T_QREST_MONITORS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_QREST_QC_ASSESSMENT_DTL> T_QREST_QC_ASSESSMENT_DTL { get; set; }
    }
}
