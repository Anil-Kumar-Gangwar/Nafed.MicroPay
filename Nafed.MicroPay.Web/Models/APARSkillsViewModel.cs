using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class APARSkillsViewModel
    {
        public IEnumerable<APARSkillSetFormHdr> APARSkillsList { get; set; }
        public UserAccessRight userRights { get; set; }
        public EmployeeProcessApproval approvalSetting { set; get; } = new EmployeeProcessApproval();

        public int EmployeeID { get; set; }
        public int? ReportingTo { get; set; }
        public int? LoggedInEmpID { set; get; }
        public APARSkillSetFormHdr APARFormHdr { get; set; }/* = new APARSkillSetFormHdr();*/
        public List<APARSkillSetFormDetail> Part1BehavioralList { get; set; }
        public List<APARSkillSetFormDetail> Part1FunctionalList { get; set; }
        public List<APARSkillSetFormDetail> Part2BehavioralList { get; set; }
        public List<APARSkillSetFormDetail> Part2FunctionalList { get; set; }
        public SubmittedBy submittedBy { set; get; }
        public int DepartmentID { set; get; }
        public int DesignationID { set; get; }
        public ProcessWorkFlow _ProcessWorkFlow { get; set; } = new ProcessWorkFlow();
        public string ReportingYr { get; set; }
        public CheckBoxListViewModel CheckBoxListBehavioral { get; set; } = new CheckBoxListViewModel();
        public CheckBoxListViewModel CheckBoxListFunctional { get; set; } = new CheckBoxListViewModel();

        public int[] CheckBoxBehavioral { get; set; }
        public int[] CheckBoxFunctional { get; set; }

        public int? APARSkillStatus { get; set; }
        public int? APARStatus { get; set; }

        public FormRulesAttributes frmAttributes { get; set; }

        public bool isAdmin { get; set; } = false;

    }
}