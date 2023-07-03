using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services.ArchiveData;
using Nafed.MicroPay.Model;
using System.IO;
using Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers.ArchiveData
{
    public class ArchiveDataController : BaseController
    {
        private readonly IArchiveDataService archiveDataService;
        public ArchiveDataController(IArchiveDataService archiveDataService)
        {
            this.archiveDataService = archiveDataService;
        }
        // GET: ArchiveData
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _GetArchivedDataTrans()
        {
            log.Info($"ArchiveDataController/_GetArchivedDataTrans");

            var tranList = archiveDataService.GetArchivedDataTransList();
            return PartialView("_ArchivedDataSummary", tranList);

        }

        public ActionResult _GetArchiveDataForm()
        {
            log.Info($"ArchiveDataController/_GetArchiveDataForm");
            ArchivedDataTransaction model = new ArchivedDataTransaction();
            return PartialView("_ArchiveDataForm", model);
        }

        [HttpPost]
        public ActionResult _PostArchiveData(ArchivedDataTransaction model)
        {
            log.Info($"ArchiveDataController/_PostArchiveData");

            bool res;
            if (ModelState.IsValid)
            {
                if (!archiveDataService.CheckArchiveExistForGivenYr(model.SelectedYear))
                {
                    var todate = new DateTime(model.SelectedYear, DateTime.Now.Month, DateTime.Now.Day);
                    var fromdate = todate.AddYears(-5);

                    DirectoryInfo sourcDirinfo = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Document"));
                    DirectoryInfo destDirinfo = new DirectoryInfo(DocumentUploadFilePath.ArchiveDataPath);

                    long destDirAvailSize;
                    long sourceDirSize = archiveDataService.DirectorySize(sourcDirinfo, destDirinfo.Root.ToString(), out destDirAvailSize);
                    long extraSize = 5242880; // 5MB
                    destDirAvailSize = (destDirAvailSize - extraSize); // add extra 5MB to destination, to avoid code failure due to insufficient of space.
                    if (destDirAvailSize > sourceDirSize)
                    {
                        res = archiveDataService.ArchiveData(userDetail.UserID,fromdate.Date, todate.Date);
                        if (res)
                        {
                            return Json(new { status = res, msgType = "success", message = "Archival process has been successfully completed." },JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("DataError", "There is not sufficient space available in the web server to perform Data Archival.");
                    }
                }
                else
                {
                    ModelState.AddModelError("DataError", "Archival process already done for the selected year. Please check detail in Data Archive Transaction List.");
                }
            }

            return PartialView("_ArchiveDataForm", model);
        }
    }
}