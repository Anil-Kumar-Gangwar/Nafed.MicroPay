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
    
    public partial class FormGroupBDetail2
    {
        public int FormDetailID { get; set; }
        public int FormGroupID { get; set; }
        public Nullable<byte> SectionID { get; set; }
        public string Activities { get; set; }
        public Nullable<decimal> ReportingAuthorityWeightage { get; set; }
        public Nullable<decimal> ReviewingAuthorityWeightage { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual FormGroupBHdr FormGroupBHdr { get; set; }
    }
}
