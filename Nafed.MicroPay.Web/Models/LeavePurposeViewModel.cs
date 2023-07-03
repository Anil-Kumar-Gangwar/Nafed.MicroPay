using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class LeavePurposeViewModel
    {
        public List<Model.LeavePurpose> leavePurposeList { get; set; }
        public Model.UserAccessRight userRights { get; set; }
    }
}