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
    
    public partial class DepartmentRight
    {
        public int DepartmentID { get; set; }
        public int MenuID { get; set; }
        public bool ShowMenu { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    
        public virtual Department Department { get; set; }
        public virtual User User { get; set; }
        public virtual Menu Menu { get; set; }
        public virtual User User1 { get; set; }
    }
}
