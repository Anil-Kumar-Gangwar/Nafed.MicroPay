using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
namespace Nafed.MicroPay.Services
{
    public class DepartmentRightsService : BaseService, IDepartmentRightsService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IDepartmentRightRepository departmentRightRepo;
        public DepartmentRightsService(IGenericRepository genericRepo, IDepartmentRightRepository departmentRightRepo)
        {
            this.genericRepo = genericRepo;
            this.departmentRightRepo = departmentRightRepo;
        }
        public List<SelectListModel> GetDepartment()
        {
            log.Info($"DepartmentRightsService/GetDepartment");
            try
            {
                var result = departmentRightRepo.GetEmpCountBasedOnDepartment();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetEmpCountBasedOnDepartment_Result, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.DepartmentID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.DepartmentName + " [" + s.EmpCount + "]"));
                });
                return Mapper.Map<List<Model.SelectListModel>>(result);
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }
        }
        public List<DepartmentRights> GetDepartmentRights(int departmentID)
        {
            log.Info($"DepartmentRightsService/GetDepartmentRights/{departmentID}");
            try
            {
                var result = genericRepo.Get<DTOModel.DepartmentRight>(x => x.DepartmentID == departmentID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.DepartmentRight, Model.DepartmentRights>()
                    .ForMember(c => c.DepartmentID, c => c.MapFrom(m => m.DepartmentID))
                    .ForMember(d => d.MenuID, o => o.MapFrom(s => s.MenuID))
                    .ForMember(d => d.ShowMenu, o => o.MapFrom(s => s.ShowMenu));
                });
                return Mapper.Map<List<Model.DepartmentRights>>(result);
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }
        }


        public int InsertUpdateDepartmentRights(int departmentID, DataTable dtDepartmentRights)
        {
            log.Info($"DepartmentRightsService/InsertUpdateDepartmentRights/{departmentID}");
            try
            {
                int result = 0;
                result = departmentRightRepo.InsertUpdateDepartmentRights(departmentID, dtDepartmentRights);
                return result;
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }

        }

        public List<int> GetDepartmentIDs()
        {
            log.Info($"GetDepartmentIDs");
            try
            {
                var departmentIDs = genericRepo.Get<DTOModel.Department>(x => (bool)!x.IsDeleted).Select(x => x.DepartmentID).ToList<int>();
                return departmentIDs;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}