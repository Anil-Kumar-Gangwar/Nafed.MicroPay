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
//using Nafed.MicroPay.Data.Repositories.IRepositories;
//using Nafed.MicroPay.Data.Repositories;

namespace MicroPay.Web.Controllers
{
    public class UserRightsController : BaseController
    {

        private readonly IUserRightsService userRightService;
        private readonly IDropdownBindService ddlService;
       // private IGenericRepository genericRepo;
        public UserRightsController(IUserRightsService userRightService, IDropdownBindService ddlService)
        {
         //  genericRepo = new GenericRepository();
            this.userRightService = userRightService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            UserRights objUserRights = new UserRights();
            var departmentList = ddlService.ddlDepartmentList();
            ViewBag.DepartmentList = departmentList;
            if (departmentList.Count() > 0)
                objUserRights.DepartmentID =departmentList.FirstOrDefault().id;
            return View(objUserRights);
        }
        public ActionResult UserMenuRights(int departmentID)
        {
            ViewBag.UserList = userRightService.GetDepartmentUserList(departmentID);
            ViewBag.Menu = userRightService.GetMenuList(departmentID);
            return PartialView("_UserMenuRights");
        }
        public ActionResult BindDepartmentUserMenuRights(int userID, int departmentID)
        {
            List<UserRights> listDepartmentRights = new List<UserRights>();
            try
            {
                listDepartmentRights = userRightService.GetDepartmentUserRights(userID, departmentID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(listDepartmentRights, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult InsertUpdateDepartmentUserRights(UserRights model)
        {
            try
            {
                DataTable dtDepartmentRights = new DataTable();
                dtDepartmentRights.TableName = "UserMenuRights";
                dtDepartmentRights.Columns.Add(new DataColumn("UserID", typeof(int)));
                dtDepartmentRights.Columns.Add(new DataColumn("DepartmentID", typeof(int)));
                dtDepartmentRights.Columns.Add(new DataColumn("MenuID", typeof(int)));
                dtDepartmentRights.Columns.Add(new DataColumn("ShowMenu", typeof(bool)));
                dtDepartmentRights.Columns.Add(new DataColumn("CreateRight", typeof(bool)));
                dtDepartmentRights.Columns.Add(new DataColumn("ViewRight", typeof(bool)));
                dtDepartmentRights.Columns.Add(new DataColumn("EditRight", typeof(bool)));
                dtDepartmentRights.Columns.Add(new DataColumn("DeleteRight", typeof(bool)));
                List<int> userIDs = null;
                int result = 0;
                int departmentID = 0;
                if (model.isAll)
                {
                    departmentID = model.DepartmentID;
                    //userIDs = genericRepo.Get<Nafed.MicroPay.Data.Models.User>(x => !(bool)x.IsDeleted && x.DepartmentID== departmentID && x.UserTypeID !=1).Select(em => new User() { UserID = em.UserID }).ToList();                    
                    userIDs = userRightService.GetUserIDByDepartment(departmentID);
                   
                    string[] arr = new string[] { };
                    if (model.hdnCheckedVal != null)
                        arr = Convert.ToString(model.hdnCheckedVal).Split(',');
                    for (int j = 0; j < userIDs.Count; j++)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            string menuid = Convert.ToString(arr[i]);
                            DataRow dr = dtDepartmentRights.NewRow();
                            dr[0] = userIDs[j];
                            dr[1] = departmentID;
                            dr[2] = menuid;
                            dr[3] = true;
                            dr[4] = true;
                            dr[5] = true;
                            dr[6] = true;
                            dr[7] = true;
                            DataRow[] row = dtDepartmentRights.Select("MenuID =" + menuid);
                            if (row.Length == 0)
                                dtDepartmentRights.Rows.Add(dr);
                        }
                        result = userRightService.InsertUpdateDepartmentUserRights(userIDs[j], departmentID, dtDepartmentRights);
                        dtDepartmentRights.Clear();
                    }
                }
                else
                {
                    if (model.DepartmentID != 0)
                    {
                        departmentID = model.DepartmentID;
                        int userID = model.UserID;
                        string[] arr = new string[] { };
                        if (model.hdnCheckedVal != null)
                            arr = Convert.ToString(model.hdnCheckedVal).Split(',');                       
                            for (int i = 0; i < arr.Length; i++)
                            {
                                string menuid = Convert.ToString(arr[i]);
                                DataRow dr = dtDepartmentRights.NewRow();
                                dr[0] = userID;
                                dr[1] = departmentID;
                                dr[2] = menuid;
                                dr[3] = true;
                                dr[4] = true;
                                dr[5] = true;
                                dr[6] = true;
                                dr[7] = true;
                                DataRow[] row = dtDepartmentRights.Select("MenuID =" + menuid);
                                if (row.Length == 0)
                                    dtDepartmentRights.Rows.Add(dr);
                            }
                            result = userRightService.InsertUpdateDepartmentUserRights(userID, departmentID, dtDepartmentRights);
                       
                    }
                    else
                        TempData["Error"] = " Please select User first.";
                }
                if (result > 0)
                    TempData["Message"] = " User rights updated successfully.";
                else TempData["Error"] = " User rights updated failed.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message.ToString();
            }
            return RedirectToAction("Index");
        }

    }
}