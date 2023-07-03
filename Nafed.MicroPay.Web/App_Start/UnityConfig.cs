using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.ImportExport;
using Nafed.MicroPay.ImportExport.Interfaces;
using Nafed.MicroPay.Services.Salary;
using Nafed.MicroPay.Services.Increment;
using Nafed.MicroPay.Services.Budget;
using Nafed.MicroPay.Services.Recruitment;
using Nafed.MicroPay.Services.Arrear;
using Nafed.MicroPay.Services.TDS;
using Nafed.MicroPay.Services.HelpDesk;
using Nafed.MicroPay.Services.ArchiveData;

namespace MicroPay.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();
            container.RegisterType<IBaseRepository, BaseRepository>();
            container.RegisterType<ILoginRepository, LoginRepository>();
            container.RegisterType<ILoginService, LoginService>();
            container.RegisterType<IMenuService, MenuService>();
            container.RegisterType<IMenuRepository, MenuRepository>();
            container.RegisterType<IGenericRepository, GenericRepository>();
            container.RegisterType<IDepartmentRightsService, DepartmentRightsService>();
            container.RegisterType<IDepartmentRightRepository, DepartmentRightRepository>();
            container.RegisterType<IUserRightsService, UserRightsService>();
            container.RegisterType<IUserRightsRepository, UserRightsRepository>();
            container.RegisterType<ICadreService, CadreService>();
            container.RegisterType<IHolidayService, HolidayService>();
            container.RegisterType<IEmailConfigurationService, EmailConfigurationService>();
            container.RegisterType<ISMSConfigurationService, SMSConfigurationService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IDropdownBindService, DropdownBindService>();
            container.RegisterType<IGradeService, GradeService>();
            container.RegisterType<ICategoryService, CategoryService>();
            container.RegisterType<IDepartmentService, DepartmentService>();
            container.RegisterType<IEmployeeTypeService, EmployeeTypeService>();
            container.RegisterType<IReligionService, ReligionService>();
            container.RegisterType<ITitleService, TitleService>();
            container.RegisterType<IDesignationService, DesignationService>();
            container.RegisterType<IBranchService, BranchService>();
            container.RegisterType<IAcadmicProfessionalDetails, AcadmicProfessionalDetailsService>();
            container.RegisterType<IMotherTongueService, MotherTongueService>();
            container.RegisterType<IRelationService, RelationService>();
            container.RegisterType<IMaritalStatusService, MaritalStatusService>();
            container.RegisterType<IDivisionService, DivisionService>();
            container.RegisterType<ISectionService, SectionService>();
            container.RegisterType<IEmployeeService, EmployeeService>();
            container.RegisterType<ILeaveService, LeaveService>();
            container.RegisterType<IMarkAttendance, MarkAttendanceService>();
            container.RegisterType<IEmployeeLeaveService, EmployeeLeaveService>();
            container.RegisterType<IStaffLeaveDetailsService, StaffLeaveDetailsService>();
            container.RegisterType<IEmployeeAttendancedetailsService, EmployeeAttendancedetailsService>();
            container.RegisterType<IImportAttendanceService, ImportAttendanceService>();
            container.RegisterType<IExcelService, ExcelService>();
            container.RegisterType<IImport, Import>();
            container.RegisterType<IAttendanceImportRepository, AttendanceImportRepository>();
            container.RegisterType<IEmployeeRepository, EmployeeRepository>();
            container.RegisterType<IEmployeeAttendanceRepository, EmployeeAttendanceRepository>();
            container.RegisterType<IBankService, BankService>();
            container.RegisterType<IBankRatesService, BankRatesService>();
            container.RegisterType<ILoanSlab, LoanSlabService>();
            container.RegisterType<IFDAService, FDAService>();
            container.RegisterType<IWagesService, WagesService>();
            container.RegisterType<IWashingAllowanceService, WashingAllowanceService>();
            container.RegisterType<IOTAService, OTAService>();
            container.RegisterType<IHillCompensationService, HillCompensationService>();
            container.RegisterType<ISalaryHeadRuleService, SalaryHeadRuleService>();
            container.RegisterType<ICEARatesService, CEARatesService>();
            container.RegisterType<ICCAService, CCAService>();
            container.RegisterType<IGisDeductionService, GisDeductionService>();
            container.RegisterType<ILeaveRepository, LeaveRepository>();

            #region Dashboard  ====  

            container.RegisterType<IDashBoardRepository, DashBoardRepository>();
            container.RegisterType<IDashBoardService, DashBoardService>();
            #endregion

            #region Appraisal Form
            container.RegisterType<IAppraisalFormService, AppraisalFormService>();
            container.RegisterType<IAppraisalRepository, AppraisalRepository>();
            #endregion

            #region Advance Search 

            container.RegisterType<IAdvanceSearchService, AdvanceSearchService>();
            container.RegisterType<IAdvanceSearchRepository, AdvanceSearchRepository>();
            #endregion

            #region Confirmation Form
            container.RegisterType<IConfirmationFormService, ConfirmationFormService>();
            container.RegisterType<IConfirmationFormRepository, ConfirmationFormRepository>();
            #endregion

            container.RegisterType<ILeaveBalanceAsOfNowService, LeaveBalanceAsOfNowService>();
            container.RegisterType<IExport, Export>();
            container.RegisterType<IDependentService, DependentService>();
            container.RegisterType<IUpdateBasicService, UpdateBasicService>();
            container.RegisterType<IUpdateBasicRepository, UpdateBasicRepository>();
            container.RegisterType<IIncrementRepository, IncrementRepository>();
            container.RegisterType<IIncrement, IncrementService>();
            container.RegisterType<IBudgetService, BudgetService>();
            container.RegisterType<IBudgetRepository, BudgetRepository>();
            container.RegisterType<ISkillService, SkillService>();
            container.RegisterType<ILTCService, LTCService>();
            container.RegisterType<IPRService, PRService>();
            container.RegisterType<IRecruitmentService, RecruitmentService>();
            container.RegisterType<IRecruitmentRepository, RecruitmentRepository>();
            container.RegisterType<ITrainingService, TrainingService>();
            container.RegisterType<ITrainingRepository, TrainingRepository>();
            container.RegisterType<ISalaryRepository, SalaryRepository>();
            container.RegisterType<IImportMonthlyInputService, ImportMonthlyInputService>();
            container.RegisterType<IDataImportRepository, DataImportRepository>();
            container.RegisterType<IAdjustOldLoanService, AdjustOldLoanService>();
            container.RegisterType<IArrearRepository, ArrearRepository>();
            container.RegisterType<IArrearService, ArrearService>();
            container.RegisterType<IFileTrackingSytemService, FileTrackingSytemService>();
            container.RegisterType<IFileManagementSystemRepository, FileManagementSystemRepository>();
            container.RegisterType<IControlSettingService, ControlSettingService>();
            container.RegisterType<IRegularEmpSalaryService, GenerateSalaryService>();
            container.RegisterType<IGenerateSalaryRepository, GenerateSalaryRepository>();
            container.RegisterType<ISalaryRepository, SalaryRepository>();
            container.RegisterType<IImportManualDataService, ImportManualDataService>();
            container.RegisterType<IIncrementExport, IncrementExport>();
            container.RegisterType<IExgratiaService, ExgratiaService>();
            container.RegisterType<ICRReportService, CRReportService>();
            container.RegisterType<ICRReportRepository, CRRepository>();
            container.RegisterType<IPayrollApprovalSettingService, PayrollApprovalSettingService>();
            container.RegisterType<ILoanApplicationRepository, LoanApplicationRepository>();
            container.RegisterType<ILoanApplicationService, LoanApplicationService>();
            container.RegisterType<IBonusRepository, BonusRepositry>();
            container.RegisterType<IExgratiaRepository, ExgratiaRepository>();
            container.RegisterType<IBonusWagesService, BonusWagesService>();
            container.RegisterType<ILoanTypeService, LoanTypeService>();
            container.RegisterType<IEmployeePFOrganisationService, EmployeePFOrganisationService>();
            container.RegisterType<IEPFNominationRepository, EPFNominationRepository>();
            container.RegisterType<IEPFNominationService, EPFNominationService>();
            container.RegisterType<IChildrenEducationService, ChildrenEducationService>();
            container.RegisterType<IChildrenEducationRepository, ChildrenEducationRepository>();
            container.RegisterType<IConveyanceBillService, ConveyanceBillService>();
            container.RegisterType<IConveyanceRepository, ConveyanceRepository>();
            container.RegisterType<ISalaryReportRepository, SalaryReportRepository>();
            container.RegisterType<ISalaryReportService, SalaryReportService>();
            container.RegisterType<INRPFLoanService, NRPFLoanService>();
            container.RegisterType<IForm12BBService, Form12BBService>();
            container.RegisterType<IAssetTypeService, AssetTypeService>();
            container.RegisterType<IManufacturerService, ManufacturerService>();
            container.RegisterType<IInventoryManagementService, InventoryManagementService>();
            container.RegisterType<ITDSTaxRuleSlabService, TDSTaxRuleSlabService>();
            container.RegisterType<IEmpApprovalProcessService, EmpApprovalProcessService>();

            #region HelpDesk
            container.RegisterType<ITicketTypeService, TicketTypeService>();
            container.RegisterType<ITicketStatusService, TicketStatusService>();
            container.RegisterType<ITicketPriorityService, TicketPriorityService>();
            container.RegisterType<ISupportGroupService, SupportGroupService>();
            container.RegisterType<ISupportTeamService, SupportTeamService>();
            container.RegisterType<ITicketRepository, TicketRepository>();
            container.RegisterType<ITicketService, TicketService>();
            #endregion



            #region Archive Data ==== 

            container.RegisterType<IArchiveDataService, ArchiveDataService>();
            container.RegisterType<IArchiveDataRepository, ArchiveDataRepository>();
            #endregion



            container.RegisterType<IAppraisalFormDueDateService, AppraisalFormDueDateService>();

            #region  PF Balance ======
            container.RegisterType<IPFAccountBalanceServices, PFAccountBalanceServices>();
            container.RegisterType<IPFAccountBalanceRepository, PFAccountBalanceRepository>();
            container.RegisterType<IImportPfBalanceService, ImportPfBalanceService>();
            container.RegisterType<IImportDAtdsService, ImportDATdsService>();
            container.RegisterType<IDATDSServices, DAtdsServices>();
            container.RegisterType<IDATdsRepository, DATdsRepository>();
            #endregion


            #region ====== Bonus ======

            container.RegisterType<IImportBonusService, ImportBonusService>();

            #endregion

            #region === Exgratia ======

            container.RegisterType<IExgratiaRepository, ExgratiaRepository>();
            container.RegisterType<IExgratiaService, ExgratiaService>();
            container.RegisterType<IImportExGratiaService, ImportExGratiaService>();
            #endregion


            #region  === Import Process Approver ===========

            container.RegisterType<IImportProcessApproverService, ImportProcessApproverService>();
            container.RegisterType<IProcessApproverRepository, ProcessApproverRepository>();

            #endregion
            container.RegisterType<IInsuranceService, InsuranceService>();
            container.RegisterType<IFTSUserService, FTSUserService>();
            container.RegisterType<ISeparationRepository, SeparationRepository>();
            container.RegisterType<ISeparationService, SeparationService>();

            container.RegisterType<IForm12BBRepository, Form12BBRepository>();
            container.RegisterType<IForm12BBDocumentRepository, Form12BBDocumentRepository>();

            container.RegisterType<IActivityLogRepository, ActivityLogRepository>();
            container.RegisterType<IActivityLogService, ActivityLogService>();

            #region Transfer Appoval
            container.RegisterType<ITransferApprovalService, TransferApprovalService>();
            container.RegisterType<ITransferApprovalRepository, TransferApprovalRepository>();
            
            #endregion
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));


        }
    }
}