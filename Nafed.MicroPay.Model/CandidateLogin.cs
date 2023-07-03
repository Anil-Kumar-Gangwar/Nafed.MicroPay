using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Nafed.MicroPay.Model
{
    public class CandidateLogin
    {
        [DisplayName("Registration No :")]
        [Required(ErrorMessage = "Please enter your Registration No.")]
        public string RegistrationNo { set; get; }

        [DisplayName("Date of birth :")]     
        [Required(ErrorMessage = "Please enter your Date of birth")]
        public DateTime DOB { set; get; }
    }
}
