using System;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class CandidateGovtWorkExperience
    {
        public int sno { set; get; }
        public int Counter { get; set; }

        [Required(ErrorMessage = "Please Enter Organization name")]
        public string OrganisationName { get; set; }
        [Required(ErrorMessage = "Please Enter Post")]
        public string Postheldonregularbasis { get; set; }

        public int RegistrationID { get; set; }

        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Please Enter From Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FromDate { get; set; }
        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Please Enter To Date")]
        [DateCompare("FromDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<decimal> PayinPB { get; set; }

        public Nullable<decimal> GP { get; set; }

        [Range(1.00, double.MaxValue, ErrorMessage = "Please Enter Basic Pay & Level")]
        public decimal BasicPay { get; set; }
        public string LevelAsPerCPC { get; set; }

        [Required(ErrorMessage = "Please Enter To Date")]
        public int Natureofappointment { get; set; }
        public string NatureOfAppointmentName { set; get; }
        public string Natureofdutiesindetail { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        [Range(1, Int64.MaxValue, ErrorMessage = "Please select Nature Of Appointment")]
        public EnumNatureOfAppointment enumNatureOfAppointment { set; get; }
    }
    public enum EnumNatureOfAppointment
    {
        Regular = 1,
        Adhoc = 2,
        Deputation = 3
    }
}
