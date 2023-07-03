using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MicroPay.Web.Models
{
    public class ImportPfBalanceVM
    {
        public List<ImportPfBalanceData> importData { set; get; }
        public Dictionary<string, string> ErrorMsgCollection { get; set; }
        public int NoOfErrors { get; set; }
        public string Message { get; set; }

        public List<string> columnName { get; set; }

        public DataTable inputData { get; set; }
    }
}