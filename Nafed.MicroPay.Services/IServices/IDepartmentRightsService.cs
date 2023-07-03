using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IDepartmentRightsService
    {
       List<SelectListModel> GetDepartment();
        List<DepartmentRights> GetDepartmentRights(int departmentID);
        int InsertUpdateDepartmentRights(int departmentID, DataTable dtDepartmentMenuRights);
        List<int> GetDepartmentIDs();
    }
}
