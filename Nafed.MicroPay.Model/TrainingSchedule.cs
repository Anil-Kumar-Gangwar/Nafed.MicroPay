using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class TrainingSchedule
    {
        public List<Training> TrainingList = new List<Training>();
        public Training _Training { get; set; }

        public List<Trainer> TrainerList = new List<Trainer>();
        public List<TrainingDocumentRepository> TrainingDocRepositoryList = new List<TrainingDocumentRepository>();
        public List<TrainingParticipant> TrainingParticipntList = new List<TrainingParticipant>();
        public List<TrainingPrerequisite> TrainingPrereqList = new List<TrainingPrerequisite>();
        public List<TrainingTopic> TrainingTopicList = new List<TrainingTopic>();
        public List<SelectListModel> ddlTrainningType { get; set; } = new List<SelectListModel>();

        public int[] CheckBoxListBehavioral { get; set; }
        public int[] CheckBoxListFunctional { get; set; }


        public int FeedBackFormHdrID { get; set; }
        public int TrainingID { get; set; }
        public string RatingType { get; set; }
        public Nullable<byte> LowerRatingValue { get; set; }
        public Nullable<byte> UpperRatingValue { get; set; }
        public Nullable<byte> TrainingStatus { get; set; }
        public bool RatingNumber { get; set; }
        public bool RatingGrade { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }

        public List<TrainingFeedBackFormDetail> TrainingFbFormDtlList = new List<Model.TrainingFeedBackFormDetail>();

        public TrainingFeedBackFormHdr TrainingFeedBackFormHeader { get; set; }

        public int EmployeeID { get; set; }
        public string ActionPlan { get; set; }

        public List<TrainingFeedbackDetailListing> TrainingFeedbackDetailList = new List<Model.TrainingFeedbackDetailListing>();

        public string TrainingTitle { get; set; }

        public List<string> topicList = new List<string>();
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }

        public bool AddAttachment { get; set; }

        public int [] Attachments { get;set;}

    }
}
