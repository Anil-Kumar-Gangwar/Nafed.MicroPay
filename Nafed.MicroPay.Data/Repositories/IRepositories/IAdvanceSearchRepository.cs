using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
     public  interface IAdvanceSearchRepository
    {
        DataTable GetAdvanceSearchResult(int filterFieldID, int selectedEmployeeType, DataTable selectedFieldIDs, DataTable payScaleDT, DataTable columnsName = null, DataTable columnDisplayName = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
