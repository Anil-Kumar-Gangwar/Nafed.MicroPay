using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class CandidateDetails
    {
        public int RegistrationID { get; set; }
        public int RequirementID { get; set; }
        public string Post { get; set; }
        public string RegistrationNo { get; set; }
        public string CandidateFullName { get; set; }
        public string PersonalEmailID { get; set; }
        [DisplayName("Date of Publish From :")]
        public Nullable<System.DateTime> PublishDateTo { get; set; }
        [DisplayName("Date of Publish To :")]
        public Nullable<System.DateTime> PublishDateFrom { get; set; }

        public int GenderID { get; set; }
        public bool EligibleForWrittenExam { get; set; }
        public string CandidatePicture { get; set; }
        public string CandidateSignature { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public int Age { get; set; }
        public bool IssueAdmitCard { get; set; }
        public string EligiblityRemark { get; set; }

        public int? TotalApplied { get; set; }
    }
}
