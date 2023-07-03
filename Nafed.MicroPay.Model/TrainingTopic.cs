using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class TrainingTopic
    {
        public int TrainingTopicID { get; set; }
        public int TrainingID { get; set; }
        public int SkillTypeID { get; set; }
        public int SkillID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }

        public string Skill1 { get; set; }
    }
}
