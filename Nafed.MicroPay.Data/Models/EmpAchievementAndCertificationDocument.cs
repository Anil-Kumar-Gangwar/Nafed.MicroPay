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
    
    public partial class EmpAchievementAndCertificationDocument
    {
        public int DocumentID { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentFilePath { get; set; }
        public Nullable<int> LinkedAchivementID { get; set; }
        public Nullable<int> LinkedCertificateID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    
        public virtual EmployeeAchievement EmployeeAchievement { get; set; }
        public virtual EmployeeCertification EmployeeCertification { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
