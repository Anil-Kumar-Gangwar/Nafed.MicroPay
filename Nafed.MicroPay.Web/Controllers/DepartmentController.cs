using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers
{
    public class DepartmentController : BaseController
    {
        // GET: Department
        private readonly IDepartmentService departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            this.departmentService = departmentService;
        }
        public ActionResult Index()
        {
            log.Info($"DepartmentController/Index");
            return View(userAccessRight);
        }

        public ActionResult DepartmentGridView(FormCollection formCollection)
        {
            log.Info($"DepartmentController/DepartmentGridView");
            try
            {
                DepartmentViewModel departmentVM = new DepartmentViewModel();
                List<Model.Department> objDepartment = new List<Model.Department>();
                departmentVM.listDepartment = departmentService.GetDeaprtmentList();
                departmentVM.userRights = userAccessRight;
                return PartialView("_DepartmentGridView", departmentVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info("DepartmentController/Create");
            try
            {
                Model.Department objDepartment = new Model.Department();
                return View(objDepartment);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.Department createDepartment)
        {
            log.Info("DepartmentController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createDepartment.DepartmentName = createDepartment.DepartmentName.Trim();
                    createDepartment.DepartmentCode = "1";
                    createDepartment.CreatedBy = userDetail.UserID;
                    createDepartment.CreatedOn = System.DateTime.Now;
                    if (departmentService.DepartmentNameExists(createDepartment.DepartmentID, createDepartment.DepartmentName))
                    {
                        ModelState.AddModelError("DepartmentNameAlreadyExist", "Department Name Already Exist");
                        return View(createDepartment);
                    }
                    //else if (departmentService.DepartmentCodeExists(createDepartment.DepartmentID, createDepartment.DepartmentCode))
                    //{
                    //    ModelState.AddModelError("DepartmentCodeAlreadyExist", "Department Code Already Exist");
                    //    return View(createDepartment);
                    //}
                    else
                    {
                        departmentService.InsertDepartment(createDepartment);
                        TempData["Message"] = "Successfully Created";
                        return RedirectToAction("Index");
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(createDepartment);
        }

        [HttpGet]
        public ActionResult Edit(int departmentID)
        {
            log.Info("DepartmentController/Edit");
            try
            {
                Model.Department objDepartment = new Model.Department();
                objDepartment = departmentService.GetDepartmentByID(departmentID);
                return View(objDepartment);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.Department editDepartment)
        {
            log.Info("DepartmentController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editDepartment.DepartmentName = editDepartment.DepartmentName.Trim();
                    editDepartment.DepartmentCode = editDepartment.DepartmentCode.Trim();
                    //if (departmentService.DepartmentNameExists(editDepartment.DepartmentID, editDepartment.DepartmentName))
                    //{
                    //    ModelState.AddModelError("DepartmentNameAlreadyExist", "Department Name Already Exist");
                    //    return View(editDepartment);
                    //}
                    //else if (departmentService.DepartmentCodeExists(editDepartment.DepartmentID, editDepartment.DepartmentCode))
                    //{
                    //    ModelState.AddModelError("DepartmentCodeAlreadyExist", "Department Code Already Exist");
                    //    return View(editDepartment);
                    //}
                    //else
                    //{
                        departmentService.UpdateDepartment(editDepartment);
                        TempData["Message"] = "Succesfully Updated";
                        return RedirectToAction("Index");
                    //}
                }

            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editDepartment);

        }


        public ActionResult Delete(int departmentID)
        {
            log.Info("Delete");
            try
            {
                departmentService.Delete(departmentID);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }

    }
}