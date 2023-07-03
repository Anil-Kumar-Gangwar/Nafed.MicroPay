using System.Collections.Generic;
using Nafed.MicroPay.Services.IServices;
namespace Nafed.MicroPay.Services.IServices
{
    public interface IAppraisalFormDueDateService
    {
        List<Model.AppraisalFormDueDate> GetAppraisalFormDueDate(string reportingYr);
        bool UpdateAppraisalForm(Model.AppraisalFormDueDate apFormDueDate);
  

    }
}
