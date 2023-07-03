using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class SalaryHead
    {
        [Display(Name = "Employee Type")]
        public Nullable<int> EmployeeTypeID { get; set; }

        public int? BranchFormulaID { set; get; }

        [Display(Name = "Field Name")]
        public string FieldName { get; set; }

        [Display(Name = "Description")]
        public string FieldDesc { get; set; }

        [Display(Name = "Abbreviation")]
        public string Abbreviation { get; set; }

        [Display(Name = "Formula")]
        public bool FormulaColumn { get; set; }

        public string FormulaText
        {
            get
            {
                return LookUpHead;
            }
        }

        public string LookUpHead { get; set; }

        [Display(Name = "Round Up To")]
        [Range(0, 1000, ErrorMessage = "{0}, cannot be less than zero")]
        public Nullable<int> RoundingUpto { get; set; }

        [Display(Name = "Working Days Dependant")]
        public bool AttendanceDep { get; set; }

        [Display(Name = "Rounding To Higher")]
        public bool RoundToHigher { get; set; }

        [Display(Name = "Monthly Input")]
        public bool MonthlyInput { get; set; }

        [Display(Name = "Seq No.")]
        [Range(1, 1000, ErrorMessage = "{0}, cannot be less than 1 and greater than 100")]

        public int SeqNo { get; set; }
        public string LookUpHeadName { get; set; }

        [Display(Name = "Active Field")]
        public bool ActiveField { get; set; }

        [Display(Name = "Special Field")]
        public bool SpecialField { get; set; }

        [Display(Name = "Special Master Field")]
        public bool SpecialFieldMaster { get; set; }

        [Display(Name = "Carry Forward(For MI Only)")]
        public bool FromMaster { get; set; }

        [Display(Name = "Is Loan Head")]

        public bool LoanHead { get; set; }

        [Display(Name = "Conditional Field")]
        public bool Conditional { get; set; }

        [Display(Name = "For Management Trainees")]
        public bool MT { get; set; }

        [Display(Name = "For Consultants")]
        public bool C { get; set; }

        [Display(Name = "For Daily Wages")]
        public bool DW { get; set; }

        [Display(Name = "For Advisors")]
        public bool A { get; set; }

        [Display(Name = "For Contract")]
        public bool CT { get; set; }

        [Display(Name = "For Deputation")]
        public bool DC { get; set; }

        [Display(Name = "For Consolidated Wage")]
        public bool CW { get; set; }

        [Display(Name = "Lower Range>=")]
        [Range(0, 9999999999.99, ErrorMessage = "{0}, cannot be less than zero ")]

        public Nullable<decimal> LowerRange { get; set; }
        [Display(Name = "Upper Range<=")]
        [Range(0, 9999999999.99, ErrorMessage = "{0}, cannot be less than zero ")]
        public Nullable<decimal> UpperRange { get; set; }

        [Display(Name = "Fixed Value")]
        public bool FixedValueFormula { get; set; }

        [Range(0, 9999999999.99, ErrorMessage = "{0}, cannot be less than zero")]
        public decimal FixedValue { get; set; }

        [Display(Name = "Slab")]

        public bool Slab { get; set; }

        [Display(Name = "Location Dependent")]
        public bool LocationDependent { get; set; }

        public FormulaType formulaType { set; get; }

        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }


        public int SelectedBranchID { set; get; }

        [Display(Name = "Location")]
        public List<SelectListModel> BranchList { get; set; }

        [Display(Name = "Head Field")]
        public List<dynamic> headFields { set; get; }
        [MaxLength(3)]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> Percentage { get; set; }

        public List<string> formulaList { set; get; }

        [Display(Name = "Check Salary Table")]
        public bool CheckHeadInEmpSalTable { set; get; }

        public string ActionType
        {
            set; get;
        }

    }

    public enum FormulaType
    {
        FormulaColumn = 1,
        FixedValueFormula = 2,
        MonthlyInput = 3,
        Slab = 4

    }
}
