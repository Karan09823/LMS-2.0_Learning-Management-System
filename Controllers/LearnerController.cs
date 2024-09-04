using LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS.Controllers
{
    public class LearnerController : Controller
    {
        DbOperation lerOperation = new DbOperation();

        

        /*learner panel*/
        public ActionResult LearnerPanel()
        {
            if (Session["LearnerModel"] == null)
            {
                return RedirectToAction("Welcome", "Home");
            }
            var learnerModel = Session["LearnerModel"] as LearnerModel;

            List<CourseData> Courses = lerOperation.AccessAssignedCourse(learnerModel.LearnerId);
            ViewBag.Courses =Courses;
            ViewBag.Learner = learnerModel;
            return View();
        }

        [HttpPost]
        /*learner logout*/
        public JsonResult LearnerLogout()
        {
            Session.Clear();
            Session.Abandon();

            return Json(new
            {
                success = true,
                message = "Logout successful"
            });


        }

        /*learnr update*/

        public JsonResult UpdateLearner(string LearnerEditName, string LearnerEditPassword)
        {
            var learnerModel = Session["LearnerModel"] as LearnerModel;

            if (learnerModel == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Learner Session Not Found!"
                });
            }

            bool check = lerOperation.LearnerEditDb(LearnerEditName, LearnerEditPassword, learnerModel.LearnerEmail, learnerModel.LearnerPassword);

            if (check)
            {
                learnerModel.LearnerName = LearnerEditName;
                learnerModel.LearnerEmail = learnerModel.LearnerEmail;
                learnerModel.LearnerPassword = LearnerEditPassword;

                Session["LearnerModel"] = learnerModel;

                return Json(new
                {
                    success = true,
                    message = "Updated Successfully"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Something went wrong!"
                });
            }
        }


        public ActionResult ShowCourseListUsingCard(int? courseid, string courseCode, string courseName, string courseDescription)
        {
            if (!courseid.HasValue)
            {
                return RedirectToAction("Welcome", "Home");
            }
            var learnerModel = Session["LearnerModel"] as LearnerModel;
            CourseData courseData = new CourseData
            {
                CourseId = courseid.Value,
                CourseName = courseName,
                CourseDescription = courseDescription,
                CourseCode = courseCode
            };
            if (learnerModel == null && courseData == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            List<ModuleData> moduleData = lerOperation.GetModuleAndSubmodule(courseid.Value);
            ViewBag.modulelist = moduleData;
            ViewBag.learnerModel = learnerModel;
            ViewBag.course = courseData;
            return View(courseData);
        }





    }
}