using Nafed.MicroPay.Data.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using System.Data;
using System.Data.SqlClient;

namespace Nafed.MicroPay.Data.Repositories
{
    public class ChildrenEducationRepository : BaseRepository, IChildrenEducationRepository
    {
        public List<GetChildrenEducationDetails_Result> GetChildrenEducationDetails(int empID, int childrenEduHdrId)
        {
            return db.GetChildrenEducationDetails(empID, childrenEduHdrId).ToList();
        }

        public int UpdateChildrenEducationData(int ChildrenEduHdrID, decimal? Amount, DateTime? ReceiptDate, DateTime? ReceiptDate2, string ReceiptNo, string ReceiptNo2, int StatusId)
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
                    dbSqlCommand.CommandText = "UpdateChildrenEducationHdr";
                    dbSqlCommand.Parameters.AddWithValue("@ChildrenEduHdrID", ChildrenEduHdrID);
                    dbSqlCommand.Parameters.AddWithValue("@Amount", Amount);
                    dbSqlCommand.Parameters.AddWithValue("@ReceiptDate", ReceiptDate);
                    dbSqlCommand.Parameters.AddWithValue("@ReceiptDate2", ReceiptDate2);
                    dbSqlCommand.Parameters.AddWithValue("@ReceiptNo", ReceiptNo);
                    dbSqlCommand.Parameters.AddWithValue("@ReceiptNo2", ReceiptNo2);
                    dbSqlCommand.Parameters.AddWithValue("@StatusId", StatusId);                    
                    if (dbSqlconnection.State == ConnectionState.Closed)
                        dbSqlconnection.Open();
                    dbSqlCommand.ExecuteNonQuery();
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
    }
}
