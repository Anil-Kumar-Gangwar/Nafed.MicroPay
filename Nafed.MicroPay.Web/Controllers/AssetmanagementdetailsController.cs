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
    public class AssetManagementDetailsController : BaseController
    {
        private readonly IInventoryManagementService imService;
        private readonly IDropdownBindService ddlService;

        public AssetManagementDetailsController(IInventoryManagementService imService, IDropdownBindService ddlService)
        {
            this.imService = imService;
            this.ddlService = ddlService;
        }
        // GET: Assetmanagementdetails
        public ActionResult Index()
        {
            BindDropdown();
            return View();
        }

        public void BindDropdown()
        {
            var assetTypeList = ddlService.GetAssetType();
            ViewBag.AssetTypeList = new SelectList(assetTypeList, "id", "value");

            List<Model.SelectListModel> selectType = new List<Model.SelectListModel>();
            ViewBag.AssetName = new SelectList(selectType, "id", "value");

            var ddlEmployee = ddlService.GetAllEmployee();
            Model.SelectListModel selectEmployee = new Model.SelectListModel();
            selectEmployee.id = 0;
            selectEmployee.value = "Select";
            ddlEmployee.Insert(0, selectEmployee);
            ViewBag.Employee = new SelectList(ddlEmployee, "id", "value");
            List<Model.SelectListModel> status = new List<Model.SelectListModel>();
            status.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            status.Add(new Model.SelectListModel() { value = "In Stock", id = 1 });
            status.Add(new Model.SelectListModel() { value = "Damage", id = 2 });
            status.Add(new Model.SelectListModel() { value = "Missing", id = 3 });
            status.Add(new Model.SelectListModel() { value = "Allocated", id = 4 });
            status.Add(new Model.SelectListModel() { value = "Repair", id = 5 });
            ViewBag.Status = new SelectList(status, "id", "value");
        }
            

        public void GetAssetList(int statusID, int assettypeID)
        {
            try
            {
                var details = ddlService.GetAssetName(statusID, assettypeID);
                Model.SelectListModel assetDetails = new Model.SelectListModel();
                assetDetails.id = 0;
                assetDetails.value = "Select";
                details.Insert(0, assetDetails);
                var assetname = new SelectList(details, "id", "value");
                ViewBag.AssetName = assetname;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        public JsonResult GetAssetName(int statusID, int assettypeID)
        {
            try
            {
                var details = ddlService.GetAssetName(statusID, assettypeID);
                Model.SelectListModel assetDetails = new Model.SelectListModel();
                assetDetails.id = 0;
                assetDetails.value = "Select";
                details.Insert(0, assetDetails);
                var assetname = new SelectList(details, "id", "value");
                return Json(new { assetname = assetname }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        public ActionResult AssetsManagementGridView(InventorymanagementViewModel IMVM)
        {
            log.Info($"AssetmanagementdetailsController/AssetManagementGridView");
            try
            {
                IMVM.listInventoryManagement = imService.GetAssetManagementList(IMVM.AssetTypeID, IMVM.IMID, IMVM.EmployeeID);
                IMVM.userRights = userAccessRight;
                return PartialView("_AssetsManagementGridView", IMVM);
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
            log.Info("AssetmanagementdetailsController/Create");
            try
            {
                Model.InventoryManagement objAssetManagement = new Model.InventoryManagement();
                BindDropdown();

                List<Model.SelectListModel> status = new List<Model.SelectListModel>();
                status.Add(new Model.SelectListModel() { value = "Select", id = 0 });              
                status.Add(new Model.SelectListModel() { value = "Allocated", id = 4 });               
                ViewBag.Status = new SelectList(status, "id", "value");

                List<Model.SelectListModel> selectType = new List<Model.SelectListModel>();
                ViewBag.AssetName = new SelectList(selectType, "id", "value");
                return View(objAssetManagement);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.InventoryManagement createAssetManagement)
        {
            log.Info("AssetmanagementdetailsController/Create");
            try
            {
                ModelState.Remove("SerialNo");
                ModelState.Remove("Price");
                ModelState.Remove("ManufacturerID");
                ModelState.Remove("AssetName");
                if (ModelState.IsValid)
                {
                    if (createAssetManagement.StatusID == 4 && createAssetManagement.DeAllocationDate.HasValue)
                    {
                        BindDropdown();
                        List<Model.SelectListModel> status = new List<Model.SelectListModel>();
                        status.Add(new Model.SelectListModel() { value = "Select", id = 0 });
                        status.Add(new Model.SelectListModel() { value = "Allocated", id = 4 });
                        ViewBag.Status = new SelectList(status, "id", "value");
                        GetAssetList(1, createAssetManagement.AssetTypeID);
                        ModelState.AddModelError("DeAllocationDate", "You can't provide Deallocation date, when the status is Allocated.");
                        return View(createAssetManagement);
                    }

                    if (!string.IsNullOrEmpty(createAssetManagement.Email))
                    {
                        int indexOfEmail = createAssetManagement.Email.IndexOf('@');
                        string Emaildomain = createAssetManagement.Email.Substring(indexOfEmail + 1);
                        if (!Emaildomain.Equals("nafed-india.com"))
                        {
                            BindDropdown();
                            List<Model.SelectListModel> status = new List<Model.SelectListModel>();
                            status.Add(new Model.SelectListModel() { value = "Select", id = 0 });
                            status.Add(new Model.SelectListModel() { value = "Allocated", id = 4 });
                            ViewBag.Status = new SelectList(status, "id", "value");
                            GetAssetList(1, createAssetManagement.AssetTypeID);
                            ModelState.AddModelError("Email", "Email Domain not matched with nafed-india.com,please enter valid email.");
                            return View(createAssetManagement);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(createAssetManagement.empEmail))
                        {
                            int indexOfAt = createAssetManagement.empEmail.IndexOf('@');
                            string domain = createAssetManagement.empEmail.Substring(indexOfAt + 1);
                            if (!domain.Equals("nafed-india.com"))
                            {
                                BindDropdown();
                                List<Model.SelectListModel> status = new List<Model.SelectListModel>();
                                status.Add(new Model.SelectListModel() { value = "Select", id = 0 });
                                status.Add(new Model.SelectListModel() { value = "Allocated", id = 4 });
                                ViewBag.Status = new SelectList(status, "id", "value");
                                GetAssetList(1, createAssetManagement.AssetTypeID);
                                ModelState.AddModelError("empEmailIDValidator", "Email Domain not matched with nafed-india.com,please update you official Email Id.");
                                return View(createAssetManagement);
                            }
                        }
                    }
                    createAssetManagement.CreatedBy = userDetail.UserID;
                    createAssetManagement.CreatedOn = DateTime.Now;
                    imService.InsertAssetManagement(createAssetManagement);
                    TempData["Message"] = "Successfully Created";
                    return RedirectToAction("Index");
                }
                else
                {
                    BindDropdown();
                    List<Model.SelectListModel> status = new List<Model.SelectListModel>();
                    status.Add(new Model.SelectListModel() { value = "Select", id = 0 });
                    status.Add(new Model.SelectListModel() { value = "Allocated", id = 4 });
                    ViewBag.Status = new SelectList(status, "id", "value");
                    GetAssetList(1, createAssetManagement.AssetTypeID);
                    return View(createAssetManagement);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int ID)
        {
            log.Info("AssetmanagementdetailsController/Edit");
            try
            {
                Model.InventoryManagement objAssetManagement = new Model.InventoryManagement();
                BindDropdown();
                objAssetManagement = imService.GetAssetManagementID(ID);
                var assetTypeList = ddlService.GetAssetName(0, objAssetManagement.AssetTypeID);
                ViewBag.AssetName = new SelectList(assetTypeList, "id", "value");
                return View(objAssetManagement);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.InventoryManagement editAssetManagement)
        {
            log.Info("InventoryManagementController/Edit");
            try
            {
                ModelState.Remove("SerialNo");
                ModelState.Remove("Price");
                ModelState.Remove("ManufacturerID");
                ModelState.Remove("AssetName");
                if (ModelState.IsValid)
                {
                    if (editAssetManagement.StatusID == 4 && editAssetManagement.DeAllocationDate.HasValue)
                    {
                        BindDropdown();                        
                        var assetTypeList = ddlService.GetAssetName(0, editAssetManagement.AssetTypeID);
                        ViewBag.AssetName = new SelectList(assetTypeList, "id", "value");
                        ModelState.AddModelError("DeAllocationDate", "You can't provide Deallocation date, when the status is Allocated.");
                        return View(editAssetManagement);
                    }
                    if (editAssetManagement.StatusID != 4 && !editAssetManagement.DeAllocationDate.HasValue)
                    {
                        BindDropdown();
                        var assetTypeList = ddlService.GetAssetName(0, editAssetManagement.AssetTypeID);
                        ViewBag.AssetName = new SelectList(assetTypeList, "id", "value");
                        ModelState.AddModelError("DeAllocationDate", "Please provide Deallocation date.");
                        return View(editAssetManagement);
                    }
                    editAssetManagement.UpdatedBy = userDetail.UserID;
                    editAssetManagement.UpdatedOn = System.DateTime.Now;
                    imService.UpdateAssetManagement(editAssetManagement);
                    TempData["Message"] = "successfully Updated";
                    return RedirectToAction("Index");

                }
                else
                {
                    BindDropdown();
                    return View(editAssetManagement);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editAssetManagement);
        }

        public ActionResult Delete(int ID)
        {
            log.Info("AssetmanagementdetailsController/Delete");
            try
            {
                imService.Deleteassetdetails(ID);
                TempData["Message"] = "successfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }

        public JsonResult GetEmployeeEmail(int employeeID)
        {
            try
            {
                var empEmailID = imService.GetEmployeeEmail(employeeID);
                int indexOfAt = empEmailID.IndexOf('@');
                string domain = empEmailID.Substring(indexOfAt + 1);
                return Json(new { empEmailID = empEmailID,domainname= domain }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }
    }
}