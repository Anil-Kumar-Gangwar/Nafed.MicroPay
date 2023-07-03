using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
namespace Nafed.MicroPay.Services.IServices
{
    public interface ITrainingService
    {
        int InsertGeneralTabDetail(TrainingSchedule trnSchedule);
        int UpdateGeneralTabDetail(TrainingSchedule trnSchedule);
        TrainingSchedule GetTrainingDetail(int trainingID);
        Training GetTrainingDtls(int trainingID);
        Training GetTrainerDetails(int trainingID);
        bool UpdateTrainerDetails(Training training);
        bool AddTrainingParticipantAttendance(TrainingSchedule tParticipants);
        List<Training> GetTrainingList(int? year = null, int? month = null);

        #region Training Participants
        List<Model.TrainingParticipantsDetail> GetTrainingParticipantsDetailsList(string employeeCode, string employeeName, int? departmentId);
        bool PostTrainingParticipants(List<Model.TrainingParticipant> participantsData, bool? isNomination = null,ProcessWorkFlow  process= null, TrainingSchedule trnSchedule = null);
        List<Model.TrainingParticipant> GetTrainingParticipantsList(int? trainingID);
        bool DeleteParticipant(int trainingParticipantID);

        #endregion Training Participants

        #region Training Document
        List<Model.TrainingDocumentRepository> GetTrainingDocumentList(int? trainingID);
        bool SaveTrainingDocument(TrainingDocumentRepository trainingDocument);
        bool DeleteTrainingDocument(int trainingDocumentID);
        #endregion Training Document

        #region TrainingPrerequisite
        bool InsertTrainingPrerequisite(TrainingSchedule trainingPrerequisite);
        TrainingSchedule GetTrainingPrerequisiteByID(int trainingID);
        #endregion

        #region Employee Feedback Form
        TrainingSchedule GetTrainingFeedbackFormDtls(int trainingID);
        int UpdateTrainingFeedbackForm(TrainingSchedule trainingSchedule);
        List<Training> GetFeedbackFormList(int? employeeID);
        TrainingSchedule GenerateFeedbackForm(int feedBackFormHdrID, int employeeID);
        int InsertEmployeeFeedbackForm(TrainingSchedule trainingSchedule);
        List<Training> GetTrainingPopupList();
        int CopyFeedbackQuesfromTraining(int prevTrainingID, int trainingID, int userID);
        #endregion

        #region Training Status
        bool UpdateTrainingStatus(TrainingSchedule training);
        bool UpdateTrainingStatusForSchedule(TrainingSchedule training);
        #endregion

        bool SendTrainingRelatedEmail(TrainingSchedule tSchedule, string mailType);
        List<Training> GetTrainingReport(CommonFilter cFilter);
        List<Training> GetEmployeeTrainingReport(CommonFilter cFilter);
        bool ExportEmployeeList(System.Data.DataTable dtTable, string sFullPath, string fileName, string tFilter);

        List<TrainingDateWiseTimeSlot> GetTrainingTimeSlots(int trainingID);
        TrainingRating GetTrainingRatings(int trainingID);

        string ExportTrainingRating(int trainingID, string filePath, string fileName);
        List<TrainingParticipant> GetTrainingNomineePending(Model.CommonFilter cFilter);
        bool NominationApproval(EmployeeLeave participantsData);
        bool FeedbackFormGenerated(int trainingID);
        List<Training> GetTrainingRrtDesignationWise(CommonFilter cFilter);
        bool ExportDesignationWise(System.Data.DataTable dtTable, string sFullPath, string fileName, string tFilter);
        List<Training> GetTrainingRptInternalExternal(CommonFilter cFilter);
        bool ExportInternalExternalWise(System.Data.DataTable dtTable, string sFullPath, string fileName, string tFilter);
        List<Training> GetTrainingRptTypeWise(CommonFilter cFilter);
        bool ExportTypeWise(System.Data.DataTable dtTable, string sFullPath, string fileName, string tFilter);
        List<SelectListModel> GetTrainingProviderList();
        List<Training> GetTrainingProviderReport(CommonFilter cFilter);
        bool ExportProviderWise(System.Data.DataTable dtTable, string sFullPath, string fileName, string tFilter);

        string[] GetTrainingProviderNames(CommonFilter cFilter);
        bool DeleteTraining(int trainingID);

        bool InsertDeletedTrainingParticipants(int userId, int trainingParticipantID);

    }
}
