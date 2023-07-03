using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface ILTCService
    {
        List<Model.LTC> GetLTCList(int? employeeID);
        Model.LTC GetLTCByID(int LTCID,int employeeID);
        List<SelectListModel> GetDependentlist(int EmployeeId);
        int InsertLTC(Model.LTC createLTC, ProcessWorkFlow workFlow);
        int GetLastLTCNo(int employeeID, bool? WhetherSelf);
        List<SelectListModel> GetAllEmployee();
        List<SelectListModel> GetAlldependent(int? employeeID);
        int UpdateLTC(LTC createLTC, ProcessWorkFlow workFlow);
        DateTime GetLastDateOfreturn(int employeeID);
        bool DeleteLTC(int LTCID);

        IEnumerable<LTC> GetLTCListForApproval(LTC filters);
        //double GetELLeaveBal(int employeeID, DateTime DateAvailLTC);
        //double GetCLLeaveBal(int employeeID, DateTime DateAvailLTC);
        //int InsertEmployeeLeave(Model.LTC createLTC, double totaldays);
        //bool InsertLeaveTrans(Model.LTC createLTC, double totaldays, int leaveID);

        #region LTC Status
        Model.LTCReport GetLTCReportByID(int LTCID, int employeeID);
        bool PostLTCReferenceNumber(LTCReport LTCReport);
        List<Holiday> GetHolidayList(DateTime? StartDate, DateTime? EndDate);
        #endregion LTC Status
    }
}
