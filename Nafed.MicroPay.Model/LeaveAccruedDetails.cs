using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class LeaveAccruedDetails
    {
        public Model.UserAccessRight userRights { get; set; }
        public int BranchId { get; set; }
        public int EmpLeaveBalID { get; set; }
        public string Name { get; set; }
        public string EmployeeCode { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string BranchName { get; set; }
        public double AccruedLeave { get; set; }
        public string LeaveType { get; set; }
        public int Designation { get; set; }
        public string DesignationName { get; set; }
        public int leavecategoryId { get; set; }
        public int id { set; get; }
        public string value { set; get; }
        public string Pr_Loc_DOJ { set; get; }
        public double EL { get; set; }
        public double EL_OpBal { get; set; }
        public int Count { get; set; }
        public int prevYear { get; set; }
        public double Medical { get; set; }
        public double Medical_Extra { get; set; }
        public double Medical_OpBal { get; set; }
        public string LeaveTypeName { get; set; }
        public double CL { get; set; }
        public int  Day { get; set; }
        public DateTime fulldate { get; set; }
        public double BalanceEL { get; set;}
        public DateTime MedicalUpdatedate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
}
}
