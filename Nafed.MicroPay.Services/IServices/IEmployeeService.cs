using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IEmployeeService
    {

        string GetEmployeeProfilePath(string empCode);
        bool EmployeeDetailsExists(int iD, string value);
        int InsertEmployeeDetails(Model.Employee employeeDetails);

        bool UpdatetEmployeePersonalDetails(Model.Employee employeeDetails);

        List<Model.Employee> GetEmployeeList(string empName, string empCode, int? designationID, int? empTypeID, string employeeType = null);

        Model.Employee GetEmployeeByID(int employeeID);

        bool UpdateEmployeeGeneralDetails(Model.Employee employee);

        bool UpdateEmployeeOtherDetails(Model.Employee empOtherDetails);

        bool UpdateEmployeePromotionalAndIncrementDtls(Model.Employee employee);

        bool UpdateEmployeeProfessionalDtls(EmployeeQualification empProfessionalDtls, int employeeID);

        bool UpdateEmployeePayScales(Model.EmployeePayScale empPayScale);

        bool DeleteEmployee(int employeeID);
        bool DeleteEmployeeDeputationInfo(int employeeID, int empDeputationID);
        Model.EmployeeProfile GetEmployeeProfile(int employeeID);

        bool CheckEmployeeCodeExistance(string empCode);

        string GetSupervisiorName(int employeeID);
        List<Model.Employee> GetEmployeeDetailsByCode(string empCode);
        IEnumerable<Model.EmployeeDeputation> GetEmployeeDeputationDtls(int employeeID, int? empDeputationID);
        bool AddAndUpdateEmpDeputationInfo(Model.EmployeeDeputation empDeputation);

        List<Model.Employee> GetEmployeesByBranchID(int? branchID);
        Model.EmployeeSalary GetEmployeeSalaryDtls(string empCode);
        bool InsertUpdateEmployeeSalary(Model.EmployeeSalary empSalary);
        List<Model.EmployeeProcessApproval> GetEmpApprovalProcesses(int employeeID);

        bool InsertEmpProcessApproval(List<Model.EmployeeProcessApproval> empProcessApproval);
        bool UpdateEmpProcessApproval(List<Model.EmployeeProcessApproval> empProcessApproval);

        bool UpdatetEmployeeProfile(Model.EmployeeProfile employeeDetails);
        bool ChangeProfilePicture(int employeeID, string pictureName, string newMobileNo, string newEmailID);
        bool ChangeMobileNo(int employeeID, string newMobileNo);

        Designation GetDesignationDtls(int designationID);
        DesignationAssignment GetDesignationAssignation(int? employeeID);
        Promotion GetPromotionForm(int? employeeID, int? transID);
        List<SubOrdinatesDetails> GetSubOrdinatesDetails(int? employeeID);

        int GetSeniorityCode(int designationID);

        #region Staff Transfer
        StaffTransfer GetStaffTransfer(int? employeeID);
        Transfer GetStaffTransferForm(int? employeeID, int? transId);

        Transfer ChangeStaffBranch(Transfer transfer);

        bool DeleteStaffTransferEntry(int transID);
        DataTable GetEmployeeTransferDetail(int? branchID, string employeeCode);
        bool ExportEmployeeTransferDetail(DataSet dsSource, string sFullPath, string fileName);
        #endregion

        EmployeeQualification GetQualificationDetail(int employeeID);
        Employee GetEmployeePaymentModeDtls(string empCode);

        string ExportBrachWiseSalaryConfiguration(int branchID, string fileName, string filePath);

        List<Model.EmployeeSuspensionPeriod> GetEmployeeSuspensionHistory(int employeeID);

        bool DoesNewPeriodOverLapped(int employeeID, DateTime periodFrom, DateTime periodTo);

        bool UpdateEmployeeAcheivement(Model.EmployeeAchievement empAchievement, List<Model.EmpAchievementAndCertificationDocument> documents);

        List<Model.EmployeeAchievement> GetEmployeeAchievement(int employeeID);

        List<Model.EmployeeCertification> GetEmployeeCertification(int employeeID);

        bool UpdateEmployeeCertification(Model.EmployeeCertification empCertification, List<Model.EmpAchievementAndCertificationDocument> documents);
        bool DeleteEmpAchievement(int empAchievementID, int userID);

        bool DeleteEmpCertificate(int empCertificateID, int userID);
        bool IsStaffBudgetAvailable(int deignationID, string year, string quataType);

        string GetAchievementAndCertificationReport(
           byte category,
           DateTime fromDate, DateTime toDate, int? employeeID, string fileName, string filePath);

    }
}
