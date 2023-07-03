using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using System.Collections.Generic;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IDropdownBindService
    {
        List<SelectListModel> ddlSkillType();
        List<SelectListModel> ddlSkill(int? skillTypeID);
        List<Model.SelectListModel> ddlMenuList();

        List<SelectListModel> ddlDepartmentList();

        List<SelectListModel> ddlUserTypeList();

        List<SelectListModel> ddlCategoryList();

        List<SelectListModel> ddlCadreList();

        List<SelectListModel> ddlCityList();

        List<SelectListModel> ddlGradeList();
        List<SelectListModel> ddlTitleList();

        List<SelectListModel> ddlEmployeeTypeList();

        List<SelectListModel> ddlGenderList();

        List<SelectListModel> ddlDesignationList();

        List<SelectListModel> ddlCadreCodeList();

        List<SelectListModel> ddlBranchList(int? branchID = null, int? userTypeID = null);

        List<SelectListModel> ddlSectionList();

        List<SelectListModel> ddlReligionList();

        List<SelectListModel> ddlMotherTongueList();

        List<SelectListModel> ddlMaritalStsList();

        List<SelectListModel> ddlBloodGroupList();

        List<SelectListModel> ddlRelationList();

        List<SelectListModel> ddlEmployeeCategoryList();

        List<SelectListModel> ddlAcedamicAndProfDtls(int typeID);

        List<SelectListModel> ddlCalendarYearList();

        List<SelectListModel> ddlLeaveCategoryList(int? genderID = null, string employeeLevel = null, int? emptypeID = null);

        List<SelectListModel> employeeByBranchID(int branchID, int? employeeID = null, int? userTypeID = null);
        List<SelectListModel> ddlAttendanceTypeList();
        List<SelectListModel> employeeReportingByID(int? EmpID);
        List<SelectListModel> employeeLeavePurposeByID(int LeaveCatID);
        List<SelectListModel> ddlstatus();
        List<dynamic> ddlBanks();
        List<SelectListModel> GetAllEmployee(int? employeeID = null);
        List<SelectListModel> ddlLeaveType();
        List<SelectListModel> ddlAppraisalForm();
        List<SelectListModel> GetAllEmployeeByProcessID(int employeeID, WorkFlowProcess wrkProcess);
        List<SelectListModel> ddlFirstDesignationList();
        List<SelectListModel> ddlFirstBranchList();
        List<SelectListModel> ddlStateList();

        List<SelectListModel> GetLoanType();
        SelectListModel GetEmployeeByPFNumber(int PFNumber);
        SelectListModel GetBranchByEmployeeId(int employeeId);

        List<SalaryHeadField> GetSalaryHead(int employeeTypeId);
        List<SelectListModel> GetEmployeeDetailsByEmployeeType(int? branchId, int? employeeTypeId);
        List<SelectListModel> GetEmployeeByDepartmentDesignationID(int? branchID, int? departmentID, int? designationID);
        List<SelectListModel> GetFileTrackingType();
        string GetBranchCode(int branchId);
        List<SelectListModel> GetStaffBudget();
        List<SelectListModel> GetAssetType();
        List<SelectListModel> GetManufacturer();
        List<SelectListModel> GetAssetName(int statusID, int assetTypeID);
        string GetStateName(int stateID);
        List<SelectListModel> GetTicketType();
        List<SelectListModel> GetTicketPriority();
        List<SelectListModel> GetFinanceYear();
        List<SelectListModel> GetEmployeeByManager(int employeeID);

        List<SelectListModel> GetNegativeSalEmployee(int salYear, int salMonth);
        List<SelectListModel> ddlDesignationListForTicket();
        List<SelectListModel> ddlDepartmentHavingEmployee();
        List<SelectListModel> GetEmployee();
        List<SelectListModel> GetCurrExEmployeeDetailsByEmployeeType(int? branchId, int? employeeTypeId);
        List<SelectListModel> ddlDepartmentHavingTicket();
        List<SalaryHeadField> GetSalaryHeadForIndividualHead(int employeeTypeId);
    }
}
