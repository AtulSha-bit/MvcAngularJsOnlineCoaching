using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineCoaching.Angular_Js_Data_Layer
{
    public class MultipleCourses
    {
      public string Course { get; set; }
      public int Duration { get; set; }
      public string ValidFrom { get; set; }
      public string ValidTo { get; set; }
      public decimal Fees { get; set; }
      public int Discount { get; set; }
      public decimal FeesWithDiscount { get; set; }
      public decimal NetAmount { get; set; }
      public string Month { get; set; }
      public string JoiningDate { get; set; }
      public int StructureId { get; set; }
    }
}