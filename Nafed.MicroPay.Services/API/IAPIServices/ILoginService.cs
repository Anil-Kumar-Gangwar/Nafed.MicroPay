using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.API.IAPIServices
{
    public interface ILoginService
    {
        bool ValidateUser(ValidateLogin userCredential, out string sAuthenticationMessage, out UserDetail userDetail);
        UserAccessRight GetUserAccessRight(int userID, int menuID);
    }
}
