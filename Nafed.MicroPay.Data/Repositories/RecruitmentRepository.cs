using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System.Data.Entity;

namespace Nafed.MicroPay.Data.Repositories
{
    public class RecruitmentRepository : BaseRepository, IRecruitmentRepository
    {
        public bool RequirementAlreadyExists(int requirementId, int designationId, DateTime? publishDate)
        {
            return db.Requirements.AsNoTracking().Any(x => x.RequirementID != requirementId
                      && x.DesinationID == designationId && DbFunctions.TruncateTime(x.PublishDate) == DbFunctions.TruncateTime(publishDate.Value) && !x.IsDeleted
                       );

        }

        public IEnumerable<Requirement> GetRequirementDetails(int? designationID, DateTime? publishDateFrom, DateTime? publishDateTo)
        {
            var result = db.Requirements.AsNoTracking().Where(x => !x.IsDeleted && (designationID.HasValue ? x.DesinationID == designationID.Value : 1 > 0)
              && ((publishDateFrom.HasValue ? DbFunctions.TruncateTime(x.PublishDate) >= DbFunctions.TruncateTime(publishDateFrom.Value) : 1 > 0) && (publishDateTo.HasValue ? DbFunctions.TruncateTime(x.PublishDate) <= DbFunctions.TruncateTime(publishDateTo.Value) : 1 > 0))).AsEnumerable();
            return result;
        }

        public IEnumerable<Requirement> GetVacanciesList()
        {
            var result = db.Requirements.AsNoTracking().Where(x => !x.IsDeleted && DbFunctions.TruncateTime(x.LastDateofApplicationReceived) >= DbFunctions.TruncateTime(DateTime.Now)).AsEnumerable();
            return result;
        }

        public void RemoveJobRequirementLocation(int requirementId)
        {
            var dd = db.JobRequirementLocations.Where(x => x.RequirementId == requirementId);
            db.JobRequirementLocations.RemoveRange(dd);
            db.SaveChanges();
        }

        public void RemoveJobRequirementQualification(int requirementId)
        {
            var dd = db.JobRequirementQualifications.Where(x => x.RequirementID == requirementId);
            db.JobRequirementQualifications.RemoveRange(dd);
            db.SaveChanges();
        }

        public bool VerifyUser(string username, DateTime spassword)
        {
            log.Info($"CandidateloginRepository/VerifyUser/{username}");
            bool IsExists = false;

            try
            {
                using (db = new MicroPayEntities())
                {
                    var user = db.CandidateRegistrations.Where(x => x.RegistrationNo.Equals(username)
                     && DbFunctions.TruncateTime(x.DOB) == DbFunctions.TruncateTime(spassword)
                     ).FirstOrDefault();
                    if (user != null)
                    {
                        //dbpassword = Convert.ToString(user.DOB.ToString("yyyyMMdd"));
                        //dbpassword =user.DOB.ToString();
                        IsExists = true;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }
            return IsExists;
        }
        public User GetUserData(string username)
        {
            log.Info($"CandidateloginRepository/VerifyUser/{username}");
            try
            {
                using (db = new MicroPayEntities())
                {
                    var user = db.Users.Include("CandidateRegistration").Where(x => x.UserName.Equals(username)).FirstOrDefault();
                    return user;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }
        }

        public void RemoveJobExamDetailCenter(int requirementId)
        {
            var dd = db.ExamCenterDetails.Where(x => x.RequirementID == requirementId);
            db.ExamCenterDetails.RemoveRange(dd);
            db.SaveChanges();
        }

        public IEnumerable<GetCandidateAppliedCount_Result> GetTotalAppliedCandidate(int? designationID, DateTime startDate, DateTime endDate)
        {
            var result = db.GetCandidateAppliedCount(designationID, startDate, endDate).ToList();
            return result.OrderByDescending(x => x.PublishDate);
        }

        public IEnumerable<GetAppliedCandidateDetail_Result> AppliedCandidatesList(int requirementID)
        {
            var result = db.GetAppliedCandidateDetail(requirementID).ToList();
            return result;
        }
    }
}
