using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class AppliedCandidateDetail
    {
        public string ApplicantName { get; set; }
        public string Gender { get; set; }
        public string FathersName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string Zoneapplied { get; set; }
        public System.DateTime DOB { get; set; }
        public string Age { get; set; }
        public string Qualification { get; set; }
        public string RelevantExperience { get; set; }
        public string TotalExperience { get; set; }
        public string GovtExperience { get; set; }
        public string GovtReleExp { get; set; }
        public decimal AnnualGrossSalary { get; set; }
        public string Address { get; set; }
        public System.DateTime DateofRegistration { get; set; }
        public System.DateTime Dateofsubmission { get; set; }
    }
}
