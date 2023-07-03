using Nafed.MicroPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;

namespace MicroPay.Web.Controllers.Form12BB
{
    public class TaxDeductionSectionController : BaseController
    {
        private readonly IForm12BBService form12BBService;

        public TaxDeductionSectionController(IForm12BBService form12BBService)
        {
            this.form12BBService = form12BBService;
        }
        // GET: TaxDeductionSection
        public ActionResult Index()
        {
            log.Info($"TaxDeductionSectionController/Index");
            return View();
        }

        public ActionResult _GetDeductionSections()
        {
            log.Info($"TaxDeductionSectionController/_GetDeductionSections");

            var fYears = ExtensionMethods.GetFinancialYrList(2017, DateTime.Now.Year)
                .Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();

            DeductionSectionVM deductionSectionVM = new DeductionSectionVM();
            deductionSectionVM.deductionSections = form12BBService.GetSectionList();
            deductionSectionVM.fYears = fYears;
            return PartialView("_SectionMaster", deductionSectionVM);
        }

        public ActionResult _GetDeductionSubSections()
        {
            log.Info($"TaxDeductionSectionController/_GetDeductionSubSections");
            try
            {
                DeductionSubSectionVM dSubSectionVM = new DeductionSubSectionVM();
                var fYears = ExtensionMethods.GetFinancialYrList(2017, DateTime.Now.Year)
                  .Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();
                dSubSectionVM.subSectionList = form12BBService.GetSubSectionList();
                dSubSectionVM.fYears = fYears;
                return PartialView("_SubSectionMaster", dSubSectionVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public ActionResult _GetDeductionSubSectionDescription()
        {
            log.Info($"TaxDeductionSectionController/_GetDeductionSubSectionDescription");
            try
            {
                SubSectionDescriptionVM dSubSecDescVM = new SubSectionDescriptionVM();
                var fYears = ExtensionMethods.GetFinancialYrList(2017, DateTime.Now.Year)
                .Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();

                dSubSecDescVM.fYears = fYears;
                dSubSecDescVM.subSectionDescpList = form12BBService.GetSubSectionDescriptions();

                return PartialView("_SubSectionDescriptionMaster", dSubSecDescVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostSectionForm(DeductionSectionVM dSectionVM, string ButtonType)
        {
            log.Info($"TaxDeductionSectionController/_PostSectionForm");
            try
            {
                if (ButtonType == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        dSectionVM.deductionSection.FYear = dSectionVM.selectedFYear;
                        dSectionVM.deductionSection.CreatedBy = userDetail.UserID;
                        dSectionVM.deductionSection.CreatedOn = DateTime.Now;
                        var sectionID = form12BBService.CreateSection(dSectionVM.deductionSection);
                        //  dSectionVM.ResponseText = "Section Created Successfully.";

                        return Json(new
                        {
                            success = "true",
                            msg = "Section Created Successfully.",
                            JsonRequestBehavior.AllowGet
                        });
                    }
                    else
                    {
                        return View("_SectionForm", dSectionVM);
                    }
                }
                else if (ButtonType == "Update")
                {
                    ModelState.Remove("selectedFYear");

                    if (ModelState.IsValid)
                    {
                        /// ==== Code Snippets to update section details ===
                        dSectionVM.deductionSection.UpdatedBy = userDetail.UserID;
                        dSectionVM.deductionSection.UpdatedOn = DateTime.Now;
                        var result = form12BBService.UpdateSection(dSectionVM.deductionSection);
                        if (result)
                            return Json(new
                            {
                                success = "true",
                                msg = "Section updated successfully.",
                                JsonRequestBehavior.AllowGet
                            });
                    }
                    else
                    {
                        return PartialView("_SectionForm", dSectionVM);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return PartialView("_SectionForm", dSectionVM);
        }

        [HttpPost]
        public ActionResult _PostSubSectionForm(DeductionSubSectionVM dSubSectionVM, string ButtonType)
        {
            log.Info($"TaxDeductionSectionController/_PostSubSectionForm");
            try
            {
                if (ButtonType == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        dSubSectionVM.deductionSubSection.CreatedBy = userDetail.UserID;
                        dSubSectionVM.deductionSubSection.CreatedOn = DateTime.Now;
                        dSubSectionVM.deductionSubSection.SectionID = dSubSectionVM.sectionID;

                        var subSectionID = form12BBService.CreateSubSection(dSubSectionVM.deductionSubSection);

                        return Json(new
                        {
                            success = "true",
                            msg = "Sub Section Created Successfully.",
                            JsonRequestBehavior.AllowGet
                        });
                    }
                    else
                    {
                        return PartialView("_SubSectionForm", dSubSectionVM);
                    }
                }
                else if (ButtonType == "Update")
                {
                    ModelState.Remove("");

                    if (ModelState.IsValid)
                    {
                        /// ==== Code Snippets to update section details ===
                        dSubSectionVM.deductionSubSection.UpdatedBy = userDetail.UserID;
                        dSubSectionVM.deductionSubSection.UpdatedOn = DateTime.Now;

                        var result = form12BBService.UpdateSubSection(dSubSectionVM.deductionSubSection);
                        if (result)
                            return Json(new
                            {
                                success = "true",
                                msg = "Sub Section updated successfully.",
                                JsonRequestBehavior.AllowGet
                            });
                    }
                    else
                    {
                        return PartialView("_SubSectionForm", dSubSectionVM);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return PartialView("_SubSectionForm", dSubSectionVM);
        }

        [HttpPost]
        public ActionResult _PostSubSectionDescForm(SubSectionDescriptionVM subSecDescVM, string ButtonType)
        {
            log.Info($"TaxDeductionSectionController/_PostSubSectionDescForm");
            try
            {
                if (ButtonType == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        subSecDescVM.deductionSubSectionDesc.CreatedBy = userDetail.UserID;
                        subSecDescVM.deductionSubSectionDesc.CreatedOn = DateTime.Now;
                        subSecDescVM.deductionSubSectionDesc.SectionID = subSecDescVM.sectionID;
                        subSecDescVM.deductionSubSectionDesc.SubSectionID = subSecDescVM.subSectionID;
                        var subSectionDescriptionID = 
                            form12BBService.CreateSubSectionDescription(subSecDescVM.deductionSubSectionDesc);

                        return Json(new
                        {
                            success = "true",
                            msg = "Sub Section Description Created Successfully.",
                            JsonRequestBehavior.AllowGet
                        });
                    }
                    else
                    {
                        return PartialView("_SubSectionDescriptionForm", subSecDescVM);
                    }
                }
                else if (ButtonType == "Update")
                {
                    ModelState.Remove("");

                    if (ModelState.IsValid)
                    {
                        /// ==== Code Snippets to update section details ===
                        subSecDescVM.deductionSubSectionDesc.UpdatedBy = userDetail.UserID;
                        subSecDescVM.deductionSubSectionDesc.UpdatedOn = DateTime.Now;

                        var result = form12BBService.UpdateSubSectionDescription(subSecDescVM.deductionSubSectionDesc);
                        if (result)
                            return Json(new
                            {
                                success = "true",
                                msg = "Sub Section Description updated successfully.",
                                JsonRequestBehavior.AllowGet
                            });
                    }
                    else
                    {
                        return PartialView("_SubSectionDescriptionForm", subSecDescVM);
                    }
                }
                return PartialView("_SubSectionDescriptionForm", subSecDescVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult DeleteSection(int sectionID)
        {
            log.Info($"TaxDeductionSectionController/DeleteSection/{sectionID}");
            try
            {
                var result = form12BBService.DeleteSection(sectionID);
                TempData["message"] = "Section deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult DeleteSubSection(int sectionID, int subSectionID)
        {
            log.Info($"TaxDeductionSectionController/DeleteSubSection/{sectionID}/{subSectionID}");
            try
            {
                var result = form12BBService.DeleteSubSection(sectionID, subSectionID);
                TempData["message"] = "Sub Section deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult DeleteSubSectionDescription(int sectionID,int subSectionID, int descriptionID)
        {
            log.Info($"TaxDeductionSectionController/DeleteSubSectionDescripttion/{sectionID}/{subSectionID}/{descriptionID}");
            try
            {
                var result = form12BBService.DeleteSubSectionDescription(sectionID, subSectionID, descriptionID);
                TempData["message"] = "Sub Section Description deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }
        public ActionResult _EditSection(int sectionID)
        {
            log.Info($"TaxDeductionSectionController/_EditSection/{sectionID}");
            try
            {
                DeductionSectionVM dSectionVM = new DeductionSectionVM();
                dSectionVM.deductionSection = form12BBService.GetFormSection(sectionID);
                dSectionVM.selectedFYear = dSectionVM.deductionSection.FYear;
                var fYears = ExtensionMethods.GetFinancialYrList(2017, DateTime.Now.Year)
               .Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();
                dSectionVM.fYears = fYears;
                return PartialView("_SectionForm", dSectionVM);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _EditSubSection(int sectionID, int subSectionID)
        {
            log.Info($"TaxDeductionSectionController/_EditSubSection/{sectionID}/{subSectionID}");
            try
            {
                DeductionSubSectionVM dSubSectionVM = new DeductionSubSectionVM();

                var fYears = ExtensionMethods.GetFinancialYrList(2017, DateTime.Now.Year)
               .Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();

                dSubSectionVM.fYears = fYears;
                dSubSectionVM.deductionSubSection = form12BBService.GetFormSubSection(sectionID, subSectionID);

                var section = form12BBService.GetFormSection(sectionID);
                dSubSectionVM.selectedFYear = section.FYear;

                var sections = form12BBService.GetSectionList(section.FYear);
                sections.Insert(0, new SelectListModel { id = 0, value = "Select" });

                dSubSectionVM.sections = sections;
                return PartialView("_SubSectionForm", dSubSectionVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public ActionResult _EditSubSectionDescription(int sectionID, int subSectionID , int descriptionID)
        {
            log.Info($"TaxDeductionSectionController/_EditSubSectionDescription/{sectionID}/{subSectionID}/{descriptionID}");
            try
            {
                SubSectionDescriptionVM dSubSecDescVM = new SubSectionDescriptionVM();
                var fYears = ExtensionMethods.GetFinancialYrList(2017, DateTime.Now.Year)
               .Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();
                dSubSecDescVM.fYears = fYears;
                dSubSecDescVM.deductionSubSectionDesc = form12BBService.GetFormSubSecDesc(sectionID, subSectionID, descriptionID);

                var sectionDtls = form12BBService.GetFormSection(sectionID);
                dSubSecDescVM.selectedFYear = sectionDtls.FYear;

                var sections = form12BBService.GetSectionList(sectionDtls.FYear);
                sections.Insert(0, new SelectListModel { id = 0, value = "Select" });

                var subSections = form12BBService.GetSubSections(sectionID);
                subSections.Insert(0, new SelectListModel { id = 0, value = "Select" });
                
                dSubSecDescVM.sections = sections;
                dSubSecDescVM.subSections = subSections;

                return PartialView("_SubSectionDescriptionForm", dSubSecDescVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }
        public JsonResult GetSectionsByFY(string fYr)
        {
            log.Info($"TaxDeductionSectionController/GetSectionsByFY/{fYr}");
            try
            {
                var sections = form12BBService.GetSectionList(fYr);
                sections.Insert(0, new SelectListModel { id = 0, value = "Select" });
                return Json(sections);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public JsonResult GetSubSectionBySectionID(int sectionID)
        {
            log.Info($"TaxDeductionSectionController/GetSectionsByFY/{sectionID}");
            try
            {
                var subSections = form12BBService.GetSubSections(sectionID);
                subSections.Insert(0, new SelectListModel { id = 0, value = "Select" });
                return Json(subSections);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public JsonResult GetSubSectionDescriptions(int sectionID, int subSectionID)
        {
            log.Info($"TaxDeductionSectionController/GetSubSectionDescriptions/sectionID:{sectionID}/subSectionID:{subSectionID}");
            try
            {
                var subSectionDescription = form12BBService.GetSubSectionDescriptions(sectionID, subSectionID);
                subSectionDescription.Insert(0, new SelectListModel { id = 0, value = "Select" });
                return Json(subSectionDescription);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}