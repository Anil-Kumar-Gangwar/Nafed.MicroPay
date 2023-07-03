using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class CandidateEducationSummary
    {
        public int sno { set; get; }
        public int Counter { get; set; }
        public int RegistrationID { get; set; }

        public int FromMonth { get; set; }
        public int FromYear { get; set; }
        public int ToMonth { get; set; }
        public int ToYear { get; set; }

        public List<SelectListModel> ddlFromYear = new List<SelectListModel>();
        public List<SelectListModel> ddlFromMonth = new List<SelectListModel>();

        public List<SelectListModel> ddlToYear = new List<SelectListModel>();
        public List<SelectListModel> ddlToMonth = new List<SelectListModel>();

        public string FromMonthName { get; set; }
        public string FromYearName { get; set; }
        public string ToMonthName { get; set; }
        public string ToYearName { get; set; }


        [Required(ErrorMessage = "Please Enter Detail.")]
        public string DegreeDiploma { get; set; }

        [Required(ErrorMessage = "Please Enter University/Institute.")]
        public string University_Institute { get; set; }
        [Required(ErrorMessage = "Please Enter Specialization/Subjects.")]
        public string Specialization_Subjects { get; set; }

        public bool Percentage_GradeSystem { get; set; }
        public Nullable<decimal> Percentage { get; set; }
        public string Grade { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }


        [Range(1, Int16.MaxValue, ErrorMessage = "Please select Qualification")]
        public OtherQualificationType OtherQualificationType { get; set; }
        public int JSelectedQualificationID { get; set; }
        public string QualificationName { set; get; }

        public string Per_GradeTextBoxCss { get; set; } = "hide";

        [Display(Name = "From Month")]
        [Range(1, 12, ErrorMessage = "Please select From Month")]
        public EnumMonth enumFromMonth { get; set; }
        [Display(Name = "From Year")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select From Year")]
        public EnumYear enumFromYear { get; set; }
        [Display(Name = "From Month")]
        [Range(1, 12, ErrorMessage = "Please select From Month")]
        public EnumMonth enumToMonth { get; set; }
        [Display(Name = "To Month")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select To Year")]
        public EnumYear enumToYear { get; set; }

    }
    public enum OtherQualificationType
    {
        [Display(Name = "Graduation")]
        Graduation = 30,
        [Display(Name = "Post Graduation")]
        PostGraduation = 40,

        [Display(Name = "Professional")]
        Professional = 50,
        [Display(Name = "Other")]
        Other = 60
    }
    public enum EnumMonth
    {
        [Display(Name = "01")]
        one = 1,
        [Display(Name = "02")]
        two = 2,
        [Display(Name = "03")]
        three = 3,
        [Display(Name = "04")]
        four = 4,
        [Display(Name = "05")]
        five = 5,
        [Display(Name = "06")]
        six = 6,
        [Display(Name = "07")]
        seven = 7,
        [Display(Name = "08")]
        eight = 8,
        [Display(Name = "09")]
        nine = 9,
        [Display(Name = "10")]
        ten = 10,
        [Display(Name = "11")]
        eleven = 11,
        [Display(Name = "12")]
        tewlve = 12

    }
    public enum EnumYear
    {
        [Display(Name = "2021")]
        twotwentyone = 2021,
        [Display(Name = "2020")]
        twotwenty = 2020,
        [Display(Name = "2019")]
        twoninteen = 2019,
        [Display(Name = "2018")]
        twoeighteen = 2018,
        [Display(Name = "2017")]
        twoseveteen = 2017,
        [Display(Name = "2016")]
        twosixteen = 2016,
        [Display(Name = "2015")]
        twofifteen = 2015,
        [Display(Name = "2014")]
        twofourteen = 2014,
        [Display(Name = "2013")]
        twothirteen = 2013,
        [Display(Name = "2012")]
        twotwelve = 2012,
        [Display(Name = "2011")]
        twoeleven = 2011,
        [Display(Name = "2010")]
        twoten = 2010,
        [Display(Name = "2009")]
        twonine = 2009,
        [Display(Name = "2008")]
        twoeight = 2008,
        [Display(Name = "2007")]
        twoseven = 2007,
        [Display(Name = "2006")]
        twosix = 2006,
        [Display(Name = "2005")]
        twofive = 2005,
        [Display(Name = "2004")]
        twofour = 2004,
        [Display(Name = "2003")]
        twothree = 2003,
        [Display(Name = "2002")]
        twotwo = 2002,
        [Display(Name = "2001")]
        twoone = 2001,
        [Display(Name = "2000")]
        twozero = 2000

    }



}
