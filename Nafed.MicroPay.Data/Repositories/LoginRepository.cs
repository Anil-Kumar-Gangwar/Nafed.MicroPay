using System;
using System.Linq;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Data.Repositories
{
    public class LoginRepository : BaseRepository, ILoginRepository
    {
        public bool VerifyUser(string username, out string dbpassword)
        {
            log.Info($"LoginRepository/VerifyUser/{username}");
            dbpassword = "";
            bool IsExists = false;

            try
            {
                using (db = new MicroPayEntities())
                {
                    var user = db.Users.Where(x => x.UserName.Equals(username) && x.IsDeleted == false).FirstOrDefault();
                    if (user != null)
                    {
                        dbpassword = user.Password;
                        IsExists = true;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }

          
            return IsExists;
        }
        public User GetUserData(string username)
        {
            log.Info($"LoginRepository/VerifyUser/{username}");
            try
            {
                using (db = new MicroPayEntities())
                {
                    var user = db.Users.Include("tblMstEmployee").Where(x => x.UserName.Equals(username) && x.IsDeleted == false).FirstOrDefault();
                    return user;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }
        }
       
    }
}
