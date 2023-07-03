using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using System.IO;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Common;
using static Nafed.MicroPay.Common.FileHelper;

namespace MicroPay.Web.Controllers.Salary
{
    public class UploadTCSFileController : BaseController
    {
        private readonly IControlSettingService controlSettingService;
        public UploadTCSFileController(IControlSettingService controlSettingService)
        {
            this.controlSettingService = controlSettingService;
        }
        // GET: UploadTCSFile
        public ActionResult Index()
        {
            log.Info($"UploadTCSFile/Index");
            MonthlyTCSFile mTCSFile = new MonthlyTCSFile();
            return View(mTCSFile);
        }

        public ActionResult GetTCSFileList()
        {
            log.Info($"UploadTCSFile/GetTCSFileList");
            List<MonthlyTCSFile> monthlyTCSFiles = new List<MonthlyTCSFile>();

            monthlyTCSFiles = controlSettingService.GetMonthlyTCSFiles();

            if (monthlyTCSFiles?.Count > 0)
                monthlyTCSFiles.ForEach(x =>
                {
                    x.TCSFileUNCPath =
                     System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DocumentUploadFilePath.TCSFilePath + "/" + x.TCSFileName)) ?

                   Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DocumentUploadFilePath.TCSFilePath + "/" + x.TCSFileName) :
                   "#";
                });
            return PartialView("_MonthlyTCSFiles", monthlyTCSFiles);
        }

        [HttpPost]
        public ActionResult UploadFile()
        {
            log.Info($"UploadTCSFile/UploadFile");

            MonthlyTCSFile mTCSFile = new MonthlyTCSFile();

            if (Request.Files.Count > 0)
            {
                int monthID = Convert.ToInt32(Request["MonthID"]);
                int yearID = Convert.ToInt32(Request["YearID"]);

                var tcsFilePeriod = monthID.ToString("00") + yearID;

                string fileName = string.Empty; string filePath = string.Empty;
                string fname = "";

                int tcsFileID = Convert.ToInt32(Request["TCSFileID"]);
                try
                {
                    HttpFileCollectionBase files = Request.Files;

                    HttpPostedFileBase file = files[0];
                    var contentType = GetFileContentType(file.InputStream);
                    var dicValue_xlxs = GetDictionaryValueByKeyName(".xlsx");
                    var dicValue_xls = GetDictionaryValueByKeyName(".xls");

                    string fileExtension = Path.GetExtension(file.FileName), msg = string.Empty;
                    int error = 0, warning = 0;
                    if ((dicValue_xlxs == contentType || dicValue_xls == contentType) && (fileExtension.ToLower() == ".xlsx" || fileExtension.ToLower() == ".xls"))
                    {
                        #region Upload File At temp location to valid sheet columns name ===

                        var tcsFileName = $"TCS{monthID.ToString("00")}{yearID.ToString().Substring(2, 2)}";

                        fileName = $"{tcsFileName}{Path.GetExtension(file.FileName)}";

                        filePath = /*DocumentUploadFilePath.ImportFilePath*/  DocumentUploadFilePath.TCSFileImportFilePath;
                        string uploadedFilePath = Server.MapPath(filePath);
                        string sPhysicalPath = Path.Combine(uploadedFilePath, fileName);
                        file.SaveAs(sPhysicalPath);

                        List<string> missingHeaders, invalidColumns;
                        var isValidHeader = controlSettingService.ReadTCSFileData(sPhysicalPath, false, out missingHeaders, out invalidColumns, out msg, out error, out warning);

                        if (!isValidHeader && invalidColumns.Count == 0)
                            return Json(new { status = "columnMissing", missingHeaders = missingHeaders });

                        else if (!isValidHeader && invalidColumns.Count > 0)
                            return Json(new { status = "inValidColumns", inValidColumns = invalidColumns });
                        #endregion

                        //fname = tcsFilePeriod + fileExtension;
                        //fileName = fname;
                        string fullPath = Request.MapPath("~/Document/MonthlyTCSFile/" + fileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        fname = Path.Combine(Server.MapPath("~/Document/MonthlyTCSFile/"), fileName);
                        file.SaveAs(fname);
                    }
                    else
                    {
                        return Json("inValidFileFormat");
                    }

                    #region Save Monthly TCS File ======

                    mTCSFile.TCSFileID = tcsFileID;
                    mTCSFile.CreatedBy = userDetail.UserID;
                    mTCSFile.TCSFileName = fileName;
                    mTCSFile.CreatedOn = DateTime.Now;
                    mTCSFile.TCSFilePeriod = tcsFilePeriod;

                    if (!controlSettingService.IsMonthlyTCSFileExists(tcsFilePeriod))
                    {
                        var mtcsFileID = controlSettingService.InsertMonthlyTCSFile(mTCSFile);

                        if (mtcsFileID > 0)
                        {
                            var tcsFiles = controlSettingService.GetMonthlyTCSFiles();
                            if (tcsFiles?.Count > 0)
                                tcsFiles.ForEach(x =>
                                {
                                    x.TCSFileUNCPath =
                                     System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DocumentUploadFilePath.TCSFilePath + "/" + x.TCSFileName)) ?
                                     Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DocumentUploadFilePath.TCSFilePath + "/" + x.TCSFileName) :
                                    "#";
                                });
                            return Json(new { part = 1, htmlData = ConvertViewToString("_MonthlyTCSFiles", tcsFiles) }, JsonRequestBehavior.AllowGet);
                        }
                        #endregion
                    }
                    else
                    {
                        mTCSFile.UpdatedBy = userDetail.UserID;
                        mTCSFile.UpdatedOn = DateTime.Now;
                        var updateTCSFile = controlSettingService.UpdateMonthlyTCSFile(mTCSFile);

                        if (updateTCSFile)
                        {
                            var tcsFiles = controlSettingService.GetMonthlyTCSFiles();
                            if (tcsFiles?.Count > 0)
                                tcsFiles.ForEach(x =>
                                {
                                    x.TCSFileUNCPath =
                                     System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DocumentUploadFilePath.TCSFilePath + "/" + x.TCSFileName)) ?

                                   Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DocumentUploadFilePath.TCSFilePath + "/" + x.TCSFileName) :
                                   "#";
                                });
                            return Json(new { part = 1, htmlData = ConvertViewToString("_MonthlyTCSFiles", tcsFiles) }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    return View();
                }
                catch (Exception ex)
                {
                    log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                    throw ex;
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
        //===
    }
}
