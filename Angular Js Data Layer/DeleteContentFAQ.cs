using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineCoaching.Angular_Js_Data_Layer
{
    public class DeleteContentFAQ
    {
        public int ContentFAQId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool IsPublished { get; set; }
    }
}