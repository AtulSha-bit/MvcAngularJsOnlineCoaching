﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineCoaching.Angular_Js_Data_Layer
{
    public class InternDocumentsUpdate
    {
        public int DocumentId { get; set; }
        public int UserId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public string DoucmentNo { get; set; }
    }
}