﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineCoaching.Angular_Js_Data_Layer
{
    public class TutorToBrandStudent
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int TutorId { get; set; }
        public int StudentCategoryId { get; set; }
        public int CourseStructureId { get; set; }

        
    }
}