using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using System.Data;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IChildrenEducationRepository
    {
        List<GetChildrenEducationDetails_Result> GetChildrenEducationDetails(int empID, int childrenEduHdrId);
        int UpdateChildrenEducationData(int ChildrenEduHdrID, decimal? Amount, DateTime? ReceiptDate, DateTime? ReceiptDate2, string ReceiptNo, string ReceiptNo2, int StatusId);
    }
}
