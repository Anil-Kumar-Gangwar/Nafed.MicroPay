using Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IConveyanceBillService
    {
        List<ConveyanceBillFormHdr> GetEmployeeSelfConveyanceList(int userEmpID, string empCode, string empName);
        ConveyanceRulesAttributes GetConveyanceFormRulesAttributes(int EmployeeID, int conveyanceDetailID);
        ConveyanceBillDetails GetConveyanceBillDetail(int employeeID, int conveyanceDetailID);
        List<Model.ConveyanceBillDescription> GetConveyanceBillDescription(int employeeId, int conveyanceBillDetailID);
        bool ConveyanceDataExists(int empID, int conveyanceBillDetailID);
        bool InsertConveyanceBillData(ConveyanceBillForm conveyanceForm, out int conveyanceDetailId);
        bool UpdateConveyanceBillData(ConveyanceBillForm conveyanceForm);
        IEnumerable<ConveyanceBillFormHdr> GetConveyanceFormHdr(ConveyanceBillApprovalFilter filters);
        List<SelectListModel> GetEmployeeFilter(int employeeID);
        List<ConveyanceBillHistoryModel> GetConveyanceBillDetails(AppraisalFormApprovalFilter filters, string procName);
        List<ConveyanceBillHistoryModel> GetConveyanceEmployeeHistory(string fromDate, string toDate, int? employeeId);
    }
}
