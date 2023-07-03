using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;

namespace MicroPay.Web.Controllers.Approval
{
    public class PayrollApprovalController : BaseController
    {
        // GET: PayrollApproval
        private readonly IDropdownBindService ddlService;
        private readonly IPayrollApprovalSettingService approvalSettting;

        public PayrollApprovalController(IDropdownBindService ddlService, IPayrollApprovalSettingService approvalSettting)
        {
            this.ddlService = ddlService;
            this.approvalSettting = approvalSettting;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult _GetApprovalSetting()
        {
            log.Info($"PayrollApprovalController/_GetApprovalSetting");

            try
            {
                PayrollApprovalSettingVM approvalSettingVM = new PayrollApprovalSettingVM();

                var approvalSetting = approvalSettting.GetApprovalSetting().ToList();

                approvalSettingVM.employees = ddlService.GetAllEmployee();
            //    approvalSettingVM.employees.Insert(0, new SelectListModel { id = 0, value = "Select" });

                var ddlEmployee = ddlService.GetAllEmployee();
                SelectListModel selectEmployee = new SelectListModel();
              //  selectEmployee.id = 0;
              //  selectEmployee.value = "Select";
                //  ddlEmployee.RemoveAll(x => x.id == employeeID);
                //ddlEmployee.Insert(0, selectEmployee);
                ViewBag.Employee = new SelectList(ddlEmployee, "id", "value");

                approvalSettingVM.approvalSetting = approvalSetting;
                return PartialView("_ApprovalProcess", approvalSettingVM);
            }
            catch (Exception)
            {
                throw;
            }


        }


        [HttpPost]
        public ActionResult _PostApprovalSetting(PayrollApprovalSettingVM pAppSettingVM)
        {
            log.Info($"PayrollApprovalController/_PostApprovalSetting");
            try
            {
                if (ModelState.IsValid)
                {
                    if (!pAppSettingVM.approvalSetting.Any(x => x.ProcessAppID > 0))
                    {
                        pAppSettingVM.approvalSetting.ForEach(ps =>
                        {
                            ps.CreatedBy = userDetail.UserID;
                            ps.CreatedOn = DateTime.Now;
                            ps.FromDate = DateTime.Now;
                            ps.Reporting2 = ps.Reporting2 == 0 ? 0 : ps.Reporting2;
                            ps.Reporting3 = ps.Reporting3 == 0 ? 0 : ps.Reporting3;
                        });
                        var result = approvalSettting.InsertPayrollApprovalSetting(pAppSettingVM.approvalSetting);
                        if (result)
                            return Json(new { part = 0, msgType = "success", msg = "Payroll process approval successfully configured." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                       PayrollApprovalSettingVM newPayrollApprovalVM = new PayrollApprovalSettingVM();
                        newPayrollApprovalVM.approvalSetting = new List<PayrollApprovalSetting>();
                        var result = false;
                        pAppSettingVM.approvalSetting.ForEach(ps =>
                        {
                            if (ps.OldReporting1 == ps.Reporting1 && ps.OldReporting2 == ps.Reporting2 
                            && ps.OldReporting3 == ps.Reporting3)
                            {

                            }
                            else
                            {
                                newPayrollApprovalVM.approvalSetting.Add(new PayrollApprovalSetting()
                                {
                                    Reporting3 = ps.Reporting3,
                                    Reporting1 = ps.Reporting1,
                                    Reporting2 = ps.Reporting2,
                                    ProcessAppID = ps.ProcessAppID,
                                    ProcessID = ps.ProcessID,
                                    ToDate = DateTime.Now,
                                  
                                    UpdatedBy = userDetail.UserID,
                                    UpdatedOn = DateTime.Now,
                                    CreatedBy = userDetail.UserID,
                                    CreatedOn = DateTime.Now,
                                    FromDate = DateTime.Now
                                });
                            }

                        });
                        if (newPayrollApprovalVM.approvalSetting.Count > 0)
                        {
                            result = approvalSettting.UpdatePayrollApprovalSetting(newPayrollApprovalVM.approvalSetting);
                        }
                        if (result)
                        {
                            //RedirectToAction("_GetEmpProcessApproval", new { employeeID = newempVM.empProcessApp[0].EmployeeID, userTypeID = 0 });
                            return Json(new {
                                part = 0, msgType = "success", msg = "Payroll process approval successfully configured."
                            }, JsonRequestBehavior.AllowGet);

                        }
                        else {
                            pAppSettingVM.employees = ddlService.GetAllEmployee();
                            pAppSettingVM.employees.Insert(0, new SelectListModel { id = 0, value = "Select" });
                        }
                    }
                }
                else
                {
                    pAppSettingVM.employees = ddlService.GetAllEmployee();
                    pAppSettingVM.employees.Insert(0, new SelectListModel { id = 0, value = "Select" });
                }
                return PartialView("_ApprovalProcess", pAppSettingVM);


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