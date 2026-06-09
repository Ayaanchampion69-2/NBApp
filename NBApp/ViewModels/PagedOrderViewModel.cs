using NBApp.Models;

namespace NBApp.ViewModels
{
    public class PagedOrdersViewModel
    {
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}