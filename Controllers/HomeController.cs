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

        public IActionResult Index(
            string? startDate,
            string? endDate,
            string? customerName,
            string sortOrder = "date_desc")
        {
            var hasStartDate = DateTime.TryParse(startDate, out var start);
            var hasEndDate = DateTime.TryParse(endDate, out var end);

            if (sortOrder != "date_desc" && sortOrder != "amount_desc")
            {
                sortOrder = "date_desc";
            }

            if (hasStartDate && hasEndDate && start > end)
            {
                var errorViewModel = new SalesHistoryViewModel
                {
                    StartDate = start,
                    EndDate = end,
                    CustomerName = customerName ?? "",
                    ErrorMessage = "Start date must be earlier than or equal to end date.",
                    Sales = Array.Empty<Sale>()
                };

                return View(errorViewModel);
            }

            try
            {
                var sales = _databaseManager.GetSales(
                customerName,
                hasStartDate ? start : null,
                hasEndDate ? end : null,
                sortOrder);

                var viewModel = new SalesHistoryViewModel
                {
                    StartDate = hasStartDate ? start : null,
                    EndDate = hasEndDate ? end : null,
                    CustomerName = customerName ?? "",
                    Sales = sales,
                    SortOrder = sortOrder
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving sales history.");

                var errorViewModel = new SalesHistoryViewModel
                {
                    StartDate = hasStartDate ? start : null,
                    EndDate = hasEndDate ? end : null,
                    CustomerName = customerName ?? "",
                    SortOrder = sortOrder,
                    ErrorMessage = "An error occurred while retrieving sales history.",
                    Sales = Array.Empty<Sale>()
                };

                return View(errorViewModel);
            }
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
