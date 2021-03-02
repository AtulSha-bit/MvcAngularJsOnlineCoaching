using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OnlineCoaching.Angular_Js_Data_Layer;
using OnlineCoaching.Linq_To_Sql;

namespace OnlineCoaching.Controllers
{
    public class TutorController : Controller
    {
        CourseDataContext db = null;
        // GET: Tutor
        public ActionResult Index()
        {
            if(Session["TutorId"] == null)
            {
                return RedirectToAction("BrandTutorLogin", "Tutor");
            }
            return View();
        }

        #region Files Upload for Resources By Tutor By Dilshad A. on 3 Oct 2020
        [HttpPost]
        public ContentResult FileUploadByBrandTutor(int CourseId)
        {
            string path = Server.MapPath("~/UploadResource/TutorResource/");
            tblResource objResource = null;
            db = new CourseDataContext();

            Int64 intTutorId = 0;
            Int64.TryParse(Session["TutorId"].ToString(), out intTutorId);
            
            foreach (string key in Request.Files)
            {
                HttpPostedFileBase postedFileBase = Request.Files[key];
                postedFileBase.SaveAs(path + postedFileBase.FileName);

                objResource = new tblResource();
                objResource.DateCreated = DateTime.Now;
                objResource.TutorId = intTutorId;
                objResource.CreatedBy = Session["User"].ToString();
                objResource.UserType = "BrandTutor";
                objResource.CourseId = CourseId;
                objResource.ResourceName = postedFileBase.FileName;
                objResource.ResourceType = postedFileBase.ContentType;
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
        #endregion END Files Upload for Resources By Tutor By Dilshad A. on 03 Oct 2020

        #region Add Resourse By Dilshad A. on 30 Sept 2020
        public ActionResult AddResourse()
        {
            return View();
        }
        #endregion END Add Resourse By Dilshad A. on 30 Sept 2020

        #region View Students by Dilshad A. on 30 Sept 2020
        public ActionResult Students()
        {
            return View();
        }
        #endregion End View Students by Dilshad A. on 30 Sept 2020

        #region BrandTutorLogin by Dilshad A. on 05 Oct 2020
        public ActionResult BrandTutorLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BrandTutorLogin(SITSPL_tblUser objUser)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.SITSPL_tblUsers
                            where d.UserName == objUser.UserName && d.Password == objUser.Password
                            join d1 in db.SITSPL_tblTutors on d.Id equals d1.TutorId
                            where d1.TutorType == "Brand-Tutor" || d.UserType == "Brand-Tutor"
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
                    return RedirectToAction("Index", "Tutor");
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
        #endregion BrandTutorLogin by Dilshad A. on 05 Oct 2020


        public ActionResult InternTutorLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult InternTutorLogin(SITSPL_tblUser objUser)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.SITSPL_tblUsers
                            where d.UserName == objUser.UserName && d.Password == objUser.Password
                            join d1 in db.SITSPL_tblTutors on d.Id equals d1.TutorId
                            where d.UserType == "Intern-Tutor"
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
                    Session["InternTutorId"] = data.TutorId;
                    Session["InternTutorUser"] = data.UserName;
                    Session["InternTutorEmail"] = data.TutorEmail;
                    Session["TutorImage"] = data.TutorImage;
                    //  return RedirectToAction("Index", "InternTutor");
                    return RedirectToAction("AddInternTutorResource", "InternTutor");
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



        public ActionResult ChangeBrandTutorPassword()
        {
            return View();
        }


        public ActionResult BrandTutorLogout()
        {
            Session["TutorId"] = null;
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("BrandTutorLogin", "Tutor");
        }


        public ActionResult BrandTutors()
        {

            return View();
        }

        public JsonResult BrandTutorCourse()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from cs in db.SITSPL_tblCourseStructures
                            join tut in db.SITSPL_tblTutors
                            on cs.TutorId equals tut.TutorId
                            join cour in db.SITSPL_tblCourses
                            on cs.CourseId equals cour.CourseId
                            join dur in db.tblDurations 
                            on cs.DurationId equals dur.DurationId
                            where tut.TutorType == "Brand-Tutor"
                            select new
                            {
                                tut.TutorId,
                                tut.TutorName,
                                cour.CourseName,
                                dur.DurationName,
                                cs.StructureId
                            }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult BrandTutorCourseList()
        {
            return View();
        }


        // Brand Tutor Course Details List 

        public JsonResult GetBrandTutorCourseDetailsBasedOnId(int id)
        {
            try
            {
                db = new CourseDataContext();
                var stru = (from d in db.SITSPL_tblCourses
                            join s in db.SITSPL_tblCourseStructures on d.CourseId equals s.CourseId
                            join tut in db.SITSPL_tblTutors on s.TutorId equals tut.TutorId

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
                    x.s.TutorId,
                    x.s.NetAmount,
                    x.d.CourseId,
                    x.d.CourseName,


                }).ToList();

                var Content = (from cont in db.tblContents
                               join c in db.SITSPL_tblCourses on cont.CourseId equals c.CourseId
                               join cs in db.SITSPL_tblCourseStructures on c.CourseId equals cs.CourseId
                               where cs.StructureId == id
                               select new
                               {
                                   cont.ContentId,
                                   cont.CourseId
                               }).FirstOrDefault();



                if (Content != null)
                {
                    var Heading = (from cont in db.tblContents
                                   where cont.ContentId == Convert.ToInt64(Content.ContentId) && cont.CourseId == Content.CourseId
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


                    var list = new { struc, Heading, description, prerequisities };

                    JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                    var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var list = new { struc };
                    JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                    var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }



            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult BrandTutorStudents()
        {
            return View();
        }



        public JsonResult StudentsOfBrandTutor()
        {
            try
            {
                db = new CourseDataContext();
                var Id = Session["TutorId"].ToString();
               
              

                var sub = (from std in db.SITSPL_tblStudentProfiles
                            join us in db.SITSPL_tblUsers
                            on std.TutorId equals us.Id
                            join stdcatg in db.tblStudentCategories
                            on std.StudentCategoryId equals stdcatg.StudentCategoryId

                            where us.Id == Convert.ToInt32(Id)
                            select new
                            {
                                std,
                                us,
                                stdcatg
                            }).ToList();

                var data = sub.Select(x => new
                {
                    x.std.StudentId,
                    x.std.Name,
                    x.std.NetAmount,
                    x.std.Email,
                    x.std.Mobile,
                    x.std.Username,
                    x.std.PaidAmount,
                    x.std.Due,
                    x.std.PaymentStatus,
                    x.std.ProfileImage,
                    x.std.PaymentMode,
                    x.std.Remarks,
                    x.std.RemarksPayment,
                    NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
                    DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                    x.std.CreatedBy,
                    DateCreated = x.std.DateCreated.HasValue ? x.std.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                    x.std.TemporaryRegNo,
                    x.std.FinalRegNo,
                    x.stdcatg.CategoryName

                }).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);

                //var sub = (from std in db.SITSPL_tblStudentProfiles
                //           join cd in db.SITSPL_tblCourseDetails
                //           on std.StudentId equals cd.StudentId into eGroup
                //           from stdpro in eGroup.DefaultIfEmpty()
                //           join cont in db.SITSPL_tblContacts
                //           on std.StudentId equals cont.UserId into eGroup2
                //           from stdpro2 in eGroup2.DefaultIfEmpty()
                //           join c in db.SITSPL_tblCourses
                //           on stdpro.CourseId equals c.CourseId into eGroup3
                //           from cour in eGroup3.DefaultIfEmpty()
                //           join stdcatg in db.tblStudentCategories
                //           on std.StudentCategoryId equals stdcatg.StudentCategoryId
                //           into eGroup4
                //           from stdcat in eGroup4.DefaultIfEmpty()

                //           join tut in db.SITSPL_tblTutors
                //           on std.TutorId equals (int?)tut.TutorId

                //           into eGroup5
                //           from brtut in eGroup5.DefaultIfEmpty()
                //           where std.TutorId == Convert.ToInt32(Id)

                //           select new
                //           {
                //               std,
                //               stdpro,
                //               stdpro2,
                //               cour,
                //               stdcat,
                //               brtut 

                //           }).ToList();

                //var stdProfile = sub.Select(x => x.std).Any();
                //var stdCourse = sub.Select(x => x.stdpro).ToList();
                //var std2Contact = sub.Select(x => x.stdpro2).ToList();
                //var country = sub.Select(x => x.cour).ToList();
                //var stdcategory = sub.Select(x => x.stdcat).ToList();

                //// Course,Contact and Country null

                //if (stdCourse[0] == null && std2Contact[0] == null && country[0] == null)
                //{
                //    var data = sub.Select(x => new
                //    {
                //        x.std.StudentId,
                //        x.std.Name,
                //        x.std.NetAmount,
                //        x.std.Email,
                //        x.std.Mobile,
                //        x.std.Username,
                //        x.std.PaidAmount,
                //        x.std.Due,
                //        x.std.PaymentStatus,
                //        x.std.ProfileImage,
                //        x.std.PaymentMode,
                //        x.std.Remarks,
                //        x.std.RemarksPayment,
                //        NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
                //        DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                //        x.std.CreatedBy,
                //        DateCreated = x.std.DateCreated.HasValue ? x.std.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                //        x.std.TemporaryRegNo,
                //        x.std.FinalRegNo,
                //    }).ToList();
                //    return Json(data, JsonRequestBehavior.AllowGet);
                //}

                //else if (stdCourse[0] == null && std2Contact[0] != null && country[0] == null)
                //{
                //    var data2 = sub.Select(x => new
                //    {
                //        x.std.StudentId,
                //        x.std.Name,
                //        x.std.NetAmount,
                //        x.std.Email,
                //        x.std.Mobile,
                //        x.std.Username,
                //        x.std.PaidAmount,
                //        x.std.Due,
                //        x.std.PaymentStatus,
                //        x.std.ProfileImage,
                //        x.std.PaymentMode,
                //        x.std.Remarks,
                //        x.std.RemarksPayment,
                //        NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
                //        DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                //        x.std.CreatedBy,
                //        DateCreated = x.std.DateCreated.HasValue ? x.std.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                //        x.std.TemporaryRegNo,
                //        x.std.FinalRegNo,
                //        x.stdpro2.State,
                //        x.stdpro2.Pincode,
                //        x.stdpro2.Landmark,
                //        x.stdpro2.Address,
                //        x.stdpro2.City,
                //        EmailId = x.stdpro2.Email,
                //        MobileNo = x.stdpro2.Mobile,
                //        x.stdpro2.UserId
                //    }).ToList();
                //    return Json(data2, JsonRequestBehavior.AllowGet);
                //}

                //else if (stdCourse[0] != null && std2Contact[0] == null && country[0] != null)
                //{
                //    var data3 = sub.Select(x => new
                //    {
                //        x.std.StudentId,
                //        x.std.Name,
                //        x.std.NetAmount,
                //        x.std.Email,
                //        x.std.Mobile,
                //        x.std.Username,
                //        x.std.PaidAmount,
                //        x.std.Due,
                //        x.std.PaymentStatus,
                //        x.std.ProfileImage,
                //        x.std.PaymentMode,
                //        x.std.Remarks,
                //        x.std.RemarksPayment,
                //        NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
                //        DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                //        x.std.CreatedBy,
                //        DateCreated = x.std.DateCreated.HasValue ? x.std.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                //        x.std.TemporaryRegNo,
                //        x.std.FinalRegNo,

                //        CoursesId = x.stdpro.CourseId,
                //        Course = x.cour.CourseName,
                //        x.stdpro.Duration,
                //        x.stdpro.JoiningDate,
                //        x.stdpro.Month,
                //        x.stdpro.NetAmountToPay,

                //    }).ToList();
                //    return Json(data3, JsonRequestBehavior.AllowGet);
                //}
                //else if (stdCourse[0] != null && std2Contact[0] != null && country[0] != null && stdcategory[0] == null)
                //{
                //    var data4 = sub.Select(x => new
                //    {
                //        x.std.StudentId,
                //        x.std.Name,
                //        x.std.Email,
                //        x.std.Mobile,
                //        x.std.Username,
                //        x.std.PaymentStatus,
                //        x.std.ProfileImage,
                //        DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                //        x.std.CreatedBy,
                //        DateCreated = x.std.DateCreated.HasValue ? x.std.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                //        x.std.TemporaryRegNo,
                //        x.std.PaidAmount,
                //        x.std.Due,
                //    }).Distinct().ToList();

                //    return Json(data4, JsonRequestBehavior.AllowGet);
                //}
                //else if (stdCourse[0] != null && std2Contact[0] != null && country[0] != null && stdcategory[0] != null)
                //{
                //    var data5 = sub.Select(x => new
                //    {
                //        x.std.StudentId,
                //        x.std.Name,
                //        x.std.Email,
                //        x.std.Mobile,
                //        x.std.Username,
                //        x.std.PaymentStatus,
                //        x.std.ProfileImage,
                //        DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                //        x.std.CreatedBy,
                //        DateCreated = x.std.DateCreated.HasValue ? x.std.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                //        x.std.TemporaryRegNo,
                //        x.stdcat.CategoryName,
                //        x.std.StudentCategoryId,
                //        x.std.PaidAmount,
                //        x.std.Due,
                //    }).Distinct().ToList();

                //    return Json(data5, JsonRequestBehavior.AllowGet);
                //}


//                return Json("", JsonRequestBehavior.AllowGet);
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

        public JsonResult CoursesForBrandTutor()
        {
            try
            {
                db = new CourseDataContext();
                var Id = Session["TutorId"].ToString();

                var data = (from cs in db.SITSPL_tblCourseStructures
                            join tut in db.SITSPL_tblTutors
                            on cs.TutorId equals tut.TutorId
                            join cour in db.SITSPL_tblCourses
                            on cs.CourseId equals cour.CourseId
                            join dur in db.tblDurations
                            on cs.DurationId equals dur.DurationId
                            where tut.TutorType == "Brand-Tutor" &&  tut.TutorId == Convert.ToInt32(Id)
                            select new
                            {
                                tut.TutorId,
                                tut.TutorName,
                                cour.CourseId,
                                cour.CourseName,
                                dur.DurationName,
                                cs.StructureId
                            }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetBrandTutorCourseStudents(int Id)
        {
            try
            {
                db = new CourseDataContext();
                var BrandTutorId = Session["TutorId"].ToString();

                var data = (from stdpro in db.SITSPL_tblStudentProfiles
                            join cd in db.SITSPL_tblCourseDetails
                            on stdpro.StudentId equals cd.StudentId
                            join cs in db.SITSPL_tblCourseStructures
                            on stdpro.CourseStructureId equals cs.StructureId
                            join cour in db.SITSPL_tblCourses
                            on cs.CourseId equals cour.CourseId
                            
                            where cs.CourseId == Id && stdpro.TutorId == Convert.ToInt32(BrandTutorId)
                            select new
                            {
                                stdpro.StudentId,
                                stdpro.Name,
                                cs.CourseId,
                                stdpro.Email,
                                stdpro.StudentCategoryId
                            }).Distinct().ToList();



  //                  select distinct stdpro.StudentId,cs.CourseId,stdpro.Email,stdpro.StudentCategoryId from
  //SITSPL_tblStudentProfile as stdpro
  //INNER JOIN SITSPL_tblCourseDetails as cd
  //on stdpro.StudentId = cd.StudentId
  //join SITSPL_tblCourseStructure as cs
  //on stdpro.StructureId = cs.StructureId
  //INNER JOIN SITSPL_tblCourse as cour
  //on cs.CourseId = cour.CourseId;

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

       [HttpPost] 
       public JsonResult AddMessageForBrandStudentsCommunicatioin(int? CourseId,int BrandTutorCourseId,bool IsAllBrandStudent,List<BrandStudents> lstBrandStudent,string StudentMsg, string StudentMsgToAll)
        {
        
            try
            {
                

                db = new CourseDataContext();
                tblCommunication objComm = null;
                Int32 intBrandTutorId = 0;
                if (Session["TutorId"].ToString() != null)
                {
                    Int32.TryParse(Session["TutorId"].ToString(), out intBrandTutorId);
                }

                Int32? BrandId = intBrandTutorId;

                ArrayList alAllEmail = new ArrayList();

                if (IsAllBrandStudent == false)
                {
                    objComm = new tblCommunication();

                    // Course Student List
                    if (lstBrandStudent != null)
                    {
                        ArrayList alEmail = new ArrayList();
                        for (int i = 0; i < lstBrandStudent.Count; i++)
                        {
                            alEmail.Add(lstBrandStudent[i].Email);
                            objComm = new tblCommunication();
                            objComm.IsAllCourseStudent = IsAllBrandStudent;
                            objComm.Id = lstBrandStudent[i].StudentId;
                            objComm.UserType = "All-Brand-Students";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Brand Tutor";
                            objComm.Message = StudentMsg;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.MessageAll = StudentMsgToAll;
                            objComm.CourseId = BrandTutorCourseId;
                            objComm.TutorId = BrandId;
                            objComm.CategoryType = "Brand Student";
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

                else if(IsAllBrandStudent == true)
                {
                    objComm = new tblCommunication();
                    ArrayList alEmail = new ArrayList();

                    //var BrandAllStudents = (from intnapply in db.tblInternApplies
                    //                         join intnstr in db.SITSPL_tblInternshipStructures
                    //                         on intnapply.InternshipStructureId equals intnstr.InternStructureId
                    //                         join intn in db.SITSPL_tblInternships
                    //                         on intnstr.InternshipId equals intn.InternshipId
                    //                         select new
                    //                         {
                    //                             intnapply.InterApllyId,
                    //                             intnapply.Name,
                    //                             intnapply.Email,
                    //                             intnapply.InternshipStructureId,
                    //                             intnstr.InternshipId,
                    //                             intn.InternshipName
                    //                         }).Distinct().ToList();

                    var BrandTutorId = Session["TutorId"].ToString();
              var BrandAllStudents =      (from stdpro in db.SITSPL_tblStudentProfiles
                     join cd in db.SITSPL_tblCourseDetails
                     on stdpro.StudentId equals cd.StudentId
                     join cs in db.SITSPL_tblCourseStructures
                     on stdpro.CourseStructureId equals cs.StructureId
                     join cour in db.SITSPL_tblCourses
                     on cs.CourseId equals cour.CourseId

                     where  stdpro.TutorId == Convert.ToInt32(BrandTutorId)
                     select new
                     {
                         stdpro.StudentId,
                         stdpro.Name,
                         cs.CourseId,
                         stdpro.Email,
                         stdpro.StudentCategoryId
                     }).Distinct().ToList();



                    if (BrandAllStudents != null)
                    {
                        for (int i = 0; i < BrandAllStudents.Count; i++)
                        {
                             alEmail.Add(BrandAllStudents[i].Email);
                             objComm = new tblCommunication();
                            
                             objComm.IsAllCourseStudent = IsAllBrandStudent;
                         
                            objComm.Id = BrandAllStudents[i].StudentId;
                            objComm.UserType = "All-Brand-Students";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.Message = StudentMsg;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.MessageAll = StudentMsgToAll;
                            objComm.CategoryType = "Brand Student";
                            objComm.TutorId = BrandId;
                            objComm.CourseId = BrandAllStudents[i].CourseId;
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

            catch(Exception ex)
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


        public ActionResult BrandTutorCommToStudents()
        {
            return View();
        }

           public JsonResult GetCommunicationData()
        {
            try
            {
                db = new CourseDataContext();

                Int64 intBrandTutorId = 0;
                if (Session["TutorId"].ToString() != null)
                {
                    Int64.TryParse(Session["TutorId"].ToString(), out intBrandTutorId);
                }
                

                //var BrandTutorId = Session["TutorId"].ToString();
                //var StdId = db.SITSPL_tblStudentProfiles.Where(x => x.TutorId == Convert.ToInt32(BrandTutorId)).Select(x => new { x.StudentId }).ToList();
                //var StudentId = StdId.Select(x => new { x.StudentId }).ToList();

                var sub = (from comm in db.tblCommunications
                           join cour in db.SITSPL_tblCourses
                           on comm.CourseId equals cour.CourseId
                           join std in db.SITSPL_tblStudentProfiles
                           on comm.Id equals std.StudentId
                           where comm.UserType == "All-Brand-Students"  && comm.CreatedBy == "Brand Tutor"
                           && comm.TutorId == intBrandTutorId
                           select new
                            {
                                comm,
                                cour,
                                std
                            }).ToList();

                var data = sub.Select(x => new
                {
                    x.comm.Id,
                    x.comm.CommunicationId,
                    x.std.Name,
                    x.comm.UserType,
                    x.comm.Message,
                    x.comm.MessageAllStudents,
                    x.comm.MessageAllTeachers,
                    x.comm.MessageAll,
                    x.comm.CategoryType,
                    x.comm.CourseId,
                    x.comm.IsAllCourseStudent,
                  DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy"),
                    x.cour.CourseName,
                    x.comm.CreatedBy,
                }).OrderByDescending(x=>x.CommunicationId).ToList();


                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



        #region View Resource By Dilshad A. on 15 Oct 2020
        public ActionResult ViewResource()
        {
            return View();
        }

        public JsonResult ViewResourceDetails()
        {
            try
            {
                db = new CourseDataContext();
                var list = (from d in db.tblResources
                            join c in db.SITSPL_tblCourses
                            on d.CourseId equals c.CourseId
                            where d.UserType == "BrandTutor"
                            select new
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
                    x.c.CourseName
                }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END View Resource By Dilshad A. on 15 Oct 2020

        #region Download Resource File By Dilshad on 15 Oct 2020        
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
        #endregion END Download Resource File By Dilshad on 15 Oct 2020



           public JsonResult GetStudentMsgToBrandTutor()
          {
            try
            {
                db = new CourseDataContext();

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult StudentsToBrandTutor()
        {
            return View();
        }


        [HttpPost]
        public JsonResult GetStudentMessageOfBrandTutor(string userType)
        {
            try
            {
                db = new CourseDataContext();
                var Id = Session["TutorId"].ToString();
                string strUserType = userType.Trim();

                if (strUserType == "All-Brand-Tutor" && Convert.ToInt32(Id) > 0)
                {
                    var data2 = (from C in db.tblCommunications
                                join S in db.SITSPL_tblStudentProfiles
                                 on C.TutorId.Value equals S.TutorId
                                where S.TutorId == Convert.ToInt32(Id) && C.UserType == strUserType && C.CreatedBy == "Brand Student"
                                select new
                                {
                                    C,S
                                }).ToList();


                    var data = data2.Select(x => new
                    {
                        x.C.Id,
                        x.C.UserType,
                        x.C.Message,
                        x.C.MessageAllStudents,
                        x.C.MessageAll,
                        x.S.Name,
                        x.C.CommunicationId,
                    DateCreated = x.C.DateCreated.ToString("dd/MM/yyyy")
                    }).OrderByDescending(x => x.CommunicationId).Distinct().ToList();

                    var list = new { data };
                    return Json(list, JsonRequestBehavior.AllowGet);

                    //var msg = (from d in db.tblCommunications
                    //           orderby d.CommunicationId descending
                    //           where d.Id == Convert.ToInt32(Id)
                    //           select new
                    //           {
                    //               d
                    //           }).ToList();

                    //if (msg != null)
                    //{
                    //    var msgAll = msg.Select(x => new
                    //    {
                    //        x.d.CommunicationId,
                    //        x.d.Message,
                    //        x.d.MessageAll,
                    //        x.d.MessageAllStudents,
                    //        DateCreated = x.d.DateCreated.ToString("dd/MM/yyyy")

                    //    }).OrderByDescending(x=>x.CommunicationId).ToList();

                    //    var list = new { data, msgAll };
                    //    return Json(list, JsonRequestBehavior.AllowGet);
                    //}
                    //else if (msg == null)
                    //{
                    //    var list = new { data };
                    //    return Json(list, JsonRequestBehavior.AllowGet);
                    //}
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }


        // Add Tutor Resource

        public ActionResult AddTutorResource()
        {
            return View();
        }

        public JsonResult CourseForTutor()
        {
            try
            {
                db = new CourseDataContext();
                Int64 intTutorId = 0;
                Int64.TryParse(Session["TutorId"].ToString(), out intTutorId);

                var CourseId = (from tut in db.SITSPL_tblTutors
                                join
coursetut in db.tblCourseTutors
on tut.TutorId equals coursetut.TutorId
                                join cour in db.SITSPL_tblCourses
                                on coursetut.CourseId equals cour.CourseId
                                select new
                                {
                                    coursetut.CourseId,
                                    cour.CourseName
                                }).ToList();

                var data = CourseId.Select(x => new
                {
                    x.CourseId,
                    x.CourseName
                }).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(ex.Message,JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ContentResult FileUploadByTutor(int CourseId)
        {
            string path = Server.MapPath("~/Upload/AddTutorResource/");
            tblResource objResource = null;
            db = new CourseDataContext();

            Int64 intTutorId = 0;
            Int64.TryParse(Session["TutorId"].ToString(), out intTutorId);

            foreach (string key in Request.Files)
            {
                HttpPostedFileBase postedFileBase = Request.Files[key];
                postedFileBase.SaveAs(path + postedFileBase.FileName);

                objResource = new tblResource();
                objResource.DateCreated = DateTime.Now;
                objResource.CreatedBy = Session["User"].ToString();
                objResource.ResourceName = postedFileBase.FileName;
                objResource.ResourceType = postedFileBase.ContentType;
                objResource.TutorId = intTutorId;
                objResource.CourseId = CourseId;
                objResource.UserType = "Tutor";
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

        // View Tutor Resource

        public ActionResult ViewTutorResource()
        {
            return View();
        }



        public JsonResult ViewTutorResourceDetails()
        {
            try
            {
                db = new CourseDataContext();
                Int64 intTutorId = 0;
                Int64.TryParse(Session["TutorId"].ToString(), out intTutorId);

           
                var list = (from d in db.tblResources
                            join c in db.SITSPL_tblCourses
                            on d.CourseId equals c.CourseId
                            where d.UserType == "Tutor" && d.TutorId == intTutorId
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
                    x.c.CourseName
                }).OrderByDescending(x=>x.ResourceId).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // Download Resources For Course Student

        public JsonResult DownloadStudentResourcesForCourseStudent()
        {
            try
            {
                db = new CourseDataContext();
                Int64 intStudentId = 0;
                Int64.TryParse(Session["StudentId"].ToString(), out intStudentId);


                var list = (from d in db.tblResources
                            join c in db.SITSPL_tblCourses
                            on d.CourseId equals c.CourseId
                            join cd in db.SITSPL_tblCourseDetails
                            on d.CourseId equals cd.CourseId
                            join std in db.SITSPL_tblStudentProfiles
                           on cd.StudentId equals std.StudentId
                            where d.UserType == "Tutor" && std.StudentId == intStudentId
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
                    x.c.CourseName
                }).OrderByDescending(x => x.ResourceId).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



        // Download Resources For Brand Student

        public JsonResult DownloadResourcesForBrandStudent()
        {
            try
            {
                db = new CourseDataContext();
                Int64 intStudentId = 0;
                Int64.TryParse(Session["StudentId"].ToString(), out intStudentId);


                var list = (from d in db.tblResources
                            join c in db.SITSPL_tblCourses
                            on d.CourseId equals c.CourseId
                            join cd in db.SITSPL_tblCourseDetails
                            on d.CourseId equals cd.CourseId
                            join std in db.SITSPL_tblStudentProfiles
                           on cd.StudentId equals std.StudentId
                            where d.UserType == "BrandTutor" && std.StudentId == intStudentId
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
                    x.c.CourseName
                }).OrderByDescending(x => x.ResourceId).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



        #region Download Resource File By Atul on 9 Nov 2020        
        public FileResult DownloadTutorResource(string strFileName)
        {
            string strFilePath = ConfigurationManager.AppSettings["downloadTutorResources"];
            if (!string.IsNullOrEmpty(strFilePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(strFilePath + strFileName);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, strFileName);
            }
            return null;
        }
        #endregion END Download Resource File By Dilshad on 15 Oct 2020


        // Get Courses For Brand Tutor Add Resource

        public JsonResult CoursesForBrandTutorResource()
        {
            try
            {
                db = new CourseDataContext();
                Int64 intBrandTutorId = 0;
                Int64.TryParse(Session["TutorId"].ToString(), out intBrandTutorId);

                //              select distinct std.TutorId,tut.TutorName from SITSPL_tblStudentProfile as std
                //inner join SITSPL_tblTutor as tut
                //on std.TutorId = tut.TutorId;

                //              select distinct coursetut.CourseId,cour.CourseName from tblCourseTutor as coursetut
                //inner join SITSPL_tblCourse as cour
                //on coursetut.CourseId = cour.CourseId
                //where coursetut.TutorId = 18;

                //var TutorId = (from std in db.SITSPL_tblStudentProfiles
                //               join tut in db.SITSPL_tblTutors
                //               on std.TutorId equals tut.TutorId
                //               where tut.TutorId == intBrandTutorId
                //               select new
                //               {
                //                   std.TutorId,
                //                   tut.TutorName
                //               }).Distinct().ToList();

                if(intBrandTutorId > 0) { 
                var data = (from coursetut in db.tblCourseTutors
                            
                            join tut in db.SITSPL_tblTutors
                            on coursetut.TutorId equals tut.TutorId
                            join cour in db.SITSPL_tblCourses
                            on coursetut.CourseId equals cour.CourseId
                            where coursetut.TutorId == intBrandTutorId
                            select new
                            {
                                coursetut.TutorId,
                                coursetut.CourseId,
                                cour.CourseName
                            }).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message,JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AdminCommToBrandTutor()
        {
            return View();
        }

        public JsonResult AdminToBrandTutorIncomingComm(string strUserType)
        {
            try
            {
                db = new CourseDataContext();

                Int64 intBrandTutorId = 0;
                Int64.TryParse(Session["TutorId"].ToString(), out intBrandTutorId);

                string usertype = strUserType.Trim();

                if (usertype == "All-Brand-Tutor" && intBrandTutorId > 0)
                {

                    var data2 = (from comm in db.tblCommunications

                                 join tut in db.SITSPL_tblTutors
                                 on comm.TutorId.Value equals tut.TutorId

                                 where comm.TutorId == intBrandTutorId && comm.UserType == usertype && comm.CategoryType == "Brand"
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
                        DateCreated = x.comm.DateCreated.ToString("dd/MM/yyyy")

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

        public JsonResult CoursesForBrandTutors()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from cs in db.SITSPL_tblCourseStructures
                            join tut in db.SITSPL_tblTutors
                            on cs.TutorId equals tut.TutorId
                            join cour in db.SITSPL_tblCourses
                            on cs.CourseId equals cour.CourseId
                            join dur in db.tblDurations
                            on cs.DurationId equals dur.DurationId
                            where tut.TutorType == "Brand-Tutor"
                            select new
                            {
                             //   tut.TutorId,
                             //   tut.TutorName,
                                cour.CourseId,
                                cour.CourseName,
                             //   dur.DurationName,
                             //   cs.StructureId
                            }).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetCourseBasedOnCategory(int CategoryId)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from stdpro in db.SITSPL_tblStudentProfiles
                                   join cd in db.SITSPL_tblCourseDetails
                                   on stdpro.StudentId equals cd.StudentId
                                   //join cs in db.SITSPL_tblCourseStructures
                                   //on stdpro.StructureId equals cs.StructureId
                                   join cour in db.SITSPL_tblCourses
                                   on cd.CourseId equals cour.CourseId
                                   join catg in db.tblStudentCategories
                                   on stdpro.StudentCategoryId equals catg.StudentCategoryId
                                   where stdpro.StudentType == "CourseStudent" && stdpro.StudentCategoryId == CategoryId
                                   select new
                                   {
                                       cour.CourseId,
                                       cour.CourseName
                                       //  stdpro.StudentCategoryId,
                                   }).Distinct().ToList();
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

        // Tutor To Course Student Communication
        public JsonResult getCourseStudentsBasedOnBatch(int CategoryId,int CourseId,int BatchId,int BatchGroupId,int SubCategoryId)
        {
            try
            {
                db = new CourseDataContext();
                int intTutorId = 0;
                int.TryParse(Session["TutorId"].ToString(), out intTutorId);

                var data = (from stdpro in db.SITSPL_tblStudentProfiles
                            join cd in db.SITSPL_tblCourseDetails
                            on stdpro.StudentId equals cd.StudentId
                            //join cs in db.SITSPL_tblCourseStructures
                            //on stdpro.StructureId equals cs.StructureId
                            join cour in db.SITSPL_tblCourses
                            on cd.CourseId equals cour.CourseId
                            join catg in db.tblStudentCategories
                            on stdpro.StudentCategoryId equals catg.StudentCategoryId
                            join subcatg in db.tblStudentSubCategories
                            on stdpro.StudentSubCategoryId equals subcatg.StudedetSubCategoryId
                            join coursetut in db.tblCourseTutors
                            on cd.CourseId equals coursetut.CourseId
                            join tut in db.SITSPL_tblTutors
                            on coursetut.TutorId equals tut.TutorId
                            where stdpro.StudentType == "CourseStudent" && stdpro.StudentCategoryId == CategoryId
                            && cd.CourseId == CourseId && stdpro.BatchId == BatchId && stdpro.BatchGroupId == BatchGroupId
                            && stdpro.StudentSubCategoryId == SubCategoryId && coursetut.TutorId == intTutorId
                            select new
                            {
                                stdpro.StudentId,
                                stdpro.Name,
                                stdpro.StudentCategoryId,
                                stdpro.StudentSubCategoryId,
                                stdpro.Email,
                                cd.CourseId
                                //cour.CourseId,
                                //cour.CourseName
                                //  stdpro.StudentCategoryId,
                            }).Distinct().ToList();
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

    }
}