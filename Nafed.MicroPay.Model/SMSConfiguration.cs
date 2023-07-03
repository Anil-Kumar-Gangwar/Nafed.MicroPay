
namespace Nafed.MicroPay.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class SMSConfiguration
    {
        public int SMSConfigurationID { get; set; }

        //[Required(ErrorMessage ="App Key is required.")]
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        [Display(Name = "Api Key")]
        public string ApiKey { get; set; }
        [Required(ErrorMessage = "SMS Url is required.")]
        [Display(Name = "SMS Url")]
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        public string SMSUrl { get; set; }

        
        [Required(ErrorMessage = "Sender ID is required.")]
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        public string SenderID { get; set; }

        //[Required]
        [Display(Name = "Channel")]
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        public string Channel { get; set; }

        //[Required]
        [Display(Name = "DCS")]
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        public string DCS { get; set; }

        //[Required(ErrorMessage = "FlashSMS is required.")]
        [Display(Name = "FlashSMS")]
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        public string FlashSMS { get; set; }

        //[Required]
        [Display(Name = "Route")]
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        public string Route { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
