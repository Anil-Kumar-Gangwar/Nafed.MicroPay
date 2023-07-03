using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ConfirmationPRSectionRemarks
    {
        public int FormHdID { get; set; }
        public int FormHeaderID { get; set; }
        public int EmployeeID { get; set; }
        public int FormTypeID { get; set; }
        [Required(ErrorMessage = "Please enter personal section remark.")]
        public string PersonalSectionRemark { get; set; }
        public bool PRSubmit { get; set; }

        public List<ConfirmationFormHdr> confHdrList { get; set; }

        public int ProcessID { get; set; }
        public int StatusID { get; set; }
        public DateTime ? DueDate { get; set; }
        public int ? UpdatedBy { get; set; }
        [Required(ErrorMessage = "Please enter file no.")]
        public string FileNo { get; set; }
        [Required(ErrorMessage = "Please enter general manager name.")]
        public string GeneralManager { get; set; }
        [Required(ErrorMessage = "Please enter general manager designation.")]
        public string GMDesignation { get; set; }

        [Required(ErrorMessage = "Please enter general manager email ID.")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
       ErrorMessage = "Please Enter Correct Email Address")]
        public string GMEmailID { set; get; }
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
      ErrorMessage = "Please Enter Correct Email Address")]
        public string EmailID1 { set; get; }
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
      ErrorMessage = "Please Enter Correct Email Address")]
        public string EmailID2 { set; get; }
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
      ErrorMessage = "Please Enter Correct Email Address")]
        public string EmailID3 { set; get; }
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
      ErrorMessage = "Please Enter Correct Email Address")]
        public string EmailID4 { set; get; }
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
      ErrorMessage = "Please Enter Correct Email Address")]
        public string EmailID5 { set; get; }

        public int TitleID { get; set; }
        public int GenderID { get; set; }
        public string PayScale { get; set; }
        public string Employee { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Branch { get; set; }
        public DateTime? OrderDate { get; set; }
        public string DVHEmployeeCode { get; set; }
    }
}
