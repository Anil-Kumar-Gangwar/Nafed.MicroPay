using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class EmpAchievementAndCertificationDocument
    {
        public int Sno { set; get; }
        public int DocumentID { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentFilePath { get; set; }
        public string DocumentUNCPath { set; get; }
        public Nullable<int> LinkedAchivementID { get; set; }
        public Nullable<int> LinkedCertificateID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    }
}
