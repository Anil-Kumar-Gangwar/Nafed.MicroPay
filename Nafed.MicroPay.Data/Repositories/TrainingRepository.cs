using Nafed.MicroPay.Data.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;

namespace Nafed.MicroPay.Data.Repositories
{
    public class TrainingRepository : BaseRepository, ITrainingRepository
    {
        public List<GetTrainingFromDetail_Result> GetTrainingFromDetail(int feedBackFormHdrID, int? trainingID, int employeeID)
        {
            return db.GetTrainingFromDetail(feedBackFormHdrID, trainingID, employeeID).ToList();
        }

        public bool AddTrainingParticipantAttendance(List<TrainingParticipant> tParticipants)
        {
            log.Info("TrainingRepository/AddTrainingParticipantAttendance");
            bool flag = false;

            try
            {
                foreach (var item in tParticipants)
                {
                    var dataRow = db.TrainingParticipants.Where(x =>
                               x.TrainingParticipantID == item.TrainingParticipantID).FirstOrDefault();

                    dataRow.TrainingAttended = item.TrainingAttended;
                    dataRow.UpdateBy = item.UpdateBy;
                    dataRow.UpdateOn = item.UpdateOn;
                }
                db.SaveChanges();
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        public int CopyFeedbackQuesfromTraining(int prevTrainingID, int trainingID, int userID)
        {
            return db.CopyFeedbackQuesfromTraining(prevTrainingID, trainingID, userID);
        }

        //public IQueryable<Training> GetTrainingReport(int? EmployeeID, DateTime? FromDate, DateTime? ToDate, int? StatusID)
        //{
        //    IQueryable<Training> result = null;

        //    result = db.Trainings.AsNoTracking().Where(x => ((FromDate.HasValue && ToDate.HasValue) ? 
        //             (DbFunctions.TruncateTime(x.StartDate) >= DbFunctions.TruncateTime(FromDate)
        //             && DbFunctions.TruncateTime(x.EndDate) <= DbFunctions.TruncateTime(ToDate)): (1 > 0))
        //             && (StatusID != 0 ? x.TrainingStatus == StatusID : (1 > 0))
        //             );

        //    return result;
        //}

        public List<GetTrainingReport_Result> GetTrainingReport(DateTime? FromDate, DateTime? ToDate, int? StatusID)
        {

            return db.GetTrainingReport(StatusID, FromDate, ToDate).ToList();
        }

        public DataTable GetTrainingRatings(int trainingID)
        {
            log.Info("TrainingRepository/AddTrainingParticipantAttendance");
            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[GetTrainingRatingForAllEmp]";

                dbSqlCommand.Parameters.AddWithValue("@trainingID", trainingID);
                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlconnection.Close();

                return dbSqlDataTable;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<GetTrainingRrtDesignationWise_Result> GetTrainingRrtDesignationWise(int designationID, DateTime? fromDate, DateTime? toDate)
        {
            return db.GetTrainingRrtDesignationWise(designationID, fromDate, toDate).ToList();
        }

        public List<GetTrainingRptInternalExternal_Result> GetTrainingRptInternalExternal(int intexternal, DateTime? fromDate, DateTime? toDate)
        {
            return db.GetTrainingRptInternalExternal(intexternal, fromDate, toDate).ToList();
        }

        public List<GetTrainingRptTypeWise_Result> GetTrainingRptTypeWise(int typeID, DateTime? fromDate, DateTime? toDate)
        {
            return db.GetTrainingRptTypeWise(typeID, fromDate, toDate).ToList();
        }

        public IQueryable<TrainingParticipant> GetTrainingProviderReport(string[] arrCheckBoxList, DateTime? FromDate, DateTime? ToDate)
        {
            IQueryable<TrainingParticipant> result = null;

            result = db.TrainingParticipants.AsNoTracking().Where(x => ((FromDate.HasValue && ToDate.HasValue) ?
                     (DbFunctions.TruncateTime(x.Training.StartDate) >= DbFunctions.TruncateTime(FromDate)
                     && DbFunctions.TruncateTime(x.Training.EndDate) <= DbFunctions.TruncateTime(ToDate)) : (1 > 0))
                     && (arrCheckBoxList.Any(y => y == x.Training.DirectorName))
                     );

            return result;
        }

        public bool InsertDeletedTrainingParticipants(int userId, int trainingParticipantID)
        {
            log.Info("TrainingRepository/InsertDeletedTrainingParticipants");
            bool flag = false;
            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[InsertDeletedTrainingParticipants]";

                dbSqlCommand.Parameters.AddWithValue("@userId", userId);
                dbSqlCommand.Parameters.AddWithValue("@trainingParticipantID", trainingParticipantID);

                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlCommand.ExecuteNonQuery();

                dbSqlconnection.Close();
                flag = true;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
            }
            return flag;
        }

    }
}
