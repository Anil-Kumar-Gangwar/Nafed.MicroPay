using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using System;
using System.Linq;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Common;
using System.Threading.Tasks;
using Nafed.MicroPay.Services.IServices;
using System.Text;
using AutoMapper;
using System.Collections.Generic;

namespace Nafed.MicroPay.Services.Salary
{
    public class GenerateSalaryService : BaseService, IRegularEmpSalaryService, IPayrollApprovalEmail
    {
        private readonly IGenericRepository genericRepo;
        private readonly IGenerateSalaryRepository generateSalaryRepo;
        private readonly IArrearRepository arrearRepository;

        public GenerateSalaryService(IGenericRepository genericRepo, IGenerateSalaryRepository generateSalaryRepo, IArrearRepository arrearRepository)
        {
            this.generateSalaryRepo = generateSalaryRepo;
            this.genericRepo = genericRepo;
            this.arrearRepository = arrearRepository;

        }
        public string GetMonthlyFileName(string tcsFilePeriod)
        {
            log.Info($"GenerateSalary/GetMonthlyFileName/tcsFilePeriod={tcsFilePeriod}");
            string tcsFileName = string.Empty;
            try
            {
                var tcsFile = genericRepo.Get<DTOModel.MonthlyTCSFile>(x => x.TCSFilePeriod == tcsFilePeriod).FirstOrDefault();
                if (tcsFile != null)
                    tcsFileName = tcsFile.TCSFileName;
            }
            catch (Exception)
            {
                throw;
            }
            return tcsFileName;
        }

        public RegularEmployeeSalary DataInputsValidation(RegularEmployeeSalary regEmpSalary)
        {
            log.Info($"GenerateSalary/DataInputsValidation");

            try
            {
                int noOfEmpSalaryRows = 0, noOfMonthlyInputRows = 0, noOfLastGeneratedSalaryRows = 0;
                regEmpSalary.selectedEmployeeTypeID = regEmpSalary.selectedEmployeeTypeID ?? 0;
                regEmpSalary.BranchesExcecptHO = regEmpSalary.enumBranch == EnumBranch.BranchesExcecptHO ? true : false;
                regEmpSalary.AllBranches = regEmpSalary.enumBranch == EnumBranch.AllBranches ? true : false;

                regEmpSalary.AllEmployees = regEmpSalary.enumEmpCategory == EnumEmpCategory.AllEmployees ? true : false;

                if (genericRepo.Exists<DTOModel.TBLMONTHLYINPUT>(x => x.SalYear == regEmpSalary.salYear && x.SalMonth == regEmpSalary.salMonth + 1))
                {
                    regEmpSalary.CustomErrorFound = true;
                    regEmpSalary.CustomErrorMsg = $"You cannot generate salary again because the data might have been modified as there exists MonthlyInput of the next month !";
                    return regEmpSalary;
                }

                List<string> empCodesOfSalaryTable = new List<string>();
                List<string> empCodesOfMonthlyInput = new List<string>();

                var empCodesFromMaster = generateSalaryRepo.GetEmployeeMasterRecords(regEmpSalary.salMonth,
                    regEmpSalary.salYear, regEmpSalary.BranchesExcecptHO, regEmpSalary.AllEmployees, regEmpSalary.selectedEmployeeTypeID.Value,
                     regEmpSalary.selectedBranchID, regEmpSalary.selectedEmployeeID);

                noOfEmpSalaryRows = generateSalaryRepo.NofEmployeeSalaryRows(
                    regEmpSalary.salMonth, regEmpSalary.salYear,
                    regEmpSalary.BranchesExcecptHO,
                    regEmpSalary.AllEmployees, regEmpSalary.selectedEmployeeTypeID.Value,
                    regEmpSalary.selectedBranchID, regEmpSalary.selectedEmployeeID, out empCodesOfSalaryTable);

                noOfMonthlyInputRows = generateSalaryRepo.NoOfMonthlyInputRows(regEmpSalary.BranchesExcecptHO,
                    regEmpSalary.AllEmployees, regEmpSalary.salMonth, regEmpSalary.salYear, regEmpSalary.selectedEmployeeTypeID.Value,
                    regEmpSalary.selectedBranchID, regEmpSalary.selectedEmployeeID, out empCodesOfMonthlyInput);

                if (!(noOfEmpSalaryRows == noOfMonthlyInputRows))
                    regEmpSalary.CustomErrorFound = true;

                else if ((noOfEmpSalaryRows == noOfMonthlyInputRows) && noOfMonthlyInputRows == 0)
                    regEmpSalary.CustomErrorFound = true;

                if (!Enumerable.SequenceEqual(empCodesOfSalaryTable.OrderBy(fElement => fElement),
                         empCodesOfMonthlyInput.OrderBy(sElement => sElement)))
                    regEmpSalary.CustomErrorFound = true;


                else if (!Enumerable.SequenceEqual(empCodesFromMaster.OrderBy(fElement => fElement),
                         empCodesOfMonthlyInput.OrderBy(sElement => sElement))
                         &&
                         !Enumerable.SequenceEqual(empCodesFromMaster.OrderBy(fElement => fElement),
                         empCodesOfSalaryTable.OrderBy(sElement => sElement)))
                    regEmpSalary.CustomErrorFound = true;

                if (regEmpSalary.CustomErrorFound)
                {
                    //if (!regEmpSalary.selectedEmployeeID.HasValue)
                    //{
                    List<string> inList1ButNotList2 = (from o in empCodesFromMaster
                                                       join p in empCodesOfMonthlyInput on o equals p into t
                                                       from od in t.DefaultIfEmpty()
                                                       where od == null
                                                       select o).ToList<string>();

                    List<string> inList2ButNotList1 = (from o in empCodesFromMaster
                                                       join p in empCodesOfSalaryTable on o equals p into t
                                                       from od in t.DefaultIfEmpty()
                                                       where od == null
                                                       select o).ToList<string>();

                    if (inList1ButNotList2.Count > 0)
                    {
                        regEmpSalary.CustomErrorMsg = $"<h5>Error : Cannot process salary due to following reason(s) : -</h5>  ";
                        regEmpSalary.CustomErrorMsg += $"<ul>There is no monthly input data for the below employee code(s)-";

                        foreach (var item in inList1ButNotList2)
                        {
                            if (!(inList1ButNotList2.Last() == item))
                                regEmpSalary.CustomErrorMsg += $"<li>{item}</li>";
                            else
                                regEmpSalary.CustomErrorMsg += $"<li>{item}</li></ul>";
                        }
                    }

                    if (inList2ButNotList1.Count > 0)
                    {
                        regEmpSalary.CustomErrorMsg += $"<ul> The following employee code(s) have no basic salary - ";
                        foreach (var item in inList2ButNotList1)
                            if (!(inList2ButNotList1.Last() == item))
                                regEmpSalary.CustomErrorMsg += $"<li>{item}</li>";
                            else
                                regEmpSalary.CustomErrorMsg += $"<li>{item}</li></ul>";
                    }
                    // }
                    //else
                    //    regEmpSalary.CustomErrorMsg = $"Cannot process, because the monthly input & salary data for this employee are not matched.";

                    if ((noOfEmpSalaryRows == noOfMonthlyInputRows) && noOfMonthlyInputRows == 0)
                        regEmpSalary.CustomErrorMsg = $"Cannot process, because there is no record(s).";
                }
                //=== Check whether case of re- generation 

                if (!regEmpSalary.CustomErrorFound)
                {
                    noOfLastGeneratedSalaryRows = generateSalaryRepo.NoOfLastGeneratedSalaryRows(regEmpSalary.BranchesExcecptHO,
                        regEmpSalary.AllEmployees, regEmpSalary.salMonth, regEmpSalary.salYear, regEmpSalary.selectedEmployeeTypeID.Value,
                        regEmpSalary.selectedBranchID, regEmpSalary.selectedEmployeeID);

                    if ((noOfEmpSalaryRows == noOfLastGeneratedSalaryRows) && noOfEmpSalaryRows > 0)
                    {
                        regEmpSalary.RegeneratedCase = true;

                        ///==== updating loan transaction data for old/ new loans....
                        generateSalaryRepo.UpdateLoanTransData(regEmpSalary.BranchesExcecptHO, regEmpSalary.AllEmployees, regEmpSalary.salMonth, regEmpSalary.salYear,
                            regEmpSalary.selectedEmployeeTypeID.Value, regEmpSalary.selectedBranchID, regEmpSalary.selectedEmployeeID);
                    }

                    var dtoMonthlyTCSFile = genericRepo.Get<DTOModel.MonthlyTCSFile>(x => x.TCSFilePeriod == regEmpSalary.TCSFilePeriod).FirstOrDefault();
                    if (dtoMonthlyTCSFile != null)
                        regEmpSalary.TCSFileName = dtoMonthlyTCSFile.TCSFileName;

                    IList<string> negtvSalEmp;
                    SalaryGenerator salGenerator = new SalaryGenerator(genericRepo, generateSalaryRepo);
                    regEmpSalary.IsSalaryCalculationDone = salGenerator.CalculateSalary(regEmpSalary, out negtvSalEmp);
                    regEmpSalary.NegativeSalEmp = negtvSalEmp
                        ;
                    log.Info($"IsSalaryCalculationDone={regEmpSalary.IsSalaryCalculationDone}");
                }
                //===  end================================

                return regEmpSalary;
            }
            catch (Exception ex)
            {
                log.Error("Salary Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public RegularEmployeeSalary SendApprovalRequest(RegularEmployeeSalary regEmpSalary)
        {
            log.Info($"GenerateSalaryService/SendApprovalRequest");
            try
            {
                var period = $"{regEmpSalary.salYear.ToString()}{ regEmpSalary.salMonth.ToString("00")}";

                regEmpSalary.selectedEmployeeTypeID = regEmpSalary.selectedEmployeeTypeID ?? 0;
                regEmpSalary.BranchesExcecptHO = regEmpSalary.enumBranch == EnumBranch.BranchesExcecptHO ? true : false;
                regEmpSalary.AllBranches = regEmpSalary.enumBranch == EnumBranch.AllBranches ? true : false;

                regEmpSalary.AllEmployees = regEmpSalary.enumEmpCategory == EnumEmpCategory.AllEmployees ? true : false;

                regEmpSalary.selectedBranchID = regEmpSalary.selectedEmployeeID.HasValue ? null : regEmpSalary.selectedBranchID;

                var reportingPersonEmails = genericRepo.Get<DTOModel.PayrollApprovalSetting>(x =>
                    x.ProcessID == (int)WorkFlowProcess.SalaryGenerate && x.ToDate == null).FirstOrDefault();
                var status = (byte)(reportingPersonEmails != null && reportingPersonEmails.Reporting2.HasValue ? 1 : 3);
                if (regEmpSalary.BranchesExcecptHO)
                {
                    if (genericRepo.Exists<DTOModel.PayrollApprovalRequest>(x => x.Period == period
                   && x.EmployeeTypeID == regEmpSalary.selectedEmployeeTypeID.Value
                   && x.ProcessID == (int)WorkFlowProcess.SalaryGenerate && x.BranchCode == "Except-HO" && (x.Status != 2 && x.Status != 4)))
                    {
                        regEmpSalary.CustomErrorFound = true;
                        regEmpSalary.CustomErrorMsg = $"Approval request already exists.";
                    }

                    else if (genericRepo.Exists<DTOModel.tblFinalMonthlySalary>(x => x.BranchCode != "99"
                     && x.SalMonth == regEmpSalary.salMonth && x.SalYear == regEmpSalary.salYear && !x.Publish
                     && x.tblMstEmployee.EmployeeTypeID == regEmpSalary.selectedEmployeeTypeID.Value))
                    {

                        DTOModel.PayrollApprovalRequest request = new DTOModel.PayrollApprovalRequest();
                        request.ProcessID = (int)WorkFlowProcess.SalaryGenerate;
                        request.CreatedOn = DateTime.Now;
                        request.CreatedBy = regEmpSalary.loggedInUserID;
                        request.EmployeeTypeID = regEmpSalary.selectedEmployeeTypeID.Value;
                        request.BranchCode = "Except-HO";
                        request.Status = status;
                        request.Period = period;

                        genericRepo.Insert<DTOModel.PayrollApprovalRequest>(request);
                        if (request.ApprovalRequestID > 0)
                            regEmpSalary.ApprovalRequestSent = true;
                    }


                    else if (genericRepo.Exists<DTOModel.tblFinalMonthlySalary>(x => x.BranchCode != "99"
                    && x.SalMonth == regEmpSalary.salMonth && x.SalYear == regEmpSalary.salYear && x.Publish
                    && x.tblMstEmployee.EmployeeTypeID == regEmpSalary.selectedEmployeeTypeID.Value ))

                    {
                        regEmpSalary.CustomErrorFound = true;
                        regEmpSalary.CustomErrorMsg = $"Cannot send approval request, because salary has been already approved for the selected period.";
                    }

                    else
                    {
                        regEmpSalary.CustomErrorFound = true;
                        regEmpSalary.CustomErrorMsg = $"Cannot send approval request, because salary has't been processed for the selected period.";
                    }
                }

                else if (regEmpSalary.AllBranches)
                {
                    var noOfAllBranches = genericRepo.GetIQueryable<DTOModel.TblMstEmployeeSalary>(x =>
                          x.tblMstEmployee.EmployeeTypeID == regEmpSalary.selectedEmployeeTypeID.Value
                          && (x.tblMstEmployee.DOLeaveOrg == null ||
                          (x.tblMstEmployee.DOLeaveOrg.Value.Year == regEmpSalary.salYear
                          && x.tblMstEmployee.DOLeaveOrg.Value.Month == regEmpSalary.salMonth
                          )) && !x.tblMstEmployee.IsSalgenrated && !x.tblMstEmployee.IsDeleted
                         ).Select(z => z.BranchID).Distinct().Count();


                    //var generatedSalaryBranchCount = (genericRepo.GetIQueryable<DTOModel.tblFinalMonthlySalary>(
                    //    x => !x.Publish && x.SalYear == regEmpSalary.salYear && x.SalMonth == regEmpSalary.salMonth
                    //    && x.EmployeeTypeID == regEmpSalary.selectedEmployeeTypeID.Value))
                    //    .Select(b => b.BranchID).Distinct().Count();


                    var generatedSalaryBranchCount = (genericRepo.GetIQueryable<DTOModel.tblFinalMonthlySalary>(
                       x => x.SalYear == regEmpSalary.salYear && x.SalMonth == regEmpSalary.salMonth
                       && x.EmployeeTypeID == regEmpSalary.selectedEmployeeTypeID.Value))
                       .Select(b => b.BranchID).Distinct().Count();

                    //var salaryGenerated = genericRepo.GetIQueryable<DTOModel.tblFinalMonthlySalary>(x=>
                    //      x.SalMonth == regEmpSalary.salMonth && x.SalYear == regEmpSalary.salYear
                    //      && x.tblMstEmployee.EmployeeTypeID == regEmpSalary.selectedEmployeeTypeID.Value
                    //      && branchIDs.Any(b=>b==x.BranchID));

                    if (genericRepo.Exists<DTOModel.PayrollApprovalRequest>(x => x.Period == period
                       && x.EmployeeTypeID == regEmpSalary.selectedEmployeeTypeID.Value
                       && x.ProcessID == (int)WorkFlowProcess.SalaryGenerate && x.BranchCode == "All" && (x.Status != 2 && x.Status != 4)))
                    {
                        regEmpSalary.CustomErrorFound = true;
                        regEmpSalary.CustomErrorMsg = $"Approval request already exists.";
                    }
                    else if (genericRepo.Exists<DTOModel.tblFinalMonthlySalary>(x => x.Publish && x.SalMonth == regEmpSalary.salMonth
                     && x.SalYear == regEmpSalary.salYear && x.EmployeeTypeID == regEmpSalary.selectedEmployeeTypeID.Value
                   ))
                    {
                        regEmpSalary.CustomErrorFound = true;
                        regEmpSalary.CustomErrorMsg = $"Cannot send approval request, because salary has been already approved for the selected period.";
                    }

                    else if (noOfAllBranches == generatedSalaryBranchCount)
                    {
                        DTOModel.PayrollApprovalRequest request = new DTOModel.PayrollApprovalRequest();
                        request.ProcessID = (int)WorkFlowProcess.SalaryGenerate;
                        request.CreatedOn = DateTime.Now;
                        request.CreatedBy = regEmpSalary.loggedInUserID;
                        request.EmployeeTypeID = regEmpSalary.selectedEmployeeTypeID.Value;
                        request.BranchCode = "All";
                        request.Status = status;
                        request.Period = period;

                        genericRepo.Insert<DTOModel.PayrollApprovalRequest>(request);
                        if (request.ApprovalRequestID > 0)
                            regEmpSalary.ApprovalRequestSent = true;
                    }

                    else
                    {
                        regEmpSalary.CustomErrorFound = true;
                        regEmpSalary.CustomErrorMsg = $"Cannot send approval request, because salary has't been processed for the selected period.";
                    }
                }
                else if (regEmpSalary.selectedBranchID.HasValue)
                {
                    var branchCode = genericRepo.GetByID<DTOModel.Branch>(regEmpSalary.selectedBranchID.Value).BranchCode;

                    if (genericRepo.Exists<DTOModel.PayrollApprovalRequest>(x => x.Period == period
                         && x.EmployeeTypeID == regEmpSalary.selectedEmployeeTypeID.Value
                         && x.ProcessID == (int)WorkFlowProcess.SalaryGenerate && x.BranchID == regEmpSalary.selectedBranchID.Value && (x.Status != 2 && x.Status != 4)))
                    {
                        regEmpSalary.CustomErrorFound = true;
                        regEmpSalary.CustomErrorMsg = $"Approval request already exists.";
                    }

                    else if (genericRepo.Exists<DTOModel.tblFinalMonthlySalary>(x => x.BranchCode == branchCode
                       && !x.Publish && x.SalMonth == regEmpSalary.salMonth && x.SalYear == regEmpSalary.salYear
                       && x.tblMstEmployee.EmployeeTypeID == regEmpSalary.selectedEmployeeTypeID.Value))
                    {
                        DTOModel.PayrollApprovalRequest request = new DTOModel.PayrollApprovalRequest();
                        request.ProcessID = (int)WorkFlowProcess.SalaryGenerate;
                        request.CreatedOn = DateTime.Now;
                        request.CreatedBy = regEmpSalary.loggedInUserID;
                        request.EmployeeTypeID = regEmpSalary.selectedEmployeeTypeID.Value;
                        request.Status = status;
                        request.BranchID = regEmpSalary.selectedBranchID.Value;
                        request.Period = period;
                        request.BranchCode = branchCode;
                        genericRepo.Insert<DTOModel.PayrollApprovalRequest>(request);
                        if (request.ApprovalRequestID > 0)
                            regEmpSalary.ApprovalRequestSent = true;
                    }

                    else if (genericRepo.Exists<DTOModel.tblFinalMonthlySalary>(x => x.Publish && x.BranchCode == branchCode
                       && x.SalMonth == regEmpSalary.salMonth && x.SalYear == regEmpSalary.salYear
                       && x.tblMstEmployee.EmployeeTypeID == regEmpSalary.selectedEmployeeTypeID.Value))
                    {
                        regEmpSalary.CustomErrorFound = true;
                        regEmpSalary.CustomErrorMsg = $"Cannot send approval request, because salary has been already approved for the selected period.";

                    }

                    else
                    {
                        regEmpSalary.CustomErrorFound = true;
                        regEmpSalary.CustomErrorMsg = $"Cannot send approval request, because salary has't been processed for the selected period.";
                    }
                }
                else if (!regEmpSalary.BranchesExcecptHO && !regEmpSalary.selectedBranchID.HasValue)  //==== Case of single employee 
                {

                    if (genericRepo.Exists<DTOModel.tblFinalMonthlySalary>(x =>
                     x.Publish && x.EmployeeID == regEmpSalary.selectedEmployeeID.Value
                     && x.SalMonth == regEmpSalary.salMonth && x.SalYear == regEmpSalary.salYear))
                    {
                        regEmpSalary.CustomErrorFound = true;
                        regEmpSalary.CustomErrorMsg = $"Cannot send approval request, because salary has't been processed for the selected period.";
                    }

                    else if (genericRepo.Exists<DTOModel.tblFinalMonthlySalary>(x => x.EmployeeID == regEmpSalary.selectedEmployeeID.Value
                    && !x.Publish && x.SalMonth == regEmpSalary.salMonth && x.SalYear == regEmpSalary.salYear))
                    {
                        DTOModel.PayrollApprovalRequest request = new DTOModel.PayrollApprovalRequest();
                        request.ProcessID = (int)WorkFlowProcess.SalaryGenerate;
                        request.CreatedOn = DateTime.Now;
                        request.CreatedBy = regEmpSalary.loggedInUserID;
                        request.EmployeeTypeID = regEmpSalary.selectedEmployeeTypeID.Value;
                        request.Status = status;
                        request.Period = period;

                        genericRepo.Insert<DTOModel.PayrollApprovalRequest>(request);
                        if (request.ApprovalRequestID > 0)
                            regEmpSalary.ApprovalRequestSent = true;
                    }

                    else
                    {
                        regEmpSalary.CustomErrorFound = true;
                        regEmpSalary.CustomErrorMsg = $"Cannot send approval request, because salary has't been processed for the selected period.";
                    }
                }
                if (regEmpSalary.ApprovalRequestSent)
                {
                    PayrollApprovalRequest request = new PayrollApprovalRequest();



                    request.Reporting1 = reportingPersonEmails.Reporting1;
                    request.Reporting2 = reportingPersonEmails.Reporting2;
                    request.Reporting3 = reportingPersonEmails.Reporting3;
                    request.Period = period;
                    request.Status = (byte)(reportingPersonEmails != null && reportingPersonEmails.Reporting2.HasValue ? (int)ApprovalStatus.RequestedByReporting1 : (int)ApprovalStatus.ApprovedByReporting2);
                    var employee = genericRepo.GetByID<DTOModel.tblMstEmployee>((reportingPersonEmails.Reporting2.HasValue ? request.Reporting2 : request.Reporting3));
                    request.emailTo = employee.OfficialEmail;

                    if (!request.BranchID.HasValue)
                        request.BranchName = request.BranchCode == "Except-HO" ? "All Branches (Except HO)" : "All Branches";

                    Task t1 = Task.Run(() => SendApprovalEmail(request));
                }

                return regEmpSalary;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendApprovalEmail(PayrollApprovalRequest request)
        {
            log.Info($"PayrollApprovalSettingService/SendApprovalEmail");

            try
            {
                EmailMessage emailMessage = new EmailMessage();
                StringBuilder mailBody = new StringBuilder();

                var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmailConfiguration, EmailMessage>()
                    .ForMember(d => d.From, o => o.MapFrom(s => $"NAFED HRMS <{s.ToEmail}>"))
                    .ForMember(d => d.To, o => o.UseValue(request.emailTo))
                    .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
                    .ForMember(d => d.Password, o => o.MapFrom(s => s.Password))
                    .ForMember(d => d.SmtpClientHost, o => o.MapFrom(s => s.Server))
                    .ForMember(d => d.SmtpPort, o => o.MapFrom(s => s.Port))
                    .ForMember(d => d.enableSSL, o => o.MapFrom(s => s.SSLStatus))
                    .ForMember(d => d.HTMLView, o => o.UseValue(true))
                    .ForMember(d => d.FriendlyName, o => o.UseValue("NAFED"));
                });

                emailMessage = Mapper.Map<EmailMessage>(emailsetting);

                if (request.Status == (int)ApprovalStatus.RequestedByReporting1)
                {
                    emailMessage.Subject = $"REVIEW & APPROVAL OF SALARY BILL";
                    mailBody.Clear();
                    mailBody.AppendFormat("<div>Dear Sir/Ma'am,</div> <br>");
                    mailBody.AppendFormat($"<div>Salary bill for the month of <b>{request.periodInDateFormat.Value.ToString("MMM, yyyy")} <b> towards branch : <b> {request.BranchName} </b> has been prepared.the same may please be reviewed.<br>");

                    mailBody.AppendFormat($"<div>Regards, </div> <br>");
                    mailBody.AppendFormat($"<div>F & A Team,  </div> <br>");
                    mailBody.AppendFormat($"<div>Nafed  </div> <br> <br>");
                }

                emailMessage.Body = mailBody.ToString();
                EmailHelper.SendEmail(emailMessage);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public RegularEmployeeSalary RevertProcessedLoanEntries(RegularEmployeeSalary regEmpSalary)
        {
            log.Info("GenerateSalaryService/RevertProcessedLoanEntries");
            try
            {
                var period = $"{regEmpSalary.salYear.ToString()}{ regEmpSalary.salMonth.ToString("00")}";

                regEmpSalary.selectedEmployeeTypeID = regEmpSalary.selectedEmployeeTypeID ?? 0;
                regEmpSalary.BranchesExcecptHO = regEmpSalary.enumBranch == EnumBranch.BranchesExcecptHO ? true : false;
                regEmpSalary.AllBranches = regEmpSalary.enumBranch == EnumBranch.AllBranches ? true : false;
                regEmpSalary.AllEmployees = regEmpSalary.enumEmpCategory == EnumEmpCategory.AllEmployees ? true : false;
                regEmpSalary.selectedBranchID = regEmpSalary.selectedEmployeeID.HasValue ? null : regEmpSalary.selectedBranchID;

                if (regEmpSalary.BranchesExcecptHO)
                {
                    var r = generateSalaryRepo.RevertProcessedLoanEntries(true, false, regEmpSalary.AllEmployees,
                        regEmpSalary.salMonth, regEmpSalary.salYear, regEmpSalary.selectedBranchID, regEmpSalary.selectedEmployeeID,
                        regEmpSalary.selectedEmployeeTypeID.Value);

                    if (r != null && r.Rows.Count > 0)
                    {
                        if (Convert.ToInt16(r.Rows[0][0]) > 0)
                        {
                            regEmpSalary.Reverted = true;
                        }
                        else
                        {
                            regEmpSalary.CustomErrorFound = true;
                            if (!string.IsNullOrEmpty(period))
                            {
                                var year = Convert.ToInt32(period.Substring(0, 4));
                                var month = Convert.ToInt32(period.Substring(4, 2));
                                regEmpSalary.CustomErrorMsg = $"No records were found for the period :{((DateTime)new DateTime(year, month, 1)).ToString("MMM, yyyy")}";
                            }
                        }
                    }
                }
                else if (regEmpSalary.AllBranches)
                {
                    var r = generateSalaryRepo.RevertProcessedLoanEntries(false, true, regEmpSalary.AllEmployees,
                   regEmpSalary.salMonth, regEmpSalary.salYear, regEmpSalary.selectedBranchID, regEmpSalary.selectedEmployeeID,
                   regEmpSalary.selectedEmployeeTypeID.Value);

                    if (r != null && r.Rows.Count > 0)
                    {
                        if (Convert.ToInt16(r.Rows[0][0]) > 0)
                        {
                            regEmpSalary.Reverted = true;
                        }
                        else
                        {
                            regEmpSalary.CustomErrorFound = true;
                            if (!string.IsNullOrEmpty(period))
                            {
                                var year = Convert.ToInt32(period.Substring(0, 4));
                                var month = Convert.ToInt32(period.Substring(4, 2));
                                regEmpSalary.CustomErrorMsg = $"No records were found for the period :{((DateTime)new DateTime(year, month, 1)).ToString("MMM, yyyy")}";
                            }
                        }
                    }
                }
                else if (regEmpSalary.selectedBranchID.HasValue)
                {
                    var r = generateSalaryRepo.RevertProcessedLoanEntries(false, false, regEmpSalary.AllEmployees,
                       regEmpSalary.salMonth, regEmpSalary.salYear, regEmpSalary.selectedBranchID, regEmpSalary.selectedEmployeeID,
                       regEmpSalary.selectedEmployeeTypeID.Value);

                    if (r != null && r.Rows.Count > 0)
                    {
                        if (Convert.ToInt16(r.Rows[0][0]) > 0)
                        {
                            regEmpSalary.Reverted = true;
                        }
                        else
                        {
                            regEmpSalary.CustomErrorFound = true;
                            if (!string.IsNullOrEmpty(period))
                            {
                                var year = Convert.ToInt32(period.Substring(0, 4));
                                var month = Convert.ToInt32(period.Substring(4, 2));
                                regEmpSalary.CustomErrorMsg = $"No records were found for the period {((DateTime)new DateTime(year, month, 1)).ToString("MMM, yyyy")}";
                            }
                        }
                    }
                }
                else if (!regEmpSalary.BranchesExcecptHO && !regEmpSalary.selectedBranchID.HasValue)  //==== Case of single employee 
                {

                }
                return regEmpSalary;
            }
            catch (Exception ex)
            {
                log.Error("Salary Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<FinalMonthlySalary> GeneratedSalaryList(int salYear)
        {
            log.Info($"GenerateSalaryService/GeneratedSalaryList");
            try
            {
                var dtoFms = genericRepo.Get<DTOModel.tblFinalMonthlySalary>(x =>

                x.RecordType.Equals("S", StringComparison.OrdinalIgnoreCase) && x.SalYear == salYear)
                   .GroupBy(x => new { x.SalMonth, x.SalYear, x.EmployeeTypeID })
                   .Select(x => new DTOModel.tblFinalMonthlySalary
                   {
                       SalYear = x.Key.SalYear,
                       SalMonth = x.Key.SalMonth,
                       EmployeeTypeID = x.Key.EmployeeTypeID
                   }).OrderByDescending(y => y.SalYear).OrderByDescending(y => y.SalMonth).ToList();
                ;
                ;
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblFinalMonthlySalary, FinalMonthlySalary>();
                });
                return Mapper.Map<List<FinalMonthlySalary>>(dtoFms);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public FinalMonthlySalary GetEmployeeSalary(int empId, int salYear, int salMonth)
        {
            log.Info($"GenerateSalaryService/GetEmployeeSalary/empId={empId}");
            try
            {
                var dtoFms = genericRepo.Get<DTOModel.tblFinalMonthlySalary>(x => x.EmployeeID == empId && x.SalYear == salYear && x.SalMonth == salMonth && x.C_NetSal < 0).FirstOrDefault()
                ;

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblFinalMonthlySalary, FinalMonthlySalary>();
                });
                return Mapper.Map<FinalMonthlySalary>(dtoFms);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public async Task<bool> UpdateEmployeeSalary(FinalMonthlySalary empSalary)
        {
            log.Info($"GenerateSalaryService/UpdateEmployeeSalary");
            try
            {

                var payPeriod = empSalary.SalYear + (empSalary.SalMonth.ToString().PadLeft(2, '0'));

                var empLoans = genericRepo.Get<DTOModel.tblLoanTran>(x => x.EmployeeCode == empSalary.EmployeeCode && x.PeriodOfPayment == payPeriod).ToList();
                if (empLoans != null && empLoans.Count > 0)
                {
                    foreach (var item in empLoans)
                    {
                        if (item.LoanType != "D_15" && item.LoanType != "D_18")
                        {
                            if (item.CurrentIInstNoPaid != 0 && item.CurrentPInstNoPaid != 0)
                            {
                                var empLoanOthr = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.PriorityNo == item.PriorityNo && x.IsNewLoanAfterDevelop == true && x.SerialNo == item.SerialNo).FirstOrDefault();
                                if (empLoanOthr != null)
                                {
                                    empLoanOthr.BalancePAmt = empLoanOthr.BalancePAmt + empLoanOthr.LastPaidPInstAmt;
                                    empLoanOthr.BalanceIAmt = empLoanOthr.BalanceIAmt + empLoanOthr.LastPaidIInstAmt;
                                    empLoanOthr.TotalBalanceAmt = empLoanOthr.TotalBalanceAmt + empLoanOthr.LastPaidIInstAmt;
                                    empLoanOthr.RemainingPInstNo = 0;
                                    empLoanOthr.RemainingIInstNo = empLoanOthr.RemainingIInstNo + 1;
                                    empLoanOthr.LastInstAmt = empLoanOthr.LastInstAmt + empLoanOthr.LastMonthInterest;
                                    empLoanOthr.LastPAmt = empLoanOthr.LastInstAmt;
                                    empLoanOthr.TotalSkippedInst = empLoanOthr.TotalSkippedInst + 1;
                                    empLoanOthr.Status = false;
                                    empLoanOthr.LastPaidPInstAmt = 0;
                                    empLoanOthr.LastPaidIInstAmt = 0;
                                    empLoanOthr.LastPaidIInstNo = empLoanOthr.LastPaidIInstNo - 1;
                                    empLoanOthr.LastPaidPInstNo = 0;

                                    genericRepo.Update(empLoanOthr);
                                }

                                var empLoanTrns = genericRepo.Get<DTOModel.tblLoanTran>(x => x.PriorityNo == item.PriorityNo && x.PeriodOfPayment == payPeriod && x.IsNewLoanAfterDevelop == 1 && x.SerialNo == item.SerialNo).FirstOrDefault();
                                if (empLoanTrns != null)
                                {
                                    empLoanTrns.SkippedInstNo = empLoanTrns.SkippedInstNo + 1;
                                    empLoanTrns.RemainingPAmt = empLoanTrns.RemainingPAmt + empLoanTrns.CurrentPInstAmtPaid;
                                    empLoanTrns.RemainingIAmt = empLoanTrns.RemainingIAmt + empLoanTrns.CurrentIInstAmtPaid;
                                    empLoanTrns.CurrentPInstNoPaid = 0;
                                    empLoanTrns.CurrentPInstAmtPaid = 0;
                                    empLoanTrns.CurrentIInstNoPaid = empLoanTrns.CurrentIInstNoPaid - 1;
                                    empLoanTrns.CurrentIInstAmtPaid = 0;
                                }
                            }
                            else
                            {
                                var empLoanOthr = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.PriorityNo == item.PriorityNo && x.IsNewLoanAfterDevelop == true && x.SerialNo == item.SerialNo).FirstOrDefault();
                                if (empLoanOthr != null)
                                {
                                    empLoanOthr.BalancePAmt = empLoanOthr.BalancePAmt + empLoanOthr.LastPaidPInstAmt;
                                    empLoanOthr.BalanceIAmt = empLoanOthr.BalanceIAmt + empLoanOthr.LastMonthInterest;
                                    empLoanOthr.TotalBalanceAmt = empLoanOthr.TotalBalanceAmt + empLoanOthr.LastPaidPInstAmt;
                                    empLoanOthr.RemainingPInstNo = empLoanOthr.RemainingPInstNo = empLoanOthr.RemainingPInstNo + 1;
                                    empLoanOthr.RemainingIInstNo = 0;
                                    empLoanOthr.LastInstAmt = 0;
                                    empLoanOthr.LastPAmt = empLoanOthr.LastInstAmt;
                                    empLoanOthr.TotalSkippedInst = empLoanOthr.TotalSkippedInst + 1;
                                    empLoanOthr.Status = false;
                                    empLoanOthr.LastPaidPInstAmt = 0;
                                    empLoanOthr.LastPaidIInstAmt = 0;
                                    empLoanOthr.LastPaidIInstNo = empLoanOthr.LastPaidIInstNo - 1;
                                    empLoanOthr.LastPaidPInstNo = 0;
                                    genericRepo.Update(empLoanOthr);
                                }

                                var empLoanTrns = genericRepo.Get<DTOModel.tblLoanTran>(x => x.PriorityNo == item.PriorityNo && x.PeriodOfPayment == payPeriod && x.IsNewLoanAfterDevelop == 1 && x.SerialNo == item.SerialNo).FirstOrDefault();
                                if (empLoanTrns != null)
                                {
                                    empLoanTrns.SkippedInstNo = empLoanTrns.SkippedInstNo + 1;
                                    empLoanTrns.RemainingPAmt = empLoanTrns.RemainingPAmt + empLoanTrns.CurrentPInstAmtPaid;
                                    empLoanTrns.RemainingIAmt = 0;
                                    empLoanTrns.CurrentPInstNoPaid = empLoanTrns.CurrentPInstNoPaid - 1;
                                    empLoanTrns.CurrentPInstAmtPaid = 0;
                                    empLoanTrns.CurrentIInstNoPaid = 0;
                                    empLoanTrns.CurrentIInstAmtPaid = 0;
                                }

                            }
                        }
                        else if (item.LoanType == "D_15")
                        {
                            #region ---for Loan Type D_15-----
                            var getEmpLPr_15 = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.PriorityNo == item.PriorityNo && x.IsNewLoanAfterDevelop == true && x.SerialNo == item.SerialNo).FirstOrDefault();
                            if (getEmpLPr_15 != null)
                            {
                                getEmpLPr_15.BalancePAmt = getEmpLPr_15.BalancePAmt + getEmpLPr_15.LastPaidPInstAmt;
                                getEmpLPr_15.BalanceIAmt = getEmpLPr_15.BalanceIAmt + getEmpLPr_15.LastPaidIInstAmt;
                                getEmpLPr_15.TotalBalanceAmt = getEmpLPr_15.TotalBalanceAmt + getEmpLPr_15.LastPaidPInstAmt + getEmpLPr_15.LastPaidIInstAmt;
                                getEmpLPr_15.RemainingPInstNo = getEmpLPr_15.RemainingPInstNo + 1;
                                getEmpLPr_15.RemainingIInstNo = 0;
                                getEmpLPr_15.LastInstAmt = getEmpLPr_15.LastInstAmt + ((getEmpLPr_15.CurrentROI * (getEmpLPr_15.LastPaidPInstAmt + 1) * (getEmpLPr_15.BalancePAmt + getEmpLPr_15.LastPaidPInstAmt)) / 1200);
                                getEmpLPr_15.LastMonthInterest = ((getEmpLPr_15.CurrentROI * (getEmpLPr_15.LastPaidPInstAmt + 1) * (getEmpLPr_15.BalancePAmt + getEmpLPr_15.LastPaidPInstAmt)) / 1200);
                                getEmpLPr_15.LastPAmt = getEmpLPr_15.LastInstAmt + ((getEmpLPr_15.CurrentROI * (getEmpLPr_15.LastPaidPInstAmt + 1) * (getEmpLPr_15.BalancePAmt + getEmpLPr_15.LastPaidPInstAmt)) / 1200);
                                getEmpLPr_15.TotalSkippedInst = getEmpLPr_15.TotalSkippedInst + 1;
                                getEmpLPr_15.Status = false;
                                getEmpLPr_15.LastPaidPInstAmt = 0;
                                getEmpLPr_15.LastPaidIInstAmt = 0;
                                getEmpLPr_15.LastPaidPInstNo = getEmpLPr_15.LastPaidPInstNo - 1;
                                getEmpLPr_15.LastPaidIInstNo = getEmpLPr_15.LastPaidIInstNo - 1;
                                genericRepo.Update(getEmpLPr_15);
                            }
                            var loanTran_15 = genericRepo.Get<DTOModel.tblLoanTran>(x => x.PeriodOfPayment == payPeriod && x.SerialNo == item.SerialNo && x.LoanType == "D_15").FirstOrDefault();
                            if (loanTran_15 != null)
                            {

                                var getEmpLn_15 = genericRepo.Get<DTOModel.tblLoanTran>(x => x.PriorityNo == item.PriorityNo && x.IsNewLoanAfterDevelop == 1 && x.PeriodOfPayment == payPeriod && x.SerialNo == item.SerialNo).FirstOrDefault();
                                if (getEmpLn_15 != null)
                                {
                                    getEmpLn_15.SkippedInstNo = getEmpLn_15.SkippedInstNo = getEmpLn_15.SkippedInstNo + 1;
                                    getEmpLn_15.RemainingPAmt = loanTran_15.RemainingPAmt;
                                    getEmpLn_15.RemainingIAmt = loanTran_15.RemainingIAmt;
                                    getEmpLn_15.CurrentPInstNoPaid = getEmpLn_15.CurrentPInstNoPaid - 1;
                                    getEmpLn_15.CurrentInterestAmt = 0;
                                    getEmpLn_15.CurrentPInstAmtPaid = 0;
                                    getEmpLn_15.CurrentIInstNoPaid = getEmpLn_15.CurrentIInstNoPaid - 1;
                                    getEmpLn_15.CurrentIInstAmtPaid = 0;
                                    getEmpLn_15.CLOSINGBALANCE = loanTran_15.CLOSINGBALANCE;
                                    genericRepo.Update(getEmpLn_15);
                                }
                            }
                            #endregion
                        }
                        else if (item.LoanType == "D_18")
                        {
                            #region ---for Loan Type D_18-----
                            var getEmpLPr_18 = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.PriorityNo == item.PriorityNo && x.IsNewLoanAfterDevelop == true && x.SerialNo == item.SerialNo).FirstOrDefault();
                            if (getEmpLPr_18 != null)
                            {
                                getEmpLPr_18.BalancePAmt = (getEmpLPr_18.BalancePAmt + getEmpLPr_18.LastPaidPInstAmt);
                                getEmpLPr_18.BalanceIAmt = 0;
                                getEmpLPr_18.TotalBalanceAmt = (getEmpLPr_18.TotalBalanceAmt + getEmpLPr_18.LastPaidPInstAmt);
                                getEmpLPr_18.RemainingPInstNo = (getEmpLPr_18.RemainingPInstNo + 1);
                                getEmpLPr_18.RemainingIInstNo = 0;
                                getEmpLPr_18.LastInstAmt = (getEmpLPr_18.LastInstAmt + empSalary.D_18_A);
                                getEmpLPr_18.LastPAmt = getEmpLPr_18.LastInstAmt;
                                getEmpLPr_18.TotalSkippedInst = getEmpLPr_18.TotalSkippedInst + 1;
                                getEmpLPr_18.Status = false;
                                getEmpLPr_18.LastPaidPInstAmt = 0;
                                getEmpLPr_18.LastPaidIInstAmt = 0;
                                getEmpLPr_18.LastPaidPInstNo = getEmpLPr_18.LastPaidPInstNo - 1;
                                getEmpLPr_18.LastPaidIInstNo = 0;
                                genericRepo.Update(getEmpLPr_18);
                            }
                            var getEmpLn_18 = genericRepo.Get<DTOModel.tblLoanTran>(x => x.PriorityNo == item.PriorityNo && x.PeriodOfPayment == payPeriod && x.IsNewLoanAfterDevelop == 1 && x.SerialNo == item.SerialNo).FirstOrDefault();
                            if (getEmpLn_18 != null)
                            {
                                getEmpLn_18.SkippedInstNo = (getEmpLn_18.SkippedInstNo + 1);
                                getEmpLn_18.RemainingPAmt = (getEmpLn_18.RemainingPAmt + getEmpLn_18.CurrentPInstAmtPaid);
                                getEmpLn_18.RemainingIAmt = 0;
                                getEmpLn_18.CurrentPInstNoPaid = (getEmpLn_18.CurrentPInstNoPaid - 1);
                                getEmpLn_18.CurrentPInstAmtPaid = 0;
                                getEmpLn_18.CurrentIInstNoPaid = 0;
                                getEmpLn_18.CurrentIInstAmtPaid = 0;
                                genericRepo.Update(getEmpLn_18);
                            }
                            #endregion
                        }

                    }
                }
                var dtoEmpSal = genericRepo.Get<DTOModel.tblFinalMonthlySalary>(x => x.EmployeeID == empSalary.EmployeeID && x.SalYear == empSalary.SalYear && x.SalMonth == empSalary.SalMonth && x.C_NetSal < 0).FirstOrDefault();
                if (dtoEmpSal != null)
                {
                    dtoEmpSal.D_PF_A = empSalary.D_PF_A;
                    dtoEmpSal.D_VPF_A = empSalary.D_VPF_A;
                    dtoEmpSal.D_01_A = empSalary.D_01_A;
                    dtoEmpSal.D_02_A = empSalary.D_02_A;
                    dtoEmpSal.D_03_A = empSalary.D_03_A;
                    dtoEmpSal.D_04_A = empSalary.D_04_A;
                    dtoEmpSal.D_05_A = empSalary.D_05_A;
                    dtoEmpSal.D_06_A = empSalary.D_06_A;
                    dtoEmpSal.D_07_A = empSalary.D_07_A;
                    dtoEmpSal.D_08_A = empSalary.D_08_A;
                    dtoEmpSal.D_09_A = empSalary.D_09_A;
                    dtoEmpSal.D_10_A = empSalary.D_10_A;
                    dtoEmpSal.D_11_A = empSalary.D_11_A;
                    dtoEmpSal.D_12_A = empSalary.D_12_A;
                    dtoEmpSal.D_13_A = empSalary.D_13_A;
                    dtoEmpSal.D_14_A = empSalary.D_14_A;
                    dtoEmpSal.D_15_A = empSalary.D_15_A;
                    dtoEmpSal.D_16_A = empSalary.D_16_A;
                    dtoEmpSal.D_17_A = empSalary.D_17_A;
                    dtoEmpSal.D_18_A = empSalary.D_18_A;
                    dtoEmpSal.D_19_A = empSalary.D_19_A;
                    dtoEmpSal.D_20_A = empSalary.D_20_A;
                    dtoEmpSal.D_21_A = empSalary.D_21_A;
                    dtoEmpSal.D_22_A = empSalary.D_22_A;
                    dtoEmpSal.D_23_A = empSalary.D_23_A;
                    dtoEmpSal.D_24_A = empSalary.D_24_A;
                    dtoEmpSal.D_25_A = empSalary.D_25_A;
                    dtoEmpSal.D_26_A = empSalary.D_26_A;
                    dtoEmpSal.D_27_A = empSalary.D_27_A;
                    dtoEmpSal.D_28_A = empSalary.D_28_A;
                    dtoEmpSal.D_29_A = empSalary.D_29_A;
                    dtoEmpSal.D_30_A = empSalary.D_30_A;
                    dtoEmpSal.C_TotEarn = empSalary.C_TotEarn;
                    dtoEmpSal.C_TotDedu = empSalary.C_TotDedu;
                    dtoEmpSal.C_NetSal = empSalary.C_NetSal;
                    dtoEmpSal.chkNegative = false;
                    dtoEmpSal.UpdatedOn = DateTime.Now;
                    genericRepo.Update(dtoEmpSal);
                }
                var getEmpPF = genericRepo.Get<DTOModel.tblPFOpBalance>(x => x.EmployeeID == empSalary.EmployeeID && x.Salyear == empSalary.SalYear && x.Salmonth == empSalary.SalMonth.ToString()).FirstOrDefault();
                if (getEmpPF != null)
                {
                    getEmpPF.TotalPFBalance = (getEmpPF.TotalPFBalance - getEmpPF.EmployeePFCont - getEmpPF.VPF - getEmpPF.EmployerPFCont - getEmpPF.InterestTotal);
                    getEmpPF.TotalPFOpeningEmpl = getEmpPF.EmplOpBal;
                    getEmpPF.TotalPFOpeningEmplr = getEmpPF.EmplrOpBal;
                    getEmpPF.EmployeePFCont = 0;
                    getEmpPF.VPF = 0;
                    getEmpPF.Pension = 0;
                    getEmpPF.EmployerPFCont = 0;
                    genericRepo.Update(getEmpPF);
                }
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void UpdateDateBranch_Pay(int salMonth, int salYear)
        {
            try
            {
                arrearRepository.UpdateDateBranch_Pay(salMonth, salYear);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;

            }

        }
    }

}
