using MicroPay.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Common;
using System.IO;
using Nafed.MicroPay.Model;
using System.Text;
using static Nafed.MicroPay.Common.FileHelper;

namespace MicroPay.Web.Controllers
{
    public class DocumentManagementController : BaseController
    {
        private readonly IDashBoardService dashboardService;

        public DocumentManagementController(IDashBoardService dashboardService)
        {
            this.dashboardService = dashboardService;
        }
        // GET: DocumentManagement
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDocumentList()
        {
            log.Info($"DocumentManagementController/GetDocumentList");
            try
            {
                DashboardDocumentsVM documentVM = new DashboardDocumentsVM();
                documentVM.dbList = dashboardService.GetDashboardDocumentList().ToList();
                return PartialView("_DocumentList", documentVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult UploadDocument()
        {
            log.Info($"DocumentManagementController/UploadDocument");
            try
            {
                DashboardDocumentsVM documentVM = new DashboardDocumentsVM();
                ViewBag.DocumentType = dashboardService.GetDocumentType();
                return PartialView("_DocumentUpload", documentVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            DashboardDocuments document = new DashboardDocuments();
            if (Request.Files.Count > 0)
            {
                string fileName = string.Empty; string filePath = string.Empty;
                string fname = "";
                string documentName = Request["DocumentName"];
                string documentDesc = Request["DocumentDesc"];
                int docTypeID = Request["DocTypeID"] == null ? 0 : Convert.ToInt16(Request["DocTypeID"]);

                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    #region Check Mime Type
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        var contentType = GetFileContentType(file.InputStream);
                        var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                        if (!IsValidFileName(file.FileName))
                        {
                            stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                            stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                            stringBuilder.Append($"I. a to z characters.");
                            stringBuilder.Append($"II. numbers(0 to 9).");
                            stringBuilder.Append($"III. - and _ with space.");
                        }
                        if (dicValue != contentType)
                        {
                            stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                            stringBuilder.Append("<br>");
                        }
                    }
                    if (stringBuilder.ToString() != "")
                    {
                        return Json(new { msgType = "error", msg = stringBuilder.ToString() }, JsonRequestBehavior.AllowGet);
                       
                    }
                    #endregion

                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];

                        fname = ExtensionMethods.SetUniqueFileName("Document-",
                     Path.GetExtension(file.FileName));
                        fileName = fname;

                        string fullPath = Request.MapPath("~/Document/DashboardDocument/" + fileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }

                        fname = Path.Combine(Server.MapPath("~/Document/DashboardDocument/"), fname);
                        file.SaveAs(fname);

                    }
                    document.DocumentName = documentName;
                    document.DocumentDesc = documentDesc;
                    document.DocumentPathName = fileName;
                    document.DocTypeID = docTypeID;
                    document.CreatedBy = userDetail.UserID;
                    document.CreatedOn = DateTime.Now;
                    var flag = dashboardService.SaveDocument(document);
                    if (flag)
                    {
                        return Json(new { msgType = "success", msg = "Document Successfully Uploaded." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { msgType = "error", msg = "Document Upload failed." }, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception ex)
                {
                    log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                    throw ex;
                }

            }
            else
            {
                ViewBag.DocumentType = dashboardService.GetDocumentType();
                return Json(new { msgType = "error", msg = "Document Upload failed." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteDocument(int documentID, string fileName)
        {
            log.Info($"DocumentManagementController/DeleteDocument/{documentID}");
            try
            {
                string fullPath = Request.MapPath("~/Document/DashboardDocument/" + fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                var deleted = dashboardService.DeleteDocument(documentID);
                TempData["Message"] = "Succesfully Deleted";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}