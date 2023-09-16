using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class LeaveEncashment
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Branch { get; set; }
        public string DesignationName { set; get; }
        public decimal ELBalAsofnow { get; set; }
        public decimal ELBal { get; set; }
        public decimal NoofDays { get; set; }
        public double BasicForMonth { get; set; }
        public double Basic { get; set; }
        public double DA { get; set; }
        public decimal DAPercentage { get; set; }
        public decimal GrossAmount
        {
            get;set;

        }
        public decimal? TDS { get; set; }
        public decimal NetAmount
        {
            get;set;
        }
        public string OrderNo { get; set; }
        public Nullable<DateTime> OrderDate { get; set; }
        public int TDSYear { get; set; }
        public double DALatest { get; set; }
        public decimal DAPercentageLatest { get; set; }
        public decimal GrossAmountLatest
        {
            get { return Convert.ToDecimal(Basic + DALatest); }
            set { }

        }
        public decimal DADifference
        {
            get { return Convert.ToDecimal(DALatest - DA); }
            set { }

        }
        public string RecordType { get; set; }

    }
}
