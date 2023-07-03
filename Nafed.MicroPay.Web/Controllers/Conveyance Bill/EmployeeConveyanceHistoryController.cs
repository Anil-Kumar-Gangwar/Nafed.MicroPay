using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Conveyance_Bill
{
    public class EmployeeConveyanceHistoryController : BaseController
    {
        private readonly IConveyanceBillService conveyanceBillService;
        private readonly IDropdownBindService ddlServices;

        public EmployeeConveyanceHistoryController(IConveyanceBillService conveyanceBillService, IDropdownBindService ddlServices)
        {
            this.conveyanceBillService = conveyanceBillService;
            this.ddlServices = ddlServices;
        }

        // GET: EmployeeConveyanceHistory
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult _EmployeeConveyanceHistoryFilters()
        {
            log.Info($"EmployeeConveyanceHistory/_EmployeeConveyanceHistoryFilters");
            try
            {
                return PartialView("_EmployeeConveyanceHistoryFilters");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        
        [HttpPost]
        public ActionResult _PostEmployeeHistoryFilters(FormCollection frmCollection)
        {
            log.Info($"EmployeeConveyanceHistory/_PostEmployeeHistoryFilters");
            try
            {
                var fromDate = frmCollection["FromDate"];
                var toDate = frmCollection["ToDate"];
                var employeeId = userDetail.EmployeeID;
                var employeeHistoryDetails = conveyanceBillService.GetConveyanceEmployeeHistory(fromDate, toDate, employeeId);
                return PartialView("_EmployeeConveyanceHistoryGridView", employeeHistoryDetails);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}