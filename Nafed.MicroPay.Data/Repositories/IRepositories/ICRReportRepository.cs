using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface ICRReportRepository
    {
        DataTable GetEmployeeDetails(string query);
    }
}
