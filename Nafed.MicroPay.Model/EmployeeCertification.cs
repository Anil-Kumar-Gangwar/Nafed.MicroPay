using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Nafed.MicroPay.Model
{
    public class EmployeeCertification
    {
        public string Employee { set; get; }
        public int EmpCertificateID { get; set; }
        public int EmployeeID { get; set; }

        [Display(Name ="Certification Name")]
        [Required(ErrorMessage = "Please enter certification name.")]
        public string CertificationName { get; set; }


        [Display(Name = "Certification Remark")]
       
        public string CertificationRemark { get; set; }

        [Required]
        [Display(Name = "Date of Issue")]
        public Nullable<System.DateTime> DateOfIssue { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }

        public EmpAchievementAndCertificationDocument EmpAchievementAndCertificationDocuments { get; set; }
        public ICollection<EmpAchievementAndCertificationDocument> documents { set; get; }
    }
}
