using System;
using System.Data;
using System.Data.SqlClient;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace Nafed.MicroPay.Data.Repositories
{
    public class ActivityLogRepository : BaseRepository, IActivityLogRepository
    {
        public DataTable GetActivityLog()
        {
            SqlConnection dbSqlconnection;
            dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
            DataTable dt = new DataTable();
            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;

                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "GetActivityLog";
             

                dbSqlDataTable = new DataTable();
                DataSet ds = new DataSet();
                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlconnection.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dbSqlconnection.Close();
            }
            return dbSqlDataTable;
        }
    }
}
