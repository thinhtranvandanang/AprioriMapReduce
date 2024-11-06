using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccordNET9
{
    public class clsSQLServer
    {
        public static List<Sale> ReadFromSQLServerToList(string connectionString, string tableName)
        {
            List<Sale> sales = new List<Sale>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT TransactionID, ProductID, TransactionDate FROM {tableName}";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sales.Add(new Sale
                            {
                                TransactionID = reader["TransactionID"].ToString(),
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                TransactionDate = Convert.ToDateTime(reader["TransactionDate"])
                            });
                        }
                    }
                }
            }
            return sales;
        }
        public static int[][] ConvertToAprioriFormat(List<Sale> transactions)
        {
            var groupedTransactions = transactions.GroupBy(t => t.TransactionID)
                .ToDictionary(g => g.Key, g => g.Select(t => t.ProductID).ToArray());

            return groupedTransactions.Values.ToArray();
        }




    }
}