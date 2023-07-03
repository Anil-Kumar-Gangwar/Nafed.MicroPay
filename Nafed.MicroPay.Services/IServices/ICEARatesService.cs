using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
        public interface ICEARatesService
    {
        List<Model.CEARates> GetCEARatesList();
        bool CEARatesExists(DateTime effectiveDate);      
        bool UpdateCEARates(Model.CEARates updateCEARates);
        bool InsertCEARates(Model.CEARates createCEARates);
        Model.CEARates GetCEARates(DateTime effectiveDate);
        bool Delete(DateTime effectiveDate);

    }
}
