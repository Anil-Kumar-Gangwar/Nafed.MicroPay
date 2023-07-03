using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class DeductionSectionVM
    {
        public List<SelectListItem> fYears { set; get; }


        [Display(Name = "Financial Year")]
        [Required(ErrorMessage = "Please select financial year")]
        public string selectedFYear { set; get; }

        public DeductionSection deductionSection { set; get; } = new DeductionSection();
        public List<DeductionSection> deductionSections { set; get; } = new List<DeductionSection>();

       
       
    }
}