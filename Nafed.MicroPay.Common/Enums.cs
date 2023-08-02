using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Common
{
    public enum FileExtension
    {
        xlsx = 1,
        xls = 2,
        pdf = 3,
        xml = 4,
        jpg = 5,
        png = 6,
        jpeg = 7,
        bmp = 8
    }
    public enum UserType
    {
        SuperUser = 1,
        Admin = 2,
        PersonnelHead = 3,
        AccountHead = 4,
        Employee = 5
    }

    public enum DocumentType
    {
        PanCard = 1,
        AadhaarCard = 2,
        ProfileImage = 3,
        PassportNo = 4,
        BankDetails = 5
    }
    public enum DashboardEvent
    {
        Birthday = 1,
        WorkAnniversary = 2
    }

    public enum EmpAttendanceStatus
    {
        Pending = 1,
        RejectedByReportingOfficer = 2,
        InProcess = 3,
        RejectedByReviwerOfficer = 4,
        AcceptedByReviewerOfficer = 5,
        Withdrawl = 6,
        RejectedByAcceptanceAuthority = 7,
        Approved = 8
    }

    public enum EmpLeaveStatus
    {
        Pending = 1,
        RejectedByReportingOfficer = 2,
        InProcess = 3,
        RejectedByReviwerOfficer = 4,
        AcceptedByReviewerOfficer = 5,
        Withdrawl = 6,
        RejectedByAcceptanceAuthority = 7,
        Approved = 8
    }

    public enum PlaceOfAttendance
    {
        
        Office = 1,
        ClientSite = 2,
        WorkFromHome = 3,
        Tour = 4
    }
    public enum AppraisalForm
    {
        FormA = 1,
        FormB = 2,
        FormC = 3,
        FormD = 4,
        FormE = 5,
        FormF = 6,
        FormG = 7,
        FormH = 8
    }
    public enum WorkFlowProcess
    {
        LeaveRequest = 1,
        AttendanceRequest = 2,
        LeaveApproval = 3,
        AttendanceApproval = 4,
        Appraisal = 5,
        AppointmentConfirmation = 6,
        PromotionConfirmation = 7,
        FileTracking = 8,
        Competency = 9,
        LTC = 10,
        SalaryGenerate = 11,
        DAArrearGenerate = 12,
        PayArrearGenerate = 13,
        LoanApplication = 14,
        ConveyanceBill = 15,
        EPFNomination = 16,
        Form11 = 17,
        NonRefundablePFLoan = 18,
        HelpDesk = 19,
        Separation = 20
    }

    public enum ApprovalRequiredLevel
    {
        One = 1,
        Two = 2,
        Three = 3
    }

    public enum ApproverType
    {
        ReportingTo = 1,
        ReviewerTo = 2,
        AcceptanceAuthority = 3
    }

    public enum AdvanceSearchFilterFields
    {
        EmployeeName = 1,
        EmployeeCode = 2,
        Branch = 3,
        Desigantion = 4,
        Divison = 5,
        Section = 6,
        Cadre = 7
    }

    public enum WorkingArea
    {
        SalaryApproval=1
    }
}
