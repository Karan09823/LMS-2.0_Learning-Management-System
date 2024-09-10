using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Antlr.Runtime.Tree;
using LMS.Models;

namespace LMS.Models
{
    public class DbOperation
    {
        SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["LMS"].ConnectionString);




        /*instructor register Dboperation*/
        public bool InstructorRegisterDb(string name, string email, string password, string role)
        {
            string checkEmailQuery = "SELECT COUNT(*) FROM User_table WHERE UserEmail = @Email and UserRole=@Role";
            string insertQuery = "INSERT INTO User_table (UserName, UserEmail, UserPassword,UserRole) VALUES (@Name, @Email, @Password,@Role)";

            using (SqlCommand checkCmd = new SqlCommand(checkEmailQuery, Con))
            {
                checkCmd.Parameters.AddWithValue("@Email", email);
                checkCmd.Parameters.AddWithValue("@Role", role);

                try
                {
                    Con.Open();
                    int emailCount = (int)checkCmd.ExecuteScalar();
                    if (emailCount > 0)
                    {
                        // Email already exists
                        return false;
                    }

                    // Email does not exist, proceed to insert
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, Con))
                    {
                        insertCmd.Parameters.AddWithValue("@Name", name);
                        insertCmd.Parameters.AddWithValue("@Email", email);
                        insertCmd.Parameters.AddWithValue("@Password", password);
                        insertCmd.Parameters.AddWithValue("@Role", role);

                        int rowsAffected = insertCmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    // Log exception
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    Con.Close();
                }
            }

        }




        /*learner register operation*/
        public bool LearnerRegisterDb(string name, string email, string password, string role)
        {
            string checkEmailQuery = "SELECT COUNT(*) FROM User_table WHERE UserEmail = @Email and UserRole=@Role";
            string query = "insert into User_table (UserName,UserEmail,UserPassword,UserRole) values(@Name,@Email,@Password,@Role)";

            using (SqlCommand checkCmd = new SqlCommand(checkEmailQuery, Con))
            {
                checkCmd.Parameters.AddWithValue("@Email", email);
                checkCmd.Parameters.AddWithValue("@Role", role);

                try
                {


                    Con.Open();
                    int emailCount = (int)checkCmd.ExecuteScalar();
                    if (emailCount > 0)
                    {
                        // Email already exists
                        return false;
                    }

                    // Email does not exist, proceed to insert
                    using (SqlCommand insertCmd = new SqlCommand(query, Con))
                    {
                        insertCmd.Parameters.AddWithValue("@Name", name);
                        insertCmd.Parameters.AddWithValue("@Email", email);
                        insertCmd.Parameters.AddWithValue("@Password", password);
                        insertCmd.Parameters.AddWithValue("@Role", role);

                        int rowsAffected = insertCmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }

                }
                catch (Exception ex)
                {
                    // Log exception
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    Con.Close();
                }
            }
        }


        /*admin login process db operation*/
        public AdminModel AdminLoginDb(string emailId, string password)
        {

            string query = "select * from User_table where UserEmail=@Email and UserPassword=@Password";

            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@Email", emailId);
                cmd.Parameters.AddWithValue("@Password", password);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    AdminModel admin = new AdminModel();

                    int adminId = Convert.ToInt32(dt.Rows[0]["UserID"]);
                    admin.Admin_id = adminId;
                    admin.Admin_Name = Convert.ToString(dt.Rows[0]["UserName"]);
                    admin.Admin_Email = Convert.ToString(dt.Rows[0]["UserEmail"]);
                    admin.Admin_password = Convert.ToString(dt.Rows[0]["Userpassword"]);
                    admin.Admin_role = Convert.ToString(dt.Rows[0]["UserRole"]);

                    return admin;
                }
                else
                {
                    return null;
                }


            }

        }

        /*Instructor login db operation*/

        public InstructorModel InstructorLoginDb(string emailId, string password)
        {

            string query = "select * from User_table where UserEmail=@Email and UserPassword=@Password";

            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@Email", emailId);
                cmd.Parameters.AddWithValue("@Password", password);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    InstructorModel instructor = new InstructorModel();

                    int instruct = Convert.ToInt32(dt.Rows[0]["UserID"]);

                    instructor.InstructorId = instruct;
                    instructor.InstructorName = Convert.ToString(dt.Rows[0]["UserName"]);
                    instructor.InstructorEmail = Convert.ToString(dt.Rows[0]["UserEmail"]);
                    instructor.InstructorPassword = Convert.ToString(dt.Rows[0]["UserPassword"]);
                    instructor.InstructorRole = Convert.ToString(dt.Rows[0]["UserRole"]);

                    return instructor;
                }
                else
                {
                    return null;
                }
            }

        }

        /*learner login dboperation*/

        public LearnerModel LearnerLogignDb(string emailId, string password)
        {
            string query = "select * from User_table where UserEmail=@Email and UserPassword=@Password";

            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@Email", emailId);
                cmd.Parameters.AddWithValue("@Password", password);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    LearnerModel lear = new LearnerModel();

                    int learnerid = Convert.ToInt32(dt.Rows[0]["UserID"]);
                    lear.LearnerId = learnerid;
                    lear.LearnerName = Convert.ToString(dt.Rows[0]["UserName"]);
                    lear.LearnerEmail = Convert.ToString(dt.Rows[0]["UserEmail"]);
                    lear.LearnerPassword = Convert.ToString(dt.Rows[0]["UserPassword"]);
                    lear.LearnerRole = Convert.ToString(dt.Rows[0]["UserRole"]);

                    return lear;

                }
                else
                {
                    return null;
                }

            }


        }

        /*admin modal edit db operation*/

        public bool AdminEditDb(string name, string password, string originalEmail, string originalPassword)
        {
            string query = "update User_table set UserName=@Name,UserPassword=@Password " +
                           "where UserEmail=@OriginalEmail and UserPassword=@OriginalPassword";

            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@OriginalEmail", originalEmail);
                cmd.Parameters.AddWithValue("@OriginalPassword", originalPassword);

                try
                {
                    Con.Open();
                    int m = cmd.ExecuteNonQuery();
                    return m > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    Con.Close();
                }
            }
        }



        /*learner edit modal db operation*/

        public bool LearnerEditDb(string name, string password, string originalemail, string originalpassword)
        {
            string query = "UPDATE User_table SET UserName = @Name,UserPassword = @Password WHERE UserEmail = @OriginEmail AND UserPassword = @OriginPassword";

            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@Name", name);

                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@OriginEmail", originalemail);
                cmd.Parameters.AddWithValue("@OriginPassword", originalpassword);

                try
                {
                    Con.Open();
                    int m = cmd.ExecuteNonQuery();
                    return m > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    Con.Close();
                }
            }
        }



        /*admin learner update*/
        public bool UpdateUserDb(int id, string name, string email, string password)
        {
            string query = "UPDATE User_table SET UserName = @Name, UserEmail = @Email, UserPassword = @Password WHERE UserID = @Id";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                try
                {
                    Con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    Con.Close();
                }
            }
        }


        public bool DeleteUserDb(string id)
        {
            string query = "DELETE FROM User_table WHERE UserID = @Id";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@Id", id);


                try
                {
                    Con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        /*instructor add into course table db operation*/


        public int AddInstructorCourseDb(string courseName, string courseCode, string description, string duration, string category, string language, string createdBy, DateTime creationDateTime,string creatorName)
        {
            string insertCourseQuery = @"
                    INSERT INTO Courses (CourseName, CourseCode, Description, Duration, Category, Language, CreatedBy, CreatedOn,CreatorName)
                    VALUES (@CourseName, @CourseCode, @description, @duration, @category, @language, @CreatedBy, @CreationDate,@creatorname);
                    SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlCommand cmd = new SqlCommand(insertCourseQuery, Con))
                {
                    cmd.Parameters.AddWithValue("@CourseName", courseName);
                    cmd.Parameters.AddWithValue("@CourseCode", courseCode);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@duration", duration);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@language", language);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                    cmd.Parameters.AddWithValue("@CreationDate", creationDateTime);
                    cmd.Parameters.AddWithValue("@creatorname", creatorName);

                    Con.Open();

                    // Execute the command and get the newly inserted CourseID
                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int courseId))
                    {
                        return courseId; // Return the newly inserted CourseID
                    }
                    else
                    {
                        return -1; // Indicates failure
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1; // Indicates failure
            }
            finally
            {
                Con.Close();
            }
        }


        /*add module by Instructor in module table*/
        public void AddModuleDb(int courseId, List<string> moduleNames)
        {
            string insertModule = "INSERT INTO Module(ModuleName, CourseID) VALUES(@moduleName, @CourseId);";

            using (var con = Con)
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand(insertModule, con, transaction))
                        {
                            cmd.Parameters.Add("@moduleName", SqlDbType.NVarChar);
                            cmd.Parameters.Add("@CourseId", SqlDbType.Int).Value = courseId;

                            foreach (var moduleName in moduleNames)
                            {
                                cmd.Parameters["@moduleName"].Value = moduleName;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /*add a particular course module*/
        public bool AddCourseModule(int courseid, string modulename)
        {
            string query = "Insert into Module(ModuleName,CourseID) values(@ModuleName,@CourseId)";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@ModuleName", modulename);
                cmd.Parameters.AddWithValue("@CourseId", courseid);
                try
                {
                    Con.Open();
                    int re = cmd.ExecuteNonQuery();
                    return re > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally { Con.Close(); }

            }
        }


        /*add submodule with img file present*/
        public bool AddSubmoduleImg(string submodulename, int linkedmoduleid, int linkedcourseid, string imgvirtualpath)
        {
            string insertwithImg = "Insert into SubModule(SubModuleName,ModuleID,CourseID,ImageFilePath) values(@subMname,@linkedModuleid,@linkedCourseid,@imgVirPath)";
            using (SqlCommand cmd = new SqlCommand(insertwithImg, Con))
            {
                cmd.Parameters.AddWithValue("@subMname", submodulename);
                cmd.Parameters.AddWithValue("@linkedModuleid", linkedmoduleid);
                cmd.Parameters.AddWithValue("@linkedCourseid", linkedcourseid);
                cmd.Parameters.AddWithValue("@imgVirPath", imgvirtualpath);
                try
                {
                    Con.Open();
                    int re = cmd.ExecuteNonQuery();
                    return re > 0 ? true : false;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally { Con.Close(); }

            }
        }
        /*add submodule with pdf file present*/

        public bool AddSubmodulePdf(string submodulename, int linkedmoduleid, int linkedcourseid, string pdfvirtualpath)
        {
            string insertwithPdf = "Insert into SubModule(SubModuleName,ModuleID,CourseID,PdfFilePath) values(@subMname,@linkedModuleid,@linkedCourseid,@pdfVirPath)";
            using (SqlCommand cmd = new SqlCommand(insertwithPdf, Con))
            {
                cmd.Parameters.AddWithValue("@subMname", submodulename);
                cmd.Parameters.AddWithValue("@linkedModuleid", linkedmoduleid);
                cmd.Parameters.AddWithValue("@linkedCourseid", linkedcourseid);
                cmd.Parameters.AddWithValue("@pdfVirPath", pdfvirtualpath);
                try
                {
                    Con.Open();
                    int re = cmd.ExecuteNonQuery();
                    return re > 0 ? true : false;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally { Con.Close(); }

            }
        }

        /*add submodule with audio file present*/
        public bool AddSubmoduleAudio(string submodulename, int linkedmoduleid, int linkedcourseid, string audiovirtualpath)
        {
            string insertwithAudio = "Insert into SubModule(SubModuleName,ModuleID,CourseID,AudioFilePath) values(@subMname,@linkedModuleid,@linkedCourseid,@imgVirPath)";
            using (SqlCommand cmd = new SqlCommand(insertwithAudio, Con))
            {
                cmd.Parameters.AddWithValue("@subMname", submodulename);
                cmd.Parameters.AddWithValue("@linkedModuleid", linkedmoduleid);
                cmd.Parameters.AddWithValue("@linkedCourseid", linkedcourseid);
                cmd.Parameters.AddWithValue("@audioVirPath", audiovirtualpath);
                try
                {
                    Con.Open();
                    int re = cmd.ExecuteNonQuery();
                    return re > 0 ? true : false;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally { Con.Close(); }

            }
        }

        /*add submodule with video file present*/
        public bool AddSubmoduleVideo(string submodulename, int linkedmoduleid, int linkedcourseid, string videovirtualpath)
        {
            string insertwithVideo = "Insert into SubModule(SubModuleName,ModuleID,CourseID,VideoFilePath) values(@subMname,@linkedModuleid,@linkedCourseid,@videoVirPath)";
            using (SqlCommand cmd = new SqlCommand(insertwithVideo, Con))
            {
                cmd.Parameters.AddWithValue("@subMname", submodulename);
                cmd.Parameters.AddWithValue("@linkedModuleid", linkedmoduleid);
                cmd.Parameters.AddWithValue("@linkedCourseid", linkedcourseid);
                cmd.Parameters.AddWithValue("@videoVirPath", videovirtualpath);
                try
                {
                    Con.Open();
                    int re = cmd.ExecuteNonQuery();
                    return re > 0 ? true : false;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally { Con.Close(); }

            }
        }

        /*add submodule with text file*/
        public bool AddSubmoduleText(string submodulename, int linkedmoduleid, int linkedcourseid, string txtvirtualpath)
        {
            string insertwithText = "Insert into SubModule(SubModuleName,ModuleID,CourseID,TextFilePath) values(@subMname,@linkedModuleid,@linkedCourseid,@txtVirPath)";
            using (SqlCommand cmd = new SqlCommand(insertwithText, Con))
            {
                cmd.Parameters.AddWithValue("@subMname", submodulename);
                cmd.Parameters.AddWithValue("@linkedModuleid", linkedmoduleid);
                cmd.Parameters.AddWithValue("@linkedCourseid", linkedcourseid);
                cmd.Parameters.AddWithValue("@txtVirPath", txtvirtualpath);
                try
                {
                    Con.Open();
                    int re = cmd.ExecuteNonQuery();
                    return re > 0 ? true : false;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally { Con.Close(); }

            }
        }

        /*View courses card*/

        public List<CourseData> GetCourseList(string InstructorEmailID)
        {
            List<CourseData> InstructorCourseList = new List<CourseData>();
            string query = "Select CourseID,CourseName,CourseCode,Description,Category,Duration,Language from Courses where CreatedBy=@EmailId";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@EmailId", InstructorEmailID);

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    InstructorCourseList.Add(new CourseData()
                    {
                        CourseId = Convert.ToInt32(dr["CourseID"]),
                        CourseName = Convert.ToString(dr["CourseName"]),
                        CourseCode = Convert.ToString(dr["CourseCode"]),
                        CourseDescription = Convert.ToString(dr["Description"]),
                        CourseCategory = Convert.ToString(dr["Category"]),
                        CourseDuration = Convert.ToInt32(dr["Duration"]),
                        CourseLanguage = Convert.ToString(dr["Language"])

                    });
                }
            }
            return InstructorCourseList;
        }

        /*get course modules */
        public List<ModuleData> GetCourseModuleList(int courseid)
        {
            List<ModuleData> ModuleList = new List<ModuleData>();
            string query = "Select ModuleID,ModuleName from Module where CourseID=@courseid";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@courseid", courseid);
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    ModuleList.Add(new ModuleData()
                    {
                        ModuleId = Convert.ToInt32(dr["ModuleID"]),
                        ModuleName = Convert.ToString(dr["ModuleName"])
                    });
                }
            }
            return ModuleList;
        }

        /*get module and submodule lists----------------------------------*/

        public List<ModuleData> GetModuleAndSubmodule(int courseid)
        {

            List<ModuleData> resourceList = new List<ModuleData>();


            string query = @"
        SELECT m.ModuleID, m.ModuleName, 
               s.SubModuleID, s.SubModuleName,
               s.ImageFilePath, s.PdfFilePath,
               s.AudioFilePath, s.VideoFilePath, s.TextFilePath
        FROM Module m
        LEFT JOIN SubModule s ON m.ModuleID = s.ModuleID
        WHERE m.CourseID = @CourseId";


            var moduleDict = new Dictionary<int, ModuleData>();


            using (SqlCommand cmd = new SqlCommand(query, Con))
            {

                cmd.Parameters.AddWithValue("@CourseId", courseid);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);


                foreach (DataRow dr in dt.Rows)
                {
                    int moduleId = Convert.ToInt32(dr["ModuleID"]);

                    // Check if the module already exists in the dictionary
                    if (!moduleDict.ContainsKey(moduleId))
                    {
                        // Create a new ModuleData instance and add it to the dictionary
                        moduleDict[moduleId] = new ModuleData
                        {
                            ModuleId = moduleId,
                            ModuleName = Convert.ToString(dr["ModuleName"]),
                            SubModules = new List<SubModuleData>()
                        };
                    }

                    // Only add submodules if they exist
                    if (dr["SubModuleID"] != DBNull.Value)
                    {

                        var fileUrl = Convert.ToString(dr["VideoFilePath"]);



                        var subModuleData = new SubModuleData
                        {
                            SubModuleId = Convert.ToInt32(dr["SubModuleID"]),
                            SubmoduleName = Convert.ToString(dr["SubModuleName"]),
                            /*FileUrl = Convert.ToString(dr["VideoFilePath"])*/
                            FileUrl = GetFirstNonEmptyFileUrl(dr)


                        };
                        Console.WriteLine(subModuleData.FileUrl);
                        Console.WriteLine(subModuleData.SubmoduleName);
                        // Add the submodule to the corresponding module
                        moduleDict[moduleId].SubModules.Add(subModuleData);
                    }
                }

                // Populate the final list with all module data
                resourceList = moduleDict.Values.ToList();
            }

            return resourceList;
        }

        private string GetFirstNonEmptyFileUrl(DataRow dr)
        {
            var filePaths = new[]
            {
        dr["TextFilePath"],
        dr["PdfFilePath"],
        dr["AudioFilePath"],
        dr["VideoFilePath"],
        dr["ImageFilePath"]
    };

            foreach (var filePath in filePaths)
            {
                var pathString = Convert.ToString(filePath);
                if (!string.IsNullOrEmpty(pathString))
                {
                    return pathString;
                }
            }

            return string.Empty;
        }

        public List<CourseData> GetCourseListAdmin()
        {
            List<CourseData> AdminCourseList = new List<CourseData>();
            string query = "Select CourseID,CourseName,CourseCode,Description,Category,Duration,Language from Courses";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {


                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    AdminCourseList.Add(new CourseData()
                    {
                        CourseId = Convert.ToInt32(dr["CourseID"]),
                        CourseName = Convert.ToString(dr["CourseName"]),
                        CourseCode = Convert.ToString(dr["CourseCode"]),
                        CourseDescription = Convert.ToString(dr["Description"]),
                        CourseCategory = Convert.ToString(dr["Category"]),
                        CourseDuration = Convert.ToInt32(dr["Duration"]),
                        CourseLanguage = Convert.ToString(dr["Language"])

                    });
                }
            }
            return AdminCourseList;
        }

        /*update course module*/
        public bool UpdateCourseModule(int moduleId, int courseid, string modulename)
        {
            string query = "update Module set ModuleName=@modulename where ModuleID=@moduleid and CourseID=@courseid";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@modulename", modulename);
                cmd.Parameters.AddWithValue("@courseid", courseid);
                cmd.Parameters.AddWithValue("@moduleid", moduleId);

                try
                {
                    Con.Open();
                    int re = cmd.ExecuteNonQuery();
                    return re > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally { Con.Close(); }
            }
        }

        public bool DeleteCourseModule(int moduleId, int courseId)
        {
            string query = "Delete from  Module where ModuleID=@moduleid and CourseID=@courseid";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@moduleid", moduleId);
                cmd.Parameters.AddWithValue("@courseid", courseId);

                try
                {
                    Con.Open();
                    int re = cmd.ExecuteNonQuery();
                    return re > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    Con.Close();

                }
            }
        }


        public bool UpdateCourseDb(int CourseId, string CourseName, string CourseDescription, int CourseDuration, string CourseLanguage, string CourseCategory, string CourseCode, string OwnerEmail, DateTime time)
        {
            string query = "Update Courses set CourseName=@coursename,CourseCode=@coursecode,Description=@description,Duration=@duration,Category=@category,Language=@language,UpdatedBy=@owner,UpdatedOn=@time Where CourseID=@courseid";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@coursename", CourseName);
                cmd.Parameters.AddWithValue("@coursecode", CourseCode);
                cmd.Parameters.AddWithValue("@description", CourseDescription);
                cmd.Parameters.AddWithValue("@duration", CourseDuration);
                cmd.Parameters.AddWithValue("@category", CourseCategory);
                cmd.Parameters.AddWithValue("@language", CourseLanguage);
                cmd.Parameters.AddWithValue("@owner", OwnerEmail);
                cmd.Parameters.AddWithValue("@time", time);
                cmd.Parameters.AddWithValue("@courseid", CourseId);
                try
                {
                    Con.Open();
                    int re = cmd.ExecuteNonQuery();
                    return re > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public bool DeleteCourseDb(int CourseId)
        {
            string query = "Delete from Courses where CourseID=@courseid";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@courseid", CourseId);
                try
                {
                    Con.Open();
                    int re = cmd.ExecuteNonQuery();
                    return re > 0 ? true : false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public bool UpdateSubmoduleImg(string submodulename, int linkedmoduleid, int linkedsubmoduleid, string imgvirtualpath)
        {

            string refreshQuery = @"
        UPDATE SubModule 
        SET ImageFilePath=NULL, PdfFilePath=NULL, AudioFilePath=NULL, VideoFilePath=NULL, TextFilePath=NULL 
        WHERE SubModuleID=@submoduleid AND ModuleID=@moduleid";

            string insertwithImg = @"
        UPDATE SubModule 
        SET SubModuleName=@submodulename, ImageFilePath=@imgfile 
        WHERE ModuleID=@moduleid AND SubModuleID=@submoduleid";

            try
            {

                Con.Open();
                using (SqlCommand refreshCmd = new SqlCommand(refreshQuery, Con))
                {
                    refreshCmd.Parameters.AddWithValue("@submoduleid", linkedsubmoduleid);
                    refreshCmd.Parameters.AddWithValue("@moduleid", linkedmoduleid);
                    refreshCmd.ExecuteNonQuery();
                }

                using (SqlCommand updateCmd = new SqlCommand(insertwithImg, Con))
                {
                    updateCmd.Parameters.AddWithValue("@submoduleid", linkedsubmoduleid);
                    updateCmd.Parameters.AddWithValue("@moduleid", linkedmoduleid);
                    updateCmd.Parameters.AddWithValue("@submodulename", submodulename);
                    updateCmd.Parameters.AddWithValue("@imgfile", imgvirtualpath);

                    int rowsAffected = updateCmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {

                Con.Close();
            }
        }

        public bool UpdateSubmodulePdf(string submodulename, int linkedmoduleid, int linkedsubmoduleid, string pdfvirtualpath)
        {

            string refreshQuery = @"
        UPDATE SubModule 
        SET ImageFilePath=NULL, PdfFilePath=NULL, AudioFilePath=NULL, VideoFilePath=NULL, TextFilePath=NULL 
        WHERE SubModuleID=@submoduleid AND ModuleID=@moduleid";

            string insertwithImg = @"
        UPDATE SubModule 
        SET SubModuleName=@submodulename, PdfFilePath=@pdffile 
        WHERE ModuleID=@moduleid AND SubModuleID=@submoduleid";

            try
            {

                Con.Open();
                using (SqlCommand refreshCmd = new SqlCommand(refreshQuery, Con))
                {
                    refreshCmd.Parameters.AddWithValue("@submoduleid", linkedsubmoduleid);
                    refreshCmd.Parameters.AddWithValue("@moduleid", linkedmoduleid);
                    refreshCmd.ExecuteNonQuery();
                }

                using (SqlCommand updateCmd = new SqlCommand(insertwithImg, Con))
                {
                    updateCmd.Parameters.AddWithValue("@submoduleid", linkedsubmoduleid);
                    updateCmd.Parameters.AddWithValue("@moduleid", linkedmoduleid);
                    updateCmd.Parameters.AddWithValue("@submodulename", submodulename);
                    updateCmd.Parameters.AddWithValue("@pdffile", pdfvirtualpath);

                    int rowsAffected = updateCmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {

                Con.Close();
            }
        }

        public bool UpdateSubmoduleAudio(string submodulename, int linkedmoduleid, int linkedsubmoduleid, string audiovirtualpath)
        {

            string refreshQuery = @"
        UPDATE SubModule 
        SET ImageFilePath=NULL, PdfFilePath=NULL, AudioFilePath=NULL, VideoFilePath=NULL, TextFilePath=NULL 
        WHERE SubModuleID=@submoduleid AND ModuleID=@moduleid";

            string insertwithImg = @"
        UPDATE SubModule 
        SET SubModuleName=@submodulename, AudioFilePath=@audiofile 
        WHERE ModuleID=@moduleid AND SubModuleID=@submoduleid";

            try
            {

                Con.Open();
                using (SqlCommand refreshCmd = new SqlCommand(refreshQuery, Con))
                {
                    refreshCmd.Parameters.AddWithValue("@submoduleid", linkedsubmoduleid);
                    refreshCmd.Parameters.AddWithValue("@moduleid", linkedmoduleid);
                    refreshCmd.ExecuteNonQuery();
                }

                using (SqlCommand updateCmd = new SqlCommand(insertwithImg, Con))
                {
                    updateCmd.Parameters.AddWithValue("@submoduleid", linkedsubmoduleid);
                    updateCmd.Parameters.AddWithValue("@moduleid", linkedmoduleid);
                    updateCmd.Parameters.AddWithValue("@submodulename", submodulename);
                    updateCmd.Parameters.AddWithValue("@audiofile", audiovirtualpath);

                    int rowsAffected = updateCmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {

                Con.Close();
            }
        }

        public bool UpdateSubmoduleVideo(string submodulename, int linkedmoduleid, int linkedsubmoduleid, string videovirtualpath)
        {

            string refreshQuery = @"
        UPDATE SubModule 
        SET ImageFilePath=NULL, PdfFilePath=NULL, AudioFilePath=NULL, VideoFilePath=NULL, TextFilePath=NULL 
        WHERE SubModuleID=@submoduleid AND ModuleID=@moduleid";

            string insertwithImg = @"
        UPDATE SubModule 
        SET SubModuleName=@submodulename, VideoFilePath=@videofile 
        WHERE ModuleID=@moduleid AND SubModuleID=@submoduleid";

            try
            {

                Con.Open();
                using (SqlCommand refreshCmd = new SqlCommand(refreshQuery, Con))
                {
                    refreshCmd.Parameters.AddWithValue("@submoduleid", linkedsubmoduleid);
                    refreshCmd.Parameters.AddWithValue("@moduleid", linkedmoduleid);
                    refreshCmd.ExecuteNonQuery();
                }

                using (SqlCommand updateCmd = new SqlCommand(insertwithImg, Con))
                {
                    updateCmd.Parameters.AddWithValue("@submoduleid", linkedsubmoduleid);
                    updateCmd.Parameters.AddWithValue("@moduleid", linkedmoduleid);
                    updateCmd.Parameters.AddWithValue("@submodulename", submodulename);
                    updateCmd.Parameters.AddWithValue("@videofile", videovirtualpath);

                    int rowsAffected = updateCmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {

                Con.Close();
            }
        }

        public bool UpdateSubmoduleText(string submodulename, int linkedmoduleid, int linkedsubmoduleid, string textvirtualpath)
        {

            string refreshQuery = @"
        UPDATE SubModule s
        SET ImageFilePath=NULL, PdfFilePath=NULL, AudioFilePath=NULL, VideoFilePath=NULL, TextFilePath=NULL 
        WHERE SubModuleID=@submoduleid AND ModuleID=@moduleid";

            string insertwithImg = @"
        UPDATE SubModule 
        SET SubModuleName=@submodulename, TextFilePath=@textfile 
        WHERE ModuleID=@moduleid AND SubModuleID=@submoduleid";

            try
            {

                Con.Open();
                using (SqlCommand refreshCmd = new SqlCommand(refreshQuery, Con))
                {
                    refreshCmd.Parameters.AddWithValue("@submoduleid", linkedsubmoduleid);
                    refreshCmd.Parameters.AddWithValue("@moduleid", linkedmoduleid);
                    refreshCmd.ExecuteNonQuery();
                }

                using (SqlCommand updateCmd = new SqlCommand(insertwithImg, Con))
                {
                    updateCmd.Parameters.AddWithValue("@submoduleid", linkedsubmoduleid);
                    updateCmd.Parameters.AddWithValue("@moduleid", linkedmoduleid);
                    updateCmd.Parameters.AddWithValue("@submodulename", submodulename);
                    updateCmd.Parameters.AddWithValue("@textfile", textvirtualpath);

                    int rowsAffected = updateCmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {

                Con.Close();
            }
        }

        public bool DeleteSubmoduleDb(int moduleid, int submoduleid)
        {
            string query = "Delete from SubModule where ModuleID=@moduleid and SubModuleID=@submoduleid";
            using (SqlCommand deleteCmd = new SqlCommand(query, Con))
            {
                deleteCmd.Parameters.AddWithValue("@moduleid", moduleid);
                deleteCmd.Parameters.AddWithValue("@submoduleid", submoduleid);

                try
                {
                    Con.Open();
                    int re = deleteCmd.ExecuteNonQuery();
                    return re > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public List<MasterRecord> GetMasterRecords(int page, int pageSize)
        {
            List<MasterRecord> userList = new List<MasterRecord>();
            int offset = (page - 1) * pageSize;
            string query = "SELECT UserID, UserName, UserEmail, UserPassword, UserRole " +
               "FROM User_table WHERE UserRole IN ('Learner', 'Instructor') " +
               "ORDER BY UserID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@Offset", offset);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    userList.Add(new MasterRecord()
                    {
                        UserId = Convert.ToInt32(dr["UserID"]),
                        UserName = Convert.ToString(dr["UserName"]),
                        UserEmail = Convert.ToString(dr["UserEmail"]),
                        UserPassword = Convert.ToString(dr["UserPassword"]),
                        UserRole = Convert.ToString(dr["UserRole"])
                    });
                }
            }

            return userList;
        }

        public List<LearnerModel> assigningLearnerList()
        {
            List<LearnerModel> learnerList = new List<LearnerModel>();
            string query = "select UserID,UserEmail,UserName from User_table where UserRole='Learner'";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    learnerList.Add(new LearnerModel()
                    {
                        LearnerId = Convert.ToInt32(dr["UserID"]),
                        LearnerEmail = Convert.ToString(dr["UserEmail"]),
                        LearnerName = Convert.ToString(dr["UserName"]),
                    });
                }
            }
            return learnerList;
        }

        public List<CourseData> assigningCourseList()
        {
            List<CourseData> courseList = new List<CourseData>();
            string query = "select CourseID,CourseName,CourseCode from Courses";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    courseList.Add(new CourseData()
                    {
                        CourseId = Convert.ToInt32(dr["CourseID"]),
                        CourseName = Convert.ToString(dr["CourseName"]),
                        CourseCode = Convert.ToString(dr["CourseCode"]),

                    });
                }

            }

            return courseList;
        }

        public List<CourseData> InstructorassigningCourseList(string instructoremail)
        {
            List<CourseData> courseList = new List<CourseData>();
            string query = "select CourseID,CourseName,CourseCode from Courses where CreatedBy=@instructoremail";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@instructoremail", instructoremail);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    courseList.Add(new CourseData()
                    {
                        CourseId = Convert.ToInt32(dr["CourseID"]),
                        CourseName = Convert.ToString(dr["CourseName"]),
                        CourseCode = Convert.ToString(dr["CourseCode"]),

                    });
                }

            }

            return courseList;
        }

        public bool AssignCourse(int ownerid, string ownername, string owneremail, int learnerid, int courseid, DateTime time)
        {
            string query = "Insert into LearnerCourse(learner_id,course_id,assigned_by,assigned_owner_name,assigned_owner_email,assignment_date) values(@learnerid,@courseid,@ownerid,@ownername,@owneremail,@time)";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@learnerid", learnerid);
                cmd.Parameters.AddWithValue("@courseid", courseid);
                cmd.Parameters.AddWithValue("@ownerid", ownerid);
                cmd.Parameters.AddWithValue("@ownername", ownername);
                cmd.Parameters.AddWithValue("@owneremail", owneremail);
                cmd.Parameters.AddWithValue("@time", time);

                try
                {
                    Con.Open();
                    int re = cmd.ExecuteNonQuery();
                    return re > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public List<CourseData> AccessAssignedCourse(int learnerid)
        {
            List<CourseData> assignedCourse = new List<CourseData>();
            string query = "Select c.CourseID, c.CourseName,c.CourseCode, c.Description, c.Category,c.Duration, c.Language from Courses c JOIN   LearnerCourse lc ON  c.CourseID = lc.course_id WHERE  lc.learner_id = @LearnerID";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@LearnerID", learnerid);

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    assignedCourse.Add(new CourseData()
                    {
                        CourseId = Convert.ToInt32(dr["CourseID"]),
                        CourseName = Convert.ToString(dr["CourseName"]),
                        CourseCode = Convert.ToString(dr["CourseCode"]),
                        CourseDescription = Convert.ToString(dr["Description"]),
                        CourseCategory = Convert.ToString(dr["Category"]),
                        CourseDuration = Convert.ToInt32(dr["Duration"]),
                        CourseLanguage = Convert.ToString(dr["Language"])

                    });
                }
            }
            return assignedCourse;

        }



        public List<ActiveLearnerModel> GetActiveLearnerList(int page, int pageSize)
        {
            List<ActiveLearnerModel> activeLearnerModels = new List<ActiveLearnerModel>();
            int offset = (page - 1) * pageSize;

            string query = "SELECT " +
                "lc.learner_id AS LearnerId, " +
                "l.UserName AS LearnerName, " +
                "l.UserEmail AS LearnerEmail, " +
                "lc.course_id AS CourseId, " +
                "c.CourseName AS CourseName, " +
                "lc.assigned_by AS OwnerId, " +
                "lc.assigned_owner_name AS OwnerName, " +
                "lc.assigned_owner_email AS OwnerEmail " +
                "FROM LearnerCourse lc " +
                "JOIN User_table l ON lc.learner_id = l.UserID " +
                "JOIN Courses c ON lc.course_id = c.CourseID " +
                "ORDER BY lc.learner_id " +
                "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@Offset", offset);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    activeLearnerModels.Add(new ActiveLearnerModel
                    {
                        LearnerId = Convert.ToInt32(dr["LearnerId"]),
                        LearnerName = Convert.ToString(dr["LearnerName"]),
                        LearnerEmail = Convert.ToString(dr["LearnerEmail"]),
                        CourseId = Convert.ToInt32(dr["CourseId"]),
                        CourseName = Convert.ToString(dr["CourseName"]),
                        OwnerId = Convert.ToInt32(dr["OwnerId"]),
                        OwnerName = Convert.ToString(dr["OwnerName"]),
                        OwnerEmail = Convert.ToString(dr["OwnerEmail"])
                    });
                }
            }

            return activeLearnerModels;
        }



        public bool DeallocateLearnerDb(int learnerid, int courseid, int ownerid)
        {
            string query = "delete from LearnerCourse where learner_id=@learnerid and course_id=@courseid and assigned_by=@ownerid";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@learnerid", learnerid);
                cmd.Parameters.AddWithValue("@courseid", courseid);
                cmd.Parameters.AddWithValue("@ownerid", ownerid);

                try
                {
                    Con.Open();
                    int re = cmd.ExecuteNonQuery();
                    return re > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public bool UpdateInstructorDb(int instructorid, string instructorname, string instructorpassword)
        {
            string query = "Update User_table set UserName=@username,UserPassword=@userpassword where UserID=@userid";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@username", instructorname);
                cmd.Parameters.AddWithValue("@userpassword", instructorpassword);
                cmd.Parameters.AddWithValue("@userid", instructorid);

                try
                {
                    Con.Open();
                    int re = cmd.ExecuteNonQuery();
                    return re > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public int TotalLearnerCountDb()
        {
            string query = "Select count(*) from User_table where UserRole='Learner'";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                try
                {
                    Con.Open();
                    int re = (int)cmd.ExecuteScalar();
                    return re;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return -1;
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public int TotalInstructorCountDb()
        {
            string query = "select count(*) from User_table where UserRole='Instructor'";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                try
                {
                    Con.Open();
                    int re = (int)cmd.ExecuteScalar();
                    return re;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return -1;
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public int TotalCourseCountDb()
        {
            string query = "Select count(*) from Courses";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                try
                {
                    Con.Open();
                    int re = (int)cmd.ExecuteScalar();
                    return re;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return -1;
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public int TotalInstructorCourseCountDb(string instructoremail)
        {
            string query = "Select count(*) from Courses where CreatedBy=@email";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@email", instructoremail);
                try
                {
                    Con.Open();
                    int re = (int)cmd.ExecuteScalar();
                    return re;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return -1;
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public int TotalActiveLearnerCountDb()
        {
            string query = "select count(learner_id) from LearnerCourse";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                try
                {
                    Con.Open();
                    int re = (int)cmd.ExecuteScalar();
                    return re;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return -1;
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public int TotalInstructorActiveLearnerCountDb(string instructoremail)
        {
            string query = "select count(distinct learner_id) from LearnerCourse where assigned_owner_email=@email";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@email", instructoremail);
                try
                {
                    Con.Open();
                    int re = (int)cmd.ExecuteScalar();
                    return re;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return -1;
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public List<InstructorModel> GetInstructorList()
        {
            List<InstructorModel> InsLi = new List<InstructorModel>();
            string query = "Select UserID, UserName,UserEmail from User_table where UserRole='Instructor'";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    InsLi.Add(new InstructorModel()
                    {
                        InstructorId = Convert.ToInt32(dr["UserID"]),
                        InstructorName = Convert.ToString(dr["UserName"]),
                        InstructorEmail = Convert.ToString(dr["UserEmail"])

                    });
                }
            }
            return InsLi;
        }

        public List<CourseData> GetPagedCourseListAdmin(int page, int pageSize)
        {
            List<CourseData> pagedCourseList = new List<CourseData>();
            int offset = (page - 1) * pageSize;

            string query = "SELECT CourseID, CourseName, CourseCode, Description, Category, Duration, Language,CreatorName FROM Courses " +
                           "ORDER BY CourseID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                cmd.Parameters.AddWithValue("@Offset", offset);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    pagedCourseList.Add(new CourseData()
                    {
                        CourseId = Convert.ToInt32(dr["CourseID"]),
                        CourseName = Convert.ToString(dr["CourseName"]),
                        CourseCode = Convert.ToString(dr["CourseCode"]),
                        CourseDescription = Convert.ToString(dr["Description"]),
                        CourseCategory = Convert.ToString(dr["Category"]),
                        CourseDuration = Convert.ToInt32(dr["Duration"]),
                        CourseLanguage = Convert.ToString(dr["Language"]),
                        CourseCreatorName = Convert.ToString(dr["CreatorName"])
                    });
                }
            }

            return pagedCourseList;
        }

        public int TotalUserCountDb()
        {
            string query = "select count (UserEmail) from User_table where UserRole in ('Instructor','Learner')";
            using (SqlCommand cmd = new SqlCommand(query, Con))
            {
                try
                {
                    Con.Open();
                    int re = (int)cmd.ExecuteScalar();
                    return re;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return -1;
                }
                finally
                {
                    Con.Close();
                }
            }
        }


    }
}