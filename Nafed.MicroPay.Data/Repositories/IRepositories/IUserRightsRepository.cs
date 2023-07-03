using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Data.Models;
namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IUserRightsRepository
    {
        int InsertUpdateDepartmentUserRights(int userID,int departmentID, DataTable departmentUserMenuRights);

        List<Menu> GetUserAccessMenuList(int departmentID);
    }
}
