using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class APARSkillSetFormDetail
    {
        public int FormDetailID { get; set; }
        public int APARHdrID { get; set; }
        public int PartID { get; set; }
        public int SkillTypeID { get; set; }
        public int SkillID { get; set; }
        public string Skill { get; set; }
        public Nullable<int> Grading { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public EnumAssessment enumAssessment { get; set; }
        public string Remark { get; set; }
        public int SkillSetID { get; set; }
        public string SkillRemark { get; set; }
        public EnumAssessment enumAssessmentReporting { get; set; }
        public string ReportingRemarks { get; set; }

    }

    public enum EnumAssessment
    {        
        [Display(Name ="Needs Development")]
        ND = 1,
        [Display(Name = "Approaching Competence")]
        AC = 2,
        [Display(Name = "Competence")]
        C = 3,
        [Display(Name = "Highly Competent")]
        HC = 4,
        [Display(Name = "Exceptional")]
        E = 5
    }
}
