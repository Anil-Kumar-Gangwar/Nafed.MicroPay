using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class EmpAchievementCertificationReportFilterVM
    {
        [Display(Name ="Employee")]
        public int ? SelectedEmployeeID { set; get; }

        [Required(ErrorMessage = "Please Select Report Type")]
       // [Range(1,2,ErrorMessage ="Please Select Report Type")]
        public byte SelectedReportType { set; get; }

        public List<SelectListModel> employees { set; get; }

        [Required(ErrorMessage = "Please Select From Date")]
        [Display(Name = "From Date")]
        public DateTime ? fromDate { set; get; }

        [Required(ErrorMessage = "Please Select To Date")]
        [Display(Name = "To Date")]
        public DateTime ? toDate { set; get; }

       
    }

   
}