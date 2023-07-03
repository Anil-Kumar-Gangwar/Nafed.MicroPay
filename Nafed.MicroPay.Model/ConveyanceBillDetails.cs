using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ConveyanceBillDetails
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string Branch { get; set; }
        public string ReportingTo { get; set; }
        public string ReportingDesignation { get; set; }
        public string ReviewerTo { get; set; }
        public string ReviewerDesignation { get; set; }
        public string AcceptanceAuth { get; set; }
        public string AcceptanceDesignation { get; set; }
        public string ReportingYear { get; set; }
        public int ConveyanceBillDetailID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int DesignationID { get; set; }
        public int DepartmentID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Dated { get; set; }
        public string Section { get; set; }
        public Nullable<decimal> TotalAmountClaimed { get; set; }
        public int FormState { get; set; }
        public Nullable<System.DateTime> RTSenddate { get; set; }
        public Nullable<System.DateTime> RVSenddate { get; set; }
        public Nullable<System.DateTime> AASenddate { get; set; }
        public Nullable<System.DateTime> EmpSenddate { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string CombineDetails { get; set; }
        public string ReportingToCombine { get; set; }
        public bool VehicleProv { get; set; }
        public VehicleProvided vehicleProvided { get; set; }
        public AcceptedOrRejected ReportingAcceptedRejected { get; set; }
        public AcceptedOrRejected ReviewingAcceptedRejected { get; set; }
        public bool IsReportingRejected { get; set; }
        public bool IsReviewingRejected { get; set; }
        public string ReportingRemarks { get; set; }
        public string ReviewingRemarks { get; set; }
    }

    public enum VehicleProvided
    {
        Provided = 1,
        NotProvided = 2
    }

    public enum AcceptedOrRejected
    {
        [Display(Name = "Accepted")]
        Accepted = 1,
        [Display(Name = "Rejected")]
        Rejected = 2
    }
}
