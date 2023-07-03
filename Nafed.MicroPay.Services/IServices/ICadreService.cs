using System.Collections.Generic;
using Model=Nafed.MicroPay.Model;
namespace Nafed.MicroPay.Services.IServices
{
    public interface ICadreService
    {
        List<Model.Cadre> GetCadreList();
        bool CadreNameExists(int? cadreID, string cadreName);
        bool CadreCodeExists(int? cadreID, string cadreCode);
        bool UpdateCadre(Model.Cadre editCadreItem);
        bool InsertCadre(Model.Cadre createCadre);
        Model.Cadre GetCadreByID(int cadreID);
        bool Delete(int cadreID);
    }
}
