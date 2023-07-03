using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class APARSkillSetViewModel
    {
        public UserAccessRight UserRights { get; set; }
       public List<APARSkillSet> APARSkillSetList { get; set; }
        public APARSkillSet APARSkillSet { get; set; }
        public APARSkillSetDetails APARSkillSetDetails { get; set; }
        public List<APARSkillSetDetails> APARSkillSetDtlList { get; set; }
        public CheckBoxListViewModel CheckBoxListBehavioral { get; set; } = new CheckBoxListViewModel();
        public CheckBoxListViewModel CheckBoxListFunctional { get; set; } = new CheckBoxListViewModel();
        public List<SelectListModel> DepartmentList { set; get; }
        public List<SelectListModel> DesignationList { set; get; }
    }
}