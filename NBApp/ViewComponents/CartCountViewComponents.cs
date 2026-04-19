using Microsoft.AspNetCore.Mvc;
using NBApp.Extensions;
using NBApp.ViewModels;


namespace NBApp.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.GetObject<CartViewModel>("ShoppingCart");
            var count = cart?.ItemCount ?? 0;
            return View(count);
        }
    }
}