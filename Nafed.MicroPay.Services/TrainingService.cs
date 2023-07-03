using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Common;
using static Nafed.MicroPay.ImportExport.TrainingReportsExport;
using System.IO;
using System.Data;

namespace Nafed.MicroPay.Services
{
    public class TrainingService : BaseService, ITrainingService
    {
        private readonly IGenericRepository genericRepo;
        private readonly ITrainingRepository trainingRepo;
        public TrainingService(IGenericRepository genericRepo, ITrainingRepository trainingRepo)
        {
            this.genericRepo = genericRepo;
            this.trainingRepo = trainingRepo;
        }

        public Training GetTrainingDtls(int trainingID)
        {
            log.Info($"TrainingService/GetTrainingDtls/{trainingID}");
            try
            {
                var dtoTraining = genericRepo.GetByID<DTOModel.Training>(trainingID);
                if (dtoTraining != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.Training, Training>()
                        .ForMember(d => d.StateName, o => o.MapFrom(s => s.State.StateName))
                        .ForMember(d => d.enumTrainingList, o => o.MapFrom(s => (EnumTrainingList)s.TrainingList));
                    });
                    return Mapper.Map<Training>(dtoTraining);

                }
                else
                {
                    return new Training();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Training> GetTrainingList(int? year = null, int? month = null)
        {
            log.Info($"TrainingService/GetTrainingList");
            try
            {
                var getTrainingList = genericRepo.Get<DTOModel.Training>(x => (month.HasValue ? month.Value == x.StartDate.Month : (1 > 0)) && (year.HasValue ? year.Value == x.StartDate.Year : (1 > 0)) && !x.IsDeleted).ToList();
                if (getTrainingList != null && getTrainingList.Count > 0)
                {

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.Training, Training>()
                        .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                        .ForMember(d => d.StartDate, o => o.MapFrom(s => s.StartDate))
                        .ForMember(d => d.EndDate, o => o.MapFrom(s => s.EndDate))
                        .ForMember(d => d.Address, o => o.MapFrom(s => s.Address))
                        .ForMember(d => d.City, o => o.MapFrom(s => s.City))
                        .ForMember(d => d.StateName, o => o.MapFrom(s => s.State.StateName))
                        .ForMember(d => d.PinCode, o => o.MapFrom(s => s.PinCode))
                        .ForMember(d => d.TrainingStatus, o => o.MapFrom(s => s.TrainingStatus))
                        .ForMember(d => d.enumTrainingList, o => o.MapFrom(s => (EnumTrainingList)s.TrainingList))
                        .ForMember(d => d.VendorName, o => o.MapFrom(s => s.VendorName))
                        .ForMember(d => d.TrainingType, o => o.MapFrom(s => s.TrainingType))
                        .ForMember(d => d.TrainingObjective, o => o.MapFrom(s => s.TrainingObjective))
                        .ForMember(d => d.ModeofTraining, o => o.MapFrom(s => s.ModeofTraining))
                        .ForMember(d => d.StartDateFromTime, o => o.MapFrom(s => s.StartDateFromTime))
                        .ForMember(d => d.StartDateToTime, o => o.MapFrom(s => s.StartDateToTime))
                        .ForMember(d => d.InternalTrainer, o => o.MapFrom(s => s.InternalTraining))
                        .ForMember(d => d.ExternalTrainer, o => o.MapFrom(s => s.ExternalTraining))
                        .ForMember(d => d.BothTrainer, o => o.MapFrom(s => s.BothTraining))
                         .ForMember(d => d.trainerList, o => o.MapFrom(s => s.Trainers))
                        ;
                        cfg.CreateMap<DTOModel.Trainer, Trainer>();
                    });
                    var dtoTrainingList = Mapper.Map<List<Training>>(getTrainingList);

                    return dtoTrainingList;
                }
                else
                    return new List<Training>();
            }
            catch (Exception)
            {
                throw;
            }

        }
        public TrainingSchedule GetTrainingDetail(int trainingID)
        {
            log.Info($"TrainingService/GetTrainingDetail");
            try
            {
                TrainingSchedule trn = new TrainingSchedule();

                var trainingDtl = genericRepo.GetByID<DTOModel.Training>(trainingID);

                if (trainingDtl != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.Training, Training>()

                    .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                    .ForMember(d => d.StateName, o => o.MapFrom(s => s.State.StateName))
                     .ForMember(d => d.OtherTopic, o => o.MapFrom(s => s.OtherTopic))
                       .ForMember(d => d.TimeSlotType, o => o.MapFrom(s => s.TimeSlotType))
                    .ForMember(d => d.enumTrainingList, o => o.MapFrom(s => (EnumTrainingList)s.TrainingList))
                        ;
                        cfg.CreateMap<DTOModel.TrainingTopic, TrainingTopic>()
                        .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                        .ForMember(d => d.SkillTypeID, o => o.MapFrom(s => s.SkillTypeID))
                        .ForMember(d => d.SkillID, o => o.MapFrom(s => s.SkillID))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                    var dtoAPARSkillDtlList = Mapper.Map<Training>(trainingDtl);
                    var dtoAPARS = Mapper.Map<List<TrainingTopic>>(trainingDtl.TrainingTopics);
                    trn._Training = dtoAPARSkillDtlList;
                    trn.TrainingTopicList = dtoAPARS;
                }
                return trn;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public int InsertGeneralTabDetail(TrainingSchedule trnSchedule)
        {
            log.Info($"TrainingService/InsertGeneralTabDetail");
            try
            {
                Mapper.Initialize(cfg =>
                cfg.CreateMap<Training, DTOModel.Training>()
                );

                var dtoTraining = Mapper.Map<DTOModel.Training>(trnSchedule._Training);

                dtoTraining.TrainingList = (short)trnSchedule._Training.enumTrainingList;

                if (trnSchedule._Training.enumResidentialNonResidential == EnumResidentialNonResidential.NonResidential)
                    dtoTraining.OnBoardTraining = true;
                else if (trnSchedule._Training.enumResidentialNonResidential == EnumResidentialNonResidential.Residential)
                    dtoTraining.OrientationTraining = true;
                else if (trnSchedule._Training.enumResidentialNonResidential == EnumResidentialNonResidential.internaltraining)
                    dtoTraining.InternalAddressType = true;

                var res = genericRepo.Insert(dtoTraining);

                if (dtoTraining.TrainingID > 0)
                {
                    //=== region to add datewise training time slot ====  (Added On- 16-Oct-20 , SG)

                    if (genericRepo.Exists<DTOModel.TrainingDateWiseTimeSlot>(x => x.TrainingID == dtoTraining.TrainingID))
                    {
                        var lastDefinedSlots = genericRepo.Get<DTOModel.TrainingDateWiseTimeSlot>(x => x.TrainingID == dtoTraining.TrainingID).ToList();
                        genericRepo.RemoveMultipleEntity<DTOModel.TrainingDateWiseTimeSlot>(lastDefinedSlots);
                    }
                    Mapper.Initialize(cfg =>
                      cfg.CreateMap<TrainingDateWiseTimeSlot, DTOModel.TrainingDateWiseTimeSlot>()
                      .ForMember(d => d.TrainingID, o => o.UseValue(dtoTraining.TrainingID)));

                    var dtoTimeSlots = Mapper.Map<List<DTOModel.TrainingDateWiseTimeSlot>>(trnSchedule._Training.distributedTimeSlots);

                    genericRepo.AddMultipleEntity<DTOModel.TrainingDateWiseTimeSlot>(dtoTimeSlots);

                    ///========end =============================
                    ///
                    if (trnSchedule.CheckBoxListBehavioral != null)
                    {
                        Mapper.Initialize(cfg =>
                        cfg.CreateMap<int, DTOModel.TrainingTopic>()
                        .ForMember(d => d.SkillTypeID, o => o.UseValue(2))
                        .ForMember(d => d.SkillID, o => o.MapFrom(s => s))
                        .ForMember(d => d.TrainingID, o => o.UseValue(dtoTraining.TrainingID))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoTraining.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                        .ForAllOtherMembers(d => d.Ignore())
                        );
                        var dtoBehavioral = Mapper.Map<List<DTOModel.TrainingTopic>>(trnSchedule.CheckBoxListBehavioral);
                        genericRepo.AddMultiple(dtoBehavioral);
                    }
                    if (trnSchedule.CheckBoxListFunctional != null)
                    {
                        Mapper.Initialize(cfg =>
                        cfg.CreateMap<int, DTOModel.TrainingTopic>()
                        .ForMember(d => d.SkillTypeID, o => o.UseValue(3))
                        .ForMember(d => d.SkillID, o => o.MapFrom(s => s))
                        .ForMember(d => d.TrainingID, o => o.UseValue(dtoTraining.TrainingID))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoTraining.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                        .ForAllOtherMembers(d => d.Ignore())
                        );
                        var dtoFunctional = Mapper.Map<List<DTOModel.TrainingTopic>>(trnSchedule.CheckBoxListFunctional);
                        genericRepo.AddMultiple(dtoFunctional);
                    }

                }
                return dtoTraining.TrainingID;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public int UpdateGeneralTabDetail(TrainingSchedule trnSchedule)
        {
            log.Info($"TrainingService/UpdateGeneralTabDetail");
            try
            {
                int flag = 0;

                var getdata = genericRepo.Get<DTOModel.Training>(x => x.TrainingID == trnSchedule._Training.TrainingID).FirstOrDefault();
                if (getdata != null)
                {
                    getdata.TrainingType = trnSchedule._Training.TrainingType;
                    getdata.StartDate = (DateTime)trnSchedule._Training.StartDate;
                    getdata.StartDateFromTime = trnSchedule._Training.StartDateFromTime;
                    getdata.StartDateToTime = trnSchedule._Training.StartDateToTime;
                    getdata.EndDate = (DateTime)trnSchedule._Training.EndDate;
                    // getdata.EndDateFromTime = trnSchedule._Training.EndDateFromTime;
                    //  getdata.EndDateToTime = trnSchedule._Training.EndDateToTime;
                    getdata.TrainingObjective = trnSchedule._Training.TrainingObjective;
                    getdata.Address = trnSchedule._Training.Address;
                    getdata.StateID = trnSchedule._Training.StateID;
                    getdata.City = trnSchedule._Training.City;
                    getdata.PinCode = trnSchedule._Training.PinCode;
                    getdata.TrainingCost = trnSchedule._Training.TrainingCost;
                    getdata.PostTrainingInvesment = trnSchedule._Training.PostTrainingInvesment;
                    getdata.UpdatedBy = trnSchedule._Training.UpdatedBy;
                    getdata.UpdatedOn = trnSchedule._Training.UpdatedOn;
                    getdata.TrainingTitle = trnSchedule._Training.TrainingTitle;
                    getdata.TrainingList = (short)trnSchedule._Training.enumTrainingList;
                    getdata.TrainingContent = trnSchedule._Training.TrainingContent;
                    getdata.ModeofTraining = trnSchedule._Training.ModeofTraining;
                    getdata.OtherTopic = trnSchedule._Training.OtherTopic;

                    getdata.TimeSlotType = trnSchedule._Training.TimeSlotType;
                    getdata.NominationDate = (DateTime?)trnSchedule._Training.NominationDate ?? null;

                    if (trnSchedule._Training.enumResidentialNonResidential == EnumResidentialNonResidential.NonResidential)
                    {
                        getdata.OnBoardTraining = true;
                        getdata.OrientationTraining = false;
                        getdata.InternalAddressType = false;
                    }
                    else if (trnSchedule._Training.enumResidentialNonResidential == EnumResidentialNonResidential.Residential)
                    {
                        getdata.OrientationTraining = true;
                        getdata.OnBoardTraining = false;
                        getdata.InternalAddressType = false;
                    }
                    else if (trnSchedule._Training.enumResidentialNonResidential == EnumResidentialNonResidential.internaltraining)
                    {
                        getdata.OrientationTraining = false;
                        getdata.OnBoardTraining = false;
                        getdata.InternalAddressType = true;
                    }
                    getdata.Remark = trnSchedule._Training.Remark;
                    Mapper.Initialize(cfg =>
                          cfg.CreateMap<TrainingSchedule, DTOModel.Training>()
                          );
                    var dtotraining = Mapper.Map<DTOModel.Training>(getdata);
                    genericRepo.Update(dtotraining);

                    flag = getdata.TrainingID;

                    if (getdata.TrainingID > 0)
                    {

                        //=== region to add datewise training time slot ====  (Added On- 16-Oct-20 , SG)

                        if (genericRepo.Exists<DTOModel.TrainingDateWiseTimeSlot>(x => x.TrainingID == flag))
                        {
                            var lastDefinedSlots = genericRepo.Get<DTOModel.TrainingDateWiseTimeSlot>(x => x.TrainingID == flag).ToList();
                            genericRepo.DeleteAll<DTOModel.TrainingDateWiseTimeSlot>(lastDefinedSlots);
                        }
                        Mapper.Initialize(cfg =>
                          cfg.CreateMap<TrainingDateWiseTimeSlot, DTOModel.TrainingDateWiseTimeSlot>()
                          .ForMember(d => d.TrainingID, o => o.UseValue(flag)));

                        var dtoTimeSlots = Mapper.Map<List<DTOModel.TrainingDateWiseTimeSlot>>(trnSchedule._Training.distributedTimeSlots);

                        genericRepo.AddMultipleEntity<DTOModel.TrainingDateWiseTimeSlot>(dtoTimeSlots);

                        ///========end =============================
                        ///
                        if (trnSchedule.CheckBoxListBehavioral != null)
                        {
                            Mapper.Initialize(cfg =>
                           cfg.CreateMap<int, DTOModel.TrainingTopic>()
                           .ForMember(d => d.SkillTypeID, o => o.UseValue(2))
                           .ForMember(d => d.SkillID, o => o.MapFrom(s => s))
                           .ForMember(d => d.TrainingID, o => o.UseValue(getdata.TrainingID))
                           .ForMember(d => d.CreatedBy, o => o.UseValue(getdata.UpdatedBy))
                           .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                           .ForAllOtherMembers(d => d.Ignore())
                           );
                            var dtoBehavioral = Mapper.Map<List<DTOModel.TrainingTopic>>(trnSchedule.CheckBoxListBehavioral);
                            var getBehavioral = genericRepo.Get<DTOModel.TrainingTopic>(x => x.TrainingID == getdata.TrainingID && x.SkillTypeID == 2).ToList<DTOModel.TrainingTopic>();

                            if (getBehavioral != null || getBehavioral.Count > 0)
                                genericRepo.DeleteAll<DTOModel.TrainingTopic>(getBehavioral);
                            if (dtoBehavioral != null)
                                genericRepo.AddMultiple<DTOModel.TrainingTopic>(dtoBehavioral);
                        }
                        if (trnSchedule.CheckBoxListFunctional != null)
                        {
                            Mapper.Initialize(cfg =>
                           cfg.CreateMap<int, DTOModel.TrainingTopic>()
                           .ForMember(d => d.SkillTypeID, o => o.UseValue(3))
                           .ForMember(d => d.SkillID, o => o.MapFrom(s => s))
                           .ForMember(d => d.TrainingID, o => o.UseValue(getdata.TrainingID))
                           .ForMember(d => d.CreatedBy, o => o.UseValue(getdata.UpdatedBy))
                           .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                           .ForAllOtherMembers(d => d.Ignore())
                           );
                            var dtoFunctional = Mapper.Map<List<DTOModel.TrainingTopic>>(trnSchedule.CheckBoxListFunctional);
                            var getFunctional = genericRepo.Get<DTOModel.TrainingTopic>(x => x.TrainingID == getdata.TrainingID && x.SkillTypeID == 3).ToList<DTOModel.TrainingTopic>();
                            if (getFunctional != null || getFunctional.Count > 0)
                                genericRepo.DeleteAll<DTOModel.TrainingTopic>(getFunctional);
                            if (dtoFunctional != null)
                                genericRepo.AddMultiple(dtoFunctional);
                        }

                    }
                    return flag;
                }
                else
                {
                    return flag = 0;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Training GetTrainerDetails(int trainingID)
        {
            log.Info($"TrainingService/GetTrainerDetails/{trainingID}");

            Training trainingInfo = new Training();
            try
            {
                var dtoTrainingInfo = genericRepo.GetByID<DTOModel.Training>(trainingID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Training, Training>()
                   .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                   .ForMember(d => d.VendorName, o => o.MapFrom(s => s.VendorName))
                   .ForMember(d => d.VendorPhoneNo, o => o.MapFrom(s => s.VendorPhoneNo))
                   .ForMember(d => d.VendorAddress, o => o.MapFrom(s => s.VendorAddress))
                   .ForMember(d => d.VendorGSTINNo, o => o.MapFrom(s => s.VendorGSTINNo))
                   .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                   .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                   .ForMember(d => d.InternalTrainer, o => o.MapFrom(s => s.InternalTraining))
                   .ForMember(d => d.ExternalTrainer, o => o.MapFrom(s => s.ExternalTraining))
                   .ForMember(d => d.BothTrainer, o => o.MapFrom(s => s.BothTraining))
                   .ForMember(d => d.TrainingStatus, o => o.MapFrom(s => s.TrainingStatus))
                   .ForMember(d => d.trainerList, o => o.MapFrom(s => s.Trainers))
                   .ForMember(d => d.enumTrainingList, o => o.MapFrom(s => (EnumTrainingList)s.TrainingList))
                   .ForMember(d => d.OtherTopic, o => o.MapFrom(s => s.OtherTopic))
                    ;
                    cfg.CreateMap<DTOModel.Trainer, Trainer>();
                });
                trainingInfo = Mapper.Map<Training>(dtoTrainingInfo);
            }
            catch (Exception)
            {
                throw;
            }
            return trainingInfo;
        }

        public bool UpdateTrainerDetails(Training training)
        {

            log.Info($"TrainingService/UpdateTrainerDetails/");

            bool flag = false;

            try
            {
                var dtoTraining = genericRepo.GetByID<DTOModel.Training>(training.TrainingID);
                dtoTraining.ExternalTraining = training.ExternalTrainer;
                dtoTraining.InternalTraining = training.InternalTrainer;
                dtoTraining.BothTraining = training.BothTrainer;
                dtoTraining.UpdatedBy = training.UpdatedBy;
                dtoTraining.UpdatedOn = training.UpdatedOn;

                if (training.BothTrainer)
                {
                    dtoTraining.VendorName = training.VendorName;
                    dtoTraining.VendorPhoneNo = training.VendorPhoneNo;
                    dtoTraining.VendorAddress = training.VendorAddress;
                    dtoTraining.VendorGSTINNo = training.VendorGSTINNo;
                    dtoTraining.DirectorName = training.DirectorName;
                }
                genericRepo.Update<DTOModel.Training>(dtoTraining);

                if (genericRepo.Exists<DTOModel.Trainer>(x => x.TrainingID == training.TrainingID))
                {
                    var prevTrainerInfo = genericRepo.Get<DTOModel.Trainer>(x => x.TrainingID == training.TrainingID).ToList();
                    genericRepo.DeleteAll<DTOModel.Trainer>(prevTrainerInfo);
                }

                if (training.trainerList?.Count > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Trainer, DTOModel.Trainer>()
                       .ForMember(d => d.TrainingID, o => o.UseValue(training.TrainingID))
                       .ForMember(d => d.CreatedBy, o => o.UseValue(training.UpdatedBy))
                       .ForMember(d => d.CreatedOn, o => o.UseValue(training.UpdatedOn));
                    });

                    var dtoTrainerInfo = Mapper.Map<List<DTOModel.Trainer>>(training.trainerList);

                    genericRepo.AddMultipleEntity<DTOModel.Trainer>(dtoTrainerInfo);
                }
                flag = true;

            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        #region Training Participants
        public List<Model.TrainingParticipantsDetail> GetTrainingParticipantsDetailsList(string employeeCode, string employeeName, int? departmentId)
        {
            log.Info($"TrainingService/GetTrainingParticipantsDetailsList/{employeeCode}/{employeeName}/{departmentId}");
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted && (departmentId.HasValue ? x.DepartmentID == departmentId.Value : 1 > 0)
                && (employeeCode != null ? x.EmployeeCode == employeeCode : 1 > 0) && (employeeName != null ? x.Name.Trim().ToLower() == employeeName.Trim().ToLower() : 1 > 0) && !x.IsDeleted && x.DOLeaveOrg == null && x.EmployeeTypeID == 5);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.TrainingParticipantsDetail>()
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeId))
                    .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name))
                    .ForMember(d => d.EmployeeBranch, o => o.MapFrom(s => s.Branch.BranchName))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.DepartmentName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.Designation.DesignationName))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var listTrainingParticipantsDetails = Mapper.Map<List<Model.TrainingParticipantsDetail>>(result);
                return listTrainingParticipantsDetails.OrderBy(x => x.EmployeeName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool PostTrainingParticipants(List<Model.TrainingParticipant> participantsData, bool? isNomination = null, ProcessWorkFlow process = null, TrainingSchedule trnSchedule = null)
        {
            log.Info($"TrainingService/PostTrainingParticipants");
            bool flag = false;
            try
            {                
                var TrainingID = participantsData.Select(z => z.TrainingID).FirstOrDefault();
                var prList = participantsData.Select(x => x.EmployeeID).ToArray();

                var getexistpartcipant = genericRepo.Get<DTOModel.TrainingParticipant>(x => prList.Any(y => y == x.EmployeeID) && x.TrainingID == TrainingID).ToList();
                if (getexistpartcipant != null && getexistpartcipant.Count > 0)
                    genericRepo.DeleteAll<DTOModel.TrainingParticipant>(getexistpartcipant);


                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.TrainingParticipant, DTOModel.TrainingParticipant>()
                    .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.Nomination, o => o.MapFrom(s => s.Nomination))
                    .ForMember(d => d.NominationAccepted, o => o.MapFrom(s => s.NominationAccepted))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var dtoTrainingParticipant = Mapper.Map<List<DTOModel.TrainingParticipant>>(participantsData);
                genericRepo.AddMultipleEntity<DTOModel.TrainingParticipant>(dtoTrainingParticipant);
                flag = true;
                if (flag && isNomination.HasValue)
                {
                    AddProcessWorkFlow(process);
                    SendTrainingRelatedEmail(trnSchedule, "Nomination");
                    //// Send mail to receiver.
                    trnSchedule.EmployeeID = (int)process.ReceiverID;
                    SendTrainingRelatedEmail(trnSchedule, "NomReceiver");
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.TrainingParticipant> GetTrainingParticipantsList(int? trainingID)

        {
            log.Info($"TrainingService/PostTrainingParticipants");
            try
            {
                var result = genericRepo.Get<DTOModel.TrainingParticipant>(x => x.TrainingID == trainingID && (x.Nomination == false || x.NominationAccepted == 8));

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TrainingParticipant, Model.TrainingParticipant>()
                    .ForMember(d => d.TrainingParticipantID, o => o.MapFrom(s => s.TrainingParticipantID))
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(d => d.EmployeeBranch, o => o.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                    .ForMember(d => d.TrainingName, o => o.MapFrom(s => s.Training.SkillType.SkillType1))
                    .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                    .ForMember(d => d.TrainingAttended, o => o.MapFrom(s => s.TrainingAttended))
                    .ForMember(d => d.FeedbackFormStatus, o => o.MapFrom(s => s.FeedbackFormStatus))
                    .ForMember(d => d.EmailID, o => o.MapFrom(s => s.tblMstEmployee.OfficialEmail))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.UpdateOn, o => o.MapFrom(s => s.UpdateOn))
                    .ForMember(d => d.UpdateBy, o => o.MapFrom(s => s.UpdateBy))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var listTrainingParticipants = Mapper.Map<List<Model.TrainingParticipant>>(result);
                return listTrainingParticipants.OrderBy(x => x.EmployeeName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool DeleteParticipant(int trainingParticipantID)
        {
            log.Info($"TrainingService/DeleteParticipant");
            bool flag = false;
            try
            {
                genericRepo.Delete<DTOModel.TrainingParticipant>(trainingParticipantID);

                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool AddTrainingParticipantAttendance(TrainingSchedule training)
        {
            log.Info($"TrainingService/AddTrainingParticipantAttendance");
            bool flag = false;
            try
            {
                //if (tParticipants.Count > 0)
                //{
                //    foreach (var item in tParticipants)
                //    {
                //        var dtoRow = genericRepo.Get<DTOModel.TrainingParticipant>(x => x.TrainingParticipantID == item.TrainingParticipantID).FirstOrDefault();
                //        dtoRow.TrainingAttended = item.TrainingAttended;
                //        dtoRow.UpdateBy = item.UpdateBy;
                //        dtoRow.UpdateOn = item.UpdateOn;
                //        genericRepo.Update<DTOModel.TrainingParticipant>(dtoRow);
                //    }
                //}

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<TrainingParticipant, DTOModel.TrainingParticipant>()
                           .ForMember(d => d.TrainingParticipantID, o => o.MapFrom(s => s.TrainingParticipantID))
                           .ForMember(d => d.UpdateBy, o => o.MapFrom(s => s.UpdateBy))
                           .ForMember(d => d.UpdateOn, o => o.MapFrom(s => s.UpdateOn))
                           .ForMember(d => d.TrainingAttended, o => o.MapFrom(s => s.TrainingAttended));
                });
                var dtoTrainingParticipants = Mapper.Map<List<DTOModel.TrainingParticipant>>(training.TrainingParticipntList);
                bool result = trainingRepo.AddTrainingParticipantAttendance(dtoTrainingParticipants);
                Task t1 = Task.Run(() => SendTrainingRelatedEmail(training, "Completed")); ///=== Send Email to those participants ,who attended the training..

                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public void SendTrainingAttendanceEmail(List<TrainingParticipant> tParticipants)
        {
            log.Info($"TrainingService/SendTrainingAttendanceEmail");
            try
            {
                EmailMessage emailMessage = new EmailMessage();
                var emailSubject = "Training Attendance Notification";
                var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                if (emailsetting != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.EmailConfiguration, EmailMessage>()
                        .ForMember(d => d.From, o => o.MapFrom(s => $"NAFED HRMS <{s.ToEmail}>"))
                        .ForMember(d => d.Subject, o => o.UseValue(emailSubject))
                        .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
                        .ForMember(d => d.Password, o => o.MapFrom(s => s.Password))
                        .ForMember(d => d.SmtpClientHost, o => o.MapFrom(s => s.Server))
                        .ForMember(d => d.SmtpPort, o => o.MapFrom(s => s.Port))
                        .ForMember(d => d.enableSSL, o => o.MapFrom(s => s.SSLStatus))
                        .ForMember(d => d.HTMLView, o => o.UseValue(true))
                        .ForMember(d => d.FriendlyName, o => o.UseValue("NAFED"));
                    });
                    emailMessage = Mapper.Map<EmailMessage>(emailsetting);

                    foreach (var item in tParticipants)
                    {
                        StringBuilder mailBody = new StringBuilder();
                        mailBody.AppendFormat($"<div>Dear {item.EmployeeName},</div> <br> <br>");
                        mailBody.AppendFormat($"<div>This is to inform you that your Application Form for </b></div> <br> <br>");
                        mailBody.AppendFormat($"<div>Regards, </div> <br>");
                        mailBody.AppendFormat($"<div>Nafed  </div> <br> <br>");
                        emailMessage.To = item.EmailID;
                        emailMessage.Body = mailBody.ToString();
                        EmailHelper.SendEmail(emailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        #endregion Training Participants

        #region Training Document
        public List<Model.TrainingDocumentRepository> GetTrainingDocumentList(int? trainingID)
        {
            log.Info($"TrainingService/GetTrainingDocumentList/{trainingID}");
            try
            {
                var result = genericRepo.Get<DTOModel.TrainingDocumentRepository>(x => x.TrainingID == trainingID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TrainingDocumentRepository, Model.TrainingDocumentRepository>()
                    .ForMember(d => d.TrainingDocumentID, o => o.MapFrom(s => s.TrainingDocumentID))
                    .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                    .ForMember(d => d.DocumentName, o => o.MapFrom(s => s.DocumentName))
                    .ForMember(d => d.DocumentPathName, o => o.MapFrom(s => s.DocumentPathName))
                    .ForMember(d => d.DocumentDetail, o => o.MapFrom(s => s.DocumentDetail))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var listTrainingDocument = Mapper.Map<List<Model.TrainingDocumentRepository>>(result);
                return listTrainingDocument.ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SaveTrainingDocument(TrainingDocumentRepository trainingDocument)
        {
            log.Info($"TrainingService/SaveTrainingDocument");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.TrainingDocumentRepository, DTOModel.TrainingDocumentRepository>()
                    .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                    .ForMember(d => d.DocumentName, o => o.MapFrom(s => s.DocumentName))
                    .ForMember(d => d.DocumentDetail, o => o.MapFrom(s => s.DocumentDetail))
                    .ForMember(d => d.DocumentPathName, o => o.MapFrom(s => s.DocumentPathName))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var dtoTrainingDocument = Mapper.Map<DTOModel.TrainingDocumentRepository>(trainingDocument);
                genericRepo.Insert<DTOModel.TrainingDocumentRepository>(dtoTrainingDocument);
                flag = true;
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public bool DeleteTrainingDocument(int trainingDocumentID)
        {
            log.Info($"TrainingService/DeleteTrainingDocument");
            bool flag = false;
            try
            {
                genericRepo.Delete<DTOModel.TrainingDocumentRepository>(trainingDocumentID);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool InsertDeletedTrainingParticipants(int userId, int trainingParticipantID)
        {
            log.Info($"TrainingService/InsertDeletedTrainingParticipants");
            bool flag = false;
            try
            {
                trainingRepo.InsertDeletedTrainingParticipants(userId, trainingParticipantID);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);

            }
            return flag;
        }

        #endregion Training Document

        #region TrainingPrerequisite

        public TrainingSchedule GetTrainingPrerequisiteByID(int trainingID)
        {
            log.Info($"TrainingPrerequisiteService/TrainingPrerequisite");
            try
            {

                TrainingSchedule trn = new TrainingSchedule();

                var trainingDtl = genericRepo.GetByID<DTOModel.Training>(trainingID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Training, Training>()

                    .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                    ;
                    cfg.CreateMap<DTOModel.TrainingPrerequisite, TrainingPrerequisite>()
                    .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                    .ForMember(d => d.Item, o => o.MapFrom(s => s.Item))
                    .ForMember(d => d.SerialNo, o => o.MapFrom(s => s.SerialNo))
                    .ForMember(d => d.Make, o => o.MapFrom(s => s.Make))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var dtotrainingprerequisiteList = Mapper.Map<Training>(trainingDtl);
                var dtoTP = Mapper.Map<List<TrainingPrerequisite>>(trainingDtl.TrainingPrerequisites);
                trn._Training = dtotrainingprerequisiteList;
                trn.TrainingPrereqList = dtoTP;
                return trn;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertTrainingPrerequisite(TrainingSchedule trainingSchedule)
        {
            log.Info($"TrainingService/InsertTrainingPrerequisite");
            bool flag = false;
            try
            {
                var getdata = genericRepo.Get<DTOModel.Training>(x => x.TrainingID == trainingSchedule._Training.TrainingID).FirstOrDefault();
                if (getdata != null)
                {
                    //  flag = getdata.TrainingID;

                    if (getdata.TrainingID > 0)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<TrainingPrerequisite, DTOModel.TrainingPrerequisite>()
                            .ForMember(d => d.TrainingID, o => o.UseValue(trainingSchedule._Training.TrainingID))
                            .ForMember(d => d.Item, o => o.MapFrom(s => s.Item))
                            .ForMember(d => d.SerialNo, o => o.MapFrom(s => s.SerialNo))
                            .ForMember(d => d.Make, o => o.MapFrom(s => s.Make))
                             .ForMember(d => d.CreatedBy, o => o.UseValue(getdata.CreatedBy))
                            .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now));
                        });
                        var dtoTrainingPrerequisiteDtls = Mapper.Map<List<DTOModel.TrainingPrerequisite>>(trainingSchedule.TrainingPrereqList);

                        var prevTrainingPrerequisiteDtls = genericRepo.Get<DTOModel.TrainingPrerequisite>(x => x.TrainingID == trainingSchedule._Training.TrainingID).ToList<DTOModel.TrainingPrerequisite>();
                        if (prevTrainingPrerequisiteDtls != null || prevTrainingPrerequisiteDtls.Count > 0)
                            genericRepo.DeleteAll<DTOModel.TrainingPrerequisite>(prevTrainingPrerequisiteDtls);
                        if (dtoTrainingPrerequisiteDtls != null)
                            genericRepo.AddMultipleEntity<DTOModel.TrainingPrerequisite>(dtoTrainingPrerequisiteDtls);
                        flag = true;
                    }
                }
                return flag;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region Employee Feedback Form

        public TrainingSchedule GetTrainingFeedbackFormDtls(int trainingID)
        {
            log.Info($"TrainingPrerequisiteService/TrainingPrerequisite");
            try
            {

                TrainingSchedule trn = new TrainingSchedule();
                trn.TrainingFbFormDtlList = new List<TrainingFeedBackFormDetail>();
                var trainingFeedbkHrd = genericRepo.Get<DTOModel.TrainingFeedBackFormHdr>(x => x.TrainingID == trainingID).FirstOrDefault();
                if (trainingFeedbkHrd != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.TrainingFeedBackFormHdr, TrainingSchedule>()

                        .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                        .ForMember(d => d.FeedBackFormHdrID, o => o.MapFrom(s => s.FeedBackFormHdrID))
                        .ForMember(d => d.RatingType, o => o.MapFrom(s => s.RatingType))
                        .ForMember(d => d.UpperRatingValue, o => o.MapFrom(s => s.UpperRatingValue))
                        //.ForMember(d => d.LowerRatingValue, o => o.MapFrom(s => s.LowerRatingValue))
                        //   .ForMember(d => d.ActionPlan, o => o.MapFrom(s => s.ActionPlan))
                        .ForAllOtherMembers(d => d.Ignore())
                        ;
                        cfg.CreateMap<DTOModel.TrainingFeedBackFormDtl, TrainingFeedBackFormDetail>();

                    });
                    var dtotrainingFeedback = Mapper.Map<TrainingSchedule>(trainingFeedbkHrd);
                    if (dtotrainingFeedback != null)
                    {
                        var dtoFeedbackList = Mapper.Map<List<TrainingFeedBackFormDetail>>(trainingFeedbkHrd.TrainingFeedBackFormDtls);
                        trn = dtotrainingFeedback;
                        trn.TrainingFbFormDtlList = dtoFeedbackList;
                    }

                }
                else
                {
                    var defaultFeedback = genericRepo.Get<DTOModel.TrainingFeedbackFormat>().ToList();
                    Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.TrainingFeedbackFormat, TrainingFeedBackFormDetail>()
                    .ForMember(d => d.PartNo, o => o.MapFrom(s => s.PartNo))
                    .ForMember(d => d.SectionPrefix, o => o.MapFrom(s => s.Prefix))
                    .ForMember(d => d.Section, o => o.MapFrom(s => s.Question))
                    .ForAllOtherMembers(d => d.Ignore())
                    );

                    var dtoFeedbackList = Mapper.Map<List<TrainingFeedBackFormDetail>>(defaultFeedback);
                    trn.TrainingFbFormDtlList = dtoFeedbackList;
                }
                return trn;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int UpdateTrainingFeedbackForm(TrainingSchedule trainingSchedule)
        {
            log.Info($"TrainingPrerequisiteService/UpdateTrainingFeedbackForm");
            int flag = 0;
            try
            {

                if (trainingSchedule.FeedBackFormHdrID == 0)
                {
                    Mapper.Initialize(cfg =>
                      cfg.CreateMap<TrainingSchedule, DTOModel.TrainingFeedBackFormHdr>()
                      .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                      .ForMember(d => d.LowerRatingValue, o => o.MapFrom(s => s.LowerRatingValue))
                      .ForMember(d => d.UpperRatingValue, o => o.MapFrom(s => s.UpperRatingValue))
                      .ForMember(d => d.RatingType, o => o.MapFrom(s => s.RatingType))
                      //.ForMember(d => d.ActionPlan, o => o.MapFrom(s => s.ActionPlan))
                      .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                      .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                      .ForAllOtherMembers(d => d.Ignore())
                      );

                    var dtoFeedback = Mapper.Map<DTOModel.TrainingFeedBackFormHdr>(trainingSchedule);
                    var res = genericRepo.Insert(dtoFeedback);
                    if (dtoFeedback.FeedBackFormHdrID > 0)
                    {
                        if (trainingSchedule.TrainingFbFormDtlList != null && trainingSchedule.TrainingFbFormDtlList.Count > 0)
                        {
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<TrainingFeedBackFormDetail, DTOModel.TrainingFeedBackFormDtl>()
                                .ForMember(d => d.FeedBackFormHdrID, o => o.UseValue(dtoFeedback.FeedBackFormHdrID))
                                .ForMember(d => d.PartNo, o => o.MapFrom(s => s.PartNo))
                                .ForMember(d => d.SectionPrefix, o => o.MapFrom(s => s.SectionPrefix))
                                .ForMember(d => d.Section, o => o.MapFrom(s => s.Section))
                                .ForMember(d => d.DisplayOrder, o => o.MapFrom(s => s.DisplayOrder))
                                 .ForMember(d => d.DisplayInBold, o => o.MapFrom(s => s.DisplayInBold))
                                .ForMember(d => d.CreatedBy, o => o.UseValue(dtoFeedback.CreatedBy))
                                .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                                .ForAllOtherMembers(d => d.Ignore());
                            });
                            var dtoFeedbackfrmDtls = Mapper.Map<List<DTOModel.TrainingFeedBackFormDtl>>(trainingSchedule.TrainingFbFormDtlList);
                            if (dtoFeedbackfrmDtls != null)
                                genericRepo.AddMultipleEntity<DTOModel.TrainingFeedBackFormDtl>(dtoFeedbackfrmDtls);
                            return flag = dtoFeedback.FeedBackFormHdrID;
                        }
                        return flag = dtoFeedback.FeedBackFormHdrID;
                    }

                }
                else if (trainingSchedule.FeedBackFormHdrID > 0)
                {
                    var getFeedbackDtl = genericRepo.Get<DTOModel.TrainingFeedBackFormHdr>(x => x.TrainingID == trainingSchedule.TrainingID && x.FeedBackFormHdrID == trainingSchedule.FeedBackFormHdrID).FirstOrDefault();
                    if (getFeedbackDtl != null)
                    {
                        getFeedbackDtl.RatingType = trainingSchedule.RatingType;
                        getFeedbackDtl.UpperRatingValue = trainingSchedule.UpperRatingValue;
                        getFeedbackDtl.LowerRatingValue = trainingSchedule.LowerRatingValue;
                        //getFeedbackDtl.ActionPlan = trainingSchedule.ActionPlan;
                        getFeedbackDtl.UpdatedBy = trainingSchedule.CreatedBy;
                        getFeedbackDtl.UpdatedOn = trainingSchedule.CreatedOn;
                        genericRepo.Update(getFeedbackDtl);
                        flag = trainingSchedule.FeedBackFormHdrID;

                        if (getFeedbackDtl.FeedBackFormHdrID > 0)
                        {
                            if (trainingSchedule.TrainingFbFormDtlList != null && trainingSchedule.TrainingFbFormDtlList.Count > 0)
                            {
                                Mapper.Initialize(cfg =>
                                {
                                    cfg.CreateMap<TrainingFeedBackFormDetail, DTOModel.TrainingFeedBackFormDtl>()
                                    .ForMember(d => d.FeedBackFormHdrID, o => o.UseValue(getFeedbackDtl.FeedBackFormHdrID))
                                    .ForMember(d => d.PartNo, o => o.MapFrom(s => s.PartNo))
                                    .ForMember(d => d.SectionPrefix, o => o.MapFrom(s => s.SectionPrefix))
                                    .ForMember(d => d.Section, o => o.MapFrom(s => s.Section))
                                    .ForMember(d => d.DisplayOrder, o => o.MapFrom(s => s.DisplayOrder))
                                     .ForMember(d => d.DisplayInBold, o => o.MapFrom(s => s.DisplayInBold))
                                    .ForMember(d => d.CreatedBy, o => o.UseValue(trainingSchedule.CreatedBy))
                                    .ForMember(d => d.CreatedOn, o => o.UseValue(trainingSchedule.CreatedOn));
                                });
                                var dtoFeedbackfrmDtls = Mapper.Map<List<DTOModel.TrainingFeedBackFormDtl>>(trainingSchedule.TrainingFbFormDtlList);
                                var getFeedbackfrmDtls = genericRepo.Get<DTOModel.TrainingFeedBackFormDtl>(x => x.FeedBackFormHdrID == trainingSchedule.FeedBackFormHdrID).ToList();
                                if (getFeedbackfrmDtls != null && getFeedbackfrmDtls.Count > 0)
                                    genericRepo.DeleteAll<DTOModel.TrainingFeedBackFormDtl>(getFeedbackfrmDtls);
                                if (dtoFeedbackfrmDtls != null && dtoFeedbackfrmDtls.Count > 0)
                                    genericRepo.AddMultipleEntity<DTOModel.TrainingFeedBackFormDtl>(dtoFeedbackfrmDtls);
                                return flag = getFeedbackDtl.FeedBackFormHdrID;
                            }

                            return flag;
                        }
                    }
                }
                else
                {
                    return flag = 0;
                }
                return flag;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Training> GetFeedbackFormList(int? employeeID)
        {
            log.Info($"TrainingService/GetFeedbackFormList");
            try
            {
                //var getTrainingList = genericRepo.Get<DTOModel.TrainingParticipant>(x => x.EmployeeID == employeeID && x.TrainingAttended).ToList();
                var getTrainingList = genericRepo.Get<DTOModel.Training>().ToList();

                if (getTrainingList != null && getTrainingList.Count > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.Training, Training>()
                        .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                        .ForMember(d => d.StartDate, o => o.MapFrom(s => s.StartDate))
                        .ForMember(d => d.EndDate, o => o.MapFrom(s => s.EndDate))
                        .ForMember(d => d.FeedbackFormStatus, o => o.MapFrom(s => s.TrainingParticipants.Where(x => x.EmployeeID == employeeID && x.TrainingID == s.TrainingID).FirstOrDefault().FeedbackFormStatus))
                        .ForMember(d => d.TrainingAttended, o => o.MapFrom(s => s.TrainingParticipants.Where(x => x.EmployeeID == employeeID && x.TrainingID == s.TrainingID).FirstOrDefault().TrainingAttended))
                        .ForMember(d => d.Address, o => o.MapFrom(s => s.Address))
                        .ForMember(d => d.City, o => o.MapFrom(s => s.City))
                        .ForMember(d => d.StateName, o => o.MapFrom(s => s.State.StateName))
                        .ForMember(d => d.PinCode, o => o.MapFrom(s => s.PinCode))
                        .ForMember(d => d.FeedBackFormHdrID, o => o.MapFrom(s => s.TrainingFeedBackFormHdrs.FirstOrDefault().FeedBackFormHdrID))
                       .ForMember(d => d.TrainingTitle, o => o.MapFrom(s => s.TrainingTitle))
                       .ForMember(d => d.TrainingTypeName, o => o.MapFrom(s => ((EnumTrainingType)s.TrainingType).GetDisplayName()))
                       .ForMember(d => d.TrainingStatus, o => o.MapFrom(s => s.TrainingStatus))
                       .ForMember(d => d.Nomination, o => o.MapFrom(s => s.TrainingParticipants.Where(x => x.EmployeeID == employeeID && x.TrainingID == s.TrainingID).FirstOrDefault().Nomination))
                       .ForMember(d => d.NominationDate, o => o.MapFrom(s => s.NominationDate))
                       .ForMember(d => d.ParticipantExist, o => o.MapFrom(s => s.TrainingParticipants.Where(x => x.EmployeeID == employeeID && x.TrainingID == s.TrainingID).FirstOrDefault().TrainingParticipantID))
                       .ForMember(d => d.trainingTopic, o => o.MapFrom(s => s.TrainingTopics.Where(x => x.TrainingID == s.TrainingID).Select(y => y.Skill)));
                        cfg.CreateMap<DTOModel.Skill, Skill>();
                    });
                    var dtoTrainingList = Mapper.Map<List<Training>>(getTrainingList);

                    return dtoTrainingList;
                }
                else
                    return new List<Training>();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public TrainingSchedule GenerateFeedbackForm(int feedBackFormHdrID, int employeeID)
        {
            log.Info($"TrainingService/GenerateFeedbackForm");
            try
            {
                TrainingSchedule trainingSchedule = new TrainingSchedule();
                var getFeedBackFormHdr = genericRepo.Get<DTOModel.TrainingFeedBackFormHdr>(x => x.FeedBackFormHdrID == feedBackFormHdrID).FirstOrDefault();
                if (getFeedBackFormHdr != null)
                {
                    var getTrainingList = trainingRepo.GetTrainingFromDetail(feedBackFormHdrID, null, employeeID);

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.TrainingFeedBackFormHdr, TrainingFeedBackFormHdr>();
                        cfg.CreateMap<DTOModel.Training, Training>();
                        cfg.CreateMap<DTOModel.Trainer, Trainer>();
                    });
                    var dtoHdrList = Mapper.Map<TrainingFeedBackFormHdr>(getFeedBackFormHdr);
                    var dtoTrainingList = Mapper.Map<Training>(getFeedBackFormHdr.Training);
                    var trainingID = dtoTrainingList.TrainingID;
                    var dtoTrainerList = Mapper.Map<List<Trainer>>(getFeedBackFormHdr.Training.Trainers);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.GetTrainingFromDetail_Result, TrainingFeedbackDetailListing>()
                        .ForMember(d => d.QuestionID, o => o.MapFrom(s => s.QuestionID))
                        .ForMember(d => d.FeedBackFormHdrID, o => o.MapFrom(s => s.FeedBackFormHdrID))
                        .ForMember(d => d.QuestionID, o => o.MapFrom(s => s.QuestionID))
                        .ForMember(d => d.PartNo, o => o.MapFrom(s => s.PartNo))
                        .ForMember(d => d.Section, o => o.MapFrom(s => s.Section))
                        .ForMember(d => d.DisplayOrder, o => o.MapFrom(s => s.DisplayOrder))
                        .ForMember(d => d.DisplayInBold, o => o.MapFrom(s => s.DisplayInBold))
                        .ForMember(d => d.enumRatingNo, o => o.MapFrom(s => s.RatingNo))
                        .ForMember(d => d.Grade, o => o.MapFrom(s => s.Grade));

                    });
                    var dtoTrainingFbList = Mapper.Map<List<TrainingFeedbackDetailListing>>(getTrainingList);
                    var GetActionPlane = genericRepo.Get<DTOModel.TrainingParticipant>(x => x.EmployeeID == employeeID && x.TrainingID == trainingID).FirstOrDefault();
                    if (GetActionPlane != null)
                    {
                        trainingSchedule.ActionPlan = GetActionPlane.ActionPlan;
                        trainingSchedule.EmployeeCode = GetActionPlane.tblMstEmployee.EmployeeCode;
                        trainingSchedule.EmployeeName = GetActionPlane.tblMstEmployee.Name;
                    }
                    else
                        trainingSchedule.ActionPlan = string.Empty;

                    trainingSchedule.TrainingFeedbackDetailList = dtoTrainingFbList;
                    trainingSchedule._Training = dtoTrainingList;
                    trainingSchedule.TrainingFeedBackFormHeader = dtoHdrList;
                    trainingSchedule.TrainerList = dtoTrainerList;
                    return trainingSchedule;
                }
                else
                {
                    return trainingSchedule;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public int InsertEmployeeFeedbackForm(TrainingSchedule trainingSchedule)
        {
            log.Info($"TrainingPrerequisiteService/InsertEmployeeFeedbackForm");
            int flag = 0;
            try
            {
                var getParticipantDtl = genericRepo.Get<DTOModel.TrainingParticipant>(x => x.TrainingID == trainingSchedule.TrainingID && x.EmployeeID == trainingSchedule.EmployeeID).FirstOrDefault();
                if (getParticipantDtl != null)
                {
                    if (!getParticipantDtl.FeedbackFormStatus)
                    {
                        if (trainingSchedule.TrainingFeedbackDetailList != null && trainingSchedule.TrainingFeedbackDetailList.Count > 0)
                        {
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<TrainingFeedbackDetailListing, DTOModel.TrainingFeedbackDetail>()
                                  .ForMember(d => d.EmployeeID, o => o.UseValue(trainingSchedule.EmployeeID))
                                .ForMember(d => d.FeedBackFormHdrID, o => o.UseValue(trainingSchedule.FeedBackFormHdrID))
                                .ForMember(d => d.TrainingID, o => o.UseValue(trainingSchedule.TrainingID))
                                .ForMember(d => d.QuestionID, o => o.MapFrom(s => s.QuestionID))
                                .ForMember(d => d.RatingType, o => o.UseValue(trainingSchedule.RatingType))
                                 .ForMember(d => d.RatingNo, o => o.MapFrom(s => (int)s.enumRatingNo))
                                  .ForMember(d => d.Grade, o => o.MapFrom(s => s.enumRatingGrade))
                                .ForMember(d => d.CreatedBy, o => o.UseValue(trainingSchedule.CreatedBy))
                                .ForMember(d => d.CreatedOn, o => o.UseValue(trainingSchedule.CreatedOn));
                            });
                            var dtoFeedbackDtls = Mapper.Map<List<DTOModel.TrainingFeedbackDetail>>(trainingSchedule.TrainingFeedbackDetailList);
                            if (dtoFeedbackDtls != null && dtoFeedbackDtls.Count > 0)
                                genericRepo.AddMultipleEntity<DTOModel.TrainingFeedbackDetail>(dtoFeedbackDtls);
                        }
                        getParticipantDtl.FeedbackFormStatus = true;
                        getParticipantDtl.ActionPlan = trainingSchedule.ActionPlan;
                        genericRepo.Update(getParticipantDtl);
                        flag = getParticipantDtl.TrainingParticipantID;
                        return flag;
                    }
                    else if (getParticipantDtl.FeedbackFormStatus)
                    {
                        if (trainingSchedule.TrainingFeedbackDetailList != null && trainingSchedule.TrainingFeedbackDetailList.Count > 0)
                        {
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<TrainingFeedbackDetailListing, DTOModel.TrainingFeedbackDetail>()
                                  .ForMember(d => d.EmployeeID, o => o.UseValue(trainingSchedule.EmployeeID))
                                .ForMember(d => d.FeedBackFormHdrID, o => o.UseValue(trainingSchedule.FeedBackFormHdrID))
                                .ForMember(d => d.TrainingID, o => o.UseValue(trainingSchedule.TrainingID))
                                .ForMember(d => d.QuestionID, o => o.MapFrom(s => s.QuestionID))
                                .ForMember(d => d.RatingType, o => o.UseValue(trainingSchedule.RatingType))
                                 .ForMember(d => d.RatingNo, o => o.MapFrom(s => (int)s.enumRatingNo))
                                  .ForMember(d => d.Grade, o => o.MapFrom(s => s.enumRatingGrade))
                                .ForMember(d => d.CreatedBy, o => o.UseValue(trainingSchedule.CreatedBy))
                                .ForMember(d => d.CreatedOn, o => o.UseValue(trainingSchedule.CreatedOn));
                            });
                            var dtoFeedbackDtls = Mapper.Map<List<DTOModel.TrainingFeedbackDetail>>(trainingSchedule.TrainingFeedbackDetailList);
                            var getFeedbackfrmDtls = genericRepo.Get<DTOModel.TrainingFeedbackDetail>(x => x.EmployeeID == trainingSchedule.EmployeeID && x.TrainingID == trainingSchedule.TrainingID && x.FeedBackFormHdrID == trainingSchedule.FeedBackFormHdrID).ToList();
                            if (getFeedbackfrmDtls != null && getFeedbackfrmDtls.Count > 0)
                                genericRepo.DeleteAll<DTOModel.TrainingFeedbackDetail>(getFeedbackfrmDtls);
                            if (dtoFeedbackDtls != null && dtoFeedbackDtls.Count > 0)
                                genericRepo.AddMultipleEntity<DTOModel.TrainingFeedbackDetail>(dtoFeedbackDtls);
                        }
                        getParticipantDtl.ActionPlan = trainingSchedule.ActionPlan;
                        genericRepo.Update(getParticipantDtl);
                        flag = getParticipantDtl.TrainingParticipantID;
                        return flag;
                    }
                    else
                        return flag = 0;

                }
                else
                {
                    return flag = 0;
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Training> GetTrainingPopupList()
        {
            log.Info($"TrainingService/GetTrainingPopupList");
            try
            {
                var getTrainingList = genericRepo.Get<DTOModel.Training>(x => !x.IsDeleted && x.TrainingFeedBackFormHdrs.Any(y => y.TrainingID == x.TrainingID)).ToList();
                if (getTrainingList != null && getTrainingList.Count > 0)
                {

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.Training, Training>()
                        .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                        .ForMember(d => d.StartDate, o => o.MapFrom(s => s.StartDate))
                        .ForMember(d => d.EndDate, o => o.MapFrom(s => s.EndDate))
                        .ForMember(d => d.Address, o => o.MapFrom(s => s.Address))
                        .ForMember(d => d.City, o => o.MapFrom(s => s.City))
                        .ForMember(d => d.StateName, o => o.MapFrom(s => s.State.StateName))
                        .ForMember(d => d.PinCode, o => o.MapFrom(s => s.PinCode))
                         .ForMember(d => d.TrainingStatus, o => o.MapFrom(s => s.TrainingStatus));

                    });
                    var dtoTrainingList = Mapper.Map<List<Training>>(getTrainingList);
                    return dtoTrainingList;
                }
                else
                    return new List<Training>();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public int CopyFeedbackQuesfromTraining(int prevTrainingID, int trainingID, int userID)
        {
            log.Info($"TrainingService/CopyFeedbackQuesfromTraining=prevTrainingID{prevTrainingID}/trainingID={trainingID}/userID={userID}");
            try
            {
                int res = trainingRepo.CopyFeedbackQuesfromTraining(prevTrainingID, trainingID, userID);
                return res;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region Training Status

        public bool UpdateTrainingStatus(TrainingSchedule training)
        {
            log.Info($"TrainingService/UpdateTrainingStatus");
            bool flag = false;
            try
            {
                var dtoTraining = genericRepo.GetByID<DTOModel.Training>(training._Training.TrainingID);
                dtoTraining.TrainingStatus = (byte)(EnumTrainingStatus)training._Training.enumTrainingStatus;
                if ((EnumTrainingStatus)training._Training.enumTrainingStatus == EnumTrainingStatus.Reschedule)
                {
                    dtoTraining.StartDate = (DateTime)training._Training.StartDate;
                    dtoTraining.EndDate = (DateTime)training._Training.EndDate;
                    dtoTraining.StartDateFromTime = training._Training.StartDateFromTime;
                    dtoTraining.StartDateToTime = training._Training.StartDateToTime;
                    dtoTraining.Address = training._Training.Address;
                    dtoTraining.StateID = training._Training.StateID;
                    dtoTraining.City = training._Training.City;
                    dtoTraining.PinCode = training._Training.PinCode;
                    dtoTraining.CancelReasonID = training._Training.CancelReasonID;
                    dtoTraining.CancelReason = training._Training.CancelReason;
                    dtoTraining.TimeSlotType = training._Training.TimeSlotType;
                }
                genericRepo.Update<DTOModel.Training>(dtoTraining);

                if (dtoTraining.TrainingID > 0)
                {
                    // auto generate feedback form, if training is mark as completed.
                    if ((EnumTrainingStatus)training._Training.enumTrainingStatus == EnumTrainingStatus.Completed)
                    {
                        Mapper.Initialize(cfg =>
                      cfg.CreateMap<TrainingSchedule, DTOModel.TrainingFeedBackFormHdr>()
                      .ForMember(d => d.TrainingID, o => o.UseValue(dtoTraining.TrainingID))
                      .ForMember(d => d.LowerRatingValue, o => o.MapFrom(s => s.LowerRatingValue))
                      .ForMember(d => d.UpperRatingValue, o => o.MapFrom(s => s.UpperRatingValue))
                      .ForMember(d => d.RatingType, o => o.UseValue("N"))
                      //.ForMember(d => d.ActionPlan, o => o.MapFrom(s => s.ActionPlan))
                      .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s._Training.CreatedBy))
                      .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s._Training.CreatedOn))
                      .ForAllOtherMembers(d => d.Ignore())
                      );

                        var dtoFeedback = Mapper.Map<DTOModel.TrainingFeedBackFormHdr>(training);
                        var res = genericRepo.Insert(dtoFeedback);
                        if (dtoFeedback.FeedBackFormHdrID > 0)
                        {
                            var getQuestions = genericRepo.Get<DTOModel.TrainingFeedbackFormat>().ToList();
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<DTOModel.TrainingFeedbackFormat, DTOModel.TrainingFeedBackFormDtl>()
                                .ForMember(d => d.FeedBackFormHdrID, o => o.UseValue(dtoFeedback.FeedBackFormHdrID))
                                .ForMember(d => d.PartNo, o => o.MapFrom(s => s.PartNo))
                                .ForMember(d => d.SectionPrefix, o => o.MapFrom(s => s.Prefix))
                                .ForMember(d => d.Section, o => o.MapFrom(s => s.Question))
                                .ForMember(d => d.DisplayOrder, o => o.UseValue(0))
                                 .ForMember(d => d.DisplayInBold, o => o.UseValue(0))
                                .ForMember(d => d.CreatedBy, o => o.UseValue(dtoFeedback.CreatedBy))
                                .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                                .ForAllOtherMembers(d => d.Ignore());
                            });
                            var dtoFeedbackfrmDtls = Mapper.Map<List<DTOModel.TrainingFeedBackFormDtl>>(getQuestions);
                            if (dtoFeedbackfrmDtls != null)
                                genericRepo.AddMultipleEntity<DTOModel.TrainingFeedBackFormDtl>(dtoFeedbackfrmDtls);
                        }
                    }
                    //=== region to add datewise training time slot ====  (Added On- 20-Oct-20)

                    if (genericRepo.Exists<DTOModel.TrainingDateWiseTimeSlot>(x => x.TrainingID == dtoTraining.TrainingID))
                    {
                        var lastDefinedSlots = genericRepo.Get<DTOModel.TrainingDateWiseTimeSlot>(x => x.TrainingID == dtoTraining.TrainingID).ToList();
                        genericRepo.DeleteAll<DTOModel.TrainingDateWiseTimeSlot>(lastDefinedSlots);
                    }
                    Mapper.Initialize(cfg =>
                      cfg.CreateMap<TrainingDateWiseTimeSlot, DTOModel.TrainingDateWiseTimeSlot>()
                      .ForMember(d => d.TrainingID, o => o.UseValue(dtoTraining.TrainingID)));

                    var dtoTimeSlots = Mapper.Map<List<DTOModel.TrainingDateWiseTimeSlot>>(training._Training.distributedTimeSlots);

                    genericRepo.AddMultipleEntity<DTOModel.TrainingDateWiseTimeSlot>(dtoTimeSlots);

                    ///========end =============================                   
                }
                if ((EnumTrainingStatus)training._Training.enumTrainingStatus != EnumTrainingStatus.Schedule)
                {
                    SendTrainingRelatedEmail(training, training._Training.enumTrainingStatus.ToString());
                }
                flag = true;

            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }
        public bool UpdateTrainingStatusForSchedule(TrainingSchedule training)
        {
            log.Info($"TrainingService/UpdateTrainingStatusForSchedule");
            bool flag = false;
            try
            {
                var dtoTraining = genericRepo.GetByID<DTOModel.Training>(training._Training.TrainingID);
                if (dtoTraining != null)
                {
                    dtoTraining.TrainingStatus = (byte)(EnumTrainingStatus)training._Training.enumTrainingStatus;
                    genericRepo.Update<DTOModel.Training>(dtoTraining);
                }
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }
        #endregion

        public static byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }

        private List<MailAttachment> GetTrainingEmailAttachments(List<TrainingDocumentRepository> trainingDocs)
        {
            log.Info($"TrainingService/GetTrainingEmailAttachments");

            List<MailAttachment> attachments = new List<MailAttachment>();
            try
            {
                if (trainingDocs.Count > 0)
                {
                    foreach (var item in trainingDocs)
                    {
                        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Document/TrainingDocument/" + item.DocumentPathName);

                        if (System.IO.File.Exists(fullPath))
                        {
                            byte[] tmpBytes = ReadFile(fullPath);
                            MemoryStream ms = new MemoryStream();
                            using (MemoryStream tempStream = new MemoryStream())
                            {
                                ms.Write(tmpBytes, 0, tmpBytes.Length);
                            }
                            if ((ms != null) && (ms.Length != 0))
                            {
                                ms.Seek(0, SeekOrigin.Begin);
                                ms.Position = 0;
                            }
                            attachments.Add(new MailAttachment { Content = ms, FileName = item.DocumentPathName });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.InnerException + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return attachments;
        }

        public bool SendTrainingRelatedEmail(TrainingSchedule tSchedule, string mailType)
        {
            log.Info($"TrainingService/SendTrainingRelatedEmail");
            try
            {
                EmailMessage emailMessage = new EmailMessage();
                var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                if (emailsetting != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.EmailConfiguration, EmailMessage>()
                        .ForMember(d => d.From, o => o.MapFrom(s => $"NAFED HRMS <{s.ToEmail}>"))
                        // .ForMember(d => d.Subject, o => o.UseValue(emailSubject))
                        .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
                        .ForMember(d => d.Password, o => o.MapFrom(s => s.Password))
                        .ForMember(d => d.SmtpClientHost, o => o.MapFrom(s => s.Server))
                        .ForMember(d => d.SmtpPort, o => o.MapFrom(s => s.Port))
                        .ForMember(d => d.enableSSL, o => o.MapFrom(s => s.SSLStatus))
                        .ForMember(d => d.HTMLView, o => o.UseValue(true))
                        .ForMember(d => d.FriendlyName, o => o.UseValue("NAFED"));
                    });
                    emailMessage = Mapper.Map<EmailMessage>(emailsetting);
                    StringBuilder mailBody = new StringBuilder();
                    if (mailType == "Completed")
                    {
                        mailBody.Clear();
                        var tParticipants = tSchedule.TrainingParticipntList.Where(x => x.TrainingAttended).ToList();

                        emailMessage.Subject = tSchedule._Training.TrainingTitle + ":" + "Training Feedback Form";
                        mailBody.AppendFormat($"<p>Dear Employee,</p>");
                        mailBody.AppendFormat($"<p>We kindly ask that you give feedback on the training <b>{tSchedule._Training.TrainingTitle } helpd on {tSchedule._Training.StartDate.Value.ToString("dd-MM-yyyy")}</b> till <b>{tSchedule._Training.EndDate.Value.ToString("dd-MM-yyyy")}</b>.</p>");
                        mailBody.AppendFormat($"<p>To give your training feedback we request you to sign in our HR portal and use training feedback form under Services tab</p>");
                        mailBody.AppendFormat($"<p>For any query regards training feedback form, please contact to Personnel Department or write mail to us on <b>personnelsection1@nafed-india.com</b></p>");
                        mailBody.AppendFormat($"<div>From, </div> ");
                        mailBody.AppendFormat($"<div><b>Personnel Department</b></div>");
                        mailBody.AppendFormat($"<div><b>ENAFED</b></div>");
                        emailMessage.Body = mailBody.ToString();

                        Task t1 = Task.Run(() => SendMail(emailMessage, tParticipants));
                    }
                    else if (mailType == "Intimation")
                    {

                        ///== Get Training Documents =============
                        if (tSchedule.AddAttachment)
                            tSchedule._Training.trainingDocs = GetSelectedDocument(tSchedule._Training.TrainingID, tSchedule.Attachments);

                        mailBody.Clear();
                        emailMessage.Subject = tSchedule._Training.TrainingTitle + ":" + "Invitation and Welcome!";

                        //   mailBody.AppendFormat($"<p>We are pleased to announce that National Agricultural Cooperative Marketing Federation of India Ltd. has arranged a training session for our employees. This training shall be a part of the Skill improvement plans. The training sessions will begin from <b>{tSchedule._Training.StartDate.Value.ToString("dd-MM-yyyy")}</b> till <b>{tSchedule._Training.EndDate.Value.ToString("dd-MM-yyyy")}</b>.</p> <br>");
                        mailBody.AppendFormat($"<p>Training Date: <b>{tSchedule._Training.StartDate.Value.ToString("dd-MM-yyyy")}</b> till <b>{tSchedule._Training.EndDate.Value.ToString("dd-MM-yyyy")}</b>.</p>");
                        if (tSchedule._Training.TimeSlotType == (byte)EnumTimeSlotType.Distributed)
                        {
                            for (int i = 0; i < tSchedule._Training.distributedTimeSlots.Count; i++)
                            {
                                mailBody.AppendFormat($"<p>Date: {tSchedule._Training.distributedTimeSlots[i].TrainingDate.ToString("dd-MM-yyyy")} - Timing: <b>{tSchedule._Training.distributedTimeSlots[i].FromTime} to {tSchedule._Training.distributedTimeSlots[i].ToTime}</b></p>");
                            }
                        }
                        else
                        {
                            mailBody.AppendFormat($"<p>Timing: <b>{tSchedule._Training.StartDateFromTime} to {tSchedule._Training.StartDateToTime}</b></p>");
                        }

                        // mailBody.AppendFormat($"<p>Our training sessions shall concentrate on providing and improving the following skills. </p>");
                        mailBody.AppendFormat($"<p>Place of training: <b>{tSchedule._Training.Address}, {tSchedule._Training.City}, {tSchedule._Training.StateName}, {tSchedule._Training.PinCode}</b></p>");
                        mailBody.AppendFormat($"<p>Training For: <b>{tSchedule._Training.TrainingTypeName}</b></p>");
                        mailBody.AppendFormat($"<ul>");
                        for (int i = 0; i < tSchedule.topicList.Count; i++)
                        {
                            mailBody.AppendFormat($"<li>{tSchedule.topicList[i].ToString()}</li>");
                        }
                        if (!string.IsNullOrEmpty(tSchedule._Training.OtherTopic))
                            mailBody.Append($"<li>{tSchedule._Training.OtherTopic}</li>");

                        mailBody.AppendFormat($"</ul>");
                        if (!tSchedule._Training.InternalAddressType)
                            mailBody.AppendFormat($"<br><p>The Competent Authority has nominated your name for attending the said training. You may please complete the nomination from attached with their letter and return the duly filled in form with your photograph and other required document to HRD Division immediately for further necessary action as the form are to be submitted to the Training provided. </p>");
                        else if (tSchedule._Training.InternalAddressType)
                            mailBody.AppendFormat($"<br><p>The Competent Authority has nominated your name for attending the said training programme. You are reqested to attend the program as per the schedule. </p>");
                        mailBody.AppendFormat($"<div>From, </div> ");
                        mailBody.AppendFormat($"<div><b>Personnel Department</b></div>");
                        mailBody.AppendFormat($"<div><b>ENAFED</b></div>");
                        emailMessage.Body = mailBody.ToString();

                        if (tSchedule.AddAttachment && tSchedule._Training.trainingDocs?.Count > 0)
                            emailMessage.Attachments = GetTrainingEmailAttachments(tSchedule._Training.trainingDocs);

                        Task t1 = Task.Run(() => SendMail(emailMessage, tSchedule.TrainingParticipntList));
                    }
                    else if (mailType == "Cancel")
                    {
                        mailBody.Clear();
                        emailMessage.Subject = emailMessage.Subject = tSchedule._Training.TrainingTitle + ":" + "Notice Training Cancellation";

                        // mailBody.AppendFormat($"<p>This is notifying you that due {tSchedule._Training.CancelReason}, we have no choice but to cancel our upcoming training which is scheduled from <b>{tSchedule._Training.StartDate.Value.ToString("dd-MM-yyyy")}</b> till <b>{tSchedule._Training.EndDate.Value.ToString("dd-MM-yyyy")}</b>.</p> <br>");
                        mailBody.AppendFormat($"<p>It is to inform that due to {tSchedule._Training.CancelReason}, the said programme stand canceled.</b>.</p>");
                        // mailBody.AppendFormat($"<p>Place of training: <b>{tSchedule._Training.Address}, {tSchedule._Training.City}, {tSchedule._Training.StateName}, {tSchedule._Training.PinCode}</b></p>");
                        // mailBody.AppendFormat($"<p>Timing: <b>{tSchedule._Training.StartDateFromTime} to {tSchedule._Training.StartDateToTime}</b></p> <br>");
                        //mailBody.AppendFormat($"<p>Please accept our sincere apologies for such a short notice and any inconvenience this may have caused.  I know how important it is for you to finally work out a plan for the nest financial year.</p>");
                        //mailBody.AppendFormat($"<p>Thank you once again for your consideration.</p> <br>");
                        mailBody.AppendFormat($"<div>From, </div> ");
                        mailBody.AppendFormat($"<div><b>Personnel Department</b></div>");
                        mailBody.AppendFormat($"<div><b>ENAFED</b></div>");
                        emailMessage.Body = mailBody.ToString();

                        Task t1 = Task.Run(() => SendMail(emailMessage, tSchedule.TrainingParticipntList));
                    }
                    else if (mailType == "Reschedule")
                    {
                        mailBody.Clear();
                        emailMessage.Subject = emailMessage.Subject = tSchedule._Training.TrainingTitle + ":" + "Notice Training Reschedule";

                        mailBody.AppendFormat($"<p>It is to inform that the said programme has been resceduled to <b>{tSchedule._Training.StartDate.Value.ToString("dd-MM-yyyy")}</b> till <b>{tSchedule._Training.EndDate.Value.ToString("dd-MM-yyyy")}</b> at the same venue.</p>");
                        mailBody.AppendFormat($"<p>Timing: <b>{tSchedule._Training.StartDateFromTime} to {tSchedule._Training.StartDateToTime}</b></p> <br>");

                        if (tSchedule._Training.TimeSlotType == (byte)EnumTimeSlotType.Distributed)
                        {
                            for (int i = 0; i < tSchedule._Training.distributedTimeSlots.Count; i++)
                            {
                                mailBody.AppendFormat($"<p>Date: {tSchedule._Training.distributedTimeSlots[i].TrainingDate.ToString("dd-MM-yyyy")} - Timing: <b>{tSchedule._Training.distributedTimeSlots[i].FromTime} to {tSchedule._Training.distributedTimeSlots[i].ToTime}</b></p>");
                            }
                        }
                        else
                        {
                            mailBody.AppendFormat($"<p>Timing: <b>{tSchedule._Training.StartDateFromTime} to {tSchedule._Training.StartDateToTime}</b></p>");
                        }
                        mailBody.AppendFormat($"<p>Place of training: <b>{tSchedule._Training.Address}, {tSchedule._Training.City}, {tSchedule._Training.StateName}, {tSchedule._Training.PinCode}</b></p>");

                        //mailBody.AppendFormat($"<div>Please accept our sincere apologies for such a short notice and any inconvenience this may have caused. </div>");

                        //mailBody.AppendFormat($"<p>Thank you once again for your consideration. </p> <br>");
                        mailBody.AppendFormat($"<div>From, </div> ");
                        mailBody.AppendFormat($"<div><b>Personnel Department</b></div>");
                        mailBody.AppendFormat($"<div><b>ENAFED</b></div>");
                        emailMessage.Body = mailBody.ToString();

                        Task t1 = Task.Run(() => SendMail(emailMessage, tSchedule.TrainingParticipntList));
                    }
                    else if (mailType == "Nomination")
                    {
                        mailBody.Clear();
                        emailMessage.Subject = "Training Nomination For : " + tSchedule._Training.TrainingTitle;

                        //   mailBody.AppendFormat($"<p>We are pleased to announce that National Agricultural Cooperative Marketing Federation of India Ltd. has arranged a training session for our employees. This training shall be a part of the Skill improvement plans. The training sessions will begin from <b>{tSchedule._Training.StartDate.Value.ToString("dd-MM-yyyy")}</b> till <b>{tSchedule._Training.EndDate.Value.ToString("dd-MM-yyyy")}</b>.</p> <br>");
                        mailBody.AppendFormat($"<p>Training Date: <b>{tSchedule._Training.StartDate.Value.ToString("dd-MM-yyyy")}</b> till <b>{tSchedule._Training.EndDate.Value.ToString("dd-MM-yyyy")}</b>.</p>");
                        if (tSchedule._Training.TimeSlotType == (byte)EnumTimeSlotType.Distributed)
                        {
                            for (int i = 0; i < tSchedule._Training.distributedTimeSlots.Count; i++)
                            {
                                mailBody.AppendFormat($"<p>Date: {tSchedule._Training.distributedTimeSlots[i].TrainingDate.ToString("dd-MM-yyyy")} - Timing: <b>{tSchedule._Training.distributedTimeSlots[i].FromTime} to {tSchedule._Training.distributedTimeSlots[i].ToTime}</b></p>");
                            }
                        }
                        else
                        {
                            mailBody.AppendFormat($"<p>Timing: <b>{tSchedule._Training.StartDateFromTime} to {tSchedule._Training.StartDateToTime}</b></p>");
                        }

                        // mailBody.AppendFormat($"<p>Our training sessions shall concentrate on providing and improving the following skills. </p>");
                        mailBody.AppendFormat($"<p>Place of training: <b>{tSchedule._Training.Address}, {tSchedule._Training.City}, {tSchedule._Training.StateName}, {tSchedule._Training.PinCode}</b></p>");
                        mailBody.AppendFormat($"<p>Training For: <b>{tSchedule._Training.TrainingTypeName}</b></p>");
                        mailBody.AppendFormat($"<ul>");
                        for (int i = 0; i < tSchedule.topicList.Count; i++)
                        {
                            mailBody.AppendFormat($"<li>{tSchedule.topicList[i].ToString()}</li>");
                        }
                        if (!string.IsNullOrEmpty(tSchedule._Training.OtherTopic))
                            mailBody.Append($"<li>{tSchedule._Training.OtherTopic}</li>");

                        mailBody.AppendFormat($"</ul>");

                        mailBody.AppendFormat("<p style='font-family:Tahoma;font-size:9pt;'> Dear <b>" + tSchedule.EmployeeName + "</b>,</p>");
                        mailBody.AppendFormat("<div>Your nomination has been done for the selected training. Now, your request has been sent to your Reporting Manager for further approval. You will get confirmation when it get approved or rejected.</div>");
                        mailBody.AppendFormat($"<div>From, </div> ");
                        mailBody.AppendFormat($"<div><b>Personnel Department</b></div>");
                        mailBody.AppendFormat($"<div><b>ENAFED</b></div>");
                        emailMessage.Body = mailBody.ToString();
                        Task t1 = Task.Run(() => SendMail(emailMessage, tSchedule.TrainingParticipntList));
                    }
                    else if (mailType == "NomReceiver")
                    {
                        mailBody.Clear();

                        var recieverMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == tSchedule.EmployeeID && !x.IsDeleted).Select(x => new
                        {
                            MobileNo = x.MobileNo,
                            OfficailEmail = x.OfficialEmail,
                            EmployeeCode = x.EmployeeCode
                        }).FirstOrDefault();

                        if (!string.IsNullOrEmpty(recieverMail.EmployeeCode))
                        {
                            PushNotification notification = new PushNotification
                            {
                                UserName = recieverMail.EmployeeCode,
                                Title = "Training Nomination Approval",
                                Message = $"Dear Sir/Madam, This is to intimate that you have received the Training Nomination Approval, for employee { tSchedule.EmployeeCode + "-" + tSchedule.EmployeeName} for further evaluation."

                            };
                            Task notif = Task.Run(() => FirebaseTopicNotification(notification));
                        }
                        if (!string.IsNullOrEmpty(recieverMail.OfficailEmail))
                            tSchedule.TrainingParticipntList.FirstOrDefault().EmailID = recieverMail.OfficailEmail;


                        emailMessage.Subject = "Training Nomination Application For: " + tSchedule._Training.TrainingTitle;

                        mailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>");
                        mailBody.AppendFormat("<p>This is to intimate that you have received the Training Nomination Application for further evaluation.</p>");
                        mailBody.AppendFormat("<p>Requesting your support for timely completion of the process by logging into: </p>");
                        mailBody.AppendFormat("<p>http://182.74.122.83/nafedhrms</p>");


                        //   mailBody.AppendFormat($"<p>We are pleased to announce that National Agricultural Cooperative Marketing Federation of India Ltd. has arranged a training session for our employees. This training shall be a part of the Skill improvement plans. The training sessions will begin from <b>{tSchedule._Training.StartDate.Value.ToString("dd-MM-yyyy")}</b> till <b>{tSchedule._Training.EndDate.Value.ToString("dd-MM-yyyy")}</b>.</p> <br>");
                        mailBody.AppendFormat($"<p>Training Date: <b>{tSchedule._Training.StartDate.Value.ToString("dd-MM-yyyy")}</b> till <b>{tSchedule._Training.EndDate.Value.ToString("dd-MM-yyyy")}</b>.</p>");
                        if (tSchedule._Training.TimeSlotType == (byte)EnumTimeSlotType.Distributed)
                        {
                            for (int i = 0; i < tSchedule._Training.distributedTimeSlots.Count; i++)
                            {
                                mailBody.AppendFormat($"<p>Date: {tSchedule._Training.distributedTimeSlots[i].TrainingDate.ToString("dd-MM-yyyy")} - Timing: <b>{tSchedule._Training.distributedTimeSlots[i].FromTime} to {tSchedule._Training.distributedTimeSlots[i].ToTime}</b></p>");
                            }
                        }
                        else
                        {
                            mailBody.AppendFormat($"<p>Timing: <b>{tSchedule._Training.StartDateFromTime} to {tSchedule._Training.StartDateToTime}</b></p>");
                        }

                        // mailBody.AppendFormat($"<p>Our training sessions shall concentrate on providing and improving the following skills. </p>");
                        mailBody.AppendFormat($"<p>Place of training: <b>{tSchedule._Training.Address}, {tSchedule._Training.City}, {tSchedule._Training.StateName}, {tSchedule._Training.PinCode}</b></p>");
                        mailBody.AppendFormat($"<p>Training For: <b>{tSchedule._Training.TrainingTypeName}</b></p>");
                        mailBody.AppendFormat($"<ul>");
                        for (int i = 0; i < tSchedule.topicList.Count; i++)
                        {
                            mailBody.AppendFormat($"<li>{tSchedule.topicList[i].ToString()}</li>");
                        }
                        if (!string.IsNullOrEmpty(tSchedule._Training.OtherTopic))
                            mailBody.Append($"<li>{tSchedule._Training.OtherTopic}</li>");

                        mailBody.AppendFormat($"</ul>");


                        mailBody.AppendFormat("<div> <p>Regards, <br/> Name : " + tSchedule.EmployeeName + "-" + tSchedule.EmployeeCode + "  </p> </div>");

                        emailMessage.Body = mailBody.ToString();
                        Task t1 = Task.Run(() => SendMail(emailMessage, tSchedule.TrainingParticipntList));
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.InnerException + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return true;
        }

        private void SendMail(EmailMessage emailMessage, List<TrainingParticipant> tParticipants)
        {
            foreach (var item in tParticipants)
            {
                emailMessage.To = item.EmailID;
                EmailHelper.SendEmail(emailMessage);
                foreach (var attachment in emailMessage.Attachments)
                {
                    attachment.Content.Dispose();
                }
            }
        }

        #region Training Report        
        public List<Training> GetEmployeeTrainingReport(CommonFilter cFilter)
        {
            log.Info($"TrainingService/GetEmployeeTrainingReport");
            try
            {
                var getTrainingList = genericRepo.Get<DTOModel.TrainingParticipant>(x => (cFilter.EmployeeID.HasValue ? x.EmployeeID == cFilter.EmployeeID.Value : (1 > 0))
                 && (cFilter.StatusID != 0 ? x.Training.TrainingStatus == cFilter.StatusID : (1 > 0))).ToList();

                if (getTrainingList != null && getTrainingList.Count > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.TrainingParticipant, Training>()
                        .ForMember(d => d.StartDate, o => o.MapFrom(s => s.Training.StartDate))
                        .ForMember(d => d.EndDate, o => o.MapFrom(s => s.Training.EndDate))
                        .ForMember(d => d.Address, o => o.MapFrom(s => s.Training.Address))
                        .ForMember(d => d.City, o => o.MapFrom(s => s.Training.City))
                        .ForMember(d => d.StateName, o => o.MapFrom(s => s.Training.State.StateName))
                        .ForMember(d => d.PinCode, o => o.MapFrom(s => s.Training.PinCode))
                        .ForMember(d => d.TrainingTitle, o => o.MapFrom(s => s.Training.TrainingTitle))
                        .ForMember(d => d.TrainingTypeName, o => o.MapFrom(s => ((EnumTrainingList)s.Training.TrainingList).GetDisplayName()))
                        .ForMember(d => d.TrainingStatus, o => o.MapFrom(s => s.Training.TrainingStatus))
                        .ForMember(d => d.EmpName, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode + " - " + s.tblMstEmployee.Name))
                        .ForMember(d => d.VendorName, o => o.MapFrom(s => s.Training.VendorName))
                        .ForMember(d => d.TrainingType, o => o.MapFrom(s => s.Training.TrainingType))
                        .ForMember(d => d.TrainingObjective, o => o.MapFrom(s => s.Training.TrainingObjective))
                        .ForMember(d => d.InternalTrainer, o => o.MapFrom(s => s.Training.InternalTraining))
                        .ForMember(d => d.ExternalTrainer, o => o.MapFrom(s => s.Training.ExternalTraining))
                        .ForMember(d => d.InternalAddressType, o => o.MapFrom(s => s.Training.InternalAddressType))
                        .ForMember(d => d.BothTrainer, o => o.MapFrom(s => s.Training.BothTraining))
                        .ForMember(d => d.ModeofTraining, o => o.MapFrom(s => s.Training.ModeofTraining))
                        .ForMember(d => d.FeedBackFormHdrID, o => o.MapFrom(s => s.Training.TrainingFeedBackFormHdrs.FirstOrDefault().FeedBackFormHdrID))
                        .ForMember(d => d.FeedbackFormStatus, o => o.MapFrom(s => s.FeedbackFormStatus))
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        ;
                    });
                    var dtoTrainingList = Mapper.Map<List<Training>>(getTrainingList); ;

                    return dtoTrainingList.OrderBy(y => y.EmpName).ToList();
                }
                else
                    return new List<Training>();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public List<Training> GetTrainingReport(CommonFilter cFilter)
        {
            log.Info($"TrainingService/GetTrainingReport");
            try
            {
                //  var getTrainingList = trainingRepo.GetTrainingReport(cFilter.EmployeeID, cFilter.FromDate, cFilter.ToDate, cFilter.StatusID).ToList();
                cFilter.StatusID = cFilter.StatusID.HasValue && cFilter.StatusID > 0 ? cFilter.StatusID : null;
                var getTrainingList = trainingRepo.GetTrainingReport(cFilter.FromDate, cFilter.ToDate, cFilter.StatusID).ToList();
                if (getTrainingList != null && getTrainingList.Count > 0)
                {

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.GetTrainingReport_Result, Training>()
                        .ForMember(d => d.StartDate, o => o.MapFrom(s => s.StartDate))
                        .ForMember(d => d.EndDate, o => o.MapFrom(s => s.EndDate))
                        .ForMember(d => d.Address, o => o.MapFrom(s => s.Address))
                        .ForMember(d => d.City, o => o.MapFrom(s => s.City))
                        .ForMember(d => d.StateName, o => o.MapFrom(s => s.StateName))
                        .ForMember(d => d.PinCode, o => o.MapFrom(s => s.PinCode))
                        .ForMember(d => d.TrainingTitle, o => o.MapFrom(s => s.TrainingTitle))
                        .ForMember(d => d.enumTrainingList, o => o.MapFrom(s => ((EnumTrainingList)s.TrainingList).GetDisplayName()))
                        .ForMember(d => d.TrainingStatus, o => o.MapFrom(s => s.TrainingStatus))
                        .ForMember(d => d.VendorName, o => o.MapFrom(s => s.VendorName))
                         .ForMember(d => d.TrainingType, o => o.MapFrom(s => s.TrainingType))
                        .ForMember(d => d.TrainingObjective, o => o.MapFrom(s => s.TrainingObjective))
                         .ForMember(d => d.InternalTrainer, o => o.MapFrom(s => s.InternalTraining))
                        .ForMember(d => d.ExternalTrainer, o => o.MapFrom(s => s.ExternalTraining))
                        //   .ForMember(d => d.InternalAddressType, o => o.MapFrom(s => s.InternalAddressType))
                        .ForMember(d => d.BothTrainer, o => o.MapFrom(s => s.BothTraining))
                        .ForMember(d => d.ModeofTraining, o => o.MapFrom(s => s.ModeofTraining))
                        .ForMember(d => d.Rating, o => o.MapFrom(s => s.Rating));
                    });
                    var dtoTrainingList = Mapper.Map<List<Training>>(getTrainingList); ;

                    return dtoTrainingList;
                }
                else
                    return new List<Training>();
            }
            catch (Exception)
            {
                throw;
            }

        }
        public bool ExportEmployeeList(System.Data.DataTable dtTable, string sFullPath, string fileName, string tFilter)
        {
            log.Info($"TrainingService/ExportEmployeeList/{sFullPath}/{fileName}");
            try
            {
                var flag = false;
                if (dtTable != null)
                {
                    if (System.IO.Directory.Exists(sFullPath))
                    {
                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                        exportHdr = dtTable.Columns.Cast<System.Data.DataColumn>()
                            .Select(x => x.ColumnName).AsEnumerable<string>();
                        sFullPath = $"{sFullPath}{fileName}";
                        ExportToExcel(exportHdr, dtTable, fileName, sFullPath, tFilter);
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Training> GetTrainingRrtDesignationWise(CommonFilter cFilter)
        {
            log.Info($"TrainingService/GetTrainingRrtDesignationWise");
            try
            {
                //  var getTrainingList = trainingRepo.GetTrainingReport(cFilter.EmployeeID, cFilter.FromDate, cFilter.ToDate, cFilter.StatusID).ToList();
                cFilter.StatusID = cFilter.StatusID.HasValue && cFilter.StatusID > 0 ? cFilter.StatusID : null;
                var getTrainingList = trainingRepo.GetTrainingRrtDesignationWise(cFilter.DesignationID.Value, cFilter.FromDate, cFilter.ToDate).ToList();
                if (getTrainingList != null && getTrainingList.Count > 0)
                {

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.GetTrainingRrtDesignationWise_Result, Training>()
                        .ForMember(d => d.EmpName, o => o.MapFrom(s => s.NAME))
                        .ForMember(d => d.StartDate, o => o.MapFrom(s => s.StartDate))
                        .ForMember(d => d.EndDate, o => o.MapFrom(s => s.EndDate))
                        .ForMember(d => d.Address, o => o.MapFrom(s => s.Venue))
                        .ForMember(d => d.TrainingTitle, o => o.MapFrom(s => s.TrainingTitle))
                        .ForMember(d => d.enumTrainingList, o => o.MapFrom(s => ((EnumTrainingList)s.TrainingList)))
                        .ForMember(d => d.VendorName, o => o.MapFrom(s => s.VendorName))
                        .ForMember(d => d.TrainingType, o => o.MapFrom(s => s.TrainingType))
                        .ForMember(d => d.TrainingObjective, o => o.MapFrom(s => s.TrainingObjective))
                        .ForMember(d => d.InternalTrainer, o => o.MapFrom(s => s.InternalTraining))
                        .ForMember(d => d.ExternalTrainer, o => o.MapFrom(s => s.ExternalTraining))
                        .ForMember(d => d.BothTrainer, o => o.MapFrom(s => s.BothTraining))
                        //   .ForMember(d => d.InternalAddressType, o => o.MapFrom(s => s.InternalAddressType))                        
                        .ForMember(d => d.ModeofTraining, o => o.MapFrom(s => s.ModeofTraining))
                        .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                        .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                        .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.DesignationName));
                    });
                    var dtoTrainingList = Mapper.Map<List<Training>>(getTrainingList);

                    return dtoTrainingList.OrderBy(y => y.EmpName).ToList();
                }
                else
                    return new List<Training>();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public bool ExportDesignationWise(System.Data.DataTable dtTable, string sFullPath, string fileName, string tFilter)
        {
            log.Info($"TrainingService/ExportDesignationWise/{sFullPath}/{fileName}");
            try
            {
                var flag = false;
                if (dtTable != null)
                {
                    if (System.IO.Directory.Exists(sFullPath))
                    {
                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                        exportHdr = dtTable.Columns.Cast<System.Data.DataColumn>()
                            .Select(x => x.ColumnName).AsEnumerable<string>();
                        sFullPath = $"{sFullPath}{fileName}";
                        ExportTrainingDesignationWise(exportHdr, dtTable, fileName, sFullPath, tFilter);
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Training> GetTrainingRptInternalExternal(CommonFilter cFilter)
        {
            log.Info($"TrainingService/GetTrainingRptInternalExternal");
            try
            {
                var getTrainingList = trainingRepo.GetTrainingRptInternalExternal(cFilter.StatusID.Value, cFilter.FromDate, cFilter.ToDate).ToList();
                if (getTrainingList != null && getTrainingList.Count > 0)
                {

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.GetTrainingRptInternalExternal_Result, Training>()
                        .ForMember(d => d.EmpName, o => o.MapFrom(s => s.NAME))
                        .ForMember(d => d.StartDate, o => o.MapFrom(s => s.StartDate))
                        .ForMember(d => d.EndDate, o => o.MapFrom(s => s.EndDate))
                        .ForMember(d => d.Address, o => o.MapFrom(s => s.Venue))
                        .ForMember(d => d.TrainingTitle, o => o.MapFrom(s => s.TrainingTitle))
                        .ForMember(d => d.enumTrainingList, o => o.MapFrom(s => ((EnumTrainingList)s.TrainingList).GetDisplayName()))
                        .ForMember(d => d.VendorName, o => o.MapFrom(s => s.VendorName))
                         .ForMember(d => d.TrainingType, o => o.MapFrom(s => s.TrainingType))
                        .ForMember(d => d.TrainingObjective, o => o.MapFrom(s => s.TrainingObjective))
                          .ForMember(d => d.InternalTrainer, o => o.MapFrom(s => s.InternalTraining))
                        .ForMember(d => d.ExternalTrainer, o => o.MapFrom(s => s.ExternalTraining))
                         .ForMember(d => d.BothTrainer, o => o.MapFrom(s => s.BothTraining))
                        //   .ForMember(d => d.InternalAddressType, o => o.MapFrom(s => s.InternalAddressType))                        
                        .ForMember(d => d.ModeofTraining, o => o.MapFrom(s => s.ModeofTraining))
                        .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID));
                    });
                    var dtoTrainingList = Mapper.Map<List<Training>>(getTrainingList); ;

                    return dtoTrainingList.OrderBy(y => y.EmpName).ToList();
                }
                else
                    return new List<Training>();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public bool ExportInternalExternalWise(System.Data.DataTable dtTable, string sFullPath, string fileName, string tFilter)
        {
            log.Info($"TrainingService/ExportInternalExternalWise/{sFullPath}/{fileName}");
            try
            {
                var flag = false;
                if (dtTable != null)
                {
                    if (System.IO.Directory.Exists(sFullPath))
                    {
                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                        exportHdr = dtTable.Columns.Cast<System.Data.DataColumn>()
                            .Select(x => x.ColumnName).AsEnumerable<string>();
                        sFullPath = $"{sFullPath}{fileName}";
                        ExportTrainingInternalExternalWise(exportHdr, dtTable, fileName, sFullPath, tFilter);
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Training> GetTrainingRptTypeWise(CommonFilter cFilter)
        {
            log.Info($"TrainingService/GetTrainingRptTypeWise");
            try
            {
                var getTrainingList = trainingRepo.GetTrainingRptTypeWise(cFilter.ProcessID, cFilter.FromDate, cFilter.ToDate).ToList();
                if (getTrainingList != null && getTrainingList.Count > 0)
                {

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.GetTrainingRptTypeWise_Result, Training>()
                        .ForMember(d => d.EmpName, o => o.MapFrom(s => s.NAME))
                        .ForMember(d => d.StartDate, o => o.MapFrom(s => s.StartDate))
                        .ForMember(d => d.EndDate, o => o.MapFrom(s => s.EndDate))
                        .ForMember(d => d.Address, o => o.MapFrom(s => s.Venue))
                        .ForMember(d => d.TrainingTitle, o => o.MapFrom(s => s.TrainingTitle))
                        .ForMember(d => d.enumTrainingList, o => o.MapFrom(s => ((EnumTrainingList)s.TrainingList).GetDisplayName()))
                        .ForMember(d => d.VendorName, o => o.MapFrom(s => s.VendorName))
                        .ForMember(d => d.TrainingType, o => o.MapFrom(s => s.TrainingType))
                        .ForMember(d => d.TrainingObjective, o => o.MapFrom(s => s.TrainingObjective))
                        .ForMember(d => d.InternalTrainer, o => o.MapFrom(s => s.InternalTraining))
                        .ForMember(d => d.ExternalTrainer, o => o.MapFrom(s => s.ExternalTraining))
                        .ForMember(d => d.BothTrainer, o => o.MapFrom(s => s.BothTraining))
                        .ForMember(d => d.ModeofTraining, o => o.MapFrom(s => s.ModeofTraining))
                        .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                        .ForMember(d => d.TrainingType, o => o.MapFrom(s => s.TrainingType));
                    });
                    var dtoTrainingList = Mapper.Map<List<Training>>(getTrainingList);

                    return dtoTrainingList.OrderBy(y => y.EmpName).ToList();
                }
                else
                    return new List<Training>();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public bool ExportTypeWise(System.Data.DataTable dtTable, string sFullPath, string fileName, string tFilter)
        {
            log.Info($"TrainingService/ExportTypeWise/{sFullPath}/{fileName}");
            try
            {
                var flag = false;
                if (dtTable != null)
                {
                    if (System.IO.Directory.Exists(sFullPath))
                    {
                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                        exportHdr = dtTable.Columns.Cast<System.Data.DataColumn>()
                            .Select(x => x.ColumnName).AsEnumerable<string>();
                        sFullPath = $"{sFullPath}{fileName}";
                        ExportTrainingTypeWise(exportHdr, dtTable, fileName, sFullPath, tFilter);
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> GetTrainingProviderList()
        {
            log.Info($"TrainingService/GetTrainingProviderList");
            try
            {
                var getTrainingList = genericRepo.Get<DTOModel.Training>(x => x.BothTraining == true).Distinct();
                if (getTrainingList != null)
                {

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.Training, SelectListModel>()
                        .ForMember(d => d.id, o => o.MapFrom(s => s.TrainingID))
                        .ForMember(d => d.value, o => o.MapFrom(s => s.DirectorName));
                    });
                    var dtoTrainingList = Mapper.Map<List<SelectListModel>>(getTrainingList);

                    return dtoTrainingList;
                }
                else
                    return new List<SelectListModel>();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public List<Training> GetTrainingProviderReport(CommonFilter cFilter)
        {
            log.Info($"TrainingService/GetTrainingProviderReport");
            try
            {
                string[] getProviderList = null;
                if (cFilter.ReportType.Equals("All"))
                {
                    getProviderList = genericRepo.Get<DTOModel.Training>(x => x.BothTraining == true).Select(em => em.DirectorName).ToArray();
                }
                else if (!cFilter.ReportType.Equals("All"))
                {
                    getProviderList = genericRepo.Get<DTOModel.Training>(x => x.BothTraining == true && cFilter.CheckBoxList.Any(y => y == x.TrainingID)).Select(em => em.DirectorName).ToArray();
                }
                var getTrainingList = trainingRepo.GetTrainingProviderReport(getProviderList, cFilter.FromDate, cFilter.ToDate).ToList();
                if (getTrainingList != null && getTrainingList.Count > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.TrainingParticipant, Training>()
                        .ForMember(d => d.StartDate, o => o.MapFrom(s => s.Training.StartDate))
                        .ForMember(d => d.EndDate, o => o.MapFrom(s => s.Training.EndDate))
                        .ForMember(d => d.Address, o => o.MapFrom(s => s.Training.Address))
                        .ForMember(d => d.City, o => o.MapFrom(s => s.Training.City))
                        .ForMember(d => d.StateName, o => o.MapFrom(s => s.Training.State.StateName))
                        .ForMember(d => d.PinCode, o => o.MapFrom(s => s.Training.PinCode))
                        .ForMember(d => d.TrainingTitle, o => o.MapFrom(s => s.Training.TrainingTitle))
                        .ForMember(d => d.enumTrainingList, o => o.MapFrom(s => ((EnumTrainingList)s.Training.TrainingList).GetDisplayName()))
                        .ForMember(d => d.VendorName, o => o.MapFrom(s => s.Training.VendorName))
                        .ForMember(d => d.TrainingObjective, o => o.MapFrom(s => s.Training.TrainingObjective))
                        .ForMember(d => d.InternalTrainer, o => o.MapFrom(s => s.Training.InternalTraining))
                        .ForMember(d => d.ExternalTrainer, o => o.MapFrom(s => s.Training.ExternalTraining))
                        .ForMember(d => d.BothTrainer, o => o.MapFrom(s => s.Training.BothTraining))
                        .ForMember(d => d.ModeofTraining, o => o.MapFrom(s => s.Training.ModeofTraining))
                        .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                        .ForMember(d => d.TrainingType, o => o.MapFrom(s => s.Training.TrainingType))
                        .ForMember(d => d.DirectorName, o => o.MapFrom(s => s.Training.DirectorName))
                        .ForMember(d => d.EmpName, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode + "-" + s.tblMstEmployee.Name));
                    });
                    var dtoTrainingList = Mapper.Map<List<Training>>(getTrainingList);
                    return dtoTrainingList.OrderBy(y => y.EmpName).ToList();

                }
                else
                {
                    return new List<Training>();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public bool ExportProviderWise(System.Data.DataTable dtTable, string sFullPath, string fileName, string tFilter)
        {
            log.Info($"TrainingService/ExportProviderWise/{sFullPath}/{fileName}");
            try
            {
                var flag = false;
                if (dtTable != null)
                {
                    if (System.IO.Directory.Exists(sFullPath))
                    {
                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                        exportHdr = dtTable.Columns.Cast<System.Data.DataColumn>()
                            .Select(x => x.ColumnName).AsEnumerable<string>();
                        sFullPath = $"{sFullPath}{fileName}";
                        ExportTrainingProviderWise(exportHdr, dtTable, fileName, sFullPath, tFilter);
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        public List<TrainingDateWiseTimeSlot> GetTrainingTimeSlots(int trainingID)
        {
            log.Info($"TrainingService/GetTrainingTimeSlots/trainingID:{trainingID}");
            try
            {
                var dtoTrainingTimeSlots = genericRepo.Get<DTOModel.TrainingDateWiseTimeSlot>(x => !x.IsDeleted
                && x.TrainingID == trainingID).OrderBy(y => y.TrainingDate).ToList();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TrainingDateWiseTimeSlot, TrainingDateWiseTimeSlot>();
                });
                return Mapper.Map<List<TrainingDateWiseTimeSlot>>(dtoTrainingTimeSlots);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public TrainingRating GetTrainingRatings(int trainingID)
        {
            log.Info($"TrainingService/GetTrainingRatings/trainingID:{trainingID}");
            try
            {
                TrainingRating trRating = new TrainingRating() { trainingID = trainingID, avgRating = new List<double>() };

                var empRatingDt = trainingRepo.GetTrainingRatings(trainingID);
                if (empRatingDt?.Rows.Count > 0)
                {
                    trRating.empRatingRows = empRatingDt;
                    trRating.headerCols = (from dc in empRatingDt.Columns.Cast<DataColumn>()
                                           select dc.ColumnName).ToList<string>();

                    for (int i = 3; i < trRating.headerCols.Count; i++)
                    {
                        var avg = empRatingDt.AsEnumerable().Average((row) => Convert.ToDouble(row[trRating.headerCols[i]]));
                        trRating.avgRating.Add(avg);
                    }

                }
                return trRating;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string ExportTrainingRating(int trainingID, string filePath, string fileName)
        {
            log.Info($"TrainingService/ExportTrainingRating/trainingID:{trainingID}");

            string result = string.Empty; string sFullPath = string.Empty;
            var model = GetTrainingRatings(trainingID);

            var arpSkillDetail = GetTrainingDetail(trainingID);
            var _training = arpSkillDetail._Training;



            if (_training.TimeSlotType == (byte)EnumTimeSlotType.Distributed)
            {
                _training.distributedTimeSlots = GetTrainingTimeSlots(trainingID);
                _training.enumTimeSlotType = EnumTimeSlotType.Distributed;
            }

            if (arpSkillDetail._Training.OrientationTraining)
                _training.enumResidentialNonResidential = EnumResidentialNonResidential.Residential;
            else if (arpSkillDetail._Training.OnBoardTraining)
                _training.enumResidentialNonResidential = EnumResidentialNonResidential.NonResidential;

            if (arpSkillDetail._Training.TrainingType == (int)EnumTrainingType.Behavioral)
                _training.TrainingTypeName = EnumTrainingType.Behavioral.GetDisplayName();
            else if (arpSkillDetail._Training.TrainingType == (int)EnumTrainingType.Functional)
                _training.TrainingTypeName = EnumTrainingType.Functional.GetDisplayName();

            _training.enumTrainingList = (EnumTrainingList)(arpSkillDetail._Training.TrainingType);
            _training.enumTimeSlotType = (EnumTimeSlotType)(arpSkillDetail._Training.TimeSlotType);
            //=======fghgfhfgh
            string venueAddress = string.Empty;

            if (!string.IsNullOrEmpty(_training.Address))
            {
                if (!string.IsNullOrEmpty(_training.StateName))
                    venueAddress += _training.Address + ",";

                else
                    venueAddress += _training.Address;
            }
            if (!string.IsNullOrEmpty(_training.StateName))
            {
                if (!string.IsNullOrEmpty(_training.City))
                    venueAddress += _training.StateName + ",";

                else
                    venueAddress += _training.StateName;
            }

            if (!string.IsNullOrEmpty(_training.City))
            {
                if (!string.IsNullOrEmpty(_training.PinCode))

                    venueAddress += _training.City + ",";

                else
                    venueAddress += _training.City;

            }
            if (!string.IsNullOrEmpty(_training.PinCode))
                venueAddress += _training.PinCode;

            model.TrainingVenue = venueAddress;

            if (model.empRatingRows?.Rows.Count > 0)
            {
                model.headerCols = (from dc in model.empRatingRows.Columns.Cast<DataColumn>()
                                    select dc.ColumnName).ToList<string>();

                for (int i = 3; i < model.headerCols.Count; i++)
                {
                    var avg = model.empRatingRows.AsEnumerable().Average((row) =>
                     Convert.ToDouble(row[model.headerCols[i]]));
                    model.avgRating.Add(avg);
                }

                if (Directory.Exists(filePath))
                {
                    sFullPath = $"{filePath}{fileName}";
                    result = ImportExport.TrainingReportsExport.ExportTrainingRating(_training,
                        model.TrainingVenue,
                        model.headerCols, model.empRatingRows,
                        $"Training Rating", sFullPath);
                }
                else
                    result = "norec";
            }
            else
                result = "notfound";

            return result;
        }

        public List<TrainingParticipant> GetTrainingNomineePending(Model.CommonFilter cFilter)
        {
            log.Info($"TrainingService/GetTrainingNomineePending");
            try
            {
                List<DTOModel.TrainingParticipant> dtoFormHdrs = new List<DTOModel.TrainingParticipant>();

                var appformsHdr = genericRepo.Get<DTOModel.TrainingParticipant>(x => x.tblMstEmployee.EmployeeProcessApprovals
                 .Any(y => y.ProcessID == (int)WorkFlowProcess.LeaveApproval
                 && y.ToDate == null
                 && (y.ReportingTo == cFilter.loggedInEmployee || y.ReviewingTo == cFilter.loggedInEmployee) && x.Nomination == true && x.NominationAccepted != 2)
                 );

                if (cFilter != null)
                {
                    if (cFilter.FromDate.HasValue && cFilter.ToDate.HasValue)
                        dtoFormHdrs = appformsHdr.Where(x => (x.CreatedOn.Date >= cFilter.FromDate.Value.Date && x.CreatedOn.Date <= cFilter.ToDate.Value.Date)).ToList();
                    //if (cFilter.StatusID.HasValue)
                    //    dtoFormHdrs = appformsHdr.Where(x => x.NominationAccepted == cFilter.StatusID.Value).ToList();
                }
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TrainingParticipant, Model.TrainingParticipant>()
                    .ForMember(d => d.TrainingParticipantID, o => o.MapFrom(s => s.TrainingParticipantID))
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(d => d.EmployeeBranch, o => o.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                    .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                    .ForMember(d => d.EmpProceeApproval, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.ProcessID == (int)WorkFlowProcess.LeaveApproval && y.ToDate == null && y.EmployeeID == s.EmployeeID)))
                    .ForMember(d => d.TrainingName, o => o.MapFrom(s => s.Training.TrainingTitle))
                    .ForMember(d => d.Nomination, o => o.MapFrom(s => s.Nomination))
                    .ForMember(d => d.NominationAccepted, o => o.MapFrom(s => s.NominationAccepted))
                    .ForMember(d => d.StartDate, o => o.MapFrom(s => s.Training.StartDate))
                    .ForMember(d => d.EndDate, o => o.MapFrom(s => s.Training.EndDate))
                    .ForMember(d => d.Address, o => o.MapFrom(s => s.Training.Address))
                    .ForMember(d => d.City, o => o.MapFrom(s => s.Training.City))
                    .ForMember(d => d.StateName, o => o.MapFrom(s => s.Training.State.StateName))
                    .ForMember(d => d.PinCode, o => o.MapFrom(s => s.Training.PinCode))
                    .ForAllOtherMembers(d => d.Ignore());
                    cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();
                });
                var listTrainingParticipants = Mapper.Map<List<Model.TrainingParticipant>>(dtoFormHdrs);
                return listTrainingParticipants.ToList();

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool NominationApproval(EmployeeLeave participantsData)
        {
            log.Info($"TrainingService/NominationApproval");
            bool flag = false;
            try
            {
                var getdata = genericRepo.Get<DTOModel.TrainingParticipant>(x => x.TrainingParticipantID == participantsData.LeaveID).FirstOrDefault();
                if (getdata != null)
                {
                    getdata.NominationAccepted = (byte)participantsData.StatusID;
                    genericRepo.Update(getdata);
                    flag = true;
                }
                if (flag)
                {
                    AddProcessWorkFlow(participantsData._ProcessWorkFlow);
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool FeedbackFormGenerated(int trainingID)
        {
            log.Info($"TrainingService/FeedbackFormGenerated/trainingID={trainingID}");
            try
            {
                return genericRepo.Exists<DTOModel.TrainingFeedBackFormHdr>(x => x.TrainingID == trainingID);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }


        }

        public List<Model.TrainingDocumentRepository> GetSelectedDocument(int? trainingID, int[] arrDocument)
        {
            log.Info($"TrainingService/GetSelectedDocument/{trainingID}");
            try
            {
                var result = genericRepo.Get<DTOModel.TrainingDocumentRepository>(x => x.TrainingID == trainingID && arrDocument.Any(y => y == x.TrainingDocumentID));

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TrainingDocumentRepository, Model.TrainingDocumentRepository>()
                    .ForMember(d => d.TrainingDocumentID, o => o.MapFrom(s => s.TrainingDocumentID))
                    .ForMember(d => d.TrainingID, o => o.MapFrom(s => s.TrainingID))
                    .ForMember(d => d.DocumentName, o => o.MapFrom(s => s.DocumentName))
                    .ForMember(d => d.DocumentPathName, o => o.MapFrom(s => s.DocumentPathName))
                    .ForMember(d => d.DocumentDetail, o => o.MapFrom(s => s.DocumentDetail))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var listTrainingDocument = Mapper.Map<List<Model.TrainingDocumentRepository>>(result);
                return listTrainingDocument.ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public string[] GetTrainingProviderNames(CommonFilter cFilter)
        {
            log.Info($"TrainingService/GetTrainingProviderNames");
            try
            {
                string[] getProviderList = null;
                if (cFilter.ReportType.Equals("All"))
                {
                    getProviderList = genericRepo.Get<DTOModel.Training>(x => x.BothTraining == true).Select(em => em.DirectorName).ToArray();
                }
                else if (!cFilter.ReportType.Equals("All"))
                {
                    getProviderList = genericRepo.Get<DTOModel.Training>(x => x.BothTraining == true && cFilter.CheckBoxList.Any(y => y == x.TrainingID)).Select(em => em.DirectorName).ToArray();

                }
                return getProviderList;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public bool DeleteTraining(int trainingID)
        {
            log.Info($"TrainingService/DeleteTraining/{trainingID}");

            Training trainingInfo = new Training();
            try
            {
                var dtoTrainingInfo = genericRepo.GetByID<DTOModel.Training>(trainingID);
                if (dtoTrainingInfo != null)
                {
                    dtoTrainingInfo.IsDeleted = true;
                    genericRepo.Update(dtoTrainingInfo);
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
    }
}
