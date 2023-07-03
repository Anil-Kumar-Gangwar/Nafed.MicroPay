using System;
using System.Data;
using System.Data.SqlClient;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace Nafed.MicroPay.Data.Repositories
{
    public class UpdateBasicRepository : BaseRepository, IUpdateBasicRepository
    {

    public int ValidateNewBasicAmount(int EmployeeSalaryID, double NewBasic)
        {
            DataTable dt = new DataTable();
            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[ValidateNewBasicAmount]";

                SqlParameter paramAttendanceDT = dbSqlCommand.Parameters.AddWithValue("@EmployeeSalaryID", EmployeeSalaryID);
                paramAttendanceDT.SqlDbType = SqlDbType.Int;

                SqlParameter paramuserID = dbSqlCommand.Parameters.AddWithValue("@NewBasicAmount", NewBasic);
                paramuserID.SqlDbType = SqlDbType.Int;

                SqlParameter output = dbSqlCommand.Parameters.AddWithValue("@ValidateNewBasic", SqlDbType.Int);
                // output.SqlDbType = SqlDbType.Int;

                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                output.Direction = ParameterDirection.Output;
                int res = dbSqlCommand.ExecuteNonQuery();
                dbSqlconnection.Close();
                return res = Convert.ToInt32(output.Value);
            }
            catch (Exception)
            {
                throw;
            }
           
        }


    }
}
