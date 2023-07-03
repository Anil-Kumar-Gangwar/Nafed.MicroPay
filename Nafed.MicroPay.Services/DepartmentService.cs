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
    public class DepartmentService : BaseService, IDepartmentService
    {

        private readonly IGenericRepository genericRepo;
        public DepartmentService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public List<Model.Department> GetDeaprtmentList()
        {
            log.Info($"DepartmentService/GetDepartment List");
            try
            {
                var result = genericRepo.Get<DTOModel.Department>(x => x.IsDeleted == false);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Department, Model.Department>()
                    .ForMember(c => c.DepartmentID, c => c.MapFrom(s => s.DepartmentID))
                    .ForMember(c => c.DepartmentCode, c => c.MapFrom(s => s.DepartmentCode))
                    .ForMember(c => c.DepartmentName, c => c.MapFrom(s => s.DepartmentName))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listDepartment = Mapper.Map<List<Model.Department>>(result);
                return listDepartment.OrderBy(x => x.DepartmentName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool DepartmentNameExists(int? departmentID, string departmentName)
        {
            log.Info($"DepartmentService/DepartmentNameExists/{departmentID}/{departmentName}");
            return genericRepo.Exists<DTOModel.Department>(x => x.DepartmentName == departmentName && x.IsDeleted == false);
        }
        public bool DepartmentCodeExists(int? departmentID, string departmentCode)
        {
            log.Info($"DepartmentService/DepartmentCodeExists/{departmentID}/{departmentCode}");
            return genericRepo.Exists<DTOModel.Department>(x => x.DepartmentID != departmentID && x.DepartmentCode == departmentCode);
        }
        public bool UpdateDepartment(Model.Department editDepartment)
        {
            log.Info($"DepartmentService/UpdateDepartment");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.Department>(editDepartment.DepartmentID);
                if (dtoObj != null)
                {
                    dtoObj.DepartmentName = editDepartment.DepartmentName;
                    dtoObj.IsActive = editDepartment.IsActive;
                    genericRepo.Update<DTOModel.Department>(dtoObj);
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

        public bool InsertDepartment(Model.Department createDepartment)
        {
            log.Info($"DepartmentService/InsertDepartment");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Department, DTOModel.Department>()
                    .ForMember(c => c.DepartmentCode, c => c.MapFrom(m => m.DepartmentCode))
                    .ForMember(c => c.DepartmentName, c => c.MapFrom(m => m.DepartmentName))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForMember(c => c.IsActive, c => c.MapFrom(s => s.IsActive))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoDepartment = Mapper.Map<DTOModel.Department>(createDepartment);
                genericRepo.Insert<DTOModel.Department>(dtoDepartment);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.Department GetDepartmentByID(int departmentID)
        {
            log.Info($"DepartmentService/GetDepartmentByID/{departmentID}");
            try
            {
                var departmentObj = genericRepo.GetByID<DTOModel.Department>(departmentID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Department, Model.Department>()
                     .ForMember(c => c.DepartmentCode, c => c.MapFrom(m => m.DepartmentCode))
                    .ForMember(c => c.DepartmentName, c => c.MapFrom(m => m.DepartmentName))
                    .ForMember(c => c.IsActive, c => c.MapFrom(m => m.IsActive))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.Department, Model.Department>(departmentObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(int departmentID)
        {
            log.Info($"DepartmentService/Delete/{departmentID}");
            bool flag = false;
            try
            {
                DTOModel.Department dtoCadre = new DTOModel.Department();
                dtoCadre = genericRepo.GetByID<DTOModel.Department>(departmentID);
                if (dtoCadre != null)
                {
                    dtoCadre.IsDeleted = true;
                    genericRepo.Update<DTOModel.Department>(dtoCadre);
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
    }
}
