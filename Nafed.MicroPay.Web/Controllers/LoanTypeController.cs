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
    public class LoanTypeController : BaseController
    {
        private readonly ILoanTypeService loanTypeService;
        public LoanTypeController(ILoanTypeService loanTypeService)
        {
            this.loanTypeService = loanTypeService;
        }
        // GET: LoanType
        public ActionResult Index()
        {
            log.Info($"LoanTypeController/Index");
            return View(userAccessRight);
        }
        public ActionResult LoanTypeGridView()
        {
            log.Info($"LoanTypeController/LoanTypeGridView");
            try
            {
                LoanTypeViewModel loanTypeVM = new LoanTypeViewModel();
                loanTypeVM.LoanTypeList = loanTypeService.GetLoanTypeList();
                loanTypeVM.userRights = userAccessRight;
                return PartialView("_LoanTypeGridView", loanTypeVM);
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
            log.Info("LoanTypeController/Create");
            try
            {
                tblMstLoanType mstLoanType = new tblMstLoanType();
                return View(mstLoanType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(tblMstLoanType createLoanType)
        {
            log.Info("LoanTypeController/Create");

            try
            {
                if (createLoanType.enumLoanPymtType == 0)
                    ModelState.AddModelError("PaymentTypeRequired", "Please select payment type.");

                if (ModelState.IsValid)
                {
                    createLoanType.CreatedBy = userDetail.UserID;
                    createLoanType.CreatedOn = DateTime.Now;
                 //   createLoanSlab.IsDeleted = false;
                    if (loanTypeService.LoanTypeExists(createLoanType.LoanType))
                    {
                        ModelState.AddModelError("LoanTypeAlreadyExist", "Loan Type Already Exist");

                        List<SelectListModel> LoanTypeData = new List<SelectListModel>();
                        LoanTypeData.Add(new SelectListModel() { value = "House Loan", id = 1 });
                        LoanTypeData.Add(new SelectListModel() { value = "Car Loan", id = 2 });
                        LoanTypeData.Add(new SelectListModel() { value = "Personal Loan", id = 3 });
                       // createLoanSlab.LoanTypeList = LoanTypeData;
                        return View(createLoanType);
                    }
                    else
                    {

                        createLoanType.PaymentType = createLoanType.enumLoanPymtType.GetDisplayName();
                        loanTypeService.InsertLoanType(createLoanType);
                        TempData["Message"] = "Successfully Created";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(createLoanType);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
         
        }


        [HttpGet]
        public ActionResult Edit(int loanTypeID)
        {
            log.Info($"LoanTypeController/Edit/{loanTypeID}");
            try
            {
                tblMstLoanType objLoanType = new tblMstLoanType();
                objLoanType = loanTypeService.GetLoanTypeDtls(loanTypeID);

                if (objLoanType.PaymentType == "EMI")
                    objLoanType.enumLoanPymtType = LoanPaymentType.EMI;
                else
                    objLoanType.enumLoanPymtType = LoanPaymentType.Installment;

                return View(objLoanType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(tblMstLoanType editLoanType)
        {
            log.Info("LoanTypeController/Edit/");
            try
            {
                if (editLoanType.enumLoanPymtType == 0)
                    ModelState.AddModelError("PaymentTypeRequired", "Please select payment type.");

                if (ModelState.IsValid)
                {
                    editLoanType.UpdatedBy = userDetail.UserID;
                    editLoanType.UpdatedOn = DateTime.Now.Date;
                    editLoanType.PaymentType = editLoanType.enumLoanPymtType.GetDisplayName();
                   
                    loanTypeService.UpdateLoanType(editLoanType);
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
            return View(editLoanType);

        }

        public ActionResult Delete(int loanTypeID)
        {
            log.Info("LoanTypeController/Delete/{loanTypeID}");
            try
            {
                loanTypeService.Delete(loanTypeID);
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