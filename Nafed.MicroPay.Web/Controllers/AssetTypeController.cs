using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
namespace MicroPay.Web.Controllers
{
    public class AssetTypeController : BaseController
    {
        private readonly IAssetTypeService assetTypeService;

        public AssetTypeController(IAssetTypeService assetTypeService)
        {
            this.assetTypeService = assetTypeService;
        }
        // GET: AssetType
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AssetTypeGridView()
        {
            log.Info($"AssetTypeController/AssetTypeGridView");
            try
            {
               List<AssetType> AssetType = new List<AssetType>();
                AssetType = assetTypeService.GetAssetTypeList(true);
                return PartialView("_AssetTypeGridView", AssetType);
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
            log.Info("AssetTypeController/Create");
            try
            {         
                AssetType objAssetType = new AssetType();
                return View(objAssetType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

     
        [HttpPost]
        public ActionResult Create(AssetType createAssetType)
        {
            log.Info("AssetTypeController/Create");
            try
            {             
                if (ModelState.IsValid)
                {
                    createAssetType.AssetTypeName = createAssetType.AssetTypeName.Trim();
                    if (assetTypeService.AssetTypeExists(createAssetType.AssetTypeName))
                    {
                        ModelState.AddModelError("AssetTypeAlreadyExist", "Asset Type Already Exist");
                    }
                    else
                    {
                        createAssetType.CreatedBy = userDetail.UserID;
                        createAssetType.CreatedOn = DateTime.Now;
                        assetTypeService.InsertAssetType(createAssetType);
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
            return View(createAssetType);
        }

        [HttpGet]
        public ActionResult Edit(int AssetTypeID)
        {
            log.Info("AssetTypeController/Edit");
            try
            {
               
                AssetType objAssetType = new AssetType();
                objAssetType = assetTypeService.GetAssetTypeID(AssetTypeID);
                return View(objAssetType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(AssetType editAssetType)
        {
            log.Info("AssetTypeController/Edit");
            try
            {
              
                if (ModelState.IsValid)
                {
                    editAssetType.AssetTypeName = editAssetType.AssetTypeName.Trim();
                    editAssetType.UpdatedBy = userDetail.UserID;
                    editAssetType.UpdatedOn = System.DateTime.Now;
                    //if (assetTypeService.AssetTypeExists(editAssetType.AssetType1))
                    //{
                    //    ModelState.AddModelError("AssetTypeAlreadyExist", "AssetType Already Exist");
                    //}
                    //else
                    //{
                        assetTypeService.UpdateAssetType(editAssetType);
                        TempData["Message"] = "successfully Updated";
                        return RedirectToAction("Index");
                   // }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editAssetType);
        }

        public ActionResult Delete(int AssetTypeID)
        {
            log.Info("AssetTypeController/Delete");
            try
            {
                assetTypeService.Delete(AssetTypeID);
                TempData["Message"] = "Successfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                if (ex.InnerException.InnerException.HResult == -2146232060)             
                    TempData["Error"] = "You can't delete Asset Type, because it is in used.";            
                else
                    TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }
    }
}