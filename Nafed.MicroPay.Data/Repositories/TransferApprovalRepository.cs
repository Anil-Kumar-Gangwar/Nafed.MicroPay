using Nafed.MicroPay.Data.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Data.Repositories
{
    public class TransferApprovalRepository : BaseRepository, ITransferApprovalRepository
    {
      public bool TransferApproval(int fromEmployeeID, int toEmployeeID, int? processId)
        {
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[TransferApprovalRights]";

                SqlParameter paramfromEmployeeID = dbSqlCommand.Parameters.AddWithValue("@fromEmployeeID", fromEmployeeID);
                paramfromEmployeeID.SqlDbType = SqlDbType.Int;

                SqlParameter paramtoEmployeeID = dbSqlCommand.Parameters.AddWithValue("@toEmployeeID", toEmployeeID);
                paramtoEmployeeID.SqlDbType = SqlDbType.Int;

                SqlParameter paramtoprocessId = dbSqlCommand.Parameters.AddWithValue("@processid", processId);
                paramtoprocessId.SqlDbType = SqlDbType.Int;

                SqlParameter output = dbSqlCommand.Parameters.AddWithValue("@output", SqlDbType.Bit);
               
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                output.Direction = ParameterDirection.Output;
                dbSqlCommand.ExecuteNonQuery();
                dbSqlconnection.Close();
                return Convert.ToBoolean(output.Value);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
