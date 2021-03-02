using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineCoaching.Angular_Js_Data_Layer
{
    public class Student
    {
        public int CourseId { get; set; }
        public int StructureId { get; set; }
        public string StudentName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string LandMark { get; set; }
        public string PinCode { get; set; }
        public decimal Fees { get; set; }
        public decimal FeeAfterDiscount { get; set; }
        public string DiscountPercent { get; set; }

        public string PaymentMode { get; set; }
        public Boolean IsPaid { get; set; }
        public DateTime PaymentDate { get; set; }

        
        public decimal NetAmount { get; set; }
        public string JoiningDate { get; set; }
        public string Month { get; set; }
        public string Remarks { get; set; }

        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DatePublished { get; set; }
        public Boolean IsPublished { get; set; }
        

    }
}