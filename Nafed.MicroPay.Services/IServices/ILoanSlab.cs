using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
        public interface ILoanSlab
    {
        List<Model.LoanSlab> GetLoabSlabList();
        bool LoabSlabExists(int? slabNo);
        bool UpdateLoabSlab(Model.LoanSlab editloanSlab);
        bool InsertLoabSlab(Model.LoanSlab createloanSlab);
        Model.LoanSlab GetLoabSlabbyNo(int? slabNo);
        bool Delete(int? slabNo,string loanType,DateTime effectiveDate);


    }
}
