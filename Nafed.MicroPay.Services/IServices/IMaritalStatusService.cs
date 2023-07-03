using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IMaritalStatusService
    {
        List<Model.MaritalStatus> GetMaritalStatusList();
        bool MaritalStatusNameExists(int? MaritalStatusID, string MaritalStatusName);
        bool MaritalStatusCodeExists(int? MaritalStatusID, string MaritalStatusCode);
        bool UpdateMaritalStatus(Model.MaritalStatus editMaritalStatusItem);
        bool InsertMaritalStatus(Model.MaritalStatus createMaritalStatus);
        Model.MaritalStatus GetMaritalStatusByID(int MaritalStatusID);
        bool Delete(int MaritalStatusID);
    }
}
