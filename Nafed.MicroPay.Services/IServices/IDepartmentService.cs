using System.Collections.Generic;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IDepartmentService
    {
        List<Model.Department> GetDeaprtmentList();
        bool DepartmentNameExists(int? departmentID, string departmentName);
        bool DepartmentCodeExists(int? departmentID, string departmentCode);
        bool UpdateDepartment(Model.Department editDepartment);
        bool InsertDepartment(Model.Department createDepartment);
        Model.Department GetDepartmentByID(int departmentID);
        bool Delete(int departmentID);
    }
}
