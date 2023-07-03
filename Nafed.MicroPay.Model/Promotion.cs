using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class Promotion : BaseEntity
    {
        public int TransID { get; set; }

        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }

        [Display(Name = "Cadre Code")]
        public string CadreCode { get; set; }

        [Display(Name = "Designation Code")]
        public string DesignationCode { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        [Display(Name = "From Date(New DOJ)")]
        [Required(ErrorMessage = "Please Enter From Date")]

        public System.DateTime? FromDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        [Display(Name = "To Date")]
        public Nullable<System.DateTime> ToDate { get; set; }

        [Display(Name = "Seniority Code")]
        public Nullable<int> SeniorityCode { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        [Display(Name = "Confirmation Date")]
        public Nullable<System.DateTime> ConfirmationDate { get; set; }

        [Display(Name = "Confirmed")]
        public bool Confirmed { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [Display(Name = "Basic Salary")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> E_Basic { get; set; }
        public Nullable<decimal> OldBasic { get; set; }
        public string WayOfPosting { get; set; }

        [Range(1, Int32.MaxValue, ErrorMessage = "Please select way of posting.")]
        public Nullable<int> WayOfPostingID { get; set; }

        [Display(Name = "TS")]
        public bool NewTS { get; set; }
        public string DESGSTUFF { get; set; }
        public string SCALE { get; set; }
        public string DesgName { get; set; }
        public string NotionalIncrement { get; set; }
        public string AnnualIncrementWEF { get; set; }

        public string NextIncremantdate { get; set; }
        public string NewBasic { get; set; }
        public string FLAG { get; set; }

        public System.DateTime CreatedOn { get; set; }
        public new int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }

        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> EmployeeID { get; set; }

        [Required(ErrorMessage = "Please select cadre.")]
        [Display(Name = "Cadre")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select cadre.")]
        public Nullable<int> CadreID { get; set; }

        [Required(ErrorMessage = "Please select designation.")]
        [Display(Name = "Designation")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select designation.")]
        public Nullable<int> DesignationID { get; set; }

        public List<SelectListModel> cadreList { set; get; }
        public List<SelectListModel> designationList { set; get; } = new List<SelectListModel>();

        [Display(Name = "Way Of Posting")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select way of posting.")]
        public WayOfPosting wayOfPostingEnum { set; get; }

        [Display(Name = "Cota")]
        public Cota cota { set; get; }

        public string Action { set; get; }
        public bool IsValidInputs { set; get; } = true;
        public string ValidationMessage { set; get; }
        public int currDesignationID { set; get; }
        public Designation currentDesignation { set; get; }
        public Designation newDesignation { set; get; }
        public bool Saved { set; get; }

        public string CaderName { get; set; }
   
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Order Date of Promotion")]
        public Nullable<System.DateTime> OrderOfPromotion { get; set; }
    }

    public enum WayOfPosting
    {
        Upgraded = 1,
        Created = 2,
        Promotion = 3,
        Demotion = 4,
        LCT = 5,
        Reversion = 6,
        Direct = 7
    }


}
