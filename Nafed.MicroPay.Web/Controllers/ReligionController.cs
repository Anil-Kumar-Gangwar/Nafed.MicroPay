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
    public class ReligionController : BaseController
    {
        private readonly IReligionService religionService;
        public ReligionController(IReligionService religionService)
        {
            this.religionService = religionService;
        }
        // GET: Category
        public ActionResult Index()
        {
            log.Info($"ReligionController/Index");
            return View(userAccessRight);
        }
        public ActionResult ReligionGridView(FormCollection formCollection)
        {
            log.Info($"ReligionController/ReligionGridView");
            try
            {
                ReligionViewModel religionVM = new ReligionViewModel();
                List<Model.Religion> objReligion = new List<Model.Religion>();
                religionVM.listReligion = religionService.GetReligion();
                religionVM.userRights = userAccessRight;
                return PartialView("_ReligionGridView", religionVM);
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
            log.Info("ReligionController/Create");
            try
            {
                Model.Religion religion = new Model.Religion();
                return View(religion);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.Religion createReligion)
        {
            log.Info("ReligionController/Create");
            try
            {
                createReligion.CreatedBy = userDetail.UserID;
                createReligion.CreatedOn = DateTime.Now;
                if (ModelState.IsValid)
                {
                    createReligion.ReligionName = createReligion.ReligionName.Trim();
                 
                    if (religionService.ReligionNameExists(createReligion.ReligionID, createReligion.ReligionName))
                    {
                        ModelState.AddModelError("ReligionNameAlreadyExist", "Religion Name Already Exist");
                        return View(createReligion);
                    }
                   
                    else
                    {
                        religionService.InsertReligion(createReligion);
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
            return View(createReligion);
        }

        [HttpGet]
        public ActionResult Edit(int religionID)
        {
            log.Info("ReligionController/Edit/{religionID}");
            try
            {
                Model.Religion objReligion = new Model.Religion();
                objReligion = religionService.GetReligionByID(religionID);
                return View(objReligion);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.Religion editReligion)
        {
            log.Info("ReligionController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editReligion.ReligionName = editReligion.ReligionName.Trim();
                   
                    if (religionService.ReligionNameExists(editReligion.ReligionID, editReligion.ReligionName))
                    {
                        ModelState.AddModelError("ReligionNameAlreadyExist", "Religion Name Already Exist");
                        return View(editReligion);
                    }
                   
                    else
                    {
                        religionService.UpdateReligion(editReligion);
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
            return View(editReligion);

        }


        public ActionResult Delete(int religionID)
        {
            log.Info($"ReligionController/Delete/{religionID}");
            try
            {
                religionService.Delete(religionID);
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