using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
  public interface IBranchService
    {
        List<Model.Branch> GetBranchList();
        bool BranchNameExists(string branchName);

        Model.Branch GetBranchByID(int branchID);

        bool UpdateBranch(Model.Branch editBranch);
        int InsertBranch(Model.Branch createBrach);

        bool Delete(int branchID);
        BranchTransfer GetBranchTransferDtls(int? employeeID);
        BranchManagerDetails GetBranchTransferForm(int? employeeID, int? transID);
        BranchManagerDetails ChangeBranch(BranchManagerDetails branchDtls);
        bool DeleteBranchTransEntry(int transID);
    }
}
