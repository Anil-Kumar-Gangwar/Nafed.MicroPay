using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class ArrearManualDataViewModel
    {
        public List<Model.ArrearManualData> ArrearManualDataList { set; get; }
        public Model.UserAccessRight userRights { get; set; } = new Model.UserAccessRight();

        [Display(Name = "Month")]
        public int Month { get; set; }
        [Display(Name = "Year")]


        public int Year { get; set; }
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [Display(Name = "Branch")]


        public int BranchID { get; set; }
    }
}