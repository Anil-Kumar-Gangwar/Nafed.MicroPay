using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class Skill
    {
        public int SkillId { get; set; }
        [Display(Name = "Skill Type :")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Skill Type")]
        public Nullable<int> SkillTypeID { get; set; }
        [Display(Name = "Skill:")]
        [Required(ErrorMessage = "Skill Is Required.")]
        public string Skill1 { get; set; }
        [Display(Name = "Deleted:")]
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }

        public string SkillType { get; set; }
    }
}
