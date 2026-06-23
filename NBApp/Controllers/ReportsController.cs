using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBApp.Areas.Identity.Data;
using NBApp.Models;
using static NBApp.Models.Order;

namespace NBApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly NBAppContext _context;

        public ReportsController(NBAppContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET /Reports/RevenueByMonth?months=12&status=All
        [HttpGet]
        public async Task<IActionResult> RevenueByMonth(int months = 12, string status = "All")
        {
            var cutoff = DateTime.Now.AddMonths(-months);

            var query = _context.Orders
                .Where(o => o.OrderDate >= cutoff);

            if (Enum.TryParse<OrderStatus>(status, out var parsedStatus))
                query = query.Where(o => o.Status == parsedStatus);

            var data = await query
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(o => o.TotalAmount),
                    Count = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            return Json(data);
        }

        // GET /Reports/OrdersByStatus?months=12
        [HttpGet]
        public async Task<IActionResult> OrdersByStatus(int months = 12)
        {
            var cutoff = DateTime.Now.AddMonths(-months);

            var data = await _context.Orders
                .Where(o => o.OrderDate >= cutoff)
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync();

            return Json(data);
        }

        // GET /Reports/TopProducts?months=12
        [HttpGet]
        public async Task<IActionResult> TopProducts(int months = 12)
        {
            var cutoff = DateTime.Now.AddMonths(-months);

            var data = await _context.Orders
                .Where(o => o.OrderDate >= cutoff)
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => new { oi.ProductId, ProductName = oi.Product.Name, CategoryName = oi.Product.Category.Name })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    Name = g.Key.ProductName,
                    Category = g.Key.CategoryName,
                    UnitsSold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .OrderByDescending(x => x.UnitsSold)
                .Take(10)
                .ToListAsync();

            return Json(data);
        }

        // GET /Reports/StatCards?months=12
        [HttpGet]
        public async Task<IActionResult> StatCards(int months = 12)
        {
            var cutoff = DateTime.Now.AddMonths(-months);

            var orders = await _context.Orders
                .Where(o => o.OrderDate >= cutoff)
                .ToListAsync();

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;
            var pendingCount = orders.Count(o => o.Status == Order.OrderStatus.Pending);
            var avgOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            return Json(new
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                PendingOrders = pendingCount,
                AvgOrderValue = avgOrderValue
            });
        }
    }
}