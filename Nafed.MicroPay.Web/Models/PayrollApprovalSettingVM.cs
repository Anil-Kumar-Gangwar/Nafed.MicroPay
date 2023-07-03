using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class PayrollApprovalSettingVM
    {
        public List<PayrollApprovalSetting> approvalSetting { set; get; }
        public List<SelectListModel> employees { set; get; }
    }
}