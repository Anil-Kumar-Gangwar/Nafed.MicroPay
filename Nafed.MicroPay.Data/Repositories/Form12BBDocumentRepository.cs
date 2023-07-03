using System;
using System.Collections.Generic;
using Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;

namespace Nafed.MicroPay.Data.Repositories
{
   public class Form12BBDocumentRepository : BaseRepository, IForm12BBDocumentRepository
    {
        public IEnumerable<GetForm12BBDocumentList_Result> GetForm12BBDocumentList(int EmpFormID)
        {
            return db.GetForm12BBDocumentList(EmpFormID);
        }
    }
}
