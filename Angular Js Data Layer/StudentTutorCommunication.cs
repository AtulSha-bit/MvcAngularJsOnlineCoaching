using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineCoaching.Angular_Js_Data_Layer
{
    // Created By Dilshad A. for Store MUltiple StudentName/Tutor Name for Communication on 14 Sept 2020
    public class StudentTutorCommunication
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Int64 StudentId { get; set; }
        public Int64 TutorId { get; set; }
       
    }
}