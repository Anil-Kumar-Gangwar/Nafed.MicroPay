using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using System.Data;

namespace MicroPay.Web.Controllers
{
    public class UserAccessControlRightsController : BaseController
    {
        private readonly IUserRightsService userRightService;
        private readonly IDropdownBindService ddlService;
        public UserAccessControlRightsController(IUserRightsService userRightService, IDropdownBindService ddlService)
        {
            this.userRightService = userRightService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            UserRights objUserRights = new UserRights();
            var departmentList = ddlService.ddlDepartmentList();
            ViewBag.DepartmentList = departmentList;
            if (departmentList.Count() > 0)
                objUserRights.DepartmentID = departmentList.FirstOrDefault().id;
            return View(objUserRights);
        }
        public ActionResult UserAccessRights(int departmentID)
        {
            ViewBag.UserList = userRightService.GetDepartmentUserList(departmentID);
            return PartialView("_UserAccessRights");
        }
        public ActionResult AccessControl(int departmentID)
        {
            UserAccessControlViewModel userAccessVM = new UserAccessControlViewModel();
            userAccessVM.menuParentList = userRightService.GetUserAccessMenuList(departmentID, true);
            userAccessVM.menuChildList = userRightService.GetUserAccessMenuList(departmentID, false);
            return PartialView("_AccessControl", userAccessVM);
        }
        public ActionResult BindDepartmentUserAccessRights(int userID, int departmentID)
        {

            try
            {
                UserAccessControlViewModel userAccessVM = new UserAccessControlViewModel();
                userAccessVM.menuParentList = userRightService.GetUserAccessMenuList(departmentID, true);
                userAccessVM.menuChildList = userRightService.GetUserAccessRightList(userID, departmentID);
                return PartialView("_AccessControl", userAccessVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult InsertUpdateUserAccessControlRights(int departmentID, string[] arrayUserAccessControlRight, bool isAll = false, int? userID = null)
        {
            try
            {
                List<Model.UserAccessControlRights> listUpdateUserAccessRights = new List<Model.UserAccessControlRights>();
                if (arrayUserAccessControlRight != null)
                {
                    if (isAll)
                    {
                        var userList = userRightService.GetUserByDepartmentID(departmentID);
                        for (int j = 0; j < userList.Count; j++)
                        {
                            for (int i = 0; i < arrayUserAccessControlRight.Length; i++)
                            {
                                Model.UserAccessControlRights obj = new Model.UserAccessControlRights();
                                string[] arr = arrayUserAccessControlRight[i].Split(',');
                                int menuid = int.Parse(arr[0]);
                                bool viewRights = bool.Parse(arr[1]);
                                bool createRights = bool.Parse(arr[2]);
                                bool editRights = bool.Parse(arr[3]);
                                bool deleteRights = bool.Parse(arr[4]);
                                obj.MenuID = menuid;
                                obj.ShowMenu = true;
                                obj.View = viewRights;
                                obj.Create = createRights;
                                obj.Edit = editRights;
                                obj.Delete = deleteRights;
                                listUpdateUserAccessRights.Add(obj);
                            }
                            userRightService.InsertUpdateUserAccessControlRights(userList[j].UserID, departmentID, listUpdateUserAccessRights);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < arrayUserAccessControlRight.Length; i++)
                        {
                            Model.UserAccessControlRights obj = new Model.UserAccessControlRights();
                            string[] arr = arrayUserAccessControlRight[i].Split(',');
                            int menuid = int.Parse(arr[0]);
                            bool viewRights = bool.Parse(arr[1]);
                            bool createRights = bool.Parse(arr[2]);
                            bool editRights = bool.Parse(arr[3]);
                            bool deleteRights = bool.Parse(arr[4]);
                            obj.MenuID = menuid;
                            obj.ShowMenu = true;
                            obj.View = viewRights;
                            obj.Create = createRights;
                            obj.Edit = editRights;
                            obj.Delete = deleteRights;
                            listUpdateUserAccessRights.Add(obj);
                        }
                        userRightService.InsertUpdateUserAccessControlRights((int)userID, departmentID, listUpdateUserAccessRights);
                    }
                    TempData["Message"] = " User rights updated successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message.ToString();
            }
            return Content("1");
        }
    }
}