using PCZInventory.Model;
using QueryResultPrinter.Workers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using TotalInventory;

namespace PCZInventory.DabaseConnection
{
    class DatabaseConnectionManager
    {
        public static DataTable getQueryResult(Form caller, QueryCommandModel queryCommandModel)
        {
            using (SqlConnection sqlConnection = new SqlConnection(DB_Configuration.ConnectionString))
            {
                DataTable dataTable = new DataTable();

                // When there's no user selection like showing data in GridViewSummary, query is excuted only once
                int loopCount = 0;
                if (!(queryCommandModel.userSelection is null))
                {
                    loopCount = queryCommandModel.userSelection.Count - 1;
                }

                for (int i = 0; i <= loopCount; i++)
                {
                    using (SqlCommand sqlCommand = new SqlCommand(queryCommandModel.storedProcedure, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add(new SqlParameter("@stdmonth", SqlDbType.NVarChar, 6));

                        sqlCommand.Parameters["@stdmonth"].Value = queryCommandModel.dateTimePicker.Value.ToString("yyyyMM");
                        
                        // When showing data in GridViewDetail, there must be additional arguments passed
                        if (!(queryCommandModel.userSelection is null))
                        {
                            Worker.AddSecondArguments(caller.Name, sqlCommand, queryCommandModel.userSelection[i]);
                        }
                        try
                        {
                            sqlConnection.Open();
                            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                            {
                                sqlDataAdapter.Fill(dataTable);
                            }
                            sqlConnection.Close();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message);
                        }
                    }
                }
                return dataTable;
            }
        }
    }
}
