using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IActivityLogRepository
    {
        DataTable GetActivityLog();
    }
}
