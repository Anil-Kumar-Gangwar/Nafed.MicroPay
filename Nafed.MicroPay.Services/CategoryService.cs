using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;

namespace Nafed.MicroPay.Services
{
     public   class CategoryService : BaseService, ICategoryService
    {
        private readonly IGenericRepository genericRepo;
        public CategoryService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public List<Model.Category> GetCategoryList()
        {
            log.Info($"CategoryService/GetCategoryList");
            try
            {
                var result = genericRepo.Get<DTOModel.Category>(x=>!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Category, Model.Category>()
                    .ForMember(c => c.CategoryID, c => c.MapFrom(s => s.CategoryID))
                    .ForMember(c => c.CategoryCode, c => c.MapFrom(s => s.CategoryCode))
                    .ForMember(c => c.CategoryName, c => c.MapFrom(s => s.CategoryName))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listCadre = Mapper.Map<List<Model.Category>>(result);
                return listCadre.OrderBy(x => x.CategoryName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool CategoryNameExists(int? categoryID, string categoryName)
        {
            log.Info($"CategoryService/CategoryNameExists/{categoryID}/{categoryName}");
            return genericRepo.Exists<DTOModel.Category>(x => x.CategoryID != categoryID && x.CategoryName == categoryName && !x.IsDeleted);
        }
        public bool CategoryCodeExists(int? categoryID, string categoryCode)
        {
            log.Info($"CategoryService/CategoryCodeExists/{categoryID}/{categoryCode}");
            return genericRepo.Exists<DTOModel.Category>(x => x.CategoryID != categoryID && x.CategoryCode == categoryCode && !x.IsDeleted);
        }
        public bool UpdateCategory(Model.Category editCategoryItem)
        {
            log.Info($"CategoryService/UpdateCategory");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.Category>(editCategoryItem.CategoryID);
                dtoObj.CategoryCode = editCategoryItem.CategoryCode;
                dtoObj.CategoryName = editCategoryItem.CategoryName;
                genericRepo.Update<DTOModel.Category>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool InsertCategory(Model.Category createCategory)
        {
            log.Info($"CategoryService/InsertCategory");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Category, DTOModel.Category>()
                    .ForMember(c => c.CategoryCode, c => c.MapFrom(m => m.CategoryCode))
                    .ForMember(c => c.CategoryName, c => c.MapFrom(m => m.CategoryName))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoCadre = Mapper.Map<DTOModel.Category>(createCategory);
                genericRepo.Insert<DTOModel.Category>(dtoCadre);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.Category GetCategoryByID(int categoryID)
        {
            log.Info($"CategoryService/GetCategoryByID/{categoryID}");
            try
            {
                var categoryObj = genericRepo.GetByID<DTOModel.Category>(categoryID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Category, Model.Category>()
                     .ForMember(c => c.CategoryCode, c => c.MapFrom(m => m.CategoryCode))
                    .ForMember(c => c.CategoryName, c => c.MapFrom(m => m.CategoryName))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.Category, Model.Category>(categoryObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(int categoryID)
        {
            log.Info($"CategoryService/Delete/{categoryID}");
            bool flag = false;
            try
            {
                DTOModel.Category dtoCategory = new DTOModel.Category();
                dtoCategory = genericRepo.GetByID<DTOModel.Category>(categoryID);
                dtoCategory.IsDeleted = true;
                genericRepo.Update<DTOModel.Category>(dtoCategory);
                flag = true;
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
