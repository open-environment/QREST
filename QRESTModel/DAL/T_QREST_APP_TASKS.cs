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
    
    public partial class T_QREST_APP_TASKS
    {
        public int TASK_IDX { get; set; }
        public string TASK_NAME { get; set; }
        public string TASK_DESC { get; set; }
        public string FREQ_TYPE { get; set; }
        public int FREQ_NUM { get; set; }
        public System.DateTime LAST_RUN_DT { get; set; }
        public System.DateTime NEXT_RUN_DT { get; set; }
        public string STATUS { get; set; }
        public string MODIFY_USER_IDX { get; set; }
        public Nullable<System.DateTime> MODIFY_DT { get; set; }
    }
}
