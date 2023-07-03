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
    public class EmployeeTypeController : BaseController
    {

        private readonly IEmployeeTypeService empTypeService;
        public EmployeeTypeController(IEmployeeTypeService empTypeService)
        {
            this.empTypeService = empTypeService;
        }

        // GET: EmployeeType
        public ActionResult Index()
        {
            log.Info($"EmployeeTypeController/Index");
            return View(userAccessRight);
        }
        public ActionResult EmpTypeGridView(FormCollection formCollection)
        {
            log.Info($"EmployeeTypeController/CadreGridView");
            try
            {
                EmployeeTypeViewModel empTypeVM = new EmployeeTypeViewModel();
                List<Model.EmployeeType> objEmpType = new List<Model.EmployeeType>();
                empTypeVM.listEmployeeType = empTypeService.GetEmployeeTypeList();
                empTypeVM.userRights = userAccessRight;
                return PartialView("_EmpTypeGridView", empTypeVM);
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
            log.Info("EmployeeTypeController/Create");
            try
            {
                Model.EmployeeType objEmpType = new Model.EmployeeType();
                return View(objEmpType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.EmployeeType createEmpType)
        {
            log.Info("EmployeeTypeController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createEmpType.EmployeeTypeName = createEmpType.EmployeeTypeName.Trim();
                    createEmpType.EmployeeTypeCode = createEmpType.EmployeeTypeCode.Trim();
                    if (empTypeService.EmployeeTypeNameExists(createEmpType.EmployeeTypeID, createEmpType.EmployeeTypeName))
                    {
                        ModelState.AddModelError("EmpTypeNameAlreadyExist", "Emplyoee Type Name Already Exist");
                        return View(createEmpType);
                    }
                    else if (empTypeService.EmployeeTypeCodeExists(createEmpType.EmployeeTypeID, createEmpType.EmployeeTypeCode))
                    {
                        ModelState.AddModelError("EmpTypeCodeAlreadyExist", "Emplyoee Type Code Already Exist");
                        return View(createEmpType);
                    }
                    else
                    {
                        empTypeService.InsertEmployeeType(createEmpType);
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
            return View(createEmpType);
        }

        [HttpGet]
        public ActionResult Edit(int empTypeID)
        {
            log.Info("EmployeeTypeController/Edit");
            try
            {
                Model.EmployeeType objEmpType = new Model.EmployeeType();
                objEmpType = empTypeService.GetEmployeeTypeByID(empTypeID);
                return View(objEmpType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.EmployeeType editEmpType)
        {
            log.Info("EmployeeTypeController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editEmpType.EmployeeTypeName = editEmpType.EmployeeTypeName.Trim();
                    editEmpType.EmployeeTypeCode = editEmpType.EmployeeTypeCode.Trim();
                    if (empTypeService.EmployeeTypeNameExists(editEmpType.EmployeeTypeID, editEmpType.EmployeeTypeName))
                    {
                        ModelState.AddModelError("EmpTypeNameAlreadyExist", "Employee Type Name Already Exist");
                        return View(editEmpType);
                    }
                    //else if (empTypeService.EmployeeTypeCodeExists(editEmpType.EmployeeTypeID, editEmpType.EmployeeTypeCode))
                    //{
                    //    ModelState.AddModelError("EmpTypeCodeAlreadyExist", "Employee Type Code Already Exist");
                    //    return View(editEmpType);
                    //}
                    else
                    {
                        empTypeService.UpdateEmployeeType(editEmpType);
                        TempData["Message"] = "Succesfully Updated";
                        return RedirectToAction("Index");
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editEmpType);

        }


        public ActionResult Delete(int empTypeID)
        {
            log.Info("EmployeeTypeController/Delete");
            try
            {
                empTypeService.Delete(empTypeID);
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