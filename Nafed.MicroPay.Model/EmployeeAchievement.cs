using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class EmployeeAchievement
    {

        public string Employee { set; get; }

        public int EmpAchievementID { get; set; }
        public int EmployeeID { get; set; }
        
        [Required]
        [Display(Name = "Achievement Name")]
        public string AchievementName { get; set; }

        [Required]
        [Display(Name = "Date of Achievement")]
        public Nullable<System.DateTime> DateOfAchievement { get; set; }

        [Display(Name = "Achievement Remark")]
        [Required(ErrorMessage = "Please enter achievement detail.")]
        public string AchievementRemark { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }

        public EmpAchievementAndCertificationDocument EmpAchievementAndCertificationDocuments { get; set; }

        public ICollection<EmpAchievementAndCertificationDocument> documents { set; get; }

    }
}
