using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ArrearManualData
    {
        public string Grade { set; get; }
        public int Sno { set; get; }
        public int arrearmonthlyinputID { get; set; }
        public int ID { get; set; }
        [DisplayName("Employee")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select employee")]
        public int EmployeeId { get; set; }

        public string Empcode { get; set; }
        public string Empname { get; set; }

        public int BranchID { get; set; }

        public string DesignationName { get; set; }
        public string BranchName { get; set; }

        [Display(Name = "Month")]
        [Required(ErrorMessage = "Please select month")]
        public int Month { get; set; }
        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please select year")]


        public int Year { get; set; }
        [Display(Name = "Head Name")]
        [Required(ErrorMessage = "Please select Head Name")]
        public string HeadName { get; set; }

        [Display(Name = "Head Value ")]
        public double HeadValue { get; set; }

        [Required(ErrorMessage = "Please enter DA rates")]
        [Display(Name = "DA Rates")]
        public double E_01 { get; set; }
        public Nullable<System.DateTime> Saldate { get; set; }

        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        [Display(Name = "Basic Salary")]
        public Nullable<decimal> BasicSalary { get; set; }

        public Nullable<decimal> AdditionalOverTime { get; set; }
        public Nullable<decimal> ChildrenEducationAllowance { get; set; }

        public Nullable<decimal> DriverAllowance { get; set; }

        [Display(Name = "Income Tax")]
        public Nullable<decimal> IncomeTaxDeduction { get; set; }
        public Nullable<decimal> LeaveWithoutPay { get; set; }
        public Nullable<decimal> LifeInsurance { get; set; }
        public Nullable<decimal> MedicalReimbursement { get; set; }
        public Nullable<decimal> MiscellaneousDeduction1 { get; set; }
        public Nullable<decimal> MiscellaneousDeduction2 { get; set; }
        public Nullable<decimal> MiscellaneousDeduction3 { get; set; }
        public Nullable<decimal> MiscellaneousIncome1 { get; set; }
        public Nullable<decimal> MiscellaneousIncome2 { get; set; }
        public Nullable<decimal> MiscellaneousIncome3 { get; set; }
        public Nullable<decimal> NafedBazaarDeduction { get; set; }
        public Nullable<decimal> NewsPaper { get; set; }
        public Nullable<decimal> OverTime { get; set; }
        public Nullable<decimal> PersonalPay { get; set; }
        public Nullable<decimal> PetrolCharges { get; set; }
        public Nullable<decimal> ProfessionalTax { get; set; }
        public Nullable<decimal> SpecialAllowance { get; set; }
        public Nullable<decimal> SpecialPay { get; set; }
        public Nullable<decimal> SundryAdvance { get; set; }
        public Nullable<decimal> Telephone { get; set; }
        public Nullable<decimal> TransportAllowance { get; set; }
        public Nullable<decimal> WashingAllowance { get; set; }

        public string error { set; get; }
        public string warning { set; get; }

        public bool isDuplicatedRow { set; get; }

        [Display(Name = "Date of generate")]
        public DateTime dateogofgenerate { set; get; }

    }

    public class ManualDataImportColValues
    {
        public List<string> empCode { set; get; }
    }
}