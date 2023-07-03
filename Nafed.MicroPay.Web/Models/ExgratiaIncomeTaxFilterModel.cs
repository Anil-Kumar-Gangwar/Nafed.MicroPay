using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MicroPay.Web.Models
{
    public class ExgratiaIncomeTaxFilterModel
    {
        [Display(Name = "Branch")]
        [Required(ErrorMessage = "Please Select Year")]
        public int branchID { set; get; }

        [Display(Name = "Employee Type")]
        [Required(ErrorMessage = "Please Select Year")]
        public int EmpTypeID { set; get; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please Select Year")]
        public string selectYearID { get; set; }
    }
}