using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
  public class FileManagement
    {
        public int FileID { get; set; }
       
        [Display(Name ="File No.")]
        [Required(ErrorMessage ="Please Enter File No.")]
        public string DiaryNo { get; set; }
        [Display(Name = "Register No.")]
        public string OtherDiaryNo { get; set; }
        [Display(Name = "Department/Section")]
        public int DepartmentID { get; set; }
        [Display(Name = "File Subject")]
        [Required(ErrorMessage = "Please Enter File Subject.")]
        public string FileSubject { get; set; }
        [Display(Name = "File Name")]
        //[Required(ErrorMessage = "Please Enter File Name.")]
        public string FileName { get; set; }
        public Nullable<int> FileLastStatus { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public int FileWorkFlowID { get; set; }

        [Display(Name ="File Type")]
        [Required(ErrorMessage = "Please Select File Type.")]
        [Range(1,Int16.MaxValue,ErrorMessage ="Please Select File Type.")]
        public int FileTypeID { get; set; }
        [Display(Name ="Putup Date")]
        [Required(ErrorMessage ="Please Enter Putup Date.")]
        public Nullable<DateTime> FilePutup { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string FileTypeName { get; set; }
        public byte? StatusID { get; set; }
        public string Remarks { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public int? ParkByID { get; set; }       
        public string ReceiverDepartment { get; set; }
        public Nullable<DateTime> SendDate { get; set; }
        public int? ReferenceID { get; set; }

        public List<FileTrackingDocuments> fileDocumentsList = new List<FileTrackingDocuments>();
        public string TAT { get; set; }
        public Nullable<System.DateTime> Readdate { get; set; }
        public Nullable<int> Readflag { get; set; }
        public string Purpose { get; set; }
        public string ForwardThrough { get; set; }
    }
}
