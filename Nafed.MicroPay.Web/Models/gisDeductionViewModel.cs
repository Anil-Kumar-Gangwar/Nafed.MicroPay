using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;
namespace MicroPay.Web.Models
{
    public class GisDeductionViewModel
    {
        public List<Model.GisDeduction> GisDeductionList { set; get; }
        public Model.UserAccessRight userRights { get; set; }
    }
}