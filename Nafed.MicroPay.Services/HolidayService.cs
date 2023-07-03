using System;
using System.Collections.Generic;
using System.Linq;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;

namespace Nafed.MicroPay.Services
{
    public class HolidayService : BaseService, IHolidayService
    {
        private readonly IGenericRepository genericRepo;
        public HolidayService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public List<Model.Holiday> GetHolidayList(int? branchID, int ? CYear)
        {
            log.Info($"HolidayService/GetHolidayList");
            try
            {
                var result = genericRepo.Get<DTOModel.Holiday>(x =>(x.BranchID==branchID.Value || x.BranchID==null) 
                && /*x.HolidayDate.Year==DateTime.Now.Year*/   x.CYear== CYear && (x.IsDeleted==false || x.IsDeleted==null));
               // var result = genericRepo.Get<DTOModel.Holiday>(x => x.BranchID ==(int?) 14);
                Mapper.Initialize(cfg =>
                {
                     cfg.CreateMap<DTOModel.Holiday, Model.Holiday>()
                     .ForMember(c => c.CYear, c => c.MapFrom(s => s.CYear))
                    .ForMember(c => c.HolidayID, c => c.MapFrom(s => s.HolidayID))
                    .ForMember(c => c.HolidayDate, c => c.MapFrom(s => s.HolidayDate))
                    .ForMember(c => c.HolidayName, c => c.MapFrom(s => s.HolidayName))
                    .ForMember(c => c.BranchName, c => c.MapFrom(s => s.Branch.BranchName))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var listHoliday = Mapper.Map<List<Model.Holiday>>(result);
                return listHoliday.OrderBy(x => x.HolidayDate).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool HolidayNameExists(DateTime ? HolidayDate, string HolidayName, int? BranchId, int ? CYear)
        {
            log.Info($"HolidayService/HolidayNameExists/{HolidayDate}/{HolidayName}/{BranchId}");

            var  branchID = BranchId == 0 ? null : BranchId;
            var holidayDate = HolidayDate.Value.Date;

            return genericRepo.Exists<DTOModel.Holiday>(x => x.HolidayDate == holidayDate 
            && x.HolidayName == HolidayName && x.BranchID == branchID && x.CYear==CYear);
        }

        public bool UpdateHoliday(Model.Holiday editHolidayItem)
        {
            log.Info($"HolidayService/UpdateHoliday");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.Holiday>(editHolidayItem.HolidayID);
                dtoObj.HolidayDate = DateTime.Parse(editHolidayItem.HolidayDate.ToString());
                dtoObj.HolidayName = editHolidayItem.HolidayName;
                dtoObj.BranchID = editHolidayItem.BranchId;
                dtoObj.UpdatedBy = editHolidayItem.UpdatedBy;
                dtoObj.UpdatedOn = editHolidayItem.UpdatedOn;
                genericRepo.Update<DTOModel.Holiday>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }
        public bool InsertHoliday(Model.Holiday createHoliday)
        {
            log.Info($"HolidayService/InsertHoliday");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Holiday, DTOModel.Holiday>()
                    .ForMember(c => c.CYear, c => c.MapFrom(m => m.CYear))
                    .ForMember(c => c.HolidayDate, c => c.MapFrom(m => m.HolidayDate))
                    .ForMember(c => c.HolidayName, c => c.MapFrom(m => m.HolidayName))
                  //.ForMember(c => c.BranchID, c => c.MapFrom(m => m.BranchId))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoHoliday = Mapper.Map<DTOModel.Holiday>(createHoliday);
                dtoHoliday.BranchID = createHoliday.BranchId.HasValue && createHoliday.BranchId.Value > 0 ? createHoliday.BranchId.Value : dtoHoliday.BranchID;
                genericRepo.Insert<DTOModel.Holiday>(dtoHoliday);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        public Model.Holiday GetHolidayByID(int HolidayID)
        {
            log.Info($"GetHolidayByID {HolidayID}");
            try
            {
                var HolidayObj = genericRepo.GetByID<DTOModel.Holiday>(HolidayID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Holiday, Model.Holiday>()
                     .ForMember(c => c.CYear, c => c.MapFrom(m => m.CYear))
                    .ForMember(c => c.HolidayDate, c => c.MapFrom(m => m.HolidayDate))
                    .ForMember(c => c.HolidayName, c => c.MapFrom(m => m.HolidayName))
                    .ForMember(c => c.BranchId, c => c.MapFrom(m => m.BranchID))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.Holiday, Model.Holiday>(HolidayObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(int holidayID)
        {

            log.Info($"HolidayService/Delete/{holidayID}");
            bool flag = false;
            try
            {
                DTOModel.Holiday dtoHoliday = new DTOModel.Holiday();
                dtoHoliday = genericRepo.GetByID<DTOModel.Holiday>(holidayID);
                if (dtoHoliday != null)
                {
                    dtoHoliday.IsDeleted = true;
                    genericRepo.Update<DTOModel.Holiday>(dtoHoliday);
                    flag = true;
                } 
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
    }
}