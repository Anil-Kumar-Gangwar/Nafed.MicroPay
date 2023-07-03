using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using MicroPay.Web.Attributes;
using AutoMapper;

namespace MicroPay.Web.Controllers
{
    public class MenuRenderController : Controller
    {
        // GET: MenuRender
        private readonly IMenuService menuService;
        private readonly ILoginService loginService;
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger
 (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MenuRenderController(IMenuService menuService, ILoginService loginService)
        {
            this.menuService = menuService;
            this.loginService = loginService;
        }
       
        [ChildActionOnly]
        [SessionTimeout]
        public ActionResult Index()
        {
            log.Info($"MenuRenderController/Index");
            try
            {
                Model.UserDetail user = (Model.UserDetail)Session["User"];
               
                int userID = 0, departmentID = 0;
                userID = user.UserID;
                departmentID = user.DepartmentID;
                MenuViewModel objMenuItemListVM = new MenuViewModel();


                var menulist = menuService.GetMenuList(userID, departmentID, 
                    user.UserTypeID,user.DeviceTypeIsMobile);

                Mapper.Initialize(cfg => {

                    cfg.CreateMap<Model.Menu, MenuVM>();
                });

                objMenuItemListVM.MenuList = Mapper.Map<List<MenuVM>>(menulist);

                //objMenuItemListVM.MenuList = menuService.GetMenuList(userID, departmentID,user.UserTypeID);
                ViewBag.Menu = objMenuItemListVM.MenuList;
                return PartialView("_Menu");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public FileResult GetImage(string imgPath)
        {
            log.Info($"Get Item image path {imgPath}");
            byte[] imageByteData = ImageService.GetImage(imgPath);
            return File(imageByteData, "image/jpg;base64");
        }

        [SessionTimeout]

        public ActionResult SetMenuID(int menuID)
        {
            log.Info($"MenuRenderController/SetMenuID/{menuID}");
            Model.UserDetail user = (Model.UserDetail)Session["User"];
            if (menuID > 0)
            {
                SetSessionForUserAccessLevel(user.UserID, menuID, user.UserTypeID);
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }
        public void SetSessionForUserAccessLevel(int userID, int menuID, int userTypeID)
        {
            log.Info($"MenuRenderController/SetMenuID{userID}/{menuID}/{userTypeID}");
            try
            {
                Model.UserAccessRight userAccess = loginService.GetUserAccessRight(userID, menuID, userTypeID);
                Session["UserAccess"] = userAccess;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult UnAuthorizeAccess()
        {
            return View("UnAuthorizeAccess");
        }

        public ViewResult Menus()
        {
            log.Info("MenuRenderController/Menus");
            try
            {
                Model.UserDetail user = (Model.UserDetail)Session["User"];
                int userID = 0, departmentID = 0;
                userID = user.UserID;
                departmentID = user.DepartmentID;
                MenuViewModel objMenuItemListVM = new MenuViewModel();


                var menulist = menuService.GetMenuList(userID, departmentID,
                    user.UserTypeID, user.DeviceTypeIsMobile);

                Mapper.Initialize(cfg => {

                    cfg.CreateMap<Model.Menu, MenuVM>();
                });

                var MenuList= Mapper.Map<List<MenuVM>>(menulist);
                MenuList = MenuList.Where(x => x.MenuID != 1).ToList();
                return View("Index", MenuList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}