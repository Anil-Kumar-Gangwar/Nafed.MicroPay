using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using AutoMapper;
using System.Data.SqlClient;

namespace Nafed.MicroPay.Services
{
    public class EmployeeTypeService :BaseService, IEmployeeTypeService
    {
        private readonly IGenericRepository genericRepo;
        public EmployeeTypeService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public List<EmployeeType> GetEmployeeTypeList()
        {
            log.Info($"EmployeeTypeService/GetEmployeeTypeList");

            try
            {
                var result = genericRepo.Get<Data.Models.EmployeeType>(x=>x.IsDeleted==false);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.EmployeeType, Model.EmployeeType>();
                });
                var listGrade = Mapper.Map<List<Model.EmployeeType>>(result);
                return listGrade.OrderBy(x => x.EmployeeTypeName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }


        }
        public bool EmployeeTypeNameExists(int? empTypeID, string empTypeName)
        {
            log.Info($"EmployeeTypeService/EmployeeTypeNameExists/{empTypeID}/{empTypeName}");
            return genericRepo.Exists<Data.Models.EmployeeType>(x => x.EmployeeTypeID != empTypeID && x.EmployeeTypeName == empTypeName && x.IsDeleted == false);
        }
        public bool UpdateEmployeeType(Model.EmployeeType editEmployeeType)
        {
            log.Info($"EmployeeTypeService/UpdateEmployeeType");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<Data.Models.EmployeeType>(editEmployeeType.EmployeeTypeID);
                dtoObj.EmployeeTypeName = editEmployeeType.EmployeeTypeName;
                genericRepo.Update<Data.Models.EmployeeType>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool InsertEmployeeType(Model.EmployeeType createGrade)
        {
            log.Info($"EmployeeTypeService/InsertEmployeeType");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                     cfg.CreateMap<Model.EmployeeType, Data.Models.EmployeeType>()
                    .ForMember(c => c.EmployeeTypeCode, c => c.MapFrom(m => m.EmployeeTypeCode))
                     .ForMember(c => c.EmployeeTypeName, c => c.MapFrom(m => m.EmployeeTypeName))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoEmployeeType = Mapper.Map<Data.Models.EmployeeType>(createGrade);
                genericRepo.Insert<Data.Models.EmployeeType>(dtoEmployeeType);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.EmployeeType GetEmployeeTypeByID(int empTypeID)
        {
            log.Info($"EmployeeTypeService/GetEmployeeTypeByID {empTypeID}");
            try
            {
                var empType = genericRepo.GetByID<Data.Models.EmployeeType>(empTypeID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.EmployeeType, Model.EmployeeType>()
                     .ForMember(c => c.EmployeeTypeID, c => c.MapFrom(m => m.EmployeeTypeID))
                     .ForMember(c => c.EmployeeTypeCode, c => c.MapFrom(m => m.EmployeeTypeCode))
                     .ForMember(c => c.EmployeeTypeName, c => c.MapFrom(m => m.EmployeeTypeName))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<Data.Models.EmployeeType, Model.EmployeeType>(empType);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(int empTypeID)
        {
            log.Info($"EmployeeTypeService/Delete{empTypeID}");
            bool flag = false;
            try
            {
                Data.Models.EmployeeType dtoEmpType = new Data.Models.EmployeeType();
                dtoEmpType = genericRepo.GetByID<Data.Models.EmployeeType>(empTypeID);
                dtoEmpType.IsDeleted = true;
                genericRepo.Update<Data.Models.EmployeeType>(dtoEmpType);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool EmployeeTypeCodeExists(int? empTypeID, string empTypeCode)
        {
            log.Info($"EmployeeTypeService/EmployeeTypeCodeExists/{empTypeID}/{empTypeCode}");
            return genericRepo.Exists<Data.Models.EmployeeType>(x => x.EmployeeTypeID != empTypeID && x.EmployeeTypeCode == empTypeCode);
        }


    }
}
