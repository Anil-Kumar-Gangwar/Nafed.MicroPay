using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
    public class FTSUser
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Please enter User Id")]
        [Display(Name = "User Id")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter Password")]
        public string Password { get; set; }
        public string hdPassword { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        [Required(ErrorMessage = "Please enter Comfirm Password")]
        [Display(Name = "Comfirm Password")]
        public string ConfirmPassword { get; set; }
        public string hdCPassword { get; set; }
        public string hdCp { get; set; }
    }
}
