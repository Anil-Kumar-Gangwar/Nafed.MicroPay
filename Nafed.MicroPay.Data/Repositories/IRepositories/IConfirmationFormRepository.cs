using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
  public interface IConfirmationFormRepository
    {
        List<ConfirmationFormAHeader> EmployeeSelfConfirmationList(int userEmpID);
        Nullable<int> ConfirmationStatusUpdate(int empID, int proceddID, int formHdrID);
        GetConfirmationDtlForOrdrRpt_Result GetConfirmationDtlForOrdrRpt(int empID,int hdrID, int formHdrID);
    }
}
