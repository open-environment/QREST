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
    
    public partial class USERLIST_DISPLAY_VIEW
    {
        public string USER_IDX { get; set; }
        public string Email { get; set; }
        public string FNAME { get; set; }
        public string LNAME { get; set; }
        public Nullable<System.DateTime> LAST_LOGIN_DT { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public bool EmailConfirmed { get; set; }
        public Nullable<System.DateTime> CREATE_DT { get; set; }
        public string Name { get; set; }
        public string ORG_ID { get; set; }
    }
}