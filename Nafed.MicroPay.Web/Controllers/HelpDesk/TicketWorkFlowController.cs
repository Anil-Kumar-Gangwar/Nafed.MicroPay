using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.HelpDesk;
using Nafed.MicroPay.Services.IServices;

namespace MicroPay.Web.Controllers.HelpDesk
{
    public class TicketWorkFlowController : BaseController
    {
        private readonly ITicketService ticketServ;
        private readonly IDropdownBindService ddlService;
        public TicketWorkFlowController(ITicketService ticketServ, IDropdownBindService ddlService)
        {
            this.ticketServ = ticketServ;
            this.ddlService = ddlService;

        }
        // GET: TicketWorkFlow
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetWorkFlowList()
        {
            log.Info($"TicketWorkFlowController/GetWorkFlowList");
            try
            {
                List<TicketWorkFlow> fworkflowList = new List<TicketWorkFlow>();
                fworkflowList = ticketServ.GetTicketWorkFlowList();            
                return PartialView("_WorkFlowList", fworkflowList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info($"TicketWorkFlowController/Create");
            try
            {
                TicketWorkFlow tworkflow = new TicketWorkFlow();
                BindDropDown();
                return View(tworkflow);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        [HttpPost]
        public ActionResult Create(TicketWorkFlow tworkflow)
        {
            log.Info($"TicketWorkFlowController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    tworkflow.CreatedBy = userDetail.UserID;
                    tworkflow.CreatedOn = DateTime.Now;
                    int res = ticketServ.InsertTWorkFlow(tworkflow);
                    if (res > 0)
                    {
                        TempData["Message"] = "Successfully created.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        BindDropDown();
                        return View(tworkflow);
                    }
                }
                else
                {
                    BindDropDown();
                    return View(tworkflow);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        [HttpGet]
        public ActionResult Edit(int tWorkFlowID)
        {
            log.Info($"TicketWorkFlowController/Edit/tWorkFlowID={tWorkFlowID}");
            try
            {
                BindDropDown();
                var getTWorkFlow = ticketServ.GetTicketWorkFlowByID(tWorkFlowID);
                return View(getTWorkFlow);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        [HttpPost]
        public ActionResult Edit(TicketWorkFlow tworkflow)
        {
            log.Info($"TicketWorkFlowController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    tworkflow.UpdatedBy = userDetail.UserID;
                    tworkflow.UpdatedOn = DateTime.Now;
                    bool res = ticketServ.UpdateTWorkFlow(tworkflow);
                    if (res)
                    {
                        TempData["Message"] = "Successfully updated.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        BindDropDown();
                        return View(tworkflow);
                    }
                }
                else
                {
                    BindDropDown();
                    return View(tworkflow);
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        void BindDropDown()
        {
            ViewBag.Department = ddlService.ddlDepartmentHavingEmployee();
            ViewBag.Designation = ddlService.ddlDesignationListForTicket();
            ViewBag.TicketType = ddlService.GetTicketType();
        }

    }
}