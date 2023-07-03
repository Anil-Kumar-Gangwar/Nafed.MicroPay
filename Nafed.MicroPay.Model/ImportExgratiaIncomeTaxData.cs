using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class ImportExgratiaIncomeTaxData
    {
        public string EmpCode { get; set; }
        public string PFNO { get; set; }
        public string Name { get; set; }
        public string DOJ { get; set; }
        public string Designation { get; set; }
        public string FinancialYear { get; set; }
        public string SalaryforExgratia { get; set; }
        public string AmountofExgratia { get; set; }
        public string NetAmount { get; set; }
        public string IncomeTax { get; set; }
        public string error { set; get; }
        public string warning { set; get; }
        public bool isDuplicatedRow { set; get; }
    }

    public class ExgratiaIncomeTaxColValues
    {
        public List<string> empCode { set; get; }
    }
}
