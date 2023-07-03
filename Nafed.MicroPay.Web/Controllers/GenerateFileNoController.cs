using MicroPay.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using static Nafed.MicroPay.Common.FileHelper;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Common;
using CaptchaMvc.HtmlHelpers;
using System.IO;
using System.Text;
using Nafed.MicroPay.Services;

namespace MicroPay.Web.Controllers
{
    public class GenerateFileNoController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IFileTrackingSytemService fileTrackingServ;
        // GET: FileTrackingSystem
        public GenerateFileNoController(IDropdownBindService ddlService, IFileTrackingSytemService fileTrackingServ)
        {
            this.ddlService = ddlService;
            this.fileTrackingServ = fileTrackingServ;
        }

        // GET: GenerateFileNo
        public ActionResult Index()
        {
            FileTrackingViewModel fileTrackVM = new Models.FileTrackingViewModel();
            fileTrackVM.IsEligibleForFTMS = fileTrackingServ.IsEmpEligibleForFTMS((int)userDetail.EmployeeID);
            return View(fileTrackVM);
        }

        public ActionResult GetFileList()
        {
            log.Info($"GenerateFileNoController/GetFileList");
            try
            {
                FileTrackingViewModel fileTrackVM = new Models.FileTrackingViewModel();
                fileTrackVM.fileManagementList = fileTrackingServ.GetFileList(userDetail.UserID);
                return PartialView("_FileList", fileTrackVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        [HttpGet]
        public ActionResult GenerateFile()
        {
            log.Info($"GenerateFileNoController/GenerateFile");
            try
            {
                FileTrackingViewModel fileVM = new Models.FileTrackingViewModel();
                fileVM.fileManagement = fileTrackingServ.GetFileInitiatorDetail((int)userDetail.EmployeeID);
                ViewBag.FileType = ddlService.GetFileTrackingType();
                //fileVM.documentFiles = new List<FileTrackingDocuments>()
                //{
                //    new FileTrackingDocuments { Sno=1 }
                //};               
                return View("GenerateFileNo", fileVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        [HttpPost]
        public ActionResult GenerateFile(FileTrackingViewModel fileVM, string ButtonType, FormCollection frm)
        {
            log.Info($"GenerateFileNoController/GenerateFile");
            try
            {
                if (ModelState.IsValid)
                {                   
                    if (fileVM.fileManagement.FileID == 0)
                    {
                        FileManagementSystem fileManagement = new FileManagementSystem();
                        fileManagement.fileManagement = fileVM.fileManagement;
                        fileManagement.fileManagement.CreatedBy = userDetail.UserID;
                        fileManagement.fileManagement.CreatedOn = DateTime.Now;
                        fileManagement.fileManagement.StatusID = 1;

                        if (Request.Files.Count > 0)
                        {
                            string fileName = string.Empty; string filePath = string.Empty;
                            string fname = "";                          
                            //  Get all files from Request object  
                            HttpFileCollectionBase files = Request.Files;

                            #region Check Mime Type
                            StringBuilder stringBuilder = new StringBuilder();
                            for (int i = 0; i < files.Count; i++)
                            {
                                HttpPostedFileBase file = files[i];
                                if (!IsValidFileName(file.FileName))
                                {
                                    stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                                    stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                                    stringBuilder.Append($"I. a to z characters.");
                                    stringBuilder.Append($"II. numbers(0 to 9).");
                                    stringBuilder.Append($"III. - and _ with space.");
                                }

                                var contentType = GetFileContentType(file.InputStream);
                                var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                                if (dicValue != contentType)
                                {
                                    stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                                    stringBuilder.Append("<br>");
                                }
                            }
                            if (stringBuilder.ToString() != "")
                            {
                                TempData["Error"] = stringBuilder.ToString();
                                return RedirectToAction("Index");                               
                            }
                            #endregion

                            var docname = frm.Get("DocName").ToString();
                            var documentName = docname.Split(',');
                            fileVM.documentFiles = new List<FileTrackingDocuments>();
                            for (int i = 0; i < files.Count; i++)
                            {
                                HttpPostedFileBase file = files[i];

                                fname = ExtensionMethods.SetUniqueFileName("Document-",
                   Path.GetExtension(file.FileName));
                                fileName = fname;
                                fileVM.documentFiles.Add(new FileTrackingDocuments
                                {
                                    DocOrignalName = file.FileName,
                                    DocPathName = fileName,
                                    DocName = documentName[i]

                                });
                                fname = Path.Combine(Server.MapPath("~/Document/FileTracking/"), fname);
                                file.SaveAs(fname);

                            }
                            fileVM.documentFiles.ForEach(x => fileManagement.fileDocumentsList.Add(new FileTrackingDocuments
                            {
                                DocName = x.DocName,
                                DocPathName = x.DocPathName,
                                CreatedBy = fileManagement.fileManagement.CreatedBy,
                                CreatedOn = fileManagement.fileManagement.CreatedOn,
                                Sno = x.Sno,
                                DocOrignalName = x.DocOrignalName
                            }));
                        }
                        fileTrackingServ.GenerateFile(fileManagement);
                        TempData["Message"] = "File Generated Successfully.";
                        return RedirectToAction("Index");
                    }
                    else if (fileVM.fileManagement.FileID > 0)
                    {
                        FileManagementSystem fileManagement = new FileManagementSystem();
                        fileManagement.fileManagement = fileVM.fileManagement;
                        fileManagement.fileManagement.UpdatedBy = userDetail.UserID;
                        fileManagement.fileManagement.UpdatedOn = DateTime.Now;

                        if (Request.Files.Count > 0)
                        {
                            string fileName = string.Empty; string filePath = string.Empty;
                            string fname = "";

                            //  Get all files from Request object  
                            HttpFileCollectionBase files = Request.Files;

                            if (fileVM.documentFiles == null)
                            {
                                var docname = frm.Get("DocName").ToString();
                                var documentName = docname.Split(',');
                                fileVM.documentFiles = new List<FileTrackingDocuments>();

                                #region Check Mime Type
                                StringBuilder stringBuilder = new StringBuilder();
                                for (int i = 0; i < files.Count; i++)
                                {
                                    HttpPostedFileBase file = files[i];
                                    if (!IsValidFileName(file.FileName))
                                    {
                                        stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                                        stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                                        stringBuilder.Append($"I. a to z characters.");
                                        stringBuilder.Append($"II. numbers(0 to 9).");
                                        stringBuilder.Append($"III. - and _ with space.");
                                    }

                                    var contentType = GetFileContentType(file.InputStream);
                                    var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                                    if (dicValue != contentType)
                                    {
                                        stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                                        stringBuilder.Append("<br>");
                                    }
                                }
                                if (stringBuilder.ToString() != "")
                                {
                                    TempData["Error"] = stringBuilder.ToString();
                                    return RedirectToAction("Index");
                                }
                                #endregion

                                for (int i = 0; i < files.Count; i++)
                                {
                                    HttpPostedFileBase file = files[i];
                                   

                                    fname = ExtensionMethods.SetUniqueFileName("Document-",
                       Path.GetExtension(file.FileName));
                                    fileName = fname;
                                    fileVM.documentFiles.Add(new FileTrackingDocuments
                                    {
                                        DocOrignalName = file.FileName,
                                        DocPathName = fileName,
                                        DocName = documentName[i]

                                    });
                                    fname = Path.Combine(Server.MapPath("~/Document/FileTracking/"), fname);
                                    file.SaveAs(fname);

                                }
                            }
                            else
                            {
                                #region Check Mime Type
                                StringBuilder stringBuilder = new StringBuilder();
                                for (int i = 0; i < files.Count; i++)
                                {
                                    HttpPostedFileBase file = files[i];
                                    if (!IsValidFileName(file.FileName))
                                    {
                                        stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                                        stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                                        stringBuilder.Append($"I. a to z characters.");
                                        stringBuilder.Append($"II. numbers(0 to 9).");
                                        stringBuilder.Append($"III. - and _ with space.");
                                    }
                                    var contentType = GetFileContentType(file.InputStream);
                                    var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                                    if (dicValue != contentType)
                                    {
                                        stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                                        stringBuilder.Append("<br>");
                                    }
                                }
                                if (stringBuilder.ToString() != "")
                                {
                                    TempData["Error"] = stringBuilder.ToString();
                                    return RedirectToAction("Index");
                                }
                                #endregion

                                for (int i = 0; i < files.Count; i++)
                                {
                                    HttpPostedFileBase file = files[i];
                                    if (string.IsNullOrEmpty(file.FileName))
                                    {
                                        var docnme = frm.Get("DocName").ToString();

                                        fname = Path.GetFileName(fileVM.documentFiles[i].DocPathName);
                                        fileVM.documentFiles[i].DocPathName = fname;

                                    }
                                    else
                                    {
                                        if (System.IO.File.Exists(fileVM.documentFiles[i].DocPathName))
                                        {
                                            System.IO.File.Delete(fileVM.documentFiles[i].DocPathName);
                                        }

                                        fname = ExtensionMethods.SetUniqueFileName("Document-",
                           Path.GetExtension(file.FileName));
                                        fileName = fname;
                                        var docname = frm.Get("DocName").ToString();

                                        fileVM.documentFiles[i].DocName = docname;
                                        fileVM.documentFiles[i].DocOrignalName = file.FileName;
                                        fileVM.documentFiles[i].DocPathName = fileName;
                                        fname = Path.Combine(Server.MapPath("~/Document/FileTracking/"), fname);
                                        file.SaveAs(fname);

                                    }
                                }
                            }
                                fileVM.documentFiles.ForEach(x => fileManagement.fileDocumentsList.Add(new FileTrackingDocuments
                                {
                                    DocName = x.DocName,
                                    DocPathName = x.DocPathName,
                                    CreatedBy = userDetail.UserID,
                                    CreatedOn = DateTime.Now,
                                    Sno = x.Sno,
                                    DocOrignalName = x.DocOrignalName
                                }));
                            }
                            fileTrackingServ.UpdateFileDetail(fileManagement);
                            TempData["Message"] = "File Detail Updated Successfully.";
                            return RedirectToAction("Index");

                        }
                        return View("GenerateFileNo", fileVM);

                    }
                return View("GenerateFileNo", fileVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }

        }

        [HttpGet]
        public ActionResult Edit(int fileID)
        {
            log.Info($"GenerateFileNoController/Edit/fileID={fileID}");
            try
            {
                FileTrackingViewModel fileVM = new Models.FileTrackingViewModel();
                var dd = fileTrackingServ.GetFileDetails(fileID);
                fileVM.fileManagement = dd.fileManagement;
                fileVM.documentFiles = dd.fileDocumentsList;
                var getInitiatorDetails = fileTrackingServ.GetFileInitiatorDetail((int)userDetail.EmployeeID);
                if (getInitiatorDetails != null)
                {
                    fileVM.fileManagement.DesignationName = getInitiatorDetails.DesignationName;
                }
                fileVM.documentFiles.ForEach(x =>
                {
                    x.DocPathName = Path.Combine(Server.MapPath("~/Document/FileTracking/"), x.DocPathName);
                });
                ViewBag.FileType = ddlService.GetFileTrackingType();
                TempData["DocumentFiles"] = fileVM;
                return View("GenerateFileNo", fileVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
        
        public JsonResult _RemoveDocumentRow(string docID)
        {
            bool flag;
            string DocPathName;
            var dcID = Convert.ToInt32(docID);
            flag = fileTrackingServ.DeleteDocument(dcID,out DocPathName);
            if (flag)
            {
                string fullPath = Request.MapPath("~/Document/FileTracking/" + DocPathName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            return Json(new { FileStatus = flag }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _AddDocumentRow()
        {
            FileTrackingDocuments ftrackDocument = new FileTrackingDocuments();
            return Json(new { htmlData = ConvertViewToString("_UploadDocumentForFile", ftrackDocument) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FileForward(int fileID, string fileNo, string department)
        {
            log.Info($"GenerateFileNoController/FileForward/fileID={fileID}");
            try
            {
                ProcessWorkFlow pworkFlow = new ProcessWorkFlow();
                ViewBag.Department = ddlService.ddlDepartmentList();
                ViewBag.Employee = ddlService.GetEmployeeByDepartmentDesignationID(userDetail.BranchID, null, null);
                pworkFlow.Purpose = fileNo;
                pworkFlow.ReferenceID = fileID;

                return PartialView("_FileForwardPopup", pworkFlow);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public JsonResult GetEmployeeByDepartment(int departmentID)
        {
            log.Info($"GenerateFileNoController/GetEmployeeByDesignation/departmentID={departmentID}");
            try
            {
                var employeedetails = ddlService.GetEmployeeByDepartmentDesignationID(null, departmentID, null);
                SelectListModel selectemployeeDetails = new SelectListModel();
                selectemployeeDetails.id = 0;
                selectemployeeDetails.value = "Select";
                employeedetails.Insert(0, selectemployeeDetails);
                var emp = new SelectList(employeedetails, "id", "value");
                return Json(new { employees = emp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult FileForward(ProcessWorkFlow pWorkFlow)
        {
            log.Info($"GenerateFileNoController/FileForward");
            try
            {
                if (!pWorkFlow.ReceiverDepartmentID.HasValue)
                    ModelState.AddModelError("DepartmentModelError", "Please Select Department.");
                if (!pWorkFlow.ReceiverID.HasValue)
                    ModelState.AddModelError("EmployeeModelError", "Please Select Employee.");
                //if (string.IsNullOrEmpty(pWorkFlow.Scomments))
                //    ModelState.AddModelError("RemarkModelError", "Please Enter Remark.");


                if (ModelState.IsValid)
                {
                    pWorkFlow.CreatedBy = userDetail.UserID;
                    pWorkFlow.StatusID = 1; // set status 1 for rollback file purpuse if first reciever is not read the file.
                    pWorkFlow.SenderID = userDetail.EmployeeID;
                    pWorkFlow.EmployeeID = (int)userDetail.EmployeeID;
                    pWorkFlow.SenderDepartmentID = userDetail.DepartmentID;
                    pWorkFlow.SenderDesignationID = userDetail.DesignationID;
                    pWorkFlow.ProcessID = (int)WorkFlowProcess.FileTracking;                  
                    fileTrackingServ.ForwardFile(pWorkFlow);

                    return Json(new
                    {
                        status = 1,
                        type = "success",
                        msg = "File Forwarded Successfully."

                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ViewBag.Department = ddlService.ddlDepartmentList();
                    ViewBag.Employee = ddlService.GetEmployeeByDepartmentDesignationID(userDetail.BranchID, pWorkFlow.ReceiverDepartmentID, null);
                    return Json(new { status = 0, htmlData = ConvertViewToString("_FileForwardPopup", pWorkFlow) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        [HttpGet]
        public ActionResult Files()
        {
            log.Info("GenerateFileNoController/Files");
            try
            {
                //var userId = (int)Session["UserId"];

                //FTSUserDepartment vm = new FTSUserDepartment();
                //vm.UserId = userId;
                return View("FileContainer");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetInboxFiles()
        {
            log.Info($"GenerateFileNoController/GetInboxFiles");
            try
            {
                var userId = (int)Session["UserId"];
                FileTrackingViewModel fileTrackVM = new Models.FileTrackingViewModel();
                var fileManagemenList = fileTrackingServ.GetInboxFileList(userId);
                fileTrackVM.fileManagementList = fileManagemenList;
                return PartialView("_InboxFileList", fileTrackVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " Datetime3Stamp-" + DateTime.Now);
                throw;
            }
        }

        public ActionResult GetOutboxFiles()
        {
            log.Info($"GenerateFileNoController/GetOutboxFiles");
            try
            {
                var userId = (int)Session["UserId"];
                FileTrackingViewModel fileTrackVM = new Models.FileTrackingViewModel();
                fileTrackVM.fileManagementList = fileTrackingServ.GetOutboxFileList(userId);
                return PartialView("_OutboxFileList", fileTrackVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public ActionResult TabFileForward(int fileID, string fileNo, string sub, int wflowID, DateTime? putupdate)
        {
            log.Info($"GenerateFileNoController/TabFileForward/fileID={fileID}");
            try
            {
                ProcessWorkFlow pworkFlow = new ProcessWorkFlow();
                FileTrackingViewModel fileTrackingVM = new FileTrackingViewModel();
                FileManagement file = new FileManagement();
                ViewBag.Department = ddlService.ddlDepartmentList();
                ViewBag.Employee = ddlService.GetEmployeeByDepartmentDesignationID(userDetail.BranchID, null, null);
                var fileForwardDtl = fileTrackingServ.GetFileForwardDetails(fileID);
                fileTrackingVM.fileManagementList = fileForwardDtl;
                pworkFlow.Purpose = fileNo;
                pworkFlow.ReferenceID = fileID;
                pworkFlow.Remark = sub;
                pworkFlow.WorkflowID = wflowID;
                fileTrackingVM.processWorkFlow = pworkFlow;
                file.FilePutup = putupdate;
                fileTrackingVM.fileManagement = file;
                //fileTrackingVM.documentFiles = new List<FileTrackingDocuments>()
                //{
                //    new FileTrackingDocuments { Sno=1 }
                //};
                TempData["DocumentFiles"] = fileTrackingVM;
                return PartialView("_FileForwardTabPopup", fileTrackingVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
        
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult TabFileForward(FormCollection collection, string ButtonType, FileTrackingViewModel fileTracking)
        {
            log.Info($"GenerateFileNoController/TabFileForward");
            try
            {
                if (ButtonType == "Add Document")
                {
                    if (fileTracking.documentFiles != null)
                    {
                        if (Request.Files.Count > 0)
                        {
                            string fileName = string.Empty;

                            //  Get all files from Request object  
                            HttpFileCollectionBase files = Request.Files;
                            for (int i = 0; i < files.Count; i++)
                            {
                                HttpPostedFileBase file = files[i];

                                fileTracking.documentFiles[i].DocOrignalName = file.FileName == "" ? fileTracking.documentFiles[i].DocOrignalName : file.FileName;

                            }
                        }


                        if (fileTracking.documentFiles != null && fileTracking.documentFiles.Count == 1)
                            fileTracking.documentFiles.FirstOrDefault().Sno = 1;
                        else
                        {
                            var s_no = 1;
                            if (fileTracking.documentFiles != null)
                            {
                                fileTracking.documentFiles.ForEach(x =>
                                {

                                    x.Sno = s_no++;
                                });
                            }
                        }
                        if (fileTracking.documentFiles.Count < 5)
                            fileTracking.documentFiles.Add(new FileTrackingDocuments()
                            {
                                Sno = fileTracking.documentFiles.Count + 1
                            });
                        ViewBag.FileType = ddlService.GetFileTrackingType();
                        TempData["DocumentFiles"] = fileTracking;
                        return Json(new { status = 0, htmlData = ConvertViewToString("_UploadDocuments", fileTracking) }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        fileTracking.documentFiles = new List<FileTrackingDocuments>()
                {
                    new FileTrackingDocuments { Sno=1 }
                };
                        ViewBag.FileType = ddlService.GetFileTrackingType();
                        TempData["DocumentFiles"] = fileTracking;
                        return Json(new { status = 0, htmlData = ConvertViewToString("_UploadDocuments", fileTracking) }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    FileManagementSystem fileManagement = new FileManagementSystem();
                    if (ModelState.IsValid)
                    {

                        fileManagement.fileManagement.CreatedBy = userDetail.UserID;
                        fileManagement.fileManagement.CreatedOn = DateTime.Now;
                        fileManagement.fileManagement.StatusID = 2;
                        if (Request.Files.Count > 0)
                        {
                            var docname = collection.Get("FileName").ToString();
                            var documentName = docname.Split(',');
                            string fileName = string.Empty; string filePath = string.Empty;
                            string fname = "";
                       //     var DocNames = collection.GetValue("FileName").RawValue;
                            //  Get all files from Request object  
                            HttpFileCollectionBase files = Request.Files;

                            #region Check Mime Type
                            StringBuilder stringBuilder = new StringBuilder();
                            for (int i = 0; i < files.Count; i++)
                            {
                                HttpPostedFileBase file = files[i];
                                if (!IsValidFileName(file.FileName))
                                {
                                    stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                                    stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                                    stringBuilder.Append($"I. a to z characters.");
                                    stringBuilder.Append($"II. numbers(0 to 9).");
                                    stringBuilder.Append($"III. - and _ with space.");
                                }

                                var contentType = GetFileContentType(file.InputStream);
                                var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                                if (dicValue != contentType)
                                {
                                    stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                                    stringBuilder.Append("<br>");
                                }
                            }
                            if (stringBuilder.ToString() != "")
                            {
                                return Json(new
                                {
                                    status = 1,
                                    type = "success",
                                    msg = stringBuilder.ToString()
                                }, JsonRequestBehavior.AllowGet);
                            }
                            #endregion

                            for (int i = 0; i < files.Count; i++)
                            {
                                HttpPostedFileBase file = files[i];

                                fname = ExtensionMethods.SetUniqueFileName("Document-",
                             Path.GetExtension(file.FileName));
                                fileName = fname;
                                string fullPath = Request.MapPath("~/Document/FileTracking/" + fileName);
                                if (System.IO.File.Exists(fullPath))
                                {
                                    System.IO.File.Delete(fullPath);
                                }

                                fname = Path.Combine(Server.MapPath("~/Document/FileTracking/"), fname);
                                file.SaveAs(fname);


                                fileManagement.fileDocumentsList.Add(new FileTrackingDocuments()
                                {
                                    DocName = documentName[i],
                                    DocPathName = fileName,
                                    CreatedBy = fileManagement.fileManagement.CreatedBy,
                                    CreatedOn = fileManagement.fileManagement.CreatedOn,
                                    Sno = 1,
                                    DocOrignalName = file.FileName
                                });
                            }
                        }

                        var revdepartmentId = ((string[])(collection.GetValue("revdepartmentId").RawValue))[0];
                        var receiverID = ((string[])(collection.GetValue("receiverID").RawValue))[0];
                        var Scomments = ((string[])(collection.GetValue("Scomments").RawValue))[0];

                        var referenceID = ((string[])(collection.GetValue("ReferenceID").RawValue))[0];
                        var workflowID = ((string[])(collection.GetValue("WorkflowID").RawValue))[0];
                        var sendDate = ((string[])(collection.GetValue("sendDate").RawValue))[0];
                        fileManagement.processWorkFlow = new ProcessWorkFlow();
                        fileManagement.processWorkFlow.CreatedBy = userDetail.UserID;
                        fileManagement.processWorkFlow.StatusID = 1;

                        fileManagement.processWorkFlow.ReceiverDepartmentID = Convert.ToInt32(revdepartmentId);
                        fileManagement.processWorkFlow.ReceiverID = Convert.ToInt32(receiverID);
                        fileManagement.processWorkFlow.Scomments = Convert.ToString(Scomments);

                        fileManagement.processWorkFlow.SenderID = userDetail.EmployeeID;
                        fileManagement.processWorkFlow.EmployeeID = (int)userDetail.EmployeeID;
                        fileManagement.processWorkFlow.SenderDepartmentID = userDetail.DepartmentID;
                        fileManagement.processWorkFlow.SenderDesignationID = userDetail.DesignationID;
                        fileManagement.processWorkFlow.ProcessID = (int)WorkFlowProcess.FileTracking;
                        fileManagement.processWorkFlow.WorkflowID = Convert.ToInt32(workflowID);
                        fileManagement.processWorkFlow.ReferenceID = Convert.ToInt32(referenceID);
                        string newSendDate = sendDate.Substring(0, sendDate.LastIndexOf("G") - 1);
                        fileManagement.processWorkFlow.Senddate = Convert.ToDateTime(newSendDate);
                        fileTrackingServ.TabForwardFile(fileManagement);

                        return Json(new
                        {
                            status = 1,
                            type = "success",
                            msg = "File Forwarded Successfully."

                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ViewBag.Department = ddlService.ddlDepartmentList();
                        ViewBag.Employee = ddlService.GetEmployeeByDepartmentDesignationID(userDetail.BranchID, fileManagement.processWorkFlow.ReceiverDepartmentID, null);
                        return Json(new
                        {
                            status = 1,
                            type = "error",
                            msg = "Error while forwarding."

                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public ActionResult FileSearch()
        {
            FileManagement fileTrackVM = new FileManagement();
            return View("SearchContainer", fileTrackVM);
        }

        public ActionResult _GetFilesGridView(FileManagement fileManagement)
        {
            log.Info($"GenerateFileNoController/_GetFilesGridView");
            try
            {
                FileTrackingViewModel fileTrackVM = new Models.FileTrackingViewModel();
                fileTrackVM.fileManagementList = fileTrackingServ.GetFileListForGridView(fileManagement);
                return PartialView("_FileTrackingGridView", fileTrackVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public ActionResult _GetFilesGridViewPopup(int fileID, string fileNo, string sub)
        {
            log.Info($"GenerateFileNoController/_GetFilesGridViewPopup/fileID={fileID}");
            try
            {
                ProcessWorkFlow pworkFlow = new ProcessWorkFlow();
                FileTrackingViewModel fileTrackingVM = new FileTrackingViewModel();
                var fileForwardDtl = fileTrackingServ.GetFileForwardDetails(fileID);
                fileTrackingVM.fileManagementList = fileForwardDtl;
                pworkFlow.Purpose = fileNo;
                pworkFlow.Remark = sub;
                fileTrackingVM.processWorkFlow = pworkFlow;

                return PartialView("_FileTrackingGridViewPopup", fileTrackingVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public ActionResult FileClose(int fileID, string fileNo, int wflowID)
        {
            log.Info($"GenerateFileNoController/FileClose/fileID={fileID}");
            try
            {
                bool flag = false;
                ProcessWorkFlow pworkFlow = new ProcessWorkFlow();
                pworkFlow.ReferenceID = fileID;
                pworkFlow.WorkflowID = wflowID;
                pworkFlow.CreatedBy = userDetail.UserID;
                pworkFlow.CreatedOn = DateTime.Now;
                flag = fileTrackingServ.FileClosed(pworkFlow);
                if (flag)
                    TempData["Message"] = "File Closed Successfully.";
                else
                    TempData["Error"] = "Problem while closing file.";

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
            return RedirectToAction("Files");
        }

        [HttpGet]
        public ActionResult _FileReceive(int fileID, string fileNo, int wflowID, DateTime? putupdate)
        {
            log.Info($"GenerateFileNoController/_FileReceive/fileID={fileID}");
            try
            {
                ProcessWorkFlow pworkFlow = new ProcessWorkFlow();
                pworkFlow.ReferenceID = fileID;
                pworkFlow.WorkflowID = wflowID;
                pworkFlow.Remark = fileNo;// set filname
                pworkFlow.Senddate = putupdate; // assign putup date
                return PartialView("_ReadPopup", pworkFlow);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        [HttpPost]
        public JsonResult _FileReceive(ProcessWorkFlow pworkFlow)
        {
            log.Info($"GenerateFileNoController/_FileReceive");
            try
            {
                pworkFlow.CreatedBy = userDetail.UserID;
                fileTrackingServ.FileReceive(pworkFlow);

                return Json(new
                {
                    status = 1,
                    type = "success",
                    msg = "File Received Successfully."

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        #region File Rollback

        public PartialViewResult FileRollback(int fileId)
        {
            log.Info("TicketController/FileRollback");
            FileTrackingViewModel fileTrackVM = new Models.FileTrackingViewModel();
            try
            {
                ProcessWorkFlow pWrokFlow = new ProcessWorkFlow();
                pWrokFlow.SenderID = userDetail.EmployeeID;
                pWrokFlow.SenderDepartmentID = userDetail.DepartmentID;
                pWrokFlow.SenderDesignationID = userDetail.DesignationID;
                pWrokFlow.ReceiverID = userDetail.EmployeeID;
                pWrokFlow.ReceiverDepartmentID = userDetail.DepartmentID;
                pWrokFlow.Scomments = "Rollback";
                pWrokFlow.StatusID = 1;
                pWrokFlow.ProcessID = (int)WorkFlowProcess.FileTracking;
                pWrokFlow.ReferenceID = fileId;
                pWrokFlow.EmployeeID = (int)userDetail.EmployeeID;
                pWrokFlow.CreatedBy = userDetail.UserID;
                pWrokFlow.CreatedOn = DateTime.Now;
                fileTrackingServ.FileRollback(pWrokFlow);
               
                fileTrackVM.fileManagementList = fileTrackingServ.GetFileList(userDetail.UserID);
                return PartialView("_FileList", fileTrackVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
            }
            return PartialView("_FileList", fileTrackVM);
        }

        #endregion

        #region Validate User
        [HttpGet]
        public ActionResult Validate()
        {
            log.Info("GenerateFileNoController/Validate");
            try
            {
                return View("ValidateUser");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult Validate(ValidateLogin validate)
        {
            log.Info("GenerateFileNoController/Validate");
            try
            {if (string.IsNullOrEmpty(validate.hduserName))
                    ModelState.AddModelError("userName", "Please enter User Name.");
                if (string.IsNullOrEmpty(validate.hdpassword))
                    ModelState.AddModelError("password", "Please enter Password.");
                if (ModelState.IsValid)
                {
                    if (!this.IsCaptchaValid("Captcha is not valid"))
                    {
                        ViewBag.LoginMessage = "Captcha is not valid";
                        return View("ValidateUser", validate);
                    }

                    bool isAuthenticated = false;
                    string sAuthenticationMessage = string.Empty;
                    ViewBag.LoginMessage = string.Empty;
                    int userId;

                    validate.userName = Password.DecryptStringAES(validate.hduserName);
                    validate.password = Password.DecryptStringAES(validate.hdpassword).Replace(validate.hdCp, "");

                    isAuthenticated = fileTrackingServ.ValidateUser(validate, out userId);
                    if (isAuthenticated)
                    {
                        Session["UserId"] = userId;
                        return RedirectToAction("Files");
                       // return RedirectToAction("Files", new { userId = userId });
                    }
                    else
                    {
                        ViewBag.LoginMessage = "Invalid Credentials";                   
                        return View("ValidateUser");
                    }
                }
                else
                    return View("ValidateUser");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion
    }
}