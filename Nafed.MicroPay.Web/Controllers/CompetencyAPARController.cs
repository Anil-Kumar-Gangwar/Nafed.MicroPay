using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers
{
    public class CompetencyAPARController : BaseController
    {
        private readonly IAppraisalFormService appraisalService;
        public CompetencyAPARController(IAppraisalFormService appraisalService)
        {
            this.appraisalService = appraisalService;
        }
        // GET: CompentencyAPAR
        public ActionResult Index()
        {
            log.Info($"CompetencyAPARController/Index");
            Models.APARSkillsViewModel empAPARVM = new Models.APARSkillsViewModel();
            var approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.Appraisal);
            empAPARVM.approvalSetting = approvalSettings ?? new EmployeeProcessApproval();
            //  Models.APARSkillsViewModel empAPARVM = new Models.APARSkillsViewModel();
            var reportingYr = DateTime.Now.GetFinancialYr();

            var getAPARskillStatus = appraisalService.GetFormAPARHdrDetail(null, userDetail.EmployeeID, reportingYr);
            empAPARVM.APARSkillStatus = getAPARskillStatus.StatusID;
            var getAPARStatus = appraisalService.GetEmployeeSelfAppraisalList((int)userDetail.EmployeeID, null, null).Where(x => x.ReportingYr == reportingYr).ToList();
            if (getAPARStatus != null && getAPARStatus.Count > 0)
                empAPARVM.APARStatus = getAPARStatus.FirstOrDefault().StatusID;

            var frmAttributes = appraisalService.GetFormSubmissionDate((int)userDetail.DesignationID);
            empAPARVM.frmAttributes = frmAttributes;

            return View(empAPARVM);
        }
    }
}