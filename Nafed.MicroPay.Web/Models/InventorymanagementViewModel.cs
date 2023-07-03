using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class InventorymanagementViewModel
    {
        public List<InventoryManagement> listInventoryManagement { get; set; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();

        public int AssetTypeID { get; set; }
        public int ManufacturerID { get; set; }
        public int IMID { get; set; }
        public int EmployeeID { get; set; }
        public string Asset{ get; set; }
        public string AssetType { get; set; }
    }
}