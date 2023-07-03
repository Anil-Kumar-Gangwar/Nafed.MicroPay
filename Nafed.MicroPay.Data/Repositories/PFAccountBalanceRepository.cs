using System;
using System.Data;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System.Data.SqlClient;

namespace Nafed.MicroPay.Data.Repositories
{
    public class PFAccountBalanceRepository : BaseRepository, IPFAccountBalanceRepository
    {
        public DataTable GetPfBalanceManualEntryFormData(int branch, byte month, int year)
        {
            log.Info($"PFAccountBalanceRepository/GetPfBalanceManualEntryFormData/branch:{branch}/month:{month}/year:{year}");

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
                dbSqlCommand.CommandText = "[GetPfOpBalanceManualSheet]";

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
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public DataSet GetImportFieldValues(DataTable dtEmpCodes, DataTable dtPfNo, DataTable dtBranch)
        {
            log.Info($"PFAccountBalanceRepository/GetImportFieldValues");
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
                dbSqlCommand.CommandText = "[GetPfBalanceImportFieldDtls]";

                SqlParameter paramEmpCode = dbSqlCommand.Parameters.AddWithValue("@tblEmpCode", dtEmpCodes);
                paramEmpCode.SqlDbType = SqlDbType.Structured;
                paramEmpCode.TypeName = "[udtGenericStringList]";

                SqlParameter paramPFNo = dbSqlCommand.Parameters.AddWithValue("@tblPfNo", dtPfNo);
                paramPFNo.SqlDbType = SqlDbType.Structured;
                paramPFNo.TypeName = "[udtGenericStringList]";


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

        public int ImportPfBalanceManualData(int userID, DataTable dt, int month, int year)
        {
            log.Info($"PFAccountBalanceRepository/ImportPfBalanceManualData");

            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[ImportPFOpBalanceManualData]";

                SqlParameter paramAttendanceDT = 
                    dbSqlCommand.Parameters.AddWithValue("@pfBalanceManualData", dt);
                paramAttendanceDT.SqlDbType = SqlDbType.Structured;
                paramAttendanceDT.TypeName = "[udtPfOpBalanceManual]";

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


        /// <summary>
        ///  Update PFNo to all pfNo depedent tables ......
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="pfNo"></param>
        public void UpdatePfNo(int employeeID, int pfNo)
        {
            log.Info($"PFAccountBalanceRepository/UpdatePfNo/employeeID:{employeeID},pfNo:{pfNo}");

            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[UpdatePFNoAcrossAllTables]";

                SqlParameter paramuserID = dbSqlCommand.Parameters.AddWithValue("@employeeID", employeeID);
                paramuserID.SqlDbType = SqlDbType.Int;

                SqlParameter paramMonth = dbSqlCommand.Parameters.AddWithValue("@pfNo", pfNo);
                paramMonth.SqlDbType = SqlDbType.Int;

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                //  dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlCommand.ExecuteNonQuery();
                dbSqlconnection.Close();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public void UpdateEmpAccountDetail(int eID, int? pfNo, string uan, string epfo, string pan, string aadhar, string ac, string bankCode, string ifscCode, int userID)
        {
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[UpdateEmpAccountDetail]";

                SqlParameter empIdParam = dbSqlCommand.Parameters.AddWithValue("@employeeID", eID);
                empIdParam.SqlDbType = SqlDbType.Int;
                SqlParameter userIdParam = dbSqlCommand.Parameters.AddWithValue("@userId", pfNo);
                userIdParam.SqlDbType = SqlDbType.Int;
                SqlParameter pfNoParam = dbSqlCommand.Parameters.AddWithValue("@pfNo", pfNo);
                pfNoParam.SqlDbType = SqlDbType.Int;
                SqlParameter uanParam = dbSqlCommand.Parameters.AddWithValue("@uan", uan);
                uanParam.SqlDbType = SqlDbType.VarChar;
                SqlParameter epfoParam = dbSqlCommand.Parameters.AddWithValue("@epfo", epfo);
                epfoParam.SqlDbType = SqlDbType.VarChar;
                SqlParameter panParam = dbSqlCommand.Parameters.AddWithValue("@pan", pan);
                panParam.SqlDbType = SqlDbType.VarChar;
                SqlParameter aadharParam = dbSqlCommand.Parameters.AddWithValue("@aadhar", aadhar);
                aadharParam.SqlDbType = SqlDbType.VarChar;
                SqlParameter acParam = dbSqlCommand.Parameters.AddWithValue("@ac", ac);
                acParam.SqlDbType = SqlDbType.VarChar;
                SqlParameter bankCodeParam = dbSqlCommand.Parameters.AddWithValue("@bankCode", bankCode);
                bankCodeParam.SqlDbType = SqlDbType.VarChar;
                SqlParameter ifscCodeParam = dbSqlCommand.Parameters.AddWithValue("@ifscCode", ifscCode);
                ifscCodeParam.SqlDbType = SqlDbType.VarChar;

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();              
                dbSqlCommand.ExecuteNonQuery();
                dbSqlconnection.Close();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public DataTable GetUnAssignedPFRecords(int? branchID)
        {
            log.Info($"PFAccountBalanceRepository/GetUnAssignedPFRecords/branchID:{branchID}");

            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                DataTable dbSqlDataTable = new DataTable();

                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[GetUnAssignedPFRecords]";

                SqlParameter paramBranchID = dbSqlCommand.Parameters.AddWithValue("@branchID", branchID);
                paramBranchID.SqlDbType = SqlDbType.Int;
                //  paramBranchID.IsNullable = true;

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlCommand.ExecuteNonQuery();
                dbSqlconnection.Close();

                return dbSqlDataTable;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int UpdateInterest(int year, decimal rate)
        {
            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[SP_UpdateInterest_Employee]";

                SqlParameter paramYear = dbSqlCommand.Parameters.AddWithValue("@fyear", year);
                paramYear.SqlDbType = SqlDbType.VarChar;
                SqlParameter paramRate = dbSqlCommand.Parameters.AddWithValue("@rate", rate);
                paramRate.SqlDbType = SqlDbType.Decimal;



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
