using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
        public interface IFDAService
    {
        List<Model.FDA> GetFDAList();
        bool FDAExists(decimal upperlimit,decimal percentage);      
        bool UpdateFDA(Model.FDA updateFDA);
        bool InsertFDA(Model.FDA createFDA);
        Model.FDA GetFDA(decimal upperlimit, decimal percentage);
        bool Delete(decimal upperlimit, decimal percentage);

    }
}
