using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Data.Repositories
{
    public class SalaryReportRepository : BaseRepository, ISalaryReportRepository
    {
        public DataSet GetMonthlyBranchWiseReport(int salMonth, int salYear, int employeeTypeID, int? branchID, out int nE_Cols, out int nD_Cols)
        {
            log.Info($"SalaryReportRepository/GetMonthlyBranchWiseReport/salMonth:{salMonth}&salYear:{salYear}&employeeTypeID:{employeeTypeID}&branchID:{branchID}");

            DataSet dsExcelData = new DataSet();

            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "GetMonthlyBranchWiseReport";
                dbSqlCommand.Parameters.Add("@salMonth", SqlDbType.Int).Value = salMonth;
                dbSqlCommand.Parameters.Add("@salYear", SqlDbType.Int).Value = salYear;
                dbSqlCommand.Parameters.Add("@employeeTypeID", SqlDbType.Int).Value = employeeTypeID;
                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = branchID;
                SqlParameter E_Col_Count = dbSqlCommand.Parameters.AddWithValue("@E_Col_Count", SqlDbType.Int);
                SqlParameter D_Col_Count = dbSqlCommand.Parameters.AddWithValue("@D_Col_Count", SqlDbType.Int);

                E_Col_Count.Direction = ParameterDirection.Output;
                D_Col_Count.Direction = ParameterDirection.Output;

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dsExcelData);
                dbSqlconnection.Close();

                nE_Cols = Convert.ToInt32(E_Col_Count.Value);
                nD_Cols = Convert.ToInt32(D_Col_Count.Value);
            }
            catch (Exception ex)
            {
                log.Error("Salary Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return dsExcelData;
        }

        public DataSet GetMonthlyEmployeeWiseReport(int salMonth, int salYear, int? employeeTypeID, int? branchID, out int nE_Cols, out int nD_Cols)
        {
            log.Info($"SalaryReportRepository/GetMonthlyEmployeeWiseReport/salMonth:{salMonth}&salYear:{salYear}&employeeTypeID:{employeeTypeID}&branchID:{branchID}");

            DataSet dsExcelData = new DataSet();

            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;

                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "GetMonthlyEmployeeWiseReport";
                dbSqlCommand.Parameters.Add("@salMonth", SqlDbType.Int).Value = salMonth;
                dbSqlCommand.Parameters.Add("@salYear", SqlDbType.Int).Value = salYear;
                dbSqlCommand.Parameters.Add("@employeeTypeID", SqlDbType.Int).Value = employeeTypeID;
                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = branchID;

                SqlParameter E_Col_Count = dbSqlCommand.Parameters.AddWithValue("@E_Col_Count", SqlDbType.Int);
                SqlParameter D_Col_Count = dbSqlCommand.Parameters.AddWithValue("@D_Col_Count", SqlDbType.Int);

                E_Col_Count.Direction = ParameterDirection.Output;
                D_Col_Count.Direction = ParameterDirection.Output;

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dsExcelData);
                dbSqlconnection.Close();

                nE_Cols = Convert.ToInt32(E_Col_Count.Value);
                nD_Cols = Convert.ToInt32(D_Col_Count.Value);
            }
            catch (Exception ex)
            {
                log.Error("Salary Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return dsExcelData;
        }

        public DataSet GetEmployeeWiseAnnualReport(int financialFrom, int financialTo, int? employeeTypeID, int? employeeId, int? branchId, out int nE_Cols, out int nD_Cols)
        {
            log.Info($"SalaryReportRepository/GetEmployeeWiseAnnualReport/financialFrom:{financialFrom}&financialTo:{financialTo}&employeeTypeID:{employeeTypeID}&employeeId:{employeeId}&branchId:{branchId}");
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
                dbSqlCommand.CommandText = "GetEmployeeWiseAnnualReport";
                dbSqlCommand.Parameters.Add("@financialFrom", SqlDbType.Int).Value = financialFrom;
                dbSqlCommand.Parameters.Add("@financialTo", SqlDbType.Int).Value = financialTo;
                dbSqlCommand.Parameters.Add("@employeeTypeID", SqlDbType.Int).Value = employeeTypeID;
                dbSqlCommand.Parameters.Add("@employeeId", SqlDbType.Int).Value = employeeId;
                dbSqlCommand.Parameters.Add("@branchId", SqlDbType.Int).Value = branchId;

                SqlParameter E_Col_Count = dbSqlCommand.Parameters.AddWithValue("@E_Col_Count", SqlDbType.Int);
                SqlParameter D_Col_Count = dbSqlCommand.Parameters.AddWithValue("@D_Col_Count", SqlDbType.Int);

                E_Col_Count.Direction = ParameterDirection.Output;
                D_Col_Count.Direction = ParameterDirection.Output;

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlconnection.Close();

                nE_Cols = Convert.ToInt32(E_Col_Count.Value);
                nD_Cols = Convert.ToInt32(D_Col_Count.Value);
            }
            catch (Exception)
            {
                throw;
            }
            return dbSqlDataTable;
        }

        public DataSet GetBranchWiseAnnualReport(int financialFrom, int financialTo, int? employeeTypeID, int? branchId, out int nE_Cols, out int nD_Cols)
        {
            log.Info($"SalaryReportRepository/GetBranchWiseAnnualReport/financialFrom:{financialFrom}&financialTo:{financialTo}&employeeTypeID:{employeeTypeID}&branchId:{branchId}");
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
                dbSqlCommand.CommandText = "GetBranchWiseAnnualReport";
                dbSqlCommand.Parameters.Add("@financialFrom", SqlDbType.Int).Value = financialFrom;
                dbSqlCommand.Parameters.Add("@financialTo", SqlDbType.Int).Value = financialTo;
                dbSqlCommand.Parameters.Add("@employeeTypeID", SqlDbType.Int).Value = employeeTypeID;
                dbSqlCommand.Parameters.Add("@branchId", SqlDbType.Int).Value = branchId;

                SqlParameter E_Col_Count = dbSqlCommand.Parameters.AddWithValue("@E_Col_Count", SqlDbType.Int);
                SqlParameter D_Col_Count = dbSqlCommand.Parameters.AddWithValue("@D_Col_Count", SqlDbType.Int);

                E_Col_Count.Direction = ParameterDirection.Output;
                D_Col_Count.Direction = ParameterDirection.Output;

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlconnection.Close();

                nE_Cols = Convert.ToInt32(E_Col_Count.Value);
                nD_Cols = Convert.ToInt32(D_Col_Count.Value);
            }
            catch (Exception)
            {
                throw;
            }
            return dbSqlDataTable;
        }

        public DataSet GetMonthlyEmployeeWisePaySlip(int salMonth, int salYear, int employeeTypeID, int? branchID, out int nE_Cols, out int nD_Cols)
        {
            log.Info($"SalaryReportRepository/GetMonthlyEmployeeWisePaySlip/salMonth:{salMonth}&salYear:{salYear}&employeeTypeID:{employeeTypeID}&branchID:{branchID}");

            DataSet dsExcelData = new DataSet();

            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "GetMonthlyEmployeeWisePaySlip";
                dbSqlCommand.Parameters.Add("@salMonth", SqlDbType.Int).Value = salMonth;
                dbSqlCommand.Parameters.Add("@salYear", SqlDbType.Int).Value = salYear;
                dbSqlCommand.Parameters.Add("@employeeTypeID", SqlDbType.Int).Value = employeeTypeID;
                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = branchID;
                SqlParameter E_Col_Count = dbSqlCommand.Parameters.AddWithValue("@E_Col_Count", SqlDbType.Int);
                SqlParameter D_Col_Count = dbSqlCommand.Parameters.AddWithValue("@D_Col_Count", SqlDbType.Int);

                E_Col_Count.Direction = ParameterDirection.Output;
                D_Col_Count.Direction = ParameterDirection.Output;

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dsExcelData);
                dbSqlconnection.Close();

                nE_Cols = Convert.ToInt32(E_Col_Count.Value);
                nD_Cols = Convert.ToInt32(D_Col_Count.Value);
            }
            catch (Exception ex)
            {
                log.Error("Salary Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return dsExcelData;
        }

        public DataSet GetBranchEmployeeWisePaySlip(int salMonth, int salYear, int employeeTypeID, int? branchID, out int nE_Cols, out int nD_Cols)
        {
            log.Info($"SalaryReportRepository/GetBranchEmployeeWisePaySlip/salMonth:{salMonth}&salYear:{salYear}&employeeTypeID:{employeeTypeID}&branchID:{branchID}");

            DataSet dsExcelData = new DataSet();

            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "GetBranchEmployeeWisePaySlip";
                dbSqlCommand.Parameters.Add("@salMonth", SqlDbType.Int).Value = salMonth;
                dbSqlCommand.Parameters.Add("@salYear", SqlDbType.Int).Value = salYear;
                dbSqlCommand.Parameters.Add("@employeeTypeID", SqlDbType.Int).Value = employeeTypeID;
                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = branchID;
                SqlParameter E_Col_Count = dbSqlCommand.Parameters.AddWithValue("@E_Col_Count", SqlDbType.Int);
                SqlParameter D_Col_Count = dbSqlCommand.Parameters.AddWithValue("@D_Col_Count", SqlDbType.Int);

                E_Col_Count.Direction = ParameterDirection.Output;
                D_Col_Count.Direction = ParameterDirection.Output;

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dsExcelData);
                dbSqlconnection.Close();

                nE_Cols = Convert.ToInt32(E_Col_Count.Value);
                nD_Cols = Convert.ToInt32(D_Col_Count.Value);
            }
            catch (Exception ex)
            {
                log.Error("Salary Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return dsExcelData;
        }

        public DataTable GetEmpDtlWithBankAccount(int salMonth, int salYear, int employeeTypeID, string bankCode)
        {
            log.Info($"SalaryReportRepository/GetEmpDtlWithBankAccount/salMonth:{salMonth}&salYear:{salYear}&employeeTypeID:{employeeTypeID}");
            DataTable dsExcelData = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "GetEmpDtlWithBankAccount";
                dbSqlCommand.Parameters.Add("@BankCode", SqlDbType.VarChar).Value = bankCode;
                dbSqlCommand.Parameters.Add("@Month", SqlDbType.Int).Value = salMonth;
                dbSqlCommand.Parameters.Add("@Year", SqlDbType.Int).Value = salYear;
                dbSqlCommand.Parameters.Add("@EmpTypeID", SqlDbType.Int).Value = employeeTypeID;

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dsExcelData);
                dbSqlconnection.Close();
            }
            catch (Exception ex)
            {
                log.Error("Salary Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return dsExcelData;
        }

        public SP_EmpWiseSalaryReport_Result GetSalarySlip(byte salMonth, short salYear, int employeeID)
        {
          return  db.SP_EmpWiseSalaryReport(salMonth, salYear, employeeID).FirstOrDefault();
        }
    }
}
