using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class SalaryHeadViewModel
    {
        public IEnumerable<SalaryHeadField> fieldList { set; get; }
        public Nafed.MicroPay.Model.UserAccessRight userRights { get; set; }
        public List<SalaryHeadSlab> salarySlabList { get; set; }
        public string FieldName { set; get; }

        [Display(Name = "Employee Type :")]
        public int? selectedEmployeeTypeID { get; set; }
        public IEnumerable<SelectListModel> employeeType { set; get; }
    }
}