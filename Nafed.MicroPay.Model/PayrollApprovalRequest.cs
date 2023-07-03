using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class PayrollApprovalRequest
    {

        public string emailTo { set; get; }
        public int sno { set; get; }
        public int ApprovalRequestID { get; set; }
        public int ProcessID { get; set; }
        public string Period { get; set; }
        public int EmployeeTypeID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public string Comments { get; set; }

        public string BranchName { set; get; }

        public DateTime? periodInDateFormat
        {
            get
            {


                if (!string.IsNullOrEmpty(Period))
                {
                    var year = Convert.ToInt32(Period.Substring(0, 4));
                    var month = Convert.ToInt32(Period.Substring(4, 2));
                    return (DateTime?)new DateTime(year, month, 1);
                }
                else
                    return (DateTime?)null;
            }
        }

        public int Reporting1 { set; get; }
        public int ? Reporting2 { set; get; }
        public int ? Reporting3 { set; get; }

        public string EmpployeeTypeName { set; get; }
        public string BranchCode { get; set; }
        public byte Status { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    }


    public enum ApprovalStatus
    {
        [Display(Name = "Requested by Reporting 1")]
        RequestedByReporting1 = 1,
        [Display(Name = "Rejected by Reporting 2")]
        RejectedByReporting2 = 2,
        [Display(Name = "Approved by Reporting 2")]
        ApprovedByReporting2 = 3,
        [Display(Name = "Rejected by Reporting 3")]
        RejectedByReporting3 = 4,
        [Display(Name = "Approved by Reporting 3")]
        ApprovedByReporting3 = 5
    }
}
