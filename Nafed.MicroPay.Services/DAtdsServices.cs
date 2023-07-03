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
using static Nafed.MicroPay.ImportExport.DATDSmanualEntryForm;
using Nafed.MicroPay.Data.Repositories;

namespace Nafed.MicroPay.Services
{
    public class DAtdsServices : BaseService, IDATDSServices
    {
        private readonly IGenericRepository genericRepo;
        private readonly IDATdsRepository daTdsRepo;

        public DAtdsServices(IGenericRepository genericRepo, IDATdsRepository daTdsRepo)
        {
            this.genericRepo = genericRepo;
            this.daTdsRepo = daTdsRepo;
        }

        public string GetDATDSTemplate(int branchID, byte month, int year, string fileName, string sFullPath)
        {
            log.Info($"PFAccountBalanceServices/GetPfBalanceManualEntryForm/{branchID}");

            string result = string.Empty;
            if (Directory.Exists(sFullPath))
            {
                sFullPath = $"{sFullPath}{fileName}";
                var empDTO = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.BranchID == branchID && !x.IsDeleted);

                DataTable dtData = new DataTable();

                dtData = daTdsRepo.GetDATDSManualEntryFormData(branchID, month, year);

                if (dtData != null && dtData.Rows.Count > 0)  //====== export attendance form if there is data ========= 
                {
                    //  dtData.Columns[0].ColumnName = "#";

                    IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                    exportHdr = dtData.Columns.Cast<DataColumn>().Select(x => x.ColumnName).AsEnumerable<string>();

                    result = ExportToExcel(exportHdr, dtData, "DA TDS Manual Entry Form", sFullPath);
                }
                else
                    result = "norec";
            }
            return result;
        }

        public int ImportDATDSManualData(int userID, List<ImportDATDSData> dataList)
        {
            log.Info($"PFAccountBalanceServices/ImportDATDSManualData");

            try
            {
                DataTable table = new DataTable();
                table.Columns.Add("EmployeeId", typeof(int));
                table.Columns.Add("Year", typeof(int));
                table.Columns.Add("Month", typeof(int));
                table.Columns.Add("DA", typeof(decimal));
                table.Columns.Add("TDS", typeof(decimal));
                table.Columns.Add("NetPay", typeof(decimal));
               


                foreach (var item in dataList)
                {
                    var row = table.NewRow();
                    row["EmployeeId"] = Convert.ToInt32(item.EmployeeId);
                    row["Year"] = Convert.ToInt32(item.Year);
                    row["Month"] = Convert.ToInt32(item.Month);

                    row["DA"] = Convert.ToDecimal(item.DA);
                    row["TDS"] = Convert.ToDecimal(item.IncomeTaxDeduction);
                    row["NetPay"] = Convert.ToDecimal(item.NetPay);
                    table.Rows.Add(row);
                }

                int month = Convert.ToInt32(table.Rows[0]["Month"]);
                int year = Convert.ToInt32(table.Rows[0]["Year"]);

                int res = daTdsRepo.ImportDATDSManualData(userID, table, month, year);
                return res;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
