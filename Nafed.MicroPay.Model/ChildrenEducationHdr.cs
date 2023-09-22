using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ChildrenEducationHdr
    {
        public int ChildrenEduHdrID { get; set; }
        public string ReportingYear { get; set; }
        public int EmployeeId { get; set; }
        public int DesignationId { get; set; }
        public int DepartmentId { get; set; }
        public int BranchId { get; set; }
        public string ReceiptNo { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> ReceiptDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> ReceiptDate2 { get; set; }

        public string ReceiptNo2 { get; set; }
      
        public Nullable<decimal> Amount { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Branch { get; set; }
        public List<Model.ChildrenEducationDetails> ChildrenEducationDetailsList { get; set; } = new List<ChildrenEducationDetails>();
        public List<Model.SelectListModel> DependentList { get; set; }
        public string StrReceiptDate { get; set; }
        public string StrReceiptDate2 { get; set; }
        public List<ChildrenEducationDocuments> ChildrenEducationDocumentsList { get; set; }

        public bool IsDependentMatch { get; set; }
        public int StatusId { get; set; }
        public string buttonTypeDetail { get; set; }
    }

    public enum ChildrenEducationStatus
    {
        [Display(Name = "Save")]
        SavedByEmployee = 1,
        [Display(Name = "Submitted")]
        SubmitedByEmployee = 2,
    }
}
