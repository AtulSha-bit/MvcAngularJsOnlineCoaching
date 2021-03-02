using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineCoaching.Angular_Js_Data_Layer
{
    public class TutorRegistration
    {
        public int TutorId { get; set; }
        public string TutorName { get; set; }
        public string TutorDescription { get; set; }
        public string TutorExperience { get; set; }
        public string TutorImage { get; set; }
        public DateTime TutorDOB { get; set; }
        public string TutorContact { get; set; }
        public string AdminDescription { get; set; }
        public bool IsDeleted { get; set; }
        public string TutorEmail { get; set; }
        public string CreatedBy {get; set;}
        public DateTime DateCreated { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string TutorType { get; set; }
    }
}