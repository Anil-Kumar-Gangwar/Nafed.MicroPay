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
    
    public partial class APARReviewedSignedCopy
    {
        public int FormID { get; set; }
        public int FormGroupID { get; set; }
        public Nullable<int> WorkFlowID { get; set; }
        public string UploadedDocName { get; set; }
        public int UploadedBy { get; set; }
        public System.DateTime UploadedOn { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string ReportingYr { get; set; }
    
        public virtual AppraisalForm AppraisalForm { get; set; }
        public virtual ProcessWorkFlow ProcessWorkFlow { get; set; }
        public virtual User User { get; set; }
    }
}
