using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System.Web.Mvc;
using Nafed.MicroPay.Common;
using System.Web;
using static Nafed.MicroPay.Common.FileHelper;
using System;
using System.IO;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.Linq;
using Nafed.MicroPay.Services.Arrear;

namespace MicroPay.Web.Controllers.Import
{
    public class ImportEmpSalaryManualDataController : BaseController
    {
        private readonly IDropdownBindService dropdownService;
        private readonly IArrearService arrearService;
        private readonly IImportManualDataService importManualdataService;

        public ImportEmpSalaryManualDataController(IDropdownBindService dropdownService, IImportManualDataService importManualdataService,
           IArrearService arrearService)
        {
            this.dropdownService = dropdownService;
            this.importManualdataService = importManualdataService;
            this.arrearService = arrearService;
        }
        public ActionResult Index()
        {
            log.Info("ImportEmpSalaryManualDataController/Index");
            return View();
        }

        private void LoadBranch()
        {
            log.Info("ImportEmpSalaryManualDataController/LoadBranch");

            var ddlBranchList = dropdownService.ddlBranchList();
            SelectListModel selectBranch = new SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");
        }

        [HttpPost]
        public ActionResult _ExportTemplate(FormCollection frm)
        {
            log.Info("ImportEmpSalaryManualDataController/_ExportTemplate");
            var branchID = Request.Form["BranchID"];
            var month = Request.Form["ddlMonth"];
            var year = Request.Form["ddlYear"];
            if (!string.IsNullOrEmpty(branchID) && !string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
            {
                string fileName = string.Empty, msg = string.Empty;
                string fullPath = Server.MapPath("~/FileDownload/");
                fileName = ExtensionMethods.SetUniqueFileName("ManualArrearDataSheet-", FileExtension.xlsx);
                var res = arrearService.GetManualdataForm(Convert.ToInt32( branchID), Convert.ToInt32(month), Convert.ToInt32(year), fileName, fullPath);
                return Json(new { fileName = fileName, fullPath = fullPath + fileName, message = "success" });
            }

            else
            {
                LoadBranch();
                if (string.IsNullOrEmpty(branchID))
                {
                  
                    ModelState.AddModelError("BranchRequired", "Select Branch");
                 }
                else if (string.IsNullOrEmpty(month))
                {
                    ModelState.AddModelError("MonthRequired", "Select Month");
                }
                else if (string.IsNullOrEmpty(year))
                {
                    ModelState.AddModelError("YearRequired", "Select Year");
                }
                return PartialView("_ExportTemplate");
            }
        }

        public ActionResult _ExportTemplate()
        {
            log.Info("ImportEmpSalaryManualDataController/_ExportTemplate");
            LoadBranch();
            return PartialView("_ExportTemplate");
        }


        public ActionResult _ImportSalManualData()
        {
            log.Info("ImportEmpSalaryManualDataController/_ImportSalManualData");
            return PartialView("_ImportSalManualData");
        }

        [HttpPost]
        public ActionResult _ImportSalManualData(HttpPostedFileBase file)
        {
            log.Info("ImportEmpSalaryManualDataController/_ImportSalManualData");

            try
            {
                string fileName = string.Empty; string filePath = string.Empty;
                int error = 0, warning = 0, duplicateEntries;
                string msg = string.Empty;
                if (file != null)
                {
                    string fileExtension = Path.GetExtension(file.FileName);
                    fileName = Path.GetFileName(file.FileName);
                    var contentType = GetFileContentType(file.InputStream);
                    var dicValue = GetDictionaryValueByKeyName(".xlsx");
                    if (dicValue == contentType && (fileExtension == ".xlsx" || fileExtension == ".xlx"))
                    {
                        #region Upload File At temp location===

                        fileName =
                                ExtensionMethods.SetUniqueFileName(Path.GetFileNameWithoutExtension(file.FileName),
                               Path.GetExtension(file.FileName));

                        filePath = DocumentUploadFilePath.ImportFilePath;
                        string uploadedFilePath = Server.MapPath(filePath);
                        string sPhysicalPath = Path.Combine(uploadedFilePath, fileName);
                        file.SaveAs(sPhysicalPath);

                        #endregion

                        var objManualData = importManualdataService.ReadAttendanceImportExcelData(sPhysicalPath, false, out msg, out error, out warning);
                        //  IList<EmpAttendanceForm> res = objAttendance.ToList();

                        duplicateEntries = objManualData.GroupBy(x => new { x.EmployeeId, x.Month,x.Year }).Sum(g => g.Count() - 1);
                        var getDuplicateRows = ExtensionMethods.FindDuplicates(objManualData, x => new { x.EmployeeId, x.Month, x.Year });
                        objManualData.Join(getDuplicateRows, (x) => new { x.EmployeeId, x.Month, x.Year }, (y) => new { y.EmployeeId, y.Month, y.Year }, (x, y) =>
                        {
                            x.isDuplicatedRow = (((x.EmployeeId == y.EmployeeId) && (x.Month == y.Month) && (x.Year == y.Year)) ? true : false);
                            return x;
                        }).ToList();

                        if (error == 0 && duplicateEntries > 0)
                            error = duplicateEntries;
                        else
                            error += duplicateEntries;

                        TempData["manualData"] = objManualData;
                        TempData["error"] = error;
                        TempData.Keep("manualData");
                        TempData.Keep("error");

                        return Json("success");
                    }
                    else
                    {
                        TempData["error"] = -2;
                        return Json("inValidFileFormat");
                    }
                }
                else
                {
                    TempData["error"] = -1;
                    return Json("nofileFound");
                    //   return Json(new { message = "nofileFound" });
                }
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult SubmitFileData()
        {
            log.Info("ImportEmpSalaryManualDataController/SubmitFileData");
            try
            {
                var importDataList = (List<ArrearManualData>)TempData["manualData"];
                var result = arrearService.ImportManualDataDetails(userDetail.UserID, userDetail.UserTypeID, importDataList);

                if (result == 1)
                {
                    return JavaScript("window.location = '" + Url.Action("Index", "ArrearManualData") + "'");
                }

                return Content("");
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);

                return Json(new { errorCode = -1, error = ex.Message }, JsonRequestBehavior.AllowGet);
                // throw;
            }

        }


        [HttpPost]
        public PartialViewResult ImportManualDataGridView()
        {
            log.Info($"ImportEmpSalaryManualDataController/ImportManualDataGridView");
            try
            {
                AttendanceImportViewModel attendanceImportVM = new AttendanceImportViewModel();
                attendanceImportVM.ErrorMsgCollection = GetErrorMessage();
                if (TempData["manualData"] != null)
                {
                    TempData.Keep("manualData");
                  
                }
                else
                attendanceImportVM.NoOfErrors = TempData["error"] != null ? (int)TempData["error"] : 0;
                return PartialView("_ImportManualDataGridView", attendanceImportVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}