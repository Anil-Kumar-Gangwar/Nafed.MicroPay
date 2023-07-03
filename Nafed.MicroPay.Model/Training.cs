using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class Training
    {
        public int TrainingID { get; set; }
        [Display(Name = "Training For")]
        [Range(1, 100, ErrorMessage = "Please select Training Title")]
        public int TrainingType { get; set; }
        public byte TimeSlotType { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please select Start Date")]
        [Display(Name = "Start Date")]
        public Nullable<System.DateTime> StartDate { get; set; }
        [Required(ErrorMessage = "Please select Time")]
        [Display(Name = "From Time")]
        public System.TimeSpan StartDateFromTime { get; set; }

        [Required(ErrorMessage = "Please select Time")]
        [Display(Name = "To Time")]
        public System.TimeSpan StartDateToTime { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please select End Date")]
        [Display(Name = "End Date")]
        public Nullable<System.DateTime> EndDate { get; set; }
        //[Required(ErrorMessage = "Please select Time")]
        //[Display(Name = "From Time")]
        //public System.TimeSpan EndDateFromTime { get; set; }
        //[Required(ErrorMessage = "Please select Time")]
        //[Display(Name = "To Time")]
        //public System.TimeSpan EndDateToTime { get; set; }
        [Required(ErrorMessage = "Please enter Training Objective")]
        [Display(Name = "Training Objective")]
        public string TrainingObjective { get; set; }

        public string Address { get; set; }
        [Display(Name = "State")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select State")]
        public Nullable<int> StateID { get; set; }
        public string City { get; set; }
        [Display(Name = "Pin Code")]
        [RegularExpression("^[1-9]{1}[0-9]{2}\\s{0,1}[0-9]{3}$", ErrorMessage = "Please enter valid Pin Code.")]
        public string PinCode { get; set; }
        [Display(Name = "Cost")]
        public Nullable<decimal> TrainingCost { get; set; }
        [Display(Name = "Post Training Invesment")]
        public Nullable<decimal> PostTrainingInvesment { get; set; }


        //public bool Oreintation_Onboarding { get; set; }
        //[Display(Name = "Training Details")]
        //public Nullable<bool> TrainingDetail { get; set; }

        [Display(Name = "Orientation")]
        public bool OrientationTraining { get; set; }

        [Display(Name = "OnBoard")]
        public bool OnBoardTraining { get; set; }

        [Display(Name = "Internal")]
        public bool InternalAddressType { get; set; }
        

        [Display(Name = "Internal")]
        public bool InternalTrainer { get; set; }

        [Display(Name = "External")]
        public bool ExternalTrainer { get; set; }

        [Display(Name = "Both")]
        public bool BothTrainer { get; set; }

        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }
        [Display(Name = "Vendor Address")]
        public string VendorAddress { get; set; }

        [Display(Name = "Director Name")]
        public string DirectorName { get; set; }

        [StringLength(10, ErrorMessage = "{0} not be exceed 10 char")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid mobile number")]
        [Display(Name = "Vendor Phone No.")]
        public string VendorPhoneNo { get; set; }
        [Display(Name = "GSTIN No.")]
        public string VendorGSTINNo { get; set; }
        public Nullable<byte> TrainingStatus { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        public string TrainingTypeName { get; set; }
        public EnumTrainingType enumTrainingType { get; set; }

        public EnumTrainingStatus enumTrainingStatus { set; get; }

        public List<Trainer> trainerList { set; get; }
        public List<TrainingDocumentRepository> trainingDocs { set; get; }
        public List<TrainingDateWiseTimeSlot> distributedTimeSlots { set; get; } = new List<TrainingDateWiseTimeSlot>();

        public List<SelectListModel> TrainingDates { set; get; } = new List<SelectListModel>();
        public int? SelectedTrainingDateIndex { set; get; }

        public System.TimeSpan? NewSlotFromTime { get; set; }

        public System.TimeSpan? NewSlotToTime { get; set; }

        public bool FeedbackFormStatus { get; set; }
        public string StateName { get; set; }
        public int? FeedBackFormHdrID { get; set; }

        [Display(Name = "Address Type")]
        [Range(1, 3, ErrorMessage = "Please Choose Address Type")]
        public EnumResidentialNonResidential enumResidentialNonResidential { set; get; }
        [Display(Name = "Training Title")]
        public string TrainingTitle { get; set; }
        [Display(Name = "Training Type")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Choose Training Type")]
        public EnumTrainingList enumTrainingList { get; set; }
        [Display(Name = "Training Content")]
        public string TrainingContent { get; set; }
        [Display(Name = "Mode of Training")]
        public string ModeofTraining { get; set; }

        public string EmpCode { get; set; }
        public string EmpName { get; set; }

        public int EmployeeID { get; set; }
        public bool Nomination { get; set; }

        [Display(Name = "Specify Other Topic")]
        public string OtherTopic { get; set; }
       
        public EnumTimeSlotType enumTimeSlotType { set; get; } = (EnumTimeSlotType)1;

        [Display(Name = "Standard")]
        public bool StandardTimeSlot { set; get; }
        [Display(Name = "Distributed")]
        public bool DistributedTimeSlot { set; get; }

        public int? CancelReasonID { get; set; }
        public string CancelReason { get; set; }
        public bool isCancelButtonShow { get; set; }

        public Nullable<decimal> Rating { get; set; }
        public bool ? TrainingAttended { get; set; }


        [DataType(DataType.Date, ErrorMessage = "Invalid Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]    
        [Display(Name = "Nomination Last Date")]
        public Nullable<System.DateTime> NominationDate { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        public int ? ParticipantExist { get; set; }
        public int ? DesignationID { get; set; }
        public string DesignationName { get; set; }

        public List<Skill> trainingTopic { set; get; }
    }
    public enum EnumTimeSlotType
    {
        [Display(Name = "Standard")]
        Standard = 1,
        [Display(Name = "Distributed")]
        Distributed = 2
    }
    public enum EnumTrainingType
    {
        [Display(Name = "Behavioral")]
        Behavioral = 2,
        [Display(Name = "Functional/Technical")]
        Functional = 3,

    }


    public enum EnumResidentialNonResidential
    {
        [Display(Name = "Residential")]
        Residential = 1,  //==== Residential
        [Display(Name = "Non Residential")]
        NonResidential = 2  ,     //====  NonResidential  
        [Display(Name = "Internal Training")]
        internaltraining = 3
    }

    public enum EnumTrainingList
    {
        [Display(Name = "Training")]
        Training = 1,
        [Display(Name = "Workshop")]
        Workshop = 2,
        [Display(Name = "Seminar")]
        Seminar = 3,
        [Display(Name = "Conference")]
        Conference = 4,
        [Display(Name = "Induction")]
        Induction = 5,
        [Display(Name = "(Domestic/International)")]
        Domestic_International = 6,

    }

    public enum EnumTrainingStatus
    {
        Schedule = 1,
        Reschedule = 2,
        Completed = 3,
        Cancel = 4,
        Planned = 5,
    }
}
