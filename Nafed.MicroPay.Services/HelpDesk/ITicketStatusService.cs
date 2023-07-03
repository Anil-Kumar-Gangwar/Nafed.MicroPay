using System.Collections.Generic;
using Nafed.MicroPay.Services.IServices;
namespace Nafed.MicroPay.Services.HelpDesk
{
    public interface ITicketStatusService
    {
        List<Model.TicketStatus> GetTicketStatusList();
        bool TicketStatusExists(string ticketStatus, string description);
        int InsertTicketStatus(Model.TicketStatus ticketStatus);
        Model.TicketStatus GetTicketStatusByID(int ticketStatusID);
        bool UpdateTicketStatus(Model.TicketStatus ticketStatus);
        bool Delete(int ticketStatusID);

    }
}
