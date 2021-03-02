using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineCoaching.Angular_Js_Data_Layer
{
    public class DocumentArray
    {
        public string ResourceName { get; set; }
        public int InternshipId { get; set; }
        public int ResourceId { get; set; }

    }

    public class DeleteInternTutorArray
    {
        public int InternshipId { get; set; }
        public int TutorId { get; set; }
        public int InternTutorId { get; set; }
    }


    public class DeleteInternResourceArray
    {
        public int InternshipId { get; set; }
        public string InternshipName { get; set; }
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
    }

}