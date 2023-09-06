using System;
using System.Data;
using System.Data.SqlClient;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace Nafed.MicroPay.Data.Repositories
{
    public class LeaveRepository : BaseRepository, ILeaveRepository
    {
        public DataTable GetEmployeeLeaveAccruedList(int LeaveCategoryID, string Month, string Year)
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
                dbSqlCommand.CommandText = "GetEmployeeLeaveAccruedReport";
                dbSqlCommand.Parameters.Add("@month", SqlDbType.VarChar).Value = Month;
                dbSqlCommand.Parameters.Add("@year", SqlDbType.VarChar).Value = Year;
                dbSqlCommand.Parameters.Add("@LeaveCategoryID", SqlDbType.Int).Value = LeaveCategoryID;

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

        public DataTable GetFillDropdown(string flag, int leavecategoryId)
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
                dbSqlCommand.CommandText = "GetFillDropdown";
                dbSqlCommand.Parameters.Add("@Flag", SqlDbType.VarChar).Value = flag;
                dbSqlCommand.Parameters.Add("@leavecategoryId", SqlDbType.Int).Value = leavecategoryId;

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

        public DataTable GetLeavevalidatemonth(int leavecategoryId, int month, int year)
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
                dbSqlCommand.CommandText = "GetLeaveValidateMonth";
                dbSqlCommand.Parameters.Add("@leavecategoryId", SqlDbType.Int).Value = leavecategoryId;
                dbSqlCommand.Parameters.Add("@UpdateYear", SqlDbType.Int).Value = year;
                dbSqlCommand.Parameters.Add("@UpdateMonth ", SqlDbType.Int).Value = month;

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
            finally { dbSqlconnection.Close(); }
            return dbSqlDataTable;
        }

        public DataTable GetRegularEmployee()
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
                dbSqlCommand.CommandText = "GetRegularEmployee";
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

        public DataTable GetELMLAccumulationdata(string EmpCode, int prevYear, int curryear, string flag)
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
                dbSqlCommand.CommandText = "GetELMLAccumulationdata";
                dbSqlCommand.Parameters.Add("@EmpCode", SqlDbType.VarChar).Value = EmpCode;
                dbSqlCommand.Parameters.Add("@prevYear", SqlDbType.Int).Value = prevYear;
                dbSqlCommand.Parameters.Add("@curryear", SqlDbType.Int).Value = curryear;
                dbSqlCommand.Parameters.Add("@flag", SqlDbType.Int).Value = flag;
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
            finally { dbSqlconnection.Close(); }
            return dbSqlDataTable;
        }

        public DataTable UpdateCLAccured(DateTime DOJ, string EmpCode, int prevYear, int curryear, int month, int userID)
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
                dbSqlCommand.CommandText = "UpdateCLAccured";
                dbSqlCommand.Parameters.Add("@DOJ", SqlDbType.DateTime).Value = DOJ;
                dbSqlCommand.Parameters.Add("@EmpCode", SqlDbType.VarChar).Value = EmpCode;
                dbSqlCommand.Parameters.Add("@prevYear", SqlDbType.Int).Value = prevYear;
                dbSqlCommand.Parameters.Add("@curryear", SqlDbType.Int).Value = curryear;
                dbSqlCommand.Parameters.Add("@month", SqlDbType.Int).Value = month;
                dbSqlCommand.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
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

        public DataTable UpdateELAccured(DateTime DOJ, string EmpCode, int prevYear, int curryear, int month, int userID)
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
                dbSqlCommand.CommandText = "UpdateELAccured";
                dbSqlCommand.Parameters.Add("@DOJ", SqlDbType.DateTime).Value = DOJ;
                dbSqlCommand.Parameters.Add("@EmpCode", SqlDbType.VarChar).Value = EmpCode;
                dbSqlCommand.Parameters.Add("@prevYear", SqlDbType.Int).Value = prevYear;
                dbSqlCommand.Parameters.Add("@curryear", SqlDbType.Int).Value = curryear;
                dbSqlCommand.Parameters.Add("@month", SqlDbType.Int).Value = month;
                dbSqlCommand.Parameters.Add("@userID", SqlDbType.Int).Value = userID;

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

        public bool AddLeaveBalance(tblLeaveBal leaveBalance)
        {
            bool flag = false;
            try
            {
                db.tblLeaveBals.Add(leaveBalance);
                db.SaveChanges();
                flag = true;
            }
            catch (Exception)
            {

                throw;
            }
            return flag;
        }

        public bool AddLeaveTrans(tblLeaveTran leaveTrans)
        {
            bool flag = false;
            try
            {
                db.tblLeaveTrans.Add(leaveTrans);
                db.SaveChanges();
                flag = true;
            }
            catch (Exception)
            {

                throw;
            }
            return flag;
        }

        public bool AddLeaveUpdate(tblLeaveUpdate leaveUpdate)
        {
            bool flag = false;
            try
            {
                db.tblLeaveUpdates.Add(leaveUpdate);
                db.SaveChanges();
                flag = true;

            }
            catch (Exception)
            {

                throw;
            }
            return flag;
        }
        public bool AddMedicalLeaveUpdate(tblMedicalLeaveUpdate MedicalleaveUpdate)
        {
            bool flag = false;
            try
            {
                db.tblMedicalLeaveUpdates.Add(MedicalleaveUpdate);
                db.SaveChanges();
                flag = true;

            }
            catch (Exception)
            {

                throw;
            }
            return flag;
        }

        public IEnumerable<GetEmployeeLeaveBal_Result> GetEmployeeLeaveBal(string empCode)
        {
            return db.GetEmployeeLeaveBal(empCode);
        }

        public IEnumerable<EmployeeLeave> GetLeaveApplied(int reportingTo)
        {
            try
            {
                List<EmployeeLeave> empLeaves = new List<EmployeeLeave>();

                //var totalLeave = (from empLeave in db.EmployeeLeaves
                //                  join leaveCategory in db.LeaveCategories
                //                  on empLeave.LeaveCategoryID equals leaveCategory.LeaveCategoryID
                //                  join em in db.tblMstEmployees on empLeave.EmployeeId equals em.EmployeeId
                //                  where empLeave.IsDeleted == false && empLeave.StatusID == 1 && em.ReportingTo==(int)reportingTo
                //                  select new { empLeave, leaveCategory.LeaveCode }).ToList();

                //totalLeave.ForEach(x => {
                //    empLeaves.Add(x.empLeave);

                //});

                //empLeaves.ForEach(y => {
                //    y.LeaveCategory.LeaveCode = totalLeave.Where(z => z.empLeave == y).FirstOrDefault().LeaveCode;
                //});
                return empLeaves;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetEmployeeLeaveDetails(string employeecode, string year)
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
                dbSqlCommand.CommandText = "GetEmployeeLeaveDetails";
                dbSqlCommand.Parameters.Add("@EmpCode", SqlDbType.VarChar).Value = employeecode;
                dbSqlCommand.Parameters.Add("@Year", SqlDbType.VarChar).Value = year;

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

        public DataTable getUnitDetails(DateTime fromDate, DateTime toDate, int BranchID, int leavecategoryID, int EmployeeID)
        {
            SqlConnection dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
            DataTable dt = new DataTable();
            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "GetUnitDetails";
                dbSqlCommand.Parameters.Add("@fromDate", SqlDbType.DateTime).Value = fromDate;
                dbSqlCommand.Parameters.Add("@toDate", SqlDbType.DateTime).Value = toDate;
                dbSqlCommand.Parameters.Add("@BranchID", SqlDbType.Int).Value = BranchID;
                dbSqlCommand.Parameters.Add("@leavecategoryID", SqlDbType.Int).Value = leavecategoryID;
                dbSqlCommand.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = EmployeeID;
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


        public DataTable checkMapLeave(int leavecategoryID, DateTime fromDate, int EmployeeID)
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
                dbSqlCommand.CommandText = "Getmapleave";
                dbSqlCommand.Parameters.Add("@LeaveCategoryID", SqlDbType.Int).Value = leavecategoryID;
                dbSqlCommand.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromDate;
                dbSqlCommand.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = EmployeeID;
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

        public DataTable GetLeaveBalanceAsOfNowDetails(string employeecode, string branchId, string year)
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
                dbSqlCommand.CommandText = "GetEmployeeLeaveBalanceAsOfNowDetails";
                dbSqlCommand.Parameters.Add("@EmpCode", SqlDbType.VarChar).Value = employeecode;
                dbSqlCommand.Parameters.Add("@BranchId", SqlDbType.VarChar).Value = branchId;
                dbSqlCommand.Parameters.Add("@Year", SqlDbType.VarChar).Value = year;
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
        public DataTable GetLeaveBalanceAsOfNowDetails(string employeecode, string employeeName, string branchId, string year)
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
                dbSqlCommand.CommandText = "GetEmployeeLeaveBalanceAsOfNowDetails";
                dbSqlCommand.Parameters.Add("@EmpCode", SqlDbType.VarChar).Value = employeecode;
                dbSqlCommand.Parameters.Add("@EmpId", SqlDbType.VarChar).Value = employeeName;
                dbSqlCommand.Parameters.Add("@BranchId", SqlDbType.VarChar).Value = branchId;
                dbSqlCommand.Parameters.Add("@Year", SqlDbType.VarChar).Value = year;
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

        public int UpdateLeavesBalance(string EmployeeCode, string LeaveYear, decimal? ELBal, decimal? MLBal, decimal? CLBal, Double? MLExtraBal)
        {
            return db.UpdateLeaveBalanceAsOfNow(EmployeeCode, ELBal, MLBal, CLBal, MLExtraBal, LeaveYear);
        }

        public DataTable chkdateforLE(int EmployeeID)
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
                dbSqlCommand.CommandText = "GetLastAppliedLE";
                dbSqlCommand.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = EmployeeID;
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

        public List<SP_GetEncashmentForF_A_Result> GetLeaveEncashForF_A(DateTime fromDate, DateTime toDate)
        {
            return db.SP_GetEncashmentForF_A(fromDate, toDate).ToList();

        }
        public DataTable GetLeaveEncashment(int year, int? branchId, int? employeeId)
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
                dbSqlCommand.CommandTimeout = 6000;
                dbSqlCommand.CommandText = "SP_GetEncashment";
                dbSqlCommand.Parameters.Add("@year", SqlDbType.VarChar).Value = year;
                dbSqlCommand.Parameters.Add("@branchId", SqlDbType.VarChar).Value = branchId;
                dbSqlCommand.Parameters.Add("@empId", SqlDbType.VarChar).Value = employeeId;
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

        public bool UpdateEncashmentTDS(List<UpdatedTDSYearly> model)
        {
            bool flag = false;
            try
            {
                foreach (var item in model)
                {
                    var empTDS = db.UpdatedTDSYearly.Where(x => x.EmployeeId == item.EmployeeId && x.TDSYear == item.TDSYear).FirstOrDefault();
                    if (empTDS != null)
                    {
                        empTDS.TDS = item.TDS;
                    }
                    else
                    {
                        db.UpdatedTDSYearly.Add(item);
                    }
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
    }
}
