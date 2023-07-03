using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;

namespace MicroPay.Web.Controllers
{
    public class ViewHolidaysController : BaseController
    {
        private readonly IHolidayService holidayService;
        public ViewHolidaysController(IHolidayService holidayService)
        {
            this.holidayService = holidayService;
        }
        // GET: ViewHolidays
        public ActionResult Index()
        {
            log.Info($"ViewHolidays/Index/");
            return View();
        }

        [HttpPost]
        public ActionResult _GetHolidayList(SelectListModel model)
        {
            log.Info($"ViewHolidays/_GetHolidayList/");
            try
            {
                List<Holiday> objHolidayList = new List<Holiday>();
                if(model.value!=null)
                   objHolidayList = holidayService.GetHolidayList(null, int.Parse(model.value));
                return PartialView("_HolidayGridView", objHolidayList);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}