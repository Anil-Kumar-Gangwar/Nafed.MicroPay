using Nafed.MicroPay.Data.Repositories.IRepositories;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Nafed.MicroPay.Data.Repositories
{
    public class AdvanceSearchRepository : BaseRepository, IAdvanceSearchRepository
    {
        public DataTable GetAdvanceSearchResult(int filterFieldID, int selectedEmployeeType, DataTable selectedFieldIDs, DataTable payScaleDT, DataTable columnsName=null, DataTable columnDisplayName=null,DateTime ? fromDate=null ,DateTime ? toDate=null)
        {
            log.Info($"AdvanceSearchRepository/GetAdvanceSearchResult/{filterFieldID}");

            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[GetAdvanceSearchResult]";
                dbSqlCommand.Parameters.Add("@defaultFieldID", SqlDbType.Int).Value = filterFieldID;
                dbSqlCommand.Parameters.Add("@selectedEmpType", SqlDbType.Int).Value = selectedEmployeeType;


                dbSqlCommand.Parameters.Add("@dateFrom", SqlDbType.Date).Value = fromDate;
                dbSqlCommand.Parameters.Add("@dateTo", SqlDbType.Date).Value = toDate;

                SqlParameter selectedValues = dbSqlCommand.Parameters.AddWithValue("@selectedValues", selectedFieldIDs);
                selectedValues.SqlDbType = SqlDbType.Structured;
                selectedValues.TypeName = "[udtGenericStringList]";


                SqlParameter payScales = dbSqlCommand.Parameters.AddWithValue("@designationPayScale", payScaleDT);
                selectedValues.SqlDbType = SqlDbType.Structured;
                selectedValues.TypeName = "[udtGenericStringList]";


                SqlParameter columnNames = dbSqlCommand.Parameters.AddWithValue("@seletedColumnList", columnsName);
                columnNames.SqlDbType = SqlDbType.Structured;
                columnNames.TypeName = "[udtSelectListModel]";


                SqlParameter columnDisplayNames = dbSqlCommand.Parameters.AddWithValue("@seletedColumnDisplayName", columnDisplayName);
                columnDisplayNames.SqlDbType = SqlDbType.Structured;
                columnDisplayNames.TypeName = "[udtSelectListModel]";

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlconnection.Close();

                return dbSqlDataTable;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
