using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroPay.Web.Models
{
    public class StaffTransferViewModel
    {
        public StaffTransfer staffTransfer { set; get; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();
    }
}