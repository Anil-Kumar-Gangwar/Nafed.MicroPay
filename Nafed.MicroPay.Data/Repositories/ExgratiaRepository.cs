using System;
using System.Data;
using System.Data.SqlClient;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Data.Models;
using System.Collections.Generic;
using System.Linq;


namespace Nafed.MicroPay.Data.Repositories
{
    public class ExgratiaRepository : BaseRepository, IExgratiaRepository
    {

        public int CalculateExgratia(int noofdays, string fromYear, string toYear, int branchID,string financialyear,bool isPercentage,int emptype)
        {
            SqlConnection dbSqlconnection;
            dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
            try
            {
                SqlCommand dbSqlCommand;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[CalculateExgratia]";

                dbSqlCommand.Parameters.Add("@noofdays", SqlDbType.Int).Value = noofdays;
                dbSqlCommand.Parameters.Add("@fromYear", SqlDbType.VarChar).Value = fromYear;
                dbSqlCommand.Parameters.Add("@toYear", SqlDbType.VarChar).Value = toYear;
                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = branchID;
                dbSqlCommand.Parameters.Add("@financialyear", SqlDbType.VarChar).Value = financialyear;
                dbSqlCommand.Parameters.Add("@isPercentage", SqlDbType.Bit).Value = isPercentage;
                dbSqlCommand.Parameters.Add("@emptype", SqlDbType.VarChar).Value = emptype;
                SqlParameter output = dbSqlCommand.Parameters.AddWithValue("@output", SqlDbType.Int);


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
            finally
            {
                dbSqlconnection.Close();
            }
        }

        public DataTable GetExgratiaExport( string fromYear, string toYear, string fyear, int branchID, int emptype)
        {
            SqlConnection dbSqlconnection;
            dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandTimeout = 0;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[ExportExgratia]";

                dbSqlCommand.Parameters.Add("@fromYear", SqlDbType.VarChar).Value = fromYear;
                dbSqlCommand.Parameters.Add("@toYear", SqlDbType.VarChar).Value = toYear;
                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = branchID;
                dbSqlCommand.Parameters.Add("@financialyear", SqlDbType.VarChar).Value = fyear;
                dbSqlCommand.Parameters.Add("@emptype", SqlDbType.Int).Value = emptype;

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlconnection.Close();

                return dbSqlDataTable;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dbSqlconnection.Close();
            }
        }

        public DataTable GetExportExgratiaTemplate(string fromYear, string toYear, string fyear, int branchID, int emptype)
        {
            SqlConnection dbSqlconnection;
            dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[ExportExgratiaTemplate]";

                dbSqlCommand.Parameters.Add("@fromYear", SqlDbType.VarChar).Value = fromYear;
                dbSqlCommand.Parameters.Add("@toYear", SqlDbType.VarChar).Value = toYear;
                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = branchID;
                dbSqlCommand.Parameters.Add("@financialyear", SqlDbType.VarChar).Value = fyear;
                dbSqlCommand.Parameters.Add("@emptype", SqlDbType.Int).Value = emptype;

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlconnection.Close();

                return dbSqlDataTable;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dbSqlconnection.Close();
            }
        }


        public DataTable GetExgratiaEntryFormData(int branch, int month, int year)
        {
            log.Info($"ExgratiaRepository/GetExgratiaEntryFormData/branch:{branch}&month:{month}&year:{year}");

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
                dbSqlCommand.CommandText = "[GetExGratiaManualSheet]";

                dbSqlCommand.Parameters.AddWithValue("@branchID", branch);
                dbSqlCommand.Parameters.AddWithValue("@month", month);
                dbSqlCommand.Parameters.AddWithValue("@year", year);
                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlconnection.Close();

                return dbSqlDataTable;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataSet GetImportFieldValues(DataTable dtEmpCodes, DataTable dtBranch)
        {
            log.Info($"ExgratiaRepository/GetImportFieldValues");
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
                dbSqlCommand.CommandText = "[GetExGratiaImportFieldDtls]";

                SqlParameter paramEmpCode = dbSqlCommand.Parameters.AddWithValue("@tblEmpCode", dtEmpCodes);
                paramEmpCode.SqlDbType = SqlDbType.Structured;
                paramEmpCode.TypeName = "[udtGenericStringList]";


                SqlParameter paramBName = dbSqlCommand.Parameters.AddWithValue("@tblBranchName", dtBranch);
                paramBName.SqlDbType = SqlDbType.Structured;
                paramBName.TypeName = "[udtGenericStringList]";

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

        public int ImportExGratiaManualData(int userID, DataTable dt, int month, int year)
        {
            log.Info($"ExgratiaRepository/ImportExGratiaManualData");

            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[ImportExGratiaManualData]";

                SqlParameter paramAttendanceDT = dbSqlCommand.Parameters.AddWithValue("@exGratiaInputDT", dt);
                paramAttendanceDT.SqlDbType = SqlDbType.Structured;
                paramAttendanceDT.TypeName = "[udtExGratiaData]";

                SqlParameter paramuserID = dbSqlCommand.Parameters.AddWithValue("@userID", userID);
                paramuserID.SqlDbType = SqlDbType.Int;

                SqlParameter paramMonth = dbSqlCommand.Parameters.AddWithValue("@month", month);
                paramMonth.SqlDbType = SqlDbType.Int;

                SqlParameter paramYear = dbSqlCommand.Parameters.AddWithValue("@year", year);
                paramYear.SqlDbType = SqlDbType.Int;

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

        public int ImportExGratiaIncomeTaxData(int userID, DataTable dt, string financialYear)
        {
            log.Info($"ExgratiaRepository/ImportExGratiaIncomeTaxData");

            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[ImportExGratiaIncomeTaxData]";

                SqlParameter paramAttendanceDT = dbSqlCommand.Parameters.AddWithValue("@exGratiaInputDT", dt);
                paramAttendanceDT.SqlDbType = SqlDbType.Structured;
                paramAttendanceDT.TypeName = "[udtExGratiaIncomeTaxData]";

                SqlParameter paramuserID = dbSqlCommand.Parameters.AddWithValue("@userID", userID);
                paramuserID.SqlDbType = SqlDbType.Int;

                SqlParameter paramMonth = dbSqlCommand.Parameters.AddWithValue("@financialYear", financialYear);
                paramMonth.SqlDbType = SqlDbType.VarChar;

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

        public void PublishExGratia(string financialyear, int emptype, int branchID)
        {
            log.Info($"ExgratiaRepository/PublishExGratia");
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[PublishExGratia]";

                dbSqlCommand.Parameters.Add("@financialyear", SqlDbType.VarChar).Value = financialyear;
                dbSqlCommand.Parameters.Add("@emptype", SqlDbType.VarChar).Value = emptype;
                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = branchID;

                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
              
                int res = dbSqlCommand.ExecuteNonQuery();
                dbSqlconnection.Close();
             
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
