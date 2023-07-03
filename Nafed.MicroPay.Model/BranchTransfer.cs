using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class BranchTransfer
    {
        public int Id { get; set; }
        public int employeeID { set; get; }
        public string employeeCode { set; get; }

        public string employeeName { set; get; }
        public string cadre { set; get; }

        public int CurrentCadreID { set; get; }
        public string designation { set; get; }

        public decimal? basicSalary { set; get; }
        public int? Sen_Code { set; get; }

        public List<BranchManagerDetails> branchTransferList { set; get; }
    }
}
