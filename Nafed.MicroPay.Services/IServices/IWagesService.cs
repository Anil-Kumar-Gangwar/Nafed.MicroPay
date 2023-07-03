using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
        public interface IWagesService
    {
        List<Model.Wages> GetWagesList();
        bool WagesExists(DateTime effectiveDate,decimal ratePerHour);      
        bool UpdateWages(Model.Wages updateWages);
        bool InsertWages(Model.Wages createWages);
        Model.Wages GetWages(DateTime effectiveDate, decimal ratePerHour);
        bool Delete(DateTime effectiveDate, decimal ratePerHour);

    }
}
