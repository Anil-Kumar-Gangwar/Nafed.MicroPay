using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IReligionService
    {

        List<Model.Religion> GetReligion();
        bool ReligionNameExists(int? religionID, string religionName);
      
        bool UpdateReligion(Model.Religion editReligion);
        bool InsertReligion(Model.Religion createReligion);
        Model.Religion GetReligionByID(int religionID);
        bool Delete(int religionID);
    }
}
