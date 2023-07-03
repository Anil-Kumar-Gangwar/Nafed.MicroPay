using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Conveyance_Bill
{
    public class ConveyanceBillHistoryController : BaseController
    {
        private readonly IConveyanceBillService conveyanceBillService;
        private readonly IDropdownBindService ddlServices;

        public ConveyanceBillHistoryController(IConveyanceBillService conveyanceBillService, IDropdownBindService ddlServices)
        {
            this.conveyanceBillService = conveyanceBillService;
            this.ddlServices = ddlServices;
        }

        // GET: ConveyanceBillHistory
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult _AdminConveyanceBillFilters()
        {
            log.Info($"ConveyanceBillHistoryController/_AdminConveyanceBillFilters");
            try
            {
                AppraisalFormApprovalFilter filters = new AppraisalFormApprovalFilter();
                filters.employees = ddlServices.GetAllEmployee();
                filters.reportingYrs = new List<SelectListModel>()
                {
                    new SelectListModel { id = 1, value = "2019-2020" },
                    new SelectListModel { id = 2, value = "2020-2021" },
                    new SelectListModel { id = 2, value = "2021-2022" },
                    new SelectListModel { id = 2, value = "2023-2024" }
                };
                return PartialView("_AdminConveyanceBillFilters", filters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostConveyanceBillFilters(AppraisalFormApprovalFilter filters)
        {
            log.Info($"ConveyanceBillHistoryController/_PostConveyanceBillFilters");
            try
            {
                if (filters.conveyanceFormState == ConveyanceFormStatus.SavedByEmployee)
                    filters.statusId = (int)ConveyanceFormStatus.SavedByEmployee;
                else if (filters.conveyanceFormState == ConveyanceFormStatus.SubmitedByEmployee)
                    filters.statusId = (int)ConveyanceFormStatus.SubmitedByEmployee;
                else if (filters.conveyanceFormState == ConveyanceFormStatus.SavedBySectionalHead)
                    filters.statusId = (int)ConveyanceFormStatus.SavedBySectionalHead;
                else if (filters.conveyanceFormState == ConveyanceFormStatus.SubmitedBySectionalHead)
                    filters.statusId = (int)ConveyanceFormStatus.SubmitedBySectionalHead;
                else if (filters.conveyanceFormState == ConveyanceFormStatus.SavedByDivisionalHead)
                    filters.statusId = (int)ConveyanceFormStatus.SavedByDivisionalHead;
                else if (filters.conveyanceFormState == ConveyanceFormStatus.SubmitedByDivisionalHead)
                    filters.statusId = (int)ConveyanceFormStatus.SubmitedByDivisionalHead;
                else
                    filters.statusId = null;

                var conveyanceForms = conveyanceBillService.GetConveyanceBillDetails(filters, "SP_ConveyanceBillHistoryDetails");
                return PartialView("_ConveyanceBillGridView", conveyanceForms);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}