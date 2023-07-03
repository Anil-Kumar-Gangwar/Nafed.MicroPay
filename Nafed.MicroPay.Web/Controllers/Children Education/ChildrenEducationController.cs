using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static Nafed.MicroPay.Common.ExtensionMethods;
using static Nafed.MicroPay.Common.FileHelper;
using Model = Nafed.MicroPay.Model;
namespace MicroPay.Web.Controllers.Children_Education
{
    public class ChildrenEducationController : BaseController
    {
        private readonly IDropdownBindService dropdownService;
        private readonly IChildrenEducationService childrenService;
        public ChildrenEducationController(IDropdownBindService dropdownService, IChildrenEducationService childrenService)
        {
            this.dropdownService = dropdownService;
            this.childrenService = childrenService;
        }

        // GET: ChildrenEducation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyChildrenEducationList()
        {
            log.Info($"ChildrenEducationController/MyChildrenEducationList");
            try
            {
                ChildrenEducationViewModel empChildrenEducationVM = new ChildrenEducationViewModel();
                var selfFormList = childrenService.GetEmployeeChildrenEducationList((int)userDetail.EmployeeID);
                empChildrenEducationVM.childrenEducationList = selfFormList;
                return PartialView("_MyChildrenEducationList", empChildrenEducationVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult Edit(int empID, int childrenEduHdrId, bool updateOrNot = true)
        {
            log.Info($"ChildrenEducation/Edit");
            try
            {
                Model.ChildrenEducationHdr childrenEduData = new Model.ChildrenEducationHdr();
                var reportingYr = DateTime.Now.GetFinancialYr();
                childrenEduData = childrenService.GetChildrenEducation(empID, childrenEduHdrId);
                childrenEduData.ChildrenEducationDetailsList = childrenService.GetChildrenEducationDetails(empID, childrenEduHdrId);
                childrenEduData.DependentList = childrenService.GetDependentList(empID);
                childrenEduData.ChildrenEducationDocumentsList = childrenService.GetChildrenEducationDocumentsList(empID, childrenEduHdrId);

                if (childrenEduData.ChildrenEducationDetailsList.Count > 0)
                {
                    var sno = 1;
                    childrenEduData.ChildrenEducationDetailsList.ForEach(x =>
                    {
                        x.sno = sno++;
                    });
                }
                if ((childrenEduData.DependentList.Count == childrenEduData.ChildrenEducationDetailsList.Count) || updateOrNot == false)
                {
                    childrenEduData.IsDependentMatch = true;
                    return View("ChildrenEducationContainer", childrenEduData);
                }
                else
                {
                    childrenEduData.IsDependentMatch = false;
                    return View("ChildrenEducationContainer", childrenEduData);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _GetChildrenEducationForm(Model.ChildrenEducationHdr childrenEduData)
        {
            log.Info($"ChildrenEducation/_GetChildrenEducationForm");
            try
            {
                TempData["ChildrenEducation"] = childrenEduData;
                TempData.Keep("ChildrenEducation");
                return PartialView("_ChildrenEducationForm", childrenEduData);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(int empID)
        {
            log.Info($"ChildrenEducation/Create");
            bool flag;
            try
            {
                var reportingYr = DateTime.Now.GetFinancialYr();
                Model.ChildrenEducationHdr childrenEduData = new Model.ChildrenEducationHdr();
                childrenEduData.ReportingYear = reportingYr;
                childrenEduData.EmployeeId = userDetail.EmployeeID.Value;
                childrenEduData.DepartmentId = userDetail.DepartmentID;
                childrenEduData.DesignationId = userDetail.DesignationID.Value;
                childrenEduData.BranchId = userDetail.BranchID;
                childrenEduData.CreatedBy = userDetail.UserID;
                childrenEduData.CreatedOn = DateTime.Now;
                childrenEduData.StatusId = (int)Model.ChildrenEducationStatus.SavedByEmployee;

                var hdrId = childrenService.ChildrenEducationExist(childrenEduData.EmployeeId, reportingYr);
                if (hdrId == 0)
                {
                    int chldrenHdrId = 0;
                    flag = childrenService.InsertChildrenEducationData(childrenEduData, out chldrenHdrId);
                    var msgs = "Application for reimbursement of Children Education Allowance submitted successfully.";
                    return Json(new { part = 2, msgType = "Inserted", empID = empID, childrenEduHdrId = chldrenHdrId, msg = msgs }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { part = 2, msgType = "Inserted", empID = empID, childrenEduHdrId = hdrId }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostChildrenEducationData(Model.ChildrenEducationHdr childrenEducationHdr, string ButtonType, FormCollection frm)
        {
            log.Info($"ChildrenEducation/_PostChildrenEducationData");
            bool flag;
            try
            {
                if (ButtonType == "Add Row")
                {
                    childrenEducationHdr.DependentList = childrenService.GetDependentList(childrenEducationHdr.EmployeeId);
                    if (childrenEducationHdr.ChildrenEducationDetailsList == null)
                        childrenEducationHdr.ChildrenEducationDetailsList = new List<Model.ChildrenEducationDetails>() {
                            new Model.ChildrenEducationDetails() { sno = 1, } };
                    else
                    {
                        if (childrenEducationHdr.ChildrenEducationDetailsList.Count == 1)
                            childrenEducationHdr.ChildrenEducationDetailsList.FirstOrDefault().sno = 1;
                        else
                        {
                            var s_no = 1;
                            childrenEducationHdr.ChildrenEducationDetailsList.ForEach(x =>
                            {
                                x.sno = s_no++;
                            });
                        }
                        if (childrenEducationHdr.ChildrenEducationDetailsList.Count < 10)
                            childrenEducationHdr.ChildrenEducationDetailsList.Add(new Model.ChildrenEducationDetails()
                            {
                                sno = childrenEducationHdr.ChildrenEducationDetailsList.Count + 1
                            });
                    }
                    TempData["ChildrenEducation"] = childrenEducationHdr;
                    return Json(new { part = 1, htmlData = ConvertViewToString("_ChildrenEducationGridView", childrenEducationHdr) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var docCount = childrenService.GetChildrenEducationDocumentsList(childrenEducationHdr.EmployeeId, childrenEducationHdr.ChildrenEduHdrID);
                    var files = (List<string>)TempData["UploadedFiles"];
                    TempData.Keep("UploadedFiles");
                    if (files.Count == 0 && docCount.Count == 0)
                        ModelState.AddModelError("FileReq", "Please upload receipts or bills");
                    if (childrenEducationHdr.StrReceiptDate != null)
                        childrenEducationHdr.ReceiptDate = Convert.ToDateTime(childrenEducationHdr.StrReceiptDate);
                    if (childrenEducationHdr.ReceiptDate == null)
                        ModelState.AddModelError("ReceiptDateReq", "Please enter receipt date");
                    if (childrenEducationHdr.StrReceiptDate2 != null)
                        childrenEducationHdr.ReceiptDate2 = Convert.ToDateTime(childrenEducationHdr.StrReceiptDate2);


                    if (childrenEducationHdr.Amount == null || childrenEducationHdr.Amount == 0)
                        ModelState.AddModelError("AmountReq", "Please enter amount");
                    if (childrenEducationHdr.ReceiptNo == null || childrenEducationHdr.ReceiptNo == "")
                        ModelState.AddModelError("ReceiptNoReq", "Please enter receipt No");
                    if (childrenEducationHdr.ChildrenEducationDetailsList.Count > 1)
                    {
                        if (childrenEducationHdr.ReceiptNo2 == null || childrenEducationHdr.ReceiptNo2 == "")
                            ModelState.AddModelError("ReceiptNoReq2", "Please enter receipt No");
                        if (childrenEducationHdr.ReceiptDate2 == null)
                            ModelState.AddModelError("ReceiptDateReq2", "Please enter receipt date");
                    }
                    if (childrenEducationHdr.ChildrenEducationDetailsList != null)
                    {
                        for (int i = 0; i < childrenEducationHdr.ChildrenEducationDetailsList.Count; i++)
                        {
                            if (childrenEducationHdr.ChildrenEducationDetailsList[i].NotApplicable == false)
                            {
                                if (childrenEducationHdr.ChildrenEducationDetailsList[i].EmpDependentID == 0)
                                    ModelState.AddModelError("ChildrenEducationDetailsList[" + i + "].EmpDependentID", "Please select child Name");
                                if (childrenEducationHdr.ChildrenEducationDetailsList[i].DOB == null)
                                    ModelState.AddModelError("ChildrenEducationDetailsList[" + i + "].DOB", "Please enter date of birth");
                                if (childrenEducationHdr.ChildrenEducationDetailsList[i].SchoolName == null || childrenEducationHdr.ChildrenEducationDetailsList[i].SchoolName == "")
                                    ModelState.AddModelError("ChildrenEducationDetailsList[" + i + "].SchoolName", "Please enter school name");
                                if (childrenEducationHdr.ChildrenEducationDetailsList[i].ClassName == null || childrenEducationHdr.ChildrenEducationDetailsList[i].ClassName == "")
                                    ModelState.AddModelError("ChildrenEducationDetailsList[" + i + "].ClassName", "Please enter class name");
                            }
                        }
                    }
                    var getDuplicateRows = ExtensionMethods.FindDuplicates(childrenEducationHdr.ChildrenEducationDetailsList, x => new { x.EmpDependentID });
                    if (getDuplicateRows.Count > 0)
                        ModelState.AddModelError("ChildrenEducationDuplicate", "Duplicate entries not allowed for the child education details.");

                    if (ModelState.IsValid)
                    {
                        childrenEducationHdr.UpdatedBy = userDetail.UserID;
                        childrenEducationHdr.UpdatedOn = DateTime.Now;
                        childrenEducationHdr.CreatedBy = userDetail.UserID;
                        childrenEducationHdr.CreatedOn = DateTime.Now;
                        var buttonType = Convert.ToString(frm["buttonTypeDetail"]);
                        childrenEducationHdr.StatusId = (buttonType == "Save" ? (int)Model.ChildrenEducationStatus.SavedByEmployee : (int)Model.ChildrenEducationStatus.SubmitedByEmployee);

                        #region Document Upload
                        List<Model.ChildrenEducationDocuments> docList = new List<Model.ChildrenEducationDocuments>();
                        //var files = (List<string>)TempData["UploadedFiles"];
                        foreach (var file in files)
                        {
                            Model.ChildrenEducationDocuments childrenEduDocuments = new Model.ChildrenEducationDocuments();
                            childrenEduDocuments.EmployeeID = childrenEducationHdr.EmployeeId;
                            childrenEduDocuments.FilePath = file;
                            childrenEduDocuments.CreatedBy = userDetail.UserID;
                            childrenEduDocuments.CreatedOn = DateTime.Now;
                            docList.Add(childrenEduDocuments);
                        }
                        #endregion
                        childrenEducationHdr.ChildrenEducationDocumentsList = docList;
                        flag = childrenService.UpdateChildrenEducationData(childrenEducationHdr);
                        var msgs = buttonType == "Save" ? "Application for reimbursement of Children Education Allowance save successfully." : "Application for reimbursement of Children Education Allowance submitted successfully.";
                        //var msgs = "Application for reimbursement of Children Education Allowance submitted successfully.";
                        return Json(new { part = 0, msgType = "success", msg = msgs }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        childrenEducationHdr = (Model.ChildrenEducationHdr)TempData["ChildrenEducation"];
                        TempData.Keep("ChildrenEducation");
                        childrenEducationHdr.ReceiptDate = Convert.ToDateTime(childrenEducationHdr.StrReceiptDate);
                        return Json(new { part = 4, htmlData = ConvertViewToString("_ChildrenEducationForm", childrenEducationHdr) }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _RemoveChildrenRow(int sNo)
        {
            try
            {
                var modelData = (Model.ChildrenEducationHdr)TempData["ChildrenEducation"];
                if (modelData != null)
                {
                    var deletedRow = modelData.ChildrenEducationDetailsList.Where(x => x.sno == sNo).FirstOrDefault();
                    modelData.ChildrenEducationDetailsList.Remove(deletedRow);
                    TempData["ChildrenEducation"] = modelData;
                    return PartialView("_ChildrenEducationGridView", modelData);
                }
                return Content("");
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
            HttpFileCollectionBase files = Request.Files;
            string fileName = string.Empty; string filePath = string.Empty;
            string fname = "";
            List<string> filename = new List<string>();
            TempData["UploadedFiles"] = null;

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
                    return Json(stringBuilder.ToString());
                }

                if (dicValue != contentType)
                {
                    stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                    stringBuilder.Append("<br>");
                }
            }
            if (stringBuilder.ToString() != "")
            {
                return Json(stringBuilder.ToString());
            }
            #endregion

            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                fname = ExtensionMethods.SetUniqueFileName("ReceiptDocument-", Path.GetExtension(file.FileName));
                fileName = fname;
                filename.Add(fileName);
                string fullPath = Request.MapPath("~/Document/ChildrenEducationReceipts/" + fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                fname = Path.Combine(Server.MapPath("~/Document/ChildrenEducationReceipts/"), fname);
                file.SaveAs(fname);
            }
            TempData["UploadedFiles"] = filename;
            return Json(files.Count + " Files Uploaded!");
        }

        public ActionResult GetChildrenEducationDocuments(int employeeId, int childrenEduHdrID)
        {
            log.Info($"ChildrenEducation/GetChildrenEducationDocuments");
            try
            {
                ChildrenEducationViewModel documentViewModel = new ChildrenEducationViewModel();
                documentViewModel.childrenEducationDocuments = childrenService.GetChildrenEducationDocumentsList(employeeId, childrenEduHdrID);
                return PartialView("_ReceiptDocumentPopUp", documentViewModel);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult DeleteDocument(int receiptId, string fileName, int employeeId, int childrenEduHdrID)
        {
            log.Info($"ChildrenEducation/DeleteDocument");
            try
            {
                ChildrenEducationViewModel documentViewModel = new ChildrenEducationViewModel();
                var flag = childrenService.DeleteReceiptsDocuments(receiptId);
                if (flag)
                {
                    string fullPath = Request.MapPath("~/Document/ChildrenEducationReceipts/" + fileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                documentViewModel.childrenEducationDocuments = childrenService.GetChildrenEducationDocumentsList(employeeId, childrenEduHdrID);
                return PartialView("_ReceiptDocumentPopUp", documentViewModel);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UpdateEmployeeDependent(int EmployeeId, int ChildrenEduHdrID)
        {
            log.Info($"ChildrenEducation/UpdateEmployeeDependent");
            try
            {
                var dependentList = childrenService.GetDependentList(EmployeeId);
                var savedDependent = childrenService.GetChildrenEducationDetails(EmployeeId, ChildrenEduHdrID);
                var result = dependentList.Where(p => !savedDependent.Any(p2 => p2.EmpDependentID == p.id));
                var deletedResult = savedDependent.Where(p => !dependentList.Any(p2 => p2.id == p.EmpDependentID)).ToList();

                List<Model.ChildrenEducationDetails> childrenEduDetails = new List<Model.ChildrenEducationDetails>();
                foreach (var notexists in result)
                {
                    Model.ChildrenEducationDetails details = new Model.ChildrenEducationDetails();
                    details.EmpDependentID = notexists.id;
                    details.ChildrenEduHdrID = ChildrenEduHdrID;
                    details.NotApplicable = false;
                    details.CreatedBy = userDetail.UserID;
                    details.CreatedOn = DateTime.Now;
                    childrenEduDetails.Add(details);
                }
                var flag = childrenService.UpdateEmployeeDependent(childrenEduDetails, deletedResult);
                return Json(new { msgType = "Updated", empID = EmployeeId, childrenEduHdrId = ChildrenEduHdrID }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}