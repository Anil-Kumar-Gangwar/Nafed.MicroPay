using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class FormGroupHHdr
    {
        public string ReportingYr { get; set; }
        public int FormGroupID { get; set; }
        public int FormID { get; set; }
        public string EmployeeCode { get; set; }
        public int EmployeeID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> WorkedPeriodUnderROFrom { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> WorkedPeriodUnderROTo { get; set; }
        public byte Intelligenceknowledge { get; set; }
        public byte AmenabilitytoDiscipline { get; set; }
        public byte PunctualityAndAttendance { get; set; }
        public byte DevotionToDuty { get; set; }
        public byte Cleanliness { get; set; }
        public byte BehaviourWithSuperiorsAndColleagues { get; set; }
        public byte Integrity { get; set; }
        public Nullable<decimal> Total { get; set; }
        public int FormState { get; set; }
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

        [Required]
        [Display(Name = "Reporting Officer Remark")]
        public string ReportingOfficerRemark { get; set; }

        [Required]
        [Display(Name = "Reviewing Officer Remark")]
        public string ReviewingOfficerRemark { get; set; }

        [Required]
        [Display(Name = "Acceptance Authority Remark")]
        public string AcceptanceAuthorityRemark { get; set; }

        public Nullable<int> DesignationID { get; set; }
        public Nullable<int> DepartmentID { get; set; }

        public Nullable<int> DossierNo { get; set; }

        public Nullable<System.DateTime> EmpSenddate { get; set; }
        public string AcceptanceAuth { get; set; }
        public string AcceptanceDesignation { get; set; }
        public Nullable<System.DateTime> RTSenddate { get; set; }
        public Nullable<System.DateTime> RVSenddate { get; set; }
        public Nullable<System.DateTime> AASenddate { get; set; }
        //#region Rating 

        //public RatingGuide RG_IntelligenceAndknowledge { set; get; }
        //public RatingGuide RG_AmenabilitytoDiscipline { set; get; }

        //public RatingGuide RG_PunctualityAndAttendance { set; get; }
        //public RatingGuide RG_DevotionToDuty { set; get; }

        //public RatingGuide RG_Cleanliness { set; get; }

        //public RatingGuide RG_BehaviourWithSuperiorsAndColleagues { set; get; }

        //public FormPart4Integrity RG_Integrity { set; get; }
        //#endregion
    }
}
