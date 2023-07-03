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
    public class LoanSlabController : BaseController
    {
        // GET: LoanSlab
        private readonly ILoanSlab loanSlabService;
        private readonly IDropdownBindService ddlServices;
        public LoanSlabController(ILoanSlab loanSlabService, IDropdownBindService ddlServices)
        {
            this.loanSlabService = loanSlabService;
            this.ddlServices = ddlServices;

        }
        public ActionResult Index()
        {
            log.Info($"LoanSlabController/Index");
            return View(userAccessRight);

        }
        public ActionResult LoanSlabGridView()
        {
            log.Info($"LoanSlabController/BankRatesGridView");
            try
            {
                LoanSlabViewModel LoanSlabVM = new LoanSlabViewModel();
                LoanSlabVM.LoanSlabList = loanSlabService.GetLoabSlabList();
                LoanSlabVM.userRights = userAccessRight;
                return PartialView("_LoanSlabGridView", LoanSlabVM);
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
            log.Info("LoanSlabController/Create");
            try
            {
                //List<SelectListModel> LoanTypeData = new List<SelectListModel>();
                //LoanTypeData.Add(new SelectListModel() { value = "House Loan", id = 1 });
                //LoanTypeData.Add(new SelectListModel() { value = "Car Loan", id = 2 });
                //LoanTypeData.Add(new SelectListModel() { value = "Personal Loan", id = 3 });
                
                LoanSlab objLoanSlab = new LoanSlab();
                objLoanSlab.EffectiveDate = DateTime.Now;
                objLoanSlab.LoanTypeList = ddlServices.GetLoanType();
                return View(objLoanSlab);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(LoanSlab createLoanSlab)
        {
            log.Info("LoanSlabController/Create");
            try
            {
                if (ModelState.IsValid)
                {

                    createLoanSlab.CreatedBy = userDetail.UserID;
                    createLoanSlab.CreatedOn = DateTime.Now;
                    createLoanSlab.IsDeleted = false;
                    if (loanSlabService.LoabSlabExists(createLoanSlab.SlabNo))
                    {
                        ModelState.AddModelError("LoanSlabAlreadyExist", "Loan Slab Already Exist");

                        //List<SelectListModel> LoanTypeData = new List<SelectListModel>();
                        //LoanTypeData.Add(new SelectListModel() { value = "House Loan", id = 1 });
                        //LoanTypeData.Add(new SelectListModel() { value = "Car Loan", id = 2 });
                        //LoanTypeData.Add(new SelectListModel() { value = "Personal Loan", id = 3 });
                        createLoanSlab.LoanTypeList = ddlServices.GetLoanType(); 
                        return View(createLoanSlab);
                    }
                    else
                    {
                        loanSlabService.InsertLoabSlab(createLoanSlab);
                        TempData["Message"] = "Successfully Created";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    
                    createLoanSlab.LoanTypeList = ddlServices.GetLoanType();
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(createLoanSlab);
        }

        [HttpGet]
        public ActionResult Edit(int slabNo)
        {
            log.Info("LoanSlabController/Edit");
            try
            {
                //List<SelectListModel> LoanTypeData = new List<SelectListModel>();
                //LoanTypeData.Add(new SelectListModel() { value = "House Loan", id = 1 });
                //LoanTypeData.Add(new SelectListModel() { value = "Car Loan", id = 2 });
                //LoanTypeData.Add(new SelectListModel() { value = "Personal Loan", id = 3 });

                LoanSlab objLoanSlab = new LoanSlab();                
                objLoanSlab = loanSlabService.GetLoabSlabbyNo(slabNo);
                objLoanSlab.LoanTypeList = ddlServices.GetLoanType();
                return View(objLoanSlab);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(LoanSlab editLoanSlab)
        {
            log.Info("LoanSlabController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                   
                    editLoanSlab.UpdatedBy = userDetail.UserID;
                    editLoanSlab.UpdatedOn = DateTime.Now.Date;
                    editLoanSlab.IsDeleted = false;
                    loanSlabService.UpdateLoabSlab(editLoanSlab);
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
            return View(editLoanSlab);

        }

        public ActionResult Delete(int slabNo,string loanType, DateTime effectiveDate)
        {
            log.Info("LoanSlabController/Delete");
            try
            {
                loanSlabService.Delete(slabNo, loanType, effectiveDate);
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