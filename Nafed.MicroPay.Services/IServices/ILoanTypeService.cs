using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.IServices
{
    public interface ILoanTypeService
    {
        List<Model.tblMstLoanType> GetLoanTypeList();
        bool LoanTypeExists(string loanType);
        bool InsertLoanType(tblMstLoanType createLoanType);
        tblMstLoanType GetLoanTypeDtls(int loanTypeID);
        bool UpdateLoanType(tblMstLoanType editLoanType);
        bool Delete(int loanTypeID);
    }
}
