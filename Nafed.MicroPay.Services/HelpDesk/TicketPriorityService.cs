using System;
using System.Collections.Generic;
using Nafed.MicroPay.Model;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;

namespace Nafed.MicroPay.Services.HelpDesk
{
    public class TicketPriorityService : BaseService, ITicketPriorityService
    {
        private readonly IGenericRepository genericRepo;
        public TicketPriorityService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.TicketPriority> GetTicketPriorityList()
        {
            log.Info($"TicketPriorityService/GetTicketPriorityList");
            try
            {
                var result = genericRepo.Get<DTOModel.ticket_priority>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.ticket_priority, Model.TicketPriority>();              
                });
                var listTicketPriority = Mapper.Map<List<Model.TicketPriority>>(result);
                return listTicketPriority;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool TicketPriorityExists(string ticketcode, string description)
        {
            return genericRepo.Exists<DTOModel.ticket_priority>(x => x.code.Equals(ticketcode) && (!string.IsNullOrEmpty(description) ? x.description.Equals(description):(1>0)) && x.IsDeleted == false);
        }

        public int InsertTicketPriority(TicketPriority ticketType)
        {
            log.Info($"TicketPriorityService/InsertTicketPriority");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.TicketPriority, DTOModel.ticket_priority>();
                });
                var dtoTicketPriority = Mapper.Map<DTOModel.ticket_priority>(ticketType);
                genericRepo.Insert<DTOModel.ticket_priority>(dtoTicketPriority);
                return dtoTicketPriority.ID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public Model.TicketPriority GetTicketPriorityByID(int id)
        {
            log.Info($"TicketPriorityService/InsertTicketPriority/{id}");
            try
            {
                var ticketTypeObj = genericRepo.GetByID<DTOModel.ticket_priority>(id);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.ticket_priority, Model.TicketPriority>();
                });
                var obj = Mapper.Map<Model.TicketPriority>(ticketTypeObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateTicketPriority(Model.TicketPriority ticketType)
        {
            log.Info($"TicketPriorityService/UpdateTicketPriority");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.ticket_priority>(ticketType.ID);
                if (dtoObj != null)
                {
                    dtoObj.code = ticketType.code;
                    dtoObj.description = ticketType.description;
                    dtoObj.UpdatedBy = ticketType.UpdatedBy;
                    dtoObj.UpdatedOn = ticketType.UpdatedOn;
                    genericRepo.Update<DTOModel.ticket_priority>(dtoObj);
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

        public bool Delete(int ticketTypeID)
        {
            log.Info($"TicketPriorityService/Delete/{ticketTypeID}");
            bool flag = false;
            try
            {

                var dtoticketType = genericRepo.GetByID<DTOModel.ticket_priority>(ticketTypeID);
                if (dtoticketType != null)
                {
                    dtoticketType.IsDeleted = true;
                    genericRepo.Update(dtoticketType);
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
