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
    
    public partial class SP_TRAIN_LESSON_USER_PROGRESS_Result
    {
        public System.Guid COURSE_IDX { get; set; }
        public System.Guid LESSON_IDX { get; set; }
        public Nullable<int> LESSON_SEQ { get; set; }
        public string LESSON_TITLE { get; set; }
        public string LESSON_DESC { get; set; }
        public int LESSON_COMP_IND { get; set; }
        public Nullable<System.DateTime> CREATE_DT { get; set; }
        public Nullable<int> LESSON_STEPS_COMP { get; set; }
    }
}
