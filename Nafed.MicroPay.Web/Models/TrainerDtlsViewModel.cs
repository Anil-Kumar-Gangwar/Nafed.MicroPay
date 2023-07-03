using System;
using System.Collections.Generic;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class TrainerDtlsViewModel
    {
        public int TrainingID { set; get; }
        public List<Trainer> TrainerList { get; set; }

        //[Display(Name = "Training Type")]
        //[Range(1,2, ErrorMessage = "Please Choose Training Type")]
        //public EnumOrientationOrOnBoardTaining enumTrainingType { set; get; }

        [Display(Name = "Trainer Agency ")]
        [Range(1, 3, ErrorMessage = "Please Choose Trainer Agency")]
        public EnumTrainer enumTrainerType { set; get; }

        [Display(Name = "Orientation")]
        public bool OrientationTraining { get; set; }

        [Display(Name = "OnBoard")]
        public bool OnBoardTraining { get; set; }

        [Display(Name = "Internal")]
        public bool InternalTrainer { get; set; }

        [Display(Name = "External")]
        public bool ExternalTrainer { get; set; }

        [Display(Name = "Both")]
        public bool BothTrainer { get; set; }

        [Display(Name = "Director Name")]
        [Required(ErrorMessage = "Please Enter Director Name")]
        public string DirectorName { get; set; }

        [Display(Name = "Vendor Name")]
        [Required(ErrorMessage = "Please Enter Vendor Name")]
        public string VendorName { get; set; }

        [Display(Name = "Vendor Address")]
        [Required(ErrorMessage = "Please Enter Vendor Address")]
        public string VendorAddress { get; set; }

        [DataType(DataType.PhoneNumber)]
        //[StringLength(10, ErrorMessage = "{0} not be exceed 12 char")]
        [RegularExpression(@"^(?(?=^[\d\-]{0,12}$)[0-9]\d{2,4}-\d{6,8})$", ErrorMessage = "Invalid Phone number")]
        [Display(Name = "Vendor Phone No.")]
        // [Required(ErrorMessage = "Please Enter Vendor Phone No.")]

        public string VendorPhoneNo { get; set; }

        [Display(Name = "GSTIN No.")]
        //   [Required(ErrorMessage = "Please Enter GSTIN No.")]
        public string VendorGSTINNo { get; set; }

        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        public enum EnumOrientationOrOnBoardTaining
        {
            Orientation = 1,  //====  Orientation Training 
            OnBoard = 2       //====  OnBoarding Training  
        }

        public enum EnumTrainer
        {
            Internal = 1,   //==== Internal Trainer .
            External = 2,  //==== External Trainer  
            Both = 3
        }

        public EnumTrainingStatus enumTrainingStatus { get; set; }
    }
}