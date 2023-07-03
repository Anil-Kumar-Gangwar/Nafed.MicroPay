using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.Data;

namespace MicroPay.Web.Models
{
    public class ImportBonusVM
    {
        public List<ImportBounsData> importData { set; get; }
        public Dictionary<string, string> ErrorMsgCollection { get; set; }
        public int NoOfErrors { get; set; }
        public string Message { get; set; }

        public List<string> columnName { get; set; }

        public DataTable inputData { get; set; }


    }
}