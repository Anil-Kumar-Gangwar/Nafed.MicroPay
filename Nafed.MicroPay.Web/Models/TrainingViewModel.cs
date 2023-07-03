using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class TrainingViewModel
    {
        public UserAccessRight userRights { get; set; }

        public List<Training> TrainingList = new List<Training>();
        public Training _Training { get; set; }
        public List<Trainer> TrainerList { get; set; }
        public List<TrainingDocumentRepository> TrainingDocRepositoryList = new List<TrainingDocumentRepository>();
        public List<TrainingParticipant> TrainingParticipntList = new List<TrainingParticipant>();
        public List<TrainingPrerequisite> TrainingPrereqList { get; set; }
        public List<TrainingTopic> TrainingTopicList = new List<TrainingTopic>();
        public List<SelectListModel> ddlTrainningType { get; set; } = new List<SelectListModel>();
        public CheckBoxListViewModel CheckBoxListBehavioral { get; set; } = new CheckBoxListViewModel();
        public CheckBoxListViewModel CheckBoxListFunctional { get; set; } = new CheckBoxListViewModel();
        public int TrainingStatus { set; get; }
        public List<string> TTopicList = new List<string>();

        #region Training Participant
        public List<TrainingParticipantsDetail> trainingParticipantsDetail { set; get; }
        public List<TrainingParticipant> trainingParticipants { set; get; }
        public int? trainingID { get; set; }

        [Display(Name = "Code")]
        public string EmployeeCode { set; get; }

        [Display(Name = "Name")]
        public string EmployeeName { set; get; }

        public List<SelectListModel> department { set; get; }

        [Display(Name = "Department")]
        public int? DepartmentID { set; get; }

        #endregion Training Participant

        #region Training Document
        public List<TrainingDocumentRepository> trainingDocument { set; get; }
        public TrainingDocumentRepository documentRepository { set; get; }
        #endregion Training Document

       public int ViewMode { get; set; }

        public bool AddAttachment { get; set; }

        public CheckBoxListViewModel CheckBoxListAttachment { get; set; } = new CheckBoxListViewModel();
    }
}