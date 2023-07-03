using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
        public interface IHillCompensationService
    {
        List<Model.HillCompensation> GetHillCompensationList();
        bool HillCompensationExists(string branchCode, decimal upperLimit);
        bool UpdateHillCompensation(Model.HillCompensation editHillCompensation);
        bool InsertHillCompensation(Model.HillCompensation createHillCompensation);
        Model.HillCompensation GetHillCompensationbyBranchAmount(string branchCode, decimal upperLimit);        
        bool Delete(string branchCode, decimal upperLimit);


    }
}
