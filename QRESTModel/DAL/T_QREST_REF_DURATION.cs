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
    
    public partial class T_QREST_REF_DURATION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_QREST_REF_DURATION()
        {
            this.T_QREST_MONITORS = new HashSet<T_QREST_MONITORS>();
            this.T_QREST_SITE_POLL_CONFIG = new HashSet<T_QREST_SITE_POLL_CONFIG>();
        }
    
        public string DURATION_CODE { get; set; }
        public string DURATION_DESC { get; set; }
        public bool ACT_IND { get; set; }
        public string CREATE_USER_IDX { get; set; }
        public Nullable<System.DateTime> CREATE_DT { get; set; }
        public string MODIFY_USER_IDX { get; set; }
        public Nullable<System.DateTime> MODIFY_DT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_QREST_MONITORS> T_QREST_MONITORS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_QREST_SITE_POLL_CONFIG> T_QREST_SITE_POLL_CONFIG { get; set; }
    }
}
