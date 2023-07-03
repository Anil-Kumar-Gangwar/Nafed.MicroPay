using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ProjectedIncrement
    {
        public int? EmployeeID { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public Nullable<decimal> CurrentBasic { get; set; }
        public Nullable<decimal> OldBasic { get; set; }
        public Nullable<decimal> LastIncrement { get; set; }
        public int BranchID { get; set; }

        public bool RowChanged { get; set; }

        public bool? ProceedFurther { set; get; }

        public string AlertMessage { set; get; }

        public Nullable<int> IncrementMonth { get; set; }

        public string Designation { get; set; }

    }

    public class StopIncrement
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public bool ValidateIncrement { get; set; }
        public string Reason { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StopIncrementEffectiveDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StopIncrementEffectiveToDate { get; set; }
    }

}
