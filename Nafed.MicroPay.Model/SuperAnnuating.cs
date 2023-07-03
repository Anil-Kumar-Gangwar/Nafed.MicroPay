using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
  public class SuperAnnuating
    {
        public bool IsChecked { get; set; }
        public int SeprationId { get; set; }
        public int EmployeeId { get; set; }
        public int SeprationType { get; set; }
        public System.DateTime DateOfAction { get; set; }
        public Nullable<int> NoticePeriod { get; set; }
        public Nullable<int> Reason { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }        
        public string EmployeeCode { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> Pr_Loc_DOJ { get; set; }
        public Nullable<System.DateTime> DOSupAnnuating { get; set; }
        public int StatusId { get; set; }
        public string DepartmentName { get; set; }
        public string BranchName { get; set; }
        public string CircularDocument { get; set; }
        public string FileNo { get; set; }
        public string ReferenceNo { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public string OtherReason { get; set; }
        public Nullable<System.DateTime> ClearanceDateUpto { get; set; }

    }

    public enum SeprationStatus
    {
        Start=1,
        Upload=2,
        Clearance=3,
        ClearanceApproved=4,
        ClearanceRejected=5,
        CircularUplaoded=6,
        DivisionalApproval=7,
        DivisionalApproved = 8,
        DivisionalRejected = 9,
        Finished =10

    }
}
