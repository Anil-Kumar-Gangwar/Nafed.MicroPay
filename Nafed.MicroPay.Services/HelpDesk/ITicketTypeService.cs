using System.Collections.Generic;
using Nafed.MicroPay.Services.IServices;
namespace Nafed.MicroPay.Services.HelpDesk
{
    public interface ITicketTypeService
    {
        List<Model.TicketType> GetTicketTypeList();
        bool TicketTypeExists(string ticketType, string description);
        int InsertTicketType(Model.TicketType ticketType);
        Model.TicketType GetTicketTypeByID(int id);
        bool UpdateTicketType(Model.TicketType ticketType);
        bool Delete(int ticketTypeID);

    }
}
