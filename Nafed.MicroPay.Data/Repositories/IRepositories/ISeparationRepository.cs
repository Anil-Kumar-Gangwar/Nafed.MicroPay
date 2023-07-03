using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface ISeparationRepository
    {
        bool AcceptRejectClearance(int sepId, int statusId);
    }
}
