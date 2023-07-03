using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class LTCViewModel
    {
        public List<Model.LTC> LTCList { set; get; }        
        public Model.UserAccessRight userRights { get; set; } = new Model.UserAccessRight();
        public Model.EmployeeProcessApproval approvalSetting { set; get; } = new Model.EmployeeProcessApproval();
    }
}