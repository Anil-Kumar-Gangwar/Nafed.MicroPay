using AutoMapper;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Services
{
    public class ConveyanceBillService : BaseService, IConveyanceBillService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IConveyanceRepository conveyanceRepo;

        public ConveyanceBillService(IGenericRepository genericRepo, IConveyanceRepository conveyanceRepo)
        {
            this.genericRepo = genericRepo;
            this.conveyanceRepo = conveyanceRepo;
        }

        public List<ConveyanceBillFormHdr> GetEmployeeSelfConveyanceList(int userEmpID, string empCode, string empName)
        {
            log.Info($"ConveyanceBillService/GetEmployeeSelfConveyanceList/{userEmpID}");
            try
            {
                List<ConveyanceBillFormHdr> empConveyanceList = new List<ConveyanceBillFormHdr>();
                var dtoConveyanceForms = conveyanceRepo.EmployeeSelfConveyanceList(userEmpID, empCode, empName).ToList();
                if (dtoConveyanceForms != null && dtoConveyanceForms.Count > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.ConveyanceBillHdr, Model.ConveyanceBillFormHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeId))
                        .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                        .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                        .ForMember(d => d.EmpProceeApproval, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.ProcessID == (int)Common.WorkFlowProcess.ConveyanceBill && y.ToDate == null && y.EmployeeID == s.EmployeeId)))
                        .ForMember(d => d.FormName, o => o.UseValue("Conveyance Bill"))
                        .ForMember(d => d.Year, o => o.MapFrom(s => s.Year))
                        .ForMember(d => d.ConveyanceBillDetailID, o => o.MapFrom(s => s.ConveyanceBillDetailID))
                        .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                        .ForMember(d => d.IsReportingRejected, o => o.MapFrom(s => s.ConveyanceBillHdrDetail.IsReportingRejected))
                        .ForMember(d => d.IsReviewingRejected, o => o.MapFrom(s => s.ConveyanceBillHdrDetail.IsReviewingRejected))
                        .ForMember(d => d.ReportingRemarks, o => o.MapFrom(s => s.ConveyanceBillHdrDetail.ReportingRemarks))
                        .ForMember(d => d.ReviewingRemarks, o => o.MapFrom(s => s.ConveyanceBillHdrDetail.ReviewingRemarks))
                        ;
                        cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();
                    });
                    return Mapper.Map<List<Model.ConveyanceBillFormHdr>>(dtoConveyanceForms);
                }
                return empConveyanceList;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ConveyanceRulesAttributes GetConveyanceFormRulesAttributes(int employeeID, int conveyanceDetailID)
        {
            log.Info($"ConveyanceBillService/GetConveyanceFormRulesAttributes{employeeID},{conveyanceDetailID}");
            try
            {
                ConveyanceRulesAttributes frmAttributes = new ConveyanceRulesAttributes();
                if (genericRepo.Exists<DTOModel.ConveyanceBillHdrDetail>(x => x.EmployeeID == employeeID && !x.IsDeleted))
                    frmAttributes = genericRepo.GetIQueryable<DTOModel.ConveyanceBillHdrDetail>(x => x.EmployeeID == employeeID && !x.IsDeleted && x.ConveyanceBillDetailID == conveyanceDetailID).Select(x => new ConveyanceRulesAttributes
                    {
                        ConveyanceBillDetailID = x.ConveyanceBillDetailID,
                        FormState = x.FormState
                    }).FirstOrDefault();
                return frmAttributes;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ConveyanceBillDetails GetConveyanceBillDetail(int employeeID, int conveyanceDetailID)
        {
            log.Info($"ConveyanceBillService/GetConveyanceBillDetail/{employeeID}/{conveyanceDetailID}");
            try
            {
                ConveyanceBillDetails conveyanceBillDetails = new ConveyanceBillDetails();
                var formConveyanceBillDTO = conveyanceRepo.GetConveyanceBillDetail(employeeID, conveyanceDetailID);
                Mapper.Initialize(
                  cfg =>
                  {
                      cfg.CreateMap<DTOModel.GetConveyanceFormDetails_Result, ConveyanceBillDetails>()
                      .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                      .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                      .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name))
                      .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.DesignationName))
                      .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.DepartmentName))
                      .ForMember(d => d.Branch, o => o.MapFrom(s => s.Branch))
                      .ForMember(d => d.ReportingTo, o => o.MapFrom(s => s.ReportingTo))
                      .ForMember(d => d.ReportingDesignation, o => o.MapFrom(s => s.ReportingDesignation))
                      .ForMember(d => d.ReviewerTo, o => o.MapFrom(s => s.ReviewerTo))
                      .ForMember(d => d.ReviewerDesignation, o => o.MapFrom(s => s.ReviewerDesignation))
                      .ForMember(d => d.AcceptanceAuth, o => o.MapFrom(s => s.AcceptanceAuth))
                      .ForMember(d => d.AcceptanceDesignation, o => o.MapFrom(s => s.AcceptanceDesignation))
                      .ForMember(d => d.ReportingYear, o => o.MapFrom(s => s.ReportingYear))
                      .ForMember(d => d.ConveyanceBillDetailID, o => o.MapFrom(s => s.ConveyanceBillDetailID))
                      .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                      .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                      .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                      .ForMember(d => d.DepartmentID, o => o.MapFrom(s => s.DepartmentID))
                      .ForMember(d => d.Dated, o => o.MapFrom(s => s.Dated))
                      .ForMember(d => d.Section, o => o.MapFrom(s => s.Section))
                      .ForMember(d => d.TotalAmountClaimed, o => o.MapFrom(s => s.TotalAmountClaimed))
                      .ForMember(d => d.VehicleProv, o => o.MapFrom(s => s.VehicleProvided.Value))
                      .ForMember(d => d.IsReportingRejected, o => o.MapFrom(s => s.IsReportingRejected))
                      .ForMember(d => d.IsReviewingRejected, o => o.MapFrom(s => s.IsReviewingRejected))
                      .ForMember(d => d.ReportingRemarks, o => o.MapFrom(s => s.ReportingRemarks))
                      .ForMember(d => d.ReviewingRemarks, o => o.MapFrom(s => s.ReviewingRemarks))
                      .ForMember(d => d.FormState, o => o.MapFrom(s => s.FormState))
                      .ForAllOtherMembers(d => d.Ignore());
                  }
                  );
                conveyanceBillDetails = Mapper.Map<List<ConveyanceBillDetails>>(formConveyanceBillDTO).FirstOrDefault();
                return conveyanceBillDetails;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.ConveyanceBillDescription> GetConveyanceBillDescription(int employeeId, int conveyanceBillDetailID)
        {
            log.Info($"ConveyanceBillService/GetConveyanceBillDescription/{employeeId}/{conveyanceBillDetailID}");
            try
            {
                IEnumerable<ConveyanceBillDescription> conveyanceBillDescription = Enumerable.Empty<ConveyanceBillDescription>();
                var formConveyanceBillDTO = conveyanceRepo.GetConveyanceBillDescription(employeeId, conveyanceBillDetailID);
                Mapper.Initialize(
                  cfg =>
                  {
                      cfg.CreateMap<DTOModel.GetConveyanceBillDescription_Result, ConveyanceBillDescription>();
                  }
                  );
                conveyanceBillDescription = Mapper.Map<List<ConveyanceBillDescription>>(formConveyanceBillDTO);
                return conveyanceBillDescription.ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool ConveyanceDataExists(int empID, int conveyanceBillDetailID)
        {
            log.Info($"ConveyanceBillService/ConveyanceDataExists");
            try
            {
                return genericRepo.Exists<DTOModel.ConveyanceBillHdr>(x => x.EmployeeId == empID && x.ConveyanceBillDetailID == conveyanceBillDetailID);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertConveyanceBillData(ConveyanceBillForm conveyanceForm, out int conveyanceDetailId)
        {
            log.Info($"ConveyanceBillService/InsertConveyanceBillData");
            bool flag = false;
            conveyanceDetailId = 0;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.ConveyanceBillDetails, DTOModel.ConveyanceBillHdrDetail>()
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.ReportingYear, o => o.MapFrom(s => s.ReportingYear))
                    .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                    .ForMember(d => d.DepartmentID, o => o.MapFrom(s => s.DepartmentID))
                    .ForMember(d => d.Dated, o => o.MapFrom(s => s.Dated))
                    .ForMember(d => d.TotalAmountClaimed, o => o.MapFrom(s => s.TotalAmountClaimed))
                    .ForMember(d => d.VehicleProvided, o => o.MapFrom(s => s.VehicleProv))
                    .ForMember(d => d.FormState, o => o.MapFrom(s => s.FormState))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.IsDeleted, o => o.UseValue(false))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var dtoConveyanceBillDetails = Mapper.Map<DTOModel.ConveyanceBillHdrDetail>(conveyanceForm.conveyanceBillDetails);
                genericRepo.Insert<DTOModel.ConveyanceBillHdrDetail>(dtoConveyanceBillDetails);
                int result = dtoConveyanceBillDetails.ConveyanceBillDetailID;
                conveyanceDetailId = result;
                if (result > 0)
                {
                    #region Add Conveyance Bill Hdr details ============
                    Model.ConveyanceBillFormHdr AddConveyanceBillFormHdr = new Model.ConveyanceBillFormHdr();

                    AddConveyanceBillFormHdr = new ConveyanceBillFormHdr
                    {
                        CreatedBy = conveyanceForm.conveyanceBillDetails.CreatedBy,
                        CreatedOn = conveyanceForm.conveyanceBillDetails.CreatedOn,
                        EmployeeID = conveyanceForm.conveyanceBillDetails.EmployeeID,
                        Year = conveyanceForm.conveyanceBillDetails.ReportingYear,
                        ConveyanceBillDetailID = result,
                        StatusID = conveyanceForm.conveyanceBillDetails.FormState
                    };
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<ConveyanceBillFormHdr, Data.Models.ConveyanceBillHdr>()
                        .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.Year, o => o.MapFrom(s => s.Year))
                        .ForMember(d => d.ConveyanceBillDetailID, o => o.MapFrom(s => s.ConveyanceBillDetailID))
                        .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                    var dtoConveyanceBillHdr = Mapper.Map<Data.Models.ConveyanceBillHdr>(AddConveyanceBillFormHdr);
                    genericRepo.Insert<Data.Models.ConveyanceBillHdr>(dtoConveyanceBillHdr);

                    #endregion

                    #region Insert Conveyance Bill Form Discription
                    var conveyanceBillFormDiscription = conveyanceForm.conveyanceBillDescriptionList;
                    if (conveyanceBillFormDiscription != null && conveyanceBillFormDiscription.Count > 0)
                    {
                        var sno = 1;
                        conveyanceBillFormDiscription.ForEach(x =>
                        {
                            x.sno = sno++;
                            x.VehicleID = (int)x.conveyanceBillDesc;
                        });
                    }
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.ConveyanceBillDescription, DTOModel.ConveyanceBillDescription>()
                        .ForMember(d => d.CreatedOn, o => o.UseValue(dtoConveyanceBillDetails.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoConveyanceBillDetails.CreatedBy))
                        .ForMember(d => d.VehicleID, o => o.MapFrom(s => s.VehicleID))
                        .ForMember(d => d.Dated, o => o.MapFrom(s => s.Dated))
                        .ForMember(d => d.ConveyanceBillDetailID, o => o.UseValue(result))
                        .ForMember(d => d.From, o => o.MapFrom(s => s.From))
                        .ForMember(d => d.To, o => o.MapFrom(s => s.To))
                        .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                    var dtoBillDescription = Mapper.Map<List<DTOModel.ConveyanceBillDescription>>(conveyanceBillFormDiscription);
                    if (dtoBillDescription != null)
                        genericRepo.AddMultipleEntity<DTOModel.ConveyanceBillDescription>(dtoBillDescription);
                    #endregion

                    if (conveyanceForm.conveyanceBillDetails.FormState == (int)ConveyanceFormState.SavedByEmployee)
                    {
                        conveyanceForm._ProcessWorkFlow.ReferenceID = result;
                        AddProcessWorkFlow(conveyanceForm._ProcessWorkFlow);
                    }
                }
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdateConveyanceBillData(ConveyanceBillForm conveyanceForm)
        {
            log.Info($"ConveyanceBillService/UpdateConveyanceBillData");
            bool flag = false;
            try
            {
                if (conveyanceForm.conveyanceBillDetails.vehicleProvided == VehicleProvided.Provided)
                    conveyanceForm.conveyanceBillDetails.VehicleProv = true;
                else if (conveyanceForm.conveyanceBillDetails.vehicleProvided == VehicleProvided.NotProvided)
                    conveyanceForm.conveyanceBillDetails.VehicleProv = false;

                if (conveyanceForm.submittedBy == Model.ConveyanceSubmittedBy.Employee)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.ConveyanceBillDetails, DTOModel.ConveyanceBillHdrDetail>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.ConveyanceBillDetailID, o => o.MapFrom(s => s.ConveyanceBillDetailID))
                        .ForMember(d => d.Dated, o => o.MapFrom(s => s.Dated))
                        .ForMember(d => d.TotalAmountClaimed, o => o.MapFrom(s => s.TotalAmountClaimed))
                        .ForMember(d => d.VehicleProvided, o => o.MapFrom(s => s.VehicleProv))
                        .ForMember(d => d.IsReportingRejected, o => o.UseValue(false))
                        .ForMember(d => d.IsReviewingRejected, o => o.UseValue(false))
                        .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                        .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn));
                    });
                    var dtoConveyanceForm = Mapper.Map<DTOModel.ConveyanceBillHdrDetail>(conveyanceForm.conveyanceBillDetails);
                    genericRepo.Update<DTOModel.ConveyanceBillHdrDetail>(dtoConveyanceForm);
                    int result = dtoConveyanceForm.ConveyanceBillDetailID;
                    if (result > 0)
                    {
                        conveyanceForm.ConveyanceDetailID = conveyanceForm.conveyanceBillDetails.ConveyanceBillDetailID;
                        conveyanceForm.ReportingYr = conveyanceForm.conveyanceBillDetails.ReportingYear;
                        conveyanceForm.FormState = conveyanceForm.conveyanceBillDetails.FormState;
                        UpdateConveyanceFormHdr(conveyanceForm);

                        #region Insert Conveyance Bill Form Discription
                        var getConveyanceDiscription = genericRepo.Get<DTOModel.ConveyanceBillDescription>(x => x.ConveyanceBillDetailID == result).ToList();
                        var formConveyanceDiscription = conveyanceForm.conveyanceBillDescriptionList;
                        if (formConveyanceDiscription.Count > 0)
                        {
                            var sno = 1;
                            formConveyanceDiscription.ForEach(x =>
                            {
                                x.sno = sno++;
                                x.VehicleID = (int)x.conveyanceBillDesc;
                            });
                        }
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.ConveyanceBillDescription, DTOModel.ConveyanceBillDescription>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoConveyanceForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoConveyanceForm.CreatedBy))
                            .ForMember(d => d.VehicleID, o => o.MapFrom(s => s.VehicleID))
                            .ForMember(d => d.Dated, o => o.MapFrom(s => s.Dated))
                            .ForMember(d => d.ConveyanceBillDetailID, o => o.UseValue(result))
                            .ForMember(d => d.From, o => o.MapFrom(s => s.From))
                            .ForMember(d => d.To, o => o.MapFrom(s => s.To))
                            .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                            .ForAllOtherMembers(d => d.Ignore());
                        });
                        var dtoConveyanceDescription = Mapper.Map<List<DTOModel.ConveyanceBillDescription>>(formConveyanceDiscription);

                        if (getConveyanceDiscription != null && getConveyanceDiscription.Count > 0)
                            genericRepo.DeleteAll<DTOModel.ConveyanceBillDescription>(getConveyanceDiscription);
                        if (dtoConveyanceDescription != null && dtoConveyanceDescription.Count() > 0)
                            genericRepo.AddMultipleEntity<DTOModel.ConveyanceBillDescription>(dtoConveyanceDescription);

                        #endregion
                        conveyanceForm._ProcessWorkFlow.ReferenceID = result;
                        AddProcessWorkFlow(conveyanceForm._ProcessWorkFlow);
                    }
                }
                else
                {
                    int status = 0;
                    if (conveyanceForm.conveyanceBillDetails.FormState != (int)ConveyanceFormState.RejectedByReporting && conveyanceForm.conveyanceBillDetails.FormState != (int)ConveyanceFormState.RejectedByReviewer)
                        status = conveyanceForm.conveyanceBillDetails.FormState;
                    else if (conveyanceForm.conveyanceBillDetails.FormState == (int)ConveyanceFormState.RejectedByReporting || conveyanceForm.conveyanceBillDetails.FormState == (int)ConveyanceFormState.RejectedByReviewer)
                    {
                        status = (int)Model.ConveyanceFormState.SavedByEmployee;
                        //conveyanceForm.conveyanceBillDetails.IsReportingRejected = false;
                        //conveyanceForm.conveyanceBillDetails.IsReviewingRejected = false;
                    }

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.ConveyanceBillDetails, DTOModel.ConveyanceBillHdrDetail>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.VehicleProvided, o => o.MapFrom(s => s.VehicleProv))
                        .ForMember(d => d.FormState, o => o.UseValue(status))
                        .ForMember(d => d.IsReportingRejected, o => o.MapFrom(s => s.IsReportingRejected))
                        .ForMember(d => d.IsReviewingRejected, o => o.MapFrom(s => s.IsReviewingRejected))
                        .ForMember(d => d.ReportingRemarks, o => o.MapFrom(s => s.ReportingRemarks))
                        .ForMember(d => d.ReviewingRemarks, o => o.MapFrom(s => s.ReviewingRemarks))
                        ;
                    });
                    var dtoConveyanceForm = Mapper.Map<DTOModel.ConveyanceBillHdrDetail>(conveyanceForm.conveyanceBillDetails);
                    genericRepo.Update<DTOModel.ConveyanceBillHdrDetail>(dtoConveyanceForm);
                    int result = dtoConveyanceForm.ConveyanceBillDetailID;
                    if (result > 0)
                    {
                        conveyanceForm.ConveyanceDetailID = conveyanceForm.conveyanceBillDetails.ConveyanceBillDetailID;
                        conveyanceForm.ReportingYr = conveyanceForm.conveyanceBillDetails.ReportingYear;
                        conveyanceForm.FormState = status;

                        UpdateConveyanceFormHdr(conveyanceForm);

                        #region Update Conveyance Bill Form Discription
                        var getConveyanceDiscription = genericRepo.Get<DTOModel.ConveyanceBillDescription>(x => x.ConveyanceBillDetailID == result).ToList();
                        var formConveyanceDiscription = conveyanceForm.conveyanceBillDescriptionList;
                        if (formConveyanceDiscription.Count > 0)
                        {
                            var sno = 1;
                            formConveyanceDiscription.ForEach(x =>
                            {
                                x.sno = sno++;
                                x.VehicleID = (int)x.conveyanceBillDesc;
                            });
                        }

                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.ConveyanceBillDescription, DTOModel.ConveyanceBillDescription>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoConveyanceForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoConveyanceForm.CreatedBy))
                            .ForMember(d => d.ConveyanceBillDetailID, o => o.UseValue(result))
                            .ForMember(d => d.VehicleID, o => o.MapFrom(s => s.VehicleID))
                            .ForMember(d => d.Dated, o => o.MapFrom(s => s.Dated))
                            .ForMember(d => d.ConveyanceBillDetailID, o => o.UseValue(result))
                            .ForMember(d => d.From, o => o.MapFrom(s => s.From))
                            .ForMember(d => d.To, o => o.MapFrom(s => s.To))
                            .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                            .ForAllOtherMembers(d => d.Ignore());
                        });
                        var dtoConveyanceDescription = Mapper.Map<List<DTOModel.ConveyanceBillDescription>>(formConveyanceDiscription);

                        if (getConveyanceDiscription != null && getConveyanceDiscription.Count() > 0)
                            genericRepo.DeleteAll<DTOModel.ConveyanceBillDescription>(getConveyanceDiscription);
                        if (dtoConveyanceDescription != null && dtoConveyanceDescription.Count() > 0)
                            genericRepo.AddMultipleEntity<DTOModel.ConveyanceBillDescription>(dtoConveyanceDescription);
                        #endregion

                        if (conveyanceForm.conveyanceBillDetails.FormState == (int)ConveyanceFormState.SubmitedByReporting ||
                           conveyanceForm.conveyanceBillDetails.FormState == (int)ConveyanceFormState.SubmitedByReviewer ||
                           conveyanceForm.conveyanceBillDetails.FormState == (int)ConveyanceFormState.SubmitedByAcceptanceAuth ||
                           conveyanceForm.conveyanceBillDetails.FormState == (int)ConveyanceFormState.RejectedByReporting ||
                           conveyanceForm.conveyanceBillDetails.FormState == (int)ConveyanceFormState.RejectedByReviewer)
                        {
                            conveyanceForm._ProcessWorkFlow.ReferenceID = result;
                            AddProcessWorkFlow(conveyanceForm._ProcessWorkFlow);
                        }
                    }
                }
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }


        public bool UpdateConveyanceFormHdr(Model.ConveyanceBillForm conveyance)
        {
            log.Info($"ConveyanceBillService/UpdateConveyanceFormHdr");
            try
            {
                var objHdr = genericRepo.Get<DTOModel.ConveyanceBillHdr>(x => x.EmployeeId == conveyance.EmployeeID && x.ConveyanceBillDetailID == conveyance.ConveyanceDetailID).FirstOrDefault();
                if (objHdr != null)
                {
                    objHdr.StatusID = conveyance.FormState;
                    genericRepo.Update<DTOModel.ConveyanceBillHdr>(objHdr);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public IEnumerable<ConveyanceBillFormHdr> GetConveyanceFormHdr(ConveyanceBillApprovalFilter filters)
        {
            log.Info($"ConveyanceBillService/GetConveyanceFormHdr/");
            try
            {
                IEnumerable<ConveyanceBillFormHdr> formHdrs = Enumerable.Empty<ConveyanceBillFormHdr>();
                List<DTOModel.ConveyanceBillHdr> dtoFormHdrs = new List<DTOModel.ConveyanceBillHdr>();

                var appformsHdr = genericRepo.GetIQueryable<DTOModel.ConveyanceBillHdr>(x => x.tblMstEmployee.EmployeeProcessApprovals
                 .Any(y => y.ProcessID == (int)Common.WorkFlowProcess.ConveyanceBill
                 && y.ToDate == null
                 && (y.ReportingTo == filters.loggedInEmployeeID || y.ReviewingTo == filters.loggedInEmployeeID) && x.StatusID != 1)
                 );

                if (filters != null)
                {
                    dtoFormHdrs = appformsHdr.Where(x =>
                    (filters.selectedEmployeeID == 0 ? (1 > 0) : x.EmployeeId == filters.selectedEmployeeID)).ToList();
                    if (filters.FromDate.HasValue && filters.ToDate.HasValue)
                        dtoFormHdrs = dtoFormHdrs.Where(x => (x.ConveyanceBillHdrDetail.Dated.Value.Date >= filters.FromDate.Value.Date && x.ConveyanceBillHdrDetail.Dated.Value.Date <= filters.ToDate.Value.Date)).ToList();
                    if (filters.StatusId.HasValue)
                        dtoFormHdrs = dtoFormHdrs.Where(x => x.StatusID == filters.StatusId.Value).ToList();
                }

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.ConveyanceBillHdr, Model.ConveyanceBillFormHdr>()
                    .ForMember(d => d.ConveyanceHdrID, o => o.MapFrom(s => s.ConveyanceHdrID))
                    .ForMember(d => d.Year, o => o.MapFrom(s => s.Year))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeId))
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(d => d.EmpName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(d => d.ConveyanceBillDetailID, o => o.MapFrom(s => s.ConveyanceBillDetailID))
                    .ForMember(d => d.FormName, o => o.UseValue("Conveyance Bill"))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                    .ForMember(d => d.EmpProceeApproval, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.ProcessID == (int)Common.WorkFlowProcess.ConveyanceBill && y.ToDate == null && y.EmployeeID == s.EmployeeId)))
                    .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                    .ForMember(d => d.DepartmentID, o => o.MapFrom(s => s.tblMstEmployee.DepartmentID))
                    .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.tblMstEmployee.DesignationID))
                    .ForMember(d => d.TotalAmount, o => o.MapFrom(s => s.ConveyanceBillHdrDetail.TotalAmountClaimed.HasValue ? (s.ConveyanceBillHdrDetail.TotalAmountClaimed.Value.ToString("0.##")) : "0"))
                    .ForAllOtherMembers(d => d.Ignore());
                    cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();
                });
                return Mapper.Map<List<Model.ConveyanceBillFormHdr>>(dtoFormHdrs.ToList());
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> GetEmployeeFilter(int employeeID)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.EmployeeProcessApproval>(x =>
                (x.ProcessID == (int)Common.WorkFlowProcess.ConveyanceBill) && x.ToDate == null
                && (x.ReportingTo == employeeID || x.ReviewingTo == employeeID));

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeProcessApproval, Model.SelectListModel>()
                   .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeID))
                   .ForMember(d => d.value, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode + "-" + s.tblMstEmployee.Name));
                });
                var employeedetail = Mapper.Map<List<Model.SelectListModel>>(result);
                return employeedetail.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<ConveyanceBillHistoryModel> GetConveyanceBillDetails(AppraisalFormApprovalFilter filters, string procName)
        {
            log.Info($"ConveyanceBillService/GetConveyanceBillDetails/");
            try
            {
                List<ConveyanceBillHistoryModel> historyList = new List<ConveyanceBillHistoryModel>();
                var conveyanceBillDetails = conveyanceRepo.GetConveyanceBillDetails(filters.selectedReportingYear, filters.selectedEmployeeID, filters.statusId, procName);
                historyList = (from DataRow dr in conveyanceBillDetails.Rows
                               select new ConveyanceBillHistoryModel()
                               {
                                   EmployeeId = Convert.ToInt32(dr["EmployeeId"]),
                                   EmployeeCode = dr["EmployeeCode"].ToString(),
                                   ReportingYr = dr["ReportingYr"].ToString(),
                                   FormName = dr["FormName"].ToString(),
                                   Name = dr["Name"].ToString(),
                                   DepartmentName = dr["DepartmentName"].ToString(),
                                   DepartmentID = Convert.ToInt32(dr["DepartmentID"]),
                                   DesignationID = Convert.ToInt32(dr["DesignationID"]),
                                   DesignationName = dr["DesignationName"].ToString(),
                                   ConveyanceStatus = Convert.ToInt32(dr["ConveyanceStatus"]),
                                   ReportingTo = Convert.ToInt32(dr["ReportingTo"]),
                                   ReviewingTo = DBNull.Value.Equals(dr["ReviewingTo"]) ? (int?)null : Convert.ToInt32(dr["ReviewingTo"]),
                                   AcceptanceAuthority = DBNull.Value.Equals(dr["AcceptanceAuthority"]) ? (int?)null : Convert.ToInt32(dr["AcceptanceAuthority"]),
                                   ConveyanceBillDetailID = Convert.ToInt32(dr["ConveyanceBillDetailID"])
                               }).ToList();
                //var result = Common.ExtensionMethods.ConvertToList<ConveyanceBillHistoryModel>(conveyanceBillDetails);
                return historyList;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<ConveyanceBillHistoryModel> GetConveyanceEmployeeHistory(string fromDate, string toDate, int? employeeId)
        {
            log.Info($"ConveyanceBillService/GetConveyanceEmployeeHistory");
            try
            {
                List<ConveyanceBillHistoryModel> historyList = new List<ConveyanceBillHistoryModel>();
                var conveyanceEmployeeHistory = conveyanceRepo.GetConveyanceEmployeeHistory(fromDate, toDate, employeeId);
                historyList = (from DataRow dr in conveyanceEmployeeHistory.Rows
                               select new ConveyanceBillHistoryModel()
                               {
                                   EmployeeId = Convert.ToInt32(dr["EmployeeId"]),
                                   EmployeeCode = dr["EmployeeCode"].ToString(),
                                   ReportingYr = dr["ReportingYr"].ToString(),
                                   FormName = dr["FormName"].ToString(),
                                   Name = dr["Name"].ToString(),
                                   DepartmentName = dr["DepartmentName"].ToString(),
                                   DepartmentID = Convert.ToInt32(dr["DepartmentID"]),
                                   DesignationID = Convert.ToInt32(dr["DesignationID"]),
                                   DesignationName = dr["DesignationName"].ToString(),
                                   ConveyanceStatus = Convert.ToInt32(dr["ConveyanceStatus"]),
                                   ReportingTo = Convert.ToInt32(dr["ReportingTo"]),
                                   ReviewingTo = DBNull.Value.Equals(dr["ReviewingTo"]) ? (int?)null : Convert.ToInt32(dr["ReviewingTo"]),
                                   AcceptanceAuthority = DBNull.Value.Equals(dr["AcceptanceAuthority"]) ? (int?)null : Convert.ToInt32(dr["AcceptanceAuthority"]),
                                   ConveyanceBillDetailID = Convert.ToInt32(dr["ConveyanceBillDetailID"]),
                                   TotalAmountClaimed = Convert.ToInt32(dr["TotalAmountClaimed"]),
                                   RequestedDate = dr["RequestedDate"].ToString()
                               }).ToList();
                return historyList;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

    }
}
