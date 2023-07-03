using System;
using System.Collections.Generic;
using Nafed.MicroPay.Model;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.HelpDesk
{
    public class TicketService : BaseService, ITicketService
    {
        private readonly IGenericRepository genericRepo;
        private readonly ITicketRepository ticktRepo;
        public TicketService(IGenericRepository genericRepo, ITicketRepository ticktRepo)
        {
            this.genericRepo = genericRepo;
            this.ticktRepo = ticktRepo;
        }

        public List<Model.Ticket> GetTicketList(int? employeeID)
        {
            log.Info($"TicketService/GetTicketList");
            try
            {
                var result = ticktRepo.GetTicketList((int)employeeID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetTicketList_Result, Model.Ticket>()
                    .ForMember(d => d.TicketType, o => o.MapFrom(s => s.code))
                    .ForMember(d => d.Department, o => o.MapFrom(s => s.DepartmentName))
                    .ForMember(d => d.ID, o => o.MapFrom(s => s.TicketNo))
                    .ForMember(d => d.TicketSolverRmk, o => o.MapFrom(s => s.Scomments))
                    .ForMember(d => d.PreviousTicket, o => o.MapFrom(s => s.PreviousTicket));
                });
                var listTicket = Mapper.Map<List<Model.Ticket>>(result);
                return listTicket;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        //public List<Model.Ticket> GetTicketList(int? employeeID)
        //{
        //    log.Info($"TicketService/GetTicketList");
        //    try
        //    {
        //        var result = genericRepo.Get<DTOModel.ticket>(x => !x.IsDeleted && (employeeID.HasValue ? x.customer_id == employeeID : (1 > 0)));
        //        Mapper.Initialize(cfg =>
        //        {
        //            cfg.CreateMap<DTOModel.ticket, Model.Ticket>()
        //            .ForMember(d => d.TicketType, o => o.MapFrom(s => s.ticket_type.code))
        //            .ForMember(d => d.Department, o => o.MapFrom(s => s.Department.DepartmentName));

        //        });
        //        var listTicket = Mapper.Map<List<Model.Ticket>>(result);
        //        return listTicket;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }
        //}
        public int InsertTicket(Ticket ticket)
        {
            log.Info($"TicketService/InsertTicket");
            try 
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Ticket, DTOModel.ticket>();
                });
                var dtoTicket = Mapper.Map<DTOModel.ticket>(ticket);
                genericRepo.Insert<DTOModel.ticket>(dtoTicket);

                if (ticket.TicketDocument != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.TicketAttachment, DTOModel.ticket_attachments>()
                        .ForMember(d => d.ticketID, o => o.UseValue(dtoTicket.ID));
                    });
                    var dtoTktAtch = Mapper.Map<DTOModel.ticket_attachments>(ticket.TicketDocument);
                    genericRepo.Insert<DTOModel.ticket_attachments>(dtoTktAtch);
                }
                return dtoTicket.ID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public Model.Ticket GetTicketByID(int id)
        {
            log.Info($"TicketService/InsertTicket/{id}");
            try
            {
                var TicketObj = genericRepo.GetByID<DTOModel.ticket>(id);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.ticket, Model.Ticket>();
                });
                var objTicket = Mapper.Map<Model.Ticket>(TicketObj);

                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.ticket_attachments, TicketAttachment>()
                      );

                var dtoDoc = Mapper.Map<TicketAttachment>(TicketObj.ticket_attachments.FirstOrDefault());
                objTicket.TicketDocument = dtoDoc;
                return objTicket;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateTicket(Model.Ticket ticket)
        {
            log.Info($"TicketService/UpdateTicket");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.ticket>(ticket.ID);
                if (dtoObj != null)
                {
                    dtoObj.DepartmentID = ticket.DepartmentID;
                    dtoObj.type_id = ticket.type_id;
                    dtoObj.DesignationID = ticket.DesignationID;
                    dtoObj.subject = ticket.subject;
                    dtoObj.Message = ticket.Message;
                    dtoObj.UpdatedBy = ticket.UpdatedBy;
                    dtoObj.UpdatedOn = ticket.UpdatedOn;
                    dtoObj.PreviousTicket = ticket.PreviousTicket;
                    genericRepo.Update<DTOModel.ticket>(dtoObj);
                    flag = true;
                    if (ticket.TicketDocument != null)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.TicketAttachment, DTOModel.ticket_attachments>()
                            .ForMember(d => d.ticketID, o => o.UseValue(ticket.ID));
                        });
                        var dtoTktAtch = Mapper.Map<DTOModel.ticket_attachments>(ticket.TicketDocument);
                        genericRepo.Insert<DTOModel.ticket_attachments>(dtoTktAtch);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool Delete(int ticketID, out string DocName)
        {
            log.Info($"TicketService/Delete/{ticketID}");
            bool flag = false;
            DocName = "";
            try
            {

                var dtoTicket = genericRepo.GetByID<DTOModel.ticket>(ticketID);
                if (dtoTicket != null)
                {
                    dtoTicket.IsDeleted = true;
                    genericRepo.Update(dtoTicket);
                    flag = true;

                    var dtodoc = genericRepo.Get<DTOModel.ticket_attachments>(x => x.ticketID == ticketID).FirstOrDefault();
                    if (dtodoc != null)
                    {
                        DocName = dtodoc.docpathname;
                        genericRepo.Delete(dtodoc);
                        flag = true;
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool DeleteDocument(int docID, out string DocName)
        {
            log.Info($"TicketService/DeleteDocument/docID={docID}");
            bool flag = false;
            DocName = "";
            try
            {
                var dtodoc = genericRepo.GetByID<DTOModel.ticket_attachments>(docID);
                if (dtodoc != null)
                {
                    DocName = dtodoc.docpathname;
                    genericRepo.Delete(dtodoc);
                    flag = true;
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int GetWorkFlowDesignation(int departmentID, int tTypeID)
        {
            log.Info($"TicketService/GetWorkFlowDesignation");
            try
            {
                var getDesignationID = genericRepo.Get<DTOModel.TicketWorkFlow>(x => x.DepartmentID == departmentID &&
                x.TicketTypeID == tTypeID);
                if (getDesignationID != null && getDesignationID.Count<DTOModel.TicketWorkFlow>() > 0)
                {
                    return getDesignationID.FirstOrDefault().DesignationID;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #region Ticket Initialization
        public List<Ticket> GetTicketForSectionHead(CommonFilter cFilter)
        {
            log.Info($"TicketService/GetTicketList");
            try
            {
                List<Ticket> objTicket = new List<Ticket>();
                // var ticketQry = genericRepo.Get<DTOModel.ticket>(
                //    x => (cFilter.FromDate.HasValue ? x.CreatedOn >= cFilter.FromDate : (1 > 0)) && (cFilter.ToDate.HasValue ? x.CreatedOn <= cFilter.ToDate : (1 > 0)) && (cFilter.loggedInEmployee == 0 ? (1 > 0) : x.DepartmentID == cFilter.loggedInEmployee) && (cFilter.DesignationID.HasValue ? x.DesignationID == cFilter.DesignationID : (1 > 0)) && (cFilter.StatusID.HasValue ? x.status_id == cFilter.StatusID : (1 > 0)));
                int? depId = cFilter.loggedInEmployee == 0 ? null : (int?)cFilter.loggedInEmployee;
                var ticketQry = ticktRepo.GetTicketForSectionHead(cFilter.FromDate, cFilter.ToDate, depId, cFilter.DesignationID, cFilter.StatusID).OrderByDescending(x => x.priority_id);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetTicketForSectionHead_Result, Model.Ticket>()
                    .ForMember(d => d.ID, o => o.MapFrom(s => s.TicketNo))
                    .ForMember(d => d.TicketType, o => o.MapFrom(s => s.code))
                    .ForMember(d => d.Department, o => o.MapFrom(s => s.DepartmentName))
                    .ForMember(d => d.TicketSolverRmk, o => o.MapFrom(s => s.Scomments));
                });
                objTicket = Mapper.Map<List<Model.Ticket>>(ticketQry);
                objTicket = objTicket.OrderByDescending(x => x.priority_id).OrderBy(y => y.status_id).ToList();

                var getDocumentData = ticktRepo.GetTicketAttachment(cFilter.FromDate, cFilter.ToDate, depId, cFilter.DesignationID, cFilter.StatusID).ToList();
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetTicketAttachment_Result, TicketAttachment>());
                var documentList = Mapper.Map<List<TicketAttachment>>(getDocumentData);

                if (objTicket != null && objTicket.Count > 0)
                {
                    var ticketID = 0;
                    for (int i = 0; i < objTicket.Count; i++)
                    {
                        ticketID = objTicket[i].ID;
                        objTicket[i].TicketDocumentList = documentList.Where(x => x.ticketID == ticketID).ToList();
                    }
                }
                return objTicket;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool TicketForward(Model.ProcessWorkFlow pWorkFlow,string tResponse=null)
        {
            log.Info($"TicketService/TicketForward");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.ticket>(pWorkFlow.ReferenceID);
                if (dtoObj != null)
                {
                    dtoObj.status_id = pWorkFlow.StatusID;
                    dtoObj.UpdatedBy = pWorkFlow.UpdatedBy;
                    dtoObj.UpdatedOn = pWorkFlow.UpdatedOn;
                    if (pWorkFlow.StatusID == 2)
                        dtoObj.agent_id = pWorkFlow.SenderID;
                    genericRepo.Update<DTOModel.ticket>(dtoObj);
                    flag = true;
                }
                if (flag)
                {
                    var lastRemark = string.Empty;
                    var dtoProcess = genericRepo.Get<DTOModel.ProcessWorkFlow>(x => x.ReferenceID == pWorkFlow.ReferenceID).OrderByDescending(x => x.WorkflowID).FirstOrDefault();
                    if (dtoProcess != null)
                    {
                        lastRemark = dtoProcess.Scomments;
                        dtoProcess.Senddate = pWorkFlow.Senddate;
                        genericRepo.Update<DTOModel.ProcessWorkFlow>(dtoProcess);
                    }
                    pWorkFlow.Senddate = null;
                    AddProcessWorkFlow(pWorkFlow);
                    /// for the retired employee only
                    if (pWorkFlow.StatusID == 4 && !string.IsNullOrEmpty(dtoObj.Email)) // resolved
                    {
                        if(tResponse=="none")
                        {
                            tResponse = lastRemark;
                        }
                        SendMailToTicketRaiser(dtoObj.Email, dtoObj.subject, tResponse, dtoObj.name, dtoObj.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public List<Ticket> GetPendingTickets(int receiverID)
        {
            log.Info($"TicketService/GetPendingTickets/ReceiverID={receiverID}");
            try
            {
                var getData = ticktRepo.GetPendingTickets(receiverID).OrderByDescending(x => x.priority_id);
                if (getData != null)
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetPendingTickets_Result, Ticket>()
                    .ForMember(d => d.ID, o => o.MapFrom(s => s.TicketNo))
                    .ForMember(d => d.name, o => o.MapFrom(s => s.name))
                    .ForMember(d => d.subject, o => o.MapFrom(s => s.subject))
                    .ForMember(d => d.Message, o => o.MapFrom(s => s.Message))
                    .ForMember(d => d.Department, o => o.MapFrom(s => s.DepartmentName))
                    .ForMember(d => d.Designation, o => o.MapFrom(s => s.DesignationName))
                    .ForMember(d => d.Remark, o => o.MapFrom(s => s.Scomments))
                    .ForMember(d => d.priority_id, o => o.MapFrom(s => s.priority_id))
                    .ForMember(d => d.customer_id, o => o.MapFrom(s => s.customer_id))
                    .ForMember(d => d.agent_id, o => o.MapFrom(s => s.agent_id))
                     .ForMember(d => d.PreviousTicket, o => o.MapFrom(s => s.PreviousTicket))
                    .ForAllOtherMembers(d => d.Ignore()));
                    var dtoTicket = Mapper.Map<List<Ticket>>(getData);
                    if (dtoTicket != null && dtoTicket.Count > 0)
                    {
                        var ticketID = 0;
                        for (int i = 0; i < dtoTicket.Count; i++)
                        {
                            ticketID = dtoTicket[i].ID;

                            var getDocumentData = genericRepo.Get<DTOModel.ticket_attachments>(x => x.ticketID == ticketID).ToList();
                            Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.ticket_attachments, TicketAttachment>());
                            var documentList = Mapper.Map<List<TicketAttachment>>(getDocumentData);
                            dtoTicket[i].TicketDocumentList = documentList;
                        }
                    }

                    return dtoTicket;
                }
                else
                    return new List<Ticket>();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public List<Ticket> GetAnsweredTickets(int senderID)
        {
            log.Info($"TicketService/GetAnsweredTickets/senderID={senderID}");
            try
            {
                var getData = ticktRepo.GetAnsweredTickets(senderID);
                if (getData != null)
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetAnsweredTickets_Result, Ticket>()
                   .ForMember(d => d.ID, o => o.MapFrom(s => s.TicketNo))
                    .ForMember(d => d.name, o => o.MapFrom(s => s.name))
                    .ForMember(d => d.subject, o => o.MapFrom(s => s.subject))
                    .ForMember(d => d.Message, o => o.MapFrom(s => s.Message))
                    .ForMember(d => d.Department, o => o.MapFrom(s => s.DepartmentName))
                    .ForMember(d => d.Designation, o => o.MapFrom(s => s.DesignationName))
                    .ForMember(d => d.Remark, o => o.MapFrom(s => s.Scomments))
                    .ForMember(d => d.priority_id, o => o.MapFrom(s => s.priority_id))
                     .ForMember(d => d.PreviousTicket, o => o.MapFrom(s => s.PreviousTicket))
                    .ForAllOtherMembers(d => d.Ignore()));
                    var dtoTicket = Mapper.Map<List<Ticket>>(getData);
                    return dtoTicket;
                }
                else
                    return new List<Ticket>();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public Ticket GetTicketResolveDtl(int ticketID)
        {
            log.Info($"TicketService/GetTicketResolveDtl/ticketID={ticketID}");
            try
            {
                var getData = ticktRepo.GetTicketAnswerDetail(ticketID);
                if (getData != null)
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.SP_GetTicketAnswerDetail_Result, Ticket>()
                   .ForMember(d => d.ID, o => o.MapFrom(s => s.ID))
                    .ForMember(d => d.name, o => o.MapFrom(s => s.SenerName))
                    .ForMember(d => d.TicketSolverRmk, o => o.MapFrom(s => s.Scomments))
                    .ForAllOtherMembers(d => d.Ignore()));
                    var dtoTicket = Mapper.Map<Ticket>(getData);
                    return dtoTicket;
                }
                else
                    return new Ticket();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public bool MarkTickerResolved(Model.ProcessWorkFlow pWorkFlow)
        {
            log.Info($"TicketService/MarkTickerResolved");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.ticket>(pWorkFlow.ReferenceID);
                if (dtoObj != null)
                {
                    dtoObj.status_id = pWorkFlow.StatusID;
                    dtoObj.UpdatedBy = pWorkFlow.UpdatedBy;
                    dtoObj.UpdatedOn = pWorkFlow.UpdatedOn;
                    genericRepo.Update<DTOModel.ticket>(dtoObj);
                    flag = true;
                }
                if (flag)
                {
                    var dtoProcess = genericRepo.Get<DTOModel.ProcessWorkFlow>(x => x.ReferenceID == pWorkFlow.ReferenceID).OrderByDescending(x => x.WorkflowID).FirstOrDefault();
                    if (dtoProcess != null)
                    {
                        dtoProcess.Senddate = pWorkFlow.Senddate;
                        genericRepo.Update<DTOModel.ProcessWorkFlow>(dtoProcess);
                    }
                    pWorkFlow.Senddate = null;
                    AddProcessWorkFlow(pWorkFlow);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        #endregion

        #region Ticket Work Flow
        public int InsertTWorkFlow(TicketWorkFlow tWrokFlow)
        {
            log.Info($"TicketService/InsertTWorkFlow");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.TicketWorkFlow, DTOModel.TicketWorkFlow>();
                });
                var dtoTicket = Mapper.Map<DTOModel.TicketWorkFlow>(tWrokFlow);
                genericRepo.Insert<DTOModel.TicketWorkFlow>(dtoTicket);
                return dtoTicket.WflowID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateTWorkFlow(TicketWorkFlow tWrokFlow)
        {
            log.Info($"TicketService/UpdateTWorkFlow");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.TicketWorkFlow>(tWrokFlow.WflowID);
                if (dtoObj != null)
                {
                    dtoObj.DepartmentID = tWrokFlow.DepartmentID;
                    dtoObj.DesignationID = tWrokFlow.DesignationID;
                    dtoObj.TicketTypeID = tWrokFlow.TicketTypeID;
                    dtoObj.UpdatedBy = tWrokFlow.UpdatedBy;
                    dtoObj.UpdatedOn = tWrokFlow.UpdatedOn;
                    genericRepo.Update<DTOModel.TicketWorkFlow>(dtoObj);
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

        public List<Model.TicketWorkFlow> GetTicketWorkFlowList()
        {
            log.Info($"TicketService/GetTicketWorkFlowList");
            try
            {
                var result = genericRepo.Get<DTOModel.TicketWorkFlow>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TicketWorkFlow, Model.TicketWorkFlow>()
                    .ForMember(d => d.WflowID, o => o.MapFrom(s => s.WflowID))
                    .ForMember(d => d.Designation, o => o.MapFrom(s => s.Designation.DesignationName))
                    .ForMember(d => d.Department, o => o.MapFrom(s => s.Department.DepartmentName))
                    .ForMember(d => d.TicketType, o => o.MapFrom(s => s.ticket_type.code));
                });
                var listTWorkFlow = Mapper.Map<List<Model.TicketWorkFlow>>(result);
                return listTWorkFlow;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public Model.TicketWorkFlow GetTicketWorkFlowByID(int tWrokFlowID)
        {
            log.Info($"TicketService/GetTicketWorkFlowByID/{tWrokFlowID}");
            try
            {
                var TicketObj = genericRepo.GetByID<DTOModel.TicketWorkFlow>(tWrokFlowID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TicketWorkFlow, Model.TicketWorkFlow>();
                });
                var obj = Mapper.Map<Model.TicketWorkFlow>(TicketObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        #endregion

        #region Ticket Priority
        public bool SetTicketPriority(Ticket ticket)
        {
            log.Info($"TicketService/SetTicketPriority");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.ticket>(ticket.ID);
                if (dtoObj != null)
                {
                    dtoObj.priority_id = ticket.priority_id;
                    dtoObj.UpdatedBy = ticket.UpdatedBy;
                    dtoObj.UpdatedOn = ticket.UpdatedOn;
                    genericRepo.Update<DTOModel.ticket>(dtoObj);
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
        #endregion

        public List<TicketForwardDetails> GetTicketForwardDetails(int ticketID)
        {
            log.Info($"GetTicketForwardDetails/ticketID={ticketID}");
            try
            {
                var getfwdDtls = ticktRepo.GetTicketForwardDetails(ticketID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetTicketForwardDetails_Result, TicketForwardDetails>()
                    .ForMember(d => d.Remarks, o => o.MapFrom(s => s.Scomments))
                    ;
                }
                );
                var dtofwrdDtls = Mapper.Map<List<TicketForwardDetails>>(getfwdDtls);
                return dtofwrdDtls;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool TicketRollback(Model.ProcessWorkFlow pWorkFlow)
        {
            log.Info($"TicketService/TicketRollback");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.ticket>(pWorkFlow.ReferenceID);
                if (dtoObj != null)
                {
                    dtoObj.status_id = pWorkFlow.StatusID;
                    dtoObj.UpdatedBy = pWorkFlow.UpdatedBy;
                    dtoObj.UpdatedOn = pWorkFlow.UpdatedOn;
                    genericRepo.Update<DTOModel.ticket>(dtoObj);
                    flag = true;
                }
                if (flag)
                {
                    var dtoProcess = genericRepo.Get<DTOModel.ProcessWorkFlow>(x => x.ReferenceID == pWorkFlow.ReferenceID && x.ProcessID == pWorkFlow.ProcessID).OrderByDescending(x => x.WorkflowID).FirstOrDefault();
                    if (dtoProcess != null)
                    {
                        dtoProcess.Senddate = pWorkFlow.Senddate;
                        genericRepo.Update<DTOModel.ProcessWorkFlow>(dtoProcess);
                    }
                    pWorkFlow.SenderID = dtoProcess.ReceiverID;
                    pWorkFlow.Senddate = null;
                    AddProcessWorkFlow(pWorkFlow);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool CheckEmployeeExist(string empCode, DateTime doj, int depId, int branchId,DateTime lastWorking, out int empId, out int desgId)
        {
            log.Info($"TicketService/TicketRollback");

            try
            {
                var dateofjoin = Convert.ToDateTime(doj);
                var dateoflastWorking = Convert.ToDateTime(lastWorking);
                var flag = ticktRepo.CheckEmployeeExist(empCode, dateofjoin, depId, branchId, dateoflastWorking);
                if (flag)
                {
                    var getEmp = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeCode == empCode &&  x.DepartmentID == depId && x.BranchID == branchId).FirstOrDefault();
                    desgId = getEmp.DesignationID;
                    empId = getEmp.EmployeeId;
                    return true;
                }
                else
                {
                    desgId = 0;
                    empId = 0;
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        private bool SendMailToTicketRaiser(string mailId, string ticketSub, string tResponse, string empName,int ticketNo)
        {
            log.Info($"ConfirmationFormService/SendMailToTicketRaiser");
            bool flag = false;
            try
            {
                StringBuilder emailBody = new StringBuilder("");
                if (!String.IsNullOrEmpty(mailId))
                {
                    emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                 + $"<p>Your Tiket No. is {ticketNo}. This is to intimate that your ticket has been resolved.</p>"
                 + $"<p>Respose : {tResponse} </p>");
                    emailBody.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");
                    emailBody.AppendFormat("<div class='font-weight-italic'> <p>This is system generated mail, do not reply on this </p> </div>");

                    Common.EmailMessage message = new Common.EmailMessage();
                    message.To = mailId;
                    message.Body = emailBody.ToString();
                    message.Subject = ticketSub;

                    Task t2 = Task.Run(() => SendEmail(message));
                }
                flag = true;
            }
            catch
            {
                flag = true;
            }
            return flag;
        }

        void SendEmail(Common.EmailMessage message)
        {
            try
            {
                var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                message.From = "NAFED HRMS <" + emailsetting.ToEmail + ">";
                message.UserName = emailsetting.UserName;
                message.Password = emailsetting.Password;
                message.SmtpClientHost = emailsetting.Server;
                message.SmtpPort = emailsetting.Port;
                message.enableSSL = emailsetting.SSLStatus;
                message.HTMLView = true;
                message.FriendlyName = "NAFED";
                Common.EmailHelper.SendEmail(message);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

    }
}
