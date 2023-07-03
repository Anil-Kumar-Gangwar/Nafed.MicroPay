using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using System.Data;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IAppraisalFormService
    {
        IEnumerable<AppraisalForm> GetAppraisalForms();
        bool InsertUpdateDesignationApprasialForm(int formID, List<int> designation, int userID);
        IEnumerable<int> LinkedDesignations(int formID);

        //IEnumerable<Employee> GetEmployeeAppraisalList(int userEmpID, string empCode, string empName);

        List<AppraisalFormHdr> GetEmployeeSelfAppraisalList(int userEmpID, string empCode, string empName);

        FormGroupAHdr GetFormGroupHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID);
        IEnumerable<FormGroupDetail1> GetFormGroupDetail1(string hdrTableName, string childTableName, int empID, int? formGroupID);
        IEnumerable<FormGroupDetail2> GetFormGroupDetail2(string hdrTableName, string childTableName, int empID, int? formGroupID);

        bool InsertFormAData(Model.AppraisalForm appraisalForm);

        bool AppraisalDataExists(int empID, int formID, string reportingYear);

        bool UpdateFormAData(Model.AppraisalForm appraisalForm, string reportingType);
        IEnumerable<FormTrainingDtls> GetFormTrainingDetail(string hdrTableName, string childTableName, int empID, int? formGroupID, int? formID, string reportingYear);
        FormGroupHHdr GetFormGroupHHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID);
        bool InsertFormHData(AppraisalForm appraisalForm);
        bool UpdateFormHData(AppraisalForm appraisalForm);
        bool InsertFormGData(AppraisalForm appraisalForm);
        FormGroupGHdr GetFormGroupGHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID);
        bool UpdateFormGData(AppraisalForm appraisalForm);
        bool IsTrainingSubmitted(int empID, string reportingYear);

        #region Form B
        bool InsertFormBData(AppraisalForm appraisalForm);
        bool UpdateFormBData(AppraisalForm appraisalForm);
        #endregion

        #region Form C
        bool InsertFormCData(AppraisalForm appraisalForm);
        bool UpdateFormCData(AppraisalForm appraisalForm);
        #endregion

        #region Form D
        FormGroupDHdr GetFormGroupDHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID);
        bool InsertFormDData(AppraisalForm appraisalForm);
        bool UpdateFormDData(AppraisalForm appraisalForm);
        #endregion

        #region Form E
        bool InsertFormEData(AppraisalForm appraisalForm);
        bool UpdateFormEData(AppraisalForm appraisalForm);
        #endregion

        #region Form F

        FormGroupAHdr GetFormGroupFHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID);
        bool InsertFormFData(AppraisalForm appraisalForm);
        bool UpdateFormFData(AppraisalForm appraisalForm);
        #endregion

        IEnumerable<AppraisalFormHdr> GetAppraisalFormHdr(AppraisalFormApprovalFilter filters);

        bool UpdateAppraisalFormHdr(AppraisalForm appraisal);

        FormRulesAttributes GetFormRulesAttributes(Common.AppraisalForm formID, int employeeID, string reportingYr);

        #region A.P.A.R Skill Set
        List<APARSkillSet> GetAPARSkillList(int? departmentID, int? designationID);
        int InsertAPARSkill(APARSkills aparSkills);
        List<APARSkillSetDetails> GetAPARSkillDetail(int skillSetID, int? skillTypeID);
        int UpdateAPARSkill(APARSkills aparSkills);
        bool Delete(int skillSetID, int userID);
        List<APARSkillSetFormHdr> GetEmployeeAPARSkillList(int userEmpID);
        APARSkillSetFormHdr GetFormAPARHdrDetail(int? branchID, int? empID, string reportingYear);
        List<APARSkillSetFormDetail> GetAPARFormDetail(int empID, int departmentID, int designationID, string reportingYr);
        int InsertAPARFormData(APARForm aparForm);
        int UpdateAPARFormData(APARForm aparForm);

        IEnumerable<APARSkillSetFormHdr> GetSkillAssessmentFormHdr(SkillAssessmentApprovalFilters filters);
        List<APARSkillSetDetails> GetSkillDetails(int departmentID);
        bool UpdateSkillRemarks(List<APARSkillSetDetails> aparSkillList);

        int GetSkillSetDetailID(int skillSetID, int skillID);
        #endregion

        FormRulesAttributes GetFormSubmissionDate(int designationID);
        List<AcarAparModel> GetAcarAparDetails(AppraisalFormApprovalFilter filters, string procName);
        FormRulesAttributes GetFormSubmittionDueDate(string reportingYr);

        void SetSectionValue(AppraisalForm roformData);

        List<ProcessWorkFlow> GetAppraisalHistory(int referenceId, int empId, int processID);

        IEnumerable<AppraisalFormHdr> GetAppraisalFormHistory(AppraisalFormApprovalFilter filters);

        #region Upload Document
        bool UpdateUploadDocumentsDetails(AppraisalForm apprForm, APARReviewedSignedCopy existingFileDetails);
        APARReviewedSignedCopy GetAPARDocumentsList(int appraisalFormID, int empID, int formGroupId);
        #endregion
        bool ExportAPARStatusList(DataTable dtTable, string sFullPath, string fileName);
        List<FormGroupAHdr> GetProcessApprovalDetail(CommonFilter filters);
        bool ExportApprovalList(DataTable dtTable, string sFullPath, string fileName, string tFilter);
        List<FormTrainingDtls> GetAPAREmployeeTrainings(int? employeeID, string ReportingYr);

        List<FormTrainingDtls> GetSubordinateTrainings(int? employeeID, string ReportingYr);
        bool InsertAPARTrainingforEmployee(AppraisalForm aprObj, int userID);
        bool ExportEmpAPARTraining(DataSet dsSource, string sFullPath, string fileName);
    }
}
