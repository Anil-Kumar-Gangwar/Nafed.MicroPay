//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nafed.MicroPay.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Training
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Training()
        {
            this.TrainingDocumentRepositories = new HashSet<TrainingDocumentRepository>();
            this.TrainingFeedBackFormHdrs = new HashSet<TrainingFeedBackFormHdr>();
            this.TrainingParticipants = new HashSet<TrainingParticipant>();
            this.TrainingPrerequisites = new HashSet<TrainingPrerequisite>();
            this.TrainingFeedbackDetails = new HashSet<TrainingFeedbackDetail>();
            this.TrainingTopics = new HashSet<TrainingTopic>();
            this.Trainers = new HashSet<Trainer>();
            this.TrainingDateWiseTimeSlots = new HashSet<TrainingDateWiseTimeSlot>();
        }
    
        public int TrainingID { get; set; }
        public int TrainingType { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.TimeSpan StartDateFromTime { get; set; }
        public System.TimeSpan StartDateToTime { get; set; }
        public System.DateTime EndDate { get; set; }
        public string TrainingObjective { get; set; }
        public string Address { get; set; }
        public Nullable<int> StateID { get; set; }
        public string City { get; set; }
        public string PinCode { get; set; }
        public Nullable<decimal> TrainingCost { get; set; }
        public Nullable<decimal> PostTrainingInvesment { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string VendorPhoneNo { get; set; }
        public string VendorGSTINNo { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool OrientationTraining { get; set; }
        public bool OnBoardTraining { get; set; }
        public bool InternalTraining { get; set; }
        public bool ExternalTraining { get; set; }
        public Nullable<byte> TrainingStatus { get; set; }
        public string TrainingTitle { get; set; }
        public Nullable<short> TrainingList { get; set; }
        public string TrainingContent { get; set; }
        public string ModeofTraining { get; set; }
        public string DirectorName { get; set; }
        public bool BothTraining { get; set; }
        public string OtherTopic { get; set; }
        public string CancelReason { get; set; }
        public Nullable<int> CancelReasonID { get; set; }
        public byte TimeSlotType { get; set; }
        public Nullable<System.DateTime> NominationDate { get; set; }
        public string Remark { get; set; }
        public bool InternalAddressType { get; set; }
    
        public virtual SkillType SkillType { get; set; }
        public virtual State State { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingDocumentRepository> TrainingDocumentRepositories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingFeedBackFormHdr> TrainingFeedBackFormHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingParticipant> TrainingParticipants { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingPrerequisite> TrainingPrerequisites { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingFeedbackDetail> TrainingFeedbackDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingTopic> TrainingTopics { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Trainer> Trainers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingDateWiseTimeSlot> TrainingDateWiseTimeSlots { get; set; }
    }
}
