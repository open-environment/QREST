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
    
    public partial class T_QREST_REF_UNITS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_QREST_REF_UNITS()
        {
            this.T_QREST_QC_ASSESSMENT = new HashSet<T_QREST_QC_ASSESSMENT>();
            this.T_QREST_REF_PAR_METHODS = new HashSet<T_QREST_REF_PAR_METHODS>();
            this.T_QREST_REF_PARAMETERS = new HashSet<T_QREST_REF_PARAMETERS>();
        }
    
        public string UNIT_CODE { get; set; }
        public string UNIT_DESC { get; set; }
        public bool ACT_IND { get; set; }
        public string CREATE_USER_IDX { get; set; }
        public Nullable<System.DateTime> CREATE_DT { get; set; }
        public string MODIFY_USER_IDX { get; set; }
        public Nullable<System.DateTime> MODIFY_DT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_QREST_QC_ASSESSMENT> T_QREST_QC_ASSESSMENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_QREST_REF_PAR_METHODS> T_QREST_REF_PAR_METHODS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_QREST_REF_PARAMETERS> T_QREST_REF_PARAMETERS { get; set; }
    }
}