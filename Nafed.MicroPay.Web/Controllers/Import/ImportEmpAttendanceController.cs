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

namespace MicroPay.Web.Controllers.Import
{
    public class ImportEmpAttendanceController : BaseController
    {
        private readonly IDropdownBindService dropdownService;
        private readonly IMarkAttendance markAttendance;
        private readonly IImportAttendanceService importAttendanceService;
        public ImportEmpAttendanceController(IDropdownBindService dropdownService,
            IMarkAttendance markAttendance, IImportAttendanceService importAttendanceService
            )
        {
            this.dropdownService = dropdownService;
            this.markAttendance = markAttendance;
            this.importAttendanceService = importAttendanceService;
        }
        // GET: ImportEmpAttendance
        public ActionResult Index()
        {
            log.Info("ImportEmpAttendanceController/Index");
            return View();
        }

        private void BindDropDown()
        {
            log.Info("ImportEmpAttendanceController/BindDropDown");

            var ddlBranchList = dropdownService.ddlBranchList();
            SelectListModel selectBranch = new SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");

            var ddlEmployeeTypeList = dropdownService.ddlEmployeeTypeList();
            SelectListModel selectEmployeeType = new SelectListModel();
            selectEmployeeType.id = 0;
            selectEmployeeType.value = "Select";
            ddlEmployeeTypeList.Insert(0, selectEmployeeType);
            ViewBag.ddlEmployeeType = new SelectList(ddlEmployeeTypeList, "id", "value");
        }

        [HttpPost]
        public ActionResult _ExportTemplate(FormCollection frm)
        {
            log.Info("ImportEmpAttendanceController/_ExportTemplate");
            var branchID = Request.Form["BranchID"];
            var EmployeeTypeId = Request.Form["EmployeeTypeId"] == "" ? (int?)null : Convert.ToInt16(Request.Form["EmployeeTypeId"]);
            DateTime? date = Request.Form["date"] == "" ? (DateTime?)null : Convert.ToDateTime(Request.Form["date"]);
            if (!string.IsNullOrEmpty(branchID) && date != null)
            {
                string fileName = string.Empty, msg = string.Empty;
                string fullPath = Server.MapPath("~/FileDownload/");
                fileName = ExtensionMethods.SetUniqueFileName("AttendanceSheet-", FileExtension.xlsx);
                var res = markAttendance.GetAttendanceForm(int.Parse(branchID), fileName, fullPath, EmployeeTypeId, date.Value);
                return Json(new { fileName = fileName, fullPath = fullPath + fileName, message = "success" });
            }
            else
            {
                BindDropDown();
                if (string.IsNullOrEmpty(branchID))
                    ModelState.AddModelError("BranchRequired", "Select Branch");
                if (date == null)
                    ModelState.AddModelError("DateRequired", "Select Date");

                return PartialView("_ExportTemplate");
            }
        }

        public ActionResult _ExportTemplate()
        {
            log.Info("ImportEmpAttendanceController/_ExportTemplate");
            BindDropDown();
            return PartialView("_ExportTemplate");
        }

        public ActionResult _ImportAttendance()
        {
            log.Info("ImportEmpAttendanceController/_ImportAttendance");
            return PartialView("_ImportAttendance");
        }

        [HttpPost]
        public ActionResult _ImportAttendance(HttpPostedFileBase file)
        {
            log.Info("ImportEmpAttendanceController/_ImportAttendance");

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

                        var objAttendance = importAttendanceService.ReadAttendanceImportExcelData(sPhysicalPath, false, out msg, out error, out warning);
                        //  IList<EmpAttendanceForm> res = objAttendance.ToList();

                        duplicateEntries = objAttendance.GroupBy(x => new { x.EmployeeID, x.InDate }).Sum(g => g.Count() - 1);
                        var getDuplicateRows = ExtensionMethods.FindDuplicates(objAttendance, x => new { x.EmployeeID, x.InDate });
                        objAttendance.Join(getDuplicateRows, (x) => new { x.EmployeeID, x.InDate }, (y) => new { y.EmployeeID, y.InDate }, (x, y) =>
                        {
                            x.isDuplicatedRow = (((x.EmployeeID == y.EmployeeID) && (x.InDate == y.InDate)) ? true : false);
                            return x;
                        }).ToList();

                        if (error == 0 && duplicateEntries > 0)
                            error = duplicateEntries;
                        else
                            error += duplicateEntries;

                        TempData["attendanceData"] = objAttendance;
                        TempData["error"] = error;
                        TempData.Keep("attendanceData");
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
        public PartialViewResult ImportAttendanceGridView()
        {
            log.Info($"MarkAttendanceController/ImportAttendanceGridView");
            try
            {
                AttendanceImportViewModel attendanceImportVM = new AttendanceImportViewModel();
                List<EmpAttendanceForm> empAttandanceForm = new List<EmpAttendanceForm>();
                attendanceImportVM.ErrorMsgCollection = GetErrorMessage();
                if (TempData["attendanceData"] != null)
                {
                    attendanceImportVM.attendanceData = (List<EmpAttendanceForm>)TempData["attendanceData"];
                    TempData.Keep("attendanceData");
                    //attendanceImportVM.NoOfErrors = TempData["error"] != null ? (int)TempData["error"] : 0;
                }
                else
                    attendanceImportVM.attendanceData = empAttandanceForm;
                attendanceImportVM.NoOfErrors = TempData["error"] != null ? (int)TempData["error"] : 0;

                return PartialView("_ImportAttendanceGridView", attendanceImportVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult SubmitFileData()
        {
            log.Info("ImportEmpAttendanceController/_ImportAttendance");
            try
            {
                var importDataList = (List<EmpAttendanceForm>)TempData["attendanceData"];
                var result = importAttendanceService.ImportAttendanceDetails(userDetail.UserID, userDetail.UserTypeID, importDataList);

                if (result == 1)
                {
                    return JavaScript("window.location = '" + Url.Action("Index", "EmployeeAttendancedetails") + "'");
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
    }
}