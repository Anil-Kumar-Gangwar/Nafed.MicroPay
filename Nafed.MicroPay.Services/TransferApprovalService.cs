using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using AutoMapper;

namespace Nafed.MicroPay.Services
{
    public class TransferApprovalService : BaseService, ITransferApprovalService
    {
        private readonly IGenericRepository genericRepo;
        private readonly ITransferApprovalRepository transferApprovalRepository;
        public TransferApprovalService(IGenericRepository genericRepo, ITransferApprovalRepository transferApprovalRepository)
        {
            this.genericRepo = genericRepo;
            this.transferApprovalRepository = transferApprovalRepository;
        }
        public List<TrainingParticipantsDetail> GetTransferEmployeeDetailsList(int type)
        {
            log.Info($"TransferApprovalService/GetTransferEmployeeDetailsList/{type}");
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted
                && (type == 2 ? x.DOLeaveOrg == null : 1 > 0));

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.TrainingParticipantsDetail>()
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeId))
                    .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name))
                    .ForMember(d => d.EmployeeBranch, o => o.MapFrom(s => s.Branch.BranchName))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.DepartmentName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.Designation.DesignationName))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var listEmployeeDetails = Mapper.Map<List<Model.TrainingParticipantsDetail>>(result);
                return listEmployeeDetails.OrderBy(x => x.EmployeeName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public bool TransferApproval(int fromEmployeeID, int toEmployeeID, int? processId)
        {
            return transferApprovalRepository.TransferApproval(fromEmployeeID, toEmployeeID, processId);
        }

        public List<Model.SelectListModel> GetProcessList()
        {
            log.Info($"TransferApprovalService/GetProcessList");
            try
            {
                var result = genericRepo.Get<DTOModel.Process>();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Process, Model.SelectListModel>()
                    .ForMember(d => d.id, o => o.MapFrom(s => s.ProcessID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.ProcessName))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var listEmployeeDetails = Mapper.Map<List<Model.SelectListModel>>(result);
                return listEmployeeDetails.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
