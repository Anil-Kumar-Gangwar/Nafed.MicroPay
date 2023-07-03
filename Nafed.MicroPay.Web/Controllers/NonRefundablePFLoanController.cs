using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using System.Data;
using System.IO;
using static Nafed.MicroPay.Common.FileHelper;
using Common = Nafed.MicroPay.Common;


namespace MicroPay.Web.Controllers
{
    public class NonRefundablePFLoanController : BaseController
    {
        private readonly INRPFLoanService NRPFLoanService;

        public NonRefundablePFLoanController(INRPFLoanService NRPFLoanService)
        {
            this.NRPFLoanService = NRPFLoanService;
        }
        public ActionResult Index()
        {
            NonRefunPFLoanDataViewModel NRPFLoanVM = new NonRefunPFLoanDataViewModel();
            NRPFLoanVM.userRights = userAccessRight;
            return View(NRPFLoanVM);
        }

        public ActionResult NonRefundablePFLoanGridView(NonRefunPFLoanDataViewModel NonRefunPFLoanVM, string ButtonType)
        {
            log.Info($"NonRefundablePFLoanController/NonRefundablePFLoanGridView");
            try
            {
                NonRefunPFLoanDataViewModel NRPFLoanVM = new NonRefunPFLoanDataViewModel();
                int? empID = null;
                if (userDetail.UserName.ToLower() == "admin")
                    empID = null;
                else empID = (int)userDetail.EmployeeID;
                if (ButtonType == null)
                {
                    var PR = NRPFLoanService.GetnonrefPFloanList((int)userDetail.EmployeeID);
                    NRPFLoanVM.NRPFLoanList = PR;
                }
                else if (ButtonType.ToLower() == "request for non refundable pf loan")
                {
                   
                    Model.NonRefundablePFLoan NRHDR = new Model.NonRefundablePFLoan();
                    NRHDR.CreatedBy = userDetail.UserID;
                    NRHDR.CreatedOn = DateTime.Now;
                    bool ID = NRPFLoanService.InsertNRPFLoanDetailsHDR((int)userDetail.EmployeeID, NRHDR);
                    var PR = NRPFLoanService.GetnonrefPFloanList((int)userDetail.EmployeeID);
                    NRPFLoanVM.NRPFLoanList = PR;
                }
                NRPFLoanVM.userRights = userAccessRight;
                var approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, Common.WorkFlowProcess.NonRefundablePFLoan);
                NRPFLoanVM.approvalSetting = approvalSettings ?? new Nafed.MicroPay.Model.EmployeeProcessApproval();
                return PartialView("_NRPFLoanGridView", NRPFLoanVM);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {
            //List<Model.SelectListModel> listofDocuments = new List<Model.SelectListModel>();
            //listofDocuments.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            //listofDocuments.Add(new Model.SelectListModel() { value = "Title deed of proposed seller.", id = 1 });
            //listofDocuments.Add(new Model.SelectListModel() { value = "Non-encumbrance certificate in respect of the dwelling site/house to be purchased.", id = 2 });
            //listofDocuments.Add(new Model.SelectListModel() { value = "Agreement with the vendor for the purchase of site/house.", id = 3 });
            //listofDocuments.Add(new Model.SelectListModel() { value = "An estimate of the cost of construction in the case of the advance for the construction of the house(Ref 7 © and 7 (d)", id = 4 });
            //listofDocuments.Add(new Model.SelectListModel() { value = "Original title deed for certificate from appropriate revenue authority regarding ownership and non-encumbrance of the land.", id =5});
            //listofDocuments.Add(new Model.SelectListModel() { value = "sanctioned construction plan.", id = 6 });
            //ViewBag.listofDocuments = new SelectList(listofDocuments, "id", "value");

            //List<Model.SelectListModel> select = new List<Model.SelectListModel>();
            //select.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            //select.Add(new Model.SelectListModel() { value = "Purchasing a dwelling house", id = 1 });
            //select.Add(new Model.SelectListModel() { value = "Purchasing a dwelling site", id = 2 });
            //select.Add(new Model.SelectListModel() { value = "Construction of a dwelling house", id = 3 });
            //select.Add(new Model.SelectListModel() { value = "Addition/Substantial alteration or sub-stantial improvement necessary to the dwelling house owned by member", id = 4 });
            //select.Add(new Model.SelectListModel() { value = "Completing the construction of the dwelling house already commenced by member", id = 5 });
            //select.Add(new Model.SelectListModel() { value = "Acquiring a flat in building", id = 6 });
            //ViewBag.PurposeofAdvanced = new SelectList(select, "id", "value"); 





        }

        [HttpGet]
        public ActionResult Create(int NRPFLoanID, int statusID, int? reportingTo, int? reviewingTo, int? acceptanceAuthority, int? empID)
        {
            log.Info($"NonRefundablePFLoanController/Create/NRPFLoanID={NRPFLoanID}");
            try
            {
                BindDropdowns();
                bool flag = false;
                Model.NonRefundablePFLoan NRPFLoanVM = new Model.NonRefundablePFLoan();

                var getList = NRPFLoanService.checkexistdata(NRPFLoanID);
                DataTable dt = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(getList);
                var approvalSettings = (dynamic)null;
                if (dt.Rows.Count > 0)
                {
                    if (statusID == 1)
                    {
                        NRPFLoanVM.EmployeeId = (int)userDetail.EmployeeID;                       
                    }
                    else
                    {
                        NRPFLoanVM.EmployeeId = (empID == null ? 0 : (int)empID); ;
                    }
                    NRPFLoanVM = NRPFLoanService.GetNRPFLoanDetails(Convert.ToInt32(dt.Rows[0]["ID"]), NRPFLoanID, statusID, Convert.ToInt32(NRPFLoanVM.EmployeeId));
                    if (NRPFLoanVM.ListofDocuments != null && NRPFLoanVM.ListofDocuments != string.Empty)
                    {
                        NRPFLoanVM.ListID = Array.ConvertAll(NRPFLoanVM.ListofDocuments.Split(','), int.Parse);
                    }
                    NRPFLoanVM.EmpProceeApproval.ReportingTo = (reportingTo == null ? 0 : (int)reportingTo);
                    NRPFLoanVM.EmpProceeApproval.ReviewingTo = (reviewingTo == null ? 0 : (int)reviewingTo);
                    NRPFLoanVM.EmpProceeApproval.AcceptanceAuthority = (acceptanceAuthority == null ? 0 : (int)acceptanceAuthority);
                    NRPFLoanVM.loggedInEmpID = (int)userDetail.EmployeeID;
                    NRPFLoanVM.FormStatus = (short)statusID;
                   

                }
                else
                {
                    NRPFLoanVM = NRPFLoanService.GetNRPFLoanDetails(0, NRPFLoanID, statusID,(int)userDetail.EmployeeID);
                    NRPFLoanVM.FormStatus = 1;
                     approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, Common.WorkFlowProcess.NonRefundablePFLoan);
                    NRPFLoanVM.EmpProceeApproval = approvalSettings ?? new Nafed.MicroPay.Model.EmployeeProcessApproval();
                    NRPFLoanVM.loggedInEmpID = (int)userDetail.EmployeeID;
                }
                if (statusID == 1)
                {
                    approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, Common.WorkFlowProcess.NonRefundablePFLoan);
                    NRPFLoanVM.EmpProceeApproval = approvalSettings ?? new Nafed.MicroPay.Model.EmployeeProcessApproval();
                }
                if ((NRPFLoanVM.EmpProceeApproval.ReportingTo == NRPFLoanVM.EmpProceeApproval.ReviewingTo) && (NRPFLoanVM.EmpProceeApproval.ReviewingTo == NRPFLoanVM.EmpProceeApproval.AcceptanceAuthority))
                    NRPFLoanVM.ApprovalHierarchy = 3;
                else if (((NRPFLoanVM.EmpProceeApproval.ReportingTo != NRPFLoanVM.EmpProceeApproval.ReviewingTo) && (NRPFLoanVM.EmpProceeApproval.ReviewingTo == NRPFLoanVM.EmpProceeApproval.AcceptanceAuthority))
                    && NRPFLoanVM.loggedInEmpID == NRPFLoanVM.EmpProceeApproval.ReviewingTo)
                    NRPFLoanVM.ApprovalHierarchy = 2.1;
                else if (((NRPFLoanVM.EmpProceeApproval.ReportingTo == NRPFLoanVM.EmpProceeApproval.ReviewingTo) && (NRPFLoanVM.EmpProceeApproval.ReviewingTo != NRPFLoanVM.EmpProceeApproval.AcceptanceAuthority))
                    && (NRPFLoanVM.loggedInEmpID == NRPFLoanVM.EmpProceeApproval.ReportingTo || NRPFLoanVM.loggedInEmpID == NRPFLoanVM.EmpProceeApproval.AcceptanceAuthority))
                    NRPFLoanVM.ApprovalHierarchy = 2.0;
                else
                    NRPFLoanVM.ApprovalHierarchy = 1;


                List<Model.SelectListModel> remittancedesiredmode = new List<Model.SelectListModel>();

                remittancedesiredmode.Add(new Model.SelectListModel() { value = "NEFT/RTGS", id = 1 });
                remittancedesiredmode.Add(new Model.SelectListModel() { value = "By crossed cheque through post (please send advance receipt in the enclosed form)", id = 2 });
                NRPFLoanVM.RemittanceDesiredMode = remittancedesiredmode;


                List<Model.SelectListModel> select = new List<Model.SelectListModel>();
                select.Add(new Model.SelectListModel() { value = "Purchasing a dwelling house", id = 1 });
                select.Add(new Model.SelectListModel() { value = "Purchasing a dwelling site", id = 2 });
                select.Add(new Model.SelectListModel() { value = "Construction of a dwelling house", id = 3 });
                select.Add(new Model.SelectListModel() { value = "Addition/Substantial alteration or sub-stantial improvement necessary to the dwelling house owned by member", id = 4 });
                select.Add(new Model.SelectListModel() { value = "Completing the construction of the dwelling house already commenced by member", id = 5 });
                select.Add(new Model.SelectListModel() { value = "Acquiring a flat in building", id = 6 });
                NRPFLoanVM.POA = select;


                List<Model.SelectListModel> listofDocuments = new List<Model.SelectListModel>();
                listofDocuments.Add(new Model.SelectListModel() { value = "Select", id = 0 });
                listofDocuments.Add(new Model.SelectListModel() { value = "Title deed of proposed seller.", id = 1 });
                listofDocuments.Add(new Model.SelectListModel() { value = "Non-encumbrance certificate in respect of the dwelling site/house to be purchased.", id = 2 });
                listofDocuments.Add(new Model.SelectListModel() { value = "Agreement with the vendor for the purchase of site/house.", id = 3 });
                listofDocuments.Add(new Model.SelectListModel() { value = "An estimate of the cost of construction in the case of the advance for the construction of the house(Ref 7 © and 7 (d)", id = 4 });
                listofDocuments.Add(new Model.SelectListModel() { value = "Original title deed for certificate from appropriate revenue authority regarding ownership and non-encumbrance of the land.", id = 5 });
                listofDocuments.Add(new Model.SelectListModel() { value = "sanctioned construction plan.", id = 6 });
                NRPFLoanVM.listDetails = listofDocuments;


                return View(NRPFLoanVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.NonRefundablePFLoan createNRPF, string ButtonType)
        {
            log.Info("EmployeePFOrganisationHeaderController/Create");
            try
            {


                List<Model.SelectListModel> remittancedesiredmode = new List<Model.SelectListModel>();

                remittancedesiredmode.Add(new Model.SelectListModel() { value = "NEFT/RTGS", id = 1 });
                remittancedesiredmode.Add(new Model.SelectListModel() { value = "By crossed cheque through post (please send advance receipt in the enclosed form)", id = 2 });
                createNRPF.RemittanceDesiredMode = remittancedesiredmode;


                List<Model.SelectListModel> select = new List<Model.SelectListModel>();
                select.Add(new Model.SelectListModel() { value = "Purchasing a dwelling house", id = 1 });
                select.Add(new Model.SelectListModel() { value = "Purchasing a dwelling site", id = 2 });
                select.Add(new Model.SelectListModel() { value = "Construction of a dwelling house", id = 3 });
                select.Add(new Model.SelectListModel() { value = "Addition/Substantial alteration or sub-stantial improvement necessary to the dwelling house owned by member", id = 4 });
                select.Add(new Model.SelectListModel() { value = "Completing the construction of the dwelling house already commenced by member", id = 5 });
                select.Add(new Model.SelectListModel() { value = "Acquiring a flat in building", id = 6 });
                createNRPF.POA = select;


                List<Model.SelectListModel> listofDocuments = new List<Model.SelectListModel>();
                listofDocuments.Add(new Model.SelectListModel() { value = "Select", id = 0 });
                listofDocuments.Add(new Model.SelectListModel() { value = "Title deed of proposed seller.", id = 1 });
                listofDocuments.Add(new Model.SelectListModel() { value = "Non-encumbrance certificate in respect of the dwelling site/house to be purchased.", id = 2 });
                listofDocuments.Add(new Model.SelectListModel() { value = "Agreement with the vendor for the purchase of site/house.", id = 3 });
                listofDocuments.Add(new Model.SelectListModel() { value = "An estimate of the cost of construction in the case of the advance for the construction of the house(Ref 7 © and 7 (d)", id = 4 });
                listofDocuments.Add(new Model.SelectListModel() { value = "Original title deed for certificate from appropriate revenue authority regarding ownership and non-encumbrance of the land.", id = 5 });
                listofDocuments.Add(new Model.SelectListModel() { value = "sanctioned construction plan.", id = 6 });
                createNRPF.listDetails = listofDocuments;


                var getexgratiaList = NRPFLoanService.checkexistdata(createNRPF.NRPFLoanID);
                DataTable dt = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(getexgratiaList);

                if (createNRPF.Amount_of_Advanced == 0)
                {
                    ModelState.AddModelError("Amount_of_Advanced", "Please enter amount of advanced.");
                   
                }
                if (createNRPF.Purpose_of_Advanced == 0)
                {
                    ModelState.AddModelError("PorposeofAdvanced", "Please select purpose of advanced.");

                }
                if (createNRPF.NamePresentOwner == null)
                {
                    ModelState.AddModelError("NamePresentOwner", "Please enter name of presrent owner.");
                  
                }
                if (createNRPF.AddressPresentOwner == null)
                {
                    ModelState.AddModelError("AddressPresentOwner", "Please enter address of presrent owner.");
                   
                }

                if (createNRPF.ListID != null && createNRPF.ListID.Length > 0)
                {
                    for (int i = 0; i < createNRPF.ListID.Length; i++)
                    {
                        createNRPF.ListofDocuments += createNRPF.ListID[i] + ",";
                    }
                }
                if (createNRPF.ListofDocuments != null && createNRPF.ListofDocuments != string.Empty)
                {
                    createNRPF.ListofDocuments = createNRPF.ListofDocuments.Substring(0, createNRPF.ListofDocuments.Length - 1);
                }

                ModelState.Remove("RemittanceDesiredMode");
                ModelState.Remove("DesiredModeofRemittance");
                ModelState.Remove("POA");
                ModelState.Remove("listDetails");
                if (ModelState.IsValid)
                {
                    createNRPF.loggedInEmpID = userDetail.EmployeeID;
                    createNRPF.EmployeeId = userDetail.EmployeeID;
                    var formNewAttributes = GetFormAttributes(createNRPF, ButtonType);
                    createNRPF.FormStatus = Convert.ToInt16(formNewAttributes.FormState);

                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                    {
                        SenderID = createNRPF.loggedInEmpID,
                        ReceiverID = formNewAttributes.ReciverID,
                        SenderDepartmentID = userDetail.DepartmentID,
                        SenderDesignationID = userDetail.DesignationID,
                        CreatedBy = userDetail.UserID,
                        EmployeeID = (int)createNRPF.EmployeeId,
                        Scomments = $"Non refundable PF loan Updated By : {(Model.SubmittedBy)(int)formNewAttributes.SubmittedBy}",
                        ProcessID = (int)Common.WorkFlowProcess.NonRefundablePFLoan,
                        StatusID = (int)createNRPF.FormStatus
                    };

                    if (ButtonType == "Save")
                    {

                        if (dt.Rows.Count > 0)
                        {
                            createNRPF.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
                            createNRPF.EmployeeId = userDetail.EmployeeID;
                            createNRPF.UpdatedBy = userDetail.UserID;
                            createNRPF.UpdatedOn = DateTime.Now;

                            NRPFLoanService.UpdateNRPFLOanDetails(createNRPF);
                        }
                        else
                        {

                            createNRPF.CreatedBy = userDetail.UserID;
                            createNRPF.CreatedOn = DateTime.Now;
                            createNRPF.StatusID = 1;
                            NRPFLoanService.InsertNRPFLoanDetails(createNRPF, workFlow);
                            createNRPF.EmployeeId = userDetail.EmployeeID;
                            createNRPF.UpdatedBy = userDetail.UserID;
                            createNRPF.UpdatedOn = DateTime.Now;
                            NRPFLoanService.UpdateNRPFLoanStatus(createNRPF, workFlow);
                        }
                        TempData["Message"] = "Successfully Saved";
                        return RedirectToAction("Index");
                    }
                    else if (ButtonType == "Submit")
                    {
                        createNRPF.FormStatus = 2;
                        createNRPF.StatusID = 2;
                        createNRPF.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
                        createNRPF.EmployeeId = Convert.ToInt32(dt.Rows[0]["EmployeeId"]);
                        createNRPF.UpdatedBy = userDetail.UserID;
                        createNRPF.UpdatedOn = DateTime.Now;
                        if (dt.Rows.Count > 0)
                        {
                            createNRPF.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
                            createNRPF.EmployeeId = userDetail.EmployeeID;
                            createNRPF.UpdatedBy = userDetail.UserID;
                            createNRPF.UpdatedOn = DateTime.Now;

                            NRPFLoanService.UpdateNRPFLOanDetails(createNRPF);
                        }

                        bool result = NRPFLoanService.UpdateNRPFLoanStatus(createNRPF, workFlow);
                        TempData["Message"] = "Successfully Submitted";
                        return RedirectToAction("Index");
                    }
                    else if (ButtonType == "Approved")
                    {
                        createNRPF.FormStatus = Convert.ToInt16(formNewAttributes.FormState);
                        createNRPF.StatusID = Convert.ToInt16(formNewAttributes.FormState);
                        createNRPF.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
                        createNRPF.EmployeeId = Convert.ToInt32(dt.Rows[0]["EmployeeId"]);
                        createNRPF.UpdatedBy = userDetail.UserID;
                        createNRPF.UpdatedOn = DateTime.Now;

                        bool result = NRPFLoanService.UpdateNRPFLoanStatus(createNRPF, workFlow);
                        if (result == true)
                        {
                            return RedirectToAction("Index", "ApprovalRequest");

                        }
                    }

                    else if (ButtonType == "Rejected")
                    {

                        createNRPF.FormStatus = Convert.ToInt16(formNewAttributes.FormState);
                        createNRPF.StatusID = Convert.ToInt16(formNewAttributes.FormState);
                        createNRPF.EmployeeId = userDetail.EmployeeID;
                        createNRPF.UpdatedBy = userDetail.UserID;
                        createNRPF.UpdatedOn = DateTime.Now;
                        bool result = NRPFLoanService.UpdateNRPFLoanStatus(createNRPF, workFlow);
                        if (result == true)
                        {
                            return RedirectToAction("Index", "ApprovalRequest");
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(createNRPF);
        }

        private Model.FormRulesAttributes GetFormAttributes(Model.NonRefundablePFLoan createNRPFLOan, string buttonType)
        {
            log.Info($"LTCController/GetFormAttributes");
            try
            {
                Model.FormRulesAttributes formRules = new Model.FormRulesAttributes();

                if (createNRPFLOan.ApprovalHierarchy == 1)
                {
                    if (createNRPFLOan.loggedInEmpID == createNRPFLOan.EmpProceeApproval.ReportingTo)
                    {

                        if (buttonType == "Submit")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.NRPFLoanFormState.AcceptedByReporting : (int)Model.NRPFLoanFormState.AcceptedByReporting);
                        }
                        else if (buttonType == "Rejected")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.NRPFLoanFormState.RejectedByReporting : (int)Model.NRPFLoanFormState.RejectedByReporting);
                        }
                        else if (buttonType == "Approved")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.NRPFLoanFormState.AcceptedByReporting : (int)Model.NRPFLoanFormState.AcceptedByReporting);
                        }
                        formRules.SubmittedBy = Model.SubmittedBy.ReportingOfficer;
                        formRules.SenderID = createNRPFLOan.EmpProceeApproval.ReportingTo;
                        formRules.ReciverID = createNRPFLOan.EmpProceeApproval.ReviewingTo;
                    }
                    else
                    {
                        if (buttonType == "Save")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.NRPFLoanFormState.SavedByEmployee : (int)Model.NRPFLoanFormState.SavedByEmployee);
                        }
                        else if (buttonType == "Submit")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.NRPFLoanFormState.SubmitedByEmployee : (int)Model.NRPFLoanFormState.SubmitedByEmployee);
                        }
                           
                        formRules.SubmittedBy = Model.SubmittedBy.Employee;
                        formRules.SenderID = createNRPFLOan.EmpProceeApproval.EmployeeID;
                        formRules.ReciverID = createNRPFLOan.EmpProceeApproval.ReportingTo;
                    }

                }
                else
                    formRules.FormState = 1;
                return formRules;
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ex.Message} StackTrace-{ex.StackTrace} DatetimeStamp-{DateTime.Now}");
                throw ex;
            }
        }
                
    }
}