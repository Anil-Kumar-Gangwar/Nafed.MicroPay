using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class RequirementViewModel
    {
        public List<Model.Requirement> RequirementList { set; get; }
        public Model.UserAccessRight userRights { get; set; } = new Model.UserAccessRight();
        [Display(Name = "Designation")]
        public int? DesignationId { get; set; }
        public List<Model.SelectListModel> designationList { set; get; }
        [Display(Name ="Publish Date From")]
        public DateTime? FromDate { get; set; }
        [Display(Name = "Publish Date To")]
        public DateTime? ToDate { get; set; }

        public List<Model.AppliedCandidateDetail> appliedCandidateList { get; set; }

        public string PostName { get; set; }
    }
}