using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;

namespace Nafed.MicroPay.Data.Repositories
{
    public class ProcessApproverRepository :BaseRepository, IProcessApproverRepository
    {
        public DataSet GetImportFieldValues(DataTable dtEmpCodes, DataTable dtReportingTo, DataTable dtReviewerTo, DataTable dtAcceptanceTo)
        {
            log.Info($"ProcessApproverRepository/GetImportFieldValues");
            DataSet dbSqlDataTable = new DataSet();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[GetProcessApproverImportFieldDtls]";

                SqlParameter paramEmpCode = dbSqlCommand.Parameters.AddWithValue("@tblEmpCode", dtEmpCodes);
                paramEmpCode.SqlDbType = SqlDbType.Structured;
                paramEmpCode.TypeName = "[udtGenericStringList]";

                SqlParameter paramReportingTo = dbSqlCommand.Parameters.AddWithValue("@tblReportingTo", dtReportingTo);
                paramReportingTo.SqlDbType = SqlDbType.Structured;
                paramReportingTo.TypeName = "[udtGenericStringList]";

                SqlParameter paramReviewerTo = dbSqlCommand.Parameters.AddWithValue("@tblReviewerTo", dtReviewerTo);
                paramReviewerTo.SqlDbType = SqlDbType.Structured;
                paramReviewerTo.TypeName = "[udtGenericStringList]";

                SqlParameter paramAcceptanceTo = dbSqlCommand.Parameters.AddWithValue("@tblAcceptanceTo", dtAcceptanceTo);
                paramAcceptanceTo.SqlDbType = SqlDbType.Structured;
                paramAcceptanceTo.TypeName = "[udtGenericStringList]";

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

         public int ImportProcessApproverData(int userID, int processID, DataTable dtApprovers)
        {
            log.Info($"ProcessApproverRepository/ImportProcessApproverData");

            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[ImportProcessApprovers]";

                SqlParameter paramAttendanceDT = dbSqlCommand.Parameters.AddWithValue("@approverDT", dtApprovers);
                paramAttendanceDT.SqlDbType = SqlDbType.Structured;
                paramAttendanceDT.TypeName = "[udtProcessApprovers]";

                SqlParameter paramuserID = dbSqlCommand.Parameters.AddWithValue("@userID", userID);
                paramuserID.SqlDbType = SqlDbType.Int;

                SqlParameter paramProcessID = dbSqlCommand.Parameters.AddWithValue("@processID", processID);
                paramProcessID.SqlDbType = SqlDbType.Int;

                SqlParameter output = dbSqlCommand.Parameters.AddWithValue("@output", SqlDbType.Int);
                // output.SqlDbType = SqlDbType.Int;

                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                output.Direction = ParameterDirection.Output;
                int res = dbSqlCommand.ExecuteNonQuery();
                dbSqlconnection.Close();
                return res = Convert.ToInt32(output.Value);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
