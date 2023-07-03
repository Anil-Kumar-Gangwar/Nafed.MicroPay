using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.Salary;
using Nafed.MicroPay.Model;
using AutoMapper;

namespace MicroPay.Web.Controllers.Salary
{
    public class SalaryHeadSlabConfigController : BaseController
    {
        private readonly ISalaryHeadRuleService salaryHeadRules;

        public SalaryHeadSlabConfigController(ISalaryHeadRuleService salaryHeadRules)
        {
            this.salaryHeadRules = salaryHeadRules;
        }
        // GET: SalaryHeadSlabConfig
        public ActionResult Index()
        {

            log.Info($"SalaryHeadSlabConfigController/Index");
            SalaryHeadViewModel sHeadSlabVM = new SalaryHeadViewModel();
            sHeadSlabVM.userRights = userAccessRight;
            sHeadSlabVM.fieldList = salaryHeadRules.GetSalaryHeadFields().Where(x => x.FieldName != "E_Basic");
            return View(sHeadSlabVM);


        }

        [HttpGet]
        public ActionResult _GetSalHeadSlabConfigGridView(string fieldName)
        {
            log.Info($"SalaryHeadSlabConfigController/_GetSalHeadSlabConfigGridView");
            try
            {
                SalaryHeadViewModel sHeadSlabVM = new SalaryHeadViewModel();
                //if (string.IsNullOrEmpty(fieldName))
                //{
                //    ModelState.AddModelError("FieldNameRequired", "Please select Head Name.");
                //}
                //else
                //{
                sHeadSlabVM.userRights = userAccessRight;
                sHeadSlabVM.salarySlabList = salaryHeadRules.GetSalaryHeadSlabList(fieldName);
                //  }
                return PartialView("_SalaryHeadSlabConfigGridView", sHeadSlabVM);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult _GetSalHeadSlabConfigGridView(SalaryHeadViewModel salaryHeadVM)
        {
            log.Info($"SalaryHeadSlabConfigController/_GetSalHeadSlabConfigGridView");
            try
            {
                SalaryHeadViewModel sHeadSlabVM = new SalaryHeadViewModel();
                if (salaryHeadVM.FieldName == null)
                {
                    sHeadSlabVM.userRights = userAccessRight;
                    sHeadSlabVM.salarySlabList = salaryHeadRules.GetSalaryHeadSlabList("");
                }
                else
                {
                    sHeadSlabVM.userRights = userAccessRight;
                    sHeadSlabVM.salarySlabList = salaryHeadRules.GetSalaryHeadSlabList(salaryHeadVM.FieldName);
                }
                return PartialView("_SalaryHeadSlabConfigGridView", sHeadSlabVM);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult _GetSalaryHeadSlabConfigDtls()
        {
            log.Info($"EmpSalaryConfigController/_GetSalaryConfigDtls");
            SalaryHeadSlab salHead = new SalaryHeadSlab();
            var salaryHeadField = salaryHeadRules.GetSalaryHeadFields().Where(x => x.FieldName != "E_Basic");
            List<dynamic> fieldList = new List<dynamic>();
            foreach (var item in salaryHeadField)
            {
                fieldList.Add(new
                {
                    FieldName = item.FieldName,
                    FieldDesc = item.FieldDesc
                });
            }
            salHead.fieldList = fieldList;
            return PartialView("_SalaryHeadSlabConfigPopup", salHead);
        }


        public ActionResult _GetSalaryHeadSlabConfig(int slabID)
        {
            log.Info($"EmpSalaryConfigController/_GetSalaryConfigDtls");
            SalaryHeadSlab salHead = new SalaryHeadSlab();
            salHead.SlabID = slabID;
            var salaryHeadDtls = salaryHeadRules.GetSalaryHeadSlab(salHead);
            salHead = salaryHeadDtls;
            var salaryHeadField = salaryHeadRules.GetSalaryHeadFields();

            List<dynamic> fieldList = new List<dynamic>();
            foreach (var item in salaryHeadField)
            {
                fieldList.Add(new
                {
                    FieldName = item.FieldName,
                    FieldDesc = item.FieldDesc
                });
            }
            salHead.fieldList = fieldList;
            return PartialView("_SalaryHeadSlabConfigPopup", salHead);
        }

        [HttpPost]
        public ActionResult _PostHeadSlabConfig(SalaryHeadSlab salaryHead,FormCollection frm)
        {
            log.Info($"SalaryHeadSlabConfigController/_PostHeadSlabConfig");
            try
            {
                var salaryHeadField = salaryHeadRules.GetSalaryHeadFields().Where(x => x.FieldName != "E_Basic"); ;
                salaryHead.SelectedFieldName = frm.Get("ddlFieldList");
                List<dynamic> fieldList = new List<dynamic>();
                foreach (var item in salaryHeadField)
                {
                    fieldList.Add(new
                    {
                        FieldName = item.FieldName,
                        FieldDesc = item.FieldDesc
                    });
                }

                salaryHead.fieldList = fieldList;


                //salaryHead.SelectedFieldName = Request.Form["ddlFieldList"];
                salaryHead.FieldName = salaryHead.FieldName == null ? salaryHead.SelectedFieldName : salaryHead.FieldName;
                if (salaryHead.FieldName == null)
                {
                    ModelState.AddModelError("FieldNameRequired", "Please select Head Name.");
                    return PartialView("_SalaryHeadSlabConfigPopup", salaryHead);
                }
                else
                {
                    if (salaryHead.LowerRange > salaryHead.UpperRange)
                    {
                        ModelState.AddModelError("RangeError", "Lower Range cannot be greater than Upper Range.");
                        return PartialView("_SalaryHeadSlabConfigPopup", salaryHead);

                    }
                    else if ((salaryHead.LowerRange.HasValue && salaryHead.UpperRange.HasValue) && salaryHead.LowerRange.Value == salaryHead.UpperRange.Value)
                    {
                        ModelState.AddModelError("RangeError", "Lower Range and Upper Range cannot be same.");
                        return PartialView("_SalaryHeadSlabConfigPopup", salaryHead);

                    }
                    else if ((salaryHead.LowerRange != null && salaryHead.UpperRange != null) && salaryHead.Amount == null)
                    {
                        ModelState.AddModelError("AmountNullError", "Amount cannot be blank.");
                        return PartialView("_SalaryHeadSlabConfigPopup", salaryHead);
                    }

                    if (ModelState.IsValid)
                    {
                        bool result;
                        string msg = "";
                        if (salaryHead.CreatedOn == default(DateTime))
                        {
                            if (salaryHeadRules.SalaryHeadSlabExists(salaryHead))
                            {
                                ModelState.AddModelError("SalarySlabExist", "Salary Head Slab Already Exist.");
                                return PartialView("_SalaryHeadSlabConfigPopup", salaryHead);
                            }
                            else
                            {
                                salaryHead.CreatedBy = userDetail.UserID;
                                salaryHead.IsDeleted = false;
                                salaryHead.CreatedOn = DateTime.Now;
                                result = salaryHeadRules.InsertSalaryHeadSlab(salaryHead);
                                msg = "Salary Head Slab successfully created.";
                            }
                        }
                        else
                        {
                            salaryHead.UpdatedBy = userDetail.UserID;
                            salaryHead.UpdatedOn = DateTime.Now;
                            salaryHead.IsDeleted = false;
                            result = salaryHeadRules.UpdateSalaryHeadSlab(salaryHead);
                            msg = "Salary Head Slab successfully updated.";
                        }

                        return Json(new
                        {
                            status = result,
                            fieldName = salaryHead.FieldName,
                            type = "success",
                            msg = msg


                        }, JsonRequestBehavior.AllowGet);
                    }
                    return PartialView("_SalaryHeadSlabConfigPopup", salaryHead);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public ActionResult Delete(int slabID, string fieldName)
        {
            SalaryHeadViewModel sHeadSlabVM = new SalaryHeadViewModel();
            log.Info("SalaryHeadSlabConfigController/Delete");
            try
            {
                SalaryHeadSlab salaryHead = new SalaryHeadSlab();
                salaryHead.SlabID = slabID;
                salaryHead.FieldName = fieldName;
                sHeadSlabVM.userRights = userAccessRight;
                bool result = false;
                result = salaryHeadRules.Delete(salaryHead);
                sHeadSlabVM.salarySlabList = salaryHeadRules.GetSalaryHeadSlabList(salaryHead.FieldName);
                return Json(new
                {
                    status = result,
                    type = "success",
                    msg = "Deleted successfully."

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

    }
}