using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MicroPay.Web.Models
{
    public class ArrearViewModel
    {
        [Display(Name = "Branch")]
        public int? branchID { set; get; }

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

    
        public List<SelectListModel> branchList { set; get; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();
    }

   
}