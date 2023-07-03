using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.TDS
{
    public interface ITDSTaxRuleSlabService
    {
        IEnumerable<tblTDSTaxRulesSlab> GetTaxRuleSlabs(string fYear);
        bool SubmitTaxRuleSlabs(tblTDSTaxRulesSlab tdsSlab);
        bool IsTaxRuleSlabExists(string fYear);
    }
}
