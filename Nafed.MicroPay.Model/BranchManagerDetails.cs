using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class BranchManagerDetails :BaseEntity
    {
        public int Id { get; set; }
        public int BranchCode { get; set; }
        public bool Saved { get; set; }
        public string BranchName { set; get; }
        public string EmployeeCode { get; set; }

        [Display(Name = "From Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Select From Date")]
        public Nullable<System.DateTime> DateFrom { get; set; }

        [Display(Name ="To Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
       
        public Nullable<System.DateTime> DateTo { get; set; }
        public Nullable<bool> BranchOffice { get; set; }
        public Nullable<bool> OnLeave { get; set; }
        public Nullable<bool> OnTour { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public new int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> EmployeeID { get; set; }

        [Display(Name = "Branch")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Branch")]
        public int branchID { get; set; }
        public List<SelectListModel> branchList { set; get; }
       public bool IsValidInputs { set; get; } = true;
      public  string ValidationMessage { set; get; }
    }
}
