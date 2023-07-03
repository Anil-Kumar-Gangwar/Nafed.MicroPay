using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class APARForm
    {
        public int EmployeeID { get; set; }
        public int? ReportingTo { get; set; }
        public int? LoggedInEmpID { set; get; }
        public APARSkillSetFormHdr APARFormHdr { get; set; } = new APARSkillSetFormHdr();
        public List<Model.APARSkillSetFormDetail> Part1BehavioralList { get; set; }
        public List<Model.APARSkillSetFormDetail> Part1FunctionalList { get; set; }
        public List<Model.APARSkillSetFormDetail> Part2BehavioralList { get; set; }
        public List<Model.APARSkillSetFormDetail> Part2FunctionalList { get; set; }
        public SubmittedBy submittedBy { set; get; }
        public int DepartmentID { set; get; }
        public int DesignationID { set; get; }
        public ProcessWorkFlow _ProcessWorkFlow { get; set; } = new ProcessWorkFlow();
        public string ReportingYr { get; set; }
       
    }
}
