using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Models
{
    public class SalaryHeadFormulaEditorVM
    {
        public string fieldName { set; get; }

        public int? BranchFormulaID { set; get; }

        public IEnumerable<string> selectedFields { set; get; }
        public IEnumerable<dynamic> fieldList { set; get; } = new List<dynamic>();

        [Display(Name = "Head Field")]
        public List<dynamic> headFields { set; get; }

        //[DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        //  [MaxLength(3)]

        [Required(ErrorMessage = "Percentage is required.")]
        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 100, ErrorMessage = "{0}, cannot be less than zero")]
        public Nullable<decimal> Percentage { get; set; }

        public int? EmployeeTypeID { set; get; }

    }
}