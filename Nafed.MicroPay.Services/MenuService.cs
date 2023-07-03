using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static Nafed.MicroPay.Common.UserType;


namespace Nafed.MicroPay.Services
{
    public class MenuService : BaseService, IMenuService
    {
        private readonly IMenuRepository menuRepository;
        private readonly IGenericRepository genericRepo;
        public MenuService(IMenuRepository menuRepository, IGenericRepository genericRepo)
        {
            this.menuRepository = menuRepository;
            this.genericRepo = genericRepo;

        }
        public List<Model.Menu> GetMenuList()
        {
            log.Info($"GetMenuList");

            try
            {
                var result = genericRepo.Get<DTOModel.Menu>();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Menu, Model.Menu>()
                    .ForMember(c => c.MenuID, c => c.MapFrom(m => m.MenuID))
                    .ForMember(d => d.MenuName, o => o.MapFrom(s => s.MenuName))
                    .ForMember(c => c.ParentID, c => c.MapFrom(m => m.ParentID))
                    .ForMember(c => c.SequenceNo, c => c.MapFrom(m => m.SequenceNo))
                    .ForMember(c => c.IconClass, c => c.MapFrom(m => m.IconClass))
                    .ForMember(c => c.Help, c => c.MapFrom(m => m.Help))
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
        public List<Model.Menu> GetMenuList(int userID, int departmentID, int userTypeID, bool activeOnMobile=false)
        {
            log.Info($"GetMenuList/userID:{userID}/departmentID:{departmentID}/userTypeID:{userTypeID}");

            try
            {
                List<Model.Menu> listMenu = new List<Model.Menu>();

                if (!(userTypeID == (int)SuperUser || userTypeID == (int)Admin))
                {
                    if (genericRepo.Get<DTOModel.UserRight>().Any(x => x.DepartmentID == departmentID
                      && x.UserID == userID))
                    {
                        var assignedDepartmentMenus = genericRepo.Get<DTOModel.DepartmentRight>(x => 
                         x.DepartmentID == departmentID
                         && x.ShowMenu).Select(y => y.MenuID).ToArray<int>();

                        var result = genericRepo.Get<DTOModel.UserRight>(x => x.DepartmentID == departmentID
                        && assignedDepartmentMenus.Any(adm => adm == x.MenuID)
                        && x.UserID == userID && x.Menu.IsActive == true
                        && (x.Menu.ActiveOnMobile == ( activeOnMobile ? true : x.Menu.ActiveOnMobile )));

                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<DTOModel.UserRight, Model.Menu>()
                            .ForMember(c => c.MenuID, c => c.MapFrom(m => m.MenuID))
                            .ForMember(d => d.MenuName, o => o.MapFrom(s => s.Menu.MenuName))
                            .ForMember(c => c.ParentID, c => c.MapFrom(m => m.Menu.ParentID))
                            .ForMember(c => c.SequenceNo, c => c.MapFrom(m => m.Menu.SequenceNo))
                            .ForMember(c => c.IconClass, c => c.MapFrom(m => m.Menu.IconClass))
                            .ForMember(c => c.URL, c => c.MapFrom(m => m.Menu.Url))
                            // .ForMember(c => c.Help, c => c.MapFrom(m => m.Menu.Help))
                            .ForMember(c=>c.ActiveOnMobile,o=>o.MapFrom(s=>s.Menu.ActiveOnMobile))
                            .ForMember(c => c.IsActive, c => c.MapFrom(m => m.Menu.IsActive));
                        });
                        listMenu = Mapper.Map<List<Model.Menu>>(result);
                    }
                    else
                    {
                        var result = genericRepo.Get<DTOModel.DepartmentRight>(x => x.DepartmentID == departmentID
                        && x.Menu.IsActive == true 
                        && (x.Menu.ActiveOnMobile == (activeOnMobile ? true : x.Menu.ActiveOnMobile)));
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<DTOModel.DepartmentRight, Model.Menu>()
                            .ForMember(c => c.MenuID, c => c.MapFrom(m => m.MenuID))
                            .ForMember(d => d.MenuName, o => o.MapFrom(s => s.Menu.MenuName))
                            .ForMember(c => c.ParentID, c => c.MapFrom(m => m.Menu.ParentID))
                            .ForMember(c => c.SequenceNo, c => c.MapFrom(m => m.Menu.SequenceNo))
                            .ForMember(c => c.IconClass, c => c.MapFrom(m => m.Menu.IconClass))
                            .ForMember(c => c.URL, c => c.MapFrom(m => m.Menu.Url))
                            // .ForMember(c => c.Help, c => c.MapFrom(m => m.Menu.Help))
                            .ForMember(c => c.ActiveOnMobile, o => o.MapFrom(s => s.Menu.ActiveOnMobile))
                            .ForMember(c => c.IsActive, c => c.MapFrom(m => m.Menu.IsActive));
                        });
                        listMenu = Mapper.Map<List<Model.Menu>>(result);
                    }
                }
                else
                {
                    var getAllmenu = genericRepo.Get<DTOModel.Menu>(x => x.IsActive == true 
                    && x.IsDeleted == false && x.ActiveOnMobile==activeOnMobile ? true : x.ActiveOnMobile);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.Menu, Model.Menu>()
                        .ForMember(c => c.MenuID, c => c.MapFrom(m => m.MenuID))
                        .ForMember(d => d.MenuName, o => o.MapFrom(s => s.MenuName))
                        .ForMember(c => c.ParentID, c => c.MapFrom(m => m.ParentID))
                        .ForMember(c => c.SequenceNo, c => c.MapFrom(m => m.SequenceNo))
                        .ForMember(c => c.IconClass, c => c.MapFrom(m => m.IconClass))
                        .ForMember(c => c.URL, c => c.MapFrom(m => m.Url))
                        // .ForMember(c => c.Help, c => c.MapFrom(m => m.Help))
                         .ForMember(c => c.ActiveOnMobile, o => o.MapFrom(s => s.ActiveOnMobile))
                        .ForMember(c => c.IsActive, c => c.MapFrom(m => m.IsActive));
                    });
                    listMenu = Mapper.Map<List<Model.Menu>>(getAllmenu);
                }
                var dtolist = genericRepo.Get<DTOModel.Menu>(x => x.IsActive == true 
                && x.ActiveOnMobile==activeOnMobile ? true : x.ActiveOnMobile
                && x.IsDeleted == false);

                listMenu.ForEach(x =>
                {
                    x.ParentName = x.ParentID != null ? dtolist.Where(y => y.MenuID == x.ParentID).FirstOrDefault().MenuName : "";
                });
                return listMenu.OrderBy(x => x.SequenceNo).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }
        public List<Model.SelectListModel> GetParentList(int? menuID = null)
        {
            log.Info($"GetParentList");
            try
            {
                var result = genericRepo.Get<DTOModel.Menu>();
                if (menuID != null)
                {
                    result = result.Where(x => x.MenuID != menuID);
                }
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Menu, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.MenuID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.MenuName));
                });
                return Mapper.Map<List<Model.SelectListModel>>(result);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool MenuExists(int? parentID, int? menuID, string menuName)
        {
            return genericRepo.Exists<DTOModel.Menu>(x => x.ParentID == parentID && x.MenuID != menuID && x.MenuName == menuName);
        }
        public bool UpdateMenu(Model.Menu editMenuItem)
        {
            log.Info($"UpdateMenu");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.Menu>(editMenuItem.MenuID);
                dtoObj.MenuName = editMenuItem.MenuName;
                dtoObj.ParentID = editMenuItem.ParentID == 0 ? null : editMenuItem.ParentID;
                dtoObj.Url = editMenuItem.URL;
                dtoObj.IconClass = editMenuItem.IconClass;
                dtoObj.SequenceNo = editMenuItem.SequenceNo;
                dtoObj.ActiveOnMobile = editMenuItem.ActiveOnMobile;
                dtoObj.IsActive = editMenuItem.IsActive;
                dtoObj.Help = editMenuItem.Help;

                //Mapper.Initialize(cfg =>
                //{
                //    cfg.CreateMap<Model.Menu, Data.Models.Menu>()
                //    .ForMember(c => c.MenuID, c => c.MapFrom(m => m.MenuID))
                //    .ForMember(c => c.ParentID, c => c.MapFrom(m => m.ParentID))
                //    .ForMember(c => c.MenuName, c => c.MapFrom(m => m.MenuName))
                //    .ForMember(c => c.Url, c => c.MapFrom(m => m.URL))
                //    .ForMember(c => c.IconClass, c => c.MapFrom(m => m.IconClass))
                //    .ForMember(c => c.IsActive, c => c.MapFrom(m => m.IsActive))
                //     .ForMember(c => c.SequenceNo, c => c.MapFrom(m => m.SequenceNo));
                //});

                //var objDto = Mapper.Map<DTOModel.Menu>(editMenuItem);
                genericRepo.Update<DTOModel.Menu>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool InsertMenu(Model.Menu createMenu)
        {
            log.Info($"InsertMenu");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Menu, Data.Models.Menu>()
                    .ForMember(c => c.MenuID, c => c.MapFrom(m => m.MenuID))
                    .ForMember(c => c.ParentID, c => c.MapFrom(m => m.ParentID == 0 ? null : m.ParentID))
                    .ForMember(c => c.MenuName, c => c.MapFrom(m => m.MenuName))
                    .ForMember(c => c.Url, c => c.MapFrom(m => m.URL))
                    .ForMember(c => c.IconClass, c => c.MapFrom(m => m.IconClass))
                    .ForMember(c => c.SequenceNo, c => c.MapFrom(m => m.SequenceNo))
                    .ForMember(c => c.Help, c => c.MapFrom(m => m.Help))
                    .ForMember(c => c.ActiveOnMobile, c => c.MapFrom(m => m.ActiveOnMobile))
                    .ForMember(c => c.IsActive, c => c.MapFrom(m => m.IsActive))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoMenu = Mapper.Map<DTOModel.Menu>(createMenu);
                genericRepo.Insert<DTOModel.Menu>(dtoMenu);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.Menu GetMenuByID(int menuID)
        {
            log.Info($"GetMenuByID {menuID}");
            try
            {
                var menuObj = genericRepo.GetByID<DTOModel.Menu>(menuID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Menu, Model.Menu>()
                     .ForMember(c => c.MenuID, c => c.MapFrom(m => m.MenuID))
                    .ForMember(c => c.ParentID, c => c.MapFrom(m => m.ParentID))
                    .ForMember(c => c.MenuName, c => c.MapFrom(m => m.MenuName))
                    .ForMember(c => c.URL, c => c.MapFrom(m => m.Url))
                    .ForMember(c => c.IconClass, c => c.MapFrom(m => m.IconClass))
                    .ForMember(c => c.SequenceNo, c => c.MapFrom(m => m.SequenceNo))
                    .ForMember(c => c.Help, c => c.MapFrom(m => m.Help))
                    .ForMember(c => c.ActiveOnMobile, c => c.MapFrom(m => m.ActiveOnMobile))
                    .ForMember(c => c.IsActive, c => c.MapFrom(m => m.IsActive))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.Menu, Model.Menu>(menuObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(int menuID)
        {
            log.Info($"Delete");
            bool flag = false;
            try
            {
                //---Delete Menu from Department Rights--//
                var departmentRightsList = genericRepo.Get<DTOModel.DepartmentRight>(x => x.MenuID == menuID);
                foreach (var item in departmentRightsList)
                {
                    genericRepo.Delete<DTOModel.DepartmentRight>(item);
                }

                //---Delete Menu from User Rights--//
                var userRightsList = genericRepo.Get<DTOModel.UserRight>(x => x.MenuID == menuID);
                foreach (var item in userRightsList)
                {
                    genericRepo.Delete<DTOModel.UserRight>(item);
                }
                //---Delete Menu from Menu--//
                DTOModel.Menu dtoMenu = new DTOModel.Menu();
                dtoMenu = genericRepo.GetByID<DTOModel.Menu>(menuID);
                genericRepo.Delete<DTOModel.Menu>(dtoMenu);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public List<UserAccessRight> GetUserMenuRights(int userID, int deptID, int userTypeID)
        {
            log.Info($"MenuService/GetUserMenuRights/userID:{userID},deptID:{deptID}");

            List<UserAccessRight> menuRights = new List<UserAccessRight>();
            try
            {
                if (genericRepo.Exists<Data.Models.UserRight>(x => x.UserID == userID && x.ShowMenu))
                {
                    if (userTypeID == (int)SuperUser || userTypeID == (int)Admin)

                    {
                        var userLevelRights = genericRepo.Get<Data.Models.Menu>().Where(x=>x.IsActive==true). Select(em => new UserAccessRight()
                        {
                            MenuID = em.MenuID,
                            DepartmentID = deptID,
                            ShowMenu = true,
                            Create = true,
                            Edit = true,
                            Delete = true,
                            View = true,
                            menu = new Menu { MenuID = em.MenuID, URL = em.Url }
                        }).ToList();

                        userLevelRights.ForEach(x =>
                        {
                            menuRights.Add(new UserAccessRight
                            {
                                MenuID = x.MenuID,
                                DepartmentID = x.DepartmentID,
                                ShowMenu = true,
                                Create = true,
                                Edit = true,
                                Delete = true,
                                View = true,
                                menu = new Menu { MenuID = x.MenuID, URL = x.menu.URL }
                            });
                        });
                    }
                    else
                    {
                        var userLevelRights = genericRepo.Get<Data.Models.UserRight>(x => x.UserID == userID && x.ShowMenu).ToList();
                        userLevelRights.ForEach(x =>
                        {
                            menuRights.Add(new UserAccessRight
                            {
                                MenuID = x.MenuID,
                                DepartmentID = x.DepartmentID,
                                ShowMenu = x.ShowMenu,
                                Create = x.CreateRight,
                                Edit = x.EditRight,
                                Delete = x.DeleteRight,
                                View = x.ViewRight,
                                menu = new Menu { MenuID = x.Menu.MenuID, URL = x.Menu.Url }
                            });
                        });
                    }

                }
                else
                {
                    var deptLevelRights = genericRepo.Get<Data.Models.DepartmentRight>(x => x.DepartmentID == deptID && x.ShowMenu).ToList();
                    deptLevelRights.ForEach(x =>
                    {
                        menuRights.Add(new UserAccessRight
                        {
                            MenuID = x.MenuID,
                            DepartmentID = x.DepartmentID,
                            ShowMenu = x.ShowMenu,
                            Create = x.ShowMenu,
                            Edit = true,
                            Delete = true,
                            View = true,
                            menu = new Menu { MenuID = x.Menu.MenuID, URL = x.Menu.Url }
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return menuRights;
        }
    }
}
