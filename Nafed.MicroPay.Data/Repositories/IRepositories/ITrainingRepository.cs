using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using System.Data;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
   public interface ITrainingRepository
    {
        List<GetTrainingFromDetail_Result> GetTrainingFromDetail(int FeedBackFormHdrID, int ? trainingID, int employeeID);
        bool AddTrainingParticipantAttendance(List<TrainingParticipant> tParticipants);

        int CopyFeedbackQuesfromTraining(int prevTrainingID, int trainingID, int userID);
        //        IQueryable<Training> GetTrainingReport(int? EmployeeID, DateTime? FromDate, DateTime? ToDate, int? StatusID);

        List<GetTrainingReport_Result> GetTrainingReport(DateTime? FromDate, DateTime? ToDate, int? StatusID);
        DataTable GetTrainingRatings(int trainingID);
        List<GetTrainingRrtDesignationWise_Result> GetTrainingRrtDesignationWise(int designationID, DateTime? fromDate, DateTime? toDate);
        List<GetTrainingRptInternalExternal_Result> GetTrainingRptInternalExternal(int intexternal, DateTime? fromDate, DateTime? toDate);
        List<GetTrainingRptTypeWise_Result> GetTrainingRptTypeWise(int typeID, DateTime? fromDate, DateTime? toDate);
        IQueryable<TrainingParticipant> GetTrainingProviderReport(string[] arrCheckBoxList, DateTime? FromDate, DateTime? ToDate);

        bool InsertDeletedTrainingParticipants(int userId, int trainingParticipantID);
    }
}
