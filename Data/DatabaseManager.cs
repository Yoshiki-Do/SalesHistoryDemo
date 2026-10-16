using Oracle.ManagedDataAccess.Client;
using SalesHistoryDemo.Models;

namespace SalesHistoryDemo.Data
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Sale> GetSales()
        {
            var sales = new List<Sale>();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                    SELECT SLIP_NUMBER, SALE_DATE, CUSTOMER_NAME,AMOUNT
                    FROM SALES
                    ORDER BY SALE_DATE DESC";

                using (var command = new OracleCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var sale = new Sale
                        {
                            SlipNumber = reader.GetString(0),
                            SaleDate = reader.GetDateTime(1),
                            CustomerName = reader.GetString(2),
                            Amount = reader.GetDecimal(3)
                        };

                        sales.Add(sale);
                    }
                }
            }

            return sales;
        }
    }
}
