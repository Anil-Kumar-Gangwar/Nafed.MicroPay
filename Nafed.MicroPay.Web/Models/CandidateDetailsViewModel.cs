using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class CandidateDetailsViewModel
    {
        public List<Model.CandidateDetails> CandidateDetails { set; get; }
        public Model.UserAccessRight userRights { get; set; } = new Model.UserAccessRight();

        public Nullable<int> IssueAdmitCard { get; set; }

        public int? DesignationID { get; set; }
        public List<Model.SelectListModel> DesignationList { set; get; }

        #region Employee Search Filter 
        public Nullable<System.DateTime> PublishDateTo { get; set; }
        public Nullable<System.DateTime> PublishDateFrom { get; set; }
        public Nullable<int> EligibleForWrittenExam { get; set; }
        #endregion
    }
}