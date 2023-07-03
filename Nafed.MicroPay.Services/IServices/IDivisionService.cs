using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.IServices
{
   public  interface IDivisionService
    {
        List<Model.Division> GetDivisionList();
        bool DivisionNameExists(int? DivisionID, string DivisionName);
        bool DivisionCodeExists(int? DivisionID, string DivisionCode);
        bool UpdateDivision(Model.Division editDivisionItem);
        bool InsertDivision(Model.Division createDivision);
        Model.Division GetDivisionByID(int DivisionID);
        bool Delete(int DivisionID);
    }
}
