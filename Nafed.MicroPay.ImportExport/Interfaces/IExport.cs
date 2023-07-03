using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace Nafed.MicroPay.ImportExport.Interfaces
{
    public interface IExport : IBase
    {
        string ExportToExcel(DataTable sourceDataTable, string sSheetName, string sFullPath, string sDefaultOrderImage = null, string sItemImageUNCPath = null, string sArchiveItemImageUNCPath = null, string[] nonExportColumn = null);
        byte[] ExportToExcel(DataTable sourceDataTable, string sSheetName);
        MemoryStream WriteExcelInMS(DataTable sourceDataTable, string sSheetName);
        string ExportToExcelWithDroDowns(DataTable sourceDataTable, string sSheetName, string sFullPath, string sDefaultOrderImage = null, string sItemImageUNCPath = null,
            bool bDropdown = false, DataSet dtItemGroup = null);

        string ExportBlankTemplateWithDroDowns(DataTable sourceDataTable, string sSheetName, string sFullPath,
          bool bDropdown = false, DataSet dsDropDownValues = null);

        string ExportToExcelWithError(DataTable sourceDataTable, string sSheetName, string sFullPath, string sDefaultOrderImage = null, string sItemImageUNCPath = null, List<string> lstSkipColumns = null);

        string ExportToExcelWithErrorDynamic(DataTable sourceDataTable, string sSheetName, string sFullPath, string sDefaultOrderImage = null, string sItemImageUNCPath = null, List<string> lstSkipColumns = null);
        bool ExportToExcel(DataSet dsSource, string sFullPath,string fileName);
        byte[] ReadMSToExcel(MemoryStream ms, DataSet dsArtworkTemplate, List<string> ArryTemplateIDs);
        bool ExportFormatedExcel(DataSet dsSource, string sFullPath, string fileName);
    }
}