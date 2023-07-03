using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class UserViewModel
    {

        public List<Model.User> userList { set; get; }

    }
}