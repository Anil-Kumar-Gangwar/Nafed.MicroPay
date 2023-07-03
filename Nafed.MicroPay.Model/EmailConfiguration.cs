using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class EmailConfiguration
    {
        public int EmailConfigurationID { get; set; }

        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid email.")]
        [Display(Name = "From Email")]
        public string ToEmail { get; set; }
       
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid email.")]
        public string Bcc { get; set; }
        public string CcEmail { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        [Display(Name = "Username")]
        public string UserName { get; set; }


        [StringLength(100, ErrorMessage = "{0} not be exceed 100 char")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} not be exceed 100 char")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public string CPassword { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        public string Server { get; set; }

        [Required]
        [Display(Name = "Port")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Invalid Port no.")]
        public int Port { get; set; }
        [Display(Name = "Signature(If any)")]
        public string Signature { get; set; }

        [Display(Name = "SSL Status")]
     
        public bool SSLStatus { get; set; }

        [Display(Name = "Maintenance")]
        public bool IsMaintenance { get; set; }

        [Display(Name = "Maintenance Mode Till")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm tt}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> MaintenanceDateTime { get; set; }

        public bool IsSSLEnable { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
       
    }
}
