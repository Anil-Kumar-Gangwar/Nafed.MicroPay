﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;
namespace MicroPay.Web.Models
{
    public class WagesViewModel
    {
        public List<Model.Wages> WagesList { set; get; }
        public Model.UserAccessRight userRights { get; set; }
    }
}