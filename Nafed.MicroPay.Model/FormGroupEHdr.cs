using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class FormGroupEHdr
    {
        public string ReportingYr { get; set; }
        public int FormGroupID { get; set; }
        public int FormID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> WorkedPeriodUnderROFrom { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> WorkedPeriodUnderROTo { get; set; }
        [Required(ErrorMessage = "Please enter Remark.")]
        public string PART2_1 { get; set; }
        public string PART2_3A { get; set; }
        [Required(ErrorMessage = "Please enter Remark.")]
        public string PART2_3B { get; set; }
        [Required(ErrorMessage = "Please enter Remark.")]
        public string PART2_4 { get; set; }

        public byte PART4_1_Grade { get; set; }
        [Required(ErrorMessage = "Please enter Remark.")]
        public string PART4_1_Remark { get; set; }

        public byte PART4_2_Grade { get; set; }

        public string PART4_2_Remark { get; set; }

        public byte PART4_3_Grade { get; set; }
        [Required(ErrorMessage = "Please enter Remark.")]
        public string PART4_3_Remark { get; set; }

        public byte PART4_4_Grade { get; set; }

        [Required(ErrorMessage = "Please enter Remark.")]
        public string PART4_4_Remark { get; set; }

        //[Required(ErrorMessage = "Please select Grade.")]
        public byte PART4_5_Grade { get; set; }

        [Required(ErrorMessage = "Please enter Remark.")]
        public string PART4_5_Remark { get; set; }

        [Required(ErrorMessage = "Please select Grade.")]
        public Nullable<decimal> PART4_6_Weightage { get; set; }

        [Required(ErrorMessage = "Please enter Remark.")]
        public string PART4_6_Remark { get; set; }
        // [Required]
        public string PART5_1 { get; set; }
        // [Required]
        public bool PART5_2 { get; set; }
        //[Required]
        [Required(ErrorMessage = "Please enter Remark.")]
        public string PART5_3 { get; set; }
        [Required]
        public string PART5_4 { get; set; }

        [Required(ErrorMessage = "Please select Grade.")]
        public Nullable<decimal> PART5_5_Weightage { get; set; }

        [Required(ErrorMessage = "Please enter Remark.")]
        public string PART5_5_Remark { get; set; }
        public string Remarks { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string EmployeeName { get; set; }
        public string PlaceOfJoin { get; set; }
        public string Qualification { get; set; }
        public System.DateTime DOJ { get; set; }
        [Display(Name = "Date of Joining")]
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<System.DateTime> DOPP { get; set; }
        public string ReportingTo { get; set; }
        public string ReviewerTo { get; set; }
        public string DesignationName { get; set; }
        public string ReportingDesignation { get; set; }
        public string ReviewerDesignation { get; set; }
        public int FormState { get; set; }
        public Nullable<int> DossierNo { get; set; }
        public string AcceptanceAuth { get; set; }
        public string AcceptanceDesignation { get; set; }
        public Nullable<System.DateTime> RTSenddate { get; set; }
        public Nullable<System.DateTime> RVSenddate { get; set; }
        public Nullable<System.DateTime> AASenddate { get; set; }
        public Nullable<System.DateTime> EmpSenddate { get; set; }

        #region Part 4 --- Grading 
        public FormGrade Part4_1_Gr { set; get; }
        public FormGrade Part4_2_Gr { set; get; }
        public FormGrade Part4_3_Gr { set; get; }
        public FormPart4Integrity Part4_4_Gr { set; get; }
        public FormGrade Part4_6_Gr { set; get; }
        #endregion

        #region Part 4 --- Integrity
        public FormPart4Integrity FormPart4Integrity { set; get; }
        #endregion
    }
}
