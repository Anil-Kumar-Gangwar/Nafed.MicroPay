using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.IO;

namespace MicroPay.Web.Controllers
{
    public class AcadmicProfessionalDetailsController : BaseController
    {
        private readonly IAcadmicProfessionalDetails acadmicProfessionalDetailsService;
        private readonly IDropdownBindService ddlService;
        public AcadmicProfessionalDetailsController(IAcadmicProfessionalDetails acadmicProfessionalDetailsService, IDropdownBindService ddlService)
        {
            this.acadmicProfessionalDetailsService = acadmicProfessionalDetailsService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            log.Info($"AcadmicProfessionalDetailsController/Index");
            BindDropdowns();
            return View(userAccessRight);
        }
        [HttpPost]
        public ActionResult AcadmicProfessionalGridView(FormCollection formCollection)
        {
            log.Info($"AcadmicProfessionalDetailsController/AcadmicProfessionalGridView");
            try
            {
                AcadmicProfessionalModel AcadmicProfessionalVM = new AcadmicProfessionalModel();

                int? typeID = (Convert.ToInt32(formCollection["ddlAcadmicType"]) == 0 ? (int?)null : Convert.ToInt32(formCollection["ddlAcadmicType"]));
                AcadmicProfessionalVM.AcadmicProfessionalList = acadmicProfessionalDetailsService.GetAcadmicProfessionalList(typeID);
                AcadmicProfessionalVM.userRights = userAccessRight;
                return PartialView("_AcadmicProfessionalGridView", AcadmicProfessionalVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {
            List<Model.SelectListModel> selectType = new List<Model.SelectListModel>();
            selectType.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            selectType.Add(new Model.SelectListModel() { value = "Academic", id = 1 });
            selectType.Add(new Model.SelectListModel() { value = "Professional", id = 2 });
            ViewBag.selectType = new SelectList(selectType, "id", "value");
        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info("AcadmicProfessionalDetailsController/Create");
            try
            {
                BindDropdowns();
                Model.AcadmicProfessionalDetailsModel objAcadmicProfessionalDetails = new Model.AcadmicProfessionalDetailsModel();
                return View(objAcadmicProfessionalDetails);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Model.AcadmicProfessionalDetailsModel createAcadmicProfessionalDetails)
        {
            log.Info("AcadmicProfessionalDetailsController/Create");
            try
            {
                BindDropdowns();
                if (createAcadmicProfessionalDetails.TypeID == 0)
                    ModelState.AddModelError("TypeRequired", "Select Type");

                if (ModelState.IsValid)
                {
                    createAcadmicProfessionalDetails.Value = createAcadmicProfessionalDetails.Value.Trim();
                    createAcadmicProfessionalDetails.CreatedBy = userDetail.UserID;
                    createAcadmicProfessionalDetails.CreatedOn = DateTime.Now;
                    if (acadmicProfessionalDetailsService.AcadmicProfessionalDetailsExists(createAcadmicProfessionalDetails.ID, createAcadmicProfessionalDetails.Value, createAcadmicProfessionalDetails.TypeID)) 
                        ModelState.AddModelError("ValueAlreadyExist", "Value Already Exist");
                    else
                    {
                        int acadmicProfessionalID = acadmicProfessionalDetailsService.InsertAcadmicProfessionalDetails(createAcadmicProfessionalDetails);
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
            return View(createAcadmicProfessionalDetails);
        }

        [HttpGet]
        public ActionResult Edit(int acadmicProfessionalDetailsID)
        {
            log.Info("AcadmicProfessionalDetailsController/Edit");
            try
            {
                BindDropdowns();
                Model.AcadmicProfessionalDetailsModel objAcadmicProfessionalDetails = new Model.AcadmicProfessionalDetailsModel();
                objAcadmicProfessionalDetails = acadmicProfessionalDetailsService.GetAcadmicProfessionalDetailsByID(acadmicProfessionalDetailsID);
                return View(objAcadmicProfessionalDetails);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Model.AcadmicProfessionalDetailsModel createAcadmicProfessionalDetails)
        {
            log.Info("AcadmicProfessionalDetailsController/Edit");
            try
            {
                BindDropdowns();

                if (createAcadmicProfessionalDetails.TypeID == 0)
                    ModelState.AddModelError("TypeRequired", "Select Type");
                if (ModelState.IsValid)
                {
                    createAcadmicProfessionalDetails.Value = createAcadmicProfessionalDetails.Value.Trim();

                    if (acadmicProfessionalDetailsService.AcadmicProfessionalDetailsExists(createAcadmicProfessionalDetails.ID, createAcadmicProfessionalDetails.Value, createAcadmicProfessionalDetails.TypeID))
                        ModelState.AddModelError("ValueAlreadyExist", "Value Already Exist");
                    else
                    {

                        acadmicProfessionalDetailsService.UpdateAcadmicProfessionalDetails(createAcadmicProfessionalDetails);

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
            return View(createAcadmicProfessionalDetails);

        }

        public ActionResult Delete(int acadmicProfessionalDetailsId)
        {
            log.Info("AcadmicProfessionalDetailsController/Delete");
            try
            {
                acadmicProfessionalDetailsService.Delete(acadmicProfessionalDetailsId);
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