using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;

namespace Nafed.MicroPay.Services
{
    public class InventoryManagementService : BaseService, IInventoryManagementService
    {
        private readonly IGenericRepository genericRepo;
        public InventoryManagementService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.InventoryManagement> GetInventoryManagementList(int assettypeID, int manufacturerID)
        {
            log.Info($"InventoryManagementService/GetInventoryManagementList");
            try
            {
                var result = genericRepo.Get<Data.Models.InventoryManagement>(x => x.IsDeleted == false
               && (assettypeID > 0 ? x.AssetTypeID == assettypeID : 1 > 0)
               && (manufacturerID > 0 ? x.ManufacturerID == manufacturerID : 1 > 0));



                //var result = genericRepo.Get<DTOModel.InventoryManagement>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.InventoryManagement, Model.InventoryManagement>()
                    .ForMember(c => c.IMID, c => c.MapFrom(s => s.IMID))
                    .ForMember(c => c.AssetTypeID, c => c.MapFrom(s => s.AssetTypeID))
                    .ForMember(c => c.ManufacturerID, c => c.MapFrom(s => s.ManufacturerID))
                     .ForMember(c => c.AssetTypeName, c => c.MapFrom(s => s.AssetType.AssetTypeName))
                     .ForMember(c => c.AssetName, c => c.MapFrom(s => s.AssetName))
                    .ForMember(c => c.ManufacturerName, c => c.MapFrom(s => s.Manufacturer.ManufacturerName))
                    .ForMember(c => c.SerialNo, c => c.MapFrom(s => s.SerialNo))
                    .ForMember(c => c.Price, c => c.MapFrom(s => s.Price))
                    .ForMember(c => c.Consumable, c => c.MapFrom(s => s.Consumable))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                     .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.CreatedName, c => c.MapFrom(s => s.User.tblMstEmployee.Name))
                    .ForMember(c => c.UpdatedName, c => c.MapFrom(s => s.User1.tblMstEmployee.Name))
                    .ForMember(c => c.ManufacturingYr, c => c.MapFrom(s => s.ManufacturingYr))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listInventoryManagement = Mapper.Map<List<Model.InventoryManagement>>(result);
                return listInventoryManagement.OrderBy(x => x.IsDeleted).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InventoryManagementExists(string serialNo)
        {
            log.Info($"InventoryManagementService/InventoryManagementExists");
            try
            {
                return genericRepo.Exists<DTOModel.InventoryManagement>(x => x.SerialNo == serialNo && !x.IsDeleted);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertInventoryManagement(Model.InventoryManagement createInventoryManagement)
        {
            log.Info($"InventoryManagementService/InsertInventoryManagement");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.InventoryManagement, DTOModel.InventoryManagement>()
                    .ForMember(c => c.AssetTypeID, c => c.MapFrom(s => s.AssetTypeID))
                    .ForMember(c => c.ManufacturerID, c => c.MapFrom(s => s.ManufacturerID))
                      .ForMember(c => c.AssetName, c => c.MapFrom(s => s.AssetName))
                    .ForMember(c => c.SerialNo, c => c.MapFrom(s => s.SerialNo))
                    .ForMember(c => c.Price, c => c.MapFrom(s => s.Price))
                    .ForMember(c => c.Consumable, c => c.MapFrom(s => s.Consumable))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.StatusID, c => c.MapFrom(s => 1))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForMember(c => c.ManufacturingYr, c => c.MapFrom(m => m.ManufacturingYr))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoInventoryManagement = Mapper.Map<DTOModel.InventoryManagement>(createInventoryManagement);
                genericRepo.Insert<DTOModel.InventoryManagement>(dtoInventoryManagement);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.InventoryManagement GetInventoryManagementID(int IMID)
        {
            log.Info($"InventoryManagementService/GetInventoryManagementID/ {IMID}");
            try
            {
                var cadreObj = genericRepo.GetByID<DTOModel.InventoryManagement>(IMID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.InventoryManagement, Model.InventoryManagement>()
                    .ForMember(c => c.IMID, c => c.MapFrom(s => s.IMID))
                    .ForMember(c => c.AssetTypeID, c => c.MapFrom(s => s.AssetTypeID))
                    .ForMember(c => c.ManufacturerID, c => c.MapFrom(s => s.ManufacturerID))
                     .ForMember(c => c.AssetName, c => c.MapFrom(s => s.AssetName))
                    .ForMember(c => c.SerialNo, c => c.MapFrom(s => s.SerialNo))
                    .ForMember(c => c.Price, c => c.MapFrom(s => s.Price))
                    .ForMember(c => c.Consumable, c => c.MapFrom(s => s.Consumable))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                       .ForMember(c => c.ManufacturingYr, c => c.MapFrom(m => m.ManufacturingYr))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.InventoryManagement, Model.InventoryManagement>(cadreObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateInventoryManagement(Model.InventoryManagement editInventoryManagement)
        {
            log.Info($"InventoryManagementService/UpdateInventoryManagement");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.InventoryManagement>(editInventoryManagement.IMID);
                if (dtoObj != null)
                {
                    dtoObj.AssetTypeID = (int)editInventoryManagement.AssetTypeID;
                    dtoObj.ManufacturerID = (int)editInventoryManagement.ManufacturerID;
                    dtoObj.AssetName = editInventoryManagement.AssetName;
                    dtoObj.SerialNo = editInventoryManagement.SerialNo;
                    dtoObj.Price = editInventoryManagement.Price;
                    dtoObj.Consumable = editInventoryManagement.Consumable;
                    dtoObj.Remarks = editInventoryManagement.Remarks;
                    dtoObj.StatusID = editInventoryManagement.StatusID;
                    dtoObj.UpdatedBy = editInventoryManagement.UpdatedBy;
                    dtoObj.UpdatedOn = editInventoryManagement.UpdatedOn;
                    dtoObj.ManufacturingYr = editInventoryManagement.ManufacturingYr;
                    genericRepo.Update<DTOModel.InventoryManagement>(dtoObj);
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool Delete(int IMID)
        {
            log.Info($"InventoryManagementService/Delete/{IMID}");
            bool flag = false;
            try
            {
                var dtoInventoryManagement = genericRepo.GetByID<DTOModel.InventoryManagement>(IMID);
                if (dtoInventoryManagement != null)
                {
                    dtoInventoryManagement.IsDeleted = true;
                    genericRepo.Update<DTOModel.InventoryManagement>(dtoInventoryManagement);
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool IsAssetInUse(int IMID)
        {
            log.Info($"InventoryManagementService/IsAssetInUse/IMID={IMID}");
            try
            {
                return genericRepo.Exists<DTOModel.AssetmanagementDetail>(x => x.IMID == IMID);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #region AssetmanagementDetail

        public List<Model.InventoryManagement> GetAssetManagementList(int assettypeID, int assetID, int employeeID)
        {
            log.Info($"InventoryManagementService/GetAssetManagementList");
            try
            {
                var result = genericRepo.Get<Data.Models.AssetmanagementDetail>(x => x.IsDeleted == false
               && (assettypeID > 0 ? x.AssetTypeID == assettypeID : 1 > 0)
               && (assetID > 0 ? x.IMID == assetID : 1 > 0)
               && (employeeID > 0 ? x.EmployeeID == employeeID : 1 > 0));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.AssetmanagementDetail, Model.InventoryManagement>()
                    .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.EmployeeID, c => c.MapFrom(s => s.EmployeeID))
                    .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                     .ForMember(c => c.EmployeeName, c => c.MapFrom(s => s.tblMstEmployee.Name))
                     .ForMember(c => c.AssetName, c => c.MapFrom(s => s.InventoryManagement.AssetName))
                    .ForMember(c => c.AssetTypeName, c => c.MapFrom(s => s.AssetType.AssetTypeName))
                    .ForMember(c => c.AllocationDate, c => c.MapFrom(s => s.AllocationDate))
                    .ForMember(c => c.DeAllocationDate, c => c.MapFrom(s => s.DeAllocationDate))
                    .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                     .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.CreatedName, c => c.MapFrom(s => s.User.tblMstEmployee.Name))
                    .ForMember(c => c.UpdatedName, c => c.MapFrom(s => s.User1.tblMstEmployee.Name))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listInventoryManagement = Mapper.Map<List<Model.InventoryManagement>>(result);
                return listInventoryManagement.OrderBy(x => x.IsDeleted).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public Model.InventoryManagement GetAssetManagementID(int ID)
        {
            log.Info($"InventoryManagementService/GetAssetManagementID/ {ID}");
            try
            {
                var assetmanagmentObj = genericRepo.GetByID<DTOModel.AssetmanagementDetail>(ID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.AssetmanagementDetail, Model.InventoryManagement>()
                    .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.IMID, c => c.MapFrom(s => s.IMID))
                    .ForMember(c => c.AssetTypeID, c => c.MapFrom(s => s.AssetTypeID))
                    .ForMember(c => c.EmployeeID, c => c.MapFrom(s => s.EmployeeID))
                    .ForMember(c => c.AllocationDate, c => c.MapFrom(s => s.AllocationDate))
                    .ForMember(c => c.DeAllocationDate, c => c.MapFrom(s => s.DeAllocationDate))
                    .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.AssetmanagementDetail, Model.InventoryManagement>(assetmanagmentObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertAssetManagement(Model.InventoryManagement createAssetManagement)
        {
            log.Info($"InventoryManagementService/InsertAssetManagement");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.InventoryManagement, DTOModel.AssetmanagementDetail>()
                    .ForMember(c => c.EmployeeID, c => c.MapFrom(s => s.EmployeeID))
                    .ForMember(c => c.AssetTypeID, c => c.MapFrom(s => s.AssetTypeID))
                     .ForMember(c => c.IMID, c => c.MapFrom(s => s.IMID))
                    .ForMember(c => c.AllocationDate, c => c.MapFrom(s => s.AllocationDate))
                    .ForMember(c => c.DeAllocationDate, c => c.MapFrom(s => s.DeAllocationDate))
                    .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoAssetManagement = Mapper.Map<DTOModel.AssetmanagementDetail>(createAssetManagement);
                genericRepo.Insert<DTOModel.AssetmanagementDetail>(dtoAssetManagement);
                var dtoInvMang = genericRepo.GetByID<Data.Models.InventoryManagement>(createAssetManagement.IMID);
                if (dtoInvMang != null)
                {
                    dtoInvMang.StatusID = (int)createAssetManagement.StatusID;
                    dtoInvMang.UpdatedBy = createAssetManagement.CreatedBy;
                    dtoInvMang.UpdatedOn = createAssetManagement.CreatedOn;
                    genericRepo.Update<Data.Models.InventoryManagement>(dtoInvMang);
                }
                if (!string.IsNullOrEmpty(createAssetManagement.Email))
                {
                    var employeeEmail = genericRepo.GetByID<DTOModel.tblMstEmployee>(createAssetManagement.EmployeeID);
                    if (employeeEmail != null)
                    {
                        employeeEmail.OfficialEmail = createAssetManagement.Email;
                        genericRepo.Update(employeeEmail);
                    }
                }
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdateAssetManagement(Model.InventoryManagement editAssetManagement)
        {
            log.Info($"InventoryManagementService/UpdateAssetManagement");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.AssetmanagementDetail>(editAssetManagement.ID);
                if (dtoObj != null)
                {
                    dtoObj.AssetTypeID = editAssetManagement.AssetTypeID;
                    dtoObj.IMID = editAssetManagement.IMID;
                    dtoObj.EmployeeID = editAssetManagement.EmployeeID;
                    dtoObj.AllocationDate = (DateTime)editAssetManagement.AllocationDate;
                    dtoObj.DeAllocationDate = editAssetManagement.DeAllocationDate;
                    dtoObj.StatusID = editAssetManagement.StatusID;
                    dtoObj.UpdatedBy = editAssetManagement.UpdatedBy;
                    dtoObj.UpdatedOn = editAssetManagement.UpdatedOn;
                    genericRepo.Update<DTOModel.AssetmanagementDetail>(dtoObj);
                    flag = true;
                    var dtoInvMang = genericRepo.GetByID<Data.Models.InventoryManagement>(editAssetManagement.IMID);
                    if (dtoInvMang != null)
                    {
                        dtoInvMang.StatusID = (int)editAssetManagement.StatusID;
                        dtoInvMang.UpdatedBy = editAssetManagement.UpdatedBy;
                        dtoInvMang.UpdatedOn = editAssetManagement.UpdatedOn;
                        genericRepo.Update<Data.Models.InventoryManagement>(dtoInvMang);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }


        public bool Deleteassetdetails(int ID)
        {
            log.Info($"InventoryManagementService/Delete/{ID}");
            bool flag = false;
            try
            {
                var dtoAssetManagement = genericRepo.GetByID<DTOModel.AssetmanagementDetail>(ID);
                if (dtoAssetManagement != null)
                {
                    dtoAssetManagement.IsDeleted = true;
                    genericRepo.Update<DTOModel.AssetmanagementDetail>(dtoAssetManagement);
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public List<Model.InventoryManagement> GetAssetHistory(int IMID)
        {
            log.Info($"InventoryManagementService/GetAssetHistory/IMID={IMID}");
            try
            {
                var result = genericRepo.Get<Data.Models.AssetmanagementDetail>(x => x.IMID == IMID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.AssetmanagementDetail, Model.InventoryManagement>()
                      .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                      .ForMember(c => c.EmployeeName, c => c.MapFrom(s => s.tblMstEmployee.Name))
                     .ForMember(c => c.AllocationDate, c => c.MapFrom(s => s.AllocationDate))
                     .ForMember(c => c.DeAllocationDate, c => c.MapFrom(s => s.DeAllocationDate))
                     .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listInventoryManagement = Mapper.Map<List<Model.InventoryManagement>>(result);
                return listInventoryManagement.OrderByDescending(x => x.AllocationDate).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public string GetEmployeeEmail(int employeeID)
        {
            log.Info($"InventoryManagementService/GetEmployeeEmail/{employeeID}");
            try
            {
                var dtoEmployee = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeID);
                if (dtoEmployee != null)
                {
                    return dtoEmployee.OfficialEmail;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

    }
}
