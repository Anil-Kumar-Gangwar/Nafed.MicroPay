using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IExgratiaService
    {
        List<ExgratiaList> checkexistExgratiaList(string financialyear);
        int GenerateList(string fYear, int year);
        List<Model.ExgratiaList> GetExgratiaList(string fyear, int? BranchID, int empTypeID);
        Model.ExgratiaList GetExgratiaByID(int? ExgratiaID);
        bool Delete(int ExgratiaID, int userID);
        List<SelectListModel> GetAllEmployee();
        int updateExgratiaTDS(Model.ExgratiaList exgratia);
        int calculateExgratia(int noofdays, string fromYear, string toYear, int branchID, string fYear, bool isPercentage, int emptype);
        DataTable GetExgratiaExport(string fromYear, string toYear, string fyear, int branchID, int emptype);
        string GetExgratiaExportTemplate(string fromYear, string toYear, string fyear, int branchID, int emptype, string fileName, string filePath);


        string GetExGratiaEntryForm(int branchID, int salMonth, int salYear, string fileName, string filePath);
        void PublishExGratia(string financialyear, int emptype, int branchID);
    }
}
