using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IOTAService
    {
        List<Model.OTA> GetOTAList();
        bool OTAExists(int oTACode);
        bool UpdateOTA(Model.OTA editOTA);
        bool InsertOTA(Model.OTA createOTA);
        Model.OTA GetOTAyNoOTACode(int oTACode);
        bool Delete(int oTACode);
        Model.OTA GetMaxOTARate();

        #region  OTA Slip
        List<Model.OTASlip> GetOTASlipList(int? employeeID);
        List<Model.OTASlip> GetOTASlipDetail(Model.CommonFilter filters);
        bool InsertOTASlip(Model.OTASlip otaSlip);
        Model.OTASlip GetOTASlip(int empID, int appNo);
        bool UpdateOTASlip(Model.OTASlip otaSlip);
        bool OTASlipExists(int empID, DateTime date);
        #endregion

    }
}
