
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Common;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IPayrollApprovalSettingService
    {
        List<PayrollApprovalSetting> GetApprovalSetting();
        bool InsertPayrollApprovalSetting(List<PayrollApprovalSetting> pApprovalSetting);
        bool UpdatePayrollApprovalSetting(List<PayrollApprovalSetting> pApprovalSetting);
        List<PayrollApprovalRequest> GetSalaryApprovalRequests(WorkFlowProcess wrkProcessID, int reportingEmpID);

        List<PayrollApprovalRequest> GetDAArrearApprovalRequest(WorkFlowProcess wrkProcessID, int reportingEmpID);

        List<PayrollApprovalRequest> GetPayArrearApprovalRequest(WorkFlowProcess wrkProcessID, int reportingEmpID);
        bool SubmitApprovalRequest(PayrollApprovalRequest request, string filePath);
        bool SubmitDAApprovalRequest(PayrollApprovalRequest request);
        bool SubmitPayArrearApprovalRequest(PayrollApprovalRequest request);
        bool SendDAArrearApprovalRequest(ArrearApprovalRequest request, out bool requestExist);
        bool SendPayArrearApprovalRequest(ArrearApprovalRequest request, out bool requestExist);
        string GetArrearReport(ArrearReportFilter rFilter);

    }
}
