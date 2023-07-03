using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class Insurance
    {
        public int InsuranceId { get; set; }
        [Display(Name ="Employee")]
        [Required(ErrorMessage ="Please select employee")]
        [Range(1,Int64.MaxValue,ErrorMessage = "Please select employee")]
        public int EmployeeId { get; set; }
       [Display(Name = "Member of Nafed Cashless Group Mediclaim policy")]
        public bool PolicyAvail { get; set; }
        [Display(Name = "Date of joining Insurance policy")]
        [Required(ErrorMessage = "Please enter date of joining insurance policy")]
        public System.DateTime ? PolicyJoinDate { get; set; }
       
        [Display(Name = "Family Sum Insured")]
        [Required(ErrorMessage = "Please enter amount")]
        public Nullable<decimal> FamilyAssuredSum { get; set; }
        [Display(Name = "Expiry date of current policy")]
        [Required(ErrorMessage = "Please enter expiry date of current policy")]
        public System.DateTime? PolicyExpDate { get; set; }
        [Display(Name = "Dependents covered under Nafed Cashless Group Mediclaim policy")]
        public bool DependentMedicalPolicy { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> Updatedon { get; set; }
        public List<InsuranceDependent> InsuranceDependenceList { get; set; }

        public string EmployeeName { get; set; }
        [Display(Name ="Designation")]
        public string DesignationName { get; set; }
    }

    public class InsuranceDependent
    {
        public int InsuranceDepId { get; set; }
        public int InsuranceId { get; set; }
        public int DependentId { get; set; }
        public System.DateTime? PolicyJoinDate { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> Updatedon { get; set; }

        public string DependentName { get; set; }
        public string Gender { get; set; }
        public string Relation { get; set; }
        public int Age { get; set; }
        public Nullable<System.DateTime> PolicyExpDate { get; set; }
        public string Reason { get; set; }
        public bool IsApplicable { get; set; }
       
    }
    }
