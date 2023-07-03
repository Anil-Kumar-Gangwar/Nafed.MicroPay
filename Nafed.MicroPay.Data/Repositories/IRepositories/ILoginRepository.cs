using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface ILoginRepository
    {
        bool VerifyUser(string username, out string dbpassword);
        User GetUserData(string username);
    }
}
