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
    public class CandidateDetailsController : BaseController
    {

        private readonly IRecruitmentService RecruitmentService;
        private readonly IDropdownBindService ddlService;
        public CandidateDetailsController(IRecruitmentService RecruitmentService, IDropdownBindService ddlService)
        {
            this.RecruitmentService = RecruitmentService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            log.Info($"CandidateDetailsController/Index");
            CandidateDetailsViewModel CVM = new CandidateDetailsViewModel();
            CVM.userRights = userAccessRight;
            CVM.DesignationList = ddlService.ddlDesignationList();
            return View(CVM);
        }

        [HttpGet]
        public ActionResult _GetCandidateDetails(int requirementID)
        {
            try
            {
                DateTime now = DateTime.Now;
                var endDate = new DateTime(now.Year, now.Month, 1);
                var startDate = endDate.AddMonths(-12);

                //CandidateVM.PublishDateFrom = CandidateVM.PublishDateFrom.HasValue ? CandidateVM.PublishDateFrom.Value.Date : startDate.Date;
                //CandidateVM.PublishDateTo = CandidateVM.PublishDateTo.HasValue ? CandidateVM.PublishDateTo.Value.Date : endDate.Date;
                //CandidateVM.EligibleForWrittenExam = CandidateVM.EligibleForWrittenExam == null ? 0 : CandidateVM.EligibleForWrittenExam;
                var EligibleForWrittenExam = 2;
                //CandidateVM.IssueAdmitCard = 0;
                CandidateDetailsViewModel CVM = new CandidateDetailsViewModel();
                var CandDetails = RecruitmentService.GetCandidateDetailsByReqID(requirementID, EligibleForWrittenExam);

                CandDetails.ForEach(x =>
                {
                    x.CandidatePicture =
                     x.CandidatePicture == null ?
                     Path.Combine(Nafed.MicroPay.Common.DocumentUploadFilePath.CandidatePhoto + "/CandidatePhoto.png")
                     :

                    System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Nafed.MicroPay.Common.DocumentUploadFilePath.CandidatePhoto + "/" + x.CandidatePicture)) ?

                     Path.Combine(Nafed.MicroPay.Common.DocumentUploadFilePath.CandidatePhoto + "/" + x.CandidatePicture) :
                     Path.Combine(Nafed.MicroPay.Common.DocumentUploadFilePath.CandidatePhoto + "/CandidatePhoto.png");

                });
                CVM.CandidateDetails = CandDetails;
                CVM.userRights = userAccessRight;
                return View("CandidateGridContainer", CVM);
            }

            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetTotalAppliedCandidate(CandidateDetailsViewModel CandidateVM)
        {
            log.Info($"CandidateDetailsController/GetTotalAppliedCandidate");
            try
            {
                DateTime now = DateTime.Now;
                var endDate = new DateTime(now.Year, now.Month, 1);
                var startDate = endDate.AddMonths(-12);
                CandidateVM.PublishDateFrom = CandidateVM.PublishDateFrom.HasValue ? CandidateVM.PublishDateFrom.Value.Date : startDate.Date;
                CandidateVM.PublishDateTo = CandidateVM.PublishDateTo.HasValue ? CandidateVM.PublishDateTo.Value.Date : endDate.Date;
                CandidateVM.CandidateDetails = RecruitmentService.GetTotalAppliedCandidate(CandidateVM.DesignationID, (DateTime)CandidateVM.PublishDateFrom, (DateTime)CandidateVM.PublishDateTo);
                return PartialView("_TotalCandidateAppliedGridView", CandidateVM);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostCandidatedetails(CandidateDetailsViewModel CandidateVM)
        {
            try
            {
                if (CandidateVM != null)
                {
                    List<Model.CandidateDetails> Candidate = new List<Model.CandidateDetails>();
                    Candidate = CandidateVM.CandidateDetails;
                    Candidate.ForEach(x => { x.UpdatedBy = userDetail.UserID; });
                    bool recID = RecruitmentService.UpdateCandidatedetails(Candidate);

                    //for (int i = 0; i < Candidate.Count; i++)
                    //{
                    //    if (Candidate[i].EligibleForWrittenExam == true)
                    //    {
                    //        var cand = RecruitmentService.GetCandidateDetails(Candidate[i].RegistrationID);
                    //        //FileStream AdmitCardPath = "";

                    //        string AdmitCardPath = Download_AdmitCard(cand[0].RegistrationNo, cand[0].CandidatePicture, cand[0].CandidateSignature);
                    //        bool issendmail = RecruitmentService.CandidateSendMail(cand, "Candidate",Server.MapPath(AdmitCardPath));
                    //    }
                    //}

                    TempData["Message"] = "successfully Updated";
                    return RedirectToAction("Index");

                }
                return Content("");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        //public string Download_AdmitCard(string registrationNo, string candidatePicture, string candidateSignature)
        //{
        //    BaseReportModel reportModel = new BaseReportModel();
        //    List<ReportParameter> parameterList = new List<ReportParameter>();
        //    parameterList.Add(new ReportParameter() { name = "registrationNo", value = registrationNo });

        //    reportModel.reportParameters = parameterList;
        //    reportModel.rptName = "AdmitCard.rpt";
        //    candidatePicture = candidatePicture ?? "manager.png";
        //    candidateSignature = candidateSignature ?? "candidatesignature.jpg";
        //    ReportDocument objReport = new ReportDocument();
        //    var rptModel = reportModel;
        //    string path = Path.Combine(Server.MapPath(rptModel.ReportFilePath), rptModel.rptName);
        //    objReport.Load(path);
        //    objReport.SetDatabaseLogon(ConfigManager.Value("odbcUser"), ConfigManager.Value("odbcPassword"), @"odbcNAFEDHR", ConfigManager.Value("odbcDatabase"));

        //    objReport.Refresh();
        //    if (rptModel.reportParameters?.Count > 0)
        //    {
        //        foreach (var item in rptModel.reportParameters)
        //            objReport.SetParameterValue($"@{item.name}", item.value);
        //    }

        //    objReport.SetParameterValue("candidatephoto", Path.Combine(Server.MapPath(rptModel.CandidateImage), candidatePicture));
        //    objReport.SetParameterValue("candidatesignature", Path.Combine(Server.MapPath(rptModel.CandidateSignature), candidateSignature));
        //    Response.Buffer = false;
        //    Response.ClearContent();
        //    Response.ClearHeaders();


        //    objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.DefaultPaperOrientation;

        //    objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

        //    Stream stream = objReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //    stream.Seek(0, SeekOrigin.Begin);

        //    //return(Response.Write(stream));

        //  //  return File(stream, "application/pdf", "$" + registrationNo + "Admit Card.pdf");

        //   // ConfigManager.Value("AdmitCardUNCPath");
        //    registrationNo = registrationNo.Replace('/', '-');
        //    objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Path.Combine(Server.MapPath(rptModel.AdmitCardCPath), $"AdmitCard_{registrationNo}.pdf") );
        //   return (ConfigManager.Value("AdmitCardUNCPath") + $"AdmitCard_{registrationNo}.pdf");

        //    //return File(stream, "application/pdf", $"Admit Card_{registrationNo}.pdf");
        //}
    }
}