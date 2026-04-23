using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBApp.Areas.Identity.Data;
using NBApp.Models;

namespace NBApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly NBAppContext _context;
        private readonly IWebHostEnvironment _environment;
        public ProductController(NBAppContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<IActionResult> Index(int? categoryId, string? searchString)
        {
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive);
            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchString) || 
                (p.Description != null && p.Description.Contains(searchString)));
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.CurrentSearchString = searchString;
            return View(await productsQuery.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductsDto productsDto)
        {
            return View();
        }
    }
}
