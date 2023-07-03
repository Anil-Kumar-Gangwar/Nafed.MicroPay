using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class PayrollApprovalSetting
    {
        public int ProcessAppID { get; set; }
        public int ProcessID { get; set; }

        public string ProcessName { get; set; }

        public int OldReporting1 { get; set; }
        public Nullable<int> OldReporting2 { get; set; }
        public Nullable<int> OldReporting3 { get; set; }


        [Display(Name = "Reporting 1")]
        [Required]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select Reporting 1")]
        public int Reporting1 { get; set; }

        [Display(Name = "Reporting 2")]
      //  [Required]
     //   [Range(1, Int64.MaxValue, ErrorMessage = "Please select Reporting 2")]
        public int? Reporting2 { get; set; }

        [Display(Name = "Reporting 3")]
       [Required]
       [Range(1, Int64.MaxValue, ErrorMessage = "Please select Reporting 3")]

        public int? Reporting3 { get; set; }

        public System.DateTime FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
