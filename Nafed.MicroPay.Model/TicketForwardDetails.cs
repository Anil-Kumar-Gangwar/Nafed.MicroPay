using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class TicketForwardDetails
    {
        public string SenderDepartment { get; set; }
        public string Sender { get; set; }
        public Nullable<System.DateTime> Senddate { get; set; }
        public string RevDepartment { get; set; }
        public string RevName { get; set; }
        public string Remarks { get; set; }
        public int ID { get; set; }
        public string TAT { get; set; }
    }
}
