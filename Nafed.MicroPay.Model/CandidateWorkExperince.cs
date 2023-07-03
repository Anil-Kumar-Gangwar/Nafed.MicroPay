using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class CandidateWorkExperince
    {
        public int sno { set; get; }
        public int Counter { get; set; }
        [Required(ErrorMessage = "Please Enter Organization name")]
        public string OrganisationName { get; set; }
        [Required(ErrorMessage = "Please Enter Designation")]
        public string Designation { get; set; }
        [Display(Name = "From Date")]
        [Required(ErrorMessage = "Please Enter From Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "Please Enter To Date")]
        [DateCompare("FromDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? ToDate { get; set; }
        public string Specialization_Subjects { get; set; }
        public Nullable<int> GradeID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CraetedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        [Range(1.00, double.MaxValue, ErrorMessage = "Anuual Turnover of the employer should be more than 1 Cr.")]
        //[Required(ErrorMessage = "Please Enter Anuual Turnover of the employer")]
        public decimal AnnualTurnoveroftheEmployer { get; set; }

        [Required(ErrorMessage = "Please Enter Nature of duties in detail")]
        public string Natureofdutiesindetail { get; set; }
        public int RegistrationID { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        [Range(1, Int16.MaxValue, ErrorMessage = "Please select Organization Type")]
        public OrganizationType OrganizationType { get; set; }
        public int OrganisationType { get; set; }
        public string OrganisationTypeName { get; set; }
    }

    public enum OrganizationType
    {
        [Display(Name = "Private Sector")]
        Private = 1,
        [Display(Name = "PSUs")]
        PSUs = 2,
        [Display(Name = "Autonomous")]
        Autonomous = 3,
        [Display(Name = "Cooperatives")]
        Cooperatives = 4


    }
}
