using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;

namespace Nafed.MicroPay.Data.Repositories
{
    public class GenerateSalaryRepository : BaseRepository, IGenerateSalaryRepository
    {
        public List<string> GetEmployeeMasterRecords(int salMonth, int salYear, bool branchesExcecptHO, bool allEmployees, int employeeType, int? branchID, int? employeeID)
        {
            log.Info($"GenerateSalaryRepository/GetEmployeeMasterRecords/{allEmployees}/{allEmployees}/{employeeType}/{branchID}/{employeeID}");
            try
            {
                var endDateOfSelectedMonth = new DateTime(salYear, salMonth, DateTime.DaysInMonth(salYear, salMonth));

                if (allEmployees && branchesExcecptHO)

                    return db.tblMstEmployees.AsNoTracking().Where(x => ((x.BranchID_Pay != null && x.BranchID_Pay != 0) ? x.BranchID_Pay != 44 : x.BranchID != 44)
                      &&
                      (
                          x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                         ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                      )
                      &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                      && !x.IsDeleted && x.EmployeeTypeID == employeeType && !x.IsSalgenrated).Select(y => y.EmployeeCode).ToList();


                else if (allEmployees && branchID.HasValue)

                    return db.tblMstEmployees.AsNoTracking().Where(x => ((x.BranchID_Pay != null && x.BranchID_Pay != 0) ? x.BranchID_Pay == branchID.Value : x.BranchID == branchID.Value)
                     &&
                     (
                        x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                        ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                     )
                     &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                     && x.EmployeeTypeID == employeeType && !x.IsSalgenrated
                     && !x.IsDeleted).Select(y => y.EmployeeCode).ToList();



                else if (allEmployees && !branchesExcecptHO && !branchID.HasValue && !employeeID.HasValue)

                    return db.tblMstEmployees.AsNoTracking().Where(x =>
                     (
                         x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                        ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                     )
                     &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                    && x.Branch.IsDeleted == false
                    && !x.IsDeleted && x.EmployeeTypeID == employeeType && !x.IsSalgenrated)
                    .Select(y => y.EmployeeCode).ToList();


                else if (employeeID.HasValue)
                    return db.tblMstEmployees.AsNoTracking().Where(x => x.EmployeeId == employeeID.Value).Select(y => y.EmployeeCode).ToList();

                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public int NofEmployeeSalaryRows(int salMonth, int salYear, bool branchesExcecptHO, bool allEmployees, int employeeType, int? branchID, int? employeeID, out List<string> empCodes)
        {
            log.Info($"GenerateSalaryRepository/NofEmployeeSalaryRows/{branchesExcecptHO}/{allEmployees}/{employeeType}/{branchID}/{employeeID}");
            try
            {
                var endDateOfSelectedMonth = new DateTime(salYear, salMonth, DateTime.DaysInMonth(salYear, salMonth));

                int noOfRows = 0; empCodes = new List<string>();
                if (allEmployees && branchesExcecptHO)
                {
                    var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x => ((x.BranchID_Pay != null && x.BranchID_Pay != 0) ? x.BranchID_Pay != 44 : x.BranchID != 44)
                     &&
                     (
                         x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                        ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                     )
                     &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                     && !x.IsDeleted && x.EmployeeTypeID == employeeType && !x.IsSalgenrated).Select(y => y.EmployeeCode).AsQueryable();

                    empCodes = db.TblMstEmployeeSalaries.AsNoTracking().Where(y => employeeCodes.Any(r => r == y.EmployeeCode)).Select(m => m.EmployeeCode).ToList();

                    //  noOfRows = db.TblMstEmployeeSalaries.AsNoTracking().Where(y => employeeCodes.Any(r => r == y.EmployeeCode)).Count();
                    noOfRows = empCodes.Count;
                }
                else if (allEmployees && branchID.HasValue)
                {
                    var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x => ((x.BranchID_Pay != null && x.BranchID_Pay != 0) ? x.BranchID_Pay == branchID.Value : x.BranchID == branchID.Value)
                     &&
                     (
                        x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                        ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                     )
                      &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                     && x.EmployeeTypeID == employeeType && !x.IsSalgenrated
                     && !x.IsDeleted).Select(y => y.EmployeeCode).AsQueryable();

                    empCodes = db.TblMstEmployeeSalaries.AsNoTracking().Where(y => employeeCodes.Any(r => r == y.EmployeeCode)).Select(m => m.EmployeeCode).ToList();

                    noOfRows = empCodes.Count;
                    // noOfRows = db.TblMstEmployeeSalaries.AsNoTracking().Where(y => employeeCodes.Any(r => r == y.EmployeeCode)).Count();
                }

                else if (allEmployees && !branchesExcecptHO && !branchID.HasValue && !employeeID.HasValue)
                {
                    var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x =>
                    (
                        x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                       ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                    )
                     &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                   && x.Branch.IsDeleted == false
                   && !x.IsDeleted && x.EmployeeTypeID == employeeType && !x.IsSalgenrated).Select(y => y.EmployeeCode).AsQueryable();

                    empCodes = db.TblMstEmployeeSalaries.AsNoTracking().Where(y => employeeCodes.Any(r => r == y.EmployeeCode)).Select(m => m.EmployeeCode).ToList();

                    noOfRows = empCodes.Count;
                    // noOfRows = db.TblMstEmployeeSalaries.AsNoTracking().Where(y => employeeCodes.Any(r => r == y.EmployeeCode)).Count();
                }
                else if (employeeID.HasValue)
                {
                    var selectedEmpCode = db.tblMstEmployees.AsNoTracking().Where(x => x.EmployeeId == employeeID.Value).FirstOrDefault().EmployeeCode;
                    empCodes = db.TblMstEmployeeSalaries.AsNoTracking().Where(y => y.EmployeeCode == selectedEmpCode).Select(m => m.EmployeeCode).ToList();
                    noOfRows = empCodes.Count;
                }
                return noOfRows;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public int NoOfMonthlyInputRows(bool branchesExcecptHO, bool allEmployees, int salMonth, int salYear, int employeeType, int? branchID, int? employeeID, out List<string> empCodes)
        {
            log.Info($"GenerateSalaryRepository/NoOfMonthlyInputRows/{branchesExcecptHO}/{allEmployees}/{employeeType}/{branchID}/{employeeID}");

            try
            {
                int noOfRows = 0; empCodes = new List<string>();
                var endDateOfSelectedMonth = new DateTime(salYear, salMonth, DateTime.DaysInMonth(salYear, salMonth));

                if (allEmployees && branchesExcecptHO)
                {
                    if (db.TBLMONTHLYINPUTs.Any(x => x.SalYear == salYear && x.SalMonth == salMonth))
                    {
                        var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x => ((x.BranchID_Pay != null && x.BranchID_Pay != 0) ? x.BranchID_Pay != 44 : x.BranchID != 44)
                        &&
                        (
                          x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                          ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                        )
                         &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                        && x.EmployeeTypeID == employeeType && !x.IsDeleted && !x.IsSalgenrated
                        ).Select(y => y.EmployeeCode).AsQueryable();

                        empCodes = db.TBLMONTHLYINPUTs.AsNoTracking().Where(y =>
                          y.SalYear == salYear && y.SalMonth == salMonth && employeeCodes.Any(r => r == y.EmployeeCode)).Select(y => y.EmployeeCode).ToList();

                        noOfRows = empCodes.Count;
                        //noOfRows = db.TBLMONTHLYINPUTs.AsNoTracking().Where(y =>
                        // y.SalYear == salYear && y.SalMonth == salMonth && employeeCodes.Any(r => r == y.EmployeeCode)).Count();
                    }
                }
                else if (allEmployees && branchID.HasValue)
                {
                    if (db.TBLMONTHLYINPUTs.Any(x => x.SalYear == salYear && x.SalMonth == salMonth))
                    {
                        var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x => ((x.BranchID_Pay != null && x.BranchID_Pay != 0) ? x.BranchID_Pay == branchID.Value : x.BranchID == branchID.Value)
                        &&
                        (
                          x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                          ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                        )
                         &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                        && !x.IsDeleted && x.EmployeeTypeID == employeeType
                        && !x.IsSalgenrated
                        ).Select(y => y.EmployeeCode).AsQueryable();

                        //noOfRows = db.TBLMONTHLYINPUTs.AsNoTracking().Where(y =>
                        //y.SalYear == salYear && y.SalMonth == salMonth &&
                        //employeeCodes.Any(r => r == y.EmployeeCode)).Count();

                        empCodes = db.TBLMONTHLYINPUTs.AsNoTracking().Where(y =>
                          y.SalYear == salYear && y.SalMonth == salMonth && employeeCodes.Any(r => r == y.EmployeeCode)).Select(y => y.EmployeeCode).ToList();

                        noOfRows = empCodes.Count;
                    }
                }

                else if (allEmployees && !branchesExcecptHO && !branchID.HasValue && !employeeID.HasValue)
                {
                    var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x =>
                        (
                           x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                          ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                        )
                         &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                        && x.Branch.IsDeleted == false && x.EmployeeTypeID == employeeType && !x.IsDeleted && !x.IsSalgenrated
                        ).Select(y => y.EmployeeCode).AsQueryable();


                    empCodes = db.TBLMONTHLYINPUTs.AsNoTracking().Where(y =>
                          y.SalYear == salYear && y.SalMonth == salMonth && employeeCodes.Any(r => r == y.EmployeeCode))
                          .Select(y => y.EmployeeCode).ToList();

                    noOfRows = empCodes.Count;

                    //noOfRows = db.TBLMONTHLYINPUTs.AsNoTracking().Where(y =>
                    // y.SalYear == salYear && y.SalMonth == salMonth && employeeCodes.Any(r => r == y.EmployeeCode)).Count();
                }
                else if (employeeID.HasValue)
                {
                    var selectedEmpCode = db.tblMstEmployees.AsNoTracking().Where(x => x.EmployeeId == employeeID.Value).FirstOrDefault().EmployeeCode;

                    empCodes = db.TBLMONTHLYINPUTs.AsNoTracking().Where(y => y.EmployeeCode == selectedEmpCode
                               && y.SalMonth == salMonth && y.SalYear == salYear)
                              .Select(y => y.EmployeeCode).ToList();
                    noOfRows = empCodes.Count;
                }
                return noOfRows;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int NoOfLastGeneratedSalaryRows(bool branchesExcecptHO, bool allEmployees, int salMonth, int salYear, int employeeType, int? branchID, int? employeeID)
        {
            log.Info($"GenerateSalaryRepository/NoOfLastGeneratedSalaryRows/{branchesExcecptHO}/{allEmployees}/{employeeType}/{branchID}/{employeeID}");

            try
            {
                int noOfRows = 0;
                var endDateOfSelectedMonth = new DateTime(salYear, salMonth, DateTime.DaysInMonth(salYear, salMonth));

                if (allEmployees && branchesExcecptHO)
                {
                    if (db.TBLMONTHLYINPUTs.Any(x => x.SalYear == salYear && x.SalMonth == salMonth))
                    {
                        var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x => ((x.BranchID_Pay != null && x.BranchID_Pay != 0) ? x.BranchID_Pay != 44 : x.BranchID != 44)
                         &&
                    (
                         x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                        ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                    )
                     &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                        && !x.IsDeleted && x.EmployeeTypeID == employeeType
                        && !x.IsSalgenrated).Select(y => y.EmployeeCode).AsQueryable();

                        noOfRows = db.tblFinalMonthlySalaries.AsNoTracking().Where(y =>
                         y.SalYear == salYear && y.SalMonth == salMonth
                         && y.RecordType.Equals("S", StringComparison.OrdinalIgnoreCase) &&
                        employeeCodes.Any(r => r == y.EmployeeCode)).Count();
                    }
                }
                else if (allEmployees && branchID.HasValue)
                {
                    if (db.TBLMONTHLYINPUTs.Any(x => x.SalYear == salYear && x.SalMonth == salMonth))
                    {
                        var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x => ((x.BranchID_Pay != null && x.BranchID_Pay != 0) ? x.BranchID_Pay == branchID.Value : x.BranchID == branchID.Value)
                         &&
                    (
                         x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                        ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                    )
                     &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                        && !x.IsDeleted && x.EmployeeTypeID == employeeType
                        && !x.IsSalgenrated
                        ).Select(y => y.EmployeeCode).AsQueryable();

                        noOfRows = db.tblFinalMonthlySalaries.AsNoTracking().Where(y =>
                        y.SalYear == salYear && y.SalMonth == salMonth &&
                        y.RecordType.Equals("S", StringComparison.OrdinalIgnoreCase) &&
                        employeeCodes.Any(r => r == y.EmployeeCode)).Count();
                    }
                }

                else if (allEmployees && !branchesExcecptHO && !branchID.HasValue && !employeeID.HasValue)
                {
                    var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x =>
                    (
                         x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                        ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                    )
                     &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                    && x.Branch.IsDeleted == false && !x.IsDeleted && x.EmployeeTypeID == employeeType
                       && !x.IsSalgenrated).Select(y => y.EmployeeCode).AsQueryable();

                    noOfRows = db.tblFinalMonthlySalaries.AsNoTracking().Where(y =>
                     y.SalYear == salYear && y.SalMonth == salMonth
                     && y.RecordType.Equals("S", StringComparison.OrdinalIgnoreCase) &&
                    employeeCodes.Any(r => r == y.EmployeeCode)).Count();
                }
                else if (employeeID.HasValue)
                {
                    var selectedEmpCode = db.tblMstEmployees.AsNoTracking().Where(x => x.EmployeeId == employeeID.Value).FirstOrDefault().EmployeeCode;
                    noOfRows = db.tblFinalMonthlySalaries.AsNoTracking().Where(y => y.EmployeeCode == selectedEmpCode
                    && y.RecordType.Equals("S", StringComparison.OrdinalIgnoreCase)
                    && y.SalMonth == salMonth && y.SalYear == salYear).Count();
                }
                return noOfRows;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateLoanTransData(bool branchesExcecptHO, bool allEmployees, int salMonth, int salYear, int employeeType, int? branchID, int? employeeID)
        {
            log.Info($"GenerateSalaryRepository/UpdateLoanTransData/{branchesExcecptHO}/{allEmployees}/{employeeType}/{branchID}/{employeeID}");
            try
            {
                var periodOfPayment = $"{salYear.ToString()}{salMonth.ToString("00")}";
                int res = db.UpdateLoanTransData(periodOfPayment, (byte?)salMonth, (short?)salYear, branchesExcecptHO, branchID, employeeID, employeeType);
                if (res > 0)
                    log.Info($"Result of procedure -'dbo.UpdateLoanTransData' execution- {res} rows(s) affected");
            }
            catch (Exception)
            {
                throw;
            }

        }

        public DataTable RevertProcessedLoanEntries(bool branchesExcecptHO, bool allBranches, bool allEmployees, int salMonth, int salYear, int? branchID, int? employeeID, int employeeTypeID)
        {
            log.Info($"GenerateSalaryRepository/RevertProcessedLoanEntries/{branchesExcecptHO}/{allEmployees}/{branchID}/{employeeID}");
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
                dbSqlCommand.CommandText = "RevertSalaryLoanEntries";

                dbSqlCommand.Parameters.AddWithValue("@salMonth", salMonth);
                dbSqlCommand.Parameters.AddWithValue("@salYear", salYear);

                dbSqlCommand.Parameters.AddWithValue("@allBranch", allBranches);
                dbSqlCommand.Parameters.AddWithValue("@allBranchExceptHO", branchesExcecptHO);
                dbSqlCommand.Parameters.AddWithValue("@selectedBranchID", branchID);
                dbSqlCommand.Parameters.AddWithValue("@employeeType", employeeTypeID);


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

        public void InsertIntoLoanPriorityHistory(string period)
        {
            log.Info($"GenerateSalaryRepository/InsertIntoLoanPriorityHistory{period}");
            try
            {
                int res = db.InsertIntoLoanPriorityHistory(period);
                if (res > 0)
                    log.Info($"Result of procedure -'dbo.InsertIntoLoanPriorityHistory' execution- {res} rows(s) inserted into history table.");
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// =====  Added changes to pass "SalMonth" , "SalYear" ======= (Sg --- 22 Jun 2020)
        /// </summary>
        /// <param name="salMonth"></param>
        /// <param name="salYear"></param>
        /// <param name="branchesExcecptHO"></param>
        /// <param name="allEmployees"></param>
        /// <param name="employeeType"></param>
        /// <param name="branchID"></param>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public List<TblMstEmployeeSalary> GetMonthlySalaryList(int salMonth, int salYear, bool branchesExcecptHO, bool allEmployees, int employeeType, int? branchID, int? employeeID)
        {
            log.Info($"GenerateSalaryRepository/GetMonthSalaryList/{branchesExcecptHO}/{allEmployees}/{employeeType}/{branchID}/{employeeID}");
            try
            {
                List<TblMstEmployeeSalary> empSalaryList = new List<TblMstEmployeeSalary>();
                var endDateOfSelectedMonth = new DateTime(salYear, salMonth, DateTime.DaysInMonth(salYear, salMonth));

                if (allEmployees && branchesExcecptHO)
                {
                    var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x => ((x.BranchID_Pay != null && x.BranchID_Pay != 0) ? x.BranchID_Pay != 44 : x.BranchID != 44)
                    &&
                    (
                         x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                        ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                    )
                     &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                     && !x.IsDeleted && x.EmployeeTypeID == employeeType
                     && !x.IsSalgenrated

                     ).Select(y => y.EmployeeCode).AsQueryable();
                    empSalaryList = db.TblMstEmployeeSalaries.AsNoTracking().Where(y => employeeCodes.Any(r => r == y.EmployeeCode)).ToList();
                }
                else if (allEmployees && branchID.HasValue)
                {
                    var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x => ((x.BranchID_Pay != null && x.BranchID_Pay != 0) ? x.BranchID_Pay == branchID.Value : x.BranchID == branchID.Value)
                    &&
                    (
                         x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                        ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                    )
                     &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                    && !x.IsDeleted && x.EmployeeTypeID == employeeType && !x.IsSalgenrated

                    ).Select(y => y.EmployeeCode).AsQueryable();
                    empSalaryList = db.TblMstEmployeeSalaries.AsNoTracking().Where(y => employeeCodes.Any(r => r == y.EmployeeCode)).ToList();
                }

                else if (allEmployees && !branchesExcecptHO && !branchID.HasValue && !employeeID.HasValue)
                {
                    var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x =>
                     (
                         x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                        ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                     )
                      &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                    && x.Branch.IsDeleted == false && !x.IsDeleted && x.EmployeeTypeID == employeeType && !x.IsSalgenrated)
                      .Select(y => y.EmployeeCode).AsQueryable();
                    empSalaryList = db.TblMstEmployeeSalaries.AsNoTracking().Where(y => employeeCodes.Any(r => r == y.EmployeeCode)).ToList();

                }

                else if (employeeID.HasValue)
                {
                    var selectedEmpCode = db.tblMstEmployees.AsNoTracking().Where(x => x.EmployeeId == employeeID.Value).FirstOrDefault().EmployeeCode;
                    empSalaryList = db.TblMstEmployeeSalaries.Where(y => y.EmployeeCode == selectedEmpCode).ToList();
                }

                var res = (from empSal in empSalaryList
                           join y in db.tblMstEmployees.AsNoTracking() on empSal.EmployeeCode equals y.EmployeeCode
                           where y.IsDeleted == false && (y.DOLeaveOrg == null || (y.DOLeaveOrg.HasValue ?
                           (y.DOLeaveOrg.Value.Year == salYear && (y.DOLeaveOrg.Value.Month == salMonth || y.DOLeaveOrg.Value.Month > salMonth))
                           : (y.DOLeaveOrg == null)
                           )
                           && (y.DOJ <= endDateOfSelectedMonth))
                           orderby y.Branch.BranchCode, y.Sen_Code descending, empSal.E_Basic descending, y.PFNO
                           select empSal).ToList();


                //foreach (var item in res)
                //{
                //    Debug.WriteLine(item.EmployeeCode);
                //}

                //Debug.WriteLine("=========");

                //foreach (var item in empSalaryList)
                //{
                //    Debug.WriteLine(item.EmployeeCode);
                //}

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<TBLMONTHLYINPUT> GetMonthlyInputList(bool branchesExcecptHO, bool allEmployees, int salMonth, int salYear, int employeeType, int? branchID, int? employeeID)
        {
            log.Info($"GenerateSalaryRepository/GetMonthlyInputList/{branchesExcecptHO}/{allEmployees}/{employeeType}/{branchID}/{employeeID}");

            try
            {
                List<TBLMONTHLYINPUT> monthlyInputList = new List<TBLMONTHLYINPUT>();
                var endDateOfSelectedMonth = new DateTime(salYear, salMonth, DateTime.DaysInMonth(salYear, salMonth));

                if (allEmployees && branchesExcecptHO)
                {
                    if (db.TBLMONTHLYINPUTs.Any(x => x.SalYear == salYear && x.SalMonth == salMonth))
                    {
                        var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x => ((x.BranchID_Pay != null && x.BranchID_Pay != 0) ? x.BranchID_Pay != 44 : x.BranchID != 44)
                        &&
                        (
                             x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                            ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                        )
                         &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                         && !x.IsDeleted && x.EmployeeTypeID == employeeType && !x.IsSalgenrated
                        ).Select(y => y.EmployeeCode).AsQueryable();

                        monthlyInputList = db.TBLMONTHLYINPUTs.AsNoTracking().Where(y =>
                         y.SalYear == salYear && y.SalMonth == salMonth &&
                        employeeCodes.Any(r => r == y.EmployeeCode)).ToList();
                    }
                }
                else if (allEmployees && branchID.HasValue)
                {
                    if (db.TBLMONTHLYINPUTs.Any(x => x.SalYear == salYear && x.SalMonth == salMonth))
                    {
                        var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x => ((x.BranchID_Pay != null && x.BranchID_Pay != 0) ? x.BranchID_Pay == branchID.Value : x.BranchID == branchID.Value)
                        &&
                        (
                             x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                            ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                        )
                         &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                        && x.EmployeeTypeID == employeeType
                        && !x.IsSalgenrated && !x.IsDeleted
                        ).Select(y => y.EmployeeCode).AsQueryable();

                        monthlyInputList = db.TBLMONTHLYINPUTs.AsNoTracking().Where(y =>
                        y.SalYear == salYear && y.SalMonth == salMonth &&
                        employeeCodes.Any(r => r == y.EmployeeCode)).ToList();
                    }
                }

                else if (allEmployees && !branchesExcecptHO && !branchID.HasValue && !employeeID.HasValue)
                {
                    if (db.TBLMONTHLYINPUTs.Any(x => x.SalYear == salYear && x.SalMonth == salMonth))
                    {
                        var employeeCodes = db.tblMstEmployees.AsNoTracking().Where(x =>
                        (
                             x.DOLeaveOrg == null || (x.DOLeaveOrg.HasValue ?
                            ((x.DOLeaveOrg.Value.Month == salMonth || x.DOLeaveOrg.Value.Month > salMonth) && x.DOLeaveOrg.Value.Year == salYear) : (x.DOLeaveOrg == null))
                        )
                         &&
                     (
                        x.DOJ <= endDateOfSelectedMonth
                     )
                       && x.Branch.IsDeleted == false && !x.IsDeleted && x.EmployeeTypeID == employeeType && !x.IsSalgenrated
                       ).Select(y => y.EmployeeCode).AsQueryable();

                        monthlyInputList = db.TBLMONTHLYINPUTs.AsNoTracking().Where(y =>
                         y.SalYear == salYear && y.SalMonth == salMonth &&
                        employeeCodes.Any(r => r == y.EmployeeCode)).ToList();
                    }
                }
                else if (employeeID.HasValue)
                {
                    var selectedEmpCode = db.tblMstEmployees.AsNoTracking().Where(x => x.EmployeeId == employeeID.Value).FirstOrDefault().EmployeeCode;
                    monthlyInputList = db.TBLMONTHLYINPUTs.AsNoTracking().Where(y => y.EmployeeCode == selectedEmpCode && y.SalMonth == salMonth && y.SalYear == salYear).ToList();
                }
                return monthlyInputList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public DataSet GetLoanPriorityMasterData(DataTable empCodeDT, string periofOfPayment)
        {
            log.Info($"GenerateSalaryRepository/GetLoanPriorityMasterData");

            DataSet dbSqlDataSet = new DataSet();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[GetLoanPriorityMasterData]";

                SqlParameter paramItemCode = dbSqlCommand.Parameters.AddWithValue("@tblEmpCode", empCodeDT);
                paramItemCode.SqlDbType = SqlDbType.Structured;
                paramItemCode.TypeName = "[udtGenericStringList]";

                dbSqlCommand.Parameters.AddWithValue("@periodOfPayment", periofOfPayment);

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataSet);
                dbSqlconnection.Close();

                return dbSqlDataSet;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }


        }


        public DataSet GetLoanPriorityHistoryData(DataTable empCodeDT, string periofOfPayment, string currentPeriod)
        {
            log.Info($"GenerateSalaryRepository/GetLoanPriorityHistoryData");

            DataSet dbSqlDataSet = new DataSet();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[GetLoanPriorityHistoryData]";

                SqlParameter paramItemCode = dbSqlCommand.Parameters.AddWithValue("@tblEmpCode", empCodeDT);
                paramItemCode.SqlDbType = SqlDbType.Structured;
                paramItemCode.TypeName = "[udtGenericStringList]";

                dbSqlCommand.Parameters.AddWithValue("@periodOfPayment", periofOfPayment);
                dbSqlCommand.Parameters.AddWithValue("@currentPeriod", currentPeriod);
                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataSet);
                dbSqlconnection.Close();

                return dbSqlDataSet;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }


        }


        public DataTable GetOTARates()
        {
            log.Info($"GenerateSalaryRepository/GetOTARates");

            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.Text;
                dbSqlCommand.CommandText = @"Select E.employeecode,E.Otacode,R.MaxRatePerHour,R.MaxAmt from tblmstEmployee E 
                                inner join tblOtarates R on e.otacode = r.otacode where e.OtaCode is not null";

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

        public bool UpdateSalaryLoanData(DataTable newLoanPriorityDT, DataTable newLoanTransDT, DataTable oldLoanPriorityDT, DataTable oldLoanTransDT)
        {
            log.Info($"GenerateSalaryRepository/UpdateSalaryLoanData");

            DataTable dt_newLoanPriorityDT = new DataTable();
            DataTable dt_newLoanTransDT = new DataTable();
            DataTable dt_oldLoanPriorityDT = new DataTable();
            DataTable dt_oldLoanTransDT = new DataTable();

            if (newLoanPriorityDT != null)
                dt_newLoanPriorityDT = RemoveDuplicatesRecords(newLoanPriorityDT);

            else
                dt_newLoanPriorityDT = newLoanPriorityDT;

            if (newLoanTransDT != null)
                dt_newLoanTransDT = RemoveDuplicatesRecords(newLoanTransDT);

            else
                dt_newLoanTransDT = newLoanTransDT;

            if (oldLoanPriorityDT != null)
                dt_oldLoanPriorityDT = RemoveDuplicatesRecords(oldLoanPriorityDT);

            else
                dt_oldLoanPriorityDT = oldLoanPriorityDT;

            if (oldLoanTransDT != null)
                dt_oldLoanTransDT = RemoveDuplicatesRecords(oldLoanTransDT);

            else
                dt_oldLoanTransDT = oldLoanTransDT;

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
                dbSqlCommand.CommandText = "[UpdateEmpSalaryLoanData]";

                SqlParameter newLoanPriority = dbSqlCommand.Parameters.AddWithValue("@newLoanPriorityDT", dt_newLoanPriorityDT);
                newLoanPriority.SqlDbType = SqlDbType.Structured;
                newLoanPriority.TypeName = "[udtLoanPriority]";




                SqlParameter newLoanTrans = dbSqlCommand.Parameters.AddWithValue("@newLoanTransDT", dt_newLoanTransDT);
                newLoanTrans.SqlDbType = SqlDbType.Structured;
                newLoanTrans.TypeName = "[udtLoanTrans]";


                SqlParameter oldLoanPriority = dbSqlCommand.Parameters.AddWithValue("@oldLoanPriorityDT", dt_oldLoanPriorityDT);
                newLoanPriority.SqlDbType = SqlDbType.Structured;
                newLoanPriority.TypeName = "[udtLoanPriority]";

                SqlParameter oldLoanTrans = dbSqlCommand.Parameters.AddWithValue("@oldLoanTransDT", dt_oldLoanTransDT);
                newLoanTrans.SqlDbType = SqlDbType.Structured;
                newLoanTrans.TypeName = "[udtLoanTrans]";


                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlconnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public DataTable GetPFOpBalance(int prevYear, byte prevMonth, DataTable dtEmpCodes)
        {
            log.Info($"GenerateSalaryRepository/GetPFOpBalance");

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
                dbSqlCommand.CommandText = "[GetEmpPFOpBalance]";

                SqlParameter paramItemCode = dbSqlCommand.Parameters.AddWithValue("@tblEmpCode", dtEmpCodes);
                paramItemCode.SqlDbType = SqlDbType.Structured;
                paramItemCode.TypeName = "[udtGenericStringList]";

                dbSqlCommand.Parameters.AddWithValue("@salMonth", prevMonth);
                dbSqlCommand.Parameters.AddWithValue("@salYear", prevYear);
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


        private DataTable RemoveDuplicatesRecords(DataTable dt)
        {
            //Returns just 5 unique rows
            var UniqueRows = dt.AsEnumerable().Distinct(DataRowComparer.Default);
            DataTable dt2 = UniqueRows.CopyToDataTable();
            return dt2;
        }

        public void UpdateEmpPensionDeduction(int salMonth, int salYear, bool branchesExcecptHO, bool allEmployees, int employeeType, int? branchID, int? employeeID)
        {
            log.Info($"GenerateSalaryRepository/UpdateLoanTransData/{branchesExcecptHO}/{allEmployees}/{employeeType}/{branchID}/{employeeID}");
            try
            {
                string filterType = string.Empty;

                if (allEmployees && branchesExcecptHO)
                    filterType = "EH";

                else if (allEmployees && branchID.HasValue)
                    filterType = "B";

                else if (allEmployees && !branchesExcecptHO && !branchID.HasValue && !employeeID.HasValue)
                    filterType = "A";
                else if (employeeID.HasValue)
                {
                    filterType = "A";
                }
                int res = db.UpdateEmpPensionDeduct(salMonth, (short?)salYear, filterType, employeeType, branchID, employeeID);
            }
            catch
            {
            }
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

                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlCommand.ExecuteNonQuery();
                dbSqlconnection.Close();

                //using (SqlConnection dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString))
                //{
                //    try
                //    {
                //        SqlCommand dbSqlCommand;
                //        dbSqlCommand = new SqlCommand();
                //        dbSqlCommand.Connection = dbSqlconnection;
                //        dbSqlCommand.CommandType = CommandType.StoredProcedure;
                //        dbSqlCommand.CommandText = "UpdateDateBranch_Pay";
                //        dbSqlCommand.Parameters.Add("@SalMonth", SqlDbType.Int).Value = salMonth;
                //        dbSqlCommand.Parameters.Add("@SalYear", SqlDbType.Int).Value = salYear;
                //        if (dbSqlconnection.State == ConnectionState.Closed)
                //            dbSqlconnection.Open();
                //        dbSqlCommand.ExecuteNonQuery();
                //        dbSqlconnection.Close();

                //    }
                //    catch (Exception)
                //    {
                //        throw;
                //    }
                //    finally
                //    {
                //        if (dbSqlconnection.State == ConnectionState.Open)
                //        {
                //            dbSqlconnection.Close();
                //        }
                //    }
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
