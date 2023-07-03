using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
  public class StaffTransfer
    {
        public int employeeID { set; get; }
        public string employeeCode { set; get; }
        public string employeeName { set; get; }

        public string branchName { set; get; }

        public int branchCode { set; get; }

        public List<Transfer> transferList { set; get; }
    }
}
