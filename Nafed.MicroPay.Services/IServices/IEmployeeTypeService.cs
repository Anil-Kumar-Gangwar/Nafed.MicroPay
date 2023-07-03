using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
   public interface IEmployeeTypeService
    {
        List<Model.EmployeeType> GetEmployeeTypeList();
        bool EmployeeTypeNameExists(int? empTypeID, string empTypeName);
        bool EmployeeTypeCodeExists(int? empTypeID, string empTypeCode);
        bool UpdateEmployeeType(Model.EmployeeType editEmpType);
        bool InsertEmployeeType(Model.EmployeeType createEmpType);
        Model.EmployeeType GetEmployeeTypeByID(int empTypeID);
        bool Delete(int empTypeID);
    }
}
