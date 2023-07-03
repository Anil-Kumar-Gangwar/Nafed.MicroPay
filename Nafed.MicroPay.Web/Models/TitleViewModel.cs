using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class TitleViewModel
    {
        public List<Title> listTitle { set; get; }
        public UserAccessRight userRights { get; set; }
    }
}