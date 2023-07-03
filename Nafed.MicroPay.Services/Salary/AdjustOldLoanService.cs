using Nafed.MicroPay.Common;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
using System.Data;
using static Nafed.MicroPay.ImportExport.Export;
namespace Nafed.MicroPay.Services.Salary
{
    public class AdjustOldLoanService : BaseService, IAdjustOldLoanService
    {
        private readonly IGenericRepository genericRepo;
        private readonly ISalaryRepository salaryRepo;     
        public AdjustOldLoanService(IGenericRepository genericRepo, ISalaryRepository salaryRepo)
        {
            this.genericRepo = genericRepo;
            this.salaryRepo = salaryRepo;    
        }
        public List<SelectListModel> GetEmployeeByLoanType(int loanTypeId, bool oldLoanEmployee)
        {
            log.Info($"AdjustOldLoanService/GetEmployeeByLoanType/{loanTypeId}");
            try
            {
                List<SelectListModel> details = new List<SelectListModel>();
                //var listDetails = salaryRepo.GetEmployeeByLoanType(loanTypeId, oldLoanEmployee);
                var listDetails = salaryRepo.GetEmployeeByLoanTypedt(loanTypeId, oldLoanEmployee);

                details = (from DataRow dr in listDetails.Rows
                           select new SelectListModel()
                           {
                               id = Convert.ToInt32(dr["EmployeeId"]),
                               value = (dr["EmployeeCode"].ToString()) + "-" + (dr["Name"].ToString())
                           }).ToList();

                //details = listDetails.Select(x => new SelectListModel { id = x.id, value = x.value }).ToList();
                return details;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public SanctionLoan GetAdjustLoanOldDetails(int employeeId, int loanTypeId, bool showOld, out List<SanctionLoan> sanctionloan)
        {
            log.Info($"AdjustOldLoanService/GetAdjustLoanOldDetails/{employeeId}/{loanTypeId}");
            try
            {
                sanctionloan = new List<SanctionLoan>();
                if (!showOld)
                {
                    var priorityNumber = string.Empty;
                    var loanDetails = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.EmployeeID == employeeId && x.LoanTypeId == loanTypeId && x.Status == false && x.LoanSanc == true).FirstOrDefault();

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Data.Models.tblMstLoanPriority, Model.SanctionLoan>()
                         .ForMember(c => c.SerialNo, c => c.MapFrom(m => m.SerialNo))
                        .ForMember(c => c.Branchcode, c => c.MapFrom(m => m.Branchcode))
                        .ForMember(c => c.BranchId, c => c.MapFrom(m => m.BranchID))
                        .ForMember(c => c.PriorityNo, c => c.MapFrom(m => m.PriorityNo))
                        .ForMember(c => c.EmpCode, c => c.MapFrom(m => m.EmpCode))
                        .ForMember(c => c.DateofApp, c => c.MapFrom(m => m.DateofApp))
                        .ForMember(c => c.DateRcptApp, c => c.MapFrom(m => m.DateRcptApp))
                        .ForMember(c => c.ReqAmt, c => c.MapFrom(m => m.ReqAmt))
                        .ForMember(c => c.Surety, c => c.MapFrom(m => m.Surety))
                        //.ForMember(c => c.LoanSanc, c => c.MapFrom(m => m.LoanSanc))
                        .ForMember(c => c.LoanSanction, c => c.MapFrom(m => m.LoanSanc))
                        .ForMember(c => c.DateofSanc, c => c.MapFrom(m => m.DateofSanc))
                        .ForMember(c => c.DateAvailLoan, c => c.MapFrom(m => m.DateAvailLoan))
                        .ForMember(c => c.EffDate, c => c.MapFrom(m => m.EffDate))
                        .ForMember(c => c.Reasonref, c => c.MapFrom(m => m.Reasonref))
                        .ForMember(c => c.Dateofref, c => c.MapFrom(m => m.Dateofref))
                        .ForMember(c => c.MaxLoanAmt, c => c.MapFrom(m => m.MaxLoanAmt))
                        .ForMember(c => c.ROI, c => c.MapFrom(m => m.ROI))
                        .ForMember(c => c.Asubmitted, c => c.MapFrom(m => m.Asubmitted))
                        .ForMember(c => c.Bsubmitted, c => c.MapFrom(m => m.Bsubmitted))
                        .ForMember(c => c.Csubmitted, c => c.MapFrom(m => m.Csubmitted))
                        .ForMember(c => c.Dsubmitted, c => c.MapFrom(m => m.Dsubmitted))
                        .ForMember(c => c.Esubmitted, c => c.MapFrom(m => m.Esubmitted))
                        .ForMember(c => c.Fsubmitted, c => c.MapFrom(m => m.Fsubmitted))
                        .ForMember(c => c.Gsubmitted, c => c.MapFrom(m => m.Gsubmitted))
                        .ForMember(c => c.Hsubmitted, c => c.MapFrom(m => m.Hsubmitted))
                        .ForMember(c => c.Detail, c => c.MapFrom(m => m.Detail))
                        .ForMember(c => c.LoanMode, c => c.MapFrom(m => m.LoanMode))
                        .ForMember(c => c.IsFloatingRate, c => c.MapFrom(m => m.IsFloatingRate))
                        .ForMember(c => c.IsInterestPayable, c => c.MapFrom(m => m.IsInterestPayable))
                        .ForMember(c => c.OriginalPInstNo, c => c.MapFrom(m => m.OriginalPInstNo))
                        .ForMember(c => c.OriginalIInstNo, c => c.MapFrom(m => m.OriginalIInstNo))
                        .ForMember(c => c.OriginalPinstAmt, c => c.MapFrom(m => m.OriginalPinstAmt))
                        .ForMember(c => c.InterestInstAmt, c => c.MapFrom(m => m.InterestInstAmt))
                        .ForMember(c => c.BalancePAmt, c => c.MapFrom(m => m.BalancePAmt))
                        .ForMember(c => c.BalanceIAmt, c => c.MapFrom(m => m.BalanceIAmt))
                        .ForMember(c => c.TotalBalanceAmt, c => c.MapFrom(m => m.TotalBalanceAmt))
                        .ForMember(c => c.RemainingPInstNo, c => c.MapFrom(m => m.RemainingPInstNo))
                        .ForMember(c => c.RemainingIInstNo, c => c.MapFrom(m => m.RemainingIInstNo))
                        .ForMember(c => c.LastPaidInstDeduDt, c => c.MapFrom(m => m.LastPaidInstDeduDt))
                        .ForMember(c => c.LastPaidPInstAmt, c => c.MapFrom(m => m.LastPaidPInstAmt))
                        .ForMember(c => c.LastPaidPInstNo, c => c.MapFrom(m => m.LastPaidPInstNo))
                        .ForMember(c => c.LastPaidIInstNo, c => c.MapFrom(m => m.LastPaidIInstNo))
                        .ForMember(c => c.LastPaidIInstNo, c => c.MapFrom(m => m.LastPaidIInstNo))
                        .ForMember(c => c.LastMonthInterest, c => c.MapFrom(m => m.LastMonthInterest))
                        .ForMember(c => c.TotalSkippedInst, c => c.MapFrom(m => m.TotalSkippedInst))
                        .ForMember(c => c.CurrentROI, c => c.MapFrom(m => m.CurrentROI))
                        .ForMember(c => c.IsNewLoanAfterDevelop, c => c.MapFrom(m => m.IsNewLoanAfterDevelop))
                        .ForMember(c => c.Status, c => c.MapFrom(m => m.Status))
                        .ForMember(c => c.LastInstAmt, c => c.MapFrom(m => m.LastInstAmt))
                        .ForMember(c => c.AdjustedSancAmt, c => c.MapFrom(m => m.AdjustedSancAmt))
                        .ForMember(c => c.EmployeeId, c => c.MapFrom(m => m.EmployeeID))
                        .ForMember(c => c.LoanTypeId, c => c.MapFrom(m => m.LoanTypeId))
                        .ForMember(c => c.AssignLoanTypeId, c => c.MapFrom(m => m.LoanTypeId))
                        .ForMember(c => c.SancAmt, c => c.MapFrom(m => m.SancAmt))
                        .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                        .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                        .ForAllOtherMembers(c => c.Ignore());
                    });
                    var objAdjustLoan = Mapper.Map<DTOModel.tblMstLoanPriority, Model.SanctionLoan>(loanDetails);
                    if (objAdjustLoan != null)
                        priorityNumber = objAdjustLoan.PriorityNo;

                    if (priorityNumber != null && priorityNumber != "")
                        sanctionloan = genericRepo.Get<DTOModel.tblLoanAdjustOld>(y => y.PriorityNo == priorityNumber)
                            .Select(y => new SanctionLoan() { PriorityNo = y.PriorityNo, DateofSanc = y.DateofDeposit, ActualInstNoRem = y.ActualInstNoRem, ChangedInstNoRem = y.ChangedInstNoRem.Value, OriginalPinstAmt = y.ActualInstAmt, ChangedInstAmt = y.ChangedInstAmt, InterestInstAmt = y.ActualInstAmt, ChangeIntAmt = y.ChangeIntAmt, EmployeeId = employeeId, LoanTypeId = loanTypeId }).ToList();

                    return objAdjustLoan;
                }
                else
                {
                    var loanDetails = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.EmployeeID == employeeId && x.LoanTypeId == loanTypeId && x.Status == false && x.LoanSanc == true).FirstOrDefault();

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Data.Models.tblMstLoanPriority, Model.SanctionLoan>()
                         .ForMember(c => c.SerialNo, c => c.MapFrom(m => m.SerialNo))
                        .ForMember(c => c.Branchcode, c => c.MapFrom(m => m.Branchcode))
                        .ForMember(c => c.BranchId, c => c.MapFrom(m => m.BranchID))
                        .ForMember(c => c.PriorityNo, c => c.MapFrom(m => m.PriorityNo))
                        .ForMember(c => c.EmpCode, c => c.MapFrom(m => m.EmpCode))
                        .ForMember(c => c.DateofApp, c => c.MapFrom(m => m.DateofApp))
                        .ForMember(c => c.DateRcptApp, c => c.MapFrom(m => m.DateRcptApp))
                        .ForMember(c => c.ReqAmt, c => c.MapFrom(m => m.ReqAmt))
                        .ForMember(c => c.Surety, c => c.MapFrom(m => m.Surety))
                        //.ForMember(c => c.LoanSanc, c => c.MapFrom(m => m.LoanSanc))
                        .ForMember(c => c.LoanSanction, c => c.MapFrom(m => m.LoanSanc))
                        .ForMember(c => c.DateofSanc, c => c.MapFrom(m => m.DateofSanc))
                        .ForMember(c => c.DateAvailLoan, c => c.MapFrom(m => m.DateAvailLoan))
                        .ForMember(c => c.EffDate, c => c.MapFrom(m => m.EffDate))
                        .ForMember(c => c.Reasonref, c => c.MapFrom(m => m.Reasonref))
                        .ForMember(c => c.Dateofref, c => c.MapFrom(m => m.Dateofref))
                        .ForMember(c => c.MaxLoanAmt, c => c.MapFrom(m => m.MaxLoanAmt))
                        .ForMember(c => c.ROI, c => c.MapFrom(m => m.ROI))
                        .ForMember(c => c.Asubmitted, c => c.MapFrom(m => m.Asubmitted))
                        .ForMember(c => c.Bsubmitted, c => c.MapFrom(m => m.Bsubmitted))
                        .ForMember(c => c.Csubmitted, c => c.MapFrom(m => m.Csubmitted))
                        .ForMember(c => c.Dsubmitted, c => c.MapFrom(m => m.Dsubmitted))
                        .ForMember(c => c.Esubmitted, c => c.MapFrom(m => m.Esubmitted))
                        .ForMember(c => c.Fsubmitted, c => c.MapFrom(m => m.Fsubmitted))
                        .ForMember(c => c.Gsubmitted, c => c.MapFrom(m => m.Gsubmitted))
                        .ForMember(c => c.Hsubmitted, c => c.MapFrom(m => m.Hsubmitted))
                        .ForMember(c => c.Detail, c => c.MapFrom(m => m.Detail))
                        .ForMember(c => c.LoanMode, c => c.MapFrom(m => m.LoanMode))
                        .ForMember(c => c.IsFloatingRate, c => c.MapFrom(m => m.IsFloatingRate))
                        .ForMember(c => c.IsInterestPayable, c => c.MapFrom(m => m.IsInterestPayable))
                        .ForMember(c => c.OriginalPInstNo, c => c.MapFrom(m => m.OriginalPInstNo))
                        .ForMember(c => c.OriginalIInstNo, c => c.MapFrom(m => m.OriginalIInstNo))
                        .ForMember(c => c.OriginalPinstAmt, c => c.MapFrom(m => m.OriginalPinstAmt))
                        .ForMember(c => c.InterestInstAmt, c => c.MapFrom(m => m.InterestInstAmt))
                        .ForMember(c => c.BalancePAmt, c => c.MapFrom(m => m.BalancePAmt))
                        .ForMember(c => c.BalanceIAmt, c => c.MapFrom(m => m.BalanceIAmt))
                        .ForMember(c => c.TotalBalanceAmt, c => c.MapFrom(m => m.TotalBalanceAmt))
                        .ForMember(c => c.RemainingPInstNo, c => c.MapFrom(m => m.RemainingPInstNo))
                        .ForMember(c => c.RemainingIInstNo, c => c.MapFrom(m => m.RemainingIInstNo))
                        .ForMember(c => c.LastPaidInstDeduDt, c => c.MapFrom(m => m.LastPaidInstDeduDt))
                        .ForMember(c => c.LastPaidPInstAmt, c => c.MapFrom(m => m.LastPaidPInstAmt))
                        .ForMember(c => c.LastPaidPInstNo, c => c.MapFrom(m => m.LastPaidPInstNo))
                        .ForMember(c => c.LastPaidIInstNo, c => c.MapFrom(m => m.LastPaidIInstNo))
                        .ForMember(c => c.LastPaidIInstNo, c => c.MapFrom(m => m.LastPaidIInstNo))
                        .ForMember(c => c.LastMonthInterest, c => c.MapFrom(m => m.LastMonthInterest))
                        .ForMember(c => c.TotalSkippedInst, c => c.MapFrom(m => m.TotalSkippedInst))
                        .ForMember(c => c.CurrentROI, c => c.MapFrom(m => m.CurrentROI))
                        .ForMember(c => c.IsNewLoanAfterDevelop, c => c.MapFrom(m => m.IsNewLoanAfterDevelop))
                        .ForMember(c => c.Status, c => c.MapFrom(m => m.Status))
                        .ForMember(c => c.LastInstAmt, c => c.MapFrom(m => m.LastInstAmt))
                        .ForMember(c => c.AdjustedSancAmt, c => c.MapFrom(m => m.AdjustedSancAmt))
                        .ForMember(c => c.EmployeeId, c => c.MapFrom(m => m.EmployeeID))
                        .ForMember(c => c.LoanTypeId, c => c.MapFrom(m => m.LoanTypeId))
                        .ForMember(c => c.AssignLoanTypeId, c => c.MapFrom(m => m.LoanTypeId))
                        .ForMember(c => c.SancAmt, c => c.MapFrom(m => m.SancAmt))
                        .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                        .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                        .ForAllOtherMembers(c => c.Ignore());
                    });
                    var objAdjustLoan = Mapper.Map<DTOModel.tblMstLoanPriority, Model.SanctionLoan>(loanDetails);

                    sanctionloan = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.EmployeeID == employeeId)
                    .Select(y => new SanctionLoan() { SerialNo = y.SerialNo, Branchcode = y.Branchcode, EmpCode = y.EmpCode, PriorityNo = y.PriorityNo, DateofSanc = y.DateofSanc, BalancePAmt = y.BalancePAmt, ActualInstNoRem = (y.OriginalPInstNo - y.LastPaidPInstNo), ChangedInstNoRem = 0, OriginalPinstAmt = y.OriginalPinstAmt, ChangedInstAmt = 0, InterestInstAmt = y.InterestInstAmt, ChangeIntAmt = 0, EmployeeId = y.EmployeeID, LoanTypeId = y.LoanTypeId, DateAvailLoan = y.DateAvailLoan }).ToList();
                    return objAdjustLoan;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateTotalRefundDetails(SanctionLoan sanctionLoan)
        {
            log.Info($"AdjustOldLoanService/UpdateTotalRefundDetails");
            bool flag = false;
            DTOModel.tblMstLoanPriority details = new DTOModel.tblMstLoanPriority();
            DTOModel.tblMstLoanPriorityHistory details1 = new DTOModel.tblMstLoanPriorityHistory();
            var period = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>().Max(x => x.period);
            var empcode = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == sanctionLoan.EmployeeId).FirstOrDefault().EmployeeCode;
            try
            {
                // Need to correct tblloanTrans Table changes takes from sujjet sir
                var loanTrans = genericRepo.Get<DTOModel.tblLoanTran>(x => x.EmployeeID == sanctionLoan.EmployeeId && x.LoanTypeID == sanctionLoan.LoanTypeId && x.IsNewLoanAfterDevelop == 1);
                if (loanTrans.Count() == 0)
                {
                    details = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.PriorityNo == sanctionLoan.PriorityNo && x.Status == false).FirstOrDefault();
                    if (details != null)
                    {
                        details.LastPaidIInstNo = 1;//lastpaidpinstno
                        genericRepo.Update<DTOModel.tblMstLoanPriority>(details);
                    }

                    details1 = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>(x => x.PriorityNo == sanctionLoan.PriorityNo && x.Status == false && x.period == period).FirstOrDefault();
                    if (details1 != null)
                    {
                        details1.LastPaidIInstNo = 1;//lastpaidpinstno
                        genericRepo.Update<DTOModel.tblMstLoanPriorityHistory>(details1);
                    }
                    flag = true;
                }

                var serialNo = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.EmpCode == empcode && x.LoanTypeId == sanctionLoan.LoanTypeId && x.SerialNo != null).Max(x => x.SerialNo.Value);

                var update = salaryRepo.UpdateTotalRefundData(sanctionLoan.PriorityNo, serialNo, period, sanctionLoan.BalancePAmt, sanctionLoan.BalanceIAmt, sanctionLoan.RefundDate, Convert.ToByte(sanctionLoan.totlRefundMonthId), Convert.ToInt16(sanctionLoan.totlRefundYearId));

                //details = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.PriorityNo == sanctionLoan.PriorityNo && x.Status == false && x.SerialNo == serialNo).FirstOrDefault();
                //if (details != null)
                //{
                //    details.Status = true;
                //    details.IsNewLoanAfterDevelop = false;
                //    details.withdrawlAmt = sanctionLoan.BalancePAmt;
                //    details.WithdrawlInterestAmt = sanctionLoan.BalanceIAmt;
                //    details.BalancePAmt = 0;
                //    details.Wdate = sanctionLoan.RefundDate;
                //    details.Rmonth = Convert.ToByte(sanctionLoan.totlRefundMonthId);
                //    details.Ryear = Convert.ToInt16(sanctionLoan.totlRefundYearId);
                //    details.LoanSanc = false;
                //    genericRepo.Update<DTOModel.tblMstLoanPriority>(details);
                //}

                //details1 = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>(x => x.PriorityNo == sanctionLoan.PriorityNo && x.Status == false && x.SerialNo == serialNo && x.period == period).FirstOrDefault();
                //if (details1 != null)
                //{
                //    details1.Status = true;
                //    details1.IsNewLoanAfterDevelop = false;
                //    details1.withdrawlAmt = sanctionLoan.BalancePAmt;
                //    details1.WithdrawlInterestAmt = sanctionLoan.BalanceIAmt;
                //    details1.BalancePAmt = 0;
                //    details1.Wdate = sanctionLoan.RefundDate;
                //    details1.Rmonth = Convert.ToByte(sanctionLoan.totlRefundMonthId);
                //    details1.Ryear = Convert.ToInt16(sanctionLoan.totlRefundYearId);
                //    details1.LoanSanc = false;
                //    genericRepo.Update<DTOModel.tblMstLoanPriorityHistory>(details1);
                //}

                var loanTransDetails = genericRepo.Get<DTOModel.tblLoanTran>(x => x.EmployeeID == sanctionLoan.EmployeeId && x.LoanTypeID == sanctionLoan.LoanTypeId).FirstOrDefault();
                if (loanTransDetails != null)
                {
                    loanTransDetails.IsNewLoanAfterDevelop = 0;
                    genericRepo.Update<DTOModel.tblLoanTran>(loanTransDetails);
                }
                insertLoanAdjustOld(sanctionLoan);
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateLastInstallAdjust(SanctionLoan sanctionLoan)
        {
            log.Info($"AdjustOldLoanService/UpdateLastInstallAdjust");
            bool flag = false;
            DTOModel.tblMstLoanPriority details = new DTOModel.tblMstLoanPriority();
            var period = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>().Max(x => x.period);
            try
            {
                details = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.PriorityNo == sanctionLoan.PriorityNo && x.Status == false).FirstOrDefault();
                if (details != null)
                {
                    details.OriginalPinstAmt = sanctionLoan.BalancePAmt;
                    details.RemainingPInstNo = 1;
                    details.RemainingIInstNo = 1;
                    details.InterestInstAmt = sanctionLoan.BalanceIAmt;

                    details.BalanceIAmt = sanctionLoan.BalanceIAmt; //==== added new line to fix Balance Interest Amount  (07-Oct-20)

                    genericRepo.Update<DTOModel.tblMstLoanPriority>(details);
                }
                var loanPriorityHistory = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>(x => x.PriorityNo == sanctionLoan.PriorityNo && x.Status == false && x.period == period).FirstOrDefault();
                if (loanPriorityHistory != null)
                {
                    loanPriorityHistory.OriginalPinstAmt = sanctionLoan.BalancePAmt;
                    loanPriorityHistory.RemainingPInstNo = 1;
                    loanPriorityHistory.RemainingIInstNo = 1;
                    loanPriorityHistory.InterestInstAmt = sanctionLoan.BalanceIAmt;
                    loanPriorityHistory.BalanceIAmt = sanctionLoan.BalanceIAmt;   //==== added new line to fix Balance Interest Amount  (07-Oct-20)
                    genericRepo.Update<DTOModel.tblMstLoanPriorityHistory>(loanPriorityHistory);
                }
                // Insert 
                sanctionLoan.RemainingIInstNo = 1;
                insertLoanAdjustOld(sanctionLoan);
                flag = true;
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        private bool insertLoanAdjustOld(Model.SanctionLoan sanctionLoan)
        {
            log.Info($"AdjustOldLoanService/insertLoanAdjustOld");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.SanctionLoan, DTOModel.tblLoanAdjustOld>()
                      .ForMember(c => c.PriorityNo, c => c.MapFrom(m => m.PriorityNo))
                     .ForMember(c => c.DateofDeposit, c => c.UseValue(DateTime.Now))
                     .ForMember(c => c.ActualInstNoRem, c => c.UseValue(0))
                     .ForMember(c => c.ChangedInstNoRem, c => c.MapFrom(m => m.RemainingIInstNo))
                     .ForMember(c => c.ActualInstAmt, c => c.MapFrom(m => m.oldP1))
                     .ForMember(c => c.ChangedInstAmt, c => c.MapFrom(m => m.BalancePAmt))
                     .ForMember(c => c.ActualIntAmt, c => c.MapFrom(m => m.oldI2))
                     .ForMember(c => c.ChangeIntAmt, c => c.MapFrom(m => m.BalanceIAmt))
                    .ForMember(c => c.CreatedOn, c => c.UseValue(DateTime.Now))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoSanctionLoan = Mapper.Map<DTOModel.tblLoanAdjustOld>(sanctionLoan);
                genericRepo.Insert<DTOModel.tblLoanAdjustOld>(dtoSanctionLoan);
                flag = true;
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateInstallmentAdjestment(SanctionLoan sanctionLoan)
        {
            log.Info($"AdjustOldLoanService/UpdateInstallmentAdjestment");
            bool flag = false;
            DTOModel.tblMstLoanPriority details = new DTOModel.tblMstLoanPriority();
            var period = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>().Max(x => x.period);
            try
            {
                details = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.PriorityNo == sanctionLoan.PriorityNo && x.Status == false && x.IsNewLoanAfterDevelop == true).FirstOrDefault();
                if (details != null)
                {
                    details.OriginalPinstAmt = sanctionLoan.BalancePAmt;
                    details.InterestInstAmt = sanctionLoan.BalanceIAmt;
                    genericRepo.Update<DTOModel.tblMstLoanPriority>(details);
                }

                var loanPriorityHistory = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>(x => x.PriorityNo == sanctionLoan.PriorityNo && x.Status == false && x.period == period).FirstOrDefault();
                if (loanPriorityHistory != null)
                {
                    loanPriorityHistory.OriginalPinstAmt = sanctionLoan.BalancePAmt;
                    loanPriorityHistory.InterestInstAmt = sanctionLoan.BalanceIAmt;
                    genericRepo.Update<DTOModel.tblMstLoanPriorityHistory>(loanPriorityHistory);
                }

                // Insert 
                sanctionLoan.RemainingIInstNo = 1;
                insertLoanAdjustOld(sanctionLoan);
                flag = true;
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateLoanFinish(SanctionLoan sanctionLoan)
        {
            log.Info($"AdjustOldLoanService/UpdateLoanFinish");
            bool flag = false;
            string empCode = "";
            int serialNo;
            DTOModel.tblMstLoanPriority details = new DTOModel.tblMstLoanPriority();
            var period = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>().Max(x => x.period);
            var empcodeDetails = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == sanctionLoan.EmployeeId).FirstOrDefault();
            if (empcodeDetails != null)
                empCode = empcodeDetails.EmployeeCode;
            try
            {
                if (empCode != null)
                    serialNo = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.EmpCode == empCode && x.LoanTypeId == sanctionLoan.LoanTypeId && x.SerialNo != null).Max(x => x.SerialNo.Value);
                else
                    serialNo = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.EmployeeID == sanctionLoan.EmployeeId && x.LoanTypeId == sanctionLoan.LoanTypeId && x.SerialNo != null).Max(x => x.SerialNo.Value);


                details = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.PriorityNo == sanctionLoan.PriorityNo && x.Status == false && x.SerialNo == serialNo).FirstOrDefault();
                if (details != null)
                {
                    details.Status = true;
                    details.IsNewLoanAfterDevelop = false;
                    details.LoanSanc = false;
                    details.BalancePAmt = 0;
                    details.InterestInstAmt = sanctionLoan.BalanceIAmt;
                    genericRepo.Update<DTOModel.tblMstLoanPriority>(details);
                }

                var loanPriorityHistory = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>(x => x.PriorityNo == sanctionLoan.PriorityNo && x.Status == false && x.SerialNo == serialNo && x.period == period).FirstOrDefault();
                if (loanPriorityHistory != null)
                {
                    loanPriorityHistory.Status = true;
                    loanPriorityHistory.IsNewLoanAfterDevelop = false;
                    loanPriorityHistory.LoanSanc = false;
                    loanPriorityHistory.BalancePAmt = 0;
                    loanPriorityHistory.InterestInstAmt = sanctionLoan.BalanceIAmt;
                    genericRepo.Update<DTOModel.tblMstLoanPriorityHistory>(loanPriorityHistory);
                }

                // Update tblloantrans  

                var loanTransDetails = genericRepo.Get<DTOModel.tblLoanTran>(x => x.EmployeeID == sanctionLoan.EmployeeId && x.LoanTypeID == sanctionLoan.LoanTypeId).FirstOrDefault();
                if (loanTransDetails != null)
                {
                    loanTransDetails.IsNewLoanAfterDevelop = 0;
                    genericRepo.Update<DTOModel.tblLoanTran>(loanTransDetails);
                }

                flag = true;
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdatePartialRefund(SanctionLoan sanctionLoan)
        {
            log.Info($"AdjustOldLoanService/UpdatePartialRefund");
            bool flag = false;
            string empCode = "";
            DTOModel.tblMstLoanPriority details = new DTOModel.tblMstLoanPriority();
            DTOModel.tblMstLoanPriorityHistory loanPriorityHistory = new DTOModel.tblMstLoanPriorityHistory();

            var period = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>().Max(x => x.period);
            var empcodeDetails = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == sanctionLoan.EmployeeId).FirstOrDefault();
            if (empcodeDetails != null)
                empCode = empcodeDetails.EmployeeCode;

            try
            {
                decimal strBalncep = 0, OrignalPamtNow = 0, RemainingInstAmt1 = 0, remainingIinstNo = 0, balanceIamt1 = 0,
                    remainingBalancePamt = 0, RemainingPInstNumNow = 0, diffrenceinInstNO = 0, remainingIntrstAmt = 0,
                    totalBalanceAmt1 = 0;
                if (empCode != "")
                    details = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.EmpCode == empCode && x.LoanTypeId == sanctionLoan.LoanTypeId && x.Status == false && x.IsNewLoanAfterDevelop == true).FirstOrDefault();
                else
                    details = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.EmployeeID == sanctionLoan.EmployeeId && x.LoanTypeId == sanctionLoan.LoanTypeId && x.Status == false && x.IsNewLoanAfterDevelop == true).FirstOrDefault();

                if (details != null)
                {
                    strBalncep = (details.BalancePAmt == null ? 0 : details.BalancePAmt.Value);
                    OrignalPamtNow = (details.OriginalPinstAmt == null ? 0 : details.OriginalPinstAmt.Value);
                    RemainingInstAmt1 = (details.InterestInstAmt == null ? 0 : details.InterestInstAmt.Value);
                    remainingIinstNo = (details.RemainingPInstNo == null ? 0 : details.RemainingPInstNo.Value);
                    balanceIamt1 = (details.BalanceIAmt == null ? 0 : details.BalanceIAmt.Value);
                }
                remainingBalancePamt = strBalncep - sanctionLoan.BalancePAmt.Value;
                RemainingPInstNumNow = (remainingBalancePamt / OrignalPamtNow);
                diffrenceinInstNO = remainingIinstNo - RemainingPInstNumNow;
                remainingIntrstAmt = balanceIamt1 - (diffrenceinInstNO * RemainingInstAmt1);
                totalBalanceAmt1 = (remainingBalancePamt) + (remainingIntrstAmt);

                details = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.PriorityNo == sanctionLoan.PriorityNo && x.IsNewLoanAfterDevelop == true).FirstOrDefault();
                if (details != null)
                {
                    details.Status = false;
                    details.IsNewLoanAfterDevelop = true;
                    details.PARTREFUND = sanctionLoan.BalancePAmt;
                    details.PARTREFUNDINT = sanctionLoan.BalanceIAmt;
                    details.LoanSanc = true;
                    details.BalancePAmt = remainingBalancePamt;
                    details.BalanceIAmt = remainingIntrstAmt;
                    details.TotalBalanceAmt = totalBalanceAmt1;
                    details.RemainingPInstNo = Convert.ToInt32(RemainingPInstNumNow);
                    details.RemainingIInstNo = Convert.ToInt32(RemainingPInstNumNow);
                    details.Wdate = sanctionLoan.RefundDate;
                    details.Rmonth = Convert.ToByte(sanctionLoan.totlRefundMonthId);
                    details.Ryear = Convert.ToInt16(sanctionLoan.totlRefundYearId);
                    genericRepo.Update<DTOModel.tblMstLoanPriority>(details);
                }

                if (empCode != "")
                    loanPriorityHistory = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>(x => x.EmpCode == empCode && x.LoanTypeId == sanctionLoan.LoanTypeId && x.Status == false && x.IsNewLoanAfterDevelop == true && x.period == period).FirstOrDefault();
                else
                    loanPriorityHistory = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>(x => x.EmployeeID == sanctionLoan.EmployeeId && x.LoanTypeId == sanctionLoan.LoanTypeId && x.Status == false && x.IsNewLoanAfterDevelop == true && x.period == period).FirstOrDefault();

                if (loanPriorityHistory != null)
                {
                    strBalncep = (loanPriorityHistory.BalancePAmt == null ? 0 : loanPriorityHistory.BalancePAmt.Value);
                    OrignalPamtNow = (loanPriorityHistory.OriginalPinstAmt == null ? 0 : loanPriorityHistory.OriginalPinstAmt.Value);
                    RemainingInstAmt1 = (loanPriorityHistory.InterestInstAmt == null ? 0 : loanPriorityHistory.InterestInstAmt.Value);
                    remainingIinstNo = (loanPriorityHistory.RemainingPInstNo == null ? 0 : loanPriorityHistory.RemainingPInstNo.Value);
                    balanceIamt1 = (loanPriorityHistory.BalanceIAmt == null ? 0 : loanPriorityHistory.BalanceIAmt.Value);
                }

                remainingBalancePamt = strBalncep - sanctionLoan.BalancePAmt.Value;
                RemainingPInstNumNow = (remainingBalancePamt / OrignalPamtNow);
                diffrenceinInstNO = remainingIinstNo - RemainingPInstNumNow;
                remainingIntrstAmt = balanceIamt1 - (diffrenceinInstNO * RemainingInstAmt1);
                totalBalanceAmt1 = (remainingBalancePamt) + (remainingIntrstAmt);


                loanPriorityHistory = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>(x => x.PriorityNo == sanctionLoan.PriorityNo && x.IsNewLoanAfterDevelop == true && x.period == period).FirstOrDefault();
                if (loanPriorityHistory != null)
                {
                    loanPriorityHistory.Status = false;
                    loanPriorityHistory.IsNewLoanAfterDevelop = true;
                    loanPriorityHistory.PARTREFUND = sanctionLoan.BalancePAmt;
                    loanPriorityHistory.PARTREFUNDINT = sanctionLoan.BalanceIAmt;
                    loanPriorityHistory.LoanSanc = true;
                    loanPriorityHistory.BalancePAmt = remainingBalancePamt;
                    loanPriorityHistory.BalanceIAmt = remainingIntrstAmt;
                    loanPriorityHistory.TotalBalanceAmt = totalBalanceAmt1;
                    loanPriorityHistory.RemainingPInstNo = Convert.ToInt32(RemainingPInstNumNow);
                    loanPriorityHistory.RemainingIInstNo = Convert.ToInt32(RemainingPInstNumNow);
                    loanPriorityHistory.Wdate = sanctionLoan.RefundDate;
                    loanPriorityHistory.Rmonth = Convert.ToByte(sanctionLoan.totlRefundMonthId);
                    loanPriorityHistory.Ryear = Convert.ToInt16(sanctionLoan.totlRefundYearId);
                    genericRepo.Update<DTOModel.tblMstLoanPriorityHistory>(loanPriorityHistory);
                }
                //insert 
                sanctionLoan.RemainingIInstNo = Convert.ToInt32(RemainingPInstNumNow);
                sanctionLoan.oldP1 = Convert.ToInt32(OrignalPamtNow);
                sanctionLoan.BalancePAmt = Convert.ToInt32(OrignalPamtNow);
                sanctionLoan.oldI2 = Convert.ToInt32(RemainingInstAmt1);
                sanctionLoan.BalanceIAmt = Convert.ToInt32(RemainingInstAmt1);
                insertLoanAdjustOld(sanctionLoan);
                // Insert End
                flag = true;
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateLoanpriority(string priority, decimal remainingInstallments, decimal installmentAmt, decimal intInstallment, decimal intInstallmentAmount, bool status, bool flag)
        {
            log.Info($"AdjustOldLoanService/UpdateLoanpriority");
            bool update = false;
            var period = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>().Max(x => x.period);
            DTOModel.tblMstLoanPriority details = new DTOModel.tblMstLoanPriority();
            Model.SanctionLoan sanctionLoan = new Model.SanctionLoan();
            details = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.PriorityNo == priority && x.IsNewLoanAfterDevelop == false).FirstOrDefault();
            if (flag && details != null)
            {
                details.RemainingPInstNo = Convert.ToInt32(remainingInstallments);
                details.LastPaidPInstNo = details.LastPaidPInstNo + 1;
                details.LastPaidPInstAmt = installmentAmt;
                details.Status = status;
                genericRepo.Update<DTOModel.tblMstLoanPriority>(details);

                //insert
                sanctionLoan.PriorityNo = priority;
                sanctionLoan.RemainingIInstNo = 1;
                sanctionLoan.oldP1 = Convert.ToInt32(installmentAmt);
                sanctionLoan.BalancePAmt = installmentAmt;
                sanctionLoan.oldI2 = 0;
                sanctionLoan.BalanceIAmt = 0;
                insertLoanAdjustOld(sanctionLoan);
            }
            else if (!flag && details != null)
            {
                int inst = Convert.ToInt32(intInstallment);
                details.RemainingIInstNo = inst;
                details.LastPaidIInstNo = 0;
                details.RemainingPInstNo = 0;
                details.LastPaidPInstAmt = 0;
                details.LastPaidIInstAmt = intInstallmentAmount;
                details.Status = status;
                genericRepo.Update<DTOModel.tblMstLoanPriority>(details);

                //insert
                sanctionLoan.PriorityNo = priority;
                sanctionLoan.RemainingIInstNo = inst;
                sanctionLoan.oldP1 = Convert.ToInt32(intInstallmentAmount);
                sanctionLoan.BalancePAmt = intInstallmentAmount;
                sanctionLoan.oldI2 = 0;
                sanctionLoan.BalanceIAmt = 0;
                insertLoanAdjustOld(sanctionLoan);
            }

            var loanPriorityHistory = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>(x => x.PriorityNo == priority && x.IsNewLoanAfterDevelop == false && x.period == period).FirstOrDefault();
            if (flag && loanPriorityHistory != null)
            {
                loanPriorityHistory.RemainingPInstNo = Convert.ToInt32(remainingInstallments);
                loanPriorityHistory.LastPaidPInstNo = details.LastPaidPInstNo + 1;
                loanPriorityHistory.LastPaidPInstAmt = installmentAmt;
                loanPriorityHistory.Status = status;
                genericRepo.Update<DTOModel.tblMstLoanPriorityHistory>(loanPriorityHistory);
            }
            else if (!flag && loanPriorityHistory != null)
            {
                int inst = Convert.ToInt32(intInstallment);
                loanPriorityHistory.RemainingIInstNo = inst;
                loanPriorityHistory.LastPaidIInstNo = 0;
                loanPriorityHistory.RemainingPInstNo = 0;
                loanPriorityHistory.LastPaidPInstAmt = 0;
                loanPriorityHistory.LastPaidIInstAmt = intInstallmentAmount;
                loanPriorityHistory.Status = status;
                genericRepo.Update<DTOModel.tblMstLoanPriorityHistory>(loanPriorityHistory);
            }
            update = true;
            return update;
        }

        public bool UpdateAllLastInstallment(int loanTypeId)
        {
            log.Info($"AdjustOldLoanService/UpdateAllLastInstallment");
            try
            {
                bool flag = false;
                var period = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>().Max(x => x.period);
                var details = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.LoanTypeId == loanTypeId && x.Status == false && x.RemainingPInstNo == 1).FirstOrDefault();
                if (details != null)
                {
                    details.OriginalPinstAmt = details.BalancePAmt;
                    details.InterestInstAmt = details.BalanceIAmt;
                    genericRepo.Update<DTOModel.tblMstLoanPriority>(details);
                }
                var loanPriorityHistory = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>(x => x.LoanTypeId == loanTypeId && x.Status == false && x.RemainingPInstNo == 1).FirstOrDefault();
                if (loanPriorityHistory != null)
                {
                    loanPriorityHistory.OriginalPinstAmt = details.BalancePAmt;
                    loanPriorityHistory.InterestInstAmt = details.BalanceIAmt;
                    genericRepo.Update<DTOModel.tblMstLoanPriorityHistory>(loanPriorityHistory);
                }
                flag = true;
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateSalaryPublishField(PublishSalaryFilters publishSalary, string buttonType)
        {
            log.Info($"AdjustOldLoanService/UpdateSalaryPublishField");
            bool flag = false;
            try
            {
                flag = salaryRepo.UpdateSalaryPublishField(publishSalary.selectedEmployeeID, publishSalary.selectedBranchID, publishSalary.selectedEmployeeTypeID, publishSalary.salMonth, publishSalary.salYear, buttonType);
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdatePublishDAArrer(PublishSalaryFilters publishSalary, string buttonType)
        {
            log.Info($"AdjustOldLoanService/UpdatePublishDAArrer");
            bool flag = false;
            try
            {
                flag = salaryRepo.UpdatePublishDAArrer(publishSalary.selectedDAEmpID, publishSalary.selectedDABranchID, publishSalary.selectedDAEmpTypeID, publishSalary.salMonth, publishSalary.salYear, buttonType);
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdatePublishPayArrer(PublishSalaryFilters publishSalary, string buttonType)
        {
            log.Info($"AdjustOldLoanService/UpdatePublishPayArrer");
            bool flag = false;
            try
            {
                flag = salaryRepo.UpdatePublishPayArrer(publishSalary.selectedPayEmpID, publishSalary.selectedPayBranchID, publishSalary.selectedPayEmpTypeID, publishSalary.salMonth, publishSalary.salYear, buttonType);
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<ArrerPeriodDetails> GetArrearPeriodsDetails(string arrerType)
        {
            log.Info($"AdjustOldLoanService/GetArrearPeriodsDetails");
            try
            {
                var result = salaryRepo.GetArrearPeriodsDetails(arrerType);
                var arrerperiodDetails = Common.ExtensionMethods.ConvertToList<ArrerPeriodDetails>(result);
                return arrerperiodDetails;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<PFLoanSummaryReport> PFLoanSummary(int month, int year,int ? toMonth)
        {
            log.Info($"AdjustOldLoanService/PFLoanSummary");
            try
            {
                var result = salaryRepo.PFLoanSummary(month, year,toMonth);
                var loanSummaryDetails = Common.ExtensionMethods.ConvertToList<PFLoanSummaryReport>(result);
                return loanSummaryDetails;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdatePFLoanSummary(decimal cpension, string employeeCode, int month, int year, int userId)
        {
            log.Info($"AdjustOldLoanService/UpdatePFLoanSummary");
            try
            {
                bool flag = false;
                var result = genericRepo.Get<DTOModel.tblFinalMonthlySalary>(x => x.SalMonth == month && x.SalYear == year && x.RecordType == "S" && x.EmployeeCode == employeeCode).FirstOrDefault();
                if (result != null)
                {
                    result.C_Pension = cpension;
                    result.UpdatedBy = userId;
                    result.UpdatedOn = DateTime.Now;
                    genericRepo.Update<DTOModel.tblFinalMonthlySalary>(result);
                    flag = true;
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #region Yearly
        public int AnuualPFRegister(string fYear, string toYear)
        {
            log.Info($"AdjustOldLoanService/AnuualPFRegister");
            try
            {
                var result = salaryRepo.GetPFOpBalance(fYear, toYear);
                if (result != null)
                    return result.Rows.Count;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int PFIUYearlyReport(string fromYear, string tYear)
        {
            log.Info($"AdjustOldLoanService/PFIUYearlyReport");
            try
            {
                var result = salaryRepo.GetPFOpBalanceIU(fromYear, tYear);
                if (result != null)
                    return result.Rows.Count;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool EDLIStatement(int mon, int yr1)
        {
            log.Info($"AdjustOldLoanService/EDLIStatement");
            try
            {
                bool flag = false;
                var result = salaryRepo.GetEDLIStatement(mon, yr1);
                foreach (DataRow row in result.Rows)
                {
                    int? PfNo = null;
                    int? total = null;
                    string employeeCode = string.Empty;
                    if (!DBNull.Value.Equals(row["Pfno"]))
                        PfNo = Convert.ToInt32(row["Pfno"]);
                    if (!DBNull.Value.Equals(row["total"]))
                        total = Convert.ToInt32(row["total"]);
                    if (!DBNull.Value.Equals(row["Employeecode"]))
                        employeeCode = Convert.ToString(row["Employeecode"]);
                    if (PfNo != null)
                    {
                        var updatePFFlag = genericRepo.Get<DTOModel.tblPFFlagStatu>(x => x.PFno == PfNo).FirstOrDefault();
                        if (updatePFFlag != null)
                        {
                            updatePFFlag.Total = total;
                            genericRepo.Update<DTOModel.tblPFFlagStatu>(updatePFFlag);
                            flag = true;
                        }
                        else
                        {
                            DTOModel.tblPFFlagStatu tblpfFlagStatus = new DTOModel.tblPFFlagStatu();
                            tblpfFlagStatus.EmployeeCode = employeeCode;
                            tblpfFlagStatus.PFno = PfNo.Value;
                            tblpfFlagStatus.EDLIFlag = false;
                            tblpfFlagStatus.FPFFlag = false;
                            tblpfFlagStatus.ZeroFlag = false;
                            tblpfFlagStatus.Pensiondeduct = false;
                            tblpfFlagStatus.Total = total;
                            tblpfFlagStatus.Forcefully = false;
                            tblpfFlagStatus.AmountOfWages = 0;
                            tblpfFlagStatus.CreatedBy = 1;
                            tblpfFlagStatus.CreatedOn = DateTime.Now;
                            genericRepo.Insert<DTOModel.tblPFFlagStatu>(tblpfFlagStatus);
                            flag = true;
                        }
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Update_FORM7PSdata(string fryear, string tryear)
        {
            log.Info($"AdjustOldLoanService/Update_FORM7PSdata");
            try
            {
                var result = salaryRepo.Update_FORM7PSdata(fryear, tryear);
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        #region PF ECR Report
        public DataSet GetECRData(int salMonth, int salYear,int? intRate)
        {
            log.Info($"AdjustOldLoanService/GetECRData/{salMonth}");
            try
            {
                var ds = salaryRepo.GetECRDataForExport(salMonth, salYear, intRate);
                return ds;              
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool ExportECR(DataTable dtTable, string sFullPath, string fileName)
        {
            try
            {
                var flag = false;               
                if (dtTable != null)
                {
                    if (System.IO.Directory.Exists(sFullPath))
                    {
                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                        exportHdr = dtTable.Columns.Cast<System.Data.DataColumn>()
                            .Select(x => x.ColumnName).AsEnumerable<string>();
                        sFullPath = $"{sFullPath}{fileName}";
                        ECRExportToExcel(exportHdr, dtTable, fileName, sFullPath, null);
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

    }

}
