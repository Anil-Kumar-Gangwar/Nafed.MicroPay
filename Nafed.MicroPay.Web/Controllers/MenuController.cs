using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using AutoMapper;

namespace MicroPay.Web.Controllers
{
    public class MenuController : BaseController
    {
        private readonly IMenuService menuService;
        public MenuController(IMenuService menuService)
        {
            this.menuService = menuService;
        }
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult MenuGridView(FormCollection formCollection)
        {
            log.Info($"MenuController/MenuGridView");
            try
            {
                MenuViewModel objMenuItemListVM = new MenuViewModel();
                var menulist= menuService.GetMenuList();

                Mapper.Initialize(cfg => {

                    cfg.CreateMap<Model.Menu,MenuVM>();
                });

                objMenuItemListVM.MenuList = Mapper.Map<List<MenuVM>>(menulist);
                return PartialView("_MenuGridView", objMenuItemListVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info("MenuController/Create");
            try
            {
                var menuParentList = menuService.GetParentList();
                Model.SelectListModel select = new Model.SelectListModel();
                select.id = 0;
                select.value = "Select";
                menuParentList.Insert(0, select);
                ViewBag.ParentList = new SelectList(menuParentList, "id", "value");
                MenuVM objMenu = new MenuVM();
                return View(objMenu);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(MenuVM createMenu)
        {
            log.Info("MenuController/Create");
            try
            {
                var menuParentList = menuService.GetParentList();
                Model.SelectListModel select = new Model.SelectListModel();
                select.id = 0;
                select.value = "Select";
                menuParentList.Insert(0, select);
                ViewBag.ParentList = new SelectList(menuParentList, "id", "value");

                if (ModelState.IsValid)
                {
                    createMenu.MenuName = createMenu.MenuName.Trim();
                    if (menuService.MenuExists(createMenu.ParentID == 0 ? null : createMenu.ParentID, createMenu.MenuID, createMenu.MenuName))
                    {
                        ModelState.AddModelError("RecordAlreadyExist", "Record Already Exist");
                        return View(createMenu);
                    }
                    else
                    {
                        createMenu.CreatedBy = userDetail.UserID;
                        createMenu.CreatedOn = System.DateTime.Now;

                        //===== Convert MenuVM to Model.Menu//============

                        Mapper.Initialize(cfg => {

                            cfg.CreateMap<MenuVM,Model.Menu>();
                        });

                        var menus = Mapper.Map<Model.Menu>(createMenu);
                        menuService.InsertMenu(menus);
                        TempData["Message"] = "Successfully Created";
                        return RedirectToAction("Index");
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(createMenu);
        }

        [HttpGet]
        public ActionResult Edit(int menuID)
        {
            log.Info("MenuItemController/Edit");
            try
            {
                var menuParentList = menuService.GetParentList(menuID);
                Model.SelectListModel select = new Model.SelectListModel();
                select.id = 0;
                select.value = "Select";
                menuParentList.Insert(0, select);
                ViewBag.ParentList = new SelectList(menuParentList, "id", "value");
                Model.Menu objMenu = new Model.Menu();
                objMenu = menuService.GetMenuByID(menuID);

                Mapper.Initialize(cfg => {

                    cfg.CreateMap<Model.Menu,MenuVM >();
                });

                var menus = Mapper.Map<MenuVM>(objMenu);


                return View(menus);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(MenuVM editMenu)
        {
            log.Info("MenuItemController/Edit");
            try
            {
                var menuParentList = menuService.GetParentList(editMenu.MenuID);
                Model.SelectListModel select = new Model.SelectListModel();
                select.id = 0;
                select.value = "Select";
                menuParentList.Insert(0, select);
                ViewBag.ParentList = new SelectList(menuParentList, "id", "value");
                if (ModelState.IsValid)
                {
                    editMenu.MenuName = editMenu.MenuName.Trim();
                    if (menuService.MenuExists(editMenu.ParentID, editMenu.MenuID, editMenu.MenuName))
                    {
                        ModelState.AddModelError("RecordAlreadyExist", "RecordAlreadyExist");
                        return View(editMenu);
                    }
                    else
                    {

                        Mapper.Initialize(cfg => {

                            cfg.CreateMap<MenuVM, Model.Menu>();
                        });

                        var menus = Mapper.Map<Model.Menu>(editMenu);

                        menuService.UpdateMenu(menus);
                        TempData["Message"] = "Succesfully Updated";
                        return RedirectToAction("Index");
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editMenu);

        }


        public ActionResult Delete(int menuID)
        {
            log.Info("MenuItemController/Delete");
            try
            {
                menuService.Delete(menuID);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }
    }
}