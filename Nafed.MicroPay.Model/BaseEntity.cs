using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class BaseEntity
    {
        public bool DeviceTypeIsMobile { set; get; } 

        public System.DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> DateLastUpdated { get; set; }
        public Nullable<int> LastUpdatedBy { get; set; }
        public string FormActionType { set; get; }

        #region Common Report Partial View Fields==
        public string PageTitle { get; set; }
        public string ControllerName { get; set; }
        #endregion

    }
}
