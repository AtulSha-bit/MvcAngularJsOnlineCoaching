using OnlineCoaching.Linq_To_Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Newtonsoft.Json;
using OnlineCoaching.Angular_Js_Data_Layer;
using System.Globalization;

namespace OnlineCoaching.Controllers
{
    public class InternshipController : Controller
    {
        CourseDataContext db = null;

        // GET: Internship
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAddedInternships()
        {
            db = new CourseDataContext();
            var data = db.SITSPL_showInternship().Where(x => x.IsDeleted == false).OrderByDescending(x => x.InternshipId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // Edit Course 
        public JsonResult EditInternship(int Id)
        {
            try
            {
                db = new CourseDataContext();
                var data = db.SITSPL_tblInternships.Where(x => x.InternshipId == Id).Select(x => x.InternshipName).FirstOrDefault();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        //  Update Internship 
        [HttpPost]
        public JsonResult UpdateInternship(int intId, string strinternship)
        {
            try
            {
                db = new CourseDataContext();
                bool isResult = false;
                //   string Cour = Course.Trim();
                if (intId > 0)
                {
                    SITSPL_tblInternship internship = db.SITSPL_tblInternships.Where(x => x.InternshipId == intId).SingleOrDefault();
                    var data = db.SITSPL_tblInternships.Select(x => x.InternshipName).ToList();

                    var alldata = db.SITSPL_tblInternships.Where(x => x.InternshipId == intId).Select(x => new
                    {
                        x.InternshipId,
                        x.InternshipName
                    }).FirstOrDefault();

                    if (data.Contains(strinternship.Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        if (intId == alldata.InternshipId && strinternship.ToLower() == alldata.InternshipName.ToLower())
                        {
                            internship.InternshipName = strinternship;
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
                        if (!string.IsNullOrEmpty(strinternship.Trim()))
                        {
                            internship.InternshipName = strinternship;
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

        // Delete Internship ( Code commented)
        [HttpPost]
        public JsonResult DeleteCourse(int Id)
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddInternship()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateInternship(SITSPL_tblInternship objInternship)
        {
            try
            {
                db = new CourseDataContext();
                bool result = false;
                string Intern = objInternship.InternshipName;
                var data = db.SITSPL_tblInternships.Select(x => new { x.InternshipName }).ToList();
                var Interns = data.Select(x => x.InternshipName);
                if (Intern != null && !Interns.Contains(Intern, StringComparer.OrdinalIgnoreCase))
                {
                    objInternship.DateCreated = DateTime.Now;
                    objInternship.DatePublished = DateTime.Now;
                    objInternship.IsDeleted = Convert.ToBoolean(0);
                    objInternship.IsPublished = true;
                    objInternship.CreatedBy = "Admin";
                    db.SITSPL_tblInternships.InsertOnSubmit(objInternship);
                    db.SubmitChanges();
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else if (Intern != null && Interns.Contains(Intern, StringComparer.OrdinalIgnoreCase))
                {
                    ViewBag.InsertMessage = "Internship already exists";
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

        // Get Courses Data
        public JsonResult GetInternship()
        {
            try
            {
                db = new CourseDataContext();
                var data = db.SITSPL_tblInternships.Select(x => new { x.InternshipId, x.InternshipName }).ToList();
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

        public ActionResult InternshipStructure()
        {

            return View();
        }

        // Add Internship Structure
        #region Add Internship Structure
        [HttpPost]
        public JsonResult AddInternshipStructure(SITSPL_tblInternshipStructure internshipStructure, List<tblInternBullet> lstBullet, List<tblInternWhoCanApply> lstWhoCanApply, List<tblInternSkillReq> lstSkills, List<tblInternPerk> lstPerk, List<tblInternActivity> lstInternActivity)
        {
            
            try
            {
               
                bool result = false;
                db = new CourseDataContext();
                internshipStructure.DateCreated = DateTime.Now;
                internshipStructure.DatePublished = DateTime.Now;
                internshipStructure.IsDeleted = false;
                internshipStructure.CreatedBy = "Admin";
                db.SITSPL_tblInternshipStructures.InsertOnSubmit(internshipStructure);
                db.SubmitChanges();

                #region New Points To Internship for Bullet Point, Required Skills, Who Can Apply, Perks, Activities
                if (internshipStructure.InternStructureId > 0)
                {
                    // Add-Update Bullet Point
                    if (lstBullet != null)
                    {
                        for (int i = 0; i < lstBullet.Count; i++)
                        {
                            if (lstBullet[i].BulletId > 0)
                            {
                                var blt = (from d in db.tblInternBullets where d.BulletId == lstBullet[i].BulletId select d).FirstOrDefault();
                                blt.IsPublished = lstBullet[i].IsPublished;
                                blt.BulletPoint = lstBullet[i].BulletPoint;
                                db.SubmitChanges();
                            }
                            else
                            {
                                tblInternBullet objBullet = new tblInternBullet();
                                objBullet.DateCreated = DateTime.Now;
                                objBullet.CreatedBy = "Admin";
                                objBullet.InternshipId = internshipStructure.InternStructureId;
                                objBullet.IsPublished = lstBullet[i].IsPublished;
                                objBullet.BulletPoint = lstBullet[i].BulletPoint;
                                db.tblInternBullets.InsertOnSubmit(objBullet);
                                db.SubmitChanges();
                            }
                        }
                    }

                    // Add-Update Required Skills
                    if (lstSkills != null)
                    {
                        for (int i = 0; i < lstSkills.Count; i++)
                        {
                            if (lstSkills[i].InternSkillId > 0)
                            {
                                var data = (from d in db.tblInternSkillReqs where d.InternSkillId == lstSkills[i].InternSkillId select d).FirstOrDefault();
                                data.IsPublished = lstSkills[i].IsPublished;


                                data.CourseName = lstSkills[i].CourseName;

                                // data.CourseName = lstSkills[i].CourseName;

                                //         data.CourseId = lstSkills[i].CourseId;

                                db.SubmitChanges();
                            }
                            else
                            {
                                tblInternSkillReq objSkills = new tblInternSkillReq();
                                objSkills.DateCreated = DateTime.Now;
                                objSkills.CreatedBy = "Admin";
                                objSkills.InternshipId = internshipStructure.InternStructureId;
                                objSkills.IsPublished = lstSkills[i].IsPublished;

                                objSkills.CourseName = lstSkills[i].CourseName;

                                db.tblInternSkillReqs.InsertOnSubmit(objSkills);
                                db.SubmitChanges();
                            }
                        }
                    }

                    // Add-Update Who Can apply
                    if (lstWhoCanApply != null)
                    {
                        for (int i = 0; i < lstWhoCanApply.Count; i++)
                        {
                            if (lstWhoCanApply[i].InternApplyId > 0)
                            {
                                var data = (from d in db.tblInternWhoCanApplies where d.InternApplyId == lstWhoCanApply[i].InternApplyId select d).FirstOrDefault();
                                data.IsPublished = lstWhoCanApply[i].IsPublished;
                                data.ApplyPoint = lstWhoCanApply[i].ApplyPoint;
                                db.SubmitChanges();
                            }
                            else
                            {
                                tblInternWhoCanApply objWhoCanApply = new tblInternWhoCanApply();
                                objWhoCanApply.DateCreated = DateTime.Now;
                                objWhoCanApply.CreatedBy = "Admin";
                                objWhoCanApply.InternshipId = internshipStructure.InternStructureId;
                                objWhoCanApply.IsPublished = lstWhoCanApply[i].IsPublished;
                                objWhoCanApply.ApplyPoint = lstWhoCanApply[i].ApplyPoint;
                                db.tblInternWhoCanApplies.InsertOnSubmit(objWhoCanApply);
                                db.SubmitChanges();
                            }
                        }
                    }

                    // Add-Update Perks
                    if (lstPerk != null)
                    {
                        for (int i = 0; i < lstPerk.Count; i++)
                        {
                            if (lstPerk[i].InternPerkId > 0)
                            {
                                var data = (from d in db.tblInternPerks where d.InternPerkId == lstPerk[i].InternPerkId select d).FirstOrDefault();
                                data.IsPublished = lstPerk[i].IsPublished;
                                data.PerkName = lstPerk[i].PerkName;
                                db.SubmitChanges();
                            }
                            else
                            {
                                tblInternPerk objPerks = new tblInternPerk();
                                objPerks.DateCreated = DateTime.Now;
                                objPerks.CreatedBy = "Admin";
                                objPerks.InternshipId = internshipStructure.InternStructureId;
                                objPerks.IsPublished = lstPerk[i].IsPublished;
                                objPerks.PerkName = lstPerk[i].PerkName;
                                db.tblInternPerks.InsertOnSubmit(objPerks);
                                db.SubmitChanges();
                            }
                        }
                    }

                    // Add-Update Activity
                    if (lstInternActivity != null)
                    {
                        for (int i = 0; i < lstInternActivity.Count; i++)
                        {
                            if (lstInternActivity[i].InternActivityId > 0)
                            {
                                var data = (from d in db.tblInternActivities where d.InternActivityId == lstInternActivity[i].InternActivityId select d).FirstOrDefault();
                                data.IsPublished = lstInternActivity[i].IsPublished;
                                //       data.ActivityType = lstInternActivity[i].ActivityType;
                                data.ActivityType = lstInternActivity[i].ActivityType;
                                data.ActivityPoint = lstInternActivity[i].ActivityPoint;
                                db.SubmitChanges();
                            }
                            else
                            {
                                tblInternActivity objActivity = new tblInternActivity();
                                objActivity.DateCreated = DateTime.Now;
                                objActivity.CreatedBy = "Admin";
                                objActivity.InternshipId = internshipStructure.InternStructureId;
                                objActivity.IsPublished = lstInternActivity[i].IsPublished;
                                objActivity.ActivityType = lstInternActivity[i].ActivityType;
                                objActivity.ActivityPoint = lstInternActivity[i].ActivityPoint;
                                db.tblInternActivities.InsertOnSubmit(objActivity);
                                db.SubmitChanges();
                            }
                        }
                    }
                }
                #endregion END New Points To Internship for Bullet Point, Required Skills, Who Can Apply, Perks, Activities

                result = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion END Add Internship Structure

        #region Show Internship Structure

        public ActionResult GetInternshipStructure()
        {
            return View();
        }

        // Internship Structure Data
        public JsonResult GetInternshipData()
        {
            db = new CourseDataContext();
            var data = (from x in db.SITSPL_tblInternshipStructures
                        join x2 in db.SITSPL_tblInternships on x.InternshipId equals x2.InternshipId
                        join x3 in db.tblDurations on x.DurationId equals x3.DurationId
                        select new
                        {
                            x,
                            x2,
                            x3
                        }).ToList();
            var list = data.Select(d => new
            {
                d.x.InternStructureId,
                d.x.InternshipId,
                d.x2.InternshipName,
                d.x.InternshipType,
                d.x.Fees,
                d.x.Discount,
                d.x.FeeAfterDiscount,
                d.x.TotalAmount,
                d.x.DurationMonths,
                d.x.DurationName,
                DurationId = d.x.DurationId,
                Durations = d.x3.DurationName,

                LastApplyDate = d.x.LastApplyDate.HasValue ? d.x.LastApplyDate.Value.ToString("dd/MM/yyyy") : null,
                d.x.TotalAvailableSeat,
                d.x.PublishedBy,
                d.x.Stipened,

                DateCreated = d.x.DateCreated.ToString("dd/MM/yyyy"),
                ValidFrom = d.x.ValidFrom.HasValue ? d.x.ValidFrom.Value.ToString("dd/MM/yyyy") : null,
                ValidTo = d.x.ValidTo.HasValue ? d.x.ValidTo.Value.ToString("dd/MM/yyyy") : null,
                CreatedDate = d.x.DateCreated.ToString("dd/MM/yyyy"),
                d.x.CreatedBy,
                LastUpdated = d.x.LastUpdated.HasValue ? d.x.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                d.x.UpdatedBy,
                d.x.IsPublished,
                d.x.DisplayOnWeb,
                d.x.IsDeleted
            }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion END Show Internship Structure

        #region Edit Internship Structure with all referenced table by Dilshad A. on 28 Aug 2020
        // Edit Internship by Dilshad on 28 Aug 2020
        [HttpPost]
        public JsonResult EditIntershipStructure(int intInterStructId)
        {
            try
            {
                db = new CourseDataContext();
                var stru = (from d in db.SITSPL_tblInternships
                            join s in db.SITSPL_tblInternshipStructures on d.InternshipId equals s.InternshipId
                            where s.InternStructureId == intInterStructId
                            join dur in db.tblDurations on s.DurationId equals dur.DurationId
                            select new
                            {
                                s,
                                dur,
                                d
                            }).ToList();

                var structure = stru.Select(x => new
                {
                    x.s.InternshipId,
                    x.s.IsPublished,
                    x.s.InternStructureId,
                    LastApplyDate = x.s.LastApplyDate.HasValue ? x.s.LastApplyDate.Value.ToString("dd/MM/yyyy") : null,
                    x.s.DurationMonths,
                   // x.s.DurationName,
                    x.s.TotalAvailableSeat,
                    x.s.InternshipType,
                    x.s.Fees,
                    x.s.Discount,
                    x.s.FeeAfterDiscount,
                    x.s.TotalAmount,
                    x.s.StipenedMoney,
                    ValidFrom = x.s.ValidFrom.HasValue ? x.s.ValidFrom.Value.ToString("dd/MM/yyyy") : null,
                    ValideTo = x.s.ValidTo.HasValue ? x.s.ValidTo.Value.ToString("dd/MM/yyyy") : null,
                    //x.dur.DurationId,
                    x.s.DurationId,
                    Durations = x.dur.DurationName,
                    x.d.InternshipName
                });

                var bullets = (from d in db.SITSPL_tblInternshipStructures
                               join bullet in db.tblInternBullets on d.InternStructureId equals bullet.InternshipId
                               where bullet.InternshipId == intInterStructId
                               select new
                               {
                                   bullet.BulletId,
                                   bullet.BulletPoint,
                                   bullet.IsPublished
                               }).ToList();

                var skillSet = (from d in db.SITSPL_tblInternshipStructures
                                join skills in db.tblInternSkillReqs on d.InternStructureId equals skills.InternshipId
                                where skills.InternshipId == intInterStructId
                                select new
                                {
                                    skills.InternSkillId,
                                    skills.CourseName,
                                    skills.IsPublished
                                }).ToList();

                var wca = (from d in db.SITSPL_tblInternshipStructures
                           join w in db.tblInternWhoCanApplies on d.InternStructureId equals w.InternshipId
                           where w.InternshipId == intInterStructId
                           select new
                           {
                               w.InternApplyId,
                               w.ApplyPoint,
                               w.IsPublished
                           }).ToList();

                var perks = (from d in db.SITSPL_tblInternshipStructures
                             join prk in db.tblInternPerks on d.InternStructureId equals prk.InternshipId
                             where prk.InternshipId == intInterStructId
                             select new
                             {
                                 prk.InternPerkId,
                                 prk.PerkName,
                                 prk.IsPublished
                             }).ToList();

                var activity = (from d in db.SITSPL_tblInternshipStructures
                                join act in db.tblInternActivities on d.InternStructureId equals act.InternshipId
                                join inacttype in db.tblInternActivityTypes on act.ActivityType equals inacttype.InternActivityTypeId
                                where act.InternshipId == intInterStructId
                                select new
                                {
                                    act.InternActivityId,
                                    act.ActivityType,
                                    inacttype.InternActivityTypeId,
                                    ActivityTypeId = inacttype.InternActivityType,
                                    act.ActivityPoint,
                                    act.IsPublished
                                }).ToList();
                var list = new { structure, bullets, skillSet, wca, perks, activity };

                JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion END Edit Internship Structure with all referenced table by Dilshad A. on 28 Aug 2020

        // Update Internship Structure       
        [HttpPost]
        public JsonResult UpdateInternshipStructure(SITSPL_tblInternshipStructure tblis, int InternStructureId, List<tblInternBullet> lstBullet, List<tblDeleteInternBullet> deleteBulletArray,List<tblInternWhoCanApply> lstWhoCanApply, List<DeleteWhoApply> deleteWhoApply, List<tblInternSkillReq> lstSkills,List<tblDeleteInternSkill> deleteSkillArray, List<tblInternPerk> lstPerk,List<tblDeleteInternPerk> deletePerkArray , List<tblInternActivity> lstInternActivity,List<tblDeleteInternActivity> deleteActivityArray)
        {

           
            try
            {
                db = new CourseDataContext();
                bool result = false;
                //var Dur = tblis.DurationName.Trim();

                SITSPL_tblInternshipStructure internstr = db.SITSPL_tblInternshipStructures.Where(x => x.InternStructureId == InternStructureId).SingleOrDefault();
                if (tblis.ValidFrom > tblis.ValidTo)
                {
                    return Json("ValidTo");
                }
                else if (internstr != null)
                {
                    internstr.InternshipId = tblis.InternshipId;
                    internstr.DurationMonths = tblis.DurationMonths;
                    internstr.InternshipType = tblis.InternshipType;
                    internstr.Fees = tblis.Fees;
                    internstr.Discount = tblis.Discount;
                    internstr.FeeAfterDiscount = tblis.FeeAfterDiscount;
                    internstr.TotalAmount = tblis.TotalAmount;
                    internstr.StipenedMoney = tblis.StipenedMoney;
                    internstr.ValidFrom = tblis.ValidFrom;
                    internstr.ValidTo = tblis.ValidTo;
                    internstr.DurationId = tblis.DurationId;
                    internstr.DatePublished = DateTime.Now;
                    internstr.IsPublished = tblis.IsPublished;
                    internstr.LastApplyDate = tblis.LastApplyDate;
                    internstr.IsDeleted = false;
                    internstr.LastUpdated = DateTime.Now;
                    internstr.UpdatedBy = "Admin";
                    db.SubmitChanges();
                    result = true;

                    #region New Points To Internship for Bullet Point, Required Skills, Who Can Apply, Perks, Activities
                    if (InternStructureId > 0)
                    {
                        // Add-Update Bullet Point
                        if (lstBullet != null)
                        {
                            for (int i = 0; i < lstBullet.Count; i++)
                            {
                                if (lstBullet[i].BulletId > 0)
                                {
                                    var blt = (from d in db.tblInternBullets where d.BulletId == lstBullet[i].BulletId select d).FirstOrDefault();
                                    blt.IsPublished = lstBullet[i].IsPublished;
                                    blt.BulletPoint = lstBullet[i].BulletPoint;
                                    db.SubmitChanges();
                                }
                                else
                                {
                                    tblInternBullet objBullet = new tblInternBullet();
                                    objBullet.DateCreated = DateTime.Now;
                                    objBullet.CreatedBy = "Admin";
                                    objBullet.InternshipId = InternStructureId;
                                    objBullet.IsPublished = lstBullet[i].IsPublished;
                                    objBullet.BulletPoint = lstBullet[i].BulletPoint;
                                    db.tblInternBullets.InsertOnSubmit(objBullet);
                                    db.SubmitChanges();
                                }
                            }
                        }

                        if (deleteBulletArray != null)
                        {
                            for (int i = 0; i < deleteBulletArray.Count; i++)
                            {
                                if (deleteBulletArray[i].BulletId > 0)
                                {

                                 tblInternBullet bullet = (from d in db.tblInternBullets where d.BulletId == deleteBulletArray[i].BulletId select d).FirstOrDefault();
                                 db.tblInternBullets.DeleteOnSubmit(bullet);
                                 db.SubmitChanges();
                                }
                            }
                        }


                        // Add-Update Required Skills
                        if (lstSkills != null)
                        {
                            for (int i = 0; i < lstSkills.Count; i++)
                            {
                                if (lstSkills[i].InternSkillId > 0)
                                {
                                    var data = (from d in db.tblInternSkillReqs where d.InternSkillId == lstSkills[i].InternSkillId select d).FirstOrDefault();
                                    data.IsPublished = lstSkills[i].IsPublished;
                                    data.CourseName = lstSkills[i].CourseName;
                                    db.SubmitChanges();
                                }
                                else
                                {
                                    tblInternSkillReq objSkills = new tblInternSkillReq();
                                    objSkills.DateCreated = DateTime.Now;
                                    objSkills.CreatedBy = "Admin";
                                    objSkills.InternshipId = InternStructureId;
                                    objSkills.IsPublished = lstSkills[i].IsPublished;
                                    objSkills.CourseName = lstSkills[i].CourseName;                                    
                                    db.tblInternSkillReqs.InsertOnSubmit(objSkills);
                                    db.SubmitChanges();
                                }
                            }
                        }



                        if (deleteSkillArray != null)
                        {
                            for (int i = 0; i < deleteSkillArray.Count; i++)
                            {
                                if (deleteSkillArray[i].InternSkillId > 0)
                                {
                                    tblInternSkillReq skill = (from d in db.tblInternSkillReqs where d.InternSkillId == deleteSkillArray[i].InternSkillId select d).FirstOrDefault();
                                    db.tblInternSkillReqs.DeleteOnSubmit(skill);
                                    db.SubmitChanges();
                                }
                            }
                        }

                        




                        // Add-Update Who Can apply
                        if (lstWhoCanApply != null)
                        {
                            for (int i = 0; i < lstWhoCanApply.Count; i++)
                            {
                                if (lstWhoCanApply[i].InternApplyId > 0)
                                {
                                    var data = (from d in db.tblInternWhoCanApplies where d.InternApplyId == lstWhoCanApply[i].InternApplyId select d).FirstOrDefault();
                                    data.IsPublished = lstWhoCanApply[i].IsPublished;
                                    data.ApplyPoint = lstWhoCanApply[i].ApplyPoint;
                                    db.SubmitChanges();
                                }
                                else
                                {
                                    tblInternWhoCanApply objWhoCanApply = new tblInternWhoCanApply();
                                    objWhoCanApply.DateCreated = DateTime.Now;
                                    objWhoCanApply.CreatedBy = "Admin";
                                    objWhoCanApply.InternshipId = InternStructureId;
                                    objWhoCanApply.IsPublished = lstWhoCanApply[i].IsPublished;
                                    objWhoCanApply.ApplyPoint = lstWhoCanApply[i].ApplyPoint;
                                    db.tblInternWhoCanApplies.InsertOnSubmit(objWhoCanApply);
                                    db.SubmitChanges();
                                }
                            }
                        }


                        if (deleteWhoApply != null)
                        {
                            for (int i = 0; i < deleteWhoApply.Count; i++)
                            {
                                if (deleteWhoApply[i].InternApplyId > 0)
                                {
                                    tblInternWhoCanApply apply = (from d in db.tblInternWhoCanApplies where d.InternApplyId == deleteWhoApply[i].InternApplyId select d).FirstOrDefault();
                                    db.tblInternWhoCanApplies.DeleteOnSubmit(apply);
                                    db.SubmitChanges();
                                }
                            }
                        }

                        



                        // Add-Update Perks
                        if (lstPerk != null)
                        {
                            for (int i = 0; i < lstPerk.Count; i++)
                            {
                                if (lstPerk[i].InternPerkId > 0)
                                {
                                    var data = (from d in db.tblInternPerks where d.InternPerkId == lstPerk[i].InternPerkId select d).FirstOrDefault();
                                    data.IsPublished = lstPerk[i].IsPublished;
                                    data.PerkName = lstPerk[i].PerkName;
                                    db.SubmitChanges();
                                }
                                else
                                {
                                    tblInternPerk objPerks = new tblInternPerk();
                                    objPerks.DateCreated = DateTime.Now;
                                    objPerks.CreatedBy = "Admin";
                                    objPerks.InternshipId = InternStructureId;
                                    objPerks.IsPublished = lstPerk[i].IsPublished;
                                    objPerks.PerkName = lstPerk[i].PerkName;
                                    db.tblInternPerks.InsertOnSubmit(objPerks);
                                    db.SubmitChanges();
                                }
                            }
                        }



                        if (deletePerkArray != null)
                        {
                            for (int i = 0; i < deletePerkArray.Count; i++)
                            {
                                if (deletePerkArray[i].InternPerkId > 0)
                                {
                                    tblInternPerk perk = (from d in db.tblInternPerks where d.InternPerkId == deletePerkArray[i].InternPerkId select d).FirstOrDefault();
                                    db.tblInternPerks.DeleteOnSubmit(perk);
                                    db.SubmitChanges();
                                }
                            }
                        }

                        

                        // Add-Update Activity
                        if (lstInternActivity != null)
                        {
                            for (int i = 0; i < lstInternActivity.Count; i++)
                            {
                                if (lstInternActivity[i].InternActivityId > 0)
                                {
                                    var data = (from d in db.tblInternActivities where d.InternActivityId == lstInternActivity[i].InternActivityId select d).FirstOrDefault();
                                    data.IsPublished = lstInternActivity[i].IsPublished;
                                    data.ActivityType = lstInternActivity[i].ActivityType;
                                    data.ActivityPoint = lstInternActivity[i].ActivityPoint;
                                    db.SubmitChanges();
                                }
                                else
                                {
                                    tblInternActivity objActivity = new tblInternActivity();
                                    objActivity.DateCreated = DateTime.Now;
                                    objActivity.CreatedBy = "Admin";
                                    objActivity.InternshipId = InternStructureId;
                                    objActivity.IsPublished = lstInternActivity[i].IsPublished;
                                    objActivity.ActivityType = lstInternActivity[i].ActivityType;
                                    objActivity.ActivityPoint = lstInternActivity[i].ActivityPoint;
                                    db.tblInternActivities.InsertOnSubmit(objActivity);
                                    db.SubmitChanges();
                                }
                            }
                        }



                        if (deleteActivityArray != null)
                        {
                            for (int i = 0; i < deleteActivityArray.Count; i++)
                            {
                                if (deleteActivityArray[i].InternActivityId > 0)
                                {
                                    tblInternActivity activity = (from d in db.tblInternActivities where d.InternActivityId == deleteActivityArray[i].InternActivityId select d).FirstOrDefault();
                                    db.tblInternActivities.DeleteOnSubmit(activity);
                                    db.SubmitChanges();
                                }
                            }
                        }



                    }
                    #endregion END New Points To Internship for Bullet Point, Required Skills, Who Can Apply, Perks, Activity
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #region Get Internship Activity Type

        public JsonResult GetInternshipActivity()
        {
            try
            {
                db = new CourseDataContext();
                var data = (from intacttype in db.tblInternActivityTypes
                            select new
                            {
                                intacttype.InternActivityTypeId,
                                intacttype.InternActivityType
                            }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Get Internship Activity Type

        #region Show Internship Structure based on published By Dilshad on 10 Sept 2020
        public ActionResult GetPublishedInternshipStructure()
        {
            return View();
        }

        // Internship Structure Data based on Published
        public JsonResult GetPublishedInternshipStructureDetails()
        {
            db = new CourseDataContext();
            var data = (from x in db.SITSPL_tblInternshipStructures
                        join x2 in db.SITSPL_tblInternships on x.InternshipId equals x2.InternshipId
                        join dur in db.tblDurations on x.DurationId equals dur.DurationId
                        where x.IsPublished == true
                        select new
                        {
                            x,
                            x2,
                            dur
                        }).ToList();
            var list = data.Select(d => new
            {
                d.x.InternStructureId,
                d.x.InternshipId,
                d.x2.InternshipName,
                d.x.InternshipType,
                d.x.Fees,
                d.x.Discount,
                d.x.FeeAfterDiscount,
                d.x.TotalAmount,          
                
                LastApplyDate = d.x.LastApplyDate.HasValue ? d.x.LastApplyDate.Value.ToString("dd/MM/yyyy") : null,
                d.x.TotalAvailableSeat,
                d.x.PublishedBy,
                d.x.Stipened,
                DateCreated = d.x.DateCreated.ToString("dd/MM/yyyy"),
                ValidFrom = d.x.ValidFrom.HasValue ? d.x.ValidFrom.Value.ToString("dd/MM/yyyy") : null,
                ValidTo = d.x.ValidTo.HasValue ? d.x.ValidTo.Value.ToString("dd/MM/yyyy") : null,
                CreatedDate = d.x.DateCreated.ToString("dd/MM/yyyy"),
                d.x.CreatedBy,
                LastUpdated = d.x.LastUpdated.HasValue ? d.x.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                d.x.UpdatedBy,
                d.x.IsPublished,
                d.x.DisplayOnWeb,
                d.x.IsDeleted
            }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion END Show Internship Structure based on published


        public ActionResult OnGoingInternship()
        {
            return View();
        }

        // &&  x.ValidFrom >= todayDate && todayDate > x.ValidTo
        // Internship Structure Data based on Published

        public JsonResult OnGoingInternshipDetails()
        {
            db = new CourseDataContext();
            var data = (from x in db.SITSPL_tblInternshipStructures
                        join x2 in db.SITSPL_tblInternships on x.InternshipId equals x2.InternshipId
                        join dur in db.tblDurations on x.DurationId equals dur.DurationId
                        where x.IsPublished == true && x.ValidFrom <= DateTime.Now &&  DateTime.Now<=x.ValidTo
                                             
                        select new
                        {
                            x,
                            x2,
                            dur
                        }).ToList();

            var list = data.Select(d => new
            {
                d.x.InternStructureId,
                d.x.InternshipId,
                d.x2.InternshipName,
                d.x.InternshipType,
                d.x.Fees,
                d.x.Discount,
                d.x.FeeAfterDiscount,
                d.x.TotalAmount,
               
                d.dur.DurationName,
                LastApplyDate = d.x.LastApplyDate.HasValue ? d.x.LastApplyDate.Value.ToString("dd/MM/yyyy") : null,
                d.x.TotalAvailableSeat,
                d.x.PublishedBy,
                d.x.StipenedMoney,
                DateCreated = d.x.DateCreated.ToString("dd/MM/yyyy"),
                ValidFrom = d.x.ValidFrom.HasValue ? d.x.ValidFrom.Value.ToString("dd/MM/yyyy") : null,
                ValidTo = d.x.ValidTo.HasValue ? d.x.ValidTo.Value.ToString("dd/MM/yyyy") : null,
                CreatedDate = d.x.DateCreated.ToString("dd/MM/yyyy"),
                d.x.CreatedBy,
                LastUpdated = d.x.LastUpdated.HasValue ? d.x.LastUpdated.Value.ToString("dd/MM/yyyy") : null,
                d.x.UpdatedBy,
                d.x.IsPublished,
                d.x.DisplayOnWeb,
                d.x.IsDeleted
            }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddTutorResourcesForInternship()
        {
            return View();
        }

        public JsonResult GetTutorResourcesForInternship(int InternshipId)
        {
            db = new CourseDataContext();
            var intern = (from instr in db.SITSPL_tblInternshipStructures
                          join intr in db.SITSPL_tblInternships
                          on instr.InternshipId equals intr.InternshipId
                          where instr.InternshipId == InternshipId
                          select new
                          {
                              instr.InternshipId,
                              intr.InternshipName
                          }).ToList();

            var intrtutor = (from d in db.SITSPL_tblInternships
                             join inttu in db.tblInternTutors on d.InternshipId equals inttu.InternshipId
                             join tut in db.SITSPL_tblTutors on inttu.TutorId equals tut.TutorId
                             where inttu.InternshipId == InternshipId
                             select new
                             {
                                 inttu.InternshipId,
                                 inttu.InternTutorId,
                                 d.InternshipName,
                                 inttu.TutorId,
                                 tut.TutorName

                             }).ToList();

            var resource = (from d in db.SITSPL_tblInternships
                            join res in db.tblInternResources
                            on d.InternshipId equals res.InternshipId
                            where res.InternshipId == InternshipId
                            select new
                            {
                                res.InternshipId,
                                d.InternshipName,
                                res.ResourceId,
                                res.ResourceName,

                            }).ToList();

            var list = new { intern, intrtutor, resource };

            JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = JsonConvert.SerializeObject(list, Formatting.None, jss);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ContentResult Upload(string files)
        {
            foreach(string key in Request.Files)
            {
                HttpPostedFileBase postedfile = Request.Files[key];
                string path = Server.MapPath("~/UploadResource/");
                postedfile.SaveAs(path+postedfile.FileName);
            }
            return Content("Success");
        }



        [HttpPost]
        public ContentResult AddTutorForInternship(List<InternshipTutorArray> InternshipArray, List<DocumentArray> Filearray, int InternshipId, List<DeleteInternTutorArray> deleteinterntutor, List<DeleteInternResourceArray> deleteInternResourceArray)
        {
            db = new CourseDataContext();
            if (InternshipArray != null)
            {
                for (int i = 0; i < InternshipArray.Count; i++)
                {
                    var objInternship = (from d in db.tblInternTutors where d.InternTutorId == InternshipArray[i].InternTutorId && d.InternshipId == InternshipId && d.TutorId == InternshipArray[i].TutorId select d).FirstOrDefault();
                    if (objInternship != null)
                    {
                        objInternship.TutorId = InternshipArray[i].TutorId;
                        objInternship.InternshipId = InternshipArray[i].InternshipId;
                        objInternship.LastUpdated = DateTime.Now;
                        objInternship.UpdatedBy = "Admin";
                        db.SubmitChanges();
                    }
                    else
                    {
                        tblInternTutor tutor = new tblInternTutor();
                        tutor.TutorId = InternshipArray[i].TutorId;
                        tutor.InternshipId = InternshipArray[i].InternshipId;
                        tutor.IsPubllished = true;
                        tutor.CreatedBy = "Admin";
                        tutor.DateCreated = DateTime.Now;
                        db.tblInternTutors.InsertOnSubmit(tutor);
                        db.SubmitChanges();

                        Int64 intInternTutorId = tutor.InternTutorId;

                        if (intInternTutorId > 0)
                        {
                            var Prefix = "IntTut-";
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
                                var AutoId = StdCodeLength;
                                var AutomaticId = AutoId.ToString();
                                SITSPL_tblUser user = new SITSPL_tblUser();
                                user.UserType = "Intern-Tutor";
                                user.Id = intInternTutorId;
                                user.UserPrefix = Prefix;
                                user.AutoId = AutomaticId;
                                user.CompletedId = StudentFinalCode;
                                user.DateCreated = DateTime.Now;
                                user.CreatedBy = "Admin";
                                user.IsPublished = true;
                                db.SITSPL_tblUsers.InsertOnSubmit(user);
                                db.SubmitChanges();
                            }
                        }
                    }
                }
            }

            if (deleteinterntutor != null)
            {
                for (int i = 0; i < deleteinterntutor.Count; i++)
                {
                    if (deleteinterntutor[i].TutorId > 0)
                    {
                        db = new CourseDataContext();
                        tblInternTutor internTutor = (from d in db.tblInternTutors where d.InternTutorId == deleteinterntutor[i].InternTutorId && d.TutorId == deleteinterntutor[i].TutorId && d.InternshipId == InternshipId select d).FirstOrDefault();
                        db.tblInternTutors.DeleteOnSubmit(internTutor);
                        db.SubmitChanges();

                    }
                }

            }

            if (Filearray != null)
            {
                // InternshipId
                for (int i = 0; i < Filearray.Count; i++)
                {

                    var InternResource = (from d in db.tblInternResources where d.InternshipId == InternshipId && d.ResourceId == Filearray[i].ResourceId select d).FirstOrDefault();
                    if (InternResource != null)
                    {
                        InternResource.InternshipId = InternshipId;
                        InternResource.ResourceName = Filearray[i].ResourceName;
                        InternResource.LastUpdated = DateTime.Now;
                        InternResource.UpdatedBy = "Admin";
                        db.SubmitChanges();
                    }
                    else
                    {
                        db = new CourseDataContext();
                        tblInternResource resource = new tblInternResource();
                        resource.ResourceName = Filearray[i].ResourceName;
                        resource.InternshipId = InternshipId;
                        resource.CreatedBy = "Admin";
                        resource.DateCreated = DateTime.Now;
                        resource.IsPubllished = true;
                        db.tblInternResources.InsertOnSubmit(resource);
                        db.SubmitChanges();
                    }
                }
            }


            if (deleteInternResourceArray != null)
            {
                for (int i = 0; i < deleteInternResourceArray.Count; i++)
                {
                    if (deleteInternResourceArray[i].ResourceId > 0)
                    {
                        db = new CourseDataContext();
                        tblInternResource internResource = (from d in db.tblInternResources where d.ResourceId == deleteInternResourceArray[i].ResourceId && d.InternshipId == InternshipId select d).FirstOrDefault();
                        db.tblInternResources.DeleteOnSubmit(internResource);
                        db.SubmitChanges();
                    }
                }

            }
            return Content("Done");
        }




    }
}