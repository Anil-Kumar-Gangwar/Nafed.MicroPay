using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class EmployeeAttendancedetailsViewModel
    {
        public Model.UserAccessRight userRights { get; set; } = new Model.UserAccessRight();
        //public List<Model.EmployeeLeave> GetEmployeeAttendancedetailsList { set; get; }
        public List<Model.EmployeeAttendanceDetails> EmployeeAttendanceDetailsList { set; get; }
        public List<Model.SelectListModel> ddlYear = new List<Model.SelectListModel>();
        public List<Model.SelectListModel> ddlMonth = new List<Model.SelectListModel>();

        public int? BranchID { get; set; }
        public int? EmployeeID { get; set; }
        public string BranchName { get; set; }
        public string EmployeeName { get; set; }
        public int SelectedMonth { get; set; }
        public int SelectYear { get; set; }

    }
}