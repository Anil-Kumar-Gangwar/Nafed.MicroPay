using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IProcessApproverRepository
    {
        DataSet GetImportFieldValues(DataTable dtEmpCodes, DataTable dtReportingTo,
            DataTable dtReviewerTo, DataTable dtAcceptanceTo);
        int ImportProcessApproverData(int userID, int processID, DataTable dtApprovers);


    }
}
