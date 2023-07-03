using Nafed.MicroPay.Model;
using System.Data;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using static Nafed.MicroPay.ImportExport.SalaryReportExport;
using System;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
namespace Nafed.MicroPay.Services.Salary
{
    public class SalaryReportService : BaseService, ISalaryReportService
    {
        private readonly ISalaryReportRepository salReportRepo;
        private readonly IGenericRepository genericRepo;

        public SalaryReportService(ISalaryReportRepository salReportRepo, IGenericRepository genericRepo)
        {
            this.salReportRepo = salReportRepo;
            this.genericRepo = genericRepo;
        }

        public string GetMonthlyBranchWiseReport(SalaryReportFilter rFilter)
        {
            log.Info($"SalaryReportService/GetMonthlyBranchWiseReport");

            string result = string.Empty; string sFullPath = string.Empty;
            string selectedPeriod = $"{rFilter.salYear.ToString()}{rFilter.salMonth.ToString("00")}";


            if (genericRepo.Exists<DTOModel.salaryheadshistory>(x => x.EmployeeTypeID == rFilter.employeeTypeID.Value
              && x.Period == selectedPeriod))
            {
                if (Directory.Exists(rFilter.filePath))
                {
                    sFullPath = $"{rFilter.filePath}{rFilter.fileName}";
                    int nE_Cols = 0, nD_Cols = 0;

                    DataSet dsReportData = salReportRepo.GetMonthlyBranchWiseReport(rFilter.salMonth, rFilter.salYear, rFilter.employeeTypeID ?? 0, rFilter.branchID ?? 0, out nE_Cols, out nD_Cols);

                    if (dsReportData != null && dsReportData.Tables[0].Rows.Count > 0)  //====== export report if there is data ========= 
                    {
                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                        exportHdr = dsReportData.Tables[0].Columns.Cast<DataColumn>().Select(x => x.ColumnName).AsEnumerable<string>();


                        dsReportData.Tables[0].ImportRow(dsReportData.Tables[1].Rows[0]);
                        var rowCount = dsReportData.Tables[0].Rows.Count;
                        result = MonthlyBranchWiseExportToExcel(rFilter.salYear, rFilter.salMonth, exportHdr, dsReportData.Tables[0], nE_Cols, nD_Cols, $"Monthly Pay Summary-{selectedPeriod}", sFullPath);
                    }
                    else
                        result = "norec";
                }
            }
            else
                result = "notfound";

            return result;
        }

        public string GetMonthlyEmployeeWiseReport(SalaryReportFilter rFilter)
        {
            log.Info($"SalaryReportService/GetMonthlyEmployeeWiseReport");

            string result = string.Empty; string sFullPath = string.Empty;
            string selectedPeriod = $"{rFilter.salYear.ToString()}{rFilter.salMonth.ToString("00")}";

            if (genericRepo.Exists<DTOModel.salaryheadshistory>(x => x.EmployeeTypeID == rFilter.employeeTypeID.Value
             && x.Period == selectedPeriod))
            {
                if (Directory.Exists(rFilter.filePath))
                {
                    sFullPath = $"{rFilter.filePath}{rFilter.fileName}";
                    int nE_Cols = 0, nD_Cols = 0;

                    DataSet dsReportData = salReportRepo.GetMonthlyEmployeeWiseReport(rFilter.salMonth, rFilter.salYear,
                        rFilter.employeeTypeID, rFilter.branchID, out nE_Cols, out nD_Cols);

                    if (dsReportData != null && dsReportData.Tables[0].Rows.Count > 0)  //====== export report if there is data ========= 
                    {
                        DataTable dtBaseData = new DataTable();
                        dtBaseData = dsReportData.Tables[0].Clone();

                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                        exportHdr = dsReportData.Tables[0].Columns.Cast<DataColumn>()
                            .Where(x => (x.ColumnName.ToString() != "BranchCode"
                            //&& x.ColumnName.ToString() != "BranchName")
                            )).Select(x => x.ColumnName).AsEnumerable<string>();

                        //    var branchCode = dsReportData.Tables[0].Rows[0]["BranchCode"].ToString();
                        //     var branchName = dsReportData.Tables[0].Rows[0]["BranchName"].ToString();

                        var distinctBrachCodes = (from row in dsReportData.Tables[0].AsEnumerable()
                                                  select row.Field<string>("BranchCode")).Distinct();

                        foreach (var item in distinctBrachCodes)
                        {
                            var drArray = (from myRow in dsReportData.Tables[0].AsEnumerable()
                                           where myRow.Field<string>("BranchCode") == item
                                           select myRow).ToArray<DataRow>();

                            var drTotArray = (from myRow in dsReportData.Tables[3].AsEnumerable()
                                              where myRow.Field<string>("BranchCode") == item
                                              select myRow).FirstOrDefault();

                            foreach (DataRow dr in drArray)
                                dtBaseData.ImportRow(dr);

                            drTotArray["SNO"] = -99;
                            dtBaseData.ImportRow(drTotArray);
                        }
                        dtBaseData.Columns.Remove("BranchCode");

                        result = MonthlyEmployeeWiseExportToExcel(exportHdr, dtBaseData, dsReportData.Tables[1], dsReportData.Tables[2], nE_Cols, nD_Cols, $"Employee Wise Monthly-{selectedPeriod}", sFullPath);

                    }
                    else
                        result = "norec";
                }
            }
            else
                result = "notfound";
            return result;
        }

        public bool GetEmployeeWiseAnnualReport(SalaryReportFilter rFilter)
        {
            log.Info($"SalaryReportService/GetEmployeeWiseAnnualReport");
            bool flag = false;
            try
            {
                int nE_Cols = 0, nD_Cols = 0;
                string sFullPath = "";
                sFullPath = $"{rFilter.filePath}{rFilter.fileName}";
                DataSet dtEmployeeWiseAnnual = new DataSet();
                DataSet dtReportData = salReportRepo.GetEmployeeWiseAnnualReport(rFilter.financialFrom,
                    rFilter.financialTo, rFilter.employeeTypeID, rFilter.employeeID,
                    rFilter.branchID, out nE_Cols, out nD_Cols);
                if (dtReportData != null && dtReportData.Tables[0].Rows.Count > 0)  //====== export EmployeeWiseAnnual if there is data ========= 
                {
                    bool res = ExportSalaryReportEmployeeWise(dtReportData, sFullPath, nE_Cols, nD_Cols, "EmployeeWiseAnnualReport");
                }
            }
            catch (Exception ex)
            {
                log.Error("Salary Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool GetBranchWiseAnnualReport(SalaryReportFilter rFilter)
        {
            log.Info($"SalaryReportService/GetBranchWiseAnnualReport");
            bool flag = false;
            try
            {
                int nE_Cols = 0, nD_Cols = 0;

                string sFullPath = "";
                sFullPath = $"{rFilter.filePath}{rFilter.fileName}";
                DataSet dtBranchWiseAnnual = new DataSet();
                DataSet dtReportData = salReportRepo.GetBranchWiseAnnualReport(rFilter.financialFrom,
                    rFilter.financialTo, rFilter.employeeTypeID, rFilter.branchID, out nE_Cols, out nD_Cols);
                if (dtReportData != null && dtReportData.Tables[0].Rows.Count > 0)  //====== export EmployeeWiseAnnual if there is data ========= 
                {
                    bool res = ExportSalaryReportBranchWiseAnnual(dtReportData, sFullPath, nE_Cols, nD_Cols, "BranchWiseAnnualReport");
                }

            }
            catch (Exception ex)
            {
                log.Error("Salary Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public string GetMonthlyEmployeeWisePaySlip(SalaryReportFilter rFilter)
        {

            log.Info($"SalaryReportService/GetMonthlyBranchWiseReport");

            string result = string.Empty; string sFullPath = string.Empty;
            string selectedPeriod = $"{rFilter.salYear.ToString()}{rFilter.salMonth.ToString("00")}";

            if (genericRepo.Exists<DTOModel.salaryheadshistory>(x => x.EmployeeTypeID == rFilter.employeeTypeID.Value
              && x.Period == selectedPeriod))
            {
                if (Directory.Exists(rFilter.filePath))
                {
                    sFullPath = $"{rFilter.filePath}{rFilter.fileName}";
                    int nE_Cols = 0, nD_Cols = 0;

                    DataSet dsReportData = salReportRepo.GetMonthlyEmployeeWisePaySlip(rFilter.salMonth, rFilter.salYear, rFilter.employeeTypeID ?? 0, rFilter.branchID ?? 0, out nE_Cols, out nD_Cols);

                    if (dsReportData != null && dsReportData.Tables[0].Rows.Count > 0)  //====== export report if there is data ========= 
                    {
                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                        exportHdr = dsReportData.Tables[0].Columns.Cast<DataColumn>().Select(x => x.ColumnName).AsEnumerable<string>();


                        dsReportData.Tables[0].ImportRow(dsReportData.Tables[1].Rows[0]);
                        var rowCount = dsReportData.Tables[0].Rows.Count;

                        result = MonthlyEmployeeWisePaySlip(rFilter.BranchName, rFilter.salYear, rFilter.salMonth, exportHdr, dsReportData.Tables[0], nE_Cols, nD_Cols, $"Monthly Pay Slip-{selectedPeriod}", sFullPath);

                    }
                    else
                        result = "norec";
                }
            }
            else
                result = "notfound";

            return result;
        }

        public string GetBranchEmployeeWisePaySlip(SalaryReportFilter rFilter)
        {
            log.Info($"SalaryReportService/GetBranchEmployeeWisePaySlip");
            try
            {
                string result = string.Empty; string sFullPath = string.Empty;
                string selectedPeriod = $"{rFilter.salYear.ToString()}{rFilter.salMonth.ToString("00")}";

                if (genericRepo.Exists<DTOModel.salaryheadshistory>(x => x.EmployeeTypeID == rFilter.employeeTypeID.Value
              && x.Period == selectedPeriod))
                {
                    if (Directory.Exists(rFilter.filePath))
                    {
                        sFullPath = $"{rFilter.filePath}{rFilter.fileName}";
                        int nE_Cols = 0, nD_Cols = 0;
                        DataSet dsReportData = salReportRepo.GetBranchEmployeeWisePaySlip(rFilter.salMonth, rFilter.salYear, rFilter.employeeTypeID ?? 0, rFilter.branchID ?? 0, out nE_Cols, out nD_Cols);

                        if (dsReportData != null && dsReportData.Tables[0].Rows.Count > 0)  //====== export report if there is data ========= 
                        {
                            IEnumerable<string> exportHdr_1 = Enumerable.Empty<string>();
                            exportHdr_1 = dsReportData.Tables[0].Columns.Cast<DataColumn>().Select(x => x.ColumnName).AsEnumerable<string>();

                            IEnumerable<string> exportHdr_2 = Enumerable.Empty<string>();
                            exportHdr_2 = dsReportData.Tables[1].Columns.Cast<DataColumn>().Select(x => x.ColumnName).AsEnumerable<string>();

                            result = BranchEmployeeWisePaySlip(rFilter.BranchName, rFilter.salYear, rFilter.salMonth, exportHdr_1, exportHdr_2, dsReportData.Tables[0], dsReportData.Tables[1], nE_Cols, nD_Cols, $"Monthly Pay Slip-{selectedPeriod}", sFullPath);

                        }
                        else
                            result = "norec";
                    }
                }
                else
                    result = "notfound";

                return result;
            }
            catch (Exception ex)
            {
                log.Error("Salary Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        #region SALARY REPORTS OPTION
        public bool ExportEmpDtlWithBankAccount(MasterReports rFilter, string sFullPath, string fileName)
        {
            log.Info($"SalaryReportService/ExportEmpDtlWithBankAccount/{sFullPath}/{fileName}");
            try
            {
                var flag = false;
                if (genericRepo.Exists<DTOModel.tblFinalMonthlySalary>(x => x.EmployeeTypeID == rFilter.EmployeeTypeID.Value && x.SalYear == rFilter.Year && x.SalMonth == rFilter.Month))
                {
                    DataTable dtTable = salReportRepo.GetEmpDtlWithBankAccount(rFilter.Month.Value, rFilter.Year.Value, rFilter.EmployeeTypeID ?? 0, rFilter.BankCode);
                    if (dtTable != null)
                    {
                        if (System.IO.Directory.Exists(sFullPath))
                        {
                            IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                            exportHdr = dtTable.Columns.Cast<System.Data.DataColumn>()
                                .Select(x => x.ColumnName).AsEnumerable<string>();
                            sFullPath = $"{sFullPath}{fileName}";
                            if (rFilter.BankCode == "IC")
                                ExportEmpDtlWithICICIBank(exportHdr, dtTable, fileName, sFullPath, rFilter.Month.Value);
                            else if (rFilter.BankCode == "PS")
                                ExportEmpDtlWithPSBank(exportHdr, dtTable, fileName, sFullPath, rFilter.Month.Value);
                        }
                        flag = true;
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

        public SalarySlipModel GetSalarySlip(SalaryReportFilter rFilter)
        {
            log.Info("SalaryReportService/GetSalarySlip");
            var getdata = salReportRepo.GetSalarySlip(rFilter.salMonth, rFilter.salYear, rFilter.employeeID.Value);

            Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.SP_EmpWiseSalaryReport_Result, SalarySlipModel>()
            );
            var dtoSlip = Mapper.Map<SalarySlipModel>(getdata);
            return dtoSlip;
        }

        public bool CheckSalaryReportPublish(int salYear, int salMonth, int empId)
        {
            try
            {
                return genericRepo.Exists<DTOModel.tblFinalMonthlySalary>(x => x.SalYear == salYear && x.SalMonth == salMonth && x.EmployeeID == empId && x.Publish == true && x.RecordType == "S");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
