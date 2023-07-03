using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class Dashboard :BaseEntity
    {
        public Nullable<DateTime> DOLeaveOrg { set; get; }
        public int ? ReportingTo { set; get; }
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string BranchName { get; set; }
        public string DesignationName { get; set; }
        public string RM { get; set; }
        public Nullable<int> EXPERIENCE { get; set; }
        public int LeaveAppliedForMonth { get; set; }
        public double LeaveBalance { get; set; }
        public IList<EmployeeDobDoj> EmployeeDobDoj { get; set; }
        public double CLBalance { get; set; }
        public double ELBalance { get; set; }

        public double Medical_Extra { get; set; }
        
        public double CLAvailed { get; set; }
        public double ELAvailed { get; set; }

        public List<DashboardDocuments> dashbrdDocList { get; set; }

        public string EmpProfilePhotoUNCPath { set; get; }

        public int EmployeeTypeId { get; set; }
    }
    public class EmployeeDobDoj {

        public string OfficialEmail { get; set; }
        public string MobileNo { get; set; }
        public string Name { get; set; }
        public string DOB { get; set; }
        public string EventType { get; set; }
        public string EmpProfilePhotoUNCPath { set; get; }
        public string DepartmentName { set; get; }
        public string AGE_NOW { set; get; }
        public string AGE_ONE_WEEK_FROM_NOW { set; get; }

        public Nullable<System.DateTime> DOBDateTime { get; set; }
        public Nullable<System.DateTime> DOJDateTime { get; set; }
    }

}
