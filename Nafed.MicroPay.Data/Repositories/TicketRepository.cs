using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using System.Data.Entity;

namespace Nafed.MicroPay.Data.Repositories
{
    public class TicketRepository : BaseRepository, ITicketRepository
    {
        public List<DTOModel.GetAnsweredTickets_Result> GetAnsweredTickets(int senderID)
        {
            return db.GetAnsweredTickets(senderID).ToList();
        }

        public List<DTOModel.GetPendingTickets_Result> GetPendingTickets(int receiverID)
        {
            return db.GetPendingTickets(receiverID).ToList();
        }

        public DTOModel.SP_GetTicketAnswerDetail_Result GetTicketAnswerDetail(int ticketID)
        {
            return db.SP_GetTicketAnswerDetail(ticketID).FirstOrDefault();
        }

        public List<DTOModel.GetTicketForwardDetails_Result> GetTicketForwardDetails(int ticketID)
        {
            return db.GetTicketForwardDetails(ticketID).ToList();
        }
        public List<DTOModel.GetTicketList_Result> GetTicketList(int employeeId)
        {
            return db.GetTicketList(employeeId).ToList();
        }

        public List<DTOModel.GetTicketForSectionHead_Result> GetTicketForSectionHead(DateTime? fromDate, DateTime? toDate, int? departmentID, int? designationID, int? statusID)
        {
            return db.GetTicketForSectionHead(fromDate, toDate, departmentID, designationID, statusID).ToList();
        }

        public List<DTOModel.GetTicketAttachment_Result> GetTicketAttachment(DateTime? fromDate, DateTime? toDate, int? departmentID, int? designationID, int? statusID)
        {
            return db.GetTicketAttachment(fromDate, toDate, departmentID, designationID, statusID).ToList();
        }

        public bool CheckEmployeeExist(string empCode, DateTime doj, int depId, int branchId, DateTime lastWorking)
        {
            var data = db.tblMstEmployees.Where(x => x.EmployeeCode == empCode && DbFunctions.TruncateTime(x.Pr_Loc_DOJ) == doj && x.DepartmentID == depId && x.BranchID == branchId && DbFunctions.TruncateTime(x.DOLeaveOrg) == lastWorking).FirstOrDefault();
            if (data != null)           
                return true;           
            else           
                return false;
           
        }
    }
}
