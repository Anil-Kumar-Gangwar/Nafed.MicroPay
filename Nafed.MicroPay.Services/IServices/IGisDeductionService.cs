using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
        public interface IGisDeductionService
    {
        List<Model.GisDeduction> GetGisDeductionList();
        bool GisDeductionExists(string category,int code);      
        bool UpdateGisDeduction(Model.GisDeduction updateGisDeduction);
        bool InsertGisDeduction(Model.GisDeduction createGisDeduction);
        Model.GisDeduction GetGisDeduction(string category, int codet);
        bool Delete(string category, int code);

    }
}
