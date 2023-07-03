using Nafed.MicroPay.Model;
using System.Collections.Generic;

namespace MicroPay.Web.Models
{
    public class SalaryConfigurationViewModel
    {
        public int BranchID { set; get; }
        public List<SelectListModel> branchList { set; get; }
        public UserAccessRight userRights { get; set; }
    }
}