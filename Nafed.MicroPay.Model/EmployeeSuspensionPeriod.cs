using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class EmployeeSuspensionPeriod
    {
        public int SuspensionPeriodID { get; set; }
        public int EmployeeID { get; set; }
        public System.DateTime PeriodFrom { get; set; }
        public System.DateTime PeriodTo { get; set; }

        public int NoOfDays
        {
            get
            {
                return (PeriodTo.Date - PeriodFrom.Date).Days+1;
            }
        }
        public double BasicSalaryPercentage { get; set; }

        public int CreatedBy { set; get; }
        public System.DateTime CreatedOn { set; get; }
    }
}
