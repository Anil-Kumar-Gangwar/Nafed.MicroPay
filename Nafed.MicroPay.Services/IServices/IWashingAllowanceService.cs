using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
        public interface IWashingAllowanceService
    {
        List<Model.WashingAllowance> GetWashingAllowanceList();
        bool WashingAllowanceExists(DateTime effectiveDate, decimal rate);      
        bool UpdateWashingAllowance(Model.WashingAllowance updateWages);
        bool InsertWashingAllowance(Model.WashingAllowance createWages);
        Model.WashingAllowance GetWashingAllowance(DateTime effectiveDate, decimal rate);
        bool Delete(DateTime effectiveDate, decimal rate);
        Model.WashingAllowance GetLatestWashingAllowance();

    }
}
