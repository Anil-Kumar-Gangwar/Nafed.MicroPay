using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using AutoMapper;
using dtoModel = Nafed.MicroPay.Data.Models;
using System.Collections.Generic;
namespace Nafed.MicroPay.Services.IServices
{
    public interface IMenuService
    {
        List<Model.Menu> GetMenuList();
        List<Model.Menu> GetMenuList(int userID,int departmentID,int userTypeID, bool activeOnMobile= false);

        List<Model.SelectListModel> GetParentList(int? menuID=null);

        bool MenuExists(int? parentID, int? menuID, string menuName);
        bool UpdateMenu(Model.Menu editMenuItem);
        bool InsertMenu(Model.Menu createMenu);
        Model.Menu GetMenuByID(int menuID);
        bool Delete(int menuID);
        List<UserAccessRight> GetUserMenuRights(int userID, int deptID,int userTypeID);
    }
}