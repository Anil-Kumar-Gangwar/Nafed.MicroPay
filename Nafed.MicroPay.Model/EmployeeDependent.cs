using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class EmployeeDependent
    {
        public int EmpDependentID { get; set; }
        public string EmpCode { get; set; }
        [DisplayName("Employee :")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select employee")]
        public int EmployeeId { get; set; }
        //[DisplayName("Dependent Code :")]
        //[Required(ErrorMessage = "Please enter dependent code")]
        public int DependentCode { get; set; }
        [DisplayName("Dependent Name :")]
        [Required(ErrorMessage = "Please enter dependent name")]
        public string DependentName { get; set; }
        [DisplayName("Gender :")]
        [Range(1,3,ErrorMessage = "Please select Gender")]
        public Nullable<int> GenderID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date Of Birth :")]
        public Nullable<System.DateTime> DOB { get; set; }

        [DisplayName("Relation :")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select relation")]
        public Nullable<int> RelationID { get; set; }
        [DisplayName("Handicapped :")]
        public bool Handicapped { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        public string Gender { get; set; }
        public string Relation { get; set; }
        [Display(Name ="PF Nominee")]
        public bool PFNominee { get; set; }
        public string Employee { get; set; }


        [Display(Name = "PF Distribution")]
        public Nullable<decimal> PFDistribution { get; set; }
        [Display(Name = "EPS Nominee")]
        public bool EPSNominee { get; set; }
        public string Address { get; set; }

        public string Branch { get; set; }
    }
}
