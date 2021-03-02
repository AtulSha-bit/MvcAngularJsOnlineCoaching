using ClosedXML.Excel;
using System.IO;
using OnlineCoaching.Linq_To_Sql;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Globalization;
using OnlineCoaching.Angular_Js_Data_Layer;
using System.Collections.Generic;
using System.Web;
using System.Text;
using Newtonsoft.Json;
using System.Collections;

namespace OnlineCoaching.Controllers
{
    public class UserController : Controller
    {

        CourseDataContext db = new CourseDataContext();

        public ActionResult Index()
        {
            return View();
        }

        // Send Otp to Mail ( Send Otp to Student)
        [HttpPost]
        public JsonResult SendOtp(string Name, string Email, string Username, string UserType, int? TutorId, int? StructureId)
        {
            try
            {
                bool result = false;
                string User = "";
                if (UserType == "Student" && Username != "" && Username != null && TutorId == null && StructureId == null)
                {
                    User = Username;
                    var emailexits = db.SITSPL_tblStudentProfiles.Where(x => x.Email == Email).Select(x => new { x.Email }).Count();

                    if (emailexits > 0)
                    {
                        return Json("Emailalready");
                    }

                    var data4 = db.SITSPL_tblStudentProfiles.Select(x => new { x.Username }).ToList();

                    if (data4 != null)
                    {
                        var Users = data4.Select(x => x.Username);
                        if (User != null && Users.Contains(User, StringComparer.OrdinalIgnoreCase))
                        {
                            return Json("Useralready");
                        }
                    }
                }

                else if (UserType == "Student" && Username != "" && Username != null && TutorId != null && StructureId != null)
                {
                    User = Username;
                    var emailexits = db.SITSPL_tblStudentProfiles.Where(x => x.TutorId == TutorId && x.CourseStructureId == StructureId && x.Email == Email).Select(x => new { x.Email }).Count();

                    if (emailexits > 0)
                    {
                        return Json("Emailalready");
                    }

                    var data4 = db.SITSPL_tblStudentProfiles.Select(x => new { x.Username }).ToList();

                    if (data4 != null)
                    {
                        var Users = data4.Select(x => x.Username);
                        if (User != null && Users.Contains(User, StringComparer.OrdinalIgnoreCase))
                        {
                            return Json("Useralready");
                        }
                    }
                }


                else if (UserType == "Tutor" && Username == null)
                {
                    if (Name != null && Email != null && UserType == "Tutor")
                    {
                        var emailexists = db.SITSPL_tblTutors.Where(x => x.TutorEmail == Email).Select(x => new { x.TutorEmail }).Count();

                        if (emailexists > 0)
                        {
                            return Json("Emailalreadyexists");
                        }
                    }
                }

                //   var emailexists = db.SITSPL_tblTutors.Where(x => x.TutorEmail == Email).Select(x => new { x.TutorEmail }).Count();
                if (UserType == "Student" || UserType == "Tutor")
                {
                    Random random = new Random();
                    String r = random.Next(0, 1000000).ToString("D6");
                    Session["OTP"] = r;
                    MailMessage mail = new MailMessage();
                    mail.To.Add(Email);

                    //  mail.From = new MailAddress("atul91915@gmail.com");
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);

                    mail.Subject = "OTP Regarding";
                    string Body = "Hello " + Name + " Your OTP number is: " + r;
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"],
                        ConfigurationManager.AppSettings["smtpPass"]);
                    // smtp.Credentials = new System.Net.NetworkCredential("username", "password"); // Enter seders User name and password  
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    //  SendMailToStudent(Name,Email);
                    result = true;

                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                int msg = 7;
                if (str == "Failure sending mail.")
                {
                    msg = 9;
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    msg = 10;
                    return Json(ex.Message, JsonRequestBehavior.AllowGet);
                }
            }
        }

        // Send Mail
        public void SendMailToStudent(string Name, string Email)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(Email);

            //  mail.From = new MailAddress("atul91915@gmail.com");

            mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);

            mail.Subject = "OTP Regarding";
            string Body = "Hello " + Name + " You are sucessfully registered as Student";
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"],
                ConfigurationManager.AppSettings["smtpPass"]);
            // smtp.Credentials = new System.Net.NetworkCredential("username", "password"); // Enter seders User name and password  
            smtp.EnableSsl = true;
            smtp.Send(mail);

        }


        // Otp Verify Code

        [HttpPost]
        public JsonResult ConfirmEmailOtp(string OTP)
        {
            try
            {
                bool result = false;
                if (Session["OTP"].ToString() != null && OTP != null)
                {
                    if (Session["OTP"].ToString() == OTP)
                    {
                        result = true;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result = false;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Application Error : " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // Get Courses Data

        public JsonResult GetCourses()
        {
            try
            {
                db = new CourseDataContext();
                var data = db.SITSPL_tblCourses.Select(x => new { x.CourseId, x.CourseName }).ToList();
                if (data != null)
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string str = ex.Message;

                return Json("Application Error : " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // Get Student Category For Course Structure

        public JsonResult StudentCategoryData()
        {
            try
            {
                db = new CourseDataContext();
                var data = db.tblStudentCategories.Where(x => x.IsPublished == true).Select(x => new { x.StudentCategoryId, x.CategoryName }).ToList();
                if (data != null)
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // Get Brand Tutor Data

        public JsonResult BrandTutorData()
        {
            try
            {
                db = new CourseDataContext();
                //var data = db.tblStudentCategories.Where(x => x.IsPublished == true).Select(x => new { x.StudentCategoryId, x.CategoryName }).ToList();

                var data = (from tut in db.SITSPL_tblTutors
                            join us in db.SITSPL_tblUsers
                            on tut.TutorId equals us.Id
                            where tut.TutorType == "Brand-Tutor"
                            select new
                            {
                                tut.TutorId,
                                tut.TutorName
                            }).Distinct().ToList();

                if (data != null)
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // Get Fees etc on the student page on change of Course: On based of Course and Duration

        public JsonResult GetCourseDetails(int Course, int Duration)
        {
            try
            {
                db = new CourseDataContext();
                Int64 intStudentId = 0;
                if (Session["StudentId"] != null)
                {
                    Int64.TryParse(Session["StudentId"].ToString(), out intStudentId);
                }

                var Category = (from std in db.SITSPL_tblStudentProfiles
                                where std.StudentId == intStudentId
                                select new
                                {
                                    std.StudentCategoryId
                                }).FirstOrDefault();

                //var data2 = (from c in db.SITSPL_tblCourses
                //             join cs in db.SITSPL_tblCourseStructures on c.CourseId equals cs.CourseId
                //             where cs.CourseId == Course && cs.DurationName == Duration.TrimStart()
                //             select new
                //             {
                //                 Course = cs.CourseId,
                //                 cs.StructureId,
                //             }).FirstOrDefault();

                var data2 = (from c in db.SITSPL_tblCourses
                             join cs in db.SITSPL_tblCourseStructures on c.CourseId equals cs.CourseId
                             where cs.CourseId == Course && cs.DurationId == Duration && cs.StdCatgId == Category.StudentCategoryId
                             select new
                             {
                                 Course = cs.CourseId,
                                 cs.StructureId,
                             }).FirstOrDefault();


                var Valid = (from x in db.SITSPL_tblCourseStructures
                             where x.CourseId == Course && x.DurationId == Duration
                             select new
                             {
                                 x.ValidTo
                             }).FirstOrDefault();

                DateTime dt = DateTime.Now;

                if (Valid != null && Valid.ValidTo <= dt)
                {
                    db = new CourseDataContext();
                    if (Valid.ValidTo <= dt)
                    {
                        return Json("expireCourse", JsonRequestBehavior.AllowGet);

                    }
                }

                if (data2 != null && data2.Course.ToString() != "" && data2.StructureId.ToString() != "")
                {
                    // Convert.ToBoolean(0)
                    var Courses = data2.Course;
                    var Months = data2.StructureId;
                    var data3 = (from c in db.SITSPL_tblCourses
                                 join cs in db.SITSPL_tblCourseStructures
                                 on c.CourseId equals cs.CourseId
                                 where (cs.CourseId == Courses && cs.StructureId == Months && cs.DurationId == Duration &&
                                 cs.StdCatgId == Category.StudentCategoryId
                                 && cs.IsDeleted == false)
                                 select new
                                 {
                                     Course = Convert.ToInt32(cs.CourseId),
                                     Structure = cs.StructureId,
                                     Duration = cs.DurationName,
                                     cs.Fees,
                                     Discount = cs.DiscountPercent,
                                     cs.NetAmount,
                                     cs.ValidFrom,
                                     cs.ValidTo
                                 }).FirstOrDefault();

                    return Json(data3, JsonRequestBehavior.AllowGet);
                }
                else if (data2 == null)
                {
                    return Json("Duration", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("Application Error : " + ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }


        // Get Fees etc on the student page on change of Duration: On based of Course and Duration

        public JsonResult GetAllDetails(int Course, int Duration)
        {
            try
            {
                db = new CourseDataContext();

                Int64 intStudentId = 0;
                if (Session["StudentId"] != null)
                {
                    Int64.TryParse(Session["StudentId"].ToString(), out intStudentId);
                }

                var Category = (from std in db.SITSPL_tblStudentProfiles
                                where std.StudentId == intStudentId
                                select new
                                {
                                    std.StudentCategoryId
                                }).FirstOrDefault();

                //var data2 = (from c in db.SITSPL_tblCourses
                //         join cs in db.SITSPL_tblCourseStructures on c.CourseId equals cs.CourseId
                //         where cs.CourseId == Course && cs.DurationName == Duration.TrimStart() 
                //         select new
                //         {
                //             Course = cs.CourseId,
                //             cs.StructureId,
                //         }).FirstOrDefault();


                var data2 = (from c in db.SITSPL_tblCourses
                             join cs in db.SITSPL_tblCourseStructures on c.CourseId equals cs.CourseId

                             where cs.CourseId == Course && cs.DurationId == Duration && cs.StdCatgId == Category.StudentCategoryId
                             select new
                             {
                                 Course = cs.CourseId,
                                 cs.StructureId,
                             }).FirstOrDefault();


                //var Valid = (from x in db.SITSPL_tblCourseStructures
                //             where x.CourseId == Course && x.DurationName == Duration.TrimStart() 
                //             select new
                //             {
                //                 x.ValidTo
                //             }).FirstOrDefault();

                var Valid = (from x in db.SITSPL_tblCourseStructures
                             where x.CourseId == Course && x.DurationId == Duration
                             select new
                             {
                                 x.ValidTo
                             }).FirstOrDefault();


                DateTime dt = DateTime.Now;

                if (Valid != null && Valid.ValidTo <= dt)
                {
                    db = new CourseDataContext();
                    if (Valid.ValidTo <= dt)
                    {
                        return Json("Courseexpire", JsonRequestBehavior.AllowGet);
                    }
                }

                if (data2 != null && data2.Course.ToString() != "" && data2.StructureId.ToString() != "" && Valid.ValidTo > dt)
                {
                    //  Convert.ToBoolean(0)
                    var Courses = data2.Course;
                    var Months = data2.StructureId;
                    var data3 = (from c in db.SITSPL_tblCourses
                                 join cs in db.SITSPL_tblCourseStructures
                                 on c.CourseId equals cs.CourseId
                                 where (cs.CourseId == Courses && cs.StructureId == Months && cs.DurationId == Duration && cs.StdCatgId == Category.StudentCategoryId
                                 && cs.IsDeleted == false)
                                 select new
                                 {
                                     Course = Convert.ToInt32(cs.CourseId),
                                     Structure = cs.StructureId,
                                     Duration = cs.DurationName,
                                     cs.Fees,
                                     Discount = cs.DiscountPercent,
                                     cs.NetAmount,
                                     cs.ValidFrom,
                                     cs.ValidTo
                                 }).FirstOrDefault();

                    return Json(data3, JsonRequestBehavior.AllowGet);
                }
                else if (data2 == null)
                {
                    return Json("Duration", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("Application Error : " + ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowStudents()
        {
            if (Session["UserId"] != null)
            {
                string strId = Session["UserId"].ToString();
            }
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Admin/Login");
            }
            return View();
        }


        // Students Show Page ( Student Data Based On Filter)

        [HttpPost]
        public JsonResult ShowStudentDetails(int? intCourseId, int? intJoiningDate, string strMonth)
        {
            try
            {
                db = new CourseDataContext();
                Session.Remove("Course");
                Session.Remove("intJoiningDate");
                Session.Remove("strMonth");

                if (intCourseId != null)
                {
                    Session["Course"] = intCourseId;
                }

                if (intJoiningDate != null)
                {
                    Session["intJoiningDate"] = intJoiningDate;
                }

                if (strMonth != null && strMonth != "")
                {
                    Session["strMonth"] = strMonth;
                }

                //var Month = strMonth.Trim();strMonth == null || strMonth == ""

                if (intCourseId != null && intJoiningDate == null && string.IsNullOrEmpty(strMonth))
                {
                    var data2 = db.SITSPL_GetStudents(intCourseId, null, null).OrderByDescending(x => x.StudentId).ToList().Distinct();
                    return Json(data2, JsonRequestBehavior.AllowGet);
                }

                else if (intCourseId == null && intJoiningDate != null && string.IsNullOrEmpty(strMonth))
                {
                    var data3 = db.SITSPL_GetStudents(null, intJoiningDate, null).OrderByDescending(x => x.StudentId).OrderByDescending(x => x.StudentId).ToList().Distinct();
                    return Json(data3, JsonRequestBehavior.AllowGet);
                }

                else if (intCourseId == null && intJoiningDate == null && !string.IsNullOrEmpty(strMonth))
                {
                    var data4 = db.SITSPL_GetStudents(null, null, strMonth.Trim()).OrderByDescending(x => x.StudentId).OrderByDescending(x => x.StudentId).ToList().Distinct();
                    return Json(data4, JsonRequestBehavior.AllowGet);
                }

                else if (intCourseId != null && intJoiningDate != null && string.IsNullOrEmpty(strMonth))
                {
                    var data8 = db.SITSPL_GetStudents(Convert.ToInt32(intCourseId), Convert.ToInt32(intJoiningDate), null).
                        OrderByDescending(x => x.StudentId).ToList().Distinct();
                    return Json(data8, JsonRequestBehavior.AllowGet);

                }

                else if (intCourseId == null && intJoiningDate != null && !string.IsNullOrEmpty(strMonth))
                {
                    var data9 = db.SITSPL_GetStudents(null, Convert.ToInt32(intJoiningDate), strMonth.Trim()).
                        OrderByDescending(x => x.StudentId).OrderByDescending(x => x.StudentId).ToList().Distinct();
                    return Json(data9, JsonRequestBehavior.AllowGet);
                }

                else if (intCourseId != null && intJoiningDate == null && !string.IsNullOrEmpty(strMonth))
                {
                    var data10 = db.SITSPL_GetStudents(Convert.ToInt32(intCourseId), null, strMonth.Trim()).OrderByDescending(x => x.StudentId).OrderByDescending(x => x.StudentId).ToList().Distinct();
                    return Json(data10, JsonRequestBehavior.AllowGet);
                }

                else if (intCourseId != null && intJoiningDate != null && !string.IsNullOrEmpty(strMonth))
                {
                    var data5 = db.SITSPL_GetStudents(intCourseId, intJoiningDate, strMonth.Trim()).OrderByDescending(x => x.StudentId).ToList().Distinct();
                    return Json(data5, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data9 = db.SITSPL_GetStudents(null, null, null).OrderByDescending(x => x.StudentId).OrderByDescending(x => x.StudentId).ToList().Distinct();
                    return Json(data9, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("Application Error : " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



        // Download Excel Of Students Based On Filter
        [HttpPost]
        public ActionResult DownloadFilteredInExcel()
        {
            try
            {
                string CourseId = "";
                string JoiningDate = "";
                string Month = "";

                if (Session["Course"] != null)
                {
                    CourseId = Session["Course"].ToString();
                }
                if (Session["intJoiningDate"] != null)
                {
                    JoiningDate = Session["intJoiningDate"].ToString();
                }
                if (Session["strMonth"] != null)
                {
                    Month = Session["strMonth"].ToString();
                }

                db = new CourseDataContext();

                if (CourseId != "" && JoiningDate == "" && Month == "")
                {
                    var data2 = db.SITSPL_GetStudents(Convert.ToInt32(CourseId), null, null).OrderByDescending(x => x.StudentId).ToList();
                    DataTable dt2 = new DataTable("Student Report Based On Course");

                    dt2.Columns.AddRange(new DataColumn[28] {  new DataColumn("Student Name"), new DataColumn("Course"),
                    new DataColumn("Duration"),new DataColumn("Mobile"), new DataColumn("Email"),new DataColumn("DOB"),
                        new DataColumn("State"),new DataColumn("City"),new DataColumn("Address"),new DataColumn("Landmark"),
               new DataColumn("Pin Code"),new DataColumn("Fees"),new DataColumn("Discount Percent"),
               new DataColumn("Payment Mode"), new DataColumn("Fee After Discount"),new DataColumn("Net Amount"),
               new DataColumn("Joining Date"), new DataColumn("Month"),new DataColumn("Created By"),
               new DataColumn("Date Created"),new DataColumn("Remarks"),new DataColumn("Payment Status"),
                    new DataColumn("Paid Amount"),new DataColumn("Due"),new DataColumn("Next Installment Date"),
                    new DataColumn("Remarks Payment"),new DataColumn("Temporary Reg No"),new DataColumn("Final Reg No")});

                    for (int i = 0; i < data2.Count; i++)
                    {
                        dt2.Rows.Add(data2[i].StudentName, data2[i].Course, data2[i].Duration, data2[i].Mobile, data2[i].Email,
                            data2[i].DOB, data2[i].State, data2[i].City, data2[i].Address, data2[i].Landmark,
                            data2[i].PinCode, data2[i].Fees, data2[i].DiscountPercent, data2[i].PaymentMode,
                            data2[i].FeeAfterDiscount, data2[i].NetAmount, data2[i].JoiningDate, data2[i].Month,
                            data2[i].CreatedBy, data2[i].DateCreated, data2[i].Remarks, data2[i].PaymentStatus, data2[i].PaidAmount,
                            data2[i].Due, data2[i].NextInstallmentDate, data2[i].RemarksPayment, data2[i].TemporaryRegNo,
                            data2[i].FinalRegNo);
                    }

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        dt2.TableName = "Student Report Based On Course";
                        wb.Worksheets.Add(dt2);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students Report.xlsx");
                        }
                    }
                }
                else if (CourseId == "" && JoiningDate != "" && Month == "")
                {
                    var data3 = db.SITSPL_GetStudents(null, Convert.ToInt32(JoiningDate), null).OrderByDescending(x => x.StudentId).ToList();
                    DataTable dt3 = new DataTable("Student On Joining Date");

                    dt3.Columns.AddRange(new DataColumn[28] {
                        new DataColumn("Student Name"),new DataColumn("Course"), new DataColumn("Duration"),
                        new DataColumn("Mobile"),new DataColumn("Email"),new DataColumn("DOB"),new DataColumn("State"),
                new DataColumn("City"),new DataColumn("Address"),new DataColumn("Landmark"),new DataColumn("PinCode"),
                new DataColumn("Fees"),new DataColumn("DiscountPercent"),new DataColumn("PaymentMode"),
                new DataColumn("FeeAfterDiscount"),new DataColumn("NetAmount"),new DataColumn("JoiningDate"),
                new DataColumn("Month"),new DataColumn("CreatedBy"),new DataColumn("DateCreated"),new DataColumn("Remarks"),
                 new DataColumn("Payment Status"),new DataColumn("Paid Amount"),new DataColumn("Due"),
                 new DataColumn("Next Installment Date"),new DataColumn("Remarks Payment"),new DataColumn("Temporary Reg No"),
                        new DataColumn("Final Reg No")});

                    for (int i = 0; i < data3.Count; i++)
                    {
                        dt3.Rows.Add(data3[i].StudentName, data3[i].Course, data3[i].Duration, data3[i].Mobile, data3[i].Email,
                            data3[i].DOB, data3[i].State, data3[i].City, data3[i].Address, data3[i].Landmark, data3[i].PinCode,
                            data3[i].Fees, data3[i].DiscountPercent, data3[i].PaymentMode, data3[i].FeeAfterDiscount,
                            data3[i].NetAmount, data3[i].JoiningDate, data3[i].Month, data3[i].CreatedBy, data3[i].DateCreated,
                            data3[i].Remarks, data3[i].PaymentStatus, data3[i].PaidAmount,
                            data3[i].Due, data3[i].NextInstallmentDate, data3[i].RemarksPayment, data3[i].TemporaryRegNo,
                            data3[i].FinalRegNo);
                    }

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        dt3.TableName = "Student On Joining Date";
                        wb.Worksheets.Add(dt3);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students Report.xlsx");
                        }
                    }
                }
                else if (CourseId == "" && JoiningDate == "" && Month != "")
                {
                    var data4 = db.SITSPL_GetStudents(null, null, Month.Trim()).OrderByDescending(x => x.StudentId).ToList();
                    DataTable dt4 = new DataTable("Student Report based on Month");

                    dt4.Columns.AddRange(new DataColumn[28] {
                        new DataColumn("Student Name"),new DataColumn("Course"),new DataColumn("Duration"),
                        new DataColumn("Mobile"),new DataColumn("Email"),new DataColumn("DOB"),new DataColumn("State"),
                        new DataColumn("City"),new DataColumn("Address"),new DataColumn("Landmark"),
                        new DataColumn("Pin Code"),new DataColumn("Fees"),new DataColumn("Discount Percent"),
                        new DataColumn("Payment Mode"), new DataColumn("Fee After Discount"),new DataColumn("Net Amount"),
                        new DataColumn("Joining Date"), new DataColumn("Month"),new DataColumn("Created By"),
                        new DataColumn("Date Created"),new DataColumn("Remarks"),
                    new DataColumn("Payment Status"),new DataColumn("Paid Amount"),new DataColumn("Due"),
                 new DataColumn("Next Installment Date"),new DataColumn("Remarks Payment"),new DataColumn("Temporary Reg No"),
                        new DataColumn("Final Reg No")});

                    for (int i = 0; i < data4.Count; i++)
                    {
                        dt4.Rows.Add(data4[i].StudentName, data4[i].Course, data4[i].Duration, data4[i].Mobile, data4[i].Email,
                            data4[i].DOB, data4[i].State, data4[i].City, data4[i].Address, data4[i].Landmark, data4[i].PinCode,
                            data4[i].Fees, data4[i].DiscountPercent, data4[i].PaymentMode, data4[i].FeeAfterDiscount,
                            data4[i].NetAmount, data4[i].JoiningDate, data4[i].Month, data4[i].CreatedBy, data4[i].DateCreated,
                            data4[i].Remarks, data4[i].PaymentStatus, data4[i].PaidAmount,
                            data4[i].Due, data4[i].NextInstallmentDate, data4[i].RemarksPayment, data4[i].TemporaryRegNo,
                            data4[i].FinalRegNo);
                    }

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        dt4.TableName = "Student Report based on Month";
                        wb.Worksheets.Add(dt4);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students Report.xlsx");
                        }
                    }
                }
                else if (CourseId != "" && JoiningDate != "" && Month == "")
                {
                    var data8 = db.SITSPL_GetStudents(Convert.ToInt32(CourseId), Convert.ToInt32(JoiningDate), null).OrderByDescending(x => x.StudentId).ToList();
                    DataTable dt10 = new DataTable("Students On Course and Joining Date");

                    dt10.Columns.AddRange(new DataColumn[28] {
                        new DataColumn("Student Name"),new DataColumn("Course"),new DataColumn("Duration"),
                        new DataColumn("Mobile"),new DataColumn("Email"),new DataColumn("DOB"),new DataColumn("State"),
                        new DataColumn("City"),new DataColumn("Address"),new DataColumn("Landmark"),new DataColumn("Pin Code"),
                        new DataColumn("Fees"),new DataColumn("Discount Percent"),new DataColumn("Payment Mode"),
                        new DataColumn("Fee After Discount"),new DataColumn("Net Amount"),new DataColumn("Joining Date"),
                        new DataColumn("Month"),new DataColumn("Created By"),new DataColumn("Date Created"),
                        new DataColumn("Remarks"),new DataColumn("Payment Status"),new DataColumn("Paid Amount"),
                        new DataColumn("Due"),new DataColumn("Next Installment Date"),new DataColumn("Remarks Payment"),
                        new DataColumn("Temporary Reg No"),new DataColumn("Final Reg No")});

                    for (int i = 0; i < data8.Count; i++)
                    {
                        dt10.Rows.Add(data8[i].StudentName, data8[i].Course, data8[i].Duration, data8[i].Mobile, data8[i].Email,
                            data8[i].DOB, data8[i].State, data8[i].City, data8[i].Address, data8[i].Landmark, data8[i].PinCode,
                            data8[i].Fees, data8[i].DiscountPercent, data8[i].PaymentMode, data8[i].FeeAfterDiscount,
                            data8[i].NetAmount, data8[i].JoiningDate, data8[i].Month, data8[i].CreatedBy, data8[i].DateCreated,
                            data8[i].Remarks, data8[i].PaymentStatus, data8[i].PaidAmount,
                            data8[i].Due, data8[i].NextInstallmentDate, data8[i].RemarksPayment, data8[i].TemporaryRegNo,
                            data8[i].FinalRegNo);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        dt10.TableName = "Students On Course,Joining";
                        wb.Worksheets.Add(dt10);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students Report.xlsx");
                        }
                    }
                }
                else if (CourseId == "" && JoiningDate != "" && Month != "")
                {
                    var data9 = db.SITSPL_GetStudents(null, Convert.ToInt32(JoiningDate), Month.Trim()).OrderByDescending(x => x.StudentId).ToList();
                    DataTable dt11 = new DataTable("Student On JoiningDate,Month");

                    dt11.Columns.AddRange(new DataColumn[28] { new DataColumn("Student Name"),new DataColumn("Course"),
                    new DataColumn("Duration"),new DataColumn("Mobile"),new DataColumn("Email"),new DataColumn("DOB"),
                    new DataColumn("State"),new DataColumn("City"),new DataColumn("Address"),new DataColumn("Landmark"),
                    new DataColumn("Pin Code"),new DataColumn("Fees"),new DataColumn("Discount Percent"),
                    new DataColumn("Payment Mode"), new DataColumn("Fee After Discount"),new DataColumn("Net Amount"),
                    new DataColumn("Joining Date"), new DataColumn("Month"),new DataColumn("Created By"),
                    new DataColumn("Date Created"),new DataColumn("Remarks"),new DataColumn("Payment Status"),
                        new DataColumn("Paid Amount"),new DataColumn("Due"),new DataColumn("Next Installment Date"),
                        new DataColumn("Remarks Payment"),new DataColumn("Temporary Reg No"),new DataColumn("Final Reg No")});

                    for (int i = 0; i < data9.Count; i++)
                    {
                        dt11.Rows.Add(data9[i].StudentName, data9[i].Course, data9[i].Duration, data9[i].Mobile, data9[i].Email,
                            data9[i].DOB, data9[i].State, data9[i].City, data9[i].Address, data9[i].Landmark, data9[i].PinCode,
                            data9[i].Fees, data9[i].DiscountPercent, data9[i].PaymentMode, data9[i].FeeAfterDiscount,
                            data9[i].NetAmount, data9[i].JoiningDate, data9[i].Month, data9[i].CreatedBy, data9[i].DateCreated,
                            data9[i].Remarks, data9[i].PaymentStatus, data9[i].PaidAmount,
                            data9[i].Due, data9[i].NextInstallmentDate, data9[i].RemarksPayment, data9[i].TemporaryRegNo,
                            data9[i].FinalRegNo);
                    }

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        dt11.TableName = "Student On JoiningDate,Month";
                        wb.Worksheets.Add(dt11);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students Report.xlsx");
                        }
                    }
                }
                else if (CourseId != "" && JoiningDate == "" && Month != "")
                {
                    var data10 = db.SITSPL_GetStudents(Convert.ToInt32(CourseId), null, Month.Trim()).OrderByDescending(x => x.StudentId).ToList();
                    DataTable dt12 = new DataTable("Student on Course,Month");

                    dt12.Columns.AddRange(new DataColumn[28] {
                        new DataColumn("Student Name"),new DataColumn("Course"),new DataColumn("Duration"),
                        new DataColumn("Mobile"),new DataColumn("Email"),new DataColumn("DOB"),new DataColumn("State"),
                        new DataColumn("City"),new DataColumn("Address"),new DataColumn("Landmark"),new DataColumn("Pin Code"),
                        new DataColumn("Fees"),new DataColumn("Discount Percent"),new DataColumn("Payment Mode"),
                        new DataColumn("Fee After Discount"),new DataColumn("Net Amount"), new DataColumn("Joining Date"),
                        new DataColumn("Month"),new DataColumn("Created By"),new DataColumn("Date Created"),
                        new DataColumn("Remarks"),new DataColumn("Payment Status"),
                        new DataColumn("Paid Amount"),new DataColumn("Due"),new DataColumn("Next Installment Date"),
                        new DataColumn("Remarks Payment"),new DataColumn("Temporary Reg No"),new DataColumn("Final Reg No")});

                    for (int i = 0; i < data10.Count; i++)
                    {
                        dt12.Rows.Add(data10[i].StudentName, data10[i].Course, data10[i].Duration, data10[i].Mobile,
                            data10[i].Email, data10[i].DOB, data10[i].State, data10[i].City, data10[i].Address,
                            data10[i].Landmark, data10[i].PinCode, data10[i].Fees, data10[i].DiscountPercent,
                            data10[i].PaymentMode, data10[i].FeeAfterDiscount, data10[i].NetAmount, data10[i].JoiningDate,
                            data10[i].Month, data10[i].CreatedBy, data10[i].DateCreated, data10[i].Remarks,
                            data10[i].PaymentStatus, data10[i].PaidAmount,
                            data10[i].Due, data10[i].NextInstallmentDate, data10[i].RemarksPayment, data10[i].TemporaryRegNo,
                            data10[i].FinalRegNo);
                    }

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        dt12.TableName = "Student on Course,Month";
                        wb.Worksheets.Add(dt12);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students Report.xlsx");
                        }
                    }
                }

                else if (CourseId != "" && JoiningDate != "" && Month != "")
                {
                    var data5 = db.SITSPL_GetStudents(Convert.ToInt32(CourseId), Convert.ToInt32(JoiningDate),
                        Month.Trim()).OrderByDescending(x => x.StudentId).ToList();
                    DataTable dt5 = new DataTable("Student Course,Joining,Month");

                    dt5.Columns.AddRange(new DataColumn[28] {
                        new DataColumn("Student Name"),new DataColumn("Course"),new DataColumn("Duration"),
                        new DataColumn("Mobile"),new DataColumn("Email"),new DataColumn("DOB"),new DataColumn("State"),
                        new DataColumn("City"),new DataColumn("Address"),new DataColumn("Landmark"),new DataColumn("Pin Code"),
                        new DataColumn("Fees"),new DataColumn("Discount Percent"),new DataColumn("Payment Mode"),
                        new DataColumn("Fee After Discount"),new DataColumn("Net Amount"),new DataColumn("Joining Date"),
                        new DataColumn("Month"),new DataColumn("Created By"),new DataColumn("Date Created"),
                        new DataColumn("Remarks"),new DataColumn("Payment Status"),
                        new DataColumn("Paid Amount"),new DataColumn("Due"),new DataColumn("Next Installment Date"),
                        new DataColumn("Remarks Payment"),new DataColumn("Temporary Reg No"),new DataColumn("Final Reg No")});

                    for (int i = 0; i < data5.Count; i++)
                    {
                        dt5.Rows.Add(data5[i].StudentName, data5[i].Course, data5[i].Duration, data5[i].Mobile, data5[i].Email,
                            data5[i].DOB, data5[i].State, data5[i].City, data5[i].Address, data5[i].Landmark, data5[i].PinCode,
                            data5[i].Fees, data5[i].DiscountPercent, data5[i].PaymentMode, data5[i].FeeAfterDiscount,
                            data5[i].NetAmount, data5[i].JoiningDate, data5[i].Month, data5[i].CreatedBy, data5[i].DateCreated,
                            data5[i].Remarks, data5[i].PaymentStatus, data5[i].PaidAmount,
                            data5[i].Due, data5[i].NextInstallmentDate, data5[i].RemarksPayment, data5[i].TemporaryRegNo,
                            data5[i].FinalRegNo);
                    }

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        dt5.TableName = "Student on Course,Joining,Month";
                        wb.Worksheets.Add(dt5);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students Report.xlsx");
                        }
                    }
                }

                else if (CourseId == "" && JoiningDate == "" && Month == "")
                {
                    var data6 = db.SITSPL_GetStudents(null, null, null).OrderByDescending(x => x.StudentId).ToList();
                    DataTable dt6 = new DataTable("Student Report Generation");

                    dt6.Columns.AddRange(new DataColumn[28] {
                        new DataColumn("Student Name"),new DataColumn("Course"),new DataColumn("Duration"),
                        new DataColumn("Mobile"),new DataColumn("Email"),new DataColumn("DOB"),new DataColumn("State"),
                        new DataColumn("City"),new DataColumn("Address"),new DataColumn("Landmark"),new DataColumn("Pin Code"),
                        new DataColumn("Fees"),new DataColumn("Discount Percent"),new DataColumn("Payment Mode"),
                        new DataColumn("Fee After Discount"),new DataColumn("Net Amount"),new DataColumn("Joining Date"),
                        new DataColumn("Month"),new DataColumn("Created By"),new DataColumn("Date Created"),
                        new DataColumn("Remarks"),new DataColumn("Payment Status"),
                        new DataColumn("Paid Amount"),new DataColumn("Due"),new DataColumn("Next Installment Date"),
                        new DataColumn("Remarks Payment"),new DataColumn("Temporary Reg No"),new DataColumn("Final Reg No")});

                    for (int i = 0; i < data6.Count; i++)
                    {
                        dt6.Rows.Add(data6[i].StudentName,
                            data6[i].Course, data6[i].Duration, data6[i].Mobile, data6[i].Email, data6[i].DOB, data6[i].State,
                            data6[i].City, data6[i].Address, data6[i].Landmark, data6[i].PinCode, data6[i].Fees,
                            data6[i].DiscountPercent, data6[i].PaymentMode, data6[i].FeeAfterDiscount, data6[i].NetAmount,
                            data6[i].JoiningDate, data6[i].Month, data6[i].CreatedBy, data6[i].DateCreated, data6[i].Remarks,
                            data6[i].PaymentStatus, data6[i].PaidAmount,
                            data6[i].Due, data6[i].NextInstallmentDate, data6[i].RemarksPayment, data6[i].TemporaryRegNo,
                            data6[i].FinalRegNo);
                    }

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        dt6.TableName = "Student Report Generation";
                        wb.Worksheets.Add(dt6);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students Report.xlsx");
                        }
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return Json("Application Error : " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult StudentLogin()
        {
            if (Session["StudentId"] != null)
            {
                string str = Session["StudentId"].ToString();
                return RedirectToAction("StudentWelcome", "User");
            }

            if (Session["StudentId"] == null)
            {
                return View("StudentLogin");
                //return RedirectToAction("StudentLogin", "User");
            }
            return View();
        }


        [HttpPost]
        public ActionResult StudentLogin(SITSPL_tblStudentProfile student)
        {
            try
            {
                db = new CourseDataContext();
                var Username = student.Username;
                var Password = student.Password;
                //var data = db.SITSPL_tblStudentProfiles.Where(x => x.Username == Username && x.Password == Password).FirstOrDefault();                
                var data = (from d in db.SITSPL_tblUsers
                            where d.UserName == Username && d.Password == Password
                            join d1 in db.SITSPL_tblStudentProfiles on d.Id equals d1.StudentId
                            select new
                            {
                                d.UserName,
                                d.UserType,
                                d.UserId,
                                d1.Email,
                                d1.ProfileImage,
                                d1.StudentId,
                                d1.StudentType,
                                d1.StudentCategoryId,
                                d1.BatchId,
                                d1.StudentSubCategoryId
                            }).SingleOrDefault();
                if (data != null)
                {
                    Session["StudentId"] = data.StudentId;
                    Session["User"] = data.UserName;
                    Session["Email"] = data.Email;
                    Session["Image"] = data.ProfileImage;
                    Session["StudentType"] = data.StudentType;
                    Session["CategoryId"] = data.StudentCategoryId;
                    Session["SubCategoryId"] = data.StudentSubCategoryId;
                    Session["BatchId"] = data.BatchId;
                    return RedirectToAction("StudentWelcome", "User");
                }
                else
                {
                    ViewBag.Message = "Wrong Username Or Password";
                    return View();
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        // Logout Student

        public ActionResult LogoutStudent()
        {
            Session["StudentId"] = null;
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("StudentLogin", "User");
        }

        //=============================================Student Module==============================================================
        //===========================================================================================================

        #region Student LWelcome Page
        public ActionResult StudentWelcome()
        {
            //if(Session["StudentId"] != null)
            //{
            //    string str = Session["StudentId"].ToString();
            //}

            if (Session["StudentId"] == null)
            {
                return RedirectToAction("StudentLogin", "User");
            }
            return View();
        }

        #endregion


        public ActionResult StudentsProfile()
        {
            if (Session["StudentId"] != null)
            {
                string str = Session["StudentId"].ToString();
            }

            if (Session["StudentId"] == null)
            {
                return RedirectToAction("StudentLogin", "User");
            }
            return View();
        }

        //   join c in db.SITSPL_tblCourses on  std2.CourseId equals c.CourseId

        public JsonResult ShowStudentData()
        {

            try
            {
                db = new CourseDataContext();
                var StudentId = Session["StudentId"].ToString();

                var TutId = (from stdpro in db.SITSPL_tblStudentProfiles
                             where stdpro.StudentId == Convert.ToInt32(StudentId)
                             select new
                             {
                                 stdpro.TutorId
                             }).FirstOrDefault();

                var sub = (from stdpro in db.SITSPL_tblStudentProfiles
                           join contact in db.SITSPL_tblContacts on
                           stdpro.StudentId equals contact.UserId into eGroup
                           from std in eGroup.DefaultIfEmpty()
                           join cd in db.SITSPL_tblCourseDetails
                           on stdpro.StudentId equals cd.StudentId into eGroup2
                           from std2 in eGroup2.DefaultIfEmpty()
                           join stdcatg in db.tblStudentCategories
                           on stdpro.StudentCategoryId equals stdcatg.StudentCategoryId
                           into eGroup3
                           from std3 in eGroup3.DefaultIfEmpty()
                           where stdpro.StudentId == Convert.ToInt32(StudentId)
                           select new
                           {
                               stdpro,
                               std,
                               std2,
                               std3
                           }).ToList();


                var stdProfile = sub.Select(x => x.stdpro).Any();
                var stdContact = sub.Select(x => x.std).ToList();
                var std2Course = sub.Select(x => x.std2).ToList();

                //  Student data exists , contact and course null

                if (stdContact[0] == null && std2Course[0] == null)
                {
                    var data = sub.Select(x => new
                    {
                        x.stdpro.DOB,
                        Created = x.stdpro.DateCreated,
                        x.stdpro.Email,
                        IDeleted = x.stdpro.IsDeleted,
                        Phone = x.stdpro.Mobile,
                        x.stdpro.Name,
                        x.stdpro.NetAmount,

                        x.stdpro.LongDescription,
                        x.stdpro.ShortDescription,
                        x.stdpro.StructureId,
                        x.stdpro.ProfileImage,
                        x.stdpro.TemporaryRegNo,
                        x.stdpro.StudentId,
                        x.stdpro.Twitterlink,
                        x.stdpro.Fblink,
                        x.stdpro.Instalink,
                        x.stdpro.Username,
                        x.stdpro.Password,
                        x.stdpro.StudentCategoryId,
                        x.std3.CategoryName,
                        TutId
                    }).ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                //  Student data exists , contact exists and course null

                else if (stdContact[0] != null && std2Course[0] == null)
                {
                    var data = sub.Select(x => new
                    {
                        x.stdpro.DOB,
                        Created = x.stdpro.DateCreated,
                        x.stdpro.Email,
                        IDeleted = x.stdpro.IsDeleted,
                        Phone = x.stdpro.Mobile,
                        x.stdpro.Name,
                        x.stdpro.NetAmount,
                        x.stdpro.LongDescription,
                        x.stdpro.ShortDescription,
                        x.stdpro.StructureId,
                        x.stdpro.ProfileImage,
                        x.stdpro.TemporaryRegNo,
                        x.stdpro.StudentId,
                        x.stdpro.Twitterlink,
                        x.stdpro.Fblink,
                        x.stdpro.Instalink,
                        x.stdpro.Username,
                        x.stdpro.Password,
                        x.std.State,
                        x.std.City,
                        x.std.Address,
                        x.std.Landmark,
                        x.std.Pincode,
                        //   x.contac.Mobile,
                        x.std.ContactId,
                        x.stdpro.StudentCategoryId,
                        x.std3.CategoryName,
                        TutId

                    }).ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else if (stdContact[0] == null && std2Course[0] != null)
                {
                    var data = sub.Select(x => new
                    {
                        x.stdpro.DOB,
                        x.stdpro.DateCreated,
                        x.stdpro.Email,
                        x.stdpro.IsDeleted,
                        Phone = x.stdpro.Mobile,
                        x.stdpro.Name,
                        x.stdpro.NetAmount,
                        x.stdpro.LongDescription,
                        x.stdpro.ShortDescription,
                        x.stdpro.StructureId,
                        x.stdpro.ProfileImage,
                        x.stdpro.TemporaryRegNo,
                        x.stdpro.StudentId,
                        x.stdpro.Twitterlink,
                        x.stdpro.Fblink,
                        x.stdpro.Instalink,
                        x.stdpro.Username,
                        x.stdpro.Password,
                        x.std2.CourseId,
                        //null? null :x.std2.CourseId,
                        x.std2.CourseValidFrom,
                        x.std2.CourseValidTo,
                        x.std2.DiscountPercent,
                        x.std2.Duration,
                        x.std2.Fees,
                        x.std2.FeesWithDiscount,
                        x.std2.IsPublished,
                        x.std2.JoiningDate,
                        x.std2.Month,
                        x.std2.CreatedBy,
                        x.stdpro.StudentCategoryId,
                        x.std3.CategoryName,
                        TutId

                    }).ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                //  Student data exists , contact and course null

                else if (stdContact[0] != null && std2Course[0] != null)
                {
                    var data = sub.Select(x => new
                    {
                        x.std2.CourseId,
                        //null? null :x.std2.CourseId,
                        x.std2.CourseValidFrom,
                        x.std2.CourseValidTo,
                        x.std2.DiscountPercent,
                        x.std2.Duration,
                        x.std2.Fees,
                        x.std2.FeesWithDiscount,
                        x.std2.IsPublished,
                        x.std2.JoiningDate,
                        x.std2.Month,
                        x.std2.CreatedBy,
                        // CreatedDate =   x.cours.DateCreated,
                        x.std.State,
                        x.std.City,
                        x.std.Address,
                        x.std.Landmark,
                        x.std.Pincode,
                        //   x.contac.Mobile,
                        x.std.ContactId,
                        //    x.contac.IsDeleted,
                        x.stdpro.DOB,
                        Created = x.stdpro.DateCreated,
                        x.stdpro.Email,
                        IDeleted = x.stdpro.IsDeleted,
                        Phone = x.stdpro.Mobile,
                        x.stdpro.Name,
                        x.stdpro.NetAmount,
                        x.stdpro.LongDescription,
                        x.stdpro.ShortDescription,
                        x.stdpro.StructureId,
                        x.stdpro.ProfileImage,
                        x.stdpro.TemporaryRegNo,
                        x.stdpro.StudentId,
                        x.stdpro.Twitterlink,
                        x.stdpro.Fblink,
                        x.stdpro.Instalink,
                        x.stdpro.Username,
                        x.stdpro.Password,
                        x.stdpro.StudentCategoryId,
                        x.std3.CategoryName,
                        TutId
                    }).ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult ShowInternshipData()
        {

            try
            {
                db = new CourseDataContext();
                string Username = "";
                Int32 InternId = 0;
                Int32.TryParse(Session["InterApllyId"].ToString(),out InternId);

                if(Session["InterApllyId"] != null) { 
                Username = Session["InternUser"].ToString();
                }

                var sub = (from intern in db.tblInternApplies
                           join contact in db.SITSPL_tblContacts on
                           intern.InterApllyId equals contact.UserId into eGroup
                           from std in eGroup.DefaultIfEmpty()
                           join intstr in db.SITSPL_tblInternshipStructures
                           on intern.InternshipStructureId equals intstr.InternStructureId into eGroup2
                           from std2 in eGroup2.DefaultIfEmpty()
                           where intern.InterApllyId == InternId
                           select new
                           {
                               intern,
                               std,
                               std2
                           }).ToList();


                var stdProfile = sub.Select(x => x.intern).Any();
                var stdContact = sub.Select(x => x.std).ToList();
                var std2Course = sub.Select(x => x.std2).ToList();

                //  Student data exists , contact and course null

                if (stdContact[0] == null && std2Course[0] == null)
                {
                    var data = sub.Select(x => new
                    {
                        x.intern.InterApllyId,
                        x.intern.InternshipStructureId,
                        x.intern.DOB,
                        x.intern.Email,
                        x.intern.Name,
                        x.intern.Twiter,
                        x.intern.Instagram,
                        x.intern.Facebook,
                        x.intern.HowDoYouKnow,
                        x.intern.CollegeUniv,
                        x.intern.Qualification,
                        x.intern.YearOfPassing,
                        x.intern.Image,
                        x.intern.CreatedBy,
                        DateCreated = x.intern.DateCreated.HasValue ? x.intern.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                        x.intern.UpdatedBy,
                        LastUpdated = x.intern.LastUpdated.HasValue ? x.intern.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                        x.intern.IsPublished,
                        x.intern.Comments,
                        x.intern.Trade,
                        x.std.Address,
                        x.std.City,
                        x.std.ContactId,
                        x.std.Mobile,
                        x.std.UserId,
                        x.std.Pincode,
                        x.std.State,
                        Username,
                        InternId


                    }).ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                //  Student data exists , contact exists and course null

                else if (stdContact[0] != null && std2Course[0] == null)
                {
                    var data = sub.Select(x => new
                    {
                        x.intern.InterApllyId,
                        x.intern.InternshipStructureId,
                        x.intern.DOB,
                        x.intern.Email,
                        x.intern.Name,
                        x.intern.Twiter,
                        x.intern.Instagram,
                        x.intern.Facebook,
                        x.intern.HowDoYouKnow,
                        x.intern.CollegeUniv,
                        x.intern.Qualification,
                        x.intern.YearOfPassing,
                        x.intern.Image,
                        x.intern.CreatedBy,
                        DateCreated = x.intern.DateCreated.HasValue ? x.intern.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                        x.intern.UpdatedBy,
                        LastUpdated = x.intern.LastUpdated.HasValue ? x.intern.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                        x.intern.IsPublished,
                        x.intern.Comments,
                        x.intern.Trade,

                        x.std.State,
                        x.std.City,
                        x.std.Address,
                        x.std.Landmark,
                        x.std.Pincode,
                        x.std.Mobile,
                        //   x.contac.Mobile,
                        x.std.ContactId,
                        Username,
                        InternId

                    }).ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else if (stdContact[0] == null && std2Course[0] != null)
                {
                    var data = sub.Select(x => new
                    {
                        x.intern.InterApllyId,
                        x.intern.InternshipStructureId,
                        x.intern.DOB,
                        x.intern.Email,
                        x.intern.Name,
                        x.intern.Twiter,
                        x.intern.Instagram,
                        x.intern.Facebook,
                        x.intern.HowDoYouKnow,
                        x.intern.CollegeUniv,
                        x.intern.Qualification,
                        x.intern.YearOfPassing,
                        x.intern.Image,
                        x.intern.CreatedBy,
                        DateCreated = x.intern.DateCreated.HasValue ? x.intern.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                        x.intern.UpdatedBy,
                        LastUpdated = x.intern.LastUpdated.HasValue ? x.intern.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                        x.intern.IsPublished,
                        x.intern.Comments,
                        x.intern.Trade,
                        x.std.State,
                        x.std.City,
                        x.std.Address,
                        x.std.Landmark,
                        x.std.Pincode,
                        x.std.Mobile,
                        //   x.contac.Mobile,
                        x.std.ContactId,
                        Username,
                        InternId


                    }).ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                //  Student data exists , contact and course null

                else if (stdContact[0] != null && std2Course[0] != null)
                {
                    var data = sub.Select(x => new
                    {

                        x.std2.CreatedBy,
                        // CreatedDate =   x.cours.DateCreated,
                        x.std.State,
                        x.std.City,
                        x.std.Address,
                        x.std.Landmark,
                        x.std.Pincode,
                        //   x.contac.Mobile,
                        x.std.ContactId,
                        //    x.contac.IsDeleted,

                        x.intern.InterApllyId,
                        x.intern.InternshipStructureId,
                        x.intern.DOB,
                        x.intern.Email,
                        x.intern.Name,
                        x.intern.Twiter,
                        x.intern.Instagram,
                        x.intern.Facebook,
                        x.intern.HowDoYouKnow,
                        x.intern.CollegeUniv,
                        x.intern.Qualification,
                        x.intern.YearOfPassing,
                        x.intern.Image,
                        DateCreated = x.intern.DateCreated.HasValue ? x.intern.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                        x.intern.UpdatedBy,
                        LastUpdated = x.intern.LastUpdated.HasValue ? x.intern.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                        x.intern.Comments,
                        x.intern.Trade,

                        x.std.Mobile,
                        Username,
                        InternId


                    }).ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult InternshipDocumentData()
        {
            try
            {
                db = new CourseDataContext();
                //var InternId = "";

                //if (Session["InterApplyId"] != null)
                //{
                //  InternId   = Session["InterApllyId"].ToString();
                //}
                //else
                //{
                //    InternId = null;
                //}

                var InternId = Session["InterApllyId"].ToString();
                db = new CourseDataContext();
                var data = (from intern in db.tblInternApplies
                            join intstr in db.SITSPL_tblInternshipStructures
                            on intern.InternshipStructureId equals intstr.InternStructureId
                            join doc in db.SITSPL_tblDocuments
                            on intern.InterApllyId equals doc.UserId
                            where intern.InterApllyId == Convert.ToInt32(InternId)
                            select new
                            {
                                intern.InterApllyId,
                                doc.DocumentId,
                                doc.UserId,
                                doc.DocumentName,
                                doc.DoucmentNo,
                                doc.CreatedBy,
                                InternId

                            }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        #region Internship Details For Internhip
        // Internship Structure Data
        public JsonResult GetInternshipDataBasedOnInternApplyId()
        {
            try
            {
                db = new CourseDataContext();
                var InternId = Session["InterApllyId"].ToString();
                var data = (from apply in db.tblInternApplies
                            join instr in db.SITSPL_tblInternshipStructures on apply.InternshipStructureId equals instr.InternStructureId
                            join dur in db.tblDurations on instr.DurationId equals dur.DurationId
                            join intern in db.SITSPL_tblInternships
                            on instr.InternshipId equals intern.InternshipId
                            where apply.InterApllyId == Convert.ToInt32(InternId)
                            select new
                            {
                                apply,
                                instr,
                                dur,
                                intern,
                            }).ToList();

                var internshipdetail = data.Select(d => new
                {
                    d.apply.InterApllyId,
                    d.apply.Name,
                    d.instr.InternStructureId,
                    ValidFrom = d.instr.ValidFrom.HasValue ? d.instr.ValidFrom.Value.ToString("dd/MM/yyyy") : null,
                    ValidTo = d.instr.ValidTo.HasValue ? d.instr.ValidTo.Value.ToString("dd/MM/yyyy") : null,
            DateCreated   =  d.apply.DateCreated.HasValue?d.apply.DateCreated.Value.ToString("dd/MM/yyyy"):null,
            d.instr.TotalAmount,
                    d.dur.DurationId,
                    d.dur.DurationName,
                    d.intern.InternshipId,
                    d.intern.InternshipName
                  
                }).ToList();

                //  var InternshipId = db.tblInternApplies.Where(x => x.InterApllyId == Convert.ToInt64(InternId)).Select(x => new { x.InternshipStructureId }).FirstOrDefault();
                var perks = (from d in db.SITSPL_tblInternshipStructures
                             join prk in db.tblInternPerks on d.InternStructureId equals prk.InternshipId
                             where prk.InternshipId == d.InternStructureId
                             join apply in db.tblInternApplies
                             on d.InternStructureId equals apply.InternshipStructureId
                             where apply.InterApllyId == Convert.ToInt64(InternId)

                             select new
                             {
                                 prk.InternPerkId,
                                 prk.PerkName,
                                 prk.IsPublished
                             }).ToList();


                var list = new { internshipdetail, perks };

                JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
                return Json(result, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion END Show Internship Details



        // Update Intern Profile

        [HttpPost]
        public ContentResult UpdateInternProfile(HttpPostedFileBase postedfile, string Username, string Phone, tblInternApply profile, int? InternshipId, SITSPL_tblContact internContact,
                      List<InternDocumentsUpdate> lstDocuments, HttpPostedFileBase resumePostedfile, HttpPostedFileBase aadhaarPostedfile)
        {
            try
            {
                string InterneId = "";

                if (InternshipId == null)
                {
                    InterneId = Session["InterApllyId"].ToString();
                }
                else
                {
                    InterneId = InternshipId.ToString();
                }

                using (CourseDataContext db = new CourseDataContext())
                {

                    DateTime today = DateTime.Today;
                    TimeSpan tm = today - profile.DOB;
                    int age = tm.Days / 365;

                    if (age < 18)
                    {
                        return Content("DateBirth");
                    }

                    tblInternApply internApply = db.tblInternApplies.Where(x => x.InterApllyId == Convert.ToInt64(InterneId)).FirstOrDefault();

                    if (internApply != null)
                    {
                        internApply.Name = profile.Name;
                        internApply.DOB = profile.DOB;
                        internApply.Email = profile.Email;
                        internApply.Twiter = profile.Twiter;
                        internApply.Facebook = profile.Facebook;
                        internApply.Instagram = profile.Instagram;


                        if (profile.HowDoYouKnow != null)
                        {
                            internApply.HowDoYouKnow = profile.HowDoYouKnow;
                        }

                        if (profile.YearOfPassing != null)
                        {
                            internApply.YearOfPassing = profile.YearOfPassing;
                        }

                        if (profile.Comments != null)
                        {
                            internApply.Comments = profile.Comments;
                        }

                        internApply.CollegeUniv = profile.CollegeUniv;
                        internApply.Qualification = profile.Qualification;

                        if (postedfile != null)
                        {
                            var filename = Guid.NewGuid().ToString() + Path.GetExtension(postedfile.FileName);
                            internApply.Image = filename;
                            // var path = Server.MapPath("~/ProjectImages/");
                            var path = Server.MapPath("~/Intern/Images/");
                            var ImgPath = path + filename;
                            postedfile.SaveAs(ImgPath);
                        }
                        db.SubmitChanges();
                    }

                    //Int64 InternId = InternshipId; 

                    if (InternshipId > 0)
                    {
                        //      SITSPL_tblContact tblContact = (from d in db.SITSPL_tblContacts where d.UserId == InternshipId select d).FirstOrDefault();
                        SITSPL_tblContact tblContact = (from d in db.SITSPL_tblContacts where d.UserId == InternshipId select d).FirstOrDefault();
                        if (tblContact != null)
                        {


                            tblContact.UserId = Convert.ToInt64(InternshipId);
                            tblContact.Email = profile.Email;
                            tblContact.Mobile = Phone;
                            tblContact.Address = internContact.Address;
                            tblContact.State = internContact.State;
                            tblContact.City = internContact.City;
                            tblContact.Pincode = internContact.Pincode;
                            if (internContact.Landmark != null)
                            {
                                tblContact.Landmark = internContact.Landmark;
                            }
                            tblContact.LastUpdated = DateTime.Now;
                            tblContact.UpdatedBy = "Admin";
                            db.SubmitChanges();

                        }
                        else
                        {
                            SITSPL_tblContact ins = new SITSPL_tblContact();
                            ins.UserId = Convert.ToInt64(InternshipId);
                            ins.Email = profile.Email;
                            ins.Mobile = Phone;

                            ins.Address = internContact.Address;
                            ins.State = internContact.State;
                            ins.City = internContact.City;
                            ins.Pincode = internContact.Pincode;
                            if (internContact.Landmark != null)
                            {
                                ins.Landmark = internContact.Landmark;
                            }
                            ins.DateCreated = DateTime.Now;
                            ins.CreatedBy = "Admin";
                            db.SITSPL_tblContacts.InsertOnSubmit(ins);
                            db.SubmitChanges();

                        }


                        if (lstDocuments != null)
                        {
                            string InternId = "";
                            if (InternshipId == null)
                            {
                                InternId = Session["InterApllyId"].ToString();
                            }
                            else
                            {
                                InternId = InternshipId.ToString();
                            }

                            for (int i = 0; i < lstDocuments.Count; i++)
                            {

                                SITSPL_tblDocument objDoc = db.SITSPL_tblDocuments.Where(x => x.DocumentId == lstDocuments[i].DocumentId && x.UserId == lstDocuments[i].UserId).FirstOrDefault();
                                if (objDoc != null)
                                {
                                    objDoc.UserId = Convert.ToInt64(InternId);
                                    //  objDoc.UserId = InternshipId;
                                    objDoc.PanNo = "-";
                                    objDoc.DoucmentNo = lstDocuments[i].DoucmentNo;
                                    //     objDoc.DocumentType = "-";
                                    objDoc.DocumentType = lstDocuments[i].DocumentType;
                                    objDoc.DocumentName = lstDocuments[i].DocumentName;
                                    objDoc.LastUpdated = DateTime.Now;
                                    objDoc.UpdatedBy = profile.Name;

                                    db.SubmitChanges();
                                }
                            }
                        }


                        #region Save Resume
                        if (resumePostedfile != null)
                        {
                            var resumePath = Server.MapPath("~/Intern/Resume/");
                            var resumeName = resumePostedfile.FileName;
                            resumePath = resumePath + resumeName;
                            resumePostedfile.SaveAs(resumePath);
                        }
                        #endregion END Save Resume

                        #region Save Aadhaar
                        if (aadhaarPostedfile != null)
                        {
                            var aadhaarPath = Server.MapPath("~/Intern/Aadhaar/");
                            var aadhaarName = aadhaarPostedfile.FileName;
                            aadhaarPath = aadhaarPath + aadhaarName;
                            aadhaarPostedfile.SaveAs(aadhaarPath);
                        }
                        #endregion END Save Aadhaar


                        SITSPL_tblUser user = (from d in db.SITSPL_tblUsers where d.Id == InternshipId && d.UserType == "Intern" select d).FirstOrDefault();
                        if (user != null)
                        {
                            user.UserName = Username;
                            user.UserType = "Intern";
                            user.Id = InternshipId;
                            user.LastUpdated = DateTime.Now;
                            user.UpdatedBy = "Admin";
                            user.IsPublished = true;
                            db.SubmitChanges();
                        }
                    }
                }
                return Content("Updated");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult ChangeInternsPasword(string Email, string CurrentPassword, string NewPassword, string ReTypePassword)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from us in db.SITSPL_tblUsers
                            join intapply in db.tblInternApplies on us.Id equals intapply.InterApllyId
                            where intapply.Email == Email
                            select new
                            {
                                us.Password,
                                us.Id,
                                us.UserName
                            }).FirstOrDefault();

                if (data != null)
                {
                    if (CurrentPassword != data.Password)
                    {
                        return Json("currpasswrong", JsonRequestBehavior.AllowGet);
                    }
                    else if (NewPassword != ReTypePassword)
                    {
                        return Json("currnewpasswrong", JsonRequestBehavior.AllowGet);
                    }

                    else if (CurrentPassword == data.Password && NewPassword == ReTypePassword)
                    {
                        SITSPL_tblUser user = db.SITSPL_tblUsers.Where(x => x.Id == data.Id).FirstOrDefault();
                        if (user != null)
                        {
                            user.UserType = "Intern";
                            user.Id = data.Id;
                            user.Password = NewPassword;
                            user.IsPublished = true;
                            user.LastUpdated = DateTime.Now;
                            user.UpdatedBy = data.UserName;
                            db.SubmitChanges();
                            return Json("passupdated", JsonRequestBehavior.AllowGet);
                        }

                    }

                }
                else
                {
                    return Json("notregistered", JsonRequestBehavior.AllowGet);
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult StudentTutorRegister()
        {
            return View();
        }

        public ActionResult StudentRegister()
        {
            return View();
        }

        public JsonResult GetStudentCode()
        {
            db = new CourseDataContext();
            var Prefix = "Std-";
            var PrefixLen = Prefix.ToString().Length;
            var student = db.SITSPL_tblUsers.ToList();
            int intStudentCount = 0;

            if (student != null)
            {
                intStudentCount = db.SITSPL_tblUsers.Where(x => x.UserType == "Student").ToList().Count();
            }

            var StdCode = "";
            var LenStdCount = intStudentCount.ToString().Length;
            var StdCodeLength = db.SITSPL_tblUsers.Where(x => x.UserType == "Student").ToList().Count() + 1;
            var StdCodeLenBalance = 11 - LenStdCount - PrefixLen;

            if (StdCodeLenBalance > 0)
            {
                if (StdCodeLenBalance.ToString().Length == 3)
                {
                    StdCode = Prefix.PadRight(Prefix.Length + 3, '0') + StdCodeLength.ToString();
                }
                else if (StdCodeLenBalance.ToString().Length == 2)
                {
                    StdCode = Prefix.PadRight(Prefix.Length + 2, '0') + StdCodeLength.ToString();
                }
                else if (StdCodeLenBalance.ToString().Length == 1)
                {
                    StdCode = Prefix.PadRight(Prefix.Length + 1, '0') + StdCodeLength.ToString();
                }
                //var No = StdCode.Substring(7);
                var StudentFinalCode = StdCode;
                var data = StudentFinalCode;
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }


        String encrypted;
        [HttpPost]
        public ContentResult InsertStudent(SITSPL_tblStudentProfile create, HttpPostedFileBase postedfile)
        {
            try
            {
                Int64 intStudentId = 0;
                using (CourseDataContext db = new CourseDataContext())
                {
                    var data = db.SITSPL_tblStudentProfiles.Count();
                    var count = data + 1;
                    var tempregno = DateTime.Now.ToString() + count;
                    DateTime today = DateTime.Today;
                    DateTime dob = DateTime.Parse(Convert.ToDateTime(create.DOB).ToShortDateString());
                    TimeSpan tm = today - dob;
                    int age = tm.Days / 365;

                    string User = create.Username;
                    var data4 = db.SITSPL_tblStudentProfiles.Select(x => new { x.Username }).ToList();
                    if (data4 != null)
                    {
                        var Users = data4.Select(x => x.Username);
                        if (User != null && Users.Contains(User, StringComparer.OrdinalIgnoreCase))
                        {
                            return Content("Useralready");
                        }
                    }

                    var emailexits = db.SITSPL_tblStudentProfiles.Where(x => x.Email == create.Email).Select(x => new { x.Email }).Count();

                    // Remove comment
                    if (emailexits > 0)
                    {
                        return Content("Emailalready");
                    }

                    if (age < 18)
                    {
                        return Content("Dob");
                    }
                    else
                    {
                        create.TemporaryRegNo = tempregno;
                        create.PaymentStatus = "Pending";
                        db.SITSPL_tblStudentProfiles.InsertOnSubmit(create);
                        db.SubmitChanges();
                        intStudentId = create.StudentId;

                        if (intStudentId > 0)
                        {
                            var Prefix = "Std-";
                            var PrefixLen = Prefix.ToString().Length;
                            var data2 = db.SITSPL_tblUsers.Where(x => x.UserType == "Student").ToList().Count();
                            var StdCode = "";
                            var LenStdCount = data2.ToString().Length;
                            var StdCodeLength = db.SITSPL_tblUsers.Where(x => x.UserType == "Student").ToList().Count() + 1;
                            var StdCodeLenBalance = 11 - LenStdCount - PrefixLen;

                            if (StdCodeLenBalance > 0)
                            {
                                if (StdCodeLenBalance.ToString().Length == 3)
                                {
                                    StdCode = Prefix.PadRight(Prefix.Length + 3, '0') + StdCodeLength.ToString();
                                }
                                else if (StdCodeLenBalance.ToString().Length == 2)
                                {
                                    StdCode = Prefix.PadRight(Prefix.Length + 2, '0') + StdCodeLength.ToString();
                                }
                                else if (StdCodeLenBalance.ToString().Length == 1)
                                {
                                    StdCode = Prefix.PadRight(Prefix.Length + 1, '0') + StdCodeLength.ToString();
                                }
                                //var No = StdCode.Substring(7);
                                var StudentFinalCode = StdCode;
                                var AutoId = StdCodeLength;
                                var AutomaticId = AutoId.ToString();

                                SITSPL_tblUser user = new SITSPL_tblUser();
                                user.UserName = create.Username;
                                user.Password = create.Password;
                                user.UserType = "Student";
                                user.Id = intStudentId;

                                user.UserPrefix = Prefix;
                                user.AutoId = AutomaticId;
                                user.CompletedId = StudentFinalCode;

                                user.DateCreated = DateTime.Now;
                                user.CreatedBy = "Admin";
                                user.IsPublished = true;
                                db.SITSPL_tblUsers.InsertOnSubmit(user);
                                db.SubmitChanges();
                            }

                            // Insert Course Details for BRand Tutor Students

if(create.CourseStructureId != null && create.StudentType == "BrandStudent") { 
                            var CourseStructureId = create.CourseStructureId;

                            //                            select StructureId, CourseId, Fees, DiscountPercent, NetAmount, ValidFrom,
                            //ValidTo, JoiningMonth, JoiningDate, DurationId, TutorId
                            //from SITSPL_tblCourseStructure where StructureId = 3086;

                            var coursedetail = (from cs in db.SITSPL_tblCourseStructures
                                                join dur in db.tblDurations
                                                on cs.DurationId equals dur.DurationId
                                                join cour in db.SITSPL_tblCourses
                                                on cs.CourseId equals cour.CourseId
                                                where cs.StructureId == CourseStructureId
                                                select new
                                                {
                                                    cs.StructureId,
                                                    cs.CourseId,
                                                    cs.Fees,
                                                    cs.DiscountPercent,
                                                    cs.NetAmount,
                                                    cs.ValidFrom,
                                                    cs.ValidTo,
                                                    cs.JoiningMonth,
                                                    cs.JoiningDate,
                                                    cs.DurationId,
                                                    dur.DurationName,
                                                    cs.TutorId
                                                }).FirstOrDefault();

                            //&& x.Duration == coursedetail.DurationName
   SITSPL_tblCourseDetail cd = db.SITSPL_tblCourseDetails.Where(x => x.StudentId == intStudentId && x.CourseId == coursedetail.CourseId && x.Duration == coursedetail.DurationId).FirstOrDefault();
                            if (cd != null)
                            {
                                cd.StudentId = intStudentId;
                                cd.Month = "-";
                                cd.JoiningDate = "-";
                                cd.LastUpdated = DateTime.Now;
                                cd.UpdatedBy = "Admin";
                                db.SubmitChanges();

                            }
                            else
                            {
                                int? DurationId = coursedetail.DurationId;

                                SITSPL_tblCourseDetail course = new SITSPL_tblCourseDetail();
                                course.CourseId = Convert.ToInt32(coursedetail.CourseId);
                                course.Duration = DurationId.Value;
                                course.StudentId = intStudentId;
                                course.StudentEmail = create.Email;
                                //DateTime dtValidFrom = coursedetail.ValidFrom;
                                //DateTime dtValidTo = coursedetail.ValidTo;
                                //cd.CourseValidFrom = dtValidFrom;

                                course.CourseValidFrom = coursedetail.ValidFrom;
                                course.CourseValidTo = coursedetail.ValidTo;
                                course.DiscountPercent = coursedetail.DiscountPercent;

                                decimal? Feesvalue = coursedetail.Fees;
                                decimal? NetAmountvalue = coursedetail.NetAmount;

                                course.Fees = Feesvalue ?? 0;
                                course.FeesWithDiscount = coursedetail.NetAmount;
                                course.Month = "0";
                                course.JoiningDate = "-";
                                course.NetAmountToPay = NetAmountvalue ?? 0;
                                course.DateCreated = DateTime.Now;
                                course.CreatedBy = "Admin";
                                db.SITSPL_tblCourseDetails.InsertOnSubmit(course);
                                db.SubmitChanges();
                            }

                            }

                            //var objCourse = (from d in db.SITSPL_tblCourseDetails where d.StudentId == intStudentId && d.CourseId == Convert.ToInt32(multicourse[i].Course) && d.Duration == multicourse[i].Duration.TrimStart() select d).FirstOrDefault();
                            //if (objCourse != null)
                            //{

                            //    objCourse.StudentId = Id;
                            //    objCourse.Month = multicourse[i].Month;
                            //    objCourse.JoiningDate = multicourse[i].JoiningDate;
                            //    objCourse.LastUpdated = DateTime.Now;
                            //    objCourse.UpdatedBy = "Admin";
                            //    db.SubmitChanges();
                            //}
                            //else
                            //{
                            //    SITSPL_tblCourseDetail cd = new SITSPL_tblCourseDetail();
                            //    cd.CourseId = Convert.ToInt32(multicourse[i].Course);
                            //    cd.Duration = multicourse[i].Duration.TrimStart();
                            //    cd.StudentId = Id;
                            //    cd.StudentEmail = Email;
                            //    DateTime dtValidFrom = DateTime.ParseExact(multicourse[i].ValidFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            //    DateTime dtValidTo = DateTime.ParseExact(multicourse[i].ValidTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            //    cd.CourseValidFrom = dtValidFrom;
                            //    cd.CourseValidTo = dtValidTo;
                            //    cd.DiscountPercent = multicourse[i].Discount;
                            //    cd.Fees = multicourse[i].Fees;
                            //    cd.FeesWithDiscount = multicourse[i].FeesWithDiscount;
                            //    cd.Month = multicourse[i].Month;
                            //    cd.JoiningDate = multicourse[i].JoiningDate;
                            //    cd.NetAmountToPay = multicourse[i].NetAmount;
                            //    cd.DateCreated = DateTime.Now;
                            //    cd.CreatedBy = "Admin";
                            //    db.SITSPL_tblCourseDetails.InsertOnSubmit(cd);
                            //    db.SubmitChanges();
                            //}


                        }

                        SendMailToStudents(create.Name, create.Email, create.TemporaryRegNo);
                        SendStudentMailToAdmin(create.Name, create.Email, create.TemporaryRegNo);
                        var path = Server.MapPath("~/ProjectImages/");
                        var filename = postedfile.FileName;
                        var ImgPath = path + filename;
                        postedfile.SaveAs(ImgPath);
                    }
                }
                return Content(intStudentId.ToString());
            }
            catch (Exception ex)
            {
                return Content("Error");
            }
        }


        public void SendMailToStudents(string Name, string Email, string TemporaryRegNo)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(Email);

            //  mail.From = new MailAddress("atul91915@gmail.com");

            mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);

            mail.Subject = "Registration Regarding";
            string Body = "Hello " + Name + " Your Temporary Registration No. is " + TemporaryRegNo + " You are sucessfully registered as Student";
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"],
                ConfigurationManager.AppSettings["smtpPass"]);
            // smtp.Credentials = new System.Net.NetworkCredential("username", "password"); // Enter seders User name and password  
            smtp.EnableSsl = true;
            smtp.Send(mail);

        }


        public void SendStudentMailToAdmin(string Name, string Email, string TemporaryRegNo)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(Email);
            //  mail.From = new MailAddress("atul91915@gmail.com");
            mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
            mail.Subject = "Student Registration Regarding";
            string Body = "Hello " + Name + " a student with Temporary Registration No. " + TemporaryRegNo + " is sucessfully registered as Student";
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"],
                ConfigurationManager.AppSettings["smtpPass"]);
            // smtp.Credentials = new System.Net.NetworkCredential("username", "password"); // Enter seders User name and password  
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }

        public ActionResult ForgetPassword()
        {

            return View();
        }


        public JsonResult VerifyStudentMailSendPassword(string Email)
        {
            try
            {
                db = new CourseDataContext();
                var data = db.SITSPL_tblStudentProfiles.Where(x => x.Email == Email).Count();

                if (data > 0)
                {

                    bool result = false;
                    var pass = db.SITSPL_tblStudentProfiles.Where(x => x.Email == Email).Select(x => x.Password).FirstOrDefault();

                    //Random random = new Random();
                    //String r = random.Next(0, 1000000).ToString("D6");
                    //Session["OTP"] = r;
                    MailMessage mail = new MailMessage();
                    mail.To.Add(Email);
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                    mail.Subject = "Password Regarding";
                    string Body = "Hello " + Email + " Your Password is: " + pass;
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"],
                        ConfigurationManager.AppSettings["smtpPass"]);
                    // smtp.Credentials = new System.Net.NetworkCredential("username", "password"); // Enter seders User name and password  
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    //  SendMailToStudent(Name,Email);
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NotRegistered", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ForgetPasswordForTutor()
        {
            return View();
        }


        public JsonResult VerifyTutorMailSendPassword(string Email)
        {
            try
            {
                db = new CourseDataContext();
                var data = db.SITSPL_tblTutors.Where(x => x.TutorEmail == Email).Count();

                if (data > 0)
                {
                    bool result = false;
                    //   var pass = db.SITSPL_tblStudentProfiles.Where(x => x.Email == Email).Select(x => x.Password).FirstOrDefault();

                    var pass = (from tut in db.SITSPL_tblTutors
                                join us in db.SITSPL_tblUsers
                                on tut.TutorId equals us.Id
                                where tut.TutorEmail == Email
                                select new
                                {
                                    us.Password
                                }).FirstOrDefault();


                    MailMessage mail = new MailMessage();
                    mail.To.Add(Email);

                    //  mail.From = new MailAddress("atul91915@gmail.com");

                    mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);

                    mail.Subject = "Password Regarding";
                    string Body = "Hello " + Email + " Your Password is: " + pass.Password;
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"],
                        ConfigurationManager.AppSettings["smtpPass"]);
                    // smtp.Credentials = new System.Net.NetworkCredential("username", "password"); // Enter seders User name and password  
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    //  SendMailToStudent(Name,Email);
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NotRegistered", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }




        [HttpPost]
        public JsonResult ChangeStudentsProfilePassword(string Email, string CurrentPassword, string NewPassword, string ReTypePassword)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from us in db.SITSPL_tblUsers
                            join std in db.SITSPL_tblStudentProfiles on us.Id equals std.StudentId
                            where std.Email == Email && us.Id == Convert.ToInt64(Session["StudentId"].ToString())
                            select new
                            {
                                us.Password,
                                us.Id,
                                us.UserName
                            }).FirstOrDefault();

                if (data != null)
                {
                    if (CurrentPassword != data.Password)
                    {
                        return Json("currpasswrong", JsonRequestBehavior.AllowGet);
                    }
                    else if (NewPassword != ReTypePassword)
                    {
                        return Json("currnewpasswrong", JsonRequestBehavior.AllowGet);
                    }

                    else if (CurrentPassword == data.Password && NewPassword == ReTypePassword)
                    {
                        SITSPL_tblUser user = db.SITSPL_tblUsers.Where(x => x.Id == data.Id).FirstOrDefault();
                        if (user != null)
                        {
                            user.UserType = "Student";
                            user.Id = data.Id;
                            user.Password = NewPassword;
                            user.IsPublished = true;
                            user.LastUpdated = DateTime.Now;
                            user.UpdatedBy = data.UserName;
                            db.SubmitChanges();
                            return Json("passupdated", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json("notregistered", JsonRequestBehavior.AllowGet);
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }






        string decrypted;


        public ActionResult TutorRegister()
        {

            return View();
        }


        [HttpPost]
        public ContentResult InsertTutor(SITSPL_tblTutor create, HttpPostedFileBase postedfile)
        {
            //return null;
            try
            {
                Int64 intTutorId = 0;
                using (CourseDataContext db = new CourseDataContext())
                {
                    DateTime today = DateTime.Today;
                    DateTime dob = DateTime.Parse(Convert.ToDateTime(create.TutorDOB).ToShortDateString());
                    TimeSpan tm = today - dob;
                    int age = tm.Days / 365;

                    if (age < 18)
                    {
                        return Content("Dob");
                    }

                    else
                    {
                        create.TutorType = "Tutor";
                        db.SITSPL_tblTutors.InsertOnSubmit(create);
                        db.SubmitChanges();
                        intTutorId = create.TutorId;
                        if (intTutorId > 0)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                SITSPL_tblDocument doc = new SITSPL_tblDocument();
                                doc.UserId = intTutorId;
                                doc.DocumentName = "-";
                                doc.DoucmentNo = "-";
                                doc.DateCreated = DateTime.Now;
                                doc.CreatedBy = "Admin";
                                db.SITSPL_tblDocuments.InsertOnSubmit(doc);
                                db.SubmitChanges();
                            }

                            SendMailToTutor(create.TutorName, create.TutorEmail);
                            SendTutorMailToAdmin(create.TutorName, create.TutorEmail);
                            var path = Server.MapPath("~/ProjectImages/");
                            var filename = postedfile.FileName;
                            var ImgPath = path + filename;
                            postedfile.SaveAs(ImgPath);
                        }
                    }
                }
                return Content(intTutorId.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public void SendMailToTutor(string Name, string Email)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(Email);
            mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
            mail.Subject = "Registration Regarding";
            string Body = "Hello " + Name + " You are sucessfully registered as Tutor";
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"],
                ConfigurationManager.AppSettings["smtpPass"]);
            // smtp.Credentials = new System.Net.NetworkCredential("username", "password"); // Enter seders User name and password  
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }


        public void SendTutorMailToAdmin(string Name, string Email)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(Email);
            //  mail.From = new MailAddress("atul91915@gmail.com");
            mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
            mail.Subject = "Registration Regarding";
            string Body = "Hello " + Name + " is sucessfully registered as Tutor in our Course";
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"],
                ConfigurationManager.AppSettings["smtpPass"]);
            // smtp.Credentials = new System.Net.NetworkCredential("username", "password"); // Enter seders User name and password  
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }

        // int StructureId

        [HttpPost]
        public ContentResult UpdateStudentProfile(decimal? Total, int Id, string Name, DateTime DOB, string Remarks, string Mobile, string Username, string Password, string Email, string Fblink, string Instalink,
            string Twitterlink, HttpPostedFileBase postedfile, SITSPL_tblContact objContact, List<MultipleCourses> multicourse, List<StudentWithCourse> sWc)
        {
            try
            {
                using (CourseDataContext db = new CourseDataContext())
                {
                    SITSPL_tblStudentProfile studentProfile = db.SITSPL_tblStudentProfiles.Where(x => x.StudentId == Id).SingleOrDefault();
                    DateTime today = DateTime.Today;
                    TimeSpan tm = today - DOB;
                    int age = tm.Days / 365;

                    if (age < 18)
                    {
                        return Content("DateBirth");
                    }
                    else
                    {
                        if (studentProfile != null)
                        {
                            studentProfile.Name = Name;
                            studentProfile.DOB = DOB;
                            studentProfile.Username = Username;
                            studentProfile.Email = Email;
                            studentProfile.Fblink = Fblink;
                            studentProfile.Instalink = Instalink;
                            studentProfile.Twitterlink = Twitterlink;
                            studentProfile.Mobile = Mobile;
                            studentProfile.PaymentMode = "Cash";

                            if (Remarks != null)
                            {
                                studentProfile.Remarks = Remarks;
                            }

                            if (Total != null)
                            {
                                studentProfile.NetAmount = Total;
                            }

                            if (postedfile != null)
                            {
                                var filename = Guid.NewGuid().ToString() + Path.GetExtension(postedfile.FileName);
                                studentProfile.ProfileImage = filename;
                                var path = Server.MapPath("~/ProjectImages/");
                                var ImgPath = path + filename;
                                postedfile.SaveAs(ImgPath);
                            }
                            db.SubmitChanges();

                            #region Add Contact Details
                            Int64 intStudentId = Id;
                            if (intStudentId > 0)
                            {
                                //var isContact = (from d in db.SITSPL_tblContacts where d.UserId == Id select d).Any();


                                //if (isContact)
                                SITSPL_tblContact contc = (from d in db.SITSPL_tblContacts where d.UserId == Id select d).FirstOrDefault();
                                if (contc != null)
                                {

                                    contc.UserId = Id;
                                    contc.Email = Email;
                                    contc.Mobile = Mobile;
                                    contc.Address = objContact.Address;
                                    contc.State = objContact.State;
                                    contc.City = objContact.City;
                                    contc.Pincode = objContact.Pincode;
                                    contc.Landmark = objContact.Landmark;
                                    contc.LastUpdated = DateTime.Now;
                                    contc.UpdatedBy = "Admin";
                                    db.SubmitChanges();
                                }
                                else
                                {
                                    SITSPL_tblContact ins = new SITSPL_tblContact();
                                    if (objContact.Address != null && objContact.State != null && objContact.City != null && objContact.Pincode != null)
                                    {
                                        ins.UserId = Id;
                                        ins.Email = Email;
                                        ins.Mobile = Mobile;

                                        if (objContact.Address != null)
                                        {
                                            ins.Address = objContact.Address;
                                        }

                                        if (objContact.State != null)
                                        {
                                            ins.State = objContact.State;
                                        }

                                        if (objContact.City != null)
                                        {
                                            ins.City = objContact.City;
                                        }

                                        if (objContact.Pincode != null)
                                        {
                                            ins.Pincode = objContact.Pincode;
                                        }

                                        if (objContact.Landmark != null)
                                        {
                                            ins.Landmark = objContact.Landmark;
                                        }

                                        ins.DateCreated = DateTime.Now;
                                        ins.CreatedBy = "Admin";
                                        db.SITSPL_tblContacts.InsertOnSubmit(ins);
                                        db.SubmitChanges();
                                    }
                                }
                                if (multicourse != null)
                                {
                                    for (int i = 0; i < multicourse.Count; i++)
                                    {
                                        var objCourse = (from d in db.SITSPL_tblCourseDetails where d.StudentId == Id && d.CourseId == Convert.ToInt32(multicourse[i].Course) && d.Duration == multicourse[i].Duration select d).FirstOrDefault();
                                        if (objCourse != null)
                                        {

                                            objCourse.StudentId = Id;
                                            objCourse.Month = multicourse[i].Month;
                                            objCourse.JoiningDate = multicourse[i].JoiningDate;
                                            objCourse.LastUpdated = DateTime.Now;
                                            objCourse.UpdatedBy = "Admin";
                                            db.SubmitChanges();
                                        }
                                        else
                                        {
                                            SITSPL_tblCourseDetail cd = new SITSPL_tblCourseDetail();
                                            cd.CourseId = Convert.ToInt32(multicourse[i].Course);
                                            cd.Duration = multicourse[i].Duration;
                                            cd.StudentId = Id;
                                            cd.StudentEmail = Email;
                                            DateTime dtValidFrom = DateTime.ParseExact(multicourse[i].ValidFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            DateTime dtValidTo = DateTime.ParseExact(multicourse[i].ValidTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            cd.CourseValidFrom = dtValidFrom;
                                            cd.CourseValidTo = dtValidTo;
                                            cd.DiscountPercent = multicourse[i].Discount;
                                            cd.Fees = multicourse[i].Fees;
                                            cd.FeesWithDiscount = multicourse[i].FeesWithDiscount;
                                            cd.Month = multicourse[i].Month;
                                            cd.JoiningDate = multicourse[i].JoiningDate;
                                            cd.NetAmountToPay = multicourse[i].NetAmount;
                                            cd.DateCreated = DateTime.Now;
                                            cd.CreatedBy = "Admin";
                                            db.SITSPL_tblCourseDetails.InsertOnSubmit(cd);
                                            db.SubmitChanges();
                                        }
                                    }
                                }
                            }
                            #endregion END Contact Details
                        }
                    }
                }
                return Content("Updated");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    


        // Student ChangePassword

        public ActionResult ChangePassword()
        {
            if (Session["StudentId"] != null)
            {
                string str = Session["StudentId"].ToString();
            }

            if (Session["StudentId"] == null)
            {
                return RedirectToAction("StudentLogin", "User");
            }

            return View();
        }


        [HttpPost]
        public JsonResult StudentExistsOrNot(string Name)
        {
            try
            {
                bool result = false;
                db = new CourseDataContext();
                //  var data = db.SITSPL_tblStudentProfiles.Where(x => x.Username == Name).Count();
                //  var data = db.SITSPL_tblStudentProfiles.Where(x => x.Username.Contains(Name)).Count();

                string User = Name;
                var data = db.SITSPL_tblStudentProfiles.Select(x => new { x.Username }).ToList();
                var Users = data.Select(x => x.Username);
                if (User != null && Users.Contains(User, StringComparer.OrdinalIgnoreCase))
                {
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult ChangePassword(string strOldPassword, string strNewPassword)
        {
            try
            {
                Int64 intStudentId = 0;
                if (Session["StudentId"].ToString() != null)
                {
                    Int64.TryParse(Session["StudentId"].ToString(), out intStudentId);
                }
                var data = (from d in db.SITSPL_tblStudentProfiles where d.StudentId == intStudentId && d.Password == strOldPassword select d).SingleOrDefault();
                if (data != null)
                {
                    data.Password = strNewPassword;
                    db.SubmitChanges();
                    //PasswordUpdated
                    return Json(intStudentId.ToString(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("OldPasswordNotMatch", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult ChangeTutorPassword()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ChangeTutorPassword(string strOldPassword, string strNewPassword)
        {
            try
            {
                Int64 intTutorId = 0;
                if (Session["TutorId"].ToString() != null)
                {
                    Int64.TryParse(Session["TutorId"].ToString(), out intTutorId);
                }

                var data = (from d in db.SITSPL_tblUsers
                            where d.Id == intTutorId &&
                            d.Password == strOldPassword
                            select d).SingleOrDefault();

                if (data != null)
                {
                    data.Password = strNewPassword;
                    db.SubmitChanges();
                    return Json(intTutorId.ToString(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("OldPasswordNotMatch", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // Change Intern Student Password

        public ActionResult ChangeInternPassword()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ChangeInternStudentPassword(string strOldPassword, string strNewPassword)
        {
            try
            {
                Int64 intInternStudentId = 0;
                if (Session["InterApllyId"].ToString() != null)
                {
                    Int64.TryParse(Session["InterApllyId"].ToString(), out intInternStudentId);
                }

                var data = (from d in db.SITSPL_tblUsers
                            where d.Id == intInternStudentId &&
                            d.Password == strOldPassword
                            select d).SingleOrDefault();

                if (data != null)
                {
                    data.Password = strNewPassword;
                    db.SubmitChanges();
                    return Json(intInternStudentId.ToString(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("InternStudentOldPasswordNotMatch", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AdminChangePassword()
        {
            return View();
        }


        public ActionResult ChangeUsersPassword()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ChangeStudentPasswordByLink(string Email)
        {
            try
            {
                bool result = false;
                db = new CourseDataContext();

                Session["Email"] = Email;

                var data = (from d in db.SITSPL_tblUsers
                            join std in db.SITSPL_tblStudentProfiles
                            on d.Id equals std.StudentId
                            where std.Email == Email
                            select new { std.Email }).Count();

                if (data > 0)
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(Email);

                    //  mail.From = new MailAddress("atul91915@gmail.com");
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);

                    mail.Subject = "Change Password By Link";
                    string Body = "Your message : <a href='http://localhost:50861/User/ChangeStudentPassword'>Change Password</a>";
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"],
                        ConfigurationManager.AppSettings["smtpPass"]);
                    // smtp.Credentials = new System.Net.NetworkCredential("username", "password"); // Enter seders User name and password  
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    result = true;

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("notregistered", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ChangeStudentPassword()
        {

            return View();
        }


        [HttpPost]
        public JsonResult ChangeCourseStudentPassword(string strOldPassword,string strNewPassword)
        {
            try
            {
                Int64 intStudentId = 0;
                if (Session["StudentId"].ToString() != null)
                {
                    Int64.TryParse(Session["StudentId"].ToString(), out intStudentId);
                }

                var data = (from d in db.SITSPL_tblUsers
                            where d.Id == intStudentId &&
                            d.Password == strOldPassword
                            select d).FirstOrDefault();

                if (data != null)
                {
                    data.Password = strNewPassword;
                    db.SubmitChanges();
                    return Json(intStudentId.ToString(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("OldPasswordNotMatch", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message,JsonRequestBehavior.AllowGet);
            }
        }

        // Change Course Students Password
       // string NewPassword, string ConfirmPassword
        public JsonResult ChangeStudentsPassword(string NewPassword,string ConfirmPassword)
        {
            try
            {
                string email = "";
                if (Session["Email"] != null)
                {
                    email = Session["Email"].ToString();
                }


                var data = (from d in db.SITSPL_tblUsers
                            join std in db.SITSPL_tblStudentProfiles
                            on d.Id equals std.StudentId
                            where std.Email == email
                            select new { d.Id }).FirstOrDefault();



                if (data != null)
                {
                    SITSPL_tblUser user = (from d in db.SITSPL_tblUsers where d.Id == data.Id select d).FirstOrDefault();
                    if (user != null)
                    {

                        if (NewPassword != "" && ConfirmPassword != "" && NewPassword.ToString() == ConfirmPassword.ToString())
                        {
                            user.Password = NewPassword;
                            db.SubmitChanges();
                            return Json("passchanged", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("passmatch", JsonRequestBehavior.AllowGet);
                        }


                    }

                }
                else
                {
                    return Json("NotRegistered", JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        #region Tutor Profile (Public Profile)
        public ActionResult TutorProfile()
        {
            return View();
        }

        [HttpPost]
        public JsonResult TutorPublicProfileById(Int64 intTutorId)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.SITSPL_tblTutors
                            where d.TutorId == intTutorId
                            select new
                            {
                                d
                            }).SingleOrDefault();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion END Tutor 


        public JsonResult GetCourseName(int CourseId)
        {
            try
            {
                db = new CourseDataContext();
                var CourseName = db.SITSPL_tblCourses.Where(x => x.CourseId == CourseId).Select(x => new { x.CourseName }).FirstOrDefault();
                return Json(CourseName, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Application Error : " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult InternshipListDetails()
        {
            return View();
        }


        #region Internship List
        public ActionResult InternshipList() => View();

        public JsonResult InternshipListDetail()
        {
            try
            {
                var data = (from d in db.SITSPL_tblInternshipStructures
                            join d1 in db.SITSPL_tblInternships on d.InternshipId equals d1.InternshipId
                            join dur in db.tblDurations on d.DurationId equals dur.DurationId
                            where d.IsPublished == true
                            select new
                            {
                                d.InternshipType,
                                d1.InternshipName,
                                d.InternStructureId,
                                d.TotalAmount,
                                d.StipenedMoney,
                                d.DurationId,
                                dur.DurationName
                            }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion


        #region  InternshipStructure Based On Id Public View Internship List Details get data

        public JsonResult InternshipStructureDataBasedOnId(int id)
        {
            try
            {
                db = new CourseDataContext();
                var stru = (from d in db.SITSPL_tblInternships
                            join s in db.SITSPL_tblInternshipStructures on d.InternshipId equals s.InternshipId
                            join du in db.tblDurations on s.DurationId equals du.DurationId
                            where s.InternStructureId == id
                            select new
                            {
                                s,
                                du,
                                d
                            }).ToList();

                var structure = stru.Select(x => new
                {
                    x.s.InternshipId,
                    x.d.InternshipName,
                    x.s.InternStructureId,
                    LastApplyDate = x.s.LastApplyDate.HasValue ? x.s.LastApplyDate.Value.ToString("dd/MM/yyyy") : null,
                    x.s.DurationMonths,
                    x.s.TotalAvailableSeat,
                    x.s.InternshipType,
                    x.s.Fees,
                    x.s.Discount,
                    x.s.FeeAfterDiscount,
                    x.s.TotalAmount,
                    ValidFrom = x.s.ValidFrom.HasValue ? x.s.ValidFrom.Value.ToString("dd/MM/yyyy") : null,
                    ValideTo = x.s.ValidTo.HasValue ? x.s.ValidTo.Value.ToString("dd/MM/yyyy") : null,
                    x.s.DurationId,
                    x.du.DurationName
                });

                var bullets = (from d in db.SITSPL_tblInternshipStructures
                               join bullet in db.tblInternBullets on d.InternStructureId equals bullet.InternshipId
                               where d.IsPublished == true
                               && bullet.InternshipId == id
                               select new
                               {
                                   bullet.BulletId,
                                   bullet.BulletPoint,
                                   bullet.IsPublished
                               }).ToList();

                var skillSet = (from d in db.SITSPL_tblInternshipStructures
                                join skills in db.tblInternSkillReqs on d.InternStructureId equals skills.InternshipId

                                where d.IsPublished == true
                                && skills.InternshipId == id
                                select new
                                {
                                    skills.InternSkillId,
                                    skills.CourseName,
                                    skills.IsPublished
                                }).ToList();


                var wca = (from d in db.SITSPL_tblInternshipStructures
                           join w in db.tblInternWhoCanApplies on d.InternStructureId equals w.InternshipId
                           where d.IsPublished == true
                           && w.InternshipId == id
                           select new
                           {
                               w.InternApplyId,
                               w.ApplyPoint,
                               w.IsPublished
                           }).ToList();

                var perks = (from d in db.SITSPL_tblInternshipStructures
                             join prk in db.tblInternPerks on d.InternStructureId equals prk.InternshipId
                             where d.IsPublished == true
                             && prk.InternshipId == id
                             select new
                             {
                                 prk.InternPerkId,
                                 prk.PerkName,
                                 prk.IsPublished
                             }).ToList();

                var activity = (from d in db.SITSPL_tblInternshipStructures
                                join act in db.tblInternActivities on d.InternStructureId equals act.InternshipId
                                where act.IsPublished == true
                                && act.InternshipId == id
                                select new
                                {
                                    act.InternActivityId,
                                    act.ActivityType,
                                    act.ActivityPoint,
                                    act.IsPublished
                                }).ToList();


                //int? InternshipsId = (from d in db.SITSPL_tblInternships
                //                      join s in db.SITSPL_tblInternshipStructures on d.InternshipId equals s.InternshipId
                //                      join du in db.tblDurations on s.DurationId equals du.DurationId
                //                      where s.InternStructureId == id
                //                      select new
                //                      {
                //                          s.InternshipId
                //                      }).ToList();

                //var Content = (from cont in db.tblContents
                //               where cont.InternCourseId == InternshipsId
                //               select new
                //               {
                //                   cont.ContentId,
                //                   cont.InternCourseId
                //               }).ToList();

                //var Content = (from cont in db.tblContents
                //               join intern in db.SITSPL_tblInternships on cont.InternCourseId equals intern.InternshipId
                //               join intstr in db.SITSPL_tblInternshipStructures on intern.InternshipId equals intstr.InternshipId
                //               where cont.InternCourseId == InternshipsId && intstr.InternStructureId == id
                //               select new
                //               {
                //                   cont.ContentId
                //               });


                var Content = (from cont in db.tblContents
                               join intern in db.SITSPL_tblInternships on cont.InternCourseId equals intern.InternshipId
                               join intstr in db.SITSPL_tblInternshipStructures on intern.InternshipId equals intstr.InternshipId
                               where intstr.InternStructureId == id
                               select new
                               {
                                   cont.ContentId,
                                   cont.InternCourseId
                               }).FirstOrDefault();



                if (Content != null)
                {
                    var Heading = (from cont in db.tblContents
                                   where cont.ContentId == Convert.ToInt64(Content.ContentId) && cont.InternCourseId == Content.InternCourseId
                                   select new
                                   {
                                       cont.ContentId,
                                       cont.CourseContentHeading,
                                       cont.IsSubHeading,
                                       cont.SubHeading,
                                       cont.ShortDescription,
                                       cont.DateCreated,
                                       cont.CourseType
                                   }).ToList();

                    var description = (from cont in db.tblContents
                                       join descrip in db.tblCourseDescriptions on
                                       cont.ContentId equals descrip.ContentId
                                       where descrip.ContentId == Convert.ToInt64(Content.ContentId)
                                       select new
                                       {
                                           descrip.DescriptionId,
                                           descrip.Description,
                                           descrip.IsPublished
                                       }).ToList();

                    var prerequisities = (from cont in db.tblContents
                                          join prereq in db.tblCoursePrerequisities on
                                          cont.ContentId equals prereq.ContentId
                                          where prereq.ContentId == Convert.ToInt64(Content.ContentId)
                                          select new
                                          {
                                              prereq.PrerequisiteId,
                                              prereq.PrerequisitePoints,
                                              prereq.IsPublished
                                          }).ToList();


                    var list = new { structure, bullets, skillSet, wca, perks, activity, Heading, description, prerequisities };

                    JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                    var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    var list = new { structure, bullets, skillSet, wca, perks, activity };
                    JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                    var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Course List

        public ActionResult CourseList()
        {
            return View();
        }


        public JsonResult PublicCoursesListDetails()
        {
            try
            {
                var data = (from d in db.SITSPL_tblCourseStructures
                            join d1 in db.SITSPL_tblCourses on d.CourseId equals d1.CourseId
                            join dur in db.tblDurations on d.DurationId equals dur.DurationId
                            where d.IsPublished == true
                            select new
                            {
                                d.DurationName,
                                d1.CourseName,
                                d.StructureId,
                                d.CourseId

                            }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        // Atul Sh. 20 August Changes
        //public JsonResult CourseListDetail()
        //{
        //    try
        //    {
        //        var StudentId = Session["StudentId"].ToString();
        //        var sub = (from std in db.SITSPL_tblStudentProfiles
        //                   join cd in db.SITSPL_tblCourseDetails
        //                   on std.StudentId equals cd.StudentId
        //                   into eGroup
        //                   from stdpro in eGroup.DefaultIfEmpty()
        //                   join c in db.SITSPL_tblCourses
        //                   on stdpro.CourseId equals c.CourseId
        //                   into eGroup2
        //                   from stdpro2 in eGroup2.DefaultIfEmpty()
        //                   where std.StudentId == Convert.ToInt32(StudentId)
        //                   select new
        //                   {
        //                       std,
        //                       stdpro,
        //                       stdpro2
        //                   }).ToList();

        //        var cdCoursedetail = sub.Select(x => x.stdpro).ToList();
        //        var contCourse = sub.Select(x => x.stdpro2).ToList();

        //        if (cdCoursedetail[0] == null && contCourse[0] == null)
        //        {
        //            var list = sub.Select(x => new
        //            {
        //                x.std.StudentId,
        //                x.std.Name,
        //                x.std.NetAmount,
        //                x.std.Email,
        //                x.std.Mobile

        //            }).ToList();

        //            return Json(list, JsonRequestBehavior.AllowGet);
        //        }
        //        else if (cdCoursedetail[0] != null && contCourse[0] != null)
        //        {
        //            var list2 = sub.Select(x => new
        //            {
        //                x.std.StudentId,
        //                x.std.Name,
        //                x.std.NetAmount,
        //                x.std.Email,
        //                x.std.Mobile,
        //                x.stdpro.CourseId,
        //                x.std.StructureId,
        //                x.stdpro2.CourseName,
        //                x.stdpro.Duration,
        //                x.stdpro.JoiningDate,
        //                x.stdpro.Month
        //            }).ToList();
        //            return Json(list2, JsonRequestBehavior.AllowGet);
        //        }
        //        return Json("", JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message, JsonRequestBehavior.AllowGet);
        //    }
        //}




        public JsonResult CourseListDetail()
        {
            try
            {
                var StudentId = Session["StudentId"].ToString();
                var sub = (from std in db.SITSPL_tblStudentProfiles
                           join cd in db.SITSPL_tblCourseDetails
                           on std.StudentId equals cd.StudentId
                           into eGroup
                           from stdpro in eGroup.DefaultIfEmpty()
                           join c in db.SITSPL_tblCourses
                           on stdpro.CourseId equals c.CourseId
                           into eGroup2
                           from stdpro2 in eGroup2.DefaultIfEmpty()
                           join dur in db.tblDurations
                           on stdpro.Duration equals dur.DurationId
                           into eGroup3
                           from stdpro3 in eGroup3.DefaultIfEmpty()
                           where std.StudentId == Convert.ToInt32(StudentId)
                           select new
                           {
                               std,
                               stdpro,
                               stdpro2,
                               stdpro3
                           }).ToList();

                var cdCoursedetail = sub.Select(x => x.stdpro).ToList();
                var contCourse = sub.Select(x => x.stdpro2).ToList();
                var durCourse = sub.Select(x => x.stdpro3).ToList();

                if (cdCoursedetail[0] == null && contCourse[0] == null && durCourse[0] == null)
                {
                    var list = sub.Select(x => new
                    {
                        x.std.StudentId,
                        x.std.Name,
                        x.std.NetAmount,
                        x.std.Email,
                        x.std.Mobile

                    }).ToList();

                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                else if (cdCoursedetail[0] != null && contCourse[0] != null && durCourse[0]!= null)
                {
                    var list2 = sub.Select(x => new
                    {
                        x.std.StudentId,
                        x.std.Name,
                        x.std.NetAmount,
                        x.std.Email,
                        x.std.Mobile,
                        x.stdpro.CourseId,
                        x.std.StructureId,
                        x.stdpro2.CourseName,
                        x.stdpro.Duration,
                        x.stdpro.JoiningDate,
                        x.stdpro.Month,
                        x.stdpro3.DurationName
                    }).ToList();
                    return Json(list2, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion End Course List


        // Courses List

        public ActionResult CoursesList()
        {
            return View();
        }


        // Course List Details

        public ActionResult CoursesListDetails()
        {
            return View();
        }


        // Course List Details Structure

        public JsonResult CourseStructureDataBasedOnId(int id)
        {
            try
            {
                db = new CourseDataContext();
                var stru = (from d in db.SITSPL_tblCourses
                            join s in db.SITSPL_tblCourseStructures on d.CourseId equals s.CourseId
                            where s.StructureId == id
                            select new
                            {
                                s,
                                d,
                            }).ToList();

                var struc = stru.Select(x => new
                {
                    x.s.DurationId,
                    x.s.DurationName,
                    ValidFrom = x.s.ValidFrom.ToString("dd/MM/yyyy"),
                    ValidTo = x.s.ValidTo.ToString("dd/MM/yyyy"),
                    x.s.Fees,
                    x.s.NetAmount,
                    x.d.CourseId,
                    x.d.CourseName,
                }).ToList();

                // Commented By Dilshad A. on 31 Oct 2020
                //var Content = (from cont in db.tblContents
                //               join c in db.SITSPL_tblCourses on cont.CourseId equals c.CourseId
                //               join cs in db.SITSPL_tblCourseStructures on c.CourseId equals cs.CourseId
                //               where cs.StructureId == id
                //               select new
                //               {
                //                   cont.ContentId,
                //                   cont.CourseId
                //               }).FirstOrDefault();



                //if (Content != null)
                //{
                var Heading = (from cont in db.tblContents
                                   //where cont.ContentId == Convert.ToInt64(Content.ContentId) && cont.CourseId == Content.CourseId
                               where cont.CourseId == struc[0].CourseId
                               select new
                               {
                                   cont.ContentId,
                                   cont.CourseContentHeading,
                                   cont.IsSubHeading,
                                   cont.SubHeading,
                                   cont.ShortDescription,
                                   cont.DateCreated,
                                   cont.CourseType
                               }).ToList();

                var description = (from cont in db.tblContents
                                   join descrip in db.tblCourseDescriptions on
                                   cont.ContentId equals descrip.ContentId
                                   //where descrip.ContentId == Convert.ToInt64(Content.ContentId)
                                   where cont.CourseId == struc[0].CourseId && descrip.IsPublished == true
                                   select new
                                   {
                                       descrip.DescriptionId,
                                       descrip.Description,
                                       descrip.IsPublished,
                                       descrip.ContentId
                                   }).ToList();

                var prerequisities = (from cont in db.tblContents
                                      join prereq in db.tblCoursePrerequisities on
                                      cont.ContentId equals prereq.ContentId
                                      //where prereq.ContentId == Convert.ToInt64(Content.ContentId)
                                      where cont.CourseId == struc[0].CourseId && prereq.IsPublished == true
                                      select new
                                      {
                                          prereq.PrerequisiteId,
                                          prereq.PrerequisitePoints,
                                          prereq.IsPublished
                                      }).ToList();


                var list = new { struc, Heading, description, prerequisities };

                JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
                return Json(result, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    var list = new { struc };
                //    JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                //    var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}



            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        #region Tutor List
        public ActionResult TutorList() => View();

        public JsonResult TutorListDetail()
        {
            db = new CourseDataContext();
            var data = db.SITSPL_tblTutors.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion End

        #region Tutor Login BY Dilshad A.
        public ActionResult TutorLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TutorLogin(SITSPL_tblUser objUser)
        {
            try
            {
                var data = (from d in db.SITSPL_tblUsers
                            where d.UserName == objUser.UserName && d.Password == objUser.Password
                            join d1 in db.SITSPL_tblTutors on d.Id equals d1.TutorId
                            select new
                            {
                                d.UserName,
                                d.UserType,
                                d.UserId,
                                d1.TutorEmail,
                                d1.TutorImage,
                                d1.TutorId
                            }).SingleOrDefault();
                if (data != null)
                {
                    Session["TutorId"] = data.TutorId;
                    Session["User"] = data.UserName;
                    Session["TutorEmail"] = data.TutorEmail;
                    Session["TutorImage"] = data.TutorImage;
                    return RedirectToAction("TutorWelcome", "User");
                }
                else
                {
                    ViewBag.Message = "Wrong Username Or Password";
                    return View();
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion END Tutor Login BY Dilshad A.

        #region TutorWelcome By Dilshad A.
        public ActionResult TutorWelcome()
        {
            return View();
        }
        #endregion

        #region Logout Tutor By Dilshad A.
        public ActionResult LogoutTutor()
        {
            Session["TutorId"] = null;
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("TutorLogin", "User");
        }
        #endregion Logout Tutor By Dilshad A.

        #region Tutor Personal Profile By Dilshad A.
        public ActionResult TutorPersonalProfile()
        {
            return View();
        }

        public ActionResult TutorPersonalProfileDetails()
        {
            try
            {
                Int64 intTutorId = 0;
                if (Session["TutorId"].ToString() != "" && Session["TutorId"].ToString() != null)
                {
                    Int64.TryParse(Session["TutorId"].ToString(), out intTutorId);
                    if (intTutorId > 0)
                    {
                        db = new CourseDataContext();
                        var data = (from d in db.SITSPL_tblTutors
                                    where d.TutorId == intTutorId
                                    select new
                                    {
                                        d.TutorId,
                                        d.TutorImage,
                                        d.TutorName,
                                        d.TutorEmail,
                                        d.TutorContact,
                                        d.TutorExperience,
                                        d.ShortDescription,
                                        d.LongDescription,
                                        TutorDOB = d.TutorDOB
                                    }).SingleOrDefault();

                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return RedirectToAction("TutorLogin", "User");
                }

                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Tutor Personal Profile By Dilshad A.    

        #region  TutorPersonalProfile Update    
        [HttpPost]
        public JsonResult UpdateTutorPersonalProfile(int Id, SITSPL_tblTutor update, HttpPostedFileBase postedfile)
        {
            try
            {
                using (CourseDataContext db = new CourseDataContext())
                {
                    SITSPL_tblTutor tutorProfile = db.SITSPL_tblTutors.Where(x => x.TutorId == Id).SingleOrDefault();
                    //   DateTime today = DateTime.Today;
                    //   TimeSpan tm = today - DOB;
                    //   int age = tm.Days / 365;

                    //    if (age < 18)
                    //     {
                    //         return Content("DateBirth");
                    //     }
                    //     else
                    //     {
                    if (tutorProfile != null)
                    {
                        tutorProfile.TutorName = update.TutorName;
                        tutorProfile.TutorContact = update.TutorContact;
                        tutorProfile.TutorEmail = update.TutorEmail;
                        tutorProfile.TutorExperience = update.TutorExperience;
                        tutorProfile.ShortDescription = update.ShortDescription;
                        tutorProfile.LongDescription = update.LongDescription;
                        tutorProfile.TutorDOB = update.TutorDOB;
                        db.SubmitChanges();
                        var path = Server.MapPath("~/ProjectImages/");
                        if (postedfile != null)
                        {
                            var filename = postedfile.FileName;
                            var ImgPath = path + filename;
                            postedfile.SaveAs(ImgPath);
                        }
                    }

                    return Json(Id, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion END TutorPersonalProfile Update    

        public ActionResult TutorCommMsg()
        {
            return View();
        }


        #region Apply for Internship(Any UG/PG/Profession can apply) By Dilshad A. on 08 Sept 2020
        public ActionResult ApplyInternship()
        {
            return View();
        }

        [HttpPost]
        public ContentResult AddApplyForInternship(tblInternApply objApply, List<MultipleDocuments> lstDocument, SITSPL_tblContact objContact, HttpPostedFileBase imgPostedfile, HttpPostedFileBase resumePostedfile, HttpPostedFileBase aadhaarPostedfile)
        {
            try
            {
                db = new CourseDataContext();

                var getInternshipName = (from d in db.SITSPL_tblInternshipStructures
                                         where d.InternStructureId == objApply.InternshipStructureId
                                         join d1 in db.SITSPL_tblInternships on d.InternshipId equals d1.InternshipId
                                         select d1.InternshipName).SingleOrDefault();

                Session["IntershipNameForApplyIntern"] = getInternshipName;
                if (objApply != null)
                {
                    objApply.DateCreated = DateTime.Now;
                    objApply.CreatedBy = objApply.Name;
                    objApply.IsPublished = true;
                    db.tblInternApplies.InsertOnSubmit(objApply);
                    db.SubmitChanges();

                    if (objApply.InterApllyId > 0)
                    {
                        if (lstDocument != null)
                        {
                            for (int i = 0; i < lstDocument.Count; i++)
                            {
                                SITSPL_tblDocument objDoc = new SITSPL_tblDocument();
                                objDoc.UserId = objApply.InterApllyId;
                                objDoc.PanNo = "-";
                                objDoc.DoucmentNo = lstDocument[i].DoucmentNo;
                                objDoc.DocumentType = "-";
                                objDoc.DocumentName = lstDocument[i].DocumentName;
                                objDoc.DateCreated = DateTime.Now;
                                objDoc.CreatedBy = objApply.Name;
                                db.SITSPL_tblDocuments.InsertOnSubmit(objDoc);
                                db.SubmitChanges();
                            }
                        }

                        if (objContact != null)
                        {
                            objContact.UserId = objApply.InterApllyId;
                            objContact.CreatedBy = objApply.Name;
                            objContact.DateCreated = DateTime.Now;
                            db.SITSPL_tblContacts.InsertOnSubmit(objContact);
                            db.SubmitChanges();
                        }

                        #region Save Image
                        var imgPath = Server.MapPath("~/Intern/Images/");
                        var imgName = imgPostedfile.FileName;
                        imgPath = imgPath + imgName;
                        imgPostedfile.SaveAs(imgPath);
                        #endregion END Save Image

                        #region Save Resume
                        if (resumePostedfile != null)
                        {
                            if (resumePostedfile.FileName != null)
                            {
                                var resumePath = Server.MapPath("~/Intern/Resume/");
                                var resumeName = resumePostedfile.FileName;
                                resumePath = resumePath + resumeName;
                                resumePostedfile.SaveAs(resumePath);
                            }
                        }
                        #endregion END Save Resume

                        #region Save Aadhaar
                        if (aadhaarPostedfile != null)
                        {
                            if (aadhaarPostedfile.FileName != null)
                            {
                                var aadhaarPath = Server.MapPath("~/Intern/Aadhaar/");
                                var aadhaarName = aadhaarPostedfile.FileName;
                                aadhaarPath = aadhaarPath + aadhaarName;
                                aadhaarPostedfile.SaveAs(aadhaarPath);
                            }
                        }
                        #endregion END Save Aadhaar

                        SendMailAfterInterRegistration(objApply.Name, objApply.Email);
                    }
                }
                return Content(objApply.InterApllyId.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Apply for Internship By Dilshad A. on 08 Sept 2020

        #region Intern Student Profile By Dilshad On 07 Sept 2020
        public ActionResult InternProfile()
        {
            return View();
        }
        #endregion END Intern Student Profile By Dilshad On 07 Sept 2020

        #region Send OTP to Inter to verify email(at registration time) By Dilshad A. On 09 Sept 2020     
        [HttpPost]
        public string SendMailAfterInterRegistration(string Name, string Email)
        {
            try
            {
                string strInternshipName = "";
                if (!string.IsNullOrEmpty(Session["IntershipNameForApplyIntern"].ToString()))
                {
                    strInternshipName = Session["IntershipNameForApplyIntern"].ToString();
                }
                MailMessage mail = new MailMessage();
                mail.To.Add(Email);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                mail.Subject = "Internship Confirmation Mail";
                string Body = "Hello " + Name + " Your have successfully register with " + strInternshipName + " Internship program: ";
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"],
                    ConfigurationManager.AppSettings["smtpPass"]);
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return "mailsent";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion END Send OTP to Inter to verify email(at registration time) By Dilshad A. On 09 Sept 2020

        #region Intern Login By Dilshad A. on 11 Sept 2020
        public ActionResult InternLogin()
        {
            if (Session["InterApllyId"] != null)
            {
                string str = Session["InterApllyId"].ToString();
                return RedirectToAction("InternWelcome", "User");
            }
            return View();
        }

        [HttpPost]
        public ActionResult InternLogin(SITSPL_tblUser objUser)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.SITSPL_tblUsers
                            where d.UserName == objUser.UserName && d.Password == objUser.Password
                            join d1 in db.tblInternApplies on d.Id equals d1.InterApllyId
                            select new
                            {
                                d.UserName,
                                d.UserType,
                                d.UserId,
                                d1.Email,
                                d1.Image,
                                d1.InterApllyId
                            }).SingleOrDefault();
                if (data != null)
                {
                    Session["InterApllyId"] = data.InterApllyId;
                    Session["InternUser"] = data.UserName;
                    Session["InternEmail"] = data.Email;
                    Session["InternImage"] = data.Image;
                    Session["UserType"] = data.UserType;
                    return RedirectToAction("InternWelcome", "User");
                }
                else
                {
                    ViewBag.Message = "Wrong Username Or Password";
                    return View();
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion Intern Login By Dilshad A. on 11 Sept 2020

        #region Logout Intern Student By Dilshad A. on 11 Sept 2020

        #region Intern Welcome Page By Dilshad A. on 11 Sept 2020
        public ActionResult InternWelcome()
        {
            return View();
        }
        #endregion END Intern Welcome Page By Dilshad A. on 11 Sept 2020
        public ActionResult InternLogout()
        {
            Session["InterApllyId"] = null;
            return RedirectToAction("Index", "Home");
        }
        #endregion END Logout Intern Student By Dilshad A. on 11 Sept 2020

        #region View Communication By DIlshad A. on 16 Sept 2020
        public ActionResult ViewCommunication()
        {
            return View();
        }

        public JsonResult ViewCommunicationDetails()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.tblCommunications select d).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion View Communication By DIlshad A. on 16 Sept 2020

        #region Get Communication Details For Intern/Student/Tutor by Dilshad A. on 25 Sept 2020
        [HttpPost]
        public JsonResult GetCommunicationById(int intId, string strUserType, int? intCourseId, int? intInternshipId)
        {
            try
            {
                db = new CourseDataContext();
                string type = strUserType.Trim();

                if (strUserType == "All-Students-Course" && intId > 0)
                {

                    var data = (from C in db.tblCommunications
                                join S in db.SITSPL_tblStudentProfiles
                                on C.Id equals S.StudentId
                                where S.StudentId == intId && C.UserType == type
                                select new
                                {
                                    C.Id,
                                    C.UserType,
                                    C.Message,
                                    C.MessageAllStudents,
                                    C.MessageAll
                                }).ToList();

                    //var msgAll = (from d in db.tblCommunications orderby d.CommunicationId descending where d.Id == null || d.Id == 0 select d).ToList();

                    var msgAll = (from d in db.tblCommunications orderby d.CommunicationId descending where d.Id == intId select d).ToList();

                    if (data != null && msgAll != null || msgAll == null)
                    {
                        var list = new { data, msgAll };
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }

                    //   var data = db.SITSPL_GetCommunicationMsgByUserAndId(intId,type).ToList();


                    //   var msgAll = (from d in db.tblCommunications orderby d.CommunicationId descending where d.Id == null || d.Id == 0 select d).ToList();

                    //var data = (from d in db.tblCommunications
                    //            where d.Id == intId && d.UserType == strUserType
                    //            select new
                    //            {
                    //                d.Id,
                    //                d.InternshipId,
                    //                d.IsAll,
                    //                d.IsAllStudent,
                    //                d.IsAllTeacher,
                    //                d.Message,
                    //                d.MessageAll,
                    //                d.UserType,
                    //                d.DateCreated,
                    //                d.CourseId,
                    //            }).ToList();

                    //if(data != null && msgAll != null || msgAll == null) { 
                    //var list = new { data, msgAll };
                    //return Json(list, JsonRequestBehavior.AllowGet);
                    //}

                }


                else if (strUserType == "All-Intern-Students" && intId > 0)
                {
                    var data2 = (from C in db.tblCommunications
                                 join S in db.tblInternApplies
                                 on C.Id equals S.InterApllyId
                                 where S.InterApllyId == intId && C.UserType == type
                                 select new
                                 {
                                     C
                                 }).ToList();

                    //      var TutId = db.tblInternApplies.Where(x => x.InterApllyId == intId).Select(x => x.InternshipStructureId).FirstOrDefault();

                    //var TutId = (from apply in db.tblInternApplies
                    //             join intrtut in db.tblInternTutors
                    //             on apply.InternshipStructureId equals intrtut.InternshipId
                    //             where apply.InterApllyId == intId
                    //             select new
                    //             {
                    //                 intrtut.TutorId,
                    //                 intrtut.InternshipId
                    //             }).FirstOrDefault();



                    //var TutName = db.SITSPL_tblTutors.Where(x => x.TutorId == TutId.TutorId).Select(x => x.TutorName).FirstOrDefault();

                    var data = data2.Select(x => new
                    {
                        x.C.Id,
                        x.C.UserType,
                        x.C.Message,
                        x.C.MessageAllStudents,
                        x.C.MessageAll,
                        DateCreated = x.C.DateCreated.ToString("dd/MM/yyyy"),
                        x.C.CommunicationId,
                        // TutName
                    }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();

                    var list = new { data };
                    return Json(list, JsonRequestBehavior.AllowGet);


                    //var msgAll = (from d in db.tblCommunications orderby d.CommunicationId descending where d.Id == intId select d).ToList();

                    //if (data != null && msgAll != null || msgAll == null)
                    //{
                    //    var list = new { data, msgAll };
                    //    return Json(list, JsonRequestBehavior.AllowGet);
                    //}

                }

                else if (strUserType == "All-Intern-Tutor" && intId > 0)
                {
                    var data2 = (from C in db.tblCommunications
                                     //join S in db.tblInternTutors
                                 join S in db.SITSPL_tblTutors
                                 on C.Id equals S.TutorId
                                 where S.TutorId == intId && C.UserType == type && C.CreatedBy == "Admin"
                                 select new
                                 {
                                     C
                                 }).ToList();

                    var data = data2.Select(x => new
                    {
                        x.C.Id,
                        x.C.UserType,
                        x.C.Message,
                        x.C.MessageAllStudents,
                        x.C.MessageAll,
                        DateCreated = x.C.DateCreated.ToString("dd/MM/yyyy"),
                        x.C.CommunicationId

                    }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();

                    var list = new { data };
                    return Json(list, JsonRequestBehavior.AllowGet);

                    //var msgAll = (from d in db.tblCommunications orderby d.CommunicationId descending where d.Id == intId select d).ToList();

                    //if (data != null && msgAll != null || msgAll == null)
                    //{
                    //    var list = new { data, msgAll };
                    //    return Json(list, JsonRequestBehavior.AllowGet);
                    //}

                }

                else if (strUserType == "All-Brand-Students" && intId > 0)
                {
                    var data2 = (from C in db.tblCommunications
                                 join S in db.SITSPL_tblStudentProfiles
                                  on C.Id equals S.StudentId
                                 where S.StudentId == intId && C.UserType == type
                                 select new
                                 {
                                     C,
                                     S
                                 }).Distinct().ToList();

                    //var TutName = (from std in db.SITSPL_tblStudentProfiles
                    //               join tuto in db.SITSPL_tblTutors
                    //               on std.TutorId equals tuto.TutorId
                    //               where std.StudentId == intId
                    //               select new
                    //               {
                    //                   tuto.TutorName
                    //               }).ToList();

                    var TutId = db.SITSPL_tblStudentProfiles.Where(x => x.StudentId == intId).Select(x => x.TutorId).FirstOrDefault();
                    var TutName = db.SITSPL_tblTutors.Where(x => x.TutorId == TutId).Select(x => x.TutorName).FirstOrDefault();

                    var data = data2.Select(x => new
                    {
                        x.C.Id,
                        x.C.UserType,
                        x.C.Message,
                        x.C.MessageAllStudents,
                        x.C.MessageAll,
                        x.C.CommunicationId,
                        x.S.Name,
                        DateCreated = x.C.DateCreated.ToString("dd/MM/yyyy"),
                        TutName
                    }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();

                    var list = new { data };
                    return Json(list, JsonRequestBehavior.AllowGet);

                    //var msg = (from d in db.tblCommunications orderby d.CommunicationId descending where d.Id == intId select 
                    //              new
                    //              {
                    //                  d
                    //              }).ToList();

                    //if(msg != null)
                    //{
                    //    var msgAll = msg.Select(x => new
                    //    {
                    //        x.d.Message,
                    //        x.d.MessageAll,
                    //        x.d.MessageAllStudents,
                    //      DateCreated =  x.d.DateCreated.ToString("dd/MM/yyyy")

                    //    }).ToList();

                    //    var list = new { data, msgAll };
                    //    return Json(list, JsonRequestBehavior.AllowGet);
                    //}
                    //else if(msg == null)
                    //{
                    //    var list = new { data};
                    //    return Json(list, JsonRequestBehavior.AllowGet);
                    //}
                }



                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Get Communication Details For Intern by Dilshad A. on 24 Sept 2020       


        public JsonResult GetBrandTutorEmail()
        {
            try
            {
                db = new CourseDataContext();

                var Email = Session["TutorEmail"].ToString();

                //if(Id != null)
                //{ 
                //var Email = (from us in db.SITSPL_tblUsers
                //             join tut in db.SITSPL_tblTutors
                //             on us.Id equals tut.TutorId
                //             where us.Id == Convert.ToInt64(Id) && us.UserType == "Brand-Tutor"
                //             select new
                //             {
                //                 tut.TutorEmail
                //             }).FirstOrDefault();

                if (Email != null)
                {
                    return Json(Email, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


        public JsonResult GetInternTutorEmail()
        {
            try
            {
                db = new CourseDataContext();

                var Email = Session["InternTutorEmail"].ToString();
                //  var Id = Session["InternTutorId"].ToString();


                if (Email != null)
                {

                    return Json(Email, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


        public JsonResult GetInternTutorId()
        {
            try
            {
                db = new CourseDataContext();

                // var Email = Session["InternTutorEmail"].ToString();
                var Id = Session["InternTutorId"].ToString();
                var data = Convert.ToInt32(Id);

                if (data > 0)
                {

                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public JsonResult ChangeBrandTutorPassword(string Email, string CurrentPassword, string NewPassword, string ReTypePassword)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from us in db.SITSPL_tblUsers
                            join tut in db.SITSPL_tblTutors on us.Id equals tut.TutorId
                            where tut.TutorEmail == Email && us.Id == Convert.ToInt64(Session["TutorId"].ToString()) && us.UserType == "Brand-Tutor"
                            select new
                            {
                                us.Password,
                                us.Id,
                                us.UserName
                            }).FirstOrDefault();

                if (data != null)
                {
                    if (CurrentPassword != data.Password)
                    {
                        return Json("currpasswrong", JsonRequestBehavior.AllowGet);
                    }
                    else if (NewPassword != ReTypePassword)
                    {
                        return Json("currnewpasswrong", JsonRequestBehavior.AllowGet);
                    }

                    else if (CurrentPassword == data.Password && NewPassword == ReTypePassword)
                    {
                        SITSPL_tblUser user = db.SITSPL_tblUsers.Where(x => x.Id == data.Id && x.UserType == "Brand-Tutor").FirstOrDefault();
                        if (user != null)
                        {
                            user.UserType = "Brand-Tutor";
                            user.Id = data.Id;
                            user.Password = NewPassword;
                            user.IsPublished = true;
                            user.LastUpdated = DateTime.Now;
                            user.UpdatedBy = data.UserName;
                            db.SubmitChanges();
                            return Json("passupdated", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json("notregistered", JsonRequestBehavior.AllowGet);
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult ChangeInternTutorPassword(string Email, string CurrentPassword, string NewPassword, string ReTypePassword)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from us in db.SITSPL_tblUsers
                            join tut in db.SITSPL_tblTutors on us.Id equals tut.TutorId
                            where tut.TutorEmail == Email && us.Id == Convert.ToInt64(Session["InternTutorId"].ToString()) && us.UserType == "Intern-Tutor"
                            select new
                            {
                                us.Password,
                                us.Id,
                                us.UserName
                            }).FirstOrDefault();

                if (data != null)
                {
                    if (CurrentPassword != data.Password)
                    {
                        return Json("currpasswrong", JsonRequestBehavior.AllowGet);
                    }
                    else if (NewPassword != ReTypePassword)
                    {
                        return Json("currnewpasswrong", JsonRequestBehavior.AllowGet);
                    }

                    else if (CurrentPassword == data.Password && NewPassword == ReTypePassword)
                    {
                        SITSPL_tblUser user = db.SITSPL_tblUsers.Where(x => x.Id == data.Id && x.UserType == "Intern-Tutor").FirstOrDefault();
                        if (user != null)
                        {
                            user.UserType = "Intern-Tutor";
                            user.Id = data.Id;
                            user.Password = NewPassword;
                            user.IsPublished = true;
                            user.LastUpdated = DateTime.Now;
                            user.UpdatedBy = data.UserName;
                            db.SubmitChanges();
                            return Json("passupdated", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json("notregistered", JsonRequestBehavior.AllowGet);
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult BrandStudentCommunicationToTutor()
        {
            try
            {
                db = new CourseDataContext();
                //var Id = Session["StudentId"].ToString();

                Int32 intStudentId = 0;
                Int32.TryParse(Session["StudentId"].ToString(), out intStudentId);
                
                //var data = (from std in db.SITSPL_tblStudentProfiles
                //            join us in db.SITSPL_tblUsers
                //            on std.StudentId equals us.Id
                //            where std.StudentId == Convert.ToInt32(Id)
                //            join tut in db.SITSPL_tblTutors
                //            on std.TutorId equals tut.TutorId
                //            where std.StudentId == Convert.ToInt32(Id)
                //            select new
                //            {
                //                tut.TutorId,
                //                tut.TutorNae
                //            }).ToList();


                //                select StudentId, std.TutorId,std.Email,tut.TutorName,tut.TutorEmail
                // from SITSPL_tblStudentProfile as std
                //INNER JOIN SITSPL_tblUser as us
                //on std.StudentId = us.Id
                //join SITSPL_tblTutor as tut
                //on tut.TutorId = std.TutorId
                //where std.StudentId = 60041;

                //var data = (from std in db.SITSPL_tblStudentProfiles
                //            join us in db.SITSPL_tblUsers
                //            on std.StudentId equals us.Id
                //            where std.StudentId == Convert.ToInt32(Id)
                //            join tut in db.SITSPL_tblTutors
                //            on std.TutorId equals tut.TutorId
                //            select new
                //            {
                //                std.StudentId,
                //                std.TutorId
                //            }).ToList();    

                //                    select StudentId, std.TutorId,std.Email,tut.TutorName,tut.TutorEmail
                // from SITSPL_tblStudentProfile as std
                //INNER JOIN SITSPL_tblUser as us
                //on std.StudentId = us.Id
                //join SITSPL_tblTutor as tut
                //on tut.TutorId = std.TutorId;

                var data = db.SITSPL_GetStudentBrandTutorById(intStudentId).ToList();
                //var data = db.SITSPL_GetStudentBrandTutorById(intId).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddStudentCommunicationToBrandTutor(tblCommunication objbrandstudenttotutor)
        {
            try
            {
                db = new CourseDataContext();
                tblCommunication objComm = null;

                Int64 intBrandStudentId = 0;
                Int64.TryParse(Session["StudentId"].ToString(), out intBrandStudentId);

                Int32? TutorId = Convert.ToInt32(objbrandstudenttotutor.Id);

                // var Id = Session["StudentId"];

                var data = db.SITSPL_GetStudentBrandTutorById(Convert.ToInt32(intBrandStudentId)).ToList();

                var Course = (from cd in db.SITSPL_tblCourseDetails
                              join std in db.SITSPL_tblStudentProfiles
                              on cd.StudentId equals std.StudentId
                              where std.StudentId == Convert.ToInt32(intBrandStudentId)
                              select new
                              {
                                  cd.CourseId
                              }).FirstOrDefault();

                objComm = new tblCommunication();
                ArrayList alEmail = new ArrayList();

                if (data != null)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        alEmail.Add(data[i].TutorEmail);

                        objComm = new tblCommunication();

                        objComm.Id = intBrandStudentId;
                        objComm.TutorId = TutorId;
                        objComm.UserType = "All-Brand-Tutor";
                        objComm.DateCreated = DateTime.Now;
                        objComm.CreatedBy = "Brand Student";
                        objComm.Message = objbrandstudenttotutor.Message;
                        objComm.MessageAllStudents = "-";
                        objComm.MessageAllTeachers = "-";
                        objComm.CategoryType = "Brand-Tutor";
                        objComm.CourseId = Course.CourseId;
                        db.tblCommunications.InsertOnSubmit(objComm);
                        db.SubmitChanges();
                    }
                    if (alEmail.Count > 0)
                    {
                        SendMailToAll(alEmail);
                    }
                }

                return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



        #region Send Mail to multiple user(Student/User) BY Dilshad A. on 26 Sept 2020
        public void SendMailToAll(ArrayList arrEmail)
        {
            try
            {
                if (arrEmail.Count > 0)
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                    mail.Subject = "New Notification";
                    string Body = "Hello Dear,  You have a new message, Please visit your portal and check it.";
                    mail.Body = Body;
                    mail.IsBodyHtml = true;

                    for (int i = 0; i < arrEmail.Count; i++)
                    {
                        mail.To.Add(new MailAddress(arrEmail[i].ToString()));
                    }

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"],
                        ConfigurationManager.AppSettings["smtpPass"]);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Send Mail to multiple user(Student/User) BY Dilshad A. on 26 Sept 2020


        public ActionResult BrandStudentCommToTutors()
        {
            return View();
        }


        // Brand Student Message To Tutor 

        public JsonResult BrandStudentMessagesToTutors()
        {
            try
            {
                db = new CourseDataContext();

                var BrandStudentId = Session["StudentId"].ToString();
                var BrandStdName = db.SITSPL_tblStudentProfiles.Where(x => x.StudentId == Convert.ToInt32(BrandStudentId)).Select(x => x.Name).FirstOrDefault();
                var TutorId = db.SITSPL_tblStudentProfiles.Where(x => x.StudentId == Convert.ToInt32(BrandStudentId)).Select(x => x.TutorId).FirstOrDefault();

                var sub = (from comm in db.tblCommunications
                           join cour in db.SITSPL_tblCourses
                           on comm.CourseId equals cour.CourseId
                           join tut in db.SITSPL_tblTutors
                           on comm.TutorId.Value equals tut.TutorId
                           where comm.UserType == "All-Brand-Tutor" && comm.TutorId == TutorId && comm.CreatedBy == "Brand Student"
                           select new
                           {
                               comm,
                               cour,
                               tut
                           }).ToList();

                var data = sub.Select(x => new
                {
                    x.comm.Id,
                    x.tut.TutorName,
                    x.tut.TutorEmail,
                    x.comm.UserType,
                    x.comm.Message,
                    x.comm.MessageAllStudents,
                    x.comm.MessageAllTeachers,
                    x.comm.MessageAll,
                    x.comm.CategoryType,
                    x.comm.CourseId,
                    x.comm.CommunicationId,
                    x.comm.IsAllCourseStudent,
                    DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),
                    x.cour.CourseName,
                    x.comm.CreatedBy,
                    BrandStdName
                }).OrderByDescending(x => x.CommunicationId).ToList();


                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult TutorCommToStudent()
        {
            return View();
        }

        // Bind Course Students For Tutor

        public JsonResult CourseStudents()
        {
            try
            {
                var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                   join cd in db.SITSPL_tblCourseDetails
                                   on stdpro.StudentId equals cd.StudentId
                                   join cs in db.SITSPL_tblCourseStructures
                                   on stdpro.StructureId equals cs.StructureId
                                   join cour in db.SITSPL_tblCourses
                                   on cs.CourseId equals cour.CourseId
                                   select new
                                   {
                                       cs.CourseId,
                                       stdpro.Email,
                                       stdpro.StudentId,
                                       stdpro.Name,
                                       stdpro.StudentCategoryId,

                                   }).Distinct().ToList();

                return Json(AllStudents, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult UnregisteredStudentsInCourse()
        {
            try
            {
                var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                   join us in db.SITSPL_tblUsers
                                   on stdpro.StudentId equals us.Id
                                   where stdpro.TutorId == null
                                   select new
                                   {
                                       //cs.CourseId,
                                       stdpro.Email,
                                       stdpro.StudentId,
                                       stdpro.Name,
                                       stdpro.StudentCategoryId,


                                   }).Distinct().ToList();

                return Json(AllStudents, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult BrandStudents()
        {
            try
            {
                db = new CourseDataContext();

                var data = (from std in db.SITSPL_tblStudentProfiles
                            join us in db.SITSPL_tblUsers
                            on std.StudentId equals us.Id
                            where std.TutorId != null
                            select new
                            {
                                std.StudentId,
                                std.Name,
                                std.TutorId,
                                std.Email,
                                std.StudentCategoryId,
                                std.CourseStructureId
                            }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult TutorsToStudentAddCommunicatioin(tblCommunication objTutorComm, List<TutorToBrandStudent> arrTutorBrandStudents, List<TutorToCourseStudent> arrTutorCourseStudents)
        {
            try
            {
                db = new CourseDataContext();
                tblCommunication objComm = null;

                var TutorId = Session["TutorId"].ToString();

                // For All Students/Tutors(Intern/Course)
                ArrayList alAllEmail = new ArrayList();

                if (arrTutorBrandStudents != null && objTutorComm.IsAllStudent == false && objTutorComm.CategoryType == "Brand Student")
                {
                    ArrayList alEmail = new ArrayList();
                    if (arrTutorBrandStudents != null)
                    {
                        for (int i = 0; i < arrTutorBrandStudents.Count; i++)
                        {
                            objComm = new tblCommunication();

                            alEmail.Add(arrTutorBrandStudents[i].Email);
                            //      alCourse.Add(StudentsWithCourse[i].CourseId);

                            objComm = new tblCommunication();
                            //objComm.IsAllTeacher = isAllTutors;

                            //objComm.IsAllCourseStudent = lstCourseStudent;
                            //objComm.IsAllInternStudent = lstInternStudent;
                            //objComm.IsAllCourseTutor = lstCourseTutor;
                            //objComm.IsAllInternTutor = lstInternTutor;

                            objComm.Id = arrTutorBrandStudents[i].StudentId;
                            objComm.UserType = "All-Brand-Students";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Tutor";
                            objComm.Message = objTutorComm.Message;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            //   objComm.MessageAll = strMsgAll;
                            //   objComm.CategoryType = "Brand Student";
                            // objComm.T = arrTutorBrandStudents[i].TutorId;


                            objComm.IsAllStudent = objTutorComm.IsAllStudent;
                            objComm.CategoryType = objTutorComm.CategoryType;
                            objComm.StudentCategoryId = arrTutorBrandStudents[i].StudentCategoryId;

                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }
                    }
                    if (alEmail.Count > 0)
                    {
                        SendMailToAllStudents(alEmail);
                    }
                    return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                }

                else if (objTutorComm.IsAllStudent == true && arrTutorBrandStudents != null || arrTutorBrandStudents == null && objTutorComm.CategoryType == "Brand Student" || objTutorComm.CategoryType == "")
                {

                    objComm = new tblCommunication();
                    ArrayList alEmail = new ArrayList();

                    var AllBrandStudents = (from std in db.SITSPL_tblStudentProfiles
                                            join us in db.SITSPL_tblUsers
                                            on std.StudentId equals us.Id
                                            where std.TutorId != null
                                            select new
                                            {
                                                std.StudentId,
                                                std.Name,
                                                std.TutorId,
                                                std.Email,
                                                std.StudentCategoryId,
                                                std.CourseStructureId
                                            }).Distinct().ToList();



                    if (AllBrandStudents != null)
                    {
                        for (int i = 0; i < AllBrandStudents.Count; i++)
                        {
                            alEmail.Add(AllBrandStudents[i].Email);
                            //      alCourse.Add(StudentsWithCourse[i].CourseId);
                            objComm = new tblCommunication();
                            objComm.Id = AllBrandStudents[i].StudentId;
                            objComm.UserType = "All-Brand-Students";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Tutor";
                            objComm.Message = objTutorComm.Message;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.IsAllStudent = objTutorComm.IsAllStudent;
                            objComm.CategoryType = objTutorComm.CategoryType;
                            objComm.StudentCategoryId = AllBrandStudents[i].StudentCategoryId;
                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }
                        if (alEmail.Count > 0)
                        {
                            SendMailToAllStudents(alEmail);
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (objTutorComm.IsAllStudent == false && objTutorComm.CategoryType == "Course Student")
                {

                    ArrayList alEmail = new ArrayList();

                    if (arrTutorCourseStudents != null)
                    {
                        for (int i = 0; i < arrTutorCourseStudents.Count; i++)
                        {
                            objComm = new tblCommunication();

                            alEmail.Add(arrTutorCourseStudents[i].Email);

                            objComm = new tblCommunication();

                            objComm.Id = arrTutorCourseStudents[i].StudentId;
                            objComm.UserType = "All-Students-Course";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Tutor";
                            objComm.Message = objTutorComm.Message;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.TutorId = Convert.ToInt32(TutorId);
                            objComm.IsAllStudent = objTutorComm.IsAllStudent;
                            objComm.CourseId = arrTutorCourseStudents[i].CourseId;
                            objComm.CategoryType = objTutorComm.CategoryType;
                            objComm.StudentCategoryId = arrTutorCourseStudents[i].StudentCategoryId;
                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }
                    }
                    if (alEmail.Count > 0)
                    {
                        SendMailToAllStudents(alEmail);
                    }
                    return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);

                }

                else if (objTutorComm.IsAllStudent == true && arrTutorCourseStudents != null || arrTutorCourseStudents == null && objTutorComm.CategoryType == "Course Student" || objTutorComm.CategoryType == "")
                {

                    objComm = new tblCommunication();
                    ArrayList alEmail = new ArrayList();


                    var AllCourseStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                             join cd in db.SITSPL_tblCourseDetails
                                             on stdpro.StudentId equals cd.StudentId
                                             //join cs in db.SITSPL_tblCourseStructures
                                             //on stdpro.StructureId equals cs.StructureId
                                             join cour in db.SITSPL_tblCourses
                                             on cd.CourseId equals cour.CourseId
                                             select new
                                             {
                                                 //cs.CourseId,
                                                 cd.CourseId,
                                                 stdpro.Email,
                                                 stdpro.StudentId,
                                                 stdpro.Name,
                                                 stdpro.StudentCategoryId,

                                             }).Distinct().ToList();



                    if (AllCourseStudents != null)
                    {
                        for (int i = 0; i < AllCourseStudents.Count; i++)
                        {
                            alEmail.Add(AllCourseStudents[i].Email);
                            //      alCourse.Add(StudentsWithCourse[i].CourseId);
                            objComm = new tblCommunication();
                            objComm.Id = AllCourseStudents[i].StudentId;
                            objComm.UserType = "All-Students-Course";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Tutor";
                            objComm.Message = objTutorComm.Message;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.CourseId = AllCourseStudents[i].CourseId;

                            objComm.IsAllStudent = objTutorComm.IsAllStudent;

                            objComm.CategoryType = objTutorComm.CategoryType;
                            objComm.StudentCategoryId = AllCourseStudents[i].StudentCategoryId;
                            objComm.TutorId = Convert.ToInt32(TutorId);
                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }
                        if (alEmail.Count > 0)
                        {
                            SendMailToAllStudents(alEmail);
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        //    if (lstInternTutor == true && lstCourseTutor == true && lstCourseStudent == true && lstInternStudent == true)
        //    {
        //        if (lstInternTutor == true)
        //        {

        //            objComm = new tblCommunication();
        //            ArrayList alEmail = new ArrayList();
        //            var AllInternTutor = (from tut in db.SITSPL_tblTutors
        //                                  join intntut in db.tblInternTutors
        //                                  on tut.TutorId equals intntut.TutorId
        //                                  join intstr in db.SITSPL_tblInternshipStructures
        //                                  on intntut.InternshipId equals intstr.InternshipId
        //                                  select new
        //                                  {
        //                                      tut.TutorName,
        //                                      tut.TutorEmail,
        //                                      intntut.InternTutorId,
        //                                      intntut.TutorId,
        //                                      intntut.InternshipId
        //                                  }).Distinct().ToList();

        //            if (AllInternTutor != null)
        //            {
        //                for (int i = 0; i < AllInternTutor.Count; i++)
        //                {
        //                    alEmail.Add(AllInternTutor[i].TutorEmail);
        //                    //      alCourse.Add(StudentsWithCourse[i].CourseId);

        //                    objComm = new tblCommunication();
        //                    //objComm.IsAllTeacher = isAllTutors;

        //                    objComm.IsAllCourseStudent = lstCourseStudent;
        //                    objComm.IsAllInternStudent = lstInternStudent;
        //                    objComm.IsAllCourseTutor = lstCourseTutor;
        //                    objComm.IsAllInternTutor = lstInternTutor;

        //                    objComm.Id = AllInternTutor[i].InternTutorId;
        //                    objComm.UserType = "All-Intern-Tutor";
        //                    objComm.DateCreated = DateTime.Now;
        //                    objComm.CreatedBy = "Admin";
        //                    objComm.Message = "";
        //                    objComm.MessageAllStudents = "-";
        //                    objComm.MessageAllTeachers = "-";
        //                    objComm.MessageAll = strMsgAll;
        //                    objComm.CategoryType = "Intern-Tutor";
        //                    objComm.InternshipId = AllInternTutor[i].InternshipId;
        //                    db.tblCommunications.InsertOnSubmit(objComm);
        //                    db.SubmitChanges();
        //                }
        //                if (alEmail.Count > 0)
        //                {
        //                    SendMailToAllTutor(alEmail);
        //                }
        //            }

        //        }


        //        //    if (lstCourseStudent == true)
        //        //    {
        //        //        objComm = new tblCommunication();
        //        //        ArrayList alEmail = new ArrayList();

        //        //        //var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
        //        //        //                   select stdpro
        //        //        //                   ).ToList();

        //        //        var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
        //        //                           join cd in db.SITSPL_tblCourseDetails
        //        //                           on stdpro.StudentId equals cd.StudentId
        //        //                           join cs in db.SITSPL_tblCourseStructures
        //        //                           on stdpro.StructureId equals cs.StructureId
        //        //                           join cour in db.SITSPL_tblCourses
        //        //                           on cs.CourseId equals cour.CourseId
        //        //                           select new
        //        //                           {
        //        //                               cs.CourseId,
        //        //                               stdpro.Email,
        //        //                               stdpro.StudentId,
        //        //                               stdpro.StudentCategoryId,

        //        //                           }).Distinct().ToList();

        //        //        //   ArrayList alCourse = new ArrayList();

        //        //        if (AllStudents != null)
        //        //        {
        //        //            for (int i = 0; i < AllStudents.Count; i++)
        //        //            {
        //        //                alEmail.Add(AllStudents[i].Email);
        //        //                //      alCourse.Add(StudentsWithCourse[i].CourseId);

        //        //                objComm = new tblCommunication();
        //        //                //objComm.IsAllTeacher = isAllTutors;

        //        //                //      bool lstInternTutor,bool lstCourseTutor,bool lstCourseStudent,bool lstInternStudent

        //        //                objComm.IsAllCourseStudent = lstCourseStudent;
        //        //                objComm.IsAllInternStudent = lstInternStudent;
        //        //                objComm.IsAllCourseTutor = lstCourseTutor;
        //        //                objComm.IsAllInternTutor = lstInternTutor;

        //        //                objComm.Id = AllStudents[i].StudentId;
        //        //                objComm.UserType = "All-Students-Course";
        //        //                objComm.DateCreated = DateTime.Now;
        //        //                objComm.CreatedBy = "Admin";
        //        //                objComm.Message = "";
        //        //                objComm.MessageAllStudents = "-";
        //        //                objComm.MessageAllTeachers = "-";
        //        //                objComm.MessageAll = strMsgAll;
        //        //                objComm.CategoryType = "Course";
        //        //                objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
        //        //                objComm.CourseId = AllStudents[i].CourseId;
        //        //                db.tblCommunications.InsertOnSubmit(objComm);
        //        //                db.SubmitChanges();
        //        //            }
        //        //            if (alEmail.Count > 0)
        //        //            {
        //        //                SendMailToAll(alEmail);
        //        //            }
        //        //        }
        //        //        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);

        //        //    }

        //        //    if (lstInternStudent == true)
        //        //    {
        //        //        var AllInterns = (from d in db.tblInternApplies
        //        //                          join d1 in db.SITSPL_tblInternshipStructures on d.InternshipStructureId equals d1.InternStructureId
        //        //                          join d2 in db.SITSPL_tblInternships on d1.InternshipId equals d2.InternshipId
        //        //                          select new
        //        //                          {
        //        //                              d.InterApllyId,
        //        //                              d.InternshipStructureId,
        //        //                              d.Name,
        //        //                              d1.InternshipId,
        //        //                              d.IsPublished,
        //        //                              d.Email
        //        //                          }).ToList();

        //        //        ArrayList alEmail = new ArrayList();
        //        //        for (int i = 0; i < AllInterns.Count; i++)
        //        //        {
        //        //            alEmail.Add(AllInterns[i].Email);
        //        //            objComm = new tblCommunication();
        //        //            //objComm.IsAllTeacher = isAllTutors;

        //        //            objComm.IsAllCourseStudent = lstCourseStudent;
        //        //            objComm.IsAllInternStudent = lstInternStudent;
        //        //            objComm.IsAllCourseTutor = lstCourseTutor;
        //        //            objComm.IsAllInternTutor = lstInternTutor;

        //        //            objComm.Id = AllInterns[i].InterApllyId;
        //        //            objComm.UserType = "All-Intern-Students";
        //        //            objComm.DateCreated = DateTime.Now;
        //        //            objComm.CreatedBy = "Admin";
        //        //            objComm.MessageAllStudents = "-";
        //        //            objComm.MessageAllTeachers = "-";
        //        //            objComm.MessageAll = strMsgAll;
        //        //            objComm.CategoryType = "Intern";
        //        //            //   objComm.StudentCategoryId = StudentCategoryId;
        //        //            objComm.InternshipId = AllInterns[i].InternshipId;
        //        //            db.tblCommunications.InsertOnSubmit(objComm);
        //        //            db.SubmitChanges();
        //        //        }

        //        //        if (alEmail.Count > 0)
        //        //        {
        //        //            SendMailToAll(alEmail);
        //        //        }

        //        //        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
        //        //    }

        //        //}

        //        return Json("", JsonRequestBehavior.AllowGet);
        //}



        public void SendMailToAllStudents(ArrayList arrEmail)
        {
            try
            {
                if (arrEmail.Count > 0)
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                    mail.Subject = "New Notification";
                    string Body = "Hello Dear,  You have a new message, Please visit your portal and check it.";
                    mail.Body = Body;
                    mail.IsBodyHtml = true;

                    for (int i = 0; i < arrEmail.Count; i++)
                    {
                        mail.To.Add(new MailAddress(arrEmail[i].ToString()));
                    }

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"],
                        ConfigurationManager.AppSettings["smtpPass"]);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult OutgoingMsgToStudent()
        {
            return View();
        }


        // Brand Student Message To Tutor 

        public JsonResult TutorToStudentMessage()
        {
            try
            {
                db = new CourseDataContext();
                //  var Id = Session["TutorId"].ToString();

                var sub = (from comm in db.tblCommunications
                           join cour in db.SITSPL_tblCourses
                           on comm.CourseId equals cour.CourseId
                           join std in db.SITSPL_tblStudentProfiles
                           on comm.Id equals std.StudentId
                           where comm.UserType == "All-Students-Course" && comm.CreatedBy == "Tutor" && comm.CategoryType == "Course Student" || comm.UserType == "All-Brand-Students" &&
                           comm.CategoryType == "Brand Student" && comm.CreatedBy == "Tutor"

                           select new
                           {
                               comm,
                               cour,
                               std
                           }).ToList();

                var data = sub.Select(x => new
                {
                    x.comm.Id,
                    x.std.Name,
                    x.std.Email,
                    x.comm.UserType,
                    x.comm.Message,
                    x.comm.CategoryType,
                    x.comm.CourseId,
                    x.comm.CommunicationId,
                    x.comm.IsAllStudent,
                    DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),
                    x.cour.CourseName,
                    x.comm.CreatedBy,
                }).OrderByDescending(x => x.CommunicationId).ToList();


                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }




        #region Add Feedback for Student by Dilshad A. on 19 Oct 2020
        public ActionResult AddStudentFeedback()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddStudentFeedbackDetails(tblFeedback objFeedback)
        {
            try
            {
                if (objFeedback != null)
                {
                    db = new CourseDataContext();
                    Int64 intStudentId = 0;
                    objFeedback.DateCreated = DateTime.Now;

                    if (Session["User"] != null)
                    {
                        objFeedback.CreatedBy = Session["User"].ToString();// User Name(Student Name)
                    }
                    else
                    {
                        objFeedback.CreatedBy = "UserType";
                    }

                    Int64.TryParse(Session["StudentId"].ToString(), out intStudentId);
                    if (Session["StudentId"] != null)
                    {
                        objFeedback.Id = intStudentId;
                    }
                    else
                    {
                        objFeedback.Id = intStudentId;
                    }

                    objFeedback.UserType = "Student";
                    //      return null;
                    db.tblFeedbacks.InsertOnSubmit(objFeedback);
                    db.SubmitChanges();
                    return Json(objFeedback.FeedbackId, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Add Feedback for Student by Dilshad A. on 19 Oct 2020


        public ActionResult AddCourseTutor()
        {
            return View();
        }


        public JsonResult CourseTutors()
        {
            try
            {
                db = new CourseDataContext();
                var CourseStudentId = Convert.ToInt32(Session["StudentId"]);



                //var courseid = (from std in db.SITSPL_tblStudentProfiles
                //                join cd in db.SITSPL_tblCourseDetails
                //                on std.StudentId equals cd.StudentId
                //                where std.StudentId == Convert.ToInt32(CourseStudentId)
                //                select new
                //                {
                //                    cd.CourseId
                //                }).OrderByDescending(x=>x.CourseId).FirstOrDefault();

                var data = (from coursetut in db.tblCourseTutors
                            join tut in db.SITSPL_tblTutors
                            on coursetut.TutorId equals tut.TutorId
                            join cour in db.SITSPL_tblCourses
                            on coursetut.CourseId equals cour.CourseId
                            where coursetut.CourseId == 23
                            select new
                            {
                                coursetut.TutorId,
                                tut.TutorName
                            }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ViewInternTutorToStudentComm()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetInternStudentCommByTutor(string strUserType)
        {
            try
            {
                string usertype = strUserType.Trim();
                var intId = Session["InterApllyId"].ToString();

                if (usertype == "All-Intern-Students" && Convert.ToInt32(intId) > 0)
                {
                    var data2 = (from C in db.tblCommunications
                                 join S in db.tblInternApplies
                                 on C.Id equals S.InterApllyId
                                 join T in db.SITSPL_tblTutors
                                 on C.TutorId.Value equals T.TutorId
                                 where S.InterApllyId == Convert.ToInt32(intId) && C.UserType == usertype && C.CreatedBy == "Intern Tutor"
                                 select new
                                 {
                                     C,
                                     T
                                 }).ToList();

                    //      var TutId = db.tblInternApplies.Where(x => x.InterApllyId == intId).Select(x => x.InternshipStructureId).FirstOrDefault();

                    //var TutId = (from apply in db.tblInternApplies
                    //             join intrtut in db.tblInternTutors
                    //             on apply.InternshipStructureId equals intrtut.InternshipId
                    //             where apply.InterApllyId == Convert.ToInt32(intId)
                    //             select new
                    //             {
                    //                 intrtut.TutorId,
                    //                 intrtut.InternshipId
                    //             }).FirstOrDefault();

                    //var TutId = (from us in db.SITSPL_tblUsers
                    //             join apply in db.tblInternApplies
                    //             on us.Id equals apply.InterApllyId
                    //             where apply.InterApllyId == Convert.ToInt32(intId) && us.UserType == "Intern"
                    //             select new
                    //             {
                    //                 apply.InternshipStructureId
                    //             }).FirstOrDefault();

                    //if (TutId != null)
                    //{
                    //    var Tutor = db.SITSPL_tblInternshipStructures.Where(x => x.InternStructureId == TutId.InternshipStructureId).Select(x => new
                    //    {
                    //        x.InternshipId
                    //    }).FirstOrDefault();

                    //    var Tutors = (from tut in db.SITSPL_tblTutors
                    //                     join intrtut in db.tblInternTutors
                    //                     on tut.TutorId equals intrtut.TutorId
                    //                     where intrtut.InternshipId == Tutor.InternshipId
                    //                     select new
                    //                     {
                    //                         tut.TutorName
                    //                     }).FirstOrDefault();
                    //}
                    //else
                    //{
                    //    return Json("", JsonRequestBehavior.AllowGet);
                    //}

                    var data = data2.Select(x => new
                    {
                        x.C.Id,
                        x.C.UserType,
                        x.C.Message,
                        x.C.MessageAllStudents,
                        x.C.MessageAll,
                        DateCreated = x.C.DateCreated.ToString("dd/MM/yyyy"),
                        x.C.CommunicationId,
                        x.T.TutorName
                        // Tutors.TutorName
                    }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();

                    var list = new { data };
                    return Json(list, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult StudentToInternTutorCommunication()
        {
            return View();
        }

        public JsonResult InternTutorsForInternStudent()
        {
            try
            {
                db = new CourseDataContext();

                var InternApllyId = Session["InterApllyId"].ToString();

                //               select InterApllyId, InternshipStructureId from tblInternApply as apply
                //inner join SITSPL_tblUser as us
                //on us.Id = apply.InterApllyId and us.UserType = 'Intern'
                //where apply.InterApllyId = 3;

                //               select InternshipId from SITSPL_tblInternshipStructure as instr
                //                                   where instr.InternStructureId = 1035;

                //               select TutorId from tblInternTutor as inttut
                //                              where inttut.InternshipId = 2;

                var InternStructureId = (from apply in db.tblInternApplies
                                         join us in db.SITSPL_tblUsers
                                         on apply.InterApllyId equals us.Id
                                         where apply.InterApllyId == Convert.ToInt32(InternApllyId) && us.UserType == "Intern"
                                         select new
                                         {
                                             apply.InterApllyId,
                                             apply.InternshipStructureId
                                         }).FirstOrDefault();

                var InternshipId = (from instr in db.SITSPL_tblInternshipStructures
                                    where instr.InternStructureId == InternStructureId.InternshipStructureId
                                    select new
                                    {
                                        instr.InternshipId
                                    }).FirstOrDefault();


                var data = (from inttut in db.tblInternTutors
                            where inttut.InternshipId == InternshipId.InternshipId
                            join tut in db.SITSPL_tblTutors
                            on inttut.TutorId equals tut.TutorId
                            select new
                            {
                                tut.TutorId,
                                tut.TutorName
                            }).ToList();






                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddMessageForStudentToInternTutorCommunication(int TutorId, string Message)
        {
            try
            {
                db = new CourseDataContext();
                tblCommunication objComm = null;

                var Id = Session["InterApllyId"].ToString();

                var InternshipStrId = (from us in db.SITSPL_tblUsers
                                       join apply in db.tblInternApplies
                                       on us.Id equals apply.InterApllyId
                                       where us.UserType == "Intern" && apply.InterApllyId == Convert.ToInt32(Id)
                                       select new
                                       {
                                           apply.InternshipStructureId
                                       }).FirstOrDefault();

                var InternshipId = (from intr in db.SITSPL_tblInternships
                                    join instr in db.SITSPL_tblInternshipStructures
                                    on intr.InternshipId equals instr.InternshipId
                                    where instr.InternStructureId == InternshipStrId.InternshipStructureId

                                    select new
                                    {
                                        instr.InternshipId
                                    }).FirstOrDefault();

                //        var data = db.SITSPL_GetStudentBrandTutorById(Convert.ToInt32(Id)).ToList();

                //var Course = (from cd in db.SITSPL_tblCourseDetails
                //              join std in db.SITSPL_tblStudentProfiles
                //              on cd.StudentId equals std.StudentId
                //              where std.StudentId == Convert.ToInt32(Id)
                //              select new
                //              {
                //                  cd.CourseId
                //              }).FirstOrDefault();

                objComm = new tblCommunication();
                ArrayList alEmail = new ArrayList();



                var IntrTut = (from intrtut in db.tblInternTutors
                               where intrtut.TutorId == TutorId && intrtut.InternshipId == InternshipId.InternshipId

                               select new
                               {
                                   intrtut.TutorId
                               }).FirstOrDefault();

                var Email = (from tut in db.SITSPL_tblTutors
                             where tut.TutorId == IntrTut.TutorId
                             select new
                             {
                                 tut.TutorEmail
                             }).FirstOrDefault();

                alEmail.Add(Email.TutorEmail);

                objComm = new tblCommunication();
                objComm.Id = Convert.ToInt32(Id);
                objComm.UserType = "All-Intern-Students";
                objComm.DateCreated = DateTime.Now;
                objComm.CreatedBy = "Intern Student";
                objComm.TutorId = TutorId;
                objComm.Message = Message;
                objComm.CategoryType = "Intern-Student";
                objComm.InternshipId = InternshipId.InternshipId;
                db.tblCommunications.InsertOnSubmit(objComm);
                db.SubmitChanges();

                if (alEmail.Count > 0)
                {
                    SendMailToAll(alEmail);
                }
                return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult ViewStudentToInternTutorCommunication()
        {
            return View();
        }

        public JsonResult GetInternStudentToInternTutorComm()
        {
            try
            {
                //          db = new CourseDataContext();
                //          var Id = Session["InterApllyId"].ToString();

                //          var list = (from comm in db.tblCommunications
                //                      join intrtut in db.tblInternTutors
                //                      on comm.InternshipId equals intrtut.InternshipId
                //                      join tut in db.SITSPL_tblTutors
                //                      on intrtut.TutorId equals tut.TutorId
                //                      join intr in db.SITSPL_tblInternships
                //                      on comm.Id equals intr.InternshipId
                //                      //join instr in db.SITSPL_tblInternshipStructures
                //                      //on intr.InternshipId equals instr.InternshipId
                //                     where comm.Id == Convert.ToInt32(Id) && intrtut.InternshipId == comm.InternshipId
                //                   //  && comm.InternshipId == instr.InternshipId
                //                     && comm.CreatedBy == "Intern Student" && comm.UserType == "All-Intern-Students"
                //                      select new
                //                      {
                //                          comm,
                //                          tut,
                //                          intr,
                //                          intrtut
                //                      }).ToList();

                //          var data = list.Select(x => new
                //          {
                //              x.comm.InternshipId,
                //              x.intr.InternshipName,
                //              x.comm.CreatedBy,
                //DateCreated =   x.comm.DateCreated.ToString("dd/MM/yyyy"),
                //              x.comm.Message,
                //              x.comm.UserType,
                //              x.comm.CategoryType,
                //              x.tut.TutorName,
                //              x.tut.TutorEmail,
                //              x.comm.CommunicationId,
                //          TutorDOB = x.tut.TutorDOB.ToString("dd/MM/yyyy")

                //          }).OrderByDescending(x=>x.CommunicationId).ToList();

                //          return Json(data, JsonRequestBehavior.AllowGet);

                db = new CourseDataContext();
                var InternId = Session["InterApllyId"].ToString();
                var list = (from comm in db.tblCommunications
                            join intr in db.SITSPL_tblInternships
                            on comm.InternshipId equals intr.InternshipId
                            join apply in db.tblInternApplies
                            on comm.Id equals apply.InterApllyId
                            join tut in db.SITSPL_tblTutors
                            on comm.TutorId.Value equals tut.TutorId
                            join instr in db.SITSPL_tblInternshipStructures
                            on apply.InternshipStructureId equals instr.InternStructureId


                            where comm.CreatedBy == "Intern Student" && comm.Id == Convert.ToInt32(InternId)
                            select new
                            {
                                comm,
                                intr,
                                apply,
                                tut

                            }).ToList();

                var data = list.Select(x => new
                {
                    x.comm.CommunicationId,
                    x.comm.CategoryType,
                    x.comm.InternshipId,
                    x.intr.InternshipName,

                    x.apply.Name,
                    x.apply.Email,
                    DOB = x.apply.DOB.ToString("dd/MM/yyyy"),
                    x.comm.Id,
                    x.comm.Message,
                    x.comm.IsAllInternStudent,
                    DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),
                    x.comm.CreatedBy,
                    x.comm.UserType,
                    x.tut.TutorName,
                    x.tut.TutorEmail,
             TutorDOB =   x.tut.TutorDOB.ToString("dd/MM/yyyy")

                }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();



                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult CourseTutorAddCommunication()
        {
            return View();
        }

        public JsonResult CourseTutorss()
        {
            try
            {
                db = new CourseDataContext();
                var CourseStudentId = Convert.ToInt32(Session["StudentId"]);

                //select distinct stdpro.StudentId,Name,cs.CourseId from SITSPL_tblStudentProfile as stdpro
                //                 inner join SITSPL_tblCourseDetails as cd
                //                on stdpro.StudentId = cd.StudentId
                //               inner join SITSPL_tblCourseStructure as cs
                //                  on stdpro.StructureId = cs.StructureId
                //                  inner join SITSPL_tblCourse as cour
                //                 on cs.CourseId = cour.CourseId;

                //var Course = (from stdpro in db.SITSPL_tblStudentProfiles
                //                join cd in db.SITSPL_tblCourseDetails
                //                on stdpro.StudentId equals cd.StudentId
                //                join cs in db.SITSPL_tblCourseStructures
                //                on stdpro.StructureId equals cs.StructureId
                //                join cour in db.SITSPL_tblCourses
                //                on cs.CourseId equals cour.CourseId
                //                where stdpro.StudentId == Convert.ToInt32(CourseStudentId)
                //                select new
                //                {
                //                    stdpro.StudentId,
                //                    cs.CourseId
                //                }).Distinct().ToList();
                //ArrayList arrCourseId = new ArrayList();

                //if (Course != null)
                //{
                //    for(int i = 0; i < Course.Count; i++)
                //    {
                //        arrCourseId.Add(Course[i].CourseId);
                //    }
                //}


                //var data = (from coursetut in db.tblCourseTutors
                //            join tut in db.SITSPL_tblTutors
                //            on coursetut.TutorId equals tut.TutorId
                //            join cour in db.SITSPL_tblCourses
                //            on coursetut.CourseId equals cour.CourseId
                //            select new
                //            {
                //                coursetut.TutorId,
                //                tut.TutorName
                //            }).ToList();

                var data = (from coursetut in db.tblCourseTutors
                            join cd in db.SITSPL_tblCourseDetails
                            on coursetut.CourseId equals cd.CourseId
                            join tut in db.SITSPL_tblTutors
                            on coursetut.TutorId equals tut.TutorId
                            join cour in db.SITSPL_tblCourses
                            on coursetut.CourseId equals cour.CourseId
                            where cd.StudentId == Convert.ToInt32(CourseStudentId)
                            select new
                            {
                                coursetut.TutorId,
                                tut.TutorName
                            }).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult GetCourseOfCourseTutor(int Id)
        {
            try
            {
                db = new CourseDataContext();
                var StudentId = Convert.ToInt32(Session["StudentId"]);

                var data = (from coursetut in db.tblCourseTutors
                            join cd in db.SITSPL_tblCourseDetails
                            on coursetut.CourseId equals cd.CourseId
                            join tut in db.SITSPL_tblTutors
                            on coursetut.TutorId equals tut.TutorId
                            join cour in db.SITSPL_tblCourses
                            on coursetut.CourseId equals cour.CourseId
                            where cd.StudentId == Convert.ToInt32(StudentId) && tut.TutorId == Id
                            select new
                            {

                                cour.CourseId,
                                cour.CourseName
                            }).Distinct().ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public JsonResult CourseStudentToTutorAddCommunication(int TutorId, int CourseId, string Message)
        {
            try
            {

                db = new CourseDataContext();
                var StudentId = Convert.ToInt32(Session["StudentId"]);
                tblCommunication objComm = null;

                ArrayList tutEmail = new ArrayList();

                objComm = new tblCommunication();

                //     tutEmail.Add(lstInternTutor[i].Email);

                objComm = new tblCommunication();
                var TutorsId = db.SITSPL_tblTutors.Where(x => x.TutorId == TutorId).Select(x => new { x.TutorEmail }).FirstOrDefault();

                objComm.Id = Convert.ToInt32(StudentId);
                objComm.UserType = "All-Students-Course";
                objComm.DateCreated = DateTime.Now;
                objComm.CreatedBy = "Course Student";
                objComm.Message = Message;
                objComm.CategoryType = "Course-Student";
                objComm.CourseId = CourseId;
                objComm.TutorId = TutorId;
                db.tblCommunications.InsertOnSubmit(objComm);
                db.SubmitChanges();

                if (tutEmail.Count > 0)
                {
                    SendMailToAllTutor(tutEmail);
                }
                return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



        public void SendMailToAllTutor(ArrayList arrEmail)
        {
            try
            {
                if (arrEmail.Count > 0)
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                    mail.Subject = "New Notification";
                    string Body = "Hello Dear,  You have a new message, Please visit your portal and check it.";
                    mail.Body = Body;
                    mail.IsBodyHtml = true;

                    for (int i = 0; i < arrEmail.Count; i++)
                    {
                        mail.To.Add(new MailAddress(arrEmail[i].ToString()));
                    }

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"],
                        ConfigurationManager.AppSettings["smtpPass"]);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // Course Tutor To Course Student Msg View

        public ActionResult ViewTutorCommToStudent()
        {
            return View();
        }

        public JsonResult GetTutorCommunicationToStudent()
        {
            try
            {
                db = new CourseDataContext();

                //              select distinct CommunicationId,Id,Message,
                //UserType,comm.CreatedBy,comm.DateCreated,
                //CategoryType,cd.CourseId,comm.TutorId,tut.TutorName,tut.TutorEmail,
                //tut.TutorDOB,tut.TutorImage,cour.CourseName from tblCommunication as comm
                //inner join SITSPL_tblCourse as cd
                //on cd.CourseId = comm.CourseId
                //inner join SITSPL_tblTutor as tut
                //on tut.TutorId = comm.TutorId
                //inner join SITSPL_tblCourse as cour
                //on comm.CourseId = cour.CourseId
                //where comm.CommunicationId = 10215 and comm.CreatedBy = 'Course Student';

                var TutorId = Session["TutorId"].ToString();

                //var list = (from comm in db.tblCommunications
                //            join tut in db.SITSPL_tblTutors
                //            on comm.TutorId equals tut.TutorId
                //            join cour in db.SITSPL_tblCourses
                //            on comm.CourseId equals cour.CourseId
                //            where comm.CreatedBy = "Course Student" && comm.Id = Convert.ToInt32(StudentId)
                //            select new
                //            {
                //                comm,
                //                cour,
                //                tut
                //            }).ToList();

                var list = (from comm in db.tblCommunications
                            join cour in db.SITSPL_tblCourses
                            on comm.CourseId equals cour.CourseId
                            join std in db.SITSPL_tblStudentProfiles
                            on comm.Id equals std.StudentId
                            where comm.CreatedBy == "Tutor" && comm.TutorId == Convert.ToInt32(TutorId)
                            select new
                            {
                                comm,
                                std,
                                cour
                            }).ToList();

                var data = list.Select(x => new
                {
                    x.comm.CommunicationId,
                    x.comm.CategoryType,
                    x.comm.CourseId,
                    x.cour.CourseName,
                    x.std.Name,
                    x.std.Email,
                    DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                    x.comm.Id,
                    x.comm.Message,
                    x.comm.IsAllStudent,
                    DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),
                    x.comm.CreatedBy,
                    x.comm.UserType,

                }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // Course Student To Tutor Incoming

        public ActionResult CourseStudentToTutorComm()
        {
            return View();
        }


        public JsonResult GetCourseTutorIncoming(string strUserType)
        {
            try
            {
                db = new CourseDataContext();
                var Tutor = Session["TutorId"].ToString();
                string usertype = strUserType.Trim();

                if (usertype == "All-Students-Course" && Convert.ToInt32(Tutor) > 0)
                {

                    var data2 = (from comm in db.tblCommunications
                                 join tut in db.SITSPL_tblTutors
                                 on comm.TutorId.Value equals tut.TutorId
                                 join cour in db.SITSPL_tblCourses
                                 on comm.CourseId equals cour.CourseId
                                 join std in db.SITSPL_tblStudentProfiles
                                 on comm.Id equals std.StudentId
                                 where comm.TutorId == Convert.ToInt32(Tutor) && comm.CreatedBy == "Course Student"
                                 select new
                                 {
                                     comm,
                                     tut,
                                     std
                                 }).ToList();

                    var data = data2.Select(x => new
                    {
                        x.comm.CommunicationId,
                        x.comm.Message,
                        DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),
                        x.std.Name
                    }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();

                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }


        }

        // Tutor To Course Student Incoming View

        public ActionResult TutorToCourseStudentIncoming()
        {
            return View();
        }

        public JsonResult GetTutorToCourseStudentIncomingComm(string strUserType)
        {
            try
            {
                db = new CourseDataContext();

                Int64 intStudentId = 0;
                Int64.TryParse(Session["StudentId"].ToString(), out intStudentId);

                string usertype = strUserType.Trim();

                if (usertype == "All-Students-Course" && intStudentId > 0)
                {

                    var data2 = (from comm in db.tblCommunications
                                 join tut in db.SITSPL_tblTutors
                                 on comm.TutorId.Value equals tut.TutorId
                                 join cour in db.SITSPL_tblCourses
                                 on comm.CourseId equals cour.CourseId
                                 join std in db.SITSPL_tblStudentProfiles
                                 on comm.Id equals std.StudentId
                                 where comm.Id == intStudentId && comm.CreatedBy == "Tutor"
                                 select new
                                 {
                                     comm,
                                     tut,
                                     std
                                 }).ToList();

                    var data = data2.Select(x => new
                    {
                        x.comm.CommunicationId,
                        x.comm.Message,
                        DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),
                        x.tut.TutorName
                    }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();

                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // Course Student To Course Tutor Outgoing Message

        public ActionResult CourseStudentToTutorOutgoing()
        {
            return View();
        }


        public JsonResult GetStudentCommunicationToCourseTutor()
        {
            try
            {
                db = new CourseDataContext();

                //              select distinct CommunicationId,Id,Message,
                //UserType,comm.CreatedBy,comm.DateCreated,
                //CategoryType,cd.CourseId,comm.TutorId,tut.TutorName,tut.TutorEmail,
                //tut.TutorDOB,tut.TutorImage,cour.CourseName from tblCommunication as comm
                //inner join SITSPL_tblCourse as cd
                //on cd.CourseId = comm.CourseId
                //inner join SITSPL_tblTutor as tut
                //on tut.TutorId = comm.TutorId
                //inner join SITSPL_tblCourse as cour
                //on comm.CourseId = cour.CourseId
                //where comm.CommunicationId = 10215 and comm.CreatedBy = 'Course Student';

                Int64 intStudentsId = 0;
                Int64.TryParse(Session["StudentId"].ToString(), out intStudentsId);

                //var list = (from comm in db.tblCommunications
                //            join tut in db.SITSPL_tblTutors
                //            on comm.TutorId equals tut.TutorId
                //            join cour in db.SITSPL_tblCourses
                //            on comm.CourseId equals cour.CourseId
                //            where comm.CreatedBy = "Course Student" && comm.Id = Convert.ToInt32(StudentId)
                //            select new
                //            {
                //                comm,
                //                cour,
                //                tut
                //            }).ToList();

                var list = (from comm in db.tblCommunications
                            join cour in db.SITSPL_tblCourses
                            on comm.CourseId equals cour.CourseId
                            join std in db.SITSPL_tblStudentProfiles
                            on comm.Id equals std.StudentId
                            join tut in db.SITSPL_tblTutors
                            on comm.TutorId.Value equals tut.TutorId
                            where comm.CreatedBy == "Course Student" && comm.Id == Convert.ToInt32(intStudentsId)
                            select new
                            {
                                comm,
                                std,
                                cour,
                                tut
                            }).ToList();

                var data = list.Select(x => new
                {
                    x.comm.CommunicationId,
                    x.comm.CategoryType,
                    x.comm.CourseId,
                    x.cour.CourseName,
                    x.std.Name,
                    x.std.Email,
                    DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                    x.comm.Id,
                    x.comm.Message,
                    x.comm.IsAllStudent,
                    DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),
                    x.comm.CreatedBy,
                    x.comm.UserType,
                    x.tut.TutorName,
                    x.tut.TutorEmail,
                    TutorDOB = x.tut.TutorDOB.ToString("dd/MM/yyyy")

                }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // Student Profile Course Details Page

        public ActionResult StudentprofileCoursedetail()
        {
            return View();
        }


        // Communication

        public ActionResult StudentprofileCommunication()
        {
            return View();
        }


        // Brand Tutor

        public ActionResult StudentprofileBrandTutor()
        {
            return View();
        }


        // Payment Mode

        public ActionResult StudentprofilePayment()
        {
            return View();
        }

        // Notification

        public ActionResult StudentprofileNotification()
        {
            return View();
        }

        // Close Account

        public ActionResult StudentprofileCloseAccount()
        {
            return View();
        }

        // Brand Tutor Profile

        public ActionResult BrandTutorProfile()
        {
            return View();
        }

        #region Get Course Details Based on Student Category (12th, UG,PG etc. Whatever Student selected at regitration time)Modified By Atul Sh.  on 9 December 2020
        public JsonResult GetCoursesByStudentCategoryId()
        {
            try
            {
                Int64 intStudentId = 0;
                Int64 intBatchId = 0;
                int intSubCatgId = 0;
                int intCategId = 0;
                if (Session["StudentId"] != null)
                {
                    Int64.TryParse(Session["StudentId"].ToString(), out intStudentId);
                    Int64.TryParse(Session["BatchId"].ToString(), out intBatchId);
                    Int32.TryParse(Session["SubCategoryId"].ToString(), out intSubCatgId);
                    Int32.TryParse(Session["CategoryId"].ToString(), out intCategId);
                    db = new CourseDataContext();
                    //var CategId = "";
                    //var SubCatgId = "";
                    //var BatchId = "";
                    //if (intStudentId > 0)
                    //{
                    //    CategId = Session["CategoryId"].ToString();
                    //    SubCatgId = Session["SubCategoryId"].ToString();
                    //    BatchId = Session["BatchId"].ToString();
                    //}
                    var data = (from d in db.SITSPL_tblCourseStructures
                                    //       join d1 in db.SITSPL_tblStudentProfiles on d.StructureId equals d1.CourseStructureId
                                join d2 in db.SITSPL_tblCourses on d.CourseId equals d2.CourseId
                                join dur in db.tblDurations on d.DurationId equals dur.DurationId
                                join d3 in db.tblStudentSubCategories on d.StudentSubCategoryId equals d3.StudedetSubCategoryId
                                where
                                //d1.StudentId == intStudentId &&
                                d.StdCatgId == intCategId && d.StudentSubCategoryId.Value == intSubCatgId &&
                                d.BatchId.Value == intBatchId
                                select new
                                {
                                    d2.CourseId,
                                    d2.CourseName
                                }).Distinct().ToList();

                    //var data = db.SITSPL_tblCourses.Select(x => new { x.CourseId, x.CourseName }).ToList();
                    if (data != null)
                    {
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string str = ex.Message;

                return Json("Application Error : " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion END Get Course Details Based on Student Category (Whatever Student selected at regitration time) By Dilshad A. on 2 Nov 2020


        // Brand Student Incoming Message from Brand Tutor

        public ActionResult BrandTutorCommToBrandStudents()
        {
            return View();
        }

        public JsonResult GetBrandTutorToBrandStudentIncomingComm(string strUserType)
        {
            try
            {
                db = new CourseDataContext();

                Int64 intStudentId = 0;
                Int64.TryParse(Session["StudentId"].ToString(), out intStudentId);

                string usertype = strUserType.Trim();

                if (usertype == "All-Brand-Students" && intStudentId > 0)
                {

                    var data2 = (from comm in db.tblCommunications
                                 join tut in db.SITSPL_tblTutors
                                 on comm.TutorId.Value equals tut.TutorId
                                 join cour in db.SITSPL_tblCourses
                                 on comm.CourseId equals cour.CourseId
                                 join std in db.SITSPL_tblStudentProfiles
                                 on comm.Id equals std.StudentId
                                 where comm.Id == intStudentId && comm.CreatedBy == "Brand Tutor"
                                 select new
                                 {
                                     comm,
                                     tut,
                                     std
                                 }).ToList();

                    var data = data2.Select(x => new
                    {
                        x.comm.CommunicationId,
                        x.comm.Message,
                        DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),
                        x.tut.TutorName
                    }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();

                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #region Add Student Course(s) Details on 05 Nov 2020
        [HttpPost]
        public JsonResult AddStudentCourses(List<MultipleCourses> multicourse)
        {
            try
            {
                bool result = false;
                Int64 intStudentId = 0;
                Int64.TryParse(Session["StudentId"].ToString(), out intStudentId);
                string Email = string.Empty;
                if (Session["Email"] != null)
                {
                    Email = Session["Email"].ToString();
                }               
                

                using (CourseDataContext db = new CourseDataContext())
                {
                    
                    
                    #region Add Course Details                        
                    if (intStudentId > 0)
                    {
                        if (multicourse != null)
                        {
                            for (int i = 0; i < multicourse.Count; i++)
                            {
                             
                                    SITSPL_tblCourseDetail cd = new SITSPL_tblCourseDetail();
                                    cd.CourseId = Convert.ToInt32(multicourse[i].Course);
                                    cd.Duration = multicourse[i].Duration;
                                    cd.StudentId = intStudentId;
                                    cd.StudentEmail = Email;
                                    DateTime dtValidFrom = DateTime.ParseExact(multicourse[i].ValidFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    DateTime dtValidTo = DateTime.ParseExact(multicourse[i].ValidTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    cd.CourseValidFrom = dtValidFrom;
                                    cd.CourseValidTo = dtValidTo;
                                    cd.DiscountPercent = multicourse[i].Discount;
                                    cd.Fees = multicourse[i].Fees;
                                    cd.FeesWithDiscount = multicourse[i].FeesWithDiscount;
                                    cd.Month = multicourse[i].Month;
                                    cd.JoiningDate = multicourse[i].JoiningDate;
                                    cd.NetAmountToPay = multicourse[i].NetAmount;
                                    cd.DateCreated = DateTime.Now;
                                    cd.CreatedBy = Session["User"].ToString();
                                    db.SITSPL_tblCourseDetails.InsertOnSubmit(cd);
                                    db.SubmitChanges();
                                result = true;
                            }
                        }
                       
                    }
                    #endregion END Course Details   
                                       
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        // Admin To Student Incoming

        public ActionResult AdminToCourseStudentIncoming()
        {
            return View();
        }

        public JsonResult AdminToCourseStudentIncomingComm(string strUserType)
        {
            try
            {
                db = new CourseDataContext();

                Int64 intStudentId = 0;
                Int64.TryParse(Session["StudentId"].ToString(), out intStudentId);

                string usertype = strUserType.Trim();

                if (usertype == "All-Students-Course" && intStudentId > 0)
                {

                    var data2 = (from comm in db.tblCommunications
                                 
                                 join std in db.SITSPL_tblStudentProfiles
                                 on comm.Id equals std.StudentId
                                 where comm.Id == intStudentId && comm.UserType == usertype && comm.CreatedBy == "Admin"
                                 select new
                                 {
                                     comm,
                                     
                                     std
                                 }).ToList();

                    var data = data2.Select(x => new
                    {
                        x.comm.CommunicationId,
                        x.comm.Message,
                        DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),
                        
                    }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();

                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult AdminToInternStudentIncoming()
        {
            return View();
        }

        public JsonResult AdminToInternStudentIncomingComm(string strUserType)
        {
            try
            {
                db = new CourseDataContext();

                Int64 intStudentId = 0;
                Int64.TryParse(Session["InterApllyId"].ToString(), out intStudentId);

                string usertype = strUserType.Trim();

                if (usertype == "All-Intern-Students" && intStudentId > 0)
                {

                    var data2 = (from comm in db.tblCommunications

                                 join std in db.tblInternApplies
                                 on comm.Id equals std.InterApllyId
                                 where comm.Id == intStudentId && comm.UserType == usertype && comm.CreatedBy == "Admin"
                                 select new
                                 {
                                     comm,

                                     std
                                 }).ToList();

                    var data = data2.Select(x => new
                    {
                        x.comm.CommunicationId,
                        x.comm.Message,
                        DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),

                    }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();

                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AdminToCourseTutorIncomingComm(string strUserType)
        {
            try
            {
                db = new CourseDataContext();

                Int64 intTutorId = 0;
                Int64.TryParse(Session["TutorId"].ToString(), out intTutorId);

                string usertype = strUserType.Trim();

                if (usertype == "All-Course-Tutor" && intTutorId > 0)
                {

                    var data2 = (from comm in db.tblCommunications

                                 join tut in db.SITSPL_tblTutors
                                 on comm.TutorId.Value equals tut.TutorId
                                 
                                 where comm.TutorId == intTutorId && comm.UserType == usertype && comm.CategoryType == "Course"
                                 && comm.CreatedBy == "Admin"
                                 select new
                                 {
                                     comm,

                                     tut
                                 }).ToList();

                    var data = data2.Select(x => new
                    {
                        x.comm.CommunicationId,
                        x.comm.Message,
                        DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),

                    }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();

                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // StudentprofileCloseAccount

        // Intern Profile Close Account

        public ActionResult InternProfileCloseAccount()
        {
            return View();
        }

        public ActionResult InternProfilePayment()
        {
            return View();
        }

        public ActionResult InternshipDetails()
        {
            return View();
        }

        public ActionResult InternQualification()
        {
            return View();
        }

        public ActionResult InternshipDocument()
        {
            return View();
        }


        [HttpPost]
        public JsonResult UpdateInternProfileQualification(tblInternApply updateinternqualification)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                Int64 intInternId = 0;
                Int64.TryParse(Session["InterApllyId"].ToString(), out intInternId);

                if(intInternId > 0) { 
                tblInternApply internApply = db.tblInternApplies.Where(x => x.InterApllyId == Convert.ToInt64(intInternId)).FirstOrDefault();

                if(internApply != null) { 

                if (updateinternqualification.HowDoYouKnow != null)
                {
                    internApply.HowDoYouKnow = updateinternqualification.HowDoYouKnow;
                }

                if (updateinternqualification.YearOfPassing != null)
                {
                    internApply.YearOfPassing = updateinternqualification.YearOfPassing;
                }

                //if (updateinternqualification.Comments != null)
                //{
                //    internApply.Comments = updateinternqualification.Comments;
                //}

                internApply.CollegeUniv = updateinternqualification.CollegeUniv;
                internApply.Qualification = updateinternqualification.Qualification;
                    db.SubmitChanges();
                        result = true;
                        return Json(result, JsonRequestBehavior.AllowGet);
                }
                    else
                    {
                        return Json("problem", JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


               

            }
            catch (Exception ex)
            {

                return Json(ex.Message,JsonRequestBehavior.AllowGet);
            }
        }


        // Update Intern Profile Documents

        [HttpPost]
        public ContentResult UpdateInternProfileDocument(List<InternDocumentsUpdate> lstDocuments, 
            HttpPostedFileBase resumePostedfile, HttpPostedFileBase aadhaarPostedfile)
        {
            try
            {
                db = new CourseDataContext();                
                    Int64 intInternId = 0;
                    Int64.TryParse(Session["InterApllyId"].ToString(), out intInternId);
                    tblInternApply internApply = db.tblInternApplies.Where(x => x.InterApllyId == Convert.ToInt64(intInternId)).FirstOrDefault();

                    //Int64 InternId = InternshipId; 

                    if (intInternId > 0)
                    {                      
                        if (lstDocuments != null)
                        {                         
                            for (int i = 0; i < lstDocuments.Count; i++)
                            {
                                SITSPL_tblDocument objDoc = db.SITSPL_tblDocuments.Where(x => x.DocumentId == lstDocuments[i].DocumentId && x.UserId == lstDocuments[i].UserId).FirstOrDefault();
                                if (objDoc != null)
                                {
                                    objDoc.UserId = intInternId;
                                    //  objDoc.UserId = InternshipId;
                                    objDoc.PanNo = "-";
                                    objDoc.DoucmentNo = lstDocuments[i].DoucmentNo;
                                    //     objDoc.DocumentType = "-";
                                    objDoc.DocumentType = lstDocuments[i].DocumentType;
                                    objDoc.DocumentName = lstDocuments[i].DocumentName;
                                    objDoc.LastUpdated = DateTime.Now;
                                    objDoc.UpdatedBy = internApply.Name;

                                    db.SubmitChanges();
                                }
                            }
                        }


                        #region Save Resume
                        if (resumePostedfile != null)
                        {
                            var resumePath = Server.MapPath("~/Intern/Resume/");
                            var resumeName = resumePostedfile.FileName;
                            resumePath = resumePath + resumeName;
                            resumePostedfile.SaveAs(resumePath);
                        }
                        #endregion END Save Resume

                        #region Save Aadhaar
                        if (aadhaarPostedfile != null)
                        {
                            var aadhaarPath = Server.MapPath("~/Intern/Aadhaar/");
                            var aadhaarName = aadhaarPostedfile.FileName;
                            aadhaarPath = aadhaarPath + aadhaarName;
                            aadhaarPostedfile.SaveAs(aadhaarPath);
                        }
                        #endregion END Save Aadhaar

                    }
                return Content("Updated");
            }
                
            
            catch (Exception ex)
            {
                return Content("Error");
            }
        }

        #region View Resource to Student By Dilshad A. on 05 Nov 2020
        public ActionResult ViewResource()
        {
            return View();
        }

        public JsonResult ViewResourceDetailsForStudent()
        {
            try
            {
                db = new CourseDataContext();
                Int64 intStudentId = 0;
                //if (Session["StudentId"] != null)
                //{
                //    Int64.TryParse(Session["StudentId"].ToString(), out intStudentId);
                //}
                //var data = (from d in db.SITSPL_tblStudentProfiles
                //            join d1 in db.tblCourseTutors on d.TutorId equals d1.TutorId
                //            join d2 in db.tblCourseResources on d1.CourseId equals d2.CourseId
                //            join d3 in db.SITSPL_tblCourses on d2.CourseId equals d3.CourseId
                //            join d4 in db.SITSPL_tblCourseStructures on d.CourseStructureId equals d4.StructureId
                //            where d.StudentId == intStudentId
                //            select new
                //            {        
                //                d,
                //                d1,
                //                d2,
                //                d3
                //            }).ToList();

                //var list = data.Select(x => new
                //{
                //    x.d2.ResourceId,
                //    x.d2.CourseId,
                //    x.d3.CourseName,
                //    x.d2.ResourceName,
                //    x.d2.ResourceType,
                //    x.d2.IsPublished,
                //    DateCreated = x.d2.DateCreated.ToString("dd/MM/yyyy"),
                //    x.d2.CreatedBy
                //}).ToList();

                //return Json(list, JsonRequestBehavior.AllowGet);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END View Resource By Dilshad A. on 05 Nov 2020

        #region Download Resource File By Dilshad on 05 Nov 2020        
        public FileResult DownloadBrandTutorResource(string strFileName)
        {
            string strFilePath = ConfigurationManager.AppSettings["downloadBrandTutorResources"];
            if (!string.IsNullOrEmpty(strFilePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(strFilePath + strFileName);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, strFileName);
            }
            return null;
        }
        #endregion END Download Resource File By Dilshad on 05 Nov 2020


        // Download Resource For Course Student

        public ActionResult DownloadResourceForCourseStudent()
        {
            return View();
        }

        // Download Resource For Brand Student

        public ActionResult DownloadResourcesForBrandStudent()
        {
            return View();
        }

        public ActionResult DownloadResourcesForInternStudent()
        {
            return View();
        }

        // Admin To Tutor Incoming

        public ActionResult AdminToTutorIncomingCommunication()
        {
            return View();
        }


        public JsonResult InternshipStructureDurationBasedOnId(int Id)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from dur in db.tblDurations
                            where dur.DurationId == Id
                            select new
                            {
                                dur.DurationName
                            }).FirstOrDefault();

                return Json(data.DurationName, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message,JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult InternshipStructureInternshipBasedOnId(int Id)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from intr in db.SITSPL_tblInternships
                            where intr.InternshipId == Id
                            select new
                            {
                                intr.InternshipName
                            }).FirstOrDefault();

                return Json(data.InternshipName, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ChangeBrandStudentPassword()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ChangeBrandStudentsPassword(string strOldPassword, string strNewPassword)
        {
            try
            {
                Int64 intStudentId = 0;
                if (Session["StudentId"].ToString() != null)
                {
                    Int64.TryParse(Session["StudentId"].ToString(), out intStudentId);
                }

                var data = (from d in db.SITSPL_tblUsers
                            where d.Id == intStudentId &&
                            d.Password == strOldPassword
                            select d).SingleOrDefault();

                if (data != null)
                {
                    data.Password = strNewPassword;
                    db.SubmitChanges();
                    return Json(intStudentId.ToString(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("OldPasswordNotMatch", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
       public JsonResult GetAdminCommunicationForInternTutorById(string strUserType)

        {
            try
            {
                db = new CourseDataContext();

                Int64 intInternTutorId = 0;
                Int64.TryParse(Session["InternTutorId"].ToString(), out intInternTutorId);

                string usertype = strUserType.Trim();

                if (usertype == "All-Intern-Tutor" && intInternTutorId > 0)
                {

                    var data2 = (from comm in db.tblCommunications

                                 join tut in db.SITSPL_tblTutors
                                 on comm.TutorId.Value equals tut.TutorId

                                 where comm.TutorId == intInternTutorId && comm.UserType == usertype
                                 && comm.CreatedBy == "Admin" && comm.CategoryType == "Intern"
                                 select new
                                 {
                                     comm,

                                     tut
                                 }).ToList();

                    var data = data2.Select(x => new
                    {
                        x.comm.CommunicationId,
                        x.comm.Message,
                        DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),

                    }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();

                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult BatchData()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from batch in db.SITSPL_tblBatches
                            where batch.IsPublished == true
                            select new
                            {
                                batch.BatchId,
                                batch.BatchSize
                            }).OrderByDescending(x=>x.BatchId).ToList();
                if(data != null)
                { 
                return Json(data, JsonRequestBehavior.AllowGet);
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message,JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult BatchGroupdata()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from batch in db.SITSPL_tblBatchGroups
                            where batch.IsPublished == true
                            select new
                            {
                                batch.BatchGroupId,
                                batch.BatchName
                            }).OrderByDescending(x => x.BatchGroupId).ToList();
                if (data != null)
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        
        public ActionResult CourseStudentToAdminOutbox()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SendCourseStudentMsgToAdmin(string MessageToAdmin)
        {
            try
            {
                db = new CourseDataContext();
                int intStudentId = 0;
                Int32.TryParse(Session["StudentId"].ToString(), out intStudentId);
                tblCommunication objComm = null;
                ArrayList alEmail = new ArrayList();

                var lstAdmin = (from us in db.SITSPL_tblUsers
                            where us.UserType == "SuperAdmin" || us.UserType == "Admin"
                            select new
                            {
                                us.UserId,
                                us.UserName,
                                us.UserType
                            }).ToList();

                for (int i = 0; i < lstAdmin.Count; i++)
                {

                    alEmail.Add("atulsh820@gmail.com");
                    objComm = new tblCommunication();
                    objComm.Id = intStudentId;
                    objComm.AdminId = lstAdmin[i].UserId;
                    objComm.UserType = lstAdmin[i].UserType;
                    objComm.DateCreated = DateTime.Now;
                    objComm.CreatedBy = "CourseStudent";
                    objComm.Message = MessageToAdmin;
                    objComm.CategoryType = "CourseStudentToAdmin";
                    db.tblCommunications.InsertOnSubmit(objComm);
                    db.SubmitChanges();
               
                }
                if (alEmail.Count > 0)
                {
                    SendMailToAll(alEmail);
                }
                return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult CourseTutorToAdminOutbox()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SendCourseTutorMsgToAdmin(string MessageToAdmin)
        {
            try
            {
                db = new CourseDataContext();
                int intTutorId = 0;
                Int32.TryParse(Session["TutorId"].ToString(), out intTutorId);
                tblCommunication objComm = null;
                ArrayList alEmail = new ArrayList();

                var lstAdmin = (from us in db.SITSPL_tblUsers
                                where us.UserType == "SuperAdmin" || us.UserType == "Admin"
                                select new
                                {
                                    us.UserId,
                                    us.UserName,
                                    us.UserType
                                }).ToList();

                for (int i = 0; i < lstAdmin.Count; i++)
                {

                    alEmail.Add("atulsh820@gmail.com");
                    objComm = new tblCommunication();
                    objComm.TutorId = intTutorId;
                    objComm.AdminId = lstAdmin[i].UserId;
                    objComm.UserType = lstAdmin[i].UserType;
                    objComm.DateCreated = DateTime.Now;
                    objComm.CreatedBy = "CourseTutor";
                    objComm.Message = MessageToAdmin;
                    objComm.CategoryType = "CourseTutorToAdmin";
                    db.tblCommunications.InsertOnSubmit(objComm);
                    db.SubmitChanges();

                }
                if (alEmail.Count > 0)
                {
                    SendMailToAll(alEmail);
                }
                return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


    }
}

