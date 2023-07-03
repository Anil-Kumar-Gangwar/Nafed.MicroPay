using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers.Reports
{
    public class AchievementCertificationReportController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IEmployeeService employeeService;
        public AchievementCertificationReportController(IDropdownBindService ddlService, IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
            this.ddlService = ddlService;
        }
        // GET: AchievementCertificationReport
        public ActionResult Index()
        {
            log.Info($"AchievementCertificationReport/Index/");
            return View();
        }
        [HttpGet]
        public ActionResult _GetFilters()
        {
            log.Info($"AchievementCertificationReport/_GetFilters/");

            try
            {
                EmpAchievementCertificationReportFilterVM viewModel = new EmpAchievementCertificationReportFilterVM();

                viewModel.employees = ddlService.GetAllEmployee(null);
                return PartialView("_Filter", viewModel);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult _ExportReport(EmpAchievementCertificationReportFilterVM model)
        {

            log.Info($"AchievementCertificationReport/_ExportReport/");
            try
            {
                model.employees = ddlService.GetAllEmployee(null);

                if (ModelState.IsValid)
                {
                    if (model.fromDate.Value > model.toDate.Value)
                    {
                        ModelState.AddModelError("DateCompareValidator", "From date must be less than To date.");
                        return Json(new
                        {
                            message = "error",
                            htmlData = ConvertViewToString("_Filter", model)
                        }, JsonRequestBehavior.AllowGet);

                    }
                    string fullPath = Server.MapPath("~/FileDownload/");
                    string fileName = string.Empty, msg = string.Empty, result = string.Empty;

                    if (model.SelectedReportType == 1)
                        fileName = ExtensionMethods.SetUniqueFileName($"Employee Achievement Report-", FileExtension.xlsx);
                    else
                        fileName = ExtensionMethods.SetUniqueFileName($"Employee Certification Report-", FileExtension.xlsx);

                    result = employeeService.GetAchievementAndCertificationReport(model.SelectedReportType,
                        model.fromDate.Value, model.toDate.Value, model.SelectedEmployeeID, fileName, fullPath);

                    if (result == "norec")
                        ModelState.AddModelError("OtherValidation", "No Record Found.");

                    else
                        return Json(new
                        {
                            fileName = fileName,
                            fullPath = fullPath + fileName,
                            message = "success",
                            htmlData = ConvertViewToString("_Filter", model)
                        }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return PartialView("_Filter", model);
        }
    }
}