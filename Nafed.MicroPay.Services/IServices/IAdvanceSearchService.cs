using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Common;
using System.Data;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IAdvanceSearchService
    {
        IEnumerable<SelectListModel> GetFilterFields(AdvanceSearchFilterFields filter, int selectedEmployeeType);
        AdvanceSearchResult GetAdvanceSearchResult(AdvanceSearchFilterFields filter, int selectedEmpType, int[] selectedIDs, SelectMoreFields moreFields = null);
        bool ExportAdvanceSearchResult(DataSet dsSource, string sFullPath, string fileName);
    }
}
