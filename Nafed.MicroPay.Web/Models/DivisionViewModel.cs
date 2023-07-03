using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class DivisionViewModel
    {
        public List<Division> listDivision { get; set; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();

    }
}