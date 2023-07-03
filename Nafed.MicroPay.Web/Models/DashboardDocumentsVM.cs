using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class DashboardDocumentsVM
    {
        public DashboardDocuments dashboardDocuments { get; set; }
        public List<DashboardDocuments> dbList { get; set; }
    }
}