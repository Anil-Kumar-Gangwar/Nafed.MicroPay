using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Repositories.IRepositories;
namespace Nafed.MicroPay.Data.Repositories
{
    public class SeparationRepository :BaseRepository, ISeparationRepository
    {
        public bool AcceptRejectClearance(int sepId,int statusId)
        {
            var getSup = db.Seprations.Where(x => x.SeprationId == sepId).FirstOrDefault();
            if (getSup != null)
            {
                getSup.StatusId = statusId;
                db.SaveChanges();
                return true;
            }
            else return false;
        }
    }
}
