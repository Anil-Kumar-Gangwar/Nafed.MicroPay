using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class SalaryManualDataViewModel
    {
        public List<Model.SalaryManualData> SalaryManualDataList { set; get; }
        public Model.UserAccessRight userRights { get; set; } = new Model.UserAccessRight();

        public List<Model.SelectListModel> EmployeeTypeList { get; set; }
        public List<Model.SelectListModel> EmployeeList { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateofJoining { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> AppointmentDate { get; set; }

    }
}