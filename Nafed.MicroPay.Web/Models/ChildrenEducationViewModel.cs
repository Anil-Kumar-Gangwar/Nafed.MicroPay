using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class ChildrenEducationViewModel
    {
        public IEnumerable<ChildrenEducationHdr> childrenEducationList { get; set; }
        public List<ChildrenEducationDocuments> childrenEducationDocuments { get; set; }
        public UserAccessRight userRights { get; set; }
    }
}