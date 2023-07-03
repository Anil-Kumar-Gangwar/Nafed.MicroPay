using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class FormGroupDHdr
    {
        public string ReportingYr { get; set; }
        public int FormGroupID { get; set; }
        public int FormID { get; set; }
        public int EmployeeID { get; set; }
        public int DesignationID { get; set; }
        public int DepartmentID { get; set; }
        public string ReportingTo { get; set; }
        public string ReviewerTo { get; set; }
        public string DesignationName { get; set; }
        public string ReportingDesignation { get; set; }
        public string ReviewerDesignation { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> WorkedPeriodUnderROFrom { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> WorkedPeriodUnderROTo { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string PlaceOfJoin { get; set; }
        public string Qualification { get; set; }
        public System.DateTime DOJ { get; set; }
        [Display(Name = "Date of Joining")]
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<System.DateTime> DOPP { get; set; }
        [Required(ErrorMessage = "Please enter remark.")]
        public string Part2_1 { get; set; }
        public string Part2_2 { get; set; }
        public string Part2_3 { get; set; }
        public string Part2_4 { get; set; }
        public string Part3_1 { get; set; }

        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_A_1 { get; set; }
        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_A_2 { get; set; }
        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_A_3 { get; set; }
        [Required(ErrorMessage = "Please enter Grade.")]

        public Nullable<decimal> Part3_2_A_4 { get; set; }
        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_B_1 { get; set; }
        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_B_2 { get; set; }
        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_B_3 { get; set; }
        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_B_4 { get; set; }
        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_B_5 { get; set; }
        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_B_6 { get; set; }
        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_B_7 { get; set; }
        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_B_8 { get; set; }

        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_C_1 { get; set; }

        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_C_2 { get; set; }

        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_C_3 { get; set; }

        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_C_4 { get; set; }

        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_C_5 { get; set; }

        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> Part3_2_C_6 { get; set; }


        [Required(ErrorMessage = "Please enter remark.")]
        public string Part4_1 { get; set; }
        public string Part4_2 { get; set; }

        [Required(ErrorMessage = "Please enter remark.")]
        public string Part4_3 { get; set; }

        [Required(ErrorMessage ="Please enter remark.")]
        public string Part4_4 { get; set; }
        public string Part4_5 { get; set; }
        [Required(ErrorMessage = "Please enter Remark.")]
        public string Part5_1 { get; set; }
        public bool Part5_2 { get; set; }
        [Required(ErrorMessage = "Please enter Remark.")]
        public string Part5_3 { get; set; }
        public string Part5_4 { get; set; }
        public string ReportingOfficerRemarks { get; set; }
        public string ReviewingOfficerRemarks { get; set; }
        [Required(ErrorMessage = "Please enter Remark.")]
        public string AcceptanceAuthorityRemarks { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

        public Nullable<byte> PART5_5_Weightage { get; set; }

        public string PART5_5_Remark { get; set; }

        public Nullable<byte> PART4_6_Weightage { get; set; }


        public Nullable<byte> PART4_3_Grade { get; set; }
        public Nullable<byte> PART4_4_Grade { get; set; }


        [Required]
        public string PART4_6_Remark { get; set; }

        public Nullable<int> DossierNo { get; set; }
        public Nullable<int> FormState { get; set; }


        public Nullable<decimal> Part3_A_Overall_Grading { get; set; }
        public Nullable<decimal> Part3_B_Overall_Grading { get; set; }
        public Nullable<decimal> Part3_C_Overall_Grading { get; set; }

        public Nullable<byte> PART4_1_Grade { get; set; }

        public Nullable<decimal> PART4_6_Grade { get; set; }
        public Nullable<decimal> PART5_5_Grade { get; set; }

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

        #region Part 4 ---- Integrity

        public FormPart4Integrity FormPart4Integrity { set; get; }

        #endregion
    }
}
