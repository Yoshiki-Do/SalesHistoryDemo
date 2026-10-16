using System;
using System.Collections.Generic;

namespace SalesHistoryDemo.Models
{
    public class SalesHistoryViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CustomerName { get; set; } = "";
        public string SortOrder { get; set; } = "date_desc";
        public string ErrorMessage { get; set; } = "";
        public IEnumerable<Sale> Sales { get; set; } = new List<Sale>();
    }
}
