using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
   public  class FormGroupGHdr
    {
        public string ReportingYr { get; set; }
        public int FormGroupID { get; set; }
        public string EmployeeCode { get; set; }
        public int FormID { get; set; }
        public int EmployeeID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> WorkedPeriodUnderROFrom { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> WorkedPeriodUnderROTo { get; set; }
        public byte JobPerformance_A { get; set; }
        public byte AdherenceToInstructions_A { get; set; }
        public byte AdherenceToInstructions_B { get; set; }

        public int FormState { get; set; }
        
        public byte Safety_A { get; set; }
        public byte Safety_B { get; set; }
        public byte Safety_C_1 { get; set; }
        public byte Safety_C_2 { get; set; }
        public byte MAINTENANCE_A { get; set; }
        public byte MAINTENANCE_B { get; set; }
        public byte DEPENDABILITY_A { get; set; }
        public byte DISCIPLINE_GENERAL_BEHAVIOUR_A { get; set; }
        public byte DISCIPLINE_GENERAL_BEHAVIOUR_B_1 { get; set; }
        public byte DISCIPLINE_GENERAL_BEHAVIOUR_B_2 { get; set; }
        public byte REGULARITY_PUNCTUALITY_A { get; set; }
        public byte REGULARITY_PUNCTUALITY_B { get; set; }
        public byte TECHNICAL_KNOWLEDGE_A { get; set; }
        public byte TECHNICAL_KNOWLEDGE_B { get; set; }

        public byte RECORD_KEEPING_A { get; set; }
        public byte INTEGRITY { get; set; }
        public Nullable<byte> total { get; set; }

        [Required]
        [Display(Name = "Reporting Officer Comment")]
        public string ReportingOfficeComment { get; set; }

        [Required]
        [Display(Name = "Reviewing Officer Comment")]
        public string ReviewingOfficerComment { get; set; }

        [Required]
        [Display(Name = "Acceptance Authority Comment")]
        public string AcceptanceAuthorityComment { get; set; }

        public bool AcceleratedPromotion { get; set; }
        public bool PromotionInDueCourse { get; set; }
    
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }


        public string ReportingTo { get; set; }
        public string ReviewerTo { get; set; }

        public string AcceptanceAuthority { get; set; }
        public string AADesignation { get; set; }
        public string DesignationName { get; set; }
        public string ReportingDesignation { get; set; }
        public string ReviewerDesignation { get; set; }
        public string EmployeeName { get; set; }

        public string PlaceOfJoin { get; set; }
        public string Qualification { get; set; }
        public System.DateTime DOJ { get; set; }
        [Display(Name = "Date of Joining")]
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<System.DateTime> DOPP { get; set; }

        public Nullable<int> DesignationID { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public string EmpPotentialDevelopment { get; set; }
        public Nullable<int> DossierNo { get; set; }

        public string AcceptanceAuth { get; set; }
        public string AcceptanceDesignation { get; set; }
        public Nullable<System.DateTime> RTSenddate { get; set; }
        public Nullable<System.DateTime> RVSenddate { get; set; }
        public Nullable<System.DateTime> AASenddate { get; set; }
        public Nullable<System.DateTime> EmpSenddate { get; set; }
    }
}
