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
    public class DepartmentRightsController : BaseController
    {
       // private IGenericRepository genericRepo;
        private readonly IMenuService menuService;
        private readonly IDepartmentRightsService departmentService;
        public DepartmentRightsController(IMenuService menuService, IDepartmentRightsService departmentService)
        {
          //  genericRepo = new GenericRepository();
            this.menuService = menuService;
            this.departmentService = departmentService;
        }
        public ActionResult Index()
        {
            ViewBag.DepartmentList = departmentService.GetDepartment();
            ViewBag.Menu = menuService.GetMenuList();
            return View();
        }

        public ActionResult BindDepartmentRights(int departmentID)
        {
            List<DepartmentRights> listDepartmentRights = new List<DepartmentRights>();
            try
            {
                listDepartmentRights = departmentService.GetDepartmentRights(departmentID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(listDepartmentRights, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult InsertUpdateDepartmentRights(DepartmentRights model)
        {
            try
            {
                DataTable dtDepartmentRights = new DataTable();
                dtDepartmentRights.TableName = "DepartmentMenuRights";
                dtDepartmentRights.Columns.Add(new DataColumn("DepartmentID", typeof(int)));
                dtDepartmentRights.Columns.Add(new DataColumn("MenuID", typeof(int)));
                dtDepartmentRights.Columns.Add(new DataColumn("ShowMenu", typeof(bool)));
                List<int> departmentIDs = null;
                int result = 0;
                if (model.isAll)
                {
                    departmentIDs = departmentService.GetDepartmentIDs();
                        //genericRepo.Get<Nafed.MicroPay.Data.Models.Department>(x => !(bool)x.IsDeleted).Select(em => new Department() { DepartmentID = em.DepartmentID }).ToList();
                    string[] arr = new string[] { };
                    if (model.hdnCheckedVal != null)
                        arr = Convert.ToString(model.hdnCheckedVal).Split(',');
                    for (int j = 0; j < departmentIDs.Count; j++)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            string menuid = Convert.ToString(arr[i]);
                            DataRow dr = dtDepartmentRights.NewRow();
                            dr[0] = departmentIDs[j];
                            dr[1] = menuid;
                            dr[2] = true;
                            DataRow[] row = dtDepartmentRights.Select("MenuID =" + menuid);
                            if (row.Length == 0)
                                dtDepartmentRights.Rows.Add(dr);
                        }
                        result = departmentService.InsertUpdateDepartmentRights(departmentIDs[j], dtDepartmentRights);
                        dtDepartmentRights.Clear();
                    }
                }
                else
                {
                    if (model.DepartmentID != 0)
                    {
                        int departmentID = model.DepartmentID;
                        string[] arr = new string[] { };
                        if (model.hdnCheckedVal != null)
                            arr = Convert.ToString(model.hdnCheckedVal).Split(',');
                        for (int i = 0; i < arr.Length; i++)
                        {
                            string menuid = Convert.ToString(arr[i]);
                            DataRow dr = dtDepartmentRights.NewRow();
                            dr[0] = departmentID;
                            dr[1] = menuid;
                            dr[2] = true;
                            DataRow[] row = dtDepartmentRights.Select("MenuID =" + menuid);
                            if (row.Length == 0)
                                dtDepartmentRights.Rows.Add(dr);
                            result = departmentService.InsertUpdateDepartmentRights(departmentID, dtDepartmentRights);
                        }
                    }
                    else
                        TempData["Error"] = " Please select Department first.";
                }
                if (result > 0)
                    TempData["Message"] = " Department rights updated successfully.";
                else TempData["Error"] = " Department rights updated failed.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message.ToString();
            }
            return RedirectToAction("Index");
        }


    }
}