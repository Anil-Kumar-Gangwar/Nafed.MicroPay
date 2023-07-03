using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class Bank
    {
        [DisplayName("Code :")]
        public string BankCode { get; set; }
        [DisplayName("Name :")]
        public string BankName { get; set; }

        [DisplayName("Branch :")]
        public string Branch { get; set; }
        [DisplayName("Address 1 :")]
        public string Address1 { get; set; }
        [DisplayName("Address 2 :")]
        public string Address2 { get; set; }
        [DisplayName("Address 3 :")]
        public string Address3 { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
