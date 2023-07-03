using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class UserAccessControlViewModel
    {

        public List<Model.UserAccessControlRights> menuParentList { set; get; }
        public List<Model.UserAccessControlRights> menuChildList { set; get; }

    }
}