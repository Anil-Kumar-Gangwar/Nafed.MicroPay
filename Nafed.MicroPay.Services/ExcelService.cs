using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.ImportExport.Interfaces;
using System.Dynamic;
using Nafed.MicroPay.ImportExport.Model;

namespace Nafed.MicroPay.Services
{
    public class ExcelService : BaseService, IExcelService
    {
       
        private readonly IImport import;
        public ExcelService(IImport import)
        {
            this.import = import;
        }
        public DataTable ReadExcel(string sFileFullPath, bool IsHeader, int iSheetIndex = 0, string sSheetName = "")
        {
            

            return import.ReadExcel(sFileFullPath, IsHeader, iSheetIndex, sSheetName);
        }
        public DataTable ReadExcelWithNoHeader(string sFileFullPath, int iSheetIndex = 0, string sSheetName = "")
        {
            return import.ReadExcelWithNoHeader(sFileFullPath, iSheetIndex, sSheetName);
        }
        public StringBuilder ReadExcelInSB(string sFileFullPath, int iSheetIndex = 0, string sSheetName = "")
        {
            return import.ReadExcelInSB(sFileFullPath, iSheetIndex, sSheetName);
        }
        public List<ExpandoObject> ReadExcelDynamic(string sFileFullPath, int iSheetIndex = 0, string sSheetName = "")
        {
            return import.ReadExcelDynamic(sFileFullPath, iSheetIndex, sSheetName);
        }

        public DataTable GetDataWithHeader(DataTable dt)
        {
            return import.GetDataWithHeader(dt);
        }

        public List<ExcelSheetList> GetWorksheetName(string fileFullPath)
        {
            return import.GetWorksheetNames(fileFullPath);
        }
        public DataTable ReadAndValidateHeader(string filePath, List<string> headerList, bool isHeader, out bool isValid, out List<string> lstMissingHeader)
        { return import.ReadAndValidateHeader(filePath, headerList, isHeader, out isValid, out lstMissingHeader); }
    }
}
