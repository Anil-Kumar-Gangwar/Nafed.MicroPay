using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.Recruitment
{
    public interface IRecruitmentService
    {
        string CandidateRegistration(CandidateRegistration candidateForm);
        CandidateRegistration GetApplicationForm(int registrationID);

        #region Candidatedetails
        List<Model.CandidateDetails> GetCandidateDetails(DateTime PublishDateFrom, DateTime PublishDateTo, int EligibleForWrittenExam, int? DesignationID, int? IssueAdmitCard);
        bool UpdateCandidatedetails(List<Model.CandidateDetails> UpdateCandidate);
        #endregion

        #region Requirement
        List<Model.Requirement> GetRequirementDetails(int? designationID, DateTime? publishDateFrom, DateTime? publishDateTo);
        bool InsertRequirememnt(Model.Requirement requirement);
        Model.Requirement GetRequirementByID(int requirementId);
        bool UpdateRequirememnt(Model.Requirement requirement);
        bool Delete(int requirementID);
        bool RequirementAlreadyExists(int requirementId, int designationId, DateTime? publishDate);
        #endregion

        List<Model.Requirement> GetVacanciesList();

        IEnumerable<SelectListModel> GetFilterFields(int filter);
        List<SelectListModel> GetLocRequirement(int jLocTypeId, int requirementId);
        List<SelectListModel> GetJobLocation(int requirementId);
        //IEnumerable<SelectListModel> GetQualificationList();
        List<SelectListModel> GetJobQualification(int requirementId);
        bool ValidateUser(CandidateLogin userCredential, out string sAuthenticationMessage, out CandidateRegistration candidateForm);
        CandidateDetails GetCandidateDetails(int registrationID);
        bool CandidateSendMail(Model.CandidateDetails Candidate, string mailType, string AdmitCard);
        List<JobExamCenterDetails> GetExamCenterDetails(int requirementId);
        bool UpdateCandidateRegistration(CandidateRegistration candidateForm);
        bool UpdateCandidateAdmitCarddetails(Model.CandidateDetails candidateDetails);

        bool VerifyCandidationRegisterInfo(int requirementID, string inputText, int type = 1);

        int? CheckAlreadyRegistered(int requirementID, string mobileNo, string emailID);

        List<CandidateDetails> GetTotalAppliedCandidate(int? designationID, DateTime startDate, DateTime endDate);
        List<Model.CandidateDetails> GetCandidateDetailsByReqID(int requirementID, int EligibleForWrittenExam);
        List<AppliedCandidateDetail> AppliedCandidatesList(int requirementID);
        bool ExportAppCandidateList(DataTable dtTable, string sFullPath, string fileName);
    }
}
