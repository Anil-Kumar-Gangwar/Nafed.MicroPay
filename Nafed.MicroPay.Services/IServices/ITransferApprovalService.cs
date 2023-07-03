using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface ITransferApprovalService
    {
        List<Model.TrainingParticipantsDetail> GetTransferEmployeeDetailsList(int type);

        bool TransferApproval(int fromEmployeeID, int toEmployeeID, int? processId);

        List<Model.SelectListModel> GetProcessList();

    }
}
