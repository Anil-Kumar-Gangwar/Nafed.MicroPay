using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IUpdateBasicRepository
    {
        int ValidateNewBasicAmount(int EmployeeId, double NewBasic);
    }
}
