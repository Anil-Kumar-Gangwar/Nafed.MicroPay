using Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;

namespace Nafed.MicroPay.Data.Repositories
{
    public class EmployeeRepository : BaseRepository, IEmployeeRepository
    {

        public IEnumerable<GetEmployeeDetails_Result> GetEmployeeList(string empName, string empCode, int? designationID, int? empTypeID)
        {
            return db.GetEmployeeDetails(empCode, empName, empTypeID, designationID).ToList();
        }
        public IEnumerable<GetDesignationPayScale_Result> GetDesignationPayScaleList(int? designationID)
        {
            return db.GetDesignationPayScale(designationID);
        }
        public int GeneratePasswordforAllEmployees(DataTable usersDT)
        {
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[GeneratePassword]";

                SqlParameter paramAttendanceDT = dbSqlCommand.Parameters.AddWithValue("@usersDT", usersDT);
                paramAttendanceDT.SqlDbType = SqlDbType.Structured;
                paramAttendanceDT.TypeName = "[udtSelectListModel]";

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

        public List<GetEmployeePromotions_Result> GetPromotionList(int? employeeID, int? transID)
        {
            return db.GetEmployeePromotions(employeeID, transID).ToList();
        }
        public List<GetUnMappedEmployeeList_Result> GetUnMappedEmployees(int? branchID)
        {
            return db.GetUnMappedEmployeeList(branchID).ToList();
        }

        public int UpdateEmployeeDeputation(int? empDeputationID, DateTime fromDate, DateTime toDate, string organizationName, int? updatedBy)
        {
            return db.UpdateEmployeeDeputation(empDeputationID, fromDate, toDate, organizationName, updatedBy);
        }
        public IEnumerable<GetSubOrdinatesDetails_Result> GetSubOrdinatesDetails(int? employeeID)
        {
            return db.GetSubOrdinatesDetails(employeeID);
        }

        public int UpdateSeniorityCode(int oldDesignationId, int? currentDesignationId, int? employeeId)
        {
            return db.PR_ManageSeniority(employeeId, oldDesignationId, currentDesignationId);
        }

        public DataTable GetEmployeeConfirmationDetails(int? formTypeId, DateTime? fromDate, DateTime? toDate)
        {
            log.Info("EmployeeRepository/GetEmployeeConfirmationDetails");
            //DataTable dt = new DataTable();
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
                dbSqlCommand.CommandText = "EmployeeConfirmationFormDetails";
                dbSqlCommand.Parameters.Add("@FormTypeId", SqlDbType.Int).Value = formTypeId;
                dbSqlCommand.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromDate;
                dbSqlCommand.Parameters.Add("@ToDate", SqlDbType.Date).Value = toDate;
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

        public DataTable GetBranchWiseSalaryConfig(int branchID)
        {
            log.Info("EmployeeRepository/GetBranchWiseSalaryConfig/branchID:{branchID}");
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
                dbSqlCommand.CommandText = "GetBranchWiseSalaryConfig";

                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = branchID;
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


        public DataTable GetEmployeeTransferDetail(int? branchID, string employeeCode)
        {
            log.Info($"EmployeeRepository/GetEmployeeTransferDetail/branchID:{branchID}");
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
                dbSqlCommand.CommandText = "sp_EmpTransferHistory";

                dbSqlCommand.Parameters.Add("@empcode", SqlDbType.VarChar).Value = employeeCode;
                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = branchID;
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

        public List<GetSeniorityList_Result> GetSeniorityList(int employeeID, int desID)
        {
            return db.GetSeniorityList(employeeID, desID).ToList();
        }

        public GetLastEmployeeCode_Result GetLastEmployeeCode(int empTypeID)
        {
            return db.GetLastEmployeeCode(empTypeID).FirstOrDefault();
        }

        public List<GetSuperAnnuating_Result> GetSuperAnnuating()
        {
            return db.GetSuperAnnuating().ToList();
        }

        public bool UpdatetEmployeePersonalDetails(tblMstEmployee employeeDetails)
        {
            log.Info($"EmployeeRepository/UpdatetEmployeePersonalDetails/{employeeDetails.EmployeeId}");

            try
            {
                var dtoObj = db.tblMstEmployees.Where(x => x.EmployeeId == employeeDetails.EmployeeId && x.IsDeleted == false).FirstOrDefault();
                if (dtoObj != null)
                {
                    dtoObj.HBName = employeeDetails.HBName;
                    dtoObj.DOB = employeeDetails.DOB;
                    dtoObj.ReligionID = employeeDetails.ReligionID == 0 ? null : employeeDetails.ReligionID;
                    dtoObj.MTongueID = employeeDetails.MTongueID == 0 ? null : employeeDetails.MTongueID;
                    dtoObj.MaritalStsID = employeeDetails.MaritalStsID == 0 ? null : employeeDetails.MaritalStsID;
                    dtoObj.BGroupID = employeeDetails.BGroupID == 0 ? null : employeeDetails.BGroupID;
                    dtoObj.ID_Mark = employeeDetails.ID_Mark;
                    dtoObj.PANNo = employeeDetails.PANNo;
                    dtoObj.AadhaarNo = employeeDetails.AadhaarNo;
                    dtoObj.PassPortNo = employeeDetails.PassPortNo;
                    dtoObj.PPIDate = employeeDetails.PPIDate;
                    dtoObj.PPEDate = employeeDetails.PPEDate;
                    dtoObj.PAdd = employeeDetails.PAdd;
                    dtoObj.PStreet = employeeDetails.PStreet;
                    dtoObj.PCity = employeeDetails.PCity;
                    dtoObj.PPin = employeeDetails.PPin;
                    dtoObj.TelPh = employeeDetails.TelPh;
                    dtoObj.PmtAdd = employeeDetails.PmtAdd;
                    dtoObj.PmtStreet = employeeDetails.PmtStreet;
                    dtoObj.PmtCity = employeeDetails.PmtCity;
                    dtoObj.PmtPin = employeeDetails.PmtPin;
                    dtoObj.AadhaarCardFilePath = employeeDetails.AadhaarCardFilePath;
                    dtoObj.PanCardFilePath = employeeDetails.PanCardFilePath;
                    dtoObj.MotherName = employeeDetails.MotherName;
                    dtoObj.IFSCCode = employeeDetails.IFSCCode;
                    dtoObj.UpdatedBy = employeeDetails.UpdatedBy;
                    dtoObj.UpdatedOn = employeeDetails.UpdatedOn;
                    dtoObj.PState = employeeDetails.PState == 0 ? null : employeeDetails.PState;
                    dtoObj.PmtState = employeeDetails.PmtState == 0 ? null : employeeDetails.PmtState;
                    dtoObj.PensionUAN = employeeDetails.PensionUAN;
                    dtoObj.PensionNumber = employeeDetails.PensionNumber;
                    dtoObj.EPFOMemberID = employeeDetails.EPFOMemberID;
                    db.SaveChanges();
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
