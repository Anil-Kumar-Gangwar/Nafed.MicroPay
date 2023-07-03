using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using Nafed.MicroPay.Data.Repositories.IRepositories;

namespace Nafed.MicroPay.Data.Repositories
{
    public class DepartmentRightRepository : BaseRepository, IDepartmentRightRepository
    {

        public int InsertUpdateDepartmentRights(int departmentID, DataTable departmentRights)
        {
            int processStatus = 0;

            using (SqlConnection dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                try
                {
                    SqlCommand dbSqlCommand;
                    dbSqlCommand = new SqlCommand();
                    dbSqlCommand.Connection = dbSqlconnection;
                    dbSqlCommand.CommandType = CommandType.StoredProcedure;
                    dbSqlCommand.CommandText = "InsertUpdateDepartmentRights";
                    dbSqlCommand.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = departmentID;
                    dbSqlCommand.Parameters.Add("@DepartmentRights", SqlDbType.Structured).Value = departmentRights;
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

        public List<Data.Models.GetEmpCountBasedOnDepartment_Result> GetEmpCountBasedOnDepartment()
        {
            return db.GetEmpCountBasedOnDepartment().ToList();
        }
    }
}
