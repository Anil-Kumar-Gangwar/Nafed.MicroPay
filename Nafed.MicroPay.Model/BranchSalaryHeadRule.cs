using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public  class BranchSalaryHeadRule
    {
        public int BranchFormulaID { get; set; }
        public string FieldName { get; set; }
        public int BranchID { get; set; }
        public bool FormulaColumn { get; set; }
        public bool FixedValueFormula { get; set; }
        public string LookUpHead { get; set; }
        public string LookUpHeadName { get; set; }
        public Nullable<decimal> FixedValue { get; set; }
        public Nullable<decimal> LowerRange { get; set; }
        public Nullable<decimal> UpperRange { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        public Nullable<int> EmployeeTypeID { get; set; }
        public int SelectedBranchID { set; get; }

        [Display(Name = "Location")]
        public List<SelectListModel> BranchList { get; set; }

        public string BranchCode { set; get; }
        public FormulaType formulaType { set; get; }

        public string ActionType
        {
            set; get;
        }
        public string Type
        {
            get
            {

                if (FormulaColumn)
                    return "Formula";
                else
                    return "Fixed Value";
            }
        }

        public string BranchName { set; get; }
    }
}
