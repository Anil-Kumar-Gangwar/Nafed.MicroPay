using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers
{
    public class DivisionController : BaseController
    {

        private readonly IDivisionService divisionService;
        public DivisionController(IDivisionService divisionService)
        {
            this.divisionService = divisionService;
        }
        public ActionResult Index()
        {
            log.Info($"DivisionController/Index");
            return View(userAccessRight);
        }

        public ActionResult DivisionGridView(FormCollection formCollection)
        {
            log.Info($"DivisionController/DivisionGridView");
            try
            {
                DivisionViewModel DivVM = new DivisionViewModel();
                List<Model.Division> objDivisionList = new List<Model.Division>();
                DivVM.listDivision = divisionService.GetDivisionList();
                DivVM.userRights = userAccessRight;
                return PartialView("_DivisionGridView", DivVM);
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
            log.Info("DivisionController/Create");
            try
            {
                Model.Division objDivision = new Model.Division();
                return View(objDivision);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult Create(Model.Division createDivision)
        {
            log.Info("DivisionController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createDivision.DivisionName = createDivision.DivisionName.Trim();
                    createDivision.DivisionCode = createDivision.DivisionCode.Trim();

                    createDivision.CreatedBy = userDetail.UserID;
                    createDivision.CreatedOn = System.DateTime.Now;

                    if (divisionService.DivisionNameExists(createDivision.DivisionID, createDivision.DivisionName))
                    {
                        ModelState.AddModelError("DivisionNameAlreadyExist", "Division Name Already Exist");
                        return View(createDivision);
                    }
                    else if (divisionService.DivisionCodeExists(createDivision.DivisionID, createDivision.DivisionCode))
                    {
                        ModelState.AddModelError("DivisionCodeAlreadyExist", "Division Code Already Exist");
                        return View(createDivision);
                    }
                    else
                    {
                        divisionService.InsertDivision(createDivision);
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
            return View(createDivision);
        }

        [HttpGet]
        public ActionResult Edit(int divisionID)
        {
            log.Info("DivisionItemController/Edit");
            try
            {
                Model.Division objDivision = new Model.Division();
                objDivision = divisionService.GetDivisionByID(divisionID);
                return View(objDivision);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.Division editDivision)
        {
            log.Info("DivisionItemController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editDivision.DivisionName = editDivision.DivisionName.Trim();
                    editDivision.DivisionCode = editDivision.DivisionCode.Trim();

                    editDivision.UpdatedBy = userDetail.UserID;
                    editDivision.UpdatedOn = System.DateTime.Now;

                    if (divisionService.DivisionNameExists(editDivision.DivisionID, editDivision.DivisionName))
                    {
                        ModelState.AddModelError("DivisionNameAlreadyExist", "Division Name Already Exist");
                        return View(editDivision);
                    }
                    else if (divisionService.DivisionCodeExists(editDivision.DivisionID, editDivision.DivisionCode))
                    {
                        ModelState.AddModelError("DivisionCodeAlreadyExist", "Division Code Already Exist");
                        return View(editDivision);
                    }
                    else
                    {
                        divisionService.UpdateDivision(editDivision);
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
            return View(editDivision); 
        } 

        public ActionResult Delete(int  divisionID)
        {
            log.Info("Delete");
            try
            {
                divisionService.Delete(divisionID);
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