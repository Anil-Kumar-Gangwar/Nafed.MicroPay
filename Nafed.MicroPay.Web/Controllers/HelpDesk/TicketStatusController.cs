using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services.HelpDesk;
using Nafed.MicroPay.Model;
namespace MicroPay.Web.Controllers.HelpDesk
{
    public class TicketStatusController : BaseController
    {
        private readonly ITicketStatusService ticketStatusServ;

        public TicketStatusController(ITicketStatusService ticketStatusServ)
        {
            this.ticketStatusServ = ticketStatusServ;

        }

        // GET: TicketStatus
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _GetTicketStatusGridView()
        {
            log.Info("TicketStatusController/_GetTicketStatusGridView");
            try
            {
                List<TicketStatus> objTicketStatusList = new List<TicketStatus>();
                objTicketStatusList = ticketStatusServ.GetTicketStatusList();
                return PartialView("_TicketStatusList", objTicketStatusList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info("TicketStatusController/Create");
            try
            {
                TicketStatus objTicketStatus = new TicketStatus();
                return View(objTicketStatus);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TicketStatus createTicketStatus)
        {
            log.Info("TicketStatusController/Create");
            try
            {

                if (ModelState.IsValid)
                {
                    createTicketStatus.code = createTicketStatus.code.Trim();
                    createTicketStatus.description = createTicketStatus.description.Trim();
                    if (ticketStatusServ.TicketStatusExists(createTicketStatus.code, null))
                    {
                        ModelState.AddModelError("TicketStatusAlreadyExist", "Ticket Status Already Exist.");
                        return View(createTicketStatus);
                    }
                    createTicketStatus.CreatedOn = DateTime.Now;
                    createTicketStatus.description = createTicketStatus.description.Trim();
                    createTicketStatus.CreatedBy = userDetail.UserID;

                    int acadmicProfessionalID = ticketStatusServ.InsertTicketStatus(createTicketStatus);
                    TempData["Message"] = "Ticket Status created successfully.";
                    return RedirectToAction("Index");

                }
                else
                {
                    return View(createTicketStatus);
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }

        }

        [HttpGet]
        public ActionResult Edit(int ticketStatusID)
        {
            log.Info("TicketStatusController/Edit");
            try
            {
                TicketStatus objTicketStatus = new TicketStatus();
                objTicketStatus = ticketStatusServ.GetTicketStatusByID(ticketStatusID);
                return View(objTicketStatus);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TicketStatus updateTicketStatus)
        {
            log.Info("TicketStatusController/Edit");
            try
            {

                if (!string.IsNullOrEmpty(updateTicketStatus.code) && !string.IsNullOrEmpty(updateTicketStatus.description))
                {
                    updateTicketStatus.code = updateTicketStatus.code.Trim();
                    updateTicketStatus.description = updateTicketStatus.description.Trim();

                    if (ticketStatusServ.TicketStatusExists(updateTicketStatus.code, updateTicketStatus.description))
                        ModelState.AddModelError("TicketStatusAlreadyExist", "Ticket Status Already Exist.");
                }
                if (ModelState.IsValid)
                {
                    ticketStatusServ.UpdateTicketStatus(updateTicketStatus);
                    TempData["Message"] = "Ticket Status updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(updateTicketStatus);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(updateTicketStatus);
        }

        public ActionResult Delete(int ticketStatusID)
        {
            log.Info("TicketStatusController/Delete");
            try
            {
                ticketStatusServ.Delete(ticketStatusID);
                TempData["Message"] = "Ticket Status deleted successfully.";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }
    }
}