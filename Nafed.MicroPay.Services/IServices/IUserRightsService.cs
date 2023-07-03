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
    public interface IUserRightsService
    {
            
        List<SelectListModel> GetDepartmentUserList(int departmentID);
        List<Model.Menu> GetMenuList(int departmentID);
        List<UserRights> GetDepartmentUserRights(int userID, int departmentID);
        int InsertUpdateDepartmentUserRights(int userID, int departmentID, DataTable dtDepartmentUserMenuRights);

        List<Model.UserAccessControlRights> GetUserAccessMenuList(int departmentID, bool isParentMenu);
        List<Model.UserAccessControlRights> GetUserAccessRightList(int departmentID, int userID);

        bool InsertUpdateUserAccessControlRights(int userID, int departmentID, List<Model.UserAccessControlRights> listUpdateUserAccessRights);
        List<User> GetUserByDepartmentID(int departmentID);
        List<int> GetUserIDByDepartment(int departmentID);
    }
}
