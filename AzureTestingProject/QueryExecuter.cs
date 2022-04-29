using System;
using System.Data;
using System.Data.SqlClient;

namespace AzureTestingProject
{
    public static class QueryExecuter
    {
        static SqlConnection cnn;
        public static string updateQuery(string query)
        {

            cnn = new SqlConnection(@"data source=192.168.0.82;initial catalog=MY_April2022;password=abc123;persist security info=True;user id=crmnext;packet size=4096;enlist=false");
          
            cnn.Open();
            try
            {
                SqlCommand UpdateCommand = new SqlCommand(query, cnn);
                UpdateCommand.ExecuteNonQuery();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;

            }
            finally
            {
                cnn.Close();

            }


        }

        public static DataTableReader ReadQuery(string query)
        {

            cnn = new SqlConnection(@"data source=192.168.0.82;initial catalog=MY_April2022;password=abc123;persist security info=True;user id=crmnext;packet size=4096;enlist=false");
            cnn.Open();
            try
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, cnn);
                DataTable dataTable = new DataTable();

                sqlDataAdapter.Fill(dataTable);

                DataTableReader dataTableReader = dataTable.CreateDataReader();
                return dataTableReader;

            }
            finally
            {
                cnn.Close();

            }


        }

    }
}
