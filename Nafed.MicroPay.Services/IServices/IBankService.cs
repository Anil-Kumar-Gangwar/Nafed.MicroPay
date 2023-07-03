using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
        public interface IBankService
    {
        List<Model.Bank> GetBankList();
        bool BankNameExists(string bankName);
        bool BankCodeExists(string bankCode);
        bool UpdateBank(Model.Bank BankCode);
        bool InsertBank(Model.Bank createBank);
        Model.Bank GetBankbyCode(string bankCode);
        bool Delete(string bankCode);

    }
}
