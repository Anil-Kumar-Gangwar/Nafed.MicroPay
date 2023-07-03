using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class TrainingParticipant
    {
        public int TrainingParticipantID { get; set; }
        public int TrainingID { get; set; }
        public int EmployeeID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }

        public string EmailID { set; get; }
        public bool TrainingAttended { get; set; }
        public bool FeedbackFormStatus { get; set; }

        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeDepartment { get; set; }
        public string EmployeeBranch { get; set; }
        public string TrainingName { get; set; }
        public bool Nomination { get; set; }
        public EmployeeProcessApproval EmpProceeApproval { set; get; } = new EmployeeProcessApproval();
        public Nullable<byte> NominationAccepted { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string StateName { get; set; }
        public string Address { get; set; }  
        public string City { get; set; }
        public string PinCode { get; set; }


    }

    public class TrainingParticipantsDetail
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeDepartment { get; set; }
        public string EmployeeBranch { get; set; }
        public bool IsChecked { get; set; }
        public string DepartmentName { get; set; }
    }
}
