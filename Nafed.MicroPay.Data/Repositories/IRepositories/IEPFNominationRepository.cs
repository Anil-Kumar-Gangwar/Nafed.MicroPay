using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
   public interface IEPFNominationRepository : IDisposable
    {
        GetEPFNominationDetail_Result GetEPFNominationDetail(int epfNoID, int employeeID);
        List<GetEPFEPSNominee_Result> GetEPFEPSNominee(int employeeID, int filterBy, int? epfNo);
        List<GetMaleEmployeeNominee_Result> GetMaleEmployeeNominee(int employeeID);
    }
}
