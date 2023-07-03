using Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IForm12BBRepository
    {
        DataTable GetForm12BBDetails(string @FYear);
    }
}
