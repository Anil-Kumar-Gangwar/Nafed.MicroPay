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
    public class UserRightsService : BaseService, IUserRightsService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IUserRightsRepository userRightRepo;
        public UserRightsService(IGenericRepository genericRepo, IUserRightsRepository userRightRepo)
        {
            this.genericRepo = genericRepo;
            this.userRightRepo = userRightRepo;
        }
        public List<Model.Menu> GetMenuList(int departmentID)
        {
            log.Info($"UserRightsService/GetMenuList/{departmentID}");

            try
            {
                var result = genericRepo.Get<DTOModel.Menu>(x => x.DepartmentRights.Any(y => y.DepartmentID == departmentID));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Menu, Model.Menu>()
                    .ForMember(c => c.MenuID, c => c.MapFrom(m => m.MenuID))
                    .ForMember(d => d.MenuName, o => o.MapFrom(s => s.MenuName))
                    .ForMember(c => c.ParentID, c => c.MapFrom(m => m.ParentID))
                    .ForMember(c => c.SequenceNo, c => c.MapFrom(m => m.SequenceNo))
                    .ForMember(c => c.IconClass, c => c.MapFrom(m => m.IconClass))
                    .ForMember(c => c.IsActive, c => c.MapFrom(m => m.IsActive));
                });
                var listMenu = Mapper.Map<List<Model.Menu>>(result);

                var dtolist = genericRepo.Get<DTOModel.Menu>();
                listMenu.ForEach(x =>
                {
                    x.ParentName = x.ParentID != null ? dtolist.Where(y => y.MenuID == x.ParentID).FirstOrDefault().MenuName : "";
                });

                return listMenu.OrderBy(x => x.MenuID).ThenBy(x => x.ParentID).ToList();

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public List<SelectListModel> GetDepartmentUserList(int departmentID)
        {
            log.Info($"UserRightsService/GetDepartmentUserList/{departmentID}");
            try
            {
                var result = genericRepo.Get<DTOModel.User>(x => x.DepartmentID == departmentID && x.IsDeleted !=true && x.UserTypeID != 1 && x.UserTypeID != 2).OrderBy(x => x.FullName);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.User, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.UserID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.FullName + " (" + s.UserName + ")"));
                });
                return Mapper.Map<List<Model.SelectListModel>>(result);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<UserRights> GetDepartmentUserRights(int userID, int departmentID)
        {
            log.Info($"UserRightsService/GetDepartmentUserRights/{userID}/{departmentID}");
            try
            {
                var result = genericRepo.Get<DTOModel.UserRight>(x => x.DepartmentID == departmentID && x.UserID == userID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.UserRight, Model.UserRights>()
                     .ForMember(c => c.UserID, c => c.MapFrom(m => m.UserID))
                    .ForMember(c => c.DepartmentID, c => c.MapFrom(m => m.DepartmentID))
                    .ForMember(d => d.MenuID, o => o.MapFrom(s => s.MenuID))
                    .ForMember(d => d.ShowMenu, o => o.MapFrom(s => s.ShowMenu));
                });
                return Mapper.Map<List<Model.UserRights>>(result);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public int InsertUpdateDepartmentUserRights(int userID, int departmentID, DataTable dtDepartmentUserMenuRights)
        {
            try
            {
                int result = 0;
                result = userRightRepo.InsertUpdateDepartmentUserRights(userID, departmentID, dtDepartmentUserMenuRights);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<Model.UserAccessControlRights> GetUserAccessMenuList(int departmentID, bool isParentMenu)
        {
            log.Info($"GetUserAccessMenuList");

            try
            {
                List<UserAccessControlRights> lstMenu = new List<UserAccessControlRights>();
                if (isParentMenu)
                {
                    var result = userRightRepo.GetUserAccessMenuList(departmentID);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.Menu, Model.UserAccessControlRights>()
                        .ForMember(c => c.MenuID, c => c.MapFrom(m => m.MenuID))
                        .ForMember(d => d.MenuName, o => o.MapFrom(s => s.MenuName))
                        .ForMember(c => c.ParentID, c => c.MapFrom(m => m.ParentID));
                    });
                    lstMenu = Mapper.Map<List<Model.UserAccessControlRights>>(result);
                }
                else
                {
                    var result = genericRepo.Get<DTOModel.Menu>(x => x.DepartmentRights.Any(y => y.DepartmentID == departmentID));
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.Menu, Model.UserAccessControlRights>()
                        .ForMember(c => c.MenuID, c => c.MapFrom(m => m.MenuID))
                        .ForMember(d => d.MenuName, o => o.MapFrom(s => s.MenuName))
                        .ForMember(c => c.ParentID, c => c.MapFrom(m => m.ParentID));
                    });
                    lstMenu = Mapper.Map<List<Model.UserAccessControlRights>>(result);
                }

                return lstMenu.OrderBy(x => x.MenuID).ToList();

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public List<Model.UserAccessControlRights> GetUserAccessRightList(int userID, int departmentID)
        {
            log.Info($"GetUserAccessRightList");

            try
            {
                List<UserAccessControlRights> lstMenu = new List<UserAccessControlRights>();

                var result = genericRepo.Get<DTOModel.Menu>(x => x.DepartmentRights.Any(y => y.DepartmentID == departmentID) && x.UserRights.Any(u => u.DepartmentID == departmentID && u.UserID == userID));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Menu, Model.UserAccessControlRights>()
                    .ForMember(c => c.MenuID, c => c.MapFrom(m => m.MenuID))
                    .ForMember(d => d.MenuName, o => o.MapFrom(s => s.MenuName))
                    .ForMember(c => c.ParentID, c => c.MapFrom(m => m.ParentID))
                    .ForMember(c => c.View, c => c.MapFrom(m => m.UserRights.Where(v => v.MenuID == m.MenuID && v.UserID == userID).FirstOrDefault().ViewRight))
                    .ForMember(c => c.Create, c => c.MapFrom(m => m.UserRights.Where(v => v.MenuID == m.MenuID && v.UserID == userID).FirstOrDefault().CreateRight))
                    .ForMember(c => c.Edit, c => c.MapFrom(m => m.UserRights.Where(v => v.MenuID == m.MenuID && v.UserID == userID).FirstOrDefault().EditRight))
                    .ForMember(c => c.Delete, c => c.MapFrom(m => m.UserRights.Where(v => v.MenuID == m.MenuID && v.UserID == userID).FirstOrDefault().DeleteRight));
                });
                lstMenu = Mapper.Map<List<Model.UserAccessControlRights>>(result);


                return lstMenu.OrderBy(x => x.MenuID).ToList();

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }


        public bool InsertUpdateUserAccessControlRights(int userID, int departmentID, List<Model.UserAccessControlRights> listUpdateUserAccessRights)
        {
            try
            {
                foreach (var item in listUpdateUserAccessRights)
                {
                    DTOModel.UserRight obj = new DTOModel.UserRight();
                    if (genericRepo.Get<DTOModel.UserRight>().Any(x => x.DepartmentID == departmentID && x.UserID == userID && x.MenuID == item.MenuID))
                    {
                        obj = genericRepo.Get<DTOModel.UserRight>(x => x.DepartmentID == departmentID && x.UserID == userID && x.MenuID == item.MenuID).FirstOrDefault();
                        obj.ViewRight = item.View;
                        obj.CreateRight = item.Create;
                        obj.EditRight = item.Edit;
                        obj.DeleteRight = item.Delete;
                        obj.UpdatedBy = userID;
                        obj.UpdatedOn = DateTime.Now;
                        genericRepo.Update<DTOModel.UserRight>(obj);
                    }
                    else
                    {
                        obj.MenuID = item.MenuID;
                        obj.DepartmentID = departmentID;
                        obj.UserID = userID;
                        obj.ShowMenu = true;
                        obj.ViewRight = item.View;
                        obj.CreateRight = item.Create;
                        obj.EditRight = item.Edit;
                        obj.DeleteRight = item.Delete;
                        obj.CreatedOn = DateTime.Now;
                        
                        genericRepo.Insert<DTOModel.UserRight>(obj);
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<Model.User> GetUserByDepartmentID(int departmentID)
        {
            log.Info($"GetUserByDepartmentID");
            try
            {
                var userID = genericRepo.Get<DTOModel.User>(x => !(bool)x.IsDeleted && x.DepartmentID == departmentID && x.UserTypeID != 1)
                .Select(em => new User() { UserID = em.UserID }).ToList();
                return userID;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<int> GetUserIDByDepartment(int departmentID)
        {
            log.Info($"GetUserIDByDepartment");
            try
            {
                var userIDs = genericRepo.Get<DTOModel.User>(x => !x.IsDeleted && (departmentID>0? x.DepartmentID == departmentID:1>0) &&
                x.UserTypeID != 1).Select(x => x.UserID).ToList<int>();
                return userIDs;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
