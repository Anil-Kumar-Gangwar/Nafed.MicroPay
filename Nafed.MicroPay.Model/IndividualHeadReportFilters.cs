using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class IndividualHeadReportFilters
    {
        [Display(Name = "Month")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Month")]
        public int salMonth { set; get; }
        [Display(Name = "Year")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Year")]
        public int salYear { set; get; }
        [Display(Name = "Employee Type")]
        public int selectedEmployeeTypeID { set; get; }
        public List<SelectListModel> employeeTypesList { set; get; }
        [Display(Name = "Branch")]
        public int branchId { set; get; }
        public List<SelectListModel> branchList { set; get; }

        [Display(Name = "Head Name")]
        [Required(ErrorMessage = "Please Select Head Name")]
        public string monthlyInputHead { get; set; }
        public IEnumerable<SalaryHeadField> monthlyInputHeadList { set; get; }

        [Display(Name = "All Branches")]
        public bool AllBranch { get; set; }
        [Display(Name = "View PF No.")]
        public bool ViewPFNo { get; set; }
        [Display(Name = "All Branch Without HO")]
        public bool AllBranchWithHo { get; set; }
        [Display(Name = "View Pay Slip No.")]
        public bool ViewPaySlipNo { get; set; }
        public string HeadDescription { get; set; }
        public bool isValid { get; set; }

        public bool chkgrp { get; set; }
        [Display(Name = "From Month")]
        public int fMonth { set; get; }
        [Display(Name = "From Year")]
        public int fYear { set; get; }
        [Display(Name = "To Month")]
        public int tMonth { set; get; }
        [Display(Name = "To Year")]
        public int tYear { set; get; }
        [Display(Name = "LIC-TCS-GIS")]
        public bool licTcsGis { get; set; }
        [Display(Name = "HBL-Loan")]
        public bool hblLoan { get; set; }
        [Display(Name = "PF-Monthly")]
        public bool pfMonthly { get; set; }
        [Display(Name = "Head Name")]
        public IList<string> selectedInputHeads { get; set; }
    }
}
