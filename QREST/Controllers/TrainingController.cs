using Microsoft.AspNet.Identity;
using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QREST.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace QREST.Controllers
{
    public class TrainingController : Controller
    {
        #region CONSTRUCTOR
        private ApplicationUserManager _userManager;

        public TrainingController()
        {
        }

        public TrainingController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        // GET: Training
        public ActionResult Index()
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmTrainingIndex
            {
                courses = db_Train.SP_TRAIN_COURSE_USER_PROGRESS(UserIDX)
            };

            return View(model);
        }


        public ActionResult Course(Guid? id)
        {
            string UserIDX = User.Identity.GetUserId();
            T_QREST_TRAIN_COURSE _course = db_Train.GetT_QREST_TRAIN_COURSE_byID(id.GetValueOrDefault());

            if (_course != null)
            {
                var model = new vmTrainingCourse
                {
                    course = _course,
                    lessons = db_Train.SP_TRAIN_LESSON_USER_PROGRESS(UserIDX, id.GetValueOrDefault())
                };

                return View(model);
            }

            //failed if got this far
            TempData["Error"] = "Unable to load course.";
            return RedirectToAction("Index");
        }



        public ActionResult Lesson(Guid? id)
        {
            string UserIDX = User.Identity.GetUserId();
            T_QREST_TRAIN_LESSON _lesson = db_Train.GetT_QREST_TRAIN_LESSON_byID(id.GetValueOrDefault());
            if (_lesson != null)
            {
                T_QREST_TRAIN_COURSE _course = db_Train.GetT_QREST_TRAIN_COURSE_byLESSONID(_lesson.LESSON_IDX);
                if (_course != null)
                {
                    var model = new vmTrainingLesson
                    {
                        course = _course,
                        lesson = _lesson,
                        steps = db_Train.SP_TRAIN_STEP_USER_PROGRESS(UserIDX, _lesson.LESSON_IDX),
                        compLessonStepIDX = db_Train.SP_TRAIN_STEP_USER_PROGRESS_nextStepToComplete(UserIDX, _lesson.LESSON_IDX)
                    };
                    return View(model);
                }
            }

            //failed if got this far
            TempData["Error"] = "Unable to load lesson.";
            return RedirectToAction("Index");

        }


        [HttpPost]
        public JsonResult LessonReset(Guid? id)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                string UserIDX = User.Identity.GetUserId();

                int succId = db_Train.DeleteT_QREST_TRAIN_LESSON_USER(id ?? Guid.Empty, UserIDX);
                if (succId == 1)
                    return Json("Success");
                else
                    return Json("Unable to reset lesson.");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StepComp(Guid? compLessonStepIDX)
        {
            string UserIDX = User.Identity.GetUserId();
            if (compLessonStepIDX != null)
            {
                //first get lesson, so can return after updated
                T_QREST_TRAIN_LESSON _lesson = db_Train.GetT_QREST_TRAIN_LESSON_byStepID(compLessonStepIDX.GetValueOrDefault());

                //then get course, so can pass on for updating course status if necessary
                T_QREST_TRAIN_COURSE _course = db_Train.GetT_QREST_TRAIN_COURSE_byLESSONID(_lesson.LESSON_IDX);

                //then update the completion of the step
                Guid? SuccID = db_Train.InsertT_QREST_TRAIN_LESSON_STEP_USER(compLessonStepIDX, UserIDX, _lesson.LESSON_IDX, _course.COURSE_IDX);

                return RedirectToAction("Lesson", new { id = _lesson.LESSON_IDX });
            }

            TempData["Error"] = "Unable to record progress";
            return RedirectToAction("Index");
        }


        public ActionResult Certificate(Guid? id)
        {
            string UserIDX = User.Identity.GetUserId();
            var user = UserManager.FindById(UserIDX);

            var model = new vmTrainingCertificate {
                course = db_Train.SP_TRAIN_COURSE_USER_PROGRESS_ByCourse(UserIDX, id.GetValueOrDefault()),
                user_name = user.FNAME + " " + user.LNAME
            };

            if (model.course == null || model.course.COURSE_COMP_IND == 0)
            {
                TempData["Error"] = "Course is not found or not yet completed";
                return RedirectToAction("Index");
            }

            return View(model);
        }


        public async Task<ActionResult> Test()
        {

            //*************STEP 1: SEND POST TO RETRIEVE AUTH TOKEN *************************
            HttpClient client = new HttpClient();
            var values = new Dictionary<string, string>
            {
                { "email", "testperson@yahoo.com" },
                { "password", "test" }
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("http://api.stevens-connect.com/authenticate", content);
            var responseString = await response.Content.ReadAsStringAsync();


            //***************STEP 2: CALL GET ON WEB API  ********************************
            //pseudo code:
            string bearerToken = "extractFromAboveCode";  //get from the json response in responseString above

            //base URL but change URI parameters in the string below with what is wanted
            string myGetUrl = "http://api.stevens-connect.com/project/{project_id}/readings?channel_ids={channel_id,channel_id...}&range_type=relative&start_date=null&end_date=null&minutes=1440&transformation=none";  

            using (var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, myGetUrl))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                var apiResponse = await client.SendAsync(requestMessage);
            }

            return View();
        }
    }
}