using System;
using System.Collections.Generic;
using System.Linq;

namespace QRESTModel.DAL
{

    public class CourseLessonDisplay
    {
        public Guid COURSE_IDX { get; set; }
        public Guid LESSON_IDX { get; set; }
        public int? LESSON_SEQ { get; set; }
        public string LESSON_TITLE { get; set; }
        public string LESSON_DESC { get; set; }
    }

    public class db_Train
    {

        //************** COURSES **********************************
        //************** COURSES **********************************
        //************** COURSES **********************************

        public static T_QREST_TRAIN_COURSE GetT_QREST_TRAIN_COURSE_byID(Guid cOURSE_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_TRAIN_COURSE.AsNoTracking()
                            where a.COURSE_IDX == cOURSE_IDX
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static T_QREST_TRAIN_COURSE GetT_QREST_TRAIN_COURSE_byLESSONID(Guid lESSON_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_TRAIN_COURSE_LESSON.AsNoTracking()
                            join b in ctx.T_QREST_TRAIN_COURSE.AsNoTracking() on a.COURSE_IDX equals b.COURSE_IDX
                            where a.LESSON_IDX == lESSON_IDX
                            select b).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<T_QREST_TRAIN_COURSE> GetT_QREST_TRAIN_COURSE()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_TRAIN_COURSE.AsNoTracking()
                            orderby a.COURSE_SEQ
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static Guid? InsertUpdateT_QREST_TRAIN_COURSE(Guid? cOURSE_IDX, string cOURSE_NAME, string cOURSE_DESC, int? cOURSE_SEQ, bool? aCT_IND)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    bool insInd = false;

                    T_QREST_TRAIN_COURSE e = (from c in ctx.T_QREST_TRAIN_COURSE
                                              where c.COURSE_IDX == cOURSE_IDX 
                                              select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_TRAIN_COURSE();
                        e.COURSE_IDX = Guid.NewGuid();
                    }

                    if (cOURSE_NAME != null) e.COURSE_NAME = cOURSE_NAME;
                    if (cOURSE_DESC != null) e.COURSE_DESC = cOURSE_DESC;
                    if (cOURSE_SEQ != null) e.COURSE_SEQ = cOURSE_SEQ;
                    if (aCT_IND != null) e.ACT_IND = aCT_IND ?? true;

                    if (insInd)
                        ctx.T_QREST_TRAIN_COURSE.Add(e);

                    ctx.SaveChanges();
                    return e.COURSE_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<SP_TRAIN_COURSE_USER_PROGRESS_Result> SP_TRAIN_COURSE_USER_PROGRESS(string uSER_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_TRAIN_COURSE_USER_PROGRESS(uSER_IDX).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static SP_TRAIN_COURSE_USER_PROGRESS_Result SP_TRAIN_COURSE_USER_PROGRESS_ByCourse(string uSER_IDX, Guid cOURSE_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = SP_TRAIN_COURSE_USER_PROGRESS(uSER_IDX);

                    var yyy = xxx.Where(x => x.COURSE_IDX == cOURSE_IDX);

                    return yyy.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }



        public static bool InsertT_QREST_TRAIN_COURSE_USER(Guid? cOURSE_IDX, string uSER_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_TRAIN_COURSE_USER e = (from c in ctx.T_QREST_TRAIN_COURSE_USER
                                                   where c.COURSE_IDX == cOURSE_IDX
                                                   && c.USER_IDX == uSER_IDX
                                                   select c).FirstOrDefault();

                    if (e == null)
                    {
                        e = new T_QREST_TRAIN_COURSE_USER();
                        e.COURSE_IDX = cOURSE_IDX.GetValueOrDefault();
                        e.USER_IDX = uSER_IDX;
                        e.CREATE_DT = System.DateTime.Now;
                        ctx.T_QREST_TRAIN_COURSE_USER.Add(e);
                        ctx.SaveChanges();

                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return false;
                }
            }
        }



        public static int DeleteT_QREST_TRAIN_COURSE(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_TRAIN_COURSE rec = new T_QREST_TRAIN_COURSE { COURSE_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }



        //************** LESSONS **********************************
        //************** LESSONS **********************************
        //************** LESSONS **********************************

        public static T_QREST_TRAIN_LESSON GetT_QREST_TRAIN_LESSON_byID(Guid lESSON_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_TRAIN_LESSON.AsNoTracking()
                            where a.LESSON_IDX == lESSON_IDX
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static T_QREST_TRAIN_LESSON GetT_QREST_TRAIN_LESSON_byStepID(Guid lESSON_STEP_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_TRAIN_LESSON.AsNoTracking()
                            join b in ctx.T_QREST_TRAIN_LESSON_STEP.AsNoTracking() on a.LESSON_IDX equals b.LESSON_IDX
                            where b.LESSON_STEP_IDX == lESSON_STEP_IDX
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<CourseLessonDisplay> GetT_QREST_TRAIN_LESSONS_byCourse(Guid cOURSE_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_TRAIN_LESSON.AsNoTracking()
                            join b in ctx.T_QREST_TRAIN_COURSE_LESSON.AsNoTracking() on a.LESSON_IDX equals b.LESSON_IDX
                            where b.COURSE_IDX == cOURSE_IDX
                            select new CourseLessonDisplay { 
                                COURSE_IDX = b.COURSE_IDX,
                                LESSON_IDX = a.LESSON_IDX,
                                LESSON_TITLE = a.LESSON_TITLE,
                                LESSON_DESC = a.LESSON_DESC,
                                LESSON_SEQ = b.LESSON_SEQ
                            }).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static CourseLessonDisplay GetT_QREST_TRAIN_LESSON_byLesson(Guid lESSON_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_TRAIN_LESSON.AsNoTracking()
                            join b in ctx.T_QREST_TRAIN_COURSE_LESSON.AsNoTracking() on a.LESSON_IDX equals b.LESSON_IDX
                            where b.LESSON_IDX == lESSON_IDX
                            select new CourseLessonDisplay
                            {
                                COURSE_IDX = b.COURSE_IDX,
                                LESSON_IDX = a.LESSON_IDX,
                                LESSON_TITLE = a.LESSON_TITLE,
                                LESSON_DESC = a.LESSON_DESC,
                                LESSON_SEQ = b.LESSON_SEQ
                            }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<SP_TRAIN_LESSON_USER_PROGRESS_Result> SP_TRAIN_LESSON_USER_PROGRESS(string uSER_IDX, Guid cOURSE_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_TRAIN_LESSON_USER_PROGRESS(uSER_IDX, cOURSE_IDX).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        
        public static bool SP_TRAIN_LESSON_USER_PROGRESS_CompletedLessonCheck(string uSER_IDX, Guid cOURSE_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = ctx.SP_TRAIN_LESSON_USER_PROGRESS(uSER_IDX, cOURSE_IDX).ToList();
                    int unfinishedCount = xxx.Where(p => p.LESSON_COMP_IND == 0).Count();
                    return (unfinishedCount == 0);
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return false;
                }
            }
        }


        public static Guid? InsertUpdateT_QREST_TRAIN_LESSON(Guid? lESSON_IDX, string lESSON_TITLE, string lESSON_DESC, Guid cOURSE_IDX, int? lESSON_SEQ)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    bool insInd = false;

                    T_QREST_TRAIN_LESSON e = (from c in ctx.T_QREST_TRAIN_LESSON
                                              where c.LESSON_IDX == lESSON_IDX
                                              select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_TRAIN_LESSON();
                        e.LESSON_IDX = Guid.NewGuid();
                    }

                    if (lESSON_TITLE != null) e.LESSON_TITLE = lESSON_TITLE;
                    if (lESSON_DESC != null) e.LESSON_DESC = lESSON_DESC;

                    if (insInd)
                        ctx.T_QREST_TRAIN_LESSON.Add(e);

                    ctx.SaveChanges();


                    //insert or update LESSON_COURSE
                    InsertUpdateT_QREST_TRAIN_COURSE_LESSON(e.LESSON_IDX, cOURSE_IDX, lESSON_SEQ);


                    return e.LESSON_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }



        public static bool InsertUpdateT_QREST_TRAIN_COURSE_LESSON(Guid lESSON_IDX, Guid cOURSE_IDX, int? lESSON_SEQ)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    bool insInd = false;

                    T_QREST_TRAIN_COURSE_LESSON e = (from c in ctx.T_QREST_TRAIN_COURSE_LESSON
                                                      where c.LESSON_IDX == lESSON_IDX
                                                      && c.COURSE_IDX == cOURSE_IDX
                                                      select c).FirstOrDefault();


                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_TRAIN_COURSE_LESSON();
                        e.COURSE_IDX = cOURSE_IDX;
                        e.LESSON_IDX = lESSON_IDX;
                    }

                    if (lESSON_SEQ != null) e.LESSON_SEQ = lESSON_SEQ;

                    if (insInd)
                        ctx.T_QREST_TRAIN_COURSE_LESSON.Add(e);

                    ctx.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return false;
                }
            }
        }


        public static bool InsertT_QREST_TRAIN_LESSON_USER(Guid? lESSON_IDX, string uSER_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_TRAIN_LESSON_USER e = (from c in ctx.T_QREST_TRAIN_LESSON_USER
                                                   where c.LESSON_IDX == lESSON_IDX
                                                        && c.USER_IDX == uSER_IDX
                                                        select c).FirstOrDefault();

                    if (e == null)
                    {
                        e = new T_QREST_TRAIN_LESSON_USER();
                        e.LESSON_IDX = lESSON_IDX.GetValueOrDefault();
                        e.USER_IDX = uSER_IDX;
                        e.CREATE_DT = System.DateTime.Now;
                        ctx.T_QREST_TRAIN_LESSON_USER.Add(e);
                        ctx.SaveChanges();

                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return false;
                }
            }
        }


        public static int DeleteT_QREST_TRAIN_LESSON(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_TRAIN_LESSON rec = new T_QREST_TRAIN_LESSON { LESSON_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        //public static int DeleteT_QREST_TRAIN_LESSON_USER(Guid id)
        //{
        //    using (QRESTEntities ctx = new QRESTEntities())
        //    {
        //        try
        //        {
        //            var lessons = (from a in ctx.T_QREST_TRAIN_LESSON.AsNoTracking()
        //                    where a.LESSON_IDX == lESSON_IDX
        //                    select a).FirstOrDefault();

        //            T_QREST_TRAIN_LESSON rec = new T_QREST_TRAIN_LESSON { LESSON_IDX = id };
        //            ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
        //            ctx.SaveChanges();

        //            return 1;
        //        }
        //        catch (Exception ex)
        //        {
        //            logEF.LogEFException(ex);
        //            return 0;
        //        }
        //    }
        //}


        //************** LESSON STEPS **********************************
        //************** LESSON STEPS **********************************
        //************** LESSON STEPS **********************************

        public static T_QREST_TRAIN_LESSON_STEP GetT_QREST_TRAIN_LESSON_STEP_byID(Guid lESSON_STEP_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_TRAIN_LESSON_STEP.AsNoTracking()
                            where a.LESSON_STEP_IDX == lESSON_STEP_IDX
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_QREST_TRAIN_LESSON_STEP> GetT_QREST_TRAIN_LESSON_STEPS_byLessonID(Guid lESSON_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from a in ctx.T_QREST_TRAIN_LESSON_STEP.AsNoTracking()
                            where a.LESSON_IDX == lESSON_IDX
                            orderby a.LESSON_STEP_SEQ
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static int DeleteT_QREST_TRAIN_LESSON_STEP(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_TRAIN_LESSON_STEP rec = new T_QREST_TRAIN_LESSON_STEP { LESSON_STEP_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }


        public static List<SP_TRAIN_STEP_USER_PROGRESS_Result> SP_TRAIN_STEP_USER_PROGRESS(string uSER_IDX, Guid lESSON_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return ctx.SP_TRAIN_STEP_USER_PROGRESS(uSER_IDX, lESSON_IDX).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static Guid? SP_TRAIN_STEP_USER_PROGRESS_nextStepToComplete(string uSER_IDX, Guid lESSON_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = ctx.SP_TRAIN_STEP_USER_PROGRESS(uSER_IDX, lESSON_IDX).ToList();
                    var yyy = xxx.Where(x => x.STEP_COMP_IND == 0).FirstOrDefault();

                    if (yyy != null)
                        return yyy.LESSON_STEP_IDX;
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static bool SP_TRAIN_STEP_USER_PROGRESS_CompletedLessonCheck(string uSER_IDX, Guid? lESSON_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var xxx = ctx.SP_TRAIN_STEP_USER_PROGRESS(uSER_IDX, lESSON_IDX).ToList();
                    int unfinishedCount = xxx.Where(p => p.STEP_COMP_IND == 0).Count();
                    return (unfinishedCount == 0);
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return false;
                }
            }
        }


        public static Guid? InsertUpdateT_QREST_TRAIN_LESSON_STEP(Guid? lESSON_STEP_IDX, Guid lESSON_IDX, int? lESSON_STEP_SEQ, string lESSON_STEP_DESC, 
            string rEQUIRED_URL, bool? rEQ_CONFIRM, string rEQUIRED_YT_VID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    bool insInd = false;

                    T_QREST_TRAIN_LESSON_STEP e = (from c in ctx.T_QREST_TRAIN_LESSON_STEP
                                                   where c.LESSON_STEP_IDX == lESSON_STEP_IDX
                                                   select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_TRAIN_LESSON_STEP();
                        e.LESSON_STEP_IDX = Guid.NewGuid();
                        e.LESSON_IDX = lESSON_IDX;
                    }

                    if (lESSON_STEP_SEQ != null) e.LESSON_STEP_SEQ = lESSON_STEP_SEQ;
                    if (lESSON_STEP_DESC != null) e.LESSON_STEP_DESC = lESSON_STEP_DESC;
                    if (rEQUIRED_URL != null) e.REQUIRED_URL = rEQUIRED_URL;
                    if (rEQ_CONFIRM != null) e.REQ_CONFIRM = rEQ_CONFIRM;
                    if (rEQUIRED_YT_VID != null) e.REQUIRED_YT_VID = rEQUIRED_YT_VID;


                    if (insInd)
                        ctx.T_QREST_TRAIN_LESSON_STEP.Add(e);

                    ctx.SaveChanges();

                    return e.LESSON_STEP_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static Guid? InsertT_QREST_TRAIN_LESSON_STEP_USER(Guid? lESSON_STEP_IDX, string uSER_IDX, Guid? lESSON_IDX, Guid? cOURSE_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_TRAIN_LESSON_STEP_USER e = (from c in ctx.T_QREST_TRAIN_LESSON_STEP_USER
                                                        where c.LESSON_STEP_IDX == lESSON_STEP_IDX
                                                        && c.USER_IDX == uSER_IDX
                                                        select c).FirstOrDefault();

                    if (e == null)
                    {
                        e = new T_QREST_TRAIN_LESSON_STEP_USER();
                        e.LESSON_STEP_IDX = lESSON_STEP_IDX.GetValueOrDefault();
                        e.USER_IDX = uSER_IDX;
                        e.CREATE_DT = System.DateTime.Now;
                        ctx.T_QREST_TRAIN_LESSON_STEP_USER.Add(e);
                        ctx.SaveChanges();

                        //now see if all steps for this lesson are done - if yes, mark lesson as done
                        if (SP_TRAIN_STEP_USER_PROGRESS_CompletedLessonCheck(uSER_IDX, lESSON_IDX))
                        {
                            bool SuccID = InsertT_QREST_TRAIN_LESSON_USER(lESSON_IDX, uSER_IDX);

                            //now see if all the lessons for this course are done - if yes, mark course as done
                            if (SP_TRAIN_LESSON_USER_PROGRESS_CompletedLessonCheck(uSER_IDX,cOURSE_IDX.GetValueOrDefault()))
                            {
                                SuccID = InsertT_QREST_TRAIN_COURSE_USER(cOURSE_IDX, uSER_IDX);
                            }
                        }

                        return lESSON_STEP_IDX;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }




    }
}
