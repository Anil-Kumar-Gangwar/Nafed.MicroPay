using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Data.Models;
using System.Linq.Expressions;

namespace Nafed.MicroPay.Data.Repositories
{
    abstract   public class BaseRepository : IDisposable, IBaseRepository
    {
        protected internal MicroPayEntities db = null;
        public BaseRepository()
        {
            db = new MicroPayEntities();
        }
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public void Dispose()
        {
        }
    }
    /// <summary>
    /// Anonymous Select list model = 
    /// </summary>
    public class AnonymousSelectList
    {
        public int id { set; get; }

        public string value { set; get; }
    }
}
