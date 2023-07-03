using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System;
using Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Data.Repositories
{
    public class UserRightsRepository : BaseRepository, IUserRightsRepository
    {
        public int InsertUpdateDepartmentUserRights(int userID, int departmentID, DataTable departmentUserRights)
        {
            log.Info($"UserRightsRepository/InsertUpdateDepartmentUserRights/{userID}/{departmentID}");
            int processStatus = 0;
           
            using (SqlConnection dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                try
                {
                    SqlCommand dbSqlCommand;
                    dbSqlCommand = new SqlCommand();
                    dbSqlCommand.Connection = dbSqlconnection;
                    dbSqlCommand.CommandType = CommandType.StoredProcedure;
                    dbSqlCommand.CommandText = "InsertUpdateDepartmentUserRights";
                    dbSqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                    dbSqlCommand.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = departmentID;
                    dbSqlCommand.Parameters.Add("@UserRights", SqlDbType.Structured).Value = departmentUserRights;
                    //dbSqlCommand.Parameters.Add("@AllUserSelected", SqlDbType.Bit).Value = isAllUserSelected;                    
                    dbSqlCommand.Parameters.Add("@ProcessStatus", SqlDbType.Int);
                    dbSqlCommand.Parameters["@ProcessStatus"].Direction = ParameterDirection.Output;

                    if (dbSqlconnection.State == ConnectionState.Closed)
                        dbSqlconnection.Open();
                    dbSqlCommand.ExecuteNonQuery();
                    processStatus = (int)dbSqlCommand.Parameters["@processStatus"].Value;
                    dbSqlconnection.Close();
                }
                catch (Exception)
                {
                    processStatus = -1;
                    throw;
                }
                finally
                {
                    if (dbSqlconnection.State == ConnectionState.Open)
                    {
                        dbSqlconnection.Close();
                    }
                }
            }

            return processStatus;
        }
        
        public List<Menu> GetUserAccessMenuList(int departmentID)
        {
            log.Info($"UserRightsRepository/GetUserAccessMenuList/{departmentID}");
            using (db = new MicroPayEntities())
            {
                var lst1 = db.Menus.Where(x => x.IsActive == true && x.DepartmentRights.Any(y => y.DepartmentID == departmentID));
                var lst2 = db.Menus.Where(x => x.IsActive == true && x.DepartmentRights.Any(y => y.DepartmentID == departmentID));

                var flist = lst1.Where(x => lst2.Any(y => y.ParentID == x.MenuID)).ToList();
                return flist;
            }
        }
    }
}
