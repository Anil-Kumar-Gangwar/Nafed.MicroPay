using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Controllers
{
    public class UpdatePFNoController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IEmployeePFOrganisationService empPfOrgService;
        public UpdatePFNoController(IDropdownBindService ddlService, IEmployeePFOrganisationService empPfOrgService)
        {
            this.empPfOrgService = empPfOrgService;
            this.ddlService = ddlService;
        }
        // GET: UpdatePFNo
        public ActionResult Index()
        {
            log.Info($"UpdatePFNo/Index/");
            return View();
        }

        public ActionResult _GetFilter()
        {
            log.Info($"UpdatePFNo/_GetFilter/");
            EmpPFOpBalance pfBalance = new EmpPFOpBalance();
            pfBalance.branches = ddlService.ddlBranchList(null, null);
            return PartialView("_Filter", pfBalance);
        }


        public ActionResult _GetUnAssignedPFList(EmpPFOpBalance model)
        {
            log.Info($"UpdatePFNo/_GetUnAssignedPFList");
            try
            {
                var dataList = empPfOrgService.GetUnAssignedPFRecords(model.BranchID);
                return PartialView("_UnAssignedPFRecords", dataList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostListItemRow(int employeeID, int pfNo)
        {
            log.Info($"UpdatePFNo/_PostListItemRow/");
            try
            {
                if (pfNo > 0)
                {
                    var res = empPfOrgService.UpdatePFNo(employeeID, pfNo);
                    if (res)
                        return Json(new { msgType = "success", msg = "PFNo updated successfully." }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { msgType = "error", msg = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { msgType = "error", msg = "Please enter valid PFNo." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _updateAccountDetails(int eID, int? pfNo, string uan, string epfo, string pan, string aadhar, string ac, string bankCode, string ifscCode)
        {
            log.Info($"UpdatePFNo/_updateAccountDetails/");
            try
            {
                if (eID > 0)
                {
                    
                    //var res = true;
                    var res = empPfOrgService.UpdateEmpAccountDetail(eID, pfNo, uan, epfo, pan, aadhar, ac, bankCode, ifscCode, userDetail.UserID);
                    if (res)
                        return Json(new { msgType = "success", msg = "Personally identifiable information updated." }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { msgType = "error", msg = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { msgType = "error", msg = "Something went wrong, please try again." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}