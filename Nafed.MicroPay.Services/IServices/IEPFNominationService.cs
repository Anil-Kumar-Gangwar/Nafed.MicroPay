using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
   public interface IEPFNominationService
    {
        Model.EPFNomination GetEPFNominationDetail(int epfNoID, int employeeID);
        List<Model.EPFDetail> GetEPFNominee(int employeeID, int filterBy, int? epfNo);
        List<Model.EPSDetail> GetEPSNominee(int employeeID, int filterBy, int? epfNo);
        int InsertEPFNominee(Model.EPFNomination epfNom);
        List<Model.EPFNomination> GetEPFNomineeList(int? employeeID,DateTime? fromDate=null,DateTime? toDate=null);
        int UpdateEPFNominee(Model.EPFNomination epfNom);
        List<Model.EPSDetail> GetMaleEmployeeEPSNominee(int employeeID);
        List<Model.EPFNomination> GetEPFApprovalList(Model.CommonFilter filters);
    }
}
