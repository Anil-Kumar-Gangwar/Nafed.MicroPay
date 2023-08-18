using Nafed.MicroPay.Data.Models;
using System.Collections.Generic;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Nafed.MicroPay.Data.Repositories
{
    public class ArrearRepository : BaseRepository, IArrearRepository
    {
        public DataTable GetImportFieldValues(DataTable empCodeDT)
        {
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
                dbSqlCommand.CommandText = "[GetManualDataImportFieldDtls]";

                SqlParameter paramItemCode = dbSqlCommand.Parameters.AddWithValue("@tblEmpCode", empCodeDT);
                paramItemCode.SqlDbType = SqlDbType.Structured;
                paramItemCode.TypeName = "[udtGenericStringList]";

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

        public int ImportManualData(int userID, DataTable DT)
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
                dbSqlCommand.CommandText = "[ImportManualData]";

                SqlParameter paramAttendanceDT = dbSqlCommand.Parameters.AddWithValue("@manualdataDT", DT);
                paramAttendanceDT.SqlDbType = SqlDbType.Structured;
                paramAttendanceDT.TypeName = "[tbl_arrearmanualdata]";

                SqlParameter paramuserID = dbSqlCommand.Parameters.AddWithValue("@userID", userID);
                paramuserID.SqlDbType = SqlDbType.Int;

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

        #region Calculate DA Arrear

        public DataTable GetMultipleTableDataResult(string fromYear, string toYear, string Flag)
        {
            log.Info($"ArrearRepository/GetMultipleTableDataResult");

            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandTimeout = 0;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[GetMultipleTableDataResult]";
                dbSqlCommand.Parameters.Add("@Flag", SqlDbType.VarChar).Value = Flag;
                dbSqlCommand.Parameters.Add("@fromYear", SqlDbType.VarChar).Value = fromYear;
                dbSqlCommand.Parameters.Add("@toYear", SqlDbType.VarChar).Value = toYear;

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

        public DataTable GetDatatableResult(string HeadValue)
        {
            log.Info($"ArrearRepository/calculateFromulaValueArrear");

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
                dbSqlCommand.CommandText = "[calculateFromulaValueArrear]";
                dbSqlCommand.Parameters.Add("@HeadValue", SqlDbType.VarChar).Value = HeadValue;

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

        public DataTable DeleteDuplicateData(int empID, string fromYear, string toYear, int BranchID, int flag, string arrearType)
        {
            log.Info($"ArrearRepository/DeleteDuplicateData");

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
                dbSqlCommand.CommandText = "[DeleteDuplicateDataforArrear]";
                dbSqlCommand.Parameters.Add("@empID", SqlDbType.Int).Value = empID;
                dbSqlCommand.Parameters.Add("@fromYear", SqlDbType.VarChar).Value = fromYear;
                dbSqlCommand.Parameters.Add("@toYear", SqlDbType.VarChar).Value = toYear;
                dbSqlCommand.Parameters.Add("@BranchID", SqlDbType.Int).Value = BranchID;
                dbSqlCommand.Parameters.Add("@flag", SqlDbType.Int).Value = flag;
                dbSqlCommand.Parameters.Add("@arrearType", SqlDbType.VarChar).Value = arrearType;
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

        public DataTable TransferarreardetailinFinalmonthlysalary(int empID, string fromYear, string toYear, int BranchID)
        {
            log.Info($"ArrearRepository/ArrearTransfer");

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
                dbSqlCommand.CommandText = "[TransferarreardetailinFinalmonthlysalary]";
                dbSqlCommand.Parameters.Add("@empID", SqlDbType.VarChar).Value = empID;
                dbSqlCommand.Parameters.Add("@fromYear", SqlDbType.VarChar).Value = fromYear;
                dbSqlCommand.Parameters.Add("@toYear", SqlDbType.VarChar).Value = toYear;
                dbSqlCommand.Parameters.Add("@BranchID", SqlDbType.VarChar).Value = BranchID;
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

        public int UpdateTblArreardummy(DataTable DTarreardummy)
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
                dbSqlCommand.CommandText = "[UpdateTblArrearDummy]";
                dbSqlCommand.CommandTimeout = 0;
                SqlParameter paramAttendanceDT = dbSqlCommand.Parameters.AddWithValue("@arreardummy", DTarreardummy);
                paramAttendanceDT.SqlDbType = SqlDbType.Structured;
                paramAttendanceDT.TypeName = "[TblArrearDummy]";

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
        public int UpdateTblArrearDetail(DataTable DTarreardetail)
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
                dbSqlCommand.CommandText = "[UpdateTblArrearDetail]";
                dbSqlCommand.CommandTimeout = 0;
                SqlParameter paramAttendanceDT = dbSqlCommand.Parameters.AddWithValue("@arreardetail", DTarreardetail);
                paramAttendanceDT.SqlDbType = SqlDbType.Structured;
                paramAttendanceDT.TypeName = "[TblArrearDetail]";
                
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

        public void UpdateWorkingDayslwp()
        {           
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[CalcMonthForDAArrear]";
                dbSqlCommand.CommandTimeout = 0;
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

        public void Calculateperiod()
        {
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[Calculateperiod]";

                dbSqlCommand.CommandTimeout = 0;
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
        public int UpdateTransportDetails(int empID, double newta, int month, int year, string fromYear, string toYear)
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
                dbSqlCommand.CommandText = "[UpdateransportDetails]";
                dbSqlCommand.Parameters.Add("@empID", SqlDbType.Int).Value = empID;
                dbSqlCommand.Parameters.Add("@newta", SqlDbType.Decimal).Value = newta;
                dbSqlCommand.Parameters.Add("@month", SqlDbType.Int).Value = month;
                dbSqlCommand.Parameters.Add("@year", SqlDbType.Int).Value = year;
                dbSqlCommand.Parameters.Add("@fromYear", SqlDbType.VarChar).Value = fromYear;
                dbSqlCommand.Parameters.Add("@toYear", SqlDbType.VarChar).Value = toYear;


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


        public List<getemployeeByBranchID_Result> getemployeeByBranchID(int branchID, string fromDate, string toDate,string empID)
        {
            return db.getemployeeByBranchID(branchID, fromDate, toDate, empID).ToList();
        }


        public int updatePayArrearDifference(int empID, string fromYear, string toYear, int BranchID, string DOG)
        {
            log.Info($"ArrearRepository/updatePayArrearDifference");

            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[InsertPayArrearDifferenceData]";
                dbSqlCommand.Parameters.Add("@empID", SqlDbType.Int).Value = empID;
                dbSqlCommand.Parameters.Add("@fromyear", SqlDbType.VarChar).Value = fromYear;
                dbSqlCommand.Parameters.Add("@toYear", SqlDbType.VarChar).Value = toYear;
                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = BranchID;
                dbSqlCommand.Parameters.Add("@dateofgenerate", SqlDbType.VarChar).Value = DOG;
                SqlParameter output = dbSqlCommand.Parameters.AddWithValue("@output", SqlDbType.Int);

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
        #endregion
        public int CreatePayArrearDetail(string fROMPERIOD, string tOPERIOD, string arrearType, string eMP, string brcode, string generateDate)
        {
            return db.CreatePayArrearDetail(fROMPERIOD, tOPERIOD, arrearType, eMP, brcode, generateDate);
        }
        
        public DataTable GetArrearPeriodsDetailsforPay(string arrerType)
        {

            DataTable dt = new DataTable();
            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandTimeout = 600;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "getArrearperiodsdetailsforPay";
                dbSqlCommand.Parameters.Add("@arrearType", SqlDbType.VarChar).Value = arrerType;
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
            return dbSqlDataTable;
        }

        public DataTable GetArrearReport(dynamic rFilter)
        {
            log.Info($"ArrearRepository/GetArrearReport/");
            DataTable dt = new DataTable();
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
                dbSqlCommand.CommandText = "[GetArrearReport]";

                dbSqlCommand.Parameters.Add("@fromPeriod", SqlDbType.Int).Value = rFilter.pFrom;
                dbSqlCommand.Parameters.Add("@toPeriod", SqlDbType.Int).Value = rFilter.pTo;
                dbSqlCommand.Parameters.Add("@ArrearType", SqlDbType.VarChar).Value = rFilter.arrearType;
                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = rFilter.branchID;
                dbSqlCommand.Parameters.Add("@employeeID", SqlDbType.Int).Value = rFilter.employeeID;

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
            return dbSqlDataTable;
        }


        public void UpdateDateBranch_Pay(int salMonth, int salYear)
        {
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();

                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[UpdateDateBranch_Pay]";
                dbSqlCommand.Parameters.Add("@SalMonth", SqlDbType.Int).Value = salMonth;
                dbSqlCommand.Parameters.Add("@SalYear", SqlDbType.Int).Value = salYear;
                dbSqlCommand.CommandTimeout = 0;
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
