using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IBankRatesService
    {
        List<Model.BankRates> GetBankRatesList();
        bool UpdateBankRates(Model.BankRates editBankRates);
        bool InsertBankRates(Model.BankRates createBankRates);
        Model.BankRates GetBankRatesbyDate(DateTime bankdate);
        bool BankDateExist(Nullable<System.DateTime> bankdate);
        bool Delete(DateTime bankdate);
        Model.BankRates GeLatestBankRate();

    }
}
