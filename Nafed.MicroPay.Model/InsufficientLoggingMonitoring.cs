using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
  public class InsufficientLoggingMonitoring
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string SessionId { get; set; }
        public string IP_address { get; set; }
        public DateTime DateTime { get; set; }
        public string Referrer { get; set; }
        public string ProcessId { get; set; }
        public string URL { get; set; }
        public string UserAgent { get; set; }
        public string Country { get; set; }
    }
}
