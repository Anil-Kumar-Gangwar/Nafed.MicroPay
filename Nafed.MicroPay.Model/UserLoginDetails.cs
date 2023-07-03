using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class UserLoginDetails
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string UserName { get; set; }
        public Nullable<System.DateTime> LoginTime { get; set; }
        public Nullable<System.DateTime> LogOutTime { get; set; }
        public string Remarks { get; set; }
    }
}
