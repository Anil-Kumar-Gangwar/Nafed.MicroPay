using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.ImportExport.Interfaces
{
    public interface IIncrementExport : IBase
    {
        bool ExportToExcelIncrement(DataSet dsSource, string sFullPath, string fileName);
    }
}
