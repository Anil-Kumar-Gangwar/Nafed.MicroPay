using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Nafed.MicroPay.Model
{
    public class Category
    {
        public int CategoryID { get; set; }

        [DisplayName("Category Code :")]
        [Required(ErrorMessage = "Please enter category code")]
        public string CategoryCode { get; set; }

        [DisplayName("Category Name :")]
        [Required(ErrorMessage = "Please enter category name")]
        public string CategoryName { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
