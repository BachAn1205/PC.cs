using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.ViewComponents
{
    [ViewComponent(Name = "Navigation")]
    public class Navigation : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public Navigation(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return View("Index", new NavigationViewModel
                {
                    Cart = new CartViewModel { Items = new List<CartItemViewModel>() }
                });
            }

            var cart = await _context.Carts
                .Include(c => c.Product)
                .Where(x => x.UserId == currentUser.Id)
                .ToListAsync();

            var cartItems = cart.Select(x => new CartItemViewModel
            {
                ProductName = x.Product.Name,
                ProductImage = x.Product.Image, // Đảm bảo cột ImageUrl có tồn tại
                Quantity = x.Qty
            }).ToList();

            var model = new NavigationViewModel
            {
                Cart = new CartViewModel
                {
                    Items = cartItems
                }
            };

            return View("Index", model);
        }
    }
}
