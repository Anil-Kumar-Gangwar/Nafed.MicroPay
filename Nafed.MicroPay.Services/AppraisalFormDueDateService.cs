using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
namespace Nafed.MicroPay.Services
{
    public class AppraisalFormDueDateService : BaseService, IAppraisalFormDueDateService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IAppraisalRepository apprepo;
        public AppraisalFormDueDateService(IGenericRepository genericRepo, IAppraisalRepository apprepo)
        {
            this.genericRepo = genericRepo;
            this.apprepo = apprepo;
        }

        public List<AppraisalFormDueDate> GetAppraisalFormDueDate(string reportingYr)
        {
            log.Info($"AppraisalFormDueDateService/GetTicketPriorityList");
            try
            {
                List<DTOModel.AppraisalForm> result = new List<Data.Models.AppraisalForm>();
                List<DTOModel.AppraisalForm> result1 = new List<Data.Models.AppraisalForm>();
                if (!string.IsNullOrEmpty(reportingYr))
                    result = genericRepo.Get<DTOModel.AppraisalForm>(x => !x.IsDeleted && x.ReportingYr == reportingYr).ToList();
                else

                    result = genericRepo.Get<DTOModel.AppraisalForm>(x => !x.IsDeleted).ToList();

                result1 = (from res in result
                           group res by res.ReportingYr into g
                           select new DTOModel.AppraisalForm
                           {
                               EmployeeSubmissionDueDate = g.Max(m => m.EmployeeSubmissionDueDate),
                               ReportingSubmissionDueDate = g.Max(m => m.ReportingSubmissionDueDate),
                               ReviewerSubmissionDueDate = g.Max(m => m.ReviewerSubmissionDueDate),
                               AcceptanceAuthSubmissionDueDate = g.Max(m => m.AcceptanceAuthSubmissionDueDate),
                               ReportingYr = g.Max(m => m.ReportingYr)
                           }).ToList();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.AppraisalForm, AppraisalFormDueDate>()
                    .ForMember(d => d.ReportingYear, o => o.MapFrom(s => s.ReportingYr))
                   .ForMember(d => d.EmployeeSubmissionDueDate, o => o.MapFrom(s => s.EmployeeSubmissionDueDate))
                   .ForMember(d => d.ReportingSubmissionDueDate, o => o.MapFrom(s => s.ReportingSubmissionDueDate))
                   .ForMember(d => d.ReviewerSubmissionDueDate, o => o.MapFrom(s => s.ReviewerSubmissionDueDate))
                   .ForMember(d => d.AcceptanceAuthSubmissionDueDate, o => o.MapFrom(s => s.AcceptanceAuthSubmissionDueDate));
                });
                var dtoAprduadate = Mapper.Map<List<AppraisalFormDueDate>>(result1);
                return dtoAprduadate;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public bool UpdateAppraisalForm(AppraisalFormDueDate apFormDueDate)
        {
            log.Info($"AppraisalFormDueDateService/UpdateAppraisalForm");
            bool flag = false;
            try
            {
                flag = apprepo.UpdateAPARDates(apFormDueDate.ReportingYear, apFormDueDate.EmployeeSubmissionDueDate, apFormDueDate.ReportingSubmissionDueDate, apFormDueDate.ReviewerSubmissionDueDate, apFormDueDate.AcceptanceAuthSubmissionDueDate);


                //var dtoObj = genericRepo.Get<DTOModel.AppraisalForm>(x=> !x.IsDeleted).FirstOrDefault();
                //if (dtoObj != null)
                //{
                //    Mapper.Initialize(cfg =>
                //    {
                //    cfg.CreateMap<DTOModel.AppraisalForm, DTOModel.AppraisalFormHistory>()
                //   .ForMember(d => d.EmployeeSubmissionDueDate, o => o.UseValue(dtoObj.EmployeeSubmissionDueDate))
                //   .ForMember(d => d.ReportingSubmissionDueDate, o => o.UseValue(dtoObj.ReportingSubmissionDueDate))
                //   .ForMember(d => d.ReviewerSubmissionDueDate, o => o.UseValue(dtoObj.ReviewerSubmissionDueDate))
                //   .ForMember(d => d.AcceptanceAuthSubmissionDueDate, o => o.UseValue(dtoObj.AcceptanceAuthSubmissionDueDate))
                //   .ForMember(d => d.FormID, o => o.UseValue(dtoObj.FormID))
                //   .ForMember(d => d.FormName, o => o.UseValue(dtoObj.FormName))               
                //   .ForAllOtherMembers(c => c.Ignore());
                //    });
                //    var dtoAprFormHistory = Mapper.Map<DTOModel.AppraisalFormHistory>(dtoObj);
                //    dtoAprFormHistory.ReportingYr = apFormDueDate.ReportingYear;
                //    genericRepo.Insert<DTOModel.AppraisalFormHistory>(dtoAprFormHistory);


                //    dtoObj.EmployeeSubmissionDueDate = apFormDueDate.EmployeeSubmissionDueDate;
                //    dtoObj.ReportingSubmissionDueDate = apFormDueDate.ReportingSubmissionDueDate;
                //    dtoObj.ReviewerSubmissionDueDate = apFormDueDate.ReviewerSubmissionDueDate;
                //    dtoObj.AcceptanceAuthSubmissionDueDate = apFormDueDate.AcceptanceAuthSubmissionDueDate;
                //    genericRepo.Update<DTOModel.AppraisalForm>(dtoObj);
                //}
                return flag;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }
    }
}
