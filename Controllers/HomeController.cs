using OnlineCoaching.Linq_To_Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace OnlineCoaching.Controllers
{
    public class HomeController : Controller
    {
        CourseDataContext db = null;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }



        public ActionResult Careers()
        {
            return View();
        }

        public ActionResult LearnMore()
        {
            return View();
        }

        #region Add For Contact(Any Candidate can contant via this Name,Email and message) By Dilshad A. on 24 Nov 2020
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddForContact(tblContact tblContact)
        {
            //return null;
            try
            {
                if (tblContact != null)
                {
                    if (string.IsNullOrEmpty(tblContact.Name) && string.IsNullOrEmpty(tblContact.Email) && string.IsNullOrEmpty(tblContact.Mobile)
                        && string.IsNullOrEmpty(tblContact.Message) && string.IsNullOrEmpty(tblContact.Country) && string.IsNullOrEmpty(tblContact.Subject))
                    {
                        return Json("AllFieldsRequired", JsonRequestBehavior.AllowGet);
                    }
                    db = new CourseDataContext();
                    tblContact.DateCreated = DateTime.Now;
                    tblContact.CreatedBy = tblContact.Name;
                    tblContact.IsPublished = true;
                    db.tblContacts.InsertOnSubmit(tblContact);
                    db.SubmitChanges();
                    SendOTPForEnqueryContactFAQ(tblContact.Name, tblContact.Email, "Contact", "ContactRes");
                    return Json(tblContact.ContactId, JsonRequestBehavior.AllowGet);
                }
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Add For Contact(Any Candidate can contant via this Name,Email and message) By Dilshad A. on 24 Nov 2020



        #region Send Otp For Enquiry
        [HttpPost]
        public JsonResult SendOtpForEnquiry(string Name, string Email, string UserType)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                SendOTPForEnqueryContactFAQ(Name, Email, "Enquiry", "");
                result = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Send Otp For Enquiry


        #region Confirm Enquiry Otp
        [HttpPost]
        public JsonResult ConfirmEnquiryOtp(string OTP)
        {
            try
            {
                bool result = false;
                if (Session["OTPEnqueryContactFAQ"].ToString() != null && OTP != null)
                {
                    if (Session["OTPEnqueryContactFAQ"].ToString() == OTP)
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
        #endregion End Confirm Enquiry OTP

        public ActionResult Enquiry()
        {
            return View();
        }
        
        #region Add Enquiry Post Method 
        [HttpPost]
        public JsonResult AddEnquiry(tblEnquiry objEnquiry)
        {
            try
            {
                if (objEnquiry != null)
                {
                    db = new CourseDataContext();
                    tblEnquiry addenquiry = new tblEnquiry();

                    addenquiry.CandidateType = objEnquiry.CandidateType;
                    addenquiry.Name = objEnquiry.Name;
                    addenquiry.Mobile = objEnquiry.Mobile;
                    addenquiry.Email = objEnquiry.Email;
                    addenquiry.EnquiryType = objEnquiry.EnquiryType;
                    addenquiry.Message = objEnquiry.Message;
                    if(objEnquiry.EnquiryType == "Intern")
                    {
                        addenquiry.InternshipId = objEnquiry.InternshipId;
                    }
                    else if(objEnquiry.EnquiryType == "Course")
                    {
                        addenquiry.StudentCategoryId = objEnquiry.StudentCategoryId;
                        addenquiry.CourseId = objEnquiry.CourseId;
                    }

                    addenquiry.DateCreated = DateTime.Now;
                    addenquiry.CreatedBy = objEnquiry.Name;
                    addenquiry.IsPublished = true;
                    db.tblEnquiries.InsertOnSubmit(addenquiry);
                    db.SubmitChanges();
                    SendOTPForEnqueryContactFAQ(objEnquiry.Name,objEnquiry.Email, "Enquiry", "EnquiryRes");
                    return Json(addenquiry.EnquiryId, JsonRequestBehavior.AllowGet);
                }
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message,JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Add Enquiry


        public ActionResult Working()
        {
            return View();
        }


        #region Send Otp For FAQ
        [HttpPost]
        public JsonResult SendOtpForFAQDetails(string Name, string Email, string UserType)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                SendOTPForEnqueryContactFAQ(Name, Email, "FAQ", "");
                result = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Send Otp For FAQ

        #region Confirm OTP For FAQ
        [HttpPost]
        public JsonResult ConfirmEmailOtpForFAQ(string OTP)
        {
            try
            {
                bool result = false;
                if (Session["OTPEnqueryContactFAQ"].ToString() != null && OTP != null)
                {
                    if (Session["OTPEnqueryContactFAQ"].ToString() == OTP)
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
        #endregion End Confirm OTP For FAQ

        #region FAQ(Can Ask any query if donot exist in FAQ List) By Dilshad A. on 24 Nov 2020
        public ActionResult FAQ()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddUserFAQDetails(tblUserFAQ tblUserFAQ)
        {
            try
            {
                if (tblUserFAQ != null)
                {
                    db = new CourseDataContext();
                    tblUserFAQ.DateCreated = DateTime.Now;
                    tblUserFAQ.CreatedBy = "Public User";
                    tblUserFAQ.IsPublished = true;
                    db.tblUserFAQs.InsertOnSubmit(tblUserFAQ);
                    db.SubmitChanges();
                    SendOTPForEnqueryContactFAQ(tblUserFAQ.Name, tblUserFAQ.Email, "FAQ", "FAQRes");
                    return Json(tblUserFAQ.UserFAQId, JsonRequestBehavior.AllowGet);
                }
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet); ;
            }
        }
        #endregion END FAQ(Can Ask any query if donot exist in FAQ List) By Dilshad A. on 24 Nov 2020

        #region Send Otp For Contact
        [HttpPost]        
        public JsonResult SendOtpForContact(string Name, string Email, string UserType)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                SendOTPForEnqueryContactFAQ(Name, Email, "Contact", "");
                result = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Send Otp For Contact

       #region Confirm Otp For Contact 
        [HttpPost]
        public JsonResult ConfirmContactOtp(string OTP)
        {
            try
            {
                bool result = false;
                if (Session["OTPEnqueryContactFAQ"].ToString() != null && OTP != null)
                {
                    if (Session["OTPEnqueryContactFAQ"].ToString() == OTP)
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

        #endregion End Confirm Otp For Contact 


        #region Send Otp For Refer
        [HttpPost]
        public JsonResult SendOtpForRefer(string Name, string Email, string UserType)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                SendOTPForEnqueryContactFAQ(Name, Email, "Refer", "");
                result = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion End Send Otp For Refer

        #region Confirm Otp For Refer
        [HttpPost]
        public JsonResult ConfirmReferOtp(string OTP)
        {
            try
            {
                bool result = false;
                if (Session["OTPEnqueryContactFAQ"].ToString() != null && OTP != null)
                {
                    if (Session["OTPEnqueryContactFAQ"].ToString() == OTP)
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
        #endregion End Confirm Otp For Refer
      
        #region ReferFriend By Dilshad A. on 25 Nov 2020        
        public ActionResult ReferFriend()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddReferFriend(tblRefer objRefer)
        {
            try
            {
                if (objRefer != null)
                {
                    db = new CourseDataContext();
                    tblRefer refer = new tblRefer();
                    refer.EmailReferBy = objRefer.EmailReferBy;
                    refer.ReferBy = objRefer.ReferBy;
                    refer.MobileRefereBy = objRefer.MobileRefereBy;
                    refer.ReferTo = objRefer.ReferTo;
                    refer.EmailReferTo = objRefer.EmailReferTo;
                    refer.MobileReferTo = objRefer.MobileReferTo;                  
                    refer.DateCreated = DateTime.Now;
                    refer.CreatedBy = objRefer.ReferBy;
                    refer.IsPublished = true;
                    db.tblRefers.InsertOnSubmit(refer);
                    db.SubmitChanges();
                    Int64 intReferId = refer.ReferId;
                    SendOTPForEnqueryContactFAQ(objRefer.ReferBy,objRefer.EmailReferBy, "Refer", "ReferRes");
                    return Json(intReferId, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion END ReferFriend By Dilshad A. on 25 Nov 2020


       

        #region Send OTP On  Mail regarding Enquery/Contact/FAQ By Dilshad A. on 25 N0v 2020               
        public void SendOTPForEnqueryContactFAQ(string Name, string Email, string msgFor, string MailResponse)
        {            
            MailMessage mail = new MailMessage();
            string Body = "";
            if (!string.IsNullOrEmpty(MailResponse))
            {
                if (MailResponse == "FAQRes")
                {
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                    mail.To.Add(Email);
                    mail.Subject = "Response for:" + msgFor;
                    Body = "Hello " + Name + ": We have received your request. Team will back to you soon.";
                }
               else if (MailResponse == "ContactRes")
                {
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                    mail.To.Add(Email);
                    mail.Subject = "Response for:" + msgFor;
                    Body = "Hello " + Name + ": We have received your request. Team will back to you soon.";
                }
                else if (MailResponse == "ReferRes")
                {
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                    mail.To.Add(Email);
                    mail.Subject = "Response for:" + msgFor;
                    Body = "Hello " + Name + ": We have received your request. Team will back to you soon.";
                }
                else if(MailResponse == "EnquiryRes")
                {
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                    mail.To.Add(Email);
                    mail.Subject = "Response for:" + msgFor;
                    Body = "Hello " + Name + ": We have received your request. Team will back to you soon.";
                }
                    


            }
           
            else
            {
                Random random = new Random();
              //  int intNum = random.Next();
                String intNum = random.Next(0, 1000000).ToString("D6");


                Session["OTPEnqueryContactFAQ"] = intNum;
                
                mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                mail.To.Add(Email);
                mail.Subject = "OTP Regarding :" + msgFor;
                Body = "Hello " + Name + " OTP for " + msgFor + " is:" + intNum;
            }
            
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
        }
        #endregion END Send OTP On  Mail regarding Enquery/Contact/FAQ By Dilshad A. on 25 N0v 2020

        #region View FAQ 

        public ActionResult ViewFAQ()
        {
            return View();
        }

        #endregion End View FAQ

        public JsonResult GetFAQDetail()
        {
            try
            {
                db = new CourseDataContext();
                var list = (from faq in db.tblUserFAQs
                            select new
                            {
                                faq
                            }).ToList();
                var data = list.Select(x => new
                {
                    x.faq.Query,
                    x.faq.Name,
                    x.faq.Mobile,
                    x.faq.UserFAQId,                  
                    x.faq.Email,
              DateCreated = x.faq.DateCreated.ToString("dd/MM/yyyy")
                }).ToList();
                
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


        [HttpPost]
        public JsonResult GetCourseForCategoryEnquiry(int StudentCategoryId)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from cs in db.SITSPL_tblCourseStructures
                            join cour in db.SITSPL_tblCourses
                            on cs.CourseId equals cour.CourseId
                            join dur in db.tblDurations 
                            on cs.DurationId equals dur.DurationId
                            where cs.StdCatgId == StudentCategoryId
                            select new
                            {
                                cour.CourseId,
                                cour.CourseName
                            }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message,JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult FAQByAdmin()
        {
            try
            {
                db = new CourseDataContext();
                var list = (from faq in db.tblFAQs
                            where faq.IsPublished == true
                            select new
                            {
                                faq
                            }).ToList();

                var data = list.Select(x => new
                {
                    x.faq.FAQId,
                    x.faq.Question,
                    x.faq.QuestionType,
                    x.faq.Answer,
                    x.faq.IsPublished,
                    x.faq.CreatedBy,
                    DateCreated = x.faq.DateCreated.ToString("dd/MM/yyyy"),
                    LastUpdated = x.faq.LastUpdated.HasValue ? x.faq.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                    x.faq.UpdatedBy

                }).OrderByDescending(x => x.FAQId).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #region Get CourseName List By CourseName of all category by Dilshad A. on 14 Dec 2020
        public ActionResult GetAllCourseByCourseName()
        {
            return View();
        }

        public JsonResult GetAllCourseByCourseNameDetails(int intCourseId)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.SITSPL_tblCourseStructures
                            join d1 in db.SITSPL_tblCourses on d.CourseId equals d1.CourseId
                            join dur in db.tblDurations on d.DurationId equals dur.DurationId
                            where d.IsPublished == true
                            && d1.CourseId == intCourseId
                            select new
                            {
                                dur.DurationName,
                                d1.CourseName,
                                d.StructureId,
                                d.CourseId
                            }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Get CourseName List By CourseName of all category by Dilshad A. on 14 Dec 2020

        #region Apply for course on CourseDetails page by Dilshad A. on 14 Dec 2020
        public ActionResult ApplyNow()
        {
            return View();
        }
        #endregion END Apply for course on CourseDetails page by Dilshad A. on 14 Dec 2020

        #region Get BatchSize Details and Fee details based on BatchSize Of Registration at the time of apply Course by Dilshad A. on 15 Dec 2020
        public JsonResult GetBatchDetails()
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
                            }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Get BatchSize Details For Registration at the time of apply Course by Dilshad A. on 15 Dec 2020

        #region Get Fee details based on BatchSize Of Registration at the time of apply Course by Dilshad A. on 16 Dec 2020
        public JsonResult GetFeeDetailsByBatchId(int intCourseStrucId)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.SITSPL_tblBatches
                            join d1 in db.SITSPL_tblCourseStructures on d.BatchId equals d1.BatchId
                            where d1.IsPublished == true
                            && d1.StructureId == intCourseStrucId
                            select new
                            {
                                d.BatchId,
                                d1.Fees,
                                d1.DiscountPercent,
                                d1.NetAmount,
                            }).SingleOrDefault();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Get Fee details based on BatchSize Of Registration at the time of apply Course by Dilshad A. on 16 Dec 2020

        #region Get SubHeading(StudentSubCategory) Based on StudentCategoryId(CourseHeading) by Dilshad A. on 17 Dec 2020
        public JsonResult GetSubHeadingByStudentCategoryId()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.tblStudentSubCategories
                            join d1 in db.tblStudentCategories on d.StudedetSubCategoryId equals d1.StudentCategoryId into Cat
                            from d1 in Cat.DefaultIfEmpty()
                            where d.IsPublished == true                            
                            select new
                            {
                                d.StudedetSubCategoryId,
                                d.StudentCategoryId,
                                d.SubCategoryName
                            }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Get SubHeading(StudentSubCategory) Based on StudentCategoryId(CourseHeading) by Dilshad A. on 17 Dec 2020
    }
}