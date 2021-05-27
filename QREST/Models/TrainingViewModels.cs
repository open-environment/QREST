using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QRESTModel.DAL;

namespace QREST.Models
{
    public class vmTrainingIndex
    {
        public List<SP_TRAIN_COURSE_USER_PROGRESS_Result> courses { get; set; }

    }


    public class vmTrainingCourse
    {
        public T_QREST_TRAIN_COURSE course { get; set; }
        public List<SP_TRAIN_LESSON_USER_PROGRESS_Result> lessons { get; set; }
    }


    public class vmTrainingLesson
    {
        public T_QREST_TRAIN_COURSE course { get; set; }
        public T_QREST_TRAIN_LESSON lesson { get; set; }
        public List<SP_TRAIN_STEP_USER_PROGRESS_Result> steps { get; set; }
        public Guid? compLessonStepIDX { get; set; }
    }

    public class vmTrainingCertificate
    {
        public SP_TRAIN_COURSE_USER_PROGRESS_Result course { get; set; }
        public string user_name { get; set; }
    }
}