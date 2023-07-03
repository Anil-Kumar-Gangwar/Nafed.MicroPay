using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using System.Data;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IAppraisalRepository
    {
        List<AppraisalFormHdr> EmployeeSelfAppraisalList(int userEmpID, string empCode, string empName);
        List<GetFormGroupHdrDetail_Result> GetFormGroupHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID);
        List<GetFormGroupDetail1_Result> GetFormGroupDetail1(string hdrTableName, string childTableName, int empID, int? formGroupID);
        List<GetFormGroupDetail2_Result> GetFormGroupDetail2(string hdrTableName, string childTableName, int empID, int? formGroupID);
        List<GetFormTrainingDtls_Result> GetFormTrainingDtls(string hdrTableName, string childTableName, int empID, int? formGroupID, int? formID,string reportingYear);
        List<GetFormGroupHHdrDetail_Result> GetFormGroupHHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID);
        List<GetFormGroupGHdrDetail_Result> GetFormGroupGHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID);
        List<GetFormGroupDHdrDetail_Result> GetFormGroupDHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID);
        List<AppraisalFormHdr> GetAppraisalFormHdr();
        List<GetFormGroupFHdrDetail_Result> GetFormGroupFHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID);

        #region APAR Skill Set
        List<APARSkillSetList_Result> GetAPARSkillList(int? departmentID, int? designationID);
        List<GetAPARSkillDetail_Result> GetAPARSkillDetail(int skillSetID, int? skillTypeID);
        List<APARSkillSetFormHdr> EmployeeSelfAPARSkillList(int userEmpID);
        List<GetAPARHdrDetail_Result> GetFormAPARHdrDetail(int? branchID, int? empID, string reportingYear);
        List<GetAPARFormDetail_Result> GetAPARFormDetail(int empID, int departmentID, int designationID, string @reportingYr);
        List<GetSkillSetDetails_Result> GetSkillSetDetails(int departmentID);
        bool UpdateSkillRemarks(DataTable seletedSkills);
        #endregion

        DataTable GetAcarAparFilters(string selectedReportingYear, int selectedEmployeeID, int? statusId, string procName);

        #region LTC
        GetLTCDetail_Result GetLTDDetail(int ltcID, int employeeID);
        List<Nafed.MicroPay.Data.Models.Holiday> GetHolidayList(DateTime? StartDate, DateTime? EndDate);
        #endregion

        #region Appraisalhistory 
        DataTable GetAppraisalHistory(int empID, int ProcessID, int ReferenceID);
        #endregion
        bool UpdateAPARDates(string financialyear, DateTime empdate, DateTime repdate, DateTime revdate, DateTime accpdate);
        List<SP_GetProcessApprovalDetail_Result> GetProcessApprovalDetail(int processID, int? branchID);

        List<GetAPARTrainingbySubordinate_Result> GetAPARTrainingbySubordinate(int? employeeID, string ReportingYr);
        List<GetSubordinateTraining_Result> GetSubordinateTraining(int? employeeID, string ReportingYr);
    }
}
