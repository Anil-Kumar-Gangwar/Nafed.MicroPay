using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class LeaveTypeViewModel
    {
        public List<Model.LeaveType> leaveTypeList { get; set; }
        public Model.UserAccessRight userRights { get; set; }
    }
}