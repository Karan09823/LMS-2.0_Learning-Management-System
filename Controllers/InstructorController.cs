using LMS.Models;
using Newtonsoft.Json;
using System;


using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;



namespace LMS.Controllers
{
    public class InstructorController : Controller
    {
        DbOperation instructOperation = new DbOperation();



        public ActionResult InstructorPanel()
        {
            if (Session["InstructorModel"] == null)
            {
                return RedirectToAction("Welcome", "Home");
            }
            var instructorModel = Session["InstructorModel"] as InstructorModel;

            int totalLearner = instructOperation.TotalLearnerCountDb();
            int totalActiveLearner = instructOperation.TotalInstructorActiveLearnerCountDb(instructorModel.InstructorEmail);
            int totalInstructorCourse = instructOperation.TotalInstructorCourseCountDb(instructorModel.InstructorEmail);
            ViewBag.TotalLearner = totalLearner;
            ViewBag.TotalInstructorActiveLearner = totalActiveLearner;
            ViewBag.TotalInstructorCourses = totalInstructorCourse;

            List<LearnerModel> learnerList = instructOperation.assigningLearnerList();
            List<CourseData> courseDataList = instructOperation.InstructorassigningCourseList(instructorModel.InstructorEmail);
            List<ActiveLearnerModel> activeLearnerModels = instructOperation.GetActiveLearnerList();
            ViewBag.ActiveLearners = activeLearnerModels;
            ViewBag.CourseDataList = courseDataList;
            ViewBag.Assigninglearner = learnerList;
            ViewBag.Instructor = instructorModel;
            return View();
        }
        /*instructor learner*/
        public ActionResult InstructorLearner()
        {
            var instructModel = Session["InstructorModel"] as InstructorModel;
            if (instructModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            return View();
        }

        /*instructor courses*/
        public ActionResult InstructorCourses()
        {
            var instructModel = Session["InstructorModel"] as InstructorModel;
            if (instructModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            List<CourseData> instructorCourses = instructOperation.GetCourseList(instructModel.InstructorEmail);
            ViewBag.courses = instructorCourses;
            ViewBag.Instructor = instructModel;
            return View();


        }

        /*instructor logout*/
        [HttpPost]
        public JsonResult Instructorlogout()
        {
            Session.Clear();
            Session.Abandon();

            return Json(new
            {
                success = true,
                message = "Logout successful"
            });

        }



        private byte[] ConvertToByteArray(HttpPostedFileBase file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.InputStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        // Helper method to save file and get its virtual path
        private string SaveFileAndGetVirtualPath(FileData fileData, HttpContextBase httpContext)
        {
            if (fileData == null || string.IsNullOrEmpty(fileData.FileName) || fileData.Content == null)
            {
                throw new ArgumentException("Invalid file data.");
            }

            // Define the path where the file will be saved
            string physicalPath = httpContext.Server.MapPath($"~/Uploads/{fileData.FileName}");

            // Save the file
            System.IO.File.WriteAllBytes(physicalPath, fileData.Content);

            // Return the virtual path of the saved file
            return $"/Uploads/{fileData.FileName}";
        }




        public ActionResult AddCourse()
        {
            if (Session["InstructorModel"] == null)
            {
                return RedirectToAction("Welcome", "Home");
            }
            var instructorModel = Session["InstructorModel"] as InstructorModel;
            ViewBag.Instructor = instructorModel;
            return View();

        }

        public ActionResult AddModules()
        {
            var instructModel = Session["InstructorModel"] as InstructorModel;
            if (instructModel == null)
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
            int courseId = instructOperation.AddInstructorCourseDb(
                courseName,
                courseCode,
                courseDescription,
                courseDuration,
                courseCategory,
                courseLanguage,
                instructModel.InstructorEmail,
                DateTime.Now
            );

            // Add modules to the database
            if (moduleTitles != null && moduleTitles.Length > 0)
            {
                instructOperation.AddModuleDb(courseId, moduleTitles.ToList());
            }

            return Json(new
            {
                success = true,
                message = "Modules added successfully"
            });
        }

        /*add a particular course module*/
        public ActionResult AddCourseModule()
        {
            var instructModel = Session["InstructorModel"] as InstructorModel;
            if (instructModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }
            var courseidString = Request.Form["AddModuleCourseid"];
            var moduleName = Request.Form["AddModuleName"];
            int courseid;
            int.TryParse(courseidString, out courseid);
            bool check = instructOperation.AddCourseModule(courseid, moduleName);

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

        /*show course modules*/

        public ActionResult ShowCourseListUsingCard(int? courseid, string courseCode, string courseName, string courseDescription)
        {
            if (!courseid.HasValue)
            {
                return RedirectToAction("Welcome", "Home");
            }
            var instructModel = Session["InstructorModel"] as InstructorModel;
            CourseData courseData = new CourseData
            {
                CourseId = courseid.Value,
                CourseName = courseName,
                CourseDescription = courseDescription,
                CourseCode = courseCode // Assuming CourseData has CourseCode property
            };
            if (instructModel == null && courseData == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            List<ModuleData> moduleData = instructOperation.GetModuleAndSubmodule(courseid.Value);
            ViewBag.modulelist = moduleData;
            ViewBag.instructor = instructModel;
            ViewBag.course = courseData;
            return View(courseData);
        }



        /*add submodules -----------------------------------------------------------------------------------------------------*/
        public ActionResult AddSubmodule()
        {
            var instructModel = Session["InstructorModel"] as InstructorModel;
            if (instructModel == null)
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
                        instructOperation.AddSubmoduleImg(SubmoduleName, moduleId, courseId, filePath);
                        break;
                    case ".pdf":
                        instructOperation.AddSubmodulePdf(SubmoduleName, moduleId, courseId, filePath);
                        break;
                    case ".mp3":
                    case ".wav":
                        instructOperation.AddSubmoduleAudio(SubmoduleName, moduleId, courseId, filePath);
                        break;
                    case ".mp4":
                        instructOperation.AddSubmoduleVideo(SubmoduleName, moduleId, courseId, filePath);
                        break;
                    case ".txt":
                        instructOperation.AddSubmoduleText(SubmoduleName, moduleId, courseId, filePath);
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

        public ActionResult UpdateCourse(int CourseId, string CourseName, string CourseDescription, int CourseDuration, string CourseLanguage, string CourseCategory, string CourseCode)
        {
            var instructModel = Session["InstructorModel"] as InstructorModel;
            if (instructModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }
            DateTime time = DateTime.Now;
            bool check = instructOperation.UpdateCourseDb(CourseId, CourseName, CourseDescription, CourseDuration, CourseLanguage, CourseCategory, CourseCode, instructModel.InstructorEmail, time);
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
            var instructModel = Session["InstructorModel"] as InstructorModel;
            if (instructModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }
            bool check = instructOperation.DeleteCourseDb(CourseId);

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

        public ActionResult UpdateModule(int moduleId, int courseId, string moduleName)
        {

            var instructModel = Session["InstructorModel"] as InstructorModel;
            if (instructModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            bool check = instructOperation.UpdateCourseModule(moduleId, courseId, moduleName);
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
            var instructModel = Session["InstructorModel"] as InstructorModel;
            if (instructModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            bool check = instructOperation.DeleteCourseModule(moduleId, courseId);
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

        public ActionResult UpdateSubmodule()
        {
            var instructModel = Session["InstructorModel"] as InstructorModel;
            if (instructModel == null)
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
                        instructOperation.UpdateSubmoduleImg(SubmoduleName, moduleId, submoduleId, filePath);
                        break;
                    case ".pdf":
                        instructOperation.UpdateSubmodulePdf(SubmoduleName, moduleId, submoduleId, filePath);
                        break;
                    case ".mp3":
                    case ".wav":
                        instructOperation.UpdateSubmoduleAudio(SubmoduleName, moduleId, submoduleId, filePath);
                        break;
                    case ".mp4":
                        instructOperation.UpdateSubmoduleVideo(SubmoduleName, moduleId, submoduleId, filePath);
                        break;
                    case ".txt":
                        instructOperation.UpdateSubmoduleText(SubmoduleName, moduleId, submoduleId, filePath);
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

            var instructModel = Session["InstructorModel"] as InstructorModel;
            if (instructModel == null)
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

            bool check = instructOperation.DeleteSubmoduleDb(moduleId, submoduleId);

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


        public ActionResult ActivateLearner(int learnerid, int courseid)
        {
            var instructModel = Session["InstructorModel"] as InstructorModel;
            if (instructModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }
            DateTime time = DateTime.Now;
            bool check = instructOperation.AssignCourse(instructModel.InstructorId, instructModel.InstructorName, instructModel.InstructorEmail, learnerid, courseid, time);
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

        public ActionResult DeallocateLearner(int learnerid, int courseid, int ownerid)
        {
            var instructModel = Session["InstructorModel"] as InstructorModel;
            if (instructModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            bool check = instructOperation.DeallocateLearnerDb(learnerid, courseid, ownerid);
            if (check)
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

        public ActionResult UpdateInstructor(string InstructorNameEdit, string InstructorPasswordEdit)
        {
            var instructModel = Session["InstructorModel"] as InstructorModel;
            if (instructModel == null)
            {
                return RedirectToAction("Welcome", "Home");
            }

            bool check = instructOperation.UpdateInstructorDb(instructModel.InstructorId, InstructorNameEdit, InstructorPasswordEdit);

            if (check)
            {
                return Json(new
                {
                    success = true,
                    message = "Details Updated Successfully"
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

    }


}