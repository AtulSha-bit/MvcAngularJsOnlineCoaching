using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineCoaching.Angular_Js_Data_Layer
{
    public class CourseDetails
    {

          public int CourseId { get; set; }
         public string DurationName { get; set; }
        public int Months { get; set; }
        public decimal Fees { get; set; }
        public int DiscountPercent { get; set; }
        public decimal NetAmount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int DurationId { get; set; }
        public Int64 TutorId { get; set; }
        public int StdCatgId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DatePublished { get; set; }
        public Boolean IsPublished { get; set; }
        public string CreatedBy { get; set; }

        //public DateTime DatePublished { get; set; }

        //public int CourseId { get; set; }
        //public string DurationName { get; set; }
        //public int Months { get; set; }
        //public decimal Fees { get; set; }
        //public int DiscountPercent { get; set; }
        //public decimal NetAmount { get; set; }
        //public string ValidFrom { get; set; }
        //public string ValidTo { get; set; }
        //public string JoiningMonth { get; set; }
        //public string JoiningDate { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime DateCreated { get; set; }
        //public string PublishedBy { get; set; }
        //public DateTime DatePublished { get; set; }

    }
}