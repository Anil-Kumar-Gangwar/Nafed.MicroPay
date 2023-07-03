using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;
using AutoMapper;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Common;
using System.IO;
namespace MicroPay.Web.Controllers.Appraisal
{
    public class TrainingFeedbackFormController : BaseController
    {
        private readonly ITrainingService trainingService;
        private readonly IDropdownBindService ddlService;
        public TrainingFeedbackFormController(ITrainingService trainingService, IDropdownBindService ddlService)
        {
            this.trainingService = trainingService;
            this.ddlService = ddlService;
        }

        // GET: TrainingFeedbackForm
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _EmployeeFeedbackFormGridView()
        {
            log.Info("TrainingFeedbackFormController/_EmployeeFeedbackFormGridView");
            try
            {
                TrainingViewModel trnVM = new TrainingViewModel();
                trnVM.userRights = userAccessRight;
                var getTrainingList = trainingService.GetFeedbackFormList(userDetail.EmployeeID);
                trnVM.TrainingList = getTrainingList;
                return PartialView("_EmployeeFeedbackFormGridView", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult GenerateFeedbackForm(int feedBackFormHdrID)
        {
            log.Info($"TrainingFeedbackFormController/GenerateFeedbackForm/{feedBackFormHdrID}");
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

        public ActionResult _GenerateFeedbackForm(int feedBackFormHdrID)
        {
            log.Info("TrainingFeedbackFormController/_GenerateFeedbackForm");
            try
            {
                TrainingFeedbackDetail trnVM = new TrainingFeedbackDetail();

                var getTrainingList = trainingService.GenerateFeedbackForm(feedBackFormHdrID, (int)userDetail.EmployeeID);
                //  trnVM.TrainingFeedbackDetailPart1List = getTrainingList.TrainingFeedbackDetailList;

                trnVM.TrainingFeedbackDetailList = getTrainingList.TrainingFeedbackDetailList.Where(x => x.PartNo == 1).ToList();
                trnVM.TrainingFeedbackDetailPart2List = getTrainingList.TrainingFeedbackDetailList.Where(x => x.PartNo == 2).ToList();
                trnVM.TrainingFeedbackDetailPart3List = getTrainingList.TrainingFeedbackDetailList.Where(x => x.PartNo == 3).ToList();

                trnVM.RatingType = getTrainingList.TrainingFeedBackFormHeader.RatingType;
                trnVM.UpperRatingValue = getTrainingList.TrainingFeedBackFormHeader.UpperRatingValue;
                trnVM.StartDate = getTrainingList._Training.StartDate;
                trnVM.EndDate = getTrainingList._Training.EndDate;
                trnVM.FeedBackFormHdrID = getTrainingList.TrainingFeedBackFormHeader.FeedBackFormHdrID;
                trnVM.TrainingID = getTrainingList._Training.TrainingID;
                trnVM.TrainingTypeName = ((EnumTrainingType)getTrainingList._Training.TrainingType).GetDisplayName();
                trnVM.TrainingTitle = getTrainingList._Training.TrainingTitle;
                trnVM.ActionPlan = getTrainingList.ActionPlan;
                trnVM.EmployeeCode = getTrainingList.EmployeeCode;
                trnVM.EmployeeName = getTrainingList.EmployeeName;

                var venueAddress = string.Empty;
                if (!string.IsNullOrEmpty(getTrainingList._Training.Address))
                {
                    if (!string.IsNullOrEmpty(getTrainingList._Training.StateName))
                    {
                        venueAddress += getTrainingList._Training.Address + ",";
                    }
                    else
                    {
                        venueAddress += getTrainingList._Training.Address;
                    }
                }
                if (!string.IsNullOrEmpty(getTrainingList._Training.StateName))
                {
                    if (!string.IsNullOrEmpty(getTrainingList._Training.City))
                    {
                        venueAddress += getTrainingList._Training.StateName + ",";
                    }
                    else
                    {
                        venueAddress += getTrainingList._Training.StateName;
                    }
                }

                if (!string.IsNullOrEmpty(getTrainingList._Training.City))
                {

                    if (!string.IsNullOrEmpty(getTrainingList._Training.PinCode))
                    {
                        venueAddress += getTrainingList._Training.City + ",";
                    }
                    else
                    {
                        venueAddress += getTrainingList._Training.City;
                    }
                }
                if (!string.IsNullOrEmpty(getTrainingList._Training.PinCode))
                {
                    venueAddress += getTrainingList._Training.PinCode;
                }

                trnVM.TrainerType = getTrainingList._Training.ExternalTrainer ? "External" : getTrainingList._Training.InternalTrainer ? "Internal" : "Both";
                trnVM.TrainingVenue = venueAddress;
                //  trnVM.TrainingInstructor = getTrainingList.TrainerList;

                if (getTrainingList.TrainerList != null && getTrainingList.TrainerList.Count > 0)
                {
                    foreach (var trainer in getTrainingList.TrainerList)
                    {
                        trnVM.TrainingInstructor = trnVM.TrainingInstructor + trainer.TrainerName + Environment.NewLine;
                    }

                }
                if (trnVM.RatingType == "N")
                {
                    //List<SelectListModel> lst = new List<SelectListModel>();
                    //var kk = Enumerable.Range(1, (int)trnVM.UpperRatingValue).ToList();
                    //Mapper.Initialize(cfg =>
                    //{

                    //    cfg.CreateMap<int, SelectListModel>().
                    //    ForMember(d => d.id, o => o.MapFrom(s => s))
                    //    .ForMember(d => d.value, o => o.MapFrom(s => s.ToString()));
                    //});

                    //trnVM.RatingList = Mapper.Map<List<SelectListModel>>(kk);
                }
                return PartialView("_TrainingFeedbackForm", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }



        }

        [HttpGet]
        public ActionResult ViewFeedbackForm(int feedBackFormHdrID)
        {
            log.Info($"TrainingFeedbackFormController/ViewFeedbackForm/{feedBackFormHdrID}");
            try
            {
                return View("ViewFeedbackFormContainer");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _ViewFeedbackForm(int feedBackFormHdrID, int? empID = null)
        {
            log.Info("TrainingFeedbackFormController/_ViewFeedbackForm");
            try
            {
                TrainingFeedbackDetail trnVM = new TrainingFeedbackDetail();

                var getTrainingList = trainingService.GenerateFeedbackForm(feedBackFormHdrID, empID.HasValue ? (int)empID : (int)userDetail.EmployeeID);
                trnVM.TrainingFeedbackDetailList = getTrainingList.TrainingFeedbackDetailList.Where(x => x.PartNo == 1).ToList();
                trnVM.TrainingFeedbackDetailPart2List = getTrainingList.TrainingFeedbackDetailList.Where(x => x.PartNo == 2).ToList();
                trnVM.TrainingFeedbackDetailPart3List = getTrainingList.TrainingFeedbackDetailList.Where(x => x.PartNo == 3).ToList();

                trnVM.UpperRatingValue = getTrainingList.TrainingFeedBackFormHeader.UpperRatingValue;
                trnVM.StartDate = getTrainingList._Training.StartDate;
                trnVM.EndDate = getTrainingList._Training.EndDate;
                trnVM.FeedBackFormHdrID = getTrainingList.TrainingFeedBackFormHeader.FeedBackFormHdrID;
                trnVM.TrainingID = getTrainingList._Training.TrainingID;
                trnVM.TrainingTypeName = ((EnumTrainingType)getTrainingList._Training.TrainingType).GetDisplayName();
                trnVM.TrainingTitle = getTrainingList._Training.TrainingTitle;
                trnVM.ActionPlan = getTrainingList.ActionPlan;
                trnVM.EmployeeCode = getTrainingList.EmployeeCode;
                trnVM.EmployeeName = getTrainingList.EmployeeName;
                var venueAddress = string.Empty;
                if (!string.IsNullOrEmpty(getTrainingList._Training.Address))
                {
                    if (!string.IsNullOrEmpty(getTrainingList._Training.StateName))
                    {
                        venueAddress += getTrainingList._Training.Address + ",";
                    }
                    else
                    {
                        venueAddress += getTrainingList._Training.Address;
                    }
                }
                if (!string.IsNullOrEmpty(getTrainingList._Training.StateName))
                {
                    if (!string.IsNullOrEmpty(getTrainingList._Training.City))
                    {
                        venueAddress += getTrainingList._Training.StateName + ",";
                    }
                    else
                    {
                        venueAddress += getTrainingList._Training.StateName;
                    }
                }

                if (!string.IsNullOrEmpty(getTrainingList._Training.City))
                {

                    if (!string.IsNullOrEmpty(getTrainingList._Training.PinCode))
                    {
                        venueAddress += getTrainingList._Training.City + ",";
                    }
                    else
                    {
                        venueAddress += getTrainingList._Training.City;
                    }
                }
                if (!string.IsNullOrEmpty(getTrainingList._Training.PinCode))
                {
                    venueAddress += getTrainingList._Training.PinCode;
                }

                trnVM.TrainerType = getTrainingList._Training.ExternalTrainer ? "External" : getTrainingList._Training.InternalTrainer ? "Internal" : "Both";
                trnVM.TrainingVenue = venueAddress;
                if (getTrainingList.TrainerList != null && getTrainingList.TrainerList.Count > 0)
                {
                    foreach (var trainer in getTrainingList.TrainerList)
                    {
                        trnVM.TrainingInstructor = trnVM.TrainingInstructor + trainer.TrainerName + Environment.NewLine;
                    }

                }
                return PartialView("_ViewFeedbackForm", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }



        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult _PostFeedbackForm(TrainingFeedbackDetail trnVM)
        {
            log.Info("TrainingFeedbackFormController/_PostFeedbackForm");
            try
            {
                //if (trnVM.RatingType == "N")
                //{
                //    //List<SelectListModel> lst = new List<SelectListModel>();
                //    //var kk = Enumerable.Range(1, 10).ToList();
                //    //Mapper.Initialize(cfg =>
                //    //{

                //    //    cfg.CreateMap<int, SelectListModel>().
                //    //    ForMember(d => d.id, o => o.MapFrom(s => s))
                //    //    .ForMember(d => d.value, o => o.MapFrom(s => s.ToString()));
                //    //});

                //    //trnVM.RatingList = Mapper.Map<List<SelectListModel>>(kk);

                if (trnVM.TrainingFeedbackDetailPart2List != null && trnVM.TrainingFeedbackDetailPart2List.Count > 0)
                {
                    foreach (var item in trnVM.TrainingFeedbackDetailPart2List)
                    {
                        trnVM.TrainingFeedbackDetailList.Add(item);
                    }
                }
                if (trnVM.TrainingFeedbackDetailPart3List != null && trnVM.TrainingFeedbackDetailPart3List.Count > 0)
                {
                    foreach (var item in trnVM.TrainingFeedbackDetailPart3List)
                    {
                        trnVM.TrainingFeedbackDetailList.Add(item);
                    }
                }

                for (int i = 0; i < trnVM.TrainingFeedbackDetailList.Count; i++)
                {
                    if (!trnVM.TrainingFeedbackDetailList[i].enumRatingNo.HasValue)
                    {
                        ModelState.AddModelError($"TrainingFeedbackDetailList[{i}].enumRatingNo", "Please select scale");

                    }
                }
                //}
                var dd = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();

                if (ModelState.IsValid)
                {
                    TrainingSchedule trnSchedule = new TrainingSchedule();
                    trnSchedule.TrainingID = trnVM.TrainingID;
                    trnSchedule.FeedBackFormHdrID = trnVM.FeedBackFormHdrID;
                    trnSchedule.RatingType = trnVM.RatingType;
                    trnSchedule.TrainingFeedbackDetailList = trnVM.TrainingFeedbackDetailList;
                    trnSchedule.CreatedBy = userDetail.UserID;
                    trnSchedule.CreatedOn = DateTime.Now;
                    trnSchedule.EmployeeID = (int)userDetail.EmployeeID;
                    trnSchedule.ActionPlan = trnVM.ActionPlan;
                    int res = trainingService.InsertEmployeeFeedbackForm(trnSchedule);

                    if (ValidateAntiXSS(trnSchedule))
                    {
                        ModelState.AddModelError("", "Updation failed, A potentially dangerous request value was detected");
                        return Json(new { status = 0, htmlData = ConvertViewToString("_TrainingFeedbackForm", trnVM) }, JsonRequestBehavior.AllowGet);
                    }
                    if (res > 0)
                    {
                        return Json(new { status = 1, msg = "Successfully Updated" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ModelState.AddModelError("RatingError", "Please select scale");
                    return Json(new { status = 0, htmlData = ConvertViewToString("_TrainingFeedbackForm", trnVM) }, JsonRequestBehavior.AllowGet);
                    //   return PartialView("_TrainingFeedbackForm", trnVM);
                }
                return PartialView("_TrainingFeedbackForm", trnVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }



        }


        public ActionResult MakeNomination(int tid)
        {
            log.Info($"TrainingFeedbackFormController/MakeNomination");
            try
            {
                List<TrainingParticipant> participantsData = new List<TrainingParticipant>();
                TrainingParticipant tparticipant = new TrainingParticipant();
                tparticipant.EmployeeID = (int)userDetail.EmployeeID;
                tparticipant.TrainingID = tid;
                tparticipant.CreatedBy = userDetail.UserID;
                tparticipant.CreatedOn = DateTime.Now;
                tparticipant.Nomination = true;
                tparticipant.NominationAccepted = (int)EmpLeaveStatus.Pending;
                participantsData.Add(tparticipant);

                var empReportings = GetEmpProcessApprovalSetting(tparticipant.EmployeeID, WorkFlowProcess.LeaveApproval);

                ProcessWorkFlow workFlow = new ProcessWorkFlow()
                {
                    SenderID = empReportings.EmployeeID,
                    ReceiverID = empReportings.ReportingTo,
                    SenderDepartmentID = userDetail.DepartmentID,
                    SenderDesignationID = userDetail.DesignationID,
                    CreatedBy = userDetail.UserID,
                    EmployeeID = (int)userDetail.EmployeeID,
                    Scomments = "Nomination for Training",
                    ProcessID = (int)WorkFlowProcess.LeaveApproval,
                    StatusID = (int)EmpLeaveStatus.Pending
                };


                var skillDetail = trainingService.GetTrainingDetail(tparticipant.TrainingID);

                TrainingSchedule trnSchedule = new TrainingSchedule();
                trnSchedule._Training = new Training();
                trnSchedule._Training = skillDetail._Training;
                trnSchedule.TrainingParticipntList = participantsData;

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

                var flag = trainingService.PostTrainingParticipants(participantsData, true, workFlow, trnSchedule);
                if (flag)
                    return Json(new { success = true, msgType = "success", msg = "Dear Employee, Your nomination has been done for the selected training. Now, your request has been sent to your Reporting Manager for further approval. You will get confirmation when it get approved or rejected." }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { success = false, msgType = "error", msg = "Nomination not done" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}