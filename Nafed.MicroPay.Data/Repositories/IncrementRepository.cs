using Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using System.Data;
using System.Data.SqlClient;



namespace Nafed.MicroPay.Data.Repositories
{
    public class IncrementRepository : BaseRepository, IIncrementRepository
    {
        public int InsertProjectedEmployee()
        {
            return db.sp_delProjectedcreated_table();
        }

        public IEnumerable<GetProjectedIncrementDetails_Result> GetProjectedIncrementDetails(int? BranchID, string EmployeeCode, string EmployeeName, int? incrementMonthId)
        {
            return db.GetProjectedIncrementDetails(BranchID, EmployeeCode, EmployeeName, incrementMonthId).ToList();
        }


        public IEnumerable<GetUpdateIncrementDetails_Result> GetUpdateIncrementDetails(int? BranchID, string EmployeeCode, string EmployeeName, int? incrementMonthId)
        {
            return db.GetUpdateIncrementDetails(BranchID, EmployeeCode, EmployeeName, incrementMonthId).ToList();
        }

        public bool UpdateProjectedEmployeeSalaryDetails(List<DTOModel.tblmstProjectedEmployeeSalary> lstProjectedEmployeeSalary)
        {
            bool flag = false;
            try
            {
                var oldProjectedEmployeeSalary = db.tblmstProjectedEmployeeSalaries.Where(x => x.EmployeeID != null).ToList();
                oldProjectedEmployeeSalary.RemoveAll(item => item == null);
                lstProjectedEmployeeSalary.Join(oldProjectedEmployeeSalary, PPSD => new
                {
                    EmployeeId = PPSD.EmployeeID.Value
                }, OPES => new
                {
                    EmployeeId = OPES.EmployeeID.Value
                },
                (PPSD, OPES) => new { PPSD, OPES }).ToList().ForEach(X =>
                {
                    X.OPES.E_Basic = X.PPSD.E_Basic;
                    X.OPES.LastBasic = X.PPSD.LastBasic;
                    X.OPES.LastIncrement = X.PPSD.LastIncrement;
                    X.OPES.LastIncrementDate = DateTime.Now;
                });
                db.SaveChanges();
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdateEmployeeSalaryDetails(List<DTOModel.TblMstEmployeeSalary> employeeSalaryDetails)
        {
            bool flag = false;
            try
            {
                var oldEmployeeSalary = db.TblMstEmployeeSalaries.Where(x => x.EmployeeID != null).ToList();
                oldEmployeeSalary.RemoveAll(item => item == null);
                employeeSalaryDetails.Join(oldEmployeeSalary, PPSD => new
                {
                    EmployeeId = PPSD.EmployeeID
                }, OPES => new
                {
                    EmployeeId = OPES.EmployeeID
                },
                (PPSD, OPES) => new { PPSD, OPES }).ToList().ForEach(X =>
                {
                    X.OPES.E_Basic = X.PPSD.E_Basic;
                    X.OPES.LastBasic = X.PPSD.LastBasic;
                    X.OPES.LastIncrement = X.PPSD.LastIncrement;
                    X.OPES.LastIncrementDate = DateTime.Now;
                });
                db.SaveChanges();
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public IEnumerable<GetValidateNewBasicAmountDetails_Result> GetValidateNewBasicAmountDetails(int? employeeId, int? incrementMonth)
        {
            return db.GetValidateNewBasicAmountDetails(employeeId, incrementMonth).ToList();
        }

        public bool UpdateStopIncrementDetails(List<DTOModel.tblMstEmployee> lstStopIncrementEmployeeDetails)
        {
            bool flag = false;
            try
            {
                var oldProjectedEmployee = db.tblmstprojectedemployees.ToList();
                var oldMstEmployee = db.tblMstEmployees.ToList();

                if (oldProjectedEmployee.Count > 0)
                {
                    lstStopIncrementEmployeeDetails.Join(oldProjectedEmployee, PPSD => new
                    {
                        EmployeeId = PPSD.EmployeeId
                    }, OPES => new
                    {
                        EmployeeId = OPES.EmployeeId
                    },
                    (PPSD, OPES) => new { PPSD, OPES }).ToList().ForEach(X =>
                    {
                        X.OPES.Reason = X.PPSD.Reason;
                        X.OPES.ValidateIncrement = X.PPSD.ValidateIncrement;
                        X.OPES.StopIncrementEffectiveDate = X.PPSD.StopIncrementEffectiveDate;
                        X.OPES.ToDate = X.PPSD.ToDate;
                    });
                }

                lstStopIncrementEmployeeDetails.Join(oldMstEmployee, PPSD => new
                {
                    EmployeeId = PPSD.EmployeeId
                }, OPES => new
                {
                    EmployeeId = OPES.EmployeeId
                },
                (PPSD, OPES) => new { PPSD, OPES }).ToList().ForEach(X =>
                {
                    X.OPES.Reason = X.PPSD.Reason;
                    X.OPES.ValidateIncrement = X.PPSD.ValidateIncrement;
                    X.OPES.StopIncrementEffectiveDate = X.PPSD.StopIncrementEffectiveDate;
                    X.OPES.ToDate = X.PPSD.ToDate;
                });
                db.SaveChanges();
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public IEnumerable<GetUpdateValidateNewBasicAmountDetails_Result> GetUpdateValidateNewBasicAmountDetails(int? employeeId, int? incrementMonth)
        {
            return db.GetUpdateValidateNewBasicAmountDetails(employeeId, incrementMonth).ToList();
        }

        public DataTable GetExportApplicableIncrement(int? branchId, int? incrementMonth, string employeeName, string employeeCode, string fileName, string fullPath, string type)
        {
            log.Info("IncrementRepository/GetExportApplicableIncrement");
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
                dbSqlCommand.CommandText = "GetApplicableIncrementExport";
                dbSqlCommand.Parameters.Add("@BranchID", SqlDbType.Int).Value = branchId;
                dbSqlCommand.Parameters.Add("@EmployeeCode", SqlDbType.VarChar).Value = employeeCode;
                dbSqlCommand.Parameters.Add("@EmployeeName", SqlDbType.VarChar).Value = employeeName;
                dbSqlCommand.Parameters.Add("@IncrementMonth", SqlDbType.Int).Value = incrementMonth;
                dbSqlCommand.Parameters.Add("@Type", SqlDbType.VarChar).Value = type;
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
    }
}
