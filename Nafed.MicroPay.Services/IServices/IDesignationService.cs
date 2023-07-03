using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.IServices
{
    public  interface IDesignationService
    {

        List<Designation> GetDesignationList(int? designationID, int? cadreID);
        bool DesignationNameExists(int? designationID, string designationName);
        bool DesignationCodeExists(int? designationID, string designationCode);
        bool UpdateDesignation(Model.Designation editDesignation);
        bool InsertDesignation(Model.Designation createDesignation);
        Model.Designation GetDesignationByID(int designationID);
        bool Delete(int designationID);
        EmployeePayScale CalculateBasicAndIncrement(EmployeePayScale payScale);
        bool UpdateDesignationPayScales(EmployeePayScale payScale);
       
        EmployeePayScale GetDesignationPayScale(int designationID);

        List<SelectListModel> GetDesignationByCadre(int cadreID);
        Promotion ChangeDesignation(Promotion promotion);
        bool DeletePromotionTransEntry(int transID);
        bool CheckConfrmChildHasRecord(int empID, int processID);
    }
}
