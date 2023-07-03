using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using System.Data;
using System.Data.SqlClient;
using Nafed.MicroPay.Data.Repositories.IRepositories;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IDepartmentRightRepository
    {
        int InsertUpdateDepartmentRights(int departmentID, DataTable departmentMenuRight);
        List<Data.Models.GetEmpCountBasedOnDepartment_Result> GetEmpCountBasedOnDepartment();

    }
}
