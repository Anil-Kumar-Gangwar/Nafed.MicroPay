using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Model = Nafed.MicroPay.Model;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers
{
    public class HolidayController : BaseController
    {
        private readonly IHolidayService holidayService;
        private readonly IDropdownBindService dropdownBindService;
        public HolidayController(IHolidayService holidayService, IDropdownBindService dropdownBindService)
        {
            this.holidayService = holidayService;
            this.dropdownBindService = dropdownBindService;
        }
        public ActionResult Index()
        {
            HolidayViewModel holidayVM = new HolidayViewModel();
            var CYear = DateTime.Now.Year;

            var cList = new List<Model.SelectListModel>();
            cList.Add(new Model.SelectListModel { id = CYear, value = CYear.ToString() });
            cList.Add(new Model.SelectListModel { id = CYear + 1, value = (CYear + 1).ToString() });
            holidayVM.CYearList = cList;
            holidayVM.BranchList = dropdownBindService.ddlBranchList();
            holidayVM.userRights = userAccessRight;
            return View(holidayVM);
        }

        public ActionResult GetCurrentHoliday()
        {
            log.Info($"HolidayController/GetCurrentHoliday");
            try
            {
                List<Model.Holiday> objHolidayList = new List<Model.Holiday>();
                objHolidayList = holidayService.GetHolidayList(userDetail.BranchID, DateTime.Now.Year);
                return PartialView("_HolidayGridView", objHolidayList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult HolidayGridView(HolidayViewModel holidayVM)
        {
            log.Info($"HolidayController/HolidayGridView");
            try
            {
                List<Model.Holiday> objHolidayList = new List<Model.Holiday>();
                objHolidayList = holidayService.GetHolidayList(holidayVM.BranchID, holidayVM.CYear);
                return PartialView("_HolidayGridView", objHolidayList);
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
            log.Info("HolidayController/Create");
            try
            {
                GetBranch();
                var CYear = DateTime.Now.Year;

                var cList = new List<Model.SelectListModel>();
                cList.Add(new Model.SelectListModel { id = CYear, value = CYear.ToString() });
                cList.Add(new Model.SelectListModel { id = CYear + 1, value = (CYear + 1).ToString() });
               

                Model.Holiday objHoliday = new Model.Holiday();
                objHoliday.CYearList = cList;
                return View(objHoliday);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.Holiday createHoliday)
        {
            log.Info("HolidayController/Create");
            try
            {
                GetBranch();
                //if (createHoliday.BranchId == 0)
                //{
                //    ModelState.AddModelError("HolidayBankRequired", "Please Select Branch");
                //    return View(createHoliday);
                //}

                var CYear = DateTime.Now.Year;
                var cList = new List<Model.SelectListModel>();
                cList.Add(new Model.SelectListModel { id= CYear,  value= CYear.ToString()});
                cList.Add(new Model.SelectListModel { id = CYear+1, value = (CYear + 1).ToString() });
               
                createHoliday.CYearList = cList;

                ModelState.Remove("BranchId");
                if (ModelState.IsValid)
                {
                    createHoliday.HolidayName = createHoliday.HolidayName.Trim();
                    createHoliday.HolidayDate = createHoliday.HolidayDate;
                    //createHoliday.BranchId = createHoliday.BranchId;
                    createHoliday.CreatedBy = userDetail.UserID;
                    createHoliday.CreatedOn = System.DateTime.Now;

                    if (holidayService.HolidayNameExists(createHoliday.HolidayDate, createHoliday.HolidayName, createHoliday.BranchId,createHoliday.CYear))
                    {
                        ModelState.AddModelError("HolidayNameAlreadyExist", "Holiday Name Already Exist");
                        return View(createHoliday);
                    }
                    else
                    {
                        createHoliday.BranchId = createHoliday.BranchId.HasValue && createHoliday.BranchId.Value == 0 ? null : createHoliday.BranchId;
                        holidayService.InsertHoliday(createHoliday);
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
            return View(createHoliday);
        }

        [HttpGet]
        public ActionResult Edit(int holidayID)
        {
            log.Info($"HolidayItemController/Edit{holidayID}");
            try
            {
                GetBranch();
                Model.Holiday objHoliday = new Model.Holiday();
               
                objHoliday = holidayService.GetHolidayByID(holidayID);

                var CYear = DateTime.Now.Year;
                var cList = new List<Model.SelectListModel>();
                cList.Add(new Model.SelectListModel { id = CYear, value = CYear.ToString() });
                cList.Add(new Model.SelectListModel { id = CYear + 1, value = (CYear + 1).ToString() });

                objHoliday.CYearList = cList;

                return View(objHoliday);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.Holiday editHoliday)
        {
            log.Info("HolidayItemController/Edit");
            try
            {
                GetBranch();

                var CYear = DateTime.Now.Year;
                var cList = new List<Model.SelectListModel>();
                cList.Add(new Model.SelectListModel { id = CYear, value = CYear.ToString() });
                cList.Add(new Model.SelectListModel { id = CYear + 1, value = (CYear + 1).ToString() });

                editHoliday.CYearList = cList;

                ModelState.Remove("BranchId");
                if (ModelState.IsValid)
                {
                    editHoliday.HolidayName = editHoliday.HolidayName.Trim();
                    editHoliday.HolidayDate = editHoliday.HolidayDate;
                    editHoliday.BranchId = editHoliday.BranchId == 0 ? null : editHoliday.BranchId;
                    editHoliday.UpdatedBy = userDetail.UserID;
                    editHoliday.UpdatedOn = System.DateTime.Now;
                    //if (holidayService.HolidayNameExists(editHoliday.HolidayDate, editHoliday.HolidayName, editHoliday.BranchId))
                    //{
                    //    ModelState.AddModelError("HolidayNameAlreadyExist", "Holiday Name Already Exist");
                    //    return View(editHoliday);
                    //}

                    //else
                    //{
                    if (editHoliday.HolidayID != 0)
                    {
                        holidayService.UpdateHoliday(editHoliday);
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
            return View(editHoliday);
        }

        public ActionResult Delete(int holidayID)
        {
            log.Info($"HolidayController/Delete{holidayID}");
            try
            {
                holidayService.Delete(holidayID);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }

        private void GetBranch()
        {
            log.Info("HolidayController/GetBranch");
            var branch = dropdownBindService.ddlBranchList();
            Model.SelectListModel select = new Model.SelectListModel();
            select.id = 0;
            select.value = "All";
            branch.Insert(0, select);
            ViewBag.Branch = new SelectList(branch, "id", "value");
        }
    }
}