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
    
    public partial class GetOutboxFiles_Result
    {
        public string ReceiverDepartment { get; set; }
        public string Receiver { get; set; }
        public int FileID { get; set; }
        public string DiaryNo { get; set; }
        public string DepartmentName { get; set; }
        public string FileSubject { get; set; }
        public string FileType { get; set; }
        public System.DateTime FilePutup { get; set; }
        public string Scomments { get; set; }
        public Nullable<int> ReferenceID { get; set; }
    }
}
