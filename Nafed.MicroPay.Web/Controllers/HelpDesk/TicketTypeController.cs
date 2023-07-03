using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.HelpDesk;
namespace MicroPay.Web.Controllers.HelpDesk
{
    public class TicketTypeController : BaseController
    {
        private readonly ITicketTypeService ticketTypeServ;

        public TicketTypeController(ITicketTypeService ticketTypeServ)
        {
            this.ticketTypeServ = ticketTypeServ;

        }
        // GET: TicketType       
        public ActionResult Index()
        {
            log.Info($"TicketTypeController/Index");
            return View();
        }

        public ActionResult _GetTicketTypeGridView()
        {
            log.Info("TicketTypeController/_GetTicketTypeGridView");
            try
            {
                List<TicketType> objTicketTypeList = new List<TicketType>();
                objTicketTypeList = ticketTypeServ.GetTicketTypeList();
                return PartialView("_TicketTypeList", objTicketTypeList);
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
            log.Info("TicketTypeController/Create");
            try
            {
                TicketType objTicketType = new TicketType();
                return View(objTicketType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TicketType createTicketType)
        {
            log.Info("TicketTypeController/Create");
            try
            {

                if (ModelState.IsValid)
                {
                    createTicketType.code = createTicketType.code.Trim();
                    createTicketType.description = createTicketType.description.Trim();
                    if (ticketTypeServ.TicketTypeExists(createTicketType.code, null))
                    {
                        ModelState.AddModelError("TicketTypeAlreadyExist", "Ticket Type Already Exist.");
                        return View(createTicketType);
                    }
                    createTicketType.CreatedOn = DateTime.Now;
                    createTicketType.description = createTicketType.description.Trim();
                    createTicketType.CreatedBy = userDetail.UserID;

                    int acadmicProfessionalID = ticketTypeServ.InsertTicketType(createTicketType);
                    TempData["Message"] = "Ticket Type created successfully.";
                    return RedirectToAction("Index");

                }
                else
                {
                    return View(createTicketType);
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
        public ActionResult Edit(int ticketTypeID)
        {
            log.Info("TicketTypeController/Edit");
            try
            {
                TicketType objTicketType = new TicketType();
                objTicketType = ticketTypeServ.GetTicketTypeByID(ticketTypeID);
                return View(objTicketType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TicketType updateTicketType)
        {
            log.Info("TicketTypeController/Edit");
            try
            {

                if (!string.IsNullOrEmpty(updateTicketType.code) && !string.IsNullOrEmpty(updateTicketType.description))
                {
                    updateTicketType.code = updateTicketType.code.Trim();
                    updateTicketType.description = updateTicketType.description.Trim();

                    if (ticketTypeServ.TicketTypeExists(updateTicketType.code, updateTicketType.description))
                        ModelState.AddModelError("TicketTypeAlreadyExist", "Ticket Type Already Exist.");
                }
                if (ModelState.IsValid)
                {
                    ticketTypeServ.UpdateTicketType(updateTicketType);
                    TempData["Message"] = "Ticket Type updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(updateTicketType);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(updateTicketType);
        }

        public ActionResult Delete(int ticketTypeID)
        {
            log.Info("TicketTypeController/Delete");
            try
            {
                ticketTypeServ.Delete(ticketTypeID);
                TempData["Message"] = "Ticket Type deleted successfully.";
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