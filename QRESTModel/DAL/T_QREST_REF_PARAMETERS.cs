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
    
    public partial class T_QREST_REF_PARAMETERS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_QREST_REF_PARAMETERS()
        {
            this.T_QREST_REF_PAR_METHODS = new HashSet<T_QREST_REF_PAR_METHODS>();
        }
    
        public string PAR_CODE { get; set; }
        public string PAR_NAME { get; set; }
        public string PAR_NAME_ALT { get; set; }
        public string CAS_NUM { get; set; }
        public string STD_UNIT_CODE { get; set; }
        public bool SHORTLIST_IND { get; set; }
        public bool ACT_IND { get; set; }
        public string CREATE_USER_IDX { get; set; }
        public Nullable<System.DateTime> CREATE_DT { get; set; }
        public string MODIFY_USER_IDX { get; set; }
        public Nullable<System.DateTime> MODIFY_DT { get; set; }
        public bool AQS_IND { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_QREST_REF_PAR_METHODS> T_QREST_REF_PAR_METHODS { get; set; }
        public virtual T_QREST_REF_UNITS T_QREST_REF_UNITS { get; set; }
    }
}
