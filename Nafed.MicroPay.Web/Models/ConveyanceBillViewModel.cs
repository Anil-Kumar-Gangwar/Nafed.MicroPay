using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;


namespace MicroPay.Web.Models
{
    public class ConveyanceBillViewModel
    {
        public IEnumerable<ConveyanceBillFormHdr> ConveyanceformList { get; set; }
        public UserAccessRight userRights { get; set; }
        public EmployeeProcessApproval approvalSetting { set; get; } = new EmployeeProcessApproval();
    }
}