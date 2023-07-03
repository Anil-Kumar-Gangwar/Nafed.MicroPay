using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;

namespace Nafed.MicroPay.Data.Repositories
{
    public class AppraisalRepository : BaseRepository, IAppraisalRepository
    {
        //public List<tblMstEmployee> EmployeeAppraisalList(int userEmpID, string empCode, string empName)
        //{
        //    return db.tblMstEmployees.AsNoTracking().Where(x => !x.IsDeleted && (x.ReportingTo == userEmpID || x.ReviewerTo == userEmpID || x.AcceptanceAuthority == userEmpID) &&
        //     (!string.IsNullOrEmpty(empCode) ? x.EmployeeCode == empCode : (1 > 0))
        //     && (!string.IsNullOrEmpty(empName) ? x.Name.Contains(empName) : (1 > 0))).ToList();

        //    //return db.EmployeeProcessApprovals.AsNoTracking().Where(x => (x.ReportingTo == userEmpID || x.ReviewingTo == userEmpID || x.AcceptanceAuthority == userEmpID) &&
        //    // (x.ToDate==null)).ToList();
        //}

        public List<AppraisalFormHdr> EmployeeSelfAppraisalList(int userEmpID, string empCode, string empName)
        {
            //return db.tblMstEmployees.AsNoTracking().Where(x => !x.IsDeleted && x.EmployeeId == userEmpID &&
            // (!string.IsNullOrEmpty(empCode) ? x.EmployeeCode == empCode : (1 > 0))
            // && (!string.IsNullOrEmpty(empName) ? x.Name.Contains(empName) : (1 > 0))).ToList();

            return db.AppraisalFormHdrs.AsNoTracking().Where(x => x.EmployeeID == userEmpID
             && x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.EmployeeID == userEmpID && y.ProcessID == 5 && y.ToDate == null)
           ).ToList();
        }
        public List<GetFormGroupHdrDetail_Result> GetFormGroupHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID)
        {
            return db.GetFormGroupHdrDetail(tableName, branchID, empID, reportingYear, formID).ToList();
        }
        public List<GetFormGroupDetail1_Result> GetFormGroupDetail1(string hdrTableName, string childTableName, int empID, int? formGroupID)
        {
            return db.GetFormGroupDetail1(hdrTableName, childTableName, empID, formGroupID).ToList();
        }
        public List<GetFormGroupDetail2_Result> GetFormGroupDetail2(string hdrTableName, string childTableName, int empID, int? formGroupID)
        {
            return db.GetFormGroupDetail2(hdrTableName, childTableName, empID, formGroupID).ToList();
        }

        public List<GetFormTrainingDtls_Result> GetFormTrainingDtls(string hdrTableName, string childTableName, int empID, int? formGroupID, int? formID,string reportingYear)
        {
            return db.GetFormTrainingDtls(hdrTableName, childTableName, empID, formGroupID, formID, reportingYear).ToList();
        }
        public List<GetFormGroupHHdrDetail_Result> GetFormGroupHHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID)
        {
            return db.GetFormGroupHHdrDetail(tableName, branchID, empID, reportingYear, formID).ToList();
        }

        public List<GetFormGroupGHdrDetail_Result> GetFormGroupGHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID)
        {
            return db.GetFormGroupGHdrDetail(tableName, branchID, empID, reportingYear, formID).ToList();
        }
        public List<GetFormGroupDHdrDetail_Result> GetFormGroupDHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID)
        {
            return db.GetFormGroupDHdrDetail(tableName, branchID, empID, reportingYear, formID).ToList();
        }
        public List<GetFormGroupFHdrDetail_Result> GetFormGroupFHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID)
        {
            return db.GetFormGroupFHdrDetail(tableName, branchID, empID, reportingYear, formID).ToList();
        }

        public List<AppraisalFormHdr> GetAppraisalFormHdr()
        {
            return db.AppraisalFormHdrs.ToList();
        }

        #region APAR Skill Set
        public List<APARSkillSetList_Result> GetAPARSkillList(int? departmentID,int ? designationID)
        {
            return db.APARSkillSetList(departmentID,designationID).ToList();
        }
        public List<GetAPARSkillDetail_Result> GetAPARSkillDetail(int skillSetID, int? skillTypeID)
        {
            return db.GetAPARSkillDetail(skillSetID, skillTypeID).ToList();
        }

        public List<APARSkillSetFormHdr> EmployeeSelfAPARSkillList(int userEmpID)
        {
            return db.APARSkillSetFormHdrs.AsNoTracking().Where(x => x.EmployeeID == userEmpID
             && x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.EmployeeID == userEmpID 
             && y.ProcessID == 5 && y.ToDate == null)
           ).ToList();
        }

        public List<GetAPARHdrDetail_Result> GetFormAPARHdrDetail(int? branchID, int? empID, string reportingYear)
        {
            return db.GetAPARHdrDetail(branchID, empID, reportingYear).ToList();
        }

        public List<GetAPARFormDetail_Result> GetAPARFormDetail(int empID,int departmentID,int designationID, string @reportingYr)
        {
            return db.GetAPARFormDetail(empID, departmentID, designationID, @reportingYr).ToList();
        }

        public List<GetSkillSetDetails_Result> GetSkillSetDetails(int departmentID)
        {
            return db.GetSkillSetDetails(departmentID, null, null,null).ToList();
        }

        public bool UpdateSkillRemarks(DataTable seletedSkills)
        {
            log.Info($"AppraisalRepository/UpdateSkillRemarks");

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
                dbSqlCommand.CommandText = "[UpdateSkillRemarks]";             

                SqlParameter selectedValues = dbSqlCommand.Parameters.AddWithValue("@seletedSkills", seletedSkills);
                selectedValues.SqlDbType = SqlDbType.Structured;
                selectedValues.TypeName = "[udtSelectedSkill]";
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
        #endregion

        public DataTable GetAcarAparFilters(string selectedReportingYear, int selectedEmployeeID, int? statusId, string procName)
        {
            log.Info("AppraisalRepository/GetAcarAparFilters");
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
                dbSqlCommand.CommandText = procName;
                dbSqlCommand.Parameters.Add("@ReportingYear", SqlDbType.VarChar).Value = selectedReportingYear;
                dbSqlCommand.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = selectedEmployeeID;
                dbSqlCommand.Parameters.Add("@Status", SqlDbType.Int).Value = statusId;
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

        #region LTC 
        public GetLTCDetail_Result GetLTDDetail(int ltcID,int employeeID)
        {
            return db.GetLTCDetail(ltcID, employeeID).FirstOrDefault();
        }
        public List<Nafed.MicroPay.Data.Models.Holiday> GetHolidayList(DateTime? StartDate, DateTime? EndDate)
        {
            return db.Holidays.Where(x => (DbFunctions.TruncateTime(x.HolidayDate) >= DbFunctions.TruncateTime(StartDate) && DbFunctions.TruncateTime(x.HolidayDate) <= DbFunctions.TruncateTime(EndDate)) && (x.IsDeleted == false || x.IsDeleted == null)).ToList();
        }
        #endregion

        #region Appraisalhistory 

        public DataTable GetAppraisalHistory(int empID, int ProcessID, int ReferenceID)
        {
            log.Info("AppraisalRepository/GetAppraisalHistory");
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
                dbSqlCommand.CommandText = "GetAppraisalHistory";
                dbSqlCommand.Parameters.Add("@empID", SqlDbType.Int).Value = empID;
                dbSqlCommand.Parameters.Add("@ProcessID", SqlDbType.Int).Value = ProcessID;
                dbSqlCommand.Parameters.Add("@ReferenceID", SqlDbType.Int).Value = ReferenceID;
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

        #endregion

        public bool UpdateAPARDates(string financialyear,DateTime empdate,DateTime repdate,DateTime revdate,DateTime accpdate)
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
                dbSqlCommand.CommandText = "UpdateARARDates";

                dbSqlCommand.Parameters.Add("@reportingYear", SqlDbType.VarChar).Value = financialyear;
                dbSqlCommand.Parameters.Add("@empDate", SqlDbType.Date).Value = empdate;
                dbSqlCommand.Parameters.Add("@repDate", SqlDbType.Date).Value = repdate;
                dbSqlCommand.Parameters.Add("@revDate", SqlDbType.Date).Value = revdate;
                dbSqlCommand.Parameters.Add("@accpDate", SqlDbType.Date).Value = accpdate;
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                int res = dbSqlCommand.ExecuteNonQuery();
                dbSqlconnection.Close();
                return true;             
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

        public List<SP_GetProcessApprovalDetail_Result> GetProcessApprovalDetail(int processID,int? branchID)
        {
            return db.SP_GetProcessApprovalDetail(processID, branchID).ToList();
        }

        public List<GetAPARTrainingbySubordinate_Result> GetAPARTrainingbySubordinate(int ? employeeID, string ReportingYr)
        {
            return db.GetAPARTrainingbySubordinate(ReportingYr, employeeID).ToList();
        }

        public List<GetSubordinateTraining_Result> GetSubordinateTraining(int? employeeID, string ReportingYr)
        {
            return db.GetSubordinateTraining(ReportingYr, employeeID).ToList();
        }
    }
}
