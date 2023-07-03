using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class Form12BBInfo
    {
        public  short ? view { set; get; }
        public int EmpFormID { get; set; }

        public string EmployeeName { set; get; }

        public byte FormState { get; set; }

        public int EmployeeID { set; get; }

        public string EmployeeCode { set; get; }
        public string Designation { set; get; }

        public string EmployeeAddress { set; get; }

        public string FYear { set; get; } 
       
        public string AYear
        {
            get
            {
                if (FYear== null)
                {
                    return "";
                }
                var fYearArr = FYear.Split(new char[] {'-' });
                var aYear = $"{(int.Parse(fYearArr[0])+1).ToString()}-{(int.Parse(fYearArr[1]) + 1).ToString()}";
                return aYear;
            }
        }       
        public string PANNo { set; get; }
        
        public string FullName { set; get; } 

        public string FatherName { set; get; }

        [StringLength(50)]

        public string LandLordName { set; get; }

        [StringLength(50)]
        public string LandLordAddress { set; get; }
        public string LandLordPANNo { set; get; }

        public string LenderName { set; get; }
        public string LenderAddress { set; get; }

        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"[A-Z]{5}\d{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN No.")]
        public string Fin_Institute_PANNo { set; get; }

        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"[A-Z]{5}\d{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN No.")]
        public string Employer_PANNo { set; get; }


        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"[A-Z]{5}\d{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN No.")]
        public string Other_PANNo { set; get; }

        [Required(ErrorMessage ="Please enter rent paid amount.")]
        public decimal ? RentPaidToLandLord { set; get; }
        
        public decimal? InterestPaidToLender { set; get; }
        
        public bool HasHRADocument { set; get; }

        public bool HasLTCDocument { set; get; }

        public bool HasDeductionOfInterestDoc { set; get; }

        public bool HasChapter_VI_I_Document { set; get; }
        public TaxDeductions taxDeductions { set; get; } = new TaxDeductions();

        public int CreatedBy { set; get; }
        public DateTime CreatedOn { set; get; }

        public int ? UpdatedBy { set; get; }

        public DateTime ? UpdatedOn { set; get; }
        public DateTime? SubmittedOn { set; get; }
        public int UploadDocument { set; get; }
    }

}
