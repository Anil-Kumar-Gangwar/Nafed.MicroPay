using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;

namespace MicroPay.Web.Controllers
{
    public class FileTrackingTypeController : BaseController
    {
        private readonly IFileTrackingSytemService fileTrackingServ;

        public FileTrackingTypeController(IFileTrackingSytemService fileTrackingServ)
        {
            this.fileTrackingServ = fileTrackingServ;

        }
        // GET: FileTrackingType
        public ActionResult Index()
        {
            log.Info($"FileTrackingTypeController/Index");
            return View(userAccessRight);
        }

        public ActionResult _GetFileTrackingTypeGridView()
        {
            log.Info("FileTrackingTypeController/_GetFileTrackingTypeGridView");
            try
            {
                List<FileTrackingType> objFileTypeList = new List<FileTrackingType>();
                objFileTypeList = fileTrackingServ.GetFileTrackingTypeList();
                return PartialView("_FileTrackingTypeList", objFileTypeList);
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
            log.Info("FileTrackingTypeController/Create");
            try
            {
                FileTrackingType objFileType = new FileTrackingType();
                return View(objFileType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FileTrackingType createFileType)
        {
            log.Info("FileTrackingTypeController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createFileType.FileType = createFileType.FileType.Trim();
                    createFileType.CreatedBy = userDetail.UserID;
                    createFileType.CreatedOn = DateTime.Now;
                    if (string.IsNullOrEmpty(createFileType.FileType))
                        ModelState.AddModelError("FileTypeAlreadyExist", "Please Enter File Type Name.");

                    else if (fileTrackingServ.FileTypeExists(createFileType.FileType))
                        ModelState.AddModelError("FileTypeAlreadyExist", "File Type Name Already Exist.");

                    else
                    {
                        int acadmicProfessionalID = fileTrackingServ.InsertFileTrackingType(createFileType);
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
            return View(createFileType);
        }

        [HttpGet]
        public ActionResult Edit(int fileTypeID)
        {
            log.Info("FileTrackingTypeController/Edit");
            try
            {
                FileTrackingType objFileType = new FileTrackingType();
                objFileType = fileTrackingServ.GetFileTrackingTypeByID(fileTypeID);
                return View(objFileType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FileTrackingType createFileType)
        {
            log.Info("FileTrackingTypeController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    createFileType.FileType = createFileType.FileType.Trim();
                    if (string.IsNullOrEmpty(createFileType.FileType))
                        ModelState.AddModelError("FileTypeAlreadyExist", "Please Enter File Type Name.");

                    else if (fileTrackingServ.FileTypeExists(createFileType.FileType))
                        ModelState.AddModelError("FileTypeAlreadyExist", "File Type Name Already Exist.");
                    else
                    {
                        fileTrackingServ.UpdateFileTrackingType(createFileType);
                        TempData["Message"] = "Successfully Updated";
                        return RedirectToAction("Index");
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(createFileType);

        }

        public ActionResult Delete(int fileTypeID)
        {
            log.Info("FileTrackingTypeController/Delete");
            try
            {
                fileTrackingServ.Delete(fileTypeID);
                TempData["Message"] = "Successfully Deleted";
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