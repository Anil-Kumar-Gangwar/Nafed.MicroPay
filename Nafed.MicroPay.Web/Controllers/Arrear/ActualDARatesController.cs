using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.Arrear;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Controllers.Arrear
{
    public class ActualDARatesController : BaseController
    {
        private readonly IArrearService arrearService;

        public ActualDARatesController(IArrearService arrearService)
        {
            this.arrearService = arrearService;
        }
        // GET: ActualDARates
        public ActionResult Index()
        {
      
            ArrearManualDataViewModel ArrearManualDataVM = new ArrearManualDataViewModel();
            ArrearManualDataVM.userRights = userAccessRight;
            return View(ArrearManualDataVM);
        }

        public ActionResult ActualDARatesGridView(ArrearManualDataViewModel ArrearManualDataVM)
        {
            log.Info($"ActualDARatesController/ActualDARatesGridView");
            try
            {
                ArrearManualDataViewModel actualDAratesVM = new ArrearManualDataViewModel();
                actualDAratesVM.ArrearManualDataList = arrearService.GetDAratesList(ArrearManualDataVM.Month, ArrearManualDataVM.Year);
                actualDAratesVM.userRights = userAccessRight;
                return PartialView("_ActualDARatesGridView", actualDAratesVM);
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
            log.Info("ActualDARatesController/Create");
            try
            {
                Model.ArrearManualData objactualDArates = new Model.ArrearManualData();
                return View(objactualDArates);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Model.ArrearManualData createArrearmanualdata)
        {
            log.Info("ActualDARatesController/Create");
            try
            {
                
                if (createArrearmanualdata.Month == 0)
                {
                    ModelState.AddModelError("Month", "Please select month");
                    return View(createArrearmanualdata);
                }
                if (createArrearmanualdata.Year == 0)
                {
                    ModelState.AddModelError("Year", "Please select year");
                    return View(createArrearmanualdata);
                }
                if (createArrearmanualdata.E_01 == 0)
                {
                    ModelState.AddModelError("E_01", "Please enter rates");
                    return View(createArrearmanualdata);
                }
                ModelState.Remove("EmployeeId"); 
                ModelState.Remove("HeadName");
                if (ModelState.IsValid)
                {
                    bool chkdata = false;
                    chkdata = arrearService.DArateexists( createArrearmanualdata.Month, createArrearmanualdata.Year, createArrearmanualdata.E_01);
                    if (chkdata)
                    {
                        TempData["Error"] = "Data already exist for this period .";
                        return RedirectToAction("Create");
                    }

                    createArrearmanualdata.CreatedOn = DateTime.Now;
                    createArrearmanualdata.CreatedBy = userDetail.UserID;
                    createArrearmanualdata.Saldate = DateTime.Now;
                    int ID = arrearService.InsertDAratesdata(createArrearmanualdata);
                    if (ID > 0)
                    {
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
            return View(createArrearmanualdata);
        }

        [HttpGet]
        public ActionResult Edit(int ID)
        {
            log.Info("ActualDARatesController/Edit");
            try
            {
                Model.ArrearManualData objactualDArates = new Model.ArrearManualData();
                objactualDArates = arrearService.GetDAratesByID(ID);
                return View(objactualDArates);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.ArrearManualData editDARatesdata)
        {
            log.Info("ActualDARatesController/Edit");
            try
            {
                ModelState.Remove("EmployeeId");
                ModelState.Remove("HeadName");
                if (ModelState.IsValid)
                {
                    editDARatesdata.UpdatedBy = userDetail.UserID;
                    editDARatesdata.UpdatedOn = DateTime.Now;

                    int ID = arrearService.UpdateDAratesdata(editDARatesdata);
                    TempData["Message"] = "Succesfully Updated";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editDARatesdata);
        }

        public ActionResult Delete(int ID)
        {
            log.Info("ActualDARatesController/Delete");
            try
            {
                bool flag = false;
                flag = arrearService.DeleteDArates(ID);
                if (flag == true)
                {
                    TempData["Message"] = "Succesfully Deleted";
                }
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