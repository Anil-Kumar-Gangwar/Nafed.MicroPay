using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MicroPay.Web.Models
{
    public class ProjectedIncrementViewModel
    {
        public List<Model.ProjectedIncrement> projectedIncrement { get; set; }
        public List<Model.StopIncrement> stopIncrementDetails { get; set; }
        public Model.UserAccessRight userRights { get; set; } = new Model.UserAccessRight();

        #region Increment Search Filter 

        [Display(Name = "Branch")]
        public int? BranchID { set; get; }
        public List<Model.SelectListModel> branchList { set; get; }
        [Display(Name = "Code")]
        public string EmployeeCode { set; get; }
        [Display(Name = "Name")]
        public string EmployeeName { set; get; }
        [Display(Name = "Month")]
        public int? incrementMonthId { set; get; }
        public List<SelectListItem> incrementMonth { set; get; }
        public string Message { set; get; }
        public int stopIncrement { set; get; } = 1;
        [Display(Name = "January")]
        public bool January { get; set; }
        [Display(Name = "July")]
        public bool July { get; set; }
        [Display(Name = "Only Stopped")]
        public bool OnlyStopped { get; set; }
        [Display(Name = "Employee")]
        public int forEmployeeId { get; set; }

        #endregion
    }
}