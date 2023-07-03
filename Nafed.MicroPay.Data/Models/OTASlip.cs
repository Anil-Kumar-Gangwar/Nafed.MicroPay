//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nafed.MicroPay.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OTASlip
    {
        public int ApplicationID { get; set; }
        public int EmployeeID { get; set; }
        public string InstructedBy { get; set; }
        public System.DateTime HolidayDate { get; set; }
        public System.TimeSpan HolidayFromTime { get; set; }
        public System.TimeSpan HolidayToTime { get; set; }
        public Nullable<System.DateTime> IndicatedDate { get; set; }
        public Nullable<System.DateTime> AttendedDate { get; set; }
        public Nullable<System.TimeSpan> AttendedFromTime { get; set; }
        public Nullable<System.TimeSpan> AttendedToTime { get; set; }
        public string ReportingTo { get; set; }
        public string ReviewingTo { get; set; }
        public byte StatusID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
    
        public virtual tblMstEmployee tblMstEmployee { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
