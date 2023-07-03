using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Nafed.MicroPay.Model
{
   public class ValidateLogin
    {
        [DisplayName("User Name")]
        public string userName { set; get; }
        public string hduserName { get; set; }
        [DisplayName("Password")]
        public string password { set; get; }
        public string hdpassword { get; set; }
        public string hdCp { get; set; }
    }
}
