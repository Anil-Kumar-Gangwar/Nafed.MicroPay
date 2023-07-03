using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class MotherTongueModel
    {
        public int ID { get; set; }
        [DisplayName("Name :")]
        [Required(ErrorMessage = "Please Enter Mother Tongue Name")]
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
