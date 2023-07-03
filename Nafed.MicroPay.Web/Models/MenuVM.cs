using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Models
{
    public class MenuVM
    {
  
        public int MenuID { get; set; }
        [Display(Name = "Menu Name :")]
        [Required(ErrorMessage = "Menu Name is required.")]
        public string MenuName { get; set; }
        [Display(Name = "Parent :")]
        public Nullable<int> ParentID { get; set; }
        [Display(Name = "Sequence :")]
        public Nullable<int> SequenceNo { get; set; }
        [Display(Name = "URL :")]
        [Required(ErrorMessage = "URL is required.")]
        public string URL { get; set; }

        [Display(Name = "Icon Class :")]
        public string IconClass { get; set; }

        [Display(Name = "Active :")]
        public bool IsActive { get; set; }


        [Display(Name = "Visible On Mobile Screen :")]
        public bool ActiveOnMobile { get; set; }

        public string ParentName { get; set; }

        [AllowHtml]
        public string Help { get; set; }


        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}