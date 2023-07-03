using AutoMapper;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Nafed.MicroPay.Services
{
    public class StaffLeaveDetailsService : BaseService, IStaffLeaveDetailsService
    {
        private readonly IGenericRepository genericRepo;

        public StaffLeaveDetailsService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<EmployeeLeave> GetStaffLeaveDetailsList(Model.EmployeeLeave empLeave, int? reportingTo, int? revwingTo, int empId)
        {
            log.Info($"StaffLeaveDetailsService/GetStaffLeaveDetailsList");
            IEnumerable<Nafed.MicroPay.Data.Models.EmployeeLeave> result = Enumerable.Empty<Nafed.MicroPay.Data.Models.EmployeeLeave>();
            try
            {
                if (empLeave.StatusID != 2)
                {
                    result = genericRepo.Get<Data.Models.EmployeeLeave>(x => x.IsDeleted == false
                && (empLeave.BranchId > 0 ? x.tblMstEmployee.BranchID == empLeave.BranchId : 1 > 0)
                && (empLeave.StatusID > 0 ? x.StatusID == empLeave.StatusID : 1 > 0)
                &&
                (
                 x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.ReportingTo == empId)
                || x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.ReviewingTo == empId)
                || x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.AcceptanceAuthority == empId)
                )
                );
                }
                else
                {
                    int[] statusIds = { 2, 4, 7 };
                    result = genericRepo.Get<Data.Models.EmployeeLeave>(x => x.IsDeleted == false
                    && (empLeave.BranchId > 0 ? x.tblMstEmployee.BranchID == empLeave.BranchId : 1 > 0)
                    && (statusIds.Any(y => y == x.StatusID))
                    &&
                    (
                     x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.ReportingTo == empId)
                    || x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.ReviewingTo == empId)
                    || x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.AcceptanceAuthority == empId)
                    )
                    );
                }

                //if (empLeave.DateFrom.HasValue && empLeave.DateTo.HasValue)
                //{
                //    result = result.Where(x => (x.DateFrom >= empLeave.DateFrom.Value.Date && x.DateFrom <= empLeave.DateTo.Value.Date) || (x.DateTo >= empLeave.DateFrom.Value.Date && x.DateTo <= empLeave.DateTo.Value.Date));
                //    // result = result.Where(x => x.CreatedOn.Date >= empLeave.DateFrom.Value.Date && x.CreatedOn.Date <= empLeave.DateTo.Value.Date);
                //}

                if (empLeave.DateFrom.HasValue && empLeave.DateTo.HasValue)
                {
                    result = result.Where(x => x.CreatedOn.Date >= empLeave.DateFrom.Value.Date && x.CreatedOn.Date <= empLeave.DateTo.Value.Date);
                }
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.EmployeeLeave, Model.EmployeeLeave>()
                    .ForMember(c => c.LeaveID, c => c.MapFrom(s => s.LeaveID))
                    .ForMember(c => c.EmployeeCode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.EmployeeName, c => c.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(c => c.DateFrom, c => c.MapFrom(s => s.DateFrom))
                    .ForMember(c => c.DateTo, c => c.MapFrom(s => s.DateTo))
                    .ForMember(c => c.Unit, c => c.MapFrom(s => s.Unit))
                    .ForMember(c => c.Reason, c => c.MapFrom(s => s.Reason))
                    .ForMember(c => c.StatusName, c => c.MapFrom(s => s.LeaveStatu.StatusDesc))
                    .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                    .ForMember(c => c.BranchId, c => c.MapFrom(s => s.tblMstEmployee.BranchID))
                    .ForMember(d => d.Branch, o => o.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                    .ForMember(d => d.ReportingTo, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.EmployeeID == s.EmployeeId && y.ProcessID == 3 && y.ToDate == null).ReportingTo))
                    .ForMember(d => d.ReportingOfficer, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.EmployeeID == s.EmployeeId && y.ProcessID == 3 && y.ToDate == null).ReportingTo))
                    .ForMember(d => d.ReviewerTo, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.EmployeeID == s.EmployeeId && y.ProcessID == 3 && y.ToDate == null).ReviewingTo))
                    .ForMember(d => d.AcceptanceAuthority, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.EmployeeID == s.EmployeeId && y.ProcessID == 3 && y.ToDate == null).AcceptanceAuthority))
                    .ForMember(d => d.DocumentName, o => o.MapFrom(s => s.DocumentName))
                    .ForMember(d => d.ReporotingToRemark, o => o.MapFrom(s => s.ReportingToRemark))
                    .ForMember(d => d.ReviewerToRemark, o => o.MapFrom(s => s.ReviewerToRemark))
                     .ForMember(d => d.AcceptanceAuthorityRemark, o => o.MapFrom(s => s.PurposeOthers))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.tblMstEmployee.EmployeeId))
                     .ForMember(c => c.ApprovalRequiredUpto, c => c.MapFrom(s => s.LeaveCategory.ApprovalRequiredUpto))
                     .ForMember(c => c.LeaveCategoryName, c => c.MapFrom(s => s.LeaveCategory.LeaveCategoryName))
                    .ForAllOtherMembers(d => d.Ignore())
                    ;
                });
                var listEmpLeave = Mapper.Map<List<Model.EmployeeLeave>>(result);
                return listEmpLeave.OrderByDescending(x => x.CreatedOn).ThenBy(x => x.StatusID).ToList();
                //return listEmpLeave;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }


        }
        public List<EmployeeLeave> GetEmployeeLeaveDetailsList(Model.EmployeeLeave empLeave, int? reportingTo, int? revwingTo, int empId)
        {
            log.Info($"StaffLeaveDetailsService/GetStaffLeaveDetailsList");
            IEnumerable<Nafed.MicroPay.Data.Models.EmployeeLeave> result = Enumerable.Empty<Nafed.MicroPay.Data.Models.EmployeeLeave>();
            try
            {
                //result = genericRepo.Get<Data.Models.EmployeeLeave>(x => x.IsDeleted == false

                ////&& (empLeave.BranchId > 0 ? x.tblMstEmployee.BranchID == empLeave.BranchId : 1 > 0)
                ////&& (empLeave.StatusID > 0 ? x.StatusID == empLeave.StatusID : 1 > 0)
                //&& 
                //(             
                // x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.ReportingTo== empId)
                //|| x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.ReviewingTo == empId)
                //|| x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.AcceptanceAuthority == empId)
                //)
                //);

                if (empLeave.StatusID != 2)
                {
                    result = genericRepo.Get<Data.Models.EmployeeLeave>(x => x.IsDeleted == false
                    && (empLeave.BranchId > 0 ? x.tblMstEmployee.BranchID == empLeave.BranchId : 1 > 0)
                    && (empLeave.StatusID > 0 ? x.StatusID == empLeave.StatusID : 1 > 0)
                    &&
                    (empLeave.EmployeeId > 0 ? x.EmployeeId == empLeave.EmployeeId : 1 > 0)

                    // x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.ReportingTo == empId)
                    //|| x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.ReviewingTo == empId)
                    //|| x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.AcceptanceAuthority == empId)
                    //)
                    );
                }
                else
                {
                    int[] statusIds = { 2, 4, 7 };
                    result = genericRepo.Get<Data.Models.EmployeeLeave>(x => x.IsDeleted == false
                    && (empLeave.BranchId > 0 ? x.tblMstEmployee.BranchID == empLeave.BranchId : 1 > 0)
                    && (statusIds.Any(y => y == x.StatusID) &&
                    (empLeave.EmployeeId > 0 ? x.EmployeeId == empLeave.EmployeeId : 1 > 0))
                    //&&
                    //(
                    // x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.ReportingTo == empId)
                    //|| x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.ReviewingTo == empId)
                    //|| x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && y.AcceptanceAuthority == empId)
                    //)
                    );
                }

                if (empLeave.DateFrom.HasValue && empLeave.DateTo.HasValue)
                {
                    result = result.Where(x => x.DateFrom >= empLeave.DateFrom.Value.Date && x.DateFrom <= empLeave.DateTo.Value.Date);
                }

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.EmployeeLeave, Model.EmployeeLeave>()
                    .ForMember(c => c.LeaveID, c => c.MapFrom(s => s.LeaveID))
                    .ForMember(c => c.EmployeeCode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.EmployeeName, c => c.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(c => c.DateFrom, c => c.MapFrom(s => s.DateFrom))
                    .ForMember(c => c.DateTo, c => c.MapFrom(s => s.DateTo))
                    .ForMember(c => c.Unit, c => c.MapFrom(s => s.Unit))
                    .ForMember(c => c.Reason, c => c.MapFrom(s => s.Reason))
                    .ForMember(c => c.StatusName, c => c.MapFrom(s => s.LeaveStatu.StatusDesc))
                    .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                    .ForMember(c => c.BranchId, c => c.MapFrom(s => s.tblMstEmployee.BranchID))
                    .ForMember(d => d.Branch, o => o.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                    .ForMember(d => d.ReportingTo, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.EmployeeID == s.EmployeeId && y.ProcessID == 3 && y.ToDate == null).ReportingTo))
                    .ForMember(d => d.ReportingOfficer, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.EmployeeID == s.EmployeeId && y.ProcessID == 3 && y.ToDate == null).ReportingTo))
                    .ForMember(d => d.ReviewerTo, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.EmployeeID == s.EmployeeId && y.ProcessID == 3 && y.ToDate == null).ReviewingTo))
                    .ForMember(d => d.AcceptanceAuthority, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.EmployeeID == s.EmployeeId && y.ProcessID == 3 && y.ToDate == null).AcceptanceAuthority))
                    .ForMember(d => d.DocumentName, o => o.MapFrom(s => s.DocumentName))
                    .ForMember(d => d.ReporotingToRemark, o => o.MapFrom(s => s.ReportingToRemark))
                    .ForMember(d => d.ReviewerToRemark, o => o.MapFrom(s => s.ReviewerToRemark))
                     .ForMember(d => d.AcceptanceAuthorityRemark, o => o.MapFrom(s => s.PurposeOthers))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.tblMstEmployee.EmployeeId))
                     .ForMember(c => c.ApprovalRequiredUpto, c => c.MapFrom(s => s.LeaveCategory.ApprovalRequiredUpto))
                     .ForMember(c => c.LeaveCategoryName, c => c.MapFrom(s => s.LeaveCategory.LeaveCategoryName))
                    .ForAllOtherMembers(d => d.Ignore())
                    ;
                });
                var listEmpLeave = Mapper.Map<List<Model.EmployeeLeave>>(result);
                //return listEmpLeave.OrderBy(x => cx.StatusID).ThenBy(x => x.DateFrom).ToList();
                return listEmpLeave;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }


        }
    }
}
