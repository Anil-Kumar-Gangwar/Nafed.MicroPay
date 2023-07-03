using System.Collections.Generic;
using Nafed.MicroPay.Services.IServices;
namespace Nafed.MicroPay.Services.HelpDesk
{
    public interface ITicketPriorityService
    {
        List<Model.TicketPriority> GetTicketPriorityList();
        bool TicketPriorityExists(string ticketPriority, string description);
        int InsertTicketPriority(Model.TicketPriority ticketPriority);
        Model.TicketPriority GetTicketPriorityByID(int ticketPriorityID);
        bool UpdateTicketPriority(Model.TicketPriority ticketPriority);
        bool Delete(int ticketPriorityID);

    }
}
