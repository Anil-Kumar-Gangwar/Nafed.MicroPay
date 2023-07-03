using Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using DTOModel = Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Data.Repositories
{
    public class SalaryRepository : BaseRepository, ISalaryRepository
    {
        public int InsertMonthlyInput(Int16 currentYear, Int16 currentMonth, Int16 PreYear, Int16 PreMonth)
        {
            return db.SP_DummyMonthlyInput(currentMonth, currentYear, PreMonth, PreYear);
        }

        public int UpdateSalaryHeadsInMonthlyInput(string Head, Int16 currentMonth, Int16 currentYear, string flag)
        {
            return db.SP_UpdateSalaryHeadsInMonthlyInput(Head, currentMonth, currentYear, flag);
        }

        public DataTable GetBranchDetails(string SalaryHead, Int16 currentMonth, Int16 currentYear, int EmployeeId)
        {
            log.Info("SalaryRepository/GetBranchDetails");
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
                dbSqlCommand.CommandText = "GetEmpBranchAndAmountDetails";
                dbSqlCommand.Parameters.Add("@SalaryHead", SqlDbType.VarChar).Value = SalaryHead;
                dbSqlCommand.Parameters.Add("@currentMonth", SqlDbType.SmallInt).Value = currentMonth;
                dbSqlCommand.Parameters.Add("@currentYear", SqlDbType.SmallInt).Value = currentYear;
                dbSqlCommand.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = EmployeeId;
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
        public int UpdateSalaryMonthlyInput(int EmployeeId, string MonthlyInputHeadId, decimal Amount, int currentMonth, int currentYear)
        {
            return db.SP_UpdateSalaryMonthlyInput(EmployeeId, MonthlyInputHeadId, Amount, currentMonth, currentYear);
        }

        public decimal? GetMedicalReimbursement(int EmployeeId, string fromYear, string toYear)
        {
            return db.SP_GetMedicalReimbursement(EmployeeId, fromYear, toYear).FirstOrDefault().HasValue ? db.SP_GetMedicalReimbursement(EmployeeId, fromYear, toYear).FirstOrDefault().Value : (decimal?)0.0;
        }

        public DataTable GetBranchCodeDetails(Int16 PreMonth, Int16 PreYear)
        {
            log.Info("SalaryRepository/GetBranchCodeDetails");
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
                dbSqlCommand.CommandText = "GetBranchCodeDetails";
                dbSqlCommand.Parameters.Add("@PreMonth", SqlDbType.VarChar).Value = PreMonth;
                dbSqlCommand.Parameters.Add("@PreYear", SqlDbType.VarChar).Value = PreYear;
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

        
        public IEnumerable<GetSanctionLoanList_Result> GetSanctionLoanList()
        {
            return db.GetSanctionLoanList();
        }

        public bool DeleteSanction(string priorityNo, DateTime DateAvailLoan)
        {
            bool flag = false;
            var loanDetails = db.tblMstLoanPriorities.SingleOrDefault(x => x.PriorityNo == priorityNo && DbFunctions.TruncateTime(x.DateAvailLoan) == DbFunctions.TruncateTime(DateAvailLoan));
            db.tblMstLoanPriorities.Remove(loanDetails);
            var period = db.tblMstLoanPriorityHistories.Max(x => x.period);
            var loanPriorityHistory = db.tblMstLoanPriorityHistories.Where(x => x.PriorityNo == priorityNo && DbFunctions.TruncateTime(x.DateAvailLoan) == DbFunctions.TruncateTime(DateAvailLoan) && x.period == period).FirstOrDefault();
            db.tblMstLoanPriorityHistories.Remove(loanPriorityHistory);
            db.SaveChanges();
            flag = true;
            return flag;
        }

        public List<AnonymousSelectList> GetEmployeeByLoanType(int loanTypeId, bool oldLoanEmployee)
        {
            if (!oldLoanEmployee)
            {
                var employeeRecord = from emp in db.tblMstEmployees
                                     join fs in db.tblMstLoanPriorities on emp.EmployeeId equals fs.EmployeeID
                                     where (fs.Status == false /*&& fs.IsNewLoanAfterDevelop == true*/ && fs.LoanTypeId == loanTypeId && emp.EmployeeTypeID == 5 && !emp.IsDeleted && emp.DOLeaveOrg == null)
                                     group emp by new
                                     {
                                         emp.EmployeeId,
                                         emp.Name,
                                         emp.EmployeeCode
                                     } into gcs
                                     select new AnonymousSelectList
                                     {
                                         id = gcs.Key.EmployeeId,
                                         value = gcs.Key.EmployeeCode + "-" + gcs.Key.Name,
                                     };
                return employeeRecord.ToList();
            }
            else
            {
                var employeeRecord = from emp in db.tblMstEmployees
                                     join fs in db.tblMstLoanPriorities on emp.EmployeeId equals fs.EmployeeID
                                     where (fs.Status == true && fs.LoanTypeId == loanTypeId && emp.EmployeeTypeID == 5 && !emp.IsDeleted && emp.DOLeaveOrg == null)
                                     group emp by new
                                     {
                                         emp.EmployeeId,
                                         emp.Name,
                                         emp.EmployeeCode
                                     } into gcs
                                     select new AnonymousSelectList
                                     {
                                         id = gcs.Key.EmployeeId,
                                         value = gcs.Key.EmployeeCode + "-" + gcs.Key.Name,
                                     };
                return employeeRecord.ToList();
            }
        }

        public bool UpdateSalaryPublishField(int? selectedEmployeeID, int? selectedBranchID, int? selectedEmployeeTypeID, int salMonth, int salYear, string buttonType)
        {
            try
            {
                using (SqlConnection dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    try
                    {
                        SqlCommand dbSqlCommand;
                        dbSqlCommand = new SqlCommand();
                        dbSqlCommand.Connection = dbSqlconnection;
                        dbSqlCommand.CommandType = CommandType.StoredProcedure;
                        dbSqlCommand.CommandText = "UpdateSalaryPublishField";
                        dbSqlCommand.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = selectedEmployeeID;
                        dbSqlCommand.Parameters.Add("@BranchID", SqlDbType.Int).Value = selectedBranchID;
                        dbSqlCommand.Parameters.Add("@EmpTypeID", SqlDbType.Int).Value = selectedEmployeeTypeID;
                        dbSqlCommand.Parameters.Add("@salMonth", SqlDbType.Int).Value = salMonth;
                        dbSqlCommand.Parameters.Add("@salYear", SqlDbType.Int).Value = salYear;
                        dbSqlCommand.Parameters.Add("@buttonType", SqlDbType.VarChar).Value = buttonType;

                        if (dbSqlconnection.State == ConnectionState.Closed)
                            dbSqlconnection.Open();
                        dbSqlCommand.ExecuteNonQuery();
                        dbSqlconnection.Close();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                        throw;
                    }
                    finally
                    {
                        if (dbSqlconnection.State == ConnectionState.Open)
                        {
                            dbSqlconnection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool GeneralReports(string branchCode, int month, int year, string headName, string headDesc, string slcode, string payslipno, int employeeTypeId)
        {
            log.Info($"SalaryRepository/GeneralReports");
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "GeneralReports";
                dbSqlCommand.Parameters.AddWithValue("@BRANCHCODE", branchCode);
                dbSqlCommand.Parameters.AddWithValue("@SALMONTH", month);
                dbSqlCommand.Parameters.AddWithValue("@SALYEAR", year);
                dbSqlCommand.Parameters.AddWithValue("@HEADNAME", headName);
                dbSqlCommand.Parameters.AddWithValue("@HEADDESC", headDesc);
                dbSqlCommand.Parameters.AddWithValue("@SLCODE", slcode);
                dbSqlCommand.Parameters.AddWithValue("@PAYSLIPNO", payslipno);
                dbSqlCommand.Parameters.AddWithValue("@EMPTYPEID", employeeTypeId);
                dbSqlconnection.Open();
                dbSqlCommand.ExecuteNonQuery();
                dbSqlconnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        public bool UpdatePublishDAArrer(int? selectedEmployeeID, int? selectedBranchID, int? selectedEmployeeTypeID, int salMonth, int salYear, string buttonType)
        {
            try
            {
                using (SqlConnection dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    try
                    {
                        SqlCommand dbSqlCommand;
                        dbSqlCommand = new SqlCommand();
                        dbSqlCommand.Connection = dbSqlconnection;
                        dbSqlCommand.CommandType = CommandType.StoredProcedure;
                        dbSqlCommand.CommandText = "UpdatePublishDAArrer";
                        dbSqlCommand.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = selectedEmployeeID;
                        dbSqlCommand.Parameters.Add("@BranchID", SqlDbType.Int).Value = selectedBranchID;
                        dbSqlCommand.Parameters.Add("@EmpTypeID", SqlDbType.Int).Value = selectedEmployeeTypeID;
                        dbSqlCommand.Parameters.Add("@salMonth", SqlDbType.Int).Value = salMonth;
                        dbSqlCommand.Parameters.Add("@salYear", SqlDbType.Int).Value = salYear;
                        dbSqlCommand.Parameters.Add("@buttonType", SqlDbType.VarChar).Value = buttonType;

                        if (dbSqlconnection.State == ConnectionState.Closed)
                            dbSqlconnection.Open();
                        dbSqlCommand.ExecuteNonQuery();
                        dbSqlconnection.Close();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                        throw;
                    }
                    finally
                    {
                        if (dbSqlconnection.State == ConnectionState.Open)
                        {
                            dbSqlconnection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdatePublishPayArrer(int? selectedEmployeeID, int? selectedBranchID, int? selectedEmployeeTypeID, int salMonth, int salYear, string buttonType)
        {
            try
            {
                using (SqlConnection dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    try
                    {
                        SqlCommand dbSqlCommand;
                        dbSqlCommand = new SqlCommand();
                        dbSqlCommand.Connection = dbSqlconnection;
                        dbSqlCommand.CommandType = CommandType.StoredProcedure;
                        dbSqlCommand.CommandText = "UpdatePublishPayArrer";
                        dbSqlCommand.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = selectedEmployeeID;
                        dbSqlCommand.Parameters.Add("@BranchID", SqlDbType.Int).Value = selectedBranchID;
                        dbSqlCommand.Parameters.Add("@EmpTypeID", SqlDbType.Int).Value = selectedEmployeeTypeID;
                        dbSqlCommand.Parameters.Add("@salMonth", SqlDbType.Int).Value = salMonth;
                        dbSqlCommand.Parameters.Add("@salYear", SqlDbType.Int).Value = salYear;
                        dbSqlCommand.Parameters.Add("@buttonType", SqlDbType.VarChar).Value = buttonType;

                        if (dbSqlconnection.State == ConnectionState.Closed)
                            dbSqlconnection.Open();
                        dbSqlCommand.ExecuteNonQuery();
                        dbSqlconnection.Close();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                        throw;
                    }
                    finally
                    {
                        if (dbSqlconnection.State == ConnectionState.Open)
                        {
                            dbSqlconnection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetArrearPeriodsDetails(string arrerType)
        {
            log.Info("SalaryRepository/GetArrearPeriodsDetails");
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
                dbSqlCommand.CommandText = "getArrearperiodsdetails";
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

        public DataTable PFLoanSummary(int month, int year,int ? toMonth)
        {
            log.Info("SalaryRepository/PFLoanSummary");
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
                dbSqlCommand.CommandText = "SP_PFLoanSummary";
                dbSqlCommand.Parameters.Add("@Month", SqlDbType.TinyInt).Value = month;
                dbSqlCommand.Parameters.Add("@Year", SqlDbType.SmallInt).Value = year;
                dbSqlCommand.Parameters.Add("@ToMonth", SqlDbType.TinyInt).Value = toMonth;
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


        #region Yearly Report
        public DataTable GetPFOpBalance(string fYear, string toYear)
        {
            log.Info("SalaryRepository/GetPFOpBalance");
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
                dbSqlCommand.CommandText = "SP_GetPFOpBalance";
                dbSqlCommand.Parameters.Add("@fromYear", SqlDbType.VarChar).Value = fYear;
                dbSqlCommand.Parameters.Add("@toYear", SqlDbType.VarChar).Value = toYear;
                dbSqlDataTable = new DataTable();
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

        public DataTable GetPFOpBalanceIU(string fYear, string toYear)
        {
            log.Info("SalaryRepository/GetPFOpBalanceIU");
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
                dbSqlCommand.CommandText = "SP_GetPFOpBalanceIU";
                dbSqlCommand.Parameters.Add("@fromYear", SqlDbType.VarChar).Value = fYear;
                dbSqlCommand.Parameters.Add("@toYear", SqlDbType.VarChar).Value = toYear;
                dbSqlDataTable = new DataTable();
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

        public DataTable GetEDLIStatement(int mon, int yr1)
        {
            log.Info("SalaryRepository/GetEDLIStatement");
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
                dbSqlCommand.CommandText = "SP_GetEDLIStatement";
                dbSqlCommand.Parameters.Add("@mon", SqlDbType.Int).Value = mon;
                dbSqlCommand.Parameters.Add("@yr1", SqlDbType.Int).Value = yr1;
                dbSqlDataTable = new DataTable();
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

        public bool Update_FORM7PSdata(string fryear, string tryear)
        {
            log.Info($"SalaryRepository/Update_FORM7PSdata");
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "PFLOAN2";
                dbSqlCommand.Parameters.AddWithValue("@fromperiod", fryear);
                dbSqlCommand.Parameters.AddWithValue("@toperiod", tryear);
                dbSqlconnection.Open();
                dbSqlCommand.ExecuteNonQuery();
                dbSqlconnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
        #endregion



        #region =========  Publish Salary ============

        public bool PublishSalary(DTOModel.PayrollApprovalRequest request)
        {
            log.Info("SalaryRepository/PublishSalary");

            bool flag = false;
            try
            {
                var year = Convert.ToInt32(request.Period.Substring(0, 4));
                var month = Convert.ToInt32(request.Period.Substring(4, 2));

                if (request.BranchID.HasValue)  //====   if a single branch is selected...
                {
                    var dtoFmSalary = db.tblFinalMonthlySalaries.Where(x => x.SalYear == year && x.SalMonth == month
                     && x.BranchCode == request.BranchCode && x.tblMstEmployee.EmployeeTypeID == request.EmployeeTypeID && x.RecordType=="S"
                    ).ToList();

                    dtoFmSalary.ForEach(x => { x.Publish = true; });
                    db.SaveChanges();

                    flag = true;
                }
                else if (!request.BranchID.HasValue)  //====   when all branches excepts HO is selected.
                {
                    if (request.BranchCode == "Except-HO")
                    {
                        var dtoFmSalary = db.tblFinalMonthlySalaries.Where(x => x.SalYear == year && x.SalMonth == month
                        && x.BranchID != 44 && x.tblMstEmployee.EmployeeTypeID == request.EmployeeTypeID && x.RecordType == "S"
                       ).ToList();

                        dtoFmSalary.ForEach(x => { x.Publish = true; });
                        db.SaveChanges();

                        flag = true;
                    }
                    else if (request.BranchCode == "All")
                    {
                        var branchIDs = db.Branches.Where(x => !x.IsDeleted
                        && x.tblMstEmployees.Any(y => !y.IsDeleted
                        && y.EmployeeTypeID == request.EmployeeTypeID)).Select(z => z.BranchID);

                        var dtoFmSalary = db.tblFinalMonthlySalaries.Where(x => x.SalYear == year && x.SalMonth == month
                        && x.tblMstEmployee.EmployeeTypeID == request.EmployeeTypeID
                        && branchIDs.Any(b => b == x.BranchID) && x.RecordType == "S"
                      ).ToList();

                        dtoFmSalary.ForEach(x => { x.Publish = true; });
                        db.SaveChanges();

                        flag = true;

                    }
                }
                else if (1 > 0)  ///===== if a single employee is selected..
                {
                    var dtoFmSalary = db.tblFinalMonthlySalaries.Where(x => x.SalYear == year && x.SalMonth == month
                   && x.EmployeeID == 1 && x.RecordType == "S").ToList();

                    dtoFmSalary.ForEach(x => { x.Publish = true; });
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        public bool UndoPublishSalary(DTOModel.PayrollApprovalRequest request)
        {
            log.Info($"SalaryRepository/UndoPublishSalary/Status:{request.Status}");

            bool flag = false;
            try
            {
                var year = Convert.ToInt32(request.Period.Substring(0, 4));
                var month = Convert.ToInt32(request.Period.Substring(4, 2));

                if (request.BranchID.HasValue)  //====   if a single branch is selected...
                {
                    var dtoFmSalary = db.tblFinalMonthlySalaries.Where(x => x.SalYear == year && x.SalMonth == month
                     && x.BranchCode == request.BranchCode && x.tblMstEmployee.EmployeeTypeID == request.EmployeeTypeID
                    && x.RecordType == "S").ToList();

                    dtoFmSalary.ForEach(x => { x.Publish = false; });
                    db.SaveChanges();

                    flag = true;
                }
                else if (!request.BranchID.HasValue)  //====   when all branches excepts HO is selected.
                {
                    if (request.BranchCode == "Except-HO")
                    {
                        var dtoFmSalary = db.tblFinalMonthlySalaries.Where(x => x.SalYear == year && x.SalMonth == month
                        && x.BranchID != 44 && x.tblMstEmployee.EmployeeTypeID == request.EmployeeTypeID
                       && x.RecordType == "S").ToList();

                        dtoFmSalary.ForEach(x => { x.Publish = false; });
                        db.SaveChanges();

                        flag = true;
                    }
                    else if (request.BranchCode == "All")
                    {
                        var branchIDs = db.Branches.Where(x => !x.IsDeleted
                        && x.tblMstEmployees.Any(y => !y.IsDeleted
                        && y.EmployeeTypeID == request.EmployeeTypeID)).Select(z => z.BranchID);

                        var dtoFmSalary = db.tblFinalMonthlySalaries.Where(x => x.SalYear == year && x.SalMonth == month
                        && x.tblMstEmployee.EmployeeTypeID == request.EmployeeTypeID
                        && branchIDs.Any(b => b == x.BranchID) && x.RecordType == "S"
                      ).ToList();

                        dtoFmSalary.ForEach(x => { x.Publish = false; });
                        db.SaveChanges();

                        flag = true;

                    }
                }
                else if (1 > 0)  ///===== if a single employee is selected..
                {
                    var dtoFmSalary = db.tblFinalMonthlySalaries.Where(x => x.SalYear == year && x.SalMonth == month
                   && x.EmployeeID == 1 && x.RecordType == "S").ToList();

                    dtoFmSalary.ForEach(x => { x.Publish = false; });
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        #endregion


        #region  ======== Publish DA Arrer ============

        public bool PublishDAArrer(PayrollApprovalRequest request)
        {
            log.Info($"SalaryRepository/PublishDAArrer/");
            bool flag = false;
            try
            {
                var year = Convert.ToInt32(request.Period.Substring(0, 4));
                var month = Convert.ToInt32(request.Period.Substring(4, 2));

                flag = UpdatePublishDAArrer(request.EmployeeID, request.BranchID, request.EmployeeTypeID, month, year, "Publish");
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        public bool UndoPublishDAArrear(PayrollApprovalRequest request)
        {
            log.Info($"SalaryRepository/UndoPublishDAArrear/");
            bool flag = false;
            try
            {
                var year = Convert.ToInt32(request.Period.Substring(0, 4));
                var month = Convert.ToInt32(request.Period.Substring(4, 2));

                flag = UpdatePublishDAArrer(request.EmployeeID, request.BranchID, request.EmployeeTypeID, month, year, "Undo");
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }
        #endregion


        #region ===== Publish Pay Arrear ==========

        public bool PublishPayArrear(PayrollApprovalRequest request)
        {
            log.Info($"SalaryRepository/PublishPayArrear/");
            bool flag = false;
            try
            {
                var year = Convert.ToInt32(request.Period.Substring(0, 4));
                var month = Convert.ToInt32(request.Period.Substring(4, 2));

                flag = UpdatePublishPayArrer(request.EmployeeID, request.BranchID, request.EmployeeTypeID, month, year, "Publish");
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }


        public bool UndoPublishPayArrear(PayrollApprovalRequest request)
        {
            log.Info($"SalaryRepository/UndoPublishPayArrear/");

            bool flag = false;
            try
            {
                var year = Convert.ToInt32(request.Period.Substring(0, 4));
                var month = Convert.ToInt32(request.Period.Substring(4, 2));

                flag = UpdatePublishPayArrer(request.EmployeeID, request.BranchID, request.EmployeeTypeID, month, year, "Reject");
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }
        #endregion

        public DataTable GetEmployeeByLoanTypedt(int loanTypeId, bool oldLoanEmployee)
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
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "GetEmployeeByLoanType";
                dbSqlCommand.Parameters.Add("@loanTypeId", SqlDbType.Int).Value = loanTypeId;
                dbSqlCommand.Parameters.Add("@oldLoanEmployee", SqlDbType.Bit).Value = oldLoanEmployee;
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

        public bool UpdateTotalRefundData(string priorityNo, int serialNo, string period, decimal? balancePAmt, decimal? balanceIAmt, DateTime? refundDate, byte totlRefundMonthId, Int16 totlRefundYearId)
        {
            try
            {
                using (SqlConnection dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    try
                    {
                        SqlCommand dbSqlCommand;
                        dbSqlCommand = new SqlCommand();
                        dbSqlCommand.Connection = dbSqlconnection;
                        dbSqlCommand.CommandType = CommandType.StoredProcedure;
                        dbSqlCommand.CommandText = "UpdateTotalRefundData";
                        dbSqlCommand.Parameters.Add("@PriorityNo", SqlDbType.VarChar).Value = priorityNo;
                        dbSqlCommand.Parameters.Add("@SerialNo", SqlDbType.Int).Value = serialNo;
                        dbSqlCommand.Parameters.Add("@period", SqlDbType.VarChar).Value = period;
                        dbSqlCommand.Parameters.Add("@balancePAmt", SqlDbType.Decimal).Value = balancePAmt;
                        dbSqlCommand.Parameters.Add("@balanceIAmt", SqlDbType.Decimal).Value = balanceIAmt;
                        dbSqlCommand.Parameters.Add("@refundDate", SqlDbType.DateTime).Value = refundDate;
                        dbSqlCommand.Parameters.Add("@totlRefundMonthId", SqlDbType.Int).Value = totlRefundMonthId;
                        dbSqlCommand.Parameters.Add("@totlRefundYearId", SqlDbType.Int).Value = totlRefundYearId;
                        if (dbSqlconnection.State == ConnectionState.Closed)
                            dbSqlconnection.Open();
                        dbSqlCommand.ExecuteNonQuery();
                        dbSqlconnection.Close();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                        throw;
                    }
                    finally
                    {
                        if (dbSqlconnection.State == ConnectionState.Open)
                        {
                            dbSqlconnection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetECRDataForExport(int salMonth, int salYear,int? intRate)
        {          
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
                dbSqlCommand.CommandText = "GetECRReport";
                dbSqlCommand.Parameters.Add("@salMonth", SqlDbType.Int).Value = salMonth;
                dbSqlCommand.Parameters.Add("@salYear", SqlDbType.Int).Value = salYear;
                dbSqlCommand.Parameters.Add("@intRate", SqlDbType.Decimal).Value = intRate;
                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataSet);
                dbSqlconnection.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return dbSqlDataSet;
        }

        public bool GeneralReportsSummation(string branchcode, string salmonthF, string salyearF, string salmonthT, string salyearT, List<string> lst, int empType)
        {
            try
            {
                using (SqlConnection dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    try
                    {
                        SqlCommand dbSqlCommand;
                        dbSqlCommand = new SqlCommand();
                        dbSqlCommand.Connection = dbSqlconnection;
                        dbSqlCommand.CommandType = CommandType.StoredProcedure;
                        dbSqlCommand.CommandText = "GeneralReportsSummation";
                        dbSqlCommand.Parameters.Add("@BRANCHCODE", SqlDbType.VarChar).Value = branchcode;
                        dbSqlCommand.Parameters.Add("@SALMONTHF", SqlDbType.VarChar).Value = salmonthF;
                        dbSqlCommand.Parameters.Add("@SALYEARF", SqlDbType.VarChar).Value = salyearF;
                        dbSqlCommand.Parameters.Add("@SALMONTHT", SqlDbType.VarChar).Value = salmonthT;
                        dbSqlCommand.Parameters.Add("@SALYEART", SqlDbType.VarChar).Value = salyearT;
                        dbSqlCommand.Parameters.Add("@HEADNAME1", SqlDbType.VarChar).Value =Convert.ToString(lst[0]);
                        dbSqlCommand.Parameters.Add("@HEADNAME2", SqlDbType.VarChar).Value = Convert.ToString(lst[1]);
                        dbSqlCommand.Parameters.Add("@HEADNAME3", SqlDbType.VarChar).Value = Convert.ToString(lst[2]);
                        dbSqlCommand.Parameters.Add("@HEADNAME4", SqlDbType.VarChar).Value = Convert.ToString(lst[3]);
                        dbSqlCommand.Parameters.Add("@HEADNAME5", SqlDbType.VarChar).Value = Convert.ToString(lst[4]);
                        dbSqlCommand.Parameters.Add("@HEADNAME6", SqlDbType.VarChar).Value = Convert.ToString(lst[5]);
                        dbSqlCommand.Parameters.Add("@EMPTYPE", SqlDbType.VarChar).Value = empType;

                        if (dbSqlconnection.State == ConnectionState.Closed)
                            dbSqlconnection.Open();
                        dbSqlCommand.ExecuteNonQuery();
                        dbSqlconnection.Close();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                        throw;
                    }
                    finally
                    {
                        if (dbSqlconnection.State == ConnectionState.Open)
                        {
                            dbSqlconnection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
