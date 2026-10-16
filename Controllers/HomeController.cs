using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SalesHistoryDemo.Models;
using SalesHistoryDemo.Data;

namespace SalesHistoryDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseManager _databaseManager;

        public HomeController(ILogger<HomeController> logger,
            DatabaseManager databaseManager)
        {
            _logger = logger;
            _databaseManager = databaseManager;
        }

        public IActionResult Index(string? startDate, string? endDate, string? customerName, string sortOrder = "date_desc")
        {
            var hasStartDate = DateTime.TryParse(startDate, out var start);
            var hasEndDate = DateTime.TryParse(endDate, out var end);

            if (hasStartDate && hasEndDate && start > end)
            {
                var errorViewModel = new SalesHistoryViewModel
                {
                    StartDate = start,
                    EndDate = end,
                    CustomerName = customerName ?? "",
                    ErrorMessage = "Start date must be ealier than or equal to end date.",
                    Sales = Array.Empty<Sale>()
                };

                return View(errorViewModel);
            }

            var sales = _databaseManager.GetSales(customerName, hasStartDate ? start : null, hasEndDate ? end : null);

            var filteredSales = sales.AsEnumerable();

            var viewModel = new SalesHistoryViewModel
            {
                StartDate = DateTime.TryParse(startDate, out var parsedStart) ? parsedStart : null,
                EndDate = DateTime.TryParse(endDate, out var parsedEnd) ? parsedEnd : null,
                CustomerName = customerName ?? "",

                Sales = sortOrder == "amount_desc" ? filteredSales.OrderByDescending(s => s.Amount).ToArray() :
                filteredSales.OrderByDescending(s => s.SaleDate).ToArray(),
                SortOrder = sortOrder
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
