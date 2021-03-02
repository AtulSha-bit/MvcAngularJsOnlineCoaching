using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineCoaching.Angular_Js_Data_Layer
{
    public class CreateInternshipStructure
    {
        public int InternStructureId { get; set; }
        public int InternshipId { get; set; }
        public string DurationName { get; set; }
        public int DurationMonths { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string InternshipType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public Boolean IsPublished { get; set; }
        public DateTime LastApplyDate { get; set; }
        public int TotalAvailableSeat { get; set; }


        public decimal Fees { get; set; }
        public int Discount { get; set; }
        public decimal FeeAfterDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Stipened {get; set;}
        public int DurationId { get; set; }


        public DateTime DatePublished { get; set; }
    }
}