using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;

namespace MicroPay.Web.Controllers.Reports
{
    public class EmployeeQualificationController : BaseController
    {
        private readonly IDropdownBindService dropdownBindService;
        public EmployeeQualificationController(IDropdownBindService dropdownBindService)
        {
            this.dropdownBindService = dropdownBindService;
        }

        public ActionResult Index()
        {
            log.Info($"EmployeeQualificationController/Index");
            try
            {
                AcadmicProfessionalModel qualificationReportFilter = new AcadmicProfessionalModel();
                return View(qualificationReportFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _ReportFilter()
        {
            log.Info($"EmployeeQualificationController/_ReportFilter");
            try
            {
                AcadmicProfessionalModel APVM = new AcadmicProfessionalModel();
                BindDropdowns();
                return PartialView("_ReportFilter", APVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {
            
            var qualification = dropdownBindService.ddlAcedamicAndProfDtls(2);
            Nafed.MicroPay.Model.SelectListModel qualificationList = new Nafed.MicroPay.Model.SelectListModel();
            qualificationList.id = 0;
            qualificationList.value = "Select";
            qualification.Insert(0, qualificationList);
            ViewBag.QualificationList = new SelectList(qualification, "id", "value");           
        }

        [HttpPost]
        public ActionResult ShowReport(AcadmicProfessionalModel APVM)
        {
            log.Info($"EmployeeQualificationController/ShowReport");
            try
            {
                //string parm = string.Empty;
                //string parmextra = string.Empty;

                //if (APVM.qualificationId == 1)
                //{
                //    parm = "%mb%";
                //    parmextra = "m.b%";
                //}
                //else if (APVM.qualificationId == 2)
                //{
                //    parm = "%agr%";
                //    parmextra = "";
                //}
                //else if (APVM.qualificationId == 3)
                //{
                //    parm = "%pg%";
                //    parmextra = "";
                //}
                //else if (APVM.qualificationId == 4)
                //{
                //    parm = "%Dip%";
                //    parmextra = "";
                //}
                //else if (APVM.qualificationId == 5)
                //{
                //    parm = "ca%";
                //    parmextra = "%icwa%";
                //}
                //else if (APVM.qualificationId == 6)
                //{
                //    parm = "%COMPUTER%";
                //    parmextra = "";
                //}
                //else if (APVM.qualificationId == 7)
                //{
                //    parm = "%UNIX%";
                //    parmextra = "C";
                //}
                BaseReportModel reportModel = new BaseReportModel();
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter() { name = "QID", value = APVM .qualificationId});               
                reportModel.reportParameters = parameterList;
                reportModel.rptName = "EmployeeQualification.rpt";
                Session["ReportModel"] = reportModel;
                return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}