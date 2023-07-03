using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;

namespace Nafed.MicroPay.Data.Repositories
{
    public class ArchiveDataRepository : BaseRepository, IArchiveDataRepository
    {

        public bool ArchiveData(int userID, DateTime fromdata, DateTime todate, Dictionary<int, int> indexs)
        {
            log.Info("ArchiveDataRepository/ArchiveData");


            int startIdx1 = 0, startIdx2 = 0, endIndex1 = 0, endIndex2 = 0;

            foreach (var item in indexs)
            {


                if (item.Key == 1)
                {
                    startIdx1 = item.Key;
                    endIndex1 = item.Value;
                }
                else
                {
                    startIdx2 = item.Key;
                    endIndex2 = item.Value;
                }
            }

            SqlConnection cnn = new SqlConnection(db.Database.Connection.ConnectionString);
            SqlTransaction transaction;

            cnn.Open();
            transaction = cnn.BeginTransaction();

            try
            {

                // Command Objects for the transaction
                SqlCommand cmd1 = new SqlCommand("ArchiveHRMSData", cnn, transaction);
                SqlCommand cmd2 = new SqlCommand("ArchiveHRMSData", cnn, transaction);

                cmd1.CommandType = CommandType.StoredProcedure;
                cmd2.CommandType = CommandType.StoredProcedure;

                cmd1.Parameters.Add(new SqlParameter("@fromdate", SqlDbType.Date));
                cmd1.Parameters["@fromdate"].Value = fromdata.Date;

                cmd1.Parameters.Add(new SqlParameter("@todate", SqlDbType.Date));
                cmd1.Parameters["@todate"].Value = todate.Date;

                cmd1.Parameters.Add(new SqlParameter("@startindex", SqlDbType.Int));
                cmd1.Parameters["@startindex"].Value = startIdx1;

                cmd1.Parameters.Add(new SqlParameter("@endindex", SqlDbType.Int));
                cmd1.Parameters["@endindex"].Value = endIndex1;
                //-----------------------------------------------------------------
                cmd2.Parameters.Add(new SqlParameter("@fromdate", SqlDbType.Date));
                cmd2.Parameters["@fromdate"].Value = fromdata.Date;

                cmd2.Parameters.Add(new SqlParameter("@todate", SqlDbType.Date));
                cmd2.Parameters["@todate"].Value = todate.Date;

                cmd2.Parameters.Add(new SqlParameter("@startindex", SqlDbType.Int));
                cmd2.Parameters["@startindex"].Value = startIdx2;

                cmd2.Parameters.Add(new SqlParameter("@endindex", SqlDbType.Int));
                cmd2.Parameters["@endindex"].Value = endIndex2;

                cmd1.ExecuteNonQuery();
                cmd2.ExecuteNonQuery();

                transaction.Commit();

                //====   Add archive transaction log detail ========

                AddTransactionSummary(userID, fromdata, todate, 2, "");

                //=====  End ==================================
                return true;
            }

            catch (SqlException sqlEx)
            {
                transaction.Rollback();


                //====   Add archive transaction log detail ========
                AddTransactionSummary(userID, fromdata, todate, 1, sqlEx.Message);

                //=====  End ==================================
                log.Error("Message-" + sqlEx.Message + " StackTrace-" + sqlEx.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw sqlEx;
            }

            finally
            {
                cnn.Close();
                cnn.Dispose();
            }
        }
        private bool AddTransactionSummary(int userID, DateTime fromdata, DateTime todate, int status, string transRemarks)
        {
            log.Info($"ArchiveDataRepository/AddTransactionSummary");
            bool flag = false;
            try
            {
                SqlCommand dbSqlCommand;
                //   SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[InsertArchiveTransLog]";

                //  dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                //  dbSqlAdapter.Fill(dbSqlDataTable);

                dbSqlCommand.Parameters.Add(new SqlParameter("@CutOffFromDate", SqlDbType.Date));
                dbSqlCommand.Parameters["@CutOffFromDate"].Value = fromdata.Date;

                dbSqlCommand.Parameters.Add(new SqlParameter("@CutOffToDate", SqlDbType.Date));
                dbSqlCommand.Parameters["@CutOffToDate"].Value = todate.Date;

                dbSqlCommand.Parameters.Add(new SqlParameter("@AchivedTillDate", SqlDbType.Date));
                dbSqlCommand.Parameters["@AchivedTillDate"].Value = todate.Date;

                dbSqlCommand.Parameters.Add(new SqlParameter("@ArchivedBy", SqlDbType.Int));
                dbSqlCommand.Parameters["@ArchivedBy"].Value = userID;

                dbSqlCommand.Parameters.Add(new SqlParameter("@TransactionDate", SqlDbType.DateTime));
                dbSqlCommand.Parameters["@TransactionDate"].Value = DateTime.Now;

                dbSqlCommand.Parameters.Add(new SqlParameter("@TransactionStatus", SqlDbType.TinyInt));
                dbSqlCommand.Parameters["@TransactionStatus"].Value = status;

                dbSqlCommand.Parameters.Add(new SqlParameter("@TransactionRemarks", SqlDbType.VarChar,500));
                dbSqlCommand.Parameters["@TransactionRemarks"].Value = transRemarks;

                dbSqlCommand.ExecuteNonQuery();
                flag = true;
                dbSqlconnection.Close();

                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public DataTable GetArchivedDataTransList()
        {
            log.Info($"ArchiveDataRepository/GetArchivedDataTransList");

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
                dbSqlCommand.CommandText = "[GetArchivedDataTransDtls]";


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

        public bool CheckArchiveExistForGivenYr(int year)
        {
            log.Info($"ArchiveDataRepository/CheckArchiveExistForGivenYr");

            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "CheckArchiveForGivenYr";

                dbSqlCommand.Parameters.Add(new SqlParameter("@year", SqlDbType.Int));
                dbSqlCommand.Parameters["@year"].Value = year;

                SqlParameter output = dbSqlCommand.Parameters.AddWithValue("@output", SqlDbType.Bit);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                output.Direction = ParameterDirection.Output;
                int res = dbSqlCommand.ExecuteNonQuery();
                dbSqlconnection.Close();

                return Convert.ToBoolean(output.Value);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
