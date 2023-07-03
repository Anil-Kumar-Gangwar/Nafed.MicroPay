using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class UserAccessControlRights
    {
        public int MenuID { get; set; }
        
        public string MenuName { get; set; }
      
        public Nullable<int> ParentID { get; set; }

        public bool ShowMenu { get; set; }
        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool View { get; set; }
        public bool Delete { get; set; }

    }
}
