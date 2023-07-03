using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using System;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.Recruitment;
using Model = Nafed.MicroPay.Model;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers.Recruitment
{
    public class CandidateSearchDetailsController : BaseController
    {
        private readonly IRecruitmentService RecruitmentService;
        private readonly IDropdownBindService ddlService;
        public CandidateSearchDetailsController(IRecruitmentService RecruitmentService, IDropdownBindService ddlService)
        {
            this.RecruitmentService = RecruitmentService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            log.Info($"CandidateSearchDetailsController/Index");
            CandidateDetailsViewModel CVM = new CandidateDetailsViewModel();
            CVM.userRights = userAccessRight;
            CVM.DesignationList = ddlService.ddlDesignationList();
            BindDropdowns();
            return View(CVM);
        }

        [HttpGet]
        public ActionResult _GetCandidateSearchDetailsGridView(CandidateDetailsViewModel CandidateVM)
        {
            BindDropdowns();
            CandidateDetailsViewModel CVM = new CandidateDetailsViewModel();
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            CandidateVM.PublishDateFrom = CandidateVM.PublishDateFrom.HasValue ? CandidateVM.PublishDateFrom : startDate.Date;
            CandidateVM.PublishDateTo = CandidateVM.PublishDateTo.HasValue ? CandidateVM.PublishDateTo : endDate.Date;
            CandidateVM.IssueAdmitCard = CandidateVM.IssueAdmitCard == null ? 1 : CandidateVM.IssueAdmitCard;
            CandidateVM.EligibleForWrittenExam = 0;

            
            var CandDetails = RecruitmentService.GetCandidateDetails((DateTime)CandidateVM.PublishDateFrom, (DateTime)CandidateVM.PublishDateTo, (int)CandidateVM.EligibleForWrittenExam, CandidateVM.DesignationID, CandidateVM.IssueAdmitCard);
            CVM.IssueAdmitCard = CandidateVM.IssueAdmitCard;
            CVM.EligibleForWrittenExam = CandidateVM.EligibleForWrittenExam;
            CVM.CandidateDetails = CandDetails;
            CVM.userRights = userAccessRight;
            return PartialView("_CandidateSearchDetailsGridView", CVM);
        }

        public void BindDropdowns()
        {
            List<SelectListItem> selectCandidate = new List<SelectListItem>();

            //selectCandidate.Add(new SelectListItem
            //{ Text = "All", Value = "0" });

            selectCandidate.Add(new SelectListItem
            { Text = "Yes", Value = "2" });

            selectCandidate.Add(new SelectListItem
            { Text = "No", Value = "1" });

            ViewBag.selectedCandidate = selectCandidate;
        }

        public ActionResult _PostCandidateSearchdetails(CandidateDetailsViewModel CandidateVM)
        {
            log.Info($"CandidateSearchDetailsController/_PostCandidateSearchdetails");
            try
            {
                CandidateVM.CandidateDetails.ForEach(x => { x.UpdatedBy = userDetail.UserID; });
                for (int i = 0; i < CandidateVM.CandidateDetails.Count; i++)
                {
                    if (CandidateVM.CandidateDetails[i].IssueAdmitCard == true)
                    {
                        var cand = RecruitmentService.GetCandidateDetails(CandidateVM.CandidateDetails[i].RegistrationID);
                        string AdmitCardPath = Download_AdmitCard(cand.RegistrationNo, cand.CandidatePicture, cand.CandidateSignature);
                        bool isSendMail = RecruitmentService.CandidateSendMail(cand, "Candidate", AdmitCardPath);
                        if (isSendMail)
                        {
                            bool updated = RecruitmentService.UpdateCandidateAdmitCarddetails(CandidateVM.CandidateDetails[i]);
                        }
                    }
                }
                TempData["Message"] = "Succesfully Updated";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public string Download_AdmitCard(string registrationNo, string candidatePicture, string candidateSignature)
        {
            BaseReportModel reportModel = new BaseReportModel();
            List<ReportParameter> parameterList = new List<ReportParameter>();
            parameterList.Add(new ReportParameter() { name = "registrationNo", value = registrationNo });

            reportModel.reportParameters = parameterList;
            reportModel.rptName = "AdmitCard.rpt";
            candidatePicture = candidatePicture ?? "CandidateAdmitCardPhoto.png";
            candidateSignature = candidateSignature ?? "candidatesignature.png";
            ReportDocument objReport = new ReportDocument();
            var rptModel = reportModel;
            string path = Path.Combine(Server.MapPath(rptModel.ReportFilePath), rptModel.rptName);
            objReport.Load(path);
            objReport.SetDatabaseLogon(ConfigManager.Value("odbcUser"), ConfigManager.Value("odbcPassword"), @"odbcNAFEDHR", ConfigManager.Value("odbcDatabase"));

            objReport.Refresh();
            if (rptModel.reportParameters?.Count > 0)
            {
                foreach (var item in rptModel.reportParameters)
                    objReport.SetParameterValue($"@{item.name}", item.value);
            }

            objReport.SetParameterValue("candidatephoto", Path.Combine(Server.MapPath(rptModel.CandidateImage), candidatePicture));
            objReport.SetParameterValue("candidatesignature", Path.Combine(Server.MapPath(rptModel.CandidateSignature), candidateSignature));
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.DefaultPaperOrientation;

            objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

            Stream stream = objReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            //return(Response.Write(stream));

            //  return File(stream, "application/pdf", "$" + registrationNo + "Admit Card.pdf");

            // ConfigManager.Value("AdmitCardUNCPath");
            registrationNo = registrationNo.Replace('/', '-');
            objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Path.Combine(Server.MapPath(rptModel.AdmitCardCPath), $"AdmitCard_{registrationNo}.pdf"));
            return (Path.Combine(Server.MapPath(rptModel.AdmitCardCPath), $"AdmitCard_{registrationNo}.pdf"));

            //return File(stream, "application/pdf", $"Admit Card_{registrationNo}.pdf");
        }
    }
}