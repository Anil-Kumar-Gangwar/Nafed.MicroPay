using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroPay.Web.Models
{
    public class CheckBoxListViewModel
    {
        public IEnumerable<CheckBox> AvailableFields { get; set; }
        public IEnumerable<CheckBox> SelectedFields { get; set; }
        public CheckBoxPostedFields PostedFields { get; set; }

        public CheckBoxPostedFields PostedFields1 { get; set; }
    }
}