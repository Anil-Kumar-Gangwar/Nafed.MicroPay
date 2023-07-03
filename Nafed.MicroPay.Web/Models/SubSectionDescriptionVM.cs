using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Models
{
    public class SubSectionDescriptionVM
    {
        public List<SelectListItem> fYears { set; get; }
        public List<SelectListModel> sections { set; get; } = new List<SelectListModel>();

        public List<SelectListModel> subSections { set; get; }=new List<SelectListModel>(); 

        [Display(Name = "Financial Year")]
        public string selectedFYear { set; get; }

        [Display(Name = "Section")]
        public int sectionID { set; get; }

        [Display(Name = "Sub Section")]
        public int subSectionID { set; get; }

        public List<DeductionSubSectionDescription> subSectionDescpList { set; get; } = new List<DeductionSubSectionDescription>();
        public DeductionSubSectionDescription deductionSubSectionDesc { set; get; } = new DeductionSubSectionDescription();
    }
}