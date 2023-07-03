using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Common;
using Model = Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Services.Budget;

namespace MicroPay.Web.Controllers.Budget
{
    public class StaffBudgetController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IBudgetService budgetService;
        private readonly IDesignationService designationService;

        public StaffBudgetController(IDropdownBindService ddlService, IBudgetService budgetService, IDesignationService designationService)
        {
            this.ddlService = ddlService;
            this.budgetService = budgetService;
            this.designationService = designationService;
        }
        public ActionResult Index()
        {
            log.Info($"StaffBudgetController/Index");
            try
            {
                StaffBudgetViewModel SBVM = new StaffBudgetViewModel();
                var reportingYr = ExtensionMethods.GetBudgetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x.Key, Value = x.Value }).OrderByDescending(x => x.Value).ToList();
                SBVM.yearList = reportingYr;
                SBVM.designationList = ddlService.ddlDesignationList();
                return View(SBVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult StaffBudgetGridView(StaffBudgetViewModel SBVM)
        {
            log.Info($"StaffBudgetController/StaffBudgetGridView");
            try
            {
                StaffBudgetViewModel staffBudgetDetails = new StaffBudgetViewModel();
                ModelState.Remove("selectYearID");             
                staffBudgetDetails.listStaffBudget = budgetService.GetStaffBudgetList(SBVM.selectYearID, SBVM.designationID);
                return PartialView("_StaffBudgetGridView", staffBudgetDetails);
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
            log.Info("StaffBudgetController/Create");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult Create(Model.StaffBudget objStaffBudeget)
        {
            log.Info("StaffBudgetController/Create");
            try
            {
                BindDropdowns();
                if (ModelState.IsValid)
                {
                    if (!budgetService.StaffBudgetExist(objStaffBudeget))
                    {
                        objStaffBudeget.CreatedBy = userDetail.UserID;
                        int createStaffBudget = budgetService.SaveStaffBudget(objStaffBudeget);
                        if (createStaffBudget > 0)
                        {
                            TempData["Message"] = "Created successfully";
                            return Json(new { url = Url.Action("Index") });
                        }
                        else
                            return PartialView("_CreateStaffBudget", objStaffBudeget);
                    }
                    else
                    {
                        objStaffBudeget.AlertMessage = "Record already exists";
                        return PartialView("_CreateStaffBudget", objStaffBudeget);
                    }
                }
                else
                {
                    return PartialView("_CreateStaffBudget", objStaffBudeget);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public PartialViewResult _CreateStaffBudget()
        {
            log.Info("StaffBudgetController/_CreateStaffBudget");
            try
            {
                BindDropdowns();
                Model.StaffBudget objStaffBudeget = new Model.StaffBudget();
                return PartialView(objStaffBudeget);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int staffBudgetId, int designationId)
        {
            log.Info("StaffBudgetController/Edit");
            try
            {
                BindDropdowns();
                Model.StaffBudget objStaffBudget = new Model.StaffBudget();
                objStaffBudget.StaffBudgetId = staffBudgetId;
                objStaffBudget.DesignationID = designationId;
                return View(objStaffBudget);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.StaffBudget staffBudeget)
        {
            log.Info("StaffBudgetController/Edit");
            try
            {
                BindDropdowns();
                if (ModelState.IsValid)
                {
                    staffBudeget.UpdatedBy = userDetail.UserID;
                    bool updateStaffBudget = budgetService.UpdateStaffBudget(staffBudeget);
                    if (updateStaffBudget)
                    {
                        TempData["Message"] = "Updated successfully.";
                        return Json(new { url = Url.Action("Index") });
                        //return RedirectToAction("Index");
                    }
                    else
                        return PartialView("_EditStaffBudget", staffBudeget);
                }
                else
                {
                    return PartialView("_EditStaffBudget", staffBudeget);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _EditStaffBudget(Model.StaffBudget staffBudeget)
        {
            log.Info("StaffBudgetController/_EditStaffBudget");
            try
            {
                ModelState.Clear();
                BindDropdowns();
                Model.StaffBudget objStaffBudeget = new Model.StaffBudget();
                objStaffBudeget = budgetService.GetStaffBudgetById(staffBudeget.StaffBudgetId);
                objStaffBudeget.TotalStaffStrenght = objStaffBudeget.BudgetedStaff;
                var promotiondetails = designationService.GetDesignationByID(staffBudeget.DesignationID.Value);
                objStaffBudeget.PromotionPer = Convert.ToInt32(promotiondetails.Promotion);
                objStaffBudeget.LTCPerc = Convert.ToInt32(promotiondetails.LCT);
                objStaffBudeget.DirectPerc = Convert.ToInt32(promotiondetails.Direct);
                objStaffBudeget.StaffBudgetId = staffBudeget.StaffBudgetId;
                objStaffBudeget.DesignationID = staffBudeget.DesignationID;
                return PartialView(objStaffBudeget);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetPresentStaff(Model.StaffBudget objStaffBudeget)
        {
            log.Info("StaffBudgetController/GetPresentStaff");
            try
            {
                ModelState.Clear();
                BindDropdowns();
                var presentStaffCount = budgetService.GetPresentStaff(objStaffBudeget.DesignationID);
                objStaffBudeget.PresentStaff = Convert.ToInt16(presentStaffCount);
                return PartialView("_CreateStaffBudget", objStaffBudeget);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult CalculateVacancies(Model.StaffBudget objStaffBudeget)
        {
            log.Info("StaffBudgetController/GetPresentStaff");
            try
            {
                BindDropdowns();
                int intMin = 0;
                int intPromotion = 0, intLCT = 0, intDirect = 0;
                objStaffBudeget.VRS = objStaffBudeget.VRS == null ? 0 : objStaffBudeget.VRS;
                objStaffBudeget.TotalStaffStrenght = Convert.ToInt16((objStaffBudeget.BudgetedStaff) - (objStaffBudeget.VRS));
                objStaffBudeget.Vac = Convert.ToInt16((objStaffBudeget.TotalStaffStrenght) - (objStaffBudeget.PresentStaff));
                var promotiondetails = designationService.GetDesignationByID(objStaffBudeget.DesignationID.Value);
                objStaffBudeget.PromotionPer = Convert.ToInt32(promotiondetails.Promotion);
                objStaffBudeget.LTCPerc = Convert.ToInt32(promotiondetails.LCT);
                objStaffBudeget.DirectPerc = Convert.ToInt32(promotiondetails.Direct);
                int vacancies = Convert.ToInt32(objStaffBudeget.Vac);

                if (objStaffBudeget.PromotionPer.Value <= objStaffBudeget.LTCPerc.Value && objStaffBudeget.LTCPerc.Value <= objStaffBudeget.DirectPerc.Value)
                {
                    intMin = objStaffBudeget.PromotionPer.Value;
                    if (intMin == 0)
                        intMin = objStaffBudeget.LTCPerc.Value;
                    if (intMin == 0)
                        intMin = objStaffBudeget.DirectPerc.Value;
                }
                else if (objStaffBudeget.LTCPerc.Value <= objStaffBudeget.DirectPerc.Value && objStaffBudeget.DirectPerc.Value <= objStaffBudeget.PromotionPer.Value)
                {
                    intMin = objStaffBudeget.LTCPerc.Value;
                    if (intMin == 0)
                        intMin = objStaffBudeget.DirectPerc.Value;
                    if (intMin == 0)
                        intMin = objStaffBudeget.PromotionPer.Value;
                }
                else
                {
                    intMin = objStaffBudeget.DirectPerc.Value;
                    if (intMin == 0)
                    {
                        if (objStaffBudeget.PromotionPer.Value <= objStaffBudeget.LTCPerc.Value)
                        {
                            intMin = objStaffBudeget.PromotionPer.Value;
                            if (intMin == 0)
                                intMin = objStaffBudeget.LTCPerc.Value;
                        }
                        else
                        {
                            intMin = objStaffBudeget.LTCPerc.Value;
                            if (intMin == 0)
                                intMin = objStaffBudeget.PromotionPer.Value;
                        }
                    }
                }

                if (intMin != 0)
                {
                    int intPromotionProp = 0, intLCTProp = 0, intDirectProp = 0;
                    int j = 0, k = 0, l = 0, z = 0;

                    intPromotionProp = (objStaffBudeget.PromotionPer.Value / intMin);
                    intLCTProp = (objStaffBudeget.LTCPerc.Value / intMin);
                    intDirectProp = (objStaffBudeget.DirectPerc.Value / intMin);
                    for (int i = 0; i <= vacancies - 1; i++)
                    {
                        z = i;
                        for (j = 1; j <= intPromotionProp; j++)
                        {
                            if (z <= vacancies - 1)
                                intPromotion = intPromotion + 1;
                            else
                                break;
                            z = z + 1;
                        }
                        i = intPromotion + intLCT + intDirect;
                        z = i;
                        for (k = 1; k <= intLCTProp; k++)
                        {
                            if (z <= vacancies - 1)
                                intLCT = intLCT + 1;
                            else
                                break;
                            z = z + 1;
                        }
                        i = intPromotion + intLCT + intDirect;
                        z = i;
                        for (l = 1; l <= intDirectProp; l++)
                        {
                            if (z <= vacancies - 1)
                                intDirect = intDirect + 1;
                            else
                                break;
                            z = z + 1;
                        }
                        i = intPromotion + intLCT + intDirect;
                        i = i - 1;
                    }
                }
                else
                {
                    intPromotion = vacancies;
                }

                objStaffBudeget.FPromotion = Convert.ToInt16(intPromotion);
                objStaffBudeget.FLTC = Convert.ToInt16(intLCT);
                objStaffBudeget.FDirect = Convert.ToInt16(intDirect);
                if (objStaffBudeget.Event == "C")
                {
                    ModelState.Clear();
                    return PartialView("_CreateStaffBudget", objStaffBudeget);
                }
                else
                {
                    ModelState.Clear();
                    return PartialView("_EditStaffBudget", objStaffBudeget);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult Delete(int staffBudgetId)
        {
            log.Info($"StaffBudgetController/Delete");
            try
            {
                budgetService.Delete(staffBudgetId);
                TempData["Message"] = "Succesfully deleted";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        public void BindDropdowns()
        {
            var ddlDesignationList = ddlService.ddlDesignationList();
            Model.SelectListModel selectDesignation = new Model.SelectListModel();
            selectDesignation.id = 0;
            selectDesignation.value = "Select";
            ddlDesignationList.Insert(0, selectDesignation);
            ViewBag.DesignationList = new SelectList(ddlDesignationList, "id", "value");
            var reportingYr = ExtensionMethods.GetBudgetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x.Key, Value = x.Value }).OrderByDescending(x => x.Value).ToList();
            ViewBag.ddlYear = new SelectList(reportingYr, "Value", "Text");
            List<Model.SelectListModel> staffMonth = new List<Model.SelectListModel>();
            staffMonth.Add(new Model.SelectListModel
            { value = "Select", id = 0 });
            staffMonth.Add(new Model.SelectListModel
            { value = "January", id = 1 });
            staffMonth.Add(new Model.SelectListModel
            { value = "July", id = 7 });
            ViewBag.StaffMonth = new SelectList(staffMonth, "id", "value");
        }

        [HttpGet]
        public ActionResult GenerateStaffBudget()
        {
            log.Info($"StaffBudgetController/GenerateStaffBudget");
            try
            {
                StaffBudgetViewModel genrateStaff = new StaffBudgetViewModel();
                var reportingYr = ExtensionMethods.GetBudgetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x.Key, Value = x.Value }).OrderByDescending(x => x.Value).ToList();
                genrateStaff.yearList = reportingYr;
                return PartialView("_GenrateStaffBudget", genrateStaff);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GenerateStaffBudget(StaffBudgetViewModel generateStaffBudget)
        {
            log.Info($"StaffBudgetController/GenerateStaffBudget");
            try
            {
                if (ModelState.IsValid)
                {
                    var staffBudgetRecord = budgetService.GetStaffBudgetList(generateStaffBudget.genrateYearID, null);
                    if (staffBudgetRecord.Count == 0)
                    {
                        var insertValue = budgetService.GenerateStaffBudget(generateStaffBudget.genrateYearID, generateStaffBudget.selectYearID);
                        return Json(new { success = "Records generated successully." });
                    }
                    else
                    {
                        return Json(new { warning = "Records are exist" });
                    }
                }
                else
                {
                    var reportingYr = ExtensionMethods.GetBudgetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x.Key, Value = x.Value }).OrderByDescending(x => x.Value).ToList();
                    generateStaffBudget.yearList = reportingYr;
                    //return PartialView("_GenrateStaffBudget", generateStaffBudget);
                    return Json(new { htmlData = ConvertViewToString("_GenrateStaffBudget", generateStaffBudget) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

    }
}