using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
namespace Nafed.MicroPay.Data.Repositories
{
    public class EPFNominationRepository : BaseRepository, IEPFNominationRepository
    {
        public GetEPFNominationDetail_Result GetEPFNominationDetail(int epfNoID, int employeeID)
        {
            return db.GetEPFNominationDetail(epfNoID, employeeID).FirstOrDefault();
        }

        public List<GetEPFEPSNominee_Result> GetEPFEPSNominee(int employeeID,int filterBy,int ? epfNo)
        {
            return db.GetEPFEPSNominee(employeeID, filterBy, epfNo).ToList();
        }
        public List<GetMaleEmployeeNominee_Result> GetMaleEmployeeNominee(int employeeID)
        {
            return db.GetMaleEmployeeNominee(employeeID).ToList();
        }
    }
}
