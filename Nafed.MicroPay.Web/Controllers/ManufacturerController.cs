using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
namespace MicroPay.Web.Controllers
{
    public class ManufacturerController : BaseController
    {
        private readonly IManufacturerService ManufacturerService;

        public ManufacturerController(IManufacturerService ManufacturerService)
        {
            this.ManufacturerService = ManufacturerService;
        }
        // GET: Manufacturer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManufacturerGridView()
        {
            log.Info($"ManufacturerController/ManufacturerGridView");
            try
            {
               List<Manufacturer> Manufacturer = new List<Manufacturer>();
                Manufacturer = ManufacturerService.GetManufacturerList(true);
                return PartialView("_ManufacturerGridView", Manufacturer);
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
            log.Info("ManufacturerController/Create");
            try
            {         
                Manufacturer objManufacturer = new Manufacturer();
                return View(objManufacturer);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

     
        [HttpPost]
        public ActionResult Create(Manufacturer createManufacturer)
        {
            log.Info("ManufacturerController/Create");
            try
            {             
                if (ModelState.IsValid)
                {
                    createManufacturer.ManufacturerName = createManufacturer.ManufacturerName.Trim();
                    if (ManufacturerService.ManufacturerExists(createManufacturer.ManufacturerName))
                    {
                        ModelState.AddModelError("ManufacturerAlreadyExist", "Manufacturer Already Exist");
                    }
                    else
                    {
                        createManufacturer.CreatedBy = userDetail.UserID;
                        createManufacturer.CreatedOn = DateTime.Now;
                        ManufacturerService.InsertManufacturer(createManufacturer);
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
            return View(createManufacturer);
        }

        [HttpGet]
        public ActionResult Edit(int ManufacturerID)
        {
            log.Info("ManufacturerController/Edit");
            try
            {
               
                Manufacturer objManufacturer = new Manufacturer();
                objManufacturer = ManufacturerService.GetManufacturerID(ManufacturerID);
                return View(objManufacturer);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Manufacturer editManufacturer)
        {
            log.Info("ManufacturerController/Edit");
            try
            {
              
                if (ModelState.IsValid)
                {
                    editManufacturer.ManufacturerName = editManufacturer.ManufacturerName.Trim();
                    editManufacturer.UpdatedBy = userDetail.UserID;
                    editManufacturer.UpdatedOn = System.DateTime.Now;
                    //if (ManufacturerService.ManufacturerExists(editManufacturer.Manufacturer1))
                    //{
                    //    ModelState.AddModelError("ManufacturerAlreadyExist", "Manufacturer Already Exist");
                    //}
                    //else
                    //{
                        ManufacturerService.UpdateManufacturer(editManufacturer);
                        TempData["Message"] = "successfully Updated";
                        return RedirectToAction("Index");
                   // }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editManufacturer);
        }

        public ActionResult Delete(int ManufacturerID)
        {
            log.Info("ManufacturerController/Delete");
            try
            {
                ManufacturerService.Delete(ManufacturerID);
                TempData["Message"] = "successfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                if (ex.InnerException.InnerException.HResult == -2146232060)
                    TempData["Error"] = "You can't delete Manufacturer, because it is in used.";
                else
                    TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }
    }
}