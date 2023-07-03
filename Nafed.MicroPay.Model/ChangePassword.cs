using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class ChangePassword
    {
        public int UserID { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }
        public Nullable<int> EmployeeID { get; set; }

        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }
        public string hdOldPassword { get; set; }
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
      //  [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{6,20}$",ErrorMessage ="Invalid Password Format.")]
    
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)\S{6,20}$", ErrorMessage = "Invalid Password Format")]
        public string Password { get; set; }
        public string hdPassword { get; set; }
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        [Compare("Password", ErrorMessage = "Password and confirm password must be same.")]
        [Display(Name = "Confirm Password")]
        public string CPassword { get; set; }
        public string hdCPassword { get; set; }
    }
}
