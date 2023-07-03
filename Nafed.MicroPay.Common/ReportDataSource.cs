using System.Collections.Generic;

namespace Nafed.MicroPay.Common
{
    class ReportDataSource
    {
        Dictionary<string, string> reportDataSource = new Dictionary<string, string>()
        {
               //{"ReportName","ProcedureName"}
               {"AMDtoAMNFD.rpt","sp_AMDtoAMNFD"},
               {"BranchManagerReport.rpt","sp_BranchManagerDetail" },
               {"BranchRegManagerReport.rpt","sp_BranchRegManager1" },
               {"ConfirmationDueReport.rpt","sp_confirmDue" },
               {"ConfirmationPeriodwiseReport.rpt","sp_confirmDuePeriodWise" },
               {"ConfirmedEmployeeReport.rpt","sp_confirmDueIII" },
               {"DepartmentWiseEmployee.rpt","SP_SECTIONWISEEMPLOYEE" },
               {"DesgnationMasterReport.rpt","sp_desgmaster" },
               {"EmployeePosted.rpt","sp_empposted" },
               {"EmployeeQualification.rpt","sp_EmployeeQualification3" },
               {"HolidayReport.rpt","sp_holidaymaster" },
               {"Increment.rpt","sp_Increment" },
               {"OfficerCountReport.rpt","sp_countofficer" },
               {"PeriodWiseStaffVacancy.rpt","sp_periodwisestaffbudgetvacancy" },
               {"Projected_Increment.rpt","sp_Projected_increment" },
               {"ResOffHO.rpt","sp_ResOffHO" },
               {"StaffBudget.rpt","sp_ProcStaffBudget" },
               {"Seniority.rpt","sp_sen_list" },
               {"staffcount.rpt","sp_countstaff" },
               {"StaffVacancy.rpt","sp_staffbudgetvacancy" },
               {"TransferHistoryReport.rpt","sp_EmpTransferHistory" },
               {"RegStfStr.rpt","sp_FemStaff/sp_StaffBud/sp_StaffBudBra/sp_StaffBudHO"},
              {"nonregularemployee.rpt","sp_nonregularemployee"},
              {"staff_bifurcation.rpt","sp_branch_bifurcation"},
              {"singledepartmentwise.rpt","SP_SECTIONWISEEMPLOYEE"},
              {"NSOHOPre.rpt","sp_emp_reg_HO"},
              {"PWESec.rpt","sp_PWESec"},
              {"OffiDelOHO.rpt","sp_off_del_OHO"},
              {"OffiB40.rpt","sp_OffiB40"},
              {"AddBrRegOffi.rpt","sp_AddBrRegOff"}

        };
    }
}
