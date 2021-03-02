using OnlineCoaching.Linq_To_Sql;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace OnlineCoaching.Controllers
{
    public class CourseController : Controller
    {
        CourseDataContext db = new CourseDataContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddCourse()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }


        // Course Create 

        [HttpPost]
        public JsonResult CreateCourse(SITSPL_tblCourse objCourse)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                string Cour = objCourse.CourseName;
                var data = db.SITSPL_tblCourses.Select(x => new { x.CourseName }).ToList();
                var Cours = data.Select(x => x.CourseName);
                if (Cour != null && !Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    objCourse.DateCreated = DateTime.Now;
                    objCourse.DatePublished = DateTime.Now;
                    objCourse.IsPublished = true;
                    objCourse.CreatedBy = "Admin";
                    db.SITSPL_tblCourses.InsertOnSubmit(objCourse);
                    db.SubmitChanges();
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else if (Cour != null && Cours.Contains(Cour, StringComparer.OrdinalIgnoreCase))
                {
                    ViewBag.InsertMessage = "Course already exists";
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

        // Getting data from Course Master
        public JsonResult GetAddedCourses()
        {
            try
            {
                db = new CourseDataContext();
                var data = db.SITSPL_AddCourse().Where(x => x.IsDeleted = true).OrderByDescending(x => x.CourseId).ToList();
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


        // Edit Course 
        public JsonResult EditCourse(int Id)
        {
            try
            {
                db = new CourseDataContext();
                var data = db.SITSPL_tblCourses.Where(x => x.CourseId == Id).Select(x => x.CourseName).FirstOrDefault();
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


        // Update Course
        [HttpPost]
        public JsonResult UpdateCourse(int intId, string strCourse)
        {
            try
            {
                db = new CourseDataContext();
                bool isResult = false;
                //   string Cour = Course.Trim();
                if (intId > 0)
                {
                    SITSPL_tblCourse course = db.SITSPL_tblCourses.Where(x => x.CourseId == intId).SingleOrDefault();
                    var data = db.SITSPL_tblCourses.Select(x => x.CourseName).ToList();

                    var alldata = db.SITSPL_tblCourses.Where(x => x.CourseId == intId).Select(x => new
                    {
                        x.CourseId,
                        x.CourseName
                    }).FirstOrDefault();

                    if (data.Contains(strCourse.Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        if (intId == alldata.CourseId && strCourse.ToLower() == alldata.CourseName.ToLower())
                        {
                            course.CourseName = strCourse;
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
                        if (!string.IsNullOrEmpty(strCourse.Trim()))
                        {
                            course.CourseName = strCourse;
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

        [HttpPost]
        public JsonResult DeleteCourse(int Id)
        {
            try
            {
                db = new CourseDataContext();
                if (Id > 0)
                {
                    SITSPL_tblCourse course = db.SITSPL_tblCourses.Where(x => x.CourseId == Id).FirstOrDefault();
                    if (course != null)
                    {
                        db.SITSPL_tblCourses.DeleteOnSubmit(course);
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
                    return Json("strcoursedepend", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ex.Message, JsonRequestBehavior.AllowGet);
                }
                //  return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // Delete Course
        //[HttpPost]
        //public JsonResult DeleteCourse(int Id)
        //{
        //    try
        //    {
        //        db = new CourseDataContext();
        //        bool result = false;
        //        SITSPL_tblCourse course = db.SITSPL_tblCourses.Where(x => x.CourseId == Id).FirstOrDefault();
        //        var alldata = (from c in db.SITSPL_tblCourses
        //                       join cs in db.SITSPL_tblCourseStructures
        //                       on c.CourseId equals cs.CourseId
        //                       join swc in db.SITSPL_tblStudentWithCourses
        //                       on cs.StructureId equals swc.StructureId
        //                       where swc.CourseId == Id
        //                       select new
        //                       {
        //                           StructureId = cs.StructureId,
        //                           CourseId = swc.CourseId
        //                       }).FirstOrDefault();

        //        if (alldata != null || alldata == null)
        //        {
        //            if (course != null && alldata != null)
        //            {
        //                return Json("Coursetaken", JsonRequestBehavior.AllowGet);
        //            }

        //            else if (course != null && alldata == null)
        //            {
        //                SITSPL_tblCourse tblcourse = db.SITSPL_tblCourses.SingleOrDefault(x => x.CourseId == Id);
        //                db.SITSPL_tblCourses.DeleteOnSubmit(tblcourse);
        //                db.SubmitChanges();
        //                result = true;
        //                return Json(result, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        return Json("", JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        string str = ex.Message;
        //        return Json("str", JsonRequestBehavior.AllowGet);
        //    }
        //}



        // Show Course Structure ( Data)
        public JsonResult ShowCourses()
        {
            try
            {
                db = new CourseDataContext();
                var data = db.SITSPL_ShowCourseStructure().Where(x => x.ISDeleted != true)
                         .OrderByDescending(x => x.CourseId).ToList();
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


        // 5 September Get Duration
        public JsonResult GetDuration()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from dur in db.tblDurations
                            select new
                            {
                                dur.DurationId,
                                dur.DurationName
                            }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult CourseDetails()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }


        // Add Course Structure

        [HttpPost]
        public JsonResult AddCourseStructure(SITSPL_tblCourseStructure courseStructure)
        {
            try
            {
                bool result = false;
                db = new CourseDataContext();
                courseStructure.DateCreated = DateTime.Now;

                if (courseStructure.IsPublished == true)
                {
                    courseStructure.DatePublished = DateTime.Now;
                }

                courseStructure.CreatedBy = "Admin";
                db.SITSPL_tblCourseStructures.InsertOnSubmit(courseStructure);
                db.SubmitChanges();
                result = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ShowCourseStructure()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        public JsonResult GetSubCategory(int CategoryId)
        {
            try
            {
                db = new CourseDataContext();
                if (CategoryId > 0)
                {
                    var data = (from subcatg in db.tblStudentSubCategories
                                join catg in db.tblStudentCategories
                                on subcatg.StudentCategoryId equals catg.StudentCategoryId
                                where subcatg.StudentCategoryId == CategoryId && subcatg.IsPublished == true
                                select new
                                {
                                    subcatg.StudedetSubCategoryId,
                                    subcatg.SubCategoryName
                                }).Distinct().ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = (from subcatg in db.tblStudentSubCategories
                                where subcatg.IsPublished == true
                                select new
                                {
                                    subcatg.StudedetSubCategoryId,
                                    subcatg.SubCategoryName
                                }).Distinct().ToList();

                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // Edit Course Structure
        public JsonResult EditCourseDetail(string CourseId, string StructureId)
        {
            try
            {
                db = new CourseDataContext();
                if (CourseId != "" && StructureId != "" && StructureId != null)
                {
                    var Dur = db.SITSPL_tblCourseStructures.Where(x => x.CourseId == Convert.ToInt32(CourseId) &&
                    x.StructureId == Convert.ToInt32(StructureId)).Select(x => new { x.DurationId }).FirstOrDefault();
                    var data2 = (from c in db.SITSPL_tblCourses
                                 join cs in db.SITSPL_tblCourseStructures on c.CourseId equals cs.CourseId
                                 join dur in db.tblDurations on cs.DurationId equals dur.DurationId
                                 join tut in db.tblCourseTutors on cs.TutorId equals tut.TutorId into tutor// Added By Dilshad A. on 16 Dec 2020
                                 from tut in tutor.DefaultIfEmpty()
                                 where (cs.CourseId == Convert.ToInt32(CourseId) && cs.StructureId == Convert.ToInt32(StructureId))
                                 select new
                                 {
                                     Course = cs.CourseId,
                                     Duration = cs.DurationName,
                                     Month = cs.Months,
                                     cs.Fees,
                                     Discount = cs.DiscountPercent,
                                     cs.NetAmount,
                                     cs.ValidFrom,
                                     cs.ValidTo,
                                     cs.StdCatgId,
                                     cs.StructureId,
                                     dur.DurationId,
                                     dur.DurationName,
                                     cs.TutorId,// Added By Dilshad A. on 16 Dec 2020
                                     cs.BatchId,// Added By Dilshad A. on 16 Dec 2020
                                     cs.StudentSubCategoryId,
                                     cs.IsPublished
                                 }).FirstOrDefault();
                    return Json(data2, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }


        //        Update Course Structure
        [HttpPost]
        public JsonResult UpdateCourseDetailsMaster(SITSPL_tblCourseStructure cs, int Course, decimal Fees,
            int Discount, decimal NetAmount, DateTime ValidFrom, DateTime ValidTo, int StructureId, int DurationId, string DurationName
            , int StudentCategoryId, bool IsPublished, Int64? BatchId, int StudentSubCategoryId)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;

                SITSPL_tblCourseStructure coursestr = db.SITSPL_tblCourseStructures.Where(x => x.CourseId == Course
                 && x.StructureId == StructureId && x.IsDeleted == false).FirstOrDefault();
                DateTime ValidFromdate = DateTime.Parse(Convert.ToDateTime(ValidFrom).ToShortDateString());
                DateTime ValidTodate = DateTime.Parse(Convert.ToDateTime(ValidTo).ToShortDateString());

                if (ValidFromdate > ValidTodate)
                {
                    return Json("ValidTo");
                }
                else if (coursestr != null)
                {
                    coursestr.CourseId = Course;
                    coursestr.Fees = Fees;
                    coursestr.DiscountPercent = Discount;
                    coursestr.NetAmount = NetAmount;
                    coursestr.ValidFrom = ValidFrom;
                    coursestr.ValidTo = ValidTo;
                    coursestr.DurationId = DurationId;
                    coursestr.StdCatgId = StudentCategoryId;
                    coursestr.StudentSubCategoryId = StudentSubCategoryId;
                    coursestr.BatchId = BatchId;                    
                   
                    if (IsPublished)
                    {
                        if (!coursestr.IsPublished)
                        {
                            coursestr.DatePublished = DateTime.Now;
                            coursestr.PublishedBy = "Admin";
                        }
                    }
                    coursestr.IsPublished = IsPublished;
                    coursestr.LastUpdated = DateTime.Now;
                    coursestr.UpdatedBy = "Admin";
                    db.SubmitChanges();
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CourseUpdate()
        {
            return View();
        }
    }
}