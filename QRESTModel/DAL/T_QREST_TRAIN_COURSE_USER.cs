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
    
    public partial class T_QREST_TRAIN_COURSE_USER
    {
        public System.Guid COURSE_IDX { get; set; }
        public string USER_IDX { get; set; }
        public System.DateTime CREATE_DT { get; set; }
    
        public virtual T_QREST_TRAIN_COURSE T_QREST_TRAIN_COURSE { get; set; }
    }
}
