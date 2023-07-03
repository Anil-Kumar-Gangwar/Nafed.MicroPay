using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{

    public class ActivityLog
    {
        [Display (Name = "Process Name") ]
        public string ProcessName
        {
            get;
            set;
        }
        [Display(Name = "User Name")]
        public string User
        {
            get;
            set;
        }
        [Display(Name = "Action Performed")]
        public string ActionPerformed
        {
            get;
            set;
        }
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate
        {
            get;
            set;
        }




    }
}
