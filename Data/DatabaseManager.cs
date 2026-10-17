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

        public List<Sale> GetSales(
            string? customerName,
            DateTime? startDate,
            DateTime? endDate,
            string sortOrder,
            int page,
            int pageSize
            )
        {
            var sales = new List<Sale>();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                    SELECT SLIP_NUMBER, SALE_DATE, CUSTOMER_NAME, AMOUNT
                    FROM SALES
                    WHERE (:customerName IS NULL
                        OR INSTR(UPPER(CUSTOMER_NAME), UPPER(:customerName)) > 0)
                        AND (:startDate IS NULL OR SALE_DATE >= :startDate)
                        AND (:endDate IS NULL OR SALE_DATE < :endDate)
                    ORDER BY
                        CASE WHEN :sortOrder = 'amount_desc' THEN AMOUNT END DESC,
                        CASE WHEN :sortOrder = 'date_desc' THEN SALE_DATE END DESC,
                        SLIP_NUMBER
                    OFFSET:offset ROWS
                    FETCH NEXT :pageSize ROWS ONLY";

                using (var command = new OracleCommand(sql, connection))
                {
                    command.BindByName = true;

                    command.Parameters.Add(
                        "customerName", OracleDbType.Varchar2
                        ).Value = string.IsNullOrWhiteSpace(customerName)
                        ? DBNull.Value
                        : customerName.Trim();

                    command.Parameters.Add(
                        "startDate", OracleDbType.Date
                        ).Value = startDate.HasValue
                        ? startDate.Value.Date
                        : DBNull.Value;

                    command.Parameters.Add(
                        "endDate", OracleDbType.Date
                        ).Value = endDate.HasValue
                        ? endDate.Value.Date.AddDays(1)
                        : DBNull.Value;

                    command.Parameters.Add(
                        "sortOrder", OracleDbType.Varchar2
                        ).Value = sortOrder;

                    command.Parameters.Add(
                        "offset", OracleDbType.Int32
                        ).Value = (page - 1) * pageSize;

                    command.Parameters.Add(
                        "pageSize", OracleDbType.Int32
                        ).Value = pageSize;

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
            }

            return sales;
        }

        public int GetSalesCount(
            string? customerName,
            DateTime? startDate,
            DateTime? endDate)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                    SELECT COUNT(*)
                    FROM SALES
                    WHERE (:customerName IS NULL
                        OR INSTR(UPPER(CUSTOMER_NAME), UPPER(:customerName)) > 0)
                        AND (:startDate IS NULL OR SALE_DATE >= :startDate)
                        AND (:endDate IS NULL OR SALE_DATE < :endDate)";

                using (var command = new OracleCommand(sql, connection))
                {
                    command.BindByName = true;

                    command.Parameters.Add(
                        "customerName", OracleDbType.Varchar2
                        ).Value = string.IsNullOrWhiteSpace(customerName)
                        ? DBNull.Value
                        : customerName.Trim();

                    command.Parameters.Add(
                        "startDate", OracleDbType.Date
                        ).Value = startDate.HasValue
                        ? startDate.Value.Date
                        : DBNull.Value;

                    command.Parameters.Add(
                        "endDate", OracleDbType.Date
                        ).Value = endDate.HasValue
                        ? endDate.Value.Date.AddDays(1)
                        : DBNull.Value;

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
    }
}
