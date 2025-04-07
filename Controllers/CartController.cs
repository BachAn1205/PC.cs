using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace Ecommerce.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        public readonly ApplicationDbContext _context;
        public readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentuser = await _userManager.GetUserAsync(HttpContext.User);

            var cart = await _context.Carts
                .Include(x => x.Product)
                .Where(x =>x .UserId == currentuser.Id)
                .ToListAsync();

            double totalCost = 0;

            foreach (var cartItem in cart)
            {
                totalCost += cartItem.Product.Price * cartItem.Qty;
            }
         
            ViewBag.TotalCost = totalCost;

            return View(cart);
        }

        public async Task<IActionResult> UpdateQty(int productId, int qty)
        {
            var product = await _context.Products.Where(x => x.Id == productId).FirstOrDefaultAsync();

            if (product == null)
            {
                return BadRequest();
            }

            var currentuser = await _userManager.GetUserAsync(HttpContext.User);

            var cartItem = await _context.Carts.Where(x => x.UserId == currentuser.Id)
                .Where(x =>x .ProductId == productId)
                .FirstOrDefaultAsync();

            if (cartItem == null) { 
                return BadRequest();
            }

            cartItem.Qty = qty;
            _context.Carts.Update(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int qty = 1)
        {
            var currentuser = await _userManager.GetUserAsync(HttpContext.User);
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return BadRequest();
            }

            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == currentuser.Id && c.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Qty += qty;
            }
            else
            {
                var cartItem = new Cart
                {
                    ProductId = productId,
                    Qty = qty,
                    UserId = currentuser.Id
                };
                _context.Carts.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Content("0");
            }

            var count = await _context.Carts
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Qty);

            return Content(count.ToString());
        }

        public async Task<IActionResult> Remove(int id)
        {
            var cartItem = await _context.Carts.FindAsync(id);
            if (cartItem == null) {
                return BadRequest();
            }
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
