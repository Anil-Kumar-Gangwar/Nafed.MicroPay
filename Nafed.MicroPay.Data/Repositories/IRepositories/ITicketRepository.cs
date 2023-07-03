using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using System.Data;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
   public interface ITicketRepository:IBaseRepository
    {
        List<GetPendingTickets_Result> GetPendingTickets(int receiverID);
        List<GetAnsweredTickets_Result> GetAnsweredTickets(int senderID);
        SP_GetTicketAnswerDetail_Result GetTicketAnswerDetail(int ticketID);
        List<GetTicketForwardDetails_Result> GetTicketForwardDetails(int ticketID);
        List<GetTicketList_Result> GetTicketList(int employeeId);
        List<GetTicketForSectionHead_Result> GetTicketForSectionHead(DateTime ? fromDate,DateTime ? toDate,int? departmentID,int? designationID,int? statusID);
        List<GetTicketAttachment_Result> GetTicketAttachment(DateTime? fromDate, DateTime? toDate, int? departmentID, int? designationID, int? statusID);
        bool CheckEmployeeExist(string empCode, DateTime doj, int depId, int branchId, DateTime lastWorking);
    }
}
