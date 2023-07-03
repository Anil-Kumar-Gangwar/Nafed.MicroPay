using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.IO;
using AutoMapper;

namespace MicroPay.Web.Controllers.Leave
{
    public class LeaveCategoryController : BaseController
    {
        private readonly ILeaveService leaveCategoryService;
        private readonly IDropdownBindService dropdownBindService;
        public LeaveCategoryController(ILeaveService leaveCategoryService, IDropdownBindService dropdownBindService)
        {
            this.leaveCategoryService = leaveCategoryService;
            this.dropdownBindService = dropdownBindService;
        }

        public ActionResult Index()
        {
            log.Info($"LeaveCategoryController/Index");
            return View(userAccessRight);
        }


        public ActionResult LeaveCategoryGridView(FormCollection formCollection)
        {
            log.Info($"LeaveCategoryController/LaveCategoryGridView");
            try
            {
                LeaveCategoryViewModel LeaveCategoryVM = new LeaveCategoryViewModel();
                var leavecategorylist= leaveCategoryService.GetLeaveCategoryList();
                //LeaveCategoryVM = leaveCategoryService.GetLeaveCategoryList();

                Mapper.Initialize(cfg => {

                    cfg.CreateMap<Model.LeaveCategory, LeaveCategoryVM>();
                });
                LeaveCategoryVM.userRights = userAccessRight;
                LeaveCategoryVM.LeaveCategoryList = Mapper.Map<List<LeaveCategoryVM>>(leavecategorylist);
                return PartialView("_LeaveCategoryGridView", LeaveCategoryVM);

         
                //return PartialView("_LeaveCategoryGridView", LeaveCategoryVM);
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
            log.Info("LeaveCategoryController/Create");
            try
            {
                GetEmployeeType();
                LeaveCategoryVM objLeaveCategoryDetails = new LeaveCategoryVM();
                //Model.LeaveCategory objLeaveCategoryDetails = new Model.LeaveCategory();
                return View(objLeaveCategoryDetails);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LeaveCategoryVM createLeaveCategoryDetails)
        {
            log.Info("LeaveCategoryController/Create");
            try
            {
                GetEmployeeType();
                if (ModelState.IsValid)
                {
                    int exist = 0;
                    createLeaveCategoryDetails.LeaveCategoryName = createLeaveCategoryDetails.LeaveCategoryName.Trim();
                    Mapper.Initialize(cfg => {

                        cfg.CreateMap<LeaveCategoryVM, Model.LeaveCategory>();
                    });
                    var leavecategoryexist = Mapper.Map<Model.LeaveCategory>(createLeaveCategoryDetails);
                    leaveCategoryService.LeaveCategoryExists(leavecategoryexist, out exist);
                    //leaveCategoryService.LeaveCategoryExists(createLeaveCategoryDetails, out exist);
                    if (exist == 1) 
                        ModelState.AddModelError("LeaveCategoryAlreadyExists", "Leave Category Already Exist");
                    if (exist == 2) 
                        ModelState.AddModelError("LeaveCodeAlreadyExists", "Leave Code Already Exist");
                    if (exist == 3)
                    {
                        ModelState.AddModelError("LeaveCategoryAlreadyExists", "Leave Category Already Exist");
                        ModelState.AddModelError("LeaveCodeAlreadyExists", "Leave Code Already Exist");
                    }
                    if(exist == 0)
                    {
                        createLeaveCategoryDetails.CreatedBy = userDetail.UserID;
                        createLeaveCategoryDetails.CreatedOn = DateTime.Now;

                        Mapper.Initialize(cfg => {

                            cfg.CreateMap<LeaveCategoryVM, Model.LeaveCategory>();
                        });

                        var leavecategory = Mapper.Map<Model.LeaveCategory>(createLeaveCategoryDetails);
                        int leaveCategoryID = leaveCategoryService.InsertLeaveCategoryDetails(leavecategory);
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
            return View(createLeaveCategoryDetails);
        }

        [HttpGet]
        public ActionResult Edit(int leaveCategoryID)
        {
            log.Info("LeaveCategoryController/Edit");
            try
            {
                GetEmployeeType();
                ////Model.LeaveCategory objLeaveCategory = new Model.LeaveCategory();
                //objLeaveCategory = leaveCategoryService.GetLeaveCategoryByID(leaveCategoryID);
                //return View(objLeaveCategory);

                Model.LeaveCategory objLeaveCategory = new Model.LeaveCategory();
                objLeaveCategory = leaveCategoryService.GetLeaveCategoryByID(leaveCategoryID);
                Mapper.Initialize(cfg => {

                    cfg.CreateMap<Model.LeaveCategory, LeaveCategoryVM>();
                });

                var leavecategory = Mapper.Map<LeaveCategoryVM>(objLeaveCategory);
                return View(leavecategory);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LeaveCategoryVM updateLeaveCategory)
        {
            log.Info("LeaveCategoryController/Edit");
            try
            {
                GetEmployeeType();
                if (ModelState.IsValid)
                {
                    int exist = 0;
                    updateLeaveCategory.LeaveCategoryName = updateLeaveCategory.LeaveCategoryName.Trim();

                    Mapper.Initialize(cfg => {

                        cfg.CreateMap<LeaveCategoryVM, Model.LeaveCategory>();
                    });
                    var leavecategoryexist = Mapper.Map<Model.LeaveCategory>(updateLeaveCategory);
                    leaveCategoryService.LeaveCategoryExists(leavecategoryexist, out exist);

                    //leaveCategoryService.LeaveCategoryExists(updateLeaveCategory, out exist);
                    if (exist == 1)
                        ModelState.AddModelError("LeaveCategoryAlreadyExists", "Leave Category Already Exist");
                    if (exist == 2)
                        ModelState.AddModelError("LeaveCodeAlreadyExists", "Leave Code Already Exist");
                    if (exist == 3)
                    {
                        ModelState.AddModelError("LeaveCategoryAlreadyExists", "Leave Category Already Exist");
                        ModelState.AddModelError("LeaveCodeAlreadyExists", "Leave Code Already Exist");
                    }
                    if(exist == 0)
                    {
                        Mapper.Initialize(cfg => {

                            cfg.CreateMap<LeaveCategoryVM, Model.LeaveCategory>();
                        });
                        var leavecategory = Mapper.Map<Model.LeaveCategory>(updateLeaveCategory);
                        leaveCategoryService.UpdateLeaveCategoryDetails(leavecategory);
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
            return View(updateLeaveCategory);

        }

        public ActionResult Delete(int leaveCategoryID)
        {
            log.Info("LeaveCategoryController/Delete");
            try
            {
                leaveCategoryService.DeleteLeaveCategory(leaveCategoryID);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }


        private void GetEmployeeType()
        {
            log.Info("LeavecategoryController/GetEmployeeType");
            var employeetype = dropdownBindService.ddlEmployeeTypeList();
            Model.SelectListModel select = new Model.SelectListModel();
            select.id = 0;
            select.value = "Select";
            employeetype.Insert(0, select);
            ViewBag.employeetype = new SelectList(employeetype, "id", "value");
        }


    }
}