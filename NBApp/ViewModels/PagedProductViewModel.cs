// ViewModels/PagedProductsViewModel.cs
using NBApp.Models;

namespace NBApp.ViewModels
{
    public class PagedProductsViewModel
    {
        public IEnumerable<Products> Products { get; set; } = new List<Products>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}