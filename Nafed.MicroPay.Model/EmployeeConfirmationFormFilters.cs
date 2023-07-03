using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class EmployeeConfirmationFormFilters
    {
        [Display(Name = "Form Type")]
        public int? FormTypeId { get; set; }
        public List<SelectListModel> FormTypeList { get; set; }
        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FromDate { get; set; }
        [Display(Name = "To Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ToDate { get; set; }
        public List<EmployeeConfirmationViewDetails> employeeConfirmationViewDetails { get; set; }
    }

    public class EmployeeConfirmationViewDetails
    {
        public int EmployeeId { get; set; }
        public string Employee { get; set; }
        public int ProcessID { get; set; }
        public string Designation { get; set; }
        public DateTime DueDate { get; set; }
        public int StatusID { get; set; }
        public int FormTypeId { get; set; }
        public string ProcessName { get; set; }
        public string Status { get; set; }
        public int FormHdrID { get; set; }
        public int HeaderId { get; set; }
        public int FormState { get; set; }
        public int FormTypeID { get; set; }
        public string BadgeStatus { get; set; }
        public bool PRSubmit { get; set; }
        public string PersonalSectionRemark { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string FileNo { get; set; }
        public string GeneralManager { get; set; }
        public string ManagingDirector { get; set; }

        public int TitleID { set; get; }
        public int GenderID { set; get; }

        public string GmEmailID { set; get; }
    }
}
