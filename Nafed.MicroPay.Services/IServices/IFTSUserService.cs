using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IFTSUserService
    {
        List<Model.FTSUser> GetUserList();
        bool UserNameExists(int? userID, string UserName);       
        bool UpdateUser(Model.FTSUser editUser);
        int InsertUser(Model.FTSUser createUser);
        Model.FTSUser GetUserByID(int userID);
        bool InsertMapDepartment(Model.FTSUserDepartment ftsDepartment);
        int[] GetMappedDepartment(int userId);
        bool UserExistInUserDepartment(int userId);
        bool UpdateMapDepartment(Model.FTSUserDepartment ftsDepartment);
    }
}
