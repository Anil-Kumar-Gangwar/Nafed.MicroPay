using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.HelpDesk;
namespace MicroPay.Web.Controllers.HelpDesk
{
    public class TicketPriorityController : BaseController
    {
        private readonly ITicketPriorityService ticketPriorityServ;
        public TicketPriorityController(ITicketPriorityService ticketPriorityServ)
        {
            this.ticketPriorityServ = ticketPriorityServ;

        }
        // GET: TicketPriority
        public ActionResult Index()
        {
            log.Info($"TicketPriorityController/Index");
            return View();
        }

        public ActionResult _GetTicketPriorityGridView()
        {
            log.Info("TicketPriorityController/_GetTicketPriorityGridView");
            try
            {
                List<TicketPriority> objTicketPriorityList = new List<TicketPriority>();
                objTicketPriorityList = ticketPriorityServ.GetTicketPriorityList();
                return PartialView("_TicketPriorityList", objTicketPriorityList);
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
            log.Info("TicketPriorityController/Create");
            try
            {
                TicketPriority objTicketPriority = new TicketPriority();
                return View(objTicketPriority);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TicketPriority createTicketPriority)
        {
            log.Info("TicketPriorityController/Create");
            try
            {

                if (ModelState.IsValid)
                {
                    createTicketPriority.code = createTicketPriority.code.Trim();
                    createTicketPriority.description = createTicketPriority.description.Trim();
                    if (ticketPriorityServ.TicketPriorityExists(createTicketPriority.code, null))
                    {
                        ModelState.AddModelError("TicketPriorityAlreadyExist", "Ticket Priority Already Exist.");
                        return View(createTicketPriority);
                    }
                    createTicketPriority.CreatedOn = DateTime.Now;
                    createTicketPriority.description = createTicketPriority.description.Trim();
                    createTicketPriority.CreatedBy = userDetail.UserID;

                    int acadmicProfessionalID = ticketPriorityServ.InsertTicketPriority(createTicketPriority);
                    TempData["Message"] = "Ticket Priority created successfully.";
                    return RedirectToAction("Index");

                }
                else
                {
                    return View(createTicketPriority);
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
        public ActionResult Edit(int ticketPriorityID)
        {
            log.Info("TicketPriorityController/Edit");
            try
            {
                TicketPriority objTicketPriority = new TicketPriority();
                objTicketPriority = ticketPriorityServ.GetTicketPriorityByID(ticketPriorityID);
                return View(objTicketPriority);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TicketPriority updateTicketPriority)
        {
            log.Info("TicketPriorityController/Edit");
            try
            {

                if (!string.IsNullOrEmpty(updateTicketPriority.code) && !string.IsNullOrEmpty(updateTicketPriority.description))
                {
                    updateTicketPriority.code = updateTicketPriority.code.Trim();
                    updateTicketPriority.description = updateTicketPriority.description.Trim();

                    if (ticketPriorityServ.TicketPriorityExists(updateTicketPriority.code, updateTicketPriority.description))
                        ModelState.AddModelError("TicketPriorityAlreadyExist", "Ticket Priority Already Exist.");
                }
                if (ModelState.IsValid)
                {
                    ticketPriorityServ.UpdateTicketPriority(updateTicketPriority);
                    TempData["Message"] = "Ticket Priority updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(updateTicketPriority);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(updateTicketPriority);
        }

        public ActionResult Delete(int ticketPriorityID)
        {
            log.Info("TicketPriorityController/Delete");
            try
            {
                ticketPriorityServ.Delete(ticketPriorityID);
                TempData["Message"] = "Ticket Priority deleted successfully.";
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