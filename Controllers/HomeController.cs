using LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace LMS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /*welcome page */
        public ActionResult Welcome()
        {

            return View();
        }



        /*register*/
        public ActionResult Register()
        {
            return View();
        }

        /*Redirection of controllers*/
        [HttpPost]
        public JsonResult CheckUserRegister(string role, string registername, string registeremail, string registerpassword, string registerkey)
        {
            string resultMessage = string.Empty;
            int specialKey;

            // Validate the role and special key
            if (role == "Instructor")
            {
                specialKey = 222222;
                if (int.TryParse(registerkey, out int key) && key == specialKey)
                {
                    bool registrationSuccessful = operation.InstructorRegisterDb(registername, registeremail, registerpassword,role);
                    if (registrationSuccessful)
                    {
                        resultMessage = "Registration as Instructor is Successful";
                    }
                    else
                    {
                        resultMessage = "Email already registered or registration failed.";
                    }
                }
                else
                {
                    resultMessage = "Invalid Key, Please provide a valid special key!";
                }
            }
            else if (role == "Learner")
            {
                specialKey = 000000;
                if (int.TryParse(registerkey, out int key) && key == specialKey)
                {
                    bool registrationSuccessful = operation.LearnerRegisterDb(registername, registeremail, registerpassword,role);
                    if (registrationSuccessful)
                    {
                        resultMessage = "Registration as Learner is Successful";
                    }
                    else
                    {
                        resultMessage = "Email already registered or registration failed.";
                    }
                }
                else
                {
                    resultMessage = "Invalid Key, Please provide a valid special key!";
                }
            }
            else
            {
                resultMessage = "Invalid Role!";
            }

            return Json(new
            {
                success = !string.IsNullOrEmpty(resultMessage),
                message = resultMessage
            });
        }









        /*DB Operation call*/

        DbOperation operation = new DbOperation();



        [HttpPost]
        public JsonResult CheckLoginProcess(string role, string email, string password)
        {
            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return Json(new
                {
                    success = false,
                    message = "All fields are required."
                });
            }

           /* object userModel = null;*/
            string redirectUrl = string.Empty;

            switch (role)
            {
                case "Admin":
                    var admin = operation.AdminLoginDb(email, password);
                    if (admin != null)
                    {
                        Session["AdminModel"] = admin;
                        redirectUrl = "/Admin/AdminPanel";
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Admin user does not exist or password is incorrect."
                        });
                    }
                    break;

                case "Learner":
                    var learner = operation.LearnerLogignDb(email, password);
                    if (learner != null)
                    {
                        Session["LearnerModel"] = learner;
                        redirectUrl = "/Learner/LearnerPanel";
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Learner user does not exist or password is incorrect."
                        });
                    }
                    break;

                case "Instructor":
                    var instructor = operation.InstructorLoginDb(email, password);
                    if (instructor != null)
                    {
                        Session["InstructorModel"] = instructor;
                        redirectUrl = "/Instructor/InstructorPanel";
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Instructor user does not exist or password is incorrect."
                        });
                    }
                    break;

                default:
                    return Json(new
                    {
                        success = false,
                        message = "Invalid role selected."
                    });
            }

            return Json(new
            {
                success = true,
                redirectUrl = redirectUrl
            });
        }



    }
}