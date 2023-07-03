using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class APARSkills
    {
        public UserAccessRight UserRights { get; set; }
        public List<APARSkillSet> APARSkillSetList { get; set; }
        public APARSkillSet APARSkillSet { get; set; } 
        public APARSkillSetDetails APARSkillSetDetails { get; set; } 
        public int [] CheckBoxListBehavioral { get; set; } 
        public int[] CheckBoxListFunctional { get; set; } 
        public List<SelectListModel> DepartmentList { set; get; }
        public List<SelectListModel> DesignationList { set; get; }


    }
}
