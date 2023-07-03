using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class DesignationViewModel
    {
        [Display(Name = "Designation")]
        public int? designationID { set; get; }
        public List<SelectListModel> designationList { set; get; }

        [Display(Name = "Cadre")]
        public int? cadreID { set; get; }
        public List<SelectListModel> cadreList { set; get; }
        public List<Designation> listDesignation { set; get; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();
    }
}