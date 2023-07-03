using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroPay.Web.Models
{
    public class CheckBoxPostedFields
    {
        //this array will be used to POST values from the form to the controller
        public int[] fieldIds { get; set; }
    }
}