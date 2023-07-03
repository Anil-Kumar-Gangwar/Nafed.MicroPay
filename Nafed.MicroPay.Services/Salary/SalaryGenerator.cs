using Nafed.MicroPay.Model;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Nafed.MicroPay.ImportExport;
using Nafed.MicroPay.Common;
using System.IO;
using Nafed.MicroPay.Data.Repositories;
using System.Dynamic;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.Salary
{
    public class SalaryGenerator
    {
        private readonly IGenericRepository genericRepo;
        private readonly IGenerateSalaryRepository generateSalaryRepo;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SalaryGenerator(IGenericRepository genericRepo, IGenerateSalaryRepository generateSalaryRepo)
        {
            this.generateSalaryRepo = generateSalaryRepo;
            this.genericRepo = genericRepo;
        }
        public bool CalculateSalary(RegularEmployeeSalary regEmpSalary, out IList<string> negSalEmp)
        {
            log.Info($"SalaryGenerator/CalculateSalary");
            try
            {
                bool flag = false;
                List<FinalMonthlySalary> listFMonthlySalary = new List<FinalMonthlySalary>();
                IEnumerable<SalaryHead> salHeads = Enumerable.Empty<SalaryHead>();
                IEnumerable<string> nonActiveHeads = Enumerable.Empty<string>();

                var dtoEmpSalaries = generateSalaryRepo.GetMonthlySalaryList(
                    regEmpSalary.salMonth, regEmpSalary.salYear,
                    regEmpSalary.BranchesExcecptHO, regEmpSalary.AllEmployees,
                    regEmpSalary.selectedEmployeeTypeID.Value, regEmpSalary.selectedBranchID,
                    regEmpSalary.selectedEmployeeID);
                // Added new logic on 16-06-2021,i.e. replace E_Basic value based on E_Basic_Pay column value.
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TblMstEmployeeSalary, EmployeeSalary>()
                    .ForMember(d => d.JoinedAfterNoon, o => o.MapFrom(s => (s.tblMstEmployee.IsJoinAfterNoon == true ? ((s.tblMstEmployee.DOJ.Year.ToString() + s.tblMstEmployee.DOJ.Month.ToString()) == (regEmpSalary.salYear.ToString() + regEmpSalary.salMonth.ToString())) ? true : false : false)))
                    .ForMember(d => d.DOJ, o => o.MapFrom(s => s.tblMstEmployee.DOJ))
                    .ForMember(d => d.SuspensionPeriods, o => o.MapFrom(s => s.tblMstEmployee.EmployeeSuspensionPeriods.Where(y => y.EmployeeID == s.EmployeeID)))
                    .ForMember(d => d.PFNO, o => o.MapFrom(s => s.tblMstEmployee.PFNO))
                    .ForMember(d => d.EmployeeCategory, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCategory.EmplCatName))
                    .ForMember(d => d.pFLoanAccuralRate, o => o.UseValue(regEmpSalary.pFLoanAccuralRate ?? 0))
                    .ForMember(d => d.E_Basic, o => o.MapFrom(s => (s.E_Basic_Pay != null && s.E_Basic_Pay != 0) ? s.E_Basic_Pay : s.E_Basic))
                    .ForMember(d => d.BranchID, o => o.MapFrom(s => (s.tblMstEmployee.BranchID_Pay != null && s.tblMstEmployee.BranchID_Pay != 0) ? s.tblMstEmployee.BranchID_Pay : s.tblMstEmployee.BranchID))
                    ;

                    cfg.CreateMap<DTOModel.EmployeeSuspensionPeriod, EmployeeSuspensionPeriod>();

                });
                var monthlySalary = Mapper.Map<List<EmployeeSalary>>(dtoEmpSalaries);

                #region Load Loan Priority/ Loan Trans Data===========

                var empCodes = (from x in monthlySalary select new { empCode = x.EmployeeCode }).ToList();
                DataTable dtEmpCodes = Common.ExtensionMethods.ToDataTable(empCodes);

                IGenerateSalaryRepository gSRepo = new GenerateSalaryRepository();
                DataSet dsLoanMaster = new DataSet();

                var period = $"{regEmpSalary.salYear.ToString()}{ regEmpSalary.salMonth.ToString("00")}";


                ///==== Delete tblLoanTran Data ========   Dated On - 28-05-2020

                if (regEmpSalary.selectedEmployeeID.HasValue)
                {
                    regEmpSalary.selectedEmployeeTypeID = genericRepo.GetByID<DTOModel.tblMstEmployee>(regEmpSalary.selectedEmployeeID.Value)
                        .EmployeeTypeID;
                }
                if (genericRepo.Exists<DTOModel.tblLoanTran>(x => x.PeriodOfPayment == period))
                {
                    var prevRecords = genericRepo.Get<DTOModel.tblLoanTran>(x => x.PeriodOfPayment == period
                     && x.tblMstEmployee.EmployeeTypeID == regEmpSalary.selectedEmployeeTypeID);
                    genericRepo.DeleteAll<DTOModel.tblLoanTran>(prevRecords);
                }

                if (!genericRepo.Exists<DTOModel.tblMstLoanPriorityHistory>(x => x.period == period))
                {
                    dsLoanMaster = gSRepo.GetLoanPriorityMasterData(dtEmpCodes, $"{regEmpSalary.PrevYear.ToString()}{ regEmpSalary.PrevMonth.ToString("00")}");
                    generateSalaryRepo.InsertIntoLoanPriorityHistory(period);
                }
                else
                {
                    dsLoanMaster = gSRepo.GetLoanPriorityHistoryData(dtEmpCodes, $"{regEmpSalary.PrevYear.ToString()}{ regEmpSalary.PrevMonth.ToString("00")}", period);
                }

                if (dsLoanMaster != null)
                {
                    PayrollMaster.LoanPriorityOld = dsLoanMaster.Tables[0];
                    PayrollMaster.LoanPriorityNew = dsLoanMaster.Tables[1];
                    PayrollMaster.LoanTransOld = dsLoanMaster.Tables[2];
                    PayrollMaster.LoanTransNew = dsLoanMaster.Tables[3];
                }
                #endregion

                #region Get Pf Opening Balance Data ==========

                var dtPFOpBalance = gSRepo.GetPFOpBalance(regEmpSalary.PrevYear, regEmpSalary.PrevMonth, dtEmpCodes);
                if (dtPFOpBalance != null)
                    PayrollMaster.empPFBalanceDT = dtPFOpBalance;

                #endregion

                ReadTCSFileInputs(monthlySalary, regEmpSalary.TCSFileName);

                var dtoMonthlyInput = generateSalaryRepo.GetMonthlyInputList(regEmpSalary.BranchesExcecptHO,
                    regEmpSalary.AllEmployees, regEmpSalary.salMonth, regEmpSalary.salYear,
                    regEmpSalary.selectedEmployeeTypeID.Value, regEmpSalary.selectedBranchID,
                    regEmpSalary.selectedEmployeeID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TBLMONTHLYINPUT, TBLMONTHLYINPUT>();
                });
                var monthlyInput = Mapper.Map<List<TBLMONTHLYINPUT>>(dtoMonthlyInput);

                /// === Get All allowances master here if ,there is any single employee whose alltendance >0 ====

                if (monthlyInput.Any(y => y.LWP != y.DaysInMonth))
                {
                    PayrollMaster.F_LoanPriorityNew = null;
                    PayrollMaster.F_LoanTransNew = null;
                    PayrollMaster.F_LoanPriorityOld = null;
                    PayrollMaster.F_LoanTransOld = null;

                    GetPayrollMaster();
                }
                if (regEmpSalary.selectedEmployeeTypeID.HasValue && regEmpSalary.selectedEmployeeTypeID.Value > 0)
                {
                    var dtoHeads = GetSalaryHeads(regEmpSalary.selectedEmployeeTypeID.Value);
                    salHeads = dtoHeads.Where(x => x.ActiveField);
                    nonActiveHeads = dtoHeads.Where(x => x.ActiveField == false).Select(x => x.FieldName).ToList();
                    PayrollMaster.salHeads = salHeads;
                }
                else
                {
                    var employeeType = genericRepo.GetByID<DTOModel.tblMstEmployee>(regEmpSalary.selectedEmployeeID.Value).EmployeeTypeID;
                    var dtoHeads = GetSalaryHeads(employeeType);
                    salHeads = dtoHeads.Where(x => x.ActiveField).ToList();
                    nonActiveHeads = dtoHeads.Where(x => x.ActiveField == false).Select(x => x.FieldName).ToList();
                    PayrollMaster.salHeads = salHeads;
                }

                if (salHeads.Any(y => y.LocationDependent == true))
                {
                    var branchIds = monthlySalary.Select(x => x.BranchID).Distinct().ToArray<int>();
                    var headArray = salHeads.Select(x => x.FieldName).ToArray<string>();

                    var dtoBSalaryHeadRules = genericRepo.GetIQueryable<DTOModel.BranchSalaryHeadRule>(x => !x.IsDeleted
                     && x.EmployeeTypeID == regEmpSalary.selectedEmployeeTypeID
                     && branchIds.Any(y => y == x.BranchID) && headArray.Any(h => h == x.FieldName)).ToList();

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.BranchSalaryHeadRule, BranchSalaryHeadRule>()
                        .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.Branch.BranchCode));
                    });
                    PayrollMaster.branchSalaryHeadRule = Mapper.Map<List<BranchSalaryHeadRule>>(dtoBSalaryHeadRules);
                }

                dynamic eSalWithNonActiveHeads = null;

                if (nonActiveHeads.Count() > 0)
                {
                    eSalWithNonActiveHeads = new ExpandoObject();
                    foreach (var item in nonActiveHeads)
                        ((IDictionary<string, object>)eSalWithNonActiveHeads)[item] = (decimal?)0;
                }
                var lastDateOfSelectedMonth = new DateTime(regEmpSalary.salYear, regEmpSalary.salMonth, DateTime.DaysInMonth(regEmpSalary.salYear, regEmpSalary.salMonth));
                for (int i = 0; i < monthlySalary.Count; i++)
                {
                    var mRow = monthlyInput.Where(x => x.EmployeeCode == monthlySalary[i].EmployeeCode).FirstOrDefault();
                    if (mRow != null)
                    {
                        if (eSalWithNonActiveHeads != null && eSalWithNonActiveHeads != null)
                            DynamicMapper<EmployeeSalary>.Map(eSalWithNonActiveHeads, monthlySalary[i]);

                        monthlySalary[i].BranchID = mRow.BranchId ?? 0;
                        monthlySalary[i].BranchCode = mRow.BranchCode;
                        var IsEmpDoleave = genericRepo.GetByID<DTOModel.tblMstEmployee>(monthlyInput[i].EmployeeId);
                        if (IsEmpDoleave != null)
                        {
                            if (IsEmpDoleave.DOLeaveOrg.HasValue && IsEmpDoleave.DOLeaveOrg.Value.Month == monthlyInput[i].SalMonth && IsEmpDoleave.DOLeaveOrg.Value.Year == monthlyInput[i].SalYear)
                            {
                                var totalLWPDys = (lastDateOfSelectedMonth.Subtract(IsEmpDoleave.DOLeaveOrg.Value.Date).TotalDays);
                                monthlyInput[i].LWP = Convert.ToInt32(totalLWPDys);
                            }
                        }
                        var f_M_Salary_ROW = OnSalaryRowProcessing((i + 1), monthlySalary[i], mRow, salHeads);
                        listFMonthlySalary.Add(f_M_Salary_ROW);
                    }
                }

                flag = SaveCalculatedSalary(listFMonthlySalary, regEmpSalary.AllBranches,
                      regEmpSalary.BranchesExcecptHO, regEmpSalary.selectedBranchID,
                      regEmpSalary.selectedEmployeeTypeID);

                Task tUpdatePension = Task.Run(() => generateSalaryRepo.UpdateEmpPensionDeduction(regEmpSalary.salMonth, regEmpSalary.salYear,
                          regEmpSalary.BranchesExcecptHO, regEmpSalary.AllEmployees, regEmpSalary.selectedEmployeeTypeID.Value, regEmpSalary.selectedBranchID,
                          regEmpSalary.selectedEmployeeID));
                negSalEmp = listFMonthlySalary.Where(x => x.chkNegative == true).Select(y => y.EmployeeCode).ToList();
                log.Info($"flag-Value={flag}");

                return flag;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IEnumerable<SalaryHead> GetSalaryHeads(int employeeType)
        {
            log.Info($"SalaryGenerator/GetSalaryHeads/{employeeType}");

            IEnumerable<DTOModel.SalaryHead> salaryHeads = Enumerable.Empty<DTOModel.SalaryHead>();
            //if (employeeType == 5)
            //{
            salaryHeads = genericRepo.Get<DTOModel.SalaryHead>(x => x.EmployeeTypeID == employeeType
           /* && x.ActiveField == true*/ && !x.IsDeleted).OrderBy(y => y.SeqNo);
            //}

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.SalaryHead, SalaryHead>()
                 .ForMember(d => d.FieldName, o => o.MapFrom(s => s.FieldName))
                 .ForMember(d => d.FieldDesc, o => o.MapFrom(s => s.FieldDesc))
                 .ForMember(d => d.FormulaColumn, o => o.MapFrom(s => s.FormulaColumn))
                 .ForMember(d => d.A, o => o.MapFrom(s => s.A))
                 .ForMember(d => d.Abbreviation, o => o.MapFrom(s => s.Abbreviation))
                 .ForMember(d => d.ActiveField, o => o.MapFrom(s => s.ActiveField))
                 .ForMember(d => d.AttendanceDep, o => o.MapFrom(s => s.AttendanceDep))
                 .ForMember(d => d.C, o => o.MapFrom(s => s.C))
                 .ForMember(d => d.Conditional, o => o.MapFrom(s => s.Conditional))
                 .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                 .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                 .ForMember(d => d.CT, o => o.MapFrom(s => s.CT))
                 .ForMember(d => d.CW, o => o.MapFrom(s => s.CW))
                 .ForMember(d => d.DC, o => o.MapFrom(s => s.DC))
                 .ForMember(d => d.DW, o => o.MapFrom(s => s.DW))
                 .ForMember(d => d.FromMaster, o => o.MapFrom(s => s.FromMaster))
                 .ForMember(d => d.LoanHead, o => o.MapFrom(s => s.LoanHead))
                 .ForMember(d => d.LookUpHead, o => o.MapFrom(s => s.LookUpHead))
                 .ForMember(d => d.LookUpHeadName, o => o.MapFrom(s => s.LookUpHeadName))
                 .ForMember(d => d.LocationDependent, o => o.MapFrom(s => s.LocationDependent))
                 .ForMember(d => d.MonthlyInput, o => o.MapFrom(s => s.MonthlyInput))
                 .ForMember(d => d.MT, o => o.MapFrom(s => s.MT))
                 .ForMember(d => d.RoundingUpto, o => o.MapFrom(s => s.RoundingUpto))
                 .ForMember(d => d.RoundToHigher, o => o.MapFrom(s => s.RoundToHigher))
                 .ForMember(d => d.SeqNo, o => o.MapFrom(s => s.SeqNo))
                 .ForMember(d => d.SpecialField, o => o.MapFrom(s => s.SpecialField))
                 .ForMember(d => d.SpecialFieldMaster, o => o.MapFrom(s => s.SpecialFieldMaster))
                 .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                 .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                 .ForMember(d => d.FixedValue, o => o.MapFrom(s => s.FixedValue))
                 .ForMember(d => d.FixedValueFormula, o => o.MapFrom(s => s.FixedValueFormula))
                 .ForMember(d => d.Slab, o => o.MapFrom(s => s.Slab))
                 .ForMember(d => d.LowerRange, o => o.MapFrom(s => s.LowerRange))
                 .ForMember(d => d.UpperRange, o => o.MapFrom(s => s.UpperRange))
                 .ForMember(d => d.EmployeeTypeID, o => o.MapFrom(s => s.EmployeeTypeID))
                 .ForMember(d => d.CheckHeadInEmpSalTable, o => o.MapFrom(s => s.CheckHeadInEmpSalTable))
                //.ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                //.ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))

                .ForAllOtherMembers(d => d.Ignore());
            });
            return Mapper.Map<IEnumerable<SalaryHead>>(salaryHeads);
        }

        private FinalMonthlySalary OnSalaryRowProcessing(int payslip_no, EmployeeSalary eSalary, TBLMONTHLYINPUT mInput, IEnumerable<SalaryHead> salHeads)
        {
            log.Info($"SalaryGenerator/OnSalaryRowProcessing/{eSalary.EmployeeCode}");

            try
            {
                FinalMonthlySalary fMonthlySalary = new FinalMonthlySalary();
                //var daysInMonth = DateTime.DaysInMonth(mInput.SalYear, mInput.SalMonth);

                bool IsLockPFLoan = mInput?.DeductPFLoan ?? false;
                bool IsLockHouseLoan = mInput?.DeductHouseLoan ?? false;
                bool IsLockFestivalLoan = mInput?.DeductFestivalLoan ?? false;
                bool IsLockCarLoan = mInput?.DeductCarLoan ?? false;
                bool IsLockScooterLoan = mInput?.DeductScooterLoan ?? false;
                bool IsLockSundryAdv = mInput?.DeductSundryAdv ?? false;
                bool IsLockNB = mInput?.DeductNB ?? false;
                bool IsSalLock = mInput?.SalaryLock ?? false;
                bool IsLockTCS = IsSalLock;

                //if(!IsSalLock)
                //{
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<EmployeeSalary, FinalMonthlySalary>()
                    .ForMember(d => d.E_Basic_D, o => o.MapFrom(s => s.E_Basic))
                    .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.BranchCode))
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.BranchID, o => o.MapFrom(s => s.BranchID))
                    .ForMember(d => d.DeductPFLoan, o => o.UseValue(IsLockPFLoan))
                    .ForMember(d => d.DeductHouseLoan, o => o.UseValue(IsLockHouseLoan))
                    .ForMember(d => d.DeductFestivalLoan, o => o.UseValue(IsLockFestivalLoan))
                    .ForMember(d => d.DeductCarLoan, o => o.UseValue(IsLockCarLoan))
                    .ForMember(d => d.DeductScooterLoan, o => o.UseValue(IsLockScooterLoan))
                    .ForMember(d => d.DeductSundryAdv, o => o.UseValue(IsLockSundryAdv))
                    .ForMember(d => d.DeductNB, o => o.UseValue(IsLockNB))
                    .ForMember(d => d.DeductTCS, o => o.UseValue(IsLockTCS))
                    .ForMember(d => d.chkNegative, o => o.UseValue(false))
                    .ForMember(d => d.chkAlwaysNegative, o => o.UseValue(false))
                    .ForMember(d => d.SalMonth, o => o.UseValue(mInput.SalMonth))
                    .ForMember(d => d.SalYear, o => o.UseValue(mInput.SalYear))
                    .ForMember(d => d.LWP, o => o.UseValue(mInput.LWP))
                    .ForMember(d => d.WorkingDays, o => o.UseValue(mInput.DaysInMonth))
                    .ForMember(d => d.Attendance, o => o.UseValue(mInput.DaysInMonth - mInput.LWP))
                    .ForMember(d => d.PaySlipNo, o => o.UseValue(payslip_no))  ///=== adding pay_slip no , based branch/ salary / sen_code 
                    .ForMember(d => d.DateofGenerateSalary, o => o.UseValue(DateTime.Now))
                    .ForMember(d => d.SeqNo, o => o.UseValue(1))
                    .ForMember(d => d.RateVPFA, o => o.MapFrom(s => s.IsRateVPF == true ? s.VPFValueRA : 0))
                    .ForMember(d => d.IsSupensionPeriodExists,
                    o => o.MapFrom(s => (s.SuspensionPeriods.Count > 0 ? true : false)));
                });
                //s => s.VPFValueRA
                fMonthlySalary = Mapper.Map<FinalMonthlySalary>(eSalary);

                if (fMonthlySalary.Attendance > 0)
                    EvaluateHeadFields(fMonthlySalary, eSalary, mInput, salHeads);

                fMonthlySalary.empPFOpBalance =
                            CalculatePFOpBalance(fMonthlySalary.SalYear, fMonthlySalary.SalMonth,
                            fMonthlySalary.PrevYear, fMonthlySalary.PrevMonth,
                            eSalary.EmployeeCode, eSalary.BranchCode, fMonthlySalary.D_PF_A,
                            fMonthlySalary.D_VPF_A, fMonthlySalary.C_Pension, eSalary.IsPensionDeducted,
                            eSalary.PFNO, eSalary.pFLoanAccuralRate);

                fMonthlySalary.empPFOpBalance.EmployeeID = eSalary.EmployeeID;
                fMonthlySalary.empPFOpBalance.BranchID = (int?)eSalary.BranchID;
                if (fMonthlySalary.C_NetSal < 0)
                {
                    fMonthlySalary.chkNegative = true;
                }
                return fMonthlySalary;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        /// <summary>
        /// Calculate Employee PF Opening Balance ..  
        /// </summary>
        /// <param name="salYear"></param>
        /// <param name="salMonth"></param>
        /// <param name="empCode"></param>
        /// <param name="bCode"></param>
        /// <param name="D_PF_A"></param>
        /// <param name="D_VPF_A"></param>
        /// <param name="C_Pension"></param>
        /// <param name="PFNo"></param>
        /// <returns></returns>
        private EmpPFOpBalance CalculatePFOpBalance(short salYear, byte salMonth, short prevYear, byte prevMonth, string empCode,
           string bCode, decimal D_PF_A, decimal D_VPF_A, decimal C_Pension, bool IsPensionDeducted, int? PFNo, decimal pFLoanAccuralRate)
        {
            log.Info($"SalaryGenerator/EmpPFOpBalance");
            try
            {


                EmpPFOpBalance empPFBalance = new EmpPFOpBalance();

                double emplOpbalance = 0, emplrOpbalance = 0, totalPFOpeningEmpl = 0, totalPFOpeningEmplr = 0;

                decimal InterestNRPFLoan = 0, nonRefundLoan = 0;
                decimal emplContribution = 0, emplrContribution = 0;
                decimal employeeInterest = 0, employerInterest = 0, interestTotal = 0, totalPFBalance = 0;

                int days = 0;
                DateTime? dateAvailLoan = null;

                DataRow[] dr = PayrollMaster.empPFBalanceDT.Select("Employeecode ='" + empCode + "'");

                if (dr != null && dr.Count() > 0)
                {
                    emplOpbalance = Convert.ToDouble(dr[0]["TotalPFOpeningEmpl"].ToString());
                    emplrOpbalance = Convert.ToDouble(dr[0]["TotalPFOpeningEmplr"].ToString());
                }

                //var nonRefundablePFLoanRow = genericRepo.Get<DTOModel.tblmstNRPFLoan>(x => x.EmployeeCode == empCode && x.LoanDeducted == 0)
                //    .FirstOrDefault();

                //if (nonRefundablePFLoanRow != null)
                //{
                //    nonRefundLoan = nonRefundablePFLoanRow.SancAmt ?? 0;
                //    if (nonRefundLoan > 0)
                //        dateAvailLoan = nonRefundablePFLoanRow.DateAvailLoan;
                //}

                if (nonRefundLoan > 0)
                {
                    days = DateTime.DaysInMonth(dateAvailLoan.Value.Year, dateAvailLoan.Value.Month) - dateAvailLoan.Value.Day;
                    InterestNRPFLoan = Math.Round(((nonRefundLoan * pFLoanAccuralRate) / (1200 * days)), 1);
                }
                emplOpbalance = emplOpbalance - (double)nonRefundLoan;
                emplContribution = D_PF_A + D_VPF_A;

                if (D_PF_A > 0)
                    emplrContribution = D_PF_A - C_Pension;
                if (emplOpbalance > 0)
                    employeeInterest = Math.Round((((decimal)emplOpbalance * pFLoanAccuralRate) / 1200), 2);
                if (emplrOpbalance > 0)
                    employerInterest = Math.Round((((decimal)emplrOpbalance * pFLoanAccuralRate) / 1200), 2);

                interestTotal = employeeInterest + employerInterest;
                totalPFBalance = (decimal)(emplOpbalance) + emplContribution + (decimal)emplrOpbalance + emplrContribution + interestTotal;
                totalPFOpeningEmpl = emplOpbalance + (double)emplContribution;
                totalPFOpeningEmplr = emplrOpbalance + (double)emplrContribution;

                #region ///////=====  Fill all pf related calculated values into model - Dated on - 15-Apr -2020 === =============

                empPFBalance.Employeecode = empCode;
                empPFBalance.BRANCHCODE = bCode;
                empPFBalance.PFAcNo = PFNo ?? 0;
                empPFBalance.Salmonth = salMonth;
                empPFBalance.Salyear = salYear;
                empPFBalance.EmplOpBal = emplOpbalance;
                empPFBalance.EmplrOpBal = emplrOpbalance;
                empPFBalance.EmployeePFCont = D_PF_A;
                empPFBalance.VPF = D_VPF_A;
                empPFBalance.Pension = C_Pension;

                empPFBalance.PensionDeduct = (bool?)IsPensionDeducted;  /// === Added new line - 15-May-2020

                empPFBalance.EmployerPFCont = emplrContribution;
                empPFBalance.EmployeeInterest = employeeInterest;
                empPFBalance.EmployerInterest = employerInterest;
                empPFBalance.InterestNRPFLoan = InterestNRPFLoan;
                empPFBalance.InterestRate = pFLoanAccuralRate;
                empPFBalance.NonRefundLoan = nonRefundLoan;
                empPFBalance.TotalPFBalance = totalPFBalance;
                empPFBalance.InterestTotal = interestTotal;
                empPFBalance.TotalPFOpeningEmpl = totalPFOpeningEmpl;
                empPFBalance.TotalPFOpeningEmplr = totalPFOpeningEmplr;
                empPFBalance.WithdrawlEmployeeAc = nonRefundLoan;
                empPFBalance.WithdrawlEmployerAc = 0;
                empPFBalance.AdditionEmployeeAc = 0;
                empPFBalance.AdditionEmployerAc = 0;
                empPFBalance.IntWDempl = 0;
                empPFBalance.IntWDemplr = 0;

                #endregion

                return empPFBalance;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get All allowances === 
        /// </summary>

        void GetPayrollMaster()
        {
            log.Info($"SalaryGenerator/GetPayrollMaster");

            #region Fill FDA list===

            var dtoFDA = genericRepo.Get<DTOModel.tblmstFDA>(x => !x.IsDeleted).OrderBy(y => y.upperlimit);

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.tblmstFDA, FDA>();
            });
            PayrollMaster.fdaSlab = Mapper.Map<List<FDA>>(dtoFDA);

            #endregion

            #region Fill CCA list =====

            var dtoCCASlab = genericRepo.Get<DTOModel.tblCCAMaster>(x => !x.IsDeleted).OrderBy(y => y.UpperLimit);
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.tblCCAMaster, CCA>();
            });
            PayrollMaster.CCASlabs = Mapper.Map<List<CCA>>(dtoCCASlab);

            #endregion

            #region Fill Hill Compensation === 

            var dtoHillComp = genericRepo.Get<DTOModel.tblMstHillComp>(x => !x.IsDeleted).OrderBy(y => y.UpperLimit);
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.tblMstHillComp, HillCompensation>();
            });
            PayrollMaster.hillCompensationSlab = Mapper.Map<List<HillCompensation>>(dtoHillComp);

            #endregion

            #region Fill Branch List ========

            var dtoBranch = genericRepo.Get<DTOModel.Branch>(x => !x.IsDeleted);
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.Branch, Branch>()
                .ForMember(d => d.BranchID, o => o.MapFrom(s => s.BranchID))
                .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.BranchCode))
                .ForMember(d => d.GradeID, o => o.MapFrom(s => s.GradeID))
                .ForMember(d => d.GradeName, o => o.MapFrom(s => s.Grade.GradeName))
                .ForAllOtherMembers(d => d.Ignore());
            });

            PayrollMaster.branchList = Mapper.Map<List<Branch>>(dtoBranch);

            #endregion

            #region Fill Loan Type ====

            var dtoLoanType = genericRepo.Get<DTOModel.tblMstLoanType>();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.tblMstLoanType, tblMstLoanType>();
            });
            PayrollMaster.loanTypes = Mapper.Map<List<tblMstLoanType>>(dtoLoanType);

            #endregion

            #region  Fill OTA Rates =======

            IGenerateSalaryRepository gSRepo = new GenerateSalaryRepository();
            var otaRates = gSRepo.GetOTARates();
            PayrollMaster.OTARates = otaRates;

            #endregion

            #region GIS Deduction ==== 

            var dtoGISDeduction = genericRepo.Get<DTOModel.tblGisDeduction>(x => !x.IsDeleted);

            Mapper.Initialize(cfg =>
            {

                cfg.CreateMap<DTOModel.tblGisDeduction, GisDeduction>();
            });
            PayrollMaster.gisDeductions = Mapper.Map<List<GisDeduction>>(dtoGISDeduction);

            #endregion

            #region Loan Slab ==== 

            var dtoLoanSlab = genericRepo.Get<DTOModel.tblmstslab>(x => !x.IsDeleted);
            Mapper.Initialize(cfg =>
            {

                cfg.CreateMap<DTOModel.tblmstslab, LoanSlab>();
            });
            PayrollMaster.loanSlab = Mapper.Map<List<LoanSlab>>(dtoLoanSlab);

            #endregion
        }

        void EvaluateHeadFields(FinalMonthlySalary fmSalary, EmployeeSalary empSal, TBLMONTHLYINPUT mInputs, IEnumerable<SalaryHead> salHeads)
        {
            log.Info($"SalaryGenerator/EvaluateHeadFields/EmployeeCode:{empSal.EmployeeCode}");

            string colName = string.Empty, colName_A = string.Empty;
            bool AD = false, MI = false, FC = false, LH = false, COND = false, RND = false,
            FV = false, FromMaster = false, DRF = false, chkMedical = false,
            chkBonus = false, chkExgratia = false;
            int RNDTO = 0, ExgratiaDays = 0, InstNo = 0;

            decimal BonusRate = 0, LWP = fmSalary.LWP ?? 0;
            string LastPaidInstDeduDt = $"{fmSalary.SalYear.ToString()}{fmSalary.SalMonth.ToString("00")}";

            try
            {
                foreach (var item in salHeads)
                {
                    decimal colValue = 0, colValue_A = 0, calculatedValue = 0;
                    colName = item.FieldName; AD = item.AttendanceDep;
                    MI = item.MonthlyInput; FC = item.FormulaColumn;
                    LH = item.LoanHead;
                    COND = item.Conditional; RND = item.RoundToHigher;
                    RNDTO = item.RoundingUpto ?? 0;
                    FV = item.FixedValueFormula;
                    FromMaster = item.FromMaster;


                    //if (colName == "D_18" && empSal.EmployeeCode == "0636")
                    //{
                    //    var mmm = 12;
                    //}
                    if (colName == "D_VPF")
                    {
                        if ((empSal.D_VPF ?? 0) > 100)
                            AD = false;
                    }
                    if (FV)
                        colValue = FixedValueFormula(item, empSal);

                    else if (!MI && !FC && !LH && !COND)
                    {
                        // ColValue = === === ==== ================================

                        if (colName == "D_09")
                        {
                            if (!string.IsNullOrEmpty(empSal.EmployeeCategory))
                            //===== Set colvalue ,that would be GIS Deduction Rate===
                            {
                                if (PayrollMaster.gisDeductions.Count > 0)
                                {
                                    var g_row = PayrollMaster.gisDeductions.FirstOrDefault(y => y.Category.Trim() == empSal.EmployeeCategory);
                                    if (g_row != null)
                                        colValue = g_row.Rate ?? 0;
                                }
                            }
                        }
                    }
                    else if (!AD && !MI && !FC && !LH && !COND)
                        colValue = 0;

                    else if (!MI && !FC && !LH && COND)
                    {
                        var prop = GetModelPropertyValue<EmployeeSalary>(empSal, colName);
                        calculatedValue = Convert.ToDecimal(prop);
                        colValue = ConditionalHead(colName, calculatedValue, empSal, DRF, mInputs.DaysInMonth,
                                  chkMedical, chkBonus, chkExgratia, BonusRate, ExgratiaDays, LWP);
                    }
                    else if (LH && PayrollMaster.loanTypes.Any(y => y.LoanType == colName))
                    {
                        int value = 0;

                        if (colName != "D_03")
                        {
                            DataRow drblMstLoanOld = null;
                            DataRow[] drArray = PayrollMaster.LoanPriorityOld.Select("EmpCode ='" + empSal.EmployeeCode + "' and LoanType = '" + colName + "' and IsNewLoanAfterDevelop = 0 and (RemainingPInstNo > 0  or RemainingIInstNo > 0 ) and status = 0");

                            if (drArray.Count() > 0)
                            {
                                drblMstLoanOld = drArray[0];
                                value = drblMstLoanOld["EmpCode"] != null ? 1 : 0;
                            }
                            if (value > 0)
                            {
                                ///==== Call a function to calculate old loan   ====  drblMstLoanOld ====== 
                                colValue = CalculateOldLoan(empSal.BranchCode, empSal.EmployeeCode, colName, false, drblMstLoanOld, LastPaidInstDeduDt);

                                if ((fmSalary.WorkingDays - fmSalary.LWP) > 0)
                                {
                                    InstNo = PayrollMaster.InstNo;
                                    if (colName == "D_15")
                                    {
                                        var fmsPFLoanInstNo_PROP = fmSalary.GetType().GetProperty("PFLoanInstNo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                        fmsPFLoanInstNo_PROP.SetValue(fmSalary, InstNo, null);
                                    }
                                    else if (colName == "D_17")
                                    {
                                        var fmsPFLoanInstNo_PROP = fmSalary.GetType().GetProperty("HBLoanInstNo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                        fmsPFLoanInstNo_PROP.SetValue(fmSalary, InstNo, null);
                                    }
                                    else if (colName == "D_18")
                                    {
                                        var fmsPFLoanInstNo_PROP = fmSalary.GetType().GetProperty("FestivalLoanInstNo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                        fmsPFLoanInstNo_PROP.SetValue(fmSalary, InstNo, null);
                                    }
                                    else if (colName == "D_19")
                                    {
                                        var fmsPFLoanInstNo_PROP = fmSalary.GetType().GetProperty("CarLoanInstNo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                        fmsPFLoanInstNo_PROP.SetValue(fmSalary, InstNo, null);
                                    }
                                    else if (colName == "D_20")
                                    {
                                        var fmsPFLoanInstNo_PROP = fmSalary.GetType().GetProperty("SundryAdvInstNo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                        fmsPFLoanInstNo_PROP.SetValue(fmSalary, InstNo, null);
                                    }
                                    else if (colName == "D_21")
                                    {
                                        var fmsPFLoanInstNo_PROP = fmSalary.GetType().GetProperty("ScooterLoanInstNo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                        fmsPFLoanInstNo_PROP.SetValue(fmSalary, InstNo, null);
                                    }
                                }
                            }
                            else
                            {
                                DataRow drTblMstLoanNew = null;
                                DataRow[] drTblMstLoanNewArr = PayrollMaster.LoanPriorityNew.Select("Empcode = '" + empSal.EmployeeCode + "' and LoanType = '" + colName + "' and IsNewLoanAfterDevelop = 1 and  status = 0 and TotalBalanceAmt > 0", " MstLoanID DESC");

                                if (drTblMstLoanNewArr.Count() > 0)
                                {
                                    drTblMstLoanNew = drTblMstLoanNewArr[0];
                                    value = drTblMstLoanNew["EmpCode"] != null ? 1 : 0;
                                }
                                if (value > 0)
                                {
                                    //===== Call a function to calculate new loan ///====== ===


                                    //if (empSal.EmployeeCode == "0636")
                                    //{
                                    //    var ffgg = empSal.BranchCode;
                                    //}

                                    colValue = CalculateNewLoan(fmSalary.BranchCode, empSal.EmployeeCode, colName, false, drTblMstLoanNew, LastPaidInstDeduDt);

                                    if ((fmSalary.WorkingDays - fmSalary.LWP) > 0)
                                    {
                                        InstNo = PayrollMaster.InstNo;
                                        if (colName == "D_15")
                                        {
                                            var fmsPFLoanInstNo_PROP = fmSalary.GetType().GetProperty("PFLoanInstNo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                            fmsPFLoanInstNo_PROP.SetValue(fmSalary, InstNo, null);
                                        }
                                        else if (colName == "D_17")
                                        {
                                            var fmsPFLoanInstNo_PROP = fmSalary.GetType().GetProperty("HBLoanInstNo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                            fmsPFLoanInstNo_PROP.SetValue(fmSalary, InstNo, null);
                                        }
                                        else if (colName == "D_18")
                                        {
                                            var fmsPFLoanInstNo_PROP = fmSalary.GetType().GetProperty("FestivalLoanInstNo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                            fmsPFLoanInstNo_PROP.SetValue(fmSalary, InstNo, null);
                                        }
                                        else if (colName == "D_19")
                                        {
                                            var fmsPFLoanInstNo_PROP = fmSalary.GetType().GetProperty("CarLoanInstNo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                            fmsPFLoanInstNo_PROP.SetValue(fmSalary, InstNo, null);
                                        }
                                        else if (colName == "D_20")
                                        {
                                            var fmsPFLoanInstNo_PROP = fmSalary.GetType().GetProperty("SundryAdvInstNo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                            fmsPFLoanInstNo_PROP.SetValue(fmSalary, InstNo, null);
                                        }
                                        else if (colName == "D_21")
                                        {
                                            var fmsPFLoanInstNo_PROP = fmSalary.GetType().GetProperty("ScooterLoanInstNo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                            fmsPFLoanInstNo_PROP.SetValue(fmSalary, InstNo, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (!MI && FC && !LH && !COND)
                    {
                        //==== Call Calculate Formula Value ....
                        colValue = CalculateFromulaValue(item, empSal, fmSalary);
                    }
                    else if (!MI && FC && !LH && COND)
                    {
                        //==== Call Calculate Formula Value ....
                        calculatedValue = CalculateFromulaValue(item, empSal, fmSalary);
                        //if(empSal.IsRateVPF.Value &&  (empSal.VPFValueRA==(decimal?)24.00))
                        //  colValue = fmSalary.D_PF_A*2;
                        //else
                        colValue = ConditionalHead(colName, calculatedValue, empSal, DRF, mInputs.DaysInMonth, chkMedical, chkBonus, chkExgratia, BonusRate, ExgratiaDays, LWP);

                    }

                    else if (MI && !FC && !LH && !COND)
                    {
                        decimal rate = 0, maxAmt = 0;
                        var mPROP = GetModelPropertyValue<TBLMONTHLYINPUT>(mInputs, colName);
                        colValue = mPROP != null ? Convert.ToDecimal(mPROP) : 0;

                        if (colName == "E_03" || colName == "E_04")
                        {
                            ////////======== Get ota rate /==== Employee ota rate ...........................

                            var dtOTARates = PayrollMaster.OTARates;
                            DataRow[] dr = dtOTARates.Select("EmployeeCode='" + empSal.EmployeeCode + "'");

                            if (dr.Count() > 0)
                            {
                                rate = dr[0]["MaxRatePerHour"] != null ? Convert.ToDecimal(dr[0]["MaxRatePerHour"]) : 0;
                                maxAmt = dr[0]["MaxAmt"] != null ? Convert.ToDecimal(dr[0]["MaxAmt"]) : 0;
                            }
                        }
                        else if (colName == "E_03")
                        {
                            colValue = mInputs.OTHrs == null ? (decimal)0 : ((decimal)mInputs.OTHrs * rate);
                            if (maxAmt < colValue) colValue = maxAmt;
                        }
                        else if (colName == "E_04")
                            colValue = mInputs.AOTHrs == null ? 0 : ((decimal)mInputs.AOTHrs * rate);

                        else if (colName == "E_06")
                        {
                            if (fmSalary.LWP >= fmSalary.WorkingDays)
                                colValue = 0;
                            else
                            {
                                var mE_06 = GetModelPropertyValue<TBLMONTHLYINPUT>(mInputs, colName);
                                colValue = mE_06 != null ? Convert.ToDecimal(mE_06) : 0;
                            }
                        }
                        else if (colName == "E_07")
                        {
                            var mE_07 = GetModelPropertyValue<TBLMONTHLYINPUT>(mInputs, colName);
                            colValue = mE_07 != null ? Convert.ToDecimal(mE_07) : 0;
                        }
                    }
                    else if (MI && !FC && !LH && COND)
                    {
                        ////  get value by monthly input -master dependent on condition
                        var mColValue = GetModelPropertyValue<TBLMONTHLYINPUT>(mInputs, colName);
                        calculatedValue = mColValue != null ? Convert.ToDecimal(mColValue) : 0;
                        colValue = ConditionalHead(colName, calculatedValue, empSal, DRF, mInputs.DaysInMonth, chkMedical, chkBonus, chkExgratia, BonusRate, ExgratiaDays, LWP);
                    }
                    else if (MI && FC && !LH && !COND)
                    {
                        ////// get value by calculate formula on monthlyinput - master
                        colValue = CalculateFromulaValue(item, empSal, fmSalary);
                        // === Call calculate formula Value function...
                    }
                    else if (MI && FC && !LH && COND)
                    {
                        /////====get value by calculate formula on  monthlyinput - master dependent on condition
                        calculatedValue = CalculateFromulaValue(item, empSal, fmSalary);
                        colValue = ConditionalHead(colName, calculatedValue, empSal, DRF, mInputs.DaysInMonth, chkMedical, chkBonus, chkExgratia, BonusRate, ExgratiaDays, LWP);
                    }

                    if (colName == "D_PF")  /// === rounding for D_PF 
                    {
                        var result = colValue - Math.Truncate(colValue);
                        if (Convert.ToDecimal(result) == (decimal)0.5)
                        {
                            colValue = colValue + (decimal).1;
                        }
                    }

                    ///====  Set all the values in Final monthly row.......                
                    if (RND)
                        colValue = Math.Round(colValue, RNDTO);
                    //colValue = CustomRounding(colValue, RNDTO);

                    if (colName != "E_Basic")
                    {
                        var fMPROP = fmSalary.GetType().GetProperty(colName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        fMPROP.SetValue(fmSalary, colValue, null);
                    }
                    else
                    {
                        ///=== Added employee suspension period check condition --- SG (28-Aug-2020)
                        if (empSal.IsSuspended && empSal.SuspensionPeriods?.Count > 0)
                        {
                            // Decimal newBasic = 0;
                            var salMonthStartDate = new DateTime(fmSalary.SalYear, fmSalary.SalMonth, 1);
                            var salMonthEndDate = new DateTime(fmSalary.SalYear, fmSalary.SalMonth, mInputs.DaysInMonth);

                            if (empSal.SuspensionPeriods.Any(y =>
                                (y.PeriodFrom <= salMonthStartDate && y.PeriodTo <= salMonthEndDate)
                                ||
                                (y.PeriodFrom >= salMonthStartDate && y.PeriodTo <= salMonthEndDate)
                                ||
                                (y.PeriodFrom >= salMonthStartDate && salMonthEndDate >= y.PeriodFrom)))
                            {
                                ///=== The employee was suspended for some/full days of this month ......

                                var fmsE_Basic = fmSalary.GetType().GetProperty(colName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                var empE_Basic = empSal.GetType().GetProperty(colName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                                var suspensionInterval = empSal.SuspensionPeriods.Where(y =>
                                (y.PeriodFrom <= salMonthStartDate && y.PeriodTo <= salMonthEndDate)
                                ||
                                (y.PeriodFrom >= salMonthStartDate && y.PeriodTo <= salMonthEndDate)
                                ||
                                (y.PeriodFrom >= salMonthStartDate && salMonthEndDate >= y.PeriodFrom)).ToList();

                                var newBasic = WithSuspendedRemuneration(empSal.E_Basic.Value, salMonthStartDate, salMonthEndDate, suspensionInterval);

                                fmsE_Basic.SetValue(fmSalary, (decimal?)newBasic, null);
                                empE_Basic.SetValue(empSal, (decimal?)newBasic, null);

                                colValue = newBasic; /// ==== Override E_basic value ,as there were a case of suspension.
                            }
                        }
                        else
                        {
                            var mE_basic = GetModelPropertyValue<FinalMonthlySalary>(fmSalary, colName);
                            colValue = mE_basic != null ? Convert.ToDecimal(mE_basic) : 0;
                        }
                    }

                    ///====  For working days dependent (_A) fields .........
                    if (colName != "C_TotEarn" && colName != "C_TotDedu" && colName != "C_NetSal" && colName != "C_Pension"
                        && colName != "C_GrossSalary" && colName != "OTHrs" && colName != "AOTHrs" && colName != "LWP")
                    {
                        colName_A = colName + "_A";

                        colValue_A = colValue - ((colValue / mInputs.DaysInMonth) * (fmSalary.LWP + (empSal.JoinedAfterNoon ? 1 : 0)) ?? 0);

                        colValue_A = Math.Round(colValue_A, 1);  ///=== added new line --- 
                        var fmsCellProp = fmSalary.GetType().GetProperty(colName_A, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                        if (AD)
                        {
                            fmsCellProp.SetValue(fmSalary, (RND ? Math.Round(colValue_A, RNDTO) : colValue_A), null);
                            //  fmsCellProp.SetValue(fmSalary, (RND ? CustomRounding(colValue_A, RNDTO) : colValue_A), null);
                        }
                        else
                            fmsCellProp.SetValue(fmSalary, (RND ? Math.Round(colValue, RNDTO) : colValue), null);
                        // fmsCellProp.SetValue(fmSalary, (RND ? CustomRounding(colValue, RNDTO) : colValue), null);
                    }

                    if (colName == "C_Pension")
                    {
                        if (AD)
                        {
                            var MC_pension = fmSalary.GetType().GetProperty(colName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                            MC_pension.SetValue(fmSalary, Math.Round(colValue, RNDTO), null);
                        }
                    }

                    #region =======  TCS LOAN DEDUCTION ===================

                    decimal TCSLn = 0, GLInstallNo = 0, ELInstallNo = 0;

                    if (colName == "D_03" && LH)
                    {
                        if ((mInputs.SalaryLock ?? false) == false)
                        {
                            if (GLInstallNo == -1)
                                GLInstallNo = 0;
                            if (ELInstallNo == -1)
                                ELInstallNo = 0;

                            TCSLn = empSal.tcsln; GLInstallNo = empSal.gl_bal; ELInstallNo = empSal.el_bal;

                            var D_03_A_PROP = fmSalary.GetType().GetProperty("D_03_A", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                            if (AD)
                                D_03_A_PROP.SetValue(fmSalary, TCSLn - ((TCSLn / mInputs.DaysInMonth) * LWP), null);
                            else
                                D_03_A_PROP.SetValue(fmSalary, (decimal?)TCSLn, null);
                        }

                        var D_03_PROP = fmSalary.GetType().GetProperty(colName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        D_03_PROP.SetValue(fmSalary, (decimal?)TCSLn, null);

                        var GLInstallNo_PROP = fmSalary.GetType().GetProperty("GLInstallNo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        GLInstallNo_PROP.SetValue(fmSalary, (decimal?)GLInstallNo, null);

                        var ELInstallNo_PROP = fmSalary.GetType().GetProperty("ELInstallNo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        ELInstallNo_PROP.SetValue(fmSalary, (decimal?)ELInstallNo, null);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                string c = colName;

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        private object GetModelPropertyValue<T>(T model, string propertyName) where T : class
        {
            var prop = model.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            return prop.GetValue(model, null);
        }

        private void ReadTCSFileInputs(List<EmployeeSalary> empSal, string tcsFileName)
        {
            log.Info($"SalaryGenerator/ReadTCSFileInputs/tcsFileName={tcsFileName}");
            try
            {
                Import ob = new Import();
                string sFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DocumentUploadFilePath.TCSFilePath + "/" + tcsFileName);
                DataTable dtExcel = ob.ReadExcel(sFilePath);

                foreach (var item in empSal)
                {
                    DataRow[] dr = dtExcel.Select("emplcd ='" + item.EmployeeCode + "'");
                    if (dr != null && dr.Count() > 0)
                    {
                        item.tcsln = Convert.ToDecimal(dr[0]["tcsln"].ToString());
                        item.D_03 = item.tcsln;
                        item.el_bal = dr[0]["el-bal"] != null && dr[0]["el-bal"].ToString() != "" ? Convert.ToDecimal(dr[0]["el-bal"].ToString()) : 0;
                        item.gl_bal = dr[0]["gl_bal"] != null && dr[0]["gl_bal"].ToString() != "" ? Convert.ToDecimal(dr[0]["gl_bal"].ToString()) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        private decimal CalculateNewLoan(string branchCode, string employeeCode, string colName, bool chkRegenerate, DataRow drNewLoans, string LastPaidInstDeduDt)
        {
            log.Info($"SalaryGenerator/CalculateNewLoan/employeeCode:{employeeCode}&branchCode:{branchCode}&colName:{colName}");
            try
            {

                //if (employeeCode == "0636")
                //{

                //    var k = 1;
                //}

                DataRow dr_LTNew;
                decimal result = 0;
                bool isSlabDependent = false, isTDSRebet = false, isInterestPayable = false, isFloatingRate = false, status = false;

                int serialNo = 0, originalPInstNo = 0, originalIInstNo = 0, remainingPInstNo = 0, remainingIInstNo = 0, lastPaidPInstNo = 0, lastPaidIInstNo = 0, currentPInstNoPaid = 0, currentIInstNoPaid = 0,
                    totalSkippedInst = 0, rOIForchk = 0;
                //    int? referLoanID = null;
                decimal adjustedSancAmt = 0, sancAmt = 0, originalPInstAmt = 0, interestInstAmt = 0, balancePAmt = 0, balanceIAmt = 0, totalBalanceAmt = 0, lastPaidPInstAmt = 0, lastPaidIInstAmt = 0, lastMonthInterest = 0,
                    lastInstAmt = 0,
                 rOI = 0, currentROI = 0, currentPInstAmtPaid = 0, remainingPAmt = 0,
                  currentInterestAmt = 0, currentIInstAmtPaid = 0, remainingIAmt = 0, tDSAmt = 0, tDSRebetROI = 0, closingBal = 0, lIA = 0, intcalDummy = 0, installmentAmount = 0;

                DateTime dateAvailLoan; DateTime? dateEffective;

                string lastPaidInstDeduDt = string.Empty, loanPriorityNo = string.Empty, loanMode = string.Empty;

                DataTable DrLTnew = new DataTable();
                DataRow Dr_LTnew = DrLTnew.NewRow();
                var loanType = PayrollMaster.loanTypes.Where(x => x.LoanType == colName && !x.IsDeleted).FirstOrDefault();

                if (loanType != null)
                {
                    loanMode = loanType.PaymentType;
                    isSlabDependent = loanType.IsSlabDependent  /*loanType.IsSlabDependent == null ? false : loanType.IsSlabDependent.Value*/;
                    isTDSRebet = loanType.IsTDSRebet /*loanType.IsTDSRebet == null ? false : loanType.IsTDSRebet.Value*/;
                }
                if (loanMode.ToLower() == "Installment".ToLower())
                {
                    if (drNewLoans["SerialNo"].ToString() != "" && drNewLoans["SerialNo"].ToString() != null)
                    {
                        serialNo = Convert.ToInt32(drNewLoans["SerialNo"]);
                    }
                    sancAmt = (drNewLoans["SancAmt"] == null || drNewLoans["SancAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["SancAmt"]);
                    adjustedSancAmt = (drNewLoans["AdjustedSancAmt"] == null || drNewLoans["AdjustedSancAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["AdjustedSancAmt"]);
                    originalPInstNo = (drNewLoans["OriginalPInstNo"] == null || drNewLoans["OriginalPInstNo"].ToString() == "") ? 0 : Convert.ToInt32(drNewLoans["OriginalPInstNo"]);
                    originalIInstNo = (drNewLoans["OriginalIInstNo"] == null || drNewLoans["OriginalIInstNo"].ToString() == "") ? 0 : Convert.ToInt32(drNewLoans["OriginalIInstNo"]);
                    originalPInstAmt = (drNewLoans["OriginalPInstAmt"] == null || drNewLoans["OriginalPInstAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["OriginalPInstAmt"]);
                    interestInstAmt = (drNewLoans["InterestInstAmt"] == null || drNewLoans["InterestInstAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["InterestInstAmt"]);
                    balancePAmt = (drNewLoans["BalancePAmt"] == null || drNewLoans["BalancePAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["BalancePAmt"]);
                    balanceIAmt = (drNewLoans["BalanceIAmt"] == null || drNewLoans["BalanceIAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["BalanceIAmt"]);
                    totalBalanceAmt = (drNewLoans["TotalBalanceAmt"] == null || drNewLoans["TotalBalanceAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["TotalBalanceAmt"]);
                    remainingPInstNo = (drNewLoans["RemainingPInstNo"] == null || drNewLoans["RemainingPInstNo"].ToString() == "") ? 0 : Convert.ToInt32(drNewLoans["RemainingPInstNo"]);
                    remainingIInstNo = (drNewLoans["RemainingIInstNo"] == null || drNewLoans["RemainingIInstNo"].ToString() == "") ? 0 : Convert.ToInt32(drNewLoans["RemainingIInstNo"]);
                    lastPaidPInstAmt = (drNewLoans["LastPaidPInstAmt"] == null || drNewLoans["LastPaidPInstAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["LastPaidPInstAmt"]);
                    lastPaidIInstAmt = (drNewLoans["LastPaidIInstAmt"] == null || drNewLoans["LastPaidIInstAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["LastPaidIInstAmt"]);
                    lastPaidPInstNo = (drNewLoans["LastPaidPInstNo"] == null || drNewLoans["LastPaidPInstNo"].ToString() == "") ? 0 : Convert.ToInt32(drNewLoans["LastPaidPInstNo"]);
                    lastPaidIInstNo = (drNewLoans["LastPaidIInstNo"] == null || drNewLoans["LastPaidIInstNo"].ToString() == "") ? 0 : Convert.ToInt32(drNewLoans["LastPaidIInstNo"]);
                    lastPaidInstDeduDt = drNewLoans["LastPaidInstDeduDt"] == null ? "" : Convert.ToString(drNewLoans["LastPaidInstDeduDt"]);
                    lastMonthInterest = (drNewLoans["LastMonthInterest"] == null || drNewLoans["LastMonthInterest"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["LastMonthInterest"]);
                    lastInstAmt = (drNewLoans["LastInstAmt"] == null || drNewLoans["LastInstAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["LastInstAmt"]);
                    loanPriorityNo = drNewLoans["PriorityNo"] == null ? "" : Convert.ToString(drNewLoans["PriorityNo"]);
                    isInterestPayable = (drNewLoans["IsInterestPayable"] == null || drNewLoans["IsInterestPayable"].ToString() == "") ? false : Convert.ToBoolean(drNewLoans["IsInterestPayable"]);
                    isFloatingRate = (drNewLoans["IsFloatingRate"] == null || drNewLoans["IsFloatingRate"].ToString() == "") ? false : Convert.ToBoolean(drNewLoans["IsFloatingRate"]);
                    dateAvailLoan = Convert.ToDateTime(drNewLoans["DateAvailLoan"]);
                    dateEffective = drNewLoans["EffDate"].ToString() != "" ? Convert.ToDateTime(drNewLoans["EffDate"]) : (DateTime?)null;
                    rOI = (drNewLoans["ROI"] == null || drNewLoans["ROI"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["ROI"]);
                    //   referLoanID = (drNewLoans["RefLoanMstID"] == null || drNewLoans["RefLoanMstID"].ToString() == "") ? (int?)null : Convert.ToInt32(drNewLoans["ROI"]);
                    //  totalSkippedInst = PayrollMaster.LoanPriorityNew.Select("SerialNo = '" + serialNo + "' and PriorityNo = '" + loanPriorityNo + "'and SkippedInstNo > 0")?.Count()??0;

                    DataRow[] dr_Array_0 = PayrollMaster.LoanTransNew.Select("SerialNo = '" + serialNo + "' and PriorityNo = '" + loanPriorityNo + "'and SkippedInstNo > 0");
                    totalSkippedInst = dr_Array_0 != null ? dr_Array_0.Count() : 0;

                    decimal slab1 = 0, slab2 = 0, slab3 = 0, slabamt1 = 0, slabamt2 = 0;

                    if (dateEffective >= DateTime.Now.AddDays(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day))
                        return result;
                    if (balancePAmt > 0)
                    {
                        currentPInstAmtPaid = originalPInstAmt;

                        #region tblmstslab
                        if (isInterestPayable && isSlabDependent)
                        {
                            var slab = PayrollMaster.loanSlab.Where(x => x.AmountOfSlab <= balancePAmt && x.LoanType == colName && x.EffectiveDate <= dateAvailLoan).OrderByDescending(x => x.SlabNo).FirstOrDefault();
                            if (slab != null)
                                rOIForchk = slab.SlabNo == null ? 0 : slab.SlabNo.Value;
                            if (rOIForchk == 0)
                            {
                                var cROI = PayrollMaster.loanSlab.Where(x => x.AmountOfSlab >= balancePAmt && x.LoanType == colName && x.EffectiveDate <= dateAvailLoan).OrderByDescending(x => x.EffectiveDate).OrderBy(x => x.AmountOfSlab).FirstOrDefault();
                                if (cROI != null)
                                    currentROI = cROI.RateOfInterest == null ? 0 : cROI.RateOfInterest.Value;
                                slab1 = Math.Round(((balancePAmt * currentROI) / 1200));
                            }
                            else
                            {
                                var cROI = PayrollMaster.loanSlab.Where(x => x.LoanType == colName && x.EffectiveDate <= dateAvailLoan).OrderByDescending(x => x.EffectiveDate).OrderBy(x => x.AmountOfSlab).FirstOrDefault();
                                if (cROI != null)
                                    currentROI = cROI.RateOfInterest == null ? 0 : cROI.RateOfInterest.Value;

                                var amtOfSlab = PayrollMaster.loanSlab.Where(x => x.AmountOfSlab <= balancePAmt && x.LoanType == colName && x.EffectiveDate <= dateAvailLoan).OrderByDescending(x => x.EffectiveDate).OrderBy(x => x.AmountOfSlab).FirstOrDefault();

                                if (amtOfSlab != null)
                                    slabamt1 = amtOfSlab.AmountOfSlab == null ? 0 : cROI.AmountOfSlab.Value;

                                slab1 = Math.Round(((slabamt1 * currentROI) / 1200));
                            }

                            if (rOIForchk == 1)
                            {
                                var cROI = PayrollMaster.loanSlab.Where(x => x.AmountOfSlab <= balancePAmt && x.LoanType == colName && x.EffectiveDate <= dateAvailLoan).OrderByDescending(x => x.EffectiveDate).OrderBy(x => x.AmountOfSlab).FirstOrDefault();
                                if (cROI != null)
                                    currentROI = cROI.RateOfInterest == null ? 0 : cROI.RateOfInterest.Value;
                                slab2 = Math.Round((((balancePAmt - slabamt1) * currentROI) / 1200));
                            }
                            else
                            {
                                if (rOIForchk == 2)
                                {
                                    var cROI = PayrollMaster.loanSlab.Where(x => x.AmountOfSlab <= balancePAmt && x.LoanType == colName && x.EffectiveDate <= dateAvailLoan).OrderByDescending(x => x.EffectiveDate).OrderBy(x => x.AmountOfSlab).FirstOrDefault();
                                    if (cROI != null)
                                        currentROI = cROI.RateOfInterest == null ? 0 : cROI.RateOfInterest.Value;

                                    var amtOfSlab = PayrollMaster.loanSlab.Where(x => x.AmountOfSlab <= balancePAmt && x.LoanType == colName && x.EffectiveDate <= dateAvailLoan).OrderByDescending(x => x.EffectiveDate).OrderByDescending(x => x.SlabNo).FirstOrDefault();
                                    if (amtOfSlab != null)
                                        slabamt2 = amtOfSlab.AmountOfSlab == null ? 0 : cROI.AmountOfSlab.Value;

                                    slab2 = Math.Round((((slabamt2 - slabamt1) * currentROI) / 1200));
                                }
                            }

                            if (rOIForchk == 2)
                            {
                                var cROI = PayrollMaster.loanSlab.Where(x => x.AmountOfSlab >= balancePAmt
                                && x.LoanType == colName && x.EffectiveDate <= dateAvailLoan)
                                .OrderByDescending(x => x.EffectiveDate)
                                .OrderByDescending(x => x.SlabNo).FirstOrDefault();

                                if (cROI != null)
                                    currentROI = cROI.RateOfInterest == null ? 0 : cROI.RateOfInterest.Value;

                                slab3 = Math.Round(((balancePAmt * currentROI) / 1200));
                            }
                            currentInterestAmt = slab1 + slab2 + slab3;

                            if (isTDSRebet)
                            {
                                var tDSRebet = PayrollMaster.loanTypes.Where(x => x.LoanType == colName).FirstOrDefault();
                                if (tDSRebet != null)
                                    tDSRebetROI = tDSRebet.TDSRebetROI == null ? 0 : tDSRebet.TDSRebetROI.Value;
                                tDSAmt = Math.Round(((balancePAmt * tDSRebetROI) / 1200));
                            }
                        }
                        else
                        {
                            currentROI = 0;
                            currentInterestAmt = 0;
                        }
                        #endregion tblmstslab

                        if (remainingPInstNo == originalPInstNo)
                        {
                            int x = (DateTime.DaysInMonth(dateAvailLoan.Year, dateAvailLoan.Month) - dateAvailLoan.Day);
                            currentInterestAmt = Math.Round((currentInterestAmt + ((sancAmt * currentROI * x) / 36500)));
                        }

                        if (lastPaidPInstNo != originalPInstNo)
                        {
                            remainingPAmt = balancePAmt - originalPInstAmt;
                            balancePAmt = remainingPAmt;
                            remainingIAmt = balanceIAmt;
                            remainingIAmt = remainingIAmt + currentInterestAmt;
                            balanceIAmt = remainingIAmt;
                            totalBalanceAmt = balancePAmt + balanceIAmt;
                            currentPInstNoPaid = (originalPInstNo - remainingPInstNo) + 1;
                            remainingPInstNo = remainingPInstNo - 1;
                            remainingIInstNo = originalIInstNo;
                            currentPInstAmtPaid = originalPInstAmt;
                            currentIInstNoPaid = 0;
                            currentIInstAmtPaid = 0;

                            if (remainingPAmt == 0 && isInterestPayable)
                            {
                                interestInstAmt = originalIInstNo > 0 ? Math.Round((balanceIAmt / originalIInstNo)) : 0;
                                remainingIInstNo = originalIInstNo;
                            }
                            else
                                interestInstAmt = 0;

                            lastPaidPInstAmt = currentPInstAmtPaid;
                            lastPaidIInstAmt = 0;
                            lastPaidPInstNo = currentPInstNoPaid;
                            lastPaidIInstNo = 0;
                            lastMonthInterest = currentInterestAmt;
                        }

                        if (lastPaidPInstNo == originalPInstNo)
                        {
                            currentPInstAmtPaid = originalPInstAmt;
                            remainingPAmt = 0;
                            balancePAmt = remainingPAmt;
                            remainingIAmt = balanceIAmt;
                            remainingIAmt = remainingIAmt + currentInterestAmt;
                            balanceIAmt = remainingIAmt;
                            totalBalanceAmt = balancePAmt + balanceIAmt;
                            currentPInstNoPaid = (originalPInstNo - remainingPInstNo);
                            remainingIInstNo = originalIInstNo;
                            currentIInstNoPaid = 0;
                            currentIInstAmtPaid = 0;

                            if (remainingPAmt == 0 && isInterestPayable)
                            {
                                interestInstAmt = originalIInstNo > 0 ? Math.Round((balanceIAmt / originalIInstNo)) : 0;
                                remainingIInstNo = originalIInstNo;
                            }
                            else
                                interestInstAmt = 0;
                            lastPaidPInstAmt = currentPInstAmtPaid;
                            lastPaidIInstAmt = 0;
                            lastPaidPInstNo = currentPInstNoPaid;
                            lastPaidIInstNo = 0;
                            lastMonthInterest = currentInterestAmt;
                        }

                        lastPaidInstDeduDt = LastPaidInstDeduDt /*DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0')*/;
                        PayrollMaster.InstNo = lastPaidPInstNo;

                        if (totalBalanceAmt == 0 && lastInstAmt == 0)
                            status = true;
                        else
                            status = false;

                        var dataLPNew = PayrollMaster.LoanPriorityNew;
                        if (PayrollMaster.F_LoanPriorityNew == null)
                            PayrollMaster.F_LoanPriorityNew = dataLPNew.Clone();
                        DataRow dataLPNewRow = PayrollMaster.F_LoanPriorityNew.NewRow();
                        //   dataLPNewRow = dataLPNew.Select("PriorityNo = '" + loanPriorityNo + "'","Order by MstLoanID DESC")[0];
                        dataLPNewRow = dataLPNew.Select("PriorityNo = '" + loanPriorityNo + "' ", " MstLoanID DESC")[0];
                        if (dataLPNewRow != null)
                        {
                            dataLPNewRow["InterestInstAmt"] = interestInstAmt;
                            dataLPNewRow["BalancePAmt"] = balancePAmt;
                            dataLPNewRow["BalanceIAmt"] = balanceIAmt;
                            dataLPNewRow["TotalBalanceAmt"] = balancePAmt + balanceIAmt;
                            dataLPNewRow["RemainingPInstNo"] = remainingPInstNo;
                            dataLPNewRow["RemainingIInstNo"] = remainingIInstNo;
                            dataLPNewRow["LastInstAmt"] = lastInstAmt;
                            dataLPNewRow["LastPaidPInstAmt"] = lastPaidPInstAmt;
                            dataLPNewRow["LastPaidPInstNo"] = lastPaidPInstNo;
                            dataLPNewRow["LastPaidIInstNo"] = lastPaidIInstNo;
                            dataLPNewRow["LastPaidIInstAmt"] = lastPaidIInstAmt;
                            dataLPNewRow["LastMonthInterest"] = lastMonthInterest;
                            dataLPNewRow["LastPaidInstDeduDt"] = lastPaidInstDeduDt;
                            dataLPNewRow["status"] = status;
                            dataLPNewRow["TotalSkippedInst"] = totalSkippedInst;
                            dataLPNewRow["TotalBalanceAmt"] = totalBalanceAmt;
                            dataLPNewRow["LoanTypeId"] = Convert.ToInt32(drNewLoans["LoanTypeId"]);
                            //  dataLPNewRow["RefLoanMstID"] = totalBalanceAmt;
                            DataRow[] foundAuthors_lp = PayrollMaster.F_LoanPriorityNew.Select("PriorityNo = '" + loanPriorityNo + "'");
                            if (foundAuthors_lp.Length != 0)
                            {
                                // do something...
                            }
                            else
                            {
                                PayrollMaster.F_LoanPriorityNew.ImportRow(dataLPNewRow);
                                PayrollMaster.F_LoanPriorityNew.AcceptChanges();
                            }
                        }

                        if (PayrollMaster.F_LoanTransNew == null)
                            PayrollMaster.F_LoanTransNew = PayrollMaster.LoanTransNew.Clone();

                        dr_LTNew = PayrollMaster.F_LoanTransNew.NewRow();

                        dr_LTNew["BranchCode"] = branchCode;
                        dr_LTNew["EmployeeCode"] = employeeCode;
                        dr_LTNew["PriorityNo"] = loanPriorityNo;
                        dr_LTNew["LoanType"] = colName;
                        dr_LTNew["SancAmt"] = sancAmt;
                        dr_LTNew["CurrentROI"] = currentROI;
                        dr_LTNew["CurrentPInstNoPaid"] = lastPaidPInstNo;
                        dr_LTNew["CurrentPInstAmtPaid"] = lastPaidPInstAmt;
                        dr_LTNew["RemainingPAmt"] = remainingPAmt;
                        dr_LTNew["CurrentInterestAmt"] = currentInterestAmt;
                        dr_LTNew["CurrentIInstNoPaid"] = lastPaidIInstNo;
                        dr_LTNew["CurrentIInstAmtPaid"] = lastPaidIInstAmt;
                        dr_LTNew["RemainingIAmt"] = remainingIAmt;
                        dr_LTNew["PeriodOfPayment"] = lastPaidInstDeduDt;
                        dr_LTNew["SkippedInstNo"] = 0;
                        dr_LTNew["TDSRebetAmt"] = 0;
                        dr_LTNew["IsNewLoanAfterDevelop"] = 1;
                        dr_LTNew["SerialNo"] = serialNo;
                        dr_LTNew["LoanTypeId"] = Convert.ToInt32(drNewLoans["LoanTypeId"]);

                        DataRow[] foundAuthors = PayrollMaster.F_LoanTransNew.Select("PriorityNo = '" + loanPriorityNo + "' AND PeriodOfPayment='" + lastPaidInstDeduDt + "'");
                        if (foundAuthors.Length != 0)
                        {
                            // do something...
                        }
                        else
                        {
                            PayrollMaster.F_LoanTransNew.Rows.Add(dr_LTNew);
                            PayrollMaster.F_LoanTransNew.AcceptChanges();
                        }

                        return lastPaidPInstAmt;

                    }

                    if (balancePAmt == 0 && balanceIAmt > 0)
                    {
                        if (originalIInstNo == lastPaidIInstNo)
                        {
                            currentPInstAmtPaid = 0;
                            remainingPAmt = 0;
                            balancePAmt = 0;
                            currentInterestAmt = 0;
                            currentROI = 0;
                            currentIInstAmtPaid = interestInstAmt;
                            remainingIAmt = balanceIAmt - interestInstAmt;
                            balanceIAmt = remainingIAmt;
                            currentPInstNoPaid = 0;
                            remainingPInstNo = 0;
                            currentIInstNoPaid = (originalIInstNo - remainingIInstNo) + 1;
                            remainingIInstNo = remainingIInstNo - 1;
                            totalBalanceAmt = balancePAmt + balanceIAmt;
                            lastPaidPInstAmt = 0;
                            lastPaidIInstAmt = currentIInstAmtPaid;
                            lastPaidPInstNo = 0;
                            lastPaidIInstNo = currentIInstNoPaid;
                            lastMonthInterest = 0;

                        }

                        if (originalIInstNo == lastPaidIInstNo)
                        {
                            currentPInstAmtPaid = 0;
                            remainingPAmt = 0;
                            balancePAmt = 0;
                            currentInterestAmt = 0;
                            currentROI = 0;
                            currentIInstAmtPaid = balanceIAmt;
                            remainingIAmt = 0;
                            balanceIAmt = 0;
                            currentPInstNoPaid = 0;
                            remainingPInstNo = 0;
                            currentIInstNoPaid = (originalIInstNo - remainingIInstNo) + 1;
                            remainingIInstNo = remainingIInstNo - 1;
                            totalBalanceAmt = balancePAmt + balanceIAmt;
                            lastPaidPInstAmt = 0;
                            lastPaidIInstAmt = currentIInstAmtPaid;
                            lastPaidPInstNo = 0;
                            lastPaidIInstNo = currentIInstNoPaid;
                            lastMonthInterest = 0;
                        }
                        lastPaidInstDeduDt = LastPaidInstDeduDt;
                        PayrollMaster.InstNo = lastPaidPInstNo;

                        if (totalBalanceAmt > 0)
                            status = false;
                        else
                            status = true;

                        //==== start ==========
                        var dataLPNew = PayrollMaster.LoanPriorityNew;
                        if (PayrollMaster.F_LoanPriorityNew == null)
                            PayrollMaster.F_LoanPriorityNew = dataLPNew.Clone();
                        DataRow dataLPNewRow = PayrollMaster.F_LoanPriorityNew.NewRow();
                        dataLPNewRow = dataLPNew.Select("PriorityNo = '" + loanPriorityNo + "'")[0];
                        if (dataLPNewRow != null)
                        {
                            dataLPNewRow["InterestInstAmt"] = interestInstAmt;
                            dataLPNewRow["BalancePAmt"] = balancePAmt;
                            dataLPNewRow["BalanceIAmt"] = balanceIAmt;
                            dataLPNewRow["TotalBalanceAmt"] = balancePAmt + balanceIAmt;
                            dataLPNewRow["RemainingPInstNo"] = remainingPInstNo;
                            dataLPNewRow["RemainingIInstNo"] = remainingIInstNo;
                            dataLPNewRow["LastInstAmt"] = lastInstAmt;
                            dataLPNewRow["LastPaidPInstAmt"] = lastPaidPInstAmt;
                            dataLPNewRow["LastPaidPInstNo"] = lastPaidPInstNo;
                            dataLPNewRow["LastPaidIInstNo"] = lastPaidIInstNo;
                            dataLPNewRow["LastPaidIInstAmt"] = lastPaidIInstAmt;
                            dataLPNewRow["LastMonthInterest"] = lastMonthInterest;
                            dataLPNewRow["LastPaidInstDeduDt"] = LastPaidInstDeduDt;
                            dataLPNewRow["status"] = status;
                            dataLPNewRow["TotalSkippedInst"] = totalSkippedInst;
                            dataLPNewRow["LoanTypeId"] = Convert.ToInt32(drNewLoans["LoanTypeId"]);

                            DataRow[] foundAuthors_lp_1 = PayrollMaster.F_LoanPriorityNew.Select("PriorityNo = '" + loanPriorityNo + "'");
                            if (foundAuthors_lp_1.Length != 0)
                            {
                                // do something...
                            }
                            else
                            {
                                PayrollMaster.F_LoanPriorityNew.ImportRow(dataLPNewRow);
                                PayrollMaster.F_LoanPriorityNew.AcceptChanges();
                            }
                        }
                        //fmSalary.newLoanPriority.Rows.Add(dataLPNewRow);

                        if (PayrollMaster.F_LoanTransNew == null)
                            PayrollMaster.F_LoanTransNew = PayrollMaster.LoanTransNew.Clone();

                        dr_LTNew = PayrollMaster.F_LoanTransNew.NewRow();
                        //dr_LTNew = PayrollMaster.LoanTransNew.NewRow();

                        dr_LTNew["BranchCode"] = branchCode;
                        dr_LTNew["EmployeeCode"] = employeeCode;
                        dr_LTNew["PriorityNo"] = loanPriorityNo;
                        dr_LTNew["LoanType"] = colName;
                        dr_LTNew["SancAmt"] = sancAmt;
                        dr_LTNew["CurrentROI"] = currentROI;
                        dr_LTNew["CurrentPInstNoPaid"] = lastPaidPInstNo;
                        dr_LTNew["CurrentPInstAmtPaid"] = lastPaidPInstAmt;
                        dr_LTNew["RemainingPAmt"] = remainingPAmt;
                        dr_LTNew["CurrentInterestAmt"] = currentInterestAmt;
                        dr_LTNew["CurrentIInstNoPaid"] = lastPaidIInstNo;
                        dr_LTNew["CurrentIInstAmtPaid"] = lastPaidIInstAmt;
                        dr_LTNew["RemainingIAmt"] = remainingIAmt;
                        dr_LTNew["PeriodOfPayment"] = LastPaidInstDeduDt;
                        dr_LTNew["SkippedInstNo"] = 0;
                        dr_LTNew["TDSRebetAmt"] = 0;
                        dr_LTNew["IsNewLoanAfterDevelop"] = 1;
                        dr_LTNew["SerialNo"] = serialNo;

                        dr_LTNew["LoanTypeId"] = Convert.ToInt32(drNewLoans["LoanTypeId"]);

                        DataRow[] foundAuthors_T = PayrollMaster.F_LoanTransNew.Select("PriorityNo = '" + loanPriorityNo + "' AND PeriodOfPayment='" + LastPaidInstDeduDt + "'");
                        if (foundAuthors_T.Length != 0)
                        {
                            // do something...
                        }
                        else
                        {
                            PayrollMaster.F_LoanTransNew.Rows.Add(dr_LTNew);
                            PayrollMaster.F_LoanTransNew.AcceptChanges();
                        }
                        // ======= end ============
                    }

                    if (balancePAmt == 0 && balanceIAmt > 0)
                        return lastPaidIInstAmt;
                    else
                        return lastPaidPInstAmt;

                }
                else if (loanMode.ToLower() == "EMI".ToLower())
                {
                    decimal balIAmt = ((drNewLoans["OriginalPInstNo"].ToString() == "" || drNewLoans["OriginalPInstNo"] == null) ? 0 : Convert.ToDecimal(drNewLoans["OriginalPInstNo"]) - (drNewLoans["LastPaidPInstNo"].ToString() == "" || drNewLoans["LastPaidPInstNo"] == null ? 0 : Convert.ToDecimal(drNewLoans["LastPaidPInstNo"]))
                        ) * (drNewLoans["InterestInstAmt"] == null || drNewLoans["InterestInstAmt"].ToString() == "" ? 0 : Convert.ToDecimal(drNewLoans["InterestInstAmt"]));

                    serialNo = (drNewLoans["SerialNo"] == null || drNewLoans["SerialNo"].ToString() == "") ? 0 : Convert.ToInt32(drNewLoans["SerialNo"]);

                    sancAmt = (drNewLoans["SancAmt"] == null || drNewLoans["SancAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["SancAmt"]);

                    originalPInstNo = (drNewLoans["OriginalPInstNo"] == null || drNewLoans["OriginalPInstNo"].ToString() == "") ? 0 : Convert.ToInt32(drNewLoans["OriginalPInstNo"]);
                    originalIInstNo = (drNewLoans["OriginalIInstNo"] == null || drNewLoans["OriginalIInstNo"].ToString() == "") ? 0 : Convert.ToInt32(drNewLoans["OriginalIInstNo"]);
                    originalPInstAmt = (drNewLoans["OriginalPInstAmt"] == null || drNewLoans["OriginalPInstAmt"].ToString() == "") ? 0 :
                                      Convert.ToDecimal(drNewLoans["OriginalPInstAmt"]);
                    interestInstAmt = (drNewLoans["InterestInstAmt"] == null || drNewLoans["InterestInstAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["InterestInstAmt"]);
                    balancePAmt = (drNewLoans["BalancePAmt"] == null || drNewLoans["BalancePAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["BalancePAmt"]);
                    balanceIAmt = balIAmt;
                    totalBalanceAmt = (drNewLoans["TotalBalanceAmt"] == null || drNewLoans["TotalBalanceAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["TotalBalanceAmt"]);
                    remainingPInstNo = (drNewLoans["RemainingPInstNo"] == null || drNewLoans["RemainingPInstNo"].ToString() == "") ? 0 : Convert.ToInt32(drNewLoans["RemainingPInstNo"]);
                    remainingIInstNo = (drNewLoans["RemainingIInstNo"] == null || drNewLoans["RemainingIInstNo"].ToString() == "") ? 0 : Convert.ToInt32(drNewLoans["RemainingIInstNo"]);
                    lastPaidPInstAmt = (drNewLoans["LastPaidPInstAmt"] == null || drNewLoans["LastPaidPInstAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["LastPaidPInstAmt"]);
                    lastPaidIInstAmt = (drNewLoans["LastPaidIInstAmt"] == null || drNewLoans["LastPaidIInstAmt"].ToString() == "") ? 0 :
                        Convert.ToDecimal(drNewLoans["LastPaidIInstAmt"]);

                    lastPaidPInstNo = (drNewLoans["LastPaidPInstNo"] == null || drNewLoans["LastPaidPInstNo"].ToString() == "")
                        ? 0 : Convert.ToInt32(drNewLoans["LastPaidPInstNo"]);

                    closingBal = (drNewLoans["BalancePAmt"] == null || drNewLoans["BalancePAmt"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["BalancePAmt"]);
                    lastPaidIInstNo = (drNewLoans["LastPaidIInstNo"] == null || drNewLoans["LastPaidIInstNo"].ToString() == "") ? 0 : Convert.ToInt32(drNewLoans["LastPaidIInstNo"]);
                    lastPaidInstDeduDt = drNewLoans["LastPaidInstDeduDt"] == null ? "" : Convert.ToString(drNewLoans["LastPaidInstDeduDt"]);
                    lastMonthInterest = (drNewLoans["LastMonthInterest"] == null || drNewLoans["LastMonthInterest"].ToString() == "") ? 0 : Convert.ToDecimal(drNewLoans["LastMonthInterest"]);
                    lastInstAmt = (drNewLoans["LastInstAmt"] == null || drNewLoans["LastInstAmt"].ToString() == "")
                        ? 0 : Convert.ToDecimal(drNewLoans["LastInstAmt"]);
                    loanPriorityNo = drNewLoans["PriorityNo"] == null ? "" : Convert.ToString(drNewLoans["PriorityNo"]);
                    isInterestPayable = (drNewLoans["IsInterestPayable"] == null || drNewLoans["IsInterestPayable"].ToString() == "") ? false : Convert.ToBoolean(drNewLoans["IsInterestPayable"]);
                    isFloatingRate = (drNewLoans["IsFloatingRate"] == null || drNewLoans["IsFloatingRate"].ToString() == "") ? false : Convert.ToBoolean(drNewLoans["IsFloatingRate"]);
                    dateAvailLoan = Convert.ToDateTime(drNewLoans["DateAvailLoan"]);
                    dateEffective = (drNewLoans["EffDate"] == null || drNewLoans["EffDate"].ToString() == "") ? (DateTime?)null : Convert.ToDateTime(drNewLoans["EffDate"]);

                    DataRow[] dr_Array = PayrollMaster.LoanTransNew.Select("SerialNo = '" + serialNo + "' and PriorityNo = '" + loanPriorityNo + "'and SkippedInstNo > 0");
                    totalSkippedInst = dr_Array != null ? dr_Array.Count() : 0;
                    currentROI = PayrollMaster.PFLoanRate ?? 0;   /*genericRepo.Get<DTOModel.tblmstbankrate>(x => !x.IsDeleted).OrderBy(x => x.EffectiveDate).FirstOrDefault().PFLoanRate.Value*/;

                    lIA = lastInstAmt;

                    if (dateEffective >= DateTime.Now.AddDays(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day))
                        return result;

                    if (remainingPInstNo == originalPInstNo)
                    {
                        int x = (DateTime.DaysInMonth(dateAvailLoan.Year, dateAvailLoan.Month) - dateAvailLoan.Day) + 1;
                        currentInterestAmt = (((sancAmt * currentROI * x) / (100 * 365)));
                        lastInstAmt = 0;
                        var cal = Convert.ToDouble(originalPInstNo) * Convert.ToDouble(currentROI) * 0.01;
                        intcalDummy = Math.Round(((sancAmt + originalPInstAmt) * Convert.ToDecimal(cal)) / 24);
                        intcalDummy = intcalDummy + currentInterestAmt;
                        interestInstAmt = Math.Round(intcalDummy / originalPInstNo);
                        balanceIAmt = 24 * interestInstAmt;

                        installmentAmount = originalPInstNo > 0 ? Math.Round(Math.Round(intcalDummy / originalPInstNo) + originalPInstAmt) : Math.Round(originalPInstAmt);
                        lastMonthInterest = 0;
                    }
                    else
                    {
                        currentInterestAmt = 0;
                        installmentAmount = originalPInstNo > 0 ? Math.Round(Math.Round((sancAmt / originalPInstNo)) + interestInstAmt) : Math.Round(interestInstAmt);
                        lastMonthInterest = currentInterestAmt;
                    }

                    currentInterestAmt = interestInstAmt;
                    remainingPAmt = balancePAmt - originalPInstAmt;
                    balancePAmt = remainingPAmt;
                    remainingIAmt = balanceIAmt - interestInstAmt;
                    balanceIAmt = remainingIAmt;
                    totalBalanceAmt = balancePAmt + balanceIAmt;
                    currentPInstAmtPaid = originalPInstAmt;
                    currentPInstNoPaid = (originalPInstNo - remainingPInstNo) + 1;
                    remainingPInstNo = remainingPInstNo - 1;
                    currentIInstNoPaid = currentPInstNoPaid;
                    remainingIInstNo = remainingPInstNo;
                    currentIInstAmtPaid = interestInstAmt;
                    lastPaidPInstAmt = currentPInstAmtPaid;
                    lastPaidIInstAmt = interestInstAmt;
                    lastPaidPInstNo = currentPInstNoPaid;
                    lastPaidIInstNo = currentIInstNoPaid;

                    lastPaidInstDeduDt = LastPaidInstDeduDt /*DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0')*/;
                    PayrollMaster.InstNo = lastPaidPInstNo;

                    if (lastPaidPInstNo == originalPInstNo)
                    {
                        installmentAmount = originalPInstAmt + interestInstAmt + lIA;
                        remainingPAmt = 0;
                        remainingIAmt = 0;
                        balancePAmt = remainingPAmt;
                        balanceIAmt = 0;
                        totalBalanceAmt = balancePAmt + balanceIAmt;
                        lastPaidPInstAmt = originalPInstAmt;
                        lastPaidIInstAmt = interestInstAmt;
                        currentPInstAmtPaid = lastPaidPInstAmt;
                        currentIInstAmtPaid = interestInstAmt;
                        lastInstAmt = 0;
                    }
                    if (lastPaidPInstNo == 24)
                        status = true;
                    else
                        status = false;

                    var dataLPNew = PayrollMaster.LoanPriorityNew;
                    if (PayrollMaster.F_LoanPriorityNew == null)
                        PayrollMaster.F_LoanPriorityNew = dataLPNew.Clone();
                    DataRow dataLPNewRow = PayrollMaster.F_LoanPriorityNew.NewRow();
                    dataLPNewRow = dataLPNew.Select("PriorityNo = '" + loanPriorityNo + "'", "MstLoanID DESC")[0];
                    if (dataLPNewRow != null)
                    {
                        dataLPNewRow["InterestInstAmt"] = interestInstAmt;
                        dataLPNewRow["BalancePAmt"] = balancePAmt;
                        dataLPNewRow["BalanceIAmt"] = balanceIAmt;
                        dataLPNewRow["RemainingPInstNo"] = remainingPInstNo;
                        dataLPNewRow["RemainingIInstNo"] = remainingIInstNo;
                        dataLPNewRow["LastPAmt"] = lastInstAmt;
                        dataLPNewRow["LastInstAmt"] =
                            dataLPNewRow["LastInstAmt"].ToString() != "" ?
                            Convert.ToDecimal(dataLPNewRow["LastInstAmt"]) : 0 + lastInstAmt;
                        dataLPNewRow["LastPaidPInstAmt"] = lastPaidPInstAmt;
                        dataLPNewRow["LastPaidPInstNo"] = lastPaidPInstNo;
                        dataLPNewRow["LastPaidIInstNo"] = lastPaidIInstNo;
                        dataLPNewRow["LastPaidIInstAmt"] = lastPaidIInstAmt;
                        dataLPNewRow["LastMonthInterest"] = lastMonthInterest;
                        dataLPNewRow["LastPaidInstDeduDt"] = lastPaidInstDeduDt;
                        dataLPNewRow["status"] = status;
                        dataLPNewRow["TotalSkippedInst"] = totalSkippedInst;
                        dataLPNewRow["LoanTypeId"] = Convert.ToInt32(drNewLoans["LoanTypeId"]);

                        PayrollMaster.F_LoanPriorityNew.ImportRow(dataLPNewRow);
                        PayrollMaster.F_LoanPriorityNew.AcceptChanges();
                    }
                    //fmSalary.newLoanPriority.Rows.Add(dataLPNewRow);

                    if (PayrollMaster.F_LoanTransNew == null)
                        PayrollMaster.F_LoanTransNew = PayrollMaster.LoanTransNew.Clone();

                    dr_LTNew = PayrollMaster.F_LoanTransNew.NewRow();
                    //dr_LTNew = PayrollMaster.LoanTransNew.NewRow();

                    dr_LTNew["BranchCode"] = branchCode;
                    dr_LTNew["EmployeeCode"] = employeeCode;
                    dr_LTNew["PriorityNo"] = loanPriorityNo;
                    dr_LTNew["LoanType"] = colName;
                    dr_LTNew["SancAmt"] = sancAmt;
                    dr_LTNew["CurrentROI"] = currentROI;
                    dr_LTNew["CurrentPInstNoPaid"] = lastPaidPInstNo;
                    dr_LTNew["CurrentPInstAmtPaid"] = lastPaidPInstAmt;
                    dr_LTNew["RemainingPAmt"] = remainingPAmt;
                    dr_LTNew["CurrentInterestAmt"] = currentInterestAmt;
                    dr_LTNew["CurrentIInstNoPaid"] = lastPaidIInstNo;
                    dr_LTNew["CurrentIInstAmtPaid"] = lastPaidIInstAmt;
                    dr_LTNew["RemainingIAmt"] = remainingIAmt;
                    dr_LTNew["PeriodOfPayment"] = lastPaidInstDeduDt;
                    dr_LTNew["SkippedInstNo"] = 0;
                    dr_LTNew["TDSRebetAmt"] = tDSAmt;
                    dr_LTNew["IsNewLoanAfterDevelop"] = 1;
                    dr_LTNew["SerialNo"] = serialNo;
                    dr_LTNew["ClosingBalance"] = closingBal;
                    dr_LTNew["LoanTypeId"] = Convert.ToInt32(drNewLoans["LoanTypeId"]);

                    PayrollMaster.F_LoanTransNew.Rows.Add(dr_LTNew);
                    PayrollMaster.F_LoanTransNew.AcceptChanges();

                    return installmentAmount;
                }
                else
                    return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                log.Error($"CalculateNewLoan-Error- EmployeeCode: {employeeCode}, ColumnName:{colName}");
                throw ex;
            }
        }

        private decimal CalculateOldLoan(string branchCode, string employeeCode, string colName, bool chkRegenerate, DataRow drblMstLoanOld, string LastPaidInstDeduDt)
        {
            log.Info($"SalaryGenerator/CalculateOldLoan/branchCode:{branchCode},employeeCode:{employeeCode},colName:{colName}");

            int RemainingPInstNo = 0, RemainingIInstNo = 0, LastPaidPInstNo = 0, status = 0,
                LastPaidIInstNo = 0, lastRemainingPInstNo = 0, lastRemainingIInstNo = 0;
            decimal LastPaidPInstAmt = 0, LastPaidIInstAmt = 0;
            string loanPriorityNo = string.Empty;  //  LastPaidInstDeduDt = string.Empty;

            DataRow Dr_LPold = null, Dr_LTold = null;

            if (drblMstLoanOld["RemainingPInstNo"] != null && Convert.ToInt32(drblMstLoanOld["RemainingPInstNo"]) > 0)
            {
                lastRemainingPInstNo = Convert.ToInt32(drblMstLoanOld["RemainingPInstNo"]) - 1;
                RemainingPInstNo = Convert.ToInt32(drblMstLoanOld["RemainingPInstNo"]) - 1;
                LastPaidPInstAmt = Convert.ToDecimal(drblMstLoanOld["LastPaidPInstAmt"]) - 1;
                LastPaidPInstNo = Convert.ToInt32(drblMstLoanOld["LastPaidPInstNo"]) + 1;
                loanPriorityNo = drblMstLoanOld["PriorityNo"] != null ? Convert.ToString(drblMstLoanOld["PriorityNo"]) : string.Empty;
                // LastPaidInstDeduDt = LastPaidInstDeduDt;

                Dr_LTold = PayrollMaster.LoanTransOld.NewRow();

                if (RemainingPInstNo > 0)
                    status = 0;
                else
                    status = 1;

                if (lastRemainingPInstNo > 0)
                {
                    PayrollMaster.InstNo = LastPaidPInstNo;
                    DataRow[] Dr_LPoldArr = PayrollMaster.LoanPriorityOld.Select("PriorityNo = '" + loanPriorityNo + "'");
                    if (Dr_LPoldArr.Count() > 0)
                        Dr_LPold = Dr_LPoldArr[0];

                    Dr_LPold["RemainingPInstNo"] = RemainingPInstNo;
                    Dr_LPold["LastPaidPInstAmt"] = LastPaidPInstAmt;
                    Dr_LPold["LastPaidPInstNo"] = LastPaidPInstNo;
                    Dr_LPold["LastPaidInstDeduDt"] = LastPaidInstDeduDt;
                    Dr_LPold["Status"] = status;

                    PayrollMaster.F_LoanPriorityOld.ImportRow(Dr_LPold);
                    PayrollMaster.F_LoanPriorityOld.AcceptChanges();

                    //======= Add  a datarow in  LoanTranOld ================

                    if (PayrollMaster.F_LoanTransOld == null)
                        PayrollMaster.F_LoanTransOld = PayrollMaster.F_LoanTransOld.Clone();

                    Dr_LTold = PayrollMaster.F_LoanTransOld.NewRow();

                    Dr_LTold["BranchCode"] = branchCode;
                    Dr_LTold["EmployeeCode"] = employeeCode;
                    Dr_LTold["PriorityNo"] = loanPriorityNo;
                    Dr_LTold["LoanType"] = colName;
                    Dr_LTold["SancAmt"] = 0.0;
                    Dr_LTold["CurrentROI"] = 0.0;
                    Dr_LTold["CurrentPInstNoPaid"] = LastPaidPInstNo;
                    Dr_LTold["CurrentPInstAmtPaid"] = LastPaidPInstAmt;
                    Dr_LTold["RemainingPAmt"] = 0.0;
                    Dr_LTold["CurrentInterestAmt"] = 0.0;
                    Dr_LTold["CurrentIInstNoPaid"] = 0.0;
                    Dr_LTold["CurrentIInstAmtPaid"] = 0.0;
                    Dr_LTold["RemainingIAmt"] = 0.0;
                    Dr_LTold["PeriodOfPayment"] = LastPaidInstDeduDt;
                    Dr_LTold["SkippedInstNo"] = 0;
                    Dr_LTold["TDSRebetAmt"] = 0;
                    Dr_LTold["IsNewLoanAfterDevelop"] = 0;
                    Dr_LTold["SerialNo"] = 0;

                    PayrollMaster.F_LoanTransOld.Rows.Add(Dr_LTold);
                    PayrollMaster.F_LoanTransOld.AcceptChanges();

                    //======= End ===========================================
                }
            }
            else if (Convert.ToInt32(drblMstLoanOld["RemainingPInstNo"]) < 1 && Convert.ToInt32(drblMstLoanOld["RemainingIInstNo"]) > 0)
            {
                Dr_LTold = PayrollMaster.LoanTransOld.NewRow();
                lastRemainingIInstNo = Convert.ToInt32(drblMstLoanOld["RemainingPInstNo"]);
                RemainingIInstNo = Convert.ToInt32(drblMstLoanOld["RemainingIInstNo"]) - 1;
                LastPaidIInstAmt = Convert.ToDecimal(drblMstLoanOld["LastPaidIInstAmt"]);
                LastPaidIInstNo = Convert.ToInt32(drblMstLoanOld["LastPaidIInstNo"]) + 1;

                //   LastPaidInstDeduDt = LastPaidInstDeduDt /*DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00")*/;

                loanPriorityNo = drblMstLoanOld["PriorityNo"] != null ? Convert.ToString(drblMstLoanOld["PriorityNo"]) : string.Empty;

                if (RemainingIInstNo > 0)
                    status = 0;
                else
                    status = 1;

                if (lastRemainingIInstNo > 0)
                {
                    PayrollMaster.InstNo = LastPaidIInstNo;
                    DataRow[] Dr_LPoldArr = PayrollMaster.LoanPriorityOld.Select("PriorityNo = '" + loanPriorityNo + "'");
                    Dr_LPold = Dr_LPoldArr[0];
                    Dr_LPold["RemainingIInstNo"] = RemainingIInstNo;
                    Dr_LPold["LastPaidIInstAmt"] = LastPaidIInstAmt;
                    Dr_LPold["LastPaidIInstNo"] = LastPaidIInstNo;
                    Dr_LPold["LastPaidInstDeduDt"] = LastPaidInstDeduDt;
                    Dr_LPold["status"] = status;

                    PayrollMaster.F_LoanPriorityOld.ImportRow(Dr_LPold);
                    PayrollMaster.F_LoanPriorityOld.AcceptChanges();

                    //======= Add  a datarow in  LoanTranOld ==========

                    if (PayrollMaster.F_LoanTransOld == null)
                        PayrollMaster.F_LoanTransOld = PayrollMaster.F_LoanTransOld.Clone();

                    Dr_LTold = PayrollMaster.F_LoanTransOld.NewRow();


                    Dr_LTold["BranchCode"] = branchCode;
                    Dr_LTold["EmployeeCode"] = employeeCode;
                    Dr_LTold["PriorityNo"] = loanPriorityNo;
                    Dr_LTold["LoanType"] = colName;
                    Dr_LTold["SancAmt"] = 0.0;
                    Dr_LTold["CurrentROI"] = 0.0;
                    Dr_LTold["CurrentPInstNoPaid"] = LastPaidPInstNo;
                    Dr_LTold["CurrentPInstAmtPaid"] = LastPaidPInstAmt;
                    Dr_LTold["RemainingPAmt"] = 0.0;
                    Dr_LTold["CurrentInterestAmt"] = 0.0;
                    Dr_LTold["CurrentIInstNoPaid"] = 0.0;
                    Dr_LTold["CurrentIInstAmtPaid"] = 0.0;
                    Dr_LTold["RemainingIAmt"] = 0.0;
                    Dr_LTold["PeriodOfPayment"] = LastPaidInstDeduDt;
                    Dr_LTold["SkippedInstNo"] = 0;
                    Dr_LTold["TDSRebetAmt"] = 0;
                    Dr_LTold["IsNewLoanAfterDevelop"] = 0;
                    Dr_LTold["SerialNo"] = 0;

                    PayrollMaster.F_LoanTransOld.Rows.Add(Dr_LTold);
                    PayrollMaster.F_LoanTransOld.AcceptChanges();

                    //======  End ====================================
                }

            }
            return LastPaidIInstAmt;
        }

        private decimal CalculateFromulaValue(SalaryHead salHeads, EmployeeSalary empSalary, FinalMonthlySalary fmSalary)
        {
            log.Info($"SalaryGenerator/CalculateFromulaValue/EmployeeCode:{empSalary.EmployeeCode}");
            try
            {
                char sep = '|';
                decimal LookUpVal = 0;

                //===  Added new condition to check IsPensionDeducted ====  SG (15-may-2020) ====
                if (salHeads.FieldName == "C_Pension")
                {
                    bool IsPensionDeducted = false;
                    var modelProperty = GetModelPropertyValue<EmployeeSalary>(empSalary, "IsPensionDeducted");
                    if (modelProperty != null)
                        IsPensionDeducted = Convert.ToBoolean(modelProperty);

                    if (!IsPensionDeducted)
                        return LookUpVal;
                }
                ///=====  End =============  

                if (salHeads.LookUpHeadName != null)
                {
                    if (!salHeads.LocationDependent)
                    {
                        LookUpVal = Convert.ToDecimal(EvaluateFormula(salHeads.LookUpHeadName, sep, fmSalary, salHeads.FieldName));
                        //var dt = salHeads.Where(x => x.FieldName == salHeads[j].FieldName && ((x.LowerRange ?? 0) <= (decimal?)LookUpVal)).OrderByDescending(x => x.LowerRange).FirstOrDefault();
                        if (salHeads.LowerRange.HasValue && salHeads.UpperRange.HasValue && salHeads.FieldName != "D_VPF")
                        {
                            LookUpVal = (salHeads.LowerRange ?? 0) >= LookUpVal ? (salHeads.LowerRange ?? 0) : LookUpVal;

                            LookUpVal = (salHeads.UpperRange ?? 0) <= LookUpVal ? (salHeads.UpperRange ?? 0) : LookUpVal;
                        }
                    }
                    else
                    {
                        var bSalaryHead = PayrollMaster.branchSalaryHeadRule.Where(x => x.FieldName == salHeads.FieldName
                        && x.BranchCode == fmSalary.BranchCode && x.FormulaColumn).FirstOrDefault();
                        if (bSalaryHead != null)
                        {
                            LookUpVal = Convert.ToDecimal(EvaluateFormula(bSalaryHead.LookUpHeadName, sep, fmSalary, salHeads.FieldName));
                            if (bSalaryHead.LowerRange.HasValue && bSalaryHead.UpperRange.HasValue && salHeads.FieldName != "D_VPF")
                            {
                                LookUpVal = (bSalaryHead.LowerRange ?? 0) <= LookUpVal ? (bSalaryHead.LowerRange ?? 0) : LookUpVal;
                                LookUpVal = (bSalaryHead.UpperRange ?? 0) <= LookUpVal ? (bSalaryHead.UpperRange ?? 0) : LookUpVal;
                            }
                        }
                    }
                }

                if (salHeads.CheckHeadInEmpSalTable)
                {
                    decimal cellValue = 0;
                    var modelProperty = GetModelPropertyValue<EmployeeSalary>(empSalary, salHeads.FieldName);

                    if (modelProperty != null)
                        cellValue = Convert.ToDecimal(modelProperty);

                    if (cellValue == 0)
                    {
                        LookUpVal = 0;
                    }
                }
                return LookUpVal;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        private double EvaluateFormula(string strFormula, char seperator, FinalMonthlySalary fmSalary, string fieldName = "")
        {
            log.Info($"SalaryGenerator/EvaluateFormula");
            try
            {
                double evaluateFormula = 0.0;
                string[] strArr = new string[0];
                double TempFormula = 0.0;
                string TempOperator = string.Empty;
                if (strFormula.Length != 0)
                {
                    strFormula = strFormula.Substring(0, (strFormula.Length - 2));
                    strArr = strFormula.Substring(strFormula.IndexOf("(") + 3).Split(seperator);

                }

                for (int i = 0; i < strArr.Length; i++)
                {
                    strArr[i] = strArr[i].Trim();
                    if (strArr[i].Trim() == "+" || strArr[i].Trim() == "-")
                        TempOperator = strArr[i].Trim();
                    if (strArr[i].Trim() != "+" && strArr[i].Trim() != "-")
                    {
                        if (TempOperator == "-")
                        {

                            TempFormula = TempFormula - ReturnHeadValue(strArr[i], fmSalary);
                        }
                        else
                        {
                            if (fieldName == "C_TotDedu" || fieldName == "C_TotEarn")
                            {
                                strArr[i] = strArr[i] + "_A";
                            }
                            if (fmSalary.IsSupensionPeriodExists && fieldName.Equals("E_02", StringComparison.OrdinalIgnoreCase) && strArr[i].Equals("E_Basic", StringComparison.OrdinalIgnoreCase))
                                TempFormula = TempFormula + ReturnHeadValue("E_Basic_D", fmSalary);
                            else
                                TempFormula = TempFormula + ReturnHeadValue(strArr[i], fmSalary);

                        }
                    }
                    else
                        TempOperator = strArr[i] == "-" ? "-" : "+";
                }
                evaluateFormula = Convert.ToDouble(strFormula.Substring(0, (strFormula.IndexOf("%") - 1))) * 0.01 * TempFormula;
                return evaluateFormula;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        private double ReturnHeadValue(string headName, FinalMonthlySalary fmSalary)
        {
            log.Info($"SalaryGenerator/ReturnHeadValue/headName:{headName}");
            try
            {
                double headValue = 0.0;
                if (headName != null)
                    headValue = Convert.ToDouble(GetModelPropertyValue<FinalMonthlySalary>(fmSalary, headName) == null ? 0.0 : GetModelPropertyValue<FinalMonthlySalary>(fmSalary, headName));



                return headValue;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        private decimal ConditionalHead(string headName, decimal calculatedValue, EmployeeSalary dRcurrent, bool dRF,
            int daysInMonth, bool chkMedical, bool chkBonus, bool chkExgratia, decimal bonusRate, int exgratiaDays, decimal lWP)
        {
            log.Info($"SalaryGenerator/ConditionalHead/headName:{headName}");
            decimal value = 0;
            try
            {
                if (headName == "E_FDA")
                {
                    decimal result = 0;
                    var currentRowBasic = Convert.ToDecimal(GetModelPropertyValue<EmployeeSalary>(dRcurrent, "E_Basic"));
                    var fdaSlab = PayrollMaster.fdaSlab.Where(x => x.upperlimit >= currentRowBasic).FirstOrDefault();
                    if (fdaSlab != null)
                        result = fdaSlab.val ?? 0;
                    value = ((result * currentRowBasic) / 100);
                }
                //else if (headName == "E_02")
                //{
                //    var currentRowHRA = Convert.ToBoolean(GetModelPropertyValue<EmployeeSalary>(dRcurrent, "HRA"));
                //    if (currentRowHRA)
                //        value = calculatedValue;
                //    else
                //        value = 0;
                //}
                else if (headName == "E_05")
                {
                    bool cca = false;
                    decimal result = 0;
                    string grade = "";
                    string brncode = string.Empty;
                    decimal calculateSum = 0;
                    cca = Convert.ToBoolean(GetModelPropertyValue<EmployeeSalary>(dRcurrent, "CCA"));
                    if (cca)
                    {
                        var currentRowBasic = Convert.ToDecimal(GetModelPropertyValue<EmployeeSalary>(dRcurrent, "E_Basic"));
                        var currentRowSP = Convert.ToDecimal(GetModelPropertyValue<EmployeeSalary>(dRcurrent, "E_SP"));
                        calculateSum = currentRowBasic + currentRowSP;
                        brncode = Convert.ToString(GetModelPropertyValue<EmployeeSalary>(dRcurrent, "BranchCode"));
                        var branch = PayrollMaster.branchList.Where(x => x.BranchCode == brncode).FirstOrDefault();
                        if (branch != null)
                            grade = branch.GradeName;
                        if (grade == "")
                            value = 0;
                        else
                        {
                            grade = "AmtCityGrade" + grade;
                            var ccaSlabs = PayrollMaster.CCASlabs.Where(x => x.UpperLimit >= calculateSum).FirstOrDefault();
                            if (ccaSlabs != null)
                                result = Convert.ToDecimal(GetModelPropertyValue<CCA>(ccaSlabs, grade));
                            value = result;
                        }

                    }
                    else
                        value = 0;
                }
                else if (headName == "E_08")
                {
                    decimal result = 0;
                    string brncode = string.Empty;
                    decimal calculateSum = 0;

                    var currentRowBasic = Convert.ToDecimal(GetModelPropertyValue<EmployeeSalary>(dRcurrent, "E_Basic"));
                    var currentRowSP = Convert.ToDecimal(GetModelPropertyValue<EmployeeSalary>(dRcurrent, "E_SP"));
                    calculateSum = currentRowBasic + currentRowSP;
                    brncode = Convert.ToString(GetModelPropertyValue<EmployeeSalary>(dRcurrent, "BranchCode"));

                    var hillCompensationSlab = PayrollMaster.hillCompensationSlab.Where(x => x.BranchCode == brncode && x.UpperLimit >= calculateSum).FirstOrDefault();
                    if (hillCompensationSlab != null)
                        result = hillCompensationSlab.Amount ?? 0;
                    value = result;
                }
                else if (headName == "E_14")
                {
                    if (chkMedical)
                    {
                        if (lWP == daysInMonth)
                            value = 0;
                        else
                            value = calculatedValue;
                    }
                    else
                        value = 0;
                }
                else if (headName == "E_15")
                {
                    if (chkBonus)
                        value = (calculatedValue * bonusRate) / 100;
                    else
                        value = 0;
                }
                else if (headName == "E_16")
                {
                    if (chkExgratia)
                        value = (calculatedValue / daysInMonth) * exgratiaDays;
                    else
                        value = 0;
                }
                else if (headName == "D_VPF")
                {
                    decimal rate = 0, d_vpf_RatePercentage = 0;

                    d_vpf_RatePercentage = PayrollMaster.salHeads.Where(x => x.FieldName == headName)
                        .FirstOrDefault().UpperRange ?? 0;

                    var currentRowVPF = Convert.ToBoolean(GetModelPropertyValue<EmployeeSalary>(dRcurrent, "IsRateVPF"));
                    if (currentRowVPF)
                    {
                        rate = Convert.ToDecimal(GetModelPropertyValue<EmployeeSalary>(dRcurrent, "VPFValueRA"));
                        if (rate > d_vpf_RatePercentage  /*24*/)
                            rate = d_vpf_RatePercentage  /*24*/;

                        value = (calculatedValue * rate) / 100;
                    }
                    else
                    {
                        var d_VPF = Convert.ToDecimal(GetModelPropertyValue<EmployeeSalary>(dRcurrent, "VPFValueRA"));
                        if (d_VPF > 0)
                        {
                            if (((calculatedValue * d_vpf_RatePercentage /*24*/) / 100) <= d_VPF)
                                value = ((calculatedValue * d_vpf_RatePercentage /*24*/) / 100);
                            else
                                value = d_VPF;
                        }
                    }
                }
                else if (headName == "D_10")
                {
                    if (dRF)
                        value = daysInMonth;
                    else
                        value = 0;
                }
                return value;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        private bool SaveCalculatedSalary(List<FinalMonthlySalary> fmSalary, bool allBranches, bool allBranchExceptHO, int? branchID, int? employeeTypeID)
        {
            log.Info($"SalaryGenerator/SaveCalculatedSalary");
            bool flag = false;
            try
            {
                var pfBalanceData = fmSalary.Select(y => y.empPFOpBalance).ToList();
                var salMonth = fmSalary.First().SalMonth;
                var salYear = fmSalary.First().SalYear;
                int?[] employeeIDs = fmSalary.Select(y => y.EmployeeID).ToArray<int?>();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<FinalMonthlySalary, DTOModel.tblFinalMonthlySalary>()
                    .ForMember(d => d.CreatedBy, o => o.UseValue(1))
                    .ForMember(d => d.RecordType, o => o.UseValue("S"))
                    .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                    .ForMember(d => d.EmployeeTypeID, o => o.UseValue(employeeTypeID ?? 5));
                    //.ForMember(d=>d.E_Basic,o=>o.MapFrom(s=>(s.IsSupensionPeriodExists && s.E_Basic_S.HasValue) ? s.E_Basic_S :s.E_Basic));
                });
                var dtoFinalMonthSalary = Mapper.Map<List<DTOModel.tblFinalMonthlySalary>>(fmSalary);

                if (allBranchExceptHO)
                {
                    if (genericRepo.Exists<DTOModel.tblFinalMonthlySalary>(x => x.BranchCode != "99"
                    && x.SalMonth == salMonth && x.SalYear == salYear
                    && x.RecordType.Equals("S", StringComparison.OrdinalIgnoreCase)))
                    {
                        var dtoPrevFinalMonthlySalary = genericRepo.Get<DTOModel.tblFinalMonthlySalary>(x => x.BranchCode != "99"
                        && x.SalMonth == salMonth && x.SalYear == salYear && x.RecordType.Equals("S", StringComparison.OrdinalIgnoreCase)
                        && x.tblMstEmployee.EmployeeTypeID == employeeTypeID.Value).ToList();

                        genericRepo.DeleteAll<DTOModel.tblFinalMonthlySalary>(dtoPrevFinalMonthlySalary);
                    }
                }
                else if (branchID.HasValue)
                {
                    var branchCode = fmSalary.FirstOrDefault().BranchCode;

                    if (genericRepo.Exists<DTOModel.tblFinalMonthlySalary>(x => x.BranchCode == branchCode
                      && x.SalMonth == salMonth && x.SalYear == salYear
                      && x.RecordType.Equals("S", StringComparison.OrdinalIgnoreCase)))
                    {
                        var dtoPrevFinalMonthlySalary = genericRepo.Get<DTOModel.tblFinalMonthlySalary>(x => x.BranchCode == branchCode
                        && x.SalMonth == salMonth && x.SalYear == salYear && x.RecordType.Equals("S", StringComparison.OrdinalIgnoreCase)
                        && x.tblMstEmployee.EmployeeTypeID == employeeTypeID.Value).ToList();
                        genericRepo.DeleteAll<DTOModel.tblFinalMonthlySalary>(dtoPrevFinalMonthlySalary);
                    }
                }

                else if (allBranches)
                {
                    if (genericRepo.Exists<DTOModel.tblFinalMonthlySalary>(x => x.SalMonth == salMonth
                    && x.SalYear == salYear && x.RecordType.Equals("S", StringComparison.OrdinalIgnoreCase)))
                    {
                        var dtoPrevFinalMonthlySalary = genericRepo.Get<DTOModel.tblFinalMonthlySalary>(x =>
                        x.SalMonth == salMonth && x.SalYear == salYear && x.RecordType.Equals("S", StringComparison.OrdinalIgnoreCase)
                        && x.tblMstEmployee.EmployeeTypeID == employeeTypeID.Value).ToList();
                        genericRepo.DeleteAll<DTOModel.tblFinalMonthlySalary>(dtoPrevFinalMonthlySalary);
                    }
                }

                else if (!allBranchExceptHO && !branchID.HasValue) //==== Case of single employee 
                {
                    var employeeCode = fmSalary.FirstOrDefault().EmployeeCode;
                    if (genericRepo.Exists<DTOModel.tblFinalMonthlySalary>(x =>
                       x.EmployeeCode == employeeCode && x.SalMonth == salMonth && x.SalYear == salYear
                       && x.RecordType.Equals("S", StringComparison.OrdinalIgnoreCase)))
                    {
                        var dtoPrevFinalMonthlySalary = genericRepo.Get<DTOModel.tblFinalMonthlySalary>(x =>
                        x.EmployeeCode == employeeCode && x.RecordType.Equals("S", StringComparison.OrdinalIgnoreCase)
                        && x.SalMonth == salMonth && x.SalYear == salYear).ToList();
                        genericRepo.DeleteAll<DTOModel.tblFinalMonthlySalary>(dtoPrevFinalMonthlySalary);
                    }

                    dtoFinalMonthSalary.FirstOrDefault().EmployeeTypeID = 5;
                }

                flag = genericRepo.AddMultipleEntity<DTOModel.tblFinalMonthlySalary>(dtoFinalMonthSalary);

                if (pfBalanceData.Count > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<EmpPFOpBalance, DTOModel.tblPFOpBalance>()
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(1));
                    });
                    var dtoEmpPFOpBalance = Mapper.Map<List<DTOModel.tblPFOpBalance>>(pfBalanceData);

                    if (allBranchExceptHO)
                    {
                        if (genericRepo.Exists<DTOModel.tblPFOpBalance>(x => x.BRANCHCODE != "99"
                          && x.Salmonth == salMonth.ToString() && x.Salyear == salYear))
                        {
                            var dtoPrevPFOpBalance = genericRepo.Get<DTOModel.tblPFOpBalance>(x => x.BRANCHCODE != "99"
                            && x.Salmonth == salMonth.ToString() && x.Salyear == salYear
                            && employeeIDs.Any(y => y == x.EmployeeID)).ToList();

                            genericRepo.DeleteAll<DTOModel.tblPFOpBalance>(dtoPrevPFOpBalance);

                            // flag = genericRepo.AddMultipleEntity<DTOModel.tblPFOpBalance>(dtoEmpPFOpBalance.Where(y=>y.BRANCHCODE!="99"));
                        }
                    }
                    else if (branchID.HasValue)
                    {
                        var branchCode = fmSalary.FirstOrDefault().BranchCode;
                        if (genericRepo.Exists<DTOModel.tblPFOpBalance>(x => x.BRANCHCODE == branchCode && x.Salmonth == salMonth.ToString() && x.Salyear == salYear))
                        {
                            var dtoPrevPFOpBalance = genericRepo.Get<DTOModel.tblPFOpBalance>(x => x.BRANCHCODE == branchCode && x.Salmonth == salMonth.ToString()
                            && x.Salyear == salYear && employeeIDs.Any(y => y == x.EmployeeID)
                            ).ToList();
                            genericRepo.DeleteAll<DTOModel.tblPFOpBalance>(dtoPrevPFOpBalance);

                            //  flag = genericRepo.AddMultipleEntity<DTOModel.tblPFOpBalance>(dtoEmpPFOpBalance);
                        }
                    }
                    else if (!allBranchExceptHO && !branchID.HasValue && !allBranches) //==== Case of single employee 
                    {
                        var employeeCode = fmSalary.FirstOrDefault().EmployeeCode;
                        if (genericRepo.Exists<DTOModel.tblPFOpBalance>(x => x.Employeecode == employeeCode
                        && x.Salmonth == salMonth.ToString() && x.Salyear == salYear))
                        {
                            var dtoPrevPFOpBalance = genericRepo.Get<DTOModel.tblPFOpBalance>(x => x.Employeecode == employeeCode
                            && x.Salmonth == salMonth.ToString() && x.Salyear == salYear).ToList();
                            genericRepo.DeleteAll<DTOModel.tblPFOpBalance>(dtoPrevPFOpBalance);

                            // flag = genericRepo.AddMultipleEntity<DTOModel.tblPFOpBalance>(dtoEmpPFOpBalance);
                        }
                    }
                    else if (allBranches)
                    {
                        if (genericRepo.Exists<DTOModel.tblPFOpBalance>(x => x.Salmonth == salMonth.ToString() && x.Salyear == salYear))
                        {
                            var dtoPrevPFOpBalance = genericRepo.Get<DTOModel.tblPFOpBalance>(x =>
                            x.Salmonth == salMonth.ToString() && x.Salyear == salYear && employeeIDs.Any(y => y == x.EmployeeID)).ToList();
                            genericRepo.DeleteAll<DTOModel.tblPFOpBalance>(dtoPrevPFOpBalance);

                            //  flag = genericRepo.AddMultipleEntity<DTOModel.tblPFOpBalance>(dtoEmpPFOpBalance);
                        }
                    }


                    flag = genericRepo.AddMultipleEntity<DTOModel.tblPFOpBalance>(dtoEmpPFOpBalance);
                }

                #region ========  Update Loan Priority / Trans Data   =======

                var f_LoanPriorityNew = PayrollMaster.F_LoanPriorityNew;
                var f_LoanTranNew = PayrollMaster.F_LoanTransNew;
                var f_LoanPriorityOld = PayrollMaster.F_LoanTransOld;
                var f_LoanTranOld = PayrollMaster.F_LoanTransOld;


                if (f_LoanPriorityNew != null || f_LoanTranNew != null || f_LoanPriorityOld != null || f_LoanTranOld != null)
                {
                    IGenerateSalaryRepository gSRepo = new GenerateSalaryRepository();
                    var updated = gSRepo.UpdateSalaryLoanData(f_LoanPriorityNew, f_LoanTranNew, f_LoanPriorityOld, f_LoanTranOld);
                    if (updated)
                        flag = updated;
                }
                #endregion

                #region === Writing Salary Head rules to salaryHeadHistory table ==========

                if (flag)
                {
                    var salaryPeriod = $"{salYear.ToString()}{salMonth.ToString("00")}";
                    employeeTypeID = !employeeTypeID.HasValue ? 5 : employeeTypeID.Value;

                    if (!allBranchExceptHO && !branchID.HasValue && !allBranches)
                        employeeTypeID = 5;

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<SalaryHead, DTOModel.salaryheadshistory>()
                       .ForMember(d => d.Period, o => o.UseValue(salaryPeriod))
                       .ForMember(d => d.CreatedBy, o => o.UseValue(1))
                       .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now));
                    });
                    var dtoSalaryHistory = Mapper.Map<List<DTOModel.salaryheadshistory>>(PayrollMaster.salHeads.ToList());

                    if (genericRepo.Exists<DTOModel.salaryheadshistory>(x => x.EmployeeTypeID == employeeTypeID && x.Period == salaryPeriod))
                    {
                        var lastSavedHistoryRecords = genericRepo.Get<DTOModel.salaryheadshistory>(x =>
                        x.EmployeeTypeID == employeeTypeID && x.Period == salaryPeriod);
                        genericRepo.DeleteAll<DTOModel.salaryheadshistory>(lastSavedHistoryRecords);
                    }
                    flag = genericRepo.AddMultipleEntity<DTOModel.salaryheadshistory>(dtoSalaryHistory);
                }
                #endregion

                #region Update Pension Deduct in TBLFinalMonthlySalary Table

                #endregion

                log.Info($"flag value={flag}");

                return true;
                //  return flag;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private decimal FixedValueFormula(SalaryHead salHead, EmployeeSalary empSalary)
        {
            log.Info($"SalaryGenerator/FixedValueFormula");

            decimal colValue = 0; decimal cellValue = 0;

            if (salHead.CheckHeadInEmpSalTable)
            {
                var modelProperty = GetModelPropertyValue<EmployeeSalary>(empSalary, salHead.FieldName);

                if (modelProperty != null)
                    cellValue = Convert.ToDecimal(modelProperty);

                if (salHead.LocationDependent)
                {
                    if (cellValue > 0)
                    {
                        var bSalHead = PayrollMaster.branchSalaryHeadRule.Where(x => x.FieldName == salHead.FieldName
                                       && x.BranchID == empSalary.BranchID && x.FixedValueFormula == true).FirstOrDefault();
                        if (bSalHead != null)
                            colValue = bSalHead.FixedValue ?? 0;
                    }
                }
                else if (!salHead.LocationDependent)
                {
                    if (cellValue > 0)
                        colValue = salHead.FixedValue;
                }
            }
            else
            {
                if (salHead.LocationDependent)
                {
                    var bSalHead = PayrollMaster.branchSalaryHeadRule.Where(x => x.FieldName == salHead.FieldName
                                   && x.BranchID == empSalary.BranchID && x.FixedValueFormula == true).FirstOrDefault();
                    if (bSalHead != null)
                        colValue = bSalHead.FixedValue ?? 0;
                }
                else
                    colValue = salHead.FixedValue;
            }
            return colValue;
        }

        public decimal CustomRounding(decimal dbl, int roundto)
        {
            string[] arr;
            string str = System.Convert.ToString(dbl);
            string sDeci;

            arr = str.Split(new char[] { '.' });


            if (arr.Length == 1)
            {
                return dbl;
            }
            sDeci = arr[1];

            if (sDeci.Length > 2)
            {
                sDeci = arr[1];
                arr[1] = sDeci.Substring(0, 2);
            }

            if (roundto == 0)
            {
                if (arr[1].Length == 1)
                {
                    arr[1] = System.Convert.ToString(System.Convert.ToInt32(arr[1]) * 10);
                    if (System.Convert.ToInt32(arr[1]) > 49)
                        return Math.Ceiling(dbl);
                    if (System.Convert.ToInt32(arr[1]) < 50)
                        return Math.Floor(dbl);
                }
                else
                {
                    if (System.Convert.ToInt32(arr[1]) > 49)
                        return Math.Ceiling(dbl);
                    if (System.Convert.ToInt32(arr[1]) < 50)
                        return Math.Floor(dbl);
                }
            }
            return Math.Round(dbl, roundto);
        }

        private decimal WithSuspendedRemuneration(decimal eBasic, DateTime mFirstDate, DateTime mLastDate,
            IEnumerable<EmployeeSuspensionPeriod> empSuspensionIntervals)
        {
            log.Info($"SalaryGenerator/WithSuspendedRemuneration/{eBasic},mFirstDate:{mFirstDate},mLastDate:{mLastDate}");
            try
            {
                decimal newBasic = 0;
                var daysInMonth = mLastDate.Day;
                var totalSuspensionDays = 0;

                foreach (var item in empSuspensionIntervals)  ///==== Remuneration in suspension  period.===
                {
                    if (item.PeriodFrom <= mFirstDate && mLastDate <= item.PeriodTo)
                    {
                        totalSuspensionDays += daysInMonth;
                        newBasic += ((eBasic * ((decimal)(item.BasicSalaryPercentage) / 100)) * daysInMonth / daysInMonth);
                    }
                    if ((item.NoOfDays == daysInMonth && item.PeriodFrom == mFirstDate))
                    {
                        totalSuspensionDays += daysInMonth;
                        newBasic += ((eBasic * ((decimal)(item.BasicSalaryPercentage) / 100)) * item.NoOfDays / daysInMonth);
                    }
                    else if (item.NoOfDays > daysInMonth && (item.PeriodFrom >= mFirstDate))
                    {
                        totalSuspensionDays += (mLastDate - item.PeriodFrom).Days + 1;
                        newBasic += ((eBasic * ((decimal)(item.BasicSalaryPercentage) / 100)) * ((mLastDate - item.PeriodFrom).Days + 1) / daysInMonth);
                    }
                    else if (item.NoOfDays > daysInMonth && item.PeriodTo <= mLastDate)
                    {
                        totalSuspensionDays += (item.PeriodTo - mFirstDate).Days + 1;
                        newBasic += ((eBasic * ((decimal)(item.BasicSalaryPercentage) / 100)) * ((item.PeriodTo - mFirstDate).Days + 1) / daysInMonth);
                    }
                }
                ///==== noraml ==

                return newBasic += (eBasic * (daysInMonth - totalSuspensionDays) / daysInMonth);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
