using Newtonsoft.Json;
using OnlineCoaching.Angular_Js_Data_Layer;
using OnlineCoaching.Linq_To_Sql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace OnlineCoaching.Controllers
{
    public class AdminController : Controller
    {
        CourseDataContext db = null;
        // GET: Admin
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(SITSPL_tblUser user)
        {
            //string retuenvalue = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    db = new CourseDataContext();
                    var Username = user.UserName;
                    var Password = user.Password;
                    var data = db.SITSPL_tblUsers.Where(x => x.UserName == Username && x.Password == Password).FirstOrDefault();
                    if (data != null)
                    {
                        if (data.UserType.ToString() == "Admin" || data.UserType.ToString() == "SuperAdmin")
                        {
                            Session["UserId"] = data.UserId;
                            Session["User"] = data.UserName;
                            Session["UserType"] = data.UserType;
                            return RedirectToAction("Home", "Admin");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "login failed");
                        ViewBag.Message = "Wrong Username Or Password";
                        //return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
                    }
                }
                return View();
                //return PartialView("_PartialAdminLogin", user);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Logout()
        {
            Session["UserId"] = null;
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Login", "Admin");
        }

        // View Students to Admin
        public ActionResult ViewStudents()
        {
            return View();
        }




        #region Bind User
        public JsonResult GetUsers()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from us in db.SITSPL_tblUsers
                            join tut in db.SITSPL_tblTutors on us.Id equals tut.TutorId into res
                            from y in res.DefaultIfEmpty()
                            join std in db.SITSPL_tblStudentProfiles on us.Id equals std.StudentId into students
                            from s in students.DefaultIfEmpty()
                            join intern in db.tblInternApplies on us.Id equals intern.InterApllyId into intrnStudent
                            from i in intrnStudent.DefaultIfEmpty()
                            join brtut in db.SITSPL_tblTutors on us.Id equals brtut.TutorId
                            into brandTutor
                            from j in brandTutor.DefaultIfEmpty()
                            join interntut in db.tblInternTutors on us.Id equals interntut.TutorId
                            into interntutor
                            from k in interntutor.DefaultIfEmpty()
                            where us.UserType == "Tutor" || us.UserType == "Intern" || us.UserType == "Student" || us.UserType == "Brand-Tutor" || us.UserType == "Intern-Tutor"
                            select new
                            {
                                us,
                                tut = y == null ? "No Tutor" : y.TutorName,
                                s,
                                i,
                                j
                            }).Distinct().ToList();

                var list = data.Select(x => new
                {
                    x.us.UserId,
                    x.us.Id,
                    x.us.UserName,
                    x.us.UserType,
                    x.us.CompletedId,
                    x.us.IsPublished,
                    DateCreated = x.us.DateCreated.ToString("dd/MM/yyyy"),
                    x.us.CreatedBy,
                    LastUpdated = x.us.LastUpdated.HasValue ? x.us.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                    x.us.UpdatedBy
                }).ToList();

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion End Bind User

        #region show students to admin

        //public JsonResult GetStudentsWithCourse()
        //{
        //    try
        //    {
        //        db = new CourseDataContext();
        //        var sub = (from std in db.SITSPL_tblStudentProfiles
        //                   join cd in db.SITSPL_tblCourseDetails
        //                   on std.StudentId equals cd.StudentId into eGroup
        //                   from stdpro in eGroup.DefaultIfEmpty()
        //                   join cont in db.SITSPL_tblContacts
        //                   on std.StudentId equals cont.UserId into eGroup2
        //                   from stdpro2 in eGroup2.DefaultIfEmpty()
        //                   join c in db.SITSPL_tblCourses
        //                   on stdpro.CourseId equals c.CourseId into eGroup3
        //                   from cour in eGroup3.DefaultIfEmpty()
        //                   join stdcatg in db.tblStudentCategories
        //                   on std.StudentCategoryId equals stdcatg.StudentCategoryId
        //                   into eGroup4
        //                   from stdcat in eGroup4.DefaultIfEmpty()
        //                   select new
        //                   {
        //                       std,
        //                       stdpro,
        //                       stdpro2,
        //                       cour,
        //                       stdcat

        //                   }).ToList();

        //        var stdProfile = sub.Select(x => x.std).Any();
        //        var stdCourse = sub.Select(x => x.stdpro).ToList();
        //        var std2Contact = sub.Select(x => x.stdpro2).ToList();
        //        var country = sub.Select(x => x.cour).ToList();
        //        var stdcategory = sub.Select(x => x.stdcat).ToList();

        //        if (stdCourse[0] == null && std2Contact[0] == null && country[0] == null)
        //        {
        //            var data = sub.Select(x => new
        //            {
        //                x.std.StudentId,
        //                x.std.Name,
        //                x.std.NetAmount,
        //                x.std.Email,
        //                x.std.Mobile,
        //                x.std.Username,
        //                x.std.PaidAmount,
        //                x.std.Due,
        //                x.std.PaymentStatus,
        //                x.std.ProfileImage,
        //                x.std.PaymentMode,
        //                x.std.Remarks,
        //                x.std.RemarksPayment,
        //                NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
        //                DOB = x.std.DOB.ToString("dd/MM/yyyy"),
        //                x.std.CreatedBy,
        //                DateCreated = x.std.DateCreated.HasValue ? x.std.DateCreated.Value.ToString("dd/MM/yyyy") : null,
        //                x.std.TemporaryRegNo,
        //                x.std.FinalRegNo,
        //            }).ToList();
        //            return Json(data, JsonRequestBehavior.AllowGet);
        //        }
        //        else if (stdCourse[0] == null && std2Contact[0] != null && country[0] == null)
        //        {
        //            var data2 = sub.Select(x => new
        //            {
        //                x.std.StudentId,
        //                x.std.Name,
        //                x.std.NetAmount,
        //                x.std.Email,
        //                x.std.Mobile,
        //                x.std.Username,
        //                x.std.PaidAmount,
        //                x.std.Due,
        //                x.std.PaymentStatus,
        //                x.std.ProfileImage,
        //                x.std.PaymentMode,
        //                x.std.Remarks,
        //                x.std.RemarksPayment,
        //                NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
        //                DOB = x.std.DOB.ToString("dd/MM/yyyy"),
        //                x.std.CreatedBy,
        //                DateCreated = x.std.DateCreated.HasValue ? x.std.DateCreated.Value.ToString("dd/MM/yyyy") : null,
        //                x.std.TemporaryRegNo,
        //                x.std.FinalRegNo,
        //                x.stdpro2.State,
        //                x.stdpro2.Pincode,
        //                x.stdpro2.Landmark,
        //                x.stdpro2.Address,
        //                x.stdpro2.City,
        //                EmailId = x.stdpro2.Email,
        //                MobileNo = x.stdpro2.Mobile,
        //                x.stdpro2.UserId
        //            }).ToList();
        //            return Json(data2, JsonRequestBehavior.AllowGet);
        //        }
        //        else if (stdCourse[0] != null && std2Contact[0] == null && country[0] != null)
        //        {
        //            var data2 = sub.Select(x => new
        //            {
        //                x.std.StudentId,
        //                x.std.Name,
        //                x.std.NetAmount,
        //                x.std.Email,
        //                x.std.Mobile,
        //                x.std.Username,
        //                x.std.PaidAmount,
        //                x.std.Due,
        //                x.std.PaymentStatus,
        //                x.std.ProfileImage,
        //                x.std.PaymentMode,
        //                x.std.Remarks,
        //                x.std.RemarksPayment,
        //                NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
        //                DOB = x.std.DOB.ToString("dd/MM/yyyy"),
        //                x.std.CreatedBy,
        //                DateCreated = x.std.DateCreated.HasValue ? x.std.DateCreated.Value.ToString("dd/MM/yyyy") : null,
        //                x.std.TemporaryRegNo,
        //                x.std.FinalRegNo,
        //                CoursesId = x.stdpro.CourseId,
        //                Course = x.cour.CourseName,
        //                x.stdpro.Duration,
        //                x.stdpro.JoiningDate,
        //                x.stdpro.Month,
        //                x.stdpro.NetAmountToPay,
        //            }).ToList();
        //            return Json(data2, JsonRequestBehavior.AllowGet);
        //        }
        //        else if (stdCourse[0] != null && std2Contact[0] != null && country[0] != null && stdcategory[0] == null)
        //        {
        //            var data3 = sub.Select(x => new
        //            {
        //                x.std.StudentId,
        //                x.std.Name,
        //                x.std.Email,
        //                x.std.Mobile,
        //                x.std.Username,
        //                x.std.PaymentStatus,
        //                x.std.ProfileImage,
        //                DOB = x.std.DOB.ToString("dd/MM/yyyy"),
        //                x.std.CreatedBy,
        //                DateCreated = x.std.DateCreated.HasValue ? x.std.DateCreated.Value.ToString("dd/MM/yyyy") : null,
        //                x.std.TemporaryRegNo,
        //                x.std.PaidAmount,
        //                x.std.Due,
        //            }).Distinct().ToList();

        //            return Json(data3, JsonRequestBehavior.AllowGet);
        //        }
        //        else if (stdCourse[0] != null && std2Contact[0] != null && country[0] != null && stdcategory[0] != null)
        //        {
        //            var data4 = sub.Select(x => new
        //            {
        //                x.std.StudentId,
        //                x.std.Name,
        //                x.std.Email,
        //                x.std.Mobile,
        //                x.std.Username,
        //                x.std.PaymentStatus,
        //                x.std.ProfileImage,
        //                DOB = x.std.DOB.ToString("dd/MM/yyyy"),
        //                x.std.CreatedBy,
        //                DateCreated = x.std.DateCreated.HasValue ? x.std.DateCreated.Value.ToString("dd/MM/yyyy") : null,
        //                x.std.TemporaryRegNo,
        //                x.stdcat.CategoryName,
        //                x.std.StudentCategoryId,
        //                x.std.PaidAmount,
        //                x.std.Due,
        //            }).Distinct().ToList();

        //            return Json(data4, JsonRequestBehavior.AllowGet);
        //        }
        //        return Json("", JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message, JsonRequestBehavior.AllowGet);
        //    }

        //}



        public JsonResult GetStudentsWithCourse()
        {
            try
            {
                db = new CourseDataContext();
                var sub = (from std in db.SITSPL_tblStudentProfiles
                           join cd in db.SITSPL_tblCourseDetails
                           on std.StudentId equals cd.StudentId into eGroup
                           from stdpro in eGroup.DefaultIfEmpty()
                           join cont in db.SITSPL_tblContacts
                           on std.StudentId equals cont.UserId into eGroup2
                           from stdpro2 in eGroup2.DefaultIfEmpty()
                           join c in db.SITSPL_tblCourses
                           on stdpro.CourseId equals c.CourseId into eGroup3
                           from cour in eGroup3.DefaultIfEmpty()
                           join stdcatg in db.tblStudentCategories
                           on std.StudentCategoryId equals stdcatg.StudentCategoryId
                           into eGroup4
                           from stdcat in eGroup4.DefaultIfEmpty()
                           join bat in db.SITSPL_tblBatches
                           on std.BatchId equals bat.BatchId into eGroup5
                           from batc in eGroup5.DefaultIfEmpty()

                           //join batgrp in db.SITSPL_tblBatchGroups
                           //on std.BatchGroupId equals batgrp.BatchGroupId
                           //into eGroup6
                           //from batcgrp in eGroup6.DefaultIfEmpty()
                           select new
                           {
                               std,
                               stdpro,
                               stdpro2,
                               cour,
                               stdcat,
                               batc
                               


                           }).ToList();

                var stdProfile = sub.Select(x => x.std).Any();
                var stdCourse = sub.Select(x => x.stdpro).ToList();
                var std2Contact = sub.Select(x => x.stdpro2).ToList();
                var country = sub.Select(x => x.cour).ToList();
                var stdcategory = sub.Select(x => x.stdcat).ToList();
                var batch = sub.Select(x => x.batc).ToList();
                

                if (stdCourse[0] == null && std2Contact[0] == null && country[0] == null)
                {
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
                        x.std.StudentType,
                        x.batc.BatchSize,
                        x.stdcat.CategoryName
                    }).ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else if (stdCourse[0] == null && std2Contact[0] != null && country[0] == null)
                {
                    var data2 = sub.Select(x => new
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
                        x.stdpro2.State,
                        x.stdpro2.Pincode,
                        x.stdpro2.Landmark,
                        x.stdpro2.Address,
                        x.stdpro2.City,
                        EmailId = x.stdpro2.Email,
                        MobileNo = x.stdpro2.Mobile,
                        x.stdpro2.UserId,
                        x.std.StudentType,
                        x.stdcat.CategoryName
                    }).ToList();
                    return Json(data2, JsonRequestBehavior.AllowGet);
                }
                else if (stdCourse[0] != null && std2Contact[0] == null && country[0] != null && batch[0] != null)
                {
                    var data2 = sub.Select(x => new
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
                        CoursesId = x.stdpro.CourseId,
                        Course = x.cour.CourseName,
                        x.stdpro.Duration,
                        x.stdpro.JoiningDate,
                        x.stdpro.Month,
                        x.stdpro.NetAmountToPay,
                        x.std.StudentType,
                        x.stdcat.CategoryName,
                        x.batc.BatchSize
                    }).ToList();
                    return Json(data2, JsonRequestBehavior.AllowGet);
                }
                else if (stdCourse[0] != null && std2Contact[0] != null && country[0] != null && stdcategory[0] == null && batch[0] != null )
                {
                    var data3 = sub.Select(x => new
                    {
                        x.std.StudentId,
                        x.std.Name,
                        x.std.Email,
                        x.std.Mobile,
                        x.std.Username,
                        x.std.PaymentStatus,
                        x.std.ProfileImage,
                        DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                        x.std.CreatedBy,
                        DateCreated = x.std.DateCreated.HasValue ? x.std.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                        x.std.TemporaryRegNo,
                        x.std.PaidAmount,
                        x.std.Due,
                        x.std.StudentType,
                        x.stdcat.CategoryName,
                        x.batc.BatchSize,
                       // x.batcgrp.BatchName
                    }).Distinct().ToList();

                    return Json(data3, JsonRequestBehavior.AllowGet);
                }
                else if (stdCourse[0] != null && std2Contact[0] != null && country[0] != null && stdcategory[0] != null && batch[0] != null)
                {
                    var data4 = sub.Select(x => new
                    {
                        x.std.StudentId,
                        x.std.Name,
                        x.std.Email,
                        x.std.Mobile,
                        x.std.Username,
                        x.std.PaymentStatus,
                        x.std.ProfileImage,
                        DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                        x.std.CreatedBy,
                        DateCreated = x.std.DateCreated.HasValue ? x.std.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                        x.std.TemporaryRegNo,
                        x.stdcat.CategoryName,
                        x.std.StudentCategoryId,
                        x.std.PaidAmount,
                        x.std.Due,
                        x.std.StudentType,
                        x.batc.BatchSize,
                      //  x.batcgrp.BatchName
                    }).Distinct().ToList();

                    return Json(data4, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


        #endregion end show students

        #region  Student Data By Admin (Getting Student Data)

        // Get Student Data On Edit For Admin

        public JsonResult GetStudentEditedData(int StudentId)
        {
            db = new CourseDataContext();

            var sub = (from std in db.SITSPL_tblStudentProfiles
                       join cd in db.SITSPL_tblCourseDetails
                       on std.StudentId equals cd.StudentId
                       into eGroup
                       from std2 in eGroup.DefaultIfEmpty()
                       join cont in db.SITSPL_tblContacts
                       on std2.StudentId equals cont.UserId
                       into eGroup2
                       from std3 in eGroup2.DefaultIfEmpty()
                       join c in db.SITSPL_tblCourses
                       on std2.StudentId equals c.CourseId
                       into eGroup3
                       from std4 in eGroup3.DefaultIfEmpty()
                       join stdcatg in db.tblStudentCategories
                       on std.StudentCategoryId equals stdcatg.StudentCategoryId
                       into eGroup4
                       from stdcat in eGroup4.DefaultIfEmpty()
                       join std5 in db.tblDurations
                       on std2.Duration equals std5.DurationId
                       into eGroup5
                       from std5 in eGroup5.DefaultIfEmpty()

                       join batgrp in db.SITSPL_tblBatchGroups
                           on std.BatchGroupId equals batgrp.BatchGroupId into eGroup6
                       from batchgrp in eGroup6.DefaultIfEmpty()
                       where std.StudentId == StudentId
                       select new
                       {
                           std,
                           std2,
                           std3,
                           std4,
                           stdcat,
                           std5,
                           batchgrp
                       }).ToList();

            var stdCourseDetail = sub.Select(x => x.std2).ToList();
            var stdContact = sub.Select(x => x.std3).ToList();
            var stdCourse = sub.Select(x => x.std4).ToList();
            var stdca = sub.Select(x => x.stdcat).ToList();
            var cddur = sub.Select(x => x.std5).ToList();
            var batchgroup = sub.Select(x => x.batchgrp).ToList();

            if (stdCourseDetail[0] == null && stdContact[0] == null && stdCourse[0] == null && stdca[0] == null && cddur[0] == null && batchgroup[0] == null)
            {
                var data = sub.Select(x => new
                {
                    x.std.StudentId,
                    x.std.Name,
                    x.std.PaymentStatus,
                    DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                    x.std.Username,
                    x.std.Mobile,
                    x.std.Email,
                    x.std.ProfileImage,
                    x.std.Fblink,
                    x.std.Instalink,
                    x.std.Twitterlink,
                    x.std.PaidAmount,
                    x.std.Due,
                    x.std.TemporaryRegNo,
                    x.std.FinalRegNo,
                    NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
                    x.std.Remarks,
                    x.std.RemarksPayment,
                    x.std.PaymentMode,
                    x.std.BatchId
                }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);

            }


            else if (stdCourseDetail[0] != null && stdContact[0] == null && stdCourse[0] != null && stdca[0] != null && cddur[0] != null && batchgroup[0] == null)
            {
                var data2 = sub.Select(x => new
                {
                    x.std.StudentId,
                    x.std.Name,
                    x.std.PaymentStatus,
                    DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                    x.std.Username,
                    x.std.Mobile,
                    x.std.Email,
                    x.std.ProfileImage,
                    x.std.Fblink,
                    x.std.Instalink,
                    x.std.Twitterlink,
                    x.std.PaidAmount,
                    x.std.Due,
                    x.std.TemporaryRegNo,
                    x.std.FinalRegNo,
                    NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
                    x.std.Remarks,
                    x.std.RemarksPayment,
                    x.std.PaymentMode,
                    CoursesId = x.std2.CourseId,
                    Course = x.std4.CourseName,
                    // x.std2.Duration,
                    x.std2.JoiningDate,
                    x.std2.Month,
                    x.std2.NetAmountToPay,
                    x.stdcat.CategoryName,
                    x.std.StudentCategoryId,
                    x.std2.Duration,
                    x.std5.DurationName,
                    x.std.BatchId

                }).ToList();

                return Json(data2, JsonRequestBehavior.AllowGet);
            }

            else if (stdCourseDetail[0] != null && stdContact[0] == null && stdCourse[0] != null && stdca[0] != null && cddur[0] != null && batchgroup[0] != null)
            {
                var data11 = sub.Select(x => new
                {
                    x.std.StudentId,
                    x.std.Name,
                    x.std.PaymentStatus,
                    DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                    x.std.Username,
                    x.std.Mobile,
                    x.std.Email,
                    x.std.ProfileImage,
                    x.std.Fblink,
                    x.std.Instalink,
                    x.std.Twitterlink,
                    x.std.PaidAmount,
                    x.std.Due,
                    x.std.TemporaryRegNo,
                    x.std.FinalRegNo,
                    NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
                    x.std.Remarks,
                    x.std.RemarksPayment,
                    x.std.PaymentMode,
                    CoursesId = x.std2.CourseId,
                    Course = x.std4.CourseName,
                    // x.std2.Duration,
                    x.std2.JoiningDate,
                    x.std2.Month,
                    x.std2.NetAmountToPay,
                    x.stdcat.CategoryName,
                    x.std.StudentCategoryId,
                    x.std2.Duration,
                    x.std5.DurationName,
                    x.std.BatchId,
                    x.batchgrp.BatchName

                }).ToList();

                return Json(data11, JsonRequestBehavior.AllowGet);
            }

            else if (stdCourseDetail[0] == null && stdContact[0] != null && stdCourse[0] == null && stdca[0] != null && cddur[0] == null)
            {

                var data3 = sub.Select(x => new
                {
                    x.std.StudentId,
                    x.std.Name,
                    x.std.PaymentStatus,
                    DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                    x.std.Username,
                    x.std.Mobile,
                    x.std.Email,
                    x.std.ProfileImage,
                    x.std.Fblink,
                    x.std.Instalink,
                    x.std.Twitterlink,
                    x.std.PaidAmount,
                    x.std.Due,
                    x.std.TemporaryRegNo,
                    x.std.FinalRegNo,
                    NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
                    x.std.Remarks,
                    x.std.RemarksPayment,
                    x.std.PaymentMode,
                    CoursesId = x.std2.CourseId,
                    // Course = x.std4.CourseName,
                    // new   //        x.std2.Duration,
                    x.std2.JoiningDate,
                    x.std2.Month,
                    x.std2.NetAmountToPay,
                    x.std3.State,
                    x.std3.City,
                    x.std3.Pincode,
                    x.std3.Landmark,
                    x.std3.Address,
                    x.stdcat.CategoryName,
                    x.std.StudentCategoryId,
                    x.std.BatchId

                }).ToList();
                return Json(data3, JsonRequestBehavior.AllowGet);
            }


            else if (stdCourseDetail[0] != null && stdContact[0] == null && stdCourse[0] == null && stdca[0] != null && cddur[0] != null)
            {

                var data4 = sub.Select(x => new
                {
                    x.std.StudentId,
                    x.std.Name,
                    x.std.PaymentStatus,
                    DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                    x.std.Username,
                    x.std.Mobile,
                    x.std.Email,
                    x.std.ProfileImage,
                    x.std.Fblink,
                    x.std.Instalink,
                    x.std.Twitterlink,
                    x.std.PaidAmount,
                    x.std.Due,
                    x.std.TemporaryRegNo,
                    x.std.FinalRegNo,
                    NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
                    x.std.Remarks,
                    x.std.RemarksPayment,
                    x.std.PaymentMode,
                    CoursesId = x.std2.CourseId,
                    // Course = x.std4.CourseName,
                    // new      x.std2.Duration,
                    x.std2.JoiningDate,
                    x.std2.Month,
                    x.std2.NetAmountToPay,
                    x.stdcat.CategoryName,
                    x.std.StudentCategoryId,
                    x.std2.Duration,
                    x.std5.DurationName,
                    x.std.BatchId
                    //   x.std3.State,
                    //   x.std3.City,
                    //   x.std3.Pincode,
                    //   x.std3.Landmark,
                    //   x.std3.Address
                }).ToList();
                return Json(data4, JsonRequestBehavior.AllowGet);
            }
            else if (stdCourseDetail[0] == null && stdContact[0] == null && stdCourse[0] == null && stdca[0] != null && cddur[0] == null)
            {
                var data5 = sub.Select(x => new
                {
                    x.std.StudentId,
                    x.std.Name,
                    x.std.PaymentStatus,
                    DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                    x.std.Username,
                    x.std.Mobile,
                    x.std.Email,
                    x.std.ProfileImage,
                    x.std.Fblink,
                    x.std.Instalink,
                    x.std.Twitterlink,
                    NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
                    x.std.TemporaryRegNo,
                    x.std.FinalRegNo,
                    x.std.Remarks,
                    x.std.RemarksPayment,
                    x.std.PaymentMode,
                    x.stdcat.CategoryName,
                    x.std.StudentCategoryId,
                    x.std.BatchId
                    //   x.std3.State,
                    //   x.std3.City,
                    //   x.std3.Pincode,
                    //   x.std3.Landmark,
                    //   x.std3.Address
                }).ToList();
                return Json(data5, JsonRequestBehavior.AllowGet);
            }


            else if (stdCourseDetail[0] != null && stdContact[0] == null && stdCourse[0] == null && stdca[0] != null && cddur[0] != null)
            {
                var data5 = sub.Select(x => new
                {
                    x.std.StudentId,
                    x.std.Name,
                    x.std.PaymentStatus,
                    DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                    x.std.Username,
                    x.std.Mobile,
                    x.std.Email,
                    x.std.ProfileImage,
                    x.std.Fblink,
                    x.std.Instalink,
                    x.std.Twitterlink,
                    x.std.TemporaryRegNo,
                    x.std.FinalRegNo,
                    NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
                    x.std.Remarks,
                    x.std.PaymentMode,
                    x.stdcat.CategoryName,
                    x.std.StudentCategoryId,
                    x.std.RemarksPayment,
                    x.std2.Duration,
                    x.std5.DurationName,
                    x.std.BatchId


                    //   x.std3.State,
                    //   x.std3.City,
                    //   x.std3.Pincode,
                    //   x.std3.Landmark,
                    //   x.std3.Address
                }).ToList();
                return Json(data5, JsonRequestBehavior.AllowGet);
            }
            else if (stdCourseDetail[0] != null && stdContact[0] != null && stdCourse[0] == null && stdca[0] != null && cddur[0] != null)
            {
                var data6 = sub.Select(x => new
                {
                    x.std.StudentId,
                    x.std.Name,
                    x.std.PaymentStatus,
                    DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                    x.std.Username,
                    x.std.Mobile,
                    x.std.Email,
                    x.std.ProfileImage,
                    x.std.Fblink,
                    x.std.Instalink,
                    x.std.Twitterlink,
                    x.std.PaidAmount,
                    x.std.Due,
                    x.std.TemporaryRegNo,
                    x.std.FinalRegNo,
                    NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
                    x.std.Remarks,
                    x.std.RemarksPayment,
                    x.std.PaymentMode,
                    CoursesId = x.std2.CourseId,
                    //          Course = x.std4.CourseName,
                    //  x.std2.Duration,
                    x.std2.JoiningDate,
                    x.std2.Month,
                    x.std2.NetAmountToPay,
                    x.std3.State,
                    x.std3.City,
                    x.std3.Pincode,
                    x.std3.Landmark,
                    x.std3.Address,
                    x.stdcat.CategoryName,
                    x.std.StudentCategoryId,
                    x.std2.Duration,
                    x.std5.DurationName,
                    x.std.BatchId,
                    x.std.BatchGroupId
                }).ToList();
                return Json(data6, JsonRequestBehavior.AllowGet);
            }

            else if (stdCourseDetail[0] == null && stdContact[0] != null && stdCourse[0] == null && stdca[0] != null && cddur[0] == null)
            {
                var data7 = sub.Select(x => new
                {
                    x.std.StudentId,
                    x.std.Name,
                    x.std.PaymentStatus,
                    DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                    x.std.Username,
                    x.std.Mobile,
                    x.std.Email,
                    x.std.ProfileImage,
                    x.std.Fblink,
                    x.std.Instalink,
                    x.std.Twitterlink,
                    x.std.PaidAmount,
                    x.std.Due,
                    x.std.TemporaryRegNo,
                    x.std.FinalRegNo,
                    NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
                    x.std.Remarks,
                    x.std.RemarksPayment,
                    x.std.PaymentMode,
                    CoursesId = x.std2.CourseId,
                    //          Course = x.std4.CourseName,              
                    x.std3.State,
                    x.std3.City,
                    x.std3.Pincode,
                    x.std3.Landmark,
                    x.std3.Address,
                    x.stdcat.CategoryName,
                    x.std.StudentCategoryId,
                    x.std.BatchId
                }).ToList();
                return Json(data7, JsonRequestBehavior.AllowGet);
            }

            else if (stdCourseDetail[0] != null && stdContact[0] == null && stdCourse[0] == null && stdca[0] != null && cddur[0] != null)
            {
                var data8 = sub.Select(x => new
                {
                    x.std.StudentId,
                    x.std.Name,
                    x.std.PaymentStatus,
                    DOB = x.std.DOB.ToString("dd/MM/yyyy"),
                    x.std.Username,
                    x.std.Mobile,
                    x.std.Email,
                    x.std.ProfileImage,
                    x.std.Fblink,
                    x.std.Instalink,
                    x.std.Twitterlink,
                    x.std.PaidAmount,
                    x.std.Due,
                    x.std.TemporaryRegNo,
                    x.std.FinalRegNo,
                    NextInstallmentDate = x.std.NextInstallmentDate.HasValue ? x.std.NextInstallmentDate.Value.ToString("dd/MM/yyyy") : null,
                    x.std.Remarks,
                    x.std.RemarksPayment,
                    x.std.PaymentMode,
                    CoursesId = x.std2.CourseId,
                    //          Course = x.std4.CourseName,              
                    //x.std3.State,
                    //x.std3.City,
                    //x.std3.Pincode,
                    //x.std3.Landmark,
                    //x.std3.Address,
                    x.stdcat.CategoryName,
                    x.std.StudentCategoryId,
                    x.std2.Duration,
                    x.std5.DurationName,
                    x.std.BatchId
                }).ToList();
                return Json(data8, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        // Edit multiple course details ( Course Taken by student shown to admin)

        // Students Data Edit Table Structure of course fees etc

        public JsonResult EditMultipleCourseDetails(int StudentId)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from stdpro in db.SITSPL_tblStudentProfiles
                            join cd in db.SITSPL_tblCourseDetails on stdpro.StudentId equals cd.StudentId
                            join c in db.SITSPL_tblCourses on cd.CourseId equals c.CourseId
                            join dur in db.tblDurations
                            on cd.Duration equals dur.DurationId
                            where cd.StudentId == StudentId
                            select new
                            {
                                stdpro,
                                c,
                                cd,
                                dur

                            }).ToList();


                var list = data.Select(x => new
                {
                    x.stdpro.StudentId,
                    Course = x.c.CourseId,
                    CourseName = x.c.CourseName,
                    Duration = x.cd.Duration,
                    x.dur.DurationName,
                    x.stdpro.Name,
                    x.stdpro.Email,
                    x.stdpro.PaidAmount,
                    x.stdpro.FinalRegNo,
                    x.stdpro.Due,
                    x.stdpro.PaymentMode,
                    x.stdpro.PaymentStatus,
                    x.stdpro.TemporaryRegNo,
                    x.stdpro.NextInstallmentDate,
                    x.stdpro.RemarksPayment,
                    x.cd.Fees,
                    x.cd.DiscountPercent,
                    FeeAfterDiscount = x.cd.FeesWithDiscount,
                    NetAmount = x.cd.NetAmountToPay,
                    ValidFrom = x.cd.CourseValidFrom.ToString("dd/MM/yyyy"),
                    ValidTo = x.cd.CourseValidTo.ToString("dd/MM/yyyy"),
                    x.cd.Month,
                    x.cd.JoiningDate
                }).ToList();

                if (list != null)
                {
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Application Error : " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion end student admin edit data

        #region Update Student By Admin
        public ActionResult UpdateStudent()
        {
            return View();
        }


        [HttpPost]
        public ContentResult UpdateStudentsProfile(HttpPostedFileBase postedfile, int StudentId, SITSPL_tblStudentProfile upd, DateTime DOB, string Address,
            string State, string City, string Pincode, string LandMark, string Remarks, string PaymentRemarks)
        {
            try
            {
                using (CourseDataContext db = new CourseDataContext())
                {

                    SITSPL_tblStudentProfile studentProfile = db.SITSPL_tblStudentProfiles.Where(x => x.StudentId == StudentId).SingleOrDefault();
                    DateTime today = DateTime.Today;
                    // DateTime dob = DateTime.Parse(Convert.ToDateTime(DOB).ToShortDateString());
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
                            studentProfile.Name = upd.Name;

                            studentProfile.DOB = DOB;
                            studentProfile.Username = upd.Username;
                            studentProfile.Email = upd.Email;
                            studentProfile.Fblink = upd.Fblink;
                            studentProfile.Instalink = upd.Instalink;
                            studentProfile.Twitterlink = upd.Twitterlink;
                            studentProfile.Mobile = upd.Mobile;
                            studentProfile.RemarksPayment = PaymentRemarks;
                            studentProfile.PaymentStatus = upd.PaymentStatus;
                            studentProfile.PaymentMode = upd.PaymentMode;
                            studentProfile.PaidAmount = upd.PaidAmount;
                            studentProfile.StudentCategoryId = upd.StudentCategoryId;
                            studentProfile.BatchId = upd.BatchId;
                            studentProfile.BatchGroupId = upd.BatchGroupId;

                            if (Remarks != null)
                            {
                                studentProfile.Remarks = Remarks;
                            }

                            studentProfile.Due = upd.Due;
                            studentProfile.NextInstallmentDate = upd.NextInstallmentDate;
                            studentProfile.PaidAmount = upd.PaidAmount;
                            //   studentProfile.TemporaryRegNo = upd.TemporaryRegNo;
                            studentProfile.FinalRegNo = upd.FinalRegNo;

                            if (postedfile != null)
                            {
                                var filename = Guid.NewGuid().ToString() + Path.GetExtension(postedfile.FileName);
                                studentProfile.ProfileImage = filename;
                                var path = Server.MapPath("~/ProjectImages/");
                                var ImgPath = path + filename;
                                postedfile.SaveAs(ImgPath);
                            }
                            db.SubmitChanges();

                            SITSPL_tblContact contact = db.SITSPL_tblContacts.Where(x => x.UserId == StudentId).FirstOrDefault();

                            if (contact != null)
                            {
                                contact.UserId = StudentId;
                                contact.Email = upd.Email;
                                contact.Mobile = upd.Mobile;
                                contact.Address = Address;
                                contact.State = State;
                                contact.City = City;
                                contact.Pincode = Pincode;
                                contact.Landmark = LandMark;
                                contact.CreatedBy = "Admin";
                                contact.DateCreated = DateTime.Now;
                                db.SubmitChanges();

                            }
                            else
                            {
                                SITSPL_tblContact contc = new SITSPL_tblContact();

                                contc.UserId = StudentId;
                                contc.Email = upd.Email;
                                contc.Mobile = upd.Mobile;
                                contc.Address = Address;
                                contc.State = State;
                                contc.City = City;
                                contc.Pincode = Pincode;
                                contc.Landmark = LandMark;
                                contc.LastUpdated = DateTime.Now;
                                contc.UpdatedBy = "Admin";
                                db.SITSPL_tblContacts.InsertOnSubmit(contc);
                                db.SubmitChanges();
                            }




                        }

                    }
                }

            }
            catch (Exception ex)
            {
                return Content("Error");
            }

            return Content("Updated");
        }

        #endregion


        public ActionResult Tutor()
        {

            return View();
        }


        [HttpPost]
        public ContentResult InsertTutors(SITSPL_tblTutor create, HttpPostedFileBase postedfile)
        {
            //return null;
            try
            {
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
                        create.CreatedBy = "Admin";
                        create.DateCreated = DateTime.Now;
                        create.IsDeleted = false;
                        db.SITSPL_tblTutors.InsertOnSubmit(create);
                        db.SubmitChanges();
                        //  SendMailToTutor(create.TutorName, create.TutorEmail);
                        //  SendTutorMailToAdmin(create.TutorName, create.TutorEmail);
                        var path = Server.MapPath("~/ProjectImages/");
                        var filename = postedfile.FileName;
                        var ImgPath = path + filename;
                        postedfile.SaveAs(ImgPath);
                    }

                }
                return Content("Insert");
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

            //  mail.From = new MailAddress("atul91915@gmail.com");

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


        public ActionResult ShowTutors()
        {
            return View();
        }

        public JsonResult GetTutors()
        {
            db = new CourseDataContext();
            var sub = (from C in db.SITSPL_tblTutors
                       join P in db.SITSPL_tblDocuments on C.TutorId equals P.UserId into eGroup
                       select new
                       {
                           C,
                           eGroup
                       }).ToList();

            var list = sub.Select(x => new
            {
                x.C.TutorId,
                x.C.TutorName,
                x.C.ShortDescription,
                x.C.LongDescription,
                x.C.TutorExperience,
                x.C.TutorImage,
                TutorDOB = x.C.TutorDOB.ToString("dd/MM/yyyy"),
                DateCreated = x.C.DateCreated.HasValue ? x.C.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                x.C.CreatedBy,
                x.C.TutorState,
                x.C.AdminDescription,
                x.C.TutorCity,
                x.C.TutorAddress,
                x.C.TutorPinCode,
                x.C.IsDeleted,
                x.C.TutorContact,
                x.C.TutorEmail,
                x.C.TutorType,
                DocumentId = from p in x.eGroup select p.DocumentId,
                DocumentName = from p in x.eGroup select p.DocumentName,
                DocumentNo = from p in x.eGroup select p.DoucmentNo,
                DocumentType = from p in x.eGroup select p.DocumentType,
            }).OrderByDescending(x=>x.TutorId).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        // Delete Course
        [HttpPost]
        public JsonResult DeleteTutors(int Id)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                SITSPL_tblTutor tutor = db.SITSPL_tblTutors.Where(x => x.TutorId == Id).FirstOrDefault();
                if (tutor != null)
                {
                    //db.SITSPL_tblTutors.DeleteOnSubmit(tutor);
                    tutor.IsDeleted = Convert.ToBoolean(1);
                    db.SubmitChanges();
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EditTutorDetail(int TutorId)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from tbltutor in db.SITSPL_tblTutors
                            join tblDoc in db.SITSPL_tblDocuments
on tbltutor.TutorId equals tblDoc.UserId
                            where tbltutor.TutorId == TutorId
                            select new
                            {
                                tbltutor,
                                tblDoc
                            }).ToList();

                var list = data.Select(x => new
                {
                    x.tbltutor.CreatedBy,
                    x.tbltutor.DateCreated,
                    x.tbltutor.TutorName,
                    x.tbltutor.TutorEmail,
                    x.tbltutor.TutorExperience,
                    x.tbltutor.TutorImage,
                    x.tbltutor.TutorDOB,
                    x.tbltutor.TutorId,
                    x.tbltutor.ShortDescription,
                    x.tbltutor.LongDescription,
                    x.tbltutor.TutorContact,
                    x.tblDoc.AadharNo,
                    x.tblDoc.DocumentId,
                    x.tblDoc.PanNo
                }).ToList();

                if (list != null)
                {
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ContentResult UpdateTutors(SITSPL_tblTutor objTutor, HttpPostedFileBase postedfile, List<MultipleDocuments> lstMultidocument)
        {
            try
            {
                using (CourseDataContext db = new CourseDataContext())
                {
                    SITSPL_tblTutor tutor = db.SITSPL_tblTutors.Where(x => x.TutorId == objTutor.TutorId).SingleOrDefault();
                    DateTime today = DateTime.Today;
                    TimeSpan tm = today - objTutor.TutorDOB;
                    int age = tm.Days / 365;
                    if (age < 18)
                    {
                        return Content("DateBirth");
                    }

                    else
                    {
                        if (tutor != null)
                        {
                            tutor.TutorId = objTutor.TutorId;
                            tutor.TutorName = objTutor.TutorName;
                            tutor.TutorEmail = objTutor.TutorEmail;
                            tutor.LongDescription = objTutor.LongDescription;
                            tutor.ShortDescription = objTutor.ShortDescription;
                            tutor.TutorExperience = objTutor.TutorExperience;
                            tutor.TutorContact = objTutor.TutorContact;
                            tutor.TutorDOB = objTutor.TutorDOB;
                            tutor.TutorType = objTutor.TutorType;
                            tutor.AdminDescription = objTutor.AdminDescription;
                            tutor.LastUpdated = DateTime.Now;
                            tutor.UpdatedBy = "Admin";
                            // tutor.TutorDOB = DateTime.Parse(Dob).ToShortDateString();

                            if (postedfile != null)
                            {
                                var filename = Guid.NewGuid().ToString() + Path.GetExtension(postedfile.FileName);
                                tutor.TutorImage = filename;
                                var path = Server.MapPath("~/ProjectImages/");
                                var ImgPath = path + filename;
                                postedfile.SaveAs(ImgPath);
                            }
                            db.SubmitChanges();
                            Int64 intTutorId = tutor.TutorId;

                            if (intTutorId > 0)
                            {
                                if (lstMultidocument != null)
                                {
                                    for (int i = 0; i < lstMultidocument.Count; i++)
                                    {

                                        SITSPL_tblDocument Document = db.SITSPL_tblDocuments.Where(x => x.UserId == objTutor.TutorId && x.DocumentId == lstMultidocument[i].DocumentId).FirstOrDefault();
                                        if (Document != null)
                                        {
                                            Document.DocumentId = lstMultidocument[i].DocumentId;
                                            Document.UserId = objTutor.TutorId;
                                            Document.DoucmentNo = lstMultidocument[i].DoucmentNo;
                                            Document.DocumentName = lstMultidocument[i].DocumentName;
                                            Document.LastUpdated = DateTime.Now;
                                            Document.UpdatedBy = "Admin";
                                            db.SubmitChanges();
                                        }
                                        else
                                        {
                                            SITSPL_tblDocument doc = new SITSPL_tblDocument();
                                            // Document.DocumentId = long.Parse(lstMultidocument[i].DocumentId);
                                            doc.UserId = objTutor.TutorId;
                                            doc.DoucmentNo = lstMultidocument[i].DoucmentNo;
                                            doc.DocumentName = lstMultidocument[i].DocumentName;
                                            doc.DateCreated = DateTime.Now;
                                            doc.CreatedBy = "Admin";
                                            db.SITSPL_tblDocuments.InsertOnSubmit(doc);
                                            db.SubmitChanges();
                                        }
                                    }
                                }

                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Error");
            }
            return Content("Updated");
        }


        public ActionResult Student()
        {
            return View();
        }


        String encrypted;

        [HttpPost]
        public ContentResult InsertStudents(SITSPL_tblStudentProfile create, HttpPostedFileBase postedfile)
        {
            try
            {
                using (CourseDataContext db = new CourseDataContext())
                {
                    DateTime today = DateTime.Today;
                    DateTime dob = DateTime.Parse(Convert.ToDateTime(create.DOB).ToShortDateString());
                    TimeSpan tm = today - dob;
                    int age = tm.Days / 365;

                    if (age < 18)
                    {
                        return Content("Dob");
                    }

                    else
                    {
                        string strmsg = string.Empty;
                        byte[] encode = new byte[create.Password.ToString().Length];
                        encode = Encoding.UTF8.GetBytes(create.Password);
                        strmsg = Convert.ToBase64String(encode);
                        encrypted = strmsg;
                        create.Password = encrypted;
                        db.SITSPL_tblStudentProfiles.InsertOnSubmit(create);
                        db.SubmitChanges();
                        //  SendMailToStudent(create.Name, create.Email);
                        //  SendMailToAdmin(create.Name, create.Email);
                        var path = Server.MapPath("~/ProjectImages/");
                        var filename = postedfile.FileName;
                        var ImgPath = path + filename;
                        postedfile.SaveAs(ImgPath);
                    }

                }
                return Content("Insert");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public void SendMailToStudent(string Name, string Email)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(Email);
            mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
            mail.Subject = "Registration Regarding";
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


        public void SendMailToAdmin(string Name, string Email)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(Email);
            //  mail.From = new MailAddress("atul91915@gmail.com");
            mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
            mail.Subject = "Registration Regarding";
            string Body = "Hello " + Name + " is sucessfully registered as Student in our Course";
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


        // Tutor Name
        public JsonResult GetTutorsData()
        {
            db = new CourseDataContext();
            var data = db.SITSPL_tblTutors.Select(x => new { x.TutorId, x.TutorName }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // Get Course Tutors

        public JsonResult GetCourseTutorsData()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from tut in db.SITSPL_tblTutors
                            join ctut in db.tblCourseTutors
                            on tut.TutorId equals ctut.TutorId
                            join cour in db.SITSPL_tblCourses
                            on ctut.CourseId equals cour.CourseId
                            select new
                            {
                                ctut.TutorId,
                                tut.TutorName
                            }).Distinct().ToList();
                if (data != null)
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

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // Get Course Tutors

        public JsonResult GetInternTutorsData()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from tut in db.SITSPL_tblTutors
                            join itut in db.tblInternTutors
                            on tut.TutorId equals itut.TutorId
                            join intr in db.SITSPL_tblInternships
                            on itut.InternshipId equals intr.InternshipId
                            select new
                            {
                                itut.TutorId,
                                tut.TutorName
                            }).Distinct().ToList();
                if (data != null)
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

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



        #region  View Student Details
        public JsonResult ViewStudent()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.SITSPL_tblStudentProfiles
                            select new
                            {
                                d.StudentId,
                                d.Name
                            }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region  View Student Details
        public JsonResult BindStudents()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.SITSPL_tblStudentProfiles
                            select new
                            {
                                d.StudentId,
                                d.Name
                            }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region 21 August Atul Sh. Changes
        public JsonResult GetStudentsCode()
        {
            db = new CourseDataContext();
            //  var data = db.SITSPL_tblUsers.Where(x => x.UserType == "Student").OrderByDescending(x => x.UserId).Select(x => x.CompletedId).FirstOrDefault();
            //  return Json(data, JsonRequestBehavior.AllowGet);
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
                var data = StudentFinalCode;
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTutorCode()
        {
            db = new CourseDataContext();
            var Prefix = "Tut-";
            var PrefixLen = Prefix.ToString().Length;
            var data2 = db.SITSPL_tblUsers.Where(x => x.UserType == "Tutor").ToList().Count();
            var StdCode = "";
            var LenStdCount = data2.ToString().Length;
            var StdCodeLength = db.SITSPL_tblUsers.Where(x => x.UserType == "Tutor").ToList().Count() + 1;
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
        #endregion


        public JsonResult GetBrandTutorCode()
        {
            db = new CourseDataContext();
            var Prefix = "BrTut-";
            var PrefixLen = Prefix.ToString().Length;
            var data2 = db.SITSPL_tblUsers.Where(x => x.UserType == "Brand-Tutor").ToList().Count();
            var StdCode = "";
            var LenStdCount = data2.ToString().Length;
            var StdCodeLength = db.SITSPL_tblUsers.Where(x => x.UserType == "Brand-Tutor").ToList().Count() + 1;
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

        public JsonResult GetInternCode()
        {
            db = new CourseDataContext();
            var Prefix = "Int-";
            var PrefixLen = Prefix.ToString().Length;
            var data2 = db.SITSPL_tblUsers.Where(x => x.UserType == "Intern").ToList().Count();
            var StdCode = "";
            var LenStdCount = data2.ToString().Length;
            var StdCodeLength = db.SITSPL_tblUsers.Where(x => x.UserType == "Intern").ToList().Count() + 1;
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



        public JsonResult GetInternTutorCode()
        {
            db = new CourseDataContext();
            var Prefix = "InTut-";
            var PrefixLen = Prefix.ToString().Length;
            var data2 = db.SITSPL_tblUsers.Where(x => x.UserType == "Intern-Tutor").ToList().Count();
            var StdCode = "";
            var LenStdCount = data2.ToString().Length;
            var StdCodeLength = db.SITSPL_tblUsers.Where(x => x.UserType == "Intern-Tutor").ToList().Count() + 1;
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



        // Intern Tutor data ( Credential by admin page)

        public JsonResult InternTutorData()
        {
            try
            {
                db = new CourseDataContext();

                var data = (from tut in db.SITSPL_tblTutors
                            join intntut in db.tblInternTutors
                            on tut.TutorId equals intntut.TutorId
                            join intstr in db.SITSPL_tblInternshipStructures
                            on intntut.InternshipId equals intstr.InternshipId
                            select new
                            {
                                tut.TutorName,

                                // intntut.InternTutorId,
                                intntut.TutorId,

                            }).Distinct().ToList();


                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        #region Add User by Admin for Tutor
        public ActionResult AddUser()
        {
            return View();
        }

        public JsonResult AddUsers(SITSPL_tblUser createuser)
        {
            try
            {
                bool result = false;
                db = new CourseDataContext();
                createuser.DateCreated = DateTime.Now;
                createuser.CreatedBy = "Admin";
                createuser.IsPublished = true;
                if (createuser.UserType == "Student")
                {
                    createuser.UserPrefix = "Std-";
                    var AutoId = (from d in db.SITSPL_tblUsers orderby d.AutoId descending where d.UserType == "Student" select d.AutoId).FirstOrDefault();
                    if (AutoId != null)
                    {
                        createuser.AutoId = AutoId;
                    }
                }
                else if (createuser.UserType == "Tutor")
                {
                    //   createuser.UserPrifix = "Tut-";
                    var AutoId = (from d in db.SITSPL_tblUsers orderby d.AutoId descending where d.UserType == "Tutor" select d.AutoId).FirstOrDefault();
                    if (AutoId != null)
                    {
                        createuser.AutoId = AutoId;
                    }
                }
                else if (createuser.UserType == "Intern")
                {
                    var AutoId = (from d in db.SITSPL_tblUsers orderby d.AutoId descending where d.UserType == "Intern" select d.AutoId).FirstOrDefault();
                    if (AutoId != null)
                    {
                        createuser.AutoId = AutoId;
                    }
                }

                else if (createuser.UserType == "Brand-Tutor")
                {
                    var AutoId = (from d in db.SITSPL_tblUsers orderby d.AutoId descending where d.UserType == "Brand-Tutor" select d.AutoId).FirstOrDefault();
                    if (AutoId != null)
                    {
                        createuser.AutoId = AutoId;
                    }
                }


                else if (createuser.UserType == "Intern-Tutor")
                {
                    var AutoId = (from d in db.SITSPL_tblUsers orderby d.AutoId descending where d.UserType == "Intern-Tutor" select d.AutoId).FirstOrDefault();
                    if (AutoId != null)
                    {
                        createuser.AutoId = AutoId;
                    }
                }

                db.SITSPL_tblUsers.InsertOnSubmit(createuser);
                db.SubmitChanges();
                result = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult UpdateUsers(SITSPL_tblUser updateuser)
        {
            try
            {
                bool result = false;
                db = new CourseDataContext();
                SITSPL_tblUser usertable = db.SITSPL_tblUsers.Where(x => x.UserId == updateuser.UserId).SingleOrDefault();
                if (usertable != null)
                {
                    usertable.UserId = updateuser.UserId;
                    usertable.Id = updateuser.Id;
                    usertable.UserName = updateuser.UserName;
                    usertable.UserType = updateuser.UserType;
                    //  usertable.Password = updateuser.Password;
                    usertable.IsPublished = updateuser.IsPublished;
                    db.SubmitChanges();
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
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion END Add User by Admin for Tutor

        #region View Users
        public ActionResult ViewUsers()
        {
            return View();
        }
        #endregion END View Users

        #region Add activity BY Dilshad A. on 26 Aug 2020
        [HttpPost]
        public JsonResult AddActivity(tblActivity objActivity)
        {
            try
            {
                if (objActivity != null)
                {
                    db = new CourseDataContext();
                    objActivity.DateCreated = DateTime.Now;
                    objActivity.ActivityDate = DateTime.Now;
                    objActivity.CreatedBy = "Admin";
                    objActivity.IsPublished = true;
                    objActivity.Id = objActivity.Id;
                    db.tblActivities.InsertOnSubmit(objActivity);
                    db.SubmitChanges();
                    return Json(objActivity.ActivityId, JsonRequestBehavior.AllowGet);
                }
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion  Add activity BY Dilshad A. on 26 Aug 2020

        #region View Activity by Dilshad A. on 26 Aug 2020
        public ActionResult ViewActivity()
        {
            return View();
        }

        public JsonResult ViewActivityDetails()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.tblActivities select d).ToList();
                var list = data.Select(x => new
                {
                    x.ActivityId,
                    x.ActivityOperation,
                    x.ActivityMessage,
                    x.ActivityDescription,
                    ActivityDate = x.ActivityDate.HasValue ? x.ActivityDate.Value.ToString("dd/MM/yyyy") : null,
                    x.IsPublished
                }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END View Activity by Dilshad A. on 26 Aug 2020


        #region Init Duration Master

        // Getting data from Course Master
        public JsonResult GetAddedDurations()
        {
            try
            {
                db = new CourseDataContext();
                //    var data = db.SITSPL_AddCourse().OrderByDescending(x => x.CourseId).ToList();
                var sub = (from dur in db.tblDurations
                           select new
                           {
                               dur
                           }).ToList();

                var data = sub.Select(x => new
                {
                    x.dur.DurationId,
                    x.dur.DurationName,
                    x.dur.CreatedBy,
                    DateCreated = x.dur.DateCreated.ToString("dd/MM/yyyy"),
                    x.dur.IsPublished
                }).ToList();

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
        #endregion Init Duration Master


        #region Add Duration Master
        public ActionResult AddDuration()
        {
            return View();
        }

        // Duration Create 

        [HttpPost]
        public JsonResult CreateDuration(tblDuration objDuration)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                string Cour = objDuration.DurationName;
                var data = db.tblDurations.Select(x => new { x.DurationName }).ToList();
                var Cours = data.Select(x => x.DurationName);
                if (Cour != null && !Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    objDuration.DateCreated = DateTime.Now;
                    objDuration.IsPublished = true;
                    objDuration.CreatedBy = "Admin";
                    objDuration.DurationName = objDuration.DurationName;
                    db.tblDurations.InsertOnSubmit(objDuration);
                    db.SubmitChanges();
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else if (Cour != null && Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    ViewBag.InsertMessage = "Duration already exists";
                    result = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("empty", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Add Duration Master

        #region  Update Duration

        // Update Course
        [HttpPost]
        public JsonResult UpdateDuration(tblDuration update)
        {
            try
            {
                db = new CourseDataContext();
                bool isResult = false;
                //   string Cour = Course.Trim();
                if (update.DurationId > 0)
                {
                    tblDuration duration = db.tblDurations.Where(x => x.DurationId == update.DurationId).SingleOrDefault();
                    var data = db.tblDurations.Select(x => x.DurationName).ToList();

                    var alldata = db.tblDurations.Where(x => x.DurationId == update.DurationId).Select(x => new
                    {
                        x.DurationId,
                        x.DurationName
                    }).FirstOrDefault();

                    if (data.Contains(update.DurationName.Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        if (update.DurationId == alldata.DurationId && update.DurationName.ToLower() == alldata.DurationName.ToLower())
                        {
                            duration.DurationName = update.DurationName;
                            duration.UpdatedBy = "Admin";
                            duration.LastUpdated = DateTime.Now;
                            db.SubmitChanges();
                            isResult = true;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            isResult = false;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(update.DurationName.Trim()))
                        {
                            duration.DurationName = update.DurationName;
                            duration.UpdatedBy = "Admin";
                            duration.LastUpdated = DateTime.Now;
                            db.SubmitChanges();
                            isResult = true;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion Update Duration



        // Delete Duration
        #region Delete Duration
        [HttpPost]
        public JsonResult DeleteDuration(int Id)
        {
            try
            {
                db = new CourseDataContext();
                if (Id > 0)
                {
                    tblDuration duration = db.tblDurations.Where(x => x.DurationId == Id).FirstOrDefault();
                    if (duration != null)
                    {
                        db.tblDurations.DeleteOnSubmit(duration);
                        db.SubmitChanges();
                        return Json(Id.ToString(), JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                string str = ex.Message;
                string err = str.Substring(0, 61);

                if (err == "The DELETE statement conflicted with the REFERENCE constraint")
                {
                    return Json("durationdepend", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ex.Message, JsonRequestBehavior.AllowGet);
                }

                //  return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion Delete Duration

        #region Add Duration Master
        public ActionResult AddInternActivityType()
        {
            return View();
        }
        #endregion End

        #region Internship Activity Type Create
        [HttpPost]
        public JsonResult CreateInternshipActivityType(tblInternActivityType objInternActivityType)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                string Cour = objInternActivityType.InternActivityType;
                var data = db.tblInternActivityTypes.Select(x => new { x.InternActivityType }).ToList();
                var Cours = data.Select(x => x.InternActivityType);
                if (Cour != null && !Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    objInternActivityType.DateCreated = DateTime.Now;
                    objInternActivityType.IsPubllished = true;
                    objInternActivityType.CreatedBy = "Admin";
                    objInternActivityType.InternActivityType = Cour.Trim();
                    db.tblInternActivityTypes.InsertOnSubmit(objInternActivityType);
                    db.SubmitChanges();
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else if (Cour != null && Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    ViewBag.InsertMessage = "Internship Activity Type already exists";
                    result = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("empty", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Internship Activity

        #region Init Internship Activity Type Master
        // Getting data from Internship Activity Type Master
        public JsonResult GetAddedInternshipType()
        {
            try
            {
                db = new CourseDataContext();
                //    var data = db.SITSPL_AddCourse().OrderByDescending(x => x.CourseId).ToList();
                var sub = (from act in db.tblInternActivityTypes
                           select new
                           {
                               act
                           }).ToList();

                var data = sub.Select(x => new
                {
                    x.act.InternActivityTypeId,
                    x.act.InternActivityType,
                    x.act.CreatedBy,
                    DateCreated = x.act.DateCreated.ToString("dd/MM/yyyy"),
                    x.act.IsPubllished
                }).ToList();

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
        #endregion Init Internship Activity Type

        #region Internship Activity Type Update
        // Update Internship Activity Type
        [HttpPost]
        public JsonResult UpdateInternActivityType(tblInternActivityType update)
        {
            try
            {
                db = new CourseDataContext();
                bool isResult = false;
                //   string Cour = Course.Trim();
                if (update.InternActivityTypeId > 0)
                {
                    tblInternActivityType duration = db.tblInternActivityTypes.Where(x => x.InternActivityTypeId == update.InternActivityTypeId).SingleOrDefault();
                    var data = db.tblInternActivityTypes.Select(x => x.InternActivityType).ToList();

                    var alldata = db.tblInternActivityTypes.Where(x => x.InternActivityTypeId == update.InternActivityTypeId).Select(x => new
                    {
                        x.InternActivityTypeId,
                        x.InternActivityType
                    }).FirstOrDefault();

                    if (data.Contains(update.InternActivityType.Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        if (update.InternActivityTypeId == alldata.InternActivityTypeId && update.InternActivityType.ToLower() == alldata.InternActivityType.ToLower())
                        {
                            duration.InternActivityType = update.InternActivityType;
                            db.SubmitChanges();
                            isResult = true;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            isResult = false;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(update.InternActivityType.Trim()))
                        {
                            duration.InternActivityType = update.InternActivityType;
                            db.SubmitChanges();
                            isResult = true;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion Update Internship Activity Type

        #region Delete Inernship  Activity Type
        // Delete Internship Activity Type
        [HttpPost]
        public JsonResult DeleteInternshipActivityType(int Id)
        {
            try
            {
                db = new CourseDataContext();
                if (Id > 0)
                {
                    tblInternActivityType deleteactivitytype = db.tblInternActivityTypes.Where(x => x.InternActivityTypeId == Id).FirstOrDefault();
                    if (deleteactivitytype != null)
                    {
                        db.tblInternActivityTypes.DeleteOnSubmit(deleteactivitytype);
                        db.SubmitChanges();
                        return Json(Id.ToString(), JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                string err = str.Substring(0, 61);

                if (err == "The DELETE statement conflicted with the REFERENCE constraint")
                {
                    return Json("strinternactivitytype", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ex.Message, JsonRequestBehavior.AllowGet);
                }

                // return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion Delete Internship Activity Type 

        #region Notification By Dilshad A. on 07 Sept 2020
        //public JsonResult GetNotificationContacts()
        //{
        //    var notificationRegisterTime = Session["LastUpdated"] != null ? Convert.ToDateTime(Session["LastUpdated"]) : DateTime.Now;
        //    NotificationComponent NC = new NotificationComponent();
        //    var list = NC.GetContacts(notificationRegisterTime);
        //    //update session here for get only new added contacts (notification)
        //    Session["LastUpdate"] = DateTime.Now;
        //    return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}
        #endregion Notification By Dilshad A. on 07 Sept 2020       

        #region Inter Student Details By Dilshad A. on 10 Sept 2020
        public ActionResult InternStudent()
        {
            return View();
        }

        public JsonResult InternStudentDetails()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from i in db.tblInternApplies
                            join c in db.SITSPL_tblContacts on i.InterApllyId equals c.UserId
                            //join d in db.SITSPL_tblDocuments on i.InterApllyId equals d.UserId
                            join inStr in db.SITSPL_tblInternshipStructures on i.InternshipStructureId equals inStr.InternStructureId
                            join intr in db.SITSPL_tblInternships on inStr.InternshipId equals intr.InternshipId
                            select new
                            {
                                i,
                                c,
                                //d,
                                inStr,
                                intr
                            }).Distinct().ToList();
                var internWithContact = data.Select(x => new
                {
                    x.i.InterApllyId,
                    x.i.Name,
                    DOB = x.i.DOB.ToString("dd/MM/yyyy"),
                    x.i.Email,
                    x.i.Twiter,
                    x.i.Instagram,
                    x.i.Facebook,
                    x.i.HowDoYouKnow,
                    x.i.Image,
                    x.i.CollegeUniv,
                    x.i.Trade,
                    x.i.YearOfPassing,
                    x.intr.InternshipId,
                    x.intr.InternshipName,
                    x.inStr.DurationMonths,
                    x.c.Mobile,
                    x.c.Address,
                    x.c.State,
                    x.c.City,
                    x.c.Pincode,
                    x.c.Landmark,
                    DateCreated = x.i.DateCreated.HasValue ? x.i.DateCreated.Value.ToString("dd/MM/yyyy") : null,
                    x.i.CreatedBy,
                    LastUpdated = x.i.LastUpdated.HasValue ? x.i.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                    x.i.UpdatedBy
                }).Distinct().ToList();

                var internDoc = (from i in db.tblInternApplies
                                 join c in db.SITSPL_tblDocuments on i.InterApllyId equals c.UserId
                                 select new
                                 {
                                     i.InterApllyId,
                                     c.UserId,
                                     c.DocumentId,
                                     c.DocumentName,
                                     c.DocumentType,
                                     c.DoucmentNo
                                 }).ToList();
                var list = new { internWithContact, internDoc };

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Inter Student Details By Dilshad A. on 10 Sept 2020

        #region Inter Tutor List        
        public ActionResult InternListTutor()
        {
            return View();
        }

        public JsonResult InterListTutorsData()
        {
            try
            {
                db = new CourseDataContext();

                var data = (from intr in db.SITSPL_tblInternships
                            select new
                            {
                                intr
                            }).ToList();

                var list = data.Select(x => new
                {
                    x.intr.InternshipId,
                    x.intr.InternshipName,
                    x.intr.CreatedBy,
                    DateCreated = x.intr.DateCreated.ToString("dd/MM/yyyy"),
                    x.intr.UpdatedBy,
                    LastUpdated = x.intr.LastUpdated.HasValue ? x.intr.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                    x.intr.IsDeleted,
                    x.intr.DeletedBy,
                    x.intr.DateDeleted,
                    x.intr.IsPublished,
                    x.intr.PublishedBy,
                    DatePublished = x.intr.DatePublished.HasValue ? x.intr.DatePublished.Value.ToString("dd/MM/yyyy") : null,
                    x.intr.DisplayOnweb
                }).ToList();

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);

            }
        }
        #endregion END Inter Tutor List        

        #region Communication to teachers/Students By Dilshad A. on 12 Sept 2020
        public ActionResult AddCommunication()
        {
            return View();
        }
        #endregion END Communication to teachers/Students By Dilshad A. on 12 Sept 2020

        #region Get Student Details for Angu Search By Dilshad A. on 12 Sept 2020
        public JsonResult GetStudentDetails()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.SITSPL_tblStudentProfiles
                            join d1 in db.SITSPL_tblUsers on d.StudentId equals d1.Id
                            select new
                            {
                                d.StudentId,
                                d1.Id,
                                d.Name,
                                d.Email,
                                d1.UserName,
                            }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Get Student Details for Angu Search By Dilshad A. on 12 Sept 2020

        #region Get Tutor Details for Angu Search By Dilshad A. on 14 Sept 2020
        public JsonResult GetTuturDetails()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.SITSPL_tblTutors
                            join d1 in db.SITSPL_tblUsers on d.TutorId equals d1.Id
                            select new
                            {
                                d.TutorId,
                                d1.Id,
                                d.TutorName,
                                d.TutorEmail,
                                d1.UserName,
                            }).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Get Tutor Details for Angu Search By Dilshad A. on 14 Sept 2020



        #region View Communication By Atul On 17 Sept 2020        
        public ActionResult Communication()
        {
            return View();
        }

        public JsonResult GetCommunications()
        {
            db = new CourseDataContext();
            var data = db.SITSPL_GetCommunication().OrderByDescending(x=>x.CommunicationId).ToList();

            #region Communication Comment Code            
            //var data = (from y in db.tblCommunications
            //            join i in db.tblInternApplies on y.Id equals i.InterApllyId into result
            //            from intern in result.DefaultIfEmpty()
            //            join t in db.SITSPL_tblTutors on y.Id equals t.TutorId into res2
            //            from tutor in res2.DefaultIfEmpty()
            //            select new
            //            {
            //                y,
            //                intern,
            //                tutor
            //            }).ToList();

            //var tutorData = data.Select(k => k.tutor).ToList();
            //var internData = data.Select(k => k.intern).ToList();
            //var commData = data.Select(k => k.y).ToList();
            //if (tutorData[0] != null && internData[0] != null && commData[0] != null)
            //{
            //    var list = data.Select(x => new
            //    {
            //        x.y.CommunicationId,
            //        x.y.Id,
            //        InternStudentName = x.intern.Name,
            //        TutorName = x.tutor.TutorName,
            //        x.y.Message,
            //        x.y.IsAllTeacher,
            //        x.y.IsAllStudent,
            //        x.y.MessageAllTeachers,
            //        x.y.MessageAllStudents,
            //        x.y.IsAll,
            //        x.y.MessageAll,
            //        x.y.UserType,
            //        x.y.CreatedBy,
            //        DateCreated = x.y.DateCreated.ToString("dd/MM/yyyy"),
            //        x.y.UpdatedBy,
            //        LastUpdated = x.y.LastUpdated.HasValue ? x.y.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
            //        x.y.DisplayOnWeb,
            //        x.y.IsPublished
            //    }).ToList();
            //    return Json(list, JsonRequestBehavior.AllowGet);
            //}
            //else if (tutorData[0] != null && commData[0] != null)
            //{
            //    var list = data.Select(x => new
            //    {
            //        x.y.CommunicationId,
            //        x.y.Id,
            //        InternStudentName = "-",
            //        TutorName = x.tutor.TutorName,
            //        x.y.Message,
            //        x.y.IsAllTeacher,
            //        x.y.IsAllStudent,
            //        x.y.MessageAllTeachers,

            //        x.y.MessageAllStudents,
            //        x.y.IsAll,
            //        x.y.MessageAll,
            //        x.y.UserType,
            //        x.y.CreatedBy,
            //        DateCreated = x.y.DateCreated.ToString("dd/MM/yyyy"),
            //        x.y.UpdatedBy,
            //        LastUpdated = x.y.LastUpdated.HasValue ? x.y.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
            //        x.y.DisplayOnWeb,
            //        x.y.IsPublished
            //    }).ToList();
            //    return Json(list, JsonRequestBehavior.AllowGet);
            //}
            //else if (internData[0] != null && commData[0] != null)
            //{
            //    var list = data.Select(x => new
            //    {
            //        x.y.CommunicationId,
            //        x.y.Id,
            //        InternStudentName = x.intern.Name,
            //        TutorName = "-",
            //        x.y.Message,
            //        x.y.IsAllTeacher,
            //        x.y.IsAllStudent,
            //        x.y.MessageAllTeachers,
            //        x.y.MessageAllStudents,
            //        x.y.IsAll,
            //        x.y.MessageAll,
            //        x.y.UserType,
            //        x.y.CreatedBy,
            //        DateCreated = x.y.DateCreated.ToString("dd/MM/yyyy"),
            //        x.y.UpdatedBy,
            //        LastUpdated = x.y.LastUpdated.HasValue ? x.y.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
            //        x.y.DisplayOnWeb,
            //        x.y.IsPublished
            //    }).ToList();
            //    return Json(list, JsonRequestBehavior.AllowGet);
            //}
            //else if (commData[0] != null)
            //{
            //    var list = data.Select(x => new
            //    {
            //        x.y.CommunicationId,
            //        x.y.Id,
            //        InternStudentName = "-",
            //        TutorName = "-",
            //        x.y.Message,
            //        x.y.IsAllTeacher,
            //        x.y.IsAllStudent,
            //        x.y.MessageAllTeachers,
            //        x.y.MessageAllStudents,
            //        x.y.IsAll,
            //        x.y.MessageAll,
            //        x.y.UserType,
            //        x.y.CreatedBy,
            //        DateCreated = x.y.DateCreated.ToString("dd/MM/yyyy"),
            //        x.y.UpdatedBy,
            //        LastUpdated = x.y.LastUpdated.HasValue ? x.y.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
            //        x.y.DisplayOnWeb,
            //        x.y.IsPublished
            //    }).ToList();
            //    return Json(list, JsonRequestBehavior.AllowGet);
            //}
            #endregion END Communication Comment Code
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion END View Communication By Atul On 17 Sept 2020

        #region AddCourse Content

        public ActionResult AddCourseContent()
        {
            return View();
        }


        [HttpPost]

        public JsonResult AddCourseContentDescription(tblContent coursecontent, List<tblCourseDescription> ContentDescriptionArray, List<tblCoursePrerequisity> arrCoursePrerequisities
            , List<tblContentWhatWillYouLearn> lstWhatWillLearn, List<tblContentRequirement> lstRequirements, List<tblContentFAQ> lstFAQs,
            List<tblContentLatestPoint> lstLatestPoints)
        {
            
            try
            {
                db = new CourseDataContext();
                bool result = false;
                tblContent content = new tblContent();
                content.CourseContentHeading = coursecontent.CourseContentHeading;
                content.CourseType = coursecontent.CourseType;
                if (coursecontent.CourseId != null)
                {
                    content.CourseId = coursecontent.CourseId;
                }
                else if (coursecontent.InternCourseId != null)
                {
                    content.InternCourseId = coursecontent.InternCourseId;
                }

                content.IsSubHeading = coursecontent.IsSubHeading;
                if (coursecontent.SubHeading != null)
                {
                    content.SubHeading = coursecontent.SubHeading;
                }

                if (coursecontent.ShortDescription != null)
                {
                    content.ShortDescription = coursecontent.ShortDescription;
                }

                content.About = coursecontent.About;
                content.IsPublished = coursecontent.IsPublished;
                content.DateCreated = DateTime.Now;
                content.CreatedBy = "Admin";
                db.tblContents.InsertOnSubmit(content);
                db.SubmitChanges();

                Int64 contentid = content.ContentId;
                if (contentid > 0)
                {
                    if (ContentDescriptionArray != null)
                    {
                        for (var i = 0; i < ContentDescriptionArray.Count; i++)
                        {


                            tblCourseDescription courseDescription = new tblCourseDescription();
                            courseDescription.ContentId = contentid;
                            courseDescription.Description = ContentDescriptionArray[i].Description;
                            courseDescription.IsPublished = ContentDescriptionArray[i].IsPublished;
                            courseDescription.DateCreated = DateTime.Now;
                            courseDescription.IsPublished = ContentDescriptionArray[i].IsPublished;
                            db.tblCourseDescriptions.InsertOnSubmit(courseDescription);
                            db.SubmitChanges();

                        }

                        if (arrCoursePrerequisities != null)
                        {
                            for (var i = 0; i < arrCoursePrerequisities.Count; i++)
                            {

                                tblCoursePrerequisity coursePrerequisity = new tblCoursePrerequisity();
                                coursePrerequisity.ContentId = contentid;
                                coursePrerequisity.PrerequisitePoints = arrCoursePrerequisities[i].PrerequisitePoints;
                                coursePrerequisity.IsPublished = arrCoursePrerequisities[i].IsPublished;
                                coursePrerequisity.DateCreated = DateTime.Now;
                                coursePrerequisity.CreatedBy = "Admin";
                                db.tblCoursePrerequisities.InsertOnSubmit(coursePrerequisity);
                                db.SubmitChanges();


                            }
                        }

                        if (lstWhatWillLearn != null)
                        {
                            for (var i = 0; i < lstWhatWillLearn.Count; i++)
                            {

                                tblContentWhatWillYouLearn contentwhatwilllearn = new tblContentWhatWillYouLearn();
                                contentwhatwilllearn.ContentId = contentid;
                                contentwhatwilllearn.LearnPoint = lstWhatWillLearn[i].LearnPoint;
                                contentwhatwilllearn.IsPublished = lstWhatWillLearn[i].IsPublished;
                                contentwhatwilllearn.DateCreated = DateTime.Now;
                                contentwhatwilllearn.CreatedBy = "Admin";
                                db.tblContentWhatWillYouLearns.InsertOnSubmit(contentwhatwilllearn);
                                db.SubmitChanges();

                            }
                        }

                        if (lstRequirements != null)
                        {
                            for (var i = 0; i < lstRequirements.Count; i++)
                            {

                                tblContentRequirement requirement = new tblContentRequirement();
                                requirement.ContentId = contentid;
                                requirement.RequirementPoint = lstRequirements[i].RequirementPoint;
                                requirement.IsPublished = lstRequirements[i].IsPublished;
                                requirement.DateCreated = DateTime.Now;
                                requirement.CreatedBy = "Admin";
                                db.tblContentRequirements.InsertOnSubmit(requirement);
                                db.SubmitChanges();


                            }
                        }

                        if (lstFAQs != null)
                        {
                            for (var i = 0; i < lstFAQs.Count; i++)
                            {

                                tblContentFAQ contentFAQ = new tblContentFAQ();
                                contentFAQ.ContentId = contentid;
                                contentFAQ.Question = lstFAQs[i].Question;
                                contentFAQ.Answer = lstFAQs[i].Answer;
                                contentFAQ.IsPublished = lstFAQs[i].IsPublished;
                                contentFAQ.DateCreated = DateTime.Now;
                                contentFAQ.CreatedBy = "Admin";
                                db.tblContentFAQs.InsertOnSubmit(contentFAQ);
                                db.SubmitChanges();

                            }
                        }

                        if (lstLatestPoints != null)
                        {
                            for (var i = 0; i < lstLatestPoints.Count; i++)
                            {

                                tblContentLatestPoint contentLatestPoint = new tblContentLatestPoint();
                                contentLatestPoint.ContentId = contentid;
                                contentLatestPoint.LatestPoint = lstLatestPoints[i].LatestPoint;
                                contentLatestPoint.IsPublished = lstLatestPoints[i].IsPublished;
                                contentLatestPoint.DateCreated = DateTime.Now;
                                contentLatestPoint.CreatedBy = "Admin";
                                db.tblContentLatestPoints.InsertOnSubmit(contentLatestPoint);
                                db.SubmitChanges();

                            }
                        }

                    }
                }

                result = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ShowCourseContent()
        {

            return View();
        }

        public JsonResult ViewCourseContent()
        {
            try
            {
                db = new CourseDataContext();
                //var data = db.SITSPL_GetCourseContent().ToList();
                //return Json(data, JsonRequestBehavior.AllowGet);

                var sub = (from content in db.tblContents
                           join cour in db.SITSPL_tblCourses
                           on content.CourseId equals cour.CourseId into eGroup
                           from std in eGroup.DefaultIfEmpty()
                           join intern in db.SITSPL_tblInternships
                           on
                           content.InternCourseId equals intern.InternshipId
                           into eGroup2
                           from std2 in eGroup2.DefaultIfEmpty()

                           select new
                           {
                               content,
                               std,
                               std2
                           }).ToList();


                var stdProfile = sub.Select(x => x.content).Any();
                var stdContact = sub.Select(x => x.std).ToList();
                var std2Course = sub.Select(x => x.std2).ToList();


                if (stdProfile == true)
                {
                    var data = sub.Select(x => new
                    {
                        x.content.ContentId,
                        x.content.CourseType,
                        x.content.CourseContentHeading,
                        x.content.IsSubHeading,
                        x.content.IsPublished,
                        DateCreated = x.content.DateCreated.ToString("dd/MM/yyyy"),
                        x.content.CreatedBy
                    }).ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }



                //if(stdContact[0] != null && std2Course[0] == null)
                //{
                //    var data = sub.Select(x => new
                //    {

                //        x.content.ContentId,
                //        x.content.CourseType,
                //        x.content.CourseContentHeading,
                //        x.content.SubHeading,
                //        x.content.IsPublished,
                //        x.content.DateCreated,
                //        x.content.IsSubHeading,
                //        x.content.CreatedBy,
                //        x.content.CourseId,
                //        x.std.CourseName,

                //    }).ToList();

                //    return Json(data, JsonRequestBehavior.AllowGet);
                //}

                //else if(stdContact[0] == null && std2Course[0] != null)
                //{
                //    var data = sub.Select(x => new
                //    {
                //        x.content.ContentId,
                //        x.content.CourseType,
                //        x.content.CourseContentHeading,
                //        x.content.SubHeading,
                //        x.content.IsPublished,
                //        x.content.DateCreated,
                //        x.content.IsSubHeading,
                //        x.content.CreatedBy,
                //        x.content.InternCourseId,
                //        x.std2.InternshipName

                //    }).ToList();

                //    return Json(data, JsonRequestBehavior.AllowGet);
                //}

                //else if(stdContact[0] != null && std2Course[0] != null)
                //{
                //    var data = sub.Select(x => new
                //    {
                //        x.content.ContentId,
                //        x.content.CourseType,
                //        x.content.CourseContentHeading,
                //        x.content.SubHeading,
                //        x.content.IsPublished,
                //        x.content.DateCreated,
                //        x.content.IsSubHeading,
                //        x.content.CreatedBy,
                //        x.content.InternCourseId,
                //        x.content.CourseId,
                //        x.std.CourseName,
                //        x.std2.InternshipName
                //    }).ToList();

                //    return Json(data, JsonRequestBehavior.AllowGet);
                //}



                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // Edit Course Content

        public JsonResult EditCourseContent(int id, string userType)
        {
            try
            {
                //  var coursecontent = '';
                db = new CourseDataContext();
                var userty = userType.Trim();

                //    var coursecontent = db.SITSPL_GetCourseContent(id, userty).ToList();
                //       var coursecontent= null;


                if (userty == "Course")
                {
                    var coursecont = (from content in db.tblContents
                                      join cour in db.SITSPL_tblCourses
                                      on content.CourseId equals cour.CourseId
                                      where content.ContentId == id && content.CourseType == "Course"
                                      select new
                                      {
                                          content,
                                          cour
                                          //    content.ContentId,
                                          //    content.CourseId,
                                          //    cour.CourseName,
                                          //    content.CourseType,
                                          //    content.CourseContentHeading,
                                          //    content.IsSubHeading,
                                          //    content.SubHeading,
                                          //    content.ShortDescription,
                                          //    content.IsPublished,
                                          //    content.CreatedBy,
                                          //content.DateCreated
                                      }).ToList();

                    var coursecontent = coursecont.Select(x => new
                    {
                        x.content.ContentId,
                        x.content.CourseId,
                        x.cour.CourseName,
                        x.content.CourseType,
                        x.content.CourseContentHeading,
                        x.content.IsSubHeading,
                        x.content.SubHeading,
                        x.content.About,
                        x.content.ShortDescription,
                        x.content.IsPublished,
                        x.content.CreatedBy,
                        DateCreated = x.content.DateCreated.ToString("dd/MM/yyyy")
                    }).ToList();


                    var description = (from cont in db.tblContents
                                       join descrip in db.tblCourseDescriptions on
                                       cont.ContentId equals descrip.ContentId
                                       where descrip.ContentId == id
                                       select new
                                       {
                                           descrip.DescriptionId,
                                           descrip.Description,
                                           descrip.IsPublished
                                       }).ToList();

                    var prerequisities = (from cont in db.tblContents
                                          join prereq in db.tblCoursePrerequisities on
                                          cont.ContentId equals prereq.ContentId
                                          where prereq.ContentId == id
                                          select new
                                          {
                                              prereq.PrerequisiteId,
                                              prereq.PrerequisitePoints,
                                              prereq.IsPublished
                                          }).ToList();

                    var whatwilllearn = (from cont in db.tblContents
                                         join whatwilearn in db.tblContentWhatWillYouLearns
                                         on cont.ContentId equals whatwilearn.ContentId
                                         where whatwilearn.ContentId == id
                                         select new
                                         {
                                             whatwilearn.WhatWillYouLearnId,
                                             whatwilearn.LearnPoint,
                                             whatwilearn.IsPublished
                                         }).ToList();

                    var contentrequirement = (from cont in db.tblContents
                                              join req in db.tblContentRequirements
                                              on cont.ContentId equals req.ContentId
                                              where req.ContentId == id
                                              select new
                                              {
                                                  req.ContentRequirementId,
                                                  req.RequirementPoint,
                                                  req.IsPublished
                                              }).ToList();

                    var contentfaq = (from cont in db.tblContents
                               join contfaq in db.tblContentFAQs
                               on cont.ContentId equals contfaq.ContentId
                               where contfaq.ContentId == id
                               select new
                               {
                                   contfaq.ContentFAQId,
                                   contfaq.Question,
                                   contfaq.Answer,
                                   contfaq.IsPublished
                               }).ToList();

                    var latestpoint = (from cont in db.tblContents
                                       join contlatestpoint in db.tblContentLatestPoints
                                       on cont.ContentId equals contlatestpoint.ContentId
                                       where contlatestpoint.ContentId == id
                                       select new
                                       {
                                           contlatestpoint.ContentLatestId,
                                           contlatestpoint.LatestPoint,
                                           contlatestpoint.IsPublished
                                       }).ToList();

                    var list = new { coursecontent, description, prerequisities, whatwilllearn, contentrequirement, contentfaq, latestpoint };
                    JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                    var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else if (userty == "Intern")
                {
                    var coursecontent = (from content in db.tblContents
                                         join intr in db.SITSPL_tblInternships
                                         on content.InternCourseId equals intr.InternshipId
                                         where content.ContentId == id && content.CourseType == "Intern"
                                         select new
                                         {
                                             content.ContentId,
                                             content.InternCourseId,
                                             intr.InternshipName,
                                             content.CourseType,
                                             content.CourseContentHeading,
                                             content.IsSubHeading,
                                             content.SubHeading,
                                             content.ShortDescription,
                                             content.IsPublished,
                                             content.CreatedBy,
                                             content.About,
                                             content.DateCreated
                                         }).ToList();

                    var description = (from cont in db.tblContents
                                       join descrip in db.tblCourseDescriptions on
                                       cont.ContentId equals descrip.ContentId
                                       where descrip.ContentId == id
                                       select new
                                       {
                                           descrip.DescriptionId,
                                           descrip.Description,
                                           descrip.IsPublished
                                       }).ToList();

                    var prerequisities = (from cont in db.tblContents
                                          join prereq in db.tblCoursePrerequisities on
                                          cont.ContentId equals prereq.ContentId
                                          where prereq.ContentId == id
                                          select new
                                          {
                                              prereq.PrerequisiteId,
                                              prereq.PrerequisitePoints,
                                              prereq.IsPublished
                                          }).ToList();

                    var whatwilllearn = (from cont in db.tblContents
                                         join whatwilearn in db.tblContentWhatWillYouLearns
                                         on cont.ContentId equals whatwilearn.ContentId
                                         where whatwilearn.ContentId == id
                                         select new
                                         {
                                             whatwilearn.WhatWillYouLearnId,
                                             whatwilearn.LearnPoint,
                                             whatwilearn.IsPublished
                                         }).ToList();

                    var contentrequirement = (from cont in db.tblContents
                                              join req in db.tblContentRequirements
                                              on cont.ContentId equals req.ContentId
                                              where req.ContentId == id
                                              select new
                                              {
                                                  req.ContentRequirementId,
                                                  req.RequirementPoint,
                                                  req.IsPublished
                                              }).ToList();

                    var contentfaq = (from cont in db.tblContents
                                      join contfaq in db.tblContentFAQs
                                      on cont.ContentId equals contfaq.ContentId
                                      where contfaq.ContentId == id
                                      select new
                                      {
                                          contfaq.ContentFAQId,
                                          contfaq.Question,
                                          contfaq.Answer,
                                          contfaq.IsPublished
                                      }).ToList();

                    var latestpoint = (from cont in db.tblContents
                                       join contlatestpoint in db.tblContentLatestPoints
                                       on cont.ContentId equals contlatestpoint.ContentId
                                       where contlatestpoint.ContentId == id
                                       select new
                                       {
                                           contlatestpoint.ContentLatestId,
                                           contlatestpoint.LatestPoint,
                                           contlatestpoint.IsPublished
                                       }).ToList();


                    var list = new { coursecontent, description, prerequisities, whatwilllearn, contentrequirement, contentfaq, latestpoint };

                    JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                    var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                //var coursecontent = db.SITSPL_GetCourseContent(id, userty).ToList();

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }


        }

        // Update Course Content

        [HttpPost]
        public JsonResult UpdateCourseContentDescription(tblContent updatecoursecontent, List<tblCourseDescription> updatecoursearr, List<DeleteContentDescription> deleteDescriptionArray, List<tblCoursePrerequisity> arrCoursePrerequisities, List<DeleteContentPrerequisities> deletePrerequisityArray
            , List<tblContentWhatWillYouLearn> lstWhatWillLearn, List<tblContentRequirement> lstRequirements, List<tblContentFAQ> lstFAQs,
            List<tblContentLatestPoint> lstLatestPoints,
            List<DeleteContentWhatWillYouLearn> deleteWhatWillLearnArray,List<DeleteContentRequirement> deleteRequirementArray,
           List<DeleteContentFAQ> deleteFAQArray,List<DeleteContentLatestPoint> deleteLatestPointArray)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                tblContent updateContent = db.tblContents.Where(x => x.ContentId == updatecoursecontent.ContentId).FirstOrDefault();
                if (updateContent != null)
                {
                    updateContent.CourseType = updatecoursecontent.CourseType;
                    updateContent.CourseContentHeading = updatecoursecontent.CourseContentHeading;
                    updateContent.IsPublished = updatecoursecontent.IsPublished;
                    updateContent.IsSubHeading = updatecoursecontent.IsSubHeading;

                    if (updatecoursecontent.SubHeading != null && updateContent.SubHeading != "" || updateContent.SubHeading != " ")
                    {
                        updateContent.SubHeading = updatecoursecontent.SubHeading;
                    }

                    if (updatecoursecontent.ShortDescription != null)
                    {
                        updateContent.ShortDescription = updatecoursecontent.ShortDescription;
                    }

                    if (updatecoursecontent.CourseType == "Course")
                    {
                        updatecoursecontent.CourseId = updatecoursecontent.CourseId;
                    }
                    else if (updatecoursecontent.CourseType == "Intern")
                    {
                        updatecoursecontent.InternCourseId = updatecoursecontent.InternCourseId;
                    }

                    updateContent.About = updatecoursecontent.About;
                    updateContent.LastUpdated = DateTime.Now;
                    updateContent.UpdatedBy = "Admin";

                    db.SubmitChanges();

                    if (updatecoursecontent.ContentId > 0)
                    {
                        if (updatecoursearr != null)
                        {
                            for (int i = 0; i < updatecoursearr.Count; i++)
                            {
                                var objContentDescription = (from d in db.tblCourseDescriptions where d.ContentId == updatecoursecontent.ContentId && d.DescriptionId == Convert.ToInt32(updatecoursearr[i].DescriptionId) select d).FirstOrDefault();
                                if (objContentDescription != null)
                                {

                                    objContentDescription.ContentId = updatecoursecontent.ContentId;
                                    objContentDescription.Description = updatecoursearr[i].Description;
                                    objContentDescription.IsPublished = updatecoursearr[i].IsPublished;
                                    objContentDescription.UpdatedBy = "Admin";
                                    objContentDescription.LastUpdated = DateTime.Now;
                                    db.SubmitChanges();
                                }
                                else
                                {
                                    tblCourseDescription courseDescription = new tblCourseDescription();
                                    courseDescription.ContentId = updatecoursecontent.ContentId;
                                    courseDescription.Description = updatecoursearr[i].Description;
                                    courseDescription.IsPublished = updatecoursearr[i].IsPublished;
                                    courseDescription.DateCreated = DateTime.Now;
                                    courseDescription.IsPublished = updatecoursearr[i].IsPublished;
                                    db.tblCourseDescriptions.InsertOnSubmit(courseDescription);
                                    db.SubmitChanges();
                                }
                            }
                        }

                        if (arrCoursePrerequisities != null)
                        {
                            for (int i = 0; i < arrCoursePrerequisities.Count; i++)
                            {
                                var objCoursePrerequisities = (from d in db.tblCoursePrerequisities where d.ContentId == updatecoursecontent.ContentId && d.PrerequisiteId == Convert.ToInt32(arrCoursePrerequisities[i].PrerequisiteId) select d).FirstOrDefault();
                                if (objCoursePrerequisities != null)
                                {

                                    objCoursePrerequisities.ContentId = updatecoursecontent.ContentId;
                                    objCoursePrerequisities.PrerequisiteId = arrCoursePrerequisities[i].PrerequisiteId;
                                    objCoursePrerequisities.PrerequisitePoints = arrCoursePrerequisities[i].PrerequisitePoints;
                                    objCoursePrerequisities.IsPublished = arrCoursePrerequisities[i].IsPublished;
                                    objCoursePrerequisities.UpdateBy = "Admin";
                                    objCoursePrerequisities.LastUpdated = DateTime.Now;
                                    db.SubmitChanges();
                                }
                                else
                                {
                                    tblCoursePrerequisity coursePrerequisity = new tblCoursePrerequisity();
                                    coursePrerequisity.ContentId = updatecoursecontent.ContentId;
                                    coursePrerequisity.PrerequisitePoints = arrCoursePrerequisities[i].PrerequisitePoints;
                                    coursePrerequisity.IsPublished = arrCoursePrerequisities[i].IsPublished;
                                    coursePrerequisity.DateCreated = DateTime.Now;
                                    coursePrerequisity.CreatedBy = "Admin";
                                    db.tblCoursePrerequisities.InsertOnSubmit(coursePrerequisity);
                                    db.SubmitChanges();
                                }
                            }
                        }

                        if (lstWhatWillLearn != null)
                        {
                            for (var i = 0; i < lstWhatWillLearn.Count; i++)
                            {
                                tblContentWhatWillYouLearn contentLearn = db.tblContentWhatWillYouLearns.Where(x => x.ContentId == updatecoursecontent.ContentId && x.WhatWillYouLearnId == lstWhatWillLearn[i].WhatWillYouLearnId).FirstOrDefault();
                                if (contentLearn != null)
                                {
                                    contentLearn.WhatWillYouLearnId = lstWhatWillLearn[i].WhatWillYouLearnId;
                                    contentLearn.LearnPoint = lstWhatWillLearn[i].LearnPoint;
                                    contentLearn.IsPublished = lstWhatWillLearn[i].IsPublished;
                                    contentLearn.LastUpdated = DateTime.Now;
                                    contentLearn.UpdatedBy = "Admin";
                                    db.SubmitChanges();
                                }

                                else
                                {
                                    tblContentWhatWillYouLearn contentwhatwilllearn = new tblContentWhatWillYouLearn();
                                    contentwhatwilllearn.ContentId = updatecoursecontent.ContentId;
                                    contentwhatwilllearn.LearnPoint = lstWhatWillLearn[i].LearnPoint;
                                    contentwhatwilllearn.IsPublished = lstWhatWillLearn[i].IsPublished;
                                    contentwhatwilllearn.DateCreated = DateTime.Now;
                                    contentwhatwilllearn.CreatedBy = "Admin";
                                    db.tblContentWhatWillYouLearns.InsertOnSubmit(contentwhatwilllearn);
                                    db.SubmitChanges();
                                }
                            }
                        }

                        if (lstRequirements != null)
                        {
                            for (var i = 0; i < lstRequirements.Count; i++)
                            {

                                tblContentRequirement contentRequirement = db.tblContentRequirements.Where(x => x.ContentId == updatecoursecontent.ContentId && x.ContentRequirementId == lstRequirements[i].ContentRequirementId).FirstOrDefault();
                                if (contentRequirement != null)
                                {
                                    contentRequirement.ContentRequirementId = lstRequirements[i].ContentRequirementId;
                                    contentRequirement.RequirementPoint = lstRequirements[i].RequirementPoint;
                                    contentRequirement.IsPublished = lstWhatWillLearn[i].IsPublished;
                                    contentRequirement.LastUpdated = DateTime.Now;
                                    contentRequirement.UpdatedBy = "Admin";
                                    db.SubmitChanges();
                                }

                                else
                                {
                                    tblContentRequirement requirement = new tblContentRequirement();
                                    requirement.ContentId = updatecoursecontent.ContentId;
                                    requirement.RequirementPoint = lstRequirements[i].RequirementPoint;
                                    requirement.IsPublished = lstRequirements[i].IsPublished;
                                    requirement.DateCreated = DateTime.Now;
                                    requirement.CreatedBy = "Admin";
                                    db.tblContentRequirements.InsertOnSubmit(requirement);
                                    db.SubmitChanges();
                                }

                            }
                        }

                        if (lstFAQs != null)
                        {
                            for (var i = 0; i < lstFAQs.Count; i++)
                            {
                                tblContentFAQ updatecontentFAQ = db.tblContentFAQs.Where(x => x.ContentId == updatecoursecontent.ContentId && x.ContentFAQId == lstFAQs[i].ContentFAQId).FirstOrDefault();
                                if (updatecontentFAQ != null)
                                {
                                    updatecontentFAQ.ContentFAQId = lstFAQs[i].ContentFAQId;
                                    updatecontentFAQ.Question = lstFAQs[i].Question;
                                    updatecontentFAQ.Answer = lstFAQs[i].Answer;
                                    updatecontentFAQ.IsPublished = lstFAQs[i].IsPublished;
                                    updatecontentFAQ.LastUpdated = DateTime.Now;
                                    updatecontentFAQ.UpdatedBy = "Admin";
                                    db.SubmitChanges();
                                }

                                else
                                {
                                    tblContentFAQ contentFAQ = new tblContentFAQ();
                                    contentFAQ.ContentId = updatecoursecontent.ContentId;
                                    contentFAQ.Question = lstFAQs[i].Question;
                                    contentFAQ.Answer = lstFAQs[i].Answer;
                                    contentFAQ.IsPublished = lstFAQs[i].IsPublished;
                                    contentFAQ.DateCreated = DateTime.Now;
                                    contentFAQ.CreatedBy = "Admin";
                                    db.tblContentFAQs.InsertOnSubmit(contentFAQ);
                                    db.SubmitChanges();
                                }

                            }
                        }

                        if (lstLatestPoints != null)
                        {
                            for (var i = 0; i < lstLatestPoints.Count; i++)
                            {

                                tblContentLatestPoint updatecontentLatestPoint = db.tblContentLatestPoints.Where(x => x.ContentId == updatecoursecontent.ContentId && x.ContentLatestId == lstLatestPoints[i].ContentLatestId).FirstOrDefault();
                                if (updatecontentLatestPoint != null)
                                {
                                    updatecontentLatestPoint.ContentLatestId = lstLatestPoints[i].ContentLatestId;
                                    updatecontentLatestPoint.LatestPoint = lstLatestPoints[i].LatestPoint;
                                    updatecontentLatestPoint.IsPublished = lstFAQs[i].IsPublished;
                                    updatecontentLatestPoint.LastUpdated = DateTime.Now;
                                    updatecontentLatestPoint.UpdatedBy = "Admin";
                                    db.SubmitChanges();
                                    db.SubmitChanges();
                                }

                                else
                                {
                                    tblContentLatestPoint contentLatestPoint = new tblContentLatestPoint();
                                    contentLatestPoint.ContentId = updatecoursecontent.ContentId;
                                    contentLatestPoint.LatestPoint = lstLatestPoints[i].LatestPoint;
                                    contentLatestPoint.IsPublished = lstLatestPoints[i].IsPublished;
                                    contentLatestPoint.DateCreated = DateTime.Now;
                                    contentLatestPoint.CreatedBy = "Admin";
                                    db.tblContentLatestPoints.InsertOnSubmit(contentLatestPoint);
                                    db.SubmitChanges();
                                }

                            }
                        }


                        if (deleteDescriptionArray != null)
                        {
                            for (int i = 0; i < deleteDescriptionArray.Count; i++)
                            {
                                if (deleteDescriptionArray[i].DescriptionId > 0)
                                {

                                    tblCourseDescription courseDescription = (from d in db.tblCourseDescriptions where d.DescriptionId == deleteDescriptionArray[i].DescriptionId && d.ContentId == updatecoursecontent.ContentId select d).FirstOrDefault();
                                    db.tblCourseDescriptions.DeleteOnSubmit(courseDescription);
                                    db.SubmitChanges();
                                }

                            }
                        }

                        if(deleteWhatWillLearnArray != null)
                        {
                            for (int i = 0; i < deleteWhatWillLearnArray.Count; i++)
                            {
                                if (deleteWhatWillLearnArray[i].WhatWillYouLearnId > 0)
                                {

                                    tblContentWhatWillYouLearn contentWhatWillLearn = (from d in db.tblContentWhatWillYouLearns where d.WhatWillYouLearnId == deleteWhatWillLearnArray[i].WhatWillYouLearnId && d.ContentId == updatecoursecontent.ContentId select d).FirstOrDefault();
                                    db.tblContentWhatWillYouLearns.DeleteOnSubmit(contentWhatWillLearn);
                                    db.SubmitChanges();
                                }

                            }
                        }


                        if (deleteRequirementArray != null)
                        {
                            for (int i = 0; i < deleteRequirementArray.Count; i++)
                            {
                                if (deleteRequirementArray[i].ContentRequirementId > 0)
                                {

                                    tblContentRequirement contentRequirement = (from d in db.tblContentRequirements where d.ContentRequirementId == deleteRequirementArray[i].ContentRequirementId && d.ContentId == updatecoursecontent.ContentId select d).FirstOrDefault();
                                    db.tblContentRequirements.DeleteOnSubmit(contentRequirement);
                                    db.SubmitChanges();
                                }

                            }
                        }


                        if (deleteFAQArray != null)
                        {
                            for (int i = 0; i < deleteFAQArray.Count; i++)
                            {
                                if (deleteFAQArray[i].ContentFAQId > 0)
                                {

                                    tblContentFAQ contentFAQ = (from d in db.tblContentFAQs where d.ContentFAQId == deleteFAQArray[i].ContentFAQId && d.ContentId == updatecoursecontent.ContentId select d).FirstOrDefault();
                                    db.tblContentFAQs.DeleteOnSubmit(contentFAQ);
                                    db.SubmitChanges();
                                }

                            }
                        }


                        if (deleteLatestPointArray != null)
                        {
                            for (int i = 0; i < deleteLatestPointArray.Count; i++)
                            {
                                if (deleteLatestPointArray[i].ContentLatestId > 0)
                                {

                                    tblContentLatestPoint contentLatestPoint = (from d in db.tblContentLatestPoints where d.ContentLatestId == deleteLatestPointArray[i].ContentLatestId && d.ContentId == updatecoursecontent.ContentId select d).FirstOrDefault();
                                    db.tblContentLatestPoints.DeleteOnSubmit(contentLatestPoint);
                                    db.SubmitChanges();
                                }

                            }
                        }


                        //                 ,
                        //    List<DeleteContentFAQ> , List< DeleteContentLatestPoint > 



                        if (deletePrerequisityArray != null)
                        {
                            for (int i = 0; i < deletePrerequisityArray.Count; i++)
                            {
                                if (deletePrerequisityArray[i].PrerequisiteId > 0)
                                {

                                    tblCoursePrerequisity coursePrerequisity = (from d in db.tblCoursePrerequisities where d.PrerequisiteId == deletePrerequisityArray[i].PrerequisiteId && d.ContentId == updatecoursecontent.ContentId select d).FirstOrDefault();
                                    db.tblCoursePrerequisities.DeleteOnSubmit(coursePrerequisity);
                                    db.SubmitChanges();
                                }

                            }
                        }


                    }

                }
                result = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        //// Get Heading Description For Course

        //public JsonResult GetCourseHeading(int CourseId)
        //{
        //    try
        //    {
        //        db = new CourseDataContext();              
        //        var coursecont = (from content in db.tblContents

        //                          where content.CourseId == CourseId
        //                          select new
        //                          {
        //                              content.CourseContentHeading,
        //                              content.IsSubHeading,
        //                              content.SubHeading,
        //                              content.ContentId
        //                          }).ToList();

        //        var description = (from cont in db.tblContents
        //                           join descrip in db.tblCourseDescriptions on
        //                           cont.ContentId equals descrip.ContentId
        //                           where cont.CourseId == CourseId
        //                           select new
        //                           {
        //                               cont.ContentId,
        //                               descrip.DescriptionId,
        //                               descrip.Description,
        //                               descrip.IsPublished
        //                           }).ToList();

        //        if (coursecont != null && description != null || description == null)
        //        {
        //            var list = new { coursecont, description };

        //            JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        //            var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
        //            return Json(result, JsonRequestBehavior.AllowGet);

        //        }
        //        else
        //        {
        //            return Json("", JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message, JsonRequestBehavior.AllowGet);
        //    }
        //}


        public JsonResult GetInternHeading(int InternId)
        {
            try
            {
                db = new CourseDataContext();
                //var coursecont = (from content in db.tblContents
                //            join courdesc in db.tblCourseDescriptions
                //            on content.ContentId equals courdesc.ContentId
                //            where content.CourseId == CourseId 
                //            select new {
                //                content.CourseContentHeading,
                //                content.IsSubHeading,
                //                content.SubHeading
                //            }).ToList();


                var Interncont = (from content in db.tblContents

                                  where content.InternCourseId == InternId
                                  select new
                                  {
                                      content.CourseContentHeading,
                                      content.IsSubHeading,
                                      content.SubHeading,
                                      content.ContentId,
                                      content.ShortDescription
                                  }).ToList();

                var interndescription = (from cont in db.tblContents
                                         join descrip in db.tblCourseDescriptions on
                                         cont.ContentId equals descrip.ContentId
                                         where cont.InternCourseId == InternId
                                         select new
                                         {
                                             descrip.ContentId,
                                             descrip.DescriptionId,
                                             descrip.Description,
                                             descrip.IsPublished
                                         }).ToList();

                if (Interncont != null && interndescription != null || interndescription == null)
                {
                    var list = new { Interncont, interndescription };

                    JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                    var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
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

        #endregion End Course Content

        #region Student Category

        public JsonResult GetStudentCategories()
        {
            try
            {
                db = new CourseDataContext();
                var sub = (from stdcatg in db.tblStudentCategories
                           select new
                           {
                               stdcatg
                           }).ToList();
                var data = sub.Select(x => new
                {
                    x.stdcatg.StudentCategoryId,
                    x.stdcatg.CategoryName,
                    DateCreated = x.stdcatg.DateCreated.ToString("dd/MM/yyyy"),
                    x.stdcatg.CreatedBy,
                    x.stdcatg.Description,
                    x.stdcatg.IsPublished,
                    LastUpdated = x.stdcatg.LastUpdated.HasValue ? x.stdcatg.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                    x.stdcatg.UpdatedBy

                }).ToList();


                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult StudentCategory()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveStudentCategory(tblStudentCategory addcategory)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                string Cour = addcategory.CategoryName;
                var data = db.tblStudentCategories.Select(x => new { x.CategoryName }).ToList();
                var Cours = data.Select(x => x.CategoryName);

                if (Cour != null && !Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    tblStudentCategory addStudentCategory = new tblStudentCategory();
                    addStudentCategory.CategoryName = addcategory.CategoryName;
                    addStudentCategory.Description = addcategory.Description;
                    addStudentCategory.IsPublished = addcategory.IsPublished;
                    addStudentCategory.DateCreated = DateTime.Now;
                    addStudentCategory.CreatedBy = "Admin";
                    db.tblStudentCategories.InsertOnSubmit(addStudentCategory);
                    db.SubmitChanges();
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);

                }

                else if (Cour != null && Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    ViewBag.InsertMessage = "Student Category already exists";
                    result = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("empty", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // Delete Student Category
        [HttpPost]
        public JsonResult DeleteStudentCategory(int Id)
        {
            try
            {
                db = new CourseDataContext();
                if (Id > 0)
                {
                    tblStudentCategory category = db.tblStudentCategories.Where(x => x.StudentCategoryId == Id).FirstOrDefault();
                    if (category != null)
                    {
                        db.tblStudentCategories.DeleteOnSubmit(category);
                        db.SubmitChanges();
                        return Json(Id.ToString(), JsonRequestBehavior.AllowGet);
                    }
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = new JavaScriptSerializer().Serialize(ex.Message.ToString());
                string str = ex.Message;
                string err = str.Substring(0, 61);

                if (err == "The DELETE statement conflicted with the REFERENCE constraint")
                {
                    return Json("strdepend", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ex.Message, JsonRequestBehavior.AllowGet);
                }


                //  return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // Update Course
        [HttpPost]
        public JsonResult UpdateStudentCategory(tblStudentCategory updatecategory)
        {
            try
            {
                db = new CourseDataContext();
                bool isResult = false;

                if (updatecategory.StudentCategoryId > 0)
                {
                    tblStudentCategory studentCategory = db.tblStudentCategories.Where(x => x.StudentCategoryId == updatecategory.StudentCategoryId).FirstOrDefault();
                    var data = db.tblStudentCategories.Select(x => x.CategoryName).ToList();
                    var alldata = db.tblStudentCategories.Where(x => x.StudentCategoryId == updatecategory.StudentCategoryId).Select(x => new
                    {
                        x.StudentCategoryId,
                        x.CategoryName
                    }).FirstOrDefault();

                    if (data.Contains(updatecategory.CategoryName.Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        if (updatecategory.StudentCategoryId == alldata.StudentCategoryId && updatecategory.CategoryName.ToLower() == alldata.CategoryName.ToLower())
                        {
                            studentCategory.CategoryName = updatecategory.CategoryName;
                            studentCategory.Description = updatecategory.Description;
                            studentCategory.IsPublished = updatecategory.IsPublished;
                            db.SubmitChanges();
                            isResult = true;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            isResult = false;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(updatecategory.CategoryName.Trim()))
                        {
                            studentCategory.CategoryName = updatecategory.CategoryName;
                            studentCategory.Description = updatecategory.Description;
                            studentCategory.IsPublished = updatecategory.IsPublished;
                            db.SubmitChanges();
                            isResult = true;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Student Category


        #region Bind Student Category Data by Dilshad A. on 21 Sept 2020
        public ActionResult ViewStudentCategory()
        {
            return View();
        }

        public JsonResult ViewStudentCategoryDetails()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.tblStudentCategories select d).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Bind Student Category Data by Dilshad A. on 21 Sept 2020

        #region Bind Intern Student Name By Dilshad A. on 22 Sept 2020
        [HttpPost]
        public ActionResult ViewIternStudentByInternshipId(int intIntershipId)
        {
            try
            {
                if (intIntershipId > 0)
                {
                    db = new CourseDataContext();
                    var data = (from d in db.tblInternApplies
                                join d1 in db.SITSPL_tblInternshipStructures on d.InternshipStructureId equals d1.InternStructureId
                                join d2 in db.SITSPL_tblInternships on d1.InternshipId equals d2.InternshipId
                                where d2.InternshipId == intIntershipId
                                select new
                                {
                                    d.InterApllyId,
                                    d.InternshipStructureId,
                                    d.Name,
                                    d.IsPublished,
                                    d.Email
                                }).ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Bind Intern Student Name By Dilshad A. on 22 Sept 2020

        #region GetCourseStudents
        [HttpPost]
        public JsonResult GetStudentsByCourseId(int intCourseId, int categoryid,int subcategoryid,int BatchId,int BatchGroupId)
        {
            try
            {
                db = new CourseDataContext();

                if (intCourseId > 0 && categoryid > 0 && subcategoryid >0 && BatchId >0 && BatchGroupId >0)
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
                                join subcatg in db.tblStudentSubCategories
                                on stdpro.StudentSubCategoryId equals subcatg.StudedetSubCategoryId
                                join coursetut in db.tblCourseTutors
                            on cd.CourseId equals coursetut.CourseId
                                join tut in db.SITSPL_tblTutors
                                on coursetut.TutorId equals tut.TutorId
                                where cd.CourseId == intCourseId && stdpro.StudentCategoryId == categoryid &&
                                stdpro.StudentSubCategoryId == subcategoryid && stdpro.StudentType == "CourseStudent"
                                && stdpro.BatchId == BatchId && stdpro.BatchGroupId == BatchGroupId
                                
                                select new
                                {
                                    cd.CourseId,
                                    cour.CourseName,
                                    stdpro.Email,
                                    stdpro.StudentId,
                                    stdpro.Name,
                                    stdpro.StructureId,
                                    stdpro.StudentCategoryId,
                                }).Distinct().ToList();

                    if (data != null)
                    {
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }


                }
                return Json("", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion End GetCourseStudents

        #region Brand GetCourseStudents
        [HttpPost]
        public JsonResult GetBrandStudentsByCourseId(int intBrandCourseId, int categoryid)
        {
            try
            {
                db = new CourseDataContext();

                if (intBrandCourseId > 0 && categoryid > 0)
                {
                    db = new CourseDataContext();
                    var data = (from stdpro in db.SITSPL_tblStudentProfiles
                                join cd in db.SITSPL_tblCourseDetails
                                on stdpro.StudentId equals cd.StudentId
                                //join cs in db.SITSPL_tblCourseStructures
                                //on stdpro.StructureId equals cs.StructureId
                                join cour in db.SITSPL_tblCourses
                                on cd.CourseId equals cour.CourseId
                                where cd.CourseId == intBrandCourseId && stdpro.StudentCategoryId == categoryid && stdpro.StudentType == "BrandStudent"
                                select new
                                {
                                    cd.CourseId,
                                    cour.CourseName,
                                    stdpro.Email,
                                    stdpro.StudentId,
                                    stdpro.Name,
                                    stdpro.StructureId,
                                    stdpro.StudentCategoryId,
                                }).Distinct().ToList();

                    if (data != null)
                    {
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion End Brand GetCourseStudents


        #region GetCourseTutors
        [HttpPost]
        public JsonResult GetTutorsByCourseId(int intCourseId)
        {
            try
            {
                db = new CourseDataContext();

                if (intCourseId > 0)
                {
                    db = new CourseDataContext();

                    var data = (from tut in db.SITSPL_tblTutors
                                join ctut in db.tblCourseTutors
                                on tut.TutorId equals ctut.TutorId
                                join cour in db.SITSPL_tblCourses
                                on ctut.CourseId equals cour.CourseId
                                join cs in db.SITSPL_tblCourseStructures
                                on cour.CourseId equals cs.CourseId
                                where ctut.CourseId == intCourseId
                                select new
                                {
                                    ctut.CourseId,
                                    cour.CourseName,
                                    tut.TutorEmail,
                                    tut.TutorId,
                                    tut.TutorName,
                                    ctut.CourseTutorId
                                }).Distinct().ToList();

                    if (data != null)
                    {
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }


                }
                return Json("", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion End GetCourseTutors


        [HttpPost]
        #region Get Brand Tutors 

        public JsonResult GetTutorsByCourseIdForBrand(int intBrandId)
        {
            try
            {
                db = new CourseDataContext();
                if (intBrandId > 0)
                {
                    db = new CourseDataContext();

                    var data = (from cs in db.SITSPL_tblCourseStructures
                                join tut in db.SITSPL_tblTutors
                                on cs.TutorId equals tut.TutorId
                                join cour in db.SITSPL_tblCourses
                                on cs.CourseId equals cour.CourseId
                                join dur in db.tblDurations
                                on cs.DurationId equals dur.DurationId
                                where tut.TutorType == "Brand-Tutor" && cour.CourseId == intBrandId
                                select new
                                {
                                    tut.TutorId,
                                    tut.TutorName,
                                    tut.TutorEmail,
                                    cour.CourseName,
                                    cour.CourseId,
                                    dur.DurationName,
                                    cs.StructureId
                                }).ToList();

                    if (data != null)
                    {
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }


                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion End Get Brand Tutors

        #region GetInternTutors
        [HttpPost]
        public JsonResult GetTutorsByInternshipId(int intInternshipId)
        {
            try
            {
                db = new CourseDataContext();

                if (intInternshipId > 0)
                {
                    db = new CourseDataContext();

                    var data = (from tut in db.SITSPL_tblTutors
                                join itut in db.tblInternTutors
                                on tut.TutorId equals itut.TutorId
                                join intr in db.SITSPL_tblInternships
                                on itut.InternshipId equals intr.InternshipId
                                join instr in db.SITSPL_tblInternshipStructures
                                on intr.InternshipId equals instr.InternshipId
                                where itut.InternshipId == intInternshipId
                                select new
                                {
                                    itut.InternshipId,
                                    intr.InternshipName,
                                    tut.TutorEmail,
                                    tut.TutorId,
                                    tut.TutorName,
                                    itut.InternTutorId
                                }).Distinct().ToList();

                    if (data != null)
                    {
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }


                }
                return Json("", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion End GetInternTutors



        #region Get Tutors By Internship For Coomunication by Dilshad A. on 22 sept 2020
        [HttpPost]
        public JsonResult GetInternTutorByIntershipId(int intInternshipId)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.SITSPL_tblInternships
                            join inttu in db.tblInternTutors on d.InternshipId equals inttu.InternshipId
                            join tut in db.SITSPL_tblTutors on inttu.TutorId equals tut.TutorId
                            where inttu.InternshipId == intInternshipId
                            select new
                            {
                                inttu.InternshipId,
                                inttu.InternTutorId,
                                d.InternshipName,
                                inttu.TutorId,
                                tut.TutorName,
                                tut.TutorEmail
                            }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Get Tutors By Internship For Coomunication by Dilshad A. on 22 sept 2020

        #region Bind StudentName Based on CourseId For Communication by Dilshad A. on 25 Sept 2020
        [HttpPost]
        public JsonResult GetStudentDetailsByCourseId(int intCourseId)
        {
            try
            {
                if (intCourseId > 0)
                {
                    db = new CourseDataContext();
                    var data = (from d in db.SITSPL_tblStudentProfiles
                                join d1 in db.SITSPL_tblCourseDetails on d.StudentId equals d1.StudentId
                                select new
                                {
                                    d.StudentId,
                                    d.Name,
                                    d1.CourseId,
                                    d.IsPublished
                                }).ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Bind StudentName Based on CourseId For Communication by Dilshad A. on 25 Sept 2020

        #region Add/Sent message(Communication) and mail to Students/Tutors and also added to his dashboard By Dilshad A. on 14 Sept 2020

        [HttpPost]
        public JsonResult AddMessageForCommunicatioin(List<StudentTutorCommunication> lstStudent, List<StudentTutorCommunication> lstTutor,
            bool isAllStudents, bool isAllTutors, string strMsgStudent, string strMsgTutor, string strMsgAll, string CategoryType, int? StudentCategoryId, int? InternshipId,
            int? CourseId, bool IsAllInternStudent, bool IsAllCourseStudent, bool IsAllInternTutor, bool IsAllCourseTutor, List<StudentTutorCommunication> lstCourseStudent, List<StudentTutorCommunication> lstBrandStudent, bool IsAllBrandStudent,int? SubCategoryId,
            int? BatchId,int? BatchGroupId)
        {

            try
            {
                db = new CourseDataContext();
                tblCommunication objComm = null;
                // For All Students/Tutors(Intern/Course)
                ArrayList alAllEmail = new ArrayList();
                //if(CategoryType != "") { 
                //string CatgType = CategoryType.Trim();
                //}

                #region 3 All
                #region All Course Students && All Intern Students && All Brand Students

                if (IsAllCourseStudent && IsAllInternStudent && IsAllBrandStudent && CategoryType == null || CategoryType == "")
                {
                    objComm = new tblCommunication();
                    // Course Student List

                    #region All Course Student
                    if (IsAllCourseStudent == true)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                           join cd in db.SITSPL_tblCourseDetails
                                           on stdpro.StudentId equals cd.StudentId
                                           //join cs in db.SITSPL_tblCourseStructures
                                           //on stdpro.StructureId equals cs.StructureId
                                           join cour in db.SITSPL_tblCourses
                                           on cd.CourseId equals cour.CourseId

                                           where stdpro.StudentType == "CourseStudent"
                                           select new
                                           {
                                               // cd.CourseId,
                                               stdpro.Email,
                                               stdpro.StudentId,
                                               //  stdpro.StudentCategoryId,
                                           }).Distinct().ToList();
                        if (AllStudents != null)
                        {
                            for (int i = 0; i < AllStudents.Count; i++)
                            {
                                alEmail.Add(AllStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                //         objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                //         objComm.CourseId = AllStudents[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }
                    }
                    #endregion End All Course Student

                    #region All Intern Student
                    if (IsAllInternStudent == true)
                    {
                        objComm = new tblCommunication();
                        ArrayList alEmail = new ArrayList();
                        var AllInternStudents = (from intnapply in db.tblInternApplies
                                                 join intnstr in db.SITSPL_tblInternshipStructures
                                                 on intnapply.InternshipStructureId equals intnstr.InternStructureId
                                                 join intn in db.SITSPL_tblInternships
                                                 on intnstr.InternshipId equals intn.InternshipId
                                                 join dur in db.tblDurations
                                                 on intnstr.DurationId equals dur.DurationId
                                                 select new
                                                 {
                                                     intnapply.InterApllyId,
                                                     //     intnapply.Name,
                                                     intnapply.Email,
                                                     //     intnapply.InternshipStructureId,
                                                     //     intnstr.InternshipId,
                                                     //     intn.InternshipName
                                                 }).Distinct().ToList();

                        if (AllInternStudents != null)
                        {
                            for (int i = 0; i < AllInternStudents.Count; i++)
                            {
                                alEmail.Add(AllInternStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllInternStudents[i].InterApllyId;
                                objComm.UserType = "All-Intern-Students";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Intern";
                                //     objComm.InternshipId = AllInternStudents[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }
                        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End All Intern Student

                    #region All Brand Students
                    if (IsAllBrandStudent == true)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllBrandStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                                join cd in db.SITSPL_tblCourseDetails
                                                on stdpro.StudentId equals cd.StudentId
                                                //join cs in db.SITSPL_tblCourseStructures
                                                //on stdpro.StructureId equals cs.StructureId
                                                join cour in db.SITSPL_tblCourses
                                                on cd.CourseId equals cour.CourseId
                                                where stdpro.StudentType == "BrandStudent"
                                                select new
                                                {
                                                    //  cs.CourseId,
                                                    stdpro.Email,
                                                    stdpro.StudentId,
                                                    //    stdpro.StudentCategoryId,
                                                }).Distinct().ToList();
                        if (AllBrandStudents != null)
                        {
                            for (int i = 0; i < AllBrandStudents.Count; i++)
                            {
                                alEmail.Add(AllBrandStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllBrandStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Course";
                                //      objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                //      objComm.CourseId = AllStudents[i].CourseId;
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
                    #endregion End All Brand Students
                }

                #endregion End All Course Students & All Intern Students && All Brand Students
                #endregion End 3 All

                #region Message To Selected Students no checkbox tick
                #region Message to Selected Course Students
                if (IsAllCourseStudent == false && IsAllInternStudent == false && IsAllBrandStudent == false && CategoryType == "Course")
                {
                    objComm = new tblCommunication();
                    if (lstCourseStudent != null)
                    {
                        ArrayList alEmail = new ArrayList();
                        for (int i = 0; i < lstCourseStudent.Count; i++)
                        {
                            alEmail.Add(lstCourseStudent[i].Email);
                            objComm = new tblCommunication();
                            objComm.IsAllCourseStudent = IsAllCourseStudent;
                            objComm.IsAllInternStudent = IsAllInternStudent;
                            objComm.IsAllCourseTutor = IsAllCourseTutor;
                            objComm.IsAllInternTutor = IsAllInternTutor;
                            objComm.Id = lstCourseStudent[i].StudentId;
                            objComm.UserType = "All-Students-Course";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.Message = strMsgStudent;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.MessageAll = strMsgAll;
                            objComm.CategoryType = "Course";

                            if(SubCategoryId != null)
                            { 
                            objComm.StudentSubCategoryId = SubCategoryId;
                            }

                            if(StudentCategoryId != null)
                            {
                                objComm.StudentCategoryId = StudentCategoryId;
                            }

                            //if(BatchId != null)
                            //{
                            //    objComm.BatchId = BatchId;
                            //}

                            //if(BatchGroupId != null)
                            //{
                            //    objComm.BatchGroupId = BatchGroupId;
                            //}

                            //    objComm.StudentCategoryId = StudentCategoryId;
                            //    objComm.CourseId = CourseId;
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
                #endregion End Message to Selected Course Students

                #region Message To Selected Intern Students
                if (IsAllInternStudent == false && !IsAllCourseStudent && !IsAllBrandStudent && CategoryType == "Intern")
                {
                    // Intern Student List
                    if (lstStudent != null)
                    {
                        ArrayList alEmail = new ArrayList();
                        for (int i = 0; i < lstStudent.Count; i++)
                        {
                            alEmail.Add(lstStudent[i].Email);
                            objComm = new tblCommunication();
                            objComm.IsAllCourseStudent = IsAllCourseStudent;
                            objComm.IsAllInternStudent = IsAllInternStudent;
                            objComm.IsAllCourseTutor = IsAllCourseTutor;
                            objComm.IsAllInternTutor = IsAllInternTutor;
                            objComm.IsAllTeacher = isAllTutors;
                            objComm.Id = lstStudent[i].StudentId;
                            objComm.UserType = "All-Intern-Students";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.Message = strMsgStudent;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.MessageAll = strMsgAll;
                            objComm.CategoryType = "Intern";
                            objComm.InternshipId = InternshipId;
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
                #endregion End Message To Selected Intern Students

                #region Message To Selected Brand Students
                if (IsAllInternStudent == false && !IsAllCourseStudent && !IsAllBrandStudent && CategoryType == "Brand")
                {
                    if (lstBrandStudent != null)
                    {
                        ArrayList alEmail = new ArrayList();
                        for (int i = 0; i < lstBrandStudent.Count; i++)
                        {
                            alEmail.Add(lstBrandStudent[i].Email);
                            objComm = new tblCommunication();
                            objComm.IsAllCourseStudent = IsAllCourseStudent;
                            objComm.IsAllInternStudent = IsAllInternStudent;
                            objComm.IsAllCourseTutor = IsAllCourseTutor;
                            objComm.IsAllInternTutor = IsAllInternTutor;
                            objComm.IsAllTeacher = isAllTutors;
                            objComm.Id = lstBrandStudent[i].StudentId;
                            objComm.UserType = "All-Students-Course";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.Message = strMsgStudent;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            //       objComm.MessageAll = strMsgAll;
                            objComm.CategoryType = "Course";
                            objComm.CourseId = CourseId;
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
                #endregion End Message To Selected Brand Students
                #endregion End Message To Selected Students no checkbox tick

                #region 1 All
                #region All Course Students Comm
                if (IsAllCourseStudent && !IsAllInternStudent && !IsAllBrandStudent && CategoryType == null || CategoryType == "")
                {
                    if (IsAllCourseStudent == true && lstCourseStudent != null || lstCourseStudent == null)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                           join cd in db.SITSPL_tblCourseDetails
                                           on stdpro.StudentId equals cd.StudentId
                                           //join cs in db.SITSPL_tblCourseStructures
                                           //on stdpro.StructureId equals cs.StructureId
                                           join cour in db.SITSPL_tblCourses
                                           on cd.CourseId equals cour.CourseId
                                           where stdpro.StudentType == "CourseStudent"
                                           select new
                                           {
                                               //  cs.CourseId,
                                               stdpro.Email,
                                               stdpro.StudentId,
                                               //    stdpro.StudentCategoryId,
                                           }).Distinct().ToList();
                        if (AllStudents != null)
                        {
                            for (int i = 0; i < AllStudents.Count; i++)
                            {
                                alEmail.Add(AllStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Course";
                                //      objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                //      objComm.CourseId = AllStudents[i].CourseId;
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
                }
                #endregion End All Course Students

                #region All Intern Students Comm
                if (IsAllInternStudent && !IsAllCourseStudent && !IsAllBrandStudent && CategoryType == null || CategoryType == "")
                {

                    if (IsAllInternStudent == true)
                    {
                        var AllInterns = (from d in db.tblInternApplies
                                          join d1 in db.SITSPL_tblInternshipStructures on d.InternshipStructureId equals d1.InternStructureId
                                          join d2 in db.SITSPL_tblInternships on d1.InternshipId equals d2.InternshipId
                                          join dur in db.tblDurations
                                          on d1.DurationId equals dur.DurationId
                                          select new
                                          {
                                              d.InterApllyId,
                                              //      d.InternshipStructureId,
                                              //   d.Name,
                                              //       d1.InternshipId,
                                              //       d.IsPublished,
                                              d.Email
                                          }).ToList();

                        ArrayList alEmail = new ArrayList();
                        for (int i = 0; i < AllInterns.Count; i++)
                        {
                            alEmail.Add(AllInterns[i].Email);
                            objComm = new tblCommunication();
                            objComm.IsAllCourseStudent = IsAllCourseStudent;
                            objComm.IsAllInternStudent = IsAllInternStudent;
                            objComm.IsAllCourseTutor = IsAllCourseTutor;
                            objComm.IsAllInternTutor = IsAllInternTutor;
                            objComm.IsAllTeacher = isAllTutors;
                            objComm.Id = AllInterns[i].InterApllyId;
                            objComm.UserType = "All-Intern-Students";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.Message = strMsgStudent;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.MessageAll = strMsgAll;
                            //objComm.CategoryType = CategoryType;
                            objComm.CategoryType = "Intern";
                            //   objComm.InternshipId = AllInterns[i].InternshipId;
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
                #endregion End All Intern Students

                #region All Brand Students Comm

                if (IsAllBrandStudent && !IsAllInternStudent && !IsAllCourseStudent && CategoryType == null || CategoryType == "")
                {
                    if (IsAllBrandStudent == true && lstBrandStudent != null || lstBrandStudent == null)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllBrandStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                                join cd in db.SITSPL_tblCourseDetails
                                                on stdpro.StudentId equals cd.StudentId
                                                //join cs in db.SITSPL_tblCourseStructures
                                                //on stdpro.StructureId equals cs.StructureId
                                                join cour in db.SITSPL_tblCourses
                                                on cd.CourseId equals cour.CourseId
                                                where stdpro.StudentType == "BrandStudent"
                                                select new
                                                {
                                                    //  cs.CourseId,
                                                    stdpro.Email,
                                                    stdpro.StudentId,
                                                    //    stdpro.StudentCategoryId,
                                                }).Distinct().ToList();
                        if (AllBrandStudents != null)
                        {
                            for (int i = 0; i < AllBrandStudents.Count; i++)
                            {
                                alEmail.Add(AllBrandStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllBrandStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Course";
                                //      objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                //      objComm.CourseId = AllStudents[i].CourseId;
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
                }

                #endregion End All Brand Students Comm

                #endregion End 1 All


                #region 2 All
                #region All Course Students & All Intern Students
                if (IsAllCourseStudent && IsAllInternStudent & !IsAllBrandStudent && CategoryType == null || CategoryType == "")
                {
                    if (IsAllCourseStudent == true)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                           join cd in db.SITSPL_tblCourseDetails
                                           on stdpro.StudentId equals cd.StudentId
                                           //join cs in db.SITSPL_tblCourseStructures
                                           //on stdpro.StructureId equals cs.StructureId
                                           join cour in db.SITSPL_tblCourses
                                           on cd.CourseId equals cour.CourseId

                                           where stdpro.StudentType == "CourseStudent"
                                           select new
                                           {
                                               // cd.CourseId,
                                               stdpro.Email,
                                               stdpro.StudentId,
                                               //  stdpro.StudentCategoryId,
                                           }).Distinct().ToList();
                        if (AllStudents != null)
                        {
                            for (int i = 0; i < AllStudents.Count; i++)
                            {
                                alEmail.Add(AllStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                //         objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                //         objComm.CourseId = AllStudents[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }
                    }


                    if (IsAllInternStudent == true)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllInternStudents = (from intnapply in db.tblInternApplies
                                                 join intnstr in db.SITSPL_tblInternshipStructures
                                                 on intnapply.InternshipStructureId equals intnstr.InternStructureId
                                                 join intn in db.SITSPL_tblInternships
                                                 on intnstr.InternshipId equals intn.InternshipId
                                                 join dur in db.tblDurations
                                                 on intnstr.DurationId equals dur.DurationId
                                                 select new
                                                 {
                                                     intnapply.InterApllyId,
                                                     //     intnapply.Name,
                                                     intnapply.Email,
                                                     //     intnapply.InternshipStructureId,
                                                     //     intnstr.InternshipId,
                                                     //     intn.InternshipName
                                                 }).Distinct().ToList();

                        if (AllInternStudents != null)
                        {
                            for (int i = 0; i < AllInternStudents.Count; i++)
                            {
                                alEmail.Add(AllInternStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllInternStudents[i].InterApllyId;
                                objComm.UserType = "All-Intern-Students";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Intern";
                                //     objComm.InternshipId = AllInternStudents[i].InternshipId;
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
                }
                #endregion End All Course Students & All Intern Students

                #region All Intern Students && All Brand Students

                if (!IsAllCourseStudent && IsAllInternStudent & IsAllBrandStudent && CategoryType == null || CategoryType == "")
                {
                    #region All Intern Comm
                    if (IsAllInternStudent == true)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllInternStudents = (from intnapply in db.tblInternApplies
                                                 join intnstr in db.SITSPL_tblInternshipStructures
                                                 on intnapply.InternshipStructureId equals intnstr.InternStructureId
                                                 join intn in db.SITSPL_tblInternships
                                                 on intnstr.InternshipId equals intn.InternshipId
                                                 join dur in db.tblDurations
                                                 on intnstr.DurationId equals dur.DurationId
                                                 select new
                                                 {
                                                     intnapply.InterApllyId,
                                                     //     intnapply.Name,
                                                     intnapply.Email,
                                                     //     intnapply.InternshipStructureId,
                                                     //     intnstr.InternshipId,
                                                     //     intn.InternshipName
                                                 }).Distinct().ToList();

                        if (AllInternStudents != null)
                        {
                            for (int i = 0; i < AllInternStudents.Count; i++)
                            {
                                alEmail.Add(AllInternStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllInternStudents[i].InterApllyId;
                                objComm.UserType = "All-Intern-Students";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Intern";
                                //     objComm.InternshipId = AllInternStudents[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }
                        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End All Intern Comm

                    #region All Brand Comm
                    if (IsAllBrandStudent == true)
                    {
                        if (lstBrandStudent != null || lstBrandStudent == null)
                        {
                            ArrayList alEmail = new ArrayList();
                            var AllBrandStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                                    join cd in db.SITSPL_tblCourseDetails
                                                    on stdpro.StudentId equals cd.StudentId
                                                    //join cs in db.SITSPL_tblCourseStructures
                                                    //on stdpro.StructureId equals cs.StructureId
                                                    join cour in db.SITSPL_tblCourses
                                                    on cd.CourseId equals cour.CourseId
                                                    where stdpro.StudentType == "BrandStudent"
                                                    select new
                                                    {
                                                        //  cs.CourseId,
                                                        stdpro.Email,
                                                        stdpro.StudentId,
                                                        //    stdpro.StudentCategoryId,
                                                    }).Distinct().ToList();
                            if (AllBrandStudents != null)
                            {
                                for (int i = 0; i < AllBrandStudents.Count; i++)
                                {
                                    alEmail.Add(AllBrandStudents[i].Email);
                                    objComm = new tblCommunication();
                                    objComm.IsAllCourseStudent = IsAllCourseStudent;
                                    objComm.IsAllInternStudent = IsAllInternStudent;
                                    objComm.IsAllCourseTutor = IsAllCourseTutor;
                                    objComm.IsAllInternTutor = IsAllInternTutor;
                                    objComm.Id = AllBrandStudents[i].StudentId;
                                    objComm.UserType = "All-Students-Course";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgStudent;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";
                                    objComm.MessageAll = strMsgAll;
                                    //objComm.CategoryType = CategoryType;
                                    objComm.CategoryType = "Course";
                                    //      objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                    //      objComm.CourseId = AllStudents[i].CourseId;
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
                    }



                    #endregion End All Brand Comm
                }
                #endregion End All Intern Students && All Brand Students

                #region All Brand Students && All Course Students

                if (IsAllBrandStudent && !IsAllInternStudent && IsAllCourseStudent && CategoryType == null || CategoryType == "")
                {
                    #region All Brand Students

                    if (IsAllBrandStudent == true && lstBrandStudent != null || lstBrandStudent == null)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllBrandStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                                join cd in db.SITSPL_tblCourseDetails
                                                on stdpro.StudentId equals cd.StudentId
                                                //join cs in db.SITSPL_tblCourseStructures
                                                //on stdpro.StructureId equals cs.StructureId
                                                join cour in db.SITSPL_tblCourses
                                                on cd.CourseId equals cour.CourseId
                                                where stdpro.StudentType == "BrandStudent"
                                                select new
                                                {
                                                    //  cs.CourseId,
                                                    stdpro.Email,
                                                    stdpro.StudentId,
                                                    //    stdpro.StudentCategoryId,
                                                }).Distinct().ToList();
                        if (AllBrandStudents != null)
                        {
                            for (int i = 0; i < AllBrandStudents.Count; i++)
                            {
                                alEmail.Add(AllBrandStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllBrandStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Course";
                                //      objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                //      objComm.CourseId = AllStudents[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }
                        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }

                    #endregion End All Brand Students

                    #region All Course Students
                    if (IsAllCourseStudent == true)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                           join cd in db.SITSPL_tblCourseDetails
                                           on stdpro.StudentId equals cd.StudentId
                                           //join cs in db.SITSPL_tblCourseStructures
                                           //on stdpro.StructureId equals cs.StructureId
                                           join cour in db.SITSPL_tblCourses
                                           on cd.CourseId equals cour.CourseId

                                           where stdpro.StudentType == "CourseStudent"
                                           select new
                                           {
                                               // cd.CourseId,
                                               stdpro.Email,
                                               stdpro.StudentId,
                                               //  stdpro.StudentCategoryId,
                                           }).Distinct().ToList();
                        if (AllStudents != null)
                        {
                            for (int i = 0; i < AllStudents.Count; i++)
                            {
                                alEmail.Add(AllStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                //         objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                //         objComm.CourseId = AllStudents[i].CourseId;
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
                    #endregion End All Course Students
                }
                #endregion End All Brand Students && All Course Students
                #endregion End 2 All


                #region 1 All Combo with 1 each alternate

                #region Message To All Course Student and Intern Student (Selected Intern Student)
                if (IsAllCourseStudent == true && !IsAllInternStudent && !IsAllBrandStudent && CategoryType == "Intern")
                {
                    #region All Course Student
                    if (IsAllCourseStudent == true)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                           join cd in db.SITSPL_tblCourseDetails
                                           on stdpro.StudentId equals cd.StudentId
                                           //join cs in db.SITSPL_tblCourseStructures
                                           //on stdpro.StructureId equals cs.StructureId
                                           join cour in db.SITSPL_tblCourses
                                           on cd.CourseId equals cour.CourseId
                                           where stdpro.StudentType == "CourseStudent"
                                           select new
                                           {
                                               // cd.CourseId,
                                               stdpro.Email,
                                               stdpro.StudentId,
                                               //  stdpro.StudentCategoryId,
                                           }).Distinct().ToList();
                        if (AllStudents != null)
                        {
                            for (int i = 0; i < AllStudents.Count; i++)
                            {
                                alEmail.Add(AllStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                //     objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                //     objComm.CourseId = AllStudents[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }
                    }
                    #endregion End All Course Student

                    #region Selected Intern Student
                    if (CategoryType == "Intern")
                    {
                        if (lstStudent != null)
                        {
                            ArrayList alEmail = new ArrayList();
                            for (int i = 0; i < lstStudent.Count; i++)
                            {
                                alEmail.Add(lstStudent[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.IsAllTeacher = isAllTutors;
                                objComm.Id = lstStudent[i].StudentId;
                                objComm.UserType = "All-Intern-Students";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Intern";
                                objComm.InternshipId = InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                            //return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End Selected Intern Student
                }
                #endregion Message To All Course Student and Intern Student (Selected Intern Student)

                #region Message To All Course Student and Brand Student (Selected Brand Student)

                if (IsAllCourseStudent == true && !IsAllInternStudent && !IsAllBrandStudent && CategoryType == "Brand")
                {
                    #region All Course Student
                    if (IsAllCourseStudent == true)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                           join cd in db.SITSPL_tblCourseDetails
                                           on stdpro.StudentId equals cd.StudentId
                                           //join cs in db.SITSPL_tblCourseStructures
                                           //on stdpro.StructureId equals cs.StructureId
                                           join cour in db.SITSPL_tblCourses
                                           on cd.CourseId equals cour.CourseId
                                           where stdpro.StudentType == "CourseStudent"
                                           select new
                                           {
                                               // cd.CourseId,
                                               stdpro.Email,
                                               stdpro.StudentId,
                                               //  stdpro.StudentCategoryId,
                                           }).Distinct().ToList();
                        if (AllStudents != null)
                        {
                            for (int i = 0; i < AllStudents.Count; i++)
                            {
                                alEmail.Add(AllStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                //     objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                //     objComm.CourseId = AllStudents[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }
                    }

                    #endregion End All Course Student

                    #region Selected Brand Student
                    if (CategoryType == "Brand")
                    {
                        ArrayList alEmail = new ArrayList();
                        if (lstBrandStudent != null)
                        {
                            for (int i = 0; i < lstBrandStudent.Count; i++)
                            {
                                alEmail.Add(lstBrandStudent[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.IsAllTeacher = isAllTutors;
                                objComm.Id = lstBrandStudent[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                //       objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                objComm.CourseId = CourseId;
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
                    #endregion End Selected Brand Student
                }
                #endregion Message To All Course Student and Brand Student (Selected Brand Student)


                #region Message To All Intern Student and  Course Student (Selected Course Student)
                if (IsAllInternStudent == true && !IsAllCourseStudent && !IsAllBrandStudent && CategoryType == "Course")
                {
                    #region All Intern Students
                    if (IsAllInternStudent == true)
                    {
                        var AllInterns = (from d in db.tblInternApplies
                                          join d1 in db.SITSPL_tblInternshipStructures on d.InternshipStructureId equals d1.InternStructureId
                                          join d2 in db.SITSPL_tblInternships on d1.InternshipId equals d2.InternshipId
                                          join dur in db.tblDurations
                                          on d1.DurationId equals dur.DurationId
                                          select new
                                          {
                                              d.InterApllyId,
                                              //     d.InternshipStructureId,
                                              //     d.Name,
                                              //     d1.InternshipId,
                                              //     d.IsPublished,
                                              d.Email
                                          }).ToList();

                        ArrayList alEmail = new ArrayList();
                        for (int i = 0; i < AllInterns.Count; i++)
                        {
                            alEmail.Add(AllInterns[i].Email);
                            objComm = new tblCommunication();
                            objComm.IsAllCourseStudent = IsAllCourseStudent;
                            objComm.IsAllInternStudent = IsAllInternStudent;
                            objComm.IsAllCourseTutor = IsAllCourseTutor;
                            objComm.IsAllInternTutor = IsAllInternTutor;
                            objComm.IsAllTeacher = isAllTutors;
                            objComm.Id = AllInterns[i].InterApllyId;
                            objComm.UserType = "All-Intern-Students";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.Message = strMsgStudent;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.MessageAll = strMsgAll;
                            objComm.CategoryType = "Intern";
                            //         objComm.InternshipId = AllInterns[i].InternshipId;
                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }
                        if (alEmail.Count > 0)
                        {
                            SendMailToAll(alEmail);
                        }
                    }
                    #endregion End All Intern Students

                    #region Selected Course Student
                    if (CategoryType == "Course" && lstCourseStudent != null || lstCourseStudent == null)
                    {
                        if (lstCourseStudent != null)
                        {
                            ArrayList alEmail = new ArrayList();
                            for (int i = 0; i < lstCourseStudent.Count; i++)
                            {
                                alEmail.Add(lstCourseStudent[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = lstCourseStudent[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                //         objComm.StudentCategoryId = StudentCategoryId;
                                //         objComm.CourseId = CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                            //return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End Selected Course Student

                }
                #endregion End Message To All Intern Student and Category Course (Selected Course Student)

                #region Message To All Intern Student and  Brand Student (Selected Brand Student)
                if (IsAllInternStudent == true && !IsAllCourseStudent && !IsAllBrandStudent && CategoryType == "Brand")
                {
                    #region All Intern Students
                    if (IsAllInternStudent == true)
                    {
                        var AllInterns = (from d in db.tblInternApplies
                                          join d1 in db.SITSPL_tblInternshipStructures on d.InternshipStructureId equals d1.InternStructureId
                                          join d2 in db.SITSPL_tblInternships on d1.InternshipId equals d2.InternshipId
                                          join dur in db.tblDurations
                                          on d1.DurationId equals dur.DurationId
                                          select new
                                          {
                                              d.InterApllyId,
                                              //     d.InternshipStructureId,
                                              //     d.Name,
                                              //     d1.InternshipId,
                                              //     d.IsPublished,
                                              d.Email
                                          }).ToList();

                        ArrayList alEmail = new ArrayList();
                        for (int i = 0; i < AllInterns.Count; i++)
                        {
                            alEmail.Add(AllInterns[i].Email);
                            objComm = new tblCommunication();
                            objComm.IsAllCourseStudent = IsAllCourseStudent;
                            objComm.IsAllInternStudent = IsAllInternStudent;
                            objComm.IsAllCourseTutor = IsAllCourseTutor;
                            objComm.IsAllInternTutor = IsAllInternTutor;
                            objComm.IsAllTeacher = isAllTutors;
                            objComm.Id = AllInterns[i].InterApllyId;
                            objComm.UserType = "All-Intern-Students";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.Message = strMsgStudent;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.MessageAll = strMsgAll;
                            objComm.CategoryType = "Intern";
                            //         objComm.InternshipId = AllInterns[i].InternshipId;
                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }
                        if (alEmail.Count > 0)
                        {
                            SendMailToAll(alEmail);
                        }

                    }
                    #endregion End All Intern Students

                    #region Selected Brand Student
                    if (CategoryType == "Brand")
                    {
                        if (lstBrandStudent != null)
                        {
                            ArrayList alEmail = new ArrayList();
                            for (int i = 0; i < lstBrandStudent.Count; i++)
                            {
                                alEmail.Add(lstBrandStudent[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.IsAllTeacher = isAllTutors;
                                objComm.Id = lstBrandStudent[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                //       objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                objComm.CourseId = CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                            //return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End Selected Brand Student

                }
                #endregion End Message To All Intern Student and Brand Student (Selected Brand Student)


                #region Message To All Brand Student and  Course Student (Selected Course Student)
                if (IsAllBrandStudent == true && !IsAllCourseStudent && !IsAllInternStudent && CategoryType == "Course")
                {
                    #region All Brand Students
                    if (IsAllBrandStudent == true)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllBrandStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                                join cd in db.SITSPL_tblCourseDetails
                                                on stdpro.StudentId equals cd.StudentId
                                                //join cs in db.SITSPL_tblCourseStructures
                                                //on stdpro.StructureId equals cs.StructureId
                                                join cour in db.SITSPL_tblCourses
                                                on cd.CourseId equals cour.CourseId
                                                where stdpro.StudentType == "BrandStudent"
                                                select new
                                                {
                                                    //  cs.CourseId,
                                                    stdpro.Email,
                                                    stdpro.StudentId,
                                                    //    stdpro.StudentCategoryId,
                                                }).Distinct().ToList();
                        if (AllBrandStudents != null)
                        {
                            for (int i = 0; i < AllBrandStudents.Count; i++)
                            {
                                alEmail.Add(AllBrandStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllBrandStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Course";
                                //      objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                //      objComm.CourseId = AllStudents[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }
                        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End All Brand Students

                    #region Selected Course Student
                    if (CategoryType == "Course")
                    {
                        if (lstCourseStudent != null)
                        {
                            ArrayList alEmail = new ArrayList();
                            for (int i = 0; i < lstCourseStudent.Count; i++)
                            {
                                alEmail.Add(lstCourseStudent[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = lstCourseStudent[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                //    objComm.StudentCategoryId = StudentCategoryId;
                                //    objComm.CourseId = CourseId;
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
                    #endregion End Selected Course Student

                }
                #endregion End Message To All Brand Student and Course Student (Selected Course Student)


                #region Message To All Brand Student and  Intern Student (Selected Intern Student)
                if (IsAllBrandStudent == true && !IsAllCourseStudent && !IsAllInternStudent && CategoryType == "Intern")
                {
                    #region All Brand Students
                    if (IsAllBrandStudent == true)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllBrandStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                                join cd in db.SITSPL_tblCourseDetails
                                                on stdpro.StudentId equals cd.StudentId
                                                //join cs in db.SITSPL_tblCourseStructures
                                                //on stdpro.StructureId equals cs.StructureId
                                                join cour in db.SITSPL_tblCourses
                                                on cd.CourseId equals cour.CourseId
                                                where stdpro.StudentType == "BrandStudent"
                                                select new
                                                {
                                                    //  cs.CourseId,
                                                    stdpro.Email,
                                                    stdpro.StudentId,
                                                    //    stdpro.StudentCategoryId,
                                                }).Distinct().ToList();
                        if (AllBrandStudents != null)
                        {
                            for (int i = 0; i < AllBrandStudents.Count; i++)
                            {
                                alEmail.Add(AllBrandStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllBrandStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Course";
                                //      objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                //      objComm.CourseId = AllStudents[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }
                        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End All Brand Students

                    #region Selected Intern Student
                    if (CategoryType == "Intern")
                    {
                        if (lstStudent != null)
                        {
                            ArrayList alEmail = new ArrayList();
                            for (int i = 0; i < lstStudent.Count; i++)
                            {
                                alEmail.Add(lstStudent[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.IsAllTeacher = isAllTutors;
                                objComm.Id = lstStudent[i].StudentId;
                                objComm.UserType = "All-Intern-Students";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Intern";
                                objComm.InternshipId = InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                            // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End Selected Course Student

                }
                #endregion End Message To All Brand Student and Intern Student (Selected Intern Student)

                #endregion 1 All Combo with 1 each alternate


                #region 2 All 1 Selected
                #region All Course Students && All Intern Students && Selected Brand Students

                if (IsAllCourseStudent && IsAllInternStudent & !IsAllBrandStudent && CategoryType == "Brand")
                {
                    if (IsAllCourseStudent == true)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                           join cd in db.SITSPL_tblCourseDetails
                                           on stdpro.StudentId equals cd.StudentId
                                           //join cs in db.SITSPL_tblCourseStructures
                                           //on stdpro.StructureId equals cs.StructureId
                                           join cour in db.SITSPL_tblCourses
                                           on cd.CourseId equals cour.CourseId

                                           where stdpro.StudentType == "CourseStudent"
                                           select new
                                           {
                                               // cd.CourseId,
                                               stdpro.Email,
                                               stdpro.StudentId,
                                               //  stdpro.StudentCategoryId,
                                           }).Distinct().ToList();
                        if (AllStudents != null)
                        {
                            for (int i = 0; i < AllStudents.Count; i++)
                            {
                                alEmail.Add(AllStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                //         objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                //         objComm.CourseId = AllStudents[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }
                    }
                    if (IsAllInternStudent == true)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllInternStudents = (from intnapply in db.tblInternApplies
                                                 join intnstr in db.SITSPL_tblInternshipStructures
                                                 on intnapply.InternshipStructureId equals intnstr.InternStructureId
                                                 join intn in db.SITSPL_tblInternships
                                                 on intnstr.InternshipId equals intn.InternshipId
                                                 join dur in db.tblDurations
                                                 on intnstr.DurationId equals dur.DurationId
                                                 select new
                                                 {
                                                     intnapply.InterApllyId,
                                                     //     intnapply.Name,
                                                     intnapply.Email,
                                                     //     intnapply.InternshipStructureId,
                                                     //     intnstr.InternshipId,
                                                     //     intn.InternshipName
                                                 }).Distinct().ToList();

                        if (AllInternStudents != null)
                        {
                            for (int i = 0; i < AllInternStudents.Count; i++)
                            {
                                alEmail.Add(AllInternStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllInternStudents[i].InterApllyId;
                                objComm.UserType = "All-Intern-Students";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Intern";
                                //     objComm.InternshipId = AllInternStudents[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }
                        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }

                    #region Selected Brand Studnets
                    if (!IsAllBrandStudent && CategoryType == "Brand" && lstBrandStudent != null || lstBrandStudent == null)
                    {
                        if (lstBrandStudent != null)
                        {
                            ArrayList alEmail = new ArrayList();
                            for (int i = 0; i < lstBrandStudent.Count; i++)
                            {
                                alEmail.Add(lstBrandStudent[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.IsAllTeacher = isAllTutors;
                                objComm.Id = lstBrandStudent[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                //       objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                objComm.CourseId = CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                            //return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End Selected Brand Students
                }

                #endregion End All Course Students && All Intern Students && Selected Brand Students 

                #region All Intern Students && All Brand Students && Selected Course Students

                if (!IsAllCourseStudent && IsAllInternStudent & IsAllBrandStudent && CategoryType == "Course")
                {
                    #region All Intern Comm
                    if (IsAllInternStudent == true)
                    {

                        ArrayList alEmail = new ArrayList();
                        var AllInternStudents = (from intnapply in db.tblInternApplies
                                                 join intnstr in db.SITSPL_tblInternshipStructures
                                                 on intnapply.InternshipStructureId equals intnstr.InternStructureId
                                                 join intn in db.SITSPL_tblInternships
                                                 on intnstr.InternshipId equals intn.InternshipId
                                                 join dur in db.tblDurations
                                                 on intnstr.DurationId equals dur.DurationId
                                                 select new
                                                 {
                                                     intnapply.InterApllyId,
                                                     //     intnapply.Name,
                                                     intnapply.Email,
                                                     //     intnapply.InternshipStructureId,
                                                     //     intnstr.InternshipId,
                                                     //     intn.InternshipName
                                                 }).Distinct().ToList();

                        if (AllInternStudents != null)
                        {
                            for (int i = 0; i < AllInternStudents.Count; i++)
                            {
                                alEmail.Add(AllInternStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllInternStudents[i].InterApllyId;
                                objComm.UserType = "All-Intern-Students";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Intern";
                                //     objComm.InternshipId = AllInternStudents[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }
                        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End All Intern Comm

                    #region All Brand Comm
                    if (IsAllBrandStudent == true)
                    {

                        if (lstBrandStudent != null || lstBrandStudent == null)
                        {
                            ArrayList alEmail = new ArrayList();
                            var AllBrandStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                                    join cd in db.SITSPL_tblCourseDetails
                                                    on stdpro.StudentId equals cd.StudentId
                                                    //join cs in db.SITSPL_tblCourseStructures
                                                    //on stdpro.StructureId equals cs.StructureId
                                                    join cour in db.SITSPL_tblCourses
                                                    on cd.CourseId equals cour.CourseId
                                                    where stdpro.StudentType == "BrandStudent"
                                                    select new
                                                    {
                                                        //  cs.CourseId,
                                                        stdpro.Email,
                                                        stdpro.StudentId,
                                                        //    stdpro.StudentCategoryId,
                                                    }).Distinct().ToList();
                            if (AllBrandStudents != null)
                            {
                                for (int i = 0; i < AllBrandStudents.Count; i++)
                                {
                                    alEmail.Add(AllBrandStudents[i].Email);
                                    objComm = new tblCommunication();
                                    objComm.IsAllCourseStudent = IsAllCourseStudent;
                                    objComm.IsAllInternStudent = IsAllInternStudent;
                                    objComm.IsAllCourseTutor = IsAllCourseTutor;
                                    objComm.IsAllInternTutor = IsAllInternTutor;
                                    objComm.Id = AllBrandStudents[i].StudentId;
                                    objComm.UserType = "All-Students-Course";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgStudent;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";
                                    objComm.MessageAll = strMsgAll;
                                    //objComm.CategoryType = CategoryType;
                                    objComm.CategoryType = "Course";
                                    //      objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                    //      objComm.CourseId = AllStudents[i].CourseId;
                                    db.tblCommunications.InsertOnSubmit(objComm);
                                    db.SubmitChanges();
                                }
                                if (alEmail.Count > 0)
                                {
                                    SendMailToAll(alEmail);
                                }
                            }
                            //   return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }
                    }
                    #endregion End All Brand Comm

                    #region Selected Course Students

                    if (IsAllCourseStudent == false && CategoryType == "Course")
                    {
                        if (lstCourseStudent != null)
                        {
                            ArrayList alEmail = new ArrayList();
                            for (int i = 0; i < lstCourseStudent.Count; i++)
                            {
                                alEmail.Add(lstCourseStudent[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = lstCourseStudent[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                //    objComm.StudentCategoryId = StudentCategoryId;
                                //    objComm.CourseId = CourseId;
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

                    #endregion End Selected Course Students
                }

                #endregion All Intern Students && All Brand Students && Selected Course Students

                #region All Brand Students && All Course Students && Selected Intern Students

                if (IsAllBrandStudent && !IsAllInternStudent && IsAllCourseStudent && CategoryType == "Intern")
                {
                    #region All Brand Students
                    if (IsAllBrandStudent == true && lstBrandStudent != null || lstBrandStudent == null)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllBrandStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                                join cd in db.SITSPL_tblCourseDetails
                                                on stdpro.StudentId equals cd.StudentId
                                                //join cs in db.SITSPL_tblCourseStructures
                                                //on stdpro.StructureId equals cs.StructureId
                                                join cour in db.SITSPL_tblCourses
                                                on cd.CourseId equals cour.CourseId
                                                where stdpro.StudentType == "BrandStudent"
                                                select new
                                                {
                                                    //  cs.CourseId,
                                                    stdpro.Email,
                                                    stdpro.StudentId,
                                                    //    stdpro.StudentCategoryId,
                                                }).Distinct().ToList();
                        if (AllBrandStudents != null)
                        {
                            for (int i = 0; i < AllBrandStudents.Count; i++)
                            {
                                alEmail.Add(AllBrandStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllBrandStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Course";
                                //      objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                //      objComm.CourseId = AllStudents[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }
                        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }

                    #endregion End All Brand Students

                    #region All Course Students
                    if (IsAllCourseStudent == true)
                    {
                        ArrayList alEmail = new ArrayList();
                        var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                           join cd in db.SITSPL_tblCourseDetails
                                           on stdpro.StudentId equals cd.StudentId
                                           //join cs in db.SITSPL_tblCourseStructures
                                           //on stdpro.StructureId equals cs.StructureId
                                           join cour in db.SITSPL_tblCourses
                                           on cd.CourseId equals cour.CourseId

                                           where stdpro.StudentType == "CourseStudent"
                                           select new
                                           {
                                               // cd.CourseId,
                                               stdpro.Email,
                                               stdpro.StudentId,
                                               //  stdpro.StudentCategoryId,
                                           }).Distinct().ToList();
                        if (AllStudents != null)
                        {
                            for (int i = 0; i < AllStudents.Count; i++)
                            {
                                alEmail.Add(AllStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.Id = AllStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                //         objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                //         objComm.CourseId = AllStudents[i].CourseId;
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
                    #endregion End All Course Students

                    #region Message To Selected Intern Students
                    if (IsAllInternStudent == false && CategoryType == "Intern" && lstStudent != null || lstStudent == null)
                    {
                        objComm = new tblCommunication();
                        // Intern Student List
                        if (lstStudent != null)
                        {
                            ArrayList alEmail = new ArrayList();
                            for (int i = 0; i < lstStudent.Count; i++)
                            {
                                alEmail.Add(lstStudent[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseStudent = IsAllCourseStudent;
                                objComm.IsAllInternStudent = IsAllInternStudent;
                                objComm.IsAllCourseTutor = IsAllCourseTutor;
                                objComm.IsAllInternTutor = IsAllInternTutor;
                                objComm.IsAllTeacher = isAllTutors;
                                objComm.Id = lstStudent[i].StudentId;
                                objComm.UserType = "All-Intern-Students";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgStudent;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Intern";
                                objComm.InternshipId = InternshipId;
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
                    #endregion End Message To Selected Intern Students

                }

                #endregion End All Brand Students && All Course Students && Selected Intern Students
                #endregion End 2 All 1 Selected



                #region Comment 3
                //else if (IsAllCourseStudent == false && !IsAllInternStudent)
                //{
                //    objComm = new tblCommunication();
                //    // Course Student List
                //    if (lstCourseStudent != null)
                //    {
                //        ArrayList alEmail = new ArrayList();
                //        for (int i = 0; i < lstCourseStudent.Count; i++)
                //        {
                //            alEmail.Add(lstCourseStudent[i].Email);
                //            objComm = new tblCommunication();
                //            objComm.IsAllCourseStudent = IsAllCourseStudent;
                //            objComm.IsAllInternStudent = IsAllInternStudent;
                //            objComm.IsAllCourseTutor = IsAllCourseTutor;
                //            objComm.IsAllInternTutor = IsAllInternTutor;
                //            objComm.Id = lstCourseStudent[i].StudentId;
                //            objComm.UserType = "All-Students-Course";
                //            objComm.DateCreated = DateTime.Now;
                //            objComm.CreatedBy = "Admin";
                //            objComm.Message = strMsgStudent;
                //            objComm.MessageAllStudents = "-";
                //            objComm.MessageAllTeachers = "-";
                //            objComm.MessageAll = strMsgAll;
                //            objComm.CategoryType = CategoryType;
                //            objComm.StudentCategoryId = StudentCategoryId;
                //            objComm.CourseId = CourseId;
                //            db.tblCommunications.InsertOnSubmit(objComm);
                //            db.SubmitChanges();
                //        }
                //        if (alEmail.Count > 0)
                //        {
                //            SendMailToAll(alEmail);
                //        }
                //        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                //    }
                //}
                #endregion Comment 3

                #region Comment 1
                // if (isAllTutors && isAllStudents == false)
                //{
                //    if (lstStudent != null)
                //    {
                //        for (int i = 0; i < lstStudent.Count; i++)
                //        {
                //            objComm = new tblCommunication();
                //            objComm.IsAllTeacher = isAllTutors;
                //            objComm.Id = lstStudent[i].StudentId;
                //            objComm.UserType = "All-Intern-Students";
                //            objComm.DateCreated = DateTime.Now;
                //            objComm.CreatedBy = "Admin";
                //            objComm.Message = strMsgStudent;
                //            objComm.MessageAllStudents = "-";
                //            objComm.MessageAllTeachers = "-";
                //            objComm.MessageAll = strMsgAll;
                //            objComm.CategoryType = CategoryType;
                //            objComm.StudentCategoryId = StudentCategoryId;
                //            objComm.InternshipId = InternshipId;
                //            db.tblCommunications.InsertOnSubmit(objComm);
                //            db.SubmitChanges();
                //        }
                //        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                //    }
                //}

                #endregion End Comment 1

                #region Comment 2
                //if (isAllStudents && isAllTutors == false)
                //{
                //    objComm = new tblCommunication();
                //    objComm.IsAllStudent = isAllStudents;
                //    if (lstTutor != null)
                //    {
                //        for (int i = 0; i < lstTutor.Count; i++)
                //        {
                //            objComm.Id = lstTutor[i].TutorId;
                //            objComm.UserType = "All-Intern-Tutor";
                //            objComm.DateCreated = DateTime.Now;
                //            objComm.CreatedBy = "Admin";
                //            objComm.Message = strMsgTutor;
                //            objComm.MessageAllStudents = "-";
                //            objComm.MessageAllTeachers = "-";
                //            objComm.MessageAll = strMsgAll;
                //            objComm.CategoryType = CategoryType;
                //            objComm.StudentCategoryId = StudentCategoryId;
                //            objComm.InternshipId = InternshipId;
                //            db.tblCommunications.InsertOnSubmit(objComm);
                //            db.SubmitChanges();
                //        }
                //        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                //    }
                //}
                #endregion End Comment 2

                return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion  END Add/Sent message and mail to Students/Tutors and also added to his dashboard By Dilshad A. on 14 Sept 2020


        // else if (IsAllCourseStudent && IsAllInternStudent)
        //{
        //    objComm = new tblCommunication();
        //    // Course Student List
        //    if (IsAllCourseStudent == true)
        //    {
        //        ArrayList alEmail = new ArrayList();
        //        var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
        //                           join cd in db.SITSPL_tblCourseDetails
        //                           on stdpro.StudentId equals cd.StudentId
        //                           join cs in db.SITSPL_tblCourseStructures
        //                           on stdpro.StructureId equals cs.StructureId
        //                           join cour in db.SITSPL_tblCourses
        //                           on cs.CourseId equals cour.CourseId
        //                           select new
        //                           {
        //                               cs.CourseId,
        //                               stdpro.Email,
        //                               stdpro.StudentId,
        //                               stdpro.StudentCategoryId,
        //                           }).Distinct().ToList();
        //        if (AllStudents != null)
        //        {
        //            for (int i = 0; i < AllStudents.Count; i++)
        //            {
        //                alEmail.Add(AllStudents[i].Email);
        //                objComm = new tblCommunication();
        //                objComm.IsAllCourseStudent = IsAllCourseStudent;
        //                objComm.IsAllInternStudent = IsAllInternStudent;
        //                objComm.IsAllCourseTutor = IsAllCourseTutor;
        //                objComm.IsAllInternTutor = IsAllInternTutor;
        //                objComm.Id = AllStudents[i].StudentId;
        //                objComm.UserType = "All-Students-Course";
        //                objComm.DateCreated = DateTime.Now;
        //                objComm.CreatedBy = "Admin";
        //                objComm.Message = strMsgStudent;
        //                objComm.MessageAllStudents = "-";
        //                objComm.MessageAllTeachers = "-";
        //                objComm.MessageAll = strMsgAll;
        //                objComm.CategoryType = "Course";
        //                objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
        //                objComm.CourseId = AllStudents[i].CourseId;
        //                db.tblCommunications.InsertOnSubmit(objComm);
        //                db.SubmitChanges();
        //            }
        //            if (alEmail.Count > 0)
        //            {
        //                SendMailToAll(alEmail);
        //            }
        //        }
        //    }
        //    if (IsAllInternStudent == true)
        //    {
        //        objComm = new tblCommunication();
        //        ArrayList alEmail = new ArrayList();
        //        var AllInternStudents = (from intnapply in db.tblInternApplies
        //                                 join intnstr in db.SITSPL_tblInternshipStructures
        //                                 on intnapply.InternshipStructureId equals intnstr.InternStructureId
        //                                 join intn in db.SITSPL_tblInternships
        //                                 on intnstr.InternshipId equals intn.InternshipId
        //                                 select new
        //                                 {
        //                                     intnapply.InterApllyId,
        //                                     intnapply.Name,
        //                                     intnapply.Email,
        //                                     intnapply.InternshipStructureId,
        //                                     intnstr.InternshipId,
        //                                     intn.InternshipName
        //                                 }).Distinct().ToList();

        //        if (AllInternStudents != null)
        //        {
        //            for (int i = 0; i < AllInternStudents.Count; i++)
        //            {
        //                alEmail.Add(AllInternStudents[i].Email);
        //                objComm = new tblCommunication();
        //                objComm.IsAllCourseStudent = IsAllCourseStudent;
        //                objComm.IsAllInternStudent = IsAllInternStudent;
        //                objComm.IsAllCourseTutor = IsAllCourseTutor;
        //                objComm.IsAllInternTutor = IsAllInternTutor;
        //                objComm.Id = AllInternStudents[i].InterApllyId;
        //                objComm.UserType = "All-Intern-Students";
        //                objComm.DateCreated = DateTime.Now;
        //                objComm.CreatedBy = "Admin";
        //                objComm.Message = strMsgStudent;
        //                objComm.MessageAllStudents = "-";
        //                objComm.MessageAllTeachers = "-";
        //                objComm.MessageAll = strMsgAll;
        //                objComm.CategoryType = "Intern";
        //                objComm.InternshipId = AllInternStudents[i].InternshipId;
        //                db.tblCommunications.InsertOnSubmit(objComm);
        //                db.SubmitChanges();
        //            }
        //            if (alEmail.Count > 0)
        //            {
        //                SendMailToAll(alEmail);
        //            }
        //        }
        //          return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
        //    }
        //}





        //else if (IsAllInternStudent && !IsAllCourseStudent)
        //{
        //    objComm = new tblCommunication();
        //    if (lstCourseStudent != null || lstCourseStudent == null)
        //    {
        //        var AllInterns = (from d in db.tblInternApplies
        //                          join d1 in db.SITSPL_tblInternshipStructures on d.InternshipStructureId equals d1.InternStructureId
        //                          join d2 in db.SITSPL_tblInternships on d1.InternshipId equals d2.InternshipId
        //                          select new
        //                          {
        //                              d.InterApllyId,
        //                              d.InternshipStructureId,
        //                              d.Name,
        //                              d1.InternshipId,
        //                              d.IsPublished,
        //                              d.Email
        //                          }).ToList();

        //        ArrayList alEmail = new ArrayList();
        //        for (int i = 0; i < AllInterns.Count; i++)
        //        {
        //            alEmail.Add(AllInterns[i].Email);
        //            objComm = new tblCommunication();
        //            objComm.IsAllCourseStudent = IsAllCourseStudent;
        //            objComm.IsAllInternStudent = IsAllInternStudent;
        //            objComm.IsAllCourseTutor = IsAllCourseTutor;
        //            objComm.IsAllInternTutor = IsAllInternTutor;
        //            objComm.IsAllTeacher = isAllTutors;
        //            objComm.Id = AllInterns[i].InterApllyId;
        //            objComm.UserType = "All-Intern-Students";
        //            objComm.DateCreated = DateTime.Now;
        //            objComm.CreatedBy = "Admin";
        //            objComm.Message = strMsgStudent;
        //            objComm.MessageAllStudents = "-";
        //            objComm.MessageAllTeachers = "-";
        //            objComm.MessageAll = strMsgAll;
        //            objComm.CategoryType = CategoryType;
        //            objComm.InternshipId = AllInterns[i].InternshipId;
        //            db.tblCommunications.InsertOnSubmit(objComm);
        //            db.SubmitChanges();
        //        }
        //        if (alEmail.Count > 0)
        //        {
        //            SendMailToAll(alEmail);
        //        }
        //        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
        //    }
        //}



        //else if (IsAllCourseStudent && !IsAllInternStudent)
        //{
        //    objComm = new tblCommunication();
        //    // Course Student List
        //    if (lstCourseStudent != null || lstCourseStudent == null)
        //    {
        //        ArrayList alEmail = new ArrayList();
        //        var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
        //                           join cd in db.SITSPL_tblCourseDetails
        //                           on stdpro.StudentId equals cd.StudentId
        //                           join cs in db.SITSPL_tblCourseStructures
        //                           on stdpro.StructureId equals cs.StructureId
        //                           join cour in db.SITSPL_tblCourses
        //                           on cs.CourseId equals cour.CourseId
        //                           select new
        //                           {
        //                               cs.CourseId,
        //                               stdpro.Email,
        //                               stdpro.StudentId,
        //                               stdpro.StudentCategoryId,
        //                           }).Distinct().ToList();
        //        if (AllStudents != null)
        //        {
        //            for (int i = 0; i < AllStudents.Count; i++)
        //            {
        //                alEmail.Add(AllStudents[i].Email);
        //                objComm = new tblCommunication();
        //                objComm.IsAllCourseStudent = IsAllCourseStudent;
        //                objComm.IsAllInternStudent = IsAllInternStudent;
        //                objComm.IsAllCourseTutor = IsAllCourseTutor;
        //                objComm.IsAllInternTutor = IsAllInternTutor;
        //                objComm.Id = AllStudents[i].StudentId;
        //                objComm.UserType = "All-Students-Course";
        //                objComm.DateCreated = DateTime.Now;
        //                objComm.CreatedBy = "Admin";
        //                objComm.Message = strMsgStudent;
        //                objComm.MessageAllStudents = "-";
        //                objComm.MessageAllTeachers = "-";
        //                objComm.MessageAll = strMsgAll;
        //                objComm.CategoryType = CategoryType;
        //                objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
        //                objComm.CourseId = AllStudents[i].CourseId;
        //                db.tblCommunications.InsertOnSubmit(objComm);
        //                db.SubmitChanges();
        //            }
        //            if (alEmail.Count > 0)
        //            {
        //                SendMailToAll(alEmail);
        //            }
        //        }
        //        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
        //    }
        //}

        #region Communication To All
        [HttpPost]
        public JsonResult AddMessageCommunicationForAll(bool isAllStudents, bool isAllTutors, string strMsgAll)
        {

            try
            {
                db = new CourseDataContext();
                tblCommunication objComm = null;
                // For All Students/Tutors(Intern/Course)
                ArrayList alAllEmail = new ArrayList();

                #region All Students & All Tutors
                if (isAllStudents && isAllTutors)
                {
                    objComm = new tblCommunication();
                    // Course Student List

                    ArrayList alEmail = new ArrayList();
                    if (isAllStudents == true)
                    {
                        #region Course Student

                        var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                           join cd in db.SITSPL_tblCourseDetails
                                           on stdpro.StudentId equals cd.StudentId
                                           //join cs in db.SITSPL_tblCourseStructures
                                           //on stdpro.StructureId equals cs.StructureId
                                           join cour in db.SITSPL_tblCourses
                                           on cd.CourseId equals cour.CourseId

                                           where stdpro.StudentType == "CourseStudent"
                                           select new
                                           {
                                               // cd.CourseId,
                                               stdpro.Email,
                                               stdpro.StudentId,
                                               //  stdpro.StudentCategoryId,
                                           }).Distinct().ToList();
                        if (AllStudents != null)
                        {
                            for (int i = 0; i < AllStudents.Count; i++)
                            {
                                alEmail.Add(AllStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllStudent = isAllStudents;
                                objComm.Id = AllStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgAll;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }

                        #endregion End Course Student

                        objComm = new tblCommunication();

                        #region All Intern Students

                        var AllInternStudents = (from intnapply in db.tblInternApplies
                                                 join intnstr in db.SITSPL_tblInternshipStructures
                                                 on intnapply.InternshipStructureId equals intnstr.InternStructureId
                                                 join intn in db.SITSPL_tblInternships
                                                 on intnstr.InternshipId equals intn.InternshipId
                                                 join dur in db.tblDurations
                                                 on intnstr.DurationId equals dur.DurationId
                                                 select new
                                                 {
                                                     intnapply.InterApllyId,
                                                     //     intnapply.Name,
                                                     intnapply.Email,
                                                     //     intnapply.InternshipStructureId,
                                                     //     intnstr.InternshipId,
                                                     //     intn.InternshipName
                                                 }).Distinct().ToList();

                        if (AllInternStudents != null)
                        {
                            for (int i = 0; i < AllInternStudents.Count; i++)
                            {
                                alEmail.Add(AllInternStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.Id = AllInternStudents[i].InterApllyId;
                                objComm.UserType = "All-Intern-Students";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.IsAllStudent = isAllStudents;
                                objComm.Message = strMsgAll;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.CategoryType = "Intern";
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }

                        #endregion End All Intern Students

                        #region All Brand Students

                        var AllBrandStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                                join cd in db.SITSPL_tblCourseDetails
                                                on stdpro.StudentId equals cd.StudentId
                                                //join cs in db.SITSPL_tblCourseStructures
                                                //on stdpro.StructureId equals cs.StructureId
                                                join cour in db.SITSPL_tblCourses
                                                on cd.CourseId equals cour.CourseId

                                                where stdpro.StudentType == "BrandStudent"
                                                select new
                                                {
                                                    // cd.CourseId,
                                                    stdpro.Email,
                                                    stdpro.StudentId,
                                                    //  stdpro.StudentCategoryId,
                                                }).Distinct().ToList();
                        if (AllBrandStudents != null)
                        {
                            for (int i = 0; i < AllBrandStudents.Count; i++)
                            {
                                alEmail.Add(AllBrandStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.Id = AllBrandStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgAll;
                                objComm.IsAllStudent = isAllStudents;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }


                        #endregion All Brand Students

                    }


                    if (isAllTutors == true)
                    {
                        #region All Course Tutor

                        objComm = new tblCommunication();
                        //   ArrayList alEmail = new ArrayList();
                        var AllCoursesTutor = (from tut in db.SITSPL_tblTutors
                                               join courtut in db.tblCourseTutors
                                               on tut.TutorId equals courtut.TutorId
                                               join courstr in db.SITSPL_tblCourseStructures
                                               on courtut.CourseId equals courstr.CourseId
                                               select new
                                               {
                                                   //  tut.TutorName,
                                                   tut.TutorEmail,
                                                   //   courtut.CourseTutorId,
                                                   courtut.TutorId,
                                                   //  courtut.CourseId
                                               }).Distinct().ToList();

                        if (AllCoursesTutor != null)
                        {
                            for (int i = 0; i < AllCoursesTutor.Count; i++)
                            {
                                alEmail.Add(AllCoursesTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                //           objComm.IsAllCourseTutor = isAllCourses;
                                //   objComm.Id = AllCoursesTutor[i].CourseTutorId;
                                objComm.UserType = "All-Course-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgAll;
                                objComm.IsAllTeacher = isAllTutors;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllCoursesTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Course";
                                //  objComm.InternshipId = AllCoursesTutor[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAllTutor(alEmail);
                            }
                        }

                        #endregion End All Course Tutor

                        //  return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);

                        #region All Intern Tutor

                        var AllInternTutor = (from tut in db.SITSPL_tblTutors
                                              join intntut in db.tblInternTutors
                                              on tut.TutorId equals intntut.TutorId
                                              join intstr in db.SITSPL_tblInternshipStructures
                                              on intntut.InternshipId equals intstr.InternshipId
                                              select new
                                              {
                                                  //    tut.TutorName,
                                                  tut.TutorEmail,
                                                  //     intntut.InternTutorId,
                                                  intntut.TutorId,
                                                  //  intntut.InternshipId
                                              }).Distinct().ToList();

                        if (AllInternTutor != null)
                        {
                            for (int i = 0; i < AllInternTutor.Count; i++)
                            {
                                alEmail.Add(AllInternTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                //              objComm.IsAllInternTutor = isAllInterns;
                                //       objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Intern-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgAll;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllInternTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Intern";
                                objComm.IsAllTeacher = isAllTutors;
                                //       objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAllTutor(alEmail);
                            }
                        }

                        #endregion End All Intern Tutor

                        #region All Brand Tutor

                        var AllBrandTutor = (from cs in db.SITSPL_tblCourseStructures
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
                                                 //     tut.TutorName,
                                                 tut.TutorEmail
                                                 //  cour.CourseName,
                                                 //  dur.DurationName,
                                                 //  cs.StructureId
                                             }).ToList();

                        if (AllBrandTutor != null)
                        {
                            for (int i = 0; i < AllBrandTutor.Count; i++)
                            {
                                alEmail.Add(AllBrandTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                //              objComm.IsAllInternTutor = isAllInterns;
                                //       objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Brand-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgAll;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllBrandTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Brand";
                                objComm.IsAllTeacher = isAllTutors;
                                //       objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAllTutor(alEmail);
                            }
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);

                        #endregion End All Brand Tutor

                    }

                }
                #endregion End All Students & All Tutors

                #region All Students
                else if (isAllStudents == true)
                {
                    #region Course Student

                    ArrayList alEmail = new ArrayList();

                    var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                       join cd in db.SITSPL_tblCourseDetails
                                       on stdpro.StudentId equals cd.StudentId
                                       //join cs in db.SITSPL_tblCourseStructures
                                       //on stdpro.StructureId equals cs.StructureId
                                       join cour in db.SITSPL_tblCourses
                                       on cd.CourseId equals cour.CourseId

                                       where stdpro.StudentType == "CourseStudent"
                                       select new
                                       {
                                           // cd.CourseId,
                                           stdpro.Email,
                                           stdpro.StudentId,
                                           //  stdpro.StudentCategoryId,
                                       }).Distinct().ToList();
                    if (AllStudents != null)
                    {
                        for (int i = 0; i < AllStudents.Count; i++)
                        {
                            alEmail.Add(AllStudents[i].Email);
                            objComm = new tblCommunication();
                            objComm.IsAllStudent = isAllStudents;
                            objComm.Id = AllStudents[i].StudentId;
                            objComm.UserType = "All-Students-Course";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.Message = strMsgAll;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.MessageAll = strMsgAll;
                            objComm.CategoryType = "Course";
                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }
                        if (alEmail.Count > 0)
                        {
                            SendMailToAll(alEmail);
                        }
                    }

                    #endregion End Course Student

                    objComm = new tblCommunication();

                    #region All Intern Students

                    var AllInternStudents = (from intnapply in db.tblInternApplies
                                             join intnstr in db.SITSPL_tblInternshipStructures
                                             on intnapply.InternshipStructureId equals intnstr.InternStructureId
                                             join intn in db.SITSPL_tblInternships
                                             on intnstr.InternshipId equals intn.InternshipId
                                             join dur in db.tblDurations
                                             on intnstr.DurationId equals dur.DurationId
                                             select new
                                             {
                                                 intnapply.InterApllyId,
                                                 //     intnapply.Name,
                                                 intnapply.Email,
                                                 //     intnapply.InternshipStructureId,
                                                 //     intnstr.InternshipId,
                                                 //     intn.InternshipName
                                             }).Distinct().ToList();

                    if (AllInternStudents != null)
                    {
                        for (int i = 0; i < AllInternStudents.Count; i++)
                        {
                            alEmail.Add(AllInternStudents[i].Email);
                            objComm = new tblCommunication();
                            objComm.Id = AllInternStudents[i].InterApllyId;
                            objComm.UserType = "All-Intern-Students";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.IsAllStudent = isAllStudents;
                            objComm.Message = strMsgAll;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.CategoryType = "Intern";
                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }
                        if (alEmail.Count > 0)
                        {
                            SendMailToAll(alEmail);
                        }
                    }

                    #endregion End All Intern Students

                    #region All Brand Students

                    var AllBrandStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                            join cd in db.SITSPL_tblCourseDetails
                                            on stdpro.StudentId equals cd.StudentId
                                            //join cs in db.SITSPL_tblCourseStructures
                                            //on stdpro.StructureId equals cs.StructureId
                                            join cour in db.SITSPL_tblCourses
                                            on cd.CourseId equals cour.CourseId

                                            where stdpro.StudentType == "BrandStudent"
                                            select new
                                            {
                                                // cd.CourseId,
                                                stdpro.Email,
                                                stdpro.StudentId,
                                                //  stdpro.StudentCategoryId,
                                            }).Distinct().ToList();
                    if (AllBrandStudents != null)
                    {
                        for (int i = 0; i < AllBrandStudents.Count; i++)
                        {
                            alEmail.Add(AllBrandStudents[i].Email);
                            objComm = new tblCommunication();
                            objComm.Id = AllBrandStudents[i].StudentId;
                            objComm.UserType = "All-Students-Course";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.Message = strMsgAll;
                            objComm.IsAllStudent = isAllStudents;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.MessageAll = strMsgAll;
                            objComm.CategoryType = "Course";
                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }
                        if (alEmail.Count > 0)
                        {
                            SendMailToAll(alEmail);
                        }
                    }
                    return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);

                    #endregion All Brand Students
                }
                #endregion End All Students

                #region All Tutors
                else if (isAllTutors == true)
                {
                    #region All Course Tutor


                    ArrayList alEmail = new ArrayList();
                    objComm = new tblCommunication();
                    //   ArrayList alEmail = new ArrayList();
                    var AllCoursesTutor = (from tut in db.SITSPL_tblTutors
                                           join courtut in db.tblCourseTutors
                                           on tut.TutorId equals courtut.TutorId
                                           join courstr in db.SITSPL_tblCourseStructures
                                           on courtut.CourseId equals courstr.CourseId
                                           select new
                                           {
                                               //  tut.TutorName,
                                               tut.TutorEmail,
                                               //   courtut.CourseTutorId,
                                               courtut.TutorId,
                                               //  courtut.CourseId
                                           }).Distinct().ToList();

                    if (AllCoursesTutor != null)
                    {
                        for (int i = 0; i < AllCoursesTutor.Count; i++)
                        {
                            alEmail.Add(AllCoursesTutor[i].TutorEmail);
                            objComm = new tblCommunication();
                            //           objComm.IsAllCourseTutor = isAllCourses;
                            //   objComm.Id = AllCoursesTutor[i].CourseTutorId;
                            objComm.UserType = "All-Course-Tutor";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.Message = strMsgAll;
                            objComm.IsAllTeacher = isAllTutors;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.TutorId = Convert.ToInt32(AllCoursesTutor[i].TutorId);
                            //objComm.CategoryType = CategoryType;
                            objComm.CategoryType = "Course";
                            //  objComm.InternshipId = AllCoursesTutor[i].CourseId;
                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }
                        if (alEmail.Count > 0)
                        {
                            SendMailToAllTutor(alEmail);
                        }
                    }

                    #endregion End All Course Tutor

                    //  return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);

                    #region All Intern Tutor

                    var AllInternTutor = (from tut in db.SITSPL_tblTutors
                                          join intntut in db.tblInternTutors
                                          on tut.TutorId equals intntut.TutorId
                                          join intstr in db.SITSPL_tblInternshipStructures
                                          on intntut.InternshipId equals intstr.InternshipId
                                          select new
                                          {
                                              //    tut.TutorName,
                                              tut.TutorEmail,
                                              //     intntut.InternTutorId,
                                              intntut.TutorId,
                                              //  intntut.InternshipId
                                          }).Distinct().ToList();

                    if (AllInternTutor != null)
                    {
                        for (int i = 0; i < AllInternTutor.Count; i++)
                        {
                            alEmail.Add(AllInternTutor[i].TutorEmail);
                            objComm = new tblCommunication();
                            //              objComm.IsAllInternTutor = isAllInterns;
                            //       objComm.Id = AllInternTutor[i].InternTutorId;
                            objComm.UserType = "All-Intern-Tutor";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.Message = strMsgAll;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.TutorId = Convert.ToInt32(AllInternTutor[i].TutorId);
                            //objComm.CategoryType = CategoryType;
                            objComm.CategoryType = "Intern";
                            objComm.IsAllTeacher = isAllTutors;
                            //       objComm.InternshipId = AllInternTutor[i].InternshipId;
                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }
                        if (alEmail.Count > 0)
                        {
                            SendMailToAllTutor(alEmail);
                        }
                    }

                    #endregion End All Intern Tutor

                    #region All Brand Tutor

                    var AllBrandTutor = (from cs in db.SITSPL_tblCourseStructures
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
                                             //     tut.TutorName,
                                             tut.TutorEmail
                                             //  cour.CourseName,
                                             //  dur.DurationName,
                                             //  cs.StructureId
                                         }).ToList();

                    if (AllBrandTutor != null)
                    {
                        for (int i = 0; i < AllBrandTutor.Count; i++)
                        {
                            alEmail.Add(AllBrandTutor[i].TutorEmail);
                            objComm = new tblCommunication();
                            //              objComm.IsAllInternTutor = isAllInterns;
                            //       objComm.Id = AllInternTutor[i].InternTutorId;
                            objComm.UserType = "All-Brand-Tutor";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.Message = strMsgAll;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.TutorId = Convert.ToInt32(AllBrandTutor[i].TutorId);
                            //objComm.CategoryType = CategoryType;
                            objComm.CategoryType = "Brand";
                            objComm.IsAllTeacher = isAllTutors;
                            //       objComm.InternshipId = AllInternTutor[i].InternshipId;
                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }
                        if (alEmail.Count > 0)
                        {
                            SendMailToAllTutor(alEmail);
                        }
                    }
                    return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);

                    #endregion End All Brand Tutor

                }
                #endregion End All Tutors

                #region no checkbox tick message to all

                else if (isAllStudents == false && isAllTutors == false)
                {
                    objComm = new tblCommunication();
                    // Course Student List

                    ArrayList alEmail = new ArrayList();
                    if (isAllStudents == false)
                    {
                        #region Course Student

                        var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                           join cd in db.SITSPL_tblCourseDetails
                                           on stdpro.StudentId equals cd.StudentId
                                           //join cs in db.SITSPL_tblCourseStructures
                                           //on stdpro.StructureId equals cs.StructureId
                                           join cour in db.SITSPL_tblCourses
                                           on cd.CourseId equals cour.CourseId

                                           where stdpro.StudentType == "CourseStudent"
                                           select new
                                           {
                                               // cd.CourseId,
                                               stdpro.Email,
                                               stdpro.StudentId,
                                               //  stdpro.StudentCategoryId,
                                           }).Distinct().ToList();
                        if (AllStudents != null)
                        {
                            for (int i = 0; i < AllStudents.Count; i++)
                            {
                                alEmail.Add(AllStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllStudent = isAllStudents;
                                objComm.Id = AllStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgAll;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }

                        #endregion End Course Student

                        objComm = new tblCommunication();

                        #region All Intern Students

                        var AllInternStudents = (from intnapply in db.tblInternApplies
                                                 join intnstr in db.SITSPL_tblInternshipStructures
                                                 on intnapply.InternshipStructureId equals intnstr.InternStructureId
                                                 join intn in db.SITSPL_tblInternships
                                                 on intnstr.InternshipId equals intn.InternshipId
                                                 join dur in db.tblDurations
                                                 on intnstr.DurationId equals dur.DurationId
                                                 select new
                                                 {
                                                     intnapply.InterApllyId,
                                                     //     intnapply.Name,
                                                     intnapply.Email,
                                                     //     intnapply.InternshipStructureId,
                                                     //     intnstr.InternshipId,
                                                     //     intn.InternshipName
                                                 }).Distinct().ToList();

                        if (AllInternStudents != null)
                        {
                            for (int i = 0; i < AllInternStudents.Count; i++)
                            {
                                alEmail.Add(AllInternStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.Id = AllInternStudents[i].InterApllyId;
                                objComm.UserType = "All-Intern-Students";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.IsAllStudent = isAllStudents;
                                objComm.Message = strMsgAll;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.CategoryType = "Intern";
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }

                        #endregion End All Intern Students

                        #region All Brand Students

                        var AllBrandStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                                                join cd in db.SITSPL_tblCourseDetails
                                                on stdpro.StudentId equals cd.StudentId
                                                //join cs in db.SITSPL_tblCourseStructures
                                                //on stdpro.StructureId equals cs.StructureId
                                                join cour in db.SITSPL_tblCourses
                                                on cd.CourseId equals cour.CourseId

                                                where stdpro.StudentType == "BrandStudent"
                                                select new
                                                {
                                                    // cd.CourseId,
                                                    stdpro.Email,
                                                    stdpro.StudentId,
                                                    //  stdpro.StudentCategoryId,
                                                }).Distinct().ToList();
                        if (AllBrandStudents != null)
                        {
                            for (int i = 0; i < AllBrandStudents.Count; i++)
                            {
                                alEmail.Add(AllBrandStudents[i].Email);
                                objComm = new tblCommunication();
                                objComm.Id = AllBrandStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgAll;
                                objComm.IsAllStudent = isAllStudents;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }


                        #endregion All Brand Students

                    }

                    if (isAllTutors == false)
                    {
                        #region All Course Tutor

                        objComm = new tblCommunication();
                        //   ArrayList alEmail = new ArrayList();
                        var AllCoursesTutor = (from tut in db.SITSPL_tblTutors
                                               join courtut in db.tblCourseTutors
                                               on tut.TutorId equals courtut.TutorId
                                               join courstr in db.SITSPL_tblCourseStructures
                                               on courtut.CourseId equals courstr.CourseId
                                               select new
                                               {
                                                   //  tut.TutorName,
                                                   tut.TutorEmail,
                                                   //   courtut.CourseTutorId,
                                                   courtut.TutorId,
                                                   //  courtut.CourseId
                                               }).Distinct().ToList();

                        if (AllCoursesTutor != null)
                        {
                            for (int i = 0; i < AllCoursesTutor.Count; i++)
                            {
                                alEmail.Add(AllCoursesTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                //           objComm.IsAllCourseTutor = isAllCourses;
                                //   objComm.Id = AllCoursesTutor[i].CourseTutorId;
                                objComm.UserType = "All-Course-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgAll;
                                objComm.IsAllTeacher = isAllTutors;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllCoursesTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Course";
                                //  objComm.InternshipId = AllCoursesTutor[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAllTutor(alEmail);
                            }
                        }

                        #endregion End All Course Tutor

                        //  return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);

                        #region All Intern Tutor

                        var AllInternTutor = (from tut in db.SITSPL_tblTutors
                                              join intntut in db.tblInternTutors
                                              on tut.TutorId equals intntut.TutorId
                                              join intstr in db.SITSPL_tblInternshipStructures
                                              on intntut.InternshipId equals intstr.InternshipId
                                              select new
                                              {
                                                  //    tut.TutorName,
                                                  tut.TutorEmail,
                                                  //     intntut.InternTutorId,
                                                  intntut.TutorId,
                                                  //  intntut.InternshipId
                                              }).Distinct().ToList();

                        if (AllInternTutor != null)
                        {
                            for (int i = 0; i < AllInternTutor.Count; i++)
                            {
                                alEmail.Add(AllInternTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                //              objComm.IsAllInternTutor = isAllInterns;
                                //       objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Intern-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgAll;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllInternTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Intern";
                                objComm.IsAllTeacher = isAllTutors;
                                //       objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAllTutor(alEmail);
                            }
                        }

                        #endregion End All Intern Tutor

                        #region All Brand Tutor

                        var AllBrandTutor = (from cs in db.SITSPL_tblCourseStructures
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
                                                 //     tut.TutorName,
                                                 tut.TutorEmail
                                                 //  cour.CourseName,
                                                 //  dur.DurationName,
                                                 //  cs.StructureId
                                             }).ToList();

                        if (AllBrandTutor != null)
                        {
                            for (int i = 0; i < AllBrandTutor.Count; i++)
                            {
                                alEmail.Add(AllBrandTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                //              objComm.IsAllInternTutor = isAllInterns;
                                //       objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Brand-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgAll;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllBrandTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Brand";
                                objComm.IsAllTeacher = isAllTutors;
                                //       objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAllTutor(alEmail);
                            }
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);

                        #endregion End All Brand Tutor

                    }

                }

                #endregion no checkbox tick message to all


                return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message,JsonRequestBehavior.AllowGet);
            }
        }
        #endregion  END Communication To All


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


        // Add Communication For Tutor
        public ActionResult AddCommunicationTutor()
        {
            return View();
        }


        [HttpPost]
        public JsonResult AddMessageForTutorCommunication(int? CourseId, int? InternshipId, List<InternTutorCommunication> lstInternTutor, bool isAllInterns, bool isAllCourses, string CategoryType, string strMsgTutor, List<AdminToTutorCommunicationCourseTutor> lstCourseTutor,
            List<AdminToTutorCommunicationInternTutor> lstInternsTutor, List<AdminToTutorCommunicationBrandTutor> lstBrandTutor, bool isAllBrandTutor)
        {
            try
            {

                db = new CourseDataContext();
                tblCommunication objComm = null;

                ArrayList tutEmail = new ArrayList();
                string CatgType = "";

                #region All Checkbox tick ( Course Tutors + Intern Tutor + Brand Tutor)
                #region All Course Tutor and All Intern Tutors & All Brand Tutor

                if (isAllCourses == true && isAllInterns == true && isAllBrandTutor == true && CategoryType == null || CategoryType == "")
                {
                    objComm = new tblCommunication();
                    if (lstCourseTutor != null || lstCourseTutor == null)
                    {
                        // ArrayList alEmail = new ArrayList();
                        var AllCoursesTutor = (from tut in db.SITSPL_tblTutors
                                               join courtut in db.tblCourseTutors
                                               on tut.TutorId equals courtut.TutorId
                                               join courstr in db.SITSPL_tblCourseStructures
                                               on courtut.CourseId equals courstr.CourseId
                                               select new
                                               {
                                                   //  tut.TutorName,
                                                   tut.TutorEmail,
                                                   //   courtut.CourseTutorId,
                                                   courtut.TutorId,
                                                   //  courtut.CourseId
                                               }).Distinct().ToList();

                        if (AllCoursesTutor != null)
                        {
                            ArrayList alBrandEmail = new ArrayList();
                            for (int i = 0; i < AllCoursesTutor.Count; i++)
                            {
                                alBrandEmail.Add(AllCoursesTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseTutor = isAllCourses;
                                //   objComm.Id = AllCoursesTutor[i].CourseTutorId;
                                objComm.UserType = "All-Course-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllCoursesTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Course";
                                //  objComm.InternshipId = AllCoursesTutor[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alBrandEmail.Count > 0)
                            {
                                SendMailToAllTutor(alBrandEmail);
                            }
                        }
                        //  return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }

                    //     objComm = new tblCommunication();


                    if (lstInternsTutor != null || lstInternsTutor == null)
                    {
                        ArrayList alInternEmail = new ArrayList();

                        var AllInternTutor = (from tut in db.SITSPL_tblTutors
                                              join intntut in db.tblInternTutors
                                              on tut.TutorId equals intntut.TutorId
                                              join intstr in db.SITSPL_tblInternshipStructures
                                              on intntut.InternshipId equals intstr.InternshipId
                                              select new
                                              {
                                                  //    tut.TutorName,
                                                  tut.TutorEmail,
                                                  //     intntut.InternTutorId,
                                                  intntut.TutorId,
                                                  //  intntut.InternshipId
                                              }).Distinct().ToList();

                        if (AllInternTutor != null)
                        {
                            for (int i = 0; i < AllInternTutor.Count; i++)
                            {
                                alInternEmail.Add(AllInternTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                objComm.IsAllInternTutor = isAllInterns;
                                //       objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Intern-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllInternTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Intern";
                                //       objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alInternEmail.Count > 0)
                            {
                                SendMailToAllTutor(alInternEmail);
                            }
                        }
                        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }

                    #region All Brand Tutors (Tutor Id 19) check
                    // objComm = new tblCommunication();
                    ArrayList alBrandTutEmail = new ArrayList();
                    if (lstBrandTutor != null || lstBrandTutor == null)
                    {
                        var AllBrandTutor = (from cs in db.SITSPL_tblCourseStructures
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
                                                 //  tut.TutorName,
                                                 tut.TutorEmail,
                                                 //  cour.CourseName,
                                                 //  cour.CourseId,
                                                 //  dur.DurationName,
                                                 //     cs.StructureId
                                             }).Distinct().ToList();

                        if (AllBrandTutor != null)
                        {
                            //tblCommunication objBrandTutor = new tblCommunication();
                            for (int i = 0; i < AllBrandTutor.Count; i++)
                            {
                                alBrandTutEmail.Add(AllBrandTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseTutor = isAllBrandTutor;

                                //      objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Brand-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllBrandTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Brand";
                                objComm.IsPublished = true;
                                //   objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alBrandTutEmail.Count > 0)
                            {
                                SendMailToAllTutor(alBrandTutEmail);
                            }
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End All Brand Tutors
                }

                #endregion End All Course Tutor and All Intern Tutors & All Brand Tutor
                #endregion End All Checkbox tick ( Course Tutors + Intern Tutor + Brand Tutor)

                #region 1 tutor Selected at a time no checkbox tick

                #region Course Tutor Selected
                if (CategoryType == "Course" && isAllCourses == false && isAllInterns == false && isAllBrandTutor == false)
                {
                    if (isAllCourses == false)
                    {
                        if (lstCourseTutor != null)
                        {
                            for (int i = 0; i < lstCourseTutor.Count; i++)
                            {
                                tutEmail.Add(lstCourseTutor[i].Email);
                                objComm = new tblCommunication();
                                //objComm.IsAllTeacher = isAllTutors;
                                objComm.IsAllCourseTutor = isAllCourses;

                                objComm.Id = lstCourseTutor[i].Id;
                                objComm.UserType = "All-Course-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = lstCourseTutor[i].TutorId;
                                objComm.CategoryType = CategoryType;

                                objComm.CourseId = lstCourseTutor[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (tutEmail.Count > 0)
                            {
                                SendMailToAllTutor(tutEmail);
                            }
                            return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                #endregion Course Tutor Selected

                #region Intern Tutor  Selected
                if (CategoryType == "Intern" && isAllCourses == false && isAllInterns == false && isAllBrandTutor == false)
                {
                    if (isAllInterns == false)
                    {

                        if (lstInternsTutor != null)
                        {
                            for (int i = 0; i < lstInternsTutor.Count; i++)
                            {
                                tutEmail.Add(lstInternsTutor[i].Email);
                                objComm = new tblCommunication();
                                //objComm.IsAllTeacher = isAllTutors;
                                objComm.IsAllInternTutor = isAllInterns;

                                objComm.Id = lstInternsTutor[i].Id;
                                objComm.UserType = "All-Intern-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";

                                objComm.CategoryType = CategoryType;
                                objComm.TutorId = lstInternsTutor[i].TutorId;
                                objComm.InternshipId = lstInternsTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (tutEmail.Count > 0)
                            {
                                SendMailToAllTutor(tutEmail);
                            }
                            return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                #endregion Intern Tutor Selected

                #region Brand Tutor Selected

                if (CategoryType == "Brand" && isAllCourses == false && isAllInterns == false && isAllBrandTutor == false)
                {
                    if (lstBrandTutor != null)
                    {
                        for (int i = 0; i < lstBrandTutor.Count; i++)
                        {
                            tutEmail.Add(lstBrandTutor[i].Email);
                            objComm = new tblCommunication();
                            objComm.IsAllCourseTutor = isAllCourses;
                            objComm.Id = lstBrandTutor[i].Id;
                            objComm.UserType = "All-Brand-Tutor";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.Message = strMsgTutor;
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.TutorId = lstBrandTutor[i].TutorId;
                            objComm.CategoryType = CatgType;
                            objComm.CourseId = lstBrandTutor[i].CourseId;
                            db.tblCommunications.InsertOnSubmit(objComm);
                            db.SubmitChanges();
                        }
                        if (tutEmail.Count > 0)
                        {
                            SendMailToAllTutor(tutEmail);
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                }

                #endregion End Brand Tutor Selected

                #endregion End 1 tutor Selected at a time no checkbox tick

                #region All 1 at time

                #region All Course Tutor
                if (isAllCourses == true && isAllInterns == false && isAllBrandTutor == false && CategoryType == null || CategoryType == "")
                {
                    if (lstCourseTutor != null || lstCourseTutor == null)
                    {
                        ArrayList alCourseEmail = new ArrayList();
                        var AllCoursesTutor = (from tut in db.SITSPL_tblTutors
                                               join courtut in db.tblCourseTutors
                                               on tut.TutorId equals courtut.TutorId
                                               join courstr in db.SITSPL_tblCourseStructures
                                               on courtut.CourseId equals courstr.CourseId
                                               select new
                                               {
                                                   //    tut.TutorName,
                                                   tut.TutorEmail,
                                                   //    courtut.CourseTutorId,
                                                   courtut.TutorId,
                                                   //    courtut.CourseId
                                               }).Distinct().ToList();

                        if (AllCoursesTutor != null)
                        {
                            for (int i = 0; i < AllCoursesTutor.Count; i++)
                            {
                                alCourseEmail.Add(AllCoursesTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseTutor = isAllCourses;
                                //     objComm.Id = AllCoursesTutor[i].CourseTutorId;
                                objComm.UserType = "All-Course-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllCoursesTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Course";
                                //  objComm.InternshipId = AllCoursesTutor[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alCourseEmail.Count > 0)
                            {
                                SendMailToAllTutor(alCourseEmail);
                            }
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }

                }
                #endregion All Course Tutor

                #region All Intern Tutors
                if (isAllInterns == true && isAllCourses == false && isAllBrandTutor == false && CategoryType == null || CategoryType == "")
                {

                    objComm = new tblCommunication();

                    // Course Student List
                    if (lstInternsTutor != null || lstInternsTutor == null)
                    {
                        ArrayList alInternTutorEmail = new ArrayList();

                        var AllInternTutor = (from tut in db.SITSPL_tblTutors
                                              join intntut in db.tblInternTutors
                                              on tut.TutorId equals intntut.TutorId
                                              join intstr in db.SITSPL_tblInternshipStructures
                                              on intntut.InternshipId equals intstr.InternshipId
                                              select new
                                              {
                                                  //     tut.TutorName,
                                                  tut.TutorEmail,
                                                  //     intntut.InternTutorId,
                                                  intntut.TutorId,
                                                  //  intntut.InternshipId
                                              }).Distinct().ToList();

                        //   ArrayList alCourse = new ArrayList();

                        if (AllInternTutor != null)
                        {
                            for (int i = 0; i < AllInternTutor.Count; i++)
                            {
                                alInternTutorEmail.Add(AllInternTutor[i].TutorEmail);

                                objComm = new tblCommunication();

                                objComm.IsAllInternTutor = isAllInterns;

                                //      objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Intern-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllInternTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Intern";
                                //   objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alInternTutorEmail.Count > 0)
                            {
                                SendMailToAllTutor(alInternTutorEmail);
                            }
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion All Intern Tutors

                #region All Brand Tutors

                if (isAllInterns == false && isAllCourses == false && isAllBrandTutor == true && CategoryType == null || CategoryType == "")
                {
                    ArrayList albrandtutEmail = new ArrayList();
                    if (lstBrandTutor != null || lstBrandTutor == null)
                    {
                        var AllBrandTutor = (from cs in db.SITSPL_tblCourseStructures
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
                                                 //  tut.TutorName,
                                                 tut.TutorEmail,
                                                 //  cour.CourseName,
                                                 //  cour.CourseId,
                                                 //  dur.DurationName,
                                                 //     cs.StructureId
                                             }).Distinct().ToList();

                        if (AllBrandTutor != null)
                        {

                            for (int i = 0; i < AllBrandTutor.Count; i++)
                            {
                                albrandtutEmail.Add(AllBrandTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseTutor = isAllBrandTutor;

                                //      objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Brand-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllBrandTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Brand";
                                objComm.IsPublished = true;
                                //   objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (albrandtutEmail.Count > 0)
                            {
                                SendMailToAllTutor(albrandtutEmail);
                            }
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion End All Brand Tutors

                #endregion End All 1 at time

                #region 2 Tutor Checkbox selected

                #region AllCourses And All Intern Tutor Message
                if (isAllCourses == true && isAllInterns == true && isAllBrandTutor == false && CategoryType == null || CategoryType == "")
                {

                    if (lstCourseTutor != null || lstCourseTutor == null)
                    {
                        // ArrayList alEmail = new ArrayList();
                        var AllCoursesTutor = (from tut in db.SITSPL_tblTutors
                                               join courtut in db.tblCourseTutors
                                               on tut.TutorId equals courtut.TutorId
                                               join courstr in db.SITSPL_tblCourseStructures
                                               on courtut.CourseId equals courstr.CourseId
                                               select new
                                               {
                                                   //  tut.TutorName,
                                                   tut.TutorEmail,
                                                   //   courtut.CourseTutorId,
                                                   courtut.TutorId,
                                                   //  courtut.CourseId
                                               }).Distinct().ToList();

                        if (AllCoursesTutor != null)
                        {
                            ArrayList alBrandEmail = new ArrayList();
                            for (int i = 0; i < AllCoursesTutor.Count; i++)
                            {
                                alBrandEmail.Add(AllCoursesTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseTutor = isAllCourses;
                                //   objComm.Id = AllCoursesTutor[i].CourseTutorId;
                                objComm.UserType = "All-Course-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllCoursesTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Course";
                                //  objComm.InternshipId = AllCoursesTutor[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alBrandEmail.Count > 0)
                            {
                                SendMailToAllTutor(alBrandEmail);
                            }
                        }
                        //  return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }

                    //     objComm = new tblCommunication();


                    if (lstInternsTutor != null || lstInternsTutor == null)
                    {
                        ArrayList alInternEmail = new ArrayList();

                        var AllInternTutor = (from tut in db.SITSPL_tblTutors
                                              join intntut in db.tblInternTutors
                                              on tut.TutorId equals intntut.TutorId
                                              join intstr in db.SITSPL_tblInternshipStructures
                                              on intntut.InternshipId equals intstr.InternshipId
                                              select new
                                              {
                                                  //    tut.TutorName,
                                                  tut.TutorEmail,
                                                  //     intntut.InternTutorId,
                                                  intntut.TutorId,
                                                  //  intntut.InternshipId
                                              }).Distinct().ToList();

                        if (AllInternTutor != null)
                        {
                            for (int i = 0; i < AllInternTutor.Count; i++)
                            {
                                alInternEmail.Add(AllInternTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                objComm.IsAllInternTutor = isAllInterns;
                                //       objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Intern-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllInternTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Intern";
                                //       objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alInternEmail.Count > 0)
                            {
                                SendMailToAllTutor(alInternEmail);
                            }
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion AllCourses And All Intern Tutors Message

                #region All Intern Tutor and All Brand Tutor Message

                if (isAllInterns == true && isAllCourses == false && isAllBrandTutor == true && CategoryType == null || CategoryType == "")
                {
                    #region All Intern Tutors
                    if (isAllInterns == true && isAllCourses == false)
                    {



                        // Course Student List
                        if (lstInternsTutor != null || lstInternsTutor == null)
                        {
                            ArrayList alInternTutorEmail = new ArrayList();

                            var AllInternTutor = (from tut in db.SITSPL_tblTutors
                                                  join intntut in db.tblInternTutors
                                                  on tut.TutorId equals intntut.TutorId
                                                  join intstr in db.SITSPL_tblInternshipStructures
                                                  on intntut.InternshipId equals intstr.InternshipId
                                                  select new
                                                  {
                                                      //     tut.TutorName,
                                                      tut.TutorEmail,
                                                      //     intntut.InternTutorId,
                                                      intntut.TutorId,
                                                      //  intntut.InternshipId
                                                  }).Distinct().ToList();

                            //   ArrayList alCourse = new ArrayList();

                            if (AllInternTutor != null)
                            {
                                for (int i = 0; i < AllInternTutor.Count; i++)
                                {
                                    alInternTutorEmail.Add(AllInternTutor[i].TutorEmail);

                                    objComm = new tblCommunication();


                                    objComm.IsAllInternTutor = isAllInterns;

                                    //      objComm.Id = AllInternTutor[i].InternTutorId;
                                    objComm.UserType = "All-Intern-Tutor";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgTutor;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";
                                    objComm.TutorId = Convert.ToInt32(AllInternTutor[i].TutorId);
                                    //objComm.CategoryType = CategoryType;
                                    objComm.CategoryType = "Intern";
                                    //   objComm.InternshipId = AllInternTutor[i].InternshipId;
                                    db.tblCommunications.InsertOnSubmit(objComm);
                                    db.SubmitChanges();
                                }
                                if (alInternTutorEmail.Count > 0)
                                {
                                    SendMailToAllTutor(alInternTutorEmail);
                                }
                            }
                            //  return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }
                    }
                    #endregion All Intern Tutors

                    #region All Brand Tutors

                    ArrayList alEmail = new ArrayList();
                    if (lstBrandTutor != null || lstBrandTutor == null)
                    {
                        var AllBrandTutor = (from cs in db.SITSPL_tblCourseStructures
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
                                                 //  tut.TutorName,
                                                 tut.TutorEmail,
                                                 //  cour.CourseName,
                                                 //  cour.CourseId,
                                                 //  dur.DurationName,
                                                 //     cs.StructureId
                                             }).Distinct().ToList();

                        if (AllBrandTutor != null)
                        {

                            for (int i = 0; i < AllBrandTutor.Count; i++)
                            {
                                alEmail.Add(AllBrandTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseTutor = isAllBrandTutor;

                                //      objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Brand-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllBrandTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Brand";
                                objComm.IsPublished = true;
                                //   objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAllTutor(alEmail);
                            }
                        }
                        return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End All Brand Tutors
                }

                #endregion End All Intern Tutor and All Brand Tutor Message

                #region All Brand Tutor & All Course Tutor

                if (isAllInterns == false && isAllCourses == true && isAllBrandTutor == true && CategoryType == null || CategoryType == "")
                {
                    #region All Brand Tutors

                    ArrayList brandtutEmail = new ArrayList();
                    if (isAllBrandTutor == true && lstBrandTutor != null || lstBrandTutor == null)
                    {
                        var AllBrandTutor = (from cs in db.SITSPL_tblCourseStructures
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
                                                 //  tut.TutorName,
                                                 tut.TutorEmail,
                                                 //  cour.CourseName,
                                                 //  cour.CourseId,
                                                 //  dur.DurationName,
                                                 //     cs.StructureId
                                             }).Distinct().ToList();

                        if (AllBrandTutor != null)
                        {

                            for (int i = 0; i < AllBrandTutor.Count; i++)
                            {
                                brandtutEmail.Add(AllBrandTutor[i].TutorEmail);

                                objComm = new tblCommunication();
                                objComm.IsAllCourseTutor = isAllBrandTutor;

                                //      objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Brand-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllBrandTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Brand";
                                objComm.IsPublished = true;
                                //   objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (brandtutEmail.Count > 0)
                            {
                                SendMailToAllTutor(brandtutEmail);
                            }
                        }
                        //  return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End All Brand Tutors

                    #region All Course Tutor
                    if (isAllCourses == true)
                    {
                        if (lstCourseTutor != null || lstCourseTutor == null)
                        {
                            ArrayList alCourseEmail = new ArrayList();
                            var AllCoursesTutor = (from tut in db.SITSPL_tblTutors
                                                   join courtut in db.tblCourseTutors
                                                   on tut.TutorId equals courtut.TutorId
                                                   join courstr in db.SITSPL_tblCourseStructures
                                                   on courtut.CourseId equals courstr.CourseId
                                                   select new
                                                   {
                                                       //    tut.TutorName,
                                                       tut.TutorEmail,
                                                       //    courtut.CourseTutorId,
                                                       courtut.TutorId,
                                                       //    courtut.CourseId
                                                   }).Distinct().ToList();

                            if (AllCoursesTutor != null)
                            {
                                for (int i = 0; i < AllCoursesTutor.Count; i++)
                                {
                                    alCourseEmail.Add(AllCoursesTutor[i].TutorEmail);
                                    objComm = new tblCommunication();
                                    objComm.IsAllCourseTutor = isAllCourses;
                                    //     objComm.Id = AllCoursesTutor[i].CourseTutorId;
                                    objComm.UserType = "All-Course-Tutor";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgTutor;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";
                                    objComm.TutorId = Convert.ToInt32(AllCoursesTutor[i].TutorId);
                                    //objComm.CategoryType = CategoryType;
                                    objComm.CategoryType = "Course";
                                    //  objComm.InternshipId = AllCoursesTutor[i].CourseId;
                                    db.tblCommunications.InsertOnSubmit(objComm);
                                    db.SubmitChanges();
                                }
                                if (alCourseEmail.Count > 0)
                                {
                                    SendMailToAllTutor(alCourseEmail);
                                }
                            }
                            return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }

                    }
                    #endregion All Course Tutor
                }

                #endregion End All Brand Tutor & All Course Tutor

                #endregion End 2 Tutor Checkbox Selected


                #region All Checkbox 1 tick with different Selected Tutor Type

                #region All Intern Tutors & Selected Course Tutors

                if (isAllInterns == true && isAllCourses == false && isAllBrandTutor == false && CategoryType == "Course")
                {

                    objComm = new tblCommunication();

                    // Course Student List
                    if (isAllInterns == true && lstInternsTutor != null || lstInternsTutor == null)
                    {
                        ArrayList alInternTutorEmail = new ArrayList();

                        var AllInternTutor = (from tut in db.SITSPL_tblTutors
                                              join intntut in db.tblInternTutors
                                              on tut.TutorId equals intntut.TutorId
                                              join intstr in db.SITSPL_tblInternshipStructures
                                              on intntut.InternshipId equals intstr.InternshipId
                                              select new
                                              {
                                                  //     tut.TutorName,
                                                  tut.TutorEmail,
                                                  //     intntut.InternTutorId,
                                                  intntut.TutorId,
                                                  //  intntut.InternshipId
                                              }).Distinct().ToList();

                        //   ArrayList alCourse = new ArrayList();

                        if (AllInternTutor != null)
                        {
                            for (int i = 0; i < AllInternTutor.Count; i++)
                            {
                                alInternTutorEmail.Add(AllInternTutor[i].TutorEmail);

                                objComm = new tblCommunication();

                                objComm.IsAllInternTutor = isAllInterns;

                                //      objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Intern-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllInternTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Intern";
                                //   objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alInternTutorEmail.Count > 0)
                            {
                                SendMailToAllTutor(alInternTutorEmail);
                            }
                        }
                        //  return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }

                    #region Selected Course Tutor 

                    if (CategoryType == "Course")
                    {
                        if (isAllCourses == false)
                        {
                            if (lstCourseTutor != null)
                            {

                                for (int i = 0; i < lstCourseTutor.Count; i++)
                                {
                                    tutEmail.Add(lstCourseTutor[i].Email);
                                    objComm = new tblCommunication();
                                    //objComm.IsAllTeacher = isAllTutors;
                                    objComm.IsAllCourseTutor = isAllCourses;

                                    objComm.Id = lstCourseTutor[i].Id;
                                    objComm.UserType = "All-Course-Tutor";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgTutor;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";
                                    objComm.TutorId = lstCourseTutor[i].TutorId;
                                    objComm.CategoryType = CategoryType;
                                    objComm.CourseId = lstCourseTutor[i].CourseId;
                                    db.tblCommunications.InsertOnSubmit(objComm);
                                    db.SubmitChanges();
                                }
                                if (tutEmail.Count > 0)
                                {
                                    SendMailToAllTutor(tutEmail);
                                }
                                return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                            }
                        }

                    }

                    #endregion End Selected Course Tutor

                }

                #endregion End All Intern Tutors & Selected Course Tutors

                #region All Intern Tutors && Selected Brand Tutors

                if (isAllInterns == true && isAllCourses == false && isAllBrandTutor == false && CategoryType == "Brand")
                {
                    #region All Intern Tutors
                    if (isAllInterns == true && isAllCourses == false)
                    {

                        objComm = new tblCommunication();

                        // Course Student List
                        if (lstInternsTutor != null || lstInternsTutor == null)
                        {
                            ArrayList alInternTutorEmail = new ArrayList();

                            var AllInternTutor = (from tut in db.SITSPL_tblTutors
                                                  join intntut in db.tblInternTutors
                                                  on tut.TutorId equals intntut.TutorId
                                                  join intstr in db.SITSPL_tblInternshipStructures
                                                  on intntut.InternshipId equals intstr.InternshipId
                                                  select new
                                                  {
                                                      //     tut.TutorName,
                                                      tut.TutorEmail,
                                                      //     intntut.InternTutorId,
                                                      intntut.TutorId,
                                                      //  intntut.InternshipId
                                                  }).Distinct().ToList();

                            //   ArrayList alCourse = new ArrayList();

                            if (AllInternTutor != null)
                            {
                                for (int i = 0; i < AllInternTutor.Count; i++)
                                {
                                    alInternTutorEmail.Add(AllInternTutor[i].TutorEmail);

                                    objComm = new tblCommunication();

                                    objComm.IsAllInternTutor = isAllInterns;

                                    //      objComm.Id = AllInternTutor[i].InternTutorId;
                                    objComm.UserType = "All-Intern-Tutor";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgTutor;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";
                                    objComm.TutorId = Convert.ToInt32(AllInternTutor[i].TutorId);
                                    //objComm.CategoryType = CategoryType;
                                    objComm.CategoryType = "Intern";
                                    //   objComm.InternshipId = AllInternTutor[i].InternshipId;
                                    db.tblCommunications.InsertOnSubmit(objComm);
                                    db.SubmitChanges();
                                }
                                if (alInternTutorEmail.Count > 0)
                                {
                                    SendMailToAllTutor(alInternTutorEmail);
                                }
                            }
                            // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }
                    }
                    #endregion End All Intern Tutors

                    #region Selected Brand Tutors
                    if (CategoryType == "Brand")
                    {
                        //if (isAllCourses == false)
                        //{
                        objComm = new tblCommunication();
                        objComm.IsAllCourseTutor = isAllCourses;
                        if (lstBrandTutor != null)
                        {
                            for (int i = 0; i < lstBrandTutor.Count; i++)
                            {
                                tutEmail.Add(lstBrandTutor[i].Email);
                                objComm = new tblCommunication();
                                //objComm.IsAllTeacher = isAllTutors;
                                objComm.IsAllCourseTutor = isAllCourses;

                                objComm.Id = lstBrandTutor[i].Id;
                                objComm.UserType = "All-Brand-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = lstBrandTutor[i].TutorId;
                                objComm.CategoryType = CatgType;

                                objComm.CourseId = lstBrandTutor[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (tutEmail.Count > 0)
                            {
                                SendMailToAllTutor(tutEmail);
                            }
                            return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }
                        //}

                    }
                    #endregion End Selected Brand Tutors

                }
                #endregion End All Intern Tutors && Selected Brand Students

                #region All Course Tutors & Selected Intern Tutors

                if (isAllInterns == false && isAllCourses == true && isAllBrandTutor == false && CategoryType == "Intern")
                {

                    #region All Course Tutors
                    if (isAllCourses == true)
                    {

                        if (lstCourseTutor != null || lstCourseTutor == null)
                        {
                            ArrayList alCourseEmail = new ArrayList();
                            var AllCoursesTutor = (from tut in db.SITSPL_tblTutors
                                                   join courtut in db.tblCourseTutors
                                                   on tut.TutorId equals courtut.TutorId
                                                   join courstr in db.SITSPL_tblCourseStructures
                                                   on courtut.CourseId equals courstr.CourseId
                                                   select new
                                                   {
                                                       //    tut.TutorName,
                                                       tut.TutorEmail,
                                                       //    courtut.CourseTutorId,
                                                       courtut.TutorId,
                                                       //    courtut.CourseId
                                                   }).Distinct().ToList();

                            if (AllCoursesTutor != null)
                            {
                                for (int i = 0; i < AllCoursesTutor.Count; i++)
                                {
                                    alCourseEmail.Add(AllCoursesTutor[i].TutorEmail);
                                    objComm = new tblCommunication();
                                    objComm.IsAllCourseTutor = isAllCourses;
                                    //     objComm.Id = AllCoursesTutor[i].CourseTutorId;
                                    objComm.UserType = "All-Course-Tutor";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgTutor;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";
                                    objComm.TutorId = Convert.ToInt32(AllCoursesTutor[i].TutorId);
                                    //objComm.CategoryType = CategoryType;
                                    objComm.CategoryType = "Course";
                                    //  objComm.InternshipId = AllCoursesTutor[i].CourseId;
                                    db.tblCommunications.InsertOnSubmit(objComm);
                                    db.SubmitChanges();
                                }
                                if (alCourseEmail.Count > 0)
                                {
                                    SendMailToAllTutor(alCourseEmail);
                                }
                            }
                            //  return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }

                    }
                    #endregion End All Course Tutors

                    #region Selected Intern Tutors

                    if (CategoryType == "Intern")
                    {
                        if (isAllInterns == false)
                        {

                            if (lstInternsTutor != null)
                            {
                                for (int i = 0; i < lstInternsTutor.Count; i++)
                                {
                                    tutEmail.Add(lstInternsTutor[i].Email);
                                    objComm = new tblCommunication();
                                    //objComm.IsAllTeacher = isAllTutors;
                                    objComm.IsAllInternTutor = isAllInterns;

                                    objComm.Id = lstInternsTutor[i].Id;
                                    objComm.UserType = "All-Intern-Tutor";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgTutor;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";

                                    objComm.CategoryType = CategoryType;
                                    objComm.TutorId = lstInternsTutor[i].TutorId;
                                    objComm.InternshipId = lstInternsTutor[i].InternshipId;
                                    db.tblCommunications.InsertOnSubmit(objComm);
                                    db.SubmitChanges();
                                }
                                if (tutEmail.Count > 0)
                                {
                                    SendMailToAllTutor(tutEmail);
                                }
                                return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    #endregion Selected Intern Tutors

                }

                #endregion End All Course Tutors && Selected Intern Tutors

                #region All Course Tutors && Selected Brand Tutors

                if (isAllInterns == false && isAllCourses == true && isAllBrandTutor == false && CategoryType == "Brand")
                {

                    #region All Course Tutors
                    if (isAllCourses == true)
                    {
                        if (lstCourseTutor != null || lstCourseTutor == null)
                        {
                            ArrayList alCourseEmail = new ArrayList();
                            var AllCoursesTutor = (from tut in db.SITSPL_tblTutors
                                                   join courtut in db.tblCourseTutors
                                                   on tut.TutorId equals courtut.TutorId
                                                   join courstr in db.SITSPL_tblCourseStructures
                                                   on courtut.CourseId equals courstr.CourseId
                                                   select new
                                                   {
                                                       //    tut.TutorName,
                                                       tut.TutorEmail,
                                                       //    courtut.CourseTutorId,
                                                       courtut.TutorId,
                                                       //    courtut.CourseId
                                                   }).Distinct().ToList();

                            if (AllCoursesTutor != null)
                            {
                                for (int i = 0; i < AllCoursesTutor.Count; i++)
                                {
                                    alCourseEmail.Add(AllCoursesTutor[i].TutorEmail);
                                    objComm = new tblCommunication();
                                    objComm.IsAllCourseTutor = isAllCourses;
                                    //     objComm.Id = AllCoursesTutor[i].CourseTutorId;
                                    objComm.UserType = "All-Course-Tutor";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgTutor;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";
                                    objComm.TutorId = Convert.ToInt32(AllCoursesTutor[i].TutorId);
                                    //objComm.CategoryType = CategoryType;
                                    objComm.CategoryType = "Course";
                                    //  objComm.InternshipId = AllCoursesTutor[i].CourseId;
                                    db.tblCommunications.InsertOnSubmit(objComm);
                                    db.SubmitChanges();
                                }
                                if (alCourseEmail.Count > 0)
                                {
                                    SendMailToAllTutor(alCourseEmail);
                                }
                            }
                            //  return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }

                    }
                    #endregion End All Course Tutors

                    #region Brand Tutor Selected

                    if (CategoryType == "Brand")
                    {
                        //if (isAllCourses == false)
                        //{
                        objComm = new tblCommunication();
                        objComm.IsAllCourseTutor = isAllCourses;
                        if (lstBrandTutor != null)
                        {
                            for (int i = 0; i < lstBrandTutor.Count; i++)
                            {
                                tutEmail.Add(lstBrandTutor[i].Email);
                                objComm = new tblCommunication();
                                //objComm.IsAllTeacher = isAllTutors;
                                objComm.IsAllCourseTutor = isAllCourses;

                                objComm.Id = lstBrandTutor[i].Id;
                                objComm.UserType = "All-Brand-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = lstBrandTutor[i].TutorId;
                                objComm.CategoryType = CatgType;

                                objComm.CourseId = lstBrandTutor[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (tutEmail.Count > 0)
                            {
                                SendMailToAllTutor(tutEmail);
                            }
                            return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }
                        //}

                    }

                    #endregion End Brand Tutor Selected

                }

                #endregion End All Course Tutors && Selected Brand Tutors

                #region All Brand Tutors && Selected Course Tutors

                if (isAllInterns == false && isAllCourses == false && isAllBrandTutor == true && CategoryType == "Course")
                {
                    #region All Brand Tutors (Tutor Id 19) check

                    ArrayList albrEmail = new ArrayList();
                    if (isAllBrandTutor == true && lstBrandTutor != null || lstBrandTutor == null)
                    {
                        var AllBrandTutor = (from cs in db.SITSPL_tblCourseStructures
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
                                                 //  tut.TutorName,
                                                 tut.TutorEmail,
                                                 //  cour.CourseName,
                                                 //  cour.CourseId,
                                                 //  dur.DurationName,
                                                 //     cs.StructureId
                                             }).Distinct().ToList();

                        if (AllBrandTutor != null)
                        {
                            tblCommunication objBrandTutor = new tblCommunication();
                            for (int i = 0; i < AllBrandTutor.Count; i++)
                            {
                                albrEmail.Add(AllBrandTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseTutor = isAllBrandTutor;

                                //      objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Brand-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllBrandTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Brand";
                                objComm.IsPublished = true;
                                //   objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (albrEmail.Count > 0)
                            {
                                SendMailToAllTutor(albrEmail);
                            }
                        }
                        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End All Brand Tutors

                    #region Course Tutor Selected
                    if (CategoryType == "Course")
                    {
                        if (isAllCourses == false)
                        {
                            if (lstCourseTutor != null)
                            {
                                for (int i = 0; i < lstCourseTutor.Count; i++)
                                {
                                    tutEmail.Add(lstCourseTutor[i].Email);
                                    objComm = new tblCommunication();
                                    //objComm.IsAllTeacher = isAllTutors;
                                    objComm.IsAllCourseTutor = isAllCourses;

                                    objComm.Id = lstCourseTutor[i].Id;
                                    objComm.UserType = "All-Course-Tutor";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgTutor;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";
                                    objComm.TutorId = lstCourseTutor[i].TutorId;
                                    objComm.CategoryType = CategoryType;

                                    objComm.CourseId = lstCourseTutor[i].CourseId;
                                    db.tblCommunications.InsertOnSubmit(objComm);
                                    db.SubmitChanges();
                                }
                                if (tutEmail.Count > 0)
                                {
                                    SendMailToAllTutor(tutEmail);
                                }
                                return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                            }
                        }

                    }
                    #endregion Course Tutor Selected

                }

                #endregion End All Brand Tutors && Selected Course Tutors

                #region All Brand Tutors && Selected Intern Tutors

                if (isAllInterns == false && isAllCourses == false && isAllBrandTutor == true && CategoryType == "Intern")
                {
                    #region All Brand Tutors (Tutor Id 19) check

                    ArrayList brandEmail = new ArrayList();
                    if (lstBrandTutor != null || lstBrandTutor == null)
                    {
                        var AllBrandTutor = (from cs in db.SITSPL_tblCourseStructures
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
                                                 //  tut.TutorName,
                                                 tut.TutorEmail,
                                                 //  cour.CourseName,
                                                 //  cour.CourseId,
                                                 //  dur.DurationName,
                                                 //     cs.StructureId
                                             }).Distinct().ToList();

                        if (AllBrandTutor != null)
                        {
                            tblCommunication objBrandTutor = new tblCommunication();
                            for (int i = 0; i < AllBrandTutor.Count; i++)
                            {
                                brandEmail.Add(AllBrandTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseTutor = isAllBrandTutor;

                                //      objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Brand-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllBrandTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Brand";
                                objComm.IsPublished = true;
                                //   objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (brandEmail.Count > 0)
                            {
                                SendMailToAllTutor(brandEmail);
                            }
                        }
                        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End All Brand Tutors

                    #region Intern Tutor  Selected
                    if (CategoryType == "Intern")
                    {
                        if (isAllInterns == false)
                        {
                            if (lstInternsTutor != null)
                            {
                                for (int i = 0; i < lstInternsTutor.Count; i++)
                                {
                                    tutEmail.Add(lstInternsTutor[i].Email);
                                    objComm = new tblCommunication();
                                    //objComm.IsAllTeacher = isAllTutors;
                                    objComm.IsAllInternTutor = isAllInterns;

                                    objComm.Id = lstInternsTutor[i].Id;
                                    objComm.UserType = "All-Intern-Tutor";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgTutor;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";

                                    objComm.CategoryType = "Intern";
                                    objComm.TutorId = lstInternsTutor[i].TutorId;
                                    objComm.InternshipId = lstInternsTutor[i].InternshipId;
                                    db.tblCommunications.InsertOnSubmit(objComm);
                                    db.SubmitChanges();
                                }
                                if (tutEmail.Count > 0)
                                {
                                    SendMailToAllTutor(tutEmail);
                                }
                                return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    #endregion Intern Tutor Selected
                }

                #endregion End All Brand Tutors && Selected Intern Tutors

                #endregion End All Checkbox 1 tick with different Selected Tutor Type


                #region 2 Checkbox tick and different category type


                #region All Intern Tutors && All Course Tutors && Selected Brand Tutors

                if (isAllInterns == true && isAllCourses == true && isAllBrandTutor == false && CategoryType == "Brand")
                {
                    #region AllCourses And All Intern Tutor Message

                    if (isAllCourses == true)
                    {
                        if (lstCourseTutor != null || lstCourseTutor == null)
                        {
                            // ArrayList alEmail = new ArrayList();
                            var AllCoursesTutor = (from tut in db.SITSPL_tblTutors
                                                   join courtut in db.tblCourseTutors
                                                   on tut.TutorId equals courtut.TutorId
                                                   join courstr in db.SITSPL_tblCourseStructures
                                                   on courtut.CourseId equals courstr.CourseId
                                                   select new
                                                   {
                                                       //  tut.TutorName,
                                                       tut.TutorEmail,
                                                       //   courtut.CourseTutorId,
                                                       courtut.TutorId,
                                                       //  courtut.CourseId
                                                   }).Distinct().ToList();

                            if (AllCoursesTutor != null)
                            {
                                ArrayList alBrandEmail = new ArrayList();
                                for (int i = 0; i < AllCoursesTutor.Count; i++)
                                {
                                    alBrandEmail.Add(AllCoursesTutor[i].TutorEmail);
                                    objComm = new tblCommunication();
                                    objComm.IsAllCourseTutor = isAllCourses;
                                    //   objComm.Id = AllCoursesTutor[i].CourseTutorId;
                                    objComm.UserType = "All-Course-Tutor";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgTutor;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";
                                    objComm.TutorId = Convert.ToInt32(AllCoursesTutor[i].TutorId);
                                    //objComm.CategoryType = CategoryType;
                                    objComm.CategoryType = "Course";
                                    //  objComm.InternshipId = AllCoursesTutor[i].CourseId;
                                    db.tblCommunications.InsertOnSubmit(objComm);
                                    db.SubmitChanges();
                                }
                                if (alBrandEmail.Count > 0)
                                {
                                    SendMailToAllTutor(alBrandEmail);
                                }
                            }
                            //  return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }

                        //     objComm = new tblCommunication();
                    }

                    if (isAllInterns == true)
                    {
                        ArrayList alInternEmail = new ArrayList();

                        var AllInternTutor = (from tut in db.SITSPL_tblTutors
                                              join intntut in db.tblInternTutors
                                              on tut.TutorId equals intntut.TutorId
                                              join intstr in db.SITSPL_tblInternshipStructures
                                              on intntut.InternshipId equals intstr.InternshipId
                                              select new
                                              {
                                                  //    tut.TutorName,
                                                  tut.TutorEmail,
                                                  //     intntut.InternTutorId,
                                                  intntut.TutorId,
                                                  //  intntut.InternshipId
                                              }).Distinct().ToList();

                        if (AllInternTutor != null)
                        {
                            for (int i = 0; i < AllInternTutor.Count; i++)
                            {
                                alInternEmail.Add(AllInternTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                objComm.IsAllInternTutor = isAllInterns;
                                //       objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Intern-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllInternTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Intern";
                                //       objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alInternEmail.Count > 0)
                            {
                                SendMailToAllTutor(alInternEmail);
                            }
                        }
                        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }

                    #endregion AllCourses And All Intern Tutors Message

                    #region Brand Tutor Selected

                    if (CategoryType == "Brand")
                    {
                        if (lstBrandTutor != null)
                        {
                            for (int i = 0; i < lstBrandTutor.Count; i++)
                            {
                                tutEmail.Add(lstBrandTutor[i].Email);
                                objComm = new tblCommunication();
                                objComm.IsAllCourseTutor = isAllCourses;
                                objComm.Id = lstBrandTutor[i].Id;
                                objComm.UserType = "All-Brand-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = lstBrandTutor[i].TutorId;
                                objComm.CategoryType = CatgType;
                                objComm.CourseId = lstBrandTutor[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (tutEmail.Count > 0)
                            {
                                SendMailToAllTutor(tutEmail);
                            }
                            return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }
                    }

                    #endregion End Brand Tutor Selected


                }

                #endregion End All Intern Tutors && All Course Tutors && Selected Brand Tutors

                #region All Course Tutors && All Brand Tutors && Selected Intern Tutors

                if (isAllInterns == false && isAllCourses == true && isAllBrandTutor == true && CategoryType == "Intern")
                {
                    #region All Course Tutor

                    if (isAllCourses == true)
                    {
                        if (lstCourseTutor != null || lstCourseTutor == null)
                        {
                            // ArrayList alEmail = new ArrayList();
                            var AllCoursesTutor = (from tut in db.SITSPL_tblTutors
                                                   join courtut in db.tblCourseTutors
                                                   on tut.TutorId equals courtut.TutorId
                                                   join courstr in db.SITSPL_tblCourseStructures
                                                   on courtut.CourseId equals courstr.CourseId
                                                   select new
                                                   {
                                                       //  tut.TutorName,
                                                       tut.TutorEmail,
                                                       //   courtut.CourseTutorId,
                                                       courtut.TutorId,
                                                       //  courtut.CourseId
                                                   }).Distinct().ToList();

                            if (AllCoursesTutor != null)
                            {
                                ArrayList alBrandEmail = new ArrayList();
                                for (int i = 0; i < AllCoursesTutor.Count; i++)
                                {
                                    alBrandEmail.Add(AllCoursesTutor[i].TutorEmail);
                                    objComm = new tblCommunication();
                                    objComm.IsAllCourseTutor = isAllCourses;
                                    //   objComm.Id = AllCoursesTutor[i].CourseTutorId;
                                    objComm.UserType = "All-Course-Tutor";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgTutor;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";
                                    objComm.TutorId = Convert.ToInt32(AllCoursesTutor[i].TutorId);
                                    //objComm.CategoryType = CategoryType;
                                    objComm.CategoryType = "Course";
                                    //  objComm.InternshipId = AllCoursesTutor[i].CourseId;
                                    db.tblCommunications.InsertOnSubmit(objComm);
                                    db.SubmitChanges();
                                }
                                if (alBrandEmail.Count > 0)
                                {
                                    SendMailToAllTutor(alBrandEmail);
                                }
                            }
                            //  return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                        }

                        //     objComm = new tblCommunication();
                    }

                    #endregion End All Course Tutor

                    #region All Brand Tutors

                    ArrayList brandtutEmail = new ArrayList();
                    if (isAllBrandTutor == true && lstBrandTutor != null || lstBrandTutor == null)
                    {
                        var AllBrandTutor = (from cs in db.SITSPL_tblCourseStructures
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
                                                 //  tut.TutorName,
                                                 tut.TutorEmail,
                                                 //  cour.CourseName,
                                                 //  cour.CourseId,
                                                 //  dur.DurationName,
                                                 //     cs.StructureId
                                             }).Distinct().ToList();

                        if (AllBrandTutor != null)
                        {

                            for (int i = 0; i < AllBrandTutor.Count; i++)
                            {
                                brandtutEmail.Add(AllBrandTutor[i].TutorEmail);

                                objComm = new tblCommunication();
                                objComm.IsAllCourseTutor = isAllBrandTutor;

                                //      objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Brand-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllBrandTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Brand";
                                objComm.IsPublished = true;
                                //   objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (brandtutEmail.Count > 0)
                            {
                                SendMailToAllTutor(brandtutEmail);
                            }
                        }
                        //  return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End All Brand Tutors

                    #region Selected Intern Tutor

                    if (CategoryType == "Intern")
                    {
                        if (isAllInterns == false)
                        {

                            if (lstInternsTutor != null)
                            {
                                for (int i = 0; i < lstInternsTutor.Count; i++)
                                {
                                    tutEmail.Add(lstInternsTutor[i].Email);
                                    objComm = new tblCommunication();
                                    //objComm.IsAllTeacher = isAllTutors;
                                    objComm.IsAllInternTutor = isAllInterns;

                                    objComm.Id = lstInternsTutor[i].Id;
                                    objComm.UserType = "All-Intern-Tutor";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgTutor;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";

                                    objComm.CategoryType = CategoryType;
                                    objComm.TutorId = lstInternsTutor[i].TutorId;
                                    objComm.InternshipId = lstInternsTutor[i].InternshipId;
                                    db.tblCommunications.InsertOnSubmit(objComm);
                                    db.SubmitChanges();
                                }
                                if (tutEmail.Count > 0)
                                {
                                    SendMailToAllTutor(tutEmail);
                                }
                                return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    #endregion End Selected Intern Tutor

                }
                #endregion End All Course Tutors && All Brand Tutors && Seected Intern Tutors

                #region All Brand Tutors && All Intern Tutors && Selected Course Tutors

                if (isAllInterns == true && isAllCourses == false && isAllBrandTutor == true && CategoryType == "Course")
                {
                    #region All Brand Tutors

                    ArrayList brandtutEmail = new ArrayList();
                    if (isAllBrandTutor == true && lstBrandTutor != null || lstBrandTutor == null)
                    {
                        var AllBrandTutor = (from cs in db.SITSPL_tblCourseStructures
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
                                                 //  tut.TutorName,
                                                 tut.TutorEmail,
                                                 //  cour.CourseName,
                                                 //  cour.CourseId,
                                                 //  dur.DurationName,
                                                 //     cs.StructureId
                                             }).Distinct().ToList();

                        if (AllBrandTutor != null)
                        {

                            for (int i = 0; i < AllBrandTutor.Count; i++)
                            {
                                brandtutEmail.Add(AllBrandTutor[i].TutorEmail);

                                objComm = new tblCommunication();
                                objComm.IsAllCourseTutor = isAllBrandTutor;

                                //      objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Brand-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllBrandTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Brand";
                                objComm.IsPublished = true;
                                //   objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (brandtutEmail.Count > 0)
                            {
                                SendMailToAllTutor(brandtutEmail);
                            }
                        }
                        //  return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }
                    #endregion End All Brand Tutors

                    #region All Intern Tutor

                    if (isAllInterns == true)
                    {
                        ArrayList alInternEmail = new ArrayList();

                        var AllInternTutor = (from tut in db.SITSPL_tblTutors
                                              join intntut in db.tblInternTutors
                                              on tut.TutorId equals intntut.TutorId
                                              join intstr in db.SITSPL_tblInternshipStructures
                                              on intntut.InternshipId equals intstr.InternshipId
                                              select new
                                              {
                                                  //    tut.TutorName,
                                                  tut.TutorEmail,
                                                  //     intntut.InternTutorId,
                                                  intntut.TutorId,
                                                  //  intntut.InternshipId
                                              }).Distinct().ToList();

                        if (AllInternTutor != null)
                        {
                            for (int i = 0; i < AllInternTutor.Count; i++)
                            {
                                alInternEmail.Add(AllInternTutor[i].TutorEmail);
                                objComm = new tblCommunication();
                                objComm.IsAllInternTutor = isAllInterns;
                                //       objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Intern-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = strMsgTutor;
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.TutorId = Convert.ToInt32(AllInternTutor[i].TutorId);
                                //objComm.CategoryType = CategoryType;
                                objComm.CategoryType = "Intern";
                                //       objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alInternEmail.Count > 0)
                            {
                                SendMailToAllTutor(alInternEmail);
                            }
                        }
                        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                    }

                    #endregion End All Intern Tutor

                    #region Course Tutor Selected
                    if (CategoryType == "Course")
                    {
                        if (isAllCourses == false)
                        {
                            if (lstCourseTutor != null)
                            {
                                for (int i = 0; i < lstCourseTutor.Count; i++)
                                {
                                    tutEmail.Add(lstCourseTutor[i].Email);
                                    objComm = new tblCommunication();
                                    //objComm.IsAllTeacher = isAllTutors;
                                    objComm.IsAllCourseTutor = isAllCourses;

                                    objComm.Id = lstCourseTutor[i].Id;
                                    objComm.UserType = "All-Course-Tutor";
                                    objComm.DateCreated = DateTime.Now;
                                    objComm.CreatedBy = "Admin";
                                    objComm.Message = strMsgTutor;
                                    objComm.MessageAllStudents = "-";
                                    objComm.MessageAllTeachers = "-";
                                    objComm.TutorId = lstCourseTutor[i].TutorId;
                                    objComm.CategoryType = CategoryType;

                                    objComm.CourseId = lstCourseTutor[i].CourseId;
                                    db.tblCommunications.InsertOnSubmit(objComm);
                                    db.SubmitChanges();
                                }
                                if (tutEmail.Count > 0)
                                {
                                    SendMailToAllTutor(tutEmail);
                                }
                                return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);
                            }
                        }

                    }
                    #endregion Course Tutor Selected
                }

                #endregion End All Brand Tutors && All Intern Tutors && Selected Course Tutors 

                #endregion End 2 Checkbox tick and different category type


                CatgType = CategoryType.Trim();



                return Json("", JsonRequestBehavior.AllowGet);
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


        // Add Communication For Tutor
        public ActionResult AddCommunicationToAll()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddCommunicationMessageForAll(bool lstInternTutor, bool lstCourseTutor, bool lstCourseStudent, bool lstInternStudent, string strMsgAll)
        {
            try
            {

                db = new CourseDataContext();
                tblCommunication objComm = null;

                if (lstInternTutor == true && lstCourseTutor == true && lstCourseStudent == true && lstInternStudent == true)
                {
                    if (lstInternTutor == true)
                    {

                        objComm = new tblCommunication();
                        ArrayList alEmail = new ArrayList();

                        //var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                        //                   select stdpro
                        //                   ).ToList();

                        var AllInternTutor = (from tut in db.SITSPL_tblTutors
                                              join intntut in db.tblInternTutors
                                              on tut.TutorId equals intntut.TutorId
                                              join intstr in db.SITSPL_tblInternshipStructures
                                              on intntut.InternshipId equals intstr.InternshipId
                                              select new
                                              {
                                                  tut.TutorName,
                                                  tut.TutorEmail,
                                                  intntut.InternTutorId,
                                                  intntut.TutorId,
                                                  intntut.InternshipId
                                              }).Distinct().ToList();

                        //   ArrayList alCourse = new ArrayList();

                        if (AllInternTutor != null)
                        {
                            for (int i = 0; i < AllInternTutor.Count; i++)
                            {
                                alEmail.Add(AllInternTutor[i].TutorEmail);
                                //      alCourse.Add(StudentsWithCourse[i].CourseId);

                                objComm = new tblCommunication();
                                //objComm.IsAllTeacher = isAllTutors;

                                objComm.IsAllCourseStudent = lstCourseStudent;
                                objComm.IsAllInternStudent = lstInternStudent;
                                objComm.IsAllCourseTutor = lstCourseTutor;
                                objComm.IsAllInternTutor = lstInternTutor;

                                objComm.Id = AllInternTutor[i].InternTutorId;
                                objComm.UserType = "All-Intern-Tutor";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = "";
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Intern-Tutor";
                                objComm.InternshipId = AllInternTutor[i].InternshipId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAllTutor(alEmail);
                            }
                        }

                    }


                    if (lstCourseStudent == true)
                    {
                        objComm = new tblCommunication();
                        ArrayList alEmail = new ArrayList();

                        //var AllStudents = (from stdpro in db.SITSPL_tblStudentProfiles
                        //                   select stdpro
                        //                   ).ToList();

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
                                               stdpro.StudentCategoryId,

                                           }).Distinct().ToList();

                        //   ArrayList alCourse = new ArrayList();

                        if (AllStudents != null)
                        {
                            for (int i = 0; i < AllStudents.Count; i++)
                            {
                                alEmail.Add(AllStudents[i].Email);
                                //      alCourse.Add(StudentsWithCourse[i].CourseId);

                                objComm = new tblCommunication();
                                //objComm.IsAllTeacher = isAllTutors;

                                //      bool lstInternTutor,bool lstCourseTutor,bool lstCourseStudent,bool lstInternStudent

                                objComm.IsAllCourseStudent = lstCourseStudent;
                                objComm.IsAllInternStudent = lstInternStudent;
                                objComm.IsAllCourseTutor = lstCourseTutor;
                                objComm.IsAllInternTutor = lstInternTutor;

                                objComm.Id = AllStudents[i].StudentId;
                                objComm.UserType = "All-Students-Course";
                                objComm.DateCreated = DateTime.Now;
                                objComm.CreatedBy = "Admin";
                                objComm.Message = "";
                                objComm.MessageAllStudents = "-";
                                objComm.MessageAllTeachers = "-";
                                objComm.MessageAll = strMsgAll;
                                objComm.CategoryType = "Course";
                                objComm.StudentCategoryId = AllStudents[i].StudentCategoryId;
                                objComm.CourseId = AllStudents[i].CourseId;
                                db.tblCommunications.InsertOnSubmit(objComm);
                                db.SubmitChanges();
                            }
                            if (alEmail.Count > 0)
                            {
                                SendMailToAll(alEmail);
                            }
                        }
                        // return Json(objComm.CommunicationId, JsonRequestBehavior.AllowGet);

                    }

                    if (lstInternStudent == true)
                    {
                        var AllInterns = (from d in db.tblInternApplies
                                          join d1 in db.SITSPL_tblInternshipStructures on d.InternshipStructureId equals d1.InternStructureId
                                          join d2 in db.SITSPL_tblInternships on d1.InternshipId equals d2.InternshipId
                                          select new
                                          {
                                              d.InterApllyId,
                                              d.InternshipStructureId,
                                              d.Name,
                                              d1.InternshipId,
                                              d.IsPublished,
                                              d.Email
                                          }).ToList();

                        ArrayList alEmail = new ArrayList();
                        for (int i = 0; i < AllInterns.Count; i++)
                        {
                            alEmail.Add(AllInterns[i].Email);
                            objComm = new tblCommunication();
                            //objComm.IsAllTeacher = isAllTutors;

                            objComm.IsAllCourseStudent = lstCourseStudent;
                            objComm.IsAllInternStudent = lstInternStudent;
                            objComm.IsAllCourseTutor = lstCourseTutor;
                            objComm.IsAllInternTutor = lstInternTutor;

                            objComm.Id = AllInterns[i].InterApllyId;
                            objComm.UserType = "All-Intern-Students";
                            objComm.DateCreated = DateTime.Now;
                            objComm.CreatedBy = "Admin";
                            objComm.MessageAllStudents = "-";
                            objComm.MessageAllTeachers = "-";
                            objComm.MessageAll = strMsgAll;
                            objComm.CategoryType = "Intern";
                            //   objComm.StudentCategoryId = StudentCategoryId;
                            objComm.InternshipId = AllInterns[i].InternshipId;
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


        [HttpPost]
        public JsonResult GetTutorForInternship(int InternId, int TutorId)
        {
            try
            {
                db = new CourseDataContext();

                //select tut.TutorId, TutorName, TutorEmail, intntut.InternTutorId, intntut.TutorId, intntut.InternshipId from SITSPL_tblTutor as tut
                //INNER JOIN tblInternTutor as intntut
                //on tut.TutorId = intntut.TutorId
                //INNER JOIN SITSPL_tblInternshipStructure as intstr
                //on intstr.InternshipId = intntut.InternshipId;

                if (InternId > 0 && TutorId > 0)
                {
                    var data = (from tut in db.SITSPL_tblTutors
                                join intntut in db.tblInternTutors
                                on tut.TutorId equals intntut.TutorId
                                join intstr in db.SITSPL_tblInternshipStructures
                                on intntut.InternshipId equals intstr.InternshipId
                                where intntut.InternshipId == InternId && intntut.TutorId == TutorId
                                select new
                                {
                                    tut.TutorName,
                                    tut.TutorEmail,
                                    intntut.InternTutorId,
                                    intntut.TutorId,
                                    intntut.InternshipId
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


        public ActionResult CourseListTutor()
        {
            return View();
        }

        public JsonResult CourseListTutorsData()
        {
            try
            {
                db = new CourseDataContext();

                var data = (from cour in db.SITSPL_tblCourses
                            select new
                            {
                                cour
                            }).ToList();

                var list = data.Select(x => new
                {
                    x.cour.CourseId,
                    x.cour.CourseName,
                    x.cour.CreatedBy,
                    DateCreated = x.cour.DateCreated.ToString("dd/MM/yyyy"),
                    x.cour.UpdatedBy,
                    LastUpdated = x.cour.LastUpdated.HasValue ? x.cour.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                    x.cour.IsDeleted,
                    x.cour.DeletedBy,
                    x.cour.DateDeleted,
                    x.cour.IsPublished,
                    x.cour.PublishedBy,
                    DatePublished = x.cour.DatePublished.HasValue ? x.cour.DatePublished.Value.ToString("dd/MM/yyyy") : null,
                    x.cour.DisplayOnWeb
                }).ToList();

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);

            }
        }


        public ActionResult AddTutorResourcesForCourse()
        {
            return View();
        }

        public JsonResult GetTutorResourcesForCourse(int CourseId)
        {
            db = new CourseDataContext();
            var intern = (from instr in db.SITSPL_tblCourseStructures
                          join intr in db.SITSPL_tblCourses
                          on instr.CourseId equals intr.CourseId
                          where instr.CourseId == CourseId
                          select new
                          {
                              instr.CourseId,
                              intr.CourseName
                          }).ToList();

            var intrtutor = (from d in db.SITSPL_tblCourses
                             join inttu in db.tblCourseTutors on d.CourseId equals inttu.CourseId
                             join tut in db.SITSPL_tblTutors on inttu.TutorId equals tut.TutorId
                             where inttu.CourseId == CourseId
                             select new
                             {
                                 inttu.CourseId,
                                 inttu.CourseTutorId,
                                 d.CourseName,
                                 inttu.TutorId,
                                 tut.TutorName

                             }).ToList();

            var resource = (from d in db.SITSPL_tblCourses
                            join res in db.tblCourseResources
                            on d.CourseId equals res.CourseId
                            where res.CourseId == CourseId
                            select new
                            {
                                res.CourseId,
                                d.CourseName,
                                res.ResourceId,
                                res.ResourceName,
                            }).ToList();

            //  intrtutor   , resource            }).ToList();

            var list = new { intern, intrtutor, resource };

            JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
            return Json(result, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ContentResult Upload(string files)
        {
            foreach (string key in Request.Files)
            {
                HttpPostedFileBase postedfile = Request.Files[key];
                string path = Server.MapPath("~/UploadCourseResource/");
                postedfile.SaveAs(path + postedfile.FileName);
            }
            return Content("Success");
        }



        [HttpPost]
        public ContentResult AddTutorForCourse(List<CourseTutorArray> CourseArray, List<DocumentArray> Filearray, int CourseId, List<DeleteCourseTutorArray> deletecoursetutor, List<DeleteCourseResourceArray> deleteCourseResourceArray)
        {
            db = new CourseDataContext();
            if (CourseArray != null)
            {
                for (int i = 0; i < CourseArray.Count; i++)
                {
                    var objCourse = (from d in db.tblCourseTutors where d.CourseTutorId == CourseArray[i].CourseTutorId && d.CourseId == CourseId && d.TutorId == CourseArray[i].TutorId select d).FirstOrDefault();
                    if (objCourse != null)
                    {
                        objCourse.TutorId = CourseArray[i].TutorId;
                        objCourse.CourseId = CourseArray[i].CourseId;
                        objCourse.LastUpdated = DateTime.Now;
                        objCourse.UpdatedBy = "Admin";
                        db.SubmitChanges();
                    }
                    else
                    {
                        tblCourseTutor tutor = new tblCourseTutor();
                        tutor.TutorId = CourseArray[i].TutorId;
                        tutor.CourseId = CourseArray[i].CourseId;
                        tutor.IsPublished = true;
                        tutor.CreatedBy = "Admin";
                        tutor.DateCreated = DateTime.Now;
                        db.tblCourseTutors.InsertOnSubmit(tutor);
                        db.SubmitChanges();

                        Int64 intCourseTutorId = tutor.CourseTutorId;

                        //if (intCourseTutorId > 0)
                        //{
                        //    var Prefix = "IntTut-";
                        //    var PrefixLen = Prefix.ToString().Length;
                        //    var data2 = db.SITSPL_tblUsers.Where(x => x.UserType == "Intern-Tutor").ToList().Count();
                        //    var StdCode = "";
                        //    var LenStdCount = data2.ToString().Length;
                        //    var StdCodeLength = db.SITSPL_tblUsers.Where(x => x.UserType == "Intern-Tutor").ToList().Count() + 1;
                        //    var StdCodeLenBalance = 11 - LenStdCount - PrefixLen;

                        //    if (StdCodeLenBalance > 0)
                        //    {
                        //        if (StdCodeLenBalance.ToString().Length == 3)
                        //        {
                        //            StdCode = Prefix.PadRight(Prefix.Length + 3, '0') + StdCodeLength.ToString();
                        //        }
                        //        else if (StdCodeLenBalance.ToString().Length == 2)
                        //        {
                        //            StdCode = Prefix.PadRight(Prefix.Length + 2, '0') + StdCodeLength.ToString();
                        //        }
                        //        else if (StdCodeLenBalance.ToString().Length == 1)
                        //        {
                        //            StdCode = Prefix.PadRight(Prefix.Length + 1, '0') + StdCodeLength.ToString();
                        //        }
                        //        //var No = StdCode.Substring(7);
                        //        var StudentFinalCode = StdCode;
                        //        var AutoId = StdCodeLength;
                        //        var AutomaticId = AutoId.ToString();
                        //        SITSPL_tblUser user = new SITSPL_tblUser();
                        //        user.UserType = "Intern-Tutor";
                        //        user.Id = intCourseTutorId;
                        //        user.UserPrefix = Prefix;
                        //        user.AutoId = AutomaticId;
                        //        user.CompletedId = StudentFinalCode;
                        //        user.DateCreated = DateTime.Now;
                        //        user.CreatedBy = "Admin";
                        //        user.IsPublished = true;
                        //        db.SITSPL_tblUsers.InsertOnSubmit(user);
                        //        db.SubmitChanges();
                        //    }
                        //}
                    }
                }
            }

            if (deletecoursetutor != null)
            {
                for (int i = 0; i < deletecoursetutor.Count; i++)
                {
                    if (deletecoursetutor[i].TutorId > 0)
                    {
                        db = new CourseDataContext();
                        tblCourseTutor courseTutor = (from d in db.tblCourseTutors where d.CourseTutorId == deletecoursetutor[i].CourseTutorId && d.TutorId == deletecoursetutor[i].TutorId && d.CourseId == CourseId select d).FirstOrDefault();
                        db.tblCourseTutors.DeleteOnSubmit(courseTutor);
                        db.SubmitChanges();

                    }
                }

            }

            if (Filearray != null)
            {
                // InternshipId
                for (int i = 0; i < Filearray.Count; i++)
                {

                    var CourseResource = (from d in db.tblCourseResources where d.CourseId == CourseId && d.ResourceId == Filearray[i].ResourceId select d).FirstOrDefault();
                    if (CourseResource != null)
                    {
                        CourseResource.CourseId = CourseId;
                        CourseResource.ResourceName = Filearray[i].ResourceName;
                        CourseResource.LastUpdated = DateTime.Now;
                        CourseResource.UpdatedBy = "Admin";
                        db.SubmitChanges();
                    }
                    else
                    {
                        db = new CourseDataContext();
                        tblCourseResource resource = new tblCourseResource();
                        resource.ResourceName = Filearray[i].ResourceName;
                        resource.CourseId = CourseId;
                        resource.CreatedBy = "Admin";
                        resource.DateCreated = DateTime.Now;
                        resource.IsPublished = true;
                        db.tblCourseResources.InsertOnSubmit(resource);
                        db.SubmitChanges();
                    }
                }
            }


            if (deleteCourseResourceArray != null)
            {
                for (int i = 0; i < deleteCourseResourceArray.Count; i++)
                {
                    string fullPath = Request.MapPath("~/UploadCourseResource/" + deleteCourseResourceArray[i].ResourceName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    if (deleteCourseResourceArray[i].ResourceId > 0)
                    {
                        db = new CourseDataContext();
                        tblCourseResource coursesResource = (from d in db.tblCourseResources where d.ResourceId == deleteCourseResourceArray[i].ResourceId && d.CourseId == CourseId select d).FirstOrDefault();
                        db.tblCourseResources.DeleteOnSubmit(coursesResource);
                        db.SubmitChanges();
                    }
                }

            }
            return Content("Done");
        }



        #region Feedback Details of students and tutors By Dilshad A. on 21 Oct 2020
        public ActionResult ViewFeedback()
        {
            return View();
        }

        public JsonResult ViewFeedbackDetails()
        {
            try
            {
                db = new CourseDataContext();
                var list = (from d in db.tblFeedbacks
                            join d1 in db.SITSPL_tblStudentProfiles on d.Id equals d1.StudentId
                            select new
                            {
                                d,
                                d1
                            }).ToList();

                var data = list.Select(x => new
                {
                    x.d.FeedbackId,
                    x.d.Id,
                    x.d1.Name,
                    x.d.Message,
                    x.d.Subject,
                    DateCreated = x.d.DateCreated.ToString("dd/MM/yyyy"),
                    x.d.CreatedBy
                }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion END Feedback Details of students and tutors By Dilshad A. on 21 Oct 2020


        // Admin Home 

        public ActionResult Home()
        {
            return View();
        }

        // Admin Student Register

        public ActionResult StudentRegister()
        {
            return View();
        }

        // Admin Tutor Register

        public ActionResult TutorRegister()
        {
            return View();
        }

        // Tutor List

        public ActionResult TutorList()
        {
            return View();
        }

        // Internship List

        public ActionResult InternshipList()
        {
            return View();
        }

        // Show Students

        public ActionResult ShowStudents()
        {
            return View();
        }

        // Admin Tutor

        public ActionResult AdminTutor()
        {
            return View();
        }

        public JsonResult GetCourseHeading(int CourseId)
        {
            try
            {
                db = new CourseDataContext();
                //var coursecont = (from content in db.tblContents
                //            join courdesc in db.tblCourseDescriptions
                //            on content.ContentId equals courdesc.ContentId
                //            where content.CourseId == CourseId 
                //            select new {
                //                content.CourseContentHeading,
                //                content.IsSubHeading,
                //                content.SubHeading
                //            }).ToList();


                var coursecont = (from content in db.tblContents

                                  where content.CourseId == CourseId
                                  select new
                                  {
                                      content.CourseContentHeading,
                                      content.IsSubHeading,
                                      content.SubHeading,
                                      content.ContentId,
                                      content.ShortDescription // Added for display short description (Based on this we can show and update) by Dilshad A. on 31 Oct 2020 t
                                  }).ToList();

                // Here description means bullets point(Content Point of Heading)
                var description = (from cont in db.tblContents
                                   join descrip in db.tblCourseDescriptions on
                                   cont.ContentId equals descrip.ContentId
                                   where cont.CourseId == CourseId
                                   select new
                                   {
                                       cont.ContentId,
                                       descrip.DescriptionId,
                                       descrip.Description,
                                       descrip.IsPublished
                                   }).ToList();

                if (coursecont != null && description != null || description == null)
                {
                    var list = new { coursecont, description };

                    JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                    var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
                if (coursecont != null && description != null || description == null)
                {
                    var list = new { coursecont, description };

                    JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                    var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
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


        #region FAQ (Frequently Asked Questions)

        public ActionResult FrequentlyAskedQuestions()
        {
            if (Session["UserType"] != null)
            {
                if (Session["UserType"].ToString() == "Admin" || Session["UserType"].ToString() == "SuperAdmin")
                {
                    return View();
                }
            }
            return RedirectToAction("Login", "Admin");
        }

        #endregion FAQ (Frequently Asked Questions)

        [HttpPost]
        public JsonResult AddFAQ(tblFAQ objaddfaq)

        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                string Cour = objaddfaq.Question;
                var data = db.tblFAQs.Select(x => new { x.Question }).ToList();
                var Cours = data.Select(x => x.Question);

                if (Cour != null && !Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    tblFAQ addfaq = new tblFAQ();
                    addfaq.Question = objaddfaq.Question;
                    addfaq.Answer = objaddfaq.Answer;
                    addfaq.QuestionType = objaddfaq.QuestionType;
                    addfaq.IsPublished = objaddfaq.IsPublished;
                    addfaq.DateCreated = DateTime.Now;
                    addfaq.CreatedBy = "Admin";
                    db.tblFAQs.InsertOnSubmit(addfaq);
                    db.SubmitChanges();
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);

                }

                else if (Cour != null && Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    ViewBag.InsertMessage = "FAQ already exists";
                    result = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("empty", JsonRequestBehavior.AllowGet);
                }
            }

            //try
            //{
            //    db = new CourseDataContext();
            //    bool result = false;
            //    tblFAQ addfaq = new tblFAQ();
            //    addfaq.Question = objaddfaq.Question;
            //    addfaq.QuestionType = objaddfaq.QuestionType;
            //    addfaq.Answer = objaddfaq.Answer;
            //    addfaq.IsPublished = objaddfaq.IsPublished;
            //    addfaq.DateCreated = DateTime.Now;
            //    addfaq.CreatedBy = "Admin";
            //    db.tblFAQs.InsertOnSubmit(addfaq);
            //    db.SubmitChanges();
            //    result = true;
            //    return Json(result, JsonRequestBehavior.AllowGet);
            //}
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #region Get FAQ Data On Init

        public JsonResult FAQinit()
        {
            try
            {
                db = new CourseDataContext();
                var list = (from faq in db.tblFAQs
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

        #endregion End Get FAQ Data On Init


        [HttpPost]

        public JsonResult UpdateFAQ(int FAQId, string Question, string Answer, string QuestionType, bool IsPublished)
        {
            try
            {
                db = new CourseDataContext();
                bool isResult = false;

                if (FAQId > 0)
                {
                    tblFAQ updatefaq = db.tblFAQs.Where(x => x.FAQId == FAQId).FirstOrDefault();
                    var data = db.tblFAQs.Select(x => x.Question).ToList();
                    var alldata = db.tblFAQs.Where(x => x.FAQId == FAQId).Select(x => new
                    {
                        x.FAQId,
                        x.Question
                    }).FirstOrDefault();

                    if (data.Contains(Question.Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        if (FAQId == alldata.FAQId && Question.ToLower() == alldata.Question.ToLower())
                        {
                            updatefaq.Question = Question;
                            updatefaq.Answer = Answer;
                            updatefaq.IsPublished = IsPublished;
                            updatefaq.QuestionType = QuestionType;
                            updatefaq.LastUpdated = DateTime.Now;
                            updatefaq.UpdatedBy = "Admin";
                            db.SubmitChanges();
                            isResult = true;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            isResult = false;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Question.Trim()))
                        {
                            updatefaq.Question = Question;
                            updatefaq.Answer = Answer;
                            updatefaq.IsPublished = IsPublished;
                            updatefaq.QuestionType = QuestionType;
                            updatefaq.LastUpdated = DateTime.Now;
                            updatefaq.UpdatedBy = "Admin";
                            db.SubmitChanges();
                            isResult = true;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);

            }


            //try
            //{
            //    db = new CourseDataContext();
            //    bool result = false;
            //    if(FAQId >0)
            //    { 
            //    tblFAQ objfaq = db.tblFAQs.Where(x => x.FAQId == FAQId).FirstOrDefault();
            //    if(objfaq != null){
            //        objfaq.Question = Question;
            //        objfaq.Answer = Answer;
            //        objfaq.QuestionType = QuestionType;
            //        objfaq.IsPublished = IsPublished;
            //        objfaq.UpdatedBy = "Admin";
            //        objfaq.LastUpdated = DateTime.Now;
            //        db.SubmitChanges();
            //            result = true;
            //            return Json(result, JsonRequestBehavior.AllowGet);
            //    }
            //    }
            //    return Json("", JsonRequestBehavior.AllowGet);

            //}
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]

        public JsonResult DeleteFAQ(int Id)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                tblFAQ deletefaq = db.tblFAQs.Where(x => x.FAQId == Id).FirstOrDefault();
                if (deletefaq != null)
                {
                    db.tblFAQs.DeleteOnSubmit(deletefaq);
                    db.SubmitChanges();
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


        public ActionResult ViewReferFriends()
        {
            return View();
        }

        public JsonResult GetReferFriendDetails()
        {
            try
            {
                db = new CourseDataContext();
                var list= (from refer in db.tblRefers 
                           select new
                           {
                               refer
                           }).ToList();

                var data = list.Select(x => new
                {
                    x.refer.ReferId,
                    x.refer.ReferBy,
                    x.refer.EmailReferBy,
                    x.refer.MobileRefereBy,
                    x.refer.ReferTo,
                    x.refer.EmailReferTo,
                    x.refer.MobileReferTo,
                    DateCreated = x.refer.DateCreated.ToString("dd/MM/yyyy"),
                    x.refer.CreatedBy

                }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message,JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewEnquiries()
        {
            return View();
        }

      //  var sub = (from std in db.SITSPL_tblStudentProfiles
        //                   join cd in db.SITSPL_tblCourseDetails
        //                   on std.StudentId equals cd.StudentId into eGroup
        //                   from stdpro in eGroup.DefaultIfEmpty()
        //                   join cont in db.SITSPL_tblContacts
        //                   on std.StudentId equals cont.UserId into eGroup2
        //                   from stdpro2 in eGroup2.DefaultIfEmpty()
        //                   join c in db.SITSPL_tblCourses
        //                   on stdpro.CourseId equals c.CourseId into eGroup3
        //                   from cour in eGroup3.DefaultIfEmpty()
        //                   join stdcatg in db.tblStudentCategories
        //                   on std.StudentCategoryId equals stdcatg.StudentCategoryId
        //                   into eGroup4
        //                   from stdcat in eGroup4.DefaultIfEmpty()
        //                   select new
        //                   {
        //                       std,
        //                       stdpro,
        //                       stdpro2,
        //                       cour,
        //                       stdcat

                       //                   }).ToList();

        public JsonResult GetUserEnquiryDetails()
        {
            try
            {
                db = new CourseDataContext();
                var data = db.SITSPL_UserEnquiryDetailsToAdmin().ToList();
                if(data != null)
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


        //       var sub = (from enq in db.tblEnquiries
        //                  join cour in db.SITSPL_tblCourses
        //                  on enq.CourseId equals cour.CourseId into eGroup
        //                  from coursepro in eGroup.DefaultIfEmpty()
        //                  join intr in db.SITSPL_tblInternships
        //                  on enq.InternshipId equals intr.InternshipId into eGroup2
        //                  from internpro in eGroup2.DefaultIfEmpty()
        //                  join stcatg in db.tblStudentCategories
        //                  on enq.StudentCategoryId equals stcatg.StudentCategoryId into eGroup3
        //                  from categpro in eGroup3.DefaultIfEmpty()
        //                  select new {
        //                      enq,
        //                      coursepro,
        //                      internpro,
        //                      categpro
        //                  }).ToList();


        //       var course = sub.Select(x => x.coursepro).ToList();

        //       var intern = sub.Select(x => x.internpro).ToList();

        //       var category = sub.Select(x => x.categpro).ToList();

        //       if(course[0] != null && category[0] != null){
        //           var coursecatg = sub.Select(x => new
        //           {
        //               x.coursepro.CourseName,
        //               x.enq.EnquiryId,
        //               x.enq.CandidateType,
        //               x.enq.Name,
        //               x.enq.Mobile,
        //               x.enq.Email,
        //               x.enq.EnquiryType,
        //               x.enq.StudentCategoryId,
        //               x.enq.CourseId,
        //               x.enq.CreatedBy,
        //DateCreated =  x.enq.DateCreated.ToString("dd/MM/yyyy"),    
        //               x.categpro.CategoryName,
        //           }).ToList();

        //           return Json(coursecatg, JsonRequestBehavior.AllowGet);
        //       }
        //       else if(course[0] == null && category[0] == null && intern[0] != null)
        //       {
        //           var interncatg = sub.Select(x => new
        //           {
        //               x.internpro.InternshipName,
        //               x.enq.EnquiryId,
        //               x.enq.CandidateType,
        //               x.enq.Name,
        //               x.enq.Mobile,
        //               x.enq.Email,
        //               x.enq.EnquiryType,                        
        //               x.enq.CreatedBy,


        //           }).ToList();

        //           return Json(interncatg, JsonRequestBehavior.AllowGet);
        //       }
        //       else if(course[0] == null && category[0] == null && intern[0] == null)
        //       {
        //           var others = sub.Select(x => new
        //           {
        //               x.enq.EnquiryId,
        //               x.enq.CandidateType,
        //               x.enq.Name,
        //               x.enq.Mobile,
        //               x.enq.Email,
        //               x.enq.EnquiryType,
        //               x.enq.CreatedBy,
        //               DateCreated = x.enq.DateCreated.ToString("dd/MM/yyyy")
        //           }).ToList();

        //           return Json(others, JsonRequestBehavior.AllowGet);

        //       }
        //       return Json("", JsonRequestBehavior.AllowGet);


        //var list = (from enquiry in db.tblEnquiries
        //            join cour in db.SITSPL_tblCourses
        //            on enquiry.CourseId equals cour.CourseId
        //            join intr in db.SITSPL_tblInternships
        //            on enquiry.InternshipId.Value equals intr.InternshipId
        //            join stdcatg in db.tblStudentCategories
        //            on enquiry.StudentCategoryId equals stdcatg.StudentCategoryId
        //            select new
        //            {
        //                enquiry,
        //                cour,
        //                intr
        //            }).ToList();

        //var data = list.Select(x => new
        //{
        //    x.enquiry.Name,
        //    x.enquiry.CandidateType,
        //    x.enquiry.Mobile,
        //    x.enquiry.Email,
        //    x.enquiry.EnquiryType,
        //    x.enquiry.StudentCategoryId,
        //    x.enquiry.CourseId,
        //    x.cour.CourseName,
        //    x.intr.InternshipName,
        //    DateCreated = x.enquiry.DateCreated.ToString("dd/MM/yyyy"),
        //    x.enquiry.CreatedBy

        //}).ToList();
        //return Json(data, JsonRequestBehavior.AllowGet);





        public ActionResult ViewContacts()
        {
            return View();
        }

        public JsonResult GetUserContactDetails()
        {
            try
            {
                db = new CourseDataContext();
                var list = (from contact in db.tblContacts
                            select new
                            {
                                contact
                            }).ToList();

                var data = list.Select(x => new
                {
                    x.contact.ContactId,
                    x.contact.Name,
                    x.contact.Email,
                    x.contact.Mobile,
                    x.contact.Country,
                    x.contact.Subject,
                    x.contact.Message,
                    DateCreated = x.contact.DateCreated.ToString("dd/MM/yyyy"),
                    x.contact.CreatedBy

                }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message,JsonRequestBehavior.AllowGet);
            }
        }

        #region View Resources for Dowload whic is uploaded by Brand Tutor by Dilshad A. on 05 Dec 2020       
        public ActionResult ViewResourceForDownload()
        {
            return View();
        }

        public JsonResult ViewResourceForDownloadDetails()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from d in db.tblPublicResources
                            join d1 in db.SITSPL_tblTutors on d.TutorId equals d1.TutorId
                            join d3 in db.tblDownloadResourceTypes on d.DownloadResourceId equals d3.DownloadResourceId
                            select new
                            {
                                d,
                                d1,d3
                            }).OrderByDescending(x => x.d.PublicResourceId).ToList();

                var list = data.Select(x => new
                {
                    x.d.PublicResourceId,
                    x.d1.TutorName,
                    x.d.ResourceName,
                    x.d.ResourceTitle,
                    x.d3.DownloadResourceName,
                    x.d.CreatedBy,
                    DateCreated = x.d.DateCreated.ToString("dd/MM/yyyy"),
                    x.d.UpdatedBy,
                    LastUpdated = x.d.LastUpdated.HasValue ? x.d.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                    x.d.IsPublished
                }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END View Resources for Dowload which is uploaded by Brand Tutor by Dilshad A. on 05 Dec 2020

        #region Download Resource which is uploaded for Public Download By Dilshad on 05 Dec 2020
        public FileResult DownloadPublicResource(string strFileName)
        {
            string strFilePath = ConfigurationManager.AppSettings["downloadPublicResources"];// Can download from Header on Public View(or On Tutor Dashboard) of Brand Tutor
            strFilePath = Server.MapPath(strFilePath);

            if (!string.IsNullOrEmpty(strFilePath))
            {
                if (System.IO.File.Exists(strFilePath + strFileName))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(strFilePath + strFileName);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, strFileName);
                }
            }
            return null;
        }
        #endregion END Download Resource which is uploaded for Public Download By Dilshad on 05 Dec 2020

        #region Update Public Resource for Download(Active and Dective) Which is added by Brand Tutor By Dilshad A. on 05 Dec 2020
        public JsonResult UpdateResourseForDownload(Int64 intResourceId, bool isPublished)
        {
            try
            {
                db = new CourseDataContext();
                var data = db.tblPublicResources.FirstOrDefault(x => x.PublicResourceId == intResourceId);
                if (data != null)
                {
                    data.IsPublished = isPublished;
                    data.LastUpdated = DateTime.Now;
                    db.SubmitChanges();
                    return Json("Updated", JsonRequestBehavior.AllowGet);
                }
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Update Public Resource for Download(Active and Dective) Which is added by Brand Tutor By Dilshad A. on 05 Dec 2020

        // Delete Activity
        #region Delete Activities
        [HttpPost]
        public JsonResult DeleteAllActivities(int Id)
        {
            try
            {
                db = new CourseDataContext();
                if (Id > 0)
                {
                    tblActivity activity = db.tblActivities.Where(x => x.ActivityId == Id).FirstOrDefault();
                    if (activity != null)
                    {
                        db.tblActivities.DeleteOnSubmit(activity);
                        db.SubmitChanges();
                        return Json(Id.ToString(), JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                string str = ex.Message;
                return Json("stractivitydepend", JsonRequestBehavior.AllowGet);
                //  return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Delete Activities

        #region Delete Intern Student
        [HttpPost]
        public JsonResult DeleteInternStudentsByAdmin(int Id)
        {
            try
            {
                db = new CourseDataContext();
                if (Id > 0)
                {
                    tblInternApply apply = db.tblInternApplies.Where(x => x.InterApllyId == Id).FirstOrDefault();
                    if (apply != null)
                    {
                        db.tblInternApplies.DeleteOnSubmit(apply);
                        db.SubmitChanges();
                        return Json(Id.ToString(), JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                string str = ex.Message;
                string err = str.Substring(0, 61);

                if (err == "The DELETE statement conflicted with the REFERENCE constraint")
                {
                    return Json("strinternstudentdepend", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ex.Message, JsonRequestBehavior.AllowGet);
                }
               
                //  return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Delete Intern Student

        #region Batch Module Code


        #region Add Batch Size View
        public ActionResult AddBatch()
        {
            return View();
        }
        #endregion End Add Batch Size View

        #region Add Batch Post Method
        [HttpPost]
        public JsonResult SaveBatch(SITSPL_tblBatch addBatch)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                string Cour = addBatch.BatchSize;
                var data = db.SITSPL_tblBatches.Select(x => new { x.BatchSize }).ToList();
                var Cours = data.Select(x => x.BatchSize);

                if (Cour != null && !Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    SITSPL_tblBatch addtblBatch = new SITSPL_tblBatch();
                    addtblBatch.BatchSize = addBatch.BatchSize;
                    addtblBatch.BatchType = addBatch.BatchType;
                    addtblBatch.IsPublished = addBatch.IsPublished;
                    addtblBatch.DateCreated = DateTime.Now;
                    addtblBatch.CreatedBy = "Admin";
                    db.SITSPL_tblBatches.InsertOnSubmit(addtblBatch);
                    db.SubmitChanges();
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);

                }

                else if (Cour != null && Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    ViewBag.InsertMessage = "Batch Size already exists";
                    result = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("empty", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Add Batch Post Method

        #region Get BatchSize Details From Db
        public JsonResult GetBatch()
        {
            try
            {
                db = new CourseDataContext();
                var sub = (from batch in db.SITSPL_tblBatches
                           select new
                           {
                               batch
                           }).ToList();
                var data = sub.Select(x => new
                {
                    x.batch.BatchId,
                    x.batch.BatchSize,
                    DateCreated = x.batch.DateCreated.ToString("dd/MM/yyyy"),
                    x.batch.CreatedBy,
                    Description = x.batch.BatchType,
                    x.batch.IsPublished,
                    LastUpdated = x.batch.LastUpdated.HasValue ? x.batch.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                    x.batch.UpdatedBy
                }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Get BatchSize Details From Db

        #region Update Batch
        [HttpPost]
        public JsonResult UpdateBatch(SITSPL_tblBatch updatebatch)
        {
            try
            {
                db = new CourseDataContext();
                bool isResult = false;

                if (updatebatch.BatchId > 0)
                {
                    SITSPL_tblBatch batchobj = db.SITSPL_tblBatches.Where(x => x.BatchId == updatebatch.BatchId).FirstOrDefault();
                    var data = db.SITSPL_tblBatches.Select(x => x.BatchSize).ToList();
                    var alldata = db.SITSPL_tblBatches.Where(x => x.BatchId == updatebatch.BatchId).Select(x => new
                    {
                        x.BatchId,
                        x.BatchSize
                    }).FirstOrDefault();

                    if (data.Contains(updatebatch.BatchSize.Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        if (updatebatch.BatchId == alldata.BatchId && updatebatch.BatchSize.ToLower() == alldata.BatchSize.ToLower())
                        {
                            batchobj.BatchSize = updatebatch.BatchSize;
                            batchobj.BatchType = updatebatch.BatchType;
                            batchobj.IsPublished = updatebatch.IsPublished;
                            batchobj.LastUpdated = DateTime.Now;
                            batchobj.UpdatedBy = "Admin";
                            db.SubmitChanges();
                            isResult = true;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            isResult = false;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(updatebatch.BatchSize.Trim()))
                        {
                            batchobj.BatchSize = updatebatch.BatchSize;
                            batchobj.BatchType = updatebatch.BatchType;
                            batchobj.IsPublished = updatebatch.IsPublished;
                            batchobj.LastUpdated = DateTime.Now;
                            batchobj.UpdatedBy = "Admin";
                            db.SubmitChanges();
                            isResult = true;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Update Batch

        #region Delete Batch
        [HttpPost]
        public JsonResult DeleteBatch(int Id)
        {
            try
            {
                db = new CourseDataContext();
                if (Id > 0)
                {
                    SITSPL_tblBatch delbatch = db.SITSPL_tblBatches.Where(x => x.BatchId == Id).FirstOrDefault();
                    if (delbatch != null)
                    {
                        db.SITSPL_tblBatches.DeleteOnSubmit(delbatch);
                        db.SubmitChanges();
                        return Json(Id.ToString(), JsonRequestBehavior.AllowGet);
                    }
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = new JavaScriptSerializer().Serialize(ex.Message.ToString());
                string str = ex.Message;
                string err = str.Substring(0, 61);

                if (err == "The DELETE statement conflicted with the REFERENCE constraint")
                {
                    return Json("strdepend", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ex.Message, JsonRequestBehavior.AllowGet);
                }
                //  return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Delete Batch

        #endregion END Batch Module Code


        #region Add Batch Group

        public ActionResult AddBatchGroup()
        {
            return View();
        }

        #endregion End Add Batch Group

        #region Add Batch Post Method
        [HttpPost]
        public JsonResult SaveBatchGroup(SITSPL_tblBatchGroup addBatchGroup)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                string Cour = addBatchGroup.BatchName;
                var data = db.SITSPL_tblBatchGroups.Select(x => new { x.BatchName }).ToList();
                var Cours = data.Select(x => x.BatchName);

                if (Cour != null && !Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    SITSPL_tblBatchGroup addtblBatch = new SITSPL_tblBatchGroup();
                    addtblBatch.BatchName = addBatchGroup.BatchName;
                    addtblBatch.BatchDescription = addBatchGroup.BatchDescription;
                    addtblBatch.IsPublished = addBatchGroup.IsPublished;
                    addtblBatch.DateCreated = DateTime.Now;
                    addtblBatch.CreatedBy = "Admin";
                    db.SITSPL_tblBatchGroups.InsertOnSubmit(addtblBatch);
                    db.SubmitChanges();
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);

                }

                else if (Cour != null && Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    ViewBag.InsertMessage = "Batch Name already exists";
                    result = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("empty", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Add Batch Post Method

        #region Get BatchSize Details From Db
        public JsonResult GetBatchGroup()
        {
            try
            {
                db = new CourseDataContext();
                var sub = (from batch in db.SITSPL_tblBatchGroups
                           select new
                           {
                               batch
                           }).ToList();
                var data = sub.Select(x => new
                {
                    x.batch.BatchGroupId,
                    x.batch.BatchName,
                    DateCreated = x.batch.DateCreated.ToString("dd/MM/yyyy"),
                    x.batch.CreatedBy,
                    Description = x.batch.BatchDescription,
                    x.batch.IsPublished,
                    LastUpdated = x.batch.LastUpdated.HasValue ? x.batch.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                    x.batch.UpdatedBy
                }).OrderByDescending(x => x.BatchGroupId).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Get BatchGroup Details From Db

        #region Update Batch Group
        [HttpPost]
        public JsonResult UpdateBatchGroup(SITSPL_tblBatchGroup updatebatch)
        {
            try
            {
                db = new CourseDataContext();
                bool isResult = false;

                if (updatebatch.BatchGroupId > 0)
                {
                    SITSPL_tblBatchGroup batchobj = db.SITSPL_tblBatchGroups.Where(x => x.BatchGroupId == updatebatch.BatchGroupId).FirstOrDefault();
                    var data = db.SITSPL_tblBatchGroups.Select(x => x.BatchName).ToList();
                    var alldata = db.SITSPL_tblBatchGroups.Where(x => x.BatchGroupId == updatebatch.BatchGroupId).Select(x => new
                    {
                        x.BatchGroupId,
                        x.BatchName
                    }).FirstOrDefault();

                    if (data.Contains(updatebatch.BatchName.Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        if (updatebatch.BatchGroupId == alldata.BatchGroupId && updatebatch.BatchName.ToLower() == alldata.BatchName.ToLower())
                        {
                            batchobj.BatchName = updatebatch.BatchName;
                            batchobj.BatchDescription = updatebatch.BatchDescription;
                            batchobj.IsPublished = updatebatch.IsPublished;
                            batchobj.LastUpdated = DateTime.Now;
                            batchobj.UpdatedBy = "Admin";
                            db.SubmitChanges();
                            isResult = true;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            isResult = false;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(updatebatch.BatchName.Trim()))
                        {
                            batchobj.BatchName = updatebatch.BatchName;
                            batchobj.BatchDescription = updatebatch.BatchDescription;
                            batchobj.IsPublished = updatebatch.IsPublished;
                            batchobj.LastUpdated = DateTime.Now;
                            batchobj.UpdatedBy = "Admin";
                            db.SubmitChanges();
                            isResult = true;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Update Batch

        #region Delete Batch Group
        [HttpPost]
        public JsonResult DeleteBatchGroup(int Id)
        {
            try
            {
                db = new CourseDataContext();
                if (Id > 0)
                {
                    SITSPL_tblBatchGroup delbatch = db.SITSPL_tblBatchGroups.Where(x => x.BatchGroupId == Id).FirstOrDefault();
                    if (delbatch != null)
                    {
                        db.SITSPL_tblBatchGroups.DeleteOnSubmit(delbatch);
                        db.SubmitChanges();
                        return Json(Id.ToString(), JsonRequestBehavior.AllowGet);
                    }
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = new JavaScriptSerializer().Serialize(ex.Message.ToString());
                string str = ex.Message;
                string err = str.Substring(0, 61);

                if (err == "The DELETE statement conflicted with the REFERENCE constraint")
                {
                    return Json("strdepend", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ex.Message, JsonRequestBehavior.AllowGet);
                }
                //  return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End Delete Batch


        #region SubCategory

        public ActionResult SubCategory()
        {
            return View();
        }

        #endregion End SubCategory

        [HttpPost]
        public JsonResult SaveStudentSubCategory(tblStudentSubCategory addsubcategory)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                string Cour = addsubcategory.SubCategoryName;
                var data = db.tblStudentSubCategories.Select(x => new { x.SubCategoryName}).ToList();
                var Cours = data.Select(x => x.SubCategoryName);

                if (Cour != null && !Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    tblStudentSubCategory addStudentSubCategory = new tblStudentSubCategory();
                    addStudentSubCategory.SubCategoryName = addsubcategory.SubCategoryName;
                    addStudentSubCategory.StudentCategoryId = addsubcategory.StudentCategoryId;
                    addStudentSubCategory.Description = addsubcategory.Description;
                    addStudentSubCategory.IsPublished = addsubcategory.IsPublished;
                    addStudentSubCategory.DateCreated = DateTime.Now;
                    addStudentSubCategory.CreatedBy = "Admin";
                    db.tblStudentSubCategories.InsertOnSubmit(addStudentSubCategory);
                    db.SubmitChanges();
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);

                }

                else if (Cour != null && Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    ViewBag.InsertMessage = "Student Sub Category already exists";
                    result = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("empty", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetStudentSubCategories()
        {
            try
            {
                db = new CourseDataContext();
                var sub = (from stdsubcatg in db.tblStudentSubCategories
                           join catg in db.tblStudentCategories
                           on stdsubcatg.StudentCategoryId equals catg.StudentCategoryId
                           select new
                           {
                               stdsubcatg,
                               catg
                           }).ToList();
                var data = sub.Select(x => new
                {
                    x.stdsubcatg.StudedetSubCategoryId,
                    x.stdsubcatg.SubCategoryName,
                    x.stdsubcatg.StudentCategoryId,
                    x.catg.CategoryName,
                    DateCreated = x.stdsubcatg.DateCreated.ToString("dd/MM/yyyy"),
                    x.stdsubcatg.CreatedBy,
                    x.stdsubcatg.Description,
                    x.stdsubcatg.IsPublished,
                    LastUpdated = x.stdsubcatg.LastUpdated.HasValue ? x.stdsubcatg.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                    x.stdsubcatg.UpdatedBy

                }).OrderByDescending(x=>x.StudedetSubCategoryId).ToList();


                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult EditStudentSubCategory(int SubCategoryId)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from stdsubcatg in db.tblStudentSubCategories
                            join catg in db.tblStudentSubCategories
                            on stdsubcatg.StudentCategoryId equals catg.StudentCategoryId
                            where stdsubcatg.StudedetSubCategoryId == SubCategoryId
                            select new
                            {
                                stdsubcatg.StudedetSubCategoryId,
                                stdsubcatg.StudentCategoryId,
                                stdsubcatg.SubCategoryName,
                                stdsubcatg.Description,
                                stdsubcatg.IsPublished
                            }).FirstOrDefault();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message,JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult UpdateStudentSubCategory(tblStudentSubCategory updatesubcategory)
        {
            try
            {
                db = new CourseDataContext();
                bool isResult = false;

                if (updatesubcategory.StudedetSubCategoryId > 0)
                {
                    tblStudentSubCategory studentsubCategory = db.tblStudentSubCategories.Where(x => x.StudedetSubCategoryId == updatesubcategory.StudedetSubCategoryId).FirstOrDefault();
                    var data = db.tblStudentSubCategories.Select(x => x.SubCategoryName).ToList();
                    var alldata = db.tblStudentSubCategories.Where(x => x.StudedetSubCategoryId == updatesubcategory.StudedetSubCategoryId).Select(x => new
                    {
                        x.StudedetSubCategoryId,
                        x.SubCategoryName
                    }).FirstOrDefault();

                    if (data.Contains(updatesubcategory.SubCategoryName.Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        if (updatesubcategory.StudedetSubCategoryId == alldata.StudedetSubCategoryId && updatesubcategory.SubCategoryName.ToLower() == alldata.SubCategoryName.ToLower())
                        {
                            studentsubCategory.SubCategoryName = updatesubcategory.SubCategoryName;
                            studentsubCategory.StudentCategoryId = updatesubcategory.StudentCategoryId;
                            studentsubCategory.Description = updatesubcategory.Description;
                            studentsubCategory.IsPublished = updatesubcategory.IsPublished;
                            studentsubCategory.LastUpdated = DateTime.Now;
                            studentsubCategory.UpdatedBy = "Admin";
                            db.SubmitChanges();
                            isResult = true;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            isResult = false;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(updatesubcategory.SubCategoryName.Trim()))
                        {
                            studentsubCategory.SubCategoryName = updatesubcategory.SubCategoryName;
                            studentsubCategory.StudentCategoryId = updatesubcategory.StudentCategoryId;
                            studentsubCategory.Description = updatesubcategory.Description;
                            studentsubCategory.IsPublished = updatesubcategory.IsPublished;
                            studentsubCategory.LastUpdated = DateTime.Now;
                            studentsubCategory.UpdatedBy = "Admin";
                            db.SubmitChanges();
                            isResult = true;
                            return Json(isResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // Delete Sub Category

        [HttpPost]
        public JsonResult DeleteStudentSubCategory(int Id)
        {
            try
            {
                db = new CourseDataContext();
                if (Id > 0)
                {
                    tblStudentSubCategory subcategory = db.tblStudentSubCategories.Where(x => x.StudedetSubCategoryId == Id).FirstOrDefault();
                    if (subcategory != null)
                    {
                        db.tblStudentSubCategories.DeleteOnSubmit(subcategory);
                        db.SubmitChanges();
                        return Json(Id.ToString(), JsonRequestBehavior.AllowGet);
                    }
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = new JavaScriptSerializer().Serialize(ex.Message.ToString());
                string str = ex.Message;
                string err = str.Substring(0, 61);

                if (err == "The DELETE statement conflicted with the REFERENCE constraint")
                {
                    return Json("strsubcatgdepend", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ex.Message, JsonRequestBehavior.AllowGet);
                }


                //  return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetAllCourseByStudentSubCategoryId(int CategoryId,int SubCategoryId)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from stdpro in db.SITSPL_tblStudentProfiles
                            join coursedet in db.SITSPL_tblCourseDetails
                           on stdpro.StudentId equals coursedet.StudentId
                            join cs in db.SITSPL_tblCourseStructures
                            on stdpro.StudentSubCategoryId equals cs.StudentSubCategoryId
                            join cour in db.SITSPL_tblCourses
                            on cs.CourseId equals cour.CourseId
                            where cs.StdCatgId == CategoryId && cs.StudentSubCategoryId == SubCategoryId
                            select new
                            {
                               //  stdpro.StudentId,
                              //  stdpro.StudentCategoryId,
                              //  stdpro.StudentSubCategoryId,
                                coursedet.CourseId,
                                cour.CourseName

                            }).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message,JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetAllCourseByStudentSubCategoryIdForTutor(int CategoryId, int SubCategoryId)
        {
            try
            {
                db = new CourseDataContext();
                var data = (from stdpro in db.SITSPL_tblStudentProfiles
                            join coursedet in db.SITSPL_tblCourseDetails
                           on stdpro.StudentId equals coursedet.StudentId
                            join cs in db.SITSPL_tblCourseStructures
                            on stdpro.StudentSubCategoryId equals cs.StudentSubCategoryId
                            join cour in db.SITSPL_tblCourses
                            on cs.CourseId equals cour.CourseId
                            where cs.StdCatgId == CategoryId && cs.StudentSubCategoryId == SubCategoryId
                            select new
                            {
                            //    stdpro.StudentId,
                            //    stdpro.StudentCategoryId,
                           //     stdpro.StudentSubCategoryId,
                                coursedet.CourseId,
                                cour.CourseName

                            }).Distinct().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult CourseStudentToAdmin()
        {
            return View();
        }

        public JsonResult GetCourseStudentToAdminMessage()
        {
            try
            {
                db = new CourseDataContext();
                int intUserId = 0;
                Int32.TryParse(Session["UserId"].ToString(), out intUserId);
                var list = (from comm in db.tblCommunications
                            join std in db.SITSPL_tblStudentProfiles
                            on comm.Id equals std.StudentId
                            join bat in db.SITSPL_tblBatches
                            on std.BatchId equals bat.BatchId
                            join batgrp in db.SITSPL_tblBatchGroups
                            on std.BatchGroupId equals batgrp.BatchGroupId
                            join cd in db.SITSPL_tblCourseDetails
                            on std.StudentId equals cd.StudentId
                            join cour in db.SITSPL_tblCourses
                            on cd.CourseId equals cour.CourseId
                            //join us in db.SITSPL_tblUsers
                            //on comm.AdminId equals us.UserId
                            where comm.CreatedBy == "CourseStudent" && comm.AdminId == intUserId && comm.CategoryType == "CourseStudentToAdmin"
                        //    && comm.UserType == "Admin" || comm.UserType == "SuperAdmin"
                            select new
                            {
                                comm,
                                std,
                                bat,
                                batgrp,
                                cd,
                                cour

                            }).ToList();

                var data = list.Select(x => new
                {
                    x.comm.CommunicationId,
                    x.comm.CategoryType,
                    x.cd.CourseId,
                    x.cour.CourseName,
                    x.bat.BatchSize,
                    x.batgrp.BatchName,
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

        public ActionResult CourseTutorToAdmin()
        {
            return View();
        }


        public JsonResult GetCourseTutorToAdminMessage()
        {
            try
            {
                db = new CourseDataContext();


                int intUserId = 0;
                Int32.TryParse(Session["UserId"].ToString(), out intUserId);

                var list = (from comm in db.tblCommunications

                            join tut in db.SITSPL_tblTutors
                            on comm.TutorId.Value equals tut.TutorId
                            join coursetut in db.tblCourseTutors
                            on tut.TutorId equals coursetut.TutorId
                            join cour in db.SITSPL_tblCourses
                            on coursetut.CourseId equals cour.CourseId
                            //join us in db.SITSPL_tblUsers
                            //on comm.AdminId equals us.UserId
                            where comm.CreatedBy == "CourseTutor" && comm.AdminId == intUserId && comm.CategoryType == "CourseTutorToAdmin"
                            // && comm.UserType == "Admin" || comm.UserType == "SuperAdmin"
                            select new
                            {
                                comm,
                                tut,
                                cour

                            }).ToList();

                var data = list.Select(x => new
                {
                    x.comm.CommunicationId,
                    x.comm.CategoryType,
                    x.cour.CourseId,
                    x.cour.CourseName,
                    x.tut.TutorName,
                    x.tut.TutorEmail,

                    DOB = x.tut.TutorDOB.ToString("dd/MM/yyyy"),
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


        public ActionResult BrandTutorToAdmin()
        {
            return View();
        }


        public JsonResult GetBrandTutorToAdminMessage()
        {
            try
            {
                db = new CourseDataContext();
                int intUserId = 0;
                Int32.TryParse(Session["UserId"].ToString(), out intUserId);
                var list = (from comm in db.tblCommunications
                            join tut in db.SITSPL_tblTutors
                            on comm.TutorId.Value equals tut.TutorId
                            join coursetut in db.tblCourseTutors
                            on tut.TutorId equals coursetut.TutorId
                            join cour in db.SITSPL_tblCourses
                            on coursetut.CourseId equals cour.CourseId
                            //join us in db.SITSPL_tblUsers
                            //on comm.AdminId equals us.UserId
                            where comm.CreatedBy == "BrandTutor" && comm.AdminId == intUserId && comm.CategoryType == "BrandTutorToAdmin"
                            // && comm.UserType == "Admin" || comm.UserType == "SuperAdmin"
                            select new
                            {
                                comm,
                                tut,
                                cour

                            }).ToList();

                var data = list.Select(x => new
                {
                    x.comm.CommunicationId,
                    x.comm.CategoryType,
                    x.cour.CourseId,
                    x.cour.CourseName,
                    x.tut.TutorName,
                    x.tut.TutorEmail,

                    DOB = x.tut.TutorDOB.ToString("dd/MM/yyyy"),
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


        public ActionResult BrandStudentToAdmin()
        {
            return View();
        }

        public JsonResult GetBrandStudentToAdminMessage()
        {
            try
            {
                db = new CourseDataContext();
                int intUserId = 0;
                Int32.TryParse(Session["UserId"].ToString(), out intUserId);
                var list = (from comm in db.tblCommunications
                            join std in db.SITSPL_tblStudentProfiles
                            on comm.Id equals std.StudentId
                            join bat in db.SITSPL_tblBatches
                            on std.BatchId equals bat.BatchId
                            join batgrp in db.SITSPL_tblBatchGroups
                            on std.BatchGroupId equals batgrp.BatchGroupId
                            join cd in db.SITSPL_tblCourseDetails
                            on std.StudentId equals cd.StudentId
                            join cour in db.SITSPL_tblCourses
                            on cd.CourseId equals cour.CourseId
                            //join us in db.SITSPL_tblUsers
                            //on comm.AdminId equals us.UserId
                            where comm.CreatedBy == "BrandStudent" && comm.AdminId == intUserId && comm.CategoryType == "BrandStudentToAdmin"
                            //    && comm.UserType == "Admin" || comm.UserType == "SuperAdmin"
                            select new
                            {
                                comm,
                                std,
                                bat,
                                batgrp,
                                cd,
                                cour

                            }).ToList();

                var data = list.Select(x => new
                {
                    x.comm.CommunicationId,
                    x.comm.CategoryType,
                    x.cd.CourseId,
                    x.cour.CourseName,
                    x.bat.BatchSize,
                    x.batgrp.BatchName,
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

    }
}

