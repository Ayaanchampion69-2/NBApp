using ContosoUniversity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NBApp.Areas.Identity.Data;
using NBApp.Models;
using NBApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NBApp.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly NBAppContext _context;

        public OrdersController(NBAppContext context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            IQueryable<Order> query;

            if (User.IsInRole("Admin"))
            {
                query = _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                query = _context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new PagedOrdersViewModel
            {
                Orders = orders,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(viewModel);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ShippingAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null) return NotFound();

            var viewModel = new OrderViewModel
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                UserId = order.User?.Id ?? "",
                DisplayName = order.User?.DisplayName ?? "",
                BuildingNumber = order.ShippingAddress?.BuildingNumber ?? "",
                Street = order.ShippingAddress?.Street ?? "",
                City = order.ShippingAddress?.City ?? "",
                PostalCode = order.ShippingAddress?.PostalCode ?? "",
                OrderItems = order.OrderItems?.Select(oi => new OrderItemViewModel
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList() ?? new()
            };

            return View(viewModel);
        }
        [Authorize(Roles = "Admin")]
        // GET: Order/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("OrderId,OrderDate,TotalAmount")] Order order)
        {
            order.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _context.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            var viewModel = new OrderViewModel
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                UserId = order.User?.Id ?? "",
                DisplayName = order.User?.DisplayName ?? "",
                BuildingNumber = order.ShippingAddress?.BuildingNumber ?? "",
                Street = order.ShippingAddress?.Street ?? "",
                City = order.ShippingAddress?.City ?? "",
                PostalCode = order.ShippingAddress?.PostalCode ?? "",
            };

            return View(viewModel);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderViewModel viewModel)
        {
            if (id != viewModel.OrderId) return NotFound();

            // Clear validation errors for fields not present in the form
            ModelState.Remove(nameof(OrderViewModel.UserId));
            ModelState.Remove(nameof(OrderViewModel.DisplayName));
            ModelState.Remove(nameof(OrderViewModel.OrderItems));

            if (!ModelState.IsValid) return View(viewModel);

            var order = await _context.Orders
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            order.Status = viewModel.Status;

            if (order.ShippingAddress != null)
            {
                order.ShippingAddress.BuildingNumber = viewModel.BuildingNumber;
                order.ShippingAddress.Street = viewModel.Street;
                order.ShippingAddress.City = viewModel.City;
                order.ShippingAddress.PostalCode = viewModel.PostalCode;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.OrderId)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
                _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}