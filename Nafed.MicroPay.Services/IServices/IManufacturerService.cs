using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
   public interface IManufacturerService
    {
       
        List<Model.Manufacturer> GetManufacturerList(bool isDeleted);
        bool ManufacturerExists(string Manufacturer);
        bool InsertManufacturer(Model.Manufacturer createManufacturer);
        Model.Manufacturer GetManufacturerID(int ManufacturerID);
        bool UpdateManufacturer(Model.Manufacturer editManufacturer);
        bool Delete(int ManufacturerID);
       
        
    }
}
