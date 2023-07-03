using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ImportBounsData
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string BranchName { get; set; }

        public string DesignationName { set; get; }
        public string Month { get; set; }
        public string Year { get; set; }

        public string Bonus { set; get; }

        public string IncomeTaxDeduction { set; get; }

        public string NetPay
        {
            set { }
            get
            {
                return (Convert.ToDecimal(Bonus) - Convert.ToDecimal(IncomeTaxDeduction)).ToString();
            }
        }

        public string EmployeeId { get; set; }
        public string BranchID { get; set; }

        public string error { set; get; }
        public string warning { set; get; }
        public bool isDuplicatedRow { set; get; }

    }

    public class BonusColValues
    {
        public List<string> empCode { set; get; }
        public List<string> BranchName { set; get; }

    }
}
