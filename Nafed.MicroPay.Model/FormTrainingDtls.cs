using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class FormTrainingDtls
    {
        public int sno { set; get; }
        public Nullable<int> TrainingID { get; set; }
        public string Remark { get; set; }
        public int FormDetailID { get; set; }
        public int FormGroupID { get; set; }
        public string ReportingYr { get; set; }
        public FormPart4Training FormTraining { set; get; }

        public string Name { get; set; }
        public string BranchName { get; set; }
        public string DesignationName { get; set; }
        public string ReportingTo { get; set; }
        public int EmployeeID { get; set; }
    }
    public enum FormPart4Training
    {
        Behavioural = 1,
        Technical = 2,
        Functional = 3
    }
}
