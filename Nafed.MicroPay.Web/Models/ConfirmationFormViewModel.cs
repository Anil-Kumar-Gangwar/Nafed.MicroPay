using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class ConfirmationFormViewModel
    {
        public List<ConfirmationFormHdr> formList { get; set; }
        public UserAccessRight userRights { get; set; }
        public EmployeeProcessApproval approvalSetting { set; get; } = new EmployeeProcessApproval();
    }
}