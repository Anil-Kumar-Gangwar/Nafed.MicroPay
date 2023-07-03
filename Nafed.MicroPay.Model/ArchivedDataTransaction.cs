using System;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class ArchivedDataTransaction
    {
        [Display(Name ="Select Year")]
        [Required(ErrorMessage ="Please Select Year.")]
        public int SelectedYear { set; get; }

        public int DataArchiveTransID { set; get; }
        public DateTime  CutOffFromDate { set; get; }

        public DateTime CutOffToDate { set; get; }

        public DateTime  AchivedTillDate { set; get; }

        public string ArchivedBy { set; get; }

        public DateTime TransactionDate { set; get; }

        public int TransactionStatus { set; get; }

       public string TransactionRemarks { set; get; }
    }

    public enum ArchiveStatus
    {
        [Display(Name = "Not Defined")]
        NotDefined =0,  
        [Display(Name ="Failed")]
         FailedAndRollback= 1,
        [Display(Name = "Success")]
        Commited =2,
        [Display(Name ="Rolback")]
        Rolback=3


    }
}
