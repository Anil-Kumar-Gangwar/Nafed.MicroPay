using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Models
{
    public class Form12BBFilterVM
    {
        public List<SelectListItem> fYears { set; get; }

        [Display(Name = "Financial Year")]
        public string selectedFYear { set; get; }
    }
}