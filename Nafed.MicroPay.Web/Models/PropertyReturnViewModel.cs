using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class PropertyReturnViewModel
    {
        public List<Model.PR> PRList { get; set; }
        public int PRID { get; set; }

        public Model.UserAccessRight userRights { get; set; } = new Model.UserAccessRight();
        [Display(Name = "Year")]
        public int? CYear { set; get; }
        public List<Model.SelectListModel> CYearList { set; get; }
        public IEnumerable<Model.SelectListModel> EmployeeList { set; get; }

        public Nullable<int> EmployeeId { get; set; }

        [Display(Name = "Code")]
        public string EmployeeCode { set; get; }



        [Display(Name = "Name")]
        public string Employeename { get; set; }


        public IEnumerable<Model.SelectListModel> PropertyTypeDetails { set; get; }

        public int PropertyType { get; set; }
        public IEnumerable<Model.SelectListModel> RelationDetails { set; get; }
        public int RelationID { get; set; }

        public IEnumerable<Model.SelectListModel> AcquiredTypeDetails { set; get; }
        public int AcquiredTypeID { get; set; }

        public int StatusID { get; set; }


    }
}