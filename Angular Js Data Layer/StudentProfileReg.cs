using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineCoaching.Angular_Js_Data_Layer
{
    public class StudentProfileReg
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string ProfileImage { get; set; }
        public string Fblink { get; set; }

        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string Twitterlink { get; set; }
        public string Instalink { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; }
        public int StudentCategoryId { get; set; }
        public int StudentSubCategoryId { get; set; }
        public int TutorId { get; set; }
        public string StudentType { get; set; }
        public Int64 BatchId { get; set; }
    }
}