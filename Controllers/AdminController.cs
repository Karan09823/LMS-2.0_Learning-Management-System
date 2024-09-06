using LMS.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.CompensatingResourceManager;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace LMS.Controllers
{
    public class AdminController : Controller
    {
        DbOperation adminOperation = new DbOperation();

      
        public ActionResult AdminPanel()
        {
            // Check if the session exists
            if (Session["AdminModel"] == null) // Ensure consistent session key usage
            {
                return RedirectToAction("Welcome", "Home");
            }
            // Load the admin panel view
            var adminModel = Session["AdminModel"] as AdminModel;

            int totalCourse = adminOperation.TotalCourseCountDb();
            int totalLearner = adminOperation.TotalLearnerCountDb();
            int totalActiveLearner=adminOperation.TotalActiveLearnerCountDb();
            int totalInstructor = adminOperation.TotalInstructorCountDb();
            List<MasterRecord> record=adminOperation.GetMasterRecords();
            ViewBag.TotalCourse = totalCourse; 
            ViewBag.TotalLearner=totalLearner;
            ViewBag.TotalActiveLearner=totalActiveLearner;
            ViewBag.TotalInstructor = totalInstructor;
            ViewBag.userlist=record;
            ViewBag.AdminModel = adminModel;
            return View();
        }

       
        public ActionResult Admin_Instructor()
        {
            if (Session["AdminModel"] == null)
            {
                return RedirectToAction("Welcome", "Home");
            }


            var adminModel = Session["AdminModel"] as AdminModel;
            ViewBag.AdminModel = adminModel;
            return View();
        }

       
        public ActionResult Admin_Learner()
        {
            if (Session["AdminModel"] == null)
            {
                return RedirectToAction("Welcome", "Home");
            }
            var adminModel = Session["AdminModel"] as AdminModel;

            List<LearnerModel> learnerList = adminOperation.assigningLearnerList();
            List<CourseData> courseDataList = adminOperation.assigningCourseList();
            List<ActiveLearnerModel> activeLearnerModels = adminOperation.GetActiveLearnerList();
            ViewBag.ActiveLearners=activeLearnerModels;
            ViewBag.CourseDataList = courseDataList;
            ViewBag.Assigninglearner=learnerList;
            ViewBag.AdminModel = adminModel;
            return View();
        }

      
        public ActionResult Admin_courses()
        {
            var adminModel = Session["AdminModel"] as AdminModel;
            if (adminModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            List<CourseData> AdminCourses = adminOperation.GetCourseListAdmin();
            ViewBag.Courses = AdminCourses;
            ViewBag.AdminModel = adminModel;
            return View();
        }


        // Update admin details
        [HttpPost]
        public JsonResult UpdateAdmin(string AdminName, string AdminPassword)
        {
            var adminModel = Session["AdminModel"] as AdminModel;
            if (adminModel == null)
            {
                return Json(new { success = false, message = "Admin session not found" });
            }

            // Use the original email and password stored in session for the WHERE clause
            bool check = adminOperation.AdminEditDb(AdminName, AdminPassword, adminModel.Admin_Email, adminModel.Admin_password);
            if (check)
            {
                // Update session with new values
                adminModel.Admin_Name = AdminName;
                adminModel.Admin_password = AdminPassword;
                adminModel.Admin_Email = adminModel.Admin_Email;
                Session["AdminModel"] = adminModel;

                return Json(new
                {
                    success = true,
                    message = "Data Updated Successfully"
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


        // Admin logout logic
        [HttpPost]
        public JsonResult Adminlogout()
        {
            Session.Clear();
            Session.Abandon();

            return Json(new
            {
                success = true,
                message = "Logout successful"
            });
        }


        [HttpPost]
        public JsonResult UpdateUser(int UserId, string UserName, string UserEmail, string UserPassword)
        {
            if (Session["AdminModel"] == null)
            {
                return Json(new { success = false, message = "Session expired" }, JsonRequestBehavior.AllowGet);
            }

            var result = adminOperation.UpdateUserDb(UserId, UserName, UserEmail, UserPassword);

            if (result)
            {
                return Json(new { success = true, message = "User Data Updated successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to Update User" });
            }
        }



       
        [HttpPost]
        public JsonResult DeleteUser(string UserId)
        {
            if (Session["AdminModel"] == null)
            {
                return Json(new { success = false, message = "Session expired" }, JsonRequestBehavior.AllowGet);
            }

            var result = adminOperation.DeleteUserDb(UserId);

            if (result)
            {
                return Json(new { success = true, message = "User Deleted Successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to Delete User" });
            }
        }

        /*add course in admin*/
        public ActionResult AddCourseByAdmin()
        {

            if (Session["AdminModel"] == null)
            {
                return RedirectToAction("Welcome", "Home");
            }
            var adminModel = Session["AdminModel"] as AdminModel;
            ViewBag.AdminModel = adminModel;

            return View();


        }

        public ActionResult AddModuleByAdmin()
        {
            var adminModel = Session["AdminModel"] as AdminModel;
            if (adminModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            // Extract course data from form data
            var courseName = Request.Form["CourseName"];
            var courseDescription = Request.Form["CourseDescription"];
            var courseCode = Request.Form["CourseCode"];
            var courseDuration = Request.Form["CourseDuration"];
            var courseLanguage = Request.Form["CourseLanguage"];
            var courseCategory = Request.Form["CourseCategory"];
            var moduleTitles = Request.Form.GetValues("ModuleTitle[]");

            // Add the course to the database and get the course ID
            int courseId = adminOperation.AddInstructorCourseDb(
                courseName,
                courseCode,
                courseDescription,
                courseDuration,
                courseCategory,
                courseLanguage,
                adminModel.Admin_Email,
                DateTime.Now
            );

            // Add modules to the database
            if (moduleTitles != null && moduleTitles.Length > 0)
            {
                adminOperation.AddModuleDb(courseId, moduleTitles.ToList());
            }

            return Json(new
            {
                success = true,
                message = "Modules added successfully"
            });
        }


        public ActionResult ShowCourseListUsingCard(int? courseid, string courseCode, string courseName, string courseDescription)
        {
            if (!courseid.HasValue)
            {
                return RedirectToAction("Welcome", "Home");
            }
            var adminModel = Session["AdminModel"] as AdminModel;
            CourseData courseData = new CourseData
            {
                CourseId = courseid.Value,
                CourseName = courseName,
                CourseDescription = courseDescription,
                CourseCode = courseCode
            };
            if (adminModel == null && courseData == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            List<ModuleData> moduleData = adminOperation.GetModuleAndSubmodule(courseid.Value);
            ViewBag.modulelist = moduleData;
            ViewBag.AdminModel = adminModel;
            ViewBag.course = courseData;
            return View(courseData);
        }

        public ActionResult UpdateModule(int moduleId, int courseId, string moduleName)
        {

            var adminModel = Session["AdminModel"] as AdminModel;
            if (adminModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            bool check = adminOperation.UpdateCourseModule(moduleId, courseId, moduleName);
            if (check)
            {
                return Json(new
                {
                    success = true,
                    message = "Module Update Successfully"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Something Went Wrong !"
                });
            }


        }

        public ActionResult DeleteModule(int moduleId, int courseId)
        {
            var adminModel = Session["AdminModel"] as AdminModel;
            if (adminModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            bool check = adminOperation.DeleteCourseModule(moduleId, courseId);
            if (check)
            {
                return Json(new
                {
                    success = true,
                    message = "Module deleted Successfully"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Something went wrong !"
                });
            }

        }


        public ActionResult UpdateCourse(int CourseId, string CourseName, string CourseDescription, int CourseDuration, string CourseLanguage, string CourseCategory, string CourseCode)
        {
            var adminModel = Session["AdminModel"] as AdminModel;
            if (adminModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }
            DateTime time = DateTime.Now;
            bool check = adminOperation.UpdateCourseDb(CourseId, CourseName, CourseDescription, CourseDuration, CourseLanguage, CourseCategory, CourseCode, adminModel.Admin_Email, time);
            if (check)
            {
                return Json(new
                {
                    success = true,
                    message = "Course Updated Sucessfully"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Something went wrong !"
                });
            }


        }

        public ActionResult DeleteCourse(int CourseId)
        {
            var adminModel = Session["AdminModel"] as AdminModel;
            if (adminModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }
            bool check = adminOperation.DeleteCourseDb(CourseId);

            if (check)
            {
                return Json(new
                {
                    success = true,
                    message = "Course Deleted Sucessfully"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Something went wrong !"
                });
            }

        }

        public ActionResult AddCourseModuleByAdmin()
        {
            var adminModel = Session["AdminModel"] as AdminModel;
            if (adminModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }
            var courseidString = Request.Form["AddModuleCourseid"];
            var moduleName = Request.Form["AddModuleName"];
            int courseid;
            int.TryParse(courseidString, out courseid);
            bool check = adminOperation.AddCourseModule(courseid, moduleName);

            if (check)
            {
                return Json(new
                {
                    success = true,
                    message = "Modules added successfully"
                });

            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Something Went Wrong !"
                });
            }


        }

        /*add submodules -----------------------------------------------------------------------------------------------------*/
        public ActionResult AddSubmoduleByAdmin()
        {
            var adminModel = Session["AdminModel"] as AdminModel;
            if (adminModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            var courseIdString = Request.Form["CourseId"];
            var moduleIdString = Request.Form["ModuleId"];
            var SubmoduleName = Request.Form["SubModuleName"];

            HttpPostedFileBase submoduleFile = Request.Files["ResourceFile"];

            if (!int.TryParse(courseIdString, out int courseId))
            {
                return Json(new { message = "Invalid Course ID." });
            }

            if (!int.TryParse(moduleIdString, out int moduleId))
            {
                return Json(new { message = "Invalid Module ID." });
            }

            if (submoduleFile != null && submoduleFile.ContentLength > 0)
            {
                string filePath = SaveFileAndGetVirtualPathSubmodule(submoduleFile, HttpContext);

                switch (Path.GetExtension(submoduleFile.FileName).ToLower())
                {
                    case ".jpg":
                    case ".png":
                        adminOperation.AddSubmoduleImg(SubmoduleName, moduleId, courseId, filePath);
                        break;
                    case ".pdf":
                        adminOperation.AddSubmodulePdf(SubmoduleName, moduleId, courseId, filePath);
                        break;
                    case ".mp3":
                    case ".wav":
                        adminOperation.AddSubmoduleAudio(SubmoduleName, moduleId, courseId, filePath);
                        break;
                    case ".mp4":
                        adminOperation.AddSubmoduleVideo(SubmoduleName, moduleId, courseId, filePath);
                        break;
                    case ".txt":
                        adminOperation.AddSubmoduleText(SubmoduleName, moduleId, courseId, filePath);
                        break;
                    default:
                        return Json(new { message = $"Unsupported file type: {Path.GetExtension(submoduleFile.FileName)}" });
                }

                return Json(new { message = "Submodule added successfully!" });
            }

            return Json(new { message = "No file selected or invalid file." });



        }

        private string SaveFileAndGetVirtualPathSubmodule(HttpPostedFileBase fileData, HttpContextBase httpContext)
        {
            if (fileData == null || fileData.ContentLength <= 0)
            {
                throw new ArgumentException("Invalid file data.");
            }

            // Define the path where the file will be saved
            string fileName = Path.GetFileName(fileData.FileName);
            string physicalPath = httpContext.Server.MapPath($"~/Uploads/{fileName}");

            // Save the file
            fileData.SaveAs(physicalPath);

            // Return the virtual path of the saved file
            return $"/Uploads/{fileName}";
        }
        /* -----------------------------------------------------------------------------------------------------*/

        public ActionResult UpdateSubmodule()
        {
            var adminModel = Session["AdminModel"] as AdminModel;
            if (adminModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            var moduleIdString = Request.Form["ModuleId"];
            var submoduleIdString = Request.Form["SubModuleId"];
            var SubmoduleName = Request.Form["SubModuleName"];

            HttpPostedFileBase submoduleFile = Request.Files["SubmoduleFile"];

            if (!int.TryParse(moduleIdString, out int moduleId))
            {
                return Json(new { message = "Invalid Module ID." });
            }
            if (!int.TryParse(submoduleIdString, out int submoduleId))
            {
                return Json(new { message = "Invalid Submodule ID." });
            }

            if (submoduleFile != null && submoduleFile.ContentLength > 0)
            {
                string filePath = SaveFileAndGetVirtualPathSubmodule(submoduleFile, HttpContext);

                switch (Path.GetExtension(submoduleFile.FileName).ToLower())
                {
                    case ".jpg":
                    case ".png":
                        adminOperation.UpdateSubmoduleImg(SubmoduleName, moduleId, submoduleId, filePath);
                        break;
                    case ".pdf":
                        adminOperation.UpdateSubmodulePdf(SubmoduleName, moduleId, submoduleId, filePath);
                        break;
                    case ".mp3":
                    case ".wav":
                        adminOperation.UpdateSubmoduleAudio(SubmoduleName, moduleId, submoduleId, filePath);
                        break;
                    case ".mp4":
                        adminOperation.UpdateSubmoduleVideo(SubmoduleName, moduleId, submoduleId, filePath);
                        break;
                    case ".txt":
                        adminOperation.UpdateSubmoduleText(SubmoduleName, moduleId, submoduleId, filePath);
                        break;
                    default:
                        return Json(new { message = $"Unsupported file type: {Path.GetExtension(submoduleFile.FileName)}" });
                }

                return Json(new { message = "Submodule Updated successfully!" });
            }

            return Json(new { message = "No file selected or invalid file." });
        }

        public ActionResult DeleteSubmodule()
        {

            var adminModel = Session["AdminModel"] as AdminModel;
            if (adminModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            var moduleIdString = Request.Form["moduleId"];
            var submoduleIdString = Request.Form["submoduleId"];
            if (!int.TryParse(moduleIdString, out int moduleId))
            {
                return Json(new { message = "Invalid Module ID." });
            }
            if (!int.TryParse(submoduleIdString, out int submoduleId))
            {
                return Json(new { message = "Invalid Submodule ID." });
            }

            bool check = adminOperation.DeleteSubmoduleDb(moduleId, submoduleId);

            if (check)
            {
                return Json(new
                {
                    success = true,
                    message = "Submodule Deleted Successfully"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Something went wrong !"
                });
            }
        }

        public ActionResult ActivateLearner(int learnerid,int courseid)
        {
            var adminModel = Session["AdminModel"] as AdminModel;
            if (adminModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }
            DateTime time= DateTime.Now;
            bool check=adminOperation.AssignCourse(adminModel.Admin_id,adminModel.Admin_Name,adminModel.Admin_Email,learnerid,courseid,time);
            if (check)
            {
                return Json(new
                {
                    success = true,
                    message = "Course Assigned Successfully"

                });
            }
            else
            {
                return Json(new
                {
                    sucess = false,
                    message = "Something went wrong !"
                });
            }

           
        }

        public ActionResult DeallocateLearner(int learnerid,int courseid,int ownerid)
        {
            var adminModel = Session["AdminModel"] as AdminModel;
            if (adminModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            bool check=adminOperation.DeallocateLearnerDb(learnerid,courseid,ownerid);
            if(check)
            {
                return Json(new
                {
                    success = true,
                    message = "Course Deallocated Successfully"
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

    }
}
