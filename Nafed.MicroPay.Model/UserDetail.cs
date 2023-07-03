using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class UserDetail:BaseEntity
    {

        public DateTime? DoLeaveOrg { set; get; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int DepartmentID { get; set; }
        public int UserTypeID { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }

        public string FullName { get; set; }

        public Nullable<int> EmployeeID { get; set; }
        public string Designation { get; set; }
        public string Location { get; set; }
        public string ReportingOfficer { get; set; }


        public string PresentAddress { get; set; }


        public string DepartmentName { get; set; }
        public List<UserAccessRight> userMenuRights { set; get; }

        public int BranchID { get; set; }
        public int GenderID { get; set; }

        public int? ReportingTo { set; get; }

        public int? ReviwerTo { set; get; }

        public int? AppraisalFormID { get; set; }

        public int? DesignationID { get; set; }
        public int step { get; set; }
        public int otpType { set; get; } = 1;

        public bool IsMaintenance { get; set; }
        public Nullable<System.DateTime> MaintenanceDateTime { get; set; }

        public int OTPCode { get; set; }
        public DateTime OTPtimeStamp { get; set; }

        public string EmpProfilePhotoUNCPath { get; set; }
        public string EmployeeCode { get; set; }

        public int EmployeeTypeId { get; set; }
        public byte? WrongAttemp { get; set; }
        public string ErrorMessage { get; set; }
    }
}
