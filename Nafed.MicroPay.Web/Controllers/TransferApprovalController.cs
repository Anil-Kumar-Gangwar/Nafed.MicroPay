using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers
{
    public class TransferApprovalController : BaseController
    {
        private readonly ITransferApprovalService transferApprovalService;

        public TransferApprovalController(ITransferApprovalService transferApprovalService)
        {
            this.transferApprovalService = transferApprovalService;
        }
        // GET: TransferApproval
        public ActionResult Index()
        {
            TransferApprovalViewModel transferApprovalViewModel = new TransferApprovalViewModel();
            transferApprovalViewModel.employeeDetail = new List<Nafed.MicroPay.Model.TrainingParticipantsDetail>();
            transferApprovalViewModel.ProcessList = transferApprovalService.GetProcessList();
            return View(transferApprovalViewModel);
        }

        public ActionResult AddTransferEmployee(int type)
        {
            try
            {
                TransferApprovalViewModel trnVM = new TransferApprovalViewModel();
                trnVM.type = type;
                trnVM.employeeDetail = transferApprovalService.GetTransferEmployeeDetailsList(type);

                return PartialView("_TransferEmployeeDetails", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult _PostTransferApproval(FormCollection collection, TransferApprovalViewModel transferApprovalViewModel)
        {
            try
            {
                var fromEmployeeID = Convert.ToInt32(collection.Get("fromEmployeeID"));
                var toEmployeeID = Convert.ToInt32(collection.Get("toEmployeeID"));
                var ProcessId = Convert.ToString(collection.Get("ProcessId")) == "" ? (int?)null : Convert.ToInt32(collection.Get("ProcessId"));
                var flag = transferApprovalService.TransferApproval(fromEmployeeID, toEmployeeID, ProcessId);

                return Json(new { saved = flag, msgType = flag == true ? "success" : "error", msg = flag == true ? "Transfer approval rights successfully." : "Transfer approval rights failed!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

    }
}