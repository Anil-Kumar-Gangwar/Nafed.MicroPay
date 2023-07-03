using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class DeductionSubSectionVM
    {
        public List<SelectListItem> fYears { set; get; }
        public List<SelectListModel> sections { set; get; } = new List<SelectListModel>();

        [Display(Name = "Financial Year")]
        public string selectedFYear { set; get; }

        [Display(Name = "Section")]
        public int sectionID { set; get; }

        public List<DeductionSubSection> subSectionList { set; get; } = new List<DeductionSubSection>();
        public DeductionSubSection deductionSubSection { set; get; } = new DeductionSubSection();


    }
}