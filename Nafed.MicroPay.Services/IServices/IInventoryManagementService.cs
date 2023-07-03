using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
   public interface IInventoryManagementService
    {
       
        List<Model.InventoryManagement> GetInventoryManagementList(int assettypeID, int manufacturerID);
        bool InventoryManagementExists(string serialNo);
        bool InsertInventoryManagement(Model.InventoryManagement createInventoryManagement);
        Model.InventoryManagement GetInventoryManagementID(int IMID);
        bool UpdateInventoryManagement(Model.InventoryManagement editInventoryManagement);
        bool Delete(int IMID);

        #region AssetmanagementDetail
        List<Model.InventoryManagement> GetAssetManagementList(int assettypeID, int assetID, int employeeID);
        Model.InventoryManagement GetAssetManagementID(int ID);
        bool InsertAssetManagement(Model.InventoryManagement createAssetManagement);

        bool UpdateAssetManagement(Model.InventoryManagement editAssetManagement);
        bool Deleteassetdetails(int ID);

        List<Model.InventoryManagement> GetAssetHistory(int IMID);

        string GetEmployeeEmail(int employeeID);
        #endregion

    }
}
