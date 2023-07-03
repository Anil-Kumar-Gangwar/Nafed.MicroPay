using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class UnAssignedPFRecord
    {
        public int SNo { set; get; }
        public int EmployeeId { set; get; }

        public string Employeecode { set; get; }

        public string Employeename { set; get; }

        public string Branchname { set; get; }

        public string DepartmentName { set; get; }

        public string DesignationName { set; get; }

        public Nullable<int> PFNO { set; get; }

        public string AadhaarNo { set; get; }
        public string IFSCCode { set; get; }
        public string EPFOMemberID { set; get; }
        public string PensionUAN { set; get; }
        public string BankAcNo { set; get; }
        public string BankCode { set; get; }
        public string PANNo { set; get; }

    }
}
