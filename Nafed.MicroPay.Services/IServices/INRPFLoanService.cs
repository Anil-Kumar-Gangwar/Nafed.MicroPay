using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;


namespace Nafed.MicroPay.Services.IServices
{
    public interface INRPFLoanService
    {
        List<Model.NonRefundablePFLoan> checkexistdata(int NRPFLoanID);
        List<Model.NonRefundablePFLoan> GetnonrefPFloanList(int EmployeeID);
        Model.NonRefundablePFLoan GetNRPFLoanDetails(int ID, int EmpPFID, int statusID, int employeeID);
        bool InsertNRPFLoanDetails(NonRefundablePFLoan createNRPFLoan, ProcessWorkFlow workFlow);
        bool UpdateNRPFLOanDetails(NonRefundablePFLoan createNRPFLoan);
        bool UpdateNRPFLoanStatus(NonRefundablePFLoan createNRPFLoan, ProcessWorkFlow workFlow);
        IEnumerable<NonRefundablePFLoan> GetNRPFLoanHdr(NRPFLoanFormApprovalFilter filters);
        bool InsertNRPFLoanDetailsHDR(int employeeID, NonRefundablePFLoan createNRPFLoan);
    }
}
