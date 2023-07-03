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
    public class MaritalStatusController : BaseController
    {

        private readonly IMaritalStatusService maritalStatusService;
        public MaritalStatusController(IMaritalStatusService maritalStatusService)
        {
            this.maritalStatusService = maritalStatusService;
        }

        public ActionResult Index()
        {
            log.Info($"MaritalStatusController/Index");
            return View(userAccessRight);
        }

        public ActionResult MaritalStatusGridView(FormCollection formCollection)
        {
            log.Info($"MaritalStatusController/MaritalStatusGridView");
            try
            {
                MaritalStatusViewModel MSVM = new MaritalStatusViewModel();
                List<Model.MaritalStatus> objMaritalStatusList = new List<Model.MaritalStatus>();
                MSVM.listMaritalStatus = maritalStatusService.GetMaritalStatusList();
                MSVM.userRights = userAccessRight;
                return PartialView("_MaritalStatusGridView", MSVM);
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
            log.Info("MaritalStatusController/Create");
            try
            {
                Model.MaritalStatus objMaritalStatus = new Model.MaritalStatus();
                return View(objMaritalStatus);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.MaritalStatus createMaritalStatus)
        {
            log.Info("MaritalStatusController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createMaritalStatus.MaritalStatusName = createMaritalStatus.MaritalStatusName.Trim();
                    createMaritalStatus.MaritalStatusCode = createMaritalStatus.MaritalStatusCode.Trim();

                    createMaritalStatus.CreatedBy = userDetail.UserID;
                    createMaritalStatus.CreatedOn = System.DateTime.Now;

                    if (maritalStatusService.MaritalStatusNameExists(createMaritalStatus.MaritalStatusID, createMaritalStatus.MaritalStatusName))
                    {
                        ModelState.AddModelError("MaritalStatusNameAlreadyExist", "MaritalStatus Name Already Exist");
                        return View(createMaritalStatus);
                    }
                    else if (maritalStatusService.MaritalStatusCodeExists(createMaritalStatus.MaritalStatusID, createMaritalStatus.MaritalStatusCode))
                    {
                        ModelState.AddModelError("MaritalStatusCodeAlreadyExist", "MaritalStatus Code Already Exist");
                        return View(createMaritalStatus);
                    }
                    else
                    {
                        maritalStatusService.InsertMaritalStatus(createMaritalStatus);
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
            return View(createMaritalStatus);
        }

        [HttpGet]
        public ActionResult Edit(int maritalStatusID)
        {
            log.Info("MaritalStatusItemController/Edit");
            try
            {
                Model.MaritalStatus objMaritalStatus = new Model.MaritalStatus();
                objMaritalStatus = maritalStatusService.GetMaritalStatusByID(maritalStatusID);
                return View(objMaritalStatus);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.MaritalStatus editMaritalStatus)
        {
            log.Info("MaritalStatusItemController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editMaritalStatus.MaritalStatusName = editMaritalStatus.MaritalStatusName.Trim();
                    editMaritalStatus.MaritalStatusCode = editMaritalStatus.MaritalStatusCode.Trim();

                    editMaritalStatus.UpdatedBy = userDetail.UserID;
                    editMaritalStatus.UpdatedOn = System.DateTime.Now;

                    if (maritalStatusService.MaritalStatusNameExists(editMaritalStatus.MaritalStatusID, editMaritalStatus.MaritalStatusName))
                    {
                        ModelState.AddModelError("MaritalStatusNameAlreadyExist", "MaritalStatus Name Already Exist");
                        return View(editMaritalStatus);
                    }
                    else if (maritalStatusService.MaritalStatusCodeExists(editMaritalStatus.MaritalStatusID, editMaritalStatus.MaritalStatusCode))
                    {
                        ModelState.AddModelError("MaritalStatusCodeAlreadyExist", "MaritalStatus Code Already Exist");
                        return View(editMaritalStatus);
                    }
                    else
                    {
                        maritalStatusService.UpdateMaritalStatus(editMaritalStatus);
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
            return View(editMaritalStatus); 
        } 

        public ActionResult Delete(int maritalStatusID)
        {
            log.Info("Delete");
            try
            {
                maritalStatusService.Delete(maritalStatusID);
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