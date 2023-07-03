using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class APARSkillSetDetails : APARSkillSet
    {
        public int SkillSetDtlID { get; set; }
        //  public int SkillSetID { get; set; }
        public int SkillTypeID { get; set; }
        //public string SkillType { get; set; }
        public int SkillID { get; set; }
        public string Remark { get; set; }
        // public string Skill { get; set; }
        //  public bool IsDeleted { get; set; }
        //   public int CreatedBy { get; set; }
        //  public System.DateTime CreatedOn { get; set; }
        //  public Nullable<int> UpdatedBy { get; set; }
        // public Nullable<System.DateTime> UpdatedOn { get; set; }
    }
}
