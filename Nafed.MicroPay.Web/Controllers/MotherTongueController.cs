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
    public class MotherTongueController : BaseController
    {
        private readonly  IMotherTongueService motherTongueService;
         
        public MotherTongueController(IMotherTongueService motherTongueService)
        {
            this.motherTongueService = motherTongueService;
        }
        public ActionResult Index()
        {
            log.Info($"MotherTongueController/Index");
            return View(userAccessRight);
        }

        public ActionResult MotherTongueGridView(FormCollection formCollection)
        {
            log.Info($"MotherTongueController/MotherTongueGridView");
            try
            {
                MotherTongueModel motherTongueVM = new MotherTongueModel();
                motherTongueVM.motherTongueList = motherTongueService.GetMotherTongueList();
                motherTongueVM.userRights = userAccessRight;
                return PartialView("_MotherTongueGridView", motherTongueVM);
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
            log.Info("MotherTongueController/Create");
            try
            {
                Model.MotherTongueModel mothertongueDetails = new Model.MotherTongueModel();
                return View(mothertongueDetails);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Model.MotherTongueModel createMotherTongue)
        {
            log.Info("MotherTongueController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createMotherTongue.Name = createMotherTongue.Name.Trim();
                    if (motherTongueService.MotherTongueExists(createMotherTongue.ID, createMotherTongue.Name))
                        ModelState.AddModelError("TongueNameAlreadyExist", "Mother Tongue Already Exist");
                    else
                    {
                        int motherTongueID = motherTongueService.InsertMotherTongueDetails(createMotherTongue);
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
            return View(createMotherTongue);
        }

        [HttpGet]
        public ActionResult Edit(int motherTongueId)
        {
            log.Info("MotherTongueController/Edit");
            try
            {
                Model.MotherTongueModel mothertongueDetails = new Model.MotherTongueModel();
                mothertongueDetails = motherTongueService.GetmotherTongueDetailsByID(motherTongueId);
                return View(mothertongueDetails);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Model.MotherTongueModel createMotherTongue)
        {
            log.Info("MotherTongueController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    createMotherTongue.Name = createMotherTongue.Name.Trim();
                    if (motherTongueService.MotherTongueExists(createMotherTongue.ID, createMotherTongue.Name))
                        ModelState.AddModelError("TongueNameAlreadyExist", "Mother Tongue Already Exist");
                    else
                    {
                        motherTongueService.UpdateMotherTongueDetails(createMotherTongue);
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
            return View(createMotherTongue);

        }

        public ActionResult Delete(int motherTongueId)
        {
            log.Info("MotherTongueController/Delete");
            try
            {
                motherTongueService.Delete(motherTongueId);
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