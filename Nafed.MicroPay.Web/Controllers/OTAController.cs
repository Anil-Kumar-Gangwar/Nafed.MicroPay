using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;

namespace MicroPay.Web.Controllers
{
    public class OTAController : BaseController
    {
        private readonly IOTAService oTAServiceServic;
        // GET: OTA

        public OTAController(IOTAService oTAServiceServic)
        {
            this.oTAServiceServic = oTAServiceServic;

        }
        public ActionResult Index()
        {
            log.Info($"OTAController/Index");
            return View(userAccessRight);

        }
        public ActionResult OTAGridView()
        {
            log.Info($"OTAController/BankRatesGridView");
            try
            {
                OTAViewModel oTAVM = new OTAViewModel();
                oTAVM.OTAList = oTAServiceServic.GetOTAList();
                oTAVM.userRights = userAccessRight;
                return PartialView("_OTAGridView", oTAVM);
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
            log.Info("OTAController/Create");
            try
            {
                List<SelectListModel> fromPay = new List<SelectListModel>();
                fromPay.Add(new SelectListModel() { value = "200-500", id = 1 });
                fromPay.Add(new SelectListModel() { value = "600-1000", id = 2 });
                fromPay.Add(new SelectListModel() { value = "1100-2000", id = 3 });

                List<SelectListModel> toPay = new List<SelectListModel>();
                toPay.Add(new SelectListModel() { value = "2000-5000", id = 1 });
                toPay.Add(new SelectListModel() { value = "6000-10000", id = 2 });
                toPay.Add(new SelectListModel() { value = "11000-20000", id = 3 });

                OTA objOTA = new OTA();
                objOTA.FromPayList = fromPay;
                objOTA.ToPayList = toPay;
                return View(objOTA);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(OTA createOTA)
        {
            log.Info("OTAController/Create");
            try
            {
                if (ModelState.IsValid)
                {

                    createOTA.CreatedBy = userDetail.UserID;
                    createOTA.CreatedOn = DateTime.Now;
                    createOTA.IsDeleted = false;
                    if (oTAServiceServic.OTAExists((int)createOTA.OTACode))
                    {
                        List<SelectListModel> fromPay = new List<SelectListModel>();
                        fromPay.Add(new SelectListModel() { value = "200-500", id = 1 });
                        fromPay.Add(new SelectListModel() { value = "600-1000", id = 2 });
                        fromPay.Add(new SelectListModel() { value = "1100-2000", id = 3 });

                        List<SelectListModel> toPay = new List<SelectListModel>();
                        toPay.Add(new SelectListModel() { value = "2000-5000", id = 1 });
                        toPay.Add(new SelectListModel() { value = "6000-10000", id = 2 });
                        toPay.Add(new SelectListModel() { value = "11000-20000", id = 3 });

                        // OTA objOTA = new OTA();
                        createOTA.FromPayList = fromPay;
                        createOTA.ToPayList = toPay;

                        ModelState.AddModelError("OTACodeAlreadyExist", "OTA Code Already Exist");
                        return View(createOTA);
                    }
                    else
                    {
                        oTAServiceServic.InsertOTA(createOTA);
                        TempData["Message"] = "Successfully Created";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    List<SelectListModel> fromPay = new List<SelectListModel>();
                    fromPay.Add(new SelectListModel() { value = "200-500", id = 1 });
                    fromPay.Add(new SelectListModel() { value = "600-1000", id = 2 });
                    fromPay.Add(new SelectListModel() { value = "1100-2000", id = 3 });

                    List<SelectListModel> toPay = new List<SelectListModel>();
                    toPay.Add(new SelectListModel() { value = "2000-5000", id = 1 });
                    toPay.Add(new SelectListModel() { value = "6000-10000", id = 2 });
                    toPay.Add(new SelectListModel() { value = "11000-20000", id = 3 });

                    // OTA objOTA = new OTA();
                    createOTA.FromPayList = fromPay;
                    createOTA.ToPayList = toPay;

                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(createOTA);
        }

        [HttpGet]
        public ActionResult Edit(int oTACode)
        {
            log.Info("OTAController/Edit");
            try
            {
                List<SelectListModel> fromPay = new List<SelectListModel>();
                fromPay.Add(new SelectListModel() { value = "200-500", id = 1 });
                fromPay.Add(new SelectListModel() { value = "600-1000", id = 2 });
                fromPay.Add(new SelectListModel() { value = "1100-2000", id = 3 });

                List<SelectListModel> toPay = new List<SelectListModel>();
                toPay.Add(new SelectListModel() { value = "2000-5000", id = 1 });
                toPay.Add(new SelectListModel() { value = "6000-10000", id = 2 });
                toPay.Add(new SelectListModel() { value = "11000-20000", id = 3 });

                OTA objOTA = new OTA();              
                objOTA = oTAServiceServic.GetOTAyNoOTACode(oTACode);
                objOTA.FromPayList = fromPay;
                objOTA.ToPayList = toPay;
                return View(objOTA);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(OTA editOTA)
        {
            log.Info("OTAController/Edit");
            try
            {
                if (ModelState.IsValid)
                {

                    editOTA.UpdatedBy = userDetail.UserID;
                    editOTA.UpdatedOn = DateTime.Now.Date;
                    editOTA.IsDeleted = false;
                    oTAServiceServic.UpdateOTA(editOTA);
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
            return View(editOTA);

        }

        public ActionResult Delete(int oTACode)
        {
            log.Info("OTAController/Delete");
            try
            {
                oTAServiceServic.Delete(oTACode);
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