using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class TDSTaxRuleSlabsVM
    {
        [Display(Name ="Financial Year")]
        public string selectedFyear { set; get; }

        public List<SelectListModel> fYList { set; get; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();

        public IEnumerable<tblTDSTaxRulesSlab> fYrTdsSlabs { set; get; }

    }
}