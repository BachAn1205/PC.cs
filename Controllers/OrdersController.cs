using Ecommerce.Data;
using EcommercePC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    public class OrdersController : Controller
    {
        public readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

   
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(x => x.OrderProducts)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include (x => x.OrderProducts)
                .ThenInclude(x => x.Product)
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Id ==id);

            return View(order);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult MarkAsPaymentSuccessful(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();

            order.Status = OrderStatus.PaymentSuccessful;
            _context.SaveChanges();

            // Tự động chuyển sang trạng thái Shipped sau khi thanh toán
            order.Status = OrderStatus.Shipped;
            _context.SaveChanges();

            return RedirectToAction("Details", new { id });
        }
        [HttpPost]
        public ActionResult MarkAsShipped(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();

            order.Status = OrderStatus.Shipped;
            _context.SaveChanges();

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        public ActionResult MarkAsDelivered(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();

            order.Status = OrderStatus.Delivered;
            _context.SaveChanges();

            return RedirectToAction("Details", new { id });
        }

    }
}
