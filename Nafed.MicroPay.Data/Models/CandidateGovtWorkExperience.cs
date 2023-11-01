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
    
    public partial class CandidateGovtWorkExperience
    {
        public int Counter { get; set; }
        public string OrganisationName { get; set; }
        public string Postheldonregularbasis { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public Nullable<decimal> PayinPB { get; set; }
        public Nullable<decimal> GP { get; set; }
        public Nullable<decimal> BasicPay { get; set; }
        public string LevelAsPerCPC { get; set; }
        public int Natureofappointment { get; set; }
        public string Natureofdutiesindetail { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public int RegistrationID { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual CandidateRegistration CandidateRegistration { get; set; }
    }
}
