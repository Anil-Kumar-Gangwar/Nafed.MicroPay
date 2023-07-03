using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MicroPay.Web.Models
{
    public class OfficerStaffViewModel
    {
        [Display(Name = "Staff Count Report")]
        public bool staffCount { get; set; }
        [Display(Name = "Officer Count Report")]
        public bool officerCount { get; set; }
        public ReportType reportType { set; get; }
    }

    public enum ReportType
    {
        Officer = 1,
        Staff = 2,
    }
}