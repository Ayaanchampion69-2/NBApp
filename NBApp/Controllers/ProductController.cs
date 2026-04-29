using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBApp.Areas.Identity.Data;
using NBApp.Models;
using System;
using System.IO;

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
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductsDto productsDto)
        {
            if (productsDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Product image is required.");
            }
            if (!ModelState.IsValid)
            {
                return View(productsDto);
            }
            //save image to wwwroot/images
            string NewFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            NewFileName += Path.GetExtension(productsDto.ImageFile!.FileName);

            string imageFullPath = _environment.WebRootPath + "/Products/" + NewFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productsDto.ImageFile.CopyTo(stream);
            }
            //save product to database
            Products product = new Products
            {
                Name = productsDto.Name,
                Description = productsDto.Description,
                Price = productsDto.Price,
                SalePrice = productsDto.SalePrice,
                ImageUrl = "/Products/" + NewFileName,
                ReleaseDate = productsDto.ReleaseDate,
                StockQuantity = productsDto.StockQuantity,
                IsActive = productsDto.IsActive,
                SKUNumber = productsDto.SKUNumber,
                CategoryId = productsDto.CategoryId
            };
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index", "Products");
        }

        public async Task<IActionResult> Edit(int ProductId)
        {
            var product = _context.Products.Find(ProductId);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            //create productdto from product
            var ProductDto = new ProductsDto()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SalePrice = product.SalePrice,

                ReleaseDate = product.ReleaseDate,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                SKUNumber = product.SKUNumber,
                CategoryId = product.CategoryId
            };
            ViewData["ProductId"] = ProductId;
            ViewData["imageUrl"] = product.ImageUrl;
            ViewData["ReleaseDate"] = product.ReleaseDate?.ToString("yyyy-MM-dd");
            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(ProductDto);

        }

        [HttpPost]
        public IActionResult Edit(int ProductId, ProductsDto productsDto)
        {
            var product = _context.Products.Find(ProductId);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = ProductId;
                ViewData["imageUrl"] = product.ImageUrl;
                ViewData["ReleaseDate"] = product.ReleaseDate?.ToString("yyyy-MM-dd");
                return View(productsDto);

            }

            //update image if new image is uploaded
            string NewFileName = product.ImageUrl;
            if(productsDto.ImageFile != null)
                {
                NewFileName = DateTime.Now.ToString("yyyyMMMMddHHmmss");
                NewFileName += Path.GetExtension(productsDto.ImageFile.FileName);

                string imageFullPath = _environment.WebRootPath + "/Products/" + NewFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productsDto.ImageFile.CopyTo(stream);
                }
                //delete old image
                string oldImageFullPath = _environment.WebRootPath + "/Products/" + product.ImageUrl;
                System.IO.File.Delete(oldImageFullPath);

            }
            //update product properties
            product.Name = productsDto.Name;
            product.Description = productsDto.Description;
            product.Price = productsDto.Price;
            product.SalePrice = productsDto.SalePrice;
            product.ImageUrl = NewFileName;
            product.ReleaseDate = productsDto.ReleaseDate;
            product.StockQuantity = productsDto.StockQuantity;
            product.IsActive = productsDto.IsActive;
            product.SKUNumber = productsDto.SKUNumber;
            product.CategoryId = productsDto.CategoryId;

            _context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }
    

    public IActionResult Delete(int ProductId)
        {
            var product = _context.Products.Find(ProductId);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            string imageFullPath = _environment.WebRootPath+"/Products/"+product.ImageUrl;
            System.IO.File.Delete(imageFullPath);

            _context.Products.Remove(product);
            _context.SaveChanges(true);

            return RedirectToAction("Index", "Products");

        }
            
            
        }
    }

