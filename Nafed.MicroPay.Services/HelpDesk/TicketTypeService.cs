using System;
using System.Collections.Generic;
using Nafed.MicroPay.Model;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;

namespace Nafed.MicroPay.Services.HelpDesk
{
    public class TicketTypeService : BaseService, ITicketTypeService
    {
        private readonly IGenericRepository genericRepo;
        public TicketTypeService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.TicketType> GetTicketTypeList()
        {
            log.Info($"TicketTypeService/GetTicketTypeList");
            try
            {
                var result = genericRepo.Get<DTOModel.ticket_type>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.ticket_type, Model.TicketType>();              
                });
                var listTicketType = Mapper.Map<List<Model.TicketType>>(result);
                return listTicketType;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool TicketTypeExists(string ticketcode, string description)
        {
            return genericRepo.Exists<DTOModel.ticket_type>(x => x.code.Equals(ticketcode) && (!string.IsNullOrEmpty(description) ? x.description.Equals(description):(1>0)) && x.IsDeleted == false);
        }

        public int InsertTicketType(TicketType ticketType)
        {
            log.Info($"TicketTypeService/InsertTicketType");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.TicketType, DTOModel.ticket_type>();
                });
                var dtoTicketType = Mapper.Map<DTOModel.ticket_type>(ticketType);
                genericRepo.Insert<DTOModel.ticket_type>(dtoTicketType);
                return dtoTicketType.ID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public Model.TicketType GetTicketTypeByID(int id)
        {
            log.Info($"TicketTypeService/InsertTicketType/{id}");
            try
            {
                var ticketTypeObj = genericRepo.GetByID<DTOModel.ticket_type>(id);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.ticket_type, Model.TicketType>();
                });
                var obj = Mapper.Map<Model.TicketType>(ticketTypeObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateTicketType(Model.TicketType ticketType)
        {
            log.Info($"TicketTypeService/UpdateTicketType");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.ticket_type>(ticketType.ID);
                if (dtoObj != null)
                {
                    dtoObj.code = ticketType.code;
                    dtoObj.description = ticketType.description;
                    dtoObj.UpdatedBy = ticketType.UpdatedBy;
                    dtoObj.UpdatedOn = ticketType.UpdatedOn;
                    genericRepo.Update<DTOModel.ticket_type>(dtoObj);
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
            log.Info($"TicketTypeService/Delete/{ticketTypeID}");
            bool flag = false;
            try
            {

                var dtoticketType = genericRepo.GetByID<DTOModel.ticket_type>(ticketTypeID);
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
