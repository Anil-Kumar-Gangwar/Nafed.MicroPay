using System;
using System.Collections.Generic;
using System.Linq;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
using System.IO;
using System.Data;
using static Nafed.MicroPay.ImportExport.PfBalanceManualEntryForm;
using Nafed.MicroPay.Data.Repositories;

namespace Nafed.MicroPay.Services
{
    public class PFAccountBalanceServices : BaseService, IPFAccountBalanceServices
    {
        private readonly IGenericRepository genericRepo;
        private readonly IPFAccountBalanceRepository pfAcBalRepo;
        public PFAccountBalanceServices(IGenericRepository genericRepo, IPFAccountBalanceRepository pfAcBalRepo)
        {
            this.genericRepo = genericRepo;
            this.pfAcBalRepo = pfAcBalRepo;
        }
        public EmpPFOpBalance GetEmployeePFDetail(int employeeID, int month, int year)
        {
            log.Info($"PFAccountBalanceServices/GetEmployeePFDetail");

            try
            {
                var getpfDtl = genericRepo.Get<DTOModel.tblPFOpBalance>(x => x.EmployeeID == employeeID && x.Salmonth == month.ToString() && x.Salyear == year).FirstOrDefault();
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.tblPFOpBalance, EmpPFOpBalance>()
                .ForMember(d => d.EmplOpBal, o => o.MapFrom(s => s.EmplOpBal))
                .ForMember(d => d.EmplrOpBal, o => o.MapFrom(s => s.EmplrOpBal))
                .ForMember(d => d.PFAcNo, o => o.MapFrom(s => s.PFAcNo))
                .ForMember(d => d.EmployeeInterest, o => o.MapFrom(s => s.EmployeeInterest))
                .ForMember(d => d.EmployerInterest, o => o.MapFrom(s => s.EmployerInterest))
                .ForMember(d => d.NonRefundLoan, o => o.MapFrom(s => s.NonRefundLoan))
                .ForMember(d => d.InterestNRPFLoan, o => o.MapFrom(s => s.InterestNRPFLoan))
                .ForMember(d => d.InterestRate, o => o.MapFrom(s => s.InterestRate))
                .ForMember(d => d.InterestTotal, o => o.MapFrom(s => s.InterestTotal))
                .ForMember(d => d.TotalPFBalance, o => o.MapFrom(s => s.TotalPFBalance))
                .ForMember(d => d.TotalPFOpeningEmpl, o => o.MapFrom(s => s.TotalPFOpeningEmpl))
                .ForMember(d => d.TotalPFOpeningEmplr, o => o.MapFrom(s => s.TotalPFOpeningEmplr))
                .ForMember(d => d.WithdrawlEmployeeAc, o => o.MapFrom(s => s.WithdrawlEmployeeAc))
                .ForMember(d => d.WithdrawlEmployerAc, o => o.MapFrom(s => s.WithdrawlEmployerAc))
                .ForMember(d => d.AdditionEmployeeAc, o => o.MapFrom(s => s.AdditionEmployeeAc))
                .ForMember(d => d.AdditionEmployerAc, o => o.MapFrom(s => s.AdditionEmployerAc))
                .ForMember(d => d.IntWDempl, o => o.MapFrom(s => s.IntWDempl))
                .ForMember(d => d.IntWDemplr, o => o.MapFrom(s => s.IntWDemplr))
                .ForMember(d => d.EmployeePFCont, o => o.MapFrom(s => s.EmployeePFCont))
                .ForMember(d => d.EmployerPFCont, o => o.MapFrom(s => s.EmployerPFCont))
                .ForAllOtherMembers(d => d.Ignore())
                );

                var dtoPFDtl = Mapper.Map<EmpPFOpBalance>(getpfDtl);
                if (dtoPFDtl != null)
                    return dtoPFDtl;
                else
                    return new EmpPFOpBalance(); ;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertPFDetail(EmpPFOpBalance empPFBal)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<EmpPFOpBalance, DTOModel.TblPFOPBalanceManual>()
                .ForMember(d => d.Year, o => o.MapFrom(s => s.Salyear))
                .ForMember(d => d.Month, o => o.MapFrom(s => s.Salmonth))
                .ForMember(d => d.PFNo, o => o.MapFrom(s => s.PFAcNo))
                .ForMember(d => d.EPF, o => o.MapFrom(s => s.EmployeePFCont))
                .ForMember(d => d.VPF, o => o.MapFrom(s => s.VPF))
                .ForMember(d => d.EmpPF, o => o.MapFrom(s => s.EmployerPFCont))
                .ForMember(d => d.Pension, o => o.MapFrom(s => s.Pension))
                .ForMember(d => d.WithEmp, o => o.MapFrom(s => s.WithdrawlEmployeeAc ?? 0))
                .ForMember(d => d.WithEmplr, o => o.MapFrom(s => s.WithdrawlEmployerAc ?? 0))
                .ForMember(d => d.AddEmp, o => o.MapFrom(s => s.AdditionEmployeeAc ?? 0))
                .ForMember(d => d.AddEmplr, o => o.MapFrom(s => s.AdditionEmployerAc ?? 0))
                .ForMember(d => d.IntEmp, o => o.MapFrom(s => s.IntWDempl ?? 0))
                .ForMember(d => d.IntEmplr, o => o.MapFrom(s => s.IntWDemplr ?? 0))
                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                .ForAllOtherMembers(d => d.Ignore());
            });

            var dtoins = Mapper.Map<DTOModel.TblPFOPBalanceManual>(empPFBal);
            genericRepo.Insert(dtoins);
            return true;
        }


        public EmpPFOpBalance SearchEmployeePFDetail(int pfNo, int month, int year)
        {
            log.Info($"PFAccountBalanceServices/SearchEmployeePFDetail");

            try
            {
                var getpfDtl = genericRepo.Get<DTOModel.tblPFOpBalance>(x => x.PFAcNo == pfNo && x.Salyear == year && x.Salmonth == month.ToString()).FirstOrDefault();
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.tblPFOpBalance, EmpPFOpBalance>()
                .ForMember(d => d.EmplOpBal, o => o.MapFrom(s => s.EmplOpBal))
                .ForMember(d => d.EmplrOpBal, o => o.MapFrom(s => s.EmplrOpBal))
                .ForMember(d => d.PFAcNo, o => o.MapFrom(s => s.PFAcNo))
                .ForMember(d => d.EmployeeInterest, o => o.MapFrom(s => s.EmployeeInterest))
                .ForMember(d => d.EmployerInterest, o => o.MapFrom(s => s.EmployerInterest))
                .ForMember(d => d.NonRefundLoan, o => o.MapFrom(s => s.NonRefundLoan))
                .ForMember(d => d.InterestNRPFLoan, o => o.MapFrom(s => s.InterestNRPFLoan))
                .ForMember(d => d.InterestRate, o => o.MapFrom(s => s.InterestRate))
                .ForMember(d => d.InterestTotal, o => o.MapFrom(s => s.InterestTotal))
                .ForMember(d => d.TotalPFBalance, o => o.MapFrom(s => s.TotalPFBalance))
                .ForMember(d => d.TotalPFOpeningEmpl, o => o.MapFrom(s => s.TotalPFOpeningEmpl))
                .ForMember(d => d.TotalPFOpeningEmplr, o => o.MapFrom(s => s.TotalPFOpeningEmplr))
                .ForMember(d => d.WithdrawlEmployeeAc, o => o.MapFrom(s => s.WithdrawlEmployeeAc))
                .ForMember(d => d.WithdrawlEmployerAc, o => o.MapFrom(s => s.WithdrawlEmployerAc))
                .ForMember(d => d.AdditionEmployeeAc, o => o.MapFrom(s => s.AdditionEmployeeAc))
                .ForMember(d => d.AdditionEmployerAc, o => o.MapFrom(s => s.AdditionEmployerAc))
                .ForMember(d => d.IntWDempl, o => o.MapFrom(s => s.IntWDempl))
                .ForMember(d => d.IntWDemplr, o => o.MapFrom(s => s.IntWDemplr))
                .ForMember(d => d.EmployeePFCont, o => o.MapFrom(s => s.EmployeePFCont))
                .ForMember(d => d.EmployerPFCont, o => o.MapFrom(s => s.EmployerPFCont))
                .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                .ForAllOtherMembers(d => d.Ignore())
                );

                var dtoPFDtl = Mapper.Map<EmpPFOpBalance>(getpfDtl);
                if (dtoPFDtl != null)
                    return dtoPFDtl;
                else
                    return new EmpPFOpBalance();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public string GetPfBalanceManualEntryForm(int branchID, byte month, int year, string fileName, string sFullPath)
        {
            log.Info($"PFAccountBalanceServices/GetPfBalanceManualEntryForm/{branchID}");

            string result = string.Empty;
            if (Directory.Exists(sFullPath))
            {
                sFullPath = $"{sFullPath}{fileName}";
                var empDTO = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.BranchID == branchID && !x.IsDeleted);

                DataTable dtData = new DataTable();

                dtData = pfAcBalRepo.GetPfBalanceManualEntryFormData(branchID, month, year);

                if (dtData != null && dtData.Rows.Count > 0)  //====== export attendance form if there is data ========= 
                {
                    //  dtData.Columns[0].ColumnName = "#";

                    IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                    exportHdr = dtData.Columns.Cast<DataColumn>().Select(x => x.ColumnName).AsEnumerable<string>();

                    result = ExportToExcel(exportHdr, dtData, "Pf Balance Manual Entry Form", sFullPath);
                }
                else
                    result = "norec";
            }
            return result;
        }
        public int ImportPfBalanceManualData(int userID, List<ImportPfBalanceData> dataList)
        {
            log.Info($"PFAccountBalanceServices/ImportPfBalanceManualData");

            try
            {
                DataTable table = new DataTable();
                table.Columns.Add("Year", typeof(int));
                table.Columns.Add("Month", typeof(int));
                table.Columns.Add("PFNo", typeof(string));
                table.Columns.Add("EPF", typeof(decimal));
                table.Columns.Add("VPF", typeof(decimal));
                table.Columns.Add("EmpPF", typeof(decimal));
                table.Columns.Add("Pension", typeof(decimal));
                table.Columns.Add("WithEmp", typeof(decimal));
                table.Columns.Add("WithEmplr", typeof(decimal));
                table.Columns.Add("AddEmp", typeof(decimal));
                table.Columns.Add("AddEmplr", typeof(decimal));
                table.Columns.Add("IntEmp", typeof(decimal));
                table.Columns.Add("IntEmplr", typeof(decimal));
                table.Columns.Add("additionEmpInt", typeof(decimal));
                table.Columns.Add("additionEmployerInt", typeof(decimal));


                foreach (var item in dataList)
                {
                    var row = table.NewRow();
                    row["Year"] = Convert.ToInt32(item.Year);
                    row["Month"] = Convert.ToInt32(item.Month);

                    row["PFNo"] = Convert.ToString(item.PFNo);
                    row["EPF"] = Convert.ToDecimal(item.EmployeePFContribution == "" ? "0" : item.EmployeePFContribution );
                    row["VPF"] = Convert.ToDecimal(item.VPF == "" ? "0" : item.VPF);
                    row["EmpPF"] = Convert.ToDecimal(item.EmployerPFContribution == "" ? "0" : item.EmployerPFContribution); 
                    row["Pension"] = Convert.ToDecimal(item.Pension == "" ? "0" : item.Pension );
                    row["WithEmp"] = Convert.ToDecimal(item.WithdrawlToEmployeeContribution == "" ? "0" : item.WithdrawlToEmployeeContribution );
                    row["WithEmplr"] = Convert.ToDecimal(item.WithdrawlToEmployerContribution == "" ? "0" : item.WithdrawlToEmployerContribution );
                    row["AddEmp"] = Convert.ToDecimal(item.AdditionToEmployeeContribution == "" ? "0" : item.AdditionToEmployeeContribution );
                    row["AddEmplr"] = Convert.ToDecimal(item.AdditionToEmployerContribution == "" ? "0" : item.AdditionToEmployerContribution );
                    row["IntEmp"] = Convert.ToDecimal(item.InterestWithdrawlToEmployeeContribution == "" ? "0" : item.InterestWithdrawlToEmployeeContribution );
                    row["IntEmplr"] = Convert.ToDecimal(item.InterestWithdrawlToEmployerContribution == "" ? "0" : item.InterestWithdrawlToEmployerContribution );
                    row["additionEmpInt"] = Convert.ToDecimal(item.AdditionToEmployeeContribution == "" ? "0" : item.AdditionToEmployeeContribution );
                    row["IntEmplr"] = Convert.ToDecimal(item.AdditionToEmployerContribution == "" ? "0" : item.AdditionToEmployerContribution );

                    table.Rows.Add(row);
                }

                int month = Convert.ToInt32(table.Rows[0]["Month"]);
                int year = Convert.ToInt32(table.Rows[0]["Year"]);

                int res = pfAcBalRepo.ImportPfBalanceManualData(userID, table, month, year);
                return res;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int UpdateInterest(int year, decimal rate)
        {
            int res = pfAcBalRepo.UpdateInterest(year,  rate);
            return res;
        }

    }
}
