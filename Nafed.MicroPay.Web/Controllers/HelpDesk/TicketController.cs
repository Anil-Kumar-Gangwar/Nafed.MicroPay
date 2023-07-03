using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.HelpDesk;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Common;
using System.IO;
using MicroPay.Web.Models;
using static Nafed.MicroPay.Common.FileHelper;
using System.Text;

namespace MicroPay.Web.Controllers.HelpDesk
{
    public class TicketController : BaseController
    {
        private readonly ITicketService ticketServ;
        private readonly IDropdownBindService ddlService;
        public TicketController(ITicketService ticketServ, IDropdownBindService ddlService)
        {
            this.ticketServ = ticketServ;
            this.ddlService = ddlService;

        }
        // GET: Ticket
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _GetTicketGridView()
        {
            log.Info("TicketController/_GetTicketGridView");
            try
            {
                List<Ticket> objTicketList = new List<Ticket>();
                objTicketList = ticketServ.GetTicketList(userDetail.EmployeeID);
                return PartialView("_MyTicketList", objTicketList);
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
            log.Info("TicketController/Create");
            try
            {
                Ticket objTicket = new Ticket();
                objTicket.name = userDetail.EmployeeCode + " - " + userDetail.FullName;
                BindDropDown();
                return View(objTicket);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _AddDocumentRow()
        {
            TicketAttachment tAttachment = new TicketAttachment();
            return Json(new { htmlData = ConvertViewToString("_UploadDocument", tAttachment) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult _RemoveDocumentRow(string docID)
        {
            bool flag;
            string DocPathName;
            var dcID = Convert.ToInt32(docID);
            flag = ticketServ.DeleteDocument(dcID, out DocPathName);
            if (flag)
            {
                string fullPath = Request.MapPath("~/Document/HelpDesk/" + DocPathName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            return Json(new { FileStatus = flag }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ticket createTicket, FormCollection frm)
        {
            log.Info("TicketController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createTicket.CreatedOn = DateTime.Now;
                    var getWrkFlowDesID = ticketServ.GetWorkFlowDesignation(createTicket.DepartmentID, createTicket.type_id);
                    if (getWrkFlowDesID == 0)
                    {
                        BindDropDown();
                        TempData["Error"] = "No Divisional Head define for this type of ticket.";
                        return View(createTicket);
                    }
                    else
                        createTicket.DesignationID = getWrkFlowDesID;

                    createTicket.status_id = (int)TicketTypeStatus.Open;
                    createTicket.CreatedBy = userDetail.UserID;
                    createTicket.CreatedOn = DateTime.Now;
                    createTicket.customer_id = userDetail.EmployeeID;
                    if (Request.Files.Count > 0)
                    {
                        string fileName = string.Empty; string filePath = string.Empty;
                        string fname = "";
                        //  Get all files from Request object  
                        HttpFileCollectionBase files = Request.Files;
                        var docname = frm.Get("DocName").ToString();
                        var documentName = docname.Split(',');
                        createTicket.TicketDocument = new TicketAttachment();

                        #region Check Mime Type
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            if (!IsValidFileName(file.FileName))
                            {
                                stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                                stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                                stringBuilder.Append($"I. a to z characters.");
                                stringBuilder.Append($"II. numbers(0 to 9).");
                                stringBuilder.Append($"III. - and _ with space.");
                            }
                            var contentType = GetFileContentType(file.InputStream);
                            var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                            if (dicValue != contentType)
                            {
                                stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                                stringBuilder.Append("<br>");
                            }
                        }
                        if (stringBuilder.ToString() != "")
                        {
                            TempData["Error"] = stringBuilder.ToString();
                            return RedirectToAction("Index");
                        }
                        #endregion

                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            fname = ExtensionMethods.SetUniqueFileName("Document-",
               Path.GetExtension(file.FileName));
                            fileName = fname;
                            createTicket.TicketDocument.docname = documentName[i];
                            createTicket.TicketDocument.docorgname = file.FileName;
                            createTicket.TicketDocument.docpathname = fileName;
                            createTicket.TicketDocument.CreatedBy = userDetail.UserID;
                            createTicket.TicketDocument.CreatedOn = DateTime.Now;
                            fname = Path.Combine(Server.MapPath("~/Document/HelpDesk/"), fname);
                            file.SaveAs(fname);
                        }
                    }
                    int res = ticketServ.InsertTicket(createTicket);
                    TempData["Message"] = "Ticket created successfully.";
                    return RedirectToAction("Index");

                }
                else
                {
                    BindDropDown();
                    return View(createTicket);
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
        public ActionResult Edit(int TicketID)
        {
            log.Info("TicketController/Edit");
            try
            {
                Ticket objTicket = new Ticket();
                objTicket.name = userDetail.EmployeeCode + " - " + userDetail.FullName;
                BindDropDown();
                objTicket = ticketServ.GetTicketByID(TicketID);
                return View(objTicket);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ticket updateTicket, FormCollection frm)
        {
            log.Info("TicketController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    var getWrkFlowDesID = ticketServ.GetWorkFlowDesignation(updateTicket.DepartmentID, updateTicket.type_id);
                    if (getWrkFlowDesID == 0)
                    {
                        BindDropDown();
                        TempData["Error"] = "No Divisional Head define for this type of ticket.";
                        return View(updateTicket);
                    }
                    else
                        updateTicket.DesignationID = getWrkFlowDesID;

                    updateTicket.UpdatedBy = userDetail.UserID;
                    updateTicket.UpdatedOn = DateTime.Now;

                    if (Request.Files.Count > 0)
                    {
                        string fileName = string.Empty; string filePath = string.Empty;
                        string fname = "";
                        //  Get all files from Request object  
                        HttpFileCollectionBase files = Request.Files;

                        #region Check Mime Type
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            if (!IsValidFileName(file.FileName))
                            {
                                stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                                stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                                stringBuilder.Append($"I. a to z characters.");
                                stringBuilder.Append($"II. numbers(0 to 9).");
                                stringBuilder.Append($"III. - and _ with space.");
                            }
                            var contentType = GetFileContentType(file.InputStream);
                            var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                            if (dicValue != contentType)
                            {
                                stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                                stringBuilder.Append("<br>");
                            }
                        }
                        if (stringBuilder.ToString() != "")
                        {
                            TempData["Message"] = stringBuilder.ToString();
                            return RedirectToAction("Index");
                        }
                        #endregion

                        var docname = frm.Get("DocName").ToString();
                        var documentName = docname.Split(',');
                        updateTicket.TicketDocument = new TicketAttachment();
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            fname = ExtensionMethods.SetUniqueFileName("Document-",
               Path.GetExtension(file.FileName));
                            fileName = fname;
                            updateTicket.TicketDocument.docname = documentName[i];
                            updateTicket.TicketDocument.docorgname = file.FileName;
                            updateTicket.TicketDocument.docpathname = fileName;
                            updateTicket.TicketDocument.CreatedBy = userDetail.UserID;
                            updateTicket.TicketDocument.CreatedOn = DateTime.Now;
                            fname = Path.Combine(Server.MapPath("~/Document/HelpDesk/"), fname);
                            file.SaveAs(fname);
                        }
                    }

                    ticketServ.UpdateTicket(updateTicket);
                    TempData["Message"] = "Ticket updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    BindDropDown();
                    return View(updateTicket);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(updateTicket);
        }

        public ActionResult Delete(int TicketID)
        {
            log.Info("TicketController/Delete");
            try
            {
                string DocPathName;
                bool flag = ticketServ.Delete(TicketID, out DocPathName);
                if (flag)
                {
                    string fullPath = Request.MapPath("~/Document/HelpDesk/" + DocPathName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                TempData["Message"] = "Ticket deleted successfully.";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }

        private void BindDropDown()
        {
            try
            {
                ViewBag.Department = ddlService.ddlDepartmentHavingTicket();
                var ddlTicketType = ddlService.GetTicketType();
                ViewBag.TypeList = new SelectList(ddlTicketType, "id", "value");

            }
            catch (Exception)
            {
            }
        }

        #region Ticket Initialization
        public ActionResult ViewTicket()
        {
            log.Info("TicketController/ViewTicket");
            try
            {
                CommonFilter cFilter = new CommonFilter();
                cFilter.StatusID = (int)TicketTypeStatus.Open;
                var enumData = from TicketTypeStatus e in Enum.GetValues(typeof(TicketTypeStatus))
                               select new
                               {
                                   ID = (int)e,
                                   Name = e.ToString()
                               };

                ViewBag.Status = new SelectList(enumData, "ID", "Name");
                return View(cFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return PartialView();
        }

        [HttpGet]
        public ActionResult TicketList()
        {
            log.Info("TicketController/TicketList");
            try
            {
                Models.HelpDesk helpDesk = new Models.HelpDesk();
                CommonFilter cFilter = new CommonFilter();
                cFilter.StatusID = (int)TicketTypeStatus.Open;
                cFilter.loggedInEmployee = userDetail.DepartmentID;
                cFilter.DesignationID = userDetail.DesignationID;
                var ticketList = ticketServ.GetTicketForSectionHead(cFilter);
                helpDesk.TList = ticketList;
                return PartialView("_TicketList", helpDesk);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return PartialView("_TicketList");
        }

        [HttpGet]
        public ActionResult GetTicketList(CommonFilter cFilter)
        {
            log.Info("TicketController/GetTicketList");
            try
            {
                Models.HelpDesk helpDesk = new Models.HelpDesk();
                cFilter.loggedInEmployee = userDetail.DepartmentID;
                cFilter.DesignationID = userDetail.DesignationID;
                var ticketList = ticketServ.GetTicketForSectionHead(cFilter);
                helpDesk.TList = ticketList;
                return PartialView("_TicketList", helpDesk);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return PartialView("_TicketList");
        }

        public ActionResult Assign(int ticketID, string subj, int empID)
        {
            log.Info($"TicketController/Assign/ticketID={ticketID}");
            try
            {
                Ticket tckt = new Ticket();
                ViewBag.Department = ddlService.ddlDepartmentList();
                tckt.ID = ticketID;
                tckt.subject = subj;
                tckt.customer_id = empID;
                List<SelectListModel> employee = new List<SelectListModel>();
                employee.Add(new SelectListModel
                { value = "Select", id = 0 });
                ViewBag.Employee = new SelectList(employee, "id", "value");
                return PartialView("_AdminTicketForward", tckt);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return PartialView();
        }

        public JsonResult GetEmployeeByDepartment(int departmentID)
        {
            log.Info($"TicketController/GetEmployeeByDepartment");
            try
            {
                var employeedetails = ddlService.GetEmployeeByDepartmentDesignationID(null, departmentID, null);
                SelectListModel selectemployeeDetails = new SelectListModel();
                selectemployeeDetails.id = 0;
                selectemployeeDetails.value = "Select";
                employeedetails.Insert(0, selectemployeeDetails);
                var emp = new SelectList(employeedetails, "id", "value");
                return Json(new { employees = emp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Assign(Ticket ticket)
        {
            log.Info("TicketController/Assign");
            try
            {
                bool flag = false;
                if (ticket.DepartmentID != 0 && ticket.agent_id.HasValue && ticket.agent_id.Value != 0)
                {
                    ProcessWorkFlow pWrokFlow = new ProcessWorkFlow();
                    pWrokFlow.SenderID = userDetail.EmployeeID;
                    pWrokFlow.SenderDepartmentID = userDetail.DepartmentID;
                    pWrokFlow.SenderDesignationID = userDetail.DesignationID;
                    pWrokFlow.ReceiverID = ticket.agent_id;
                    pWrokFlow.ReceiverDepartmentID = ticket.DepartmentID;
                    pWrokFlow.Scomments = ticket.Message;
                    pWrokFlow.StatusID = (int)TicketTypeStatus.Pending;
                    pWrokFlow.ProcessID = (int)WorkFlowProcess.HelpDesk;
                    pWrokFlow.ReferenceID = ticket.ID;
                    pWrokFlow.EmployeeID = (int)ticket.customer_id;
                    pWrokFlow.CreatedBy = userDetail.UserID;
                    pWrokFlow.CreatedOn = DateTime.Now;
                    flag = ticketServ.TicketForward(pWrokFlow);
                }
                else
                {
                    if (ticket.DepartmentID == 0)
                        ModelState.AddModelError("DepartmentModelError", "Please select department.");

                    if (!ticket.agent_id.HasValue || ticket.agent_id.Value == 0)
                        ModelState.AddModelError("EmployeeModelError", "Please select employee.");

                    ViewBag.Department = ddlService.ddlDepartmentList();

                    var employeedetails = ddlService.GetEmployeeByDepartmentDesignationID(null, ticket.DepartmentID, null);
                    SelectListModel selectemployeeDetails = new SelectListModel();
                    selectemployeeDetails.id = 0;
                    selectemployeeDetails.value = "Select";
                    employeedetails.Insert(0, selectemployeeDetails);
                    ViewBag.Employee = new SelectList(employeedetails, "id", "value");

                    return Json(new { status = 0, htmlData = ConvertViewToString("_AdminTicketForward", ticket) }, JsonRequestBehavior.AllowGet);
                }
                if (flag)
                    return Json(new { status = 1, type = "success", msg = "Ticket assign successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return PartialView("_AdminTicketForward", ticket);
        }

        [HttpGet]
        public ActionResult Tickets()
        {
            log.Info("TicketController/Tickets");
            try
            {
                return View("TicketContainer");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetPendingTicket()
        {
            log.Info($"TicketController/GetPendingTicket");
            try
            {
                Models.HelpDesk helpDesk = new Models.HelpDesk();              
               var tList = ticketServ.GetPendingTickets((int)userDetail.EmployeeID);
                TempData["pendingList"] = tList;
                helpDesk.TList = tList;              
                return PartialView("_PendingTicket", helpDesk);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " Datetime3Stamp-" + DateTime.Now);
                throw;
            }
        }

        public ActionResult GetAnsweredTicket()
        {
            log.Info($"TicketController/GetAnsweredTicket");
            try
            {
                Models.HelpDesk helpDesk = new Models.HelpDesk();
               var tList = new List<Ticket>();
                tList = ticketServ.GetAnsweredTickets((int)userDetail.EmployeeID);
                helpDesk.TList = tList;
                return PartialView("_AnsweredTicketList", helpDesk);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        [HttpGet]
        public ActionResult AnswerToTicket(int ticketID)
        {
            log.Info($"TicketController/AnswerToTicket/ticketID={ticketID}");
            try
            {
                Ticket tckt = new Ticket();
                List<Ticket> ticketList = (List<Ticket>)TempData["pendingList"];
                var ansTicket = ticketList.Where(x => x.ID == ticketID).FirstOrDefault();
                if (ansTicket != null)
                {
                    tckt = ansTicket;
                }
                TempData.Keep("pendingList");
                return PartialView("_AnswerPendingTicket", tckt);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return PartialView();
        }

        [HttpPost]
        public ActionResult AnswerToTicket(Ticket ticket)
        {
            log.Info("TicketController/AnswerToTicket");
            try
            {
                bool flag = false;
                if (!string.IsNullOrEmpty(ticket.TicketSolverRmk))
                {
                    ProcessWorkFlow pWrokFlow = new ProcessWorkFlow();
                    pWrokFlow.SenderID = userDetail.EmployeeID;
                    pWrokFlow.SenderDepartmentID = userDetail.DepartmentID;
                    pWrokFlow.SenderDesignationID = userDetail.DesignationID;
                    pWrokFlow.ReceiverID = ticket.agent_id;
                    pWrokFlow.ReceiverDepartmentID = ticket.DepartmentID;
                    pWrokFlow.Scomments = ticket.TicketSolverRmk;
                    pWrokFlow.StatusID = (int)TicketTypeStatus.Answered;
                    pWrokFlow.ProcessID = (int)WorkFlowProcess.HelpDesk;
                    pWrokFlow.ReferenceID = ticket.ID;
                    pWrokFlow.EmployeeID = (int)ticket.customer_id;
                    pWrokFlow.CreatedBy = userDetail.UserID;
                    pWrokFlow.CreatedOn = DateTime.Now;
                    flag = ticketServ.TicketForward(pWrokFlow);
                }
                else
                {
                    ModelState.AddModelError("TicketSolverRmkModelError", "Please enter your remark.");
                    return Json(new { status = 0, htmlData = ConvertViewToString("_AnswerPendingTicket", ticket) }, JsonRequestBehavior.AllowGet);
                }
                if (flag)
                    return Json(new { status = flag, type = "success", msg = "Ticket status updated successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return PartialView("_AnswerPendingTicket", ticket);
        }

        [HttpGet]
        public ActionResult MarkTicketAsResolved(int ticketID, string subj, int empID, int status)
        {
            log.Info($"TicketController/MarkTicketAsResolved/ticketID={ticketID}");
            try
            {
                Ticket tckt = new Ticket();
                ViewBag.Department = ddlService.ddlDepartmentList();
                tckt.ID = ticketID;
                tckt.subject = subj;
                tckt.customer_id = empID;
                tckt.status_id = status;
                var getTicketSolverRmk = ticketServ.GetTicketResolveDtl(ticketID);
                tckt.TicketSolverRmk = getTicketSolverRmk.TicketSolverRmk;
                tckt.name = getTicketSolverRmk.name;
                List<SelectListModel> employee = new List<SelectListModel>();
                employee.Add(new SelectListModel
                { value = "Select", id = 0 });
                ViewBag.Employee = new SelectList(employee, "id", "value");

                return PartialView("_MarkTicketAsResolve", tckt);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return PartialView();
        }

        [HttpPost]
        public ActionResult MarkTicketAsResolved(Ticket ticket, string ButtonType)
        {
            log.Info("TicketController/MarkTicketAsResolved");
            try
            {
                bool flag = false;
                if (ButtonType == "Answer & Mark as Resolved")
                {
                    if (!string.IsNullOrEmpty(ticket.TicketSolverRmk))
                    {
                        ProcessWorkFlow pWrokFlow = new ProcessWorkFlow();
                        pWrokFlow.SenderID = userDetail.EmployeeID;
                        pWrokFlow.SenderDepartmentID = userDetail.DepartmentID;
                        pWrokFlow.SenderDesignationID = userDetail.DesignationID;
                        pWrokFlow.ReceiverID = userDetail.EmployeeID;
                        pWrokFlow.ReceiverDepartmentID = userDetail.DepartmentID;
                        pWrokFlow.Scomments = ticket.TicketSolverRmk;
                        pWrokFlow.StatusID = (int)TicketTypeStatus.Answered;
                        pWrokFlow.ProcessID = (int)WorkFlowProcess.HelpDesk;
                        pWrokFlow.ReferenceID = ticket.ID;
                        pWrokFlow.EmployeeID = (int)ticket.customer_id;
                        pWrokFlow.CreatedBy = userDetail.UserID;
                        pWrokFlow.CreatedOn = DateTime.Now;

                        flag = ticketServ.TicketForward(pWrokFlow);

                        pWrokFlow.ReceiverID = (int)ticket.customer_id;
                        pWrokFlow.ReceiverDepartmentID = null;
                        pWrokFlow.Scomments = null;
                        pWrokFlow.StatusID = (int)TicketTypeStatus.Resolved;
                        // Mark ticket as resolved after insert data for Answered status
                        flag = ticketServ.TicketForward(pWrokFlow, ticket.TicketSolverRmk);
                    }
                    else
                    {
                        ModelState.AddModelError("TicketSolverRmkModelError", "Please enter your remark.");
                        return Json(new { status = 0, htmlData = ConvertViewToString("_MarkTicketAsResolve", ticket) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (ButtonType == "Resolved")
                {
                    ProcessWorkFlow pWrokFlow = new ProcessWorkFlow();
                    pWrokFlow.SenderID = userDetail.EmployeeID;
                    pWrokFlow.SenderDepartmentID = userDetail.DepartmentID;
                    pWrokFlow.SenderDesignationID = userDetail.DesignationID;
                    pWrokFlow.ReceiverID = (int)ticket.customer_id;
                    pWrokFlow.ReceiverDepartmentID = null;
                    pWrokFlow.Scomments = null;
                    pWrokFlow.StatusID = (int)TicketTypeStatus.Resolved;
                    pWrokFlow.ProcessID = (int)WorkFlowProcess.HelpDesk;
                    pWrokFlow.ReferenceID = ticket.ID;
                    pWrokFlow.EmployeeID = (int)ticket.customer_id;
                    pWrokFlow.CreatedBy = userDetail.UserID;
                    pWrokFlow.CreatedOn = DateTime.Now;

                    flag = ticketServ.TicketForward(pWrokFlow,"none");
                }

                if (flag)
                    return Json(new { status = 1, type = "success", msg = "Ticket resolved successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return PartialView("_MarkTicketAsResolve", ticket);
        }
        #endregion

        #region Ticket Status
        public ActionResult TicketSearch()
        {
            CommonFilter cFilter = new CommonFilter();
            cFilter.StatusID = (int)TicketTypeStatus.Open;
            var enumData = from TicketTypeStatus e in Enum.GetValues(typeof(TicketTypeStatus))
                           select new
                           {
                               ID = (int)e,
                               Name = e.ToString()
                           };

            ViewBag.Status = new SelectList(enumData, "ID", "Name");
            return View("SearchContainer", cFilter);
        }

        public ActionResult GetTicketTrackList()
        {
            log.Info($"TicketController/GetTicketTrackList");
            try
            {
                CommonFilter cFilter = new CommonFilter();
                cFilter.StatusID = (int)TicketTypeStatus.Open;
                var ticketList = ticketServ.GetTicketForSectionHead(cFilter);
                return PartialView("_TicketTrackingGridView", ticketList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
        public ActionResult GetTicketTrackingGV(CommonFilter cFilter)
        {
            log.Info($"TicketController/_GetTicketTrackingGV");
            try
            {
                var ticketList = ticketServ.GetTicketForSectionHead(cFilter);
                return PartialView("_TicketTrackingGridView", ticketList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="fileNo"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public ActionResult _GetTicketTrackingGVPopup(int tID, string sub)
        {
            log.Info($"TicketController/_GetTicketTrackingGVPopup/fileID={tID}");
            try
            {
                Ticket objTicket = new Ticket();
                var ticketForwardDtl = ticketServ.GetTicketForwardDetails(tID);
                objTicket.subject = sub;
                objTicket.ID = tID;
                objTicket.TForwardList = ticketForwardDtl;
                return PartialView("_TicketTrackingGridViewPopup", objTicket);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        [HttpGet]
        public ActionResult TicketPriority(int ticketID, string subj)
        {
            log.Info($"TicketController/TicketPriority");
            try
            {
                Ticket objTicket = new Ticket();
                objTicket.ID = ticketID;
                objTicket.subject = subj;
                ViewBag.TicketPriority = ddlService.GetTicketPriority();
                return PartialView("_SetPriority", objTicket);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        [HttpPost]
        public ActionResult SetTicketPriority(Ticket objTicket)
        {
            log.Info($"TicketController/SetTicketPriority");
            try
            {
                if (objTicket.priority_id.HasValue)
                {                   
                    var res = ticketServ.SetTicketPriority(objTicket);
                    if (res)
                    {                        
                        return Json(new { status = res, type = "success", msg = "Ticket priority set successfully." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = res, type = "error", msg = "Unable to set ticket priority." }, JsonRequestBehavior.AllowGet);
                    }
                }                 
                
                else
                {
                    return PartialView("_SetPriority",objTicket);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
        #endregion

        #region Ticket Rollback

        public PartialViewResult TicketRollback(int ticketID,int empId)
        {
            log.Info("TicketController/MarkTicketAsResolved");
            Models.HelpDesk helpDesk = new Models.HelpDesk();
            try
            {
                ProcessWorkFlow pWrokFlow = new ProcessWorkFlow();
                pWrokFlow.SenderID = userDetail.EmployeeID;
                pWrokFlow.SenderDepartmentID = userDetail.DepartmentID;
                pWrokFlow.SenderDesignationID = userDetail.DesignationID;
                pWrokFlow.ReceiverID = userDetail.EmployeeID;
                pWrokFlow.ReceiverDepartmentID = userDetail.DepartmentID;
                pWrokFlow.Scomments = "Rollback";
                pWrokFlow.StatusID = (int)TicketTypeStatus.Open;
                pWrokFlow.ProcessID = (int)WorkFlowProcess.HelpDesk;
                pWrokFlow.ReferenceID = ticketID;
                pWrokFlow.EmployeeID = empId;
                pWrokFlow.CreatedBy = userDetail.UserID;
                pWrokFlow.CreatedOn = DateTime.Now;
                ticketServ.TicketRollback(pWrokFlow);


                CommonFilter cFilter = new CommonFilter();
                cFilter.StatusID = (int)TicketTypeStatus.Open;
                cFilter.loggedInEmployee = userDetail.DepartmentID;
                cFilter.DesignationID = userDetail.DesignationID;
                var ticketList = ticketServ.GetTicketForSectionHead(cFilter);
                helpDesk.TList = ticketList;
                return PartialView("_TicketList", helpDesk);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
            }
            return PartialView("_TicketList", helpDesk);
        }

        #endregion
    }
}