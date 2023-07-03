using System.Collections.Generic;
using Nafed.MicroPay.Services.IServices;
using System;

namespace Nafed.MicroPay.Services.HelpDesk
{
    public interface ITicketService
    {
        List<Model.Ticket> GetTicketList(int? employeeID);
        int InsertTicket(Model.Ticket Ticket);
        Model.Ticket GetTicketByID(int TicketID);
        bool UpdateTicket(Model.Ticket Ticket);
        bool Delete(int ticketID, out string DocName);
        bool DeleteDocument(int docID, out string DocName);
        int GetWorkFlowDesignation(int departmentID, int tTypeID);
        #region Ticket Initialization
        List<Model.Ticket> GetTicketForSectionHead(Model.CommonFilter cFilter);
        bool TicketForward(Model.ProcessWorkFlow pWorkFlow,string tResponse= null);
        List<Model.Ticket> GetPendingTickets(int receiverID);
        List<Model.Ticket> GetAnsweredTickets(int senderID);
        Model.Ticket GetTicketResolveDtl(int ticketID);
        #endregion

        #region Ticket Work Flow
        int InsertTWorkFlow(Model.TicketWorkFlow tWrokFlow);
        bool UpdateTWorkFlow(Model.TicketWorkFlow tWrokFlow);
        List<Model.TicketWorkFlow> GetTicketWorkFlowList();
        Model.TicketWorkFlow GetTicketWorkFlowByID(int tWrokFlowID);
        #endregion
        #region Set Ticket Priority
        bool SetTicketPriority(Model.Ticket ticket);
        #endregion

        List<Model.TicketForwardDetails> GetTicketForwardDetails(int ticketID);
        bool TicketRollback(Model.ProcessWorkFlow pWorkFlow);

        bool CheckEmployeeExist(string empCode, DateTime doj, int depId, int branchId,DateTime lastWorking, out int empId, out int desgId);
    }
}
