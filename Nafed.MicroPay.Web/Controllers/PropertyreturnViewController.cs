using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.Linq;
namespace MicroPay.Web.Controllers
{
    public class PropertyreturnViewController : BaseController
    {
        private readonly IPRService PRService;
        private readonly IDropdownBindService ddlService;

        public PropertyreturnViewController(IPRService PRService, IDropdownBindService ddlService)
        {
            this.PRService = PRService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            PropertyReturnViewModel PRVM = new PropertyReturnViewModel();
            PRVM.userRights = userAccessRight;
            return View(PRVM);
        }


        [HttpGet]
        public ActionResult _PropertyReturnView(PropertyReturnViewModel PRVM)
        {
            log.Info($"PropertyreturnViewController/_PropertyReturnGridView");
            try
            {
                PropertyReturnViewModel propertyreturnVM = new PropertyReturnViewModel();
                propertyreturnVM.PRList = PRService.GetPRViewList(PRVM.CYear == null ? 0 : (int)PRVM.CYear);
                propertyreturnVM.userRights = userAccessRight;
                return PartialView("_PropertyReturnView", propertyreturnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult _PRView(int? EmployeeID, int? PRID, int? Year)
        {
            log.Info($"PropertyreturnViewController/_PRView");
            try
            {
                PropertyReturnViewModel PRVM = new PropertyReturnViewModel();

                PRVM.userRights = userAccessRight;
                if (PRID == null)
                {
                    PRID = Convert.ToInt32(Session["PRID"]);
                }

                PRVM.PRList = PRService.GetPRGVList(EmployeeID == null ? 0 : (int)EmployeeID, (int)PRID, Year == null ? 0 : (int)Year);
                if (EmployeeID.HasValue)
                {
                    ViewBag.EmployeeID = EmployeeID.Value;
                    Session["EmployeeID"] = EmployeeID.Value;
                }
                if (Year.HasValue)
                {
                    ViewBag.Year = Year.Value;
                    Session["Year"] = Year.Value;
                }

                bool flag = false;
                Model.PR uDetail = new Model.PR();
                flag = PRService.getemployeeallDetails(0, EmployeeID == null ? 0 : (int)EmployeeID, out uDetail, Year);
                if (flag)
                {
                    ViewBag.DesignationName = uDetail.DesignationName;
                    ViewBag.BasicPay = uDetail.E_Basic;
                    ViewBag.Employeename = uDetail.Employeename;
                    ViewBag.BranchName = uDetail.BranchName;
                }
                
                return PartialView("_PRView", PRVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _PropertyReturnGridView(PropertyReturnViewModel PrRVM)
        {
            log.Info($"PropertyreturnViewController/_PropertyReturnGridView");
            try
            {
                PropertyReturnViewModel PRVM = new PropertyReturnViewModel();
                PRVM.userRights = userAccessRight;
                var CYear = DateTime.Now.Year;
                if(PrRVM.EmployeeId==null)
                {
                    PrRVM.EmployeeId = Convert.ToInt32(Session["EmployeeID"]);
                }
                if (PrRVM.EmployeeId == null)
                {
                    PRVM.PRList = PRService.GetPRGVList((int)PrRVM.EmployeeId, 0, 0);
                }
                else
                {
                    PRVM.PRList = PRService.GetPRGVList((int)PrRVM.EmployeeId, 0, 0);
                }
                return PartialView("_PropertyReturnGridView", PRVM);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

    }
}