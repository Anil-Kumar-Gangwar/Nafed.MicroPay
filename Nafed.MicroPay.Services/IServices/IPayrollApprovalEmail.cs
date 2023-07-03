using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IPayrollApprovalEmail
    {
        void SendApprovalEmail(PayrollApprovalRequest request);
    }
}
