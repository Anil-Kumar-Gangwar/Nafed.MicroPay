using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class DesignationAppraisalFormVM
    {

        public IEnumerable<AppraisalForm> appraisalForms { set; get; }

        public List<SelectListModel> designations { set; get; }

        public UserAccessRight userRights { get; set; }

        [Required]
        public int ? appraisalFormID { set; get; }

       
    }
}