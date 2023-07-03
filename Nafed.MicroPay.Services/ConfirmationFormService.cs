using System;
using System.Collections.Generic;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
using System.Linq;
using static Nafed.MicroPay.Common.ExtensionMethods;
using System.Threading.Tasks;
using System.Text;

namespace Nafed.MicroPay.Services
{
    public class ConfirmationFormService : BaseService, IConfirmationFormService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IEmployeeRepository empRepo;
        private readonly IConfirmationFormRepository confirmRepo;

        public ConfirmationFormService(IGenericRepository genericRepo, IEmployeeRepository empRepo, IConfirmationFormRepository confirmRepo)
        {
            this.genericRepo = genericRepo;
            this.empRepo = empRepo;
            this.confirmRepo = confirmRepo;
        }

        public List<ConfirmationFormHdr> GetConfirmationForms(int? employeeID)
        {
            log.Info($"ConfirmationFormService/GetConfirmationForms/{employeeID}");

            try
            {
                var dtoForms = genericRepo.Get<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID && x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.EmployeeID == employeeID && (y.ProcessID == 6 || y.ProcessID == 7) && y.ToDate == null));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.ConfirmationFormHdr, Model.ConfirmationFormHdr>()
                   .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                   .ForMember(d => d.FormConfirmationType, o => o.MapFrom(s => s.Process.ProcessName))
                   .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessID))
                   .ForMember(d => d.FormTypeID, o => o.MapFrom(s => s.FormTypeID))
                   .ForMember(d => d.FormTypeName, o => o.MapFrom(s => s.ConfirmationForm.FormName))
                   .ForAllOtherMembers(d => d.Ignore());
                });
                return Mapper.Map<List<ConfirmationFormHdr>>(dtoForms);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ConfirmationFormDetail GetEmployeeConfirmationList(int formTypeID, int? processTypeID, int? employeeID, int empProcessAppID, int formABHdrID)
        {
            log.Info($"ConfirmationFormService/GetEmployeeConfirmationUserList/{formTypeID}");
            try
            {
                var getDesignationId = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == employeeID && !x.IsDeleted).FirstOrDefault().DesignationID;
                var payscale = empRepo.GetDesignationPayScaleList(getDesignationId).FirstOrDefault();
                if (formTypeID == 1)
                {
                    var dtoConfChild = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.EmployeeID == employeeID && x.EmpProcessAppID == empProcessAppID && x.FormAHeaderID == formABHdrID).FirstOrDefault();
                    var dtoConfirmationUserForms = genericRepo.Get<DTOModel.ConfirmationFormAHeader>(x => x.EmployeeId == employeeID && x.ProcessId == processTypeID && !x.IsDeleted && x.FormAHeaderId == formABHdrID).OrderByDescending(x => x.DueConfirmationDate).FirstOrDefault();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.ConfirmationFormAHeader, Model.ConfirmationFormDetail>()
                        .ForMember(d => d.FormAHeaderId, o => o.MapFrom(s => s.FormAHeaderId))
                       .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                       .ForMember(d => d.DesignationId, o => o.MapFrom(s => s.DesignationId))
                       .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                       .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName))
                       .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.Designation.DesignationName))
                       .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                       .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                       .ForMember(d => d.DOJ, o => o.MapFrom(s => (s.tblMstEmployee.Pr_Loc_DOJ == null ? "" : s.tblMstEmployee.Pr_Loc_DOJ.Value.ToString("dd/MM/yyyy"))))
                       .ForMember(d => d.orderOfPromotion, o => o.MapFrom(s => (s.tblMstEmployee.orderofpromotion == null ? "" : s.tblMstEmployee.orderofpromotion.Value.ToString("dd/MM/yyyy"))))
                       .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => (s.DueConfirmationDate == null ? "" : s.DueConfirmationDate.Value.ToString("dd/MM/yyyy"))))
                       .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
                       .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
                       .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
                       .ForMember(d => d.Point8_1, o => o.UseValue(dtoConfChild.Point8_1 ?? 0))
                       .ForMember(d => d.Point8_2, o => o.UseValue(dtoConfChild.Point8_2 ?? 0))
                       .ForMember(d => d.Point8_3, o => o.UseValue(dtoConfChild.Point8_3 ?? 0))
                       .ForMember(d => d.Point8_4, o => o.UseValue(dtoConfChild.Point8_4 ?? 0))
                       .ForMember(d => d.Point8_5, o => o.UseValue(dtoConfChild.Point8_5 ?? 0))
                       .ForMember(d => d.Point8_6, o => o.UseValue(dtoConfChild.Point8_6 ?? 0))
                       .ForMember(d => d.Point8_7, o => o.UseValue(dtoConfChild.Point8_7 ?? 0))
                       .ForMember(d => d.Point8_8, o => o.UseValue(dtoConfChild.Point8_8 ?? 0))
                       .ForMember(d => d.Point8_9, o => o.UseValue(dtoConfChild.Point8_9 ?? 0))
                       .ForMember(d => d.Point8_10, o => o.UseValue(dtoConfChild.Point8_10 ?? 0))
                       .ForMember(d => d.Point8_11, o => o.UseValue(dtoConfChild.Point8_11 ?? 0))
                       .ForMember(d => d.Point8_12, o => o.UseValue(dtoConfChild.Point8_12 ?? 0))
                       .ForMember(d => d.Point8_13, o => o.UseValue(dtoConfChild.Point8_13 ?? 0))
                       .ForMember(d => d.Point9, o => o.UseValue(dtoConfChild.Point9))
                       .ForMember(d => d.Point9_Remark, o => o.UseValue(dtoConfChild.Point9_Remark))
                       .ForMember(d => d.Point10, o => o.UseValue(dtoConfChild.Point10))
                       .ForMember(d => d.Point10_Remark, o => o.UseValue(dtoConfChild.Point10_Remark))
                       .ForMember(d => d.Point11, o => o.UseValue(dtoConfChild.Point11))
                       .ForMember(d => d.Point11_Remark, o => o.UseValue(dtoConfChild.Point11_Remark))
                       .ForMember(d => d.Point12, o => o.UseValue(dtoConfChild.Point12))
                       .ForMember(d => d.Point12_Remark, o => o.UseValue(dtoConfChild.Point12_Remark))
                       .ForMember(d => d.ConfirmationRecommended, o => o.UseValue(dtoConfChild.ConfirmationRecommended))
                       .ForMember(d => d.ConfirmationRecommendedReporting, o => o.UseValue(dtoConfChild.ConfirmationRecommendedReporting))
                       .ForMember(d => d.ConfirmationClarification, o => o.UseValue(dtoConfChild.ConfirmationClarification))
                       .ForMember(d => d.ReportingOfficerComment, o => o.UseValue(dtoConfChild.ReportingOfficerComment))
                       .ForMember(d => d.ReviewingOfficerComment, o => o.UseValue(dtoConfChild.ReviewingOfficerComment))
                       .ForMember(d => d.PayScale, o => o.UseValue(payscale.PayScale))
                       .ForMember(d => d.FormState, o => o.UseValue(dtoConfChild.StatusID))
                       .ForMember(d => d.RVPoint8_1, o => o.UseValue(dtoConfChild.RVPoint8_1 ?? 0))
                       .ForMember(d => d.RVPoint8_2, o => o.UseValue(dtoConfChild.RVPoint8_2 ?? 0))
                       .ForMember(d => d.RVPoint8_3, o => o.UseValue(dtoConfChild.RVPoint8_3 ?? 0))
                       .ForMember(d => d.RVRVPoint8_4, o => o.UseValue(dtoConfChild.RVRVPoint8_4 ?? 0))
                       .ForMember(d => d.RVRVPoint8_5, o => o.UseValue(dtoConfChild.RVRVPoint8_5 ?? 0))
                       .ForMember(d => d.RVPoint8_6, o => o.UseValue(dtoConfChild.RVPoint8_6 ?? 0))
                       .ForMember(d => d.RVPoint8_7, o => o.UseValue(dtoConfChild.RVPoint8_7 ?? 0))
                       .ForMember(d => d.RVPoint8_8, o => o.UseValue(dtoConfChild.RVPoint8_8 ?? 0))
                       .ForMember(d => d.RVPoint8_9, o => o.UseValue(dtoConfChild.RVPoint8_9 ?? 0))
                       .ForMember(d => d.RVPoint8_10, o => o.UseValue(dtoConfChild.RVPoint8_10 ?? 0))
                       .ForMember(d => d.RVPoint8_11, o => o.UseValue(dtoConfChild.RVPoint8_11 ?? 0))
                       .ForMember(d => d.RVPoint8_12, o => o.UseValue(dtoConfChild.RVPoint8_12 ?? 0))
                       .ForMember(d => d.RVPoint8_13, o => o.UseValue(dtoConfChild.RVPoint8_13 ?? 0))
                       .ForMember(d => d.RVPoint9, o => o.UseValue(dtoConfChild.RVPoint9))
                       .ForMember(d => d.RVPoint9_Remark, o => o.UseValue(dtoConfChild.RVPoint9_Remark))
                       .ForMember(d => d.RVPoint10, o => o.UseValue(dtoConfChild.RVPoint10))
                       .ForMember(d => d.RVPoint10_Remark, o => o.UseValue(dtoConfChild.RVPoint10_Remark))
                       .ForMember(d => d.RVPoint11, o => o.UseValue(dtoConfChild.RVPoint11))
                       .ForMember(d => d.RVPoint11_Remark, o => o.UseValue(dtoConfChild.RVPoint11_Remark))
                       .ForMember(d => d.RVPoint12, o => o.UseValue(dtoConfChild.RVPoint12))
                       .ForMember(d => d.RVPoint12_Remark, o => o.UseValue(dtoConfChild.RVPoint12_Remark))
                       .ForAllOtherMembers(d => d.Ignore());
                    });
                    var confFormDtls = Mapper.Map<Model.ConfirmationFormDetail>(dtoConfirmationUserForms);
                    if (processTypeID == 7)
                    {
                        var promotionJoinDt = genericRepo.Get<DTOModel.tblpromotion>(x => x.EmployeeCode == confFormDtls.EmployeeCode && x.ToDate == null).FirstOrDefault();
                        if (promotionJoinDt != null)
                        {
                            confFormDtls.DOJ = promotionJoinDt.FromDate.HasValue ? promotionJoinDt.FromDate.Value.ToString("dd/MM/yyyy") : "";
                            confFormDtls.orderOfPromotion = promotionJoinDt.OrderOfPromotion.HasValue ? promotionJoinDt.OrderOfPromotion.Value.ToString("dd/MM/yyyy") : "";
                        }
                    }

                    return confFormDtls;
                }
                else
                {
                    var currentdate = DateTime.Now;
                    var dtoConfChild = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.EmployeeID == employeeID && x.EmpProcessAppID == empProcessAppID && x.FormBHeaderID == formABHdrID).FirstOrDefault();
                    var dtoConfirmationUserForms = genericRepo.Get<DTOModel.ConfirmationFormBHeader>(x => x.EmployeeId == employeeID && x.ProcessId == processTypeID && !x.IsDeleted && x.FormBHeaderId == formABHdrID).OrderByDescending(x => x.DueConfirmationDate).FirstOrDefault();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.ConfirmationFormBHeader, Model.ConfirmationFormDetail>()
                        .ForMember(d => d.FormBHeaderId, o => o.MapFrom(s => s.FormBHeaderId))
                       .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                       .ForMember(d => d.DesignationId, o => o.MapFrom(s => s.DesignationId))
                       .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                       .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName))
                       .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.Designation.DesignationName))
                       .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                       .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                       .ForMember(d => d.DOJ, o => o.MapFrom(s => (s.tblMstEmployee.Pr_Loc_DOJ == null ? "" : s.tblMstEmployee.Pr_Loc_DOJ.Value.ToString("dd/MM/yyyy"))))
                       .ForMember(d => d.orderOfPromotion, o => o.MapFrom(s => (s.tblMstEmployee.orderofpromotion == null ? "" : s.tblMstEmployee.orderofpromotion.Value.ToString("dd/MM/yyyy"))))
                       .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => (s.DueConfirmationDate == null ? "" : s.DueConfirmationDate.Value.ToString("dd/MM/yyyy"))))
                       .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
                       .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
                       .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
                       .ForMember(d => d.Point8_1, o => o.UseValue(dtoConfChild.Point8_1 ?? 0))
                       .ForMember(d => d.Point8_2, o => o.UseValue(dtoConfChild.Point8_2 ?? 0))
                       .ForMember(d => d.Point8_3, o => o.UseValue(dtoConfChild.Point8_3 ?? 0))
                       .ForMember(d => d.Point8_4, o => o.UseValue(dtoConfChild.Point8_4 ?? 0))
                       .ForMember(d => d.Point8_5, o => o.UseValue(dtoConfChild.Point8_5 ?? 0))
                       .ForMember(d => d.Point8_6, o => o.UseValue(dtoConfChild.Point8_6 ?? 0))
                       .ForMember(d => d.Point8_7, o => o.UseValue(dtoConfChild.Point8_7 ?? 0))
                       .ForMember(d => d.Point8_8, o => o.UseValue(dtoConfChild.Point8_8 ?? 0))
                       .ForMember(d => d.Point8_9, o => o.UseValue(dtoConfChild.Point8_9 ?? 0))
                       .ForMember(d => d.Point8_10, o => o.UseValue(dtoConfChild.Point8_10 ?? 0))
                       .ForMember(d => d.Point8_11, o => o.UseValue(dtoConfChild.Point8_11 ?? 0))
                       .ForMember(d => d.Point8_12, o => o.UseValue(dtoConfChild.Point8_12 ?? 0))
                       .ForMember(d => d.Point8_13, o => o.UseValue(dtoConfChild.Point8_13 ?? 0))
                       .ForMember(d => d.Point9, o => o.UseValue(dtoConfChild.Point9))
                       .ForMember(d => d.Point9_Remark, o => o.UseValue(dtoConfChild.Point9_Remark))
                       .ForMember(d => d.Point10, o => o.UseValue(dtoConfChild.Point10))
                       .ForMember(d => d.Point10_Remark, o => o.UseValue(dtoConfChild.Point10_Remark))
                       .ForMember(d => d.Point11, o => o.UseValue(dtoConfChild.Point11))
                       .ForMember(d => d.Point11_Remark, o => o.UseValue(dtoConfChild.Point11_Remark))
                       .ForMember(d => d.Point12, o => o.UseValue(dtoConfChild.Point12))
                       .ForMember(d => d.Point12_Remark, o => o.UseValue(dtoConfChild.Point12_Remark))
                       .ForMember(d => d.ConfirmationRecommended, o => o.UseValue(dtoConfChild.ConfirmationRecommended))
                       .ForMember(d => d.ConfirmationRecommendedReporting, o => o.UseValue(dtoConfChild.ConfirmationRecommendedReporting))
                       .ForMember(d => d.ConfirmationClarification, o => o.UseValue(dtoConfChild.ConfirmationClarification))
                       .ForMember(d => d.ReportingOfficerComment, o => o.UseValue(dtoConfChild.ReportingOfficerComment))
                       .ForMember(d => d.ReviewingOfficerComment, o => o.UseValue(dtoConfChild.ReviewingOfficerComment))
                       .ForMember(d => d.PayScale, o => o.UseValue(payscale.PayScale))
                       .ForMember(d => d.FormState, o => o.UseValue(dtoConfChild.StatusID))
                        .ForMember(d => d.RVPoint8_1, o => o.UseValue(dtoConfChild.RVPoint8_1 ?? 0))
                       .ForMember(d => d.RVPoint8_2, o => o.UseValue(dtoConfChild.RVPoint8_2 ?? 0))
                       .ForMember(d => d.RVPoint8_3, o => o.UseValue(dtoConfChild.RVPoint8_3 ?? 0))
                       .ForMember(d => d.RVRVPoint8_4, o => o.UseValue(dtoConfChild.RVRVPoint8_4 ?? 0))
                       .ForMember(d => d.RVRVPoint8_5, o => o.UseValue(dtoConfChild.RVRVPoint8_5 ?? 0))
                       .ForMember(d => d.RVPoint8_6, o => o.UseValue(dtoConfChild.RVPoint8_6 ?? 0))
                       .ForMember(d => d.RVPoint8_7, o => o.UseValue(dtoConfChild.RVPoint8_7 ?? 0))
                       .ForMember(d => d.RVPoint8_8, o => o.UseValue(dtoConfChild.RVPoint8_8 ?? 0))
                       .ForMember(d => d.RVPoint8_9, o => o.UseValue(dtoConfChild.RVPoint8_9 ?? 0))
                       .ForMember(d => d.RVPoint8_10, o => o.UseValue(dtoConfChild.RVPoint8_10 ?? 0))
                       .ForMember(d => d.RVPoint8_11, o => o.UseValue(dtoConfChild.RVPoint8_11 ?? 0))
                       .ForMember(d => d.RVPoint8_12, o => o.UseValue(dtoConfChild.RVPoint8_12 ?? 0))
                       .ForMember(d => d.RVPoint8_13, o => o.UseValue(dtoConfChild.RVPoint8_13 ?? 0))
                       .ForMember(d => d.RVPoint9, o => o.UseValue(dtoConfChild.RVPoint9))
                       .ForMember(d => d.RVPoint9_Remark, o => o.UseValue(dtoConfChild.RVPoint9_Remark))
                       .ForMember(d => d.RVPoint10, o => o.UseValue(dtoConfChild.RVPoint10))
                       .ForMember(d => d.RVPoint10_Remark, o => o.UseValue(dtoConfChild.RVPoint10_Remark))
                       .ForMember(d => d.RVPoint11, o => o.UseValue(dtoConfChild.RVPoint11))
                       .ForMember(d => d.RVPoint11_Remark, o => o.UseValue(dtoConfChild.RVPoint11_Remark))
                       .ForMember(d => d.RVPoint12, o => o.UseValue(dtoConfChild.RVPoint12))
                       .ForMember(d => d.RVPoint12_Remark, o => o.UseValue(dtoConfChild.RVPoint12_Remark))
                       .ForAllOtherMembers(d => d.Ignore());
                    });
                    var confFormDtls = Mapper.Map<Model.ConfirmationFormDetail>(dtoConfirmationUserForms);
                    if (processTypeID == 7)
                    {
                        var promotionJoinDt = genericRepo.Get<DTOModel.tblpromotion>(x => x.EmployeeCode == confFormDtls.EmployeeCode && x.ToDate == null).FirstOrDefault();
                        if (promotionJoinDt != null)
                        {
                            confFormDtls.DOJ = promotionJoinDt.FromDate.HasValue ? promotionJoinDt.FromDate.Value.ToString("dd/MM/yyyy") : "";
                            confFormDtls.orderOfPromotion = promotionJoinDt.OrderOfPromotion.HasValue ? promotionJoinDt.OrderOfPromotion.Value.ToString("dd/MM/yyyy") : "";
                        }
                    }
                    return confFormDtls;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateFormAData(ConfirmationFormHdr confirmationFormAHdr)
        {
            log.Info($"ConfirmationFormService/UpdateFormAData");
            bool flag = false;
            try
            {
                if (confirmationFormAHdr.ApprovalHierarchy != 2)
                {
                    if (confirmationFormAHdr.submittedBy == Model.ConfirmationSubmittedBy.ReportingOfficer)
                    {
                        var headerid = confirmationFormAHdr.confirmationFormDetail.FormAHeaderId;
                        var dtoObj = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.FormHdrID == confirmationFormAHdr.FormHdrID && x.EmployeeID == confirmationFormAHdr.EmployeeID && x.EmpProcessAppID == confirmationFormAHdr.EmpProcessAppID && x.IsRejected == false && x.FormAHeaderID == headerid).FirstOrDefault();
                        dtoObj.Point8_1 = confirmationFormAHdr.confirmationFormDetail.Point8_1;
                        dtoObj.Point8_2 = confirmationFormAHdr.confirmationFormDetail.Point8_2;
                        dtoObj.Point8_3 = confirmationFormAHdr.confirmationFormDetail.Point8_3;
                        dtoObj.Point8_4 = confirmationFormAHdr.confirmationFormDetail.Point8_4;
                        dtoObj.Point8_5 = confirmationFormAHdr.confirmationFormDetail.Point8_5;
                        dtoObj.Point8_6 = confirmationFormAHdr.confirmationFormDetail.Point8_6;
                        dtoObj.Point8_7 = confirmationFormAHdr.confirmationFormDetail.Point8_7;
                        dtoObj.Point8_8 = confirmationFormAHdr.confirmationFormDetail.Point8_8;
                        dtoObj.Point8_9 = confirmationFormAHdr.confirmationFormDetail.Point8_9;
                        dtoObj.Point8_10 = confirmationFormAHdr.confirmationFormDetail.Point8_10;
                        dtoObj.Point8_11 = confirmationFormAHdr.confirmationFormDetail.Point8_11;
                        dtoObj.Point8_12 = confirmationFormAHdr.confirmationFormDetail.Point8_12;
                        dtoObj.Point8_13 = confirmationFormAHdr.confirmationFormDetail.Point8_13;
                        dtoObj.Point9 = confirmationFormAHdr.confirmationFormDetail.Point9;
                        dtoObj.Point9_Remark = confirmationFormAHdr.confirmationFormDetail.Point9_Remark;
                        dtoObj.Point10 = confirmationFormAHdr.confirmationFormDetail.Point10;
                        dtoObj.Point10_Remark = confirmationFormAHdr.confirmationFormDetail.Point10_Remark;
                        dtoObj.Point11 = confirmationFormAHdr.confirmationFormDetail.Point11;
                        dtoObj.Point11_Remark = confirmationFormAHdr.confirmationFormDetail.Point11_Remark;
                        dtoObj.Point12 = confirmationFormAHdr.confirmationFormDetail.Point12;
                        dtoObj.Point12_Remark = confirmationFormAHdr.confirmationFormDetail.Point12_Remark;
                        dtoObj.FormState = confirmationFormAHdr.confirmationFormDetail.FormState;
                        dtoObj.StatusID = (int)confirmationFormAHdr.confirmationFormDetail.FormState;
                        //dtoObj.ConfirmationRecommended = confirmationFormAHdr.confirmationFormDetail.ConfirmationRecommended;
                        dtoObj.ConfirmationRecommendedReporting = confirmationFormAHdr.confirmationFormDetail.ConfirmationRecommendedReporting;
                        dtoObj.ReportingOfficerComment = confirmationFormAHdr.confirmationFormDetail.ReportingOfficerComment;
                        dtoObj.UpdatedBy = confirmationFormAHdr.UpdatedBy;
                        dtoObj.UpdatedOn = confirmationFormAHdr.UpdatedOn;
                        genericRepo.Update<DTOModel.ConfirmationStatu>(dtoObj);
                        flag = true;

                    }
                    else if (confirmationFormAHdr.submittedBy == Model.ConfirmationSubmittedBy.ReviewingOfficer)
                    {
                        var headerid = confirmationFormAHdr.confirmationFormDetail.FormAHeaderId;
                        var dtoObj = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.FormHdrID == confirmationFormAHdr.FormHdrID && x.EmployeeID == confirmationFormAHdr.EmployeeID && x.EmpProcessAppID == confirmationFormAHdr.EmpProcessAppID && x.IsRejected == false && x.FormAHeaderID == headerid).FirstOrDefault();
                        dtoObj.ConfirmationRecommended = confirmationFormAHdr.confirmationFormDetail.ConfirmationRecommended;
                        dtoObj.FormState = confirmationFormAHdr.confirmationFormDetail.FormState;
                        dtoObj.StatusID = (int)confirmationFormAHdr.confirmationFormDetail.FormState;
                        dtoObj.ReviewingOfficerComment = confirmationFormAHdr.confirmationFormDetail.ReviewingOfficerComment;
                        dtoObj.ConfirmationClarification = confirmationFormAHdr.confirmationFormDetail.ConfirmationClarification;

                        dtoObj.RVPoint8_1 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_1;
                        dtoObj.RVPoint8_2 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_2;
                        dtoObj.RVPoint8_3 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_3;
                        dtoObj.RVRVPoint8_4 = confirmationFormAHdr.confirmationFormDetail.RVRVPoint8_4;
                        dtoObj.RVRVPoint8_5 = confirmationFormAHdr.confirmationFormDetail.RVRVPoint8_5;
                        dtoObj.RVPoint8_6 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_6;
                        dtoObj.RVPoint8_7 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_7;
                        dtoObj.RVPoint8_8 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_8;
                        dtoObj.RVPoint8_9 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_9;
                        dtoObj.RVPoint8_10 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_10;
                        dtoObj.RVPoint8_11 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_11;
                        dtoObj.RVPoint8_12 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_12;
                        dtoObj.RVPoint8_13 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_13;
                        dtoObj.RVPoint9 = confirmationFormAHdr.confirmationFormDetail.RVPoint9;
                        dtoObj.RVPoint9_Remark = confirmationFormAHdr.confirmationFormDetail.RVPoint9_Remark;
                        dtoObj.RVPoint10 = confirmationFormAHdr.confirmationFormDetail.RVPoint10;
                        dtoObj.RVPoint10_Remark = confirmationFormAHdr.confirmationFormDetail.RVPoint10_Remark;
                        dtoObj.RVPoint11 = confirmationFormAHdr.confirmationFormDetail.RVPoint11;
                        dtoObj.RVPoint11_Remark = confirmationFormAHdr.confirmationFormDetail.RVPoint11_Remark;
                        dtoObj.RVPoint12 = confirmationFormAHdr.confirmationFormDetail.RVPoint12;
                        dtoObj.RVPoint12_Remark = confirmationFormAHdr.confirmationFormDetail.RVPoint12_Remark;
                        genericRepo.Update<DTOModel.ConfirmationStatu>(dtoObj);
                        flag = true;
                    }

                    //if (flag)
                    //{

                    //    var dtoObjStatus = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.FormHdrID == confirmationFormAHdr.FormHdrID && x.EmployeeID == confirmationFormAHdr.EmployeeID && x.EmpProcessAppID == confirmationFormAHdr.EmpProcessAppID).FirstOrDefault();
                    //    if (dtoObjStatus != null)
                    //    {
                    //        dtoObjStatus.StatusID = (int)confirmationFormAHdr.confirmationFormDetail.FormState;
                    //        genericRepo.Update<DTOModel.ConfirmationStatu>(dtoObjStatus);
                    //    }
                    //}
                }
                else if (confirmationFormAHdr.ApprovalHierarchy == 2)
                {
                    var headerid = confirmationFormAHdr.confirmationFormDetail.FormAHeaderId;
                    var dtoObj = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.FormHdrID == confirmationFormAHdr.FormHdrID && x.EmployeeID == confirmationFormAHdr.EmployeeID && x.EmpProcessAppID == confirmationFormAHdr.EmpProcessAppID && x.IsRejected == false && x.FormAHeaderID == headerid).FirstOrDefault();

                    dtoObj.Point8_1 = confirmationFormAHdr.confirmationFormDetail.Point8_1;
                    dtoObj.Point8_2 = confirmationFormAHdr.confirmationFormDetail.Point8_2;
                    dtoObj.Point8_3 = confirmationFormAHdr.confirmationFormDetail.Point8_3;
                    dtoObj.Point8_4 = confirmationFormAHdr.confirmationFormDetail.Point8_4;
                    dtoObj.Point8_5 = confirmationFormAHdr.confirmationFormDetail.Point8_5;
                    dtoObj.Point8_6 = confirmationFormAHdr.confirmationFormDetail.Point8_6;
                    dtoObj.Point8_7 = confirmationFormAHdr.confirmationFormDetail.Point8_7;
                    dtoObj.Point8_8 = confirmationFormAHdr.confirmationFormDetail.Point8_8;
                    dtoObj.Point8_9 = confirmationFormAHdr.confirmationFormDetail.Point8_9;
                    dtoObj.Point8_10 = confirmationFormAHdr.confirmationFormDetail.Point8_10;
                    dtoObj.Point8_11 = confirmationFormAHdr.confirmationFormDetail.Point8_11;
                    dtoObj.Point8_12 = confirmationFormAHdr.confirmationFormDetail.Point8_12;
                    dtoObj.Point8_13 = confirmationFormAHdr.confirmationFormDetail.Point8_13;
                    dtoObj.Point9 = confirmationFormAHdr.confirmationFormDetail.Point9;
                    dtoObj.Point9_Remark = confirmationFormAHdr.confirmationFormDetail.Point9_Remark;
                    dtoObj.Point10 = confirmationFormAHdr.confirmationFormDetail.Point10;
                    dtoObj.Point10_Remark = confirmationFormAHdr.confirmationFormDetail.Point10_Remark;
                    dtoObj.Point11 = confirmationFormAHdr.confirmationFormDetail.Point11;
                    dtoObj.Point11_Remark = confirmationFormAHdr.confirmationFormDetail.Point11_Remark;
                    dtoObj.Point12 = confirmationFormAHdr.confirmationFormDetail.Point12;
                    dtoObj.Point12_Remark = confirmationFormAHdr.confirmationFormDetail.Point12_Remark;
                    dtoObj.FormState = confirmationFormAHdr.confirmationFormDetail.FormState;
                    dtoObj.StatusID = (int)confirmationFormAHdr.confirmationFormDetail.FormState;
                    dtoObj.ConfirmationRecommendedReporting = confirmationFormAHdr.confirmationFormDetail.ConfirmationRecommendedReporting;
                    dtoObj.ReportingOfficerComment = confirmationFormAHdr.confirmationFormDetail.ReportingOfficerComment;
                    dtoObj.UpdatedBy = confirmationFormAHdr.UpdatedBy;
                    dtoObj.UpdatedOn = confirmationFormAHdr.UpdatedOn;
                    dtoObj.ConfirmationRecommended = confirmationFormAHdr.confirmationFormDetail.ConfirmationRecommended;
                    dtoObj.ReviewingOfficerComment = confirmationFormAHdr.confirmationFormDetail.ReviewingOfficerComment;
                    dtoObj.ConfirmationClarification = confirmationFormAHdr.confirmationFormDetail.ConfirmationClarification;

                    dtoObj.RVPoint8_1 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_1;
                    dtoObj.RVPoint8_2 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_2;
                    dtoObj.RVPoint8_3 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_3;
                    dtoObj.RVRVPoint8_4 = confirmationFormAHdr.confirmationFormDetail.RVRVPoint8_4;
                    dtoObj.RVRVPoint8_5 = confirmationFormAHdr.confirmationFormDetail.RVRVPoint8_5;
                    dtoObj.RVPoint8_6 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_6;
                    dtoObj.RVPoint8_7 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_7;
                    dtoObj.RVPoint8_8 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_8;
                    dtoObj.RVPoint8_9 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_9;
                    dtoObj.RVPoint8_10 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_10;
                    dtoObj.RVPoint8_11 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_11;
                    dtoObj.RVPoint8_12 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_12;
                    dtoObj.RVPoint8_13 = confirmationFormAHdr.confirmationFormDetail.RVPoint8_13;
                    dtoObj.RVPoint9 = confirmationFormAHdr.confirmationFormDetail.RVPoint9;
                    dtoObj.RVPoint9_Remark = confirmationFormAHdr.confirmationFormDetail.RVPoint9_Remark;
                    dtoObj.RVPoint10 = confirmationFormAHdr.confirmationFormDetail.RVPoint10;
                    dtoObj.RVPoint10_Remark = confirmationFormAHdr.confirmationFormDetail.RVPoint10_Remark;
                    dtoObj.RVPoint11 = confirmationFormAHdr.confirmationFormDetail.RVPoint11;
                    dtoObj.RVPoint11_Remark = confirmationFormAHdr.confirmationFormDetail.RVPoint11_Remark;
                    dtoObj.RVPoint12 = confirmationFormAHdr.confirmationFormDetail.RVPoint12;
                    dtoObj.RVPoint12_Remark = confirmationFormAHdr.confirmationFormDetail.RVPoint12_Remark;
                    genericRepo.Update<DTOModel.ConfirmationStatu>(dtoObj);
                    flag = true;

                    //if (flag)
                    //{
                    //    var dtoObjStatus = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.FormHdrID == confirmationFormAHdr.FormHdrID && x.EmployeeID == confirmationFormAHdr.EmployeeID && x.EmpProcessAppID == confirmationFormAHdr.EmpProcessAppID).FirstOrDefault();
                    //    if (dtoObjStatus != null)
                    //    {
                    //        dtoObjStatus.StatusID = (int)confirmationFormAHdr.confirmationFormDetail.FormState;
                    //        genericRepo.Update<DTOModel.ConfirmationStatu>(dtoObjStatus);
                    //    }
                    //}
                }
                updateStatus(confirmationFormAHdr);

                if (confirmationFormAHdr._ConfirmationClarification?.FormHdrID > 0
                    && confirmationFormAHdr.loggedInEmpID == confirmationFormAHdr.ReportingTo)
                {
                    var lastClarificationRequest = genericRepo.Get<DTOModel.ConfirmationClarification>(x =>
                    x.FormHdrID == confirmationFormAHdr.FormHdrID && x.FormAHeaderID == confirmationFormAHdr._ConfirmationClarification.FormABHeaderID).OrderByDescending(y => y.ClarificationID)
                    .First();

                    //=====  Add RO Clarification Comments =======

                    lastClarificationRequest.ROClarification = confirmationFormAHdr._ConfirmationClarification.ROClarification;
                    lastClarificationRequest.UpdatedBy = confirmationFormAHdr._ConfirmationClarification.UpdatedBy;
                    lastClarificationRequest.UpdatedOn = confirmationFormAHdr._ConfirmationClarification.UpdatedOn;

                    genericRepo.Update<DTOModel.ConfirmationClarification>(lastClarificationRequest);

                }

                if (confirmationFormAHdr.loggedInEmpID == confirmationFormAHdr.ReviewingTo)
                    AddConfirmationClarification(confirmationFormAHdr._ConfirmationClarification);

                if (confirmationFormAHdr.confirmationFormDetail.FormState == (int)ConfirmationFormState.SubmitedByReporting ||
                    confirmationFormAHdr.confirmationFormDetail.FormState == (int)ConfirmationFormState.SubmitedByReviewer ||
                   confirmationFormAHdr.confirmationFormDetail.FormState == (int)ConfirmationFormState.RejectedByReporting || confirmationFormAHdr.confirmationFormDetail.FormState == (int)ConfirmationFormState.RejectedByReviewer || (confirmationFormAHdr.loggedInEmpID == confirmationFormAHdr.ReviewingTo && confirmationFormAHdr.confirmationFormDetail.ConfirmationClarification && confirmationFormAHdr.confirmationFormDetail.FormState == (int)ConfirmationFormState.SavedByReporting))
                {
                    AddProcessWorkFlow(confirmationFormAHdr._ProcessWorkFlow);
                    if (confirmationFormAHdr.ApprovalHierarchy == 1)
                        SendMailMessageToReceiver(confirmationFormAHdr);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdateFormBData(ConfirmationFormHdr confirmationFormBHdr)
        {
            log.Info($"ConfirmationFormService/UpdateFormBData");
            bool flag = false;
            try
            {
                if (confirmationFormBHdr.ApprovalHierarchy != 2)
                {
                    if (confirmationFormBHdr.submittedBy == Model.ConfirmationSubmittedBy.ReportingOfficer)
                    {
                        var headerid = confirmationFormBHdr.confirmationFormDetail.FormBHeaderId;
                        var dtoObj = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.FormHdrID == confirmationFormBHdr.FormHdrID && x.EmployeeID == confirmationFormBHdr.EmployeeID && x.EmpProcessAppID == confirmationFormBHdr.EmpProcessAppID && x.IsRejected == false && x.FormBHeaderID == headerid).FirstOrDefault();

                        dtoObj.Point8_1 = confirmationFormBHdr.confirmationFormDetail.Point8_1;
                        dtoObj.Point8_2 = confirmationFormBHdr.confirmationFormDetail.Point8_2;
                        dtoObj.Point8_3 = confirmationFormBHdr.confirmationFormDetail.Point8_3;
                        dtoObj.Point8_4 = confirmationFormBHdr.confirmationFormDetail.Point8_4;
                        dtoObj.Point8_5 = confirmationFormBHdr.confirmationFormDetail.Point8_5;
                        dtoObj.Point8_6 = confirmationFormBHdr.confirmationFormDetail.Point8_6;
                        dtoObj.Point8_7 = confirmationFormBHdr.confirmationFormDetail.Point8_7;
                        dtoObj.Point8_8 = confirmationFormBHdr.confirmationFormDetail.Point8_8;
                        dtoObj.Point8_9 = confirmationFormBHdr.confirmationFormDetail.Point8_9;
                        dtoObj.Point8_10 = confirmationFormBHdr.confirmationFormDetail.Point8_10;
                        dtoObj.Point8_11 = confirmationFormBHdr.confirmationFormDetail.Point8_11;
                        dtoObj.Point8_12 = confirmationFormBHdr.confirmationFormDetail.Point8_12;
                        dtoObj.Point8_13 = confirmationFormBHdr.confirmationFormDetail.Point8_13;
                        dtoObj.Point9 = confirmationFormBHdr.confirmationFormDetail.Point9;
                        dtoObj.Point9_Remark = confirmationFormBHdr.confirmationFormDetail.Point9_Remark;
                        dtoObj.Point10 = confirmationFormBHdr.confirmationFormDetail.Point10;
                        dtoObj.Point10_Remark = confirmationFormBHdr.confirmationFormDetail.Point10_Remark;
                        dtoObj.Point11 = confirmationFormBHdr.confirmationFormDetail.Point11;
                        dtoObj.Point11_Remark = confirmationFormBHdr.confirmationFormDetail.Point11_Remark;
                        dtoObj.Point12 = confirmationFormBHdr.confirmationFormDetail.Point12;
                        dtoObj.Point12_Remark = confirmationFormBHdr.confirmationFormDetail.Point12_Remark;
                        dtoObj.FormState = confirmationFormBHdr.confirmationFormDetail.FormState;
                        dtoObj.StatusID = (int)confirmationFormBHdr.confirmationFormDetail.FormState;
                        //dtoObj.ConfirmationRecommended = confirmationFormAHdr.confirmationFormDetail.ConfirmationRecommended;
                        dtoObj.ConfirmationRecommendedReporting = confirmationFormBHdr.confirmationFormDetail.ConfirmationRecommendedReporting;
                        dtoObj.ReportingOfficerComment = confirmationFormBHdr.confirmationFormDetail.ReportingOfficerComment;
                        dtoObj.UpdatedBy = confirmationFormBHdr.UpdatedBy;
                        dtoObj.UpdatedOn = confirmationFormBHdr.UpdatedOn;
                        genericRepo.Update<DTOModel.ConfirmationStatu>(dtoObj);
                        flag = true;
                    }
                    else if (confirmationFormBHdr.submittedBy == Model.ConfirmationSubmittedBy.ReviewingOfficer)
                    {
                        var headerid = confirmationFormBHdr.confirmationFormDetail.FormBHeaderId;
                        var dtoObj = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.FormHdrID == confirmationFormBHdr.FormHdrID && x.EmployeeID == confirmationFormBHdr.EmployeeID && x.EmpProcessAppID == confirmationFormBHdr.EmpProcessAppID && x.IsRejected == false && x.FormBHeaderID == headerid).FirstOrDefault();
                        dtoObj.ConfirmationRecommended = confirmationFormBHdr.confirmationFormDetail.ConfirmationRecommended;
                        dtoObj.FormState = confirmationFormBHdr.confirmationFormDetail.FormState;
                        dtoObj.StatusID = (int)confirmationFormBHdr.confirmationFormDetail.FormState;
                        dtoObj.ReviewingOfficerComment = confirmationFormBHdr.confirmationFormDetail.ReviewingOfficerComment;
                        dtoObj.ConfirmationClarification = confirmationFormBHdr.confirmationFormDetail.ConfirmationClarification;

                        dtoObj.RVPoint8_1 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_1;
                        dtoObj.RVPoint8_2 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_2;
                        dtoObj.RVPoint8_3 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_3;
                        dtoObj.RVRVPoint8_4 = confirmationFormBHdr.confirmationFormDetail.RVRVPoint8_4;
                        dtoObj.RVRVPoint8_5 = confirmationFormBHdr.confirmationFormDetail.RVRVPoint8_5;
                        dtoObj.RVPoint8_6 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_6;
                        dtoObj.RVPoint8_7 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_7;
                        dtoObj.RVPoint8_8 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_8;
                        dtoObj.RVPoint8_9 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_9;
                        dtoObj.RVPoint8_10 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_10;
                        dtoObj.RVPoint8_11 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_11;
                        dtoObj.RVPoint8_12 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_12;
                        dtoObj.RVPoint8_13 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_13;
                        dtoObj.RVPoint9 = confirmationFormBHdr.confirmationFormDetail.RVPoint9;
                        dtoObj.RVPoint9_Remark = confirmationFormBHdr.confirmationFormDetail.RVPoint9_Remark;
                        dtoObj.RVPoint10 = confirmationFormBHdr.confirmationFormDetail.RVPoint10;
                        dtoObj.RVPoint10_Remark = confirmationFormBHdr.confirmationFormDetail.RVPoint10_Remark;
                        dtoObj.RVPoint11 = confirmationFormBHdr.confirmationFormDetail.RVPoint11;
                        dtoObj.RVPoint11_Remark = confirmationFormBHdr.confirmationFormDetail.RVPoint11_Remark;
                        dtoObj.RVPoint12 = confirmationFormBHdr.confirmationFormDetail.RVPoint12;
                        dtoObj.RVPoint12_Remark = confirmationFormBHdr.confirmationFormDetail.RVPoint12_Remark;
                        genericRepo.Update<DTOModel.ConfirmationStatu>(dtoObj);
                        flag = true;
                    }
                    //if (flag)
                    //{

                    //    var dtoObjStatus = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.FormHdrID == confirmationFormBHdr.FormHdrID && x.EmployeeID == confirmationFormBHdr.EmployeeID && x.EmpProcessAppID == confirmationFormBHdr.EmpProcessAppID).FirstOrDefault();
                    //    if (dtoObjStatus != null)
                    //    {
                    //        dtoObjStatus.StatusID = (int)confirmationFormBHdr.confirmationFormDetail.FormState;
                    //        dtoObjStatus.UpdatedBy = confirmationFormBHdr.UpdatedBy;
                    //        dtoObjStatus.UpdatedOn = confirmationFormBHdr.UpdatedOn;
                    //        genericRepo.Update<DTOModel.ConfirmationStatu>(dtoObjStatus);
                    //    }
                    //}
                }
                else if (confirmationFormBHdr.ApprovalHierarchy == 2)
                {
                    var headerid = confirmationFormBHdr.confirmationFormDetail.FormBHeaderId;
                    var dtoObj = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.FormHdrID == confirmationFormBHdr.FormHdrID && x.EmployeeID == confirmationFormBHdr.EmployeeID && x.EmpProcessAppID == confirmationFormBHdr.EmpProcessAppID && x.IsRejected == false && x.FormBHeaderID == headerid).FirstOrDefault();
                    dtoObj.Point8_1 = confirmationFormBHdr.confirmationFormDetail.Point8_1;
                    dtoObj.Point8_2 = confirmationFormBHdr.confirmationFormDetail.Point8_2;
                    dtoObj.Point8_3 = confirmationFormBHdr.confirmationFormDetail.Point8_3;
                    dtoObj.Point8_4 = confirmationFormBHdr.confirmationFormDetail.Point8_4;
                    dtoObj.Point8_5 = confirmationFormBHdr.confirmationFormDetail.Point8_5;
                    dtoObj.Point8_6 = confirmationFormBHdr.confirmationFormDetail.Point8_6;
                    dtoObj.Point8_7 = confirmationFormBHdr.confirmationFormDetail.Point8_7;
                    dtoObj.Point8_8 = confirmationFormBHdr.confirmationFormDetail.Point8_8;
                    dtoObj.Point8_9 = confirmationFormBHdr.confirmationFormDetail.Point8_9;
                    dtoObj.Point8_10 = confirmationFormBHdr.confirmationFormDetail.Point8_10;
                    dtoObj.Point8_11 = confirmationFormBHdr.confirmationFormDetail.Point8_11;
                    dtoObj.Point8_12 = confirmationFormBHdr.confirmationFormDetail.Point8_12;
                    dtoObj.Point8_13 = confirmationFormBHdr.confirmationFormDetail.Point8_13;
                    dtoObj.Point9 = confirmationFormBHdr.confirmationFormDetail.Point9;
                    dtoObj.Point9_Remark = confirmationFormBHdr.confirmationFormDetail.Point9_Remark;
                    dtoObj.Point10 = confirmationFormBHdr.confirmationFormDetail.Point10;
                    dtoObj.Point10_Remark = confirmationFormBHdr.confirmationFormDetail.Point10_Remark;
                    dtoObj.Point11 = confirmationFormBHdr.confirmationFormDetail.Point11;
                    dtoObj.Point11_Remark = confirmationFormBHdr.confirmationFormDetail.Point11_Remark;
                    dtoObj.Point12 = confirmationFormBHdr.confirmationFormDetail.Point12;
                    dtoObj.Point12_Remark = confirmationFormBHdr.confirmationFormDetail.Point12_Remark;
                    dtoObj.FormState = confirmationFormBHdr.confirmationFormDetail.FormState;
                    dtoObj.StatusID = (int)confirmationFormBHdr.confirmationFormDetail.FormState;
                    dtoObj.ConfirmationRecommendedReporting = confirmationFormBHdr.confirmationFormDetail.ConfirmationRecommendedReporting;
                    dtoObj.ReportingOfficerComment = confirmationFormBHdr.confirmationFormDetail.ReportingOfficerComment;
                    dtoObj.UpdatedBy = confirmationFormBHdr.UpdatedBy;
                    dtoObj.UpdatedOn = confirmationFormBHdr.UpdatedOn;
                    dtoObj.ConfirmationRecommended = confirmationFormBHdr.confirmationFormDetail.ConfirmationRecommended;
                    dtoObj.ReviewingOfficerComment = confirmationFormBHdr.confirmationFormDetail.ReviewingOfficerComment;
                    dtoObj.ConfirmationClarification = confirmationFormBHdr.confirmationFormDetail.ConfirmationClarification;

                    dtoObj.RVPoint8_1 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_1;
                    dtoObj.RVPoint8_2 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_2;
                    dtoObj.RVPoint8_3 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_3;
                    dtoObj.RVRVPoint8_4 = confirmationFormBHdr.confirmationFormDetail.RVRVPoint8_4;
                    dtoObj.RVRVPoint8_5 = confirmationFormBHdr.confirmationFormDetail.RVRVPoint8_5;
                    dtoObj.RVPoint8_6 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_6;
                    dtoObj.RVPoint8_7 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_7;
                    dtoObj.RVPoint8_8 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_8;
                    dtoObj.RVPoint8_9 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_9;
                    dtoObj.RVPoint8_10 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_10;
                    dtoObj.RVPoint8_11 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_11;
                    dtoObj.RVPoint8_12 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_12;
                    dtoObj.RVPoint8_13 = confirmationFormBHdr.confirmationFormDetail.RVPoint8_13;
                    dtoObj.RVPoint9 = confirmationFormBHdr.confirmationFormDetail.RVPoint9;
                    dtoObj.RVPoint9_Remark = confirmationFormBHdr.confirmationFormDetail.RVPoint9_Remark;
                    dtoObj.RVPoint10 = confirmationFormBHdr.confirmationFormDetail.RVPoint10;
                    dtoObj.RVPoint10_Remark = confirmationFormBHdr.confirmationFormDetail.RVPoint10_Remark;
                    dtoObj.RVPoint11 = confirmationFormBHdr.confirmationFormDetail.RVPoint11;
                    dtoObj.RVPoint11_Remark = confirmationFormBHdr.confirmationFormDetail.RVPoint11_Remark;
                    dtoObj.RVPoint12 = confirmationFormBHdr.confirmationFormDetail.RVPoint12;
                    dtoObj.RVPoint12_Remark = confirmationFormBHdr.confirmationFormDetail.RVPoint12_Remark;
                    genericRepo.Update<DTOModel.ConfirmationStatu>(dtoObj);
                    flag = true;
                    //if (flag)
                    //{

                    //    var dtoObjStatus = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.FormHdrID == confirmationFormBHdr.FormHdrID && x.EmployeeID == confirmationFormBHdr.EmployeeID && x.EmpProcessAppID == confirmationFormBHdr.EmpProcessAppID).FirstOrDefault();
                    //    if (dtoObjStatus != null)
                    //    {
                    //        dtoObjStatus.StatusID = (int)confirmationFormBHdr.confirmationFormDetail.FormState;
                    //        dtoObjStatus.UpdatedBy = confirmationFormBHdr.UpdatedBy;
                    //        dtoObjStatus.UpdatedOn = confirmationFormBHdr.UpdatedOn;
                    //        genericRepo.Update<DTOModel.ConfirmationStatu>(dtoObjStatus);
                    //    }
                    //}
                }
                updateStatus(confirmationFormBHdr);

                if (confirmationFormBHdr._ConfirmationClarification?.FormHdrID > 0
                    && confirmationFormBHdr.loggedInEmpID == confirmationFormBHdr.ReportingTo)
                {
                    var lastClarificationRequest = genericRepo.Get<DTOModel.ConfirmationClarification>(x =>
                    x.FormHdrID == confirmationFormBHdr.FormHdrID && x.FormBHeaderID == confirmationFormBHdr._ConfirmationClarification.FormABHeaderID).OrderByDescending(y => y.ClarificationID)
                    .First();

                    //=====  Add RO Clarification Comments =======

                    lastClarificationRequest.ROClarification = confirmationFormBHdr._ConfirmationClarification.ROClarification;
                    lastClarificationRequest.UpdatedBy = confirmationFormBHdr._ConfirmationClarification.UpdatedBy;
                    lastClarificationRequest.UpdatedOn = confirmationFormBHdr._ConfirmationClarification.UpdatedOn;

                    genericRepo.Update<DTOModel.ConfirmationClarification>(lastClarificationRequest);

                }

                if (confirmationFormBHdr.loggedInEmpID == confirmationFormBHdr.ReviewingTo)
                {
                    AddConfirmationClarification(confirmationFormBHdr._ConfirmationClarification);
                }
                if (confirmationFormBHdr.confirmationFormDetail.FormState == (int)ConfirmationFormState.SubmitedByReporting ||
                    confirmationFormBHdr.confirmationFormDetail.FormState == (int)ConfirmationFormState.SubmitedByReviewer ||
                   confirmationFormBHdr.confirmationFormDetail.FormState == (int)ConfirmationFormState.RejectedByReporting || confirmationFormBHdr.confirmationFormDetail.FormState == (int)ConfirmationFormState.RejectedByReviewer || (confirmationFormBHdr.loggedInEmpID == confirmationFormBHdr.ReviewingTo && confirmationFormBHdr.confirmationFormDetail.ConfirmationClarification && confirmationFormBHdr.confirmationFormDetail.FormState == (int)ConfirmationFormState.SavedByReporting))
                {
                    AddProcessWorkFlow(confirmationFormBHdr._ProcessWorkFlow);
                    if (confirmationFormBHdr.ApprovalHierarchy == 1)
                        SendMailMessageToReceiver(confirmationFormBHdr);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        private bool updateStatus(ConfirmationFormHdr confirmationFormHdr)
        {
            bool flag = false;
            try
            {
                if ((confirmationFormHdr.ReviewingTo.HasValue && confirmationFormHdr.confirmationFormDetail.FormState != 7) || (!confirmationFormHdr.ReviewingTo.HasValue && confirmationFormHdr.confirmationFormDetail.FormState != 4))
                {
                    var isupdate = confirmRepo.ConfirmationStatusUpdate(confirmationFormHdr.EmployeeID, confirmationFormHdr.ProcessID, confirmationFormHdr.FormHdrID);// check whether master table has to update or not for status column
                    if (isupdate.HasValue && isupdate.Value == 0) // when all the rows of confirmation status table has same status then update master table
                    {
                        var dtoObj = genericRepo.Get<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == confirmationFormHdr.EmployeeID && x.ProcessID == confirmationFormHdr.ProcessID && x.FormTypeID == confirmationFormHdr.FormTypeID && x.FormHdrID == confirmationFormHdr.FormHdrID).FirstOrDefault();
                        dtoObj.StatusID = confirmationFormHdr.confirmationFormDetail.FormState;
                        genericRepo.Update<DTOModel.ConfirmationFormHdr>(dtoObj);
                        flag = true;
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        //private bool ExtendConfirmationForm(ConfirmationPRSectionRemarks confirmationFormHdr)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        var dtoObj = genericRepo.Get<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == confirmationFormHdr.EmployeeID && x.ProcessID == confirmationFormHdr.ProcessID && x.FormTypeID == confirmationFormHdr.FormTypeID && x.FormHdrID == confirmationFormHdr.FormHdID).FirstOrDefault();
        //        //dtoObj.StatusID = 0;
        //        Mapper.Initialize(cfg =>
        //        {
        //            cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormHdr>()
        //            .ForMember(d => d.StatusID, o => o.UseValue(0));
        //        });
        //        var dtoConfirmationHdr = Mapper.Map<DTOModel.ConfirmationFormHdr>(dtoObj);
        //        genericRepo.Update<DTOModel.ConfirmationFormHdr>(dtoConfirmationHdr);
        //        var formHdrID = dtoObj.FormHdrID;

        //        if (confirmationFormHdr.FormTypeID == 1)
        //        {
        //            Model.ConfirmationFormAHdr confirmationFormA = new ConfirmationFormAHdr();
        //            confirmationFormA.EmployeeId = confirmationFormHdr.EmployeeID;
        //            confirmationFormA.ProcessId = confirmationFormHdr.ProcessID;
        //            confirmationFormA.CertificatesReceived = true;
        //            confirmationFormA.PoliceVerification = true;
        //            confirmationFormA.CreatedBy = confirmationFormHdr.UpdatedBy == null ? 1 : confirmationFormHdr.UpdatedBy.Value;
        //            confirmationFormA.CreatedOn = DateTime.Now;
        //            DateTime dt;
        //            var dueDate = DateTime.TryParse(confirmationFormHdr.DueDate.Value.ToString(), out dt);
        //            confirmationFormA.DueConfirmationDate = dt.AddMonths(6);
        //            confirmationFormA.FormHdrID = formHdrID;

        //            var dtoConfirmationFormA = genericRepo.Get<DTOModel.ConfirmationFormAHeader>(x => x.EmployeeId == confirmationFormHdr.EmployeeID && x.ProcessId == confirmationFormHdr.ProcessID && x.FormHdrID == confirmationFormHdr.FormHdID).FirstOrDefault();
        //            if (dtoConfirmationFormA != null)
        //            {
        //                confirmationFormA.DesignationId = dtoConfirmationFormA.DesignationId;
        //                confirmationFormA.BranchId = dtoConfirmationFormA.BranchId;
        //                //dtoConfirmationFormA.FormState = (int)ConfirmationFormState.Rejected;
        //                Mapper.Initialize(cfg =>
        //                {
        //                    cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormAHeader>()
        //                    .ForMember(d => d.FormState, o => o.UseValue((int)ConfirmationFormState.Rejected))
        //                    .ForMember(d => d.UpdatedBy, o => o.UseValue(confirmationFormA.CreatedBy))
        //                    .ForMember(d => d.UpdatedOn, o => o.UseValue(confirmationFormA.CreatedOn));
        //                });
        //                var dtoConfirmationA = Mapper.Map<DTOModel.ConfirmationFormAHeader>(dtoConfirmationFormA);
        //                genericRepo.Update<DTOModel.ConfirmationFormAHeader>(dtoConfirmationA);
        //                flag = true;
        //            }

        //            Mapper.Initialize(cfg =>
        //                {
        //                    cfg.CreateMap<Model.ConfirmationFormAHdr, DTOModel.ConfirmationFormAHeader>()
        //                    .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
        //                    .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
        //                    .ForMember(d => d.DesignationId, o => o.MapFrom(s => s.DesignationId))
        //                    .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
        //                    .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
        //                    .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
        //                    .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => s.DueConfirmationDate))
        //                    .ForMember(d => d.FormState, o => o.UseValue(0))
        //                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
        //                    .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
        //                    .ForMember(d => d.FormHdrID, o => o.MapFrom(s => s.FormHdrID))
        //                    .ForAllOtherMembers(d => d.Ignore());
        //                });
        //            var dtoConfirmationFormAHdr = Mapper.Map<DTOModel.ConfirmationFormAHeader>(confirmationFormA);
        //            genericRepo.Insert<DTOModel.ConfirmationFormAHeader>(dtoConfirmationFormAHdr);


        //            var dtoObjStatus = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.EmployeeID == confirmationFormHdr.EmployeeID && x.ProcessId == confirmationFormHdr.ProcessID && x.FormHdrID == confirmationFormHdr.FormHdID && x.IsRejected == false).ToList();
        //            if (dtoObjStatus != null && dtoObjStatus.Count > 0)
        //            {
        //                foreach (var item in dtoObjStatus)
        //                {
        //                    item.IsRejected = true;
        //                    item.UpdatedBy = confirmationFormA.CreatedBy;
        //                    item.UpdatedOn = confirmationFormA.CreatedOn;
        //                    genericRepo.Update<DTOModel.ConfirmationStatu>(item);
        //                }
        //            }

        //            Mapper.Initialize(cfg =>
        //            {
        //                cfg.CreateMap<DTOModel.ConfirmationStatu, DTOModel.ConfirmationStatu>()
        //                .ForMember(d => d.FormHdrID, o => o.UseValue(formHdrID))
        //                .ForMember(d => d.StatusID, o => o.UseValue(0))
        //                .ForMember(d => d.EmpProcessAppID, o => o.MapFrom(s => s.EmpProcessAppID))
        //                .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
        //                .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
        //                .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
        //                .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
        //                .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => s.DueConfirmationDate))
        //                .ForMember(d => d.FormState, o => o.UseValue(0))
        //                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
        //                .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
        //                .ForMember(d => d.FormAHeaderID, o => o.UseValue(dtoConfirmationFormAHdr.FormAHeaderId))
        //                .ForAllOtherMembers(d => d.Ignore());
        //            });
        //            var dtoConfStatus = Mapper.Map<List<DTOModel.ConfirmationStatu>>(dtoObjStatus);
        //            if (dtoConfStatus != null && dtoConfStatus.Count > 0)
        //            {
        //                genericRepo.AddMultipleEntity<DTOModel.ConfirmationStatu>(dtoConfStatus);
        //                flag = true;
        //            }
        //        }
        //        else if (confirmationFormHdr.FormTypeID == 2)
        //        {
        //            Model.ConfirmationFormAHdr confirmationFormB = new ConfirmationFormAHdr();
        //            confirmationFormB.EmployeeId = confirmationFormHdr.EmployeeID;
        //            confirmationFormB.ProcessId = confirmationFormHdr.ProcessID;
        //            confirmationFormB.CertificatesReceived = true;
        //            confirmationFormB.PoliceVerification = true;
        //            confirmationFormB.CreatedBy = confirmationFormHdr.UpdatedBy == null ? 1 : confirmationFormHdr.UpdatedBy.Value;
        //            confirmationFormB.CreatedOn = DateTime.Now;
        //            DateTime dt;
        //            var dueDate = DateTime.TryParse(confirmationFormHdr.DueDate.Value.ToString(), out dt);
        //            confirmationFormB.DueConfirmationDate = dt.AddMonths(6);
        //            confirmationFormB.FormHdrID = formHdrID;


        //            var dtoConfirmationFormB = genericRepo.Get<DTOModel.ConfirmationFormBHeader>(x => x.EmployeeId == confirmationFormHdr.EmployeeID && x.ProcessId == confirmationFormHdr.ProcessID && x.FormHdrID == confirmationFormHdr.FormHdID).FirstOrDefault();
        //            if (dtoConfirmationFormB != null)
        //            {
        //                confirmationFormB.BranchId = dtoConfirmationFormB.BranchId;
        //                confirmationFormB.DesignationId = dtoConfirmationFormB.DesignationId;

        //              //  dtoConfirmationFormB.FormState = (int)ConfirmationFormState.Rejected;
        //                Mapper.Initialize(cfg =>
        //                {
        //                    cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormBHeader>()
        //                    .ForMember(d => d.FormState, o => o.UseValue((int)ConfirmationFormState.Rejected))
        //                    .ForMember(d => d.UpdatedBy, o => o.UseValue(confirmationFormB.CreatedBy))
        //                    .ForMember(d => d.UpdatedOn, o => o.UseValue(confirmationFormB.CreatedOn));
        //                });
        //                var dtoConfirmationB = Mapper.Map<DTOModel.ConfirmationFormBHeader>(dtoConfirmationFormB);
        //                genericRepo.Update<DTOModel.ConfirmationFormBHeader>(dtoConfirmationB);
        //                flag = true;
        //            }

        //            Mapper.Initialize(cfg =>
        //            {
        //                cfg.CreateMap<Model.ConfirmationFormAHdr, DTOModel.ConfirmationFormBHeader>()
        //                .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
        //                .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
        //                .ForMember(d => d.DesignationId, o => o.MapFrom(s => s.DesignationId))
        //                 .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
        //                 .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
        //                 .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
        //                 .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => s.DueConfirmationDate))
        //                 .ForMember(d => d.FormState, o => o.UseValue(0))
        //                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
        //                .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
        //                .ForMember(d => d.FormHdrID, o => o.MapFrom(s => s.FormHdrID))
        //                .ForAllOtherMembers(d => d.Ignore());
        //            });
        //            var dtoConfirmationFormBHdr = Mapper.Map<DTOModel.ConfirmationFormBHeader>(confirmationFormB);
        //            genericRepo.Insert<DTOModel.ConfirmationFormBHeader>(dtoConfirmationFormBHdr);


        //            var dtoObjStatus = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.EmployeeID == confirmationFormHdr.EmployeeID && x.ProcessId == confirmationFormHdr.ProcessID && x.FormHdrID == confirmationFormHdr.FormHdID && x.IsRejected == false).ToList();
        //            if (dtoObjStatus != null && dtoObjStatus.Count > 0)
        //            {
        //                foreach (var item in dtoObjStatus)
        //                {
        //                    item.IsRejected = true;
        //                    item.UpdatedBy = confirmationFormB.CreatedBy;
        //                    item.UpdatedOn = confirmationFormB.CreatedOn;
        //                    genericRepo.Update<DTOModel.ConfirmationStatu>(item);
        //                }
        //            }

        //            Mapper.Initialize(cfg =>
        //            {
        //                cfg.CreateMap<DTOModel.ConfirmationStatu, DTOModel.ConfirmationStatu>()
        //                .ForMember(d => d.FormHdrID, o => o.UseValue(formHdrID))
        //                .ForMember(d => d.StatusID, o => o.UseValue(0))
        //                .ForMember(d => d.EmpProcessAppID, o => o.MapFrom(s => s.EmpProcessAppID))
        //                .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
        //                .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
        //                .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
        //                .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
        //                .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => s.DueConfirmationDate))
        //                .ForMember(d => d.FormState, o => o.UseValue(0))
        //                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
        //                .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
        //                 .ForMember(d => d.FormBHeaderID, o => o.UseValue(dtoConfirmationFormBHdr.FormBHeaderId))
        //                .ForAllOtherMembers(d => d.Ignore());
        //            });
        //            var dtoConfStatus = Mapper.Map<List<DTOModel.ConfirmationStatu>>(dtoObjStatus);
        //            if (dtoConfStatus != null && dtoConfStatus.Count > 0)
        //            {
        //                genericRepo.AddMultipleEntity<DTOModel.ConfirmationStatu>(dtoConfStatus);
        //                flag = true;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }
        //    return flag;
        //}

        private bool ExtendConfirmationForm(ConfirmationPRSectionRemarks confirmationFormHdr)
        {
            bool flag = false; int? confirmFormAHdrID = (int?)null, confirmFormBHdrID = (int?)null;
            try
            {
                var isSecondRejection = genericRepo.Exists<DTOModel.ConfirmationStatu>(
                    x => x.FormHdrID == confirmationFormHdr.FormHdID
                    && x.EmployeeID == confirmationFormHdr.EmployeeID
                    && x.IsRejected && !x.IsDeleted);

                if (!isSecondRejection)
                {
                    var dtoObj = genericRepo.Get<DTOModel.ConfirmationFormHdr>(x =>
                    x.EmployeeID == confirmationFormHdr.EmployeeID
                    && x.ProcessID == confirmationFormHdr.ProcessID
                    && x.FormTypeID == confirmationFormHdr.FormTypeID
                    && x.FormHdrID == confirmationFormHdr.FormHdID).FirstOrDefault();
                    //dtoObj.StatusID = 0;
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormHdr>()
                        .ForMember(d => d.StatusID, o => o.UseValue(0));
                    });
                    var dtoConfirmationHdr = Mapper.Map<DTOModel.ConfirmationFormHdr>(dtoObj);
                    genericRepo.Update<DTOModel.ConfirmationFormHdr>(dtoConfirmationHdr);
                    var formHdrID = dtoObj.FormHdrID;
                }
                if (confirmationFormHdr.FormTypeID == 1)
                {
                    Model.ConfirmationFormAHdr confirmationFormA = new ConfirmationFormAHdr();
                    confirmationFormA.EmployeeId = confirmationFormHdr.EmployeeID;
                    confirmationFormA.ProcessId = confirmationFormHdr.ProcessID;
                    confirmationFormA.CertificatesReceived = true;
                    confirmationFormA.PoliceVerification = true;
                    confirmationFormA.CreatedBy = confirmationFormHdr.UpdatedBy == null ? 1 : confirmationFormHdr.UpdatedBy.Value;
                    confirmationFormA.CreatedOn = DateTime.Now;
                    DateTime dt;
                    var dueDate = DateTime.TryParse(confirmationFormHdr.DueDate.Value.ToString(), out dt);
                    confirmationFormA.DueConfirmationDate = dt.AddMonths(6);
                    confirmationFormA.FormHdrID = confirmationFormHdr.FormHdID;

                    var dtoConfirmationFormA = genericRepo.Get<DTOModel.ConfirmationFormAHeader>(x => x.EmployeeId == confirmationFormHdr.EmployeeID && x.ProcessId == confirmationFormHdr.ProcessID && x.FormHdrID == confirmationFormHdr.FormHdID).FirstOrDefault();
                    if (dtoConfirmationFormA != null)
                    {
                        confirmationFormA.DesignationId = dtoConfirmationFormA.DesignationId;
                        confirmationFormA.BranchId = dtoConfirmationFormA.BranchId;
                        //dtoConfirmationFormA.FormState = (int)ConfirmationFormState.Rejected;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormAHeader>()
                            .ForMember(d => d.FormState, o => o.UseValue((int)ConfirmationFormState.Rejected));
                        });
                        var dtoConfirmationA = Mapper.Map<DTOModel.ConfirmationFormAHeader>(dtoConfirmationFormA);
                        genericRepo.Update<DTOModel.ConfirmationFormAHeader>(dtoConfirmationA);
                        flag = true;
                    }

                    if (!isSecondRejection)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.ConfirmationFormAHdr, DTOModel.ConfirmationFormAHeader>()
                            .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                            .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
                            .ForMember(d => d.DesignationId, o => o.MapFrom(s => s.DesignationId))
                            .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                            .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
                            .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
                            .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => s.DueConfirmationDate))
                            .ForMember(d => d.FormState, o => o.UseValue(0))
                            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                            .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                            .ForMember(d => d.FormHdrID, o => o.MapFrom(s => s.FormHdrID))
                            .ForAllOtherMembers(d => d.Ignore());
                        });
                        var dtoConfirmationFormAHdr = Mapper.Map<DTOModel.ConfirmationFormAHeader>(confirmationFormA);
                        genericRepo.Insert<DTOModel.ConfirmationFormAHeader>(dtoConfirmationFormAHdr);
                        confirmFormAHdrID = dtoConfirmationFormAHdr.FormAHeaderId;
                    }

                    var dtoObjStatus = genericRepo.Get<DTOModel.ConfirmationStatu>(x =>
                    x.EmployeeID == confirmationFormHdr.EmployeeID
                    && x.ProcessId == confirmationFormHdr.ProcessID
                    && x.FormHdrID == confirmationFormHdr.FormHdID
                    && x.IsRejected == false).ToList();

                    if (dtoObjStatus != null && dtoObjStatus.Count > 0)
                    {
                        foreach (var item in dtoObjStatus)
                        {
                            item.IsRejected = true;
                            genericRepo.Update<DTOModel.ConfirmationStatu>(item);
                        }
                    }

                    if (!isSecondRejection)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<DTOModel.ConfirmationStatu, DTOModel.ConfirmationStatu>()
                            .ForMember(d => d.FormHdrID, o => o.UseValue(confirmationFormHdr.FormHdID))
                            .ForMember(d => d.StatusID, o => o.UseValue(0))
                            .ForMember(d => d.EmpProcessAppID, o => o.MapFrom(s => s.EmpProcessAppID))
                            .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                            .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
                            .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
                            .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
                            .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => s.DueConfirmationDate))
                            .ForMember(d => d.FormState, o => o.UseValue(0))
                            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                            .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                            .ForMember(d => d.FormAHeaderID, o => o.UseValue(confirmFormAHdrID))
                            .ForAllOtherMembers(d => d.Ignore());
                        });
                        var dtoConfStatus = Mapper.Map<List<DTOModel.ConfirmationStatu>>(dtoObjStatus);
                        if (dtoConfStatus != null && dtoConfStatus.Count > 0)
                        {
                            genericRepo.AddMultipleEntity<DTOModel.ConfirmationStatu>(dtoConfStatus);
                            flag = true;
                        }
                    }
                }
                else if (confirmationFormHdr.FormTypeID == 2)
                {
                    Model.ConfirmationFormAHdr confirmationFormB = new ConfirmationFormAHdr();
                    confirmationFormB.EmployeeId = confirmationFormHdr.EmployeeID;
                    confirmationFormB.ProcessId = confirmationFormHdr.ProcessID;
                    confirmationFormB.CertificatesReceived = true;
                    confirmationFormB.PoliceVerification = true;
                    confirmationFormB.CreatedBy = confirmationFormHdr.UpdatedBy == null ? 1 : confirmationFormHdr.UpdatedBy.Value;
                    confirmationFormB.CreatedOn = DateTime.Now;
                    DateTime dt;
                    var dueDate = DateTime.TryParse(confirmationFormHdr.DueDate.Value.ToString(), out dt);
                    confirmationFormB.DueConfirmationDate = dt.AddMonths(6);
                    confirmationFormB.FormHdrID = confirmationFormHdr.FormHdID;


                    var dtoConfirmationFormB = genericRepo.Get<DTOModel.ConfirmationFormBHeader>(x => x.EmployeeId == confirmationFormHdr.EmployeeID && x.ProcessId == confirmationFormHdr.ProcessID && x.FormHdrID == confirmationFormHdr.FormHdID).FirstOrDefault();
                    if (dtoConfirmationFormB != null)
                    {
                        confirmationFormB.BranchId = dtoConfirmationFormB.BranchId;
                        confirmationFormB.DesignationId = dtoConfirmationFormB.DesignationId;

                        dtoConfirmationFormB.FormState = (int)ConfirmationFormState.Rejected;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormBHeader>();
                        });
                        var dtoConfirmationB = Mapper.Map<DTOModel.ConfirmationFormBHeader>(dtoConfirmationFormB);
                        genericRepo.Update<DTOModel.ConfirmationFormBHeader>(dtoConfirmationB);
                        flag = true;
                    }

                    if (!isSecondRejection)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.ConfirmationFormAHdr, DTOModel.ConfirmationFormBHeader>()
                            .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                            .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
                            .ForMember(d => d.DesignationId, o => o.MapFrom(s => s.DesignationId))
                             .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                             .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
                             .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
                             .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => s.DueConfirmationDate))
                             .ForMember(d => d.FormState, o => o.UseValue(0))
                            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                            .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                            .ForMember(d => d.FormHdrID, o => o.MapFrom(s => s.FormHdrID))
                            .ForAllOtherMembers(d => d.Ignore());
                        });
                        var dtoConfirmationFormBHdr = Mapper.Map<DTOModel.ConfirmationFormBHeader>(confirmationFormB);
                        genericRepo.Insert<DTOModel.ConfirmationFormBHeader>(dtoConfirmationFormBHdr);
                        confirmFormBHdrID = dtoConfirmationFormBHdr.FormBHeaderId;
                    }
                    var dtoObjStatus = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.EmployeeID == confirmationFormHdr.EmployeeID && x.ProcessId == confirmationFormHdr.ProcessID && x.FormHdrID == confirmationFormHdr.FormHdID && x.IsRejected == false).ToList();
                    if (dtoObjStatus != null && dtoObjStatus.Count > 0)
                    {
                        foreach (var item in dtoObjStatus)
                        {
                            item.IsRejected = true;
                            genericRepo.Update<DTOModel.ConfirmationStatu>(item);
                        }
                    }

                    if (!isSecondRejection)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<DTOModel.ConfirmationStatu, DTOModel.ConfirmationStatu>()
                            .ForMember(d => d.FormHdrID, o => o.UseValue(confirmationFormHdr.FormHdID))
                            .ForMember(d => d.StatusID, o => o.UseValue(0))
                            .ForMember(d => d.EmpProcessAppID, o => o.MapFrom(s => s.EmpProcessAppID))
                            .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                            .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
                            .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
                            .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
                            .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => s.DueConfirmationDate))
                            .ForMember(d => d.FormState, o => o.UseValue(0))
                            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                            .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                             .ForMember(d => d.FormBHeaderID, o => o.UseValue(confirmFormBHdrID))
                            .ForAllOtherMembers(d => d.Ignore());
                        });
                        var dtoConfStatus = Mapper.Map<List<DTOModel.ConfirmationStatu>>(dtoObjStatus);
                        if (dtoConfStatus != null && dtoConfStatus.Count > 0)
                        {
                            genericRepo.AddMultipleEntity<DTOModel.ConfirmationStatu>(dtoConfStatus);
                            flag = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        private bool AddConfirmationClarification(ConfirmationClarification confirmationClarification)
        {
            log.Info($"ConfirmationFormService/AddConfirmationClarification");
            bool flag = false;
            try
            {
                if (confirmationClarification != null)
                {
                    if (confirmationClarification.FormTypeID == 1)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<ConfirmationClarification, DTOModel.ConfirmationClarification>()
                            .ForMember(d => d.FormAHeaderID, o => o.MapFrom(s => s.FormABHeaderID));
                        });
                    }
                    else if (confirmationClarification.FormTypeID == 2)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<ConfirmationClarification, DTOModel.ConfirmationClarification>()
                            .ForMember(d => d.FormBHeaderID, o => o.MapFrom(s => s.FormABHeaderID));
                        });
                    }
                    var dtoNewRecord = Mapper.Map<DTOModel.ConfirmationClarification>(confirmationClarification);
                    genericRepo.Insert<DTOModel.ConfirmationClarification>(dtoNewRecord);

                    if (dtoNewRecord.ClarificationID > 0)
                        flag = true;
                    return flag;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public List<SelectListModel> GetEmployeeFilter(int employeeID)
        {
            try
            {
                var employeeIds = genericRepo.Get<DTOModel.EmployeeProcessApproval>(x =>
                (x.ProcessID == 6 || x.ProcessID == 7) && x.ToDate == null
                && (x.ReportingTo == employeeID || x.ReviewingTo == employeeID || x.AcceptanceAuthority == employeeID)).Select(y => y.EmployeeID).Distinct().ToArray<int>();

                var confirmationHdr = genericRepo.Get<DTOModel.ConfirmationFormHdr>(y => employeeIds.Any(x => x == y.EmployeeID && y.StatusID > 0)).Select(z => z.EmployeeID).ToArray<int>();

                var employeeDetails = genericRepo.Get<DTOModel.tblMstEmployee>(x => confirmationHdr.Any(y => y == x.EmployeeId)).ToList();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.SelectListModel>()
                   .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeId))
                   .ForMember(d => d.value, o => o.MapFrom(s => s.EmployeeCode + "-" + s.Name));
                });
                var employeedetail = Mapper.Map<List<Model.SelectListModel>>(employeeDetails);
                return employeedetail.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public IEnumerable<ConfirmationFormHdr> GetConfirmationFormHdr(ConfirmationFormApprovalFilter filters)
        {
            log.Info($"ConfirmationFormService/GetAppraisalFormHdr/");
            try
            {
                IEnumerable<ConfirmationFormHdr> formHdrs = Enumerable.Empty<ConfirmationFormHdr>();
                List<DTOModel.ConfirmationStatu> dtoFormHdrs = new List<DTOModel.ConfirmationStatu>();

                //var processIDList = genericRepo.Get<DTOModel.EmployeeProcessApproval>(x => (x.ReportingTo == filters.loggedInEmployeeID || x.ReviewingTo == filters.loggedInEmployeeID) && (x.ProcessID == (int)Common.WorkFlowProcess.AppointmentConfirmation || x.ProcessID == (int)Common.WorkFlowProcess.PromotionConfirmation) && x.ToDate == null).Select(z => z.ProcessID).Distinct().ToArray();
                int[] processIDList = { 6, 7 };
                var EmpProcessAppIDList = genericRepo.Get<DTOModel.EmployeeProcessApproval>(x => (x.ReportingTo == filters.loggedInEmployeeID || x.ReviewingTo == filters.loggedInEmployeeID) && (x.ProcessID == (int)Common.WorkFlowProcess.AppointmentConfirmation || x.ProcessID == (int)Common.WorkFlowProcess.PromotionConfirmation) && x.ToDate == null).Select(z => z.EmpProcessAppID).ToArray();

                var appformsHdr = genericRepo.Get<DTOModel.ConfirmationStatu>(x => processIDList.Any(y => y == x.ProcessId) && EmpProcessAppIDList.Any(z => z == x.EmpProcessAppID) && x.StatusID > 0 && x.IsRejected == false);

                //  processIDList = processIDList.ToList().Where(p => appformsHdr.ToList().Any(x => x.ProcessID == p)).ToArray();
                //  EmployeeIDList = EmployeeIDList.ToList().Where(l => appformsHdr.ToList().Any(y => y.EmployeeID == l)).ToArray();

                //   var confDto = genericRepo.GetIQueryable<DTOModel.ConfirmationFormHdr>(x => x.tblMstEmployee.EmployeeProcessApprovals
                //  .Any(y => processIDList.Any( z=> z==y.ProcessID)  && y.ToDate == null && (y.ReportingTo == filters.loggedInEmployeeID || y.ReviewingTo == filters.loggedInEmployeeID || y.AcceptanceAuthority == filters.loggedInEmployeeID))
                //   );


                if (filters != null)
                {
                    dtoFormHdrs = appformsHdr.Where(x =>
                    (filters.selectedEmployeeID == 0 ? (1 > 0) : x.EmployeeID == filters.selectedEmployeeID) &&
                    x.ConfirmationFormHdr.FormTypeID == (filters.selectedFormID == 0 ? x.ConfirmationFormHdr.FormTypeID : filters.selectedFormID) && (filters.selectedProcessID == 0 ? (1 > 0) : x.ProcessId == filters.selectedProcessID)).ToList();

                    if (filters.StatusId.HasValue)
                        dtoFormHdrs = dtoFormHdrs.Where(x => x.StatusID == filters.StatusId.Value).ToList();
                }



                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.ConfirmationStatu, Model.ConfirmationFormHdr>()
                    .ForMember(d => d.FormHdrID, o => o.MapFrom(s => s.FormHdrID))
                    .ForMember(d => d.FormTypeID, o => o.MapFrom(s => s.ConfirmationFormHdr.FormTypeID))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(d => d.EmpName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(d => d.FormTypeName, o => o.MapFrom(s => s.ConfirmationFormHdr.ConfirmationForm.FormName))
                    .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                    .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessId))
                    .ForMember(d => d.EmpProcessAppID, o => o.MapFrom(s => s.EmpProcessAppID))
                    .ForMember(d => d.FormConfirmationType, o => o.MapFrom(s => s.ConfirmationFormHdr.Process.ProcessName))
                    .ForMember(d => d.FormABHeaderID, o => o.MapFrom(s => s.ConfirmationFormHdr.FormTypeID == 1 ? s.FormAHeaderID : s.ConfirmationFormHdr.FormTypeID == 2 ? s.FormBHeaderID : 0))
                    .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.CreatedOn))
                    //.ForMember(d => d.EmpProceeApproval, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => (processIDList.Any(x => x == y.ProcessID) && EmployeeIDList.Any(z => z == y.EmployeeID) && y.ToDate == null))))
                    .ForAllOtherMembers(d => d.Ignore());

                    //cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();
                });
                var result = Mapper.Map<List<Model.ConfirmationFormHdr>>(dtoFormHdrs.ToList());
                var dtoFormHdrList = dtoFormHdrs.ToList();

                for (int i = 0; i < result.Count; i++)
                {
                    result[i].EmpProceeApproval = GetEmpProcessApproval(result[i].EmployeeID, filters.loggedInEmployeeID, result[i].ProcessID, result[i].EmpProcessAppID);
                    //   var getformStatus = dtoFormHdrList[i] .Where(x => x.EmployeeID == result[i].EmployeeID && result[i].EmpProceeApproval == null ? x.EmpProcessAppID == 0 : x.EmpProcessAppID == result[i].EmpProceeApproval.EmpProcessAppID).FirstOrDefault();
                    //  if (getformStatus != null)
                    //  {
                    //    result[i].StatusID = getformStatus.StatusID;
                    // }
                }
                if (result != null)
                    result = result.Where(x => x.StatusID != null).ToList();

                result.ForEach(y => { y.ToShow = GetConfirmationView(y.ProcessID, y.FormTypeID, y.EmployeeID); });
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public EmployeeProcessApproval GetEmpProcessApproval(int employeeID, int loggedInEmployeeID, int processID, int? EmpProcessAppID = null)
        {
            log.Info($"ConfirmationFormService/GetEmpProcessApproval");
            try
            {
                var empProcessDetails = genericRepo.Get<DTOModel.EmployeeProcessApproval>(x => x.EmployeeID == employeeID && (EmpProcessAppID.HasValue ? x.EmpProcessAppID == EmpProcessAppID : (1 > 0)) && (x.ReportingTo == loggedInEmployeeID || x.ReviewingTo == loggedInEmployeeID) && x.ProcessID == processID && x.ToDate == null).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();
                });
                var result = Mapper.Map<Model.EmployeeProcessApproval>(empProcessDetails);
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ConfirmationFormRulesAttributes GetConfirmationFormRulesAttributes(int formID, int employeeID, int processID, int empProcessAppID, int FrmABHdrID)
        {
            log.Info($"ConfirmationFormService/GetFormRulesAttributes{formID},{employeeID}");
            try
            {
                ConfirmationFormRulesAttributes frmAttributes = new ConfirmationFormRulesAttributes();
                #region Get /Set Form Submission Due Dates/

                var currentYr = DateTime.Now.Year;
                var currentDate = DateTime.Now;
                var doj = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeID).Pr_Loc_DOJ;
                #endregion

                if (formID == 1)
                {
                    if (genericRepo.Exists<DTOModel.ConfirmationFormAHeader>(x => x.EmployeeId == employeeID && !x.IsDeleted))
                    {
                        frmAttributes = genericRepo.GetIQueryable<DTOModel.ConfirmationFormAHeader>(x => x.EmployeeId == employeeID && !x.IsDeleted && x.ProcessId == processID && x.FormAHeaderId == FrmABHdrID).OrderByDescending(x => x.DueConfirmationDate).Select(x => new ConfirmationFormRulesAttributes
                        {
                            FormID = 1,
                            FormGroupID = x.FormAHeaderId,
                            //  FormState = x.FormState
                        }).FirstOrDefault();
                        var getStatus = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.EmployeeID == employeeID && x.EmpProcessAppID == empProcessAppID && x.FormAHeaderID == FrmABHdrID && x.IsRejected == false).FirstOrDefault();
                        if (getStatus != null)
                            frmAttributes.FormState = getStatus.StatusID;
                    }
                }
                else if (formID == 2)
                {
                    if (genericRepo.Exists<DTOModel.ConfirmationFormBHeader>(x => x.EmployeeId == employeeID && !x.IsDeleted))
                    {
                        frmAttributes = genericRepo.GetIQueryable<DTOModel.ConfirmationFormBHeader>(x => x.EmployeeId == employeeID && !x.IsDeleted && x.ProcessId == processID && x.FormBHeaderId == FrmABHdrID).OrderByDescending(x => x.DueConfirmationDate).Select(x => new ConfirmationFormRulesAttributes
                        {
                            FormID = 2,
                            FormGroupID = x.FormBHeaderId,
                            //  FormState = x.FormState
                        }).FirstOrDefault();
                        var getStatus = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.EmployeeID == employeeID && x.EmpProcessAppID == empProcessAppID && x.FormBHeaderID == FrmABHdrID && x.IsRejected == false).FirstOrDefault();
                        if (getStatus != null)
                            frmAttributes.FormState = getStatus.StatusID;
                    }
                }
                //frmAttributes.ReviewerSubmissionDate = doj.Value.AddYears(1);
                //frmAttributes.ReportingSubmissionDate = doj.Value.AddYears(1);
                return frmAttributes;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public CommonDetails GetRTRVDetails(int? employeeID)
        {
            log.Info($"ConfirmationFormService/GetRTRVDetails");
            try
            {
                var details = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeID.Value);
                Model.CommonDetails commonDetails = new CommonDetails()
                {
                    Name = details.Name,
                    Designation = details.Designation.DesignationName,
                    Department = details.Department.DepartmentName
                };
                return commonDetails;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public Employee GetEmployeeByEmpID(int employeeID)
        {
            try
            {
                var dtoForms = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == employeeID && !x.IsDeleted && x.DOLeaveOrg == null).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.Employee>()
                   .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeId))
                   .ForMember(d => d.DOJ, o => o.MapFrom(s => s.DOJ))
                   .ForMember(d => d.orderofpromotion, o => o.MapFrom(s => s.orderofpromotion))
                   .ForAllOtherMembers(d => d.Ignore());
                });
                return Mapper.Map<Employee>(dtoForms);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool GetConfirmationView(int processId, int? formTypeId, int employeeId)
        {
            try
            {
                bool flag = false;
                var currentYr = DateTime.Now.Year;
                var currentDate = DateTime.Now.Date;
                var dueconfirmationDate = DateTime.Now.Date;
                if (formTypeId == 1)
                {
                    var details = genericRepo.Get<DTOModel.ConfirmationFormAHeader>(x => x.EmployeeId == employeeId
                    && x.ProcessId == processId).OrderByDescending(x => x.DueConfirmationDate).FirstOrDefault();

                    if (details != null && details.DueConfirmationDate.HasValue)
                    {
                        dueconfirmationDate = details.DueConfirmationDate.Value;
                        // if (currentDate >= dueconfirmationDate.AddDays(-10) && currentDate <= dueconfirmationDate.AddDays(30))
                        flag = true;
                    }
                }
                else
                {
                    var details = genericRepo.Get<DTOModel.ConfirmationFormBHeader>(x => x.EmployeeId == employeeId
                    && x.ProcessId == processId).OrderByDescending(x => x.DueConfirmationDate).FirstOrDefault();
                    if (details != null && details.DueConfirmationDate.HasValue)
                    {

                        dueconfirmationDate = details.DueConfirmationDate.Value;
                        //  if (currentDate >= dueconfirmationDate.AddDays(-10) && currentDate <= dueconfirmationDate.AddDays(30))
                        flag = true;
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<EmployeeConfirmationViewDetails> GetEmployeeConfirmationDetails(EmployeeConfirmationFormFilters filters)
        {
            log.Info($"ConfirmationFormService/GetEmployeeConfirmationDetails");
            try
            {
                filters.FormTypeId = filters.FormTypeId == 0 ? null : filters.FormTypeId;
                var details = empRepo.GetEmployeeConfirmationDetails(filters.FormTypeId, filters.FromDate, filters.ToDate);
                var result = Common.ExtensionMethods.ConvertToList<EmployeeConfirmationViewDetails>(details);
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateEmployeeConfirmationStatus(int formHdrID, int headerId, int formTypeId, int employeeID, int processID)
        {
            log.Info($"ConfirmationFormService/UpdateEmployeeConfirmationStatus");
            try
            {
                bool flag = false;
                var dtoConfirmationHdr = genericRepo.Get<DTOModel.ConfirmationFormHdr>(x => x.FormTypeID == formTypeId && x.FormHdrID == formHdrID).FirstOrDefault();
                if (dtoConfirmationHdr != null)
                {
                    dtoConfirmationHdr.StatusID = 1;
                    dtoConfirmationHdr.UpdatedBy = 1;
                    dtoConfirmationHdr.UpdatedOn = DateTime.Now;

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormHdr>()
                        .ForAllMembers(d => d.Ignore());
                    });
                    var dtoConfirmation = Mapper.Map<DTOModel.ConfirmationFormHdr>(dtoConfirmationHdr);
                    genericRepo.Update<DTOModel.ConfirmationFormHdr>(dtoConfirmation);
                }
                if (formTypeId == 1)
                {
                    var dtoConfirmationFormA = genericRepo.Get<DTOModel.ConfirmationFormAHeader>(x => x.FormAHeaderId == headerId).FirstOrDefault();
                    if (dtoConfirmationFormA != null)
                    {
                        dtoConfirmationFormA.FormState = 1;

                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormAHeader>();
                        });
                        var dtoConfirmationA = Mapper.Map<DTOModel.ConfirmationFormAHeader>(dtoConfirmationFormA);

                        genericRepo.Update<DTOModel.ConfirmationFormAHeader>(dtoConfirmationA);
                        flag = true;
                    }
                }
                else
                {
                    var dtoConfirmationFormB = genericRepo.Get<DTOModel.ConfirmationFormBHeader>(x => x.FormBHeaderId == headerId).FirstOrDefault();
                    if (dtoConfirmationFormB != null)
                    {
                        dtoConfirmationFormB.FormState = 1;

                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormBHeader>();
                        });
                        var dtoConfirmationB = Mapper.Map<DTOModel.ConfirmationFormBHeader>(dtoConfirmationFormB);
                        genericRepo.Update<DTOModel.ConfirmationFormBHeader>(dtoConfirmationB);
                        flag = true;
                    }
                }
                if (flag)
                {
                    var dtoconfStatus = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.FormHdrID == formHdrID && x.IsRejected == false).ToList();
                    if (dtoconfStatus != null && dtoconfStatus.Count > 0)
                    {
                        foreach (var item in dtoconfStatus)
                        {
                            item.StatusID = 1;
                            item.FormState = 1;
                            genericRepo.Update<DTOModel.ConfirmationStatu>(item);
                            flag = true;
                        }
                    }
                }
                var reciverIDList = genericRepo.Get<DTOModel.EmployeeProcessApproval>(y => y.EmployeeID == employeeID && y.ProcessID == processID && y.ToDate == null).ToList();
                var empDetails = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == employeeID && x.DOLeaveOrg == null && !x.IsDeleted).FirstOrDefault();

                for (int i = 0; i < reciverIDList.Count; i++)
                {
                    SendMailMessageToReceiver(reciverIDList[i].ReportingTo, empDetails.EmployeeCode, empDetails.Name);
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public CommonDetails GetAADetails(int? employeeID)
        {
            log.Info($"ConfirmationFormService/GetAADetails");
            try
            {
                var details = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == employeeID.Value).FirstOrDefault();
                if (details != null)
                {
                    Model.CommonDetails commonDetails = new CommonDetails()
                    {
                        Name = details.Name,
                        Designation = details.Designation.DesignationName
                    };
                    return commonDetails;
                }
                else
                {
                    Model.CommonDetails commonDetails = new CommonDetails()
                    {
                        Name = string.Empty,
                        Designation = string.Empty
                    };
                    return commonDetails;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool PostPersonalSectionRemarks(ConfirmationPRSectionRemarks confirmationPRRemark)
        {
            log.Info($"ConfirmationFormService/PostPersonalSectionRemarks");
            bool flag = false;
            try
            {
                var dtoConfirmationHdr = genericRepo.Get<DTOModel.ConfirmationFormHdr>(x => x.ProcessID == confirmationPRRemark.ProcessID && x.FormHdrID == confirmationPRRemark.FormHdID && x.EmployeeID == confirmationPRRemark.EmployeeID).FirstOrDefault();
                if (dtoConfirmationHdr != null)
                {
                    //dtoConfirmationHdr.StatusID = confirmationPRRemark.StatusID;
                    //Mapper.Initialize(cfg =>
                    //{
                    //    cfg.CreateMap<ConfirmationPRSectionRemarks, DTOModel.ConfirmationFormHdr>();
                    //});
                    //var result = Mapper.Map<DTOModel.ConfirmationFormHdr>(dtoConfirmationHdr);
                    //genericRepo.Update<DTOModel.ConfirmationFormHdr>(result);

                    if (confirmationPRRemark.ProcessID == (int)Common.WorkFlowProcess.AppointmentConfirmation && confirmationPRRemark.PRSubmit)
                    {
                        var dtoEmployee = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == confirmationPRRemark.EmployeeID).FirstOrDefault();
                        if (dtoEmployee != null)
                        {
                            dtoEmployee.ConfirmationDate = confirmationPRRemark.DueDate;
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<ConfirmationPRSectionRemarks, DTOModel.tblMstEmployee>();
                            });
                            var resulttblmst = Mapper.Map<DTOModel.tblMstEmployee>(dtoEmployee);
                            genericRepo.Update<DTOModel.tblMstEmployee>(resulttblmst);

                        }
                    }
                    flag = true;
                }

                if (confirmationPRRemark.FormTypeID == 1)
                {
                    var dtoConfirmationFormA = genericRepo.Get<DTOModel.ConfirmationFormAHeader>(x => x.FormAHeaderId == confirmationPRRemark.FormHeaderID && x.FormHdrID == confirmationPRRemark.FormHdID && x.EmployeeId == confirmationPRRemark.EmployeeID).FirstOrDefault();
                    if (dtoConfirmationFormA != null)
                    {
                        dtoConfirmationFormA.PersonalSectionRemark = confirmationPRRemark.PersonalSectionRemark;
                        dtoConfirmationFormA.PRSubmit = confirmationPRRemark.PRSubmit;
                        dtoConfirmationFormA.FormState = confirmationPRRemark.StatusID;
                        dtoConfirmationFormA.FileNo = confirmationPRRemark.FileNo;
                        dtoConfirmationFormA.GmEmailID = confirmationPRRemark.GMEmailID;
                        dtoConfirmationFormA.GeneralManager = confirmationPRRemark.GeneralManager;
                        dtoConfirmationFormA.ManagingDirector = confirmationPRRemark.GMDesignation;
                        dtoConfirmationFormA.EMail1 = confirmationPRRemark.EmailID1;
                        dtoConfirmationFormA.EMail2 = confirmationPRRemark.EmailID2;
                        dtoConfirmationFormA.EMail3 = confirmationPRRemark.EmailID3;
                        dtoConfirmationFormA.EMail4 = confirmationPRRemark.EmailID4;
                        dtoConfirmationFormA.EMail5 = confirmationPRRemark.EmailID5;
                        dtoConfirmationFormA.DVHeadEmpCode = confirmationPRRemark.DVHEmployeeCode;
                        dtoConfirmationFormA.UpdatedBy = confirmationPRRemark.UpdatedBy;
                        dtoConfirmationFormA.UpdatedOn = DateTime.Now;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<ConfirmationPRSectionRemarks, DTOModel.ConfirmationFormAHeader>();
                        });
                        var result = Mapper.Map<DTOModel.ConfirmationFormAHeader>(dtoConfirmationFormA);
                        genericRepo.Update<DTOModel.ConfirmationFormAHeader>(result);
                        if (result.FormAHeaderId > 0)
                        {
                            flag = true;
                            ConfirmationFormHdr hdr = new ConfirmationFormHdr()
                            {
                                EmployeeID = dtoConfirmationFormA.EmployeeId,
                                ProcessID = dtoConfirmationFormA.ProcessId,
                                StatusID = confirmationPRRemark.StatusID
                            };
                            //  SendMailMessageToReceiver(hdr);
                        }
                    }
                }
                else
                {
                    var dtoConfirmationFormB = genericRepo.Get<DTOModel.ConfirmationFormBHeader>(x => x.FormBHeaderId == confirmationPRRemark.FormHeaderID && x.FormHdrID == confirmationPRRemark.FormHdID && x.EmployeeId == confirmationPRRemark.EmployeeID).FirstOrDefault();
                    if (dtoConfirmationFormB != null)
                    {
                        dtoConfirmationFormB.PersonalSectionRemark = confirmationPRRemark.PersonalSectionRemark;
                        dtoConfirmationFormB.PRSubmit = confirmationPRRemark.PRSubmit;
                        dtoConfirmationFormB.FormState = confirmationPRRemark.StatusID;
                        dtoConfirmationFormB.FileNo = confirmationPRRemark.FileNo;
                        dtoConfirmationFormB.GmEmailID = confirmationPRRemark.GMEmailID;
                        dtoConfirmationFormB.GeneralManager = confirmationPRRemark.GeneralManager;
                        dtoConfirmationFormB.ManagingDirector = confirmationPRRemark.GMDesignation;
                        dtoConfirmationFormB.EMail1 = confirmationPRRemark.EmailID1;
                        dtoConfirmationFormB.EMail2 = confirmationPRRemark.EmailID2;
                        dtoConfirmationFormB.EMail3 = confirmationPRRemark.EmailID3;
                        dtoConfirmationFormB.EMail4 = confirmationPRRemark.EmailID4;
                        dtoConfirmationFormB.EMail5 = confirmationPRRemark.EmailID5;
                        dtoConfirmationFormB.DVHeadEmpCode = confirmationPRRemark.DVHEmployeeCode;
                        dtoConfirmationFormB.UpdatedBy = confirmationPRRemark.UpdatedBy;
                        dtoConfirmationFormB.UpdatedOn = DateTime.Now;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<ConfirmationPRSectionRemarks, DTOModel.ConfirmationFormBHeader>();
                        });
                        var result = Mapper.Map<DTOModel.ConfirmationFormBHeader>(dtoConfirmationFormB);
                        genericRepo.Update<DTOModel.ConfirmationFormBHeader>(result);
                        if (result.FormBHeaderId > 0)
                            flag = true;
                    }
                }
                if (flag && confirmationPRRemark.StatusID == (int)ConfirmationFormState.Rejected && confirmationPRRemark.PRSubmit)
                {
                    ExtendConfirmationForm(confirmationPRRemark);
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SendMailMessageToReceiver(int recieverID, string empCode, string empName)
        {
            log.Info($"ConfirmationFormService/SendMailMessageToReceiver/recieverID={recieverID}");
            bool flag = false;
            try
            {
                StringBuilder emailBody = new StringBuilder("");
                var recieverMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == recieverID && !x.IsDeleted).Select(x => new
                {
                    MobileNo = x.MobileNo,
                    OfficailEmail = x.OfficialEmail,
                    EmployeeCode=x.EmployeeCode
                }).FirstOrDefault();

                if (!string.IsNullOrEmpty(recieverMail.EmployeeCode))
                {
                    PushNotification notification = new PushNotification
                    {
                        UserName = recieverMail.EmployeeCode,
                        Title = "Confirmation Intimation",
                        Message = $"Dear Sir/Madam, This is to intimate that you have received the Confirmation Form Approval, for employee { empCode + "-" + empName} for further evaluation."

                    };
                    Task notif = Task.Run(() => FirebaseTopicNotification(notification));
                }

                if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                {
                    emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                 + "<p>This is to intimate that you have received the confirmation Form of <b>" + empCode + "-" + empName + "</b> for further evaluation.</p>"
                 + "<p>Requesting your support for timely completion of the process by logging into: </p>"
                 + "<p>http://182.74.122.83/nafedhrms</p>"
                 + "<p>Please get in touch with Personnel Section in case of any disconnect/clarification required. </p> </div>");

                    emailBody.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");

                    Common.EmailMessage message = new Common.EmailMessage();
                    message.To = recieverMail.OfficailEmail;
                    message.Body = emailBody.ToString();
                    message.Subject = "NAFED-HRMS : Confirmation Intimation";

                    Task t2 = Task.Run(() => SendEmail(message));
                }
                else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                {
                    emailBody.AppendFormat("Dear Sir/Madam,"
                  + "This is to intimate that you have received the confirmation Form of " + empCode + "-" + empName + " for further evaluation."
                  + "Requesting your support for timely completion of the process by logging into:"
                  + "http://182.74.122.83/nafedhrms"
                  + "Please get in touch with Personnel Section in case of any disconnect/clarification required.");
                    emailBody.AppendFormat("Regards, ENAFED");

                    Task t1 = Task.Run(() => SendMessage(recieverMail.MobileNo, emailBody.ToString()));
                }
                flag = true;
            }
            catch
            {
                flag = true;
            }
            return flag;
        }

        void SendEmail(Common.EmailMessage message)
        {
            try
            {
                var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                message.From = "NAFED HRMS <" + emailsetting.ToEmail + ">";
                message.UserName = emailsetting.UserName;
                message.Password = emailsetting.Password;
                message.SmtpClientHost = emailsetting.Server;
                message.SmtpPort = emailsetting.Port;
                message.enableSSL = emailsetting.SSLStatus;
                message.HTMLView = true;
                message.FriendlyName = "NAFED";
                Common.EmailHelper.SendEmail(message);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SendMessage(string mobileNo, string message)
        {
            try
            {
                SMSConfiguration smssetting = new SMSConfiguration();
                var smssettings = genericRepo.Get<DTOModel.SMSConfiguration>().FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SMSConfiguration, Model.SMSConfiguration>();

                });
                smssetting = Mapper.Map<Model.SMSConfiguration>(smssettings);

                string msgRecepient = mobileNo.Length == 10 ? "91" + mobileNo : mobileNo;
                Common.SMSParameter sms = new Common.SMSParameter();

                sms.MobileNo = msgRecepient;
                sms.Message = message;
                sms.URL = smssetting.SMSUrl;
                sms.User = smssetting.UserName;
                sms.Password = smssetting.Password;
                return Common.SMSHelper.SendSMS(sms);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return false;
            }
        }

        public List<ConfirmationClarification> GetConfirmationClarification(int formHdrID, int empProcessAppID, int formTypeId, int childHeaderId)
        {
            log.Info($"ConfirmationFormService/GetConfirmationClarification");
            try
            {
                var empClarification = genericRepo.Get<DTOModel.ConfirmationClarification>(x => x.FormHdrID == formHdrID && x.EmpProcessAppID == empProcessAppID && (formTypeId == 1 ? x.FormAHeaderID == childHeaderId : x.FormBHeaderID == childHeaderId)).ToList();

                if (empClarification == null)
                    return new List<ConfirmationClarification>();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.ConfirmationClarification, Model.ConfirmationClarification>();
                });
                var result = Mapper.Map<List<Model.ConfirmationClarification>>(empClarification);
                return result.OrderByDescending(x => x.CreatedOn).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SendMailMessageToReceiver(ConfirmationFormHdr model)
        {
            log.Info($"ConfirmationFormService/SendMailMessageToReceiver/model={model}");
            bool flag = false;
            try
            {
                StringBuilder emailBody = new StringBuilder("");
                int recieverID = 0;
                if (model.StatusID > 6)
                    recieverID = model.EmployeeID;
                else if (model.loggedInEmpID == model.ReportingTo && model.ApprovalHierarchy == 1)
                    recieverID = model._ProcessWorkFlow.ReceiverID.Value;
                else if (model.loggedInEmpID == model.ReviewingTo)
                {
                    var gethrmail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.DesignationID == 80 && x.DepartmentID == 36 && !x.IsDeleted && x.DOLeaveOrg == null).FirstOrDefault();
                    if (gethrmail != null)
                        recieverID = gethrmail.EmployeeId;
                    else
                        recieverID = model.ReportingTo.Value;
                }

                var recieverMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == recieverID && !x.IsDeleted).Select(x => new
                {
                    MobileNo = x.MobileNo,
                    OfficailEmail = x.OfficialEmail,
                    EmployeeCode=x.EmployeeCode
                }).FirstOrDefault();


                if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                {
                    if (model.StatusID > 6)
                    {
                        if (model.StatusID == 7)
                        {
                            emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                        + "<p>Your probation period is hereby extended for a further period of six months w.e.f. " + model.CreatedOn + "");
                        }
                        else if (model.StatusID == 8)
                        {
                            emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                        + "<p>You are hereby confirmed on the existing post, with effect from" + model.CreatedOn + "");
                        }

                    }
                    else if (model.loggedInEmpID == model.ReportingTo && model.ApprovalHierarchy == 1)
                    {
                        if (!string.IsNullOrEmpty(recieverMail.EmployeeCode))
                        {
                            PushNotification notification = new PushNotification
                            {
                                UserName = recieverMail.EmployeeCode,
                                Title = "Confirmation Approval",
                                Message = $"Dear Sir/Madam, This is to intimate that you have received the Confirmation Form Approval, for employee { model.confirmationFormDetail.EmployeeCode + "-" + model.confirmationFormDetail.EmployeeName} for further evaluation."

                            };
                            Task notif = Task.Run(() => FirebaseTopicNotification(notification));
                        }

                        emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                    + "<p>This is to intimate that you have received the <b>" + (model.FormTypeID == 1 ? "Form A" : "Form B") + "</b> confirmation Form of <b>" + model.confirmationFormDetail.EmployeeCode + "-" + model.confirmationFormDetail.EmployeeName + "</b> for further evaluation.</p>"
                    + "<p>Requesting your support for timely completion of the process by logging into: </p>"
                    + "<p>http://182.74.122.83/nafedhrms</p>"
                    + "<p>Please get in touch with Personnel Section in case of any disconnect/clarification required. </p> </div>");
                    }
                    else if (model.loggedInEmpID == model.ReviewingTo)
                    {
                        if (model.confirmationFormDetail.ConfirmationRecommended)
                        {
                            emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                        + "<p>This is to intimate that the <b>" + (model.FormTypeID == 1 ? "Form A" : "Form B") + "</b> confirmation Form of <b>" + model.confirmationFormDetail.EmployeeCode + "-" + model.confirmationFormDetail.EmployeeName + "</b> has been done.</p>"
                        + "<p>http://182.74.122.83/nafedhrms</p>"
                        + "<p>Please get in touch with Personnel Section in case of any disconnect/clarification required. </p> </div>");
                        }
                        else if (!model.confirmationFormDetail.ConfirmationRecommended && !model.confirmationFormDetail.ConfirmationClarification)
                        {
                            emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                       + "<p>This is to intimate that the <b>" + (model.FormTypeID == 1 ? "Form A" : "Form B") + "</b> confirmation Form of <b>" + model.confirmationFormDetail.EmployeeCode + "-" + model.confirmationFormDetail.EmployeeName + "</b> has been done.</p>"
                       + "<p>http://182.74.122.83/nafedhrms</p>"
                       + "<p>Please get in touch with Personnel Section in case of any disconnect/clarification required. </p> </div>");
                        }
                        else if (model.confirmationFormDetail.ConfirmationClarification)
                        {
                            emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                    + "<p>This is to intimate that you have received the <b>" + (model.FormTypeID == 1 ? "Form A" : "Form B") + "</b> confirmation Form of <b>" + model.confirmationFormDetail.EmployeeCode + "-" + model.confirmationFormDetail.EmployeeName + "</b> for <b>" + model.confirmationFormDetail.ReviewingOfficerComment + "</b> clarification.</p>"
                    + "<p>Requesting your support for timely completion of the process by logging into: </p>"
                    + "<p>http://182.74.122.83/nafedhrms</p>"
                    + "<p>Please get in touch with Personnel Section in case of any disconnect/clarification required. </p> </div>");
                        }
                    }

                    emailBody.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");
                    Common.EmailMessage message = new Common.EmailMessage();
                    message.To = recieverMail.OfficailEmail;
                    message.Body = emailBody.ToString();
                    if (model.StatusID > 6)
                    {
                        if (model.StatusID == 7)
                            message.Subject = "NAFED-HRMS : Confirmation Extended";
                        else if (model.StatusID == 8)
                            message.Subject = "NAFED-HRMS : Confirmation Confirmed";
                    }
                    else if ((model.loggedInEmpID == model.ReportingTo && model.confirmationFormDetail.ConfirmationRecommendedReporting) || (model.loggedInEmpID == model.ReviewingTo && model.confirmationFormDetail.ConfirmationRecommended))
                        message.Subject = "NAFED-HRMS : Confirmation Form Accepted";
                    else if ((model.loggedInEmpID == model.ReportingTo && !model.confirmationFormDetail.ConfirmationRecommendedReporting) || (model.loggedInEmpID == model.ReviewingTo && !model.confirmationFormDetail.ConfirmationRecommended && !model.confirmationFormDetail.ConfirmationClarification))
                        message.Subject = "NAFED-HRMS : Confirmation Form Rejected";
                    else if (model.loggedInEmpID == model.ReviewingTo && model.confirmationFormDetail.ConfirmationClarification)
                        message.Subject = "NAFED-HRMS : Confirmation Form Clarification Sought";

                    Task t2 = Task.Run(() => SendEmail(message));
                }
                else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                {
                    if (model.loggedInEmpID == model.ReportingTo && model.ApprovalHierarchy == 1)
                    {
                        emailBody.AppendFormat("Dear Sir/Madam,"
                  + "This is to intimate that you have received the <b>" + (model.FormTypeID == 1 ? "Form A" : "Form B") + "</b> confirmation Form of " + model.confirmationFormDetail.EmployeeCode + "-" + model.confirmationFormDetail.EmployeeName + " for further evaluation."
                  + "Requesting your support for timely completion of the process by logging into:"
                  + "http://182.74.122.83/nafedhrms"
                  + "Please get in touch with Personnel Section in case of any disconnect/clarification required.");
                    }
                    else if (model.loggedInEmpID == model.ReviewingTo)
                    {
                        if (model.confirmationFormDetail.ConfirmationRecommended)
                        {
                            emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                        + "<p>This is to intimate that the <b>" + (model.FormTypeID == 1 ? "Form A" : "Form B") + "</b> confirmation Form of <b>" + model.confirmationFormDetail.EmployeeCode + "-" + model.confirmationFormDetail.EmployeeName + "</b> has been done.</p>"
                        + "<p>http://182.74.122.83/nafedhrms</p>"
                        + "<p>Please get in touch with Personnel Section in case of any disconnect/clarification required. </p> </div>");
                        }
                        else if (!model.confirmationFormDetail.ConfirmationRecommended)
                        {
                            emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                       + "<p>This is to intimate that the <b>" + (model.FormTypeID == 1 ? "Form A" : "Form B") + "</b> confirmation Form of <b>" + model.confirmationFormDetail.EmployeeCode + "-" + model.confirmationFormDetail.EmployeeName + "</b> has been done.</p>"
                       + "<p>http://182.74.122.83/nafedhrms</p>"
                       + "<p>Please get in touch with Personnel Section in case of any disconnect/clarification required. </p> </div>");
                        }
                        else if (model.confirmationFormDetail.ConfirmationClarification)
                        {
                            emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                    + "<p>This is to intimate that you have received the <b>" + (model.FormTypeID == 1 ? "Form A" : "Form B") + "</b> confirmation Form of <b>" + model.confirmationFormDetail.EmployeeCode + "-" + model.confirmationFormDetail.EmployeeName + "</b> for <b>" + model.confirmationFormDetail.ReviewingOfficerComment + "</b> clarification.</p>"
                    + "<p>Requesting your support for timely completion of the process by logging into: </p>"
                    + "<p>http://182.74.122.83/nafedhrms</p>"
                    + "<p>Please get in touch with Personnel Section in case of any disconnect/clarification required. </p> </div>");
                        }
                    }
                    emailBody.AppendFormat("Regards, ENAFED");
                    Task t1 = Task.Run(() => SendMessage(recieverMail.MobileNo, emailBody.ToString()));
                }
                flag = true;
            }
            catch
            {
                flag = true;
            }
            return flag;
        }

        public ConfirmationPRSectionRemarks GetConfirmationDetails(int formHdrID, int formTypeId, int employeeID, int childHdrID)
        {
            log.Info($"ConfirmationFormService/GetConfirmationDetails");
            try
            {
                ConfirmationPRSectionRemarks cnfRemark = new ConfirmationPRSectionRemarks();
                var getconfdtl = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.FormHdrID == formHdrID && x.EmployeeID == employeeID && (formTypeId == 1 ? x.FormAHeaderID == childHdrID : x.FormBHeaderID == childHdrID)).ToList();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.ConfirmationStatu, ConfirmationFormHdr>()
                    .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                    .ForMember(d => d.ReportingTo, o => o.MapFrom(s => s.EmployeeProcessApproval.tblMstEmployee2.EmployeeId))
                    .ForMember(d => d.ReviewingTo, o => o.MapFrom(s => s.EmployeeProcessApproval.tblMstEmployee3.EmployeeId))
                    .ForMember(d => d.ReportingName, o => o.MapFrom(s => s.EmployeeProcessApproval.tblMstEmployee2.Name))
                    .ForMember(d => d.ReviewingName, o => o.MapFrom(s => s.EmployeeProcessApproval.tblMstEmployee3.Name))
                    .ForMember(d => d.ReportingRemark, o => o.MapFrom(s => s.ReportingOfficerComment))
                    .ForMember(d => d.ReviewerRemark, o => o.MapFrom(s => s.ReviewingOfficerComment))
                    .ForMember(d => d.EmpProcessAppID, o => o.MapFrom(s => s.EmpProcessAppID))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessId))
                    .ForMember(d => d.FormHdrID, o => o.MapFrom(s => s.FormHdrID))
                    .ForAllOtherMembers(d => d.Ignore());

                });
                var dtoObj = Mapper.Map<List<ConfirmationFormHdr>>(getconfdtl);
                cnfRemark.confHdrList = dtoObj;
                if (formTypeId == 1)
                {
                    var getPersonnelRemrk = genericRepo.Get<DTOModel.ConfirmationFormAHeader>(x => x.FormHdrID == formHdrID && x.EmployeeId == employeeID && x.FormAHeaderId == childHdrID);
                    if (getPersonnelRemrk?.Count() > 0)
                    {
                        cnfRemark.PersonalSectionRemark = getPersonnelRemrk.FirstOrDefault().PersonalSectionRemark;
                        cnfRemark.PRSubmit = getPersonnelRemrk.FirstOrDefault().PRSubmit.HasValue ? (bool)getPersonnelRemrk.FirstOrDefault().PRSubmit : false;
                        cnfRemark.FileNo = getPersonnelRemrk.FirstOrDefault().FileNo;
                        cnfRemark.GMEmailID = getPersonnelRemrk.FirstOrDefault().GmEmailID;
                        cnfRemark.EmailID1 = getPersonnelRemrk.FirstOrDefault().EMail1;
                        cnfRemark.EmailID2 = getPersonnelRemrk.FirstOrDefault().EMail2;
                        cnfRemark.EmailID3 = getPersonnelRemrk.FirstOrDefault().EMail3;
                        cnfRemark.EmailID4 = getPersonnelRemrk.FirstOrDefault().EMail4;
                        cnfRemark.EmailID5 = getPersonnelRemrk.FirstOrDefault().EMail5;
                        cnfRemark.GeneralManager = getPersonnelRemrk.FirstOrDefault().GeneralManager;
                        cnfRemark.GMDesignation = getPersonnelRemrk.FirstOrDefault().ManagingDirector;
                        cnfRemark.DVHEmployeeCode = getPersonnelRemrk.FirstOrDefault().DVHeadEmpCode;
                        cnfRemark.StatusID = getPersonnelRemrk.FirstOrDefault().FormState.Value;
                    }
                }
                else if (formTypeId == 2)
                {
                    var getPersonnelRemrk = genericRepo.Get<DTOModel.ConfirmationFormBHeader>(x => x.FormHdrID == formHdrID && x.EmployeeId == employeeID && x.FormBHeaderId == childHdrID);
                    if (getPersonnelRemrk?.Count() > 0)
                    {
                        cnfRemark.PersonalSectionRemark = getPersonnelRemrk.FirstOrDefault().PersonalSectionRemark;
                        cnfRemark.PRSubmit = getPersonnelRemrk.FirstOrDefault().PRSubmit.HasValue ? (bool)getPersonnelRemrk.FirstOrDefault().PRSubmit : false;
                        cnfRemark.FileNo = getPersonnelRemrk.FirstOrDefault().FileNo;
                        cnfRemark.GMEmailID = getPersonnelRemrk.FirstOrDefault().GmEmailID;
                        cnfRemark.EmailID1 = getPersonnelRemrk.FirstOrDefault().EMail1;
                        cnfRemark.EmailID2 = getPersonnelRemrk.FirstOrDefault().EMail2;
                        cnfRemark.EmailID3 = getPersonnelRemrk.FirstOrDefault().EMail3;
                        cnfRemark.EmailID4 = getPersonnelRemrk.FirstOrDefault().EMail4;
                        cnfRemark.EmailID5 = getPersonnelRemrk.FirstOrDefault().EMail5;
                        cnfRemark.DVHEmployeeCode = getPersonnelRemrk.FirstOrDefault().DVHeadEmpCode;
                        cnfRemark.GeneralManager = getPersonnelRemrk.FirstOrDefault().GeneralManager;
                        cnfRemark.GMDesignation = getPersonnelRemrk.FirstOrDefault().ManagingDirector;
                        cnfRemark.StatusID = getPersonnelRemrk.FirstOrDefault().FormState.Value;
                    }
                }

                return cnfRemark;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ConfirmationPRSectionRemarks GetConfirmationDtlForOrdrRpt(int empID, int hdrID, int formHdrID)
        {
            log.Info($"ConfirmationFormService/GetConfirmationDtlForOrdrRpt/empID={empID}/hdrID={hdrID}/formHdrID={formHdrID}");
            var getData = confirmRepo.GetConfirmationDtlForOrdrRpt(empID, hdrID, formHdrID);

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.GetConfirmationDtlForOrdrRpt_Result, ConfirmationPRSectionRemarks>()
                .ForMember(d => d.Employee, o => o.MapFrom(s => s.Employee))
                .ForMember(d => d.Designation, o => o.MapFrom(s => s.Designation))
                .ForMember(d => d.DueDate, o => o.MapFrom(s => s.DueDate))
                .ForMember(d => d.Branch, o => o.MapFrom(s => s.BranchName))
                .ForMember(d => d.Department, o => o.MapFrom(s => s.DepartmentName))
                .ForMember(d => d.FileNo, o => o.MapFrom(s => s.FileNo))
                .ForMember(d => d.GeneralManager, o => o.MapFrom(s => s.GeneralManager))
                .ForMember(d => d.GMDesignation, o => o.MapFrom(s => s.ManagingDirector))
                .ForMember(d => d.TitleID, o => o.MapFrom(s => s.TitleID))
                .ForMember(d => d.GenderID, o => o.MapFrom(s => s.GenderID))
                .ForMember(d => d.GMEmailID, o => o.MapFrom(s => s.GmEmailID))
                .ForMember(d => d.EmailID1, o => o.MapFrom(s => s.EMail1))
                .ForMember(d => d.EmailID2, o => o.MapFrom(s => s.EMail2))
                .ForMember(d => d.EmailID3, o => o.MapFrom(s => s.EMail3))
                .ForMember(d => d.EmailID4, o => o.MapFrom(s => s.EMail4))
                .ForMember(d => d.EmailID5, o => o.MapFrom(s => s.EMail5))
                .ForMember(d => d.PayScale, o => o.MapFrom(s => s.payscale))
                 .ForMember(d => d.OrderDate, o => o.MapFrom(s => s.OrderDate))
                .ForAllOtherMembers(d => d.Ignore());
            });
            var dtoList = Mapper.Map<ConfirmationPRSectionRemarks>(getData);
            if (dtoList == null)
                dtoList = new ConfirmationPRSectionRemarks();

            return dtoList;
        }

        #region Send Mail
        public bool SendMailForOrderReport(ConfirmationPRSectionRemarks confDtlforReport, string ordrRptPath)
        {
            log.Info($"ConfirmationFormService/SendMailForOrderReport");
            bool flag = false;
            try
            {
                var recieverMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == confDtlforReport.EmployeeID && !x.IsDeleted).Select(x => new
                {
                    OfficailEmail = x.OfficialEmail
                }).FirstOrDefault();

                var gethrmail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.DesignationID == 80 && x.DepartmentID == 36 && !x.IsDeleted && x.DOLeaveOrg == null).FirstOrDefault();

                StringBuilder emailBody = new StringBuilder("");
                if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                {

                    if (confDtlforReport.StatusID == 7)
                    {
                        emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                    + "<p>Your probation period is hereby extended for a further period of six months w.e.f. " + confDtlforReport.DueDate.Value.AddMonths(6).ToString("dd-MMM-yyy") + "");
                    }
                    else if (confDtlforReport.StatusID == 8)
                    {
                        emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                    + "<p>You are hereby confirmed on the existing post, with effect from" + confDtlforReport.DueDate.Value.ToString("dd-MMM-yyy") + "");
                    }

                    var ccMail = string.Empty;

                    if (!string.IsNullOrEmpty(confDtlforReport.EmailID1))
                        ccMail = confDtlforReport.EmailID1;
                    if (!string.IsNullOrEmpty(confDtlforReport.EmailID2))
                        ccMail = ccMail + ',' + confDtlforReport.EmailID2;
                    if (!string.IsNullOrEmpty(confDtlforReport.EmailID3))
                        ccMail = ccMail + ',' + confDtlforReport.EmailID3;
                    if (!string.IsNullOrEmpty(confDtlforReport.EmailID4))
                        ccMail = ccMail + ',' + confDtlforReport.EmailID4;
                    if (!string.IsNullOrEmpty(confDtlforReport.EmailID5))
                        ccMail = ccMail + ',' + confDtlforReport.EmailID5;
                    if (gethrmail != null && string.IsNullOrEmpty(gethrmail.OfficialEmail))
                        ccMail = ccMail + ',' + gethrmail.OfficialEmail;

                    Common.EmailMessage message = new Common.EmailMessage();
                    message.To = recieverMail.OfficailEmail;
                    message.CC = ccMail;
                    message.Body = emailBody.ToString();
                    if (confDtlforReport.ProcessID == 7)
                        message.Subject = "NAFED - Promotion Confirmation";
                    else
                        message.Subject = "NAFED - Appointment Confirmation";

                    Task t1 = Task.Run(() => GetEmailConfiguration(message, ordrRptPath));
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

        private void GetEmailConfiguration(Common.EmailMessage messageObj, string ordrRptPath)
        {
            try
            {
                var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                Common.EmailMessage message = new Common.EmailMessage();
                message.To = messageObj.To;
                message.CC = messageObj.CC;
                message.From = "NAFED HRMS <" + emailsetting.ToEmail + ">";
                message.Subject = messageObj.Subject;
                message.UserName = emailsetting.UserName;
                message.Password = emailsetting.Password;
                message.SmtpClientHost = emailsetting.Server;
                message.SmtpPort = emailsetting.Port;
                message.enableSSL = emailsetting.SSLStatus;
                message.Body = messageObj.Body;
                message.HTMLView = true;
                message.Attachments = new List<Common.MailAttachment>() { new Common.MailAttachment { FileName = ordrRptPath, Content = null } };
                message.FriendlyName = "NAFED";
                Common.EmailHelper.SendEmail(message);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        public dynamic GetEmployeeNameDesignation(string empCode)
        {
            log.Info($"ConfirmationFormService/GetEmployeeNameDesignation/empCode={empCode}");
            try
            {
                dynamic empData =null;
                empData = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeCode == empCode && !x.IsDeleted && x.DOLeaveOrg == null).Select(x => new
                {
                    name = x.Name,
                    desg = x.Designation.DesignationName
                }).FirstOrDefault();
               
                return empData;
                
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
