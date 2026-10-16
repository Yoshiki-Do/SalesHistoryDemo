namespace SalesHistoryDemo.Models
{
    public class Sale
    {
        public string SlipNumber { get; set; } = "";
        public DateTime SaleDate { get; set; }
        public string CustomerName { get; set; } = "";
        public decimal Amount { get; set; }

    }
}
