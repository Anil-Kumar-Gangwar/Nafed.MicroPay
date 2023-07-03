using Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
   public interface IRecruitmentRepository
    {
        bool RequirementAlreadyExists(int requirementId, int designationId, DateTime? publishDate);
        IEnumerable<Requirement> GetRequirementDetails(int? designationID, DateTime? publishDateFrom, DateTime? publishDateTo);
        IEnumerable<Requirement> GetVacanciesList();
        void RemoveJobRequirementLocation(int requirementId);
        void RemoveJobRequirementQualification(int requirementId);
        bool VerifyUser(string username, DateTime spassword);
        User GetUserData(string username);
        void RemoveJobExamDetailCenter(int requirementId);
        IEnumerable<GetCandidateAppliedCount_Result> GetTotalAppliedCandidate(int? designationID, DateTime startDate, DateTime endDate);
        IEnumerable<GetAppliedCandidateDetail_Result> AppliedCandidatesList(int requirementID);
    }
}
