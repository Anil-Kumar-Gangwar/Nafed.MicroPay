using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface ITransferApprovalRepository
    {
        bool TransferApproval(int fromEmployeeID, int toEmployeeID, int? processId);
    }
}
