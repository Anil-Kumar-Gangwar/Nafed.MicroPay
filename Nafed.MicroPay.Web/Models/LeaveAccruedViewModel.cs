using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class LeaveAccruedViewModel
    {
        public Model.UserAccessRight userRights { get; set; }
        public List<Model.LeaveAccruedDetails> LeaveAccruedDetailsList { set; get; }
    }
}