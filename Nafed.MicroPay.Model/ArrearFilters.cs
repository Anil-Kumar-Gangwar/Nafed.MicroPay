using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Nafed.MicroPay.Model
{
 public class ArrearFilters
    {
        [Display(Name = "Branch")]
        public int? branchID { set; get; }
        public int? employeeTypeID { set; get; }
        public bool AllBranch { set; get; }
        
        public string BranchName { get; set; }
        public string BranchCode { get; set; }

        public int? employeeID { set; get; }

        [Display(Name = "Report Type")]
        public int ReportTypeID { set; get; }
        [Display(Name = "Bank")]
        public string BankCode { get; set; }

        [Display(Name = "From date")]
        public DateTime fromdate { set; get; }
        [Display(Name = "To Date")]
        public DateTime todate { set; get; }

        [Display(Name = "Date of generate")]
        public DateTime dateogofgenerate { set; get; }
        [Display(Name = "From period")]
        public string fromPeriod { get; set; }
        [Display(Name = "To period")]
        public string toPeriod { get; set; }

        [Display(Name = "Generate date")]
        public string ArrerPeriodDetailsDA { get; set; }
        public string ArrerPeriodDetailsPay { get; set; }

        [Display(Name = "Order No.")]
        public string OrderNo { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        public List<SelectListModel> listArrerPeriod { get; set; }
        public List<SelectListModel> branchList { set; get; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();
    }

    public class ArrerPeriodDetailsForPAYDA
    {
        public string details { get; set; }
        public string DOG { get; set; }
        public string fromperiod { get; set; }
        public string toperiod { get; set; }

        public string fromperiod1 { get; set; }
        public string toperiod1 { get; set; }

        public string mon { get; set; }
        public DateTime? orderDate { get; set; }
        public string orderNo { get; set; }

    }
}
