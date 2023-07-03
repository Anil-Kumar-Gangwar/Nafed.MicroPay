using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
        public interface ICCAService
    {
        List<Model.CCA> GetCCAList();
        bool CCAExists(decimal upperLimit);      
        bool UpdateCCA(Model.CCA updateCCA);
        bool InsertCCA(Model.CCA createCCA);
        Model.CCA GetCCA(decimal upperLimit);
        bool Delete(decimal upperLimit);

    }
}
