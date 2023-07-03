using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using AutoMapper;

namespace Nafed.MicroPay.Services
{
    public class OTAService : BaseService, IOTAService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IEmployeeAttendanceRepository attendanceRepo;
        public OTAService(IGenericRepository genericRepo, IEmployeeAttendanceRepository attendanceRepo)
        {
            this.genericRepo = genericRepo;
            this.attendanceRepo = attendanceRepo;
        }
        public bool OTAExists(int oTACode)
        {
            log.Info($"OTAService/OTAExists/{oTACode}");
            return genericRepo.Exists<DTOModel.tblOTARate>(x => x.OTACode == oTACode && (bool)!x.IsDeleted);

        }
        public OTA GetOTAyNoOTACode(int oTACode)
        {
            log.Info($"OTAService/GetOTAyNoOTACode/{oTACode}");
            try
            {
                var oTAObj = genericRepo.Get<DTOModel.tblOTARate>(x => x.OTACode == oTACode).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.tblOTARate, Model.OTA>()
                     .ForMember(c => c.SNo, c => c.MapFrom(m => m.SNo))
                    .ForMember(c => c.FromPay, c => c.MapFrom(m => m.FromPay))
                 .ForMember(c => c.ToPay, c => c.MapFrom(m => m.ToPay))
                    .ForMember(c => c.MaxRateperHour, c => c.MapFrom(m => m.MaxRateperHour))
                      .ForMember(c => c.MaxAmt, c => c.MapFrom(m => m.MaxAmt))
                    .ForMember(c => c.OTACode, c => c.MapFrom(m => m.OTACode))
                     .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblOTARate, Model.OTA>(oTAObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<OTA> GetOTAList()
        {
            log.Info($"OTAService/GetOTAList");
            try

            {
                var result = genericRepo.Get<DTOModel.tblOTARate>(x => (bool)!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblOTARate, Model.OTA>()
                      .ForMember(c => c.SNo, c => c.MapFrom(m => m.SNo))
                    .ForMember(c => c.FromPay, c => c.MapFrom(m => m.FromPay))
                 .ForMember(c => c.ToPay, c => c.MapFrom(m => m.ToPay))
                    .ForMember(c => c.MaxRateperHour, c => c.MapFrom(m => m.MaxRateperHour))
                      .ForMember(c => c.MaxAmt, c => c.MapFrom(m => m.MaxAmt))
                    .ForMember(c => c.OTACode, c => c.MapFrom(m => m.OTACode))
                     .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var listOTA = Mapper.Map<List<Model.OTA>>(result);
                return listOTA.OrderBy(x => x.OTACode).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool InsertOTA(OTA createOTA)
        {
            log.Info($"OTAService/InsertOTA");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.OTA, DTOModel.tblOTARate>()
                    .ForMember(c => c.FromPay, c => c.MapFrom(m => m.FromPay))
                 .ForMember(c => c.ToPay, c => c.MapFrom(m => m.ToPay))
                    .ForMember(c => c.MaxRateperHour, c => c.MapFrom(m => m.MaxRateperHour))
                      .ForMember(c => c.MaxAmt, c => c.MapFrom(m => m.MaxAmt))
                    .ForMember(c => c.OTACode, c => c.MapFrom(m => m.OTACode))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                   .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(m => m.UpdatedBy))
                   .ForMember(c => c.UpdatedOn, c => c.MapFrom(m => m.UpdatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoOTA = Mapper.Map<DTOModel.tblOTARate>(createOTA);
                genericRepo.Insert<DTOModel.tblOTARate>(dtoOTA);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        public bool UpdateOTA(OTA editOTA)
        {
            log.Info($"OTAService/UpdateOTA");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.OTA, DTOModel.tblOTARate>()
                     .ForMember(c => c.SNo, c => c.MapFrom(m => m.SNo))
                     .ForMember(c => c.FromPay, c => c.MapFrom(m => m.FromPay))
                 .ForMember(c => c.ToPay, c => c.MapFrom(m => m.ToPay))
                    .ForMember(c => c.MaxRateperHour, c => c.MapFrom(m => m.MaxRateperHour))
                      .ForMember(c => c.MaxAmt, c => c.MapFrom(m => m.MaxAmt))
                    .ForMember(c => c.OTACode, c => c.MapFrom(m => m.OTACode))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(m => m.UpdatedOn))
                   .ForMember(c => c.UpdatedBy, c => c.MapFrom(m => m.UpdatedBy))
                   .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoOTA = Mapper.Map<DTOModel.tblOTARate>(editOTA);
                genericRepo.Update<DTOModel.tblOTARate>(dtoOTA);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }
        public bool Delete(int oTACode)
        {
            log.Info($"OTAService/Delete/{oTACode}");
            bool flag = false;
            try
            {
                DTOModel.tblOTARate dtoLoanSlab = new DTOModel.tblOTARate();
                dtoLoanSlab = genericRepo.Get<DTOModel.tblOTARate>(x => x.OTACode == oTACode).FirstOrDefault();
                dtoLoanSlab.IsDeleted = true;
                genericRepo.Update<DTOModel.tblOTARate>(dtoLoanSlab);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        public OTA GetMaxOTARate()
        {
            log.Info($"OTAService/GetMaxOTARate");

            OTA otaRate = new OTA();

            try
            {
                var otaRates = genericRepo.Get<DTOModel.tblOTARate>(x => !x.IsDeleted).OrderBy(y => y.SNo).FirstOrDefault();

                Mapper.Initialize(cfg =>
                {

                    cfg.CreateMap<DTOModel.tblOTARate, OTA>();
                });
                otaRate = Mapper.Map<OTA>(otaRates);
                return otaRate;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region OTA Slip
        public List<OTASlip> GetOTASlipList(int? employeeID)
        {
            log.Info($"OTAService/GetOTASlipList");
            try
            {
                var getOTASlipList = genericRepo.Get<DTOModel.OTASlip>(x => x.EmployeeID == employeeID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.OTASlip, Model.OTASlip>()
                     .ForMember(c => c.ApplicationID, c => c.MapFrom(m => m.ApplicationID))
                     .ForMember(c => c.EmployeeID, c => c.MapFrom(m => m.EmployeeID))
                     .ForMember(c => c.HolidayDate, c => c.MapFrom(m => m.HolidayDate))
                     .ForMember(c => c.HolidayFromTime, c => c.MapFrom(m => m.HolidayFromTime))
                     .ForMember(c => c.HolidayToTime, c => c.MapFrom(m => m.HolidayToTime))
                     .ForMember(c => c.StatusID, c => c.MapFrom(m => m.StatusID))
                     .ForMember(c => c.InstructedBy, c => c.MapFrom(m => m.InstructedBy))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoOTASlip = Mapper.Map<List<OTASlip>>(getOTASlipList);
                return dtoOTASlip;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<OTASlip> GetOTASlipDetail(CommonFilter filters)
        {
            log.Info($"OTAService/GetOTASlipDetail/");
            try
            {
                var getOTASlipList = genericRepo.GetIQueryable<DTOModel.OTASlip>
                (x => x.tblMstEmployee.EmployeeProcessApprovals
                       .Any(y => y.ProcessID == (int)Common.WorkFlowProcess.AttendanceApproval
                       && y.ToDate == null
                       && (y.ReportingTo == filters.loggedInEmployee
                       || y.ReviewingTo == filters.loggedInEmployee
                       || y.AcceptanceAuthority == filters.loggedInEmployee))
                ).ToList();
                if (filters.FromDate.HasValue && filters.ToDate.HasValue)
                {
                    getOTASlipList= getOTASlipList.Where(x =>  x.HolidayDate.Date >= filters.FromDate.Value.Date && x.HolidayDate.Date <= filters.ToDate.Value.Date).ToList();
                }
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.OTASlip, Model.OTASlip>()
                  .ForMember(c => c.ApplicationID, c => c.MapFrom(m => m.ApplicationID))
                     .ForMember(c => c.HolidayDate, c => c.MapFrom(m => m.HolidayDate))
                     .ForMember(c => c.HolidayFromTime, c => c.MapFrom(m => m.HolidayFromTime))
                     .ForMember(c => c.HolidayToTime, c => c.MapFrom(m => m.HolidayToTime))
                     .ForMember(c => c.EmployeeID, c => c.MapFrom(m => m.EmployeeID))
                     .ForMember(c => c.StatusID, c => c.MapFrom(m => m.StatusID))
                     .ForMember(c => c.EmployeeCode, c => c.MapFrom(m => m.tblMstEmployee.EmployeeCode))
                     .ForMember(c => c.EmployeeName, c => c.MapFrom(m => m.tblMstEmployee.Name))
                     .ForMember(c => c.InstructedBy, c => c.MapFrom(m => m.InstructedBy))
                    .ForMember(d => d.approvalSetting, o => o.MapFrom(s =>
                     s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y =>
                     y.ProcessID == (int)Common.WorkFlowProcess.AttendanceApproval && y.ToDate == null && y.EmployeeID == s.EmployeeID)))
                    .ForAllOtherMembers(d => d.Ignore());

                    cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();

                });
               var hh= Mapper.Map<List<OTASlip>>(getOTASlipList.ToList());
                return hh;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public OTASlip GetOTASlip(int empID, int appNo)
        {
            log.Info($"OTAService/GetOTASlip/empID={empID}/appNo={appNo}");
            try
            {
                //var getOTASlipList = genericRepo.Get<DTOModel.OTASlip>(x => x.EmployeeID == empID && x.ApplicationID == appNo).FirstOrDefault();
                var getOTASlipList = attendanceRepo.GetOTAslipDetail(empID, appNo);

                if (getOTASlipList != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.GetOTAslipDetail_Result, Model.OTASlip>()
                         .ForMember(c => c.ApplicationID, c => c.MapFrom(m => m.ApplicationID))
                         .ForMember(c => c.EmployeeID, c => c.MapFrom(m => m.EmployeeID))
                         .ForMember(c => c.HolidayDate, c => c.MapFrom(m => m.HolidayDate))
                         .ForMember(c => c.HolidayFromTime, c => c.MapFrom(m => m.HolidayFromTime))
                         .ForMember(c => c.HolidayToTime, c => c.MapFrom(m => m.HolidayToTime))
                         .ForMember(c => c.StatusID, c => c.MapFrom(m => m.StatusID))
                         .ForMember(c => c.InstructedBy, c => c.MapFrom(m => m.InstructedBy))
                         .ForMember(c => c.EmployeeName, c => c.MapFrom(m => m.Name))
                         .ForMember(c => c.AttendedDate, c => c.MapFrom(m => m.AttendedDate))
                         .ForMember(c => c.IndicatedDate, c => c.MapFrom(m => m.IndicatedDate))
                         .ForMember(c => c.AttendedFromTime, c => c.MapFrom(m => m.AttendedFromTime))
                         .ForMember(c => c.AttendedToTime, c => c.MapFrom(m => m.AttendedToTime))
                         .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                         .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                          .ForMember(c => c.ReportingName, c => c.MapFrom(m => m.ReportingTo))
                         .ForMember(c => c.ReviewingName, c => c.MapFrom(m => m.ReviewerTo))
                         .ForAllOtherMembers(c => c.Ignore());
                    });
                    var dtoOTASlip = Mapper.Map<OTASlip>(getOTASlipList);
                    return dtoOTASlip;
                }
                else
                    return new OTASlip();

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertOTASlip(OTASlip otaSlip)
        {
            log.Info($"OTAService/InsertOTASlip");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<OTASlip, DTOModel.OTASlip>()
                     .ForMember(c => c.EmployeeID, c => c.MapFrom(m => m.EmployeeID))
                     .ForMember(c => c.HolidayDate, c => c.MapFrom(m => m.HolidayDate))
                     .ForMember(c => c.HolidayFromTime, c => c.MapFrom(m => m.HolidayFromTime))
                     .ForMember(c => c.HolidayToTime, c => c.MapFrom(m => m.HolidayToTime))
                     .ForMember(c => c.StatusID, c => c.MapFrom(m => m.StatusID))
                     .ForMember(c => c.InstructedBy, c => c.MapFrom(m => m.InstructedBy))
                     .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoOTASlip = Mapper.Map<DTOModel.OTASlip>(otaSlip);
                genericRepo.Insert<DTOModel.OTASlip>(dtoOTASlip);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateOTASlip(OTASlip otaSlip)
        {
            log.Info($"OTAService/UpdateOTASlip");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<OTASlip, DTOModel.OTASlip>()
                     .ForMember(c => c.ApplicationID, c => c.MapFrom(m => m.ApplicationID))
                     .ForMember(c => c.EmployeeID, c => c.MapFrom(m => m.EmployeeID))
                     .ForMember(c => c.HolidayDate, c => c.MapFrom(m => m.HolidayDate))
                     .ForMember(c => c.HolidayFromTime, c => c.MapFrom(m => m.HolidayFromTime))
                     .ForMember(c => c.HolidayToTime, c => c.MapFrom(m => m.HolidayToTime))
                     .ForMember(c => c.StatusID, c => c.MapFrom(m => m.StatusID))
                     .ForMember(c => c.InstructedBy, c => c.MapFrom(m => m.InstructedBy))
                     .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                     .ForMember(c => c.IndicatedDate, c => c.MapFrom(m => m.IndicatedDate))
                     .ForMember(c => c.AttendedDate, c => c.MapFrom(m => m.AttendedDate))
                     .ForMember(c => c.AttendedFromTime, c => c.MapFrom(m => m.AttendedFromTime))
                     .ForMember(c => c.AttendedToTime, c => c.MapFrom(m => m.AttendedToTime))
                     .ForMember(c => c.UpdateBy, c => c.MapFrom(m => m.UpdateBy))
                     .ForMember(c => c.UpdateOn, c => c.MapFrom(m => m.UpdateOn))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoOTASlip = Mapper.Map<DTOModel.OTASlip>(otaSlip);
                genericRepo.Update<DTOModel.OTASlip>(dtoOTASlip);
                if (otaSlip.StatusID != (int)Common.EmpAttendanceStatus.Pending)
                {
                    otaSlip._ProcessWorkFlow.ReferenceID = dtoOTASlip.ApplicationID;
                    AddProcessWorkFlow(otaSlip._ProcessWorkFlow);
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool OTASlipExists(int empID, DateTime date)
        {
            log.Info($"OTAService/OTAExists");
            try
            {
                return genericRepo.Exists<DTOModel.OTASlip>(x => x.EmployeeID == empID && x.HolidayDate == date && x.StatusID !=7 && x.StatusID !=2);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }
        #endregion
    }
}
