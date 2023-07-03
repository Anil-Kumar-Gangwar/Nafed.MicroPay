using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class Form12BBDocumentList
    {
        public int Form12BBDocumentID { get; set; }
        public int EmpFormID { get; set; }
        public string NatureOfClaim { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }
        public int? SectionID { get; set; }
        public int? SubSectionID { get; set; }
        public int? SubSectionDescriptionID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string SectionName { get; set; }
        public string SubSectionName { get; set; }
        public string Description { get; set; }
    }
}
