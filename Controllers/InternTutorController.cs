using OnlineCoaching.Angular_Js_Data_Layer;
using OnlineCoaching.Linq_To_Sql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace OnlineCoaching.Controllers
{
    public class InternTutorController : Controller
    {
        CourseDataContext db = null;
        // GET: InternTutor
        public ActionResult Index()
        {
            if (Session["InternTutorId"] == null)
            {
                return RedirectToAction("InternTutorLogin", "Tutor");
            }
            return View();
        }

        public ActionResult InternTutorLogout()
        {
            Session["InternTutorId"] = null;
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("InternTutorLogin", "Tutor");
        }

        public ActionResult ChangeInternTutorPassword()
        {
            if (Session["InternTutorId"] == null)
            {
                return RedirectToAction("InternTutorLogin", "Tutor");
            }
            return View();
        }

        public ActionResult Communication()
        {

            return View();
        }

        public ActionResult ViewStudents()
        {
            return View();
        }

        public JsonResult GetInternStudents()
        {
            try
            {
                db = new CourseDataContext();
                //              select apply.InterApllyId,Name,DOB,Email,Image,CollegeUniv,
                //Qualification,YearOfPassing,apply.DateCreated,HowDoYouKnow

                //from tblInternApply as apply
                //INNER JOIN SITSPL_tblUser as us
                //on us.Id = apply.InterApllyId

                var InternTutorId = Session["InternTutorId"].ToString();

                var list = (from apply in db.tblInternApplies
                            join us in db.SITSPL_tblUsers
                            on apply.InterApllyId equals us.Id
                            
                            
                            select new
                            {
                                apply,
                                us

                            }).ToList();

                var data = list.Select(x => new
                {
                    x.apply.InterApllyId,
                    x.apply.Name,
                   DOB= x.apply.DOB.ToString("dd/MM/yyyy"),
                   x.apply.Email,
                   x.apply.Image,
                   x.apply.CollegeUniv,
                   x.apply.Qualification,
                   x.apply.YearOfPassing,
                    DateCreated = x.apply.DateCreated.HasValue ? x.apply.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                    x.apply.HowDoYouKnow,
                   x.us.UserType,
                   x.us.UserName
                }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddCommunication()
        {
            return View();
        }

        public JsonResult Internship()
        {
            try
            {
                db = new CourseDataContext();

                var data = (from apply in db.tblInternApplies
                            join instr in db.SITSPL_tblInternshipStructures
                            on apply.InternshipStructureId equals instr.InternStructureId
                            join intr in db.SITSPL_tblInternships
                            on instr.InternshipId equals intr.InternshipId
                            select new
                            {
                                intr.InternshipId,
                                intr.InternshipName
                            }).Distinct().ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetInternTutorStudents(int Id)
        {
            try
            {
                db = new CourseDataContext();
               // var InternTutorId = Session["InternTutorId"].ToString();

                Int64 intInternTutorId= 0;
                Int64.TryParse(Session["InternTutorId"].ToString(), out intInternTutorId);

                var data = (from apply in db.tblInternApplies
                            join instr in db.SITSPL_tblInternshipStructures
                            on apply.InternshipStructureId equals instr.InternStructureId
                            
                            join intr in db.SITSPL_tblInternships
                            on instr.InternshipId equals intr.InternshipId
                            join inttut in db.tblInternTutors
                            on instr.InternshipId equals inttut.InternshipId
                            where inttut.InternshipId == Id && inttut.TutorId == Convert.ToInt32(intInternTutorId)
                            select new
                            {
                                apply.InterApllyId,
                                apply.Name,
                                instr.InternshipId,
                                apply.Email
                                
                            }).Distinct().ToList();

               

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public JsonResult AddMessageForInternStudentCommunication(int InternshipId, bool IsAllInternStudent, List<InternStudentsForTutor> lstInternStudent, string StudentMsg)
        {

            try
            {
           
                db = new CourseDataContext();
                tblCommunication objComm = null;
                var InternTutorsId = Session["InternTutorId"].ToString();

                ArrayList alAllEmail = new ArrayList();

                if (IsAllInternStudent == false)
                {
                    objComm = new tblCommunication();

                    // Course Student List
                    if (lstInternStudent != null)
                    {
                        ArrayList alEmail = new ArrayList();
                        for (int i = 0; i < lstInternStudent.Count; i++)
                        {
                            alEmail.Add(lstInternStudent[i].Email);
                            objComm = new tblCommunication();
                            objComm.IsAllInternStudent = IsAllInternStudent;
                            objComm.Id = lstInternStudent[i].StudentId;
                            objComm.UserType = "All-Intern-Students";
                            objComm.TutorId = Convert.ToInt32(InternTutorsId);
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Intern Tutor";
                            objComm.Message = StudentMsg;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                           
                            objComm.InternshipId = lstInternStudent[i].InternshipId;
                            objComm.TutorId = Convert.ToInt32(InternTutorsId);
                            objComm.CategoryType = "Intern Tutor";
                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }

                        if (alEmail.Count > 0)
                        {
                            SendMailToAll(alEmail);
                        }

                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                }

                else if (IsAllInternStudent == true)
                {
                    objComm = new tblCommunication();
                    ArrayList alEmail = new ArrayList();

                   
                    var InternTutorId = Session["InternTutorId"].ToString();

                     var InternAllStudents = (from apply in db.tblInternApplies
                            join instr in db.SITSPL_tblInternshipStructures
                            on apply.InternshipStructureId equals instr.InternStructureId
                            
                            join intr in db.SITSPL_tblInternships
                            on instr.InternshipId equals intr.InternshipId
                            join inttut in db.tblInternTutors
                            on instr.InternshipId equals inttut.InternshipId
                                              where inttut.TutorId == Convert.ToInt32(InternTutorId)
                                            //  where inttut.InternshipId == InternshipId &&
                            select new
                            {
                                apply.InterApllyId,
                                apply.Name,
                                instr.InternshipId,
                                apply.Email
                                
                            }).Distinct().ToList();



                    if (InternAllStudents != null)
                    {
                        for (int i = 0; i < InternAllStudents.Count; i++)
                        {
                            alEmail.Add(InternAllStudents[i].Email);
                            objComm = new tblCommunication();

                            objComm.IsAllInternStudent = IsAllInternStudent;

                            objComm.Id = InternAllStudents[i].InterApllyId;
                            objComm.UserType = "All-Intern-Students";
                            objComm.TutorId = Convert.ToInt32(InternTutorsId);
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Intern Tutor";
                            objComm.Message = StudentMsg;
                           
                           
                            objComm.CategoryType = "Intern Tutor";
                            objComm.TutorId = Convert.ToInt32(InternTutorsId);
                         
                            objComm.InternshipId = InternAllStudents[i].InternshipId;
                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }

                        if (alEmail.Count > 0)
                        {
                            SendMailToAll(alEmail);
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

        public void SendMailToAll(ArrayList arrEmail)
        {
            try
            {
                if (arrEmail.Count > 0)
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                    mail.Subject = "New Notification";
                    string Body = "Hello Dear, You have a new message, Please visit your portal and check it.";
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
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"], ConfigurationManager.AppSettings["smtpPass"]);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult ViewCommunication()
        {
            return View();
        }

        public JsonResult GetMessagesToInternStudent()
        {
            try
            {
                db = new CourseDataContext();
                var InternTutorId = Session["InternTutorId"].ToString();
                var list = (from comm in db.tblCommunications
                            join intr in db.SITSPL_tblInternships
                            on comm.InternshipId equals intr.InternshipId
                            join apply in db.tblInternApplies
                            on comm.Id equals apply.InterApllyId
                           join instr in db.SITSPL_tblInternshipStructures
                           on apply.InternshipStructureId equals instr.InternStructureId
                           

                            where comm.CreatedBy == "Intern Tutor"  && comm.TutorId == Convert.ToInt32(InternTutorId)
                            select new
                            {
                                comm,
                                intr,
                                apply
                            }).ToList();

                var data = list.Select(x => new
                {
                    x.comm.CommunicationId,
                    x.comm.CategoryType,
                    x.comm.InternshipId,
                    x.intr.InternshipName,
                    x.apply.Name,
                    x.apply.Email,
         DOB=       x.apply.DOB.ToString("dd/MM/yyyy"),
                    x.comm.Id,
                    x.comm.Message,
                    x.comm.IsAllInternStudent,
      DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),
                    x.comm.CreatedBy,
                    x.comm.UserType,
                   
                    
                }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();



                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult InternStudentCommunication()
        {

            return View();
        }

        public JsonResult GetInternStudentMessage(string strUserType)
        {
            try
            {


                //from tut in db.SITSPL_tblTutors
                //join us in db.SITSPL_tblUsers
                //on tut.TutorId equals us.Id
                //where us.UserType == "Intern-Tutor"
                //join comm in db.tblCommunications
                //on tut.TutorId equals comm.TutorId

                db = new CourseDataContext();
                var inttId = Session["InternTutorId"].ToString();

                string usertype = strUserType.Trim();

                if (usertype == "All-Intern-Tutor" && Convert.ToInt32(inttId) > 0)
                    {

                    var data2 = (from comm in db.tblCommunications
                                join apply in db.tblInternApplies
                                on comm.Id equals apply.InterApllyId
                                where comm.TutorId == Convert.ToInt32(inttId) //&& comm.CreatedBy = "Intern Student"
                                select new
                                {
                                    comm,
                                    apply
                                }).ToList();

                    var data = data2.Select(x => new
                    {
                        x.comm.CommunicationId,
                        x.comm.Message,
          DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),
                        x.apply.Name
                    }).OrderByDescending(x=>x.CommunicationId).Distinct().ToList();
                    
                    return Json(data, JsonRequestBehavior.AllowGet);

                    //  select comm.CreatedBy,apply.Name,UserType,Message,comm.DateCreated from tblCommunication as comm
                    // inner join tblInternApply as apply
                    // on apply.InterApllyId = comm.Id
                    //where comm.CreatedBy = 'Intern Student'and comm.TutorId = 18;


                    //var TutorId = (
                    //               from comm in db.tblCommunications
                    //               join tut in db.SITSPL_tblTutors
                    //               on comm.TutorId equals ?Convert.ToInt64(tut.TutorId)
                    //               join us in db.SITSPL_tblUsers
                    //               on tut.TutorId equals us.Id && us.UserType == "Intern-Tutor"
                    //               where us.Id == Convert.ToInt32(inttId)
                    //               select new
                    //               {
                    //                   comm.TutorId,
                    //               //    us.UserName,
                    //               //    us.UserType
                    //               }).FirstOrDefault();


                    //    x.C.Id,
                    //    x.C.UserType,
                    //    x.C.Message,
                    //    x.C.MessageAllStudents,
                    //    x.C.MessageAll,
                    //    DateCreated = x.C.DateCreated.ToString("dd/MM/yyyy"),
                    //    x.C.CommunicationId,
                    //    Tutors.TutorName
                    //}).OrderByDescending(x => x.CommunicationId).Distinct().ToList();




                    //                        select distinct comm.TutorId,us.UserName,us.UserType
                    //  from tblCommunication as comm
                    //  inner join SITSPL_tblTutor as tut
                    //  on comm.TutorId = tut.TutorId
                    //  inner join SITSPL_tblUser as us
                    //  on us.Id = tut.TutorId and us.UserType = 'Intern-Tutor';


                    //                        select comm.CreatedBy,apply.Name,UserType,Message,comm.DateCreated from tblCommunication as comm
                    //inner join tblInternApply as apply
                    //on apply.InterApllyId = comm.Id
                    //where comm.CreatedBy = 'Intern Student'and comm.TutorId = 18;

                   // return Json("", JsonRequestBehavior.AllowGet);
                        
                    }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
                    
                }
            catch(Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
          
             
            }
            
        
        // Add Intern Tutor Resource

        public ActionResult AddInternTutorResource()
        {
            return View();
        }

        [HttpPost]
        public ContentResult FileUploadByInternTutor(int InternshipId)
        {
            string path = Server.MapPath("~/InternTutor/Resource/");
            tblResource objResource = null;
            db = new CourseDataContext();

            Int64 intInternTutorId = 0;
            Int64.TryParse(Session["InternTutorId"].ToString(), out intInternTutorId);

            foreach (string key in Request.Files)
            {
                HttpPostedFileBase postedFileBase = Request.Files[key];
                postedFileBase.SaveAs(path + postedFileBase.FileName);

                objResource = new tblResource();
                objResource.DateCreated = DateTime.Now;
                objResource.CreatedBy = Session["InternTutorUser"].ToString();
                objResource.ResourceName = postedFileBase.FileName;
                objResource.ResourceType = postedFileBase.ContentType;
                objResource.TutorId = intInternTutorId;
                //  objResource.CourseId = CourseId;
                objResource.InternshipId = InternshipId;
                objResource.CreatedBy = Session["InternTutorUser"].ToString();
                objResource.UserType = "InternTutor";
                db.tblResources.InsertOnSubmit(objResource);
                db.SubmitChanges();
            }

            if (Request.Files.Count > 0)
            {
                if (objResource.ResourceId > 0)
                {
                    return Content("Successfully");
                }
                return Content("Failed");
            }
            return Content("Failed");
        }

        // Get Course For Intern Tutor

        public JsonResult InternshipForInternTutor()
        {
            try
            {
                db = new CourseDataContext();
                Int64 intInternTutorId = 0;
                Int64.TryParse(Session["InternTutorId"].ToString(), out intInternTutorId);

                var InternId = (from tut in db.SITSPL_tblTutors
                                join intrtut in db.tblInternTutors
                                on tut.TutorId equals intrtut.TutorId
                                join intr in db.SITSPL_tblInternships
                                on intrtut.InternshipId equals intr.InternshipId
                                select new
                                {
                                    intrtut.InternshipId,
                                    intr.InternshipName
                                }).ToList();

                var data = InternId.Select(x => new
                {
                    x.InternshipId,
                    x.InternshipName
                }).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewInternTutorResource()
        {
            return View();
        }


        public JsonResult ViewInternTutorResourceDetails()
        {
            try
            {
                db = new CourseDataContext();
                Int64 intInternTutorId = 0;
                Int64.TryParse(Session["InternTutorId"].ToString(), out intInternTutorId);
                var list = (from d in db.tblResources
                            join c in db.SITSPL_tblInternships
                            on d.InternshipId equals c.InternshipId
                            where d.UserType == "InternTutor" && d.TutorId == intInternTutorId
                            select
                            new
                            {
                                d,
                                c
                            }).ToList();

                var data = list.Select(x => new
                {
                    x.d.ResourceId,
                    x.d.ResourceName,
                    x.d.ResourceType,
                    x.d.IsPubllished,
                    DateCreated = x.d.DateCreated.ToString("dd/MM/yyyy"),
                    x.d.CreatedBy,
                    x.d.UserType,
                    x.c.InternshipName
                }).OrderByDescending(x => x.ResourceId).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        

        #region Download Resource File By Atul on 9 Nov 2020        
        public FileResult DownloadInternTutorResource(string strFileName)
        {
            string strFilePath = ConfigurationManager.AppSettings["downloadInternTutorResources"];
            if (!string.IsNullOrEmpty(strFilePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(strFilePath + strFileName);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, strFileName);
            }
            return null;
        }
        #endregion END Download Resource File By Dilshad on 15 Oct 2020


        // Download Resources For Intern Student

        public JsonResult DownloadResourcesForInternStudent()
        {
            try
            {
                db = new CourseDataContext();
                Int64 intInternStudentId = 0;
                Int64.TryParse(Session["InterApllyId"].ToString(), out intInternStudentId);

                var InternshipStructureId = (from apply in db.tblInternApplies
                                             where apply.InterApllyId == intInternStudentId
                                             select new
                                             {
                                                 apply.InternshipStructureId
                                             }).FirstOrDefault();

                var list = (from d in db.tblResources
                            join c in db.SITSPL_tblInternships
                            on d.InternshipId equals c.InternshipId
                            join intrstr in db.SITSPL_tblInternshipStructures
                            on c.InternshipId equals intrstr.InternshipId
                            where intrstr.InternStructureId == InternshipStructureId.InternshipStructureId


                            where d.UserType == "InternTutor"

                           // from d in db.tblResources
                           // join c in db.SITSPL_tblCourses
                           // on d.CourseId equals c.CourseId
                           // join cd in db.SITSPL_tblCourseDetails
                           // on d.CourseId equals cd.CourseId
                           // join std in db.SITSPL_tblStudentProfiles
                           //on cd.StudentId equals std.StudentId
                           // where d.UserType == "BrandTutor" && std.StudentId == intStudentId
                            select
                            new
                            {
                                d,
                                c
                            }).ToList();

                var data = list.Select(x => new
                {
                    x.d.ResourceId,
                    x.d.ResourceName,
                    x.d.ResourceType,
                    x.d.IsPubllished,
                    DateCreated = x.d.DateCreated.ToString("dd/MM/yyyy"),
                    x.d.CreatedBy,
                    x.d.UserType,
                    x.c.InternshipName
                }).OrderByDescending(x => x.ResourceId).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // Change Intern Tutor Password by old and new password:

        public ActionResult ChangePasswordForInternTutor()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ChangePasswordForInternTutors(string strOldPassword, string strNewPassword)
        {
            try
            {
                db = new CourseDataContext();
                Int64 intInternTutorId = 0;
                if (Session["InternTutorId"].ToString() != null)
                {
                    Int64.TryParse(Session["InternTutorId"].ToString(), out intInternTutorId);
                }

                var data = (from d in db.SITSPL_tblUsers
                            where d.Id == intInternTutorId &&
                            d.Password == strOldPassword
                            select d).FirstOrDefault();

                if (data != null)
                {
                    data.Password = strNewPassword;
                    db.SubmitChanges();
                    return Json(intInternTutorId.ToString(), JsonRequestBehavior.AllowGet);
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



    }
}