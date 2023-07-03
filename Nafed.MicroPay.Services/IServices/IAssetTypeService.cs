using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
   public interface IAssetTypeService
    {
        #region Asset Type
        List<Model.AssetType> GetAssetTypeList(bool isDeleted);
        bool AssetTypeExists(string AssetType);
        bool InsertAssetType(Model.AssetType createAssetType);
        Model.AssetType GetAssetTypeID(int AssetTypeID);
        bool UpdateAssetType(Model.AssetType editAssetType);
        bool Delete(int AssetTypeID);
        #endregion
        
    }
}
