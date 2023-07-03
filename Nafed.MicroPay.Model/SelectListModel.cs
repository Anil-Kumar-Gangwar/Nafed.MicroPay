using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class SelectListModel 
    {
        public int id { set; get; }
        public string value { set; get; }

        public static explicit operator SelectListModel(List<object> v)
        {
            throw new NotImplementedException();
        }
    }
}
