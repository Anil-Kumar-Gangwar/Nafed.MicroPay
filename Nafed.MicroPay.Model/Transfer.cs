using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class Transfer
    {
        public int TransId { get; set; }
        public string EmployeeCode { get; set; }
        public Nullable<int> BranchCode { get; set; }
        [Display(Name = "From Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Select From Date")]
        public Nullable<System.DateTime> FromDate { get; set; }
        [Display(Name = "To Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ToDate { get; set; }
        public string BranchName { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        [Display(Name = "Branch")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Branch")]
        public int BranchId { get; set; }

        public List<SelectListModel> branchList { set; get; }

        public string FormActionType { get; set; }

        public bool Saved { get; set; }

        public string ValidationMessage { get; set; }

        public bool IsValidInputs { get; set; } = true;

        public string Name { get; set; }
    }
}
