using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.HelpDesk;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static Nafed.MicroPay.Common.FileHelper;
namespace MicroPay.Web.Controllers
{
    public class HelpDeskController : Controller
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger
          (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ITicketService ticketServ;
        private readonly IDropdownBindService ddlService;
        public HelpDeskController(ITicketService ticketServ, IDropdownBindService ddlService)
        {
            this.ticketServ = ticketServ;
            this.ddlService = ddlService;

        }
        // GET: HelpDesk
        public ActionResult Add()
        {
            log.Info("HelpDeskController/Add");
            try
            {
                Ticket objTicket = new Ticket();
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
        public ActionResult Add(Ticket createTicket, FormCollection frm)
        {
            log.Info("HelpDeskController/Add");
            try
            {
                if (ModelState.IsValid)
                {
                    int empId = 0;
                    int desgId = 0;
                    if (ticketServ.CheckEmployeeExist(createTicket.EmployeeCode, createTicket.DOJ.Value, createTicket.LastDepartment.Value, createTicket.BranchId.Value,createTicket.LastWorking.Value, out empId, out desgId))
                    {
                        createTicket.CreatedOn = DateTime.Now;
                        var getWrkFlowDesID = ticketServ.GetWorkFlowDesignation(createTicket.DepartmentID, createTicket.type_id);
                        if (getWrkFlowDesID == 0)
                        {
                            BindDropDown();
                            ViewBag.Error = "No Divisional Head define for this type of ticket.";
                            return View(createTicket);
                        }
                        else
                            createTicket.DesignationID = getWrkFlowDesID;

                        createTicket.status_id = (int)TicketTypeStatus.Open;
                        createTicket.CreatedBy = 1;
                        createTicket.CreatedOn = DateTime.Now;
                        createTicket.customer_id = empId;
                        createTicket.name = createTicket.EmployeeCode + " - " + createTicket.EmployeeName;
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
                                BindDropDown();
                                ViewBag.Error = stringBuilder.ToString();
                                return View(createTicket);
                            }
                            #endregion

                            var docname = frm.Get("DocName").ToString();
                            var documentName = docname.Split(',');
                            createTicket.TicketDocument = new TicketAttachment();
                            for (int i = 0; i < files.Count; i++)
                            {
                                HttpPostedFileBase file = files[i];
                                fname = ExtensionMethods.SetUniqueFileName("Document-",
                   Path.GetExtension(file.FileName));
                                fileName = fname;
                                createTicket.TicketDocument.docname = documentName[i];
                                createTicket.TicketDocument.docorgname = file.FileName;
                                createTicket.TicketDocument.docpathname = fileName;
                                createTicket.TicketDocument.CreatedBy = 1;
                                createTicket.TicketDocument.CreatedOn = DateTime.Now;
                                fname = Path.Combine(Server.MapPath("~/Document/HelpDesk/"), fname);
                                file.SaveAs(fname);
                            }
                        }
                        int res = ticketServ.InsertTicket(createTicket);
                        ViewBag.Message = $"Ticket created successfully.Your Ticket No. is {res}, please note for future reference.";
                        BindDropDown();
                        return View(createTicket);
                    }
                    else
                    {
                        BindDropDown();
                        ViewBag.Error = "Your details not match in database, please enter correct details and try again.";
                        return View(createTicket);
                    }
                }
                else
                {
                    BindDropDown();
                    ViewBag.Error = "Your details not match in database, please enter correct details and try again.";
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

        private void BindDropDown()
        {
            try
            {
                ViewBag.LastDepartment = ddlService.ddlDepartmentList();
                ViewBag.Department = ddlService.ddlDepartmentHavingTicket();
                var ddlTicketType = ddlService.GetTicketType();
                ViewBag.TypeList = new SelectList(ddlTicketType, "id", "value");
                ViewBag.Branch = ddlService.ddlBranchList();
            }
            catch (Exception)
            {
            }
        }

        [NonAction]
        public string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }
    }
}