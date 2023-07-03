using System;
using System.Collections.Generic;
using Nafed.MicroPay.Model;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;

namespace Nafed.MicroPay.Services.HelpDesk
{
    public class TicketStatusService : BaseService, ITicketStatusService
    {
        private readonly IGenericRepository genericRepo;
        public TicketStatusService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.TicketStatus> GetTicketStatusList()
        {
            log.Info($"TicketStatusService/GetTicketStatusList");
            try
            {
                var result = genericRepo.Get<DTOModel.ticket_status>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.ticket_status, Model.TicketStatus>();              
                });
                var listTicketStatus = Mapper.Map<List<Model.TicketStatus>>(result);
                return listTicketStatus;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool TicketStatusExists(string tStatuscode, string description)
        {
            return genericRepo.Exists<DTOModel.ticket_status>(x => x.code.Equals(tStatuscode) && (!string.IsNullOrEmpty(description) ? x.description.Equals(description):(1>0)) && x.IsDeleted == false);
        }

        public int InsertTicketStatus(TicketStatus ticketStatus)
        {
            log.Info($"TicketStatusService/InsertTicketStatus");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.TicketStatus, DTOModel.ticket_status>();
                });
                var dtoTicketStatus = Mapper.Map<DTOModel.ticket_status>(ticketStatus);
                genericRepo.Insert<DTOModel.ticket_status>(dtoTicketStatus);
                return dtoTicketStatus.ID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public Model.TicketStatus GetTicketStatusByID(int id)
        {
            log.Info($"TicketStatusService/InsertTicketStatus/{id}");
            try
            {
                var ticketStatusObj = genericRepo.GetByID<DTOModel.ticket_status>(id);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.ticket_status, Model.TicketStatus>();
                });
                var obj = Mapper.Map<Model.TicketStatus>(ticketStatusObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateTicketStatus(Model.TicketStatus ticketStatus)
        {
            log.Info($"TicketStatusService/UpdateTicketStatus");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.ticket_status>(ticketStatus.ID);
                if (dtoObj != null)
                {
                    dtoObj.code = ticketStatus.code;
                    dtoObj.sort_order = ticketStatus.sort_order;
                    dtoObj.description = ticketStatus.description;
                    dtoObj.UpdatedBy = ticketStatus.UpdatedBy;
                    dtoObj.UpdatedOn = ticketStatus.UpdatedOn;
                    genericRepo.Update<DTOModel.ticket_status>(dtoObj);
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

        public bool Delete(int ticketStatusID)
        {
            log.Info($"TicketStatusService/Delete/{ticketStatusID}");
            bool flag = false;
            try
            {

                var dtoticketStatus = genericRepo.GetByID<DTOModel.ticket_status>(ticketStatusID);
                if (dtoticketStatus != null)
                {
                    dtoticketStatus.IsDeleted = true;
                    genericRepo.Update(dtoticketStatus);
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
