using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;
using AutoMapper;
using Nafed.MicroPay.Model;
using System.Web;
using Nafed.MicroPay.Common;
using System.IO;
using System.Data;
using System.Text;
using static Nafed.MicroPay.Common.FileHelper;
using System.ComponentModel;

namespace MicroPay.Web.Controllers.Appraisal
{
    public class TrainingController : BaseController
    {
        // GET: Training
        private readonly IDropdownBindService ddlService;
        private readonly ITrainingService trainingService;

        public TrainingController(IDropdownBindService ddlService, ITrainingService trainingService)
        {
            this.ddlService = ddlService;
            this.trainingService = trainingService;
        }
        public ActionResult Index()
        {
            log.Info($"TrainingController/Index");

            return View();
        }
        public PartialViewResult GetFilter()
        {
            log.Info("TrainingController/GetFilter");
            try
            {
                CommonFilter cFilter = new CommonFilter();
                return PartialView("_Filter", cFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public PartialViewResult PostFilter(CommonFilter cFilter)
        {
            log.Info("TrainingController/PostFilter");
            try
            {
                TrainingViewModel trnVM = new TrainingViewModel();

                trnVM.userRights = userAccessRight;
                var getTrainingList = trainingService.GetTrainingList(cFilter.Year, (int?)cFilter.Month);
                trnVM.TrainingList = getTrainingList;
                return PartialView("_TrainingGridView", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public ActionResult _GetTrainingGridView()
        {
            log.Info("TrainingController/GenerateGeneralTab");
            try
            {
                TrainingViewModel trnVM = new TrainingViewModel();

                trnVM.userRights = userAccessRight;
                var getTrainingList = trainingService.GetTrainingList();
                trnVM.TrainingList = getTrainingList;
                return PartialView("_TrainingGridView", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info("TrainingController/Create");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        void BindDropDown()
        {
            var ddlState = ddlService.ddlStateList();
            ddlState.OrderBy(x => x.value);
            SelectListModel State = new SelectListModel();
            State.id = 0;
            State.value = "Select";
            ddlState.Insert(0, State);
            ViewBag.ddlState = new SelectList(ddlState, "id", "value");
        }

        public ActionResult GenerateGeneralTab()
        {
            log.Info("TrainingController/GenerateGeneralTab");
            try
            {
                BindDropDown();
                TrainingViewModel trnVM = new TrainingViewModel();
                trnVM._Training = new Training();
                List<SelectListModel> trainingType = new List<SelectListModel>();
                trainingType.Add(new SelectListModel { id = 2, value = "Behavioral" });
                trainingType.Add(new SelectListModel { id = 3, value = "Functional/Technical" });

                trnVM.ddlTrainningType = trainingType;
                //trnVM._Training.enumTrainingStatus =EnumTrainingStatus.Schedule;
                var selectedCheckboxesBehavioral = new List<CheckBox>();
                var getFieldsValueBehavioral = ddlService.ddlSkill(2);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                trnVM.CheckBoxListBehavioral.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueBehavioral);
                trnVM.CheckBoxListBehavioral.SelectedFields = selectedCheckboxesBehavioral;

                //-----------------------------------
                var selectedCheckboxesFunctional = new List<CheckBox>();
                var getFieldsValueFunctional = ddlService.ddlSkill(3);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                trnVM.CheckBoxListFunctional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueFunctional);
                trnVM.CheckBoxListFunctional.SelectedFields = selectedCheckboxesFunctional;

                return PartialView("_TrainingSchedule", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GenerateGeneralTab(TrainingViewModel trnVM)
        {
            log.Info("TrainingController/GenerateGeneralTab");
            try
            {

                if (trnVM._Training.enumTimeSlotType == EnumTimeSlotType.Distributed)
                    trnVM._Training.DistributedTimeSlot = true;
                else if (trnVM._Training.enumTimeSlotType == EnumTimeSlotType.Standard)
                    trnVM._Training.StandardTimeSlot = true;

                ModelState.Remove("_Training.TrainingID");


                var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
                if (trnVM.CheckBoxListBehavioral.PostedFields == null && trnVM.CheckBoxListFunctional.PostedFields == null)
                {
                    ModelState.AddModelError("SkillValidator", "Please Select Skill(s)");
                }
                if (string.IsNullOrEmpty(trnVM._Training.Address))
                    ModelState.AddModelError("AddressValidator", "Please enter Address");
                if (string.IsNullOrEmpty(trnVM._Training.City))
                    ModelState.AddModelError("CityValidator", "Please enter City");
                //if (string.IsNullOrEmpty(trnVM._Training.PinCode))
                //    ModelState.AddModelError("PinCodeValidator", "Please enter PinCode");
                if (string.IsNullOrEmpty(trnVM._Training.TrainingTitle))
                    ModelState.AddModelError("TrainingTitleValidator", "Please enter Training Title");

                if (trnVM._Training.StartDate.HasValue && trnVM._Training.EndDate.HasValue)
                {
                    if (trnVM._Training.StartDate.Value > trnVM._Training.EndDate.Value)
                    {
                        ModelState.AddModelError("StartDateValidator", "Start Date can't be greater then End Date.");
                    }
                }

                if (trnVM._Training.NominationDate.HasValue)
                {
                    if (trnVM._Training.NominationDate.Value > trnVM._Training.EndDate.Value)
                    {
                        ModelState.AddModelError("NominationDateValidator", "Nomnation Date can't be greater then End Date.");
                    }
                }
                if (ModelState.IsValid)
                {

                    if (trnVM._Training.TrainingID > 0)
                    {
                        #region Update Block

                        TrainingSchedule trnSchedule = new TrainingSchedule();
                        trnSchedule._Training = new Training();
                        trnSchedule._Training = trnVM._Training;
                        trnSchedule._Training.TimeSlotType = (byte)trnVM._Training.enumTimeSlotType;

                        if (trnVM._Training.TrainingType == 2)
                        {
                            if (trnVM.CheckBoxListBehavioral.PostedFields != null)
                                trnSchedule.CheckBoxListBehavioral = trnVM.CheckBoxListBehavioral.PostedFields.fieldIds;
                        }
                        else if (trnVM._Training.TrainingType == 3)
                        {
                            if (trnVM.CheckBoxListFunctional.PostedFields != null)
                                trnSchedule.CheckBoxListFunctional = trnVM.CheckBoxListFunctional.PostedFields.fieldIds;
                        }
                        trnSchedule._Training.UpdatedBy = userDetail.UserID;
                        trnSchedule._Training.UpdatedOn = DateTime.Now;
                        trnSchedule._Training.StateID = trnVM._Training.StateID == 0 ? null : trnVM._Training.StateID;
                        trnSchedule._Training.TrainingTitle = trnVM._Training.TrainingTitle;

                        int res = trainingService.UpdateGeneralTabDetail(trnSchedule);
                        if (res > 0)
                        {
                            BindDropDown();
                            List<SelectListModel> trainingType = new List<SelectListModel>();
                            trainingType.Add(new SelectListModel { id = 2, value = "Behavioral" });
                            trainingType.Add(new SelectListModel { id = 3, value = "Functional/Technical" });
                            trnVM.ddlTrainningType = trainingType;
                            var selectedCheckboxesBehavioral = new List<CheckBox>();
                            var getFieldsValueBehavioral = ddlService.ddlSkill(2);

                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<SelectListModel, CheckBox>()
                                .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                                .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                            });
                            trnVM.CheckBoxListBehavioral.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueBehavioral);
                            if (trnVM.CheckBoxListBehavioral.PostedFields != null)
                            {
                                selectedCheckboxesBehavioral = new List<CheckBox>(trnVM.CheckBoxListBehavioral.PostedFields.fieldIds.Select(x => new CheckBox { Id = x }));
                                trnVM.CheckBoxListBehavioral.SelectedFields = selectedCheckboxesBehavioral;
                            }

                            //-----------------------------------
                            var selectedCheckboxesFunctional = new List<CheckBox>();
                            var getFieldsValueFunctional = ddlService.ddlSkill(3);
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<SelectListModel, CheckBox>()
                                .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                                .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                            });
                            trnVM.CheckBoxListFunctional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueFunctional);
                            if (trnVM.CheckBoxListFunctional.PostedFields != null)
                            {
                                selectedCheckboxesFunctional = new List<CheckBox>(trnVM.CheckBoxListFunctional.PostedFields.fieldIds.Select(x => new CheckBox { Id = x }));
                                trnVM.CheckBoxListFunctional.SelectedFields = selectedCheckboxesFunctional;
                            }
                            //========== new line of codes ========= Dated - 17-Oct-2020

                            trnVM._Training.TrainingDates =
                                trnVM._Training.distributedTimeSlots.Select(x => new SelectListModel { id = x.TrainingTimeSlotID, value = x.TrainingDate.ToString("dd/MM/yyyy") }).ToList();
                            return Json(new { status = 2, trainingID = res, activeTab = 2, msgType = "success", msg = "Successfully Updated", htmlData = ConvertViewToString("_TrainingSchedule", trnVM) }, JsonRequestBehavior.AllowGet);

                        }

                        #endregion

                    }
                    else
                    {
                        #region Insert Block

                        TrainingSchedule trnSchedule = new TrainingSchedule();
                        trnSchedule._Training = new Training();
                        trnSchedule._Training = trnVM._Training;
                        trnSchedule._Training.TrainingStatus = (byte)EnumTrainingStatus.Planned;
                        trnSchedule._Training.TimeSlotType = (byte)trnSchedule._Training.enumTimeSlotType;

                        if (trnVM._Training.TrainingType == 2)
                        {
                            if (trnVM.CheckBoxListBehavioral.PostedFields != null)
                                trnSchedule.CheckBoxListBehavioral = trnVM.CheckBoxListBehavioral.PostedFields.fieldIds;
                        }
                        else if (trnVM._Training.TrainingType == 3)
                        {
                            if (trnVM.CheckBoxListFunctional.PostedFields != null)
                                trnSchedule.CheckBoxListFunctional = trnVM.CheckBoxListFunctional.PostedFields.fieldIds;
                        }
                        trnSchedule._Training.CreatedBy = userDetail.UserID;
                        trnSchedule._Training.CreatedOn = DateTime.Now;
                        trnSchedule._Training.StateID = trnVM._Training.StateID == 0 ? 1 : trnVM._Training.StateID;
                        trnSchedule._Training.TrainingTitle = trnVM._Training.TrainingTitle;
                        int res = trainingService.InsertGeneralTabDetail(trnSchedule);
                        if (res > 0)
                        {
                            TempData["TrainingID"] = res;
                            TempData.Keep("TrainingID");
                            ViewBag.ActiveTab = 1;
                            ViewBag.TrainingID = res;
                            return Json(new { status = "1", trainingID = res, activeTab = 1, msgType = "success", msg = "Training Calender has been created, now you have to provide trainer details in the same screen. If you want to upload any document(s) to related training, you can upload it in next Tab." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            BindDropDown();
                            List<SelectListModel> trainingType = new List<SelectListModel>();
                            trainingType.Add(new SelectListModel { id = 2, value = "Behavioral" });
                            trainingType.Add(new SelectListModel { id = 3, value = "Functional/Technical" });

                            trnVM.ddlTrainningType = trainingType;

                            var selectedCheckboxesBehavioral = new List<CheckBox>();
                            var getFieldsValueBehavioral = ddlService.ddlSkill(2);

                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<SelectListModel, CheckBox>()
                                .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                                .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                            });
                            trnVM.CheckBoxListBehavioral.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueBehavioral);
                            trnVM.CheckBoxListBehavioral.SelectedFields = selectedCheckboxesBehavioral;

                            //-----------------------------------
                            var selectedCheckboxesFunctional = new List<CheckBox>();
                            var getFieldsValueFunctional = ddlService.ddlSkill(3);
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<SelectListModel, CheckBox>()
                                .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                                .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                            });
                            trnVM.CheckBoxListFunctional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueFunctional);
                            trnVM.CheckBoxListFunctional.SelectedFields = selectedCheckboxesFunctional;

                            return PartialView("_TrainingSchedule", trnVM);
                        }
                        #endregion
                    }
                }
                else
                {
                    ModelState.AddModelError("ModelError", "");
                    BindDropDown();
                    List<SelectListModel> trainingType = new List<SelectListModel>();
                    trainingType.Add(new SelectListModel { id = 2, value = "Behavioral" });
                    trainingType.Add(new SelectListModel { id = 3, value = "Functional/Technical" });
                    trnVM.ddlTrainningType = trainingType;
                    var selectedCheckboxesBehavioral = new List<CheckBox>();
                    var getFieldsValueBehavioral = ddlService.ddlSkill(2);

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    trnVM.CheckBoxListBehavioral.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueBehavioral);
                    if (trnVM.CheckBoxListBehavioral.PostedFields != null)
                    {
                        selectedCheckboxesBehavioral = new List<CheckBox>(trnVM.CheckBoxListBehavioral.PostedFields.fieldIds.Select(x => new CheckBox { Id = x }));
                        trnVM.CheckBoxListBehavioral.SelectedFields = selectedCheckboxesBehavioral;
                    }

                    //-----------------------------------
                    var selectedCheckboxesFunctional = new List<CheckBox>();
                    var getFieldsValueFunctional = ddlService.ddlSkill(3);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    trnVM.CheckBoxListFunctional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueFunctional);
                    if (trnVM.CheckBoxListFunctional.PostedFields != null)
                    {
                        selectedCheckboxesFunctional = new List<CheckBox>(trnVM.CheckBoxListFunctional.PostedFields.fieldIds.Select(x => new CheckBox { Id = x }));
                        trnVM.CheckBoxListFunctional.SelectedFields = selectedCheckboxesFunctional;
                    }

                    //=======new line of codes -- Added On - 17-oct-2020=====
                    var tDates = trnVM._Training.distributedTimeSlots
                       .Select(y => y.TrainingDate).OrderBy(y => y).Distinct().ToList();

                    var sno = 1;
                    tDates.ForEach(x =>
                    {
                        trnVM._Training.TrainingDates.Add(
                            new SelectListModel()
                            {
                                id = sno++,
                                value = x.ToString("dd/MM/yyyy")
                            });
                    });
                    //=======End ======================
                    trnVM._Training.distributedTimeSlots = trnVM._Training.distributedTimeSlots.OrderBy(y => y.TrainingDate).ToList();

                    return PartialView("_TrainingSchedule", trnVM);
                }
                return PartialView("_TrainingSchedule", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _EditGeneralTab(int trainingID)
        {
            log.Info($"TrainingController/_EditGeneralTab/{trainingID}");
            try
            {
                var arpSkillDetail = trainingService.GetTrainingDetail(trainingID);

                BindDropDown();
                TrainingViewModel trnVM = new TrainingViewModel();
                trnVM._Training = arpSkillDetail._Training;

                if (trnVM._Training.TimeSlotType == (byte)EnumTimeSlotType.Distributed)
                {
                    trnVM._Training.distributedTimeSlots = trainingService.GetTrainingTimeSlots(trainingID);

                    //========== new line of codes ========= Dated - 17-Oct-2020

                    var tDates = trnVM._Training.distributedTimeSlots
                        .Select(y => y.TrainingDate).OrderBy(y => y).Distinct().ToList();

                    var sno = 1;
                    tDates.ForEach(x =>
                    {
                        trnVM._Training.TrainingDates.Add(
                            new SelectListModel()
                            {
                                id = sno++,
                                value = x.ToString("dd/MM/yyyy")
                            });
                    });

                    //=========== end =======================
                    trnVM._Training.enumTimeSlotType = EnumTimeSlotType.Distributed;
                }

                if (arpSkillDetail._Training.OrientationTraining)
                    trnVM._Training.enumResidentialNonResidential = EnumResidentialNonResidential.Residential;
                else if (arpSkillDetail._Training.OnBoardTraining)
                    trnVM._Training.enumResidentialNonResidential = EnumResidentialNonResidential.NonResidential;
                else if (arpSkillDetail._Training.InternalAddressType)
                    trnVM._Training.enumResidentialNonResidential = EnumResidentialNonResidential.internaltraining;

                trnVM.TrainingStatus = trainingService.GetTrainingDtls(trainingID)?.TrainingStatus ?? 0;

                List<SelectListModel> trainingType = new List<SelectListModel>();
                trainingType.Add(new SelectListModel { id = 2, value = "Behavioral" });
                trainingType.Add(new SelectListModel { id = 3, value = "Functional/Technical" });

                trnVM.ddlTrainningType = trainingType;

                var selectedCheckboxesBehavioral = new List<CheckBox>();
                var getFieldsValueBehavioral = ddlService.ddlSkill(2);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                trnVM.CheckBoxListBehavioral.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueBehavioral);
                trnVM.CheckBoxListBehavioral.SelectedFields = selectedCheckboxesBehavioral;
                trnVM.CheckBoxListBehavioral.SelectedFields = arpSkillDetail.TrainingTopicList.Where(x => x.SkillTypeID == 2).ToList().Select(y => new CheckBox { Id = y.SkillID });

                //-----------------------------------
                var selectedCheckboxesFunctional = new List<CheckBox>();
                var getFieldsValueFunctional = ddlService.ddlSkill(3);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                trnVM.CheckBoxListFunctional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueFunctional);
                trnVM.CheckBoxListFunctional.SelectedFields = selectedCheckboxesFunctional;

                trnVM.CheckBoxListFunctional.SelectedFields = arpSkillDetail.TrainingTopicList.Where(x => x.SkillTypeID == 3).ToList().Select(y => new CheckBox { Id = y.SkillID });

                return PartialView("_TrainingSchedule", trnVM);
            }

            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int trainingID)
        {
            log.Info($"TrainingController/Edit/{trainingID}");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _GetAddMoreTimeSlot(int? trainingID, DateTime fromDate, DateTime toDate)
        {
            log.Info($"TrainingController/_GetDistributedTimeSlots/fromDate:{fromDate}&todate:{toDate}");
            try
            {
                TrainingViewModel trnVM = new TrainingViewModel();
                trnVM._Training = new Training();

                List<TrainingDateWiseTimeSlot> timeSlots = new List<TrainingDateWiseTimeSlot>();

                //if (trainingID.HasValue)
                //{
                //    timeSlots = trainingService.GetTrainingTimeSlots(trainingID.Value);
                //}
                if (!trainingID.HasValue || timeSlots?.Count == 0)
                {
                    DateTime startDate = fromDate;
                    int idx = 0;
                    while (startDate <= toDate)
                    {
                        timeSlots.Add(new TrainingDateWiseTimeSlot
                        {
                            TrainingDate = startDate,
                            sNo = ++idx
                        });
                        startDate = startDate.AddDays(1);
                    }
                }
                trnVM._Training.TrainingDates = (from c in timeSlots.Distinct()
                                                 select new SelectListModel
                                                 {
                                                     value = c.TrainingDate.ToString("dd/MM/yyyy"),
                                                     id = c.sNo
                                                 }).ToList();

                return PartialView("_AddMoreTimeSlot", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult _GetDistributedTimeSlots(int? trainingID, DateTime fromDate, DateTime toDate)
        {
            log.Info($"TrainingController/_GetDistributedTimeSlots/fromDate:{fromDate}&todate:{toDate}");
            try
            {
                TrainingViewModel tvm = new TrainingViewModel();
                tvm._Training = new Training();
                List<TrainingDateWiseTimeSlot> timeSlots = new List<TrainingDateWiseTimeSlot>();

                TempData["timeSlots"] = null;
                //  timeSlots = null;
                //if (trainingID.HasValue)
                //{
                //    timeSlots = trainingService.GetTrainingTimeSlots(trainingID.Value);
                //}
                if (!trainingID.HasValue || timeSlots?.Count == 0)
                {
                    DateTime startDate = fromDate;
                    while (startDate <= toDate)
                    {
                        timeSlots.Add(new TrainingDateWiseTimeSlot
                        {
                            TrainingDate = startDate,
                            CreatedBy = userDetail.UserID,
                            CreatedOn = DateTime.Now

                        });
                        startDate = startDate.AddDays(1);
                    }
                }
                else
                {
                }
                tvm._Training.distributedTimeSlots = timeSlots;
                TempData["timeSlots"] = timeSlots;
                //if (timeSlots.Count > 0)
                // //   TempData["timeSlots"] = timeSlots;
                return PartialView("_DistributedTimeSlot", tvm);
            }
            catch (Exception)
            {
                throw;
            }

        }

        #region Trainer Details

        public ActionResult _EditTrainerDetails(int trainingID)
        {
            log.Info($"TrainingController/_EditTrainerDetails/{trainingID}");
            try
            {
                TrainerDtlsViewModel trainerVM = new TrainerDtlsViewModel();
                //  TrainingViewModel trainingVM = new TrainingViewModel();
                var trainerDtls = trainingService.GetTrainerDetails(trainingID);

                if (trainerDtls != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Training, TrainerDtlsViewModel>();
                    });
                    trainerVM = Mapper.Map<TrainerDtlsViewModel>(trainerDtls);
                    trainerVM.enumTrainingStatus = (EnumTrainingStatus)(trainerDtls.TrainingStatus ?? 0);
                    if (trainerVM.TrainerList?.Count > 0)
                    {
                        int sno = 1;
                        trainerVM.TrainerList.ForEach(x =>
                        {
                            x.sno = sno++;
                            // x.TrainingID = trainingID;
                        });
                    }

                    if (trainerDtls.ExternalTrainer)
                        trainerVM.enumTrainerType = TrainerDtlsViewModel.EnumTrainer.External;
                    else if (trainerDtls.InternalTrainer)
                        trainerVM.enumTrainerType = TrainerDtlsViewModel.EnumTrainer.Internal;
                    else if (trainerDtls.BothTrainer)
                        trainerVM.enumTrainerType = TrainerDtlsViewModel.EnumTrainer.Both;

                }
                return PartialView("_EditTrainerDetails", trainerVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _EditTrainerDetails(TrainerDtlsViewModel trainerVM, string ButtonType)
        {
            log.Info($"TrainingController/_EditTrainerDetails/{"Post"}");


            if (ButtonType == "Add Row")
            {
                if (trainerVM.TrainerList == null)
                    trainerVM.TrainerList = new List<Trainer>() {
                           new Trainer() { sno = 1, } };
                else
                {
                    if (trainerVM.TrainerList.Count == 1)
                        trainerVM.TrainerList.FirstOrDefault().sno = 1;
                    else
                    {
                        var s_no = 1;
                        trainerVM.TrainerList.ForEach(x =>
                        {
                            x.sno = s_no++;
                        });

                    }
                    if (trainerVM.TrainerList.Count < 5)
                        trainerVM.TrainerList.Add(new Trainer()
                        {
                            sno = trainerVM.TrainerList.Count + 1,
                            TrainingID = trainerVM.TrainingID
                        });
                }
                TempData["TrainerList"] = trainerVM;
                return Json(new { part = 1, htmlData = ConvertViewToString("_TrainerList", trainerVM) }, JsonRequestBehavior.AllowGet);
            }
            else if (ButtonType == "Update")
            {
                if (trainerVM.enumTrainerType == TrainerDtlsViewModel.EnumTrainer.Internal)
                {
                    trainerVM.ExternalTrainer = false; trainerVM.BothTrainer = false; trainerVM.InternalTrainer = true;
                    ModelState.Remove("VendorName");
                    ModelState.Remove("VendorAddress");
                    ModelState.Remove("VendorPhoneNo");
                    ModelState.Remove("VendorGSTINNo");
                    ModelState.Remove("DirectorName");
                }
                else if (trainerVM.enumTrainerType == TrainerDtlsViewModel.EnumTrainer.External)
                {
                    trainerVM.ExternalTrainer = true; trainerVM.BothTrainer = false; trainerVM.InternalTrainer = false;
                    ModelState.Remove("VendorName");
                    ModelState.Remove("VendorAddress");
                    ModelState.Remove("VendorPhoneNo");
                    ModelState.Remove("VendorGSTINNo");
                    ModelState.Remove("DirectorName");
                }
                else if (trainerVM.enumTrainerType == TrainerDtlsViewModel.EnumTrainer.Both)
                    trainerVM.BothTrainer = true;

                trainerVM.UpdatedBy = userDetail.UserID;
                trainerVM.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<TrainerDtlsViewModel, Training>();
                    });
                    var trainerInfo = Mapper.Map<Training>(trainerVM);

                    var res = trainingService.UpdateTrainerDetails(trainerInfo);
                    if (res)
                        return Json(new
                        {
                            msgType = "success",
                            msg = "Successfully Updated",
                            htmlData = ConvertViewToString("_EditTrainerDetails", trainerVM)
                        }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { part = 2, htmlData = ConvertViewToString("_EditTrainerDetails", trainerVM) }, JsonRequestBehavior.AllowGet);
                }
            }
            return PartialView("_EditTrainerDetails", trainerVM);

        }


        public ActionResult _RemoveTrainerRow(int sno)
        {
            var modelData = (TrainerDtlsViewModel)TempData["TrainerList"];
            if (modelData != null)
            {
                var deletedRow = modelData.TrainerList.Where(x => x.sno == sno).FirstOrDefault();
                modelData.TrainerList.Remove(deletedRow);
                TempData["TrainerList"] = modelData;
                return PartialView("_TrainerList", modelData);
            }
            return Content("");
        }

        #endregion

        #region Training Participants
        public ActionResult _EditTrainingParticipants(int? trainingID)
        {
            log.Info($"TrainingController/_EditTrainingParticipants");
            try
            {
                //TrainingParticipantsViewModel trainingParticipantsVM = new TrainingParticipantsViewModel();

                TrainingViewModel trnVM = new TrainingViewModel();
                trnVM.department = ddlService.ddlDepartmentList();
                trnVM.trainingParticipants = trainingService.GetTrainingParticipantsList(trainingID);
                trnVM.trainingID = trainingID;
                //trnVM.userRights = userAccessRight;
                trnVM.TrainingStatus = trainingService.GetTrainingDtls(trainingID.Value)?.TrainingStatus ?? 0;

                var selectedCheckboxes = new List<CheckBox>();
                var getFieldsValue = trainingService.GetTrainingDocumentList(trnVM.trainingID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<TrainingDocumentRepository, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.TrainingDocumentID))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.DocumentName));
                });
                trnVM.CheckBoxListAttachment.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValue);
                trnVM.CheckBoxListAttachment.SelectedFields = selectedCheckboxes;


                return PartialView("_TrainingParticipateContainer", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult AddTrainingParticipants(int trainingID, int status)
        {
            log.Info($"TrainingController/AddTrainingParticipants");
            try
            {
                //TrainingParticipantsViewModel trainingParticipantsVM = new TrainingParticipantsViewModel();

                TrainingViewModel trnVM = new TrainingViewModel();

                trnVM.trainingParticipantsDetail = trainingService.GetTrainingParticipantsDetailsList(null, null, null);
                var participantList = trainingService.GetTrainingParticipantsList(trainingID).Select(x => x.EmployeeID).ToList();
                trnVM.trainingParticipantsDetail = trnVM.trainingParticipantsDetail.Where(p => !participantList.Any(x => x == p.EmployeeID)).ToList();
                trnVM.trainingID = trainingID;
                trnVM.TrainingStatus = status;
                return PartialView("_TrainingParticipateDetails", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _GetTrainingParticipateGridView(TrainingViewModel trainingParticipants)
        {
            log.Info($"TrainingController/_GetTrainingParticipateGridView");
            try
            {
                //TrainingParticipantsViewModel trainingParticipantsVM = new TrainingParticipantsViewModel();
                TrainingViewModel trnVM = new TrainingViewModel();

                if (trainingParticipants.EmployeeCode != null)
                    trainingParticipants.EmployeeCode = trainingParticipants.EmployeeCode.Trim() == "" ? null : trainingParticipants.EmployeeCode.Trim();
                if (trainingParticipants.EmployeeName != null)
                    trainingParticipants.EmployeeName = trainingParticipants.EmployeeName.Trim() == "" ? null : trainingParticipants.EmployeeName.Trim();
                trnVM.trainingParticipantsDetail = trainingService.GetTrainingParticipantsDetailsList(trainingParticipants.EmployeeCode, trainingParticipants.EmployeeName, trainingParticipants.DepartmentID);
                var participantList = trainingService.GetTrainingParticipantsList(trainingParticipants.trainingID).Select(x => x.EmployeeID).ToList();
                trnVM.trainingParticipantsDetail = trnVM.trainingParticipantsDetail.Where(p => !participantList.Any(x => x == p.EmployeeID)).ToList();
                return PartialView("_TrainingParticipateDetails", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult _PostTrainingParticipantsdetails(FormCollection collection, TrainingViewModel trainingParticipants)
        {
            log.Info($"TrainingController/_PostTrainingParticipantsdetails");
            try
            {
                var ParticipantList = collection.Get("ParticipantID").ToString();
                var trainingParticipantLst = ParticipantList.Split(',');

                //TrainingParticipantsViewModel trainingParticipantsVM = new TrainingParticipantsViewModel();
                TrainingViewModel trnVM = new TrainingViewModel();
                trnVM.TrainingStatus = Convert.ToInt32(collection.Get("TrainingStatus"));
                int? trainingID = Convert.ToInt32(collection.Get("trainingID"));

                List<TrainingParticipant> participantsData = new List<TrainingParticipant>();
                //trainingParticipants.trainingParticipantsDetail = trainingParticipants.trainingParticipantsDetail.Where(x => x.IsChecked == true).ToList();
                //  if (trainingParticipants.trainingParticipantsDetail.Count > 0)
                if (trainingParticipantLst.Count() > 0)
                {

                    // for (int i = 0; i < trainingParticipants.trainingParticipantsDetail.Count; i++)
                    for (int i = 0; i < trainingParticipantLst.Count(); i++)
                    {
                        TrainingParticipant insertTrainingParticipants = new TrainingParticipant();
                        // insertTrainingParticipants.EmployeeID = trainingParticipants.trainingParticipantsDetail[i].EmployeeID;
                        insertTrainingParticipants.EmployeeID = Convert.ToInt32(trainingParticipantLst[i].ToString());
                        insertTrainingParticipants.TrainingID = trainingID.Value;
                        insertTrainingParticipants.CreatedBy = userDetail.UserID;
                        insertTrainingParticipants.CreatedOn = DateTime.Now;
                        participantsData.Add(insertTrainingParticipants);
                    }
                    var flag = trainingService.PostTrainingParticipants(participantsData);

                    trnVM.trainingParticipants = trainingService.GetTrainingParticipantsList(trainingID);
                    return Json(new { saved = true, msgType = "success", msg = "Particpant added successfully.", htmlData = ConvertViewToString("_TrainingParticipate", trnVM) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    trnVM.trainingParticipantsDetail = trainingService.GetTrainingParticipantsDetailsList(null, null, null);
                    var participantList = trainingService.GetTrainingParticipantsList(trainingID.Value).Select(x => x.EmployeeID).ToList();
                    trnVM.trainingParticipantsDetail = trnVM.trainingParticipantsDetail.Where(p => !participantList.Any(x => x == p.EmployeeID)).ToList();
                    trnVM.trainingID = trainingID;
                    return Json(new { saved = true, part = 4, htmlData = ConvertViewToString("_TrainingParticipateDetails", trnVM) }, JsonRequestBehavior.AllowGet);
                    //return PartialView("_TrainingParticipateDetails", trnVM);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult DeleteParticipants(int trainingParticipantID, int trainingId)
        {
            log.Info($"TrainingController/DeleteParticipants");
            try
            {
                TrainingViewModel trnVM = new TrainingViewModel();
                var isDeleted = trainingService.InsertDeletedTrainingParticipants(userDetail.UserID, trainingParticipantID);
                if (isDeleted)
                    trainingService.DeleteParticipant(trainingParticipantID);
                trnVM.trainingParticipants = trainingService.GetTrainingParticipantsList(trainingId);
                return PartialView("_TrainingParticipate", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            //return RedirectToAction("_EditTrainingParticipants", new { trainingID = 2 });
        }

        [HttpPost]
        public ActionResult _TrainingAttendedParticipants(TrainingViewModel tvModel, string ButtonType)
        {
            try
            {
                if (ButtonType == "Step 2: Send Intimation to Participant")
                {
                    var skillDetail = trainingService.GetTrainingDetail((int)tvModel.trainingID);

                    var trainingAttendanceDtls = tvModel.trainingParticipants;
                    TrainingSchedule trnSchedule = new TrainingSchedule();
                    trnSchedule._Training = new Training();
                    trnSchedule._Training = skillDetail._Training;
                    trnSchedule.TrainingParticipntList = trainingAttendanceDtls;
                    if (tvModel.CheckBoxListAttachment.PostedFields != null && tvModel.CheckBoxListAttachment.PostedFields.fieldIds.Count() > 0)
                    {
                        trnSchedule.AddAttachment = true;
                        trnSchedule.Attachments = tvModel.CheckBoxListAttachment.PostedFields.fieldIds;
                    }
                    if (skillDetail._Training.OrientationTraining)
                        skillDetail._Training.enumResidentialNonResidential = EnumResidentialNonResidential.Residential;
                    else if (skillDetail._Training.OnBoardTraining)
                        skillDetail._Training.enumResidentialNonResidential = EnumResidentialNonResidential.NonResidential;

                    if (skillDetail._Training.TrainingType == (int)EnumTrainingType.Behavioral)
                        skillDetail._Training.TrainingTypeName = EnumTrainingType.Behavioral.GetDisplayName();
                    else if (skillDetail._Training.TrainingType == (int)EnumTrainingType.Functional)
                        skillDetail._Training.TrainingTypeName = EnumTrainingType.Functional.GetDisplayName();

                    List<string> topicList = new List<string>();
                    List<string> functionalList = new List<string>();

                    if (trnSchedule._Training.TrainingType == 2)
                    {
                        var behavioralTopic = skillDetail.TrainingTopicList.Where(x => x.SkillTypeID == 2).ToList();
                        // trnSchedule.CheckBoxListBehavioral = tvModel.CheckBoxListBehavioral.PostedFields.fieldIds;
                        var getFieldsValueBehavioral = ddlService.ddlSkill(2);
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<TrainingTopic, string>()
                            .ConvertUsing(s => getFieldsValueBehavioral.Where(x => x.id == s.SkillID).FirstOrDefault().value);

                        });
                        topicList = Mapper.Map<List<string>>(behavioralTopic);
                    }
                    else if (trnSchedule._Training.TrainingType == 3)
                    {
                        var functionalTopic = skillDetail.TrainingTopicList.Where(x => x.SkillTypeID == 3).ToList();
                        // trnSchedule.CheckBoxListFunctional = tvModel.CheckBoxListFunctional.PostedFields.fieldIds;
                        var getFieldsValueFunctional = ddlService.ddlSkill(3);
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<TrainingTopic, string>()
                            .ConvertUsing(s => getFieldsValueFunctional.Where(x => x.id == s.SkillID).FirstOrDefault().value);

                        });
                        topicList = Mapper.Map<List<string>>(functionalTopic);
                    }

                    trnSchedule.topicList = topicList;

                    if (trnSchedule._Training.TimeSlotType == (byte)EnumTimeSlotType.Distributed)
                        trnSchedule._Training.distributedTimeSlots = trainingService.GetTrainingTimeSlots(trnSchedule._Training.TrainingID);


                    bool result = trainingService.SendTrainingRelatedEmail(trnSchedule, "Intimation");
                    // bool result = true;
                    if (result)
                        return Json(new
                        {
                            Tab4Part = 1,
                            msgType = "success",
                            msg = "Intimation sent successfully to Participant",
                            htmlData = ConvertViewToString("_TrainingParticipate", tvModel)
                        }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (!trainingService.FeedbackFormGenerated((int)tvModel.trainingID))
                    {
                        return Json(new
                        {
                            Tab4Part = 2,
                            msgType = "error",
                            msg = "You can't mark attendance right now because, training feedback form not generated, please generate feedback form.",
                            htmlData = ConvertViewToString("_TrainingParticipate", tvModel)
                        }, JsonRequestBehavior.AllowGet);
                    }

                    var trainingAttendanceDtls = tvModel.trainingParticipants;
                    trainingAttendanceDtls.ForEach(x => { x.UpdateOn = DateTime.Now; x.UpdateBy = userDetail.UserID; });
                    TrainingSchedule trnSchedule = new TrainingSchedule();
                    trnSchedule.TrainingParticipntList = trainingAttendanceDtls;
                    bool result = trainingService.AddTrainingParticipantAttendance(trnSchedule);
                    if (result)
                        return Json(new
                        {
                            Tab4Part = 1,
                            msgType = "success",
                            msg = "Successfully Updated",
                            htmlData = ConvertViewToString("_TrainingParticipate", tvModel)
                        }, JsonRequestBehavior.AllowGet);
                }
                return PartialView("_TrainingParticipate", tvModel);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion Training Participants

        #region Training Document Repository
        public ActionResult _EditTrainingDocuments(int? trainingID)
        {
            log.Info($"TrainingController/_EditTrainingDocuments");
            try
            {
                TrainingViewModel trnVM = new TrainingViewModel();
                trnVM.trainingDocument = trainingService.GetTrainingDocumentList(trainingID);
                trnVM.TrainingStatus = trainingService.GetTrainingDtls(trainingID.Value)?.TrainingStatus ?? 0;
                trnVM.documentRepository = new TrainingDocumentRepository();
                trnVM.trainingID = trainingID;
                return PartialView("_TrainingDocumentContainer", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            TrainingDocumentRepository trainingDocument = new TrainingDocumentRepository();
            List<TrainingDocumentRepository> trainingDocumentList = new List<TrainingDocumentRepository>();
            TrainingViewModel trnVM = new TrainingViewModel();
            if (Request.Files.Count > 0)
            {
                string fileName = string.Empty; string filePath = string.Empty;
                string fname = "";
                string documentName = Request["DocumentName"];
                string documentDetails = Request["DocumentDetails"];
                int trainingID = Convert.ToInt32(Request["TrainingID"]);
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;

                    #region Check Mime Type
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        var contentType = GetFileContentType(file.InputStream);

                        if (!IsValidFileName(file.FileName))
                        {
                            stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                            stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                            stringBuilder.Append($"I. a to z characters.");
                            stringBuilder.Append($"II. numbers(0 to 9).");
                            stringBuilder.Append($"III. - and _ with space.");
                        }

                        var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                        if (dicValue != contentType)
                        {
                            stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                            stringBuilder.Append("<br>");
                        }
                    }
                    if (stringBuilder.ToString() != "")
                    {
                        return Json(new { part = 0, htmlData = stringBuilder.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    #endregion
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];

                        fname = ExtensionMethods.SetUniqueFileName("TrainingDocument-",
                     Path.GetExtension(file.FileName));
                        fileName = fname;

                        //filePath = "~/" + DocumentUploadFilePath.CandidatePhoto;

                        string fullPath = Request.MapPath("~/Document/TrainingDocument/" + fileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }


                        fname = Path.Combine(Server.MapPath("~/Document/TrainingDocument/"), fname);
                        file.SaveAs(fname);

                    }

                    #region Save Training Document

                    trainingDocument.DocumentName = documentName;
                    trainingDocument.DocumentDetail = documentDetails;
                    trainingDocument.DocumentPathName = fileName;
                    trainingDocument.TrainingID = trainingID;
                    trainingDocument.CreatedBy = userDetail.UserID;
                    trainingDocument.CreatedOn = DateTime.Now;
                    var flag = trainingService.SaveTrainingDocument(trainingDocument);
                    if (flag)
                    {
                        trainingDocumentList = trainingService.GetTrainingDocumentList(trainingID);
                    }
                    #endregion
                    trnVM.trainingDocument = trainingDocumentList;
                    // Returns message that successfully uploaded  
                    return Json(new { part = 1, htmlData = ConvertViewToString("_TrainingDocumentList", trnVM) }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                    throw ex;
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }


        public ActionResult DeleteTrainingDocument(int trainingDocumentID, string fileName, int? trainingID)
        {
            log.Info($"TrainingController/DeleteTrainingDocument");
            TrainingViewModel trnVM = new TrainingViewModel();
            string fullPath = Request.MapPath("~/Document/TrainingDocument/" + fileName);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            var deleted = trainingService.DeleteTrainingDocument(trainingDocumentID);
            trnVM.trainingDocument = trainingService.GetTrainingDocumentList(trainingID);
            return PartialView("_TrainingDocumentList", trnVM);
        }

        #endregion Training Document Repository

        #region TrainingPrerequisite

        public ActionResult _EditTrainingPreqTab(int trainingID)
        {
            log.Info($"TrainingController/_EditTrainingPreqTab/{trainingID}");
            try
            {
                var trainingPrerequisiteDetail = trainingService.GetTrainingPrerequisiteByID(trainingID);
                TempData["TrainingID"] = trainingID;
                TempData.Keep("TrainingID");
                ViewBag.ActiveTab = 1;
                ViewBag.TrainingID = trainingID;
                TrainingViewModel trnVM = new TrainingViewModel();
                trnVM.TrainingPrereqList = trainingPrerequisiteDetail.TrainingPrereqList;
                trnVM._Training = trainingPrerequisiteDetail._Training;
                trnVM.TrainingStatus = trainingService.GetTrainingDtls(trainingID)?.TrainingStatus ?? 0;

                return PartialView("CreateTrainingPrerequisiteDetails", trnVM);
            }

            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _RemoveTargetRow(int sNo)
        {
            var modelData = (TrainingViewModel)TempData["TrainingPrerequisite"];
            if (modelData != null)
            {
                var deletedRow = modelData.TrainingPrereqList.Where(x => x.sno == sNo).FirstOrDefault();
                modelData.TrainingPrereqList.Remove(deletedRow);
                TempData["TrainingPrerequisite"] = modelData;
                return PartialView("CreateTrainingPrerequisiteDetails", modelData);
            }
            return Content("");

        }

        [HttpPost]
        public ActionResult CreateTrainingPrerequisiteDetails(TrainingViewModel createTrainingPrerequisite, string ButtonType)
        {
            log.Info("TrainingPrerequisiteController/CreateTrainingPrerequisiteDetails");
            try
            {
                if (ButtonType == "Add Training Prerequisite")
                {

                    if (createTrainingPrerequisite.TrainingPrereqList == null)
                        createTrainingPrerequisite.TrainingPrereqList = new List<TrainingPrerequisite>() {
                           new TrainingPrerequisite() { sno = 1, } };
                    else
                    {
                        if (createTrainingPrerequisite.TrainingPrereqList.Count == 1)
                            createTrainingPrerequisite.TrainingPrereqList.FirstOrDefault().sno = 1;
                        else
                        {
                            var s_no = 1;
                            createTrainingPrerequisite.TrainingPrereqList.ForEach(x =>
                            {
                                x.sno = s_no++;
                            });

                        }
                        if (createTrainingPrerequisite.TrainingPrereqList.Count < 5)
                            createTrainingPrerequisite.TrainingPrereqList.Add(new TrainingPrerequisite()
                            {
                                sno = createTrainingPrerequisite.TrainingPrereqList.Count + 1,
                                TrainingID = createTrainingPrerequisite._Training.TrainingID
                            });
                    }
                    TempData["TrainingPrerequisite"] = createTrainingPrerequisite;
                    return Json(new { status = "1", htmlData = ConvertViewToString("CreateTrainingPrerequisiteDetails", createTrainingPrerequisite) }, JsonRequestBehavior.AllowGet);
                }
                else if (ButtonType == "Update")
                {
                    ModelState.Remove("_Training.TrainingObjective");
                    ModelState.Remove("_Training.enumResidentialNonResidential");
                    ModelState.Remove("_Training.TrainingType");
                    ModelState.Remove("_Training.StartDate");
                    ModelState.Remove("_Training.EndDate");

                    if (ModelState.IsValid)
                    {
                        TrainingSchedule trnSchedule = new TrainingSchedule();
                        trnSchedule.TrainingPrereqList = createTrainingPrerequisite.TrainingPrereqList;
                        trnSchedule._Training = new Training();
                        trnSchedule._Training.TrainingID = createTrainingPrerequisite._Training.TrainingID;


                        var result = trainingService.InsertTrainingPrerequisite(trnSchedule);
                        if (result)
                        {
                            return Json(new { status = "2", trainingID = result, activeTab = 2, msg = "Successfully updated" }, JsonRequestBehavior.AllowGet);
                        }


                    }
                    return PartialView("CreateTrainingPrerequisiteDetails", createTrainingPrerequisite);
                }
                return View("");
            }

            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }


        #endregion

        #region Employee Feedback Form
        public ActionResult _EditEmployeeFeedbackForm(int trainingID)
        {
            TrainingFeedBackFormViewModel vm = new Models.TrainingFeedBackFormViewModel();
            vm.TrainingID = trainingID;
            var getFeedbackDtl = trainingService.GetTrainingFeedbackFormDtls(trainingID);
            vm.TrainingFeedBackFormList = getFeedbackDtl.TrainingFbFormDtlList.Where(x => x.PartNo == 1).ToList();
            vm.TrainingFeedBackFormPart2List = getFeedbackDtl.TrainingFbFormDtlList.Where(x => x.PartNo == 2).ToList();
            vm.TrainingFeedBackFormPart3List = getFeedbackDtl.TrainingFbFormDtlList.Where(x => x.PartNo == 3).ToList();
            //  vm.TrainingFeedBackFormList = getFeedbackDtl.TrainingFbFormDtlList;
            var s_no = 1;
            vm.TrainingFeedBackFormList.ForEach(x =>
            {
                x.sno = s_no++;
            });
            s_no = 1;
            vm.TrainingFeedBackFormPart2List.ForEach(x =>
            {
                x.sno = s_no++;
            });
            s_no = 1;
            vm.TrainingFeedBackFormPart3List.ForEach(x =>
            {
                x.sno = s_no++;
            });
            vm.FeedBackFormHdrID = getFeedbackDtl.FeedBackFormHdrID;
            vm.RatingType = getFeedbackDtl.RatingType;
            vm.UpperRatingValue = getFeedbackDtl.UpperRatingValue;
            vm.LowerRatingValue = getFeedbackDtl.LowerRatingValue;
            //   vm.ActionPlan = getFeedbackDtl.ActionPlan;
            if (getFeedbackDtl.RatingType == "N")
                vm.enumRatingType = EnumRatingType.Number;
            else if (getFeedbackDtl.RatingType == "G")
                vm.enumRatingType = EnumRatingType.Grade;

            return PartialView("_EditEmployeeFeedbackForm", vm);
        }

        [HttpPost]
        public ActionResult _EditEmployeeFeedbackForm(TrainingFeedBackFormViewModel trainingFbVM, string ButtonType)
        {
            if (ButtonType == "Part1")
            {

                if (trainingFbVM.TrainingFeedBackFormList == null)
                    trainingFbVM.TrainingFeedBackFormList = new List<TrainingFeedBackFormDetail>() {
                           new TrainingFeedBackFormDetail() { sno = 1, PartNo=1} };
                else
                {
                    if (trainingFbVM.TrainingFeedBackFormList.Count == 1)
                    {
                        trainingFbVM.TrainingFeedBackFormList.FirstOrDefault().sno = 1;
                        trainingFbVM.TrainingFeedBackFormList.FirstOrDefault().PartNo = 1;
                    }
                    else
                    {
                        var s_no = 1;
                        trainingFbVM.TrainingFeedBackFormList.ForEach(x =>
                        {
                            x.sno = s_no++;
                            x.PartNo = 1;
                        });

                    }
                    if (trainingFbVM.TrainingFeedBackFormList.Count < 30)
                        trainingFbVM.TrainingFeedBackFormList.Add(new TrainingFeedBackFormDetail()
                        {
                            sno = trainingFbVM.TrainingFeedBackFormList.Count + 1,
                            PartNo = 1
                        });
                }
                TempData["EmployeeFeedbackFormList"] = trainingFbVM;
                return Json(new { part = 1, htmlData = ConvertViewToString("_FeedbackFormPart1List", trainingFbVM) }, JsonRequestBehavior.AllowGet);
            }
            if (ButtonType == "Part2")
            {

                if (trainingFbVM.TrainingFeedBackFormPart2List == null)
                    trainingFbVM.TrainingFeedBackFormPart2List = new List<TrainingFeedBackFormDetail>() {
                           new TrainingFeedBackFormDetail() { sno = 1,  PartNo=2 } };
                else
                {
                    if (trainingFbVM.TrainingFeedBackFormPart2List.Count == 1)
                    {
                        trainingFbVM.TrainingFeedBackFormPart2List.FirstOrDefault().sno = 1;
                        trainingFbVM.TrainingFeedBackFormPart2List.FirstOrDefault().PartNo = 2;
                    }
                    else
                    {
                        var s_no = 1;
                        trainingFbVM.TrainingFeedBackFormPart2List.ForEach(x =>
                        {
                            x.sno = s_no++;
                            x.PartNo = 2;
                        });

                    }
                    if (trainingFbVM.TrainingFeedBackFormPart2List.Count < 30)
                        trainingFbVM.TrainingFeedBackFormPart2List.Add(new TrainingFeedBackFormDetail()
                        {
                            sno = trainingFbVM.TrainingFeedBackFormPart2List.Count + 1,
                            PartNo = 2
                        });
                }
                TempData["EmployeeFeedbackFormPart2List"] = trainingFbVM;
                return Json(new { part = 2, htmlData = ConvertViewToString("_FeedbackFormPart2List", trainingFbVM) }, JsonRequestBehavior.AllowGet);
            }
            if (ButtonType == "Part3")
            {

                if (trainingFbVM.TrainingFeedBackFormPart3List == null)
                    trainingFbVM.TrainingFeedBackFormPart3List = new List<TrainingFeedBackFormDetail>() {
                           new TrainingFeedBackFormDetail() { sno = 1,  PartNo = 3} };
                else
                {
                    if (trainingFbVM.TrainingFeedBackFormPart3List.Count == 1)
                    {
                        trainingFbVM.TrainingFeedBackFormPart3List.FirstOrDefault().sno = 1;
                        trainingFbVM.TrainingFeedBackFormPart3List.FirstOrDefault().PartNo = 3;
                    }
                    else
                    {
                        var s_no = 1;
                        trainingFbVM.TrainingFeedBackFormPart3List.ForEach(x =>
                        {
                            x.sno = s_no++;
                            x.PartNo = 3;
                        });

                    }
                    if (trainingFbVM.TrainingFeedBackFormPart3List.Count < 30)
                        trainingFbVM.TrainingFeedBackFormPart3List.Add(new TrainingFeedBackFormDetail()
                        {
                            sno = trainingFbVM.TrainingFeedBackFormPart3List.Count + 1,
                            PartNo = 3
                        });
                }
                TempData["EmployeeFeedbackFormPart3List"] = trainingFbVM;
                return Json(new { part = 3, htmlData = ConvertViewToString("_FeedbackFormPart3List", trainingFbVM) }, JsonRequestBehavior.AllowGet);
            }
            else if (ButtonType == "Update" || ButtonType == "Generate Feedback Form")
            {
                trainingFbVM.RatingType = "N"; // add later to enter rating type as 1 to 5
                if (trainingFbVM.enumRatingType == EnumRatingType.Grade)
                {
                    trainingFbVM.RatingType = "G";
                    ModelState.Remove("LowerRatingValue");
                    ModelState.Remove("UpperRatingValue");
                }
                else if (trainingFbVM.enumRatingType == EnumRatingType.Number)
                {
                    trainingFbVM.RatingType = "N";
                    trainingFbVM.LowerRatingValue = 1;
                }
                if (trainingFbVM.TrainingFeedBackFormList == null || trainingFbVM.TrainingFeedBackFormList.Count == 0)
                {
                    ModelState.AddModelError("QuestionValidator", "Please add Question(s) for Feedback Form.");
                    return Json(new { fff = 2, htmlData = ConvertViewToString("_EditEmployeeFeedbackForm", trainingFbVM) }, JsonRequestBehavior.AllowGet);
                }
                for (int i = 0; i < trainingFbVM.TrainingFeedBackFormList.Count; i++)
                {
                    ModelState.Remove("TrainingFeedBackFormList[" + i + "].enumRatingGrade");
                    ModelState.Remove("TrainingFeedBackFormList[" + i + "].RatingNo");
                }

                var ff = ModelState.Where(x => x.Value.Errors.Count > 1).ToList();
                if (ModelState.IsValid)
                {
                    TrainingSchedule trainingsch = new TrainingSchedule();
                    trainingsch.TrainingFbFormDtlList = trainingFbVM.TrainingFeedBackFormList;
                    if (trainingFbVM.TrainingFeedBackFormPart2List != null && trainingFbVM.TrainingFeedBackFormPart2List.Count > 0)
                    {
                        foreach (var item in trainingFbVM.TrainingFeedBackFormPart2List)
                        {
                            trainingsch.TrainingFbFormDtlList.Add(item);
                        }
                    }
                    if (trainingFbVM.TrainingFeedBackFormPart3List != null && trainingFbVM.TrainingFeedBackFormPart3List.Count > 0)
                    {
                        foreach (var item in trainingFbVM.TrainingFeedBackFormPart3List)
                        {
                            trainingsch.TrainingFbFormDtlList.Add(item);
                        }
                    }
                    trainingsch.TrainingID = trainingFbVM.TrainingID;
                    trainingsch.CreatedBy = userDetail.UserID;
                    trainingsch.CreatedOn = DateTime.Now;
                    trainingsch.RatingType = trainingFbVM.RatingType;
                    trainingsch.LowerRatingValue = trainingFbVM.LowerRatingValue;
                    trainingsch.UpperRatingValue = trainingFbVM.UpperRatingValue;
                    trainingsch.FeedBackFormHdrID = trainingFbVM.FeedBackFormHdrID;

                    var res = trainingService.UpdateTrainingFeedbackForm(trainingsch);
                    if (res > 0)
                        return Json(new
                        {

                            msgType = "success",
                            msg = "Successfully Updated",
                            htmlData = ConvertViewToString("_EditEmployeeFeedbackForm", trainingFbVM)
                        }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { fff = 2, htmlData = ConvertViewToString("_EditEmployeeFeedbackForm", trainingFbVM) }, JsonRequestBehavior.AllowGet);
                }
            }
            return PartialView("_EditEmployeeFeedbackForm", trainingFbVM);
        }

        public ActionResult _RemoveFeedbackRowPart1(int sno)
        {
            var modelData = (TrainingFeedBackFormViewModel)TempData["EmployeeFeedbackFormList"];
            if (modelData != null)
            {
                var deletedRow = modelData.TrainingFeedBackFormList.Where(x => x.sno == sno).FirstOrDefault();
                modelData.TrainingFeedBackFormList.Remove(deletedRow);
                TempData["EmployeeFeedbackFormList"] = modelData;
                return PartialView("_FeedbackFormPart1List", modelData);
            }
            return Content("");
        }
        public ActionResult _RemoveFeedbackRowPart2(int sno)
        {
            var modelData = (TrainingFeedBackFormViewModel)TempData["EmployeeFeedbackFormPart2List"];
            if (modelData != null)
            {
                var deletedRow = modelData.TrainingFeedBackFormPart2List.Where(x => x.sno == sno).FirstOrDefault();
                modelData.TrainingFeedBackFormPart2List.Remove(deletedRow);
                TempData["EmployeeFeedbackFormList"] = modelData;
                return PartialView("_FeedbackFormPart2List", modelData);
            }
            return Content("");
        }
        public ActionResult _RemoveFeedbackRowPart3(int sno)
        {
            var modelData = (TrainingFeedBackFormViewModel)TempData["EmployeeFeedbackFormPart3List"];
            if (modelData != null)
            {
                var deletedRow = modelData.TrainingFeedBackFormPart3List.Where(x => x.sno == sno).FirstOrDefault();
                modelData.TrainingFeedBackFormPart3List.Remove(deletedRow);
                TempData["EmployeeFeedbackFormList"] = modelData;
                return PartialView("_FeedbackFormPart3List", modelData);
            }
            return Content("");
        }
        public ActionResult _GetTrainingListPopup(int trainingID)
        {
            log.Info("TrainingController/_GetTrainingListPopup");
            try
            {
                TrainingViewModel trnVM = new TrainingViewModel();
                var getTrainingList = trainingService.GetTrainingPopupList();
                trnVM.TrainingList = getTrainingList;
                trnVM.trainingID = trainingID;
                return PartialView("_TrainingGridPopup", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _GetFeedbackFormQuestions(int trainingID)
        {
            TrainingFeedBackFormViewModel vm = new Models.TrainingFeedBackFormViewModel();
            vm.TrainingID = trainingID;
            var getFeedbackDtl = trainingService.GetTrainingFeedbackFormDtls(trainingID);
            vm.TrainingFeedBackFormList = getFeedbackDtl.TrainingFbFormDtlList;
            vm.FeedBackFormHdrID = getFeedbackDtl.FeedBackFormHdrID;
            vm.RatingType = getFeedbackDtl.RatingType;
            vm.UpperRatingValue = getFeedbackDtl.UpperRatingValue;
            vm.LowerRatingValue = getFeedbackDtl.LowerRatingValue;
            //   vm.ActionPlan = getFeedbackDtl.ActionPlan;
            if (getFeedbackDtl.RatingType == "N")
                vm.enumRatingType = EnumRatingType.Number;
            else if (getFeedbackDtl.RatingType == "G")
                vm.enumRatingType = EnumRatingType.Grade;

            return PartialView("_FeedbackQuestions", vm);
        }

        public ActionResult QuesFromPrvtoNewTraining(int trainingID, int prevTrainingID)
        {
            int res = trainingService.CopyFeedbackQuesfromTraining(prevTrainingID, trainingID, (int)userDetail.UserID);
            if (res > 0)
            {
                TrainingFeedBackFormViewModel trainingFbVM = new Models.TrainingFeedBackFormViewModel();
                trainingFbVM.TrainingID = trainingID;
                var getFeedbackDtl = trainingService.GetTrainingFeedbackFormDtls(trainingID);
                trainingFbVM.TrainingFeedBackFormList = getFeedbackDtl.TrainingFbFormDtlList;
                trainingFbVM.FeedBackFormHdrID = getFeedbackDtl.FeedBackFormHdrID;
                trainingFbVM.RatingType = getFeedbackDtl.RatingType;
                trainingFbVM.UpperRatingValue = getFeedbackDtl.UpperRatingValue;
                trainingFbVM.LowerRatingValue = getFeedbackDtl.LowerRatingValue;
                if (getFeedbackDtl.RatingType == "N")
                    trainingFbVM.enumRatingType = EnumRatingType.Number;
                else if (getFeedbackDtl.RatingType == "G")
                    trainingFbVM.enumRatingType = EnumRatingType.Grade;

                TempData["EmployeeFeedbackFormList"] = trainingFbVM;

                return Json(
                    new
                    {
                        status = 1,
                        msg = "Feedback Questions Successfully copied.",
                        msgType = "success",
                        htmlData = ConvertViewToString("_EditEmployeeFeedbackForm", trainingFbVM)
                    }
                    , JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(
                   new
                   {
                       status = 0,
                       msg = "problem whilw coping Feedback Questions.",
                       msgType = "error"
                   }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Training Status 
        public ActionResult _EditTrainingStatus(int trainingID)
        {
            log.Info($"TrainingController/_EditTrainingStatus/{trainingID}");
            try
            {
                TrainingViewModel tvm = new TrainingViewModel();
                tvm._Training = new Training();
                var trainingData = trainingService.GetTrainingDtls(trainingID);
                trainingData.enumTrainingStatus = trainingData.TrainingStatus.HasValue ? (EnumTrainingStatus)trainingData.TrainingStatus : EnumTrainingStatus.Schedule;
                BindDropDown();
                List<SelectListModel> CancelReason = new List<SelectListModel>()
                {
                    new SelectListModel {id=1,value="Unavailability of Trainer" },
                    new SelectListModel { id=2,value="Less Participants"},
                     new SelectListModel { id=3,value="Other"}
                };

                ViewBag.CancelReason = new SelectList(CancelReason, "id", "value");

                if (trainingData.TimeSlotType == (byte)EnumTimeSlotType.Distributed)
                {
                    trainingData.distributedTimeSlots = trainingService.GetTrainingTimeSlots(trainingID);

                    //========== new line of codes ========= Dated - 20-Oct-2020

                    var tDates = trainingData.distributedTimeSlots
                        .Select(y => y.TrainingDate).OrderBy(y => y).Distinct().ToList();

                    var sno = 1;
                    tDates.ForEach(x =>
                    {
                        trainingData.TrainingDates.Add(
                            new SelectListModel()
                            {
                                id = sno++,
                                value = x.ToString("dd/MM/yyyy")
                            });
                    });

                    //=========== end =======================
                    trainingData.enumTimeSlotType = EnumTimeSlotType.Distributed;
                }

                tvm._Training = trainingData;
                tvm._Training.CancelReason = tvm._Training.CancelReasonID != 3 ? string.Empty : tvm._Training.CancelReason;
                return PartialView("_EditTrainingStatus", tvm);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _EditTrainingStatus(TrainingViewModel training)
        {
            try
            {
                log.Info($"Training Status Change Details: 1. TimeStamp-" + DateTime.Now + " 2. User Id- " + userDetail.UserName + " 3. User Name- " + userDetail.FullName + " 4. Training Id- " + training._Training.TrainingID + "Training Name- " + training._Training.TrainingTitle + " 6. Status- " + training._Training.enumTrainingStatus);

                bool isCancelError = false;
                BindDropDown();
                if (training._Training.enumTrainingStatus != 0)
                {
                    ModelState.Remove("_Training.TrainingObjective");
                    ModelState.Remove("_Training.enumResidentialNonResidential");
                    ModelState.Remove("_Training.enumTrainingList");

                    if (string.IsNullOrEmpty(training._Training.Address))
                        ModelState.AddModelError("AddressValidator", "Please enter Address");
                    if (string.IsNullOrEmpty(training._Training.City))
                        ModelState.AddModelError("CityValidator", "Please enter City");
                    //if (string.IsNullOrEmpty(training.PinCode))
                    //    ModelState.AddModelError("PinCodeValidator", "Please enter PinCode");
                    if (string.IsNullOrEmpty(training._Training.TrainingTitle))
                        ModelState.AddModelError("TrainingTitleValidator", "Please enter Training Title");

                    if (training._Training.StartDate.HasValue && training._Training.EndDate.HasValue)
                    {
                        if (training._Training.StartDate.Value > training._Training.EndDate.Value)
                        {
                            ModelState.AddModelError("StartDateValidator", "Start Date can't be greater then End Date.");
                        }
                    }
                    if (training._Training.CancelReasonID == 3 && string.IsNullOrEmpty(training._Training.CancelReason))
                    {
                        ModelState.AddModelError("CancelValidator", "Please provide other cancellation remark.");
                        isCancelError = true;
                    }

                    if (ModelState.IsValid)
                    {
                        if (training._Training.enumTimeSlotType == EnumTimeSlotType.Distributed)
                            training._Training.DistributedTimeSlot = true;
                        else if (training._Training.enumTimeSlotType == EnumTimeSlotType.Standard)
                            training._Training.StandardTimeSlot = true;

                        TrainingSchedule tSchedule = new TrainingSchedule();
                        tSchedule._Training = new Training();
                        tSchedule._Training = training._Training;
                        tSchedule._Training.CancelReason = training._Training.CancelReasonID == 1 ? "Unavailability of Trainer" : training._Training.CancelReasonID == 2 ? "Less Participants" : training._Training.CancelReason;
                        tSchedule.TrainingParticipntList = trainingService.GetTrainingParticipantsList(training._Training.TrainingID);
                        var stateName = ddlService.GetStateName((int)tSchedule._Training.StateID);
                        tSchedule._Training.StateName = stateName;
                        tSchedule._Training.TimeSlotType = (byte)training._Training.enumTimeSlotType;
                        tSchedule._Training.CreatedBy = userDetail.UserID;
                        tSchedule._Training.CreatedOn = DateTime.Now;
                        var flag = trainingService.UpdateTrainingStatus(tSchedule);
                        if (flag)
                        {
                            List<SelectListModel> CancelReason = new List<SelectListModel>()
                            {
                              new SelectListModel {id=1,value="Unavailability of Trainer" },
                              new SelectListModel { id=2,value="Less Participants"},
                             new SelectListModel { id=3,value="Other"}
                            };

                            ViewBag.CancelReason = new SelectList(CancelReason, "id", "value");

                            return Json(new
                            {
                                ff = 2,
                                Tab6Part = training._Training.enumTrainingStatus == EnumTrainingStatus.Reschedule ? 2 : 1,
                                msgType = "success",
                                msg = "Training status Updated Successfully.",
                                trainingID = training._Training.TrainingID,
                                htmlData = ConvertViewToString("_EditTrainingStatus", training)
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        List<SelectListModel> CancelReason = new List<SelectListModel>()
                            {
                              new SelectListModel {id=1,value="Unavailability of Trainer" },
                              new SelectListModel { id=2,value="Less Participants"},
                             new SelectListModel { id=3,value="Other"}
                            };
                        ViewBag.CancelReason = new SelectList(CancelReason, "id", "value");

                        training._Training.isCancelButtonShow = isCancelError ? true : false;
                        return Json(new
                        {
                            Tab6Part = isCancelError ? 0 : 3,
                            htmlData = ConvertViewToString("_EditTrainingStatus", training)
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    List<SelectListModel> CancelReason = new List<SelectListModel>()
                            {
                              new SelectListModel {id=1,value="Unavailability of Trainer" },
                              new SelectListModel { id=2,value="Less Participants"},
                             new SelectListModel { id=3,value="Other"}
                            };

                    ViewBag.CancelReason = new SelectList(CancelReason, "id", "value");

                    ModelState.AddModelError("TrainingStatusValidation", "Please select Training Status.");
                    // return Json(new { htmlData = ConvertViewToString("_EditTrainingStatus", training) }, JsonRequestBehavior.AllowGet);
                }
                return PartialView("_EditTrainingStatus", training);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _EditTrainingStatusForSchedule(int trainingID)
        {
            try
            {
                TrainingSchedule tSchedule = new TrainingSchedule();
                tSchedule._Training = new Training();
                tSchedule._Training.TrainingID = trainingID;
                tSchedule._Training.enumTrainingStatus = EnumTrainingStatus.Schedule;
                var flag = trainingService.UpdateTrainingStatusForSchedule(tSchedule);
                if (flag)
                {
                    return Json(new
                    {
                        msgType = "success",
                        msg = "Successfully Updated",
                        trainingID = trainingID
                    }, JsonRequestBehavior.AllowGet);
                }
                TrainingViewModel tvm = new TrainingViewModel();
                tvm._Training = new Training();
                tvm._Training = tSchedule._Training;
                return PartialView("_EditTrainingStatus", tvm);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region View Training

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult ScheduleIndex()
        {
            log.Info($"TrainingController/ScheduleIndex");
            EmployeeViewModel vm = new Models.EmployeeViewModel();
            return View("ScheduleIndex", vm);
        }

        public ActionResult AttendanceIndex()
        {
            log.Info($"TrainingController/AttendanceIndex");
            EmployeeViewModel vm = new Models.EmployeeViewModel();
            vm._viewmode = 1;
            return View("ScheduleIndex", vm);
        }
        public ActionResult _ViewTrainings()
        {
            log.Info("TrainingController/_ViewTrainings");
            try
            {
                TrainingViewModel trnVM = new TrainingViewModel();
                var getTrainingList = trainingService.GetTrainingList();
                trnVM.TrainingList = getTrainingList.Where(x => x.TrainingStatus != 4).ToList();
                trnVM.ViewMode = 2;
                return PartialView("_TrainingGridView", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _ViewAttendanceTrainings()
        {
            log.Info("TrainingController/_ViewAttendanceTrainings");
            try
            {
                TrainingViewModel trnVM = new TrainingViewModel();
                var getTrainingList = trainingService.GetTrainingList();
                trnVM.TrainingList = getTrainingList.Where(x => x.TrainingStatus == (int)EnumTrainingStatus.Completed).ToList();
                trnVM.ViewMode = 4;
                return PartialView("_TrainingGridView", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult TrainingSchedule(int trainingID)
        {
            log.Info($"TrainingController/TrainingSchedule/{trainingID}");
            try
            {
                var arpSkillDetail = trainingService.GetTrainingDetail(trainingID);

                BindDropDown();
                TrainingViewModel trnVM = new TrainingViewModel();
                trnVM._Training = arpSkillDetail._Training;
                if (trnVM._Training.TimeSlotType == 2)
                    trnVM._Training.distributedTimeSlots = trainingService.GetTrainingTimeSlots(trainingID);
                trnVM._Training.enumTrainingStatus = (EnumTrainingStatus)(arpSkillDetail._Training.TrainingStatus ?? 0);

                if (arpSkillDetail._Training.OrientationTraining)
                    trnVM._Training.enumResidentialNonResidential = EnumResidentialNonResidential.Residential;
                else if (arpSkillDetail._Training.OnBoardTraining)
                    trnVM._Training.enumResidentialNonResidential = EnumResidentialNonResidential.NonResidential;
                else if (arpSkillDetail._Training.InternalAddressType)
                    trnVM._Training.enumResidentialNonResidential = EnumResidentialNonResidential.internaltraining;

                trnVM.TrainingStatus = trainingService.GetTrainingDtls(trainingID)?.TrainingStatus ?? 0;
                trnVM._Training.TrainingTypeName = trnVM._Training.TrainingType == 2 ? "Behavioral" : "Functional/Technical";

                //List<SelectListModel> trainingType = new List<SelectListModel>();
                //trainingType.Add(new SelectListModel { id = 2, value = "Behavioral" });
                //trainingType.Add(new SelectListModel { id = 3, value = "Functional/Technical" });

                //trnVM.ddlTrainningType = trainingType;
                List<string> topicList = new List<string>();
                if (trnVM._Training.TrainingType == 2)
                {
                    var behavioralTopic = arpSkillDetail.TrainingTopicList.Where(x => x.SkillTypeID == 2).ToList();
                    var getFieldsValueBehavioral = ddlService.ddlSkill(2);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<TrainingTopic, string>()
                        .ConvertUsing(s => getFieldsValueBehavioral.Where(x => x.id == s.SkillID).FirstOrDefault().value);

                    });
                    topicList = Mapper.Map<List<string>>(behavioralTopic);
                }
                else if (trnVM._Training.TrainingType == 3)
                {
                    var functionalTopic = arpSkillDetail.TrainingTopicList.Where(x => x.SkillTypeID == 3).ToList();
                    var getFieldsValueFunctional = ddlService.ddlSkill(3);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<TrainingTopic, string>()
                        .ConvertUsing(s => getFieldsValueFunctional.Where(x => x.id == s.SkillID).FirstOrDefault().value);

                    });
                    topicList = Mapper.Map<List<string>>(functionalTopic);
                }
                trnVM.TTopicList = topicList;
                ViewBag.trainingID = trainingID;
                return PartialView("ViewTrainingContainer", trnVM);
            }

            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _ViewTrainerDetails(int trainingID)
        {
            log.Info($"TrainingController/_ViewTrainerDetails/{trainingID}");
            try
            {
                TrainerDtlsViewModel trainerVM = new TrainerDtlsViewModel();
                //  TrainingViewModel trainingVM = new TrainingViewModel();
                var trainerDtls = trainingService.GetTrainerDetails(trainingID);

                if (trainerDtls != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Training, TrainerDtlsViewModel>();
                    });
                    trainerVM = Mapper.Map<TrainerDtlsViewModel>(trainerDtls);
                    trainerVM.enumTrainingStatus = (EnumTrainingStatus)(trainerDtls.TrainingStatus ?? 0);
                    if (trainerVM.TrainerList?.Count > 0)
                    {
                        int sno = 1;
                        trainerVM.TrainerList.ForEach(x =>
                        {
                            x.sno = sno++;
                            // x.TrainingID = trainingID;
                        });
                    }

                    if (trainerDtls.ExternalTrainer)
                        trainerVM.enumTrainerType = TrainerDtlsViewModel.EnumTrainer.External;
                    else if (trainerDtls.InternalTrainer)
                        trainerVM.enumTrainerType = TrainerDtlsViewModel.EnumTrainer.Internal;
                    else if (trainerDtls.BothTrainer)
                        trainerVM.enumTrainerType = TrainerDtlsViewModel.EnumTrainer.Both;

                }
                return PartialView("_ViewTrainerDetails", trainerVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public ActionResult FeedbackIndex()
        {
            log.Info($"TrainingController/FeedbackIndex");
            return View("FeedbackIndex");
        }

        public ActionResult _ViewFeedbackTrainings()
        {
            log.Info("TrainingController/_ViewFeedbackTrainings");
            try
            {
                TrainingViewModel trnVM = new TrainingViewModel();
                var getTrainingList = trainingService.GetTrainingList();
                trnVM.TrainingList = getTrainingList.Where(x => x.TrainingStatus == (int)EnumTrainingStatus.Completed).ToList();
                trnVM.ViewMode = 3;
                return PartialView("_TrainingGridViewForFeedback", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult TrainingFeedback(int trainingID)
        {
            log.Info($"TrainingController/TrainingFeedback/{trainingID}");
            try
            {
                var arpSkillDetail = trainingService.GetTrainingDetail(trainingID);

                BindDropDown();
                TrainingViewModel trnVM = new TrainingViewModel();
                trnVM._Training = arpSkillDetail._Training;

                trnVM._Training.enumTrainingStatus = (EnumTrainingStatus)(arpSkillDetail._Training.TrainingStatus ?? 0);

                if (arpSkillDetail._Training.OrientationTraining)
                    trnVM._Training.enumResidentialNonResidential = EnumResidentialNonResidential.Residential;
                else if (arpSkillDetail._Training.OnBoardTraining)
                    trnVM._Training.enumResidentialNonResidential = EnumResidentialNonResidential.NonResidential;
                else if (arpSkillDetail._Training.InternalAddressType)
                    trnVM._Training.enumResidentialNonResidential = EnumResidentialNonResidential.internaltraining;

                trnVM.TrainingStatus = trainingService.GetTrainingDtls(trainingID)?.TrainingStatus ?? 0;
                trnVM._Training.TrainingTypeName = trnVM._Training.TrainingType == 2 ? "Behavioral" : "Functional/Technical";

                //List<SelectListModel> trainingType = new List<SelectListModel>();
                //trainingType.Add(new SelectListModel { id = 2, value = "Behavioral" });
                //trainingType.Add(new SelectListModel { id = 3, value = "Functional/Technical" });

                //trnVM.ddlTrainningType = trainingType;
                List<string> topicList = new List<string>();
                if (trnVM._Training.TrainingType == 2)
                {
                    var behavioralTopic = arpSkillDetail.TrainingTopicList.Where(x => x.SkillTypeID == 2).ToList();
                    var getFieldsValueBehavioral = ddlService.ddlSkill(2);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<TrainingTopic, string>()
                        .ConvertUsing(s => getFieldsValueBehavioral.Where(x => x.id == s.SkillID).FirstOrDefault().value);

                    });
                    topicList = Mapper.Map<List<string>>(behavioralTopic);
                }
                else if (trnVM._Training.TrainingType == 3)
                {
                    var functionalTopic = arpSkillDetail.TrainingTopicList.Where(x => x.SkillTypeID == 3).ToList();
                    var getFieldsValueFunctional = ddlService.ddlSkill(3);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<TrainingTopic, string>()
                        .ConvertUsing(s => getFieldsValueFunctional.Where(x => x.id == s.SkillID).FirstOrDefault().value);

                    });
                    topicList = Mapper.Map<List<string>>(functionalTopic);
                }
                trnVM.TTopicList = topicList;
                return PartialView("ViewFeedbackContainer", trnVM);
            }

            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        #region Training Reports

        [HttpGet]
        public ActionResult EmployeeReport()
        {
            CommonFilter cfilter = new CommonFilter();
            cfilter.ReportType = "E";// for employee
            return View(cfilter);
        }

        public ActionResult _ReportFilter()
        {
            var employees = ddlService.GetAllEmployee();
            ViewBag.EmployeeList = new SelectList(employees, "id", "value");

            List<SelectListModel> Status = new List<SelectListModel>
                {
                    new SelectListModel { id = 0, value = "Select" },
                    new SelectListModel { id = 5, value = "Planned" },
                    new SelectListModel { id = 1, value = "Schedule" },
                    new SelectListModel { id = 2, value = "Reschedule" },
                    new SelectListModel { id = 3, value = "Completed" },
                    new SelectListModel { id = 4, value = "Cancel" },

                };
            ViewBag.Status = new SelectList(Status, "id", "value");

            return PartialView("_ReportFilter");
        }
        [HttpPost]
        public ActionResult EmployeeReport(CommonFilter cFilter, string ButtonType)
        {
            log.Info("TrainingController/EmployeeReport");
            try
            {
                if (ButtonType == "Export")
                {
                    DataTable exportData = new DataTable();
                    if (TempData["EmployeeDtl"] != null)
                    {
                        string tFilter = string.Empty;
                        var EmployeeList = (DataTable)TempData["EmployeeDtl"];
                        string fileName = string.Empty, msg = string.Empty;
                        string fullPath = Server.MapPath("~/FileDownload/");
                        if (cFilter.EmployeeID.HasValue && cFilter.EmployeeID > 0)
                        {
                            tFilter = EmployeeList.Rows[0]["EmpName"].ToString();
                            fileName = "Training Report-" + "(" + EmployeeList.Rows[0]["EmpName"].ToString() + ")" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;
                        }
                        else
                            fileName = "Training Report" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;

                        TempData.Keep("EmployeeDtl");
                        DataColumn dtc0 = new DataColumn("S.No.");
                        DataColumn dtc1 = new DataColumn("Training Title");
                        DataColumn dtc2 = new DataColumn("External/Internal");
                        DataColumn dtc3 = new DataColumn("Training Type");
                        DataColumn dtc4 = new DataColumn("Training Provider/Vendor");
                        //  DataColumn dtc5 = new DataColumn("Trainer");
                        DataColumn dtc6 = new DataColumn("Training Objective");
                        DataColumn dtc7 = new DataColumn("Venue");
                        DataColumn dtc8 = new DataColumn("Date (From - To)");
                        DataColumn dtc9 = new DataColumn("Duration");
                        DataColumn dtc10 = new DataColumn("Mode of Training");
                        DataColumn dtc11 = new DataColumn("Status");
                        DataColumn dtc12 = new DataColumn("Rating (out of 5)");
                        exportData.Columns.Add(dtc0);
                        exportData.Columns.Add(dtc1);
                        exportData.Columns.Add(dtc2);
                        exportData.Columns.Add(dtc3);
                        exportData.Columns.Add(dtc4);
                        exportData.Columns.Add(dtc6);
                        exportData.Columns.Add(dtc7);
                        exportData.Columns.Add(dtc8);
                        exportData.Columns.Add(dtc9);
                        exportData.Columns.Add(dtc10);
                        exportData.Columns.Add(dtc11);
                        if (!cFilter.EmployeeID.HasValue) /// Rating only show for Admin Report
                            exportData.Columns.Add(dtc12);

                        for (int i = 0; i < EmployeeList.Rows.Count; i++)
                        {
                            DataRow row = exportData.NewRow();
                            row["S.No."] = i + 1;
                            row["Training Title"] = EmployeeList.Rows[i]["TrainingTitle"].ToString();
                            row["External/Internal"] = Convert.ToBoolean(EmployeeList.Rows[i]["ExternalTrainer"]) == true ? "External" : Convert.ToBoolean(EmployeeList.Rows[i]["InternalTrainer"]) ? "Internal" : "Both";
                            row["Training Type"] = EmployeeList.Rows[i]["TrainingTypeName"].ToString();
                            row["Training Provider/Vendor"] = EmployeeList.Rows[i]["VendorName"].ToString();
                            row["Training Objective"] = EmployeeList.Rows[i]["TrainingObjective"].ToString();
                            var address = "";
                            if (!string.IsNullOrEmpty(EmployeeList.Rows[i]["Address"].ToString()))
                            {
                                if (!string.IsNullOrEmpty(EmployeeList.Rows[i]["StateName"].ToString()))
                                {
                                    address = address + EmployeeList.Rows[i]["Address"].ToString() + ",";
                                }
                                else
                                {
                                    address = address + EmployeeList.Rows[i]["Address"].ToString();
                                }
                            }
                            if (!string.IsNullOrEmpty(EmployeeList.Rows[i]["StateName"].ToString()))
                            {
                                if (!string.IsNullOrEmpty(EmployeeList.Rows[i]["City"].ToString()))
                                {
                                    address = address + EmployeeList.Rows[i]["StateName"].ToString() + ",";
                                }
                                else
                                {
                                    address = address + EmployeeList.Rows[i]["StateName"].ToString();
                                }
                            }

                            if (!string.IsNullOrEmpty(EmployeeList.Rows[i]["City"].ToString()))
                            {

                                if (!string.IsNullOrEmpty(EmployeeList.Rows[i]["PinCode"].ToString()))
                                {
                                    address = address + EmployeeList.Rows[i]["City"].ToString() + ",";
                                }
                                else
                                {
                                    address = address + EmployeeList.Rows[i]["City"].ToString();
                                }

                            }
                            if (!string.IsNullOrEmpty(EmployeeList.Rows[i]["PinCode"].ToString()))
                            {
                                address = address + EmployeeList.Rows[i]["PinCode"].ToString();
                            }
                            row["Venue"] = address;
                            row["Date (From - To)"] = Convert.ToDateTime(EmployeeList.Rows[i]["StartDate"]).ToString("dd-MM-yyyy") + " - " + Convert.ToDateTime(EmployeeList.Rows[i]["EndDate"]).ToString("dd-MM-yyyy");
                            row["Duration"] = (Convert.ToDateTime(EmployeeList.Rows[i]["EndDate"]) - Convert.ToDateTime(EmployeeList.Rows[i]["StartDate"])).TotalDays + 1 + " Day(s)";
                            row["Mode of Training"] = EmployeeList.Rows[i]["ModeofTraining"].ToString();

                            var status = Convert.ToInt16(EmployeeList.Rows[i]["TrainingStatus"]);
                            var statusName = "";

                            if (status == 3)
                                statusName = "Completed";
                            else if (status == 4)
                                statusName = "Cancel";
                            else if (status == 2)
                                statusName = "Reschedule";
                            else if (status == 1)
                                statusName = "Schedule";
                            else if (status == 5)
                                statusName = "Planned";

                            row["Status"] = statusName;

                            if (!cFilter.EmployeeID.HasValue) /// Rating only show for Admin Report
                                row["Rating (out of 5)"] = EmployeeList.Rows[i]["Rating"].ToString();

                            exportData.Rows.Add(row);
                        }
                        var res = trainingService.ExportEmployeeList(exportData, fullPath, fileName, tFilter);
                        fullPath = $"{fullPath}{fileName}";
                        return JavaScript("window.location = '" + Url.Action("DownloadAndDelete", "Base", new { @sFileName = fileName, @sFileFullPath = fullPath }) + "'");

                    }
                    return Content("");
                }
                else
                {
                    if (ButtonType == "View")
                    {
                        BaseReportModel reportModel = new BaseReportModel();
                        List<ReportParameter> parameterList = new List<ReportParameter>();
                        string rptName = string.Empty;
                        if (cFilter.EmployeeID.HasValue)
                        {
                            reportModel.reportType = 1;
                            parameterList.Add(new ReportParameter { name = "EmpID", value = cFilter.EmployeeID.Value });
                            parameterList.Add(new ReportParameter { name = "StatusID", value = cFilter.StatusID });
                            rptName = "TrainingEmployeeWise.rpt";
                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = rptName;
                            Session["ReportModel"] = reportModel;
                            return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            reportModel.reportType = 1;
                            cFilter.StatusID = cFilter.StatusID == 0 ? null : cFilter.StatusID;
                            parameterList.Add(new ReportParameter { name = "StatusID", value = cFilter.StatusID });
                            parameterList.Add(new ReportParameter { name = "FromDate", value = cFilter.FromDate });
                            parameterList.Add(new ReportParameter { name = "ToDate", value = cFilter.ToDate });
                            rptName = "TrainingStatusWise.rpt";

                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = rptName;
                            Session["ReportModel"] = reportModel;
                            return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        TrainingViewModel trnVM = new TrainingViewModel();
                        List<Training> getTrainingList = null;
                        if (cFilter.EmployeeID.HasValue)
                            getTrainingList = trainingService.GetEmployeeTrainingReport(cFilter);
                        else
                            getTrainingList = trainingService.GetTrainingReport(cFilter);

                        trnVM.TrainingList = getTrainingList;
                        TempData["EmployeeDtl"] = ExtensionMethods.ToDataTable(getTrainingList);
                        if (cFilter.EmployeeID.HasValue)
                            return PartialView("_EmployeeReportsGrid", trnVM);
                        else
                            return PartialView("_AdminReportsGrid", trnVM);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult TrainingReport()
        {
            CommonFilter cfilter = new CommonFilter();
            cfilter.ReportType = "A";// for Admin
            return View("EmployeeReport", cfilter);
        }
        public ActionResult _ReportFilterAdmin()
        {
            List<SelectListModel> Status = new List<SelectListModel>
                {
                    new SelectListModel { id = 0, value = "Select" },
                    new SelectListModel { id = 5, value = "Planned" },
                    new SelectListModel { id = 1, value = "Schedule" },
                    new SelectListModel { id = 2, value = "Reschedule" },
                    new SelectListModel { id = 3, value = "Completed" },
                    new SelectListModel { id = 4, value = "Cancel" },

                };
            ViewBag.Status = new SelectList(Status, "id", "value");
            CommonFilter cFilter = new CommonFilter();
            return PartialView("_ReportFilterAdmin", cFilter);
        }


        [HttpGet]
        public ActionResult DesignationReport()
        {
            CommonFilter cfilter = new CommonFilter();
            cfilter.ReportType = "D";// training report designation wise
            return View("ReportContainer", cfilter);
        }
        public PartialViewResult DesignationFilter()
        {
            var designation = ddlService.ddlDesignationList();
            ViewBag.Designation = new SelectList(designation, "id", "value");
            CommonFilter cFilter = new CommonFilter();
            cFilter.ReportType = "D";// training report designation wise
            return PartialView("_MultipleReportFilter", cFilter);
        }

        [HttpGet]
        public ActionResult InternalExternalReport()
        {
            CommonFilter cfilter = new CommonFilter();
            cfilter.ReportType = "I";// training report Internal External wise
            return View("ReportContainer", cfilter);
        }

        public PartialViewResult InternalExternalFilter()
        {
            List<SelectListModel> Status = new List<SelectListModel>
                {
                    new SelectListModel { id = 1, value = "Internal" },
                    new SelectListModel { id = 2, value = "External" },
                    new SelectListModel { id = 3, value = "Both" }
                };
            ViewBag.TrainerType = new SelectList(Status, "id", "value");
            CommonFilter cFilter = new CommonFilter();
            cFilter.ReportType = "I";// training report Internal External wise
            return PartialView("_MultipleReportFilter", cFilter);
        }

        [HttpGet]
        public ActionResult TrainingTypeReport()
        {
            CommonFilter cfilter = new CommonFilter();
            cfilter.ReportType = "T";// training report for training type wise
            return View("ReportContainer", cfilter);
        }

        public PartialViewResult TrainingTypeFilter()
        {
            var enumData = from EnumTrainingList e in Enum.GetValues(typeof(EnumTrainingList))
                           select new
                           {
                               ID = (int)e,
                               Name = e.GetDisplayName()
                           };
            ViewBag.TrainingType = new SelectList(enumData, "ID", "Name");
            CommonFilter cFilter = new CommonFilter();
            cFilter.ReportType = "T";//training report for training type wise
            return PartialView("_MultipleReportFilter", cFilter);
        }

        [HttpGet]
        public ActionResult TrainingProviderReport()
        {
            CommonFilter cfilter = new CommonFilter();
            cfilter.ReportType = "P";// training provider report
            return View("ReportContainer", cfilter);
        }
        public PartialViewResult TrainingProviderFilter()
        {
            var tprovider = trainingService.GetTrainingProviderList();
            ViewBag.TrainingProvider = new SelectList(tprovider, "id", "value");
            CommonFilter cFilter = new CommonFilter();
            cFilter.ReportType = "P";// training provider report
            return PartialView("_MultipleReportFilter", cFilter);
        }


        [HttpPost]
        public ActionResult TrainingReport(CommonFilter cFilter, string ButtonType)
        {
            log.Info("TrainingController/TrainingReport");
            try
            {
                if (ButtonType == "Export")
                {
                    string tFilter = string.Empty;
                    DataTable exportData = new DataTable();
                    if (TempData["TempTrainingRpt"] != null)
                    {
                        var EmployeeList = (DataTable)TempData["TempTrainingRpt"];
                        string fileName = string.Empty, msg = string.Empty;
                        string fullPath = Server.MapPath("~/FileDownload/");
                        if (cFilter.DesignationID.HasValue && cFilter.DesignationID > 0)
                        {
                            tFilter = EmployeeList.Rows[0]["DesignationName"].ToString();
                            // fileName = "Training Report for Designation Wise" + "(" + EmployeeList.Rows[0]["DesignationName"].ToString() + ")" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;
                            fileName = "Training Report for Designation Wise" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;
                        }
                        else if (cFilter.StatusID.HasValue && cFilter.StatusID > 0)
                        {
                            tFilter = cFilter.StatusID == 1 ? "Internal" : cFilter.StatusID == 2 ? "External" : "Both";
                            fileName = "Training Report for External Internal Wise" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;
                        }
                        else if (cFilter.ProcessID > 0)
                        {
                            tFilter = ((EnumTrainingList)cFilter.ProcessID).GetDisplayName();
                            fileName = "Training Report for Training Type" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;
                        }
                        else if (!string.IsNullOrEmpty(cFilter.ReportType))
                        {
                            tFilter = cFilter.ReportType;
                            fileName = "Training Report for Training Provider" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;
                        }

                        TempData.Keep("TempTrainingRpt");
                        DataColumn dtc0 = new DataColumn("S.No.");
                        DataColumn dtcn = new DataColumn("Name");
                        DataColumn dtc1 = new DataColumn("Training Title");
                        DataColumn dtc2 = new DataColumn("External/Internal");
                        DataColumn dtc3 = new DataColumn("Training Type");
                        DataColumn dtc11 = new DataColumn("Director Name");
                        DataColumn dtc4 = new DataColumn("Training Provider/Vendor");
                        DataColumn dtc6 = new DataColumn("Training Objective");
                        DataColumn dtc7 = new DataColumn("Venue");
                        DataColumn dtc8 = new DataColumn("Date (From - To)");
                        DataColumn dtc9 = new DataColumn("Duration");
                        DataColumn dtc10 = new DataColumn("Mode of Training");

                        exportData.Columns.Add(dtc0);
                        exportData.Columns.Add(dtcn);
                        exportData.Columns.Add(dtc1);
                        exportData.Columns.Add(dtc2);
                        exportData.Columns.Add(dtc3);
                        if (!string.IsNullOrEmpty(cFilter.ReportType))
                        {
                            exportData.Columns.Add(dtc11);
                        }
                        exportData.Columns.Add(dtc4);
                        exportData.Columns.Add(dtc6);
                        exportData.Columns.Add(dtc7);
                        exportData.Columns.Add(dtc8);
                        exportData.Columns.Add(dtc9);
                        exportData.Columns.Add(dtc10);

                        for (int i = 0; i < EmployeeList.Rows.Count; i++)
                        {
                            DataRow row = exportData.NewRow();
                            row["S.No."] = i + 1;
                            row["Name"] = EmployeeList.Rows[i]["EmpName"].ToString();
                            row["Training Title"] = EmployeeList.Rows[i]["TrainingTitle"].ToString();
                            row["External/Internal"] = Convert.ToBoolean(EmployeeList.Rows[i]["ExternalTrainer"]) == true ? "External" : Convert.ToBoolean(EmployeeList.Rows[i]["InternalTrainer"]) ? "Internal" : "Both";
                            row["Training Type"] = ((EnumTrainingList)Convert.ToInt16(EmployeeList.Rows[i]["enumTrainingList"])).GetDisplayName();
                            row["Training Provider/Vendor"] = EmployeeList.Rows[i]["VendorName"].ToString();
                            row["Training Objective"] = EmployeeList.Rows[i]["TrainingObjective"].ToString();

                            if (!string.IsNullOrEmpty(cFilter.ReportType))
                            {
                                var address = "";
                                if (!string.IsNullOrEmpty(EmployeeList.Rows[i]["Address"].ToString()))
                                {
                                    if (!string.IsNullOrEmpty(EmployeeList.Rows[i]["StateName"].ToString()))
                                    {
                                        address = address + EmployeeList.Rows[i]["Address"].ToString() + ",";
                                    }
                                    else
                                    {
                                        address = address + EmployeeList.Rows[i]["Address"].ToString();
                                    }
                                }
                                if (!string.IsNullOrEmpty(EmployeeList.Rows[i]["StateName"].ToString()))
                                {
                                    if (!string.IsNullOrEmpty(EmployeeList.Rows[i]["City"].ToString()))
                                    {
                                        address = address + EmployeeList.Rows[i]["StateName"].ToString() + ",";
                                    }
                                    else
                                    {
                                        address = address + EmployeeList.Rows[i]["StateName"].ToString();
                                    }
                                }

                                if (!string.IsNullOrEmpty(EmployeeList.Rows[i]["City"].ToString()))
                                {

                                    if (!string.IsNullOrEmpty(EmployeeList.Rows[i]["PinCode"].ToString()))
                                    {
                                        address = address + EmployeeList.Rows[i]["City"].ToString() + ",";
                                    }
                                    else
                                    {
                                        address = address + EmployeeList.Rows[i]["City"].ToString();
                                    }

                                }
                                if (!string.IsNullOrEmpty(EmployeeList.Rows[i]["PinCode"].ToString()))
                                {
                                    address = address + EmployeeList.Rows[i]["PinCode"].ToString();
                                }
                                row["Venue"] = address;

                                row["Director Name"] = EmployeeList.Rows[i]["DirectorName"].ToString();
                            }
                            else
                                row["Venue"] = EmployeeList.Rows[i]["Address"].ToString();
                            row["Date (From - To)"] = Convert.ToDateTime(EmployeeList.Rows[i]["StartDate"]).ToString("dd-MM-yyyy") + " - " + Convert.ToDateTime(EmployeeList.Rows[i]["EndDate"]).ToString("dd-MM-yyyy");
                            row["Duration"] = (Convert.ToDateTime(EmployeeList.Rows[i]["EndDate"]) - Convert.ToDateTime(EmployeeList.Rows[i]["StartDate"])).TotalDays + 1 + " Day(s)";
                            row["Mode of Training"] = EmployeeList.Rows[i]["ModeofTraining"].ToString();
                            exportData.Rows.Add(row);
                        }
                        if (cFilter.DesignationID.HasValue && cFilter.DesignationID > 0)
                            trainingService.ExportDesignationWise(exportData, fullPath, fileName, tFilter);
                        else if (cFilter.StatusID.HasValue && cFilter.StatusID > 0)
                            trainingService.ExportInternalExternalWise(exportData, fullPath, fileName, tFilter);
                        else if (cFilter.ProcessID > 0)
                            trainingService.ExportTypeWise(exportData, fullPath, fileName, tFilter);
                        else if (!string.IsNullOrEmpty(cFilter.ReportType))
                            trainingService.ExportProviderWise(exportData, fullPath, fileName, tFilter);


                        fullPath = $"{fullPath}{fileName}";
                        return JavaScript("window.location = '" + Url.Action("DownloadAndDelete", "Base", new { @sFileName = fileName, @sFileFullPath = fullPath }) + "'");

                    }
                    return Content("");
                }
                else
                {
                    if (ButtonType == "View")
                    {
                        BaseReportModel reportModel = new BaseReportModel();
                        List<ReportParameter> parameterList = new List<ReportParameter>();
                        string rptName = string.Empty;
                        if (cFilter.DesignationID.HasValue && cFilter.DesignationID.Value > 0)
                        {
                            reportModel.reportType = 1;
                            parameterList.Add(new ReportParameter { name = "DesignationID", value = cFilter.DesignationID.Value });
                            parameterList.Add(new ReportParameter { name = "FromDate", value = cFilter.FromDate });
                            parameterList.Add(new ReportParameter { name = "ToDate", value = cFilter.ToDate });
                            rptName = "TrainingDesignationWise.rpt";

                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = rptName;
                            Session["ReportModel"] = reportModel;
                            return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);

                        }
                        else if (cFilter.StatusID.HasValue && cFilter.StatusID.Value > 0)
                        {
                            reportModel.reportType = 1;
                            parameterList.Add(new ReportParameter { name = "trainertypeID", value = cFilter.StatusID.Value });
                            parameterList.Add(new ReportParameter { name = "FromDate", value = cFilter.FromDate });
                            parameterList.Add(new ReportParameter { name = "ToDate", value = cFilter.ToDate });
                            rptName = "TrainingInternalExternalWise.rpt";

                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = rptName;
                            Session["ReportModel"] = reportModel;
                            return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                        }
                        else if (cFilter.ProcessID > 0)
                        {
                            reportModel.reportType = 1;
                            parameterList.Add(new ReportParameter { name = "ttypeID", value = cFilter.ProcessID });
                            parameterList.Add(new ReportParameter { name = "FromDate", value = cFilter.FromDate });
                            parameterList.Add(new ReportParameter { name = "ToDate", value = cFilter.ToDate });
                            rptName = "TrainingTypeWise.rpt";

                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = rptName;
                            Session["ReportModel"] = reportModel;
                            return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                        }
                        else if (!string.IsNullOrEmpty(cFilter.ReportType))
                        {
                            string[] getTrainerName = null;
                            if (cFilter.ReportType != "All")
                                getTrainerName = trainingService.GetTrainingProviderNames(cFilter);
                            reportModel.reportType = 1;
                            //var trainerName = string.Empty;
                            //foreach (var item in getTrainerName)
                            //{
                            //    trainerName += item + ',';
                            //}
                            //trainerName = trainerName.Remove(trainerName.Length-1);
                            var trainerName = string.Empty;
                            if (getTrainerName != null)
                                trainerName = getTrainerName.LongLength > 0 ? string.Join(",", getTrainerName) : null;

                            parameterList.Add(new ReportParameter { name = "directorName", value = trainerName });
                            parameterList.Add(new ReportParameter { name = "FromDate", value = cFilter.FromDate });
                            parameterList.Add(new ReportParameter { name = "ToDate", value = cFilter.ToDate });
                            rptName = "TrainingProvidereWise.rpt";

                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = rptName;
                            Session["ReportModel"] = reportModel;
                            return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {

                        TempData["TempTrainingRpt"] = null;
                        TrainingViewModel trnVM = new TrainingViewModel();
                        List<Training> getTrainingList = null;
                        if (cFilter.DesignationID.HasValue && cFilter.DesignationID.Value > 0)
                            getTrainingList = trainingService.GetTrainingRrtDesignationWise(cFilter);
                        else if (cFilter.StatusID.HasValue && cFilter.StatusID.Value > 0)
                            getTrainingList = trainingService.GetTrainingRptInternalExternal(cFilter);
                        else if (cFilter.ProcessID > 0)
                            getTrainingList = trainingService.GetTrainingRptTypeWise(cFilter);
                        else if (!string.IsNullOrEmpty(cFilter.ReportType))
                            getTrainingList = trainingService.GetTrainingProviderReport(cFilter);

                        trnVM.TrainingList = getTrainingList;

                        if (cFilter.DesignationID.HasValue && cFilter.DesignationID.Value > 0)
                        {
                            TempData["TempTrainingRpt"] = ExtensionMethods.ToDataTable(getTrainingList);
                            return PartialView("_DesignationReports", trnVM);
                        }
                        else if (cFilter.StatusID.HasValue && cFilter.StatusID.Value > 0)
                        {
                            TempData["TempTrainingRpt"] = ExtensionMethods.ToDataTable(getTrainingList);
                            return PartialView("_InternalExternalReport", trnVM);
                        }
                        else if (cFilter.ProcessID > 0)
                        {
                            TempData["TempTrainingRpt"] = ExtensionMethods.ToDataTable(getTrainingList);
                            return PartialView("_TrainingTypeReport", trnVM);
                        }
                        else if (!string.IsNullOrEmpty(cFilter.ReportType))
                        {
                            TempData["TempTrainingRpt"] = ExtensionMethods.ToDataTable(getTrainingList);
                            return PartialView("_TrainingTypeReport", trnVM);
                        }
                        else
                            return Content("");
                    }
                    return Content("");
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        [HttpPost]
        public ActionResult AddMoreTimeSlot(DateTime? date, TimeSpan? frmTime, TimeSpan? toTime)
        {
            log.Info($"Training/AddMoreTimeSlot/date:{date}&frmTime:{frmTime}&toTime:{toTime}");
            TrainingViewModel tvm = new TrainingViewModel();
            tvm._Training = new Training();

            if (TempData["timeSlots"] != null)
            {
                tvm._Training.NewSlotFromTime = frmTime;
                tvm._Training.NewSlotToTime = toTime;
                var createdTimeSlots = ((List<TrainingDateWiseTimeSlot>)TempData["timeSlots"])
                    .OrderBy(y => y.TrainingDate).ToList();

                var tDates = createdTimeSlots
                 .Select(y => y.TrainingDate).OrderBy(y => y).Distinct().ToList();

                var sno = 1;
                tDates.ForEach(x =>
                {
                    tvm._Training.TrainingDates.Add(
                        new SelectListModel()
                        {
                            id = sno++,
                            value = x.ToString("dd/MM/yyyy")
                        });
                });

                if (date.HasValue)
                {
                    DateTime? fromDateTime = null, toDateTime = null;
                    if (frmTime.HasValue)
                        fromDateTime = date.Value.Add(frmTime.Value);
                    if (toTime.HasValue)
                        toDateTime = date.Value.Add(toTime.Value);

                    if (fromDateTime.HasValue && toDateTime.HasValue)
                    {
                        TempData["timeSlots"] = createdTimeSlots;
                        if (fromDateTime.Value > toDateTime.Value)
                        {
                            tvm._Training.SelectedTrainingDateIndex = (tvm._Training.TrainingDates.Where(x => x.value == date.Value.ToString("dd/MM/yyyy")))?.FirstOrDefault().id;
                            ModelState.AddModelError("FromDateRangeValidation", "From Time must be less than To Time");
                            return Json(new { msgType = "error", htmlData = ConvertViewToString("_AddMoreTimeSlot", tvm) }, JsonRequestBehavior.AllowGet);
                        }

                        //else if(toDateTime.Value < fromDateTime.Value)
                        //{
                        //    ModelState.AddModelError("ToDateRangeValidation", "From Time must be less than To Time");
                        //    return Json(new { msgType = "error", htmlData = ConvertViewToString("_AddMoreTimeSlot", tvm) }, JsonRequestBehavior.AllowGet);
                        //}


                    }
                    else if (!fromDateTime.HasValue || !toDateTime.HasValue)
                    {
                        if (!fromDateTime.HasValue)
                            ModelState.AddModelError("FromTimeRequired", "Please Select From Time");
                        if (!toDateTime.HasValue)
                            ModelState.AddModelError("ToTimeRequired", "Please Select To Time");
                        TempData["timeSlots"] = createdTimeSlots;

                        tvm._Training.SelectedTrainingDateIndex = (tvm._Training.TrainingDates.Where(x => x.value == date.Value.ToString("dd/MM/yyyy")))?.FirstOrDefault().id;
                        return Json(new { msgType = "error", htmlData = ConvertViewToString("_AddMoreTimeSlot", tvm) }, JsonRequestBehavior.AllowGet);
                    }

                    if (createdTimeSlots.Where(x => x.TrainingDate == date).Count() < 3)
                    {
                        TrainingDateWiseTimeSlot newTimeSlot = new TrainingDateWiseTimeSlot()
                        {
                            CreatedBy = userDetail.UserID,
                            CreatedOn = DateTime.Now,
                            TrainingDate = date.Value,
                            FromTime = frmTime,
                            ToTime = toTime
                        };
                        createdTimeSlots.Add(newTimeSlot);
                        TempData["timeSlots"] = createdTimeSlots;
                        tvm._Training.distributedTimeSlots = createdTimeSlots;
                        tvm._Training.SelectedTrainingDateIndex =
                    //createdTimeSlots.
                    //OrderBy(y=>y.TrainingDate).
                    //Where(x => x.TrainingDate == date.Value).Count();

                    createdTimeSlots.OrderBy(y => y.TrainingDate).ToList().IndexOf(newTimeSlot);
                    }
                    else
                    {
                        TempData["timeSlots"] = createdTimeSlots;
                        tvm._Training.SelectedTrainingDateIndex = (tvm._Training.TrainingDates.Where(x => x.value == date.Value.ToString("dd/MM/yyyy")))?.FirstOrDefault().id;
                        ModelState.AddModelError("MaxTimeSlotValidation", "Cannot add more than 3 time slots.");


                        return Json(new { status = 2, msgType = "error", htmlData = ConvertViewToString("_AddMoreTimeSlot", tvm) }, JsonRequestBehavior.AllowGet);
                    }


                }
                else
                {
                    TempData["timeSlots"] = createdTimeSlots;
                    ModelState.AddModelError("DateFieldRequired", "Please Select Date");
                    return Json(new { msgType = "error", htmlData = ConvertViewToString("_AddMoreTimeSlot", tvm) }, JsonRequestBehavior.AllowGet);
                }
            }

            return PartialView("_DistributedTimeSlot", tvm);
        }

        public ActionResult _GetTrainingRating(int trainingID)
        {
            log.Info($"Training/_GetTrainingRating/trainingID:{trainingID}");
            try
            {
                var model = trainingService.GetTrainingRatings(trainingID);

                var arpSkillDetail = trainingService.GetTrainingDetail(trainingID);

                TrainingViewModel trnVM = new TrainingViewModel();
                trnVM._Training = arpSkillDetail._Training;

                if (trnVM._Training.TimeSlotType == (byte)EnumTimeSlotType.Distributed)
                {
                    trnVM._Training.distributedTimeSlots = trainingService.GetTrainingTimeSlots(trainingID);
                    trnVM._Training.enumTimeSlotType = EnumTimeSlotType.Distributed;
                }

                if (arpSkillDetail._Training.OrientationTraining)
                    trnVM._Training.enumResidentialNonResidential = EnumResidentialNonResidential.Residential;
                else if (arpSkillDetail._Training.OnBoardTraining)
                    trnVM._Training.enumResidentialNonResidential = EnumResidentialNonResidential.NonResidential;

                if (arpSkillDetail._Training.TrainingType == (int)EnumTrainingType.Behavioral)
                    trnVM._Training.TrainingTypeName = EnumTrainingType.Behavioral.GetDisplayName();
                else if (arpSkillDetail._Training.TrainingType == (int)EnumTrainingType.Functional)
                    trnVM._Training.TrainingTypeName = EnumTrainingType.Functional.GetDisplayName();

                trnVM._Training.enumTrainingList = (EnumTrainingList)(arpSkillDetail._Training.TrainingType);
                trnVM._Training.enumTimeSlotType = (EnumTimeSlotType)(arpSkillDetail._Training.TimeSlotType);
                //=======
                string venueAddress = string.Empty;

                if (!string.IsNullOrEmpty(trnVM._Training.Address))
                {
                    if (!string.IsNullOrEmpty(trnVM._Training.StateName))
                        venueAddress += trnVM._Training.Address + ",";

                    else
                        venueAddress += trnVM._Training.Address;
                }
                if (!string.IsNullOrEmpty(trnVM._Training.StateName))
                {
                    if (!string.IsNullOrEmpty(trnVM._Training.City))
                        venueAddress += trnVM._Training.StateName + ",";

                    else
                        venueAddress += trnVM._Training.StateName;
                }

                if (!string.IsNullOrEmpty(trnVM._Training.City))
                {
                    if (!string.IsNullOrEmpty(trnVM._Training.PinCode))

                        venueAddress += trnVM._Training.City + ",";

                    else
                        venueAddress += trnVM._Training.City;

                }
                if (!string.IsNullOrEmpty(trnVM._Training.PinCode))
                    venueAddress += trnVM._Training.PinCode;

                model.TrainingVenue = venueAddress;
                model.trainingVM = trnVM;
                return PartialView("_TrainingRating", model);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _ExportTrainingRating(int? trainingID)
        {
            log.Info($"Training/_ExportTrainingRating");

            string fullPath = Server.MapPath("~/FileDownload/");
            string fileName = string.Empty, msg = string.Empty, result = string.Empty;

            fileName = "Training Rating Report." + FileExtension.xlsx;

            result = trainingService.ExportTrainingRating((int)trainingID, fullPath, fileName);

            if (result == "notfound")
                ModelState.AddModelError("ReportTypeRequired", "No Record Found.");
            else
                return Json(new
                {
                    fileName = fileName,
                    fullPath = fullPath + fileName,
                    message = "success"
                }, JsonRequestBehavior.AllowGet);

            return Content("");
        }

        #region Delete Traning

        public ActionResult Delete(int trainingID)
        {
            log.Info("DeleteSanction");
            try
            {

                var flag = trainingService.DeleteTraining(trainingID);
                TempData["Message"] = "Successfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");

        }
        #endregion
    }
}
