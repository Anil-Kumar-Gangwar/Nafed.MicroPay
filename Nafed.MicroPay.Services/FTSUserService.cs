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
using Nafed.MicroPay.Common;

namespace Nafed.MicroPay.Services
{
    public class FTSUserService : BaseService, IFTSUserService
    {
        private readonly IGenericRepository genericRepo;
        public FTSUserService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public List<Model.FTSUser> GetUserList()
        {
            log.Info($"FTSUserService/GetUserList");
            try
            {
                var result = genericRepo.Get<DTOModel.FTSUser>(x => x.IsDeleted == false && x.UserName.ToLower() != "admin");
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.FTSUser, Model.FTSUser>()
                    .ForMember(c => c.UserId, c => c.MapFrom(s => s.UserId))
                    .ForMember(c => c.UserName, c => c.MapFrom(s => s.UserName))
                    .ForMember(c => c.Password, c => c.MapFrom(s => s.Password))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listUser = Mapper.Map<List<Model.FTSUser>>(result);
                return listUser.OrderBy(x => x.UserName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UserNameExists(int? UserId, string UserName)
        {
            log.Info($"FTSUserService/UserNameExists/{UserId}/{UserName}");
            return genericRepo.Exists<DTOModel.FTSUser>(x => x.UserId != UserId && x.UserName == UserName && x.IsDeleted == false);
        }       
        
        public bool UpdateUser(Model.FTSUser editUserItem)
        {
            log.Info($"FTSUserService/UpdateUser");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.FTSUser>(editUserItem.UserId);
                dtoObj.Password = editUserItem.Password;
                genericRepo.Update<DTOModel.FTSUser>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }    
        public int InsertUser(Model.FTSUser createUser)
        {
            log.Info($"FTSUserService/InsertUser");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.FTSUser, DTOModel.FTSUser>()
                     .ForMember(c => c.UserId, c => c.MapFrom(s => s.UserId))                   
                    .ForMember(c => c.UserName, c => c.MapFrom(s => s.UserName))
                    .ForMember(c => c.Password, c => c.MapFrom(s => s.Password))                   
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))                  
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoUser = Mapper.Map<DTOModel.FTSUser>(createUser);
                genericRepo.Insert<DTOModel.FTSUser>(dtoUser);             
                return dtoUser.UserId;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            // return 0;
        }

        public Model.FTSUser GetUserByID(int UserId)
        {
            log.Info($"FTSUserService/GetUserByID {UserId}");
            try
            {
                var UserObj = genericRepo.GetByID<DTOModel.FTSUser>(UserId);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.FTSUser, Model.FTSUser>()
                       .ForMember(c => c.UserId, c => c.MapFrom(s => s.UserId))                   
                       .ForMember(c => c.UserName, c => c.MapFrom(s => s.UserName))
                       .ForMember(c => c.Password, c => c.MapFrom(s => s.Password))                     
                       .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                       .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))                    
                       .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.FTSUser, Model.FTSUser>(UserObj);
                log.Info($"FTSUserService/UserName {obj.UserName}");
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertMapDepartment(Model.FTSUserDepartment ftsDepartment)
        {
            log.Info($"FTSUserService/InsertMapDepartment");
            try
            {
                bool flag = false;

                for (int i = 0; i < ftsDepartment.intDepartmentId.Length; i++)
                {      
                    Mapper.Initialize(cfg => cfg.CreateMap<Model.FTSUserDepartment, DTOModel.FTSUserDepartment>()                    
                    .ForMember(d => d.UserId, o => o.MapFrom(s=> s.UserId))
                    .ForMember(d => d.DepartmentId, o => o.MapFrom(s => s.intDepartmentId[i]))
                    .ForAllOtherMembers(d => d.Ignore())
                    );
                    var dtoDepartWorkFlow = Mapper.Map<DTOModel.FTSUserDepartment>(ftsDepartment);
                    genericRepo.Insert(dtoDepartWorkFlow);
                    flag = true;                    
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }

        }

        public int[] GetMappedDepartment(int userId)
        {
            log.Info($"FTSUserService/GetMappedDepartment");
            try
            {
                var getData = genericRepo.Get<DTOModel.FTSUserDepartment>(x => x.UserId == userId)
            ;
            Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.FTSUserDepartment,Model.FTSUserDepartment>()                
                   .ForMember(d => d.DepartmentId, o => o.MapFrom(s => s.DepartmentId))
                   .ForAllOtherMembers(d => d.Ignore())
                   );
            var dtoDepartWorkFlow = Mapper.Map<List<Model.FTSUserDepartment>>(getData);
            List<int> termsList = new List<int>();
            for (int i = 0; i < dtoDepartWorkFlow.Count; i++)
            {
                termsList.Add(dtoDepartWorkFlow[i].DepartmentId);
            }

            return termsList.ToArray();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public bool UserExistInUserDepartment(int userId)
        {
            log.Info($"FTSUserService/userExistInUserDepartment");
            try {
                return genericRepo.Exists<DTOModel.FTSUserDepartment>(x => x.UserId == userId);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
}

        public bool UpdateMapDepartment(Model.FTSUserDepartment ftsDepartment)
        {
            log.Info($"FTSUserService/UpdateMapDepartment");
            try
            {
                bool flag = false;
                var getdata = genericRepo.Get<DTOModel.FTSUserDepartment>(x => x.UserId == ftsDepartment.UserId);
                if(getdata !=null && getdata.Count()>0)
                {
                    genericRepo.DeleteAll(getdata);
                }
                for (int i = 0; i < ftsDepartment.intDepartmentId.Length; i++)
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<Model.FTSUserDepartment, DTOModel.FTSUserDepartment>()
                    .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId))
                    .ForMember(d => d.DepartmentId, o => o.MapFrom(s => s.intDepartmentId[i]))
                    .ForAllOtherMembers(d => d.Ignore())
                    );
                    var dtoDepartWorkFlow = Mapper.Map<DTOModel.FTSUserDepartment>(ftsDepartment);
                    genericRepo.Insert(dtoDepartWorkFlow);
                    flag = true;
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }

        }

    }
}
