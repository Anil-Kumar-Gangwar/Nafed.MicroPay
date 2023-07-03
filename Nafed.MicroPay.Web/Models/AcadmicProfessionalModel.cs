using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class AcadmicProfessionalModel
    {
        public List<Model.AcadmicProfessionalDetailsModel> AcadmicProfessionalList { set; get; }
        public Model.UserAccessRight userRights { get; set; } = new Model.UserAccessRight();

        [Display(Name = "Proff. Qualification")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Qualifcation")]
        public int qualificationId { get; set; }
    }
}