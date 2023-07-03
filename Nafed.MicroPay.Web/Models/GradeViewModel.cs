using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroPay.Web.Models
{
    public class GradeViewModel
    {
        public UserAccessRight userRights { get; set; }
        public List<Grade> listGrade { get; set; }
    }
}