using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class SkillType
    {
        public int SkillTypeID { get; set; }
        [Display(Name = "Skill Type :")]
        [Required(ErrorMessage = "Skill Type is required.")]
        public string SkillType1 { get; set; }
        [Display(Name = "Delete :")]
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    }
}
