using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NBApp.Areas.Identity.Data;
using NBApp.Models;

namespace NBApp.Validators
{
    public class ProductValidator : AbstractValidator<Products>
    {

        private readonly NBAppContext _context;


        public ProductValidator(NBAppContext ctx)
        {
            _context = ctx;
            RuleFor(prod => prod.Name).NotEmpty().MaximumLength(200);
            RuleFor(p => p.Description).MaximumLength(1000);
            RuleFor(p => p.Price).NotEmpty().GreaterThan(0).WithMessage("Price must be greater than 0."); ;
            RuleFor(p => p.ImageUrl).MaximumLength(500);
            RuleFor(p => p.StockQuantity)
                .NotNull().WithMessage("Stock Quantity is required.");
            RuleFor(p => p.CategoryId)
                .NotNull().WithMessage("Please select a category.");

            //Regular Expressions
            // SKU Number format: XXX-0000
            // - Must start with exactly 3 uppercase letters (A-Z)
            // - Followed by a hyphen (-)
            // - Ending with exactly 4 digits (0-9)
            // Example valid values: ABC-1234, XYZ-9999, PRD-0001
            RuleFor(p => p.SKUNumber)
                .Matches(@"^[A-Z]{3}-[0-9]{4}$")
                .WithMessage("SKU Number must be in format XXX-0000 (3 uppercase letters, hyphen, 4 digits)")
                .When(p => !string.IsNullOrEmpty(p.SKUNumber)); // Only validate if provided


            //Comparing Two Properties
            //Cross-Property (Conditional) Validation
            // Sale price must be at least 10% less than regular price
            RuleFor(p => p.SalePrice)
                .Must((product, salePrice) => salePrice < product.Price * 0.9m)
                .WithMessage("Sale Price must be at least 10% less than regular Price.")
                .When(p => p.SalePrice.HasValue && p.Price.HasValue);

            //Database Duplicate Check
            // Check for duplicate product name (case-insensitive)
            RuleFor(p => p.Name)
               .MustAsync(async (product, name, cancellation) =>
               {
                   var exists = await _context.Products
                       .AnyAsync(p => p.Name.ToLower() == name.ToLower()
                                   && p.ProductId != product.ProductId, cancellation);
                   return !exists; // Return true if no duplicate found
               })
               .WithMessage("A product with this name already exists.").When(p => !string.IsNullOrEmpty(p.Name)); // Only check duplicates if name is provided;

            //Date Validation (Past/Future Checks)
            // Release date must be on or before January 1, 2026
            RuleFor(p => p.ReleaseDate)
                .LessThanOrEqualTo(new DateTime(2026, 1, 1))
                .WithMessage("Release Date must be on or before January 1, 2026.")
                .When(p => p.ReleaseDate.HasValue);

        }
    }
}