using Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IForm12BBDocumentRepository
    {
        IEnumerable<GetForm12BBDocumentList_Result> GetForm12BBDocumentList(int EmpFormID);
    }
}
