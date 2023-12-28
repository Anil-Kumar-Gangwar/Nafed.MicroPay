using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.IO;

namespace MicroPay.Web.Controllers
{
    public class UpdateBranchEmailController : BaseController
    {
        private readonly IBranchService branchService;
        private readonly IDropdownBindService ddlService;
        public UpdateBranchEmailController(IBranchService branchService, IDropdownBindService ddlService)
        {
            this.branchService = branchService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BranchGridView(FormCollection formCollection)
        {
            log.Info($"UpdateBranchEmailController/BranchGridView");
            try
            {
                BranchViewModel branchVM = new BranchViewModel();
                branchVM.branchList = branchService.GetBranchList();
                return PartialView("_BranchGridView", branchVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public PartialViewResult _GetBranchEmail(int branchId)
        {
            log.Info($"UpdateBranchEmailController/_GetBranchEmail{branchId}");
            try
            {
                Model.Branch objBranch = new Model.Branch();
                objBranch = branchService.GetBranchByID(branchId);
                return PartialView("Edit",objBranch);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult UpdateEmail(Model.Branch objBranch)
        {
            log.Info("UpdateBranchEmailController/UpdateEmail");
            try
            {
                if (branchService.EmailidExists(objBranch.EmailId, branchId: objBranch.BranchID))
                {
                    return Json(new { success = false, msg = "Email Address Already Exist." }, JsonRequestBehavior.AllowGet);
                }

                var result = branchService.UpdateBranchEmail(objBranch.BranchID, objBranch.EmailId);
                if (result)
                {
                    TempData["Message"] = "Branch email updated successfully.";
                    return Json(new { success = result, msg = "Branch email updated successfully." }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    return Json(new { success = result, msg = "Branch email updation failed!" }, JsonRequestBehavior.AllowGet);
                }
                  
               
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


    }
}