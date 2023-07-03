using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;

namespace Nafed.MicroPay.Data.Repositories
{
  public class ConfirmationFormRepository: BaseRepository, IConfirmationFormRepository
    {
        public List<ConfirmationFormAHeader> EmployeeSelfConfirmationList(int userEmpID)
        {
            return db.ConfirmationFormAHeaders.AsNoTracking().Where(x => x.EmployeeId == userEmpID
             && x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.EmployeeID == userEmpID && y.ProcessID == 6 && y.ToDate == null)
           ).ToList();
        }
        public Nullable<int> ConfirmationStatusUpdate(int empID,int proceddID,int formHdrID)
        {
            return db.Conf_StatusHaveToUpdateorNot(empID, proceddID, formHdrID).FirstOrDefault();
        }

        public GetConfirmationDtlForOrdrRpt_Result GetConfirmationDtlForOrdrRpt(int empID, int hdrID, int formHdrID)
        {
            return db.GetConfirmationDtlForOrdrRpt(empID, hdrID, formHdrID).FirstOrDefault();
        }
    }
}
