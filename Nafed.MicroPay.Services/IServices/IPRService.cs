using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nafed.MicroPay.Services.IServices
{
    public interface IPRService
    {
        List<Model.PR> GetPRGVList(int EmployeeID, int? PRID, int? Year);
        List<Model.PR> GetPRHList(int EmployeeID);
        List<SelectListModel> GetAllEmployee();
        Model.PR GetPRList(int PRID);
        int InsertPRHeaders(Model.PR createPR);
        bool InsertPR(Model.PR createPR, int PRID);
        bool UpdatePR(Model.PR createPR);
        bool UpdatePRStatus(int PRID,bool isApplicable);
        bool DeletePR(int PRID);
        List<Model.PR> GetPRViewList( int? Year);
        bool getemployeeallDetails(int designationID, int employeeID, out PR PRdetail, int? year = null);
        Model.PR GetPropertyReturn(int year, int employeeID);
    }
}
