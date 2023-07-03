using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ImportProcessApprovers
    {
        public string ECode { set; get; }
        public string ReportingTo { set; get; }

        public string ReviewerTo { set; get; }

        public string AcceptanceTo { set; get; }

        public string EmployeeId { get; set; }
        public string ReportingToID { get; set; }

        public string ReviewerToID { get; set; }

        public string AcceptanceToID { get; set; }

        public string error { set; get; }
        public string warning { set; get; }
        public bool isDuplicatedRow { set; get; }
    }


    public class ApproverColValues
    {
        public List<string> ECode { set; get; }
        public List<string> ReportingTo { set; get; }

        public List<string> ReviewerTo { set; get; }

        public List<string> AcceptanceTo { set; get; }
    }
}
