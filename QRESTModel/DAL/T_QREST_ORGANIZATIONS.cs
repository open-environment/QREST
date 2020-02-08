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
    
    public partial class T_QREST_ORGANIZATIONS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_QREST_ORGANIZATIONS()
        {
            this.T_QREST_ORG_EMAIL_RULE = new HashSet<T_QREST_ORG_EMAIL_RULE>();
            this.T_QREST_ORG_USERS = new HashSet<T_QREST_ORG_USERS>();
            this.T_QREST_SITES = new HashSet<T_QREST_SITES>();
            this.T_QREST_AQS = new HashSet<T_QREST_AQS>();
            this.T_QREST_DATA_IMPORTS = new HashSet<T_QREST_DATA_IMPORTS>();
        }
    
        public string ORG_ID { get; set; }
        public string ORG_NAME { get; set; }
        public string AQS_AGENCY_CODE { get; set; }
        public string STATE_CD { get; set; }
        public Nullable<int> EPA_REGION { get; set; }
        public string AQS_NAAS_UID { get; set; }
        public string AQS_NAAS_PWD { get; set; }
        public bool ACT_IND { get; set; }
        public string CREATE_USER_IDX { get; set; }
        public Nullable<System.DateTime> CREATE_DT { get; set; }
        public string MODIFY_USER_IDX { get; set; }
        public Nullable<System.DateTime> MODIFY_DT { get; set; }
        public Nullable<bool> SELF_REG_IND { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_QREST_ORG_EMAIL_RULE> T_QREST_ORG_EMAIL_RULE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_QREST_ORG_USERS> T_QREST_ORG_USERS { get; set; }
        public virtual T_QREST_REF_AQS_AGENCY T_QREST_REF_AQS_AGENCY { get; set; }
        public virtual T_QREST_REF_REGION T_QREST_REF_REGION { get; set; }
        public virtual T_QREST_REF_STATE T_QREST_REF_STATE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_QREST_SITES> T_QREST_SITES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_QREST_AQS> T_QREST_AQS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_QREST_DATA_IMPORTS> T_QREST_DATA_IMPORTS { get; set; }
    }
}
