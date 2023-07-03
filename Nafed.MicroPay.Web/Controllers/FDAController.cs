using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers
{
    public class FDAController : BaseController
    {
        private readonly IFDAService fdaService;
        public FDAController(IFDAService fdaService)
        {
            this.fdaService = fdaService;
        }
        // GET: FDA
        public ActionResult Index()
        {
            log.Info($"FDAController/Index");
            return View(userAccessRight);
        }

        public ActionResult FDAGridView()
        {
            log.Info($"FDAController/FDAGridView");
            try
            {
                FDAViewModel FDAVM = new FDAViewModel();
                FDAVM.FDAList = fdaService.GetFDAList();
                FDAVM.userRights = userAccessRight;
                return PartialView("_FDAGridView", FDAVM);
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
            log.Info("FDAController/Create");
            try
            {
                FDA objFDA = new FDA();
                return View(objFDA);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(FDA createFDA)
        {
            log.Info("FDAController/Create");
            try
            {

                if (ModelState.IsValid)
                {

                    createFDA.CreatedBy = userDetail.UserID;
                    createFDA.CreatedOn = DateTime.Now;
                    createFDA.IsDeleted = false;
                    if (fdaService.FDAExists(createFDA.upperlimit, (decimal)createFDA.val))
                    {
                        ModelState.AddModelError("UpperLimitAlreadyExist", "Upper Limit Already Exist");
                        return View(createFDA);
                    }
                    else
                    {
                        fdaService.InsertFDA(createFDA);
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
            return View(createFDA);
        }

        [HttpGet]
        public ActionResult Edit(decimal upperlimit, decimal percentage)
        {
            log.Info("FDAController/Edit");
            try
            {
                FDA objFDA = new FDA();
                objFDA = fdaService.GetFDA(upperlimit, percentage);
                return View(objFDA);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(FDA updateFD)
        {
            log.Info("FDAController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    updateFD.UpdatedBy = userDetail.UserID;
                    updateFD.UpdatedOn = DateTime.Now;
                    updateFD.IsDeleted = false;
                    fdaService.UpdateFDA(updateFD);
                    TempData["Message"] = "Succesfully Updated";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(updateFD);

        }

        public ActionResult Delete(decimal upperlimit, decimal percentage)
        {
            log.Info("FDAController/Delete");
            try
            {
                fdaService.Delete(upperlimit, percentage);
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