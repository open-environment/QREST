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
    
    public partial class T_QREST_SITE_POLL_CONFIG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_QREST_SITE_POLL_CONFIG()
        {
            this.T_QREST_SITE_POLL_CONFIG_DTL = new HashSet<T_QREST_SITE_POLL_CONFIG_DTL>();
        }
    
        public System.Guid POLL_CONFIG_IDX { get; set; }
        public System.Guid SITE_IDX { get; set; }
        public string RAW_DURATION_CODE { get; set; }
        public string LOGGER_TYPE { get; set; }
        public string LOGGER_SOURCE { get; set; }
        public Nullable<int> LOGGER_PORT { get; set; }
        public string LOGGER_USERNAME { get; set; }
        public string LOGGER_PASSWORD { get; set; }
        public string DELIMITER { get; set; }
        public Nullable<int> DATE_COL { get; set; }
        public string DATE_FORMAT { get; set; }
        public Nullable<int> TIME_COL { get; set; }
        public string TIME_FORMAT { get; set; }
        public bool ACT_IND { get; set; }
        public string CREATE_USER_IDX { get; set; }
        public Nullable<System.DateTime> CREATE_DT { get; set; }
        public string MODIFY_USER_IDX { get; set; }
        public Nullable<System.DateTime> MODIFY_DT { get; set; }
        public string LOCAL_TIMEZONE { get; set; }
        public string CONFIG_NAME { get; set; }
        public string TIME_POLL_TYPE { get; set; }
        public string CONFIG_DESC { get; set; }
        public string LOGGER_FILE_NAME { get; set; }
    
        public virtual T_QREST_REF_DURATION T_QREST_REF_DURATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_QREST_SITE_POLL_CONFIG_DTL> T_QREST_SITE_POLL_CONFIG_DTL { get; set; }
        public virtual T_QREST_SITES T_QREST_SITES { get; set; }
    }
}
