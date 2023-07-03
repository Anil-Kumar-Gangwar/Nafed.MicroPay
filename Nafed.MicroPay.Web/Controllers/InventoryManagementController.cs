using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model = Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using MicroPay.Web.Models;
namespace MicroPay.Web.Controllers
{
    public class InventoryManagementController : BaseController
    {
        private readonly IInventoryManagementService imService;
        private readonly IDropdownBindService dropDownService;
        public InventoryManagementController(IInventoryManagementService imService, IDropdownBindService dropDownService)
        {
            this.imService = imService;
            this.dropDownService = dropDownService;
        }
        // GET: InventoryManagement
        public ActionResult Index()
        {
            BindDropdown();
            return View();
        }

        public ActionResult InventoryManagementGridView(InventorymanagementViewModel IMVM)
        {
            log.Info($"InventoryManagementController/InventoryManagementGridView");
            try
            {
                //InventorymanagementViewModel InventoryManagement = new InventorymanagementViewModel();
                //List<Model.InventoryManagement> objINVM = new List<Model.InventoryManagement>();
                IMVM.listInventoryManagement = imService.GetInventoryManagementList(IMVM.AssetTypeID, IMVM.ManufacturerID);
                IMVM.userRights = userAccessRight;
                return PartialView("_InventoryManagementGridView", IMVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdown()
        {
            var assetTypeList = dropDownService.GetAssetType();         
            ViewBag.AssetTypeList = new SelectList(assetTypeList, "id", "value");

            var manufacturerList = dropDownService.GetManufacturer();
            ViewBag.ManufacturerList = new SelectList(manufacturerList, "id", "value");

            List<Model.SelectListModel> status = new List<Model.SelectListModel>();
            status.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            status.Add(new Model.SelectListModel() { value = "In Stock", id = 1 });
            status.Add(new Model.SelectListModel() { value = "Damage", id = 2 });
            status.Add(new Model.SelectListModel() { value = "Missing", id = 3 });
           // status.Add(new Model.SelectListModel() { value = "Allocated", id = 4 });
            status.Add(new Model.SelectListModel() { value = "Repair", id = 5 });
            ViewBag.Status = new SelectList(status, "id", "value");

            var MYear = DateTime.Now.Year;
            var MYearList = new List<Model.SelectListModel>();
            for (int i = 2006; i <= MYear; i++)
            {
                MYearList.Add(new Model.SelectListModel { id =i , value = (i).ToString() });
            }
           var yearList = new SelectList(MYearList, "id", "value");
            ViewBag.CYearList = yearList.OrderByDescending(x => x.Value);
        }
        [HttpGet]
        public ActionResult Create()
        {
            log.Info("InventoryManagementController/Create");
            try
            {
                Model.InventoryManagement objInventoryManagement = new Model.InventoryManagement();
                BindDropdown();
                return View(objInventoryManagement);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.InventoryManagement createInventoryManagement)
        {
            log.Info("InventoryManagementController/Create");
            try
            {
                ModelState.Remove("AllocationDate");
                ModelState.Remove("EmployeeID");
                ModelState.Remove("IMID");
                if (ModelState.IsValid)
                {

                    if (imService.InventoryManagementExists(createInventoryManagement.SerialNo))
                    {
                        ModelState.AddModelError("InventoryManagementAlreadyExist", "InventoryManagement Already Exist");
                    }
                    else
                    {
                        createInventoryManagement.CreatedBy = userDetail.UserID;
                        createInventoryManagement.CreatedOn = DateTime.Now;
                        imService.InsertInventoryManagement(createInventoryManagement);
                        TempData["Message"] = "Successfully Created";
                        return RedirectToAction("Index");
                    }
                }
                else {
                    BindDropdown();
                    return View(createInventoryManagement);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(createInventoryManagement);
        }

        [HttpGet]
        public ActionResult Edit(int imid)
        {
            log.Info("InventoryManagementController/Edit");
            try
            {

                Model.InventoryManagement objInventoryManagement = new Model.InventoryManagement();
                BindDropdown();
                objInventoryManagement = imService.GetInventoryManagementID(imid);
                return View(objInventoryManagement);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.InventoryManagement editInventoryManagement)
        {
            log.Info("InventoryManagementController/Edit");
            try
            {
                ModelState.Remove("AllocationDate");
                ModelState.Remove("EmployeeID");
                ModelState.Remove("IMID");
                if (ModelState.IsValid)
                {
                    ModelState.Remove("AllocationDate");
                    ModelState.Remove("EmployeeID");
                    editInventoryManagement.UpdatedBy = userDetail.UserID;
                    editInventoryManagement.UpdatedOn = System.DateTime.Now;                   
                    imService.UpdateInventoryManagement(editInventoryManagement);
                    TempData["Message"] = "successfully Updated";
                    return RedirectToAction("Index");
                 
                }
                else
                {
                    BindDropdown();
                    return View(editInventoryManagement);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editInventoryManagement);
        }

        public ActionResult Delete(int imid)
        {
            log.Info("InventoryManagementController/Delete");
            try
            {
                imService.Delete(imid);
                TempData["Message"] = "successfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }

        public ActionResult AssetHistory(int IMID, string assetType, string asset)
        {
            log.Info($"InventoryManagementController/AssetHistory/imid={IMID}");
            try
            {
                InventorymanagementViewModel INVM = new InventorymanagementViewModel();
                INVM.AssetType = assetType;
                INVM.Asset = asset;
                INVM.listInventoryManagement = imService.GetAssetHistory(IMID);
                return PartialView("_AssetsManagementHistory", INVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}